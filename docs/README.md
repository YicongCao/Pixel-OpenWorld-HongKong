# 香港 HD-2D 研究总目录

本目录是工作流依赖的静态知识库，按八个主题保存事实、研究、设计建议、计划、资产清单与实施证据。**执行步骤统一在 [workflows](../workflows/README.md)，docs 是名词，workflows 是动词。** 当前已有可运行广东道切片及午后烘焙，但没有通过完整 v0.1 与最终美术验收的街区；研究建议和实际测试分开标识。

浏览器阅读从 [项目总纲图文版](01-vision/index.html) 进入；左侧可切换八章。每章 HTML 只包含自己的主题，图片／地图在所属页面内嵌，跨章跳转需保留目录结构。Markdown 是维护源，HTML 由脚本生成。

## 主章节

| 章节 | 回答的问题 | 研究源入口 | 图文版 |
| --- | --- | --- | --- |
| 01 项目总纲 | 做什么、为什么做、首版不做什么？ | [目标与范围](01-vision/README.md) | [打开](01-vision/index.html) |
| 02 地理与交通 | 真实地点在哪里，数据能证明什么？ | [地图、地标、交通与来源](02-geography/README.md) | [打开](02-geography/index.html) |
| 03 HD-2D 空间与美术 | 怎么看、怎么走、像素与纵深怎样统一？ | [镜头、尺度、规范与概念](03-hd2d/README.md) | [打开](03-hd2d/index.html) |
| 04 引擎与运行架构 | 选什么技术，哪些组件复用、哪些待验证？ | [引擎、Agent、渲染与运行](04-engineering/README.md) | [打开](04-engineering/index.html) |
| 05 街道生活 | 怎样让香港街道有设施、有行为、有声音？ | [资产族、活动与时间天气](05-street-life/README.md) | [打开](05-street-life/index.html) |
| 06 生产研究与实施规划 | 生产依据、资产预算与版本计划是什么？ | [Roadmap、首版与生产](06-production/README.md) | [打开](06-production/index.html) |
| 07 验证与证据 | 怎么验收，哪些结果真的测过？ | [验收、历史记录与当前检查](07-validation/README.md) | [打开](07-validation/index.html) |
| 08 街区档案 | 某个具体街区进行到哪里？ | [实施归档规范](08-blocks/README.md) | [打开](08-blocks/index.html) |

## 当前实施规划

- [远期 Roadmap](06-production/roadmap.md)：从单街段到九龙步行网络的版本范围和扩区门槛。
- [首街区：弥敦道 × 海防道／A1](08-blocks/tst-nathan-haiphong/plan.md)：选段、扩展路线和真实参考；[三张 HD-2D 概念](08-blocks/tst-nathan-haiphong/assets.md)。
- [首个可玩版本 v0.1](06-production/v0.1-implementation.md)：选段、灰盒、美术、街道生活、存档与实机交付的实施顺序。

## 制作流程的外部入口与研究依据

- [素材建模与拼装流程](../workflows/10-hd2d/hd2d-production-workflow.md)：从镜头合同到可行走小样，先验收再扩产。
- [角色多方向与动作](../workflows/20-assets/character-production.md)、[镜头／灯光／景深](../workflows/10-hd2d/camera-lighting.md)：分别维护生产细则。
- [专业来源与本地源码证据](06-production/hd2d-production-sources.md)：查阅于 2026-11-01；流程研究不等于已完成资产或实机验收。

## 给 Codex 的阅读入口

- [Agent.md](Agent.md)：研究资料的读取顺序、证据等级和维护规则。
- [Skill.md](Skill.md)：根据所选 workflow 定位依赖的研究主题。
- [街区还原工作流](../workflows/00-project/街区还原工作流.md)：真实数据 → 街景参考 → imagegen → 资产／建模 → Unity → 动线验证的完整执行流程。提交推送规则见根目录 [AGENTS.md](../AGENTS.md)。

首次进入读 01 和相关主题摘要；具体任务按 Skill.md 定位，不要求每次把全部调研塞入上下文。引擎／包版本、商铺与通行条件等会变化的结论，实施前重新核对。

## 维护与复现

- 修改对应主题 Markdown，不直接手改生成的 HTML；`scripts/docs_catalog.json` 是章节渲染清单。
- 重建：`../game-research/.venv-hk-data/bin/python scripts/build_report.py`。
- 检查：`python3 scripts/validate_docs.py`；检查结果写入第 07 章，不触碰上游数据。
- 原始地图与许可仍位于 [data/hong-kong](../data/hong-kong/README.md)，概念源图位于 `assets/concepts/` 和 `assets/blocks/`，不为整理文档重复搬动原数据与生产资产。
- 旧长页和混合文档已被本目录替代；章节去向、图件哈希与原提交见 [迁移记录](07-validation/migration-map.json)。历史校验保留原文，只代表当时的报告。

## 本轮补充

- [八方旅人 II 访谈事实与流程启发](03-hd2d/octopath2-interview-lessons.md)。
- [香港标识来源与缺口](05-street-life/hong-kong-signage-reference.md)。
- [广东道资产目录与剩余工序](08-blocks/tst-canton-demo/scene-inventory.md)。
- [工作流拆分记录](07-validation/workflow-reorganization.md) 与 [路径迁移映射](07-validation/workflow-migration.json)。
