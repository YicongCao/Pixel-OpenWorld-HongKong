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

public static class BuildArtSample
{
    const string G="Assets/HongKong/Generated",A=G+"/Art";
    public const string ScenePath=G+"/Scenes/NathanArtSample.unity";
    static Transform group;
    static Material paving,plaster,tile,asphalt,glass,stone,metal,leaves,trim,dark,red,gold,wood,glassClear;
    static readonly List<Light> lamps=new();
    public static string Create()
    {
        if(EditorApplication.isPlaying)throw new Exception("Edit mode required");
        if(EditorSceneManager.GetActiveScene().isDirty)throw new Exception("Save current scene edits before creating the sample");
        if(File.Exists(ScenePath))throw new Exception("Sample scene exists; refine it without rebuilding over edits");
        foreach(var d in new[]{"Materials","Textures","Meshes","Prefabs"})Directory.CreateDirectory(A+"/"+d);
        AssetDatabase.Refresh();
        PrepareTexture(G+"/Textures/surfaces-v1.png",true,false);
        PrepareTexture(G+"/Textures/white-hat-sheet-v1.png",false,false);
        var atlas=AssetDatabase.LoadAssetAtPath<Texture2D>(G+"/Textures/surfaces-v1.png");
        var mats=new List<Material>();string[] names={"Paving","Plaster","Terracotta","Asphalt","Glass","Stone","Metal","Foliage"};
        for(int i=0;i<8;i++)
        {
            int col=i%4,row=i/4;
            int x0=Mathf.RoundToInt(col*atlas.width/4f)+2,x1=Mathf.RoundToInt((col+1)*atlas.width/4f)-2;
            int y0=Mathf.RoundToInt((1-row)*atlas.height/2f)+2,y1=Mathf.RoundToInt((2-row)*atlas.height/2f)-2;
            var tex=new Texture2D(x1-x0,y1-y0,TextureFormat.RGBA32,false);
            tex.SetPixels(atlas.GetPixels(x0,y0,x1-x0,y1-y0));tex.Apply();
            string path=A+"/Textures/"+names[i]+".png";
            File.WriteAllBytes(path,tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);PrepareTexture(path,false,true);
            var m=Mat(names[i],Color.white);m.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);mats.Add(m);
        }
        paving=mats[0];plaster=mats[1];tile=mats[2];asphalt=mats[3];glass=mats[4];stone=mats[5];metal=mats[6];leaves=mats[7];
        trim=Mat("Warm steel",new Color(.72f,.77f,.74f));dark=Mat("Graphite",new Color(.10f,.16f,.18f));
        red=Mat("Sign red",new Color(.57f,.025f,.055f));gold=Mat("Sign gold",new Color(1,.73f,.22f));
        wood=Mat("Tree bark",new Color(.26f,.20f,.15f));
        glassClear=Mat("Canopy glazing",new Color(.45f,.73f,.70f,.48f));
        glassClear.SetFloat("_Surface",1);glassClear.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);glassClear.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
        glassClear.SetFloat("_ZWrite",0);glassClear.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");glassClear.renderQueue=3000;glassClear.SetOverrideTag("RenderType","Transparent");
        if(!AssetDatabase.CopyAsset(G+"/Scenes/NathanGraybox.unity",ScenePath))throw new Exception("Scene copy failed");
        EditorSceneManager.OpenScene(ScenePath);
        var root=new GameObject("07 | A1 corner art sample (photo-era approximation)");group=root.transform;
        foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            if(r.name=="Continuous measured-grade path")TextureMesh(r,paving,"Sidewalk");
            if(r.name.StartsWith("Nathan Road") || r.name.StartsWith("Haiphong Road")){r.sharedMaterial=asphalt;r.sharedMaterial.mainTextureScale=new Vector2(10,35);}
            if(r.name.Contains("1170462817"))TextureMesh(r,plaster,"Corner building");
            if(r.name.Contains("149352980"))TextureMesh(r,stone,"Mosque massing");
            if(r.name.StartsWith("Route evidence"))r.enabled=false;
        }
        var a1=GameObject.Find("a1 | osm-snapshot:gis_osm_buildings_a_free:1087128390");if(a1)UnityEngine.Object.DestroyImmediate(a1);
        MakeEntrance();MakeCornerShop();MakeParkEdge();MakeShelters();MakeRoadDetails();
        var hero=UnityEngine.Object.FindFirstObjectByType<StreetWalker>();
        foreach(Transform t in hero.transform) t.gameObject.SetActive(false);
        var visual=GameObject.CreatePrimitive(PrimitiveType.Quad);visual.name="Pixel visual | clipped 3D light";UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());
        visual.transform.SetParent(hero.transform,false);
        var heroMat=Mat("White Hat sprite",Color.white);heroMat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(G+"/Textures/white-hat-sheet-v1.png");
        heroMat.SetFloat("_AlphaClip",1);heroMat.SetFloat("_Cutoff",.5f);heroMat.EnableKeyword("_ALPHATEST_ON");heroMat.SetFloat("_Cull",0);heroMat.renderQueue=2450;
        var mr=visual.GetComponent<MeshRenderer>();mr.sharedMaterial=heroMat;mr.shadowCastingMode=ShadowCastingMode.Off;
        var actor=hero.gameObject.AddComponent<PixelActor>();actor.metadata=AssetDatabase.LoadAssetAtPath<TextAsset>(G+"/Textures/hero-frames.json");actor.walker=hero;actor.visual=visual.transform;actor.plane=mr;actor.viewCamera=hero.viewCamera;
        group=hero.transform;var blob=Primitive("Contact shadow proxy",PrimitiveType.Cylinder,hero.transform.position+Vector3.up*.014f,new Vector3(.72f,.012f,.48f),dark);group=root.transform;
        PrefabUtility.SaveAsPrefabAsset(hero.gameObject,A+"/Prefabs/WhiteHatPrototype.prefab");
        var cine=UnityEngine.Object.FindFirstObjectByType<CinemachineCamera>();cine.transform.rotation=Quaternion.Euler(28,-110,0);
        cine.Lens.FieldOfView=32;
        var composer=cine.GetComponent<CinemachinePositionComposer>();composer.CameraDistance=34;composer.TargetOffset=new Vector3(0,2.6f,0);
        hero.viewCamera.transform.SetPositionAndRotation(hero.transform.position+composer.TargetOffset-cine.transform.forward*34,cine.transform.rotation);
        var volume=new GameObject("08 | Atmosphere").AddComponent<Volume>();volume.isGlobal=true;
        var profile=ScriptableObject.CreateInstance<VolumeProfile>();AssetDatabase.CreateAsset(profile,A+"/Atmosphere.asset");volume.sharedProfile=profile;
        var tone=profile.Add<Tonemapping>(true);tone.mode.Override(TonemappingMode.ACES);
        var bloom=profile.Add<Bloom>(true);bloom.intensity.Override(.18f);bloom.threshold.Override(1.1f);
        var color=profile.Add<ColorAdjustments>(true);color.postExposure.Override(.25f);color.contrast.Override(7);color.saturation.Override(-5);
        var dof=profile.Add<DepthOfField>(true);dof.mode.Override(DepthOfFieldMode.Bokeh);dof.focusDistance.Override(34);dof.focalLength.Override(170);dof.aperture.Override(2);dof.active=false;
        foreach(var component in profile.components)AssetDatabase.AddObjectToAsset(component,profile);
        EditorUtility.SetDirty(profile);
        var atmosphere=volume.gameObject.AddComponent<StreetAtmosphere>();atmosphere.sun=RenderSettings.sun;atmosphere.streetLights=lamps.ToArray();atmosphere.volume=volume;atmosphere.walker=hero;
        var hud=UnityEngine.Object.FindFirstObjectByType<StreetHud>();hud.stage="V1-03 / first corner art sample — not final v0.1";
        QualitySettings.vSyncCount=1;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return ScenePath;
    }
    static void PrepareTexture(string path,bool readable,bool mip)
    {
        var t=(TextureImporter)AssetImporter.GetAtPath(path);t.isReadable=readable;t.textureType=TextureImporterType.Default;t.alphaIsTransparency=true;
        t.filterMode=FilterMode.Point;t.mipmapEnabled=mip;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=4096;
        t.wrapMode=mip?TextureWrapMode.Repeat:TextureWrapMode.Clamp;t.SaveAndReimport();
    }
    static Material Mat(string name,Color c)
    {
        var path=A+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}
        m.color=c;m.SetFloat("_Smoothness",.12f);return m;
    }
    static void TextureMesh(MeshRenderer r,Material m,string name)
    {
        var filter=r.GetComponent<MeshFilter>();var mesh=UnityEngine.Object.Instantiate(filter.sharedMesh);var verts=mesh.vertices;var normals=mesh.normals;var uv=new Vector2[verts.Length];
        for(int i=0;i<uv.Length;i++)uv[i]=Mathf.Abs(normals[i].y)>.7f?new Vector2(verts[i].x*.5f,verts[i].z*.5f):new Vector2((Mathf.Abs(normals[i].x)>.7f?verts[i].z:verts[i].x)*.35f,verts[i].y*.35f);
        mesh.uv=uv;AssetDatabase.CreateAsset(mesh,A+"/Meshes/"+name+".asset");filter.sharedMesh=mesh;r.sharedMaterial=m;
    }
    static GameObject Box(string name,Vector3 p,Vector3 s,Material m,bool collide=false)
    {var g=Primitive(name,PrimitiveType.Cube,p,s,m);if(collide)g.AddComponent<BoxCollider>();return g;}
    static GameObject Primitive(string name,PrimitiveType type,Vector3 p,Vector3 s,Material m)
    {var g=GameObject.CreatePrimitive(type);g.name=name;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());g.transform.position=p;g.transform.localScale=s;g.transform.SetParent(group,true);g.GetComponent<Renderer>().sharedMaterial=m;return g;}
    static void Label(string text,string fallback,Vector3 p,float size,Color color,Vector3 rotation)
    {
        var go=new GameObject("Sign | "+fallback);go.transform.SetParent(group,true);go.transform.position=p;go.transform.rotation=Quaternion.Euler(rotation);
        var tm=go.AddComponent<TextMesh>();tm.text=fallback;tm.fontSize=64;tm.characterSize=size;tm.anchor=TextAnchor.MiddleCenter;tm.alignment=TextAlignment.Center;tm.color=color;
        var label=go.AddComponent<WorldLabel>();label.content=text;label.fallback=fallback;
    }
    static void Lamp(Vector3 p)
    {
        Box("Lamp column",p+Vector3.up*2.8f,new Vector3(.13f,5.6f,.13f),metal);
        Box("Lamp housing",p+new Vector3(.25f,5.6f,0),new Vector3(.9f,.16f,.42f),trim);
        var l=new GameObject("Warm street light").AddComponent<Light>();l.transform.SetParent(group);l.transform.position=p+Vector3.up*5.4f;
        l.type=LightType.Point;l.color=new Color(1,.66f,.32f);l.intensity=7;l.range=13;l.shadows=LightShadows.None;l.enabled=false;lamps.Add(l);
    }
    static void MakeEntrance()
    {
        var parent=group;group=new GameObject("A1 | canopy, structure, stairs, sign").transform;group.parent=parent;
        Box("A1 rear / player boundary",new Vector3(-19,2.3f,8),new Vector3(1,4.4f,11),dark,true);
        Box("A1 west wall",new Vector3(-21,3,8),new Vector3(1,6,12),stone,true);
        for(int i=0;i<4;i++)Box("Entrance step",new Vector3(-9.9f-i*.42f,.24f+i*.12f,8),new Vector3(.43f,.20f+i*.24f,10.5f),stone,true);
        Box("Entrance landing",new Vector3(-15.1f,.75f,8),new Vector3(7.5f,.2f,10.5f),stone,true);
        foreach(float z in new[]{2.1f,13.9f})
        {
            Box("Steel portal",new Vector3(-10.8f,3.35f,z),new Vector3(.35f,6.4f,.35f),trim,true);
            Box("Stone side wall",new Vector3(-15.5f,1.7f,z),new Vector3(9,2.8f,.4f),stone,true);
            Box("Glass side screen",new Vector3(-16,4.6f,z),new Vector3(10,2.8f,.1f),glassClear);
        }
        Box("MTR station name band",new Vector3(-10.5f,4.2f,8),new Vector3(.25f,.85f,11.6f),dark);
        Box("A1 red tile",new Vector3(-10.32f,4.2f,12.85f),new Vector3(.12f,.78f,1.1f),red);
        Label("尖沙咀站  Tsim Sha Tsui Station","Tsim Sha Tsui Station",new Vector3(-10.28f,4.2f,7.7f),.09f,new Color(.98f,.94f,.79f),new Vector3(0,-90,0));
        Label("A1","A1",new Vector3(-10.23f,4.2f,12.85f),.14f,Color.white,new Vector3(0,-90,0));
        for(int i=0;i<9;i++)
        {
            float z=2.5f+i*1.4f;
            Box("Canopy steel beam",new Vector3(-15,6.45f,z),new Vector3(11,.18f,.13f),trim);
            if(i<8)Box("Canopy glass",new Vector3(-15,6.55f,z+.7f),new Vector3(10.8f,.08f,1.25f),glassClear);
        }
        Box("Canopy front rail",new Vector3(-9.5f,6.45f,8),new Vector3(.18f,.2f,12.6f),trim);
        foreach(float z in new[]{3.2f,6.5f,9.5f,12.6f})
        {Box("Hand rail",new Vector3(-11.4f,1.35f,z),new Vector3(2.7f,.09f,.09f),trim);Box("Rail post",new Vector3(-10.2f,.95f,z),new Vector3(.09f,1,.09f),trim);}
        Box("Entry lighting",new Vector3(-11.8f,3.6f,8),new Vector3(.14f,.12f,9.8f),gold);
        PrefabUtility.SaveAsPrefabAsset(group.gameObject,A+"/Prefabs/A1EntranceSample.prefab");group=parent;
    }
    static void MakeCornerShop()
    {
        var parent=group;group=new GameObject("Corner shop | photo-era Lukfook reference").transform;group.parent=parent;
        Box("Building roof",new Vector3(-27,22,-29),new Vector3(42,.35f,24),plaster);
        for(int level=0;level<6;level++)
        {
            float y=5.4f+level*2.8f;
            Box("Floor trim",new Vector3(-27,y-1.25f,-16.85f),new Vector3(42,.18f,.32f),trim);
            for(int w=0;w<12;w++)
            {
                float x=-46.5f+w*3.4f;
                Box("Window frame",new Vector3(x,y,-16.7f),new Vector3(2.55f,1.8f,.22f),trim);
                Box("Teal pane",new Vector3(x,y,-16.55f),new Vector3(2.3f,1.58f,.08f),glass);
                Box("Window mullion",new Vector3(x,y,-16.48f),new Vector3(.08f,1.7f,.06f),trim);
                if((w+level)%3==0)
                {Box("Air conditioner",new Vector3(x+.8f,y-.65f,-16.2f),new Vector3(.8f,.55f,.65f),trim);Box("AC grille",new Vector3(x+.8f,y-.65f,-15.84f),new Vector3(.6f,.37f,.04f),dark);}
            }
            for(int w=0;w<6;w++)
            {float z=-19-w*3.6f;Box("Nathan window frame",new Vector3(-5.2f,y,z),new Vector3(.22f,1.8f,2.6f),trim);Box("Nathan window",new Vector3(-5.04f,y,z),new Vector3(.08f,1.56f,2.3f),glass);}
        }
        for(int shop=0;shop<5;shop++)
        {
            float x=-43+shop*8;
            Box("Shop reveal",new Vector3(x,1.75f,-16.5f),new Vector3(7.4f,3.2f,.4f),dark);
            Box("Shop glazing",new Vector3(x,1.7f,-16.23f),new Vector3(6.7f,2.55f,.12f),glass);
            Box("Display base",new Vector3(x,.55f,-15.9f),new Vector3(6.4f,.75f,.55f),stone);
            Box("Awning",new Vector3(x,3.4f,-15.4f),new Vector3(7.8f,.2f,2.5f),shop==4?red:metal);
        }
        Box("Lukfook sign",new Vector3(-10.8f,3.95f,-16.1f),new Vector3(8.4f,.9f,.3f),red);
        Label("六福珠寶  LUKFOOK","LUKFOOK JEWELLERY",new Vector3(-10.8f,3.95f,-15.91f),.115f,new Color(1,.82f,.4f),new Vector3(0,180,0));
        Box("Vertical shop sign",new Vector3(-4.4f,7.2f,-19.5f),new Vector3(.4f,5,1.6f),red);
        Label("六\n福\n珠\n寶","LUK\nFOOK",new Vector3(-4.15f,7.2f,-19.5f),.18f,new Color(1,.82f,.4f),new Vector3(0,-90,0));
        PrefabUtility.SaveAsPrefabAsset(group.gameObject,A+"/Prefabs/CornerShopSample.prefab");group=parent;
    }
    static void MakeParkEdge()
    {
        Box("Haiphong park retaining wall",new Vector3(-63,2,3.5f),new Vector3(78,4,1),stone,true);
        Box("Terracotta pavilion facade",new Vector3(-59,5,13),new Vector3(50,6,.5f),tile);
        for(int i=0;i<7;i++)Tree(new Vector3(-28-i*10,0,7+i%2*2),i);
        Tree(new Vector3(-20,0,25),8);Tree(new Vector3(-25,0,32),9);
        Primitive("Mosque dome silhouette (approximate)",PrimitiveType.Sphere,new Vector3(-34,17,54),new Vector3(16,12,16),stone);
        foreach(float z in new[]{36f,69f})
        {
            Primitive("Minaret silhouette",PrimitiveType.Cylinder,new Vector3(-18,11,z),new Vector3(2.3f,11,2.3f),stone);
            Primitive("Minaret cap",PrimitiveType.Sphere,new Vector3(-18,22,z),new Vector3(2.7f,2.5f,2.7f),stone);
        }
        Lamp(new Vector3(-8.7f,.4f,17));Lamp(new Vector3(-40,0,-2));Lamp(new Vector3(-70,0,-4));
    }
    static void Tree(Vector3 p,int seed)
    {
        Primitive("Tree trunk",PrimitiveType.Cylinder,p+Vector3.up*4,new Vector3(.7f,4,.7f),wood);
        var random=new System.Random(90+seed);
        for(int i=0;i<7;i++)
        {float x=(float)random.NextDouble()*6-3,z=(float)random.NextDouble()*6-3,y=7+(float)random.NextDouble()*3;
         Primitive("Leaf cluster",PrimitiveType.Sphere,p+new Vector3(x,y,z),new Vector3(4.7f,3.2f,4.7f),leaves);}
    }
    static void MakeShelters()
    {
        for(int i=0;i<3;i++)
        {
            float x=-5.8f-i*.6f,z=30+i*17;
            Box("Shelter green roof",new Vector3(x,4,z),new Vector3(2.6f,.27f,9),metal);
            foreach(float dz in new[]{-4f,4f})Box("Shelter post",new Vector3(x-.7f,2.2f,z+dz),new Vector3(.14f,3.8f,.14f),trim);
            Box("KMB stop pole",new Vector3(x+1.1f,2,z-5),new Vector3(.12f,3.5f,.12f),red);
            Box("KMB stop head",new Vector3(x+1.1f,3.4f,z-5),new Vector3(.18f,.8f,.6f),red);
            Label("KMB","KMB",new Vector3(x+1.21f,3.4f,z-5),.07f,Color.white,new Vector3(0,-90,0));
        }
    }
    static void MakeRoadDetails()
    {
        for(int i=0;i<21;i++)
        {
            Box("Nathan lane dash",new Vector3(8,-.38f,-35+i*6),new Vector3(.16f,.025f,2.5f),trim);
            Box("Nathan curb marking",new Vector3(-1.9f,-.37f,-35+i*6),new Vector3(.12f,.025f,5.5f),gold);
        }
        for(int i=0;i<14;i++)Box("Haiphong lane dash",new Vector3(-10-i*6,-.38f,-15),new Vector3(2.5f,.025f,.16f),trim);
        Label("海防道  Haiphong Road","Haiphong Road",new Vector3(-20,3.1f,-2.7f),.08f,Color.white,new Vector3(0,180,0));
        Box("Street name plate",new Vector3(-20,3.1f,-2.9f),new Vector3(5,.65f,.1f),metal);
    }
}
