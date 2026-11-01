#!/usr/bin/env python3
"""Derive a versioned Unity graybox from the archived first-block GIS. Upstream is read-only."""
import hashlib
import json
from pathlib import Path

from pyproj import Transformer
from shapely.geometry import LineString, shape
from shapely.ops import transform

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "data/blocks/tst-nathan-haiphong"
OUT = SOURCE / "implementation"
UNITY = ROOT / "unity/HongKong/Assets/HongKong/Generated/Data"
ORIGIN = [114.17205, 22.29825]
BASE_HKPD = 6.73
CHAIN = [240017881, 240018425, 240023534, 240018426, 240018427,
         290038684, 240018432, 290038685, 240023532, 240018076,
         240018571, 240018665, 240017648]


def main():
    projection = Transformer.from_crs(4326, 2326, always_xy=True)
    ox, oz = projection.transform(*ORIGIN)
    paths = json.loads((SOURCE / "pedestrian.json").read_text())["paths"]
    by_id = {p["properties"]["PedestrianRouteID"]: p for p in paths}
    chain, refs = [], []
    for ident in CHAIN:
        source = by_id[ident]
        prop = source["properties"]
        assert prop["Location"] == 1 and prop["FeatureType"] == 1
        assert prop["FloorID"] is None  # explicitly selected surface route, not a projection join
        points = source["geometry"]["coordinates"][0]
        if chain:
            first = sum((chain[-1][i] - points[0][i]) ** 2 for i in range(3))
            last = sum((chain[-1][i] - points[-1][i]) ** 2 for i in range(3))
            if last < first:
                points = points[::-1]
            gap = sum((chain[-1][i] - points[0][i]) ** 2 for i in range(3)) ** .5
            assert gap < .02, (ident, gap)
            points = points[1:]
        chain.extend(points)
        refs.append(prop["source_id"])
    # Trim the west end within the selected data window; preserve measured grade by interpolation.
    a, b = chain[-2:]
    t = ((ox - 100) - a[0]) / (b[0] - a[0])
    chain[-1] = [a[i] + t * (b[i] - a[i]) for i in range(3)]
    route = [dict(x=round(x-ox, 4), y=round(h-BASE_HKPD, 4), z=round(z-oz, 4))
             for x, z, h in chain]
    line = LineString([(p['x'], p['z']) for p in route])
    buildings, clearance = [], []
    for f in json.loads((SOURCE / "buildings.geojson").read_text())["features"]:
        props = f["properties"]
        geom = transform(projection.transform, shape(f["geometry"]))
        geom = transform(lambda x,y,z=None: (x-ox, y-oz), geom)
        if geom.geom_type == "MultiPolygon":
            assert len(geom.geoms) == 1, (props["source_id"], "multi-part building requires explicit split")
            geom = geom.geoms[0]
        if geom.geom_type != "Polygon" or not (-120 < geom.centroid.x < 0 and -65 < geom.centroid.y < 110):
            continue
        ident = props["osm_id"]
        roof = props.get("type") == "roof"
        # Nested dome footprints must not become separate buildings.
        if ident in ("184759923", "184759925"):
            continue
        kind = "shelter" if roof else "building"
        if ident == "1087128390": kind = "a1"
        if ident == "149352980": kind = "mosque"
        height = {"shelter": 3.1, "a1": 6.0, "mosque": 15.0}.get(kind, 22.0)
        buildings.append(dict(sourceId=props["source_id"], name=props["name"] or ident, kind=kind,
                              height=height, baseHeight=0.0,
                              ring=[dict(x=round(x,4),y=0,z=round(z,4)) for x,z in list(geom.exterior.coords)[:-1]]))
        if not roof:
            clearance.append(dict(source_id=props["source_id"], distance_m=round(line.distance(geom),3)))
    assert min(v['distance_m'] for v in clearance) > .5, clearance
    doc = dict(blockId="tst-nathan-haiphong", originCrs84=ORIGIN,
               originEpsg2326=[ox,oz], heightDatumHkpd=BASE_HKPD, routeSourceIds=refs,
               route=route, corridorWidth=4.0, buildings=buildings,
               status="graybox_design_not_surveyed_pavement",
               notes=["Route is a connected chain of government surface path geometries; current on-site access is unverified.",
                      "Route elevations preserve source HKPD minus 6.73 m. Corridor width and building heights are prototype choices, not measurements.",
                      "OSM shelter roofs are overhead assets, never solid buildings. No geographic compression."])
    encoded=json.dumps(doc,ensure_ascii=False,indent=2)+"\n"
    OUT.mkdir(exist_ok=True); UNITY.mkdir(parents=True,exist_ok=True)
    (OUT/'layout.json').write_text(encoded); (UNITY/'layout.json').write_text(encoded)
    evidence=dict(route_length_m=round(line.length,2), points=len(route), segments=refs,
                  connection_tolerance_m=.02, collider_radius_m=.32, building_clearance=clearance,
                  source_hashes={n:hashlib.sha256((SOURCE/n).read_bytes()).hexdigest()
                                 for n in ['pedestrian.json','buildings.geojson','design-layout.json']},
                  output_sha256=hashlib.sha256(encoded.encode()).hexdigest(),
                  correction="Replaces the illustration's unvalidated line crossing A1 with an explicitly connected surface-route chain.",
                  limitations=["Not a pedestrian-network full graph proof or current site survey.","Corridor edges and building heights remain graybox estimates."])
    (OUT/'layout-validation.json').write_text(json.dumps(evidence,ensure_ascii=False,indent=2)+"\n")
    print(json.dumps({"route_length_m":evidence['route_length_m'],"points":len(route),"buildings":len(buildings),"minimum_clearance_m":min(v['distance_m'] for v in clearance)}))


if __name__ == '__main__':
    main()
