# 香港 HD-2D 研究总目录

本目录按八个主题组织研究。**当前只有资料、策划和概念图，没有已验收的可玩街区。** 研究基线为 2026-11-01 的策划 v0.4；新增产品 Roadmap 与首个可玩版本 v0.1 的实施规划。策划版本号不表示游戏发布进度，候选方案仍须实机验证。

浏览器阅读从 [项目总纲图文版](01-vision/index.html) 进入；左侧可切换八章。每章 HTML 只包含自己的主题，图片／地图在所属页面内嵌，跨章跳转需保留目录结构。Markdown 是维护源，HTML 由脚本生成。

## 主章节

| 章节 | 回答的问题 | 研究源入口 | 图文版 |
| --- | --- | --- | --- |
| 01 项目总纲 | 做什么、为什么做、首版不做什么？ | [目标与范围](01-vision/README.md) | [打开](01-vision/index.html) |
| 02 地理与交通 | 真实地点在哪里，数据能证明什么？ | [地图、地标、交通与来源](02-geography/README.md) | [打开](02-geography/index.html) |
| 03 HD-2D 空间与美术 | 怎么看、怎么走、像素与纵深怎样统一？ | [镜头、尺度、规范与概念](03-hd2d/README.md) | [打开](03-hd2d/index.html) |
| 04 引擎与运行架构 | 选什么技术，哪些组件复用、哪些待验证？ | [引擎、Agent、渲染与运行](04-engineering/README.md) | [打开](04-engineering/index.html) |
| 05 街道生活 | 怎样让香港街道有设施、有行为、有声音？ | [资产族、活动与时间天气](05-street-life/README.md) | [打开](05-street-life/index.html) |
| 06 资产生产与实施 | 先做什么，怎样形成可复用的生产方法？ | [Roadmap、首版与生产](06-production/README.md) | [打开](06-production/index.html) |
| 07 验证与证据 | 怎么验收，哪些结果真的测过？ | [验收、历史记录与当前检查](07-validation/README.md) | [打开](07-validation/index.html) |
| 08 街区档案 | 某个具体街区进行到哪里？ | [实施归档规范](08-blocks/README.md) | [打开](08-blocks/index.html) |

## 当前实施规划

- [远期 Roadmap](06-production/roadmap.md)：从单街段到九龙步行网络的版本范围和扩区门槛。
- [首个可玩版本 v0.1](06-production/v0.1-implementation.md)：选段、灰盒、美术、街道生活、存档与实机交付的实施顺序。

## 给 Codex 的阅读入口

- [Agent.md](Agent.md)：研究资料的读取顺序、证据等级和维护规则。
- [Skill.md](Skill.md)：根据当前任务选择需要阅读的主题与操作步骤。
- [根目录 agent.md](../agent.md)：真实数据 → 街景参考 → imagegen → 资产／建模 → Unity → 动线验证的完整执行流程，以及每批改动提交推送规则。

首次进入读 01 和相关主题摘要；具体任务按 Skill.md 定位，不要求每次把全部调研塞入上下文。引擎／包版本、商铺与通行条件等会变化的结论，实施前重新核对。

## 维护与复现

- 修改对应主题 Markdown，不直接手改生成的 HTML；`scripts/docs_catalog.json` 是章节渲染清单。
- 重建：`../game-research/.venv-hk-data/bin/python scripts/build_report.py`。
- 检查：`python3 scripts/validate_docs.py`；检查结果写入第 07 章，不触碰上游数据。
- 原始地图与许可仍位于 [data/hong-kong](../data/hong-kong/README.md)，概念源图仍位于 `assets/concepts/`，不为整理文档重复搬动原数据与生产资产。
- 旧长页和混合文档已被本目录替代；章节去向、图件哈希与原提交见 [迁移记录](07-validation/migration-map.json)。历史校验保留原文，只代表当时的报告。
