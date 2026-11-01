# 白帽漫游：像素九龙

香港 HD-2D 像素步行开放世界（像素角色＋立体街景）的前期研究。当前没有可玩游戏版本。

- [Agent 街区还原工作流](agent.md)：数据骨架 → 实景参考 → HD-2D 概念 → 资产／建模 → Unity 街道生活 → 主角动线验证；每个完成并验证的修改批次提交并推送。

- [离线图文策划案](docs/香港像素漫游策划案.html)：概念图、交互式源数据地图、方案与引擎比较，图片内嵌，可单独复制打开。
- [创意与技术策划案 Markdown](docs/创意与技术策划案.md)
- [香港街道生活与沉浸感设计](docs/research/香港街道生活与沉浸感设计.md)：扩展地标、15 类资产族、16 类活动、声景与验收。
- [街道设施与站口补充数据](data/hong-kong/street-life/README.md)
- [HD-2D 空间与技术方案](docs/research/HD-2D空间与技术方案.md)：真实街网与镜头对照、横向／纵深组织、3D 渲染、组件验证。
- [引擎调研](docs/research/引擎调研.md)
- [香港选区资料包](data/hong-kong/README.md)
- [imagegen 提示词](assets/concepts/prompts.json)

数据来自隔壁 `game-research` 的已有香港公开资料快照，本项目保存选区派生数据与必要参考文件。五张概念图都是探索图，不是实机画面或生产资源；旧四张保留为历史研究，新增 [HD-2D 街景](assets/concepts/hd2d/05-canton-road-hd2d.png) 及其 [完整提示词](assets/concepts/hd2d/prompt.txt)。主选验证 Unity 3D 管线，Unreal 为主要备选，尚未锁定框架与版本。

复现提取：`../game-research/.venv-hk-data/bin/python scripts/extract_hk.py`。完整检查：`../game-research/.venv-hk-data/bin/python scripts/validate_delivery.py`。离线报告生成：`../game-research/.venv-hk-data/bin/python scripts/build_report.py`。

街道生活补充提取：`../game-research/.venv-hk-data/bin/python scripts/extract_street_life.py`；结果校验记录见 [street-life-validation.json](docs/research/street-life-validation.json)。

HD-2D 道路对照图复现：`../game-research/.venv-hk-data/bin/python scripts/build_hd2d_study.py`；输出和边界见 [走廊资料包](data/hong-kong/hd2d-study/README.md)。
