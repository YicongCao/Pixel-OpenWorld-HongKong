# 引擎复评：HD-2D、组件复用与 Agent 工作流

2026-11-01 · v0.4 · 官方文档调研与工程判断；尚未验证框架、MCP、渲染和导出的完整组合。

环境实施更新：现已选择本机 Unity `6000.3.27f1` LTS 建立环境验证工程，工具／包的实测情况见 [本地开发环境](local-development.md)。以下比较保留研究时的判断；付费框架与 HD-2D 街区组合仍未验收。

## 当前建议

**优先验证 Unity 6 + URP Universal Renderer（3D）＋一套可复用的 3D 角色／场景框架。Unreal 升为主要备选，Godot 3D 保留为开源备选。** TopDown Engine 的 3D 模式是首个框架候选，尚未购买、安装或锁定。名字里的 TopDown 不等于只能画平面瓦片；本项目要验证它的三维运动逻辑能否配合低角度像素表现。

v0.4 替代原来的“先纯 2D 灯光与分层虚化，必要时再改 2.5D”路线。真实体积、深度、台阶和镜头区从第一阶段进入架构。Unity 本身并不附送完整 RPG；推荐成立的条件是框架真正省下通用组件工作，像素／3D 混合呈现无需重写核心控制器。

评估顺序是：满足目标画面与通行空间 → 减少必要通用组件自研 → Agent 能完整修改、运行和诊断 → 本机导出与维护成本。没有剧情／战斗不构成淘汰 RPG Maker 的理由；但其最强的事件／数据库功能在本项目暂时不是主要工作量。

## 五个候选的项目适配

下表为本项目工程判断，不是实测性能排名或完整市场调查。MCP 的“官方／第三方／实验性”与本机是否配置成功分开记录。

| 候选 | 能复用什么 | HD-2D 代价 | Agent 路径 | 本轮定位 |
| --- | --- | --- | --- | --- |
| Unity + 成熟框架 | 框架角色／暂停／触发／场景管理，Cinemachine 相机及引擎 3D 工具 | 需适配像素角色受光、深度、阴影与镜头区；无需另造三维渲染器 | 官方 CLI / Pipeline / MCP，C# 编辑与构建测试 | 主选验证，整体平衡较好 |
| Unreal | 3D 角色运动、相机、导航、材质与后处理；具体生活／保存系统仍需组合 | 有《八方旅人》制作先例；像素人物与独特美术仍需自建，使用 UE 不等于自动得到该风格 | 官方实验性 MCP、编辑器 Python、C++／Blueprint | 主要备选；当视觉优先且编辑闭环顺畅时可反超 |
| Godot 3D | CharacterBody3D、Sprite3D、相机、灯光与导航等基础 | 像素人物混合 3D 可行；完整菜单／存档／街段管理更需自行整合 | 文本脚本／场景、官方 CLI/headless；社区 MCP | 开源备选，适合接受更多框架组装责任 |
| GameMaker | 2D 资源、对象、房间、模板与编辑流程 | 可编程 3D、着色和离屏合成，但本项目三维材质／深度／相机流程需更多自建或插件 | 官方 gm-cli 与资源工具 MCP | 降级原因是空间管线匹配度，不是缺少 Agent 支持 |
| RPG Maker MZ | 标准 JRPG 地图、事件、菜单、存档与数据库最齐全 | 默认 2D 流程；需要另加 3D／自由移动插件并验证楼层、碰撞、灯光、景深组合 | JS/JSON 与第三方 MCP，编辑器协同有限制 | 若回到传统 JRPG 地图可重新评估；不主选本次 HD-2D |

## 为什么暂时仍选 Unity

本项目需要的通用功能是稳定步行、输入、镜头、触发器、暂停、区域切换、位置保存和环境角色组织。先围绕这些功能选一套基础框架，不采购剧情、经济、战斗系统来充当“功能完整度”。

TopDown Engine 官方明确区分 2D 与 3D 控制器，并允许把角色逻辑根节点与视觉子节点分开；这支持“3D 控制器＋像素人物平面”的候选结构。官方镜头文档说明可接其他相机脚本，示例普遍使用 Cinemachine。**这些只证明存在适配入口，不能证明低角度相机、当前 Cinemachine 版本、台阶和像素材质的组合已经通过。** [角色结构](https://topdown-engine-docs.moremountains.com/character-classes.html)、[镜头](https://topdown-engine-docs.moremountains.com/cameras.html)、[框架功能](https://topdown-engine.moremountains.com/)

| 系统 | 复用优先级 | 项目负责的适配 |
| --- | --- | --- |
| 输入与移动 | 框架 3D controller 与其输入适配层 | 镜头相对输入、台阶／坡面参数、白帽四／八向动画；避免同时挂第二套控制器 |
| 镜头 | 框架兼容的 Cinemachine 版本 | 街段镜头区、边界余量、转向输入连续性、焦点跟随 |
| 暂停、设置、场景切换 | 复用框架已有部分 | 清楚区分暂停与城市时间；选区进入／退出与资源归属 |
| 保存 | 复用可用序列化／保存设施 | 自定义区域、世界位置、高度、朝向、楼层、时间与版本；库存存档不等于城市存档 |
| 环境导航 | 引擎导航包或框架已有路径方式，先核对责任 | 过街等灯、队列预约、卸货目的地、跨层连接与避让；不与玩家控制器重复争抢位置 |
| 画面 | URP 3D 材质、阴影与 Volume 效果 | 像素角色深度、遮挡分件、接触阴影与透明边缘验证 |
| 地理和生活 | 本项目新增 | GIS 到游戏布局映射、来源证据、活动状态和声景；不宣称商用框架已提供香港城市系统 |

先锁定一套输入、一套角色运动责任、一套镜头控制责任。官方包并非全部无条件叠加：AI Navigation 等仍是待核对的集成候选。若框架需要大规模修改源码才能支持楼梯或相机方向，其“省工”假设就失败，必须重新比较，而不是因为已经选过就坚持使用。

本机读取到 Unity Editor `6000.6.7f1`，不把它误写为已安装的 6.3 LTS。正式版本在框架／URP／Cinemachine 兼容表明确后锁定；本轮没有安装、升级或修改编辑器。先记录现有版本，避免盲目追最新包。

## Unreal 为什么进入主要备选

《八方旅人 II》有公开的 Unreal 制作访谈，是目标风格的生产先例。UE 的 3D 材质、相机和后处理工作流值得认真比较，但示例成功不代表我们的人员、资产和硬件已具备同样结果。[开发者访谈](https://www.unrealengine.com/developer-interviews/octopath-traveler-ii-builds-a-bigger-bolder-world-in-its-stunning-hd-2d-style?lang=en-US)

**当前官方 UE 5.8 文档已经提供实验性的 Unreal MCP。** 它可暴露 Actor、灯光、材质实例、界面检查和自动化测试等工具；需要相应 Toolsets，单启用 MCP 服务不等于所有功能已可用。官方同时说明功能仍有缺口，API／数据格式可能变化。因此不再用“缺少官方 Agent 集成”排除 UE，也不将实验能力视作稳定生产承诺。[官方 Unreal MCP](https://dev.epicgames.com/documentation/unreal-engine/unreal-mcp-in-unreal-editor)

UE 官方 Python 可自动化编辑器；运行时玩法依然需要 C++／Blueprint 等方案，不能把编辑器 Python 当游戏脚本。对于以文本修改、差异审查和重复构建为主的工作方式，Blueprint／资源资产的编辑和验证覆盖需要实际测试；这是一项工程成本判断，不是说 Agent 无法处理它们。[官方编辑器 Python](https://dev.epicgames.com/documentation/en-us/unreal-engine/scripting-the-unreal-editor-using-python)

暂选 Unity 的理由是 C# 与编辑器 API、框架候选及现有本机环境能较直接形成一次小规模验证。UE 反超的触发条件是：Unity 样板在关键画面或控制器上反复需要大改，而 UE 对应样板用现成能力能明显减少集成工作，且本机编辑、诊断、导出链路成立。不会为了比较而同时制作五套城市；必要时只将同一街角转成 UE 对照样板。HD-2D 不要求一开始启用 Lumen／Nanite 或光追。

## Agent 集成要验证完整闭环

需要的是“改源码／场景 → 导入编译 → 运行固定路线 → 读取日志与截图 → 修复 → 导出”，不是仅连接 MCP 成功。实际覆盖、人工步骤与耗时应留下记录。

- **Unity**：官方推荐 CLI / Pipeline，支持 `unity command`、`unity eval` 和 `unity mcp`；旧 `com.unity.ai.assistant` 内置 MCP 已弃用。具体方法与返回格式以锁定版本为准，本轮未连接或测试。[官方迁移说明](https://docs.unity.com/en-us/unity-cli/replace-mcp-server-unity-cli)
- **Unreal**：采用上面的官方实验性 MCP 与编辑器脚本候选，验证工具集是否覆盖实际素材和调试需求；不因文档出现接口就自动升级工程到对应版本。
- **Godot**：官方 CLI 支持导入、运行、headless 与导出；文本场景适合代码审查。MCP 社区实现须单独核对维护与功能，不能称为官方集成。[官方命令行](https://docs.godotengine.org/en/stable/tutorials/editor/command_line_tutorial.html)、[社区示例](https://github.com/signalcompose/godot-mcp)
- **GameMaker**：官方 `gm-cli` 包含资源操作与 MCP，不能继续沿用“缺少 Agent 工具”的印象。[YoYoGames 仓库](https://github.com/YoYoGames/gm-cli)
- **RPG Maker MZ**：第三方 MCP 可编辑地图／数据库／事件；作者要求写 JSON 期间关闭编辑器，避免互相覆盖。自定义 3D 插件仍需另测，工具存在不代表插件兼容。[第三方项目](https://github.com/Redseb/rpgmaker-mz-mcp)

## 渲染取舍已经收敛

采用真实 3D 深度，让道路、墙角、阶梯、雨篷和桥面提供前后关系。像素角色优先验证受光 alpha-clipped 平面与正确深度／阴影通道；透明对象需要额外检查。Unity 提供对应 Lit 材质和 URP 景深，艺术素材与相机仍需专门适配。[Lit](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/lit-shader.html)、[景深](https://docs.unity.com/en-us/engine/6000.0/manual/post-processing-and-full-screen-effects/urp/effect-list/depth-of-field-urp)

Godot 的 Sprite3D／AnimatedSprite3D 支持受光、billboard 与 alpha cut，可形成类似结构；原生相机景深依赖 Forward+ 或 Mobile，不支持 Compatibility。若换 Godot，应评估 3D 管线而非回到 Canvas2D 加后期伪造整个深度。[SpriteBase3D](https://docs.godotengine.org/en/stable/classes/class_spritebase3d.html)、[CameraAttributesPractical](https://docs.godotengine.org/en/stable/classes/class_cameraattributespractical.html)

GameMaker 的 surfaces／shaders 和 MZ 插件可以实现额外效果，但本轮未找到并验证一套能同时省下三维场景、分层通行、深度材质和相机工作量的完整组合。因此降低优先级，不能笼统声称这些引擎“做不了”。[GameMaker Surfaces](https://manual.gamemaker.io/monthly/en/GameMaker_Language/GML_Reference/Drawing/Surfaces/Surfaces.htm)、[MZ 产品](https://www.rpgmakerweb.com/products/rpg-maker-mz)

## 采购与锁定之前的通过条件

先验证一处横向街角、纵深支路、阶梯／桥下测试件、一栋分层商铺和白帽角色。画面分别测试白天、夜晚、景深开关；系统分别测试移动、镜头区、暂停、两区域往返与位置／楼层读档。Agent 必须能完成一次场景及材质修改后的编译、运行诊断和 Mac 导出。

记录直接复用、适配和新增代码各是什么；记录已安装引擎、渲染器、框架与包版本、人工干预步骤、帧时和显存／内存指标。通过后才锁定组合并批量制作资产；当前没有任何实测证明某引擎帧率更高、MCP 零故障或框架已经适配。

空间见 [街网与镜头](../03-hd2d/street-camera.md)，像素规范见 [美术研究](../03-hd2d/art-direction.md)，遮挡见 [运行架构](runtime-architecture.md)，样板见 [实施验证](../06-production/prototype.md)。
