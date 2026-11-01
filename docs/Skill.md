# 按任务读取研究的操作指南

用途：让 Codex 在调研、修改策划或制作街区时，快速定位已知结论、证据和待验证项。此文件通过根目录指引显式读取，是项目内任务阅读指南；不假定仅凭 `Skill.md` 文件名就被宿主自动注册为技能。

## 任务路由

| 当前任务 | 先读 | 按需补读 | 产出归属 |
| --- | --- | --- | --- |
| 理解项目／调整范围 | [01 目标](01-vision/vision.md) | [06 路线图](06-production/roadmap.md) | 01，必要时同步 06 |
| 选择街区／提取地图／查站口线路 | [02 数据盘点](02-geography/data-inventory.md) | [02 地标](02-geography/scope-landmarks.md)、[数据 README](../data/hong-kong/README.md) 与 manifest | 数据包＋02，具体选段记 08 |
| 设计横向主街／纵深／高差 | [03 街网与镜头](03-hd2d/street-camera.md) | 02 地理来源、[04 架构](04-engineering/runtime-architecture.md) | 03，具体镜头区记 08 |
| imagegen／角色／纹理／概念迭代 | [03 美术](03-hd2d/art-direction.md) | [旧概念档案](03-hd2d/concept-gallery.md)、[06 生产](06-production/asset-pipeline.md)、街区还原工作流的参考图和提示词步骤 | 资产目录＋03／08 |
| 制作角色多视角／站姿／步行图集 | [03 角色制作](03-hd2d/character-production.md) | [06 生产 SOP](06-production/hd2d-production-workflow.md)、[专业资料](06-production/hd2d-production-sources.md) | 资产源与导出＋08 验证 |
| 建模／UV／贴图／Unity 拼装 | [06 生产 SOP](06-production/hd2d-production-workflow.md) | [04 镜头与渲染](04-engineering/hd2d-camera-lighting.md)、[专业资料](06-production/hd2d-production-sources.md) | 资产／Unity＋08 验证 |
| 引擎／框架／MCP／光照景深 | [04 引擎复评](04-engineering/engine-selection.md) | [04 镜头与渲染](04-engineering/hd2d-camera-lighting.md)、[04 架构](04-engineering/runtime-architecture.md)、[06 小样](06-production/prototype.md) | 04，真实测试记 07／08 |
| 人流／汽车／公交／地铁口／街头活动 | [05 街道生活](05-street-life/street-life.md) | 02 站位与连接、04 运行结构 | 05，具体实例记 08 |
| 规划首版／拆解实施任务 | [06 首版实施](06-production/v0.1-implementation.md) | [06 Roadmap](06-production/roadmap.md)、[07 验收](07-validation/acceptance.md) | 06 规划，实际进度记 08 |
| 制作某街区／安排下一阶段 | [街区还原工作流](06-production/街区还原工作流.md) | [06 路线图](06-production/roadmap.md)、[08 归档](08-blocks/README.md) | 08 计划与实施记录 |
| 行走验证／性能／结果复核 | [07 验收](07-validation/acceptance.md) | [07 证据说明](07-validation/evidence.md)、目标街区记录 | 07 通用结论，08 实际结果 |
| 维护报告／移动研究资料 | [Agent.md](Agent.md) | [总目录](README.md)、`scripts/docs_catalog.json` | 对应主题＋必要索引与校验 |

## 执行方法

1. 从路由表选定主主题，读取对应研究原文及其中直接相关的来源；不要把旧 HTML 当成唯一事实来源。
2. 列出本任务可复用的结论、需要复核的时效信息、真实缺口和必要验证。用户已经确定的 HD-2D、白帽主角、香港街道生活目标继续保留。
3. 优先使用已有数据和资产。涉及位置／通行先查地图与官方图；涉及真实外观先核对街景；涉及当前技术接口查安装版本和官方资料。当前文档没有提供的事实不能编造。
4. 仅在执行对应操作时读取需要的工具技能，例如 imagegen、Blender 或发布工具；以当前工具 schema 为准，文档不承诺接口已经安装可用。
5. 修改内容的所属主题，其他主题使用交叉链接；若新结论取代旧建议，写清取代范围与仍未验证的部分。保留原证据，不篡改历史测试。
6. 重建图文版并运行文档检查；按影响范围实测页面或游戏。完成后遵循根目录 AGENTS.md 的 commit／push 规则。

## 文档维护命令

从仓库根目录执行：

```sh
../game-research/.venv-hk-data/bin/python scripts/build_report.py
python3 scripts/validate_docs.py
```

前者按清单生成各主题的 `index.html` 与地理章节地图；后者检查目录约束、内部引用、图片、来源和迁移哈希，写入第 07 章。需要中文地图重新绘制时才运行 `scripts/build_hd2d_study.py`；完整 GIS 重提取与 FGDB 校验不是每次编辑文档的必需步骤。
