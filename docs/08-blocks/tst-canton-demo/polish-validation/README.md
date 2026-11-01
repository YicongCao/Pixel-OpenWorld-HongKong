# 30 米精修验证证据

2026-11-01 · Unity 6000.3.27f1 / URP 17.3.0 / Apple M5 / 1920×1080 Development Player。

- [当前起点实机](start.png)／[景深开启](day-dof.png)／[纵深支路](route-3.png)。支路仍以原型体块为主，不属于本批 30 米立面精修范围。
- [原生渲染](pixel-mode-0.png)／[960 宽像素档](pixel-mode-2.png)，默认 1280 宽；不是缩低基础场景几何渲染成本。
- [第一轮烘焙前画面](before-bake-first-iteration.png)：同机位与 49 米镜头，NPC、公交时刻不同；后续还调整了店内光强、招牌和 NPC 图集。因此与最终截图是迭代对比，不能作为只改变烘焙一个变量的严格 A/B。
- [输入检查](input-result.json)：17 项通过；包括实际 Input System 方向、停步、暂停、锁定日景、DOF 和像素档位。
- [构建摘要](build.json)：独立 macOS Development 构建；[烘焙审计](bake-audit.json)／[烘焙产物](bake-result.json) 记录静态、动态、UV2、贴图与探针。

图件均为真实运行画面，不是 imagegen 概念图。输入验证是虚拟键盘驱动实际 InputAction／CharacterController，不是物理键盘和全部 UI 验收；几何乘客、重复人流、部分招牌／透明边缘仍是已知美术缺口。没有本批完整路线录像、十分钟稳定性与主观画质验收。

[独立 Player 结果](result.json)：180.26 秒，含 120 秒静止观察；12/12 路线点，公交 12 轮、交付 8 次，无运行错误／上车违规／掉落恢复。P95=17.29 ms、P99=17.65 ms；Unity allocated memory=148.13 MiB，不是 RSS，未达到 P95≤16.7 ms。

[观察中段](observation-middle.png) · [观察结束](observation-end.png) · [运行日志摘要](runtime-log-summary.txt) · [证据哈希](manifest.json)。完整本机 Player.log 保留在忽略目录 `.local/canton-polish/player/`；证据清单记录其哈希。
