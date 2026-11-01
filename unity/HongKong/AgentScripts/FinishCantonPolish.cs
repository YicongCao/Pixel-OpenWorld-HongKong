using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using HongKong.Walk;
public static class FinishCantonPolish
{
    const string Dir="Assets/HongKong/Authored/CantonPolish";
    public static string Apply()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Dir+"/CantonRoadPolish.unity")throw new Exception("Wrong scene or mode");
        if(GameObject.Find("Polish finish v1"))throw new Exception("Revision already applied; preserve authored changes");new GameObject("Polish finish v1");
        GameObject.Find("Apple").GetComponent<TextMesh>().characterSize=.22f;
        GameObject.Find("Apple").transform.position=new Vector3(-13,9.4f,7.4f);
        var duplicate=GameObject.Find("CANTON PLAZA");if(duplicate)duplicate.SetActive(false);
        foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None))if(light.name.StartsWith("Baked retail interior"))light.intensity=65;
        var worker=GameObject.Find("Delivery worker | frontage patrol");UnityEngine.Object.DestroyImmediate(worker.GetComponent<CantonPedestrian>());var delivery=worker.AddComponent<CantonDelivery>();
        var box=GameObject.CreatePrimitive(PrimitiveType.Cube);box.name="Carried delivery carton";box.transform.SetParent(worker.transform,false);box.transform.localPosition=new Vector3(.25f,.9f,-.08f);box.transform.localScale=new Vector3(.43f,.35f,.3f);UnityEngine.Object.DestroyImmediate(box.GetComponent<Collider>());box.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Materials/Oak.mat");delivery.parcel=box;box.SetActive(false);
        var bus=UnityEngine.Object.FindFirstObjectByType<CantonBus>();
        var rubber=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Materials/Deep petrol.mat");var silver=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Materials/Brushed silver.mat");var luminous=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Materials/Warm luminous.mat");
        foreach(var t in bus.GetComponentsInChildren<Transform>().ToArray())if(t.name=="Wheel" || t.name=="DEMO")UnityEngine.Object.DestroyImmediate(t.gameObject);
        foreach(float x in new[]{-3f,3f})foreach(float z in new[]{-1.25f,1.25f})
        {
            var wheel=GameObject.CreatePrimitive(PrimitiveType.Cylinder);wheel.name="Round rubber wheel";wheel.transform.SetParent(bus.transform,false);wheel.transform.localPosition=new Vector3(x,.52f,z);wheel.transform.localRotation=Quaternion.Euler(90,0,0);wheel.transform.localScale=new Vector3(1.05f,.12f,1.05f);UnityEngine.Object.DestroyImmediate(wheel.GetComponent<Collider>());wheel.GetComponent<Renderer>().sharedMaterial=rubber;
            var hub=GameObject.CreatePrimitive(PrimitiveType.Cylinder);hub.name="Wheel hub";hub.transform.SetParent(bus.transform,false);hub.transform.localPosition=new Vector3(x,.52f,z+Mathf.Sign(z)*.13f);hub.transform.localRotation=Quaternion.Euler(90,0,0);hub.transform.localScale=new Vector3(.46f,.02f,.46f);UnityEngine.Object.DestroyImmediate(hub.GetComponent<Collider>());hub.GetComponent<Renderer>().sharedMaterial=silver;
        }
        foreach(float z in new[]{-1.24f,1.24f})
        {
            AddBox(bus.transform,"Bus silver waist",new Vector3(0,2.45f,z),new Vector3(9,.11f,.045f),silver);
            AddBox(bus.transform,"Lower silver trim",new Vector3(0,.38f,z),new Vector3(9,.15f,.045f),silver);
            for(int i=0;i<7;i++){float x=-3.5f+i*1.15f;foreach(float y in new[]{1.8f,3.1f})AddBox(bus.transform,"Window glint",new Vector3(x,y+.26f,z+Mathf.Sign(z)*.04f),new Vector3(.85f,.035f,.03f),silver);}
        }
        foreach(float z in new[]{-.8f,.8f})AddBox(bus.transform,"Front headlamp",new Vector3(4.52f,.75f,z),new Vector3(.04f,.22f,.32f),luminous);
        var walker=UnityEngine.Object.FindFirstObjectByType<StreetWalker>();walker.gameObject.AddComponent<StreetInputValidation>().checkNathanRoadBoundary=false;
        // Frozen route remains the full 68m regression; the refined visual strip itself is 30m.
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();return "Sign scale, shop fill, round bus wheels, trim and bounded delivery loop";
    }
    static void AddBox(Transform parent,string n,Vector3 p,Vector3 s,Material m){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(parent,false);g.transform.localPosition=p;g.transform.localScale=s;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());g.GetComponent<Renderer>().sharedMaterial=m;}
    public static object Audit()
    {
        var scene=EditorSceneManager.GetActiveScene();var renderers=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        var report=new{scene=scene.path,lightmapped=renderers.Count(r=>r.lightmapIndex>=0 && r.lightmapIndex<LightmapSettings.lightmaps.Length),dynamicLightmapped=renderers.Where(r=>r.GetComponentInParent<StreetWalker>() || r.GetComponentInParent<CantonBus>() || r.GetComponentInParent<CantonPedestrian>() || r.GetComponentInParent<CantonDelivery>()).Count(r=>r.lightmapIndex>=0 && r.lightmapIndex<LightmapSettings.lightmaps.Length),lightmaps=LightmapSettings.lightmaps.Length,probes=LightmapSettings.lightProbes?LightmapSettings.lightProbes.count:0,giMode=Lightmapping.lightingSettings.mixedBakeMode.ToString(),backend=Lightmapping.lightingSettings.lightmapper.ToString(),staticMissingUv2=renderers.Count(r=>GameObjectUtility.AreStaticEditorFlagsSet(r.gameObject,StaticEditorFlags.ContributeGI) && r.GetComponent<MeshFilter>() && r.GetComponent<MeshFilter>().sharedMesh.uv2.Length==0)};
        if(report.lightmaps==0 || report.probes==0 || report.lightmapped==0 || report.dynamicLightmapped!=0 || report.staticMissingUv2!=0)throw new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(report));
        File.WriteAllText(Dir+"/bake-audit.json",Newtonsoft.Json.JsonConvert.SerializeObject(report,Newtonsoft.Json.Formatting.Indented));return report;
    }
}
