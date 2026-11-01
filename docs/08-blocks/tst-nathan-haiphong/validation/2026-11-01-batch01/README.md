# 批次 01 · 实机证据

2026-11-01，Unity 6000.3.27f1、URP 17.3.0、Apple M5。本目录是实际运行结果；照片／imagegen 概念在各自来源目录，不混作实机证据。

## 条件与文件

- [最终构建记录](build.json)：34 米镜头、Development Mac Player，包含 x86_64／arm64，实际只在本机 Apple Silicon 测试；0 构建错误、0 构建警告。保留基准 commit、关键源文件和可执行文件 SHA-256。最终提交通过本目录的 Git 历史定位，不在文件中制造自引用 commit 哈希。
- [灰盒 Editor](graybox-editor.json)／[灰盒独立 Player](graybox-player.json)：约 179 米源路线，25 点往返共 50 个检查点；实际路程另包含出生点到北端的起步段。
- [街角 Editor 路线](art-editor.json)：43 米镜头，50 点通过、无坠落恢复；距离含此前输入测试约 2.7 米，不能与独立测试距离直接比较。
- [输入验证](input-editor.json)：11 项通过。通过虚拟键盘走实际 InputAction／CharacterController，覆盖 D、Esc、N、F、暂停时位置不变、边界与有效图集；不声称测试了所有物理按键或所有 UI 按钮。
- [最终街角独立 Player](art-player.json)：34 米镜头，1080p、PC 质量、VSync=1，短程往返；包括开始时昼夜／景深截图开销。50/50 检查点通过，坠落恢复 0、运行错误 0；109.62 秒实际移动 427.61 米。P95 17.51 ms、P99 22.75 ms，Unity 分配内存 113.07 MiB（不是进程 RSS）。详见 JSON 和 [运行日志节选](player-log-excerpt.txt)。
- [初始宽镜头结果](wide-camera/art-player.json) 与 [对应构建](wide-camera/build.json)：保留用户反馈前的 43 米版本；不将它的截图当作最终交付镜头。

## 原始游戏画面

以下为 Player 自身截图，包含 HUD、操作提示和 Development Build 标识；没有后期改画或裁图。

- [白天，景深关](player-day.png)
- [白天，景深开](player-day-dof.png)
- [夜晚，景深开](player-night-dof.png)
- [海防道支路端点](player-branch.png)
- [弥敦道北端](player-north.png)

最终镜头比反馈前更近，主角投影约增大四分之一；仍保留海防道纵深。用户只确认了“适度拉近”的方向，未确认最终画面质量。Bokeh 已持久化并在独立构建加载，焦点跟随稳定目标深度；强度及整体 HD-2D 风格还需打磨。

## 检查边界与问题

这组结果支持“路线能走、控制可用、资产和渲染配置能进入独立程序”，不支持“完整 v0.1 已完成”。正式合同的 12 名 NPC／车流／巴士／活动负载尚不存在；本批运行约两分钟，没有做 10 分钟稳定性、存档恢复、跨层测试或正式性能验收。

宽镜头短测 P95 17.48 ms、P99 21.20 ms；即使多数帧接近 60 FPS，也不能称已达到计划的 P95 ≤16.7 ms 门槛。最终近镜头数据另列，禁止混用灰盒高速帧时或 Editor 低分辨率结果代表最终性能。

Editor 相机截图曾出现两条 Metal memoryless depth load/store 警告；修复后没有游戏脚本运行错误。字体重复警告与丢失的 Volume 子资产已修复；中文世界标识在本机 Player 可见，HUD 暂用英文，跨平台字体仍是交付缺口。

仍需修：背景占位、路缘／路面断层及局部前坪接缝、站口细节尺度、树冠形状、立面纹理重复、角色边缘及步态、完整遮挡／真实阴影。候车雨篷和 KMB 字样是展示占位，不是准确站序或运行公交。站口视觉台阶尚未成为玩家可进入的站内空间。

验证结束后自动测试 Player 退出；Editor 留在 Edit mode，关闭本轮 auto-tick 和后台运行覆盖，保留项目自身 run-in-background 设置。
