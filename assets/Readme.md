# 资产目录

本目录保存视觉资产源文件与来源记录。当前有五张早期探索图和首街区三张新参考图（另保留一张弃用迭代），尚无已验收的游戏贴图、角色动画、模型或 Unity 资源包。操作前读 [Agent.md](Agent.md)，项目约定见 [AGENTS.md](../AGENTS.md)。

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

首批：[弥敦道 × 海防道概念与输入记录](blocks/tst-nathan-haiphong/generated/concepts/manifest.json)，用途与偏差见 [检查记录](../docs/08-blocks/tst-nathan-haiphong/assets.md)。`authored/` 尚未创建。
