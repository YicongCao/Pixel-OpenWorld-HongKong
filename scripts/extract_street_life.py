#!/usr/bin/env python3
"""Read-only source extraction for street-life planning, not runtime map generation."""
import collections
import hashlib
import json
from pathlib import Path
import osmium
from shapely.geometry import shape, mapping

ROOT = Path(__file__).resolve().parents[1]
DATA = ROOT / 'data/hong-kong'
SOURCE = ROOT.parent / 'game-research/data/hong-kong'
OUT = DATA / 'street-life'
OUT.mkdir(exist_ok=True)
BBOX = json.loads((DATA / 'manifest.json').read_text())['selection_bbox_crs84']
PBF = SOURCE / 'raw/osm/hong-kong-261022.osm.pbf'

def sha(path):
    h = hashlib.sha256()
    with path.open('rb') as f:
        for block in iter(lambda: f.read(1024 * 1024), b''): h.update(block)
    return h.hexdigest()

def save(name, value):
    (OUT / name).write_text(json.dumps(value, ensure_ascii=False, indent=2) + '\n')

class StreetNodes(osmium.SimpleHandler):
    def __init__(self):
        super().__init__()
        self.features = []

    def node(self, node):
        if not node.location.valid(): return
        x, y = node.location.lon, node.location.lat
        if not (BBOX[0] <= x <= BBOX[2] and BBOX[1] <= y <= BBOX[3]): return
        tags = dict(node.tags)
        categories = []
        for key, values in {
            'railway': {'subway_entrance'},
            'highway': {'traffic_signals', 'crossing', 'street_lamp', 'elevator'},
            'amenity': {'bench', 'waste_basket', 'recycling', 'post_box', 'drinking_water', 'telephone', 'taxi', 'toilets'},
            'emergency': {'fire_hydrant'},
            'barrier': {'bollard'},
            'natural': {'tree'},
        }.items():
            if tags.get(key) in values: categories.append(key + ':' + tags[key])
        if not categories: return
        self.features.append({'type': 'Feature', 'geometry': {'type': 'Point', 'coordinates': [x, y]},
            'properties': {'source_id': 'osm:node:' + str(node.id), 'snapshot': '2026-10-22',
                'categories': categories, 'tags': tags,
                'evidence_status': 'snapshot_node_not_field_verified'}})

handler = StreetNodes()
handler.apply_file(str(PBF))
handler.features.sort(key=lambda f: int(f['properties']['source_id'].split(':')[-1]))
save('street-nodes.geojson', {'type': 'FeatureCollection', 'features': handler.features})
entrances = [f for f in handler.features if 'railway:subway_entrance' in f['properties']['categories']]
save('rail-entrance-candidates.geojson', {'type': 'FeatureCollection', 'features': entrances})

# Human-selected identity anchors. Labels do not imply current operation or exact entrances.
groups = [
    ('钟楼', '尖沙咀海滨', ['尖沙咀鐘樓 Tsim Sha Tsui Clock Tower']),
    ('香港文化中心', '尖沙咀海滨', ['香港文化中心 Hong Kong Cultural Centre']),
    ('香港太空馆', '尖沙咀海滨', ['香港太空館']),
    ('香港艺术馆', '尖沙咀海滨', ['香港藝術館']),
    ('天星码头', '尖沙咀海滨', ['尖沙咀碼頭', '尖沙咀天星碼頭']),
    ('半岛酒店', '尖沙咀南', ['香港半島酒店 The Peninsula Hong Kong']),
    ('1881 Heritage', '尖沙咀南', ['1881 Heritage', '富衛1881公館']),
    ('重慶大厦', '弥敦道', ['重慶大廈 Chungking Mansions']),
    ('美丽都大厦', '弥敦道', ['美麗都大廈 Mirador Mansion']),
    ('九龙公园', '尖沙咀内街', ['九龍公園 Kowloon Park']),
    ('九龙清真寺', '弥敦道', ['九龍清真寺及伊斯蘭中心']),
    ('圣安德烈堂', '弥敦道', ["聖安德烈堂 St. Andrew's Church"]),
    ('香港文物探知馆', '九龙公园', ['香港文物探知館', 'Hong Kong Heritage Discovery Centre']),
    ('前九龙英童学校', '弥敦道', ['前九龍英童學校', 'Former Kowloon British School']),
    ('The ONE', '尖沙咀内街', ['The One']),
    ('美丽华广场', '尖沙咀内街', ['美麗華廣場']),
    ('讯号山与讯号塔', '尖沙咀东侧', ['訊號塔 Signal Tower', '訊號山花園', 'Signal Hill Garden']),
    ('香港科学馆', '尖东', ['香港科學館 Hong Kong Science Museum']),
    ('香港历史博物馆', '尖东', ['香港歷史博物館 Hong Kong Museum of History']),
    ('尖沙咀中心', '尖东', ['尖沙咀中心 Tsim Sha Tsui Centre']),
    ('帝国中心', '尖东', ['帝國中心 Empire Centre']),
    ('市政局百周年纪念花园', '尖东', ['市政局百週年紀念花園', 'Urban Council Centenary Garden']),
    ('庙街', '佐敦—油麻地', ['廟街 Temple Street', '廟街夜市 Temple Street Night Market', '廟街牌樓']),
    ('油麻地天后庙', '油麻地', ['油麻地天后廟', 'Yau Ma Tei Tin Hau Temple']),
    ('玉器市场', '油麻地', ['油蔴地玉器小販市場']),
    ('油麻地果栏', '油麻地', ['油麻地果欄 Yau Ma Tei Wholesale Fruit Market']),
    ('油麻地戏院', '油麻地', ['油麻地戲院 Yau Ma Tei Theatre']),
    ('红砖屋', '油麻地', ['紅磚屋 Red Brick Building']),
    ('朗豪坊', '旺角', ['朗豪坊 Langham Place']),
    ('女人街', '旺角', ['女人街', "Ladies' Market"]),
    ('金鱼街', '旺角', ['金魚街', 'Goldfish Market']),
    ('花墟', '旺角北侧', ['花墟', 'Flower Market']),
    ('M+', '西九', ['M+']),
    ('香港故宫文化博物馆', '西九', ['香港故宮文化博物館', 'Hong Kong Palace Museum']),
    ('环球贸易广场与圆方', '西九', ['環球貿易廣場', '圓方 Elements']),
]
files = [DATA / 'osm' / (name + '.geojson') for name in ['buildings', 'pois', 'poi_areas', 'transport', 'transport_areas', 'landuse']]
features = [(p, f) for p in files for f in json.loads(p.read_text())['features']]
anchor_rows = []
anchor_points = []
for idx, (label, district, needles) in enumerate(groups, 1):
    matches = []
    for path, f in features:
        name = f['properties'].get('name') or ''
        # Short Latin names require exact matching, to avoid unrelated shop names.
        if not any((name == n if n in ['M+', 'The One'] else n in name) for n in needles): continue
        point = shape(f['geometry']).representative_point()
        match = {'source_file': str(path.relative_to(ROOT)), 'source_id': f['properties']['source_id'],
            'name': name, 'fclass': f['properties'].get('fclass'),
            'representative_point_crs84': [point.x, point.y], 'point_is_entrance': False}
        matches.append(match)
        anchor_points.append({'type': 'Feature', 'geometry': mapping(point), 'properties': {
            'anchor_id': f'HKL-{idx:03}', 'label': label, **match}})
    anchor_rows.append({'id': f'HKL-{idx:03}', 'label': label, 'district': district,
        'status': 'snapshot_matches_need_review' if matches else 'no_match_not_proof_of_absence',
        'matches': matches})
save('landmark-evidence.json', {'snapshot': '2026-10-22', 'groups': anchor_rows,
    'note': 'Name matches are not verified placements; multiple layers can represent the same place. Label points are not entrances.'})
save('landmark-evidence.geojson', {'type': 'FeatureCollection', 'features': anchor_points})

counts = collections.Counter(c for f in handler.features for c in f['properties']['categories'])
inventory = {}
for path in files:
    fs = json.loads(path.read_text())['features']
    inventory[path.stem] = dict(collections.Counter(f['properties'].get('fclass', 'unknown') for f in fs))
catalog = json.loads((SOURCE / 'catalog/downloads.json').read_text())
reference_entries = [r for r in catalog if r.get('status') == 'downloaded' and
    (r.get('path', '').startswith('raw/engineering/') or r.get('path', '').startswith('raw/environment/'))]
save('reference-index.json', {'upstream_root': str(SOURCE), 'entries': reference_entries,
    'note': 'Index only: source PDFs not copied or individually revalidated in this extension.'})
inputs = [PBF, DATA / 'manifest.json', SOURCE / 'catalog/downloads.json'] + files
manifest = {'created': '2026-11-01', 'snapshot': '2026-10-22', 'bbox_crs84': BBOX,
    'source_read_only': True, 'source_sha256': {str(p): sha(p) for p in inputs},
    'node_count': len(handler.features), 'category_counts': dict(counts),
    'rail_entrance_candidates': len(entrances), 'landmark_groups': len(groups),
    'landmark_groups_with_matches': sum(bool(r['matches']) for r in anchor_rows),
    'existing_layer_inventory': inventory,
    'limitations': ['node-only PBF extraction: ways and relations are not included',
        'OSM completeness and current existence are not verified',
        'rail entrance station assignment and exit codes require MTR PDF cross-check',
        'counts are source features, not independent physical assets or game content targets',
        'window is a study rectangle, not an administrative district',
        'no NPC, merchant schedule, live traffic, audio or photographic evidence extracted'],
    'outputs': {p.name: sha(p) for p in sorted(OUT.iterdir()) if p.suffix in ['.json', '.geojson'] and p.name != 'manifest.json'}}
save('manifest.json', manifest)
print(json.dumps({k: manifest[k] for k in ['node_count', 'category_counts', 'rail_entrance_candidates', 'landmark_groups', 'landmark_groups_with_matches']}, ensure_ascii=False))
