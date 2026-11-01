# 验证记录及证据边界

## 已有历史记录

下列 JSON 按原字节迁移，属于旧版研究交付的记录，不是当前分章 HTML 或可玩游戏的新测试结果。旧 HTML 的哈希、文件数和路径应结合原提交读取，不能改成新结果。

- [初版数据与报告交付](history/delivery-validation.json)：旧四张图报告与数据检查。
- [街道生活补充](history/street-life-validation.json)：入口候选、锚点、历史地图交互检查。
- [HD-2D 报告检查](history/hd2d-validation.json)：旧五张概念图＋地图对照报告。
- [源数据包校验](../../data/hong-kong/validation.json)：数据提取的校验；没有证明实机功能。

## 本次文档整理

- [迁移映射](migration-map.json)：原提交、章节去向、图件和历史记录的原始哈希。
- [当前文档结构校验](docs-validation.json)：分章目录、内部链接、图片、来源与派生文件哈希；由 `scripts/validate_docs.py` 生成。
- [浏览器检查](browser-review.json)：仅记录这次真实执行的页面和交互检查。

当前仍无可玩街区、Unity Player 性能或用户主观画面验收记录。后续每个街区结果应存入 [街区档案](../08-blocks/README.md) 的独立目录，不能以本页检查替代实机验证。
