# 第一条可走路线 · 派生数据

2026-11-01，实施批次 01。母包原件未修改；来源及许可继承 [街区资料包](../README.md) 与 [manifest](../manifest.json)。

- [layout.json](layout.json)：Unity 的本地米制路线与 26 个西侧建筑／设施体块。
- [layout-validation.json](layout-validation.json)：13 段来源 ID、源文件／输出 SHA-256、连续性阈值、建筑净距与限制。
- 同字节运行时副本：`unity/HongKong/Assets/HongKong/Generated/Data/layout.json`。

原点 CRS84 `[114.17205,22.29825]`，投影 EPSG:2326；Unity X 向东、Z 向北、Y = 原始 HKPD 高程 − 6.73 米。一 Unity 单位为一米。没有按概念图缩放地理，也没有把行人路径当完整地形 DEM。

原设计示意虚线经过 A1 建筑脚印，不能直接用作寻路。此次显式选择 13 段政府地面路径，均为 `Location=1`、`FeatureType=1`、`FloorID=null`，逐端点三维检查连接误差小于 0.02 米；西端截取到本地 X = −100 米并插值高程。派生路线 25 点、179.34 米，中心线距非雨篷建筑最小 2.998 米。**这是所选链路的几何验证，不是全图连通证明或今日现场开放证明。**

4 米走廊宽度、建筑高度、路面横坡、背景与边界均为原型设计。只做西侧可见体块，未把被裁去的东侧楼体解释成现实空地；屋顶雨篷不充当实心建筑，清真寺重复穹顶脚印不作为独立楼体。

从仓库根运行 `../game-research/.venv-hk-data/bin/python scripts/build_v01_layout.py`，只读本仓库归档数据并写本派生目录及 Unity generated 副本；不改上游工程。后续更换路线须保留源 ID、坐标说明并重新通过通行验证。
