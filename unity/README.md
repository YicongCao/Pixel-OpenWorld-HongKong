# Unity 开发工程

唯一工程路径：[HongKong](HongKong/)。已进入 v0.1：有可行走路线、灰盒及 A1 街角早期样板，尚非已验收的完整香港街区。

- Editor：Unity 6.3 LTS `6000.3.27f1`（Apple Silicon）；精确修订见 [ProjectVersion.txt](HongKong/ProjectSettings/ProjectVersion.txt)。
- 渲染：URP Universal Renderer（3D），沿用官方 URP 模板。
- 依赖：[manifest.json](HongKong/Packages/manifest.json) 和 [packages-lock.json](HongKong/Packages/packages-lock.json)，包修改优先通过 Unity Package Manager／Pipeline。
- 测试场景：`Assets/EnvironmentCheck/EnvironmentCheck.unity`；测试代码只用于环境验证，不是游戏控制器、美术样板或生产街区。
- 当前街角场景：`Assets/HongKong/Generated/Scenes/NathanArtSample.unity`；灰盒对照：同目录 `NathanGraybox.unity`。运行时脚本在 `Assets/HongKong/Runtime/`，可复现建场／修订脚本在 `AgentScripts/`，采用资产在 `Assets/HongKong/Generated/`。
- 保留 `Assets`／`.meta`／`Packages`／`ProjectSettings`，忽略 `Library`、`Temp`、`Logs`、`UserSettings` 和构建产物。根 `assets/` 是源资产库，不是本工程的 `Assets/`。

## 本机操作

在仓库根执行，CLI 会使用工程锁定的 Editor：

```sh
unity open "$PWD/unity/HongKong" --until-ready
unity status --project-path "$PWD/unity/HongKong" --json
unity command editor status --project-path "$PWD/unity/HongKong" --json
unity command console status --project-path "$PWD/unity/HongKong" --json
```

所有编辑器命令显式指定本工程，禁止误操作隔壁 game-research。代码修改后运行 `unity recompile --project-path "$PWD/unity/HongKong"`；命令参数以本机 CLI 帮助与已连接 Pipeline schema 为准。

项目菜单 `Hong Kong > Environment Check > Build Mac Player` 将测试程序导出到仓库忽略目录 `.local/builds/`。建场景菜单只允许首次创建，已有场景会拒绝覆盖以保护精修。

## 当前可玩样板

本地 Player：仓库根 `.local/builds/HongKong-v01-sample.app`。WASD／方向键行走，Shift 快走，Esc 暂停／回起点／退出，N 昼夜，F 景深。没有 NPC、公交运行、存档和音效；站内和公园深处不可进入。完整进度、来源／镜头参数与结果见 [实施批次 01](../docs/08-blocks/tst-nathan-haiphong/implementation.md)。

在仓库根构建当前已保存场景（已在本机执行）：

```sh
unity command build run --project-path "$PWD/unity/HongKong" \
  --target StandaloneOSX --outputPath "$PWD/.local/builds/HongKong-v01-sample.app" \
  --options '["Development","DetailedBuildReport"]' \
  --scenes '["Assets/HongKong/Generated/Scenes/NathanArtSample.unity"]' --confirm true
unity command build status --project-path "$PWD/unity/HongKong"
open .local/builds/HongKong-v01-sample.app
```

构建异步，必须检查 status 为 completed 且 Succeeded 后再启动。自动路线回归可直接执行 `.app/Contents/MacOS/香港像素漫游`，附 `-streetSmokeTest -streetOutput <绝对输出目录> -screen-width 1920 -screen-height 1080 -screen-fullscreen 0`；运行完成自动写 JSON／PNG 并退出。默认启动保持手动游玩。

生成场景已有时不要直接重跑 Create。源图集／材质来自仓库根 `assets/blocks/`，数据脚本只更新 generated 派生文件；不覆盖 authored。要验证首次生成，应在另建、不含既有生成场景／网格的隔离工程副本执行，原工程保留。后处理 Volume 组件必须持久化为子资产，不能仅在内存 `profile.Add` 后假设保存成功。

操作前读根 [AGENTS.md](../AGENTS.md)，按需读项目内 `.agents/skills/unity-cli`、`unity-pipeline`、`unity-package-management`。验证结果和限制见 [开发环境记录](../docs/04-engineering/local-development.md)。

当前可玩 demo：`Assets/HongKong/Generated/CantonDemo/CantonRoadDemo.unity`，本机 `.local/builds/HongKong-Canton-Demo.app`。组件、操作与真实状态见 [广东道记录](../docs/08-blocks/tst-canton-demo/README.md)。
