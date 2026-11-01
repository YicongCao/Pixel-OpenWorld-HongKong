#!/usr/bin/env python3
"""Validate chapter structure, offline links and preserved research evidence."""
import base64
import hashlib
from html.parser import HTMLParser
import json
from pathlib import Path
import re
from urllib.parse import unquote, urlsplit

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / 'docs'
REPORT = DOCS / '07-validation/docs-validation.json'
errors = []
checks = {}

def check(condition, message):
    if not condition:
        errors.append(message)

def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

catalog = json.loads((ROOT / 'scripts/docs_catalog.json').read_text())
check({p.name for p in DOCS.iterdir() if p.is_file()} == {'README.md', 'Agent.md', 'Skill.md'}, 'docs root must contain only the three reading entry files')
check({p.name for p in DOCS.iterdir() if p.is_dir()} == {c['id'] for c in catalog}, 'chapter directory/catalog mismatch')
check(len(catalog) == 8 and len({c['id'] for c in catalog}) == 8, 'expected eight unique chapters')
for c in catalog:
    for name in c['documents']:
        check((DOCS / c['id'] / name).is_file(), f'missing chapter document: {c["id"]}/{name}')
    check((DOCS / c['id'] / 'index.html').is_file(), f'missing chapter page: {c["id"]}')

class Page(HTMLParser):
    def __init__(self):
        super().__init__()
        self.ids, self.links, self.images, self.current = [], [], [], []
    def handle_starttag(self, tag, attrs):
        attrs = dict(attrs)
        if 'id' in attrs:
            self.ids.append(attrs['id'])
        if tag == 'a' and 'href' in attrs:
            self.links.append(attrs['href'])
            if attrs.get('aria-current') == 'page':
                self.current.append(attrs['href'])
        if tag == 'img':
            self.images.append(attrs)

pages = {}
for c in catalog:
    path = DOCS / c['id'] / 'index.html'
    if not path.exists():
        continue
    page = Page()
    page.feed(path.read_text())
    pages[path.resolve()] = page
    check(len(page.ids) == len(set(page.ids)), f'duplicate HTML IDs: {path}')
    check(len(page.current) == 1, f'current chapter marker: {path}')
    check(('geo-map' in page.ids) == (c['id'] == '02-geography'), f'map belongs only to geography: {path}')
    for image in page.images:
        check(image.get('width') and image.get('height') and image.get('alt'), f'image dimensions/alt: {path}')
        check(image.get('src', '').startswith('data:image/png;base64,'), f'non-embedded image: {path}')

link_count = 0

def validate_link(source, url):
    global link_count
    parsed = urlsplit(url)
    if parsed.scheme or parsed.netloc:
        return
    link_count += 1
    target = (source.parent / unquote(parsed.path)).resolve() if parsed.path else source.resolve()
    check(target == REPORT or target.exists(), f'broken local link: {source.relative_to(ROOT)} -> {url}')
    if parsed.fragment and target in pages:
        check(unquote(parsed.fragment) in pages[target].ids, f'broken HTML anchor: {source.relative_to(ROOT)} -> {url}')

md_files = list(DOCS.rglob('*.md')) + [ROOT / n for n in ('README.md', 'AGENTS.md', 'agent.md', 'data/hong-kong/README.md', 'data/hong-kong/hd2d-study/README.md')]
for path in md_files:
    text = re.sub(r'```.*?```', '', path.read_text(), flags=re.S)
    for url in re.findall(r'!?\[[^\]]*\]\(([^)]+)\)', text):
        validate_link(path, url)
for path, page in pages.items():
    for url in page.links:
        validate_link(path, url)

# Source and concept files were reorganized by reference, not rewritten.
original = json.loads((ROOT / 'data/hong-kong/manifest.json').read_text())
for name, entry in original['outputs'].items():
    path = ROOT / 'data/hong-kong' / name
    check(path.is_file() and digest(path) == entry['sha256'], f'original data changed: {name}')
for sub in ('street-life', 'hd2d-study'):
    manifest = json.loads((ROOT / f'data/hong-kong/{sub}/manifest.json').read_text())
    for name, entry in manifest['outputs'].items():
        path = ROOT / name if sub == 'hd2d-study' else ROOT / f'data/hong-kong/{sub}' / name
        expected = entry if isinstance(entry, str) else entry['sha256']
        check(path.is_file() and digest(path) == expected, f'derived output hash: {path}')
migration = json.loads((DOCS / '07-validation/migration-map.json').read_text())
source_urls = set(migration['external_source_urls'])
current_urls = set(re.findall(r'\]\((https?://[^)]+)\)', '\n'.join(p.read_text() for p in DOCS.rglob('*.md'))))
check(source_urls <= current_urls, 'original external source URLs were lost')
for item in migration['moved_files']:
    path = ROOT / item['to']
    check(path.is_file() and digest(path) == item['sha256'], f'migrated evidence bytes changed: {path}')
for item in migration['sections']:
    check((ROOT / item['target']).exists(), f'migration destination absent: {item["target"]}')

expected_images = {digest(p) for p in (ROOT / 'assets/concepts').rglob('*.png')}
expected_images.add(digest(DOCS / '03-hd2d/figures/corridor-projection.png'))
actual_images = []
for page in pages.values():
    for image in page.images:
        if image.get('src', '').startswith('data:image/png;base64,'):
            actual_images.append(hashlib.sha256(base64.b64decode(image['src'].split(',', 1)[1])).hexdigest())
check(set(actual_images) == expected_images and len(actual_images) == len(expected_images) == 6, 'concept/diagram embedding mismatch or duplication')
check('docs/Agent.md' in (ROOT / 'AGENTS.md').read_text() and 'docs/Skill.md' in (ROOT / 'AGENTS.md').read_text(), 'root AGENTS must explicitly route to docs guidance')
# Future rebuilds must not resurrect the old mixed document tree.
for name in ('build_report.py', 'build_hd2d_study.py'):
    text = (ROOT / 'scripts' / name).read_text()
    check('docs/research' not in text and '香港像素漫游策划案.html' not in text, f'obsolete output path: {name}')

checks.update(chapters=len(catalog), markdown_files=len(md_files), local_links=link_count,
              embedded_images=len(actual_images), original_data_hashes=len(original['outputs']),
              migrated_files=len(migration['moved_files']), external_sources_preserved=len(source_urls))
result = {'scope': 'documentation structure, offline references and preserved data; not gameplay',
          'passed': not errors, 'checks': checks, 'errors': errors,
          'html_sha256': {str(p.relative_to(ROOT)): digest(p) for p in pages}}
REPORT.parent.mkdir(parents=True, exist_ok=True)
REPORT.write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n')
print(json.dumps({'passed': not errors, 'checks': checks, 'errors': errors}, ensure_ascii=False))
raise SystemExit(bool(errors))
