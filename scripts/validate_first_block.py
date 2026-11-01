"""Validate block provenance and spatial selection; never certifies gameplay."""
import hashlib,json
from pathlib import Path
from shapely.geometry import shape,box
from shapely.ops import transform
from pyproj import Transformer
ROOT=Path(__file__).resolve().parents[1];D=ROOT/'data/blocks/tst-nathan-haiphong';A=ROOT/'assets/blocks/tst-nathan-haiphong/generated/concepts';R=ROOT/'references/blocks/tst-nathan-haiphong'
def read(p):return json.loads(p.read_text())
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
m=read(D/'manifest.json');errors=[];checks={}
def check(ok,msg):
 if not ok:errors.append(msg)
for name,h in m['inputs'].items(): check(sha(ROOT/name)==h,'source changed: '+name)
for name,h in m['outputs'].items():check(sha(D/name)==h,'output changed: '+name)
window=box(*m['bbox_crs84']);t=Transformer.from_crs('OGC:CRS84',2326,always_xy=True);mw=transform(t.transform,window)
for name in ['roads.geojson','buildings.geojson','pois.geojson','street-nodes.geojson','rail-entrance-candidates.geojson']:
 fs=read(D/name)['features'];check(len(fs)==m['counts'][name],name+' count')
 check(all(shape(f['geometry']).intersects(window) for f in fs),name+' selection')
 ids=[f['properties']['source_id'] for f in fs];check(len(ids)==len(set(ids)),name+' duplicate source IDs')
ps=read(D/'pedestrian.json')['paths'];check(len(ps)==m['counts']['pedestrian_paths'],'pedestrian count');check(all(shape(p['geometry']).intersects(mw) for p in ps),'pedestrian selection')
for mode in ['bus','gmb']:
 b=read(D/(mode+'.json'));ids={s['STOP_ID'] for s in b['stops']};rids={r['ROUTE_ID'] for r in b['routes_serving_block']};check(all(r['STOP_ID'] in ids and r['ROUTE_ID'] in rids for r in b['route_stops_in_block']),mode+' orphan route/stop')
 check(all(window.covers(shape({'type':'Point','coordinates':[s['longitude'],s['latitude']]})) for s in b['stops']),mode+' stop selection')
k=read(D/'kmb-live-snapshot.json');ids={s['stop'] for s in k['stops']};check(all(r['stop'] in ids for r in k['route_stops_in_block']),'KMB orphan stop');check(len(ids)==9,'KMB stop count')
a1=[f for f in read(D/'rail-entrance-candidates.geojson')['features'] if f['properties']['tags'].get('ref')=='A1'];check(len(a1)==1,'A1 unique')
a=read(A/'manifest.json')
for name,h in a['inputs'].items():check(sha(ROOT/name)==h,'concept input changed: '+name)
for item in a['outputs']:check(sha(ROOT/item['file'])==item['sha256'],'concept changed: '+item['file'])
for r in read(R/'index.json')['references']:
 check(sha(R/r['file'])==r['sha256'],'photo original hash: '+r['id']);p=r['input_preview'];check(sha(R/p['file'])==p['sha256'],'photo input hash: '+r['id']);check(r['actually_viewed'] and r['license_url'].startswith('https://creativecommons.org/'),'photo provenance: '+r['id'])
checks={'source_hashes':len(m['inputs']),'output_hashes':len(m['outputs']),'spatial_feature_counts':m['counts'],'photo_originals_and_previews':6,'selected_concepts':3,'superseded_concepts':1,'gameplay_tested':False}
result={'date':'2026-11-01','scope':'Hashes, bbox selection, IDs and references only; no walkability, levels, traffic, screenshot authenticity or performance certification','passed':not errors,'checks':checks,'errors':errors}
(ROOT/'docs/08-blocks/tst-nathan-haiphong/data-validation.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n');print(json.dumps(result,ensure_ascii=False));raise SystemExit(bool(errors))
