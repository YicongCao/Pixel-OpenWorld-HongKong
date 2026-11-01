# Apple 橱窗样板验证 · 2026-11-01

Unity 6000.3.27f1，Apple M5，1920×1080 独立 Development Player。场景为 `Authored/CantonPolish/CantonRoadPolish.unity`；本机 `.local/builds/HongKong-Canton-Polish.app` 已更新。

## 结果

- [构建](build.json)：Succeeded，9.26 秒，0 错误／0 警告。Editor 最终 groundTruth.consoleErrors=0，compilationFailed=false。
- [独立 Player](result.json)：68.06 秒，原路线 12/12，新增 Apple 左／中／右／返回 4/4，累计行走 242.33 米；公交 4 轮、交付 3 次，0 错误、0 掉落、0 上客违规。
- [日志摘录](runtime-log-summary.txt)：1 组 lightmap、294 个 Light Probes 在 Player 加载，局部 reflection.bakedTexture 存在。未仅凭 Editor 预览判定加载成功。
- [烘焙检查](bake-audit.json)：397 个对象接入 lightmap，动态对象接入数 0，静态缺 UV2 数 0；[烘焙路径](bake-result.json)与场景、贴图一同入库。
- 帧时 P95=17.27 ms、P99=17.67 ms；Unity allocated memory 148.42 MiB（不是 RSS）。短程巡检，不是十分钟性能验收；仍未达 P95≤16.7 ms，不把本轮与前一轮小幅差异解释成稳定性能提升。

## 画面检查

已实际查看 [同起点](start.png)、[左侧](apple-bay-0.png)、[正面](apple-bay-1.png)、[右侧](apple-bay-2.png)、[景深开启](apple-bay-dof.png)。返回静帧另存 [第四点](apple-bay-3.png)，像素档对照为 [原生](pixel-mode-0.png)和[960 宽](pixel-mode-2.png)。

观察：原相机下，样板的柜、桌、设备与背板可区分，关闭 DOF 后仍能读出浅室内层次；左右机位没有看到明显的透明面前后颠倒，白帽和人行道保持可读。最终玻璃保留的是低强度静态反射，不是动态街景镜面。未做连续视频级闪烁检查；单帧通过不代表所有透明／景深边缘或运动画质已验收。

[改动前同机位](../brand-validation/start.png)与[旧目标概念对照](../concept-comparison.html)保持历史版本。看整张新截图时，中间一跨以外的粗糙部分仍然存在；本轮没有扩产到整店。Apple 大标志会遮挡上层中部陈列，这是目前构图中保留的实际遮挡，不通过透视作弊消除。

运行参数：`-streetSmokeTest -streetAppleBay -streetOutput <绝对输出目录>`。新增巡检为测试 opt-in，正常玩家启动不会自动走路。Player 完成后正常退出；Editor 已停止 Play，并恢复 auto-tick／run-in-background 临时覆盖设置。

[证据 SHA256](manifest.json) · [实施与局限](../apple-bay-v1.md)。
