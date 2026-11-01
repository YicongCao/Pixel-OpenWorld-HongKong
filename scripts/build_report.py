#!/usr/bin/env python3
"""Build topic-scoped standalone HTML chapters and the GIS evidence map."""
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
(DOCS / '02-geography/figures/九龙选区数据图.svg').write_text(svg,encoding='utf-8')

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
        if not href.startswith(('http:','https:','#')):
            target=(base/href).resolve()
            if target in document_targets:
                page,anchor=document_targets[target]
                href=('#'+anchor) if page==current_page else os.path.relpath(page,current_page.parent)+'#'+anchor
            else: href=os.path.relpath(target,current_page.parent)
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

MAP_HTML = '<section id="data-map"><h2>先保留城市，再决定怎样画</h2><p>这张图来自实际提取的 OSM 与 CSDI 要素。可开关图层、缩放和拖动；点击地标查看来源。新增可开关的 144 个轨道入口候选与 32 组有地图匹配的街区锚点。它展示研究覆盖，不代表已制作的游戏地图。</p><div class="stats"><div class="stat"><b>6,729</b><span>建筑面</span></div><div class="stat"><b>12,497</b><span>道路要素，含步道</span></div><div class="stat"><b>34,282</b><span>三维行人路径</span></div><div class="stat"><b>16</b><span>港铁参考图 PDF</span></div></div><div class="map-tools">__CONTROLS__</div><div class="map-tools"><button id="zoom-in" aria-label="放大地图">＋ 放大</button><button id="zoom-out" aria-label="缩小地图">－ 缩小</button><button id="zoom-reset">重置</button></div><div id="map-wrap">__MAP__</div><div id="map-info" aria-live="polite">地标面使用内部代表点，不是入口；旺角 / 油麻地 / 佐敦文字为方位示意。行人网有 Z 值，平面重叠不表示可以互通。</div><div class="note">浅色底不是精确陆地面，蓝色仅为已提取局部水面；尚未补齐完整海岸线。资料：© OpenStreetMap contributors / Geofabrik（2026-10-22）、香港地政总署 CSDI、运输署。研究窗口不等于首版开放面积。</div></section>'
MAP_JS = "\nconst svg=document.getElementById('geo-map'), initial=[0,0,860,1030];let view=[...initial],drag=null;\nconst apply=()=>svg.setAttribute('viewBox',view.join(' '));\nfunction zoom(f){const w=Math.min(860,Math.max(100,view[2]*f)),h=w*1030/860;view=[view[0]+(view[2]-w)/2,view[1]+(view[3]-h)/2,w,h];apply()}\ndocument.querySelectorAll('[data-layer]').forEach(el=>el.addEventListener('change',()=>document.getElementById('layer-'+el.dataset.layer).style.display=el.checked?'':'none'));\ndocument.getElementById('zoom-in').onclick=()=>zoom(.7);document.getElementById('zoom-out').onclick=()=>zoom(1/.7);document.getElementById('zoom-reset').onclick=()=>{view=[...initial];apply()};\nfunction showMarker(el){document.getElementById('map-info').textContent=el.dataset.detail}\ndocument.querySelectorAll('.marker').forEach(el=>{el.addEventListener('click',()=>showMarker(el));el.addEventListener('keydown',e=>{if(e.key==='Enter'||e.key===' '){e.preventDefault();showMarker(el)}})});\nsvg.addEventListener('pointerdown',e=>{if(e.target.closest('.marker'))return;drag={x:e.clientX,y:e.clientY,view:[...view],matrix:svg.getScreenCTM().inverse()};svg.setPointerCapture(e.pointerId);svg.classList.add('dragging')});\nsvg.addEventListener('pointermove',e=>{if(!drag)return;const a=new DOMPoint(drag.x,drag.y).matrixTransform(drag.matrix),b=new DOMPoint(e.clientX,e.clientY).matrixTransform(drag.matrix);view=[drag.view[0]-(b.x-a.x),drag.view[1]-(b.y-a.y),drag.view[2],drag.view[3]];apply()});\n['pointerup','pointercancel','lostpointercapture'].forEach(t=>svg.addEventListener(t,()=>{drag=null;svg.classList.remove('dragging')}));"
controls=''.join(f'<label><input type="checkbox" data-layer="{key}" {"checked" if checked else ""}> {name}</label>'
    for key,name,checked in [('buildings','建筑',True),('roads','道路',True),('landuse','绿地',True),('water','局部水面',True),('railways','铁路',True),('pedestrian','行人网',False),('bus','巴士站',False),('entrances','轨道入口候选',False),('life-anchors','新增街区锚点',False),('labels','原有地标',True)])
catalog = json.loads((ROOT/'scripts/docs_catalog.json').read_text())
document_targets = {}
for chapter in catalog:
    page = DOCS/chapter['id']/'index.html'
    for name in chapter['documents']:
        document_targets[(page.parent/name).resolve()] = (page, 'doc-'+Path(name).stem.lower())

CSS = r"""
:root{--ink:#183c34;--muted:#63756c;--paper:#f8f5ec;--line:#d6dfd2;--accent:#a94e32}*{box-sizing:border-box}body{margin:0;background:var(--paper);color:var(--ink);font:16px/1.85 -apple-system,BlinkMacSystemFont,"PingFang SC",sans-serif}a{color:var(--accent);text-underline-offset:4px}header{background:#173d34;color:#fff8e9;padding:42px max(5vw,24px)}header small{color:#b7cebb;letter-spacing:2px}header h1{font-size:clamp(29px,4vw,48px);line-height:1.3;margin:12px 0}header p{margin:8px 0;max-width:800px;color:#d6e2d5}.layout{display:grid;grid-template-columns:240px minmax(0,1fr);gap:42px;max-width:1440px;margin:auto;padding:30px 32px}.sidebar{position:sticky;top:20px;align-self:start;max-height:94vh;overflow:auto}.sidebar a{display:block;text-decoration:none;padding:8px 12px;color:var(--ink);border-radius:6px}.sidebar a[aria-current=page]{background:#dce7d6;font-weight:650}.sidebar strong{display:block;font-size:12px;color:var(--muted);letter-spacing:1px;margin:16px 12px 7px}.sidebar .local a{font-size:13px;line-height:1.6;padding:6px 12px}.sidebar .source{border-top:1px solid var(--line);margin-top:18px;padding-top:10px;font-size:12px}main{min-width:0}article{margin-bottom:42px}h2{font-size:27px;line-height:1.45;margin:30px 0 16px}h3{font-size:22px;line-height:1.5;margin-top:38px;padding-top:22px;border-top:1px solid var(--line)}h4{font-size:18px}h2,h3,h4,article,section{scroll-margin-top:24px}p{margin:14px 0}li{margin:9px 0}strong{color:#16503f}code{background:#e9eddf;border-radius:4px;padding:2px 5px;font-size:.87em;overflow-wrap:anywhere}pre{background:#e9eddf;border-radius:8px;padding:20px;overflow:auto}pre code{white-space:pre}figure{margin:24px 0;background:#fff;border:1px solid var(--line);border-radius:9px;overflow:hidden}figure img{display:block;width:100%;height:auto}figcaption{font-size:13px;color:var(--muted);padding:10px 16px}.table-scroll{overflow:auto;margin:22px 0}table{border-collapse:collapse;width:100%;min-width:610px;font-size:14px;line-height:1.7}td,th{padding:12px;border-bottom:1px solid var(--line);text-align:left;vertical-align:top}th{background:#e1e9da}.status,.note{font-size:14px;padding:14px 18px;border-left:4px solid #ad7147;background:#f0e8d8;margin:18px 0}.stats{display:grid;grid-template-columns:repeat(4,1fr);gap:12px;margin:22px 0}.stat{background:#e1e9da;padding:14px;border-radius:8px}.stat b{display:block;font-size:24px}.stat span{font-size:12px}.map-tools{display:flex;gap:12px;flex-wrap:wrap;margin:14px 0;font-size:13px}.map-tools label{white-space:nowrap;cursor:pointer}.map-tools button{border:0;border-radius:5px;background:#245443;color:white;padding:8px 14px;cursor:pointer}#map-wrap{border:1px solid var(--line);overflow:hidden;border-radius:8px}#geo-map{display:block;width:100%;max-height:760px;touch-action:none;cursor:grab}#geo-map.dragging{cursor:grabbing}#map-info{font-size:13px;color:var(--muted);min-height:48px;overflow-wrap:anywhere}.pager{display:flex;gap:20px;justify-content:space-between;border-top:1px solid var(--line);padding:24px 0}.footer{font-size:12px;color:var(--muted);padding:14px 0}details{border:1px solid var(--line);border-radius:8px;padding:0 18px;margin:25px 0}summary{cursor:pointer;font-size:18px;font-weight:600;padding:16px 0}@media(max-width:900px){.layout{display:block;padding:22px 18px}.sidebar{position:static;max-height:none;margin-bottom:26px}.sidebar .chapters{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));font-size:13px}.sidebar .local{display:none}.sidebar .source{display:flex;flex-wrap:wrap}.stats{grid-template-columns:repeat(2,1fr)}header{padding:28px 20px}h2{font-size:24px}}@media print{.sidebar,.pager,.map-tools{display:none}.layout{display:block}header{background:white;color:#173d34}header p{color:#173d34}figure{break-inside:avoid}}
"""
ANCHOR_JS = r"""
function revealHash(){const id=decodeURIComponent(location.hash.slice(1)),target=document.getElementById(id);if(!target)return;let parent=target.closest('details');if(parent)parent.open=true;target.scrollIntoView()}
window.addEventListener('hashchange',revealHash);if(location.hash)revealHash();
document.querySelectorAll('a[href^="#"]').forEach(a=>a.addEventListener('click',()=>{const target=document.getElementById(decodeURIComponent(a.hash.slice(1)));if(target?.closest('details'))target.closest('details').open=true}));
"""
results=[]
for index, chapter in enumerate(catalog):
    current_page = DOCS/chapter['id']/'index.html'
    articles=[]
    local=[]
    for name in chapter['documents']:
        path=current_page.parent/name
        anchor='doc-'+path.stem.lower()
        title=path.read_text().splitlines()[0].lstrip('# ')
        local.append(f'<a href="#{anchor}">{html.escape(title)}</a>')
        rendered=f'<article id="{anchor}">{markdown(path,path.stem.lower())}</article>'
        if name=='concept-gallery.md': rendered='<details><summary>旧版四张概念图 · 历史研究</summary>'+rendered+'</details>'
        articles.append(rendered)
        if chapter['id']=='02-geography' and name=='README.md':
            articles.append(MAP_HTML.replace('__CONTROLS__',controls).replace('__MAP__',svg))
    global_nav=''.join(f'<a href="../{c["id"]}/index.html"'+(' aria-current="page"' if c['id']==chapter['id'] else '')+f'>{c["number"]} · {html.escape(c["title"])}</a>' for c in catalog)
    pager=[]
    for offset,label in [(-1,'上一章'),(1,'下一章')]:
        n=index+offset
        if 0<=n<len(catalog):
            c=catalog[n];pager.append(f'<a href="../{c["id"]}/index.html">{label}：{html.escape(c["title"])}</a>')
    scripts=(MAP_JS if chapter['id']=='02-geography' else '')+ANCHOR_JS
    page=f'''<!doctype html><html lang="zh-CN"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>{chapter['number']} · {html.escape(chapter['title'])} — 香港像素漫游</title><style>{CSS}</style></head><body>
<header><small>香港像素漫游 / 香港 HD-2D 研究 / {chapter['number']} OF 08</small><h1>{html.escape(chapter['title'])}</h1><p>{html.escape(chapter['description'])}</p></header>
<div class="layout"><aside class="sidebar"><strong>主章节</strong><nav class="chapters" aria-label="主章节">{global_nav}</nav><nav class="local" aria-label="本章资料"><strong>本章资料</strong>{''.join(local)}</nav><div class="source"><a href="../README.md">研究总目录 · Markdown</a><a href="../Agent.md">Agent 阅读约定</a><a href="../Skill.md">按任务读取研究</a></div></aside>
<main><div class="status">研究基线 2026-11-01 · 当前没有已验收的可玩街区；概念、候选方案与历史检查分别标记。</div>{''.join(articles)}<nav class="pager" aria-label="章节翻页">{''.join(pager)}</nav><footer class="footer">本页按主题生成，图片与地图内嵌。跨章及源文件跳转需保留目录；外部来源需要联网。请修改 Markdown 后运行 scripts/build_report.py 重建。</footer></main></div><script>{scripts}</script></body></html>'''
    current_page.write_text(page,encoding='utf-8')
    results.append({'path':str(current_page.relative_to(ROOT)),'bytes':current_page.stat().st_size})
print(json.dumps({'chapters':results,'map_paths':len(paths)},ensure_ascii=False))
