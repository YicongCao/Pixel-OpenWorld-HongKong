# 广东道 30 米精修 · 第 01 批

后续修订：[品牌店招第 01 批](brands-v1.md) 已替换本页所述泛称招牌；本页保留当批实现与测试记录。

2026-11-01。按用户要求继续精修一个 20–30 米小场景，并实际执行光照烘焙。美术工作集中在设计坐标 X=-20…10 的 30 米临街片段；保留原 68.1 米可走空间与海防道连接作为回归路线。不是新增一段精确测绘街道，也没有把 Apple 与真实旧铺的邻接关系认定为事实。

## 本次产物与入口

- Unity：`Assets/HongKong/Authored/CantonPolish/CantonRoadPolish.unity`。原 `Generated/CantonDemo/CantonRoadDemo.unity` 保留。
- 本机独立 app：`.local/builds/HongKong-Canton-Polish.app`。WASD／方向键行走，Shift 快走，Esc 暂停／重置／退出，F 景深，P 像素密度。本场景固定午后烘焙，N 不切夜景。
- [生成源／提示词／哈希](../../../assets/blocks/tst-canton-demo/generated/polish-v1/manifest.json)；Unity 导入副本 `Generated/CantonPolish/Textures`；采用的场景、材质、UV2 网格、光照数据放 `Authored/CantonPolish`。
- [原始待办](polish-plan.md) 保留完整质量门槛；本批只关闭有证据的子项，不把整张清单标完成。

## 改了什么

Apple 从不透明色块改成前玻璃、两层浅室内、独立细柱、夹层、桌面／设备、灯带、楼梯轮廓和侧返。只做稳定探索镜头及路线需要的可见体积；没有制作可进入商店、完整真实楼梯或精确商标。参考 [Apple 官方广东道店照片](https://www.apple.com/hk/en/retail/cantonroad/)；原始官方照片仅本地临时参考，不再分发。

相邻三组通用商铺增加橱窗纹理、凹凸窗跨、窗台／空调、雨篷、独立繁中招牌、植栽、货箱与暖光。店名“日常百貨／茶·食／九龍商行”为设计占位，不能解释成实际在此经营的商户。墙材与街面风格参考海防道 2024 照片，而非把该照片的整条街搬到广东道。

路缘、栏杆、排水槽、树池形成中小尺度层次；保留候车／过街开口，不用全场缩窄道路代替密度。方块树冠换成 alpha-clipped 交叉叶簇卡片；近处仍有树干和三维遮挡。前景植栽给画面深度，但风动、反射和近距离卡片侧视仍待精修。

新增四类像素行人、每类四帧侧向步行，按位移推进动画并使用透明轮廓切片和脚底锚点。第一稿带非对称肩包，已经弃用；第二稿移除包带后才允许左右镜像。不是四向 NPC 动画，未将这套侧向素材用作向纵深上车的人。白帽四向原图集沿用；自然承重、抬脚及角色像素颗粒仍需手工精修。

搬运者采用取货停顿 → 携箱走到店边 → 交付停顿 → 空手返回的有限循环，暂停时停止；这是生活活动编排，不是真实卸货调查。巴士保留 UnityHFSM 开门／上客／离站逻辑，增加圆轮胎、轮毂、腰线、车窗分层及车灯。候车乘客仍是几何占位，车身、司机与车门还不是最终车辆资产。

## 镜头与材质合同

保持透视、25° 俯角、Y 方位 0°、24° FOV；跟随距离由 55 缩至 49 米，构图偏移 Y=3。主角在 1920×1080 起点画面约 80 px 高；左右沿街、前后深入连接，3D 碰撞与深度不变。默认像素管线宽 1280，保留原生／960／640 对照；DOF 默认关闭，F 可切换。

材质 atlas 拆成四块：地砖、墙面、橱窗、窗户／空调。Unity 按 128 px 小图导入，Point + mipmaps；原始生成图完整保存。道具与 NPC 使用 URP Lit alpha clipping（0.5），真实 3D 阴影与受光；玻璃使用透明混合，透明深度与景深边缘仍需专项检查。镜头固定让我们能够集中制作立面与侧返，不需要为每件远景物体做全向精细模型；也不能据此省掉可走转角和投影体积。

## 实际烘焙

采用 Progressive GPU（Apple M5）、Mixed Baked Indirect 主光、Baked 店内灯；静态网格生成 UV2，动态角色／公交／搬运道具不设 Contribute GI。参数：8 texels/m、1024 atlas、4 px padding、32/128/64 samples、3 bounces，静态 scaleInLightmap 0.55、远景 0.15。配置和 `.exr`／方向 PNG／LightingData 与场景一并入库。

本批产出 1 组光照贴图、258 个 Light Probes；380 个渲染对象接入 lightmap，动态对象接入静态 lightmap 数为 0，参与 GI 的网格缺 UV2 数为 0。一次预烘检查后增加店内补光并重烘。实时主光仍负责动态直射和阴影；不声称烘焙后无灯光成本。

原日夜切换组件增加 opt-in `bakedAfternoon`：仅本精修场景锁定光照，避免日景间接光配夜景太阳。原实时 demo 行为保持。规范与官方依据见 [烘焙规则](../../../workflows/10-hd2d/camera-lighting.md#7-小场景光照烘焙与交付)。

## 验证证据

[本批证据目录](polish-validation/README.md) 保存起点／景深／支路／像素档位截图、输入、构建与烘焙结果。第一轮烘焙前图与最终图还包含灯光和素材迭代，不能归因成单变量烘焙 A/B。

实测：独立 Player 运行 180.26 秒，含 120 秒起点静止观察；12/12 路线点、216.60 米累计行走、公交 12 轮、交付 8 次，0 运行错误、0 上车违规、0 掉落恢复。1 组 lightmap 与 258 个探针在 Player 加载成功。输入 17 项通过。最终构建 7.45 秒、0 错误／警告。帧时 P95=17.29 ms、P99=17.65 ms，Unity allocated memory 148.13 MiB（非进程 RSS）；仍未达到 P95≤16.7 ms，也不是十分钟性能验收。

[机器可读结果](polish-validation/result.json) · [两分钟观察末画面](polish-validation/observation-end.png) · [证据哈希](polish-validation/manifest.json)。

## 重现与修改边界

`PolishCanton.Create` 从原 demo 另存；目标精修场景存在即拒绝覆盖。`FinishCantonPolish.Apply` 是一次性修订，已有标记即拒绝再次应用。`RefreshCitizenAtlas` 只重导入 NPC 图集并更新对应切片。`PolishCanton.Bake` 明确启动烘焙，`BakeStatus` 保存实际产物路径；`FinishCantonPolish.Audit` 检查静态／动态分工与 UV2。全部通过 Unity Editor API 创建序列化资产，未手写场景 YAML 或 `.meta`。

沿用 URP／Cinemachine／Input System、MIT PixelPipeline、UnityHFSM，无新增外部插件。阅读并使用了 Pipeline 的 `commands/scripts.md`、`commands/build-and-compilation.md` 及已有 capture 工作流。工具成本记录：本批 imagegen 共 4 次，含 NPC 去除非对称配件的局部修订 1 次；素材只导入采用稿，原稿保留来源记录。没有可靠 Token 计量，不报告节省比例。

## 距离精致小场景仍缺的工序

1. 同段连续实景与准确商户／尺度核对；当前邻接与店名仍是设计占位，不能把贴图密度等同地理准确。
2. 白帽和 NPC 动作逐帧整理、独立站姿／搬箱动作、正背向候车人物；人流避让、停留和目的地容量，减少重叠与机械往返。
3. 品牌标识与独有店面精修；墙材平铺／UV、雨篷布边、管线和玻璃反射，公交真实轮廓／内饰／司机／文字。
4. 远楼、支路与前景整合；树干与叶簇侧视、风动、车辆与行人接触阴影，消除重复和透明边缘伪影。
5. 脚步、车门、搬运与街道声景；来源许可、距离衰减与状态同步。
6. 夜景需要独立烘焙与完整切换；提高最终 lightmap 密度后检查噪声、接缝和探针过渡；完成十分钟性能／录像与用户主观美术验收。

本批证明了可走场景的美术替换、浅室内、活动及烘焙管线可以一起运行，不代表已达到《八方旅人 II》的精致程度，也不代表完整 v0.1。
