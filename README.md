# 白帽漫游：像素九龙

香港 HD-2D 像素步行开放世界：像素角色＋立体街景＋街道生活。当前只有研究、地图资料与概念图，没有可玩版本。

- [研究总目录](docs/README.md)：八个主题的 Markdown 源文档与独立图文页。
- [从项目总纲开始阅读图文版](docs/01-vision/index.html)：通过章节导航阅读，不再拼接全部研究成长页。
- [远期 Roadmap](docs/06-production/roadmap.md)／[首个可玩版本 v0.1 实施规划](docs/06-production/v0.1-implementation.md)：版本范围、任务依赖与验收门槛。
- [Codex 阅读约定](docs/Agent.md)／[按任务读取研究](docs/Skill.md)。
- [街区还原工作流](docs/06-production/街区还原工作流.md)：数据骨架 → 实景参考 → HD-2D 图生图 → 资产／建模 → Unity 街道生活 → 主角动线验证。
- [香港选区资料包](data/hong-kong/README.md)：只读复用相邻 game-research 的公开资料快照，源数据与设计分别保存。

五张概念图是探索图，不是实机画面。新旧概念与偏差归入 [空间与美术](docs/03-hd2d/README.md)；技术选择与兼容性归入 [引擎与运行架构](docs/04-engineering/README.md)。

复现文档：`../game-research/.venv-hk-data/bin/python scripts/build_report.py`。结构／引用检查：`python3 scripts/validate_docs.py`。数据提取与完整检查命令见资料包 README，历史结果及本次迁移记录见 [验证与证据](docs/07-validation/README.md)。

每个完成并验证的项目修改批次都生成 commit 并推送；目标远程及提交规则见根目录 [AGENTS.md](AGENTS.md)。
