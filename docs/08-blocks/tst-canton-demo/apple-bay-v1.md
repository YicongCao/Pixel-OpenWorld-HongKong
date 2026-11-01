# Apple 5.8 米橱窗样板 · 第 01 批

2026-11-01。承接 [同机位目标与差距分析](concept-gap-v1.md)，集中精修 Apple 中间一跨，设计坐标 X=-15.9…-10.1，复用到上下两层。镜头、主角大小、街道布局和莎莎保持原方案；两侧旧桌面与空白后墙仍是原型，用于判断下一步扩产价值。门牌参考真实店面，室内陈列和尺寸仍是设计解释。

## 做了什么

- 用独立的后墙、天花凹槽、地板饰面、陈列框、低柜和搁板建立浅室内；增加笔记本、平板、手机的底座／边框／屏幕分件与桌面小牌。桌面顶高约距样板地板一米，替换原先偏矮的桌组。
- 生成一张四格像素材质图集，分别为浅橡木、冷灰石材、配件展示和屏幕图案；由 Unity 提取成四张贴图，Point＋mipmaps，最大导入边长 128／256。商品图的轻微画内阴影只作为小型陈列细节，不能宣称全是无光照材质。
- 将原整面透明薄立方体的 Renderer 关闭，保留碰撞。拆成左、右原材质区和中间两张新单面玻璃；新玻璃保持低 alpha，并按本地 URP 17.3 `BaseShaderGUI` 设置 Preserve Specular，避免透明度一起削弱反射高光。玻璃不参与静态 GI 或投影。
- 补两盏 Baked 矩形灯、灯带、局部 box-projected 烘焙反射探针，以及 36 个室内 Light Probes。烘焙后再保存反射贴图；总探针由 258 增至 294。金属、木料、背板分别调色和粗糙度。
- 第一轮检查后修正上层饰面高差，并降低浅色木材与配件背板的亮度。原上层地板顶面为 6.11 米，样板饰面中心修到 6.14 米；模板不能简单假设上下层地面始终相差 6 米。

素材：[生成源／提示词／哈希](../../../assets/blocks/tst-canton-demo/generated/apple-bay-v1/manifest.json)。输入为已保存的目标概念，借用其色彩方向；对应官方照片证据链沿用 [品牌实施](brands-v1.md)。本批内置 imagegen 一次；简单分件直接用 Unity 制作，无新增插件或 Blender 依赖。

## 文件与重现

- 场景仍为 `Assets/HongKong/Authored/CantonPolish/CantonRoadPolish.unity`。
- 采用材质／反射在 `Authored/CantonPolish/AppleBayV1/`；图集与四张派生贴图在 `Generated/AppleBayV1/`。原生成源完整保存，未覆盖早期图集。
- `AgentScripts/AppleBay.cs`：`Apply` 创建样板，已有目录或根节点即拒绝覆盖；`Refine` 为一次性校正，已有校正标记即拒绝重复。现有场景不要再次执行这两个方法。
- 完整生产顺序为 Apply → Refine → `PolishCanton.Bake`／BakeStatus → `AppleBay.BakeReflection` → `FinishCantonPolish.Audit` → 独立构建与验证。首轮未校正预览只作过程记录。
- 新增可选验证参数 `-streetAppleBay`，与 `-streetSmokeTest -streetOutput <目录>` 配合，完成原路线后实际走到 X=-17、-13、-9、-13 的四个点，再保存景深开启对照；检查反射贴图在 Player 中加载。日常启动不执行该流程。

使用 Unity CLI／Pipeline 技能与安装版本的 `commands/scripts.md`、`commands/capture.md`、`commands/build-and-compilation.md` 及 `playmode-verification-loop.md`。一次编译发现当前 Unity 已弃用 `LightType.Area`，按诊断改用 `Rectangle` 后才执行建物；没有带错误构建。

## 验证与限制

本批截图、烘焙、构建与独立行走结果见 [验证目录](apple-bay-validation/README.md)。既有品牌版和概念对照不覆盖，防止把历史实机变成新版本。自动巡检检查实际 CharacterController 行走；画面质量仍需结合固定机位截图与用户主观评审。

本批完成的是橱窗模块小样，整个 Apple 两侧仍明显简化。右侧原楼梯在样板范围外，本轮没有精修；没有新增店员或可进入商店。反射为静态探针近似，不反射动态行人，也未解决所有透明物体的景深边缘。近距离材质纹理、完整立面的节奏、角色与车辆像素统一、运动闪烁仍需后续检查，不能宣称已达到概念或《八方旅人 II》品质。

下一步先评审这组样板的明暗、材质与尺寸；通过后沿既有窗跨扩展到两侧，并独立完善实际可见的楼梯与玻璃转角，再精修莎莎陈列。不同模块保持少量变化，避免机械复制同一展示屏和商品图。
