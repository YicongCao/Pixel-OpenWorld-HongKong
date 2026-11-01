# 品牌修订验证 · 2026-11-01

场景 `Authored/CantonPolish/CantonRoadPolish.unity`；Unity 6000.3.27f1、Apple M5、StandaloneOSX Development Player，1920×1080。本机产物 `.local/builds/HongKong-Canton-Polish.app`。

- [构建](build.json)：5.08 秒，0 错误、0 警告。
- [行走结果](result.json)：59.03 秒，12/12 路线点，216.60 米累计行走，公交 4 轮、交付 3 次，0 运行错误、0 掉落、0 上客违规。沿用 CharacterController 的自动意图测试；本批没有重跑键盘输入测试。
- [烘焙检查](bake-audit.json)：382 个 lightmapped 对象，动态 lightmapped=0，静态缺 UV2=0；[烘焙产物](bake-result.json)为 1 组 lightmap 和 258 个探针，[Player 日志](runtime-log-summary.txt)确认成功加载。
- 帧时 P95=17.51 ms，P99=17.74 ms，Unity allocated memory 148.10 MiB（非 RSS）。这是短程回归，不是十分钟性能或最终美术验收，不声称达到 P95≤16.7 ms。

已实际查看 [起点](start.png)、[景深开启](day-dof.png)、[支路](route-3.png)。起点 Apple 与莎莎标志完整可见；修正了首次 Editor 预览中的树冠遮挡，品牌标志在景深开启时仍可辨识。保留 [原生档位](pixel-mode-0.png)、[960 宽档位](pixel-mode-2.png)与[返回](returned.png)截图供对照。店内商品仍是简化体块，浅室内与旧铺贴图仍需精修。

[证据哈希](manifest.json) · [实施与官方来源](../brands-v1.md)。图像是独立 Player 实机捕获，没有重绘或后期补标志。上一批 `polish-validation` 保留不覆盖。
