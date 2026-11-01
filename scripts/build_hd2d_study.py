#!/usr/bin/env python3
"""GIS-grounded corridor presentation study; not a pedestrian routing graph."""
import hashlib,json
from pathlib import Path
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from matplotlib.font_manager import FontProperties, findSystemFonts
from matplotlib.lines import Line2D
from shapely.geometry import shape,box,mapping
from shapely.ops import transform,unary_union
from pyproj import Transformer
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'docs/research/hd2d'; OUT.mkdir(exist_ok=True)
DATA=ROOT/'data/hong-kong'
proj=Transformer.from_crs('OGC:CRS84','EPSG:2326',always_xy=True)
bounds=[114.164,22.294,114.175,22.310]
clip=transform(proj.transform,box(*bounds))
minx,miny,maxx,maxy=clip.bounds
rows=json.loads((DATA/'osm/roads.geojson').read_text())['features']
colors={'廣東道 Canton Road':'#c35f37','彌敦道 Nathan Road':'#286f83','海防道 Haiphong Road':'#94448a','佐敦道 Jordan Road':'#4c7b42','柯士甸道 Austin Road':'#b78d25'}
short={'廣東道 Canton Road':'广东道','彌敦道 Nathan Road':'弥敦道','海防道 Haiphong Road':'海防道','佐敦道 Jordan Road':'佐敦道','柯士甸道 Austin Road':'柯士甸道'}
selected=[]
for f in rows:
 name=f['properties'].get('name')
 if name in colors:
  g=transform(proj.transform,shape(f['geometry'])).intersection(clip)
  if not g.is_empty: selected.append((f,g))
font_candidates=[p for p in findSystemFonts() if any(n in p for n in ('STHeiti Light','NotoSansCJK','NotoSansSC','Arial Unicode','PingFang'))]
if not font_candidates: raise RuntimeError('Install a CJK font (Noto Sans CJK / STHeiti / PingFang) before rendering.')
font=FontProperties(fname=sorted(font_candidates)[0])
plt.rcParams['axes.unicode_minus']=False
fig,axes=plt.subplots(1,2,figsize=(15,8.4),gridspec_kw={'width_ratios':[.85,1.5]})
fig.patch.set_facecolor('#f7f3e9')
for ax in axes:ax.set_facecolor('#eeeadf');ax.set_xticks([]);ax.set_yticks([]);ax.spines[['top','bottom','left','right']].set_visible(False)
def lines(g):
 if g.geom_type=='LineString':yield list(g.coords)
 elif hasattr(g,'geoms'):
  for c in g.geoms:yield from lines(c)
def points(coords,mode):
 if mode==0:return ([x-minx for x,y,*_ in coords],[y-miny for x,y,*_ in coords])
 return ([y-miny for x,y,*_ in coords],[-(x-minx)*.55 for x,y,*_ in coords])
for f in rows:
 g=transform(proj.transform,shape(f['geometry']))
 if not g.intersects(clip):continue
 for coords in lines(g.intersection(clip)):
  for mode,ax in enumerate(axes):ax.plot(*points(coords,mode),color='#cbc9bf',lw=.5,zorder=1)
for f,g in selected:
 for coords in lines(g):
  for mode,ax in enumerate(axes):ax.plot(*points(coords,mode),color=colors[f['properties']['name']],lw=2.6,zorder=3)
for ax in axes:ax.set_aspect('equal',adjustable='box')
axes[0].set_title('A  真实道路骨架 · 北向上',fontproperties=font,fontsize=16,pad=20)
axes[1].set_title('B  镜头组织示意 · 北向右、东向近景',fontproperties=font,fontsize=16,pad=20)
axes[0].text(.02,.97,'N ↑',transform=axes[0].transAxes,fontsize=13,va='top')
axes[1].text(.02,.97,'屏幕横向 → 北\n纵深通道 ↓ 东（近景）',fontproperties=font,transform=axes[1].transAxes,fontsize=11,va='top')
# Label positions are manual cartographic annotations; not entrances or measured junctions.
labels=[('广东道',114.169,22.299,'廣東道 Canton Road'),('弥敦道',114.1723,22.303,'彌敦道 Nathan Road'),('海防道',114.1705,22.2981,'海防道 Haiphong Road'),('佐敦道',114.1695,22.3058,'佐敦道 Jordan Road'),('柯士甸道',114.170,22.3032,'柯士甸道 Austin Road')]
for label,lon,lat,name in labels:
 x,y=proj.transform(lon,lat)
 for mode,ax in enumerate(axes):
  xs,ys=points([(x,y)],mode)
  offsets={0:{'广东道':(-65,2),'弥敦道':(12,0),'海防道':(0,-23),'佐敦道':(-20,10),'柯士甸道':(-55,-20)},1:{'广东道':(-15,16),'弥敦道':(-10,-23),'海防道':(-32,-16),'佐敦道':(0,6),'柯士甸道':(-10,4)}}
  ax.annotate(label,(xs[0],ys[0]),xytext=offsets[mode][label],textcoords='offset points',fontproperties=font,color=colors[name],fontsize=12,bbox={'facecolor':'#f7f3e9','edgecolor':'none','alpha':.88,'pad':2},zorder=5)
legend=[Line2D([0],[0],color=c,lw=3,label=short[n]) for n,c in colors.items()]
fig.legend(handles=legend,prop=font,loc='lower center',ncol=5,bbox_to_anchor=(.5,.105),frameon=False)
fig.suptitle('同一街网，两种观看方式：横向主街 + 纵深连接',fontproperties=font,fontsize=23,y=.96,color='#1b483e')
fig.text(.06,.07,'B 对东向距离做 0.55 倍示意压缩，仅演示观看方向；不是最终投影、导航图或全城同屏场景。',fontproperties=font,fontsize=11)
fig.text(.06,.035,'海防道连接广东道与弥敦道；前往佐敦道需继续北行。佐敦道若横向展示，应使用独立镜头区。',fontproperties=font,fontsize=11)
fig.text(.06,.009,'来源：© OpenStreetMap contributors / Geofabrik · 2026-10-22 快照；绘图 2026-11-01。',fontproperties=font,fontsize=9,color='#617068')
fig.subplots_adjust(left=.055,right=.97,top=.86,bottom=.19,wspace=.22)
fig.savefig(OUT/'corridor-projection.png',dpi=150,facecolor=fig.get_facecolor())
fig.savefig(OUT/'corridor-projection.svg',facecolor=fig.get_facecolor())
plt.close(fig)
evidence={'created':'2026-11-01','source_snapshot':'2026-10-22','study_bbox_crs84':bounds,'drawing_geometry':'clipped in EPSG:2326; output GeoJSON preserves complete selected CRS84 source geometries','source_file':'data/hong-kong/osm/roads.geojson','source_sha256':hashlib.sha256((DATA/'osm/roads.geojson').read_bytes()).hexdigest(),'road_groups':{},'note':'Clipped geometry for drawing only. No automatic routing edges created. Screen projection is a design schematic, not a surveyed view.'}
for name in colors:
 fs=[(f,g) for f,g in selected if f['properties']['name']==name]
 u=unary_union([g for f,g in fs]); evidence['road_groups'][name]={'feature_count_in_study':len(fs),'source_ids':[f['properties']['source_id'] for f,g in fs],'geometry_bounds_hk1980':list(u.bounds)}
outputs=ROOT/'data/hong-kong/hd2d-study'
(outputs/'corridor-evidence.json').write_text(json.dumps(evidence,ensure_ascii=False,indent=2)+'\n')
(outputs/'corridor-roads.geojson').write_text(json.dumps({'type':'FeatureCollection','features':[f for f,g in selected]},ensure_ascii=False)+'\n')
hashfiles=['docs/research/hd2d/corridor-projection.png','docs/research/hd2d/corridor-projection.svg','data/hong-kong/hd2d-study/corridor-evidence.json','data/hong-kong/hd2d-study/corridor-roads.geojson']
manifest={'created':'2026-11-01','source_sha256':evidence['source_sha256'],'outputs':{p:hashlib.sha256((ROOT/p).read_bytes()).hexdigest() for p in hashfiles},'font':font.get_file(),'matplotlib_version':matplotlib.__version__}
(outputs/'manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
print(json.dumps({n:v['feature_count_in_study'] for n,v in evidence['road_groups'].items()},ensure_ascii=False))
