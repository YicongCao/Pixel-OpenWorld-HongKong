"""Build an offline, self-contained comparison without altering either source image."""
from pathlib import Path
import base64

ROOT = Path(__file__).resolve().parents[1]
CURRENT = ROOT / 'docs/08-blocks/tst-canton-demo/brand-validation/start.png'
CONCEPT = ROOT / 'assets/blocks/tst-canton-demo/generated/concepts/current-target-v1/concept.png'
OUTPUT = ROOT / 'docs/08-blocks/tst-canton-demo/concept-comparison.html'

def data_url(path):
    return 'data:image/png;base64,' + base64.b64encode(path.read_bytes()).decode('ascii')

PAGE = '''<!doctype html>
<html lang="zh-Hans"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>广东道 · 实机与 HD-2D 目标</title>
<style>
:root{color-scheme:dark;font:16px/1.65 system-ui,-apple-system,sans-serif;background:#121b20;color:#e7eee9}
*{box-sizing:border-box}body{margin:0}main{max-width:1500px;margin:auto;padding:28px 24px 56px}
h1{font-size:clamp(25px,4vw,40px);line-height:1.2;margin:10px 0 16px}h2{font-size:19px;margin:0 0 8px}
p{margin:8px 0 16px}.muted{color:#b4c2c4}.eyebrow{color:#b5c99e;letter-spacing:.13em;font-size:12px}
nav{display:flex;gap:8px;flex-wrap:wrap;margin:20px 0 12px}button{font:inherit;padding:8px 18px;border:1px solid #52656a;border-radius:7px;background:#233239;color:inherit;cursor:pointer}
button[aria-pressed=true]{background:#c6d5b4;color:#17241e;border-color:#c6d5b4}button:focus-visible,input:focus-visible{outline:3px solid #eccb79;outline-offset:3px}
.stage{position:relative;aspect-ratio:16/9;background:#091216;overflow:hidden;border:1px solid #42545c;border-radius:7px}
.stage img{position:absolute;width:100%;height:100%;object-fit:contain;inset:0}.target{clip-path:inset(0 0 0 var(--split,50%))}
.line{position:absolute;left:var(--split,50%);height:100%;border-left:2px solid #e8dba8;pointer-events:none}
.tags{position:absolute;left:12px;right:12px;top:12px;display:flex;justify-content:space-between;pointer-events:none}.tags span{padding:4px 10px;background:#111d24db;border-radius:4px;font-size:13px}
.slider{display:flex;align-items:center;gap:16px;margin:12px 0}input{flex:1;accent-color:#c6d5b4;min-width:60px}output{min-width:45px}a{color:#c6d5b4}
.cards{display:grid;grid-template-columns:repeat(3,1fr);gap:16px;margin:24px 0}.card{padding:20px;border:1px solid #35474e;background:#1a282f;border-radius:8px}.card p{margin:0}
.note{border-left:3px solid #d8ba7b;padding:8px 18px;color:#cfdbdc}.small{font-size:13px}footer{border-top:1px solid #35474e;margin-top:26px;padding-top:18px}
@media(max-width:700px){main{padding:20px 12px}.cards{grid-template-columns:1fr}.tags span{font-size:11px;padding:2px 5px}.slider{gap:8px}}
</style>
<main><div class="eyebrow">香港像素漫游 / CANTON ROAD / 2026.11.01</div>
<h1>同一个街段，离目标画面还差什么？</h1>
<p class="muted">当前可玩版本与 imagegen 概念。主街、店铺顺序、白帽位置与午后时段保持近似一致；概念不是新实机。</p>
<nav aria-label="比较模式"><button data-mode="compare" aria-pressed="true">拖动对照</button><button data-mode="current" aria-pressed="false">当前实机</button><button data-mode="concept" aria-pressed="false">目标概念</button></nav>
<div class="stage" id="stage"><img src="__CURRENT__" alt="当前独立 Player：Apple、莎莎与白帽主角"><img class="target" id="target" src="__CONCEPT__" alt="相同街段的生成 HD-2D 美术目标"><div class="line" id="line"></div><div class="tags"><span id="currentTag">实机 · 5b912df</span><span id="conceptTag">概念 · 未实现</span></div></div>
<label class="slider" id="sliderRow"><span>分界位置</span><input id="split" type="range" min="0" max="100" value="50" aria-label="实机与概念的分界百分比"><output id="value" for="split">50%</output></label>
<p class="small muted">原图完整显示，无裁切、补绘或几何配准。生成图的楼层基线、人物大小、门楣宽度略有漂移；拖动用于视觉对照，不是逐像素误差检测。键盘可切换按钮并用方向键调整分界。</p>
<div class="cards"><article class="card"><h2>已有：空间骨架</h2><p>横向行走、前后遮挡、白帽与品牌锚点已经成立。当前版本还有真正运行的公交、搬运和光照烘焙。</p></article><article class="card"><h2>最大差距：材质与层次</h2><p>Apple 的玻璃、柱、陈列和后墙尚未充分分离；店铺商品与入口仍偏占位。优先完成一个可见商铺模块。</p></article><article class="card"><h2>最后处理：光影与景深</h2><p>材质、像素簇和几何稳定后，再重烘与调焦点。概念的亮灯牌和平滑反射也需要克制。</p></article></div>
<div class="note">不把差距换算成虚假的完成百分比。目标图只提供静态美术方向；动作、通行、透明深度、帧时仍需要在 Unity 中逐项验证。</div>
<footer><a href="concept-gap-v1.md">详细差距与下一轮工序</a> · <a href="brands-v1.md">商户核对与来源</a><p class="small muted">本页两张图均内嵌，可离线打开。概念输入：当前 Player 截图＋Apple 官方外观＋莎莎官方店面；构图沿用压缩场景，不代表精确测绘。没有新增巷道、坡道或港铁口。</p></footer>
</main><script>
const stage=document.getElementById('stage'),range=document.getElementById('split'),target=document.getElementById('target');
const line=document.getElementById('line'),row=document.getElementById('sliderRow'),value=document.getElementById('value');
const ct=document.getElementById('currentTag'),gt=document.getElementById('conceptTag');
function update(){stage.style.setProperty('--split',range.value+'%');value.textContent=range.value+'%'}
range.addEventListener('input',update);
document.querySelectorAll('button[data-mode]').forEach(b=>b.addEventListener('click',()=>{
 document.querySelectorAll('button[data-mode]').forEach(x=>x.setAttribute('aria-pressed',String(x===b)));
 const m=b.dataset.mode;row.hidden=m!=='compare';row.style.display=m==='compare'?'flex':'none';line.hidden=m!=='compare';
 target.style.clipPath=m==='concept'?'none':m==='current'?'inset(0 0 0 100%)':'';
 ct.style.visibility=m==='concept'?'hidden':'visible';gt.style.visibility=m==='current'?'hidden':'visible';
}));update();
</script></html>'''

OUTPUT.write_text(PAGE.replace('__CURRENT__', data_url(CURRENT)).replace('__CONCEPT__', data_url(CONCEPT)), encoding='utf-8')
print(f'Wrote {OUTPUT.relative_to(ROOT)} ({OUTPUT.stat().st_size} bytes)')
