#!/usr/bin/env python3
"""Build a self-contained HTML report and an SVG from extracted GIS evidence."""
import base64
import html
import json
import os
import re
from pathlib import Path
from PIL import Image
from pyproj import Transformer
from shapely.geometry import shape, box
from shapely.ops import transform

ROOT = Path(__file__).resolve().parents[1]
DATA = ROOT / 'data/hong-kong'
DOCS = ROOT / 'docs'
manifest = json.loads((DATA / 'manifest.json').read_text())
proj = Transformer.from_crs('OGC:CRS84', 'EPSG:2326', always_xy=True)
b = manifest['selection_bbox_crs84']
extent = proj.transform_bounds(*b, densify_pts=21)
clip = box(*extent)
W, H, margin = 860, 1030, 34
scale = min((W-margin*2)/(extent[2]-extent[0]), (H-margin*2)/(extent[3]-extent[1]))

def xy(x, y): return margin+(x-extent[0])*scale, H-margin-(y-extent[1])*scale
def path_geometry(g):
    if g.is_empty: return ''
    if g.geom_type in ['MultiPolygon','MultiLineString','GeometryCollection']:
        return ' '.join(path_geometry(part) for part in g.geoms)
    if g.geom_type == 'Polygon':
        def ring(coords):
            points = [xy(x,y) for x,y,*_ in coords]
            return 'M'+'L'.join(f'{x:.1f},{y:.1f}' for x,y in points)+'Z'
        return ring(g.exterior.coords)+' '.join(ring(r.coords) for r in g.interiors)
    if g.geom_type in ['LineString','LinearRing']:
        points = [xy(x,y) for x,y,*_ in g.coords]
        return 'M'+'L'.join(f'{x:.1f},{y:.1f}' for x,y in points)
    return ''

svg = [f'<svg id="geo-map" role="img" aria-label="由真实选区数据绘制的九龙研究图，可开关图层和缩放" viewBox="0 0 {W} {H}" xmlns="http://www.w3.org/2000/svg">',
       '<style>.map-label{font:13px system-ui,sans-serif;fill:#173c34;paint-order:stroke;stroke:#f9f6ed;stroke-width:4px;stroke-linejoin:round}.marker{cursor:pointer}.marker:hover circle{fill:#c3522d}</style>',
       f'<rect width="{W}" height="{H}" fill="#f2efe5"/>']
source_features = {}
for group in ['water','landuse','buildings','roads','railways']:
    fs = json.loads((DATA / f'osm/{group}.geojson').read_text())['features']
    styles = {'water':'fill="#bbd7df" stroke="none"', 'landuse':'fill="#cbd9bc" stroke="none"',
              'buildings':'fill="#d2c8b5" stroke="#bdb29f" stroke-width=".25"',
              'roads':'fill="none" stroke="#fffdf6" stroke-width="1.2"',
              'railways':'fill="none" stroke="#b68876" stroke-width=".8" stroke-dasharray="3 2"'}
    svg.append(f'<g id="layer-{group}" {styles[group]}>')
    for f in fs:
        props = f['properties']
        if group == 'landuse' and props.get('fclass') not in ['park','grass','forest','recreation_ground']: continue
        g = transform(proj.transform, shape(f['geometry'])).intersection(clip).simplify(.7)
        d = path_geometry(g)
        if not d: continue
        attr = ''
        if group == 'roads':
            cls = props.get('fclass')
            if cls in ['footway','path','steps']: attr = ' stroke="#c1c7b9" stroke-width=".4"'
            elif cls in ['primary','secondary','trunk','motorway']: attr = ' stroke="#fbf9ed" stroke-width="2"'
        svg.append(f'<path d="{d}"{attr}/>')
    svg.append('</g>')

paths = json.loads((DATA / 'pedestrian/paths-hk1980.json').read_text())['paths']
svg.append('<g id="layer-pedestrian" style="display:none" fill="none" stroke="#318c7d" stroke-width=".55" opacity=".7">')
for p in paths:
    d = path_geometry(shape(p['geometry']).intersection(clip).simplify(1))
    if d: svg.append(f'<path d="{d}"/>')
svg.append('</g>')

svg.append('<g id="layer-bus" style="display:none" fill="#cc633f" opacity=".8">')
for p in json.loads((DATA / 'transport/bus.json').read_text())['stops']:
    x,y=xy(float(p['X']),float(p['Y']))
    svg.append(f'<circle cx="{x:.1f}" cy="{y:.1f}" r="2.3"><title>BUS stop {html.escape(p["STOP_ID"])}</title></circle>')
svg.append('</g>')

entrance_data = DATA / 'street-life/rail-entrance-candidates.geojson'
if entrance_data.exists():
    svg.append('<g id="layer-entrances" style="display:none">')
    for f in json.loads(entrance_data.read_text())['features']:
        x, y = xy(*proj.transform(*f['geometry']['coordinates']))
        p = f['properties']; tags = p['tags']
        detail = f'轨道出入口候选｜{p["source_id"]}｜出口标签 {tags.get("ref", "未标注")}｜所属车站、地面/地下及开放情况待港铁图核对；OSM 2026-10-22'
        svg.append(f'<g class="marker" tabindex="0" role="button" aria-label="轨道入口候选" data-detail="{html.escape(detail,quote=True)}"><circle cx="{x:.1f}" cy="{y:.1f}" r="3.3" fill="#ba3544" stroke="white" stroke-width=".8"/><title>{html.escape(detail)}</title></g>')
    svg.append('</g>')

anchor_data = DATA / 'street-life/landmark-evidence.json'
if anchor_data.exists():
    svg.append('<g id="layer-life-anchors" style="display:none">')
    order = {'buildings': 0, 'poi_areas': 1, 'pois': 2, 'landuse': 3, 'transport_areas': 4, 'transport': 5}
    for group in json.loads(anchor_data.read_text())['groups']:
        if not group['matches']: continue
        candidate = min(group['matches'], key=lambda m: order.get(Path(m['source_file']).stem, 9))
        x, y = xy(*proj.transform(*candidate['representative_point_crs84']))
        detail = f'{group["label"]}｜{group["id"]}｜{candidate["name"]}｜{candidate["source_id"]}｜名称匹配代表点，非入口；OSM 2026-10-22'
        svg.append(f'<g class="marker" tabindex="0" role="button" aria-label="{html.escape(group["label"],quote=True)}" data-detail="{html.escape(detail,quote=True)}"><rect x="{x-4:.1f}" y="{y-4:.1f}" width="8" height="8" fill="#3668ad" stroke="white" stroke-width="1"/><title>{html.escape(detail)}</title></g>')
    svg.append('</g>')

for name in ['buildings','poi_areas','transport']:
    for f in json.loads((DATA / f'osm/{name}.geojson').read_text())['features']: source_features[f['id']]=f
markers = [
('gis_osm_transport_free:7919700008','西九龙高铁站',-100,-15),
('gis_osm_buildings_a_free:545997140','戏曲中心',-85,15),
('gis_osm_buildings_a_free:232889713','中港城',-65,12),
('gis_osm_pois_a_free:1445402642','海港城',-80,5),
('gis_osm_buildings_a_free:1044263638','K11 MUSEA',25,20),
('gis_osm_pois_a_free:965355341','K11 Art Mall',25,-12),
('gis_osm_transport_free:2546436295','尖东站',35,-5),
('gis_osm_transport_free:6610744414','柯士甸站',35,-5),
('gis_osm_transport_free:6586975227','九龙站',-65,-15),
]
svg.append('<g id="layer-labels">')
for key,label,dx,dy in markers:
    f=source_features['osm-snapshot:'+key]
    point=transform(proj.transform,shape(f['geometry'])).representative_point()
    x,y=xy(point.x,point.y)
    detail=f'{label}｜{f["id"]}｜OSM 2026-10-22；面对象使用内部代表点，不是入口位置。'
    svg.append(f'<g class="marker" tabindex="0" role="button" aria-label="{html.escape(label)}" data-detail="{html.escape(detail,quote=True)}"><circle cx="{x:.1f}" cy="{y:.1f}" r="4.5" fill="#215d50" stroke="white" stroke-width="1.5"/><line x1="{x:.1f}" y1="{y:.1f}" x2="{x+dx:.1f}" y2="{y+dy:.1f}" stroke="#42695d" stroke-width=".7"/><text class="map-label" x="{x+dx:.1f}" y="{y+dy:.1f}">{html.escape(label)}</text></g>')
svg.append('</g>')
# District labels are explicitly cartographic orientation annotations, not entrances.
for label,lon,lat in [('旺角 · 后续扩展',114.1695,22.320),('油麻地 · 北向连接',114.171,22.313),('佐敦 · 后续扩展',114.171,22.3065)]:
    x,y=xy(*proj.transform(lon,lat));svg.append(f'<text class="map-label" x="{x:.1f}" y="{y:.1f}" font-weight="700">{label}</text>')
svg.append(f'<path d="M45,55 L45,25 M39,34 L45,25 L51,34" stroke="#215d50" fill="none" stroke-width="2"/><text x="60" y="38" class="map-label">N</text>')
bar=500*scale
svg.append(f'<path d="M45,{H-20} h{bar:.1f}" stroke="#215d50" stroke-width="3"/><text class="map-label" x="45" y="{H-28}">500 m · 真实坐标；非游戏比例</text></svg>')
svg=''.join(svg)
(DOCS / 'research/九龙选区数据图.svg').write_text(svg,encoding='utf-8')

toc=[]
def inline(text, base):
    tokens=[]
    def token(content):
        idx=len(tokens);tokens.append(content);return f'ZZTOKEN{idx}ZZ'
    def image(m):
        path=(base/m.group(2)).resolve()
        data=base64.b64encode(path.read_bytes()).decode()
        with Image.open(path) as im: width,height=im.size
        caption = ' · imagegen 概念图，非实机截图' if 'assets/concepts' in str(path) else ' · GIS 派生图；镜头投影为设计示意'
        return token(f'<figure><img loading="lazy" width="{width}" height="{height}" src="data:image/png;base64,{data}" alt="{html.escape(m.group(1),quote=True)}"/><figcaption>{html.escape(m.group(1))}{caption}</figcaption></figure>')
    text=re.sub(r'!\[([^]]*)\]\(([^)]+)\)',image,text)
    def link(m):
        href=m.group(2)
        if not href.startswith(('http:','https:','#')): href=os.path.relpath((base/href).resolve(),DOCS)
        return token(f'<a href="{html.escape(href,quote=True)}">{html.escape(m.group(1))}</a>')
    text=re.sub(r'\[([^]]+)\]\(([^)]+)\)',link,text)
    text=re.sub(r'`([^`]+)`',lambda m: token('<code>'+html.escape(m.group(1))+'</code>'),text)
    text=html.escape(text)
    text=re.sub(r'\*\*(.+?)\*\*',r'<strong>\1</strong>',text)
    for i,t in enumerate(tokens): text=text.replace(f'ZZTOKEN{i}ZZ',t)
    return text

def markdown(path,prefix):
    lines=path.read_text().splitlines();out=[];i=0
    while i<len(lines):
        line=lines[i]
        if not line.strip(): i+=1;continue
        if line.startswith('#'):
            level=len(line)-len(line.lstrip('#'));title=line[level:].strip();id=f'{prefix}-{i}'
            if level==2: toc.append((id,title))
            out.append(f'<h{min(level+1,6)} id="{id}">{inline(title,path.parent)}</h{min(level+1,6)}>');i+=1
        elif line.startswith('|'):
            table=[]
            while i<len(lines) and lines[i].startswith('|'):
                cols=[c.strip() for c in lines[i].strip('|').split('|')]
                if not all(re.fullmatch(r'[-: ]+',c) for c in cols): table.append(cols)
                i+=1
            out.append('<div class="table-scroll"><table>')
            for n,row in enumerate(table):
                tag='th' if n==0 else 'td';out.append('<tr>'+''.join(f'<{tag}>{inline(c,path.parent)}</{tag}>' for c in row)+'</tr>')
            out.append('</table></div>')
        elif line.startswith('- ') or re.match(r'\d+\. ',line):
            ordered=not line.startswith('- ');tag='ol' if ordered else 'ul';out.append(f'<{tag}>')
            while i<len(lines) and (re.match(r'\d+\. ',lines[i]) if ordered else lines[i].startswith('- ')):
                out.append('<li>'+inline(re.sub(r'^(?:- |\d+\. )','',lines[i]),path.parent)+'</li>');i+=1
            out.append(f'</{tag}>')
        elif line.startswith('```'):
            i+=1;block=[]
            while i<len(lines) and not lines[i].startswith('```'):block.append(lines[i]);i+=1
            out.append('<pre><code>'+html.escape('\n'.join(block))+'</code></pre>');i+=1
        else:
            paragraph=[]
            while i<len(lines) and lines[i].strip() and not re.match(r'^(#|\||- |\d+\. |```)',lines[i]):paragraph.append(lines[i]);i+=1
            rendered=inline(' '.join(paragraph),path.parent)
            out.append(rendered if rendered.startswith('<figure>') else '<p>'+rendered+'</p>')
    return '\n'.join(out)

body=markdown(DOCS/'创意与技术策划案.md','plan')
hd2d=markdown(DOCS/'research/HD-2D空间与技术方案.md','hd2d')
street_life=markdown(DOCS/'research/香港街道生活与沉浸感设计.md','life')
engine=markdown(DOCS/'research/引擎调研.md','engine')
nav=''.join(f'<a href="#{id}">{html.escape(title)}</a>' for id,title in toc)
controls=''.join(f'<label><input type="checkbox" data-layer="{key}" {"checked" if checked else ""}> {name}</label>'
    for key,name,checked in [('buildings','建筑',True),('roads','道路',True),('landuse','绿地',True),('water','局部水面',True),('railways','铁路',True),('pedestrian','行人网',False),('bus','巴士站',False),('entrances','轨道入口候选',False),('life-anchors','新增街区锚点',False),('labels','原有地标',True)])
template='''<!doctype html><html lang="zh-CN"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>白帽漫游 · 香港像素开放世界策划案</title>
<style>
:root{--ink:#183c34;--paper:#f8f5ec;--muted:#64736c;--accent:#bd5838}*{box-sizing:border-box}html{scroll-behavior:smooth;scroll-padding-top:80px}body{margin:0;color:var(--ink);background:var(--paper);font:16px/1.85 -apple-system,BlinkMacSystemFont,"PingFang SC","Microsoft Yahei",sans-serif}a{color:#985131;text-underline-offset:4px}header{padding:70px max(6vw,24px) 50px;background:#173d34;color:#f8f5ec}header small{letter-spacing:3px;color:#b6cdb8}header h1{font:600 clamp(36px,6vw,76px)/1.2 Georgia,serif;margin:22px 0}header p{max-width:780px;color:#d4dfd5}.pills{display:flex;flex-wrap:wrap;gap:10px}.pills span{padding:5px 15px;border:1px solid #557366;border-radius:30px;font-size:13px}.layout{display:grid;grid-template-columns:230px minmax(0,1fr);max-width:1440px;margin:auto;gap:45px;padding:40px 32px}aside{position:sticky;top:20px;align-self:start;max-height:92vh;overflow:auto;font-size:13px}aside a{display:block;padding:6px 0;color:#4e665b;text-decoration:none}aside a:hover{color:var(--accent)}main{min-width:0}h2{font-size:32px;line-height:1.4;margin:36px 0 22px}h3{font-size:25px;line-height:1.45;border-top:1px solid #d7dccd;padding-top:34px;margin-top:52px}h4{font-size:20px}p{margin:16px 0}strong{color:#154f40}li{margin:10px 0}figure{margin:24px 0;background:#fff;border:1px solid #deded1;border-radius:10px;overflow:hidden}figure img{display:block;width:100%;height:auto}figcaption{padding:10px 18px;font-size:13px;color:var(--muted)}code{background:#eceee3;border-radius:4px;padding:2px 5px;font-size:.87em;overflow-wrap:anywhere}pre{padding:20px;background:#eceee3;overflow:auto}pre code{white-space:pre}.table-scroll{overflow:auto;margin:25px 0}table{border-collapse:collapse;min-width:620px;width:100%;font-size:14px;line-height:1.7}th{background:#e0e8da;text-align:left}td,th{padding:13px 15px;border-bottom:1px solid #d5dbcf;vertical-align:top}.stats{display:grid;grid-template-columns:repeat(4,1fr);gap:14px;margin:24px 0}.stat{padding:18px;background:#e5eadc;border-radius:10px}.stat b{font:600 26px Georgia;display:block}.stat span{font-size:12px;color:#587064}.note{background:#f0e7d7;border-left:4px solid #b2744b;padding:16px 22px;font-size:14px}.map-tools{display:flex;flex-wrap:wrap;gap:12px;margin:16px 0;font-size:13px}.map-tools label{cursor:pointer;white-space:nowrap}.map-tools button{background:#234e41;color:white;border:0;border-radius:5px;padding:7px 14px;cursor:pointer}#map-wrap{border:1px solid #d2d9ca;border-radius:10px;overflow:hidden;background:#f2efe5;max-width:820px}#geo-map{display:block;width:100%;max-height:760px;touch-action:none;cursor:grab}#geo-map.dragging{cursor:grabbing}#map-info{min-height:48px;font-size:13px;color:#5a6a60;margin:10px 0;overflow-wrap:anywhere}.footer{font-size:13px;color:#698073;border-top:1px solid #ccd5c6;padding:30px 0}details{margin-top:40px}summary{cursor:pointer;font-weight:600;font-size:24px;padding:18px;background:#e5eadc;border-radius:8px}@media(max-width:900px){.layout{display:block;padding:25px 20px}aside{position:static;max-height:220px;margin-bottom:30px;border-bottom:1px solid #ccd5c6;padding-bottom:15px;columns:2}.stats{grid-template-columns:repeat(2,1fr)}header{padding-top:45px}h2{font-size:28px}h3{font-size:23px}}@media print{aside,.map-tools{display:none}.layout{display:block}header{background:white;color:#173d34}header p{color:#173d34}figure,table{break-inside:avoid}}
</style>
<header><small>PROJECT STUDY / 2026.11.01</small><h1>白帽漫游 · 像素九龙</h1><p>像素人物走进立体香港：横向主街、纵深连接与街道生活。</p><div class="pills"><span>HD-2D WALKING WORLD</span><span>5 张概念图</span><span>空间与技术策划 v0.4</span><span>Unity 3D + 框架 · 待验证</span><span>数据已提取 · 游戏未制作</span></div></header>
<div class="layout"><aside><a href="#data-map"><b>选区与数据地图</b></a>__NAV__<a href="#engine-details">完整引擎调研</a></aside><main>
<section id="data-map"><h2>先保留城市，再决定怎样画</h2><p>这张图来自实际提取的 OSM 与 CSDI 要素。可开关图层、缩放和拖动；点击地标查看来源。新增可开关的 144 个轨道入口候选与 32 组有地图匹配的街区锚点。它展示研究覆盖，不代表已制作的游戏地图。</p><div class="stats"><div class="stat"><b>6,729</b><span>建筑面</span></div><div class="stat"><b>12,497</b><span>道路要素，含步道</span></div><div class="stat"><b>34,282</b><span>三维行人路径</span></div><div class="stat"><b>16</b><span>港铁参考图 PDF</span></div></div><div class="map-tools">__CONTROLS__</div><div class="map-tools"><button id="zoom-in" aria-label="放大地图">＋ 放大</button><button id="zoom-out" aria-label="缩小地图">－ 缩小</button><button id="zoom-reset">重置</button></div><div id="map-wrap">__MAP__</div><div id="map-info" aria-live="polite">地标面使用内部代表点，不是入口；旺角 / 油麻地 / 佐敦文字为方位示意。行人网有 Z 值，平面重叠不表示可以互通。</div><div class="note">浅色底不是精确陆地面，蓝色仅为已提取局部水面；尚未补齐完整海岸线。资料：© OpenStreetMap contributors / Geofabrik（2026-10-22）、香港地政总署 CSDI、运输署。研究窗口不等于首版开放面积。</div></section>
<article>__BODY__</article><article id="hd2d">__HD2D__</article><article id="street-life">__STREET_LIFE__</article><details id="engine-details"><summary>完整引擎调研与官方依据</summary>__ENGINE__</details><div class="footer">本页图片与地图全部内嵌，可离线单独打开。官方来源链接需联网；源数据与 Markdown 链接需保留项目文件夹。概念图由内置 imagegen 生成，非实机截图。当前未做游戏性能测试。</div></main></div>
<script>
const svg=document.getElementById('geo-map'), initial=[0,0,860,1030];let view=[...initial],drag=null;
const apply=()=>svg.setAttribute('viewBox',view.join(' '));
function zoom(f){const w=Math.min(860,Math.max(100,view[2]*f)),h=w*1030/860;view=[view[0]+(view[2]-w)/2,view[1]+(view[3]-h)/2,w,h];apply()}
document.querySelectorAll('aside a[href^="#engine-"]').forEach(el=>el.addEventListener('click',()=>{document.getElementById('engine-details').open=true}));
document.querySelectorAll('[data-layer]').forEach(el=>el.addEventListener('change',()=>document.getElementById('layer-'+el.dataset.layer).style.display=el.checked?'':'none'));
document.getElementById('zoom-in').onclick=()=>zoom(.7);document.getElementById('zoom-out').onclick=()=>zoom(1/.7);document.getElementById('zoom-reset').onclick=()=>{view=[...initial];apply()};
function showMarker(el){document.getElementById('map-info').textContent=el.dataset.detail}
document.querySelectorAll('.marker').forEach(el=>{el.addEventListener('click',()=>showMarker(el));el.addEventListener('keydown',e=>{if(e.key==='Enter'||e.key===' '){e.preventDefault();showMarker(el)}})});
svg.addEventListener('pointerdown',e=>{if(e.target.closest('.marker'))return;drag={x:e.clientX,y:e.clientY,view:[...view],matrix:svg.getScreenCTM().inverse()};svg.setPointerCapture(e.pointerId);svg.classList.add('dragging')});
svg.addEventListener('pointermove',e=>{if(!drag)return;const a=new DOMPoint(drag.x,drag.y).matrixTransform(drag.matrix),b=new DOMPoint(e.clientX,e.clientY).matrixTransform(drag.matrix);view=[drag.view[0]-(b.x-a.x),drag.view[1]-(b.y-a.y),drag.view[2],drag.view[3]];apply()});
['pointerup','pointercancel','lostpointercapture'].forEach(t=>svg.addEventListener(t,()=>{drag=null;svg.classList.remove('dragging')}));
</script></html>'''
for key,value in [('NAV',nav),('CONTROLS',controls),('MAP',svg),('BODY',body),('HD2D',hd2d),('STREET_LIFE',street_life),('ENGINE',engine)]:template=template.replace('__'+key+'__',value)
target=DOCS/'香港像素漫游策划案.html';target.write_text(template,encoding='utf-8')
print(json.dumps({'html':str(target),'bytes':target.stat().st_size,'map_features':len(paths),'concept_images_embedded':5,'gis_diagrams_embedded':1},ensure_ascii=False))
