# 工作流目录：如何制作香港像素漫游

**工作流建设是工程第一优先级。** `docs` 是名词：事实、调研、设计合同、资产清单与实施证据；`workflows` 是动词：引用这些资料，把输入变成可验收的成果。原始数据在 data，实景在 references，生产资产在 assets／Unity，均不因目录拆分而复制。

这里是 **1 个街区总流程＋3 个 HD-2D 专项流程＋3 个资产流程＋2 个维护／迭代流程**，共 9 个有 ID 的执行流程；不是九套互相竞争的端到端方法。

## 00 · 工程总流程与维护

- [WF-BLOCK 街区还原总流程](00-project/街区还原工作流.md)：接到“还原某街区”时选用，串联数据、实景、镜头、概念、资产、装配、活动、烘焙、实机验收与交付。
- [WF-MAINTAIN 工作流维护与反馈闭环](00-project/workflow-maintenance.md)：研究／文档／工具维护、新建流程、吸收用户反馈；负责方法改进，不代替具体制作。

## 10 · HD-2D 制作子流程

- [WF-HD2D 素材建模与拼装 SOP](10-hd2d/hd2d-production-workflow.md)：先锁镜头与像素合同，完成角色＋商铺＋连接路的小样，再按资产类型扩产。
- [WF-LIGHT 镜头、受光、景深与烘焙](10-hd2d/camera-lighting.md)：镜头区、人物深度／投影、灯光预设、试烘／最终烘焙及效果检查。
- [WF-SCENE 场景装配、生活运行与交付验收](10-hd2d/scene-assembly-validation.md)：把已制作的零件变成能自然行走的街区；负责摆放、通行、行为、声景、运行与交付检查，调用 WF-LIGHT 做烘焙。
- [WF-ITERATE 小步精修与固定视角省量](10-hd2d/iteration-and-fidelity.md)：每次局部迭代的可见面预算、模块复用、成本记录和固定条件对照；配合上述主流程使用。

## 20 · 资产类型制作流程

- [WF-CHAR 角色方向、站姿与动作](20-assets/character-production.md)：白帽主角、NPC、携物／候车等动作，图集和足底锚点。
- [WF-BUILDING 建筑与商铺模块](20-assets/building-production.md)：定尺、可见面、结构拆件、UV、像素纹理和导出。
- [WF-PROPS 香港景物与标识](20-assets/scenery-production.md)：商标、路牌、公交站牌、地面设施、植物、车辆及活动道具；依类型执行具体配方。

## 选择与执行

先看 [工作流使用举例](EXAMPLES.md)：以“精修公交站牌／观光巴士／出租车”和“按 20–30 米连续制作广东道”两组问答，示范主流程选择、子流程组合与验收边界，供后续 Agent 类推使用。

完整街区使用 WF-BLOCK 调用子流程；只修站牌选择 WF-PROPS，再执行 WF-SCENE 的摆放回归；只改镜头选择 WF-LIGHT，并复查 WF-BUILDING 的可见面和 WF-CHAR 的方向；只整理研究选择 WF-MAINTAIN。同一任务选择一个主流程，子流程按需调用，避免重复劳动。

流程文件中的“状态”表示规范／验证成熟度；实施是否通过，以对应街区的证据为准。每次执行记录流程版本、范围、跳过理由、结果和反馈；模板与改版方法见 WF-MAINTAIN。**写好了流程不等于已经做完或验证了游戏。**

## 资料入口

- [静态资料总目录](../docs/README.md) 与 [阅读路由](../docs/Skill.md)。
- [专业生产来源](../docs/06-production/hd2d-production-sources.md)、[八方旅人 II 访谈启发](../docs/03-hd2d/octopath2-interview-lessons.md)。
- [广东道资产盘点与缺口](../docs/08-blocks/tst-canton-demo/scene-inventory.md)、[香港标识依据](../docs/05-street-life/hong-kong-signage-reference.md)。

## 维护约定

流程采用稳定 ID 和版本号；步骤含输入、产物、检查与回退，外部资料归 docs 后引用。核心门槛不能因局部反馈被删掉；仅补相关步骤，不把每次失败日志堆入流程。根目录 [AGENTS.md](../AGENTS.md) 规定执行和提交授权。历史迁移路径在 [迁移映射](../docs/07-validation/workflow-migration.json) 保留，旧证据不改写。
