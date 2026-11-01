# 广东道 demo 数据

本包从本仓库已归档的 `game-research` 香港快照中提取；不修改上游。`manifest.json` 记录来源哈希、窗口、原始几何与输出哈希。OSM 经纬度为 CRS84，行人网为 EPSG:2326 / HKPD；公交来自完整表，站序仍仅为局部。港铁街道图存 `tst-street.pdf`。

`layout.json` 是独立游戏设计：取广东道 OSM 564217668 约 151.4 米中心线，拉直并缩放为 68.1 游戏米，北向屏幕左、东向纵深。人行道宽度、楼高、店面摆放、平地及海防道连接口是快速 demo 编排，不是测绘还原。原始道路与建筑没有被改写。

这次没有把 400 条行人路径自动连成导航网，也没有将 4 个公交站代表点当成精确停靠车道。演示站位未验证，车辆不冒充真实线路。A1 在海防道东端／弥敦道，本场景不凭空增加广东道港铁口。Apple 100 号地址和立面已查官网；店面精确 footprint 对齐仍待补。

重建：`../game-research/.venv-hk-data/bin/python scripts/build_canton_demo_data.py`。仅生成本包与 Unity 的 CantonDemo.json，不覆盖已保存场景。来源署名沿用 [香港数据包](../../hong-kong/README.md)。
