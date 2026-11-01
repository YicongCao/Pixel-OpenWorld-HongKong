#!/usr/bin/env python3
"""Read-only extraction from game-research; generated files only in this project.
Run with ../game-research/.venv-hk-data/bin/python scripts/extract_hk.py
or a Python environment with pyogrio, shapely, numpy and pyproj.
"""
import argparse
import csv
import hashlib
import json
import shutil
from pathlib import Path

import numpy as np
import pyogrio
from pyproj import Transformer
from shapely import from_wkb, to_geojson
from shapely.geometry import box

ROOT = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser()
parser.add_argument('--source', type=Path, default=ROOT.parent / 'game-research/data/hong-kong')
args = parser.parse_args()
SRC = args.source.resolve()
OUT = ROOT / 'data/hong-kong'
OUT.mkdir(parents=True, exist_ok=True)
BBOX = (114.150, 22.289, 114.186, 22.328)
REGION = box(*BBOX)
to_hk = Transformer.from_crs('OGC:CRS84', 'EPSG:2326', always_xy=True)
to_wgs = Transformer.from_crs('EPSG:2326', 'OGC:CRS84', always_xy=True)
HK_BBOX = to_hk.transform_bounds(*BBOX, densify_pts=21)
sources, outputs = {}, {}

def sha(path):
    h = hashlib.sha256()
    with path.open('rb') as f:
        for block in iter(lambda: f.read(1024 * 1024), b''):
            h.update(block)
    return h.hexdigest()

def source(path):
    key = str(path.relative_to(SRC))
    if key not in sources:
        if path.is_dir():
            files = [{'path': str(p.relative_to(path)), 'sha256': sha(p)}
                     for p in sorted(path.rglob('*')) if p.is_file()]
            sources[key] = {'files': files, 'sha256': hashlib.sha256(
                json.dumps(files, sort_keys=True).encode()).hexdigest(),
                'hash_method': 'SHA256 of sorted file manifest JSON (sort_keys=True)'}
        else:
            sources[key] = {'bytes': path.stat().st_size, 'sha256': sha(path)}
    return key

def clean(v):
    if v is None: return None
    if isinstance(v, np.datetime64): return None if np.isnat(v) else str(v)
    if isinstance(v, np.generic): v = v.item()
    if isinstance(v, float) and not np.isfinite(v): return None
    return v

def write(name, data, count=None):
    p = OUT / name
    p.parent.mkdir(parents=True, exist_ok=True)
    p.write_text(json.dumps(data, ensure_ascii=False, separators=(',', ':'), default=str), encoding='utf-8')
    outputs[name] = {'bytes': p.stat().st_size, 'sha256': sha(p), 'count': count}

def read_layer(path, layer, bbox=None):
    meta, fids, geoms, cols = pyogrio.raw.read(path, layer=layer, bbox=bbox, return_fids=True)
    rows = [{str(k): clean(c[i]) for k, c in zip(meta['fields'], cols)} for i in range(len(fids))]
    return meta, fids, geoms, rows

gpkg = SRC / 'derived/osm/hong-kong.gpkg'
source(gpkg)
all_features = []
for short, layer in {
    'roads': 'gis_osm_roads_free', 'buildings': 'gis_osm_buildings_a_free',
    'pois': 'gis_osm_pois_free', 'poi_areas': 'gis_osm_pois_a_free',
    'transport': 'gis_osm_transport_free', 'transport_areas': 'gis_osm_transport_a_free',
    'railways': 'gis_osm_railways_free', 'water': 'gis_osm_water_a_free',
    'landuse': 'gis_osm_landuse_a_free',
}.items():
    meta, fids, geoms, rows = read_layer(gpkg, layer, BBOX)
    features = []
    for fid, wkb, props in zip(fids, geoms, rows):
        geom = from_wkb(wkb)
        if not geom.intersects(REGION): continue
        # Keep source geometry intact. Whole features may extend outside the selection.
        props.update(source_id=f'osm-snapshot:{layer}:{props["osm_id"]}', source_layer=layer,
                     snapshot='2026-10-22', selection_crosses_boundary=not REGION.covers(geom))
        features.append({'type': 'Feature', 'id': props['source_id'],
                         'properties': props, 'geometry': json.loads(to_geojson(geom))})
    write(f'osm/{short}.geojson', {'type': 'FeatureCollection', 'features': features}, len(features))
    all_features.extend(features)
    print(short, len(features), flush=True)

terms = ['戲曲', 'xiqu', '海港城', 'harbour city', '中港城', 'china hong kong',
         '西九龍', 'west kowloon', 'k11', 'apple', '7-eleven', "mcdonald", '廣東道',
         'canton road', '彌敦道', 'nathan road', '梳士巴利', 'salisbury']
candidates = [f for f in all_features if any(t in (f['properties'].get('name') or '').lower() for t in terms)]
write('landmark-candidates.geojson', {'type': 'FeatureCollection', 'features': candidates}, len(candidates))

gdb = SRC / 'derived/pedestrian/3DPN_P2.gdb'
source(gdb)
meta, fids, geoms, rows = read_layer(gdb, 'PedestrianRoute', HK_BBOX)
paths, refs = [], {k: set() for k in ['FloorID', 'BuildingID', 'SiteID', 'TerminalID', 'AccessTimeID', 'PedestrianRouteID']}
for wkb, props in zip(geoms, rows):
    geom = from_wkb(wkb)
    # The transformed bounding rectangle is conservative; retain source endpoints and Z.
    props['source_id'] = f'3dpn:{props["PedestrianRouteID"]}'
    paths.append({'properties': props, 'geometry': json.loads(to_geojson(geom))})
    for k in refs:
        if k == 'BuildingID':
            refs[k].update([props.get('BuildingID_1'), props.get('BuildingID_2')])
        else: refs[k].add(props.get(k))
write('pedestrian/paths-hk1980.json', {'horizontal_crs': 'EPSG:2326', 'vertical_crs': 'EPSG:5738',
      'units': 'metres', 'axis_order': 'easting,northing,HKPD height',
      'selection_bbox': HK_BBOX, 'selection': 'intersects bounding rectangle; not clipped; not a validated routing graph',
      'source_crs_wkt': meta['crs'], 'paths': paths}, len(paths))
relations = {}
for layer, key in [('PedRouteRelFloorPoly', 'FloorID'), ('PedRouteRelBldgPoly', 'BuildingID'),
                   ('PedRouteRelSitePoly', 'SiteID'), ('PedRouteRelTerminalPoly', 'TerminalID'),
                   ('PedRouteRelDistancePost', 'PedestrianRouteID'), ('AccessTime', 'AccessTimeID'),
                   ('AccessTimeDetails', 'AccessTimeID')]:
    _, _, _, all_rows = read_layer(gdb, layer)
    relations[layer] = [r for r in all_rows if r.get(key) in refs[key]]
write('pedestrian/relations.json', relations, sum(map(len, relations.values())))
print('pedestrian paths', len(paths), flush=True)

transport = {}
def csv_rows(path):
    source(path)
    with path.open(encoding='utf-8-sig', newline='') as f: return list(csv.DictReader(f))

for mode in ['BUS', 'GMB', 'FERRY']:
    base = SRC / 'derived/transport'
    stops = csv_rows(base / f'STOP_{mode}__STOP.csv')
    selected = []
    for r in stops:
        try: lon, lat = to_wgs.transform(float(r['X']), float(r['Y']))
        except (ValueError, TypeError): continue
        if BBOX[0] <= lon <= BBOX[2] and BBOX[1] <= lat <= BBOX[3]:
            selected.append({**r, 'source_id': f'td:{mode}:stop:{r["STOP_ID"]}', 'longitude': lon, 'latitude': lat})
    ids = {r['STOP_ID'] for r in selected}
    rstop = [r for r in csv_rows(base / f'RSTOP_{mode}__RSTOP.csv') if r['STOP_ID'] in ids]
    route_ids = {r['ROUTE_ID'] for r in rstop}
    routes = [r for r in csv_rows(base / f'ROUTE_{mode}__ROUTE.csv') if r['ROUTE_ID'] in route_ids]
    transport[mode] = {'stops': len(selected), 'route_stop_rows': len(rstop), 'route_records': len(routes)}
    write(f'transport/{mode.lower()}.json', {'mode': mode, 'source_version': '2026-10-14',
          'stop_xy_crs': 'EPSG:2326', 'selection': 'stops within bbox; route stop rows are PARTIAL per route',
          'not_included': 'vehicle route geometry, complete outside-region stop sequence, timetables, fares, runtime simulation',
          'stops': selected, 'route_stops_in_region': rstop, 'routes_serving_region': routes}, len(selected))

def copy_file(src, dest):
    source(src)
    dest = OUT / dest
    dest.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(src, dest)
    outputs[str(dest.relative_to(OUT))] = {'bytes': dest.stat().st_size, 'sha256': sha(dest)}

index_path = SRC / 'derived/mtr-stations.json'
source(index_path)
mtr = [r for r in json.loads(index_path.read_text()) if r['code'] in ['tst','ets','aus','kow','jor','ymt','mok','mkk']]
for r in mtr:
    dest = f'mtr/{Path(r["path"]).name}'
    copy_file(SRC / r['path'], dest)
    r['upstream_path'], r['path'] = r['path'], dest
write('mtr/index.json', mtr, len(mtr))
copy_file(SRC / 'raw/csdi/pedestrian/3DPN_DataDictionary_v2.2.pdf', 'reference/3DPN_DataDictionary_v2.2.pdf')
copy_file(SRC / 'derived/hong-kong-memory-elements.json', 'reference/upstream-memory-elements.json')
copy_file(SRC / 'raw/osm/osm-license.html', 'reference/osm-license.html')
catalog = json.loads((SRC / 'catalog/downloads.json').read_text())
catalog_selected = [r for r in catalog if r.get('path') in sources or
                    any(s in r.get('id','') for s in ['osm', 'pedestrian', 'csdi-terms', 'td-full'])]
write('source-catalog.json', catalog_selected, len(catalog_selected))
manifest = {'created': '2026-11-01', 'source_root': str(SRC), 'selection_bbox_crs84': BBOX,
            'selection_area_is_not_playable_area': True,
            'local_origin_crs84': [114.170, 22.300], 'local_origin_epsg2326': to_hk.transform(114.170,22.300),
            'game_coordinates': 'not generated; geographic compression is a separate authored mapping',
            'transport_counts': transport, 'sources': sources, 'outputs': outputs,
            'tool_versions': {'pyogrio': pyogrio.__version__, 'numpy': np.__version__},
            'gaps': ['No verified utility pipe network', 'No verified current shop frontage or operating status',
                     'No West Kowloon high speed rail station layout in the copied MTR local-station set',
                     'No validated walking graph or game collision geometry',
                     'No complete coastline land mask; water polygons alone do not establish coastline',
                     'OSM candidate names may match unrelated businesses; not final landmark placements']}
write('manifest.json', manifest)
print(json.dumps({'osm_layers': {k:v['count'] for k,v in outputs.items() if k.startswith('osm/')},
                  'pedestrian':len(paths),'transport':transport,'mtr_pdfs':len(mtr)}, ensure_ascii=False, indent=2))
