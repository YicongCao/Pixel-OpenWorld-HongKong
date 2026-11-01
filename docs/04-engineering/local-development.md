# 本地 Unity 开发环境

2026-11-01 完成本机环境验证。唯一工程是 `unity/HongKong/`，项目中文标题统一为「香港像素漫游」。诊断程序内部产品名保留 `Hong Kong Environment Check`，用于区别游戏 Player；本次没有制作可玩街区。

## 已安装与锁定

使用本机已有的 Unity 6.3 LTS **6000.3.27f1**（修订 `672d562a42a7`，Apple Silicon），路径 `/Users/cong/Applications/Unity/6000.3.27f1/Unity.app`。另有 6000.6.7f1，本工程不混用。Unity Personal 授权有效；未安装或升级 Editor。

官方 Unity CLI 从 1.0.0-beta.14 更新至 **1.0.0-beta.15**，位于 `/Users/cong/.unity/bin/unity`。项目使用 URP 3D 模板，依赖以 [manifest](../../unity/HongKong/Packages/manifest.json) 和 [lock](../../unity/HongKong/Packages/packages-lock.json) 为准：

- URP 17.3.0；Input System 1.20.1；AI Navigation 2.0.16；Test Framework 1.6.0（模板依赖）。
- Cinemachine **3.1.7**（新增）；官方 Unity Pipeline **0.9.1-exp.1**（新增，实验包）。
- 模板还保留 Timeline、UGUI、IDE 等依赖，未购买 TopDown Engine，也未增加重复角色框架。

安装了项目级官方 `unity-cli`、`unity-pipeline`、`unity-package-management` 技能；未修改全局 Codex 配置。CLI 可直接操作 Editor，当前不需要另建第三方 MCP 服务。[Unity 官方迁移说明](https://docs.unity.com/en-us/unity-cli/replace-mcp-server-unity-cli) 区分了 CLI 与旧 Assistant 内置 MCP；[Pipeline](https://docs.unity.com/en-us/unity-cli/unity-pipeline/unity-pipeline-package) 提供工程内执行能力。CLI 和 Pipeline 的实验状态仍需纳入升级风险，不能自动升级依赖。

## 已执行的检查

机器为 Apple M5、24 GB，macOS 27.0.1；有 Xcode Command Line Tools，没有完整 Xcode 和独立 dotnet SDK。本次使用 Unity 自带编译链和 Mono macOS ARM64 开发构建。

1. Editor 打开指定工程，安装依赖后编译成功。最终真实 Console 状态为编译失败 false、错误 0、警告 0。
2. 通过 CLI 执行 C# 创建独立环境场景；URP 材质、主光、阴影、相机和动态方块可见。创建脚本拒绝覆盖已有测试场景。
3. Play Mode 中 frameCount 从 4644 变为 4794；已查看 [Editor 截图](../07-validation/environment-2026-11-01/editor.png)。随后回到 Edit Mode，关闭自动 tick，恢复后台执行覆盖设置。
4. 导出 `.local/builds/HongKongEnvironment.app`，实际运行独立程序，120 帧后写入证据并正常退出（退出码 0）。[Player JSON](../07-validation/environment-2026-11-01/player.json) 记录 Metal、URP、1280×720 和键盘设备检出；[Player 截图](../07-validation/environment-2026-11-01/player.png) 已目视核对。

这些结果证明本地编辑、编译、渲染、截图和导出链路可用。没有验证角色输入映射、Cinemachine 跟随、NavMesh 寻路、HD-2D 景深、街区负载或帧率目标；键盘设备检出也不等于手动操控通过。V1-00 的环境部分完成，首版技术切片仍需继续。

## 可复用入口

从仓库根执行，始终显式指定工程：

```sh
unity open "$PWD/unity/HongKong" --until-ready
unity status --project-path "$PWD/unity/HongKong" --json
unity recompile --project-path "$PWD/unity/HongKong"
unity command console status --project-path "$PWD/unity/HongKong" --json
unity command script eval 'return HongKong.EnvironmentCheck.EnvironmentBootstrap.BuildMac();' --project-path "$PWD/unity/HongKong" --timeout 300
```

编辑器菜单 `Hong Kong > Environment Check > Build Mac Player` 等价。诊断程序接受 `-envSmokeTest -envOutput <绝对目录>`，输出 player.json/player.png 后退出；历史证据应保留，重新运行使用新目录。其他接口以当前 CLI 帮助和 Pipeline schema 为准。项目导航见 [Unity README](../../unity/README.md)。

## 本次遇到的问题与恢复

- 初次启动出现布局恢复弹窗：退出本工程 Editor，将安装目录的工厂 Default.wlt 复制到本工程被忽略的 `UserSettings/Layouts/default-6000.dwlt` 后重开。没有点击全局布局重置。随后接受新工程的 URP 材质升级。
- 包安装期间一次 Editor 中断、一次 domain reload 令命令报告 socket hang up；重开并等待就绪，实际 manifest/lock 和编译证实 Cinemachine 安装完成。
- 截图 API 拒绝跨项目路径；改用工程内临时目录后复制证据，再通过 AssetDatabase 清理临时资产。Pipeline 的历史错误缓冲中保留这次拒绝，最终 Console 无真实错误。
- 本地缓存、原始诊断日志、授权状态和构建程序不提交；只提交工程源、锁文件、技能与去敏证据。完整 Xcode/IL2CPP、其他平台导出仍未验证。

文档检查：八章 HTML 已重建，文档／引用／原始数据哈希检查通过。本轮浏览器的 file:// 预览被 URL 安全策略阻止，未做新的页面目视验收；Unity 两张渲染截图已通过图像工具检查。
