# 广东道交通资产精修 v1

本批制作与独立 Player 功能回归已完成，美术仍待评审。主流程 WF-PROPS 1.0；子流程 WF-SCENE 1.0、WF-LIGHT 2.0、WF-ITERATE 1.0；起始提交 7b055ae。仅修改交通资产与相关验证，保留店铺精修。

## 取证与设计边界

- 运输署的士运营范围：广东道只投放红色市区的士；绿色新界、蓝色大屿山的士制作独立预制体，不加入广东道交通。来源：https://www.td.gov.hk/en/transport_in_hong_kong/public_transport/taxi/details_of_taxi_operating_areas_/
- 城巴实时开放数据 H2 inbound：广东道站点 001403 戏曲中心、001402 北京道；本片段不伪造 H2 上下客站，观光车通过。原始 JSON 在 data/blocks/tst-canton-demo/transport-v1。
- 城巴 2025-08-01 官方宣传照第 3 页：黄蓝半开篷、三轴、前段上层有顶、后段露天座位；按此制作风格化模型，不宣称精确复刻车厂图纸。来源：https://www.citybus.com.hk/en/uploadedPressRelease/21658_20250801_ENG.pdf
- 本仓库运输署完整快照的 td:BUS:stop:13364 为广东道港威酒店，关联九巴 271 系列；参考该身份制作站牌，实际摆位仍为压缩设计。H2 不停此站。站牌外形依据九巴圆牌及分层线路信息架照片，现场当日支架细节尚未逐项确认。
- 出租车参考经典 Comfort 四门轿车照片：红／银顶、绿／白顶、浅蓝／白顶，独立 TAXI 顶灯、门字、黑防擦条、后视镜和轮毂。参考照片有年代，车型并不代表现时整个车队。

## 制作与验收计划

可编辑程序模型源、持久化网格／材质和四种车辆预制体；运行资产不依赖相邻仓库。改造现有巴士视觉，新增红的士车流；绿／蓝仅资产展示验证。保留旧场景可从 Git 恢复，新的生产入口不得重复覆盖精修。

验收：编译、模型检查、全路线、至少两分钟驻足观察、交通循环、区域投放、门与停站逻辑、车道间距、探针受光和独立 Player 截图。静态站牌替换后检查并重新烘焙。完整 v0.1、精确车道测绘和乘车功能不属于本批。


## 实际交付与复现入口

- [资产合同](../../../assets/blocks/tst-canton-demo/authored/transport-v1/manifest.json)：5 个预制体（站牌、H2、三色的士），持久化网格／材质／碰撞与轮子枢轴。[实景索引](../../../references/blocks/tst-canton-demo/transport-v1/README.md)；[场景实例与区域检查](../../../data/blocks/tst-canton-demo/transport-v1/design-instances.json)。
- 可编辑生产源为 `unity/HongKong/AgentScripts/TransportV1.cs`。Unity 根目录相对入口 `Apply` 只创建一次、拒绝覆盖；`RefineMarkings` 定向修正字号与深度材质；`Gallery` 临时实例化资产、渲染后销毁，不把绿色／蓝色车加入街景；`Audit` 检查区域投放与车道净距。需要新一轮造型精修时写显式修订，不删除目录强行重建。
- 规则轮廓采用程序网格，车窗、轮、灯、座椅、牌面独立分件；本批无需 Blender 或 imagegen，未重复制作整街概念。延续已锁定镜头、人物及原有店铺；复用本仓库上游切片，无需改动或重跑 game-research。
- 公交牌包含红圆牌、271 系列线路格、繁英站名、信息柜、雨帽、螺钉、杆与底座。班次／票价未取证，信息柜仅列线路，不伪造时刻。
- 的士保留经典 Comfort 的引擎盖／行李厢轮廓、斜玻璃、双色顶、顶灯、门字、防擦条、把手、镜、轮毂、前白后黄车牌底；不生成真实牌照号码。
- H2 改为黄蓝半开篷三轴车，前部上层遮顶、后部可见座椅和安全栏杆。当前不是厂商精确模型；车门在 +Z 乘客侧，镜头常见另一侧。
- 运行沿既有压缩坐标：公交 Z=-3、的士 Z=-6.8，朝 +X；公交与的士碰撞盒横向净距 1.55m。两辆的士初始纵向相隔 44m、同速循环。本站不服务 H2，旧上客乘客停用；旧 demo 场景的 HFSM 停靠逻辑保留。
- 本批材质为 URP 色块／玻璃色与可控排字；未制作最终手绘像素材质图集。TextMesh 字号已缩放并采用独立深度测试 Shader，避免背面字透过车体。中文依赖本机 PingFang／Heiti；跨平台嵌入字体仍待办。

## 验证结果

独立 macOS Development Player，Unity 6000.3.27f1／Apple M5／1920×1080，构建 0 警告、0 错误。原始 [result.json](transport-validation/player/result.json)、[构建摘要](transport-validation/build-summary.json) 和 [资产检查图](transport-validation/asset-gallery.png) 已归档。

- 188.87 秒，包含 120 秒驻足观察；12/12 往返检查点、4 个 Apple 近景点抵达，约 242.3m，0 掉落恢复。
- H2 11 轮通过、0 上下客违规；仅 2 辆红色市区的士，合计 25 轮循环；卸货 8 次。区域／车道检查通过。当前规则车流没有智能避让或真实路口调度，不能据此宣称全交通仿真。
- P95 17.28ms、P99 17.63ms；Unity 已分配内存约 151MiB（非进程总内存），短回归不等于十分钟性能验收。
- 光照重新烘焙后 Player 加载 1 张 Lightmap、294 探针，Apple 烘焙反射仍可用。无夜景修改；保持原午后效果。
- 主路线、纵深支路、默认像素档位、DOF 开关、三色资产展示已检查。重启独立窗口并实测 D/A 与 Esc 暂停／恢复，窗口保留可试玩。无存档系统，因此未测存档。
- 运行期测试收集到 0 Error／Exception；退出日志存在 Unity Development Player 线程提前结束／清理期 MemoryLeaks 输出，尚未归因，不据此宣称长时间零泄漏。

本地试玩程序：`.local/builds/HongKong-Canton-Transport.app`（构建输出不纳入 Git）。WASD／方向键行走，Shift 快走，Esc 暂停，F 景深，P 像素密度。重跑时从仓库根启动：
```sh
".local/builds/HongKong-Canton-Transport.app/Contents/MacOS/香港像素漫游" -screen-fullscreen 0 -screen-width 1920 -screen-height 1080 -streetSmokeTest -streetAppleBay -streetTransport -streetObserveSeconds 120 -streetOutput "$PWD/.local/transport-retest"
```

## 工作流闭环与剩余精修

本批执行 WF-PROPS 1.0 的取证、灰模接口、建模、材质、实机检查和交接，WF-SCENE 1.0 的摆放／活动／回归，WF-LIGHT 2.0 的重烘与加载核对，WF-ITERATE 1.0 的局部范围控制。无需重跑完整地理提取、整街概念、角色生产或夜景。

发现背面文字穿透／字号过大后修正材质和字号，并通过资产图与 Player 复验。WF-PROPS 升至 1.1：增加深度文字、线路与站牌关系、地区投放门槛；WF-SCENE 升至 1.1：区分通过／停靠，要求区域、车道、循环与暂停检查。

下一轮重点：取得港威酒店站牌当日正面／侧背照片，校准版式字体与精确摆位；改善默认视距下小字的像素可读性；补车身曲面／轮拱／内饰和像素纹理，观光巴士增乘客；处理巴士经过时短暂完全遮住主角的可读性（先同机位验证，再选轮廓提示或其他方法）。这些是当前差距，不是已通过的美术项。三色的士涂装与经典车型参考已有依据，但尺寸仍为估计。
