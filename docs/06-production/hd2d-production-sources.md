# HD-2D 制作：专业来源与核查记录

查阅日期：2026-11-01。只将实际读到的开发者访谈、教学正文、软件官方说明及本地源码用于结论；未观看的视频不标为已研究，不以搜索摘要推断未公开的内部参数。本文不归档或再分发访谈图片、视频与整篇原文。

## 开发团队与专业动画教学

1. **Acquire／Square Enix，2023-08-22**：[Octopath Traveler II developer interview](https://www.unrealengine.com/developer-interviews/octopath-traveler-ii-builds-a-bigger-bolder-world-in-its-stunning-hd-2d-style?lang=en-US)。Epic 发布的开发者原访谈；支持二维像素人物、三维背景、固定探索镜头及角色／背景协调问题。未公开本项目可直接使用的角色尺寸、帧表、FOV 或景深预设；不能把 Unreal 的工具链写成 Unity 已实现功能。
2. **Gemdrops／CGWORLD，2024-04-09**：[SO2R 前篇](https://cgworld.jp/article/202404-so2r-1.html)。具名开发团队的技术访谈与说明图；支持 Unity URP 混合角色、面片穿插、受光和独立投影问题。其移动为 12 方向、待机为 8 方向，且部分镜像；只是该作案例，不是我们的标准。SO2R 不能被称为八方旅人官方 HD-2D 管线。
3. **Gemdrops／CGWORLD，2024-04-10**：[SO2R 后篇](https://cgworld.jp/article/202404-so2r-2.html)。支持镜头区混合、早期复用建筑、先精修入口小区域再扩展等实际做法。该作的写实背景、资源采购和世界缩放不自动适用于香港地理还原。
4. **Animation Mentor，专业教学正文**：[Animating a Basic Human Walk Cycle](https://www.animationmentor.com/blog/tutorial-animating-human-walk-cycle/)。支持先接触／承重／经过等关键姿势再细化循环；课程使用的 Maya 和连续动画帧数不作为像素资产硬参数。本次依据网页文字，没有声称看完嵌入视频。

## 像素制作与导出

5. **Aseprite 官方**：[Animation](https://www.aseprite.org/docs/animation/) 与 [Onion Skinning](https://www.aseprite.org/docs/onion-skinning/)。时间轴、帧、预览及前后帧参照；支持生产方法，不证明当前本机已安装。
6. **Aseprite 官方**：[Tags](https://www.aseprite.org/docs/tags/)。标签组织帧区间；播放方向与角色观察方向分开。
7. **Aseprite 官方**：[CLI](https://www.aseprite.org/docs/cli/)。PNG 图集、JSON、标签、留边的批处理能力；本文命令是待本机验证的模板。
8. **Unity 6.3 官方**：[Sprite texture import](https://docs.unity3d.com/6000.3/Documentation/Manual/texture-type-sprite.html)。像素／单位、采样与导入参数。平面网格的实际尺寸仍由网格控制，不能把 Sprite 的 PPU 原样套用。

## 模型、UV 与材质

9. **Blender 4.5 LTS 官方手册可访问版本**：[UV Editing](https://docs.blender.org/UATEST/manual/en/4.5/modeling/meshes/uv/editing.html)。UV 缩放、拼岛、间距和布局操作；使用检索可读的官方 UATEST 路径，常规英文路径本次读取失败。
10. **Blender 5.0 官方手册**：[UV Layout](https://docs.blender.org/manual/en/5.0/addons/import_export/mesh_uv_layout.html)。支持将 UV 布局导出为绘制参照再导回材质。该页用于工作方法，不据此要求升级或安装 Blender 5.0。
11. **Blender 4.5 LTS 官方手册可访问版本**：[FBX](https://docs.blender.org/UATEST/manual/en/4.5/addons/import_export/scene_fbx.html)。单位／轴向等导出参数；文档有未补齐项，因此以一米测试件和 Unity 实际导入核对，不给未经验证的万能参数组。常规英文路径本次读取失败。
12. **Unity 6.3 官方**：[URP Lit](https://docs.unity3d.com/6000.3/Documentation/Manual/urp/lit-shader.html)。表面类型、alpha clipping、受影和材质通道；不证明任意 Sprite 着色器可用于 3D 灯光。

## 镜头与景深

13. **Cinemachine 3.1.7 官方**：[Position Composer](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html)。构图位置、阻尼、dead zone、只移动不旋转的跟随能力；已与本地 3.1.7 随包文档交叉核对。
14. **Cinemachine 3.1.7 官方**：[Blender Settings](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineBlending.html)。镜头对混合曲线和时长、名称匹配；这里 Blender 是混合配置名称，不是建模软件。
15. **Unity 6.3 官方**：[Depth of Field modes](https://docs.unity.com/en-us/engine/6000.3/manual/post-processing-and-full-screen-effects/urp/effect-list/depth-of-field-urp/volume-override) 与 [参数](https://docs.unity.com/en-us/engine/6000.3/manual/post-processing-and-full-screen-effects/urp/effect-list/depth-of-field-urp/volume-override-reference)。Gaussian 远景、Bokeh 近远景，以及焦距／光圈等设置；不能外推正交支持或项目性能。

## 本地版本与源码证据

本次只读检查现有 Packages manifest、Library 中已安装包的文档和 shader，没有修改包或运行新的 Unity 测试。运行栈为 URP 17.3.0／Core 17.3.0、Cinemachine 3.1.7；依赖定义见 [Unity manifest](../../unity/HongKong/Packages/manifest.json)。缓存路径不作为可移植依赖，其他机器按包名／相对文件定位。

以下路径相对于 `unity/HongKong/Library/PackageCache/`，SHA-256 用于标识本次读到的精确内容：

- `com.unity.render-pipelines.universal@ee0b6bbace10/Shaders/PostProcessing/BokehDepthOfField.shader`，行 27、38–42：对焦距离参数对比相机 eye depth，区分近远 CoC。SHA-256：`0832e88938830e8e08613830643ef0b48572408e40fc4540de7e702731b1e233`。
- `com.unity.render-pipelines.universal@ee0b6bbace10/Shaders/PostProcessing/GaussianDepthOfField.shader`，行 70：同样调用双参数 `LinearEyeDepth`。SHA-256：`daf1449bbd23d14eba8381cba415b2fae995ea7a1f0ab279d1731e40f3537735`。
- `com.unity.render-pipelines.core@6c7e6aba313a/ShaderLibrary/Common.hlsl`，行 1203–1211：该重载明确标注不适用于正交投影。SHA-256：`0a6a5fcf220db789b3cebf0421b71ba19a0fca9846bc6036f776f0254571c035`。
- `com.unity.render-pipelines.universal@ee0b6bbace10/Shaders/Lit.shader`，行 184、323、367 及对应 alpha test：ShadowCaster、DepthOnly、DepthNormals 通道。SHA-256：`6f4648b6b5271132cfed1d7d7c771ba0a73ef7972470d6694047804d259c0997`。
- `com.unity.cinemachine@f3f96bcb59af/Documentation~/CinemachinePositionComposer.md`。SHA-256：`b0c727184fda318123ad4e85ed9f31ec31c76b5548ed3d7d3d75fbe9e44ec7bd`。
- `com.unity.cinemachine@f3f96bcb59af/Documentation~/CinemachineBlending.md`。SHA-256：`e68af4fbc8d50765045caea1587a9c7f6b9bbeaa11e70118fcf9b2612df8581b`。

由上述源码得出的判断：当前原生 DOF 的正交路线有风险，优先做弱透视样板；焦点初值应对应相机前向深度。**这是源码推论，不是已经测出某个机型上的错误画面或性能结论。** 包升级后重新核对，不能永久沿用这个限制。

## 明确未被资料证明的事项

- 八方旅人的具体源像素高度、每方向帧数、材质全部参数、完整内部 DCC 工具链，不能由截图反推为事实。
- 四／八方向、6 帧、48–64 源像素、俯角 25–35°、模块尺寸等仍是我们的试作输入。
- 3D 绑定后渲染多方向再像素修订是可评估的项目备选，不是已核实的八方旅人通用生产方法。
- 本次没有安装 Aseprite／Blender 插件，没有交付新角色／模型／Unity 场景，也没有完成 HD-2D 游戏画面或整街性能验收。

资料结论落在 [角色生产](../../workflows/20-assets/character-production.md)、[镜头与渲染](../../workflows/10-hd2d/camera-lighting.md) 和 [总生产 SOP](../../workflows/10-hd2d/hd2d-production-workflow.md)；根目录只维护执行要求与入口，避免复制整篇研究造成分叉。
