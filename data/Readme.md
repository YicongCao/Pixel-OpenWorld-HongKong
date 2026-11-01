# 数据目录

本目录保存香港地理与交通资料及可追溯派生结果。操作前读 [Agent.md](Agent.md)，再读具体资料包的 README 和 manifest。数据能解释地图来源，不自动证明今日通行、商店现状或游戏寻路正确。

## 现有资料导航

- [hong-kong/README.md](hong-kong/README.md) 与 [manifest.json](hong-kong/manifest.json)：从相邻 game-research 只读提取的选区、快照日期、转换规则、源／输出哈希与缺口。
- `hong-kong/osm/`：道路、建筑、POI、铁路、水面及用地等 CRS84 图层；地标名称匹配仅为候选。
- `hong-kong/pedestrian/`：EPSG:2326 米制 XY＋HKPD 高程路径、楼层等关联表和空间筛选验证。
- `hong-kong/transport/`：巴士、小巴、渡轮站点及关联路线／区内站序；来自完整表，区内站序不等于完整线路或车辆轨迹。
- `hong-kong/mtr/`：八个港铁站的街道／站内布局 PDF；站名、出口、通行层级与修订日期须核对，不能代替高铁西九龙站专属图。
- `hong-kong/reference/` 与 [source-catalog.json](hong-kong/source-catalog.json)：字典、条款、旧研究参考与上游来源索引；索引存在不代表全部原件都已复制。
- [street-life/README.md](hong-kong/street-life/README.md) 与 [manifest](hong-kong/street-life/manifest.json)：街道节点、轨道入口候选、地标锚点和工程环境资料索引，独立派生包。
- [hd2d-study/README.md](hong-kong/hd2d-study/README.md) 与 [manifest](hong-kong/hd2d-study/manifest.json)：五组主街的道路选择证据；镜头投影为设计示意，不是导航图。

当前没有已确认的地下供水／排污／电力管网包，也没有完整车道级轨迹、实时人流或现场照片。详细覆盖以各包说明为准。

## 街区数据（实施时建立）

`blocks/<block_id>/` 尚未创建。选段确定后存必要原件／派生数据、来源 manifest、逻辑骨架和缺口；公共快照可按源 ID 引用，不重复复制整包。地理事实与游戏布局／压缩分别保存，运行时不能依赖隔壁仓库。

照片参考归 `references/blocks/<block_id>/`（待创建），视觉资源归 [assets](../assets/Readme.md)，研究结论归 [地理与交通](../docs/02-geography/README.md)，具体进度归 [街区档案](../docs/08-blocks/README.md)。

## 使用与重建

从仓库根运行；按所需包选择操作，更新前先检查脚本的读写范围：

- `scripts/extract_hk.py`：主选区提取；依赖与命令见主资料包 README。
- `scripts/extract_street_life.py`：街道生活派生包提取。
- `scripts/build_hd2d_study.py`：主街派生资料与投影图研究。
- `scripts/validate_docs.py`：文档引用与既有数据哈希检查，适合文档改动。
- `scripts/validate_delivery.py`：主资料包完整检查，含较重的 FGDB 独立扫描；不作为每次编辑说明的必需步骤。

只读使用 `../game-research`，不要在其目录修改文件或重新生成资源。新增或更新本仓库资料后记录日期、来源、许可、坐标和哈希，再按根约定提交推送。
