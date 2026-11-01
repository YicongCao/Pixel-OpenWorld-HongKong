# GitHub HD-2D Unity 项目与组件复用筛选

2026-11-01 · 以本工程 Unity 6000.3.27f1／URP 17.3.0、Input System、Cinemachine 3 为基线。已在线读取仓库、包清单、许可证与指定源码；**未导入候选、未在本机编译它们，也未验证其性能**。结论是采用前的技术筛选，不能写成已集成。

## 结论与优先级

有值得复用的项目，但此次未找到同时满足许可清楚、版本匹配、弱透视 HD-2D、真实街区及完整生活系统的现成底座。继续使用现有工程，逐个接入小组件，比整体换模板更合适。以下是项目判断，不是作者的兼容性承诺。

1. **优先研究 bababuyyy 的像素管线**：确实使用 URP 17.3.0，可借鉴像素描边、放大与像素漂移补偿。先单独比较渲染效果，不接管现有相机。
2. **下一批活动原型优先试 UnityHFSM**：用现成层级状态机承载公交、候车、站口和卸货的阶段切换，业务规则仍由本项目实现。无需再做一套通用状态机底层。
3. **遮挡辅助参考 Unity-simple-HD2D 和 Chishikii**：分别看角色面片／投影与按层控制的描边，避免全局透墙或 Z-sort。
4. **景深继续复用 URP，镜头继续复用 Cinemachine**：没有必要为了 HD-2D 另写一套 DOF、相机跟随或完整 RPG 框架。
5. 暂缓高版本包及强烈 PSX 风格管线；不因一个效果包升级整个项目。

## 候选清单

| 项目 | 实际核查版本／许可 | 能减少的工作 | 本项目决定 |
| --- | --- | --- | --- |
| [unity-isometric-pixel-pipeline](https://github.com/bababuyyy/unity-isometric-pixel-pipeline) | Unity 6000.3.5f2；URP 17.3.0；MIT；检查到 2026-05-16 提交 | RenderGraph 像素描边、合成／放大、镜头补偿、昼夜职责分离 | 第一渲染候选，拆取试验；整套相机不采用 |
| [Unity-simple-HD2D](https://github.com/floatingsnowflake/Unity-simple-HD2D) | Unity 2021.3.29f1c1；URP 12.1.12；MIT；2023-09-08 提交 | Sprite 转 MeshRenderer、受影／投影、旧 HD-2D 场景组织 | 参考或局部移植；排除旧控制器及全局 Z 排序 |
| [UnityHFSM](https://github.com/Inspiaaa/UnityHFSM) | `upm` 包清单 2.3.0；MIT；仓库 pushed_at 2026-03-25 | 通用层级状态机、转换与退出协调 | 第一行为组件候选，单独试公交循环 |
| [URP-Render-Features](https://github.com/Chishikii/URP-Render-Features) | README 指明 URP 17.0.3；MIT；main 提交 2025-12-01 | 按 LayerMask／RenderingLayer 选择的描边 | 可见性辅助备选，先核查具体 pass |
| [retropsx-urp](https://github.com/Natteens/retropsx-urp) | 包 1.1.7；最低 Unity 6000.0／URP 17.0.3；MIT；2026-09-05 提交 | 分辨率呈现、色彩与雾模块、原生 UI 分离 | 第二渲染备选，关闭 PSX 抖动／仿射 UV／CRT |
| [stylized-rendering](https://github.com/cannoli-cat/stylized-rendering) | 包 1.1.0 要求 Unity 6000.5／URP 17.5.0；CC BY 4.0；2026-10-06 提交 | 调色板、抖色、轮廓与锐化 | 暂不安装；版本高于工程，不能只看 README 的“Unity 6” |
| [unity-URP-2.5D-lit-shader](https://github.com/Jiaquarium/unity-URP-2.5D-lit-shader) | README 为 Unity 2020.2.1／URP 10.2.1+；main 根目录未见许可证；2024-07-21 提交 | billboard 与墙体相交时的深度修正思路 | 只列技术参考，不复制其代码 |
| [hd2d-rpg](https://github.com/yintony96/hd2d-rpg) | 描述称 Unity 6 URP；仓库 API license=null；2026-04-14 pushed_at | 完整项目结构线索 | 非采用候选，未深入审查代码或实机质量 |
| [TCC-HD2D-Valquiria](https://github.com/Smurillopng/TCC-HD2D-Valquiria) | README 声明含 Odin Inspector 等第三方资产；API 未返回许可证；2024-01-05 pushed_at | RPG 场景／系统组织参考 | 不整体移植；根仓库公开不能证明第三方资产可再分发 |
| [DitherFx](https://github.com/NullTale/DitherFx) | 仓库许可证为非商业用途并限制再分发；2025-06-09 提交 | 抖色外观参考 | 排除免费商用依赖，不纳入当前开源采用池 |

日期表示检查到的分支提交或仓库推送时间，不等于作者持续维护保证。GitHub stars、名字像 HD-2D 或演示漂亮都没有被当作可用性证据。许可证判断只界定本次采用范围；实际引入时仍需保留对应版本的通知、修改说明及第三方资产条款。

## 第一候选：像素管线，能复用什么

固定检查版本为 [f08bc6b](https://github.com/bababuyyy/unity-isometric-pixel-pipeline/tree/f08bc6bf34b81c265171972610b23a8ef7c8d019)。版本直接核对 [ProjectVersion](https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/f08bc6bf34b81c265171972610b23a8ef7c8d019/ProjectSettings/ProjectVersion.txt)、[manifest](https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/f08bc6bf34b81c265171972610b23a8ef7c8d019/Packages/manifest.json) 与 [MIT](https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/f08bc6bf34b81c265171972610b23a8ef7c8d019/LICENSE)。

已阅读 [PixelRendererFeature.cs](https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/f08bc6bf34b81c265171972610b23a8ef7c8d019/Assets/Rendering/RenderFeatures/PixelRendererFeature.cs)：使用 `RecordRenderGraph`，在 `AfterRenderingPostProcessing` 后复制全分辨率颜色，再低分辨率取样、描边、合成和放大。深度／法线是输入依赖，效果所需纹理由 RenderGraph 创建。可以少写一套渲染 pass 生命周期、采样与合成逻辑。

但它先画全分辨率场景再降采样，**不能据此推导出减少主场景几何或光照开销**；还会增加 pass 和深度／法线需求。640×360 是作者默认值，不应变成本项目全局约定；先比较 960×540、1280×720 与原生画面，保留中文路牌和白帽的可读性。这些是本项目实验档位，尚未测量。

已阅读 [IsometricCameraController.cs](https://github.com/bababuyyy/unity-isometric-pixel-pipeline/blob/f08bc6bf34b81c265171972610b23a8ef7c8d019/Assets/Scripts/Systems/IsometricCameraController.cs)：`Awake` 会强制 `orthographic=true`，像素尺寸由 `orthographicSize` 计算，还调用旧 `UnityEngine.Input`。**不能直接挂到当前相机**：它会改变弱透视和景深前提，并和 Cinemachine 的变换控制竞争。只把屏幕网格对齐／UV 补偿作为待移植思路；透视下不同深度像素尺寸不同，需要另外验证，不能把正交公式照搬。

作者的 toon、草与云影更接近全 3D 像素化。香港样板需要像素人物和有体积的纹理建筑，不能用全屏像素化替代立面美术。先试渲染部件；不搬地形、草、相机、输入、项目设置和所有包清单。MIT 通知在真正引入代码时随采用目录保存。

## 旧 HD-2D 示例：哪些代码值得看、哪些应跳过

检查 [8145946](https://github.com/floatingsnowflake/Unity-simple-HD2D/tree/81459467442741f96f39e8fbecb2a1099572727c)，实际是 URP 12／Cinemachine 2.8.9，不是 Unity 6 模板。

- [SpriteShadow.cs](https://github.com/floatingsnowflake/Unity-simple-HD2D/blob/81459467442741f96f39e8fbecb2a1099572727c/Assets/HD2DProject/Scripts/Sprites/SpriteShadow.cs) 设置 Sprite／Tilemap Renderer 的受影和双面投影；说明这些职责可以复用 Unity Renderer 配置，不必先发明影子系统。正确帧裁切、材质 ShadowCaster 与自阴影仍要单独验证，开启标志不保证效果正确。
- [SpriteQuad.cs](https://github.com/floatingsnowflake/Unity-simple-HD2D/blob/81459467442741f96f39e8fbecb2a1099572727c/Assets/HD2DProject/Scripts/Sprites/SpriteQuad.cs) 把 Sprite 的纹理／UV 交给 MeshRenderer。它访问 `.material` 和 `.mesh`，会涉及实例化；`SetSprite` 内还使用字段 `sprite` 判断传入对象。可参考转换方法，但不替换我们现有的图集＋MaterialPropertyBlock 播放器，先衡量是否真的少代码。
- [SpriteSorting.cs](https://github.com/floatingsnowflake/Unity-simple-HD2D/blob/81459467442741f96f39e8fbecb2a1099572727c/Assets/HD2DProject/Scripts/Sprites/SpriteSorting.cs) 把世界 Z 乘 16 写入 sortingOrder。它无法表达天桥、站口、台阶与上下层遮挡，不采用。
- [HD2DCharacter.cs](https://github.com/floatingsnowflake/Unity-simple-HD2D/blob/81459467442741f96f39e8fbecb2a1099572727c/Assets/HD2DProject/Scripts/Characters/HD2DCharacter.cs) 依赖 `TeamMingo` 类型，并直接增加 Transform 位置。不是可直接替换的带 3D 碰撞通用控制器，保留当前 CharacterController／Input System。

仅局部采用自有脚本时保留 [MIT](https://github.com/floatingsnowflake/Unity-simple-HD2D/blob/81459467442741f96f39e8fbecb2a1099572727c/LICENSE)，素材目录另核查。不要把该仓库整套场景和外来美术搬进生产工程。

## 行为组件与遮挡备选

[UnityHFSM](https://github.com/Inspiaaa/UnityHFSM) 提供层级状态、转换和退出时机协调；[UPM package.json](https://github.com/Inspiaaa/UnityHFSM/blob/upm/package.json) 当前为 2.3.0，[许可证](https://github.com/Inspiaaa/UnityHFSM/blob/master/LICENSE.md) 为 MIT。这次只核查文档／包／许可，未深入运行时实现或运行测试；实际采用须再锁定 tag 或 commit，不能长期跟踪浮动 `upm`。

适合公交的 `Approach → Stop → DoorOpen → Alight → Board → DoorClose → Depart`，以及收货的准备、搬运、交付、清场。它减少状态机基础代码；**不会自动提供香港车道、候车容量、上下客同步、站口寻路或存档语义**。寻路继续使用已装 AI Navigation，数据配置仍由本项目负责；不因引入状态机另做一个可视化 RPG 编辑器。

[Chishikii](https://github.com/Chishikii/URP-Render-Features) 的 [README](https://github.com/Chishikii/URP-Render-Features/blob/2590abe28229110fefef58e2a0e03b6f5b53a2fb/README.md) 明确区分功能：outline 已适配 RenderGraph，但 blur 和 desaturation 尚未适配。适合研究局部受控的主角轮廓提示；本次没有逐行审查其 shader，不把全部功能称为 URP 17 可直接使用。遮挡辅助应限于短暂、必要场景；正常情况下仍遵守 3D 深度，不永久穿墙显示角色。

## 高版本、其他风格与许可限制

[Stylized Rendering 包清单](https://github.com/cannoli-cat/stylized-rendering/blob/22edcfdae269b8f06e51886be560ab39349f8798/package.json) 比 README 的“Unity 6”更具体：要求 6000.5／URP 17.5.0。其 [LICENSE.md](https://github.com/cannoli-cat/stylized-rendering/blob/22edcfdae269b8f06e51886be560ab39349f8798/LICENSE.md) 当前为 CC BY 4.0，蓝噪声另有 CC0；不能把它误记成 MIT。当前只保留调色板／抖色设计参考，不安装、不升级引擎。

[RetroPSX](https://github.com/Natteens/retropsx-urp/tree/7102c2558d8addec2584d22e48ee50fa2bc217bd) 的 [package.json](https://github.com/Natteens/retropsx-urp/blob/7102c2558d8addec2584d22e48ee50fa2bc217bd/package.json) 最低要求更接近我们。它是 PSX 工具集，配置模块化且保留普通 URP 材质；可以考察呈现／雾／UI 分离，但仿射 UV、顶点抖动和 CRT 不符合当前目标。README 的 UI 路径说明还标注了在 6000.5.7f1 验证、旧版未运行测试，不能从最低版本字段推出所有功能均已在 6000.3 验证。许可见 [MIT](https://github.com/Natteens/retropsx-urp/blob/7102c2558d8addec2584d22e48ee50fa2bc217bd/LICENSE.md)。

[Jiaquarium](https://github.com/Jiaquarium/unity-URP-2.5D-lit-shader) 的 [README](https://github.com/Jiaquarium/unity-URP-2.5D-lit-shader/blob/f0cb34f340634e7a66a595c696e5a65dc33edbf4/README.md) 针对倾斜正交面片修改 clip-space depth，并列出非方向光阴影等限制。技术问题与我们相关，但根目录和包周边未核到明确许可证，不能把“公开可读”当成可直接分发。若日后获得授权，仍须核验颜色、DepthOnly、DepthNormals、ShadowCaster 在 URP 17 的一致性和弱透视适用性。

[DitherFx 的仓库许可](https://github.com/NullTale/DitherFx/blob/ca182e968870d9907ebbd50d5b7e61e9c520d220/LICENSE.md) 明确限制商业与再分发，故不纳入免费商用依赖；这不是对 GitHub 上所有 shader 的泛化判断。其他 HD-2D 仓库的自述不能证明其资产权利或成熟度。

## 对 v0.1 实施规划的具体调整

先建立独立渲染实验副本，不改变地理、控制器和正式样板。比较“当前画面／只加描边与放大／加补偿”三组，保存相同路线、镜头、1080p 的独立 Player 证据；景深分别开关，并观察玻璃 A1、树冠、白帽边缘、繁中店招及前后景。效果选择必须考虑整体 HD-2D 风格，而非某张截图的像素数量。

复用验收包括：不强制正交、不和 Cinemachine 竞争、不切换回旧 Input、不把 UI 全屏降采样；白帽的颜色／深度／阴影裁切一致；多相机状态不互相污染；无 RenderGraph 读写冲突、黑屏或构建 shader stripping；记录新增 pass 和真实帧时，不能宣称降分辨率天然加速。先分别测试，再决定采用代码范围和署名。

行为原型优先用 UnityHFSM 做一条公交循环和一个目的地行人，再连到 AI Navigation。验证重复三轮：车门未开不能上客、队列容量有限、演员有退出点、重置不重复生成。它替代通用状态机骨架，不替代现实站位核实与生活编排。

本次成果仅为研究与采用建议：没有复制第三方源代码／美术、安装新包或修改运行时。后续真正引入时按固定版本、独立命名空间、第三方通知和可关闭的组件接入；不 fork 整个别人项目作为新的主工程。

候选版本与检查范围见 [机器可读索引](references/github-hd2d-candidates-2026-11-01.json)。
