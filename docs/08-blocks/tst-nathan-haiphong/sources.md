# 来源与实际观察

查验日期 2026-11-01。地图身份、照片年代和设计活动分开记录。完整文件哈希与输入链见 [数据 manifest](../../../data/blocks/tst-nathan-haiphong/manifest.json) 和 [照片索引](../../../references/blocks/tst-nathan-haiphong/index.json)。

## 地图与公共交通

只读检查了 game-research 的香港 README、catalog/downloads.json 及本项目母包 manifest。既有快照已经覆盖此次窗口，因此复用它们，不重复拷贝全港 GPKG／FGDB。子集保留 70 个道路要素、55 个建筑脚印、234 条三维行人路径，并保留入口候选、关系上下文、完整表派生公交／小巴数据及尖沙咀两张港铁 PDF。

[港铁官方街道图](https://www.mtr.com.hk/archive/ch/services/maps/tst.pdf) 本次重新下载的 SHA-256 与仓库快照一致，印刷日期为 10/2025。已查看整页：印刷图的北在左方；A1 位于弥敦道西侧、海防道北侧，九龙公园／清真寺在该侧。[港铁布局图](https://www.mtr.com.hk/archive/en/services/layouts/tst.pdf) 用于后续核实入口层级，不把站点代表坐标当梯口几何。

[DATA.GOV.HK 九巴数据目录](https://data.gov.hk/en-data/dataset/hk-td-tis_21-etakmb) 提供营办者站点与站序。已实际请求 [站点 API](https://data.etabus.gov.hk/v1/transport/kmb/stop) 和 [站序 API](https://data.etabus.gov.hk/v1/transport/kmb/route-stop)，子集保存在 kmb-live-snapshot.json，保留各响应的生成时间。西侧清真寺有 YT106、YT107、YT108、YT109 多个站位；比如 YT108 对应 `68106C827BF891AB`，位置 `[114.171954,22.298819]`。不是将所有巴士排到同一根站牌。

首轮可从 YT108 的 2 号线候选开始研究（API bound O、seq 3），但车辆方向不能从 O/I 字母直接推断；需结合前后站、道路方向与车型核实。当前只保存站序，不生成道路轨迹或假定实时班次。KMB ID 和运输署 STOP_ID 分别保留。

## 已查看的真实照片

- **A1，2024-08-12，Qwer132477，CC BY 4.0**：[原页面](https://commons.wikimedia.org/wiki/File:Tsim_Sha_Tsui_Station_Exit_A1_2024_08_part1.jpg)。玻璃罩、钢支撑、下行口、旁边上行台阶、扶手、站名条、路面铺装可辨；左侧旧楼有冷气机与六福等招牌。用来核对外观，不证明 2026 店铺现状。拍摄朝向按站口关系推断为大致向西，不使用未经核实的精确方位角。
- **海防道，2024-04-13，LN9267，CC BY-SA 4.0**：[原页面](https://commons.wikimedia.org/wiki/File:Haiphong_Road_13-04-2024.jpg)。页面坐标 `[114.171067,22.298069]`，照片元数据方向约 278°，朝西。商铺在左／南侧，石挡墙、粉色小方砖建筑、树木在右／北侧；车道、行人道、栏杆和灯杆各有空间。UPSO、mannings 为照片年代参考；工人收货活动属于设计，没有从照片推断日程。
- **清真寺，2026-07-21，LN9267，CC BY-SA 4.0**：[原页面](https://commons.wikimedia.org/wiki/File:Kowloon_Masjid_and_Islamic_Centre_21-07-2026.jpg)。页面坐标 `[114.172189,22.298722]`，元数据方向约 293°。可辨白色拱窗屏风、圆顶、棕榈、入口台阶、绿顶巴士雨篷、红色分站牌和候车行人。照片比地铁口图更新，但不是全街现状普查。

原件及 Commons 1280px 版本均已保存，未裁改；后者实际输入 imagegen。地理布局图另由 GIS 生成。许可链接、作者、原图 URL、下载日期及哈希保存在照片索引。照片是美术参考，不能用 EXIF 海拔代替测量高程。

## 缺口与本轮限缩

主镜头重点展示西侧，弥敦道东侧尚缺对应立面参考，不制作其可走商铺。没有逐米路宽、车道转向／信号配时、当前施工、地下通行条件和店铺最新清单。生成的暖光、雨夜、卸货、NPC 数量与站口进出节奏都是设计研究，不能作为现实观察证据。富豪雪糕驻点和休息日聚会未在此段核实，暂不固定布置。
