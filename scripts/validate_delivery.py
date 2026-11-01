#!/usr/bin/env python3
"""Validate the extracted research pack, not game runtime functionality."""
import hashlib
import json
from pathlib import Path
import pyogrio
from shapely import from_wkb, intersects
from shapely.geometry import box
from pypdf import PdfReader
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / 'data/hong-kong'
manifest = json.loads((OUT / 'manifest.json').read_text())
errors, observations = [], []

def sha(path):
    h = hashlib.sha256()
    with path.open('rb') as f:
        for block in iter(lambda: f.read(1024 * 1024), b''): h.update(block)
    return h.hexdigest()

for rel, info in manifest['outputs'].items():
    path = OUT / rel
    if not path.is_file() or sha(path) != info['sha256']: errors.append(f'output hash: {rel}')
    if path.suffix in ['.json', '.geojson']: json.loads(path.read_text())

src = Path(manifest['source_root'])
for rel, info in manifest['sources'].items():
    path = src / rel
    if path.is_dir():
        files = [{'path':str(p.relative_to(path)), 'sha256':sha(p)} for p in sorted(path.rglob('*')) if p.is_file()]
        digest = hashlib.sha256(json.dumps(files, sort_keys=True).encode()).hexdigest()
    else: digest = sha(path)
    if digest != info['sha256']: errors.append(f'source hash: {rel}')

data = json.loads((OUT / 'pedestrian/paths-hk1980.json').read_text())
_, _, geoms, cols = pyogrio.raw.read(src / 'derived/pedestrian/3DPN_P2.gdb',
    layer='PedestrianRoute', columns=['PedestrianRouteID'])
mask = intersects(from_wkb(geoms), box(*data['selection_bbox']))
expected = {int(x) for x in cols[0][mask]}
actual = {r['properties']['PedestrianRouteID'] for r in data['paths']}
spatial = {'checked':'2026-11-01', 'check':'full source scan without spatial-index filtering',
    'full_source_count':len(geoms), 'expected_count':len(expected), 'actual_count':len(actual),
    'missing_ids':sorted(expected-actual), 'extra_ids':sorted(actual-expected), 'passed':expected == actual,
    'note':'Independent check for the GDAL FGDB spatial index warning during extraction.'}
(OUT / 'pedestrian/spatial-selection-validation.json').write_text(json.dumps(spatial, indent=2))
if expected != actual: errors.append('pedestrian selection mismatch')
if len(actual) != len(data['paths']): errors.append('duplicate pedestrian IDs')

for mode in ['bus', 'gmb', 'ferry']:
    d = json.loads((OUT / f'transport/{mode}.json').read_text())
    stops = {r['STOP_ID'] for r in d['stops']}
    routes = {r['ROUTE_ID'] for r in d['routes_serving_region']}
    for r in d['route_stops_in_region']:
        if r['STOP_ID'] not in stops or r['ROUTE_ID'] not in routes: errors.append(f'{mode} foreign key')
    observations.append({'mode':mode,'stops':len(stops),'route_records':len(d['routes_serving_region'])})

pdfs = list((OUT / 'mtr').glob('*.pdf'))
pages = {p.name:len(PdfReader(p).pages) for p in pdfs}
if len(pdfs) != 16 or any(n < 1 for n in pages.values()): errors.append('MTR PDF count/readability')
concepts = []
for p in sorted((ROOT / 'assets/concepts').glob('*.png')):
    with Image.open(p) as im:
        im.verify()
    with Image.open(p) as im:
        concepts.append({'file':p.name,'width':im.width,'height':im.height,'mode':im.mode,'sha256':sha(p)})
if len(concepts) != 4: errors.append('concept count')
prompts = json.loads((ROOT / 'assets/concepts/prompts.json').read_text())
if {p['id'] for p in prompts['prompts']} != {Path(p['file']).stem for p in concepts}: errors.append('prompt/file mismatch')
report = {'checked':'2026-11-01','passed':not errors,'errors':errors,'source_hashes_checked':len(manifest['sources']),
    'output_hashes_checked':len(manifest['outputs']),'pedestrian':spatial,'transport':observations,
    'mtr_pdf_pages':pages,'concepts':concepts,
    'not_tested':['gameplay','game performance','current storefronts','pixel-perfect production assets','legal clearance']}
(OUT / 'validation.json').write_text(json.dumps(report,ensure_ascii=False,indent=2))
(ROOT / 'assets/concepts/manifest.json').write_text(json.dumps({'created':'2026-11-01','tool':'built-in image_gen',
    'status':'art direction concepts; not runtime assets','prompts':'prompts.json','images':concepts},ensure_ascii=False,indent=2))
print(json.dumps({'passed':not errors,'errors':errors,'sources':len(manifest['sources']),
    'outputs':len(manifest['outputs']),'pedestrian':len(actual),'pdfs':len(pdfs),'concepts':len(concepts)},ensure_ascii=False))
raise SystemExit(bool(errors))
