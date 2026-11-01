# 广东道 demo 实机验证 · 2026-11-01

Unity 6000.3.27f1、URP 17.3.0；macOS 独立 Development Player，Apple M5，1920×1080、VSync=1，默认 1280 宽像素处理。编译与构建 0 错误、0 警告，构建耗时 9.212 秒。MIT 通知打包到 StreamingAssets。

- [构建记录](build.json)
- [最终行走结果](result.json)：12/12 检查点，216.60 游戏米，57.92 秒；公交 4 轮，未开门上客计数 0，掉落恢复 0，运行时捕获异常 0。
- [输入回归](input-result.json)：17 项通过，覆盖四向移动／帧选择、停步、暂停、昼夜、DOF 和 P 档位；是 Input System 虚拟键盘，并非物理键盘 UI 认证。测试克隆 InputSettings 允许后台输入，结束恢复，不更改正式输入配置。
- 帧时 P95 17.46 ms，P99 17.67 ms；Unity allocated memory 115.7 MiB，不是 RSS。**未达到严格 P95≤16.7 ms，且非十分钟稳定性或美术验收。** 截图采集与切换包含在短程结果内，未以此证明像素 pass 加速。

## 截图

[默认日景](start.png) · [纵深支路](route-3.png) · [主街端点](route-5.png) · [日景 DOF](day-dof.png) · [夜景 DOF](night-dof.png)

同一固定机位比较：[原生](pixel-mode-0.png) · [1280](pixel-mode-1.png) · [960](pixel-mode-2.png) · [640](pixel-mode-3.png)。NPC／车辆仍在运动，因此并非逐像素冻结场景对照；世界路牌经过像素化，HUD 保持原生。1280 可读性较好，作为此次默认；640 只作极端比较。景深开关已有视觉输出，但精美程度尚不达标。

## 本批修复与未覆盖项

修复了上游 shader 的等边 smoothstep 除零警告、RenderGraph 的显式深度／法线／描边依赖、路牌遮住支路角色，以及人行道缺碰撞导致脚底略微下沉（最终 root Y≈0.195）。公交与行人随暂停停止，公交队列固定复用一人。

选择直接复用 pass 生命周期和 HFSM，未导入正交控制器、第三方演示美术或旧 Z 排序。未测试多游戏相机、分屏、XR、Windows、长时间帧时和精修动画。Development Player 退出时仍有运行库线程／分配诊断，本次退出码 0；不能用短程结果宣称不存在长期内存问题。

人群／建筑／车辆仍是原型，压缩布局与真实数据的差异见 [场景说明](../README.md)，密度与素材待办见 [精修规划](../polish-plan.md)。
