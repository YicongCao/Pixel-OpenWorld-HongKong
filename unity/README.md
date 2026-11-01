# Unity 开发工程

唯一工程路径：[HongKong](HongKong/)。当前为本地环境验证工程，不是已验收的香港街区。

- Editor：Unity 6.3 LTS `6000.3.27f1`（Apple Silicon）；精确修订见 [ProjectVersion.txt](HongKong/ProjectSettings/ProjectVersion.txt)。
- 渲染：URP Universal Renderer（3D），沿用官方 URP 模板。
- 依赖：[manifest.json](HongKong/Packages/manifest.json) 和 [packages-lock.json](HongKong/Packages/packages-lock.json)，包修改优先通过 Unity Package Manager／Pipeline。
- 测试场景：`Assets/EnvironmentCheck/EnvironmentCheck.unity`；测试代码只用于环境验证，不是游戏控制器、美术样板或生产街区。
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

操作前读根 [AGENTS.md](../AGENTS.md)，按需读项目内 `.agents/skills/unity-cli`、`unity-pipeline`、`unity-package-management`。验证结果和限制见 [开发环境记录](../docs/04-engineering/local-development.md)。
