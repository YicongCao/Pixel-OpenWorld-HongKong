#!/usr/bin/env python3
"""Archive Canton source data separately from an explicitly rectified demo design."""
import json,hashlib,shutil
from pathlib import Path
from shapely.geometry import shape,box
from pyproj import Transformer
ROOT=Path(__file__).resolve().parents[1]; out=ROOT/'data/blocks/tst-canton-demo';out.mkdir(parents=True,exist_ok=True)
bounds=[114.1673,22.2977,114.1700,22.3007];window=box(*bounds); hashes={}
for name in ['roads','buildings','pois','transport']:
 p=ROOT/f'data/hong-kong/osm/{name}.geojson';j=json.loads(p.read_text());j['features']=[f for f in j['features'] if shape(f['geometry']).intersects(window)]
 (out/f'{name}.geojson').write_text(json.dumps(j,ensure_ascii=False));hashes[str(p.relative_to(ROOT))]=hashlib.sha256(p.read_bytes()).hexdigest()
bus=json.loads((ROOT/'data/hong-kong/transport/bus.json').read_text());stops=[s for s in bus['stops'] if window.covers(shape({'type':'Point','coordinates':[s['longitude'],s['latitude']]}))];ids={s['STOP_ID'] for s in stops}
bus['stops']=stops;bus['route_stops_in_region']=[s for s in bus['route_stops_in_region'] if s['STOP_ID'] in ids];(out/'bus.json').write_text(json.dumps(bus,ensure_ascii=False))
p=ROOT/'data/hong-kong/pedestrian/paths-hk1980.json';j=json.loads(p.read_text());t=Transformer.from_crs(4326,2326,always_xy=True);b=box(*t.transform(*bounds[:2]),*t.transform(*bounds[2:]));j['paths']=[s for s in j['paths'] if shape(s['geometry']).intersects(b)];(out/'pedestrian.json').write_text(json.dumps(j,ensure_ascii=False))
shutil.copyfile(ROOT/'data/hong-kong/mtr/mtr-maps-tst.pdf',out/'tst-street.pdf') if (ROOT/'data/hong-kong/mtr/mtr-maps-tst.pdf').exists() else None
roads=json.loads((out/'roads.geojson').read_text())['features'];main=next(f for f in roads if f['properties']['osm_id']=='564217668');xy=[t.transform(*q) for q in main['geometry']['coordinates']];length=sum(((b[0]-a[0])**2+(b[1]-a[1])**2)**.5 for a,b in zip(xy,xy[1:]));span=round(length*.45,3)
layout={'block':'tst-canton-demo','sourceRoadId':main['id'],'sourceLengthM':length,'designLengthM':span,'designScale':.45,'mapping':'Canton centerline rectified to horizontal X; north is -X, east is +Z. Cross-street, sidewalk widths, elevations, storefront placement and activities are design approximations; source geometry unchanged. Not a surveyed replica.','route':[{'x':x,'y':.22,'z':z} for x,z in [(-span/2+3,2),(0,2),(span/2-8,2),(span/2-8,19),(span/2-8,2),(span/2-3,2)]],'busStopVerified':False,'mtr':'A1 belongs to Nathan/Haiphong east of this slice; no invented Canton station entrance.'}
(out/'layout.json').write_text(json.dumps(layout,ensure_ascii=False,indent=2));target=ROOT/'unity/HongKong/Assets/HongKong/Generated/Data/CantonDemo.json';shutil.copyfile(out/'layout.json',target)
for source in ['data/hong-kong/transport/bus.json','data/hong-kong/pedestrian/paths-hk1980.json','data/hong-kong/mtr/mtr-maps-tst.pdf']:
 hashes[source]=hashlib.sha256((ROOT/source).read_bytes()).hexdigest()
(out/'manifest.json').write_text(json.dumps({'date':'2026-11-01','bboxCRS84':bounds,'upstream':'Read-only copies already extracted from ../game-research; original snapshots retained','sourceHashes':hashes,'counts':{'pedestrian':len(j['paths']),'busStops':len(stops)},'outputs':{p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in out.iterdir() if p.name!='manifest.json'}},indent=2))
print(layout);print('paths',len(j['paths']),'stops',len(stops))
