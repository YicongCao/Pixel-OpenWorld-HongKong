# HD-2D 镜头、受光、遮挡与景深实施

调研日期：2026-11-01。对应本仓库 Unity 6000.3.27f1、URP 17.3.0、Cinemachine 3.1.7。公开资料及本地包源码核对见 [来源索引](../06-production/hd2d-production-sources.md)。以下为实施与验证方案；本次没有创建或运行新的渲染样板。

## 1. 镜头先于正式资产

八方旅人 II 开发者明确区分固定探索镜头与事件／战斗中的大幅度镜头运动。因此不能拿宣传片的旋转镜头，推导日常探索必须支持自由环绕。[Acquire 原访谈](https://www.unrealengine.com/developer-interviews/octopath-traveler-ii-builds-a-bigger-bolder-world-in-its-stunning-hd-2d-style?lang=en-US)

**项目方案**：探索保持区内稳定方位，平移跟随；主街与支巷需要时用受约束的镜头区衔接。先以弱透视为主要实现候选，再比较正交。俯角 25–35°仅为已有美术实验范围，不是官方参数。镜头合同记录投影、俯角、方位、FOV／正交尺寸、跟随距离、构图偏移、目标分辨率和主角屏幕像素高度；任何改动都检查受影响的角色视图和建筑侧面。

灰盒中摆主街、纵深巷道、商铺转角、前景雨篷和白帽占位体，从以下机位验证：主街两端、路口、巷道入口／深处。画面应同时读出当前位置和可到达的下一处；前景不封死通道，远楼不假装可达。不是往画面上下加模糊条就完成空间层次。

## 2. Cinemachine 的职责和项目需要补的部分

Cinemachine 3.1 的 Position Composer 可以移动相机维持目标构图，同时不改变相机旋转，支持透视与正交；适合作为稳定探索镜头的起点。先跟随角色逻辑根上的稳定目标，不跟随走路上下跳动的帽尖。调 dead zone 和 damping 后再考虑 lookahead，避免放大动画噪声。[Position Composer 官方说明](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html)

本项目需要实现／配置的部分：街段触发区、相机可移动范围、入口和出口的滞回、近景遮挡策略、输入基底、焦点平滑。Cinemachine 的 Custom Blends 可定义镜头对的曲线和时长，但不会自动理解道路或人物贴图的朝向；其镜头匹配按名称进行，改名后检查配置。[Blender Settings 官方说明](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineBlending.html)

先在主街 → 巷道 → 返回的路线测试镜头边界，往返越界而不反复切换。按住同一移动键时不能突然反向；需要调整镜头方位时同步验证角色方向选择和输入基底，不只观察镜头插值是否流畅。自动绕障碍旋转相机可能暴露未制作的角色视角和建筑背面，不能直接当作默认避障方案。

## 3. 像素人物如何在 3D 世界里站稳

首轮采用逻辑根节点负责世界位置／碰撞，视觉面片以脚底为锚点，仅绕世界竖轴朝向相机。其优点是保持站立；代价是俯拍时会有投影压缩，必须配合按游戏俯角绘制的精灵实测。不能把“billboard”理解为所有轴随相机旋转，再容忍人物脚底穿地。

Gemdrops 在 SO2R 中为像素面片专门处理了穿插、受光和独立投影，说明这些并非导入 PNG 后自动成立。该项目背景偏写实，我们只借鉴问题识别和验证方法，不照搬其复杂深度重置等特定补丁。[Unity 商业案例原访谈](https://cgworld.jp/article/202404-so2r-1.html)

**材质小样顺序**：

1. 先以 URP 3D Lit／相容 Shader Graph 的不透明表面＋alpha clipping 验证硬边轮廓；SpriteRenderer 若采用专用着色器，单独确认支持当前 Universal Renderer 的 3D 灯光，不能按名字认定 Sprite-Lit 材质通用。
2. 检查颜色、深度和投影通道使用相同帧、UV 和裁切阈值。当前 Lit 源码包含 ForwardLit、DepthOnly、DepthNormals、ShadowCaster；自定义动画／billboard 顶点变形也必须在所需通道一致应用，不能只有彩色画面移动。
3. 用墙体、栏杆、雨篷、第二个角色测试前后关系；真实 Z 深度为基础。若面片互穿，先检查尺度、锚点、姿态和最小间距，再决定局部处理，不能直接全场景改为 Y-sort。
4. 侧光测试投影；可见面片的薄片阴影不成立时，比较独立投影代理与简化接触阴影。代理应随角色动作、足底和光源一致变化，不能旋转可见人物来迁就阴影。
5. 最后才加局部法线、受控边缘光、遮挡淡出。法线能改变受光，不能生成真实人物侧面或正确的人体体积。

Alpha clipping 是阈值丢弃像素；半透明混合是另一类表面行为。玻璃、雨粒、树叶淡出须分别检查深度与后处理，不能用一套透明材质包办。[URP Lit 官方参数](https://docs.unity3d.com/6000.3/Documentation/Manual/urp/lit-shader.html)

## 4. 光照先清楚，景深最后加

先关 Bloom、DOF 和强烈调色，用白天主光检查地面接触、楼体体积和角色明暗。再做夜间路灯／橱窗侧光；招牌 emission、照亮行人与路面的实际灯光、Bloom 光晕分开调试。招牌明亮不等于会自动照亮路面。

本项目试作先控制实时灯数量，再比较静态烘焙和动态人物使用的探针方案；如果切日夜，分别检查烘焙结果与当前光源是否矛盾。无须先实现光追或完整实时 GI。资产颜色图保留受光余地，避免生成图里的固定强阴影与引擎重复叠加。

## 5. 景深的实际配置与限制

URP 官方文档区分：**Gaussian 只做远景虚化**，较快且半径受限；**Bokeh 支持近景和远景虚化**，成本较高。因此“前景树叶／雨篷虚化＋中景人物清楚＋远楼虚化”应验证 Bokeh，低配置远景方案可比较 Gaussian。不能要求两者画面完全等价。[Unity 6.3 景深模式](https://docs.unity.com/en-us/engine/6000.3/manual/post-processing-and-full-screen-effects/urp/effect-list/depth-of-field-urp/volume-override)

执行顺序：

1. 在当前 URP 相机启用后处理，添加 Volume 的 Depth of Field override，确认相机 Volume Mask 与权重确实作用于该配置。先看场景深度调试结果，确保人物没有缺席、透明物件没有错误地借用背景深度。
2. 用前、中、后三个有深度间隔的测试物验证效果，再放回雨篷、主角、后排楼。没有真实深度分隔时，调模糊强度不能凭空制造纵深。
3. Gaussian 调 Start／End；Bokeh 调 Focus Distance、Focal Length、Aperture。其焦距是该效果的参数，不能假定会自动跟随 Cinemachine 的 FOV。每次改变镜头构图都复核景深。[参数参考](https://docs.unity.com/en-us/engine/6000.3/manual/post-processing-and-full-screen-effects/urp/effect-list/depth-of-field-urp/volume-override-reference)
4. 焦点跟随稳定的主角／可走区目标，不跟随每帧动画起伏。依据当前包的 eye-depth 算法，采用 `dot(target - cameraPosition, cameraForward)` 的正向相机空间深度作为初始距离，而非无条件用欧氏距离；限制范围并平滑变化，检查斜向偏移时仍清楚。这是由源码推导的项目实现方案，未在样板验收。
5. 留出可玩的清晰区域，验证主角、路缘、站口编号、转弯入口都可读；必要时减弱模糊而不是只对着人物拉极薄焦平面。UI 独立检查，提供关闭景深选项。
6. 比较同机位 DOF 关闭／Gaussian／Bokeh 的截图、行走录像和独立 Player 帧时。特别检查白帽边缘漏色、玻璃、树叶、车窗、斜向像素闪烁和切镜拉焦；不能只验收静态截图。

**正交限制须优先处理**：本地 URP 17.3.0 的两种 DOF shader 使用 `LinearEyeDepth(depth, _ZBufferParams)`；对应 Core 函数注明不适用于正交投影。这是当前源码层面的明确风险，不是一次实机失败报告。因此先以弱透视做原生 DOF 样板；保留正交方案时，必须另验正交深度重建／替代效果与维护成本，不能承诺原生景深直接正确。源码路径与哈希见 [本地证据](../06-production/hd2d-production-sources.md)。

所谓“微缩／移轴感”在这里是美术目标；URP 的标准景深并不等于物理倾斜镜头的焦平面。不要把屏幕渐变模糊当作真实深度证据，也不要据此改变尖沙咀的实际坡度。

## 6. 合并验收与回退

角色先通过无景深的深度／受光检查，建筑先通过无景深的轮廓／像素密度检查；再合并最终光照与 DOF。出现问题时依次关 DOF、Bloom、半透明和自定义受光，定位到具体环节，不用更强模糊遮掩错误。

交付 Camera／Volume 配置、角色材质与投影策略、截图机位、路线、日志及版本。性能记录必须包含实际分辨率、灯光和人数、CPU／GPU 帧时是否可获取；未测值写缺失，不填推测数字。此门槛不替代 [首版整体验收](../07-validation/acceptance.md)。

## 7. 小场景光照烘焙与交付

2026-11-01 补充。用户要求在小场景精修最后加入实际烘焙。它负责稳定的间接反弹光、固定店内灯与接触层次；动态直射阴影仍实时计算，不把“烘焙”说成取消所有灯光成本。

Unity 的 Mixed 模式可以把间接光预计算，并保留实时直射；Light Probes 向移动物体提供烘焙间接光。探针应覆盖明暗过渡与人物的活动体积，不能只沿地面铺一条线。[Light Modes](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/lighting-light-sources/components/configuring/light-modes/choose) · [Light Probes](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/light-probes/light-probes)

本机是 Apple Silicon：Unity 6000.3 官方说明该编辑器不支持 CPU Progressive Lightmapper，使用 Progressive GPU；这不是对其他平台的统一要求。[后端支持说明](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/configure/progressive-lightmapper)

本次广东道小样采用：Mixed / Baked Indirect 主光，Baked 店内点光，Directional lightmap，8 texels/m 基准密度、1024 atlas、4 px padding、32 direct / 128 indirect / 64 environment samples、最多 3 次反弹。大部分静态物体 scaleInLightmap=0.55，高层远景=0.15；这些是小样成本参数，尚非最终画质标准。380 个静态渲染对象接入一组颜色／方向贴图，258 个探针覆盖主街与支路，动态角色与车体不参与静态烘焙。实际产物及独立运行验证见 [精修记录](../08-blocks/tst-canton-demo/polish-v1.md)。

执行条件：静态网格需要 UV2、Contribute GI、正确的接收模式；小装饰可使用探针减轻 lightmap 成本。保存 `.lighting`、场景、LightingData、贴图和 `.meta`；不能只保存截图或 Library 缓存。`Lightmapping.BakeAsync()` 只表示启动，必须检查 `isRunning` 结束、产物数量／路径、动态对象排除与 Player 中的数据加载。[场景设置](https://docs.unity.com/en-us/engine/6000.3/manual/lighting-overview/direct-and-indirect-lighting/lightmapping/baking-before-runtime/lightmapping) · [BakeAsync API](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityeditor/lightmapping/bakeasync)

最终步骤是烘焙后复验：同机位对照、无 DOF 的近处接缝／漏光检查、主角和车辆进出阴影、路径往返及帧时。第一轮低分辨率试烘不算最终交付。改灯、静态物体或影响反弹的材质后重烘；一个日景 lightmap 不能搭配夜间太阳当作已完成夜景。当前精修场景锁定午后，原 demo 的实时昼夜独立保留。
