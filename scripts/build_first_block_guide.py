"""Deterministic GIS plan and camera guide, distinct from generated concept art."""
import json
from pathlib import Path
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from matplotlib.patches import Polygon,Rectangle
from shapely.geometry import shape,box
from shapely.ops import transform
from pyproj import Transformer
ROOT=Path(__file__).resolve().parents[1];D=ROOT/'data/blocks/tst-nathan-haiphong'
origin=[114.17205,22.29825]; proj=Transformer.from_crs('OGC:CRS84',2326,always_xy=True);x0,y0=proj.transform(*origin)
def local(x,y,z=None):return (x-x0,y-y0) if z is None else (x-x0,y-y0,z)
def xy(lon,lat):x,y=proj.transform(lon,lat);return x-x0,y-y0
fig,ax=plt.subplots(figsize=(12,9),dpi=150);ax.set_facecolor('#eef2e9')
for name in ['buildings','roads']:
 for f in json.loads((D/(name+'.geojson')).read_text())['features']:
  g=transform(local,transform(proj.transform,shape(f['geometry'])))
  parts=list(g.geoms) if hasattr(g,'geoms') else [g]
  for p in parts:
   if p.geom_type=='Polygon': ax.add_patch(Polygon(p.exterior.coords,fc='#c7c6bd',ec='#999c95',lw=.4))
   elif p.geom_type=='LineString':
    major=any(k in f['properties'].get('name','') for k in ['Nathan','Haiphong']);xx,yy=p.xy;ax.plot(xx,yy,color='#707c85' if major else '#b3babe',lw=4 if major else 1,zorder=2)
for f in json.loads((D/'rail-entrance-candidates.geojson').read_text())['features']:
 x,y=xy(*f['geometry']['coordinates']);ax.scatter(x,y,c='#b72f48',s=35,zorder=4);ax.annotate(f['properties']['tags'].get('ref','?'),(x,y),xytext=(5,5),textcoords='offset points',color='#9c1833',weight='bold')
kmb=json.loads((D/'kmb-live-snapshot.json').read_text())
for s in kmb['stops']:
 x,y=xy(float(s['long']),float(s['lat']));ax.scatter(x,y,c='#28629a',s=20,zorder=4)
ax.text(-58,58,'Kowloon Mosque\n(west side of Nathan)',ha='center',fontsize=10)
ax.text(-115,105,'Kowloon Park',ha='center',color='#376347')
ax.text(-105,-11,'Haiphong Road → west',fontsize=10)
ax.text(30,4,'Nathan Road',rotation=90,fontsize=11)
# Design route: approximate sidewalk corridor only, not a validated nav mesh.
route=[(114.1719,22.29886),(114.1719,22.29832),(114.17177,22.29822),(114.1708,22.29822)]
r=[xy(*p) for p in route];ax.plot(*zip(*r),color='#d88812',lw=3,ls='--',label='Proposed walking corridor (unvalidated)')
ax.annotate('Camera A / B\nlook WEST\nscreen-right = NORTH',xy=(-15,22),xytext=(22,29),arrowprops={'arrowstyle':'->','color':'#802f89','lw':2},fontsize=8,color='#802f89')
ax.annotate('N',xy=(58,108),xytext=(58,82),arrowprops={'arrowstyle':'-|>','color':'#243340'},ha='center',weight='bold')
ax.plot([-145,-95],[-60,-60],color='black',lw=3);ax.text(-120,-68,'50 m',ha='center')
ax.set_xlim(-160,70);ax.set_ylim(-70,115);ax.set_aspect('equal');ax.set_xlabel('East offset (metres)');ax.set_ylabel('North offset (metres)')
ax.set_title('TST Nathan × Haiphong | source skeleton + design camera',fontsize=15,pad=16)
ax.legend(loc='upper left',fontsize=8)
fig.text(.09,.025,'Source: OSM contributors / Geofabrik 2026-10-22; MTR 10/2025; KMB snapshot 2026-11-01.\nBuildings/roads are map evidence; dashed route and camera are design. No level connections inferred.',fontsize=8)
fig.tight_layout(rect=[0,.065,1,1]);fig.savefig(D/'layout-guide.png');plt.close(fig)
(D/'design-layout.json').write_text(json.dumps({'status':'design_proposal_not_navigation','origin_crs84':origin,'unity_axes':'X east, Z north, Y height; no compression applied to source data','camera_main':{'look':'west','screen_right':'north','pitch_degrees':30,'projection':'weak perspective','horizontal_main':'Nathan Road','depth_branch':'Haiphong Road westward; mosque on right/north side, shops on left/south side'},'walking_corridor_crs84':route,'route_is_unverified':True,'height':'No invented hill; only verified kerbs, entrance stairs and park terraces.','commercial_policy':'Keep photo-era storefront candidates; do not insert distant Apple/K11/Harbour City into this block'},indent=2)+'\n')
print(D/'layout-guide.png')
