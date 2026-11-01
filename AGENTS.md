# 香港像素漫游 · 项目约定

本文件是项目唯一的根目录 Agent 入口，维护常驻项目约定、工作流路由与提交／推送规则。还原街区时必须读取 [街区还原工作流](workflows/00-project/街区还原工作流.md)，按任务范围执行。

本项目是香港尖沙咀及九龙相关区域的 HD-2D 像素步行 RPG，首版没有剧情、战斗和可乘交通。用户要求以《八方旅人》为参照的像素角色＋立体像素街景、三角白帽白衣小巫师和真实商业元素。

- 研究统一从 `docs/README.md` 进入；读取 `docs/Agent.md` 和 `docs/Skill.md` 后按任务选择主题。引擎、尺度和制作参数仍为待实机验证的研究建议。
- 分析成果保存项目内普通 Markdown、独立 HTML 或标准图表，不使用 cursor/canvas 文件。
- 地理来源与游戏设计分别保存；不擅自修改隔壁 `game-research`。原始数据经纬度 / 米制 / 高程基准不能混用。
- 数据更新先读 `data/hong-kong/README.md` 和 manifest；公交完整表与增量表分开，天桥 / 地面不得因投影相交自动互连。
- 生成资产与手工精修分目录；重建脚本不得覆盖手工资产。概念图不是已验收游戏资源。
- 沿用已有授权，例行可逆操作不重复确认；不自动把旧项目双岸 3D 和乘车需求带入本项目。

- 终极体验以“沉浸式香港街道生活”为准，地标、普通建筑/设施、人群与劳动活动、声景和时间天气共同验收。优先按 `docs/05-street-life/street-life.md` 组织场景；地图事实、历史观察与设计编排分开，不用随机行人或重复地标代替生活。
- 商铺品牌是沉浸感的必要资产：制作经核实的商标图形／标准字、招牌配色、安装位置和对应店面类型，不能用“百货／茶食”等泛称代替最终店招。优先官方门店目录与实景，记录核对日期；未核实的商户不随意挂品牌，历史与当前门店分开。标识独立分件，保留来源，实机检查小字号、Bloom 与像素处理后的辨识度；不得把品牌资产误标为代码的开源许可。广东道已采用 Apple 与莎莎，详见 [品牌实施](docs/08-blocks/tst-canton-demo/brands-v1.md)。

- 当前空间／渲染基线见 `docs/03-hd2d/street-camera.md`：横向主街＋纵深连接＋有证据的高差，采用 3D 空间、碰撞与深度；不沿用纯 2D TopDown、固定 640×360 或全局 Y-sort 前提。地理方向与镜头方向分开。

## 工程导航与按需读取

先检查工作区和本任务范围，再读取相关目录说明；不要每次扫描整库。下列路径相对仓库根目录。目录新增、迁移或职责变化时同步维护本节和对应索引。

- [README.md](README.md)：项目现状与总体入口；[docs/README.md](docs/README.md) 是研究总目录。
- `workflows/`：整个工程的执行入口，见 [工作流目录](workflows/README.md)；00 总流程与反馈维护，10 HD-2D 制作／场景／渲染／迭代，20 角色／建筑／景物。
- `docs/`：静态资料、研究、计划与证据。按 [Agent.md](docs/Agent.md) 和 [Skill.md](docs/Skill.md) 选择主题：01 总纲、02 地理交通、03 HD-2D 镜头美术、04 引擎架构、05 街道生活、06 生产研究与规划、07 验证证据、08 街区档案。先查主题索引，再读具体研究；现有版本状态见 [Roadmap](docs/06-production/roadmap.md) 与 [首版实施](docs/06-production/v0.1-implementation.md)。
- `data/`：地图／城建／交通来源与派生资料。涉及数据先读 [Readme.md](data/Readme.md)、[Agent.md](data/Agent.md)，再查 `hong-kong/` 对应包的 README／manifest；道路建筑在 osm，三维行人路径在 pedestrian，公交在 transport，港铁图在 mtr，设施补充在 street-life，走廊研究在 hd2d-study。
- `assets/`：概念与未来生产资产。涉及资产先读 [Readme.md](assets/Readme.md)、[Agent.md](assets/Agent.md)；当前 `concepts/` 是探索图，含 `hd2d/` 子集；`blocks/tst-nathan-haiphong/generated/hero` 与 `materials` 是已导入实机的小样，仍待美术验收。
- `scripts/`：`extract_hk.py`／`extract_street_life.py` 提取数据，`build_hd2d_study.py` 生成走廊研究，`build_report.py` 按 `docs_catalog.json` 渲染章节，`validate_docs.py` 校验文档，`validate_delivery.py` 完整校验数据。研究依赖见 [requirements-research.txt](scripts/requirements-research.txt)；运行前检查读写范围，不因改文档重跑全量提取。
- 首街区实现派生：`scripts/build_v01_layout.py` 从归档数据生成米制路线／来源校验，`scripts/prepare_v01_assets.py` 检查图集透明轮廓并复制采用资产；运行方式和边界见 [实施记录](docs/08-blocks/tst-nathan-haiphong/implementation.md)。源图集、切片与导入副本须一起校验，已有场景不因数据更新自动重建。
- 街区制作目录：`data/blocks/<block_id>/` 存街区骨架与来源，`references/blocks/<block_id>/` 存实景索引及可保存原件，`assets/blocks/<block_id>/generated/`／`authored/` 分离生成与精修；`docs/08-blocks/<block_id>/` 存计划及实机验证。当前已建立 `tst-nathan-haiphong` 的数据、实景、概念和计划；已有约 179 米路线、灰盒与 A1 早期实机样板，尚无 authored 精修或完整 v0.1 验收，入口见 [首街区计划](docs/08-blocks/tst-nathan-haiphong/plan.md)。
- `unity/`：唯一 Unity 工程为 `unity/HongKong/`，先读 [Unity README](unity/README.md) 和 [本地环境记录](docs/04-engineering/local-development.md)。使用锁定的 Editor／Packages；所有 CLI 编辑器操作显式传本工程 `--project-path`，不得误操作旧项目。当前精修入口为 `Assets/HongKong/Authored/CantonPolish/CantonRoadPolish.unity`，原 demo 为 `Assets/HongKong/Generated/CantonDemo/CantonRoadDemo.unity`；原街角小样 `Assets/HongKong/Generated/Scenes/NathanArtSample.unity` 与镜头对照 `NathanHD2DStudy.unity` 保留，原实施结果见 [批次 01](docs/08-blocks/tst-nathan-haiphong/implementation.md)。`AgentScripts/` 是版本化生成／修订入口，`Assets/HongKong/Runtime/` 是运行时适配及验证；不能把环境场景或街角样板称为完整 v0.1。
- `.agents/skills/`：按需读取 Unity 官方的 `unity-cli`、`unity-pipeline`、`unity-package-management`；它们指导工具操作，版本与项目范围以本仓库约定为准，不自动升级引擎或采购框架。
- HD-2D 生产入口：[素材建模与拼装 SOP](workflows/10-hd2d/hd2d-production-workflow.md) 统筹资产链路；[角色制作](workflows/20-assets/character-production.md) 负责转面与动作；[镜头／灯光／景深](workflows/10-hd2d/camera-lighting.md) 负责运行时画面；[专业资料与源码核查](docs/06-production/hd2d-production-sources.md) 标明依据和未证实参数。
- [.gitignore](.gitignore)：临时／敏感文件和引擎缓存边界；`.local/`／`.private/` 是按需使用的忽略目录。`../game-research` 是只读上游，不是本项目运行时依赖。

复用候选见 [GitHub HD-2D 组件筛选](docs/04-engineering/github-hd2d-reuse.md)。开始新增通用渲染或行为底层前先查该清单；其中 PixelPipeline 与 UnityHFSM 已在广东道采用，固定版本／许可及实测见街区记录；其余仍为研究候选，采用时核对固定版本／许可并单独验证，不整体替换主工程。

## 工作流优先：每次实施必须执行

**打造可复用、可验证、能吸收反馈的工作流是本工程第一优先级。** 一次成果应同时留下可复现的方法；不能只修一张图／一个对象而让相同问题在下一批重现。

1. 从 [workflows/README.md](workflows/README.md) 选择一个主工作流，列出需要调用的子流程；开始时简述选择、执行步骤范围与交付物。局部修复只执行适用步骤，注明跳过原因与已有证据，不强制重跑整条链。
2. 没有合适流程时，先按 [WF-MAINTAIN](workflows/00-project/workflow-maintenance.md) 新建最小流程，再实施需求。研究／文档／工具维护也用此流程；不为了匹配现有流程擅自扩大用户范围。
3. 按流程读取 docs 的证据、data 的骨架、references 的实景、assets 的资产。遵守输入、顺序、输出与验收门槛；失败回到相关步骤，不用下游效果掩盖上游缺陷。
4. 用户提出改进时，记录反馈 → 定位所属步骤与根因 → 修订所引用流程及检查项 → 修正产物 → 同条件复验。局部实例参数留在街区记录，通用规则回写 workflow；新规则未实测标为待验证，不把用户未认可的结果标为美术通过。
5. 每批记录 workflow ID／版本／起始 Git 提交、输入版本、执行／跳过步骤、产物、验证证据与遗留项。执行记录存 `docs/08-blocks/<block_id>/`，跨工程维护记录存 `docs/07-validation/`，模板见 WF-MAINTAIN。
6. 一项步骤只在一个 workflow 维护；父流程通过链接调用子流程。`docs` 维护名词（事实、研究、计划、清单、证据），`workflows` 维护动词（制作与验证步骤）；更新目录、引用与工作流版本后验证并 commit／push。

完整街区选 [WF-BLOCK](workflows/00-project/街区还原工作流.md)，HD-2D 小样选 [WF-HD2D](workflows/10-hd2d/hd2d-production-workflow.md)，摆放／活动／交付选 [WF-SCENE](workflows/10-hd2d/scene-assembly-validation.md)，镜头／灯光／烘焙选 [WF-LIGHT](workflows/10-hd2d/camera-lighting.md)，角色／建筑／景物分别选 WF-CHAR、WF-BUILDING、WF-PROPS。精修时按 [WF-ITERATE](workflows/10-hd2d/iteration-and-fidelity.md) 限定可见面、复用模块、分层验证，不能删掉必要深度、真实街侧和活动空间。

## 所有仓库改动必须提交并推送

- 远程固定为 `git@github.com:YicongCao/Pixel-OpenWorld-HongKong.git`，默认分支 `main`。用户已授权本项目后续修改后的 commit 与 push，无需每次重复询问。
- 任何新增、修改、删除或重命名的项目文件都适用，包括文档、规则、脚本、地图、参考索引、生成／手工资产、模型、Unity 场景及 `.meta`，不只源代码。以每个完成且验证的修改批次为提交单元；持续任务在阶段完成时提交并推送，不在单次文件写入尚未完成时提交中间损坏状态。
- 开始先看工作区、当前分支、远程和远程更新；有已有用户改动时保留，不丢弃、不重置。只暂存本任务拥有的改动，不将不相关内容静默混入。
- 完成一个修改批次后，检查 diff／新增文件清单、敏感内容与大文件，执行适合改动的验证，再创建描述实际结果的 commit 并 push。生成的有效资产必须纳入版本管理；缓存、临时输出、凭证和不得再分发的参考原件不属于可推送成果。
- 常规使用普通 Git；新增大二进制前检查远程文件限制，必要时规划 Git LFS 并验证对象确实上传，不能只提交指针或直接忽略交付资产。不要把临时构建缓存当资产，也不要无授权购买存储或订阅。
- push 后核对远程目标分支的 SHA 与本地提交一致，并检查工作区。推送失败保留本地 commit，报告具体原因与未同步状态；不得宣称成功，不 force-push，不改远程地址绕过问题。
- 遇到远程新提交先获取并检查，保留双方工作后正常合并／变基；不可覆盖别人的历史。用户明确要求隔离分支时使用该分支并说明推送目标。
- 这是一项 Agent 执行规则，不是后台文件监控器：每次 Agent 完成修改必须落实提交推送；不宣称用户或外部程序保存文件后会自动实时同步。
