using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using HongKong.Walk;

public static class AppleBay
{
    const string Scene="Assets/HongKong/Authored/CantonPolish/CantonRoadPolish.unity";
    const string Dir="Assets/HongKong/Authored/CantonPolish/AppleBayV1";
    const string Tex="Assets/HongKong/Generated/AppleBayV1";
    static Transform root;
    static Material oak,stone,silver,dark,wall,screen,accessories,light;
    static Mesh cube;
    static void Guard(){if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Scene)throw new Exception("Open CantonRoadPolish in Edit mode");}
    public static object Apply()
    {
        Guard();if(GameObject.Find("Apple 5.8m bay v1") || AssetDatabase.IsValidFolder(Dir))throw new Exception("Authored bay exists; revise explicitly, never overwrite");
        Directory.CreateDirectory(Dir);AssetDatabase.Refresh();
        root=new GameObject("Apple 5.8m bay v1").transform;
        cube=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/HongKong/Authored/CantonPolish/Meshes/UV2 0.asset");
        if(!cube || cube.vertexCount!=24 || cube.uv2.Length==0)throw new Exception("Expected shared cube with baked UV2");
        var source=Import(Tex+"/material-atlas.png",2048,true);
        oak=Mat("Pale pixel oak",new Color(.78f,.66f,.48f),.25f);oak.mainTexture=Tile(source,0,1,"oak",128);
        stone=Mat("Cool limestone",new Color(.66f,.70f,.72f),.28f);stone.mainTexture=Tile(source,1,1,"limestone",128);
        silver=Mat("Satin silver",new Color(.43f,.52f,.58f),.52f);silver.SetFloat("_Metallic",.38f);
        dark=Mat("Device charcoal",new Color(.025f,.035f,.045f),.38f);
        wall=Mat("Recess warm grey",new Color(.32f,.36f,.37f),.1f);
        accessories=Mat("Accessory backing",new Color(.85f,.88f,.88f),.15f);accessories.mainTexture=Tile(source,0,0,"accessories",256);
        screen=Mat("Screen pixel wallpaper",Color.white,.4f);screen.mainTexture=Tile(source,1,0,"screen",128);screen.EnableKeyword("_EMISSION");screen.SetTexture("_EmissionMap",screen.mainTexture);screen.SetColor("_EmissionColor",Color.white*.35f);
        light=Mat("Warm strip",new Color(.92f,.88f,.74f),.2f);light.EnableKeyword("_EMISSION");light.SetColor("_EmissionColor",new Color(1,.86f,.65f)*1.5f);light.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive;
        // Disable old furniture only in this bay. Keep all original colliders and other bays.
        foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            var p=r.transform.position;
            if(p.x>-15.9f && p.x<-10.1f && new[]{"Oak display tabletop","Oak table leg","Device screen","Device foot"}.Contains(r.name))r.enabled=false;
            if(r.name=="Interior silver column" && Mathf.Abs(p.x+13)<3.1f)r.sharedMaterial=silver;
        }
        // The previous transparent cube rendered front and back surfaces. Preserve its collision,
        // replace its rendering with one plane per facade region; only the bay gets new material.
        var old=GameObject.Find("Clear glazing").GetComponent<MeshRenderer>();old.enabled=false;
        var glass=new Material(old.sharedMaterial);glass.name="Single surface cool glass";glass.color=new Color(.42f,.65f,.73f,.045f);glass.SetFloat("_Smoothness",.82f);glass.SetFloat("_Cull",2);AssetDatabase.CreateAsset(glass,Dir+"/Glass.mat");
        Quad("Unchanged left glazing",new Vector3(-20.35f,6.5f,7.62f),new Vector2(8.9f,12.5f),old.sharedMaterial);
        Quad("Unchanged right glazing",new Vector3(-5.65f,6.5f,7.62f),new Vector2(8.9f,12.5f),old.sharedMaterial);
        foreach(float x in new[]{-14.45f,-11.55f})Quad("Bay single surface glazing",new Vector3(x,6.5f,7.62f),new Vector2(2.9f,12.5f),glass);
        for(int level=0;level<2;level++)
        {
            float y=level*6;
            Box("Bay floor finish",new Vector3(-13,y+.40f,10.05f),new Vector3(5.8f,.04f,4.75f),stone,true);
            Box("Bay inset back wall",new Vector3(-13,y+3.1f,12.17f),new Vector3(5.72f,5.2f,.16f),wall,true);
            Box("Bay ceiling reveal",new Vector3(-13,y+5.72f,10.1f),new Vector3(5.7f,.08f,4.6f),wall,true);
            // One display table with real side thickness, legs and differentiated devices.
            Box("Bay oak tabletop",new Vector3(-13,y+1.38f,9.25f),new Vector3(3.65f,.15f,1.45f),oak,true);
            foreach(float x in new[]{-14.56f,-11.44f})Box("Bay oak end leg",new Vector3(x,y+.89f,9.25f),new Vector3(.12f,.90f,1.12f),oak,false);
            Box("Bay under-table brace",new Vector3(-13,y+1.10f,9.55f),new Vector3(3.25f,.1f,.1f),silver,false);
            Laptop(new Vector3(-14.05f,y+1.47f,9.18f));
            Tablet(new Vector3(-13,y+1.49f,9.17f));
            Phone(new Vector3(-11.95f,y+1.47f,9.17f));
            foreach(float x in new[]{-14.5f,-13.5f,-12.5f})Box("Small product placard",new Vector3(x,y+1.48f,8.72f),new Vector3(.16f,.025f,.10f),silver,false);
            // Separate backing, edge frame, shelf and front artwork preserve 3D recesses.
            Box("Accessory recess frame",new Vector3(-14.4f,y+2.88f,11.96f),new Vector3(2.35f,2.35f,.24f),silver,true);
            Quad("Pixel accessory products",new Vector3(-14.4f,y+2.88f,11.825f),new Vector2(2.18f,2.18f),accessories);
            Box("Accessory oak cabinet",new Vector3(-14.4f,y+.92f,11.65f),new Vector3(2.35f,1.0f,.8f),oak,true);
            Box("Hero screen bezel",new Vector3(-11.7f,y+3.22f,11.96f),new Vector3(2.2f,1.5f,.16f),dark,true);
            Quad("Hero screen pixels",new Vector3(-11.7f,y+3.22f,11.865f),new Vector2(2.08f,1.38f),screen);
            Box("Rear display shelf",new Vector3(-11.7f,y+1.65f,11.55f),new Vector3(2.2f,.1f,.8f),oak,true);
            Phone(new Vector3(-12.2f,y+1.72f,11.48f));Tablet(new Vector3(-11.3f,y+1.72f,11.48f));
            foreach(float x in new[]{-15.4f,-10.6f})Box("Recessed ceiling light strip",new Vector3(x,y+5.60f,10.1f),new Vector3(.065f,.035f,3.9f),light,false);
            Box("Product wash strip",new Vector3(-14.4f,y+4.15f,11.76f),new Vector3(2.12f,.055f,.08f),light,false);
            var lamp=new GameObject("Bay baked area "+level).AddComponent<Light>();lamp.transform.SetParent(root);lamp.transform.SetPositionAndRotation(new Vector3(-13,y+5.45f,10),Quaternion.Euler(90,0,0));lamp.type=LightType.Rectangle;lamp.areaSize=new Vector2(4,2);lamp.range=6;lamp.intensity=6;lamp.color=new Color(1,.94f,.83f);lamp.lightmapBakeType=LightmapBakeType.Baked;
        }
        // Front sill and small joint caps add depth without changing navigation.
        Box("Bay silver sill",new Vector3(-13,.48f,7.58f),new Vector3(5.8f,.10f,.15f),silver,true);
        foreach(float x in new[]{-15.9f,-13f,-10.1f})Box("Bay glazing seam",new Vector3(x,6.45f,7.56f),new Vector3(.032f,12.0f,.045f),silver,false);
        // A local box-projected cubemap will be captured only after lighting finishes.
        var probe=new GameObject("Apple bay local reflection").AddComponent<ReflectionProbe>();probe.transform.SetParent(root);probe.transform.position=new Vector3(-13,6.4f,9.6f);probe.mode=ReflectionProbeMode.Baked;probe.refreshMode=ReflectionProbeRefreshMode.OnAwake;probe.resolution=128;probe.size=new Vector3(7,13,7);probe.boxProjection=true;probe.blendDistance=1;probe.intensity=.65f;probe.hdr=true;
        // Small display props sample probes within the actual interior, not street-side probes.
        var group=UnityEngine.Object.FindFirstObjectByType<LightProbeGroup>();var pts=group.probePositions.ToList();
        foreach(float x in new[]{-15f,-13f,-11f})foreach(float y in new[]{1f,3f,5f,7f,9f,11f})foreach(float z in new[]{8.5f,11.5f})pts.Add(group.transform.InverseTransformPoint(new Vector3(x,y,z)));group.probePositions=pts.ToArray();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return new{bay=new[]{-15.9,-10.1},objects=root.childCount,probePositions=pts.Count,rebakeRequired=true};
    }
    static void Laptop(Vector3 p)
    {Box("Laptop base",p,new Vector3(.6f,.035f,.43f),silver,false);Box("Laptop lid",p+new Vector3(0,.26f,.15f),new Vector3(.60f,.43f,.035f),dark,false);Quad("Laptop wallpaper",p+new Vector3(0,.26f,.126f),new Vector2(.54f,.37f),screen);}
    static void Tablet(Vector3 p)
    {Box("Tablet support",p+new Vector3(0,.08f,0),new Vector3(.3f,.12f,.22f),silver,false);Box("Tablet bezel",p+new Vector3(0,.3f,.04f),new Vector3(.32f,.46f,.035f),dark,false);Quad("Tablet pixels",p+new Vector3(0,.3f,.015f),new Vector2(.27f,.40f),screen);}
    static void Phone(Vector3 p)
    {Box("Phone stand",p,new Vector3(.22f,.045f,.19f),silver,false);Box("Phone body",p+new Vector3(0,.20f,0),new Vector3(.19f,.35f,.035f),dark,false);Quad("Phone pixels",p+new Vector3(0,.20f,-.024f),new Vector2(.15f,.29f),screen);}
    static GameObject Box(string name,Vector3 p,Vector3 s,Material m,bool gi)
    {var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root);g.transform.position=p;g.transform.localScale=s;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());var r=g.GetComponent<MeshRenderer>();r.sharedMaterial=m;r.lightProbeUsage=LightProbeUsage.BlendProbes;if(gi){g.GetComponent<MeshFilter>().sharedMesh=cube;GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.ContributeGI);r.receiveGI=ReceiveGI.Lightmaps;r.scaleInLightmap=1;}return g;}
    static GameObject Quad(string name,Vector3 p,Vector2 s,Material m)
    {var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name=name;g.transform.SetParent(root);g.transform.position=p;g.transform.localScale=new Vector3(s.x,s.y,1);UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());var r=g.GetComponent<MeshRenderer>();r.sharedMaterial=m;r.shadowCastingMode=ShadowCastingMode.Off;r.lightProbeUsage=LightProbeUsage.BlendProbes;return g;}
    static Material Mat(string name,Color c,float smooth)
    {var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.name=name;m.color=c;m.SetFloat("_Smoothness",smooth);AssetDatabase.CreateAsset(m,Dir+"/"+name+".mat");return m;}
    static Texture2D Import(string path,int max,bool read)
    {AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var i=(TextureImporter)AssetImporter.GetAtPath(path);i.isReadable=read;i.maxTextureSize=max;i.npotScale=TextureImporterNPOTScale.None;i.filterMode=FilterMode.Point;i.mipmapEnabled=true;i.textureCompression=TextureImporterCompression.Uncompressed;i.wrapMode=TextureWrapMode.Clamp;i.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Texture2D>(path);}
    static Texture2D Tile(Texture2D source,int x,int y,string name,int max)
    {int w=source.width/2,h=source.height/2;var tile=new Texture2D(w,h,TextureFormat.RGBA32,false);tile.SetPixels(source.GetPixels(x*w,y*h,w,h));tile.Apply();string path=Tex+"/"+name+".png";File.WriteAllBytes(path,tile.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tile);return Import(path,max,false);}
    public static object BakeReflection()
    {Guard();if(Lightmapping.isRunning)throw new Exception("Wait for lightmap bake first");var p=GameObject.Find("Apple bay local reflection").GetComponent<ReflectionProbe>();string path=Dir+"/InteriorReflection.exr";if(!Lightmapping.BakeReflectionProbe(p,path))throw new Exception("Reflection bake failed");AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);p.bakedTexture=AssetDatabase.LoadAssetAtPath<Texture>(path);EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();return new{reflection=path,loaded=p.bakedTexture!=null};}
    public static string Refine()
    {
        Guard();var bay=GameObject.Find("Apple 5.8m bay v1");
        if(GameObject.Find("Apple bay calibrated v1"))throw new Exception("Calibration already applied");
        new GameObject("Apple bay calibrated v1").transform.SetParent(bay.transform);
        // URP 17.3 BaseShaderGUI: preserveSpecular requires One source blending and
        // _ALPHAPREMULTIPLY_ON. Use the installed package's public setup method.
        var glass=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Glass.mat");
        glass.SetFloat("_Blend",0);glass.SetFloat("_BlendModePreserveSpecular",1);BaseShaderGUI.SetupMaterialBlendMode(glass);
        // Original upper floor is at 6.11m, unlike ground-floor top at .37m.
        foreach(Transform t in bay.transform)
            if(t.position.y>6 && t.position.z>8 && !t.GetComponent<ReflectionProbe>())t.position+=Vector3.down*.26f;
        var m=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Pale pixel oak.mat");m.color=new Color(.65f,.55f,.42f);
        var back=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Accessory backing.mat");back.color=new Color(.72f,.76f,.76f);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return "Preserve glass specular, align upper floor, reduce pale retail material glare; rebake required";
    }
    public static object Inspect()
    {return new{scene=EditorSceneManager.GetActiveScene().path,dirty=EditorSceneManager.GetActiveScene().isDirty,playing=Application.isPlaying,frame=Time.frameCount,lightmaps=LightmapSettings.lightmaps.Length,bay=GameObject.Find("Apple 5.8m bay v1")!=null};}
    public static string Preview(float x,bool dof)
    {if(!Application.isPlaying)throw new Exception("Preview requires Play mode");var w=UnityEngine.Object.FindFirstObjectByType<StreetWalker>();w.automatic=false;w.paused=false;var c=w.GetComponent<CharacterController>();c.enabled=false;w.transform.position=new Vector3(x,.22f,2);c.enabled=true;var a=UnityEngine.Object.FindFirstObjectByType<StreetAtmosphere>();a.dofEnabled=dof;a.Apply();Debug.Log("[AppleBayPreview] x="+x+" dof="+dof+" frame="+Time.frameCount);return "Preview placed; allow camera follow to settle before capture";}
}
