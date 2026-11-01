# 街道生活补充资料

2026-11-01 从旧 `game-research` 资料只读提取；OSM 快照为 2026-10-22，沿用当前项目 CRS84 选区矩形。它不是尖沙咀行政范围，也不是已核实现场设施普查。

- `street-nodes.geojson`：3,257 个原始 PBF 节点，保留原始标签与 `osm:node:<id>`。仅按指定类别筛选节点，不包括全部 ways/relations。类别计数不能相加为独立物件总量，同一节点可有多个类别。
- `rail-entrance-candidates.geojson`：上述节点中 144 个 `railway=subway_entrance` 候选。部分带 `ref`、`level` 或无障碍标签；所属车站、地面/地下、实际开放情况仍需配对港铁 PDF。出口号会在不同车站重复，不能拿它当全局 ID。
- `landmark-evidence.json`：35 个人工选定锚点组，32 组有名称匹配。每个候选保留源图层、ID、名称和代表点。可能匹配到相关巴士站或多个图层，不能把每条匹配都当建筑本体。
- `landmark-evidence.geojson`：同一证据的代表点形式，面对象用内部标签点，**不是入口/活动生成点**。
- `reference-index.json`：旧库 69 项已下载工程/环境资料的索引，未复制全部原件，也未重新逐页核验。
- `manifest.json`：输入哈希、输出哈希、类别统计与限制；补充包独立管理，不改写原提取包 manifest。

复现：在项目根目录执行 `../game-research/.venv-hk-data/bin/python scripts/extract_street_life.py`。需要 osmium、shapely。脚本覆盖本目录的派生 JSON/GeoJSON，手工编辑内容应另存文件。

原始 OSM 节点类目包括：过街点 1,447、信号节点 789、轨道入口 144、电梯 133、树 370、长椅 95、邮箱 72、厕所 71、出租车相关点 45，以及其他少量设施。它们可能与原 POI 图层重复，不能两套相加。缺少标注不代表现实没有设施。

本包没有人群身份、实时人流、商贩日程、音频、街景照片或完整通行规则。NPC 活动以独立设计配置表达，不冒充 GIS 事实。继续保留 OpenStreetMap contributors / Geofabrik 来源及主资料包的使用条件说明。
