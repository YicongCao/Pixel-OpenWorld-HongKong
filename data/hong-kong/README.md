# 香港选区资料包

2026-11-01 从 `../game-research/data/hong-kong` 只读提取。来源为旧库已有快照，不代表今天所有道路、商店与站口均经过现场核查。提取包约 60 MB，不复制旧 3D 模型和引擎工程。

## 空间范围与日期

CRS84 窗口 `[114.150,22.289,114.186,22.328]`；这是西九龙—尖沙咀—尖东—佐敦—油麻地—旺角的研究窗口，含海水和边界缓冲，不是完整行政范围或首版开放面积。

OSM 快照为 2026-10-22；运输署版本表为 2026-10-14；CSDI 行人网元数据版本为 2026-09-16；上游采集批次为 2026-10-23。MTR PDF 的采集日期不能视为其地图修订日期。

## 已保存

- `osm/`：9 个 CRS84 GeoJSON 图层。道路 12,497、建筑 6,729、点 POI 3,085、面 POI 986、交通点 949、交通面 25、铁路 262、水面 26、用地 1,636。几何相交筛选，不切断源对象；部分几何延伸到窗口外。
- `landmark-candidates.geojson`：名称匹配候选。可能匹配到 Apple Hostel 等非目标商户；需要人工选定，不能把文本匹配当已核实地标。
- `pedestrian/paths-hk1980.json`：34,282 条原生 XYZ 路径，EPSG:2326 + EPSG:5738（HKPD）。这是带明确坐标声明的 JSON，不冒充 RFC7946/WGS84 高程 GeoJSON。
- `pedestrian/relations.json`：7 张关联表的选区相关记录，共 1,924 行；保留楼层、建筑、场地、站点、时段关联。枚举必须查随包数据字典，空时段不等于全天开放。
- `transport/`：巴士 320、小巴 334、渡轮 4 个站点；相应区内站序行 4,975 / 849 / 15，关联路线表记录 499 / 84 / 7。由完整表导出，不使用 `raw/transport/td-*.csv` 增量变更表。
- `mtr/`：尖沙咀 tst、尖东 ets、柯士甸 aus、九龙 kow、佐敦 jor、油麻地 ymt、旺角 mok、旺角东 mkk，各街道图与站内布局图，共 16 PDF。九龙站、柯士甸站不是高铁西九龙站；本包没有后者的专属站内图。
- `reference/`：CSDI 数据字典、OSM 使用条件快照与旧项目城市记忆候选。旧记忆表的优先级不自动成为本项目需求。
- `source-catalog.json`：相关下载来源、URL、采集状态和历史校验值；保留上游上下文，目录存在不表示每一项原件都被复制到本包。
- `manifest.json`：实际读取的源文件路径与 SHA-256、输出文件 SHA-256、数量、坐标与缺口。

## 转换规则

OSM GPKG 内部 `srs_id=100000` 的 WKT 表示 OGC CRS84（经度、纬度），不是 EPSG:100000。行人网以转换后的保守 EPSG:2326 矩形筛选，保留整条路径和 Z，不切分投影交叉点。

运输署站点 X/Y 从 EPSG:2326 转到经纬度后按窗口筛选。路线表连接键是 ROUTE_ID；站序保留 ROUTE_SEQ、STOP_SEQ 等原始字段。导出的站序只有窗口内部分，不能作为完整线路；没有生成车辆路线或离线时刻表。不同 mode 保持命名空间。

记录的局部原点为 `[114.170,22.300]`，对应 EPSG:2326 `[835559.6420668046,817895.7546802935]` 米。暂未生成游戏坐标。今后先做 `east = X-X0, north = Y-Y0`，游戏布局另按明确比例映射到 `game_x=东 / game_z=北 / game_y=高`；先扣除明确记录的本地高程基准，原始 HKPD Z 保留不变。建筑在美术上的垂直压缩与地面地图缩放不是一回事。

稳定标识采用 `osm-snapshot:<layer>:<osm_id>`、`3dpn:<PedestrianRouteID>`、`td:<mode>:stop:<STOP_ID>`。OSM 分层导出没有完整保留原始 node/way/relation 类型，因此不凭几何类型编造全局 OSM URL。名称相同不自动合并。

## 验证与复现

```sh
../game-research/.venv-hk-data/bin/python scripts/extract_hk.py
../game-research/.venv-hk-data/bin/python scripts/validate_delivery.py
../game-research/.venv-hk-data/bin/python scripts/build_report.py
```

在本项目根目录执行。可使用其他 Python 环境，依赖见 `scripts/requirements-research.txt`。提取脚本支持 `--source /path/to/game-research/data/hong-kong`；生成的选区数据及离线 HTML 独立可读，只有重建依赖上游。

读取 FGDB 时 GDAL 报告空间索引深度可疑。已绕过空间索引，对全量 465,892 条几何做独立相交扫描，选中 ID 与 34,282 条输出一致。证据见 `pedestrian/spatial-selection-validation.json`。运行 `validate_delivery.py` 会再次验证，而非只信旧日志。

校验成功说明数据提取一致、JSON/PDF 可读、引用完整到已声明范围；不表示已验证楼层通行、商店现状、碰撞、寻路或运行性能。

## 署名与未解决项

记录 OpenStreetMap contributors / Geofabrik、香港地政总署 CSDI、运输署和港铁的来源；OSM 使用条件保存在本包，上游政府条款 URL 保存在 source-catalog。这里只整理来源，不把公开地图、网页照片和真实品牌一律声明为可任意再分发的最终美术。

没有确认地下供水 / 排污 / 电力管线数据；没有逐店现状立面、完整商场室内、完整海岸线面、全部站口通行条件、音频或车道级路径。本轮不复制未经本轮 HD-2D 管线验证的 3D Tiles 样本、规划高度控制和庞大票价表。正式商店位置需要官网 / 街段资料复核。

## 街道生活补充（2026-11-01）

[street-life/README.md](street-life/README.md) 独立保存从原始 PBF 追加的 3,257 个街道节点（含 144 个轨道入口候选）、35 组街区锚点证据及工程/环境资料索引。它使用单独 manifest；与本包 POI 可能重复，不能相加为设施总量。补充提取命令为 `../game-research/.venv-hk-data/bin/python scripts/extract_street_life.py`。

## HD-2D 走廊研究（2026-11-01）

[hd2d-study/README.md](hd2d-study/README.md) 记录从现有道路图层选择的五组道路及来源哈希；没有更新原始地图、生成地形或建立人行导航图。镜头投影是独立设计示意。
