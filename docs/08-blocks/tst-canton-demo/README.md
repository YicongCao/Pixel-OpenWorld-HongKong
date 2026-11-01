# 广东道 · 首个可玩复用 demo

2026-11-01。用户本轮指定广东道；此前弥敦道／海防道 A1 保留为独立制作小样。当前目标是快速验证 HD-2D 组件在可走场景中的组合，不是完整 v0.1 或最终街区美术。

最新进展：[品牌店招修订](brands-v1.md)，在 [30 米精修与光照烘焙](polish-v1.md) 上加入 Apple 与莎莎标识。新场景 `Authored/CantonPolish/CantonRoadPolish.unity`，本机 `.local/builds/HongKong-Canton-Polish.app`；固定午后、F 景深、P 像素密度。以下描述保留原快速 demo，避免混淆两版。

## 打开与操作

Unity 场景：`Assets/HongKong/Generated/CantonDemo/CantonRoadDemo.unity`。

独立 macOS app：仓库本机 `.local/builds/HongKong-Canton-Demo.app`。WASD／方向键移动，Shift 快走，Esc 暂停／返回起点／退出，N 昼夜，F 景深，P 在原生／1280／960／640 宽度像素处理间切换。UI 保持原生分辨率。

## 实际复用

- bababuyyy `unity-isometric-pixel-pipeline` 固定 `f08bc6bf34b81c265171972610b23a8ef7c8d019`：采用 RenderGraph 特征和三份 shader，MIT 随目录保存。加项目命名空间、只作用于带标记的 Base Game 相机、显式声明深度／法线／描边依赖、按宽高比设置内部尺寸；关闭正交镜头补偿。没有导入原相机、地形、美术、输入或项目设置。附加独立 Canton renderer，其他场景默认 renderer 保留。
- UnityHFSM 2.3.0 固定 `f8fdc2efb862522acd26fab6cd940886c84cde37`，通过 UPM 安装：承载双层巴士 Approach → Open → Board → Close → Depart 循环，队列容量为一，复用同一乘客，无无限生成；不是实际营运时刻或完整公交系统。
- URP 17.3 的深度、受光、阴影、Bloom／Bokeh DOF；Cinemachine 3 跟随；Input System／CharacterController／现有白帽图集；现有铺地与墙材。没有重写这些通用底层。
- 旧 Unity-simple-HD2D 控制器／Z 排序、高版本 stylized-rendering、许可不明 shader 未采用。该决定及更多候选见 [复用筛选](../../04-engineering/github-hd2d-reuse.md)。

## 场景与镜头

主街横向、海防道连接向纵深。当前透视相机 25° 俯角、24° FOV、55 米距离、Y 方位 0°；角色四向与固定游戏轴一致。相比之前 20° 镜头略扩展视野，以同时看清店面、行人和支路。角色仍使用足底锚定的像素面片，碰撞在 3D 空间。没有任意旋转镜头；建模集中于路线可见的正立面、侧面、雨篷、前景及投影。

[数据说明](../../../data/blocks/tst-canton-demo/README.md) 与 [实景核对](../../../references/blocks/tst-canton-demo/README.md) 分别存事实与设计。约 151.4 米广东道中心线压缩拉直为 68.1 游戏米；店面位置、尺寸、平地、支路口和站位是快速设计。源数据、来源 ID 和几何完整保留。不可把本 demo 称作精准城市复原。

本版有可行走角色、建筑体块与店招、街树、路缘、十名循环行人、一个公交乘客、一个静态卸货占位、双层巴士循环。NPC 是几何占位，经像素管线处理；尚未制作像素人物图集。玻璃是简化不透明材质，Apple 目前用文字识别，没有真正透明室内或准确楼梯。没有可进商店、可乘公交、地铁内部、真实车道级调度、声音、存档或完整卸货动作。

## 重现与修改

1. 先运行数据脚本，安装锁定依赖并编译。
2. Unity CLI `script run --file AgentScripts/BuildCantonDemo.cs --entry BuildCantonDemo.Create`。脚本拒绝覆盖现有场景。
3. `RefineCantonDemo.Apply` 是本批可重复的修订，统一字符尺寸／镜头并添加车辆碰撞。编辑 authored 资产另存变体。
4. 构建 `StandaloneOSX`，只包括本场景。`-streetSmokeTest -streetOutput <绝对目录>` 自动保存四种渲染档位、昼夜／景深、往返与支路截图、结果 JSON，完成后退出。

所有 CLI 操作必须显式传本工程绝对 `--project-path`，不得操作隔壁工程。

## 验证与限制

结果与截图见 [本批验证](validation/README.md)。像素管线先全分辨率渲染再降采样，增加五个 pass 和深度／法线输入，不能宣称自动提速。弱透视移动仍可能出现像素闪烁，未移植正交像素吸附公式；当前只验证单游戏相机，未覆盖分屏、XR、其他平台。

## 后续精修待办

用户指出的街面拥挤度不足、建筑稀疏及未制作贴图已纳入 [精修工序与待办](polish-plan.md)。下一批先做小范围密度与一个完整立面，不扩张街区。
