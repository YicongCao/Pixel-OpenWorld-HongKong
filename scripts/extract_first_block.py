"""Read-only subset of the existing HK snapshot; no geographic compression or nav graph."""
import hashlib,json,shutil
from pathlib import Path
from shapely.geometry import shape,box
from shapely.ops import transform
from pyproj import Transformer
ROOT=Path(__file__).resolve().parents[1]
SRC=ROOT/'data/hong-kong'; OUT=ROOT/'data/blocks/tst-nathan-haiphong'
BBOX=[114.1705,22.2976,114.1726,22.2993]
window=box(*BBOX); to_m=Transformer.from_crs('OGC:CRS84',2326,always_xy=True)
window_m=transform(to_m.transform,window); inputs={}; counts={}
def digest(p): return hashlib.sha256(p.read_bytes()).hexdigest()
def read(name):
 p=SRC/name; inputs[str(p.relative_to(ROOT))]=digest(p);return json.loads(p.read_text())
def write(name,d):
 p=OUT/name;p.parent.mkdir(parents=True,exist_ok=True);p.write_text(json.dumps(d,ensure_ascii=False,separators=(',',':'))+'\n')
OUT.mkdir(parents=True,exist_ok=True)
for name in ['osm/roads.geojson','osm/buildings.geojson','osm/pois.geojson','street-life/rail-entrance-candidates.geojson','street-life/street-nodes.geojson']:
 d=read(name); selected=[f for f in d['features'] if shape(f['geometry']).intersects(window)]
 write(Path(name).name,{'type':'FeatureCollection','features':selected});counts[Path(name).name]=len(selected)
ped=read('pedestrian/paths-hk1980.json'); paths=[p for p in ped['paths'] if shape(p['geometry']).intersects(window_m)]
write('pedestrian.json',{'horizontal_crs':'EPSG:2326','vertical_crs':'EPSG:5738 HKPD','units':'metres','selection':'Full source geometries intersecting projected bbox; not a connected navigation graph','paths':paths});counts['pedestrian_paths']=len(paths)
# Retain original relationship tables as small context rather than stripping transitive dependencies.
write('pedestrian-relations-context.json',read('pedestrian/relations.json'))
for mode in ['bus','gmb']:
 d=read('transport/'+mode+'.json');stops=[s for s in d['stops'] if BBOX[0]<=s['longitude']<=BBOX[2] and BBOX[1]<=s['latitude']<=BBOX[3]]
 ids={s['STOP_ID'] for s in stops};rs=[r for r in d['route_stops_in_region'] if r['STOP_ID'] in ids];rids={r['ROUTE_ID'] for r in rs}
 write(mode+'.json',{**{k:v for k,v in d.items() if k not in ['stops','route_stops_in_region','routes_serving_region']},'selection':{'bbox_crs84':BBOX,'complete_routes':False},'stops':stops,'route_stops_in_block':rs,'routes_serving_block':[r for r in d['routes_serving_region'] if r['ROUTE_ID'] in rids]});counts[mode]={'stops':len(stops),'route_stop_rows':len(rs)}
for name in ['mtr-maps-tst.pdf','mtr-layouts-tst.pdf']:
 p=SRC/'mtr'/name;inputs[str(p.relative_to(ROOT))]=digest(p);shutil.copy2(p,OUT/name)
kmb=ROOT/'.local/block-research/kmb-stops.json';kr=ROOT/'.local/block-research/kmb-route-stops.json'
if (OUT/'kmb-live-snapshot.json').exists():
 counts['kmb_stops']=len(json.loads((OUT/'kmb-live-snapshot.json').read_text())['stops'])
elif kmb.exists() and kr.exists():
 d=json.loads(kmb.read_text());stops=[s for s in d['data'] if BBOX[0]<=float(s['long'])<=BBOX[2] and BBOX[1]<=float(s['lat'])<=BBOX[3]];ids={s['stop'] for s in stops}
 rd=json.loads(kr.read_text()); rs=[r for r in rd['data'] if r['stop'] in ids]
 write('kmb-live-snapshot.json',{'retrieved':'2026-11-01','urls':['https://data.etabus.gov.hk/v1/transport/kmb/stop','https://data.etabus.gov.hk/v1/transport/kmb/route-stop'],'raw_sha256':[digest(kmb),digest(kr)],'generated_timestamps':[d.get('generated_timestamp'),rd.get('generated_timestamp')],'stops':stops,'route_stops_in_block':rs,'complete_routes':False,'note':'KMB stop IDs are not TD STOP_ID; no automatic cross-provider join'});counts['kmb_stops']=len(stops)
write('manifest.json',{'created':'2026-11-01','block_id':'tst-nathan-haiphong','bbox_crs84':BBOX,'bbox_is_not_playable_boundary':True,'upstream':'../game-research/data/hong-kong (read-only; parent package already copied)','parent_manifest':'../../hong-kong/manifest.json','parent_manifest_sha256':digest(SRC/'manifest.json'),'inputs':inputs,'counts':counts,'selection':'Full geometries intersecting bbox; native geometry/attributes unchanged. Relationships retain parent-region context. Bus/GMB from complete-table-derived snapshots; selected stop sequences are partial.','crs':'GeoJSON CRS84 degrees; pedestrian EPSG:2326 metres + HKPD. No DEM or height compression.','attribution':['OpenStreetMap contributors / Geofabrik (ODbL)','Lands Department CSDI','Transport Department','MTR Corporation','KMB / DATA.GOV.HK'],'terms':'See ../../hong-kong/source-catalog.json and reference/osm-license.html; no blanket artwork license','reproduce':'../game-research/.venv-hk-data/bin/python scripts/extract_first_block.py (preserves existing KMB evidence; first run requires raw KMB stop and route-stop responses in .local/block-research)' ,'gaps':['Lane-level paths and traffic signals unverified','No field-verified walkable graph','No reliable terrain DEM','Storefront identities and shop status require dated photos','No measured NPC schedules or ice-cream van timetable'],'outputs':{str(p.relative_to(OUT)):digest(p) for p in sorted(OUT.iterdir()) if p.is_file() and p.name not in ['manifest.json','README.md','layout-guide.png','layout-guide.svg','design-layout.json']}})
print(json.dumps(counts))
