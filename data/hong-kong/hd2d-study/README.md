# HD-2D 主街走廊派生资料

2026-11-01。读取本项目 `osm/roads.geojson`，没有修改源图层或隔壁仓库。

- 窗口：CRS84 `[114.164,22.294,114.175,22.310]`。按相交选择广东道、弥敦道、海防道、佐敦道、柯士甸道五组同名道路，共 233 个源要素；含分段与不同道路类型，不等于独立街道数。
- `corridor-roads.geojson`：保留所选源对象完整经纬度几何及属性；不是裁剪后的线，也不是人行图。
- `corridor-evidence.json`：原图层哈希、每组要素源 ID、绘图窗口和米制边界。
- `manifest.json`：本轮派生输出哈希、绘图依赖版本和字体记录；不修改原提取包 manifest。
- 图在 `docs/research/hd2d/corridor-projection.png` 与 `.svg`。绘图时先转 EPSG:2326 并裁剪，左图北向上；右图屏幕横轴对应北，纵轴对应东的反向，东向距离乘 0.55 仅作示意。不是游戏坐标规范或最终相机投影。

© OpenStreetMap contributors / Geofabrik，2026-10-22 快照。没有合并道路分段、去重物理道路、生成导航边、推断桥面连接或提取地形。地图中的线路不能直接作为角色可行走路径。

复现：在项目根目录运行 `../game-research/.venv-hk-data/bin/python scripts/build_hd2d_study.py`；需要 matplotlib、pyproj、shapely 及可用中文字体。报告重新生成使用 `scripts/build_report.py`。

港铁图只读对照使用原资料包内 `mtr-maps-tst.pdf`、`mtr-maps-jor.pdf`；本轮派生的阅读 PNG 位于 `docs/research/hd2d/`。页脚修订日期分别 10/2025、11/2024，不能将它们当作 2026-11-01 现场核查。相应官方链接、九巴 W4 资料日期与镜头设计解释见 `docs/research/HD-2D空间与技术方案.md`。
