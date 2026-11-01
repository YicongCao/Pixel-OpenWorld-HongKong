# 按工作流读取研究的路由

用途：让 Codex 在调研、修改策划或制作街区时，快速定位已知结论、证据和待验证项。此文件通过根目录指引显式读取，是项目内任务阅读指南；不假定仅凭 `Skill.md` 文件名就被宿主自动注册为技能。

先由 [workflows 目录](../workflows/README.md) 选择主执行流程；本页仅定位它依赖的名词资料，不是另一套制作流程。

## 资料路由

| 当前任务 | 先读 | 按需补读 | 产出归属 |
| --- | --- | --- | --- |
| 理解项目／调整范围 | [01 目标](01-vision/vision.md) | [06 路线图](06-production/roadmap.md) | 01，必要时同步 06 |
| 选择街区／提取地图／查站口线路 | [02 数据盘点](02-geography/data-inventory.md) | [02 地标](02-geography/scope-landmarks.md)、[数据 README](../data/hong-kong/README.md) 与 manifest | 数据包＋02，具体选段记 08 |
| 设计横向主街／纵深／高差 | [03 街网与镜头](03-hd2d/street-camera.md) | 02 地理来源、[04 架构](04-engineering/runtime-architecture.md) | 03，具体镜头区记 08 |
| imagegen／角色／纹理／概念迭代 | [03 美术](03-hd2d/art-direction.md) | [旧概念档案](03-hd2d/concept-gallery.md)、[06 生产](06-production/asset-pipeline.md)、街区还原工作流的参考图和提示词步骤 | 资产目录＋03／08 |
| 制作角色多视角／站姿／步行图集 | [03 美术规范](03-hd2d/art-direction.md) | [WF-HD2D](../workflows/10-hd2d/hd2d-production-workflow.md)、[专业资料](06-production/hd2d-production-sources.md) | 资产源与导出＋08 验证 |
| 建模／UV／贴图／Unity 拼装 | [06 专业生产来源](06-production/hd2d-production-sources.md) | [WF-LIGHT](../workflows/10-hd2d/camera-lighting.md)、[专业资料](06-production/hd2d-production-sources.md) | 资产／Unity＋08 验证 |
| 引擎／框架／MCP／光照景深 | [04 引擎复评](04-engineering/engine-selection.md) | [WF-LIGHT](../workflows/10-hd2d/camera-lighting.md)、[04 架构](04-engineering/runtime-architecture.md)、[06 小样](06-production/prototype.md) | 04，真实测试记 07／08 |
| 人流／汽车／公交／地铁口／街头活动 | [05 街道生活](05-street-life/street-life.md) | 02 站位与连接、04 运行结构 | 05，具体实例记 08 |
| 规划首版／拆解实施任务 | [06 首版实施](06-production/v0.1-implementation.md) | [06 Roadmap](06-production/roadmap.md)、[07 验收](07-validation/acceptance.md) | 06 规划，实际进度记 08 |
| 制作某街区／安排下一阶段 | [街区还原工作流](../workflows/00-project/街区还原工作流.md) | [06 路线图](06-production/roadmap.md)、[08 归档](08-blocks/README.md) | 08 计划与实施记录 |
| 行走验证／性能／结果复核 | [07 验收](07-validation/acceptance.md) | [07 证据说明](07-validation/evidence.md)、目标街区记录 | 07 通用结论，08 实际结果 |
| 维护报告／移动研究资料 | [Agent.md](Agent.md) | [总目录](README.md)、`scripts/docs_catalog.json` | 对应主题＋必要索引与校验 |

## 维护归属

来源事实与方案差异更新本目录对应主题；通用执行方法更新所选 workflow；具体任务的产物、配置、验证与缺口写入街区档案。地图、照片、生成概念与实机证据不可混称。环境版本与工具接口按任务重新核实，不因文档写有命令便认定工具可用。

文档整理、研究更新、引用迁移和生成图文页的操作与验证统一执行 [WF-MAINTAIN](../workflows/00-project/workflow-maintenance.md)，不在本页复制命令与步骤。
