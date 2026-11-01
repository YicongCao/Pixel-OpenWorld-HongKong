# 香港像素漫游 · 项目约定

本文件是项目唯一的根目录 Agent 入口，维护常驻项目约定与提交／推送规则。还原街区时必须读取 [街区还原工作流](docs/06-production/街区还原工作流.md)，按任务范围执行。

本项目是香港尖沙咀及九龙相关区域的 HD-2D 像素步行 RPG，首版没有剧情、战斗和可乘交通。用户要求以《八方旅人》为参照的像素角色＋立体像素街景、三角白帽白衣小巫师和真实商业元素。

- 研究统一从 `docs/README.md` 进入；读取 `docs/Agent.md` 和 `docs/Skill.md` 后按任务选择主题。引擎、尺度和制作参数仍为待实机验证的研究建议。
- 分析成果保存项目内普通 Markdown、独立 HTML 或标准图表，不使用 cursor/canvas 文件。
- 地理来源与游戏设计分别保存；不擅自修改隔壁 `game-research`。原始数据经纬度 / 米制 / 高程基准不能混用。
- 数据更新先读 `data/hong-kong/README.md` 和 manifest；公交完整表与增量表分开，天桥 / 地面不得因投影相交自动互连。
- 生成资产与手工精修分目录；重建脚本不得覆盖手工资产。概念图不是已验收游戏资源。
- 沿用已有授权，例行可逆操作不重复确认；不自动把旧项目双岸 3D 和乘车需求带入本项目。

- 终极体验以“沉浸式香港街道生活”为准，地标、普通建筑/设施、人群与劳动活动、声景和时间天气共同验收。优先按 `docs/05-street-life/street-life.md` 组织场景；地图事实、历史观察与设计编排分开，不用随机行人或重复地标代替生活。

- 当前空间／渲染基线见 `docs/03-hd2d/street-camera.md`：横向主街＋纵深连接＋有证据的高差，采用 3D 空间、碰撞与深度；不沿用纯 2D TopDown、固定 640×360 或全局 Y-sort 前提。地理方向与镜头方向分开。

## 工程导航与按需读取

先检查工作区和本任务范围，再读取相关目录说明；不要每次扫描整库。下列路径相对仓库根目录。目录新增、迁移或职责变化时同步维护本节和对应索引。

- [README.md](README.md)：项目现状与总体入口；[docs/README.md](docs/README.md) 是研究总目录。
- `docs/`：按 [Agent.md](docs/Agent.md) 和 [Skill.md](docs/Skill.md) 选择主题：01 总纲、02 地理交通、03 HD-2D 镜头美术、04 引擎架构、05 街道生活、06 生产与规划、07 验证证据、08 街区档案。先查主题索引，再读具体研究；现有版本状态见 [Roadmap](docs/06-production/roadmap.md) 与 [首版实施](docs/06-production/v0.1-implementation.md)。
- `data/`：地图／城建／交通来源与派生资料。涉及数据先读 [Readme.md](data/Readme.md)、[Agent.md](data/Agent.md)，再查 `hong-kong/` 对应包的 README／manifest；道路建筑在 osm，三维行人路径在 pedestrian，公交在 transport，港铁图在 mtr，设施补充在 street-life，走廊研究在 hd2d-study。
- `assets/`：概念与未来生产资产。涉及资产先读 [Readme.md](assets/Readme.md)、[Agent.md](assets/Agent.md)；当前 `concepts/` 是探索图，含 `hd2d/` 子集，不能当成已验收游戏资源。
- `scripts/`：`extract_hk.py`／`extract_street_life.py` 提取数据，`build_hd2d_study.py` 生成走廊研究，`build_report.py` 按 `docs_catalog.json` 渲染章节，`validate_docs.py` 校验文档，`validate_delivery.py` 完整校验数据。研究依赖见 [requirements-research.txt](scripts/requirements-research.txt)；运行前检查读写范围，不因改文档重跑全量提取。
- 街区制作目录：`data/blocks/<block_id>/` 存街区骨架与来源，`references/blocks/<block_id>/` 存实景索引及可保存原件，`assets/blocks/<block_id>/generated/`／`authored/` 分离生成与精修；`docs/08-blocks/<block_id>/` 存计划及实机验证。当前已建立 `tst-nathan-haiphong` 的数据、实景、概念和计划；尚无 authored 精修或街区实机结果，入口见 [首街区计划](docs/08-blocks/tst-nathan-haiphong/plan.md)。
- `unity/`：唯一 Unity 工程为 `unity/HongKong/`，先读 [Unity README](unity/README.md) 和 [本地环境记录](docs/04-engineering/local-development.md)。使用锁定的 Editor／Packages；所有 CLI 编辑器操作显式传本工程 `--project-path`，不得误操作旧项目。环境测试场景不代表已完成街区。
- `.agents/skills/`：按需读取 Unity 官方的 `unity-cli`、`unity-pipeline`、`unity-package-management`；它们指导工具操作，版本与项目范围以本仓库约定为准，不自动升级引擎或采购框架。
- [.gitignore](.gitignore)：临时／敏感文件和引擎缓存边界；`.local/`／`.private/` 是按需使用的忽略目录。`../game-research` 是只读上游，不是本项目运行时依赖。

## 街区还原工作流：主 Agent 必须落实

用户要求还原街区时，先完整读取 [详细工作流](docs/06-production/街区还原工作流.md)；以下为执行摘要，具体提示词和验收条件在该文档维护。仅规划或局部修改时按请求范围执行。

1. **确定范围与数据骨架**：建立 block_id、边界和验证路线；复用本仓库数据，再从 game-research 只读提取缺少的地图、建筑、公交与港铁资料到街区数据包，保存来源与坐标基准。
2. **核对真实街景**：在线查找并实际查看目标街段照片，核对位置、方向、年代、站口／站位；把参考和缺口写入街区参考索引。
3. **生成 HD-2D 概念**：必要时先做灰盒镜头；把真实街景与布局引导图作为 imagegen 输入，明确横向主街、纵深巷道、前中远景、建筑重叠、真实高差、光照景深和白帽主角，不靠生成图推导地理。
4. **拆分生产资产**：按核实骨架制作地面、街道、建筑、车辆／公交、地铁口和景物贴图／分件；先验收一个商铺，生成与手工精修分开，文字独立校正。
5. **按需建模并导入 Unity**：简单几何直接搭建，复杂轮廓／UV 按需 Blender；校验尺度、材质、深度、灯光、碰撞与镜头，保留可编辑源文件及依赖版本。
6. **让街道运作**：加入有目的地的行人、按道路方向运行的车辆、公交靠站／上下客、港铁口进出，以及有始有终的生活活动和同步声景。
7. **主角试走并打磨**：用白帽主角横向往返、深入支路、经过人群与交通节点，检查遮挡、景深、通行与存档；导出独立 Player，留存截图／录像、日志、性能与未解决项。
8. **验证后交付**：更新对应街区档案及目录索引，每个完成并验证的批次按下节 commit／push。概念、数据校验或工具连接成功均不能代替实机验收。

## 所有仓库改动必须提交并推送

- 远程固定为 `git@github.com:YicongCao/Pixel-OpenWorld-HongKong.git`，默认分支 `main`。用户已授权本项目后续修改后的 commit 与 push，无需每次重复询问。
- 任何新增、修改、删除或重命名的项目文件都适用，包括文档、规则、脚本、地图、参考索引、生成／手工资产、模型、Unity 场景及 `.meta`，不只源代码。以每个完成且验证的修改批次为提交单元；持续任务在阶段完成时提交并推送，不在单次文件写入尚未完成时提交中间损坏状态。
- 开始先看工作区、当前分支、远程和远程更新；有已有用户改动时保留，不丢弃、不重置。只暂存本任务拥有的改动，不将不相关内容静默混入。
- 完成一个修改批次后，检查 diff／新增文件清单、敏感内容与大文件，执行适合改动的验证，再创建描述实际结果的 commit 并 push。生成的有效资产必须纳入版本管理；缓存、临时输出、凭证和不得再分发的参考原件不属于可推送成果。
- 常规使用普通 Git；新增大二进制前检查远程文件限制，必要时规划 Git LFS 并验证对象确实上传，不能只提交指针或直接忽略交付资产。不要把临时构建缓存当资产，也不要无授权购买存储或订阅。
- push 后核对远程目标分支的 SHA 与本地提交一致，并检查工作区。推送失败保留本地 commit，报告具体原因与未同步状态；不得宣称成功，不 force-push，不改远程地址绕过问题。
- 遇到远程新提交先获取并检查，保留双方工作后正常合并／变基；不可覆盖别人的历史。用户明确要求隔离分支时使用该分支并说明推送目标。
- 这是一项 Agent 执行规则，不是后台文件监控器：每次 Agent 完成修改必须落实提交推送；不宣称用户或外部程序保存文件后会自动实时同步。
