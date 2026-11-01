using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using HongKong.Walk;
using Newtonsoft.Json;

public static class BuildStreet
{
    const string Root="Assets/HongKong/Generated";
    public const string ScenePath=Root+"/Scenes/NathanGraybox.unity";
    [Serializable] public class Layout { public Vector3[] route; public Building[] buildings; public float corridorWidth; }
    [Serializable] public class Building { public string sourceId,name,kind; public Vector3[] ring; public float height,baseHeight; }
    static Material concrete, pavement, road, green, white, black, red;
    static int meshNumber;
    public static string Create()
    {
        if(EditorApplication.isPlaying) throw new Exception("Stop owned Play session first");
        if(File.Exists(ScenePath)) throw new Exception("Scene exists: open/refine it; Create never overwrites existing scenes.");
        if(EditorSceneManager.GetActiveScene().isDirty) throw new Exception("Current scene has unsaved edits; save it before changing scenes.");
        foreach(var name in new[]{"Scenes","Materials","Meshes"})Directory.CreateDirectory(Root+"/"+name);
        AssetDatabase.Refresh();
        var data=JsonConvert.DeserializeObject<Layout>(File.ReadAllText(Root+"/Data/layout.json"));
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        concrete=Mat("Concrete",new Color(.58f,.62f,.62f)); pavement=Mat("Pavement",new Color(.56f,.64f,.62f));
        road=Mat("Road",new Color(.17f,.23f,.26f)); green=Mat("Park",new Color(.2f,.32f,.28f));
        white=Mat("Ivory",new Color(.94f,.91f,.8f)); black=Mat("Charcoal",new Color(.07f,.12f,.16f)); red=Mat("MTRRed",new Color(.68f,.07f,.13f));
        var geometry=new GameObject("01 | GIS footprint graybox");
        foreach(var b in data.buildings)
        {
            var go=new GameObject(b.kind+" | "+b.sourceId);go.transform.parent=geometry.transform;
            var mesh=Extrude(b.ring,b.height);
            SaveMesh(mesh);
            go.AddComponent<MeshFilter>().sharedMesh=mesh;
            go.AddComponent<MeshRenderer>().sharedMaterial=b.kind=="mosque"?white:concrete;
            if(b.kind=="shelter")
            { go.transform.position=Vector3.up*2.8f;go.transform.localScale=new Vector3(1,.1f,1); }
            else go.AddComponent<MeshCollider>().sharedMesh=mesh;
        }
        Box("Nathan Road (approximate width)",new Vector3(9,-.5f,30),new Vector3(24,.2f,220),road,false);
        Box("Haiphong Road (approximate width)",new Vector3(-68,-.5f,-15),new Vector3(155,.2f,18),road,false);
        Box("Park background",new Vector3(-74,-.6f,65),new Vector3(150,.2f,120),green,false);
        var path=new GameObject("02 | Source-connected pedestrian surface");
        BuildCorridor(data.route,data.corridorWidth,path.transform);
        for(int i=0;i<data.route.Length;i++)
        {
            var marker=GameObject.CreatePrimitive(PrimitiveType.Sphere); marker.name="Route evidence "+i;
            marker.transform.position=data.route[i]+Vector3.up*.035f;marker.transform.localScale=new Vector3(.18f,.04f,.18f);
            UnityEngine.Object.DestroyImmediate(marker.GetComponent<Collider>());marker.GetComponent<Renderer>().sharedMaterial=white;
            marker.transform.parent=path.transform;
        }
        var hero=new GameObject("White Hat | Player");hero.transform.position=data.route[12]+Vector3.up*.12f;
        var cc=hero.AddComponent<CharacterController>();cc.height=1.65f;cc.radius=.32f;cc.center=new Vector3(0,.84f,0);cc.stepOffset=.25f;cc.slopeLimit=40;cc.skinWidth=.03f;
        var walker=hero.AddComponent<StreetWalker>();walker.route=data.route;
        var body=Box("Graybox robe",hero.transform.position+Vector3.up*.7f,new Vector3(.62f,1.15f,.45f),white,false);body.transform.SetParent(hero.transform,true);
        var hat=new GameObject("Graybox triangular hat");hat.transform.parent=hero.transform;hat.transform.localPosition=new Vector3(0,1.2f,0);
        var hatMesh=Cone(.49f,.85f,8);SaveMesh(hatMesh);hat.AddComponent<MeshFilter>().sharedMesh=hatMesh;hat.AddComponent<MeshRenderer>().sharedMaterial=white;
        var camObj=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener),typeof(CinemachineBrain));camObj.tag="MainCamera";
        var cam=camObj.GetComponent<Camera>();cam.nearClipPlane=.1f;cam.farClipPlane=350;cam.backgroundColor=new Color(.45f,.57f,.60f);cam.clearFlags=CameraClearFlags.SolidColor;
        cam.GetUniversalAdditionalCameraData().renderPostProcessing=true;walker.viewCamera=cam;
        var rig=new GameObject("03 | Stable west-facing exploration camera");
        rig.transform.rotation=Quaternion.Euler(30,-90,0);
        var cine=rig.AddComponent<CinemachineCamera>();cine.Follow=hero.transform;cine.Lens.FieldOfView=32;
        var composer=rig.AddComponent<CinemachinePositionComposer>();composer.CameraDistance=54;composer.TargetOffset=new Vector3(0,2,0);composer.Damping=new Vector3(.35f,.45f,.35f);
        cam.transform.SetPositionAndRotation(hero.transform.position+Vector3.up*2-rig.transform.forward*54,rig.transform.rotation);
        var sun=new GameObject("04 | Sun").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.8f;sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(48,-35,0);RenderSettings.sun=sun;
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.5f,.57f,.62f);
        RenderSettings.fog=true;RenderSettings.fogColor=cam.backgroundColor;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=100;RenderSettings.fogEndDistance=230;
        var hud=new GameObject("05 | Prototype HUD").AddComponent<StreetHud>();hud.walker=walker;
        var check=new GameObject("06 | Traversal evidence").AddComponent<StreetValidation>();check.walker=walker;
        PlayerSettings.productName="香港像素漫游";PlayerSettings.companyName="PixelOpenWorldHongKong";
        PlayerSettings.defaultScreenWidth=1920;PlayerSettings.defaultScreenHeight=1080;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;
        PlayerSettings.runInBackground=true;
        FixBoundaries();
        EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        AssetDatabase.SaveAssets();
        Debug.Log("[StreetBuilder] Created source-based graybox, "+data.route.Length+" route points, "+data.buildings.Length+" footprints");
        return ScenePath;
    }
    public static string FixBoundaries()
    {
        var points=UnityEngine.Object.FindFirstObjectByType<StreetWalker>().route;
        foreach(var obj in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            if(obj.name!="Prototype boundary")continue;
            bool first=Vector3.Distance(obj.position,points[0])<Vector3.Distance(obj.position,points[points.Length-1]);
            int n=first?0:points.Length-1,other=first?1:points.Length-2;
            Vector3 outward=Vector3.ProjectOnPlane(points[n]-points[other],Vector3.up).normalized;
            obj.position=points[n]+Vector3.up*.55f+outward*.45f;
        }
        if(!Application.isPlaying && !string.IsNullOrEmpty(EditorSceneManager.GetActiveScene().path))
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        return "End guards leave room for the controller radius at measured route endpoints";
    }
    static Material Mat(string name,Color c)
    { var p=Root+"/Materials/"+name+".mat";var existing=AssetDatabase.LoadAssetAtPath<Material>(p);if(existing)return existing;var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=c;m.SetFloat("_Smoothness",.12f);AssetDatabase.CreateAsset(m,p);return m; }
    public static GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat,bool collider)
    {var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.position=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=mat;if(!collider)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
    static void SaveMesh(Mesh mesh){AssetDatabase.CreateAsset(mesh,Root+"/Meshes/graybox-"+(meshNumber++)+".asset");}
    static void BuildCorridor(Vector3[] points,float width,Transform parent)
    {
        var vertices=new List<Vector3>();var triangles=new List<int>();
        for(int i=0;i<points.Length;i++)
        {
            Vector3 d=i==0?points[1]-points[0]:i==points.Length-1?points[i]-points[i-1]:points[i+1]-points[i-1];d.y=0;d.Normalize();
            Vector3 side=Vector3.Cross(d,Vector3.up).normalized*width*.5f;
            vertices.Add(points[i]-side);vertices.Add(points[i]+side);
            if(i>0)
            { int a=(i-1)*2,b=i*2;triangles.AddRange(new[]{a,b,a+1,a+1,b,b+1}); }
        }
        var mesh=new Mesh{name="Connected route surface"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();
        // Enforce upward-facing collision triangles independently of source route direction.
        if(mesh.normals[0].y<0){for(int j=0;j<triangles.Count;j+=3){int v=triangles[j];triangles[j]=triangles[j+1];triangles[j+1]=v;}mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();}
        SaveMesh(mesh);var g=new GameObject("Continuous measured-grade path");g.transform.parent=parent;
        g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=pavement;g.AddComponent<MeshCollider>().sharedMesh=mesh;
        for(int i=0;i<points.Length-1;i++)
            for(int side=0;side<2;side++)
            {
                Vector3 a=vertices[i*2+side],b=vertices[(i+1)*2+side],delta=b-a;
                var edge=Box("Graybox edge guard",(a+b)*.5f+Vector3.up*.5f,new Vector3(.12f,1,delta.magnitude),green,true);
                edge.transform.rotation=Quaternion.LookRotation(delta);edge.transform.parent=parent;
                edge.GetComponent<Renderer>().enabled=false;
            }
        for(int i=0;i<2;i++)
        {
            int n=i==0?0:points.Length-1;Vector3 a=vertices[n*2],b=vertices[n*2+1];
            var edge=Box("Prototype boundary",(a+b)*.5f+Vector3.up*.55f,new Vector3(.16f,1.1f,(b-a).magnitude),red,true);
            edge.transform.rotation=Quaternion.LookRotation(b-a);edge.transform.parent=parent;
        }
    }
    static Mesh Cone(float radius,float height,int sides)
    {
        var v=new List<Vector3>();var t=new List<int>();
        for(int i=0;i<sides;i++)
        {float a=i*Mathf.PI*2/sides,b=(i+1)*Mathf.PI*2/sides;int n=v.Count;
         v.Add(new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius));v.Add(Vector3.up*height);v.Add(new Vector3(Mathf.Cos(b)*radius,0,Mathf.Sin(b)*radius));t.AddRange(new[]{n,n+1,n+2});}
        var mesh=new Mesh();mesh.SetVertices(v);mesh.SetTriangles(t,0);mesh.RecalculateNormals();return mesh;
    }
    static Mesh Extrude(Vector3[] ring,float height)
    {
        var v=new List<Vector3>();var t=new List<int>();
        // Polygon side walls use independent vertices for hard normals. Both windings ensure source ring convention is irrelevant.
        for(int i=0;i<ring.Length;i++)
        {int n=v.Count;Vector3 a=ring[i],b=ring[(i+1)%ring.Length];v.AddRange(new[]{a,b,b+Vector3.up*height,a+Vector3.up*height});t.AddRange(new[]{n,n+1,n+2,n,n+2,n+3});v.AddRange(new[]{a,b,b+Vector3.up*height,a+Vector3.up*height});t.AddRange(new[]{n+6,n+5,n+4,n+7,n+6,n+4});}
        // Roof triangulation is deliberately omitted in the graybox: footprint side walls drive collision.
        var mesh=new Mesh{name="GIS footprint walls"};mesh.SetVertices(v);mesh.SetTriangles(t,0);mesh.RecalculateNormals();return mesh;
    }
}
