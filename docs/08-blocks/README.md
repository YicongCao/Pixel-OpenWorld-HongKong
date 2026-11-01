# 08 · 街区档案

每个实际制作街区的计划、证据、资产与实机结果。

[研究总目录](../README.md) · [本章图文版](index.html)

## 当前状态

已建立 [弥敦道 × 海防道／A1 街区计划](tst-nathan-haiphong/plan.md)、[来源](tst-nathan-haiphong/sources.md) 与 [三张 HD-2D 概念及资产检查](tst-nathan-haiphong/assets.md)。已进入 [实施批次 01](tst-nathan-haiphong/implementation.md)：有可走路线和 A1 早期实机样板，尚无完整 v0.1 的街道生活及验收。

## 新街区的归档约定

每次按稳定 `block_id` 创建子目录。用 `plan.md` 记录范围、地图边界、镜头方向、主角路线、活动、依赖及缺口；`sources.md` 引用本仓库数据与真实街景证据；`assets.md` 记录资产清单与生产路径；`validation/` 保存真实实机记录。

地理原件仍在 `data/blocks/<block_id>/`，实景索引在 `references/blocks/<block_id>/`，资产在 `assets/blocks/<block_id>/`；本目录只存对应研究与实施记录。源文件不因文档归档而重复复制。

执行顺序见 [街区还原工作流](../../workflows/00-project/街区还原工作流.md)，阅读路线见 [docs/Skill.md](../Skill.md)。新增街区时更新本章 README 的列表；通用结论回写所属主题章节，避免把全部研究继续堆在街区计划里。

- [广东道复用 demo](tst-canton-demo/README.md)：本轮快速可玩切片，压缩编排与实际验证单列。

- [广东道 30 米精修与烘焙](tst-canton-demo/polish-v1.md)：独立 authored 场景、生产图集、浅室内、像素人流与 GPU lightmap／探针。
