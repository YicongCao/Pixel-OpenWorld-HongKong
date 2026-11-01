# 资产目录

本目录保存视觉资产源文件与来源记录。当前有五张早期探索图和首街区三张新参考图（另保留一张弃用迭代），新增首批角色图集、材质和 Unity 街角样板；已验证导入与行走，但尚无已验收的最终美术资产。操作前读 [Agent.md](Agent.md)，项目约定见 [AGENTS.md](../AGENTS.md)。

## 现有内容

- [concepts/manifest.json](concepts/manifest.json)：前四张街景／角色探索图的文件、尺寸和哈希；提示词见 [prompts.json](concepts/prompts.json)。
- [concepts/hd2d/manifest.json](concepts/hd2d/manifest.json)：第五张 HD-2D 构图探索及已知偏差；包括虚构的商场、站口与便利店邻接，不能据此定位设施。
- [概念图档案](../docs/03-hd2d/concept-gallery.md)：各图用途、风格变化与限制；[美术方向](../docs/03-hd2d/art-direction.md) 维护制作建议。

现有图件仅作探索，不自动满足真实街景图生图、分层资产、动画、深度或实机验收要求。

## 街区结构

- `blocks/<block_id>/generated/`：概念、贴图、图集等生成输出，附提示词、输入参考清单／哈希与工具记录。
- `blocks/<block_id>/authored/`：手工精修、可编辑模型源和采用的修订版本；生成脚本不得覆盖。
- 每个街区建立资产清单，关联 `asset_id`、`place_id/source_id`、版本、尺寸／像素密度、用途和最终采用路径；工程导入副本需能追溯到源资产。

真实照片与参考索引归 `references/blocks/<block_id>/`；地理数据归 [data](../data/Readme.md)；实机截图、录像和验收归 [街区档案](../docs/08-blocks/README.md)。本目录不是 Unity 工程 `Assets/`，导入时不混淆两者。

完整生产顺序见 [街区还原工作流](../docs/06-production/街区还原工作流.md)，首版范围见 [v0.1 规划](../docs/06-production/v0.1-implementation.md)。新增资产后更新对应 manifest 和本目录索引，按根约定验证、提交并推送。

首批：[弥敦道 × 海防道概念与输入记录](blocks/tst-nathan-haiphong/generated/concepts/manifest.json)，用途与偏差见 [检查记录](../docs/08-blocks/tst-nathan-haiphong/assets.md)。该街区 `authored/` 尚未创建。

首批生产小样：[白帽图集与哈希](blocks/tst-nathan-haiphong/generated/hero/manifest.json)、[28 帧切片](blocks/tst-nathan-haiphong/generated/hero/frames.json)、[八类材质与哈希](blocks/tst-nathan-haiphong/generated/materials/manifest.json)。采用方式、实机结果和缺口见 [实施记录](../docs/08-blocks/tst-nathan-haiphong/implementation.md)。

广东道精修：[生成源／提示词／哈希](blocks/tst-canton-demo/generated/polish-v1/manifest.json)，包含四格材质、街物与行人图集。NPC 第一稿弃用，采用去除非对称配件的第二稿；不是最终动作验收。Unity authored 场景、分件、材质和烘焙产物独立保存，见 [精修记录](../docs/08-blocks/tst-canton-demo/polish-v1.md)。

广东道品牌：[Apple 生成标识](blocks/tst-canton-demo/generated/brands-v1/manifest.json)与[莎莎官方标识采用稿](blocks/tst-canton-demo/authored/brands-v1/manifest.json)。地址、参考和使用边界见 [品牌实施](../docs/08-blocks/tst-canton-demo/brands-v1.md)，不将品牌原件套用代码或材质图集许可。

当前街段目标：[同机位 HD-2D 概念与输入记录](blocks/tst-canton-demo/generated/concepts/current-target-v1/manifest.json)。以品牌修订 Player 画面和两家店的官方照片作为输入；[对照与偏差](../docs/08-blocks/tst-canton-demo/concept-gap-v1.md)明确其为美术目标，不能当作新实机或测绘事实。
