# Tripo3D 香港街景模块试验提示词

日期：2026-11-02；起始 Git：5122a71。用户反馈：Agent 手工／程序建模成本高，先用 Tripo3D 验证替代路径。执行 WF-PROPS 1.1 的表现形式／交接范围，调用 WF-BUILDING 1.0 的建筑模块约束；本批只交付提示词与检查方法，尚未生成／下载／导入 Tripo 模型。

## 第一件：双店面临街建筑模块

建议先生成一个包含两个店面的建筑段，验证“能放入游戏的模型”，再用不同模块拼出 20–30 米街景。尺寸 10×12×4 米为试验设计值，并非广东道某栋建筑测绘结果。此模型用于风格与可用性验证，不能替换已经核实的真实门店立面。人物、车辆、站牌分别生成；地面与巷道空间留给 Unity 装配。

粘贴到 Text to 3D 主提示词框：

```text
A single modular Hong Kong street-front building section for an HD-2D RPG. About 10 m wide, 12 m tall, 4 m deep; four storeys with two tightly adjoining ground-floor shops. Straight rectangular structure, realistic human scale. Recessed shop entrances, shallow display windows, projecting awnings and blank signboards. Dense upper window rows, box air conditioners and a few drainpipes. Weathered cream ceramic tiles, gray concrete, dark metal frames. Clearly modeled front facade and right-hand side return, with real depth in recesses and projections; simple closed rear. Designed for a fixed, slightly elevated front three-quarter game camera. Restrained hand-painted pixel-art surface colors, crisp architectural edges and readable large shapes; not voxel cubes. Neutral base color without painted cast shadows, glare or blur. Flat bottom, no pedestal. Building only: no road, pavement, people, vehicles, trees, sky, text or logos.
```

## 参数与边界

- 若界面提供 Clean Topology／Smart Mesh，可先以约 10,000 面、开启纹理做一件；这是测试预算，不是整个游戏的已验证标准。纹理先 2K（若该路径支持），不要第一轮就追求最高细节。
- Tripo 官方建议明确主体、结构、材质、风格及排除项，并优先英文；官方将文本与图片生成列为不同输入路径，不能默认图生 3D 会同时遵守此段文字。若改用照片／概念图，先整理单体参考，避免把人物、车流或景深模糊一起重建。
- “HD-2D”在这里约束比例、表面色块和可见面。真实景深、光照、阴影、像素采样由 Unity 实现；不要求模型把虚化、固定透视或阳光阴影画死在贴图里。
- 门窗凹进、雨篷突出和转角厚度必须成立；背面可简化，侧面保留在支路观察时会露出的结构。“简化背面”不等于整栋只剩一张正面平面。
- 初试留空招牌，后续复用已核实的品牌／繁英文字独立资产；不是取消真实商业元素。准确字体、品牌和街侧不能依赖生成模型猜测。
- 提示词里的尺寸、分件、UV、干净拓扑都是目标，生成后必须检查，不能把自然语言约束当成输出保证。

## 看结果时检查

1. 正面与左右斜侧面：门窗有没有融化、雨篷是否真有厚度、侧面是否只剩涂抹纹理。
2. 放一个约 1.7m 的人形参照：店门约 2.2–2.6m 的试验尺度是否协调；导入后重新校准单位与原点。
3. 关后处理并转动光源：贴图是否已有强阴影、玻璃是否只是画死的反光；材质能否在既有场景受光。
4. 沿现有固定镜头横走和进入支路：正侧面、主角遮挡、像素密度是否成立。无须第一轮制作完整室内或不可见后街。
5. 导出模型＋贴图（优先现有 Unity 链路可接收的 FBX；GLB 可保留作原始交付），记录任务 ID、模型版本、提示词、生成设置、原件哈希及使用许可；分割／减面／碰撞／UV2在检查后补做。
6. 记录生成次数、额度消耗与导入清理分钟数，再与现有程序模型比较成本。尚未在本项目验证 Tripo 可减少多少工作量。

## 官方依据

- [Tripo Text to 3D](https://www.tripo3d.ai/product-doc/model-generation/text-to-3d)：输入组织、模型路径、面数参数与图文输入边界；2026-11-02 查询。
- [Tripo prompt-to-3D 指南](https://www.tripo3d.ai/blog/prompt-to-3d)：先做聚焦单件，生成后仍需检查与清理；2026-11-02 查询。
