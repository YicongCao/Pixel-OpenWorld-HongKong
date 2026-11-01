#!/usr/bin/env python3
"""Inspect generated atlases, record slice metadata, and copy unchanged PNGs into Unity."""
import hashlib
import json
import shutil
from pathlib import Path
from PIL import Image

ROOT=Path(__file__).resolve().parents[1]
ASSETS=ROOT/'assets/blocks/tst-nathan-haiphong/generated'
DEST=ROOT/'unity/HongKong/Assets/HongKong/Generated/Textures'

def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()

def main():
    DEST.mkdir(parents=True,exist_ok=True)
    hero=ASSETS/'hero/white-hat-sheet-v1.png'
    image=Image.open(hero)
    assert image.mode=='RGBA'
    alpha=image.getchannel('A')
    frames=[]
    for row,direction in enumerate(['front','right','back','left']):
        for col in range(7):
            x0,y0,x1,y1=[round(v) for v in (col*image.width/7,row*image.height/4,(col+1)*image.width/7,(row+1)*image.height/4)]
            bounds=alpha.crop((x0,y0,x1,y1)).point(lambda a:255 if a>127 else 0).getbbox()
            assert bounds is not None
            left,top,right,bottom=bounds
            frames.append(dict(direction=direction,column=col,
                rect=dict(x=x0+left,y=image.height-(y0+bottom),width=right-left,height=bottom-top),
                footAnchorSourcePx=[x0+(left+right)/2,y0+bottom],durationMs=120 if col else 500))
    doc=dict(columns=7,rows=4,width=image.width,height=image.height,frames=frames,
             directions=['front','right','back','left'],worldHeight=1.9,
             source_sha256=sha(hero),status='generated_animation_sample_requires_art_review',
             notes=['Grid requested dimensions were not respected; metadata uses actual image dimensions and inspected alpha bounds.',
                    'No pixels edited. Runtime uses frame UV rectangles, direction-based height normalization and bottom anchors.',
                    'Row 0 idle height 177 px; side-view proportions differ. Generated rim pixels and gait need visual review.'])
    for p in [ASSETS/'hero/frames.json',DEST/'hero-frames.json']:
        p.write_text(json.dumps(doc,ensure_ascii=False,indent=2)+'\n')
    shutil.copyfile(hero,DEST/'white-hat-sheet-v1.png')
    texture=ASSETS/'materials/surfaces-v1.png';shutil.copyfile(texture,DEST/'surfaces-v1.png')
    for folder,png,inputs,notes in [
        ('hero',hero,['assets/blocks/tst-nathan-haiphong/generated/concepts/day-v2.png'],doc['notes']),
        ('materials',texture,['references/blocks/tst-nathan-haiphong/a1-2024-1280.jpg','assets/blocks/tst-nathan-haiphong/generated/concepts/day-v2.png'],
         ['8 material swatches in 4x2 grid; generated image is not a surveyed facade.',
          'Exact border seamlessness not guaranteed; Unity extracts inner rectangles and tests repeat sampling.',
          'Material swatch positions: paving, plaster, terracotta, asphalt / glass, stone, green metal, foliage.'])]:
        im=Image.open(png)
        manifest=dict(tool='image_gen.imagegen',date='2026-11-01',file=png.name,sha256=sha(png),size=list(im.size),
            prompt='prompt.txt',prompt_sha256=sha(ASSETS/folder/'prompt.txt'),
            inputs=[dict(path=p,sha256=sha(ROOT/p)) for p in inputs],
            status='production_sample_not_final_art',notes=notes)
        (ASSETS/folder/'manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2)+'\n')
    print(json.dumps({'frames':len(frames),'hero_size':image.size,'pixel_edits':False,'copies_verified':sha(hero)==sha(DEST/hero.name)}))

if __name__=='__main__':main()
