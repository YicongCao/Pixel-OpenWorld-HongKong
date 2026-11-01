# v0.1 实施批次 01：可走路线与 A1 街角样板

2026-11-01 · 已开始执行近期计划。交付的是可操作的早期样板，**不是完整 v0.1、也不是已获认可的最终 HD-2D 画面**。

## 本批落地内容

1. 由政府地面行人路径派生 179.34 米连续路线，保留真实方向和高程，接入 26 个来源明确的西侧建筑／雨篷体块。原示意虚线穿过 A1 的问题已修正。数据依据见 [实施派生包](../../../data/blocks/tst-nathan-haiphong/implementation/README.md)。
2. Unity 可操作场景：弥敦道横向移动、转入海防道纵深、边界碰撞、暂停与回到起点。复用 CharacterController、Input System、Cinemachine 和 URP；项目代码只处理本街区控制适配、图集播放、照明和验证，没有采购或引入新的 RPG 框架。
3. imagegen 生成一套白帽四向图集和八种表面材质。图集每方向一张站姿＋六张行走帧，共 28 帧；生成尺寸偏离提示，因此按实际透明轮廓制作切片和脚底数据。原 PNG 不做像素改写，采用版本及来源哈希独立保存。
4. A1 小样按玻璃罩、钢架、台阶、标识、栏杆分件；南侧转角铺面使用旧照片中的六福珠寶识别元素；路面、空调、窗格、树冠和公交雨篷为几何小样。品牌年代见 [实景来源](sources.md)，不能据旧照片声称今日店铺仍营业。
5. 白天／夜晚和 Bokeh 景深开关；角色是 alpha-clipped、受 3D 灯光与深度影响的面片，脚下有接触阴影代理。角色真实投影／阴影代理精修仍待完成。

## 工程与操作

工程入口：[Unity README](../../../unity/README.md)。默认构建场景为 `Assets/HongKong/Generated/Scenes/NathanArtSample.unity`；保留 `NathanGraybox.unity` 作为对照。源资产在根 `assets/blocks/tst-nathan-haiphong/generated/`，引擎采用副本与程序化模型在 Unity 的 `Assets/HongKong/Generated/`，没有 authored 精修内容。

- WASD／方向键：相对屏幕方向行走；Shift：快走。
- Esc：暂停、继续、回到起点、退出。
- N：白天／夜晚；F：景深开关，默认关闭以检查几何与脚底。
- 只开放声明的人行路线；站内、公园深处和车行道不开放。可见站口前坪不等于全部有通行碰撞，后续要按真实边界扩为行走面。

本地构建放 `.local/builds/HongKong-v01-sample.app`，可在 Finder 打开或执行 `open .local/builds/HongKong-v01-sample.app`。构建包可再生且未提交到 Git；源码、场景、资产、证据纳入版本管理。

## 当前镜头与资产合同：实验参数

灰盒使用 30° 俯角／朝向 Y=−90°／32° FOV／54 米距离。街角样板为观察海防道南侧立面改为 **28° 俯角／Y=−110°／32° 垂直 FOV／34 米跟随距离**，不自动旋转。主街仍以横向为主，屏幕右大致向北；世界 X 东、Z 北没有改变。用户看过首张 43 米距离的实机图后选择“适度拉近镜头，突出主角”，本批据此缩短距离，目标投影增大约 26%；这仅确认构图偏好，不等于完整风格验收。跟随目标高于根部 2.6 米，窗口基线 1920×1080；主角约 1.9 米、画面约 80–95 像素高，具体随姿态和投影变化。

街景素材按 4×2 图集在 Unity 内裁切，Point 采样、128 最大导入尺寸＋mip 是本轮像素密度实验，不是已锁定的最终标准。角色 Point、不压缩、不启用 mip，四向独立足底与身高归一；步态按移动距离推进，停步使用站姿。侧向／背向比例、帽形与步态仍有生成误差，不能把有效切片等同于动作质量合格。

简单模型由版本化 C# 生成并保存为可编辑 Unity 资源，未使用 Blender。A1、主角、商铺部件有 Prefab；保存了独立材质与网格。中文世界标识使用运行时系统字体，HUD 暂用英文避开本机 IMGUI 字体警告；未分发系统字体，跨平台打包前需换成明确许可且经过测试的字体资产。

## 重建与验证入口

仅在新建、隔离的工作副本中重建。已有场景时构建器会拒绝覆盖；日常改动直接编辑副本或增加明确的修订脚本，不删除现有精修场景强行重跑。

```sh
../game-research/.venv-hk-data/bin/python scripts/build_v01_layout.py
../game-research/.venv-hk-data/bin/python scripts/prepare_v01_assets.py
unity recompile --project-path "$PWD/unity/HongKong"
unity command script run --project-path "$PWD/unity/HongKong" --file AgentScripts/BuildStreet.cs --entry BuildStreet.Create
unity command script run --project-path "$PWD/unity/HongKong" --file AgentScripts/BuildArtSample.cs --entry BuildArtSample.Create
unity command script run --project-path "$PWD/unity/HongKong" --file AgentScripts/RefineStreetSample.cs --entry RefineStreetSample.Apply
unity command script run --project-path "$PWD/unity/HongKong" --file AgentScripts/RefineStreetSample.cs --entry RefineStreetSample.RepairProfileAndEdges
unity command script run --project-path "$PWD/unity/HongKong" --file AgentScripts/RefineStreetSample.cs --entry RefineStreetSample.CloseCamera
```

Imagegen 不参与确定性重建，已采用的 PNG 和切片元数据随 Git 保存。构建器调用的 Newtonsoft.Json 目前来自锁定包依赖；移除包或清理依赖时应重新核查，不能只复制这几个脚本到空白工程。

实际截图、输入结果、两种镜头的独立构建、短程帧时与限制统一归档于 [批次 01 验证记录](validation/2026-11-01-batch01/README.md)。构建包源码可追溯，但当前没有发布正式 v0.1 标签。

`StreetValidation` 的 `-streetSmokeTest -streetOutput <绝对目录>` 启动完整往返并自动退出；正常双击应用不会自动移动。自动测试给同一个 CharacterController 提供世界方向意图，不沿路径直接传送。`StreetInputValidation` 另外通过 Input System 虚拟键盘验证真实绑定、暂停、效果开关及路边碰撞；它不是人工键盘操作验收。

## 当前缺口与下批优先级

- V1-01 已有连通路线和来源链，但现地开放、精确人行道宽度／站位锚点仍待核实。
- V1-02 已形成灰盒、控制与镜头；独立跨层技术测试、不同窗口、转角遮挡全覆盖仍待补充，不能把平面路线通过扩写为全部跨层通过。
- V1-03 进入实机样板阶段。树冠球形感、灰色背景体块、道路／路缘高差、立面重复纹理、夜景与角色比例需要进一步打磨；尚未得到用户风格反馈。
- V1-04 未扩为完整精做街段；现在大部分背景仍是占位体块。不能把概念的细节密度描述成实机已有。
- V1-05 行人、进站、车流、公交靠站上下客及卸货活动尚未实现；当前站牌／雨篷只是展示几何，并无运行中的公交。
- V1-06 存档、开始／继续、音量和空间声景尚未实现；晴雨及工作日／休息日预设也未实现。
- V1-07 尚未进行包含目标人车负载的 10 分钟性能／内存验收，也没有首版发布候选。短时样板测试不替代它。

下一批先修正这个街角的像素密度、站口尺度、道路接缝与遮挡；同时在已有灰盒上验证目的地行人、站口出入及公交状态循环。风格获得反馈后才扩大精做范围，不把全部地图先铺成空街。

本轮使用 Unity CLI／Pipeline 技能，查阅本地 `scripts.md`、`capture.md`、`build-and-compilation.md`、`editor-lifecycle-and-observability.md` 与 Play Mode 验证循环。工具短暂域重载断连须等实例恢复后重试，不把断连冒称引擎失败。

## 实际验证结果

独立 Mac Player 在 Apple M5、1920×1080、PC 质量档／VSync 1、Development 构建下通过 50/50 检查点，约 109.8 秒、426.54 米，零运行错误、零坠落恢复。往返包含出生点先走向北端的距离，不能把它当作路线长度。P95 17.48 ms、P99 21.20 ms；短时 P95 高于完整版本目标 16.7 ms，且没有目标人车负载，**不能记为性能达标**。构建 74.3 秒、340,572,776 字节，构建错误／警告均为零；二进制包含 arm64／x86_64，仅在本机 Apple Silicon 执行。

[独立程序往返 JSON](validation/2026-11-01-batch01/art-player.json)、[输入验证](validation/2026-11-01-batch01/input-editor.json)、[编辑器往返](validation/2026-11-01-batch01/art-editor.json)、[构建与来源指纹](validation/2026-11-01-batch01/build.json)。截图：[白天](validation/2026-11-01-batch01/player-day.png)、[白天景深](validation/2026-11-01-batch01/player-day-dof.png)、[夜景](validation/2026-11-01-batch01/player-night-dof.png)、[海防道端点](validation/2026-11-01-batch01/player-haiphong.png)。

11 项输入／配置检查通过，包括暂停禁止移动、恢复、侧边碰撞、图集矩形和重载后 DOF 配置存在。Bokeh 已进入保存／加载与独立程序对照，当前虚化较弱，仍待和目标画风比较。编辑器离屏截图阶段出现两条 Metal memoryless depth load/store 警告；本次 Player 日志未发现同类警告或异常。系统字体的 IMGUI 重复警告通过 HUD 暂用内置英文字体消除，世界双语标识保留。

已停止本 Agent 的 Play 会话，关闭自动 tick 与临时后台覆盖，保留工程原本 runInBackground=true。未安装新包、未修改 game-research。独立构建不进入 Git，采用源资产、场景／meta 和选定证据进入 Git。
