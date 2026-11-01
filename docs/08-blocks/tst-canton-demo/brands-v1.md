# 广东道品牌店招 · 第 01 批

2026-11-01。用户要求以真实商标增强熟悉感。本批将精修场景的泛称店招替换为两组经过官方门店资料核对的品牌识别，保留原镜头、行走范围和交通逻辑。

## 实际落地

- **Apple 广东道店**：官方地址为广东道 100 号。以官方店面照片为图生图参考，制作透明底白色缺口苹果与独立叶片，替换原先的普通“Apple”文字，悬挂在两层玻璃店面中部。这是游戏化图形版本，不是官方矢量母版。
- **莎莎广东道店**：官网实时门店列表“九龙 → 油尖旺区”显示地址为 **广东道 86–98 号文利大厦地下 4–5 号铺**。采用官网洋红色 SaSa 标准字，参考该店官方照片的浅银蓝门楣、深色边框和明亮化妆品陈列，替换“日常百貨／茶·食”两组占位店名。
- 原第三组虚构“九龍商行”文字换成已核实的大厦名称“文利大廈”，作为入口标识。入口位置和外形仍是设计，不声称已核实真实大堂。
- 第一轮实机发现树冠遮住莎莎标志左侧；将独立标志向同一门楣右侧移动到 X=4.8，不移动街树或改变通行空间。

两店门牌得到核实，但没有测绘连续立面：当前 30 米片段中的邻接、宽度、入口与树木位置仍是压缩编排。官网照片没有明确拍摄日期，不能据此声称整个街面在当日完全相同。历史资料中的莎莎 68–80 号店不混入当前 86–98 号记录。

## 来源与资产

- [Apple 官方店页](https://www.apple.com/hk/en/retail/cantonroad/)及其[官方外观照片](https://rtlimages.apple.com/cmc/dieter/store/16_9/R499.png?interpolation=progressive-bicubic&output-format=jpg&output-quality=85&resize=2880%3A1612)。照片只作本地参考，不随仓库再分发。
- [莎莎官方门店目录](https://www.sasa.com.hk/v2/Shop/StoreList/17)：通过实际加载的可见门店列表核对，静态网页抓取只有空壳，不能当作不存在门店的证据。
- [莎莎该店实景照片](https://img2.cdn.91app.hk/o2o/image/593ffec4-f070-4cdf-a397-6f553f5dc974.jpg)与[官网标识原件](https://cms-static.cdn.91app.hk/images/original/17/77cbf5b3-54bb-4710-a383-4e4f36302618-1765160784-uecg2dclbo_d.png)。照片只作参考；标识原件保存 SHA256。
- [Apple 生成源与提示词](../../../assets/blocks/tst-canton-demo/generated/brands-v1/manifest.json)；[莎莎采用原件与来源](../../../assets/blocks/tst-canton-demo/authored/brands-v1/manifest.json)。品牌标识不适用本项目代码 MIT 或此前材质图集的 CC BY-SA 声明，归属说明随 Player 一并提供。

## 制作方法

店招作为独立面片／门楣组件，不画死进建筑大贴图。Apple 使用一次 imagegen 参考生成；莎莎直接采用官方图形，避免生成模型改坏标准字。Unity UV 只选择洋红标志区域，排除原图下方灰色标语，保留原 PNG 像素。

纹理 Point + mipmaps、最大 256、alpha clip 0.5；招牌低强度 emission 保持辨识度，招牌面片不烘焙 GI、不投影。静态门楣和柜台参与 GI，重新烘焙；小商品与动态角色不加入静态 lightmap。不能靠高 Bloom 把轮廓照成一团。品牌尺寸和颜色在实机截图检查，后续仍需整理像素边缘和店内陈列。

`AgentScripts/BrandCanton.cs` 的 `Apply` 为一次性修订，检测已有根节点后拒绝覆盖；`AdjustVisibility` 仅移动招牌并更新位置记录。使用 Unity CLI / Pipeline 的 `commands/scripts.md`、`commands/build-and-compilation.md` 和 `commands/capture.md` 工作流，无新增插件。采用资产、材质、场景、烘焙数据一起保存。

## 验证与下一步

本批独立 Player、截图、构建和烘焙证据见 [验证目录](brand-validation/README.md)，保留上一批结果不覆盖。当前只落实 Apple 与莎莎；7-Eleven、麦当劳、银行等在未来所处街段核实后再加入，不能为“像香港”随机挂在未确认铺位。

后续补连续街景、准确铺位边界、招牌厚度与侧招、开关门状态、店内品类陈列和橱窗反射；地图事实与设计编排继续分开。商标辨识度是场景验收项，不能以普通店名文字或生成图里的错字代替。
