using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using HongKong.Walk;

public static class PolishCanton
{
    const string Dir="Assets/HongKong/Authored/CantonPolish";
    const string Tex="Assets/HongKong/Generated/CantonPolish/Textures";
    public const string Scene=Dir+"/CantonRoadPolish.unity";
    static Transform root;
    static Material stone,metal,dark,wood,warm,wall,window,shop,paving;
    static Texture2D props,npcs;
    static Material propMat,npcMat;
    static List<object> cuts=new List<object>();
    public static string Create()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().isDirty)throw new Exception("Save current scene and stop Play mode first");
        if(File.Exists(Scene))throw new Exception("Authored scene exists; do not overwrite refinement");
        Directory.CreateDirectory(Dir);Directory.CreateDirectory(Dir+"/Materials");Directory.CreateDirectory(Dir+"/Meshes");AssetDatabase.Refresh();
        EditorSceneManager.OpenScene("Assets/HongKong/Generated/CantonDemo/CantonRoadDemo.unity");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),Scene);
        root=new GameObject("30m refinement | design x -20 to 10").transform;
        var source=Import("material-atlas.png",false);props=Import("street-props.png",true);npcs=Import("pedestrians.png",true);
        stone=Mat("Limestone",new Color(.7f,.69f,.61f));metal=Mat("Brushed silver",new Color(.55f,.60f,.60f));dark=Mat("Deep petrol",new Color(.055f,.09f,.095f));wood=Mat("Oak",new Color(.49f,.30f,.14f));warm=Mat("Warm luminous",new Color(.9f,.66f,.36f),true);
        paving=Textured("Pavers",Tile(source,0,1),new Color(.69f,.71f,.69f));wall=Textured("Mosaic",Tile(source,1,1),new Color(.72f,.71f,.66f));shop=Textured("Shop display",Tile(source,0,0),Color.white);window=Textured("Window service module",Tile(source,1,0),new Color(.8f,.84f,.81f));
        propMat=Textured("Street cutouts",props,Color.white,true);npcMat=Textured("Citizen cutouts",npcs,Color.white,true);
        // Existing source-mapped road, collision skeleton and 68m regression route remain intact.
        foreach(var g in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Select(r=>r.gameObject).ToArray())
        {
            if(g.name=="Pixel canopy mass")UnityEngine.Object.DestroyImmediate(g);
            else if(g.name=="East sidewalk" || g.name=="Haiphong connecting street")
            {
                var m=UnityEngine.Object.Instantiate(paving);m.name=g.name+" pavers";m.mainTextureScale=new Vector2(g.transform.localScale.x/2.5f,g.transform.localScale.z/2.5f);AssetDatabase.CreateAsset(m,Dir+"/Materials/"+m.name+".mat");g.GetComponent<Renderer>().sharedMaterial=m;
            }
        }
        RebuildApple();
        // Detailed frontage finishes only in the selected 30m strip; never claim these generic bays are surveyed shops.
        foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            if(r.name=="Window" && r.transform.position.x<12 && r.transform.position.x>-2)
            {r.sharedMaterial=window;r.transform.localScale=new Vector3(2.45f,2.45f,.15f);}
            if(r.name=="Shop window" && r.transform.position.x<12 && r.transform.position.x>-2)
            {r.sharedMaterial=shop;r.transform.localScale=new Vector3(4.65f,3.4f,.15f);}
            if(r.name=="CANTON PLAZA volume")r.sharedMaterial=wall;
        }
        for(int i=0;i<3;i++)
        {
            float x=.5f+i*4.8f;
            Box("Recessed shop jamb",new Vector3(x-2.35f,1.9f,7.5f),new Vector3(.18f,3.6f,.8f),metal);
            Box("Canvas awning",new Vector3(x,3.7f,6.8f),new Vector3(4.5f,.18f,2),i%2==0?Mat("Awning burgundy "+i,new Color(.34f,.12f,.105f)):dark);
            Box("Awning valance",new Vector3(x,3.45f,5.85f),new Vector3(4.5f,.4f,.08f),dark);
            Box("Shop sign panel",new Vector3(x,4.35f,7.3f),new Vector3(4.45f,.6f,.15f),dark);
            Label(i==0?"日常百貨":i==1?"茶 · 食":"九龍商行",new Vector3(x,4.35f,7.19f),.13f,new Color(.96f,.83f,.51f));
            Card("Potted shrub",new Vector3(x-1.9f,.2f,5.4f),2.0f,1);
            Card("Deliveries",new Vector3(x+1.8f,.2f,6.3f),.9f,3);
            Box("Shop light strip",new Vector3(x,3.5f,7),new Vector3(3.7f,.06f,.09f),warm);
            var light=new GameObject("Baked shop fill").AddComponent<Light>();light.transform.SetParent(root);light.transform.position=new Vector3(x,2.8f,6.8f);light.type=LightType.Point;light.color=new Color(1,.66f,.32f);light.intensity=3;light.range=5;light.lightmapBakeType=LightmapBakeType.Baked;
        }
        // Continuous kerb, metal pedestrian rail and slots: fine structures create scale without squeezing real road width.
        for(int i=0;i<16;i++)
        {
            float x=-21+i*2;
            Box("Kerb stone",new Vector3(x,.12f,-.72f),new Vector3(1.95f,.25f,.28f),stone);
            if(x>-4 && x<8)continue; // leave boarding and crossing approaches open
            Box("Rail upright",new Vector3(x,.8f,-.25f),new Vector3(.065f,1.3f,.065f),dark);
            Box("Rail top",new Vector3(x+1,.95f,-.25f),new Vector3(2,.045f,.045f),dark);
            Box("Rail lower",new Vector3(x+1,.55f,-.25f),new Vector3(2,.035f,.035f),dark);
            for(int k=0;k<5;k++)Box("Rail spindle",new Vector3(x+k*.4f,.74f,-.25f),new Vector3(.025f,.4f,.025f),dark);
        }
        foreach(float x in new[]{-18f,-8f,7f})
        {
            Box("Drain recess",new Vector3(x,.19f,-.45f),new Vector3(.65f,.018f,.25f),dark);
            for(int k=0;k<6;k++)Box("Drain grate",new Vector3(x-.27f+k*.1f,.21f,-.45f),new Vector3(.025f,.02f,.26f),metal);
        }
        foreach(float x in new[]{-28f,-1f,19f,34f})
        {
            Card("Tree crown cutout",new Vector3(x,3.6f,4),5.6f,0);
            // Crossed crown planes provide side depth and cast alpha-tested shadows.
            var side=Card("Tree crown depth",new Vector3(x,3.6f,4),4.8f,0);side.transform.rotation=Quaternion.Euler(0,65,0);
            Box("Tree pit",new Vector3(x,.2f,4),new Vector3(1.35f,.08f,1.35f),dark);
        }
        Card("Orange litter bin",new Vector3(-2.8f,.2f,4.7f),1.15f,2);
        Card("Foreground foliage",new Vector3(-15,.1f,-11),4.6f,1);
        // Fixed exploration view: foreground occlusion, shallow retail interiors, full-depth street connection.
        var walker=UnityEngine.Object.FindFirstObjectByType<StreetWalker>();walker.transform.position=new Vector3(-5,.22f,2);
        var cine=UnityEngine.Object.FindFirstObjectByType<CinemachineCamera>();cine.Lens.FieldOfView=24;var composer=cine.GetComponent<CinemachinePositionComposer>();composer.CameraDistance=49;composer.TargetOffset=new Vector3(0,3.0f,0);
        walker.viewCamera.transform.SetPositionAndRotation(walker.transform.position+composer.TargetOffset-cine.transform.forward*49,cine.transform.rotation);
        foreach(var mover in UnityEngine.Object.FindObjectsByType<CantonPedestrian>(FindObjectsSortMode.None))
        {ConvertCitizen(mover.gameObject,Mathf.RoundToInt(mover.offset/9)%4);mover.leftLeg=null;mover.rightLeg=null;}
        var worker=GameObject.Find("Delivery worker placeholder");if(worker){ConvertCitizen(worker,1);worker.name="Delivery worker | frontage patrol";var move=worker.AddComponent<CantonPedestrian>();move.a=new Vector3(4,.22f,6);move.b=new Vector3(11,.22f,6);move.speed=.55f;}
        // The bus passenger needs front/back travel frames; leave the existing geometry until that direction set is authored.
        var hud=UnityEngine.Object.FindFirstObjectByType<StreetHud>();hud.stage="30m art study / baked afternoon";hud.bakedAfternoon=true;
        var at=UnityEngine.Object.FindFirstObjectByType<StreetAtmosphere>();at.bakedAfternoon=true;at.night=false;at.dofEnabled=false;
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(at.volume.sharedProfile),Dir+"/Atmosphere.asset");at.volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Dir+"/Atmosphere.asset");
        if(at.volume.sharedProfile.TryGet<ColorAdjustments>(out var color)){color.postExposure.value=-.15f;color.saturation.value=-6;}
        at.sun.transform.rotation=Quaternion.Euler(45,-32,0);at.sun.color=new Color(1,.86f,.68f);at.sun.intensity=1.7f;at.sun.lightmapBakeType=LightmapBakeType.Mixed;at.sun.shadowStrength=.85f;
        foreach(var l in at.streetLights)l.enabled=false;
        RenderSettings.ambientMode=AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.30f,.38f,.46f);RenderSettings.ambientEquatorColor=new Color(.18f,.22f,.23f);RenderSettings.ambientGroundColor=new Color(.09f,.075f,.06f);RenderSettings.ambientIntensity=1;RenderSettings.fogColor=new Color(.42f,.52f,.55f);RenderSettings.fogDensity=.006f;
        PrepareBake();
        File.WriteAllText(Dir+"/atlas-cuts.json",Newtonsoft.Json.JsonConvert.SerializeObject(cuts,Newtonsoft.Json.Formatting.Indented));
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();return Scene;
    }
    static void RebuildApple()
    {
        string[] remove={"Apple rear volume","Glass frontage","Glass mullion","Interior light ribbon","Apple mezzanine","Apple canopy","Display table","Warm display","Visible stair cue","Apple"};
        foreach(var t in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))if(remove.Contains(t.name))UnityEngine.Object.DestroyImmediate(t.gameObject);
        float x=-13;
        Box("Apple pale rear wall",new Vector3(x,6.5f,12.5f),new Vector3(24,13,.45f),stone,true);
        Box("Apple left return",new Vector3(x-11.8f,6.5f,10),new Vector3(.4f,13,5.5f),metal,true);
        Box("Apple right return",new Vector3(x+11.8f,6.5f,10),new Vector3(.4f,13,5.5f),metal,true);
        Box("Apple ground floor",new Vector3(x,.28f,10),new Vector3(24,.18f,5.5f),stone);
        Box("Apple first floor",new Vector3(x,6,10),new Vector3(24,.22f,5.5f),stone);
        Box("Apple ceiling",new Vector3(x,12.8f,10),new Vector3(24,.2f,5.5f),stone);
        Box("Apple horizontal fascia",new Vector3(x,6,7.35f),new Vector3(24,.22f,.2f),dark);
        Box("Apple canopy",new Vector3(x,13,7.6f),new Vector3(25,.4f,1.2f),metal);
        var glass=Mat("Clear shop glazing",new Color(.33f,.58f,.61f,.11f));glass.SetFloat("_Surface",1);glass.SetFloat("_ZWrite",0);glass.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);glass.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);glass.SetOverrideTag("RenderType","Transparent");glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");glass.renderQueue=3000;glass.SetFloat("_Smoothness",.5f);
        var pane=Box("Clear glazing",new Vector3(x,6.5f,7.65f),new Vector3(23.6f,12.5f,.03f),glass,true);pane.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
        for(int i=0;i<9;i++)Box("Fine glass joint",new Vector3(x-11.6f+i*2.9f,6.5f,7.57f),new Vector3(.035f,12.4f,.04f),metal);
        foreach(float dx in new[]{-8.5f,-3f,3f,8.5f})Box("Interior silver column",new Vector3(x+dx,6.5f,10.8f),new Vector3(.52f,13,.52f),metal);
        for(int floor=0;floor<2;floor++)for(int i=0;i<5;i++)
        {
            float px=x-9+i*4.2f,y=.8f+floor*6;
            Box("Oak display tabletop",new Vector3(px,y,9.2f),new Vector3(2.6f,.12f,1.1f),wood);
            foreach(float dx in new[]{-1.1f,1.1f})Box("Oak table leg",new Vector3(px+dx,y*.0f+.4f+floor*6,9.2f),new Vector3(.12f,.8f,.8f),wood);
            for(int d=0;d<3;d++){Box("Device screen",new Vector3(px-.8f+d*.8f,y+.21f,9.3f),new Vector3(.35f,.32f,.035f),dark);Box("Device foot",new Vector3(px-.8f+d*.8f,y+.08f,9.1f),new Vector3(.27f,.025f,.24f),metal);}
            Box("Ceiling ribbon",new Vector3(px,5.78f+floor*6,10),new Vector3(.06f,.04f,4.5f),warm);
        }
        for(int i=0;i<12;i++)Box("Stair tread",new Vector3(x+10.5f,.4f+i*.45f,8.4f+i*.3f),new Vector3(1.5f,.15f,.38f),metal);
        Label("Apple",new Vector3(x,9.4f,7.4f),.45f,Color.white);
        for(int floor=0;floor<2;floor++){var light=new GameObject("Baked retail interior "+floor).AddComponent<Light>();light.transform.SetParent(root);light.transform.position=new Vector3(x,4+floor*6,9.5f);light.type=LightType.Point;light.range=15;light.intensity=14;light.color=new Color(1,.9f,.76f);light.lightmapBakeType=LightmapBakeType.Baked;}
    }
    static Texture2D Import(string file,bool alpha)
    {
        string path=Tex+"/"+file;AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var i=(TextureImporter)AssetImporter.GetAtPath(path);i.isReadable=true;i.alphaIsTransparency=alpha;i.filterMode=FilterMode.Point;i.mipmapEnabled=true;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=2048;i.npotScale=TextureImporterNPOTScale.None;i.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static Texture2D Tile(Texture2D src,int col,int row)
    {
        string path=Tex+"/tile-"+col+row+".png";int w=src.width/2,h=src.height/2;
        var t=new Texture2D(w,h,TextureFormat.RGBA32,false);t.SetPixels(src.GetPixels(col*w,row*h,w,h));t.Apply();File.WriteAllBytes(path,t.EncodeToPNG());UnityEngine.Object.DestroyImmediate(t);AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var i=(TextureImporter)AssetImporter.GetAtPath(path);i.maxTextureSize=128;i.filterMode=FilterMode.Point;i.mipmapEnabled=true;i.wrapMode=TextureWrapMode.Repeat;i.textureCompression=TextureImporterCompression.Uncompressed;i.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static Vector4 Cut(Texture2D texture,int column,int row,int columns,int rows)
    {
        int cw=texture.width/columns,ch=texture.height/rows,x0=column*cw,y0=(rows-1-row)*ch,minx=cw,miny=ch,maxx=0,maxy=0;
        var pixels=texture.GetPixels(x0,y0,cw,ch);for(int y=0;y<ch;y++)for(int x=0;x<cw;x++)if(pixels[y*cw+x].a>.5f){minx=Math.Min(minx,x);maxx=Math.Max(maxx,x);miny=Math.Min(miny,y);maxy=Math.Max(maxy,y);}
        if(minx>maxx)throw new Exception("Empty alpha cell");
        var r=new Vector4((maxx-minx+1f)/texture.width,(maxy-miny+1f)/texture.height,(x0+minx)/(float)texture.width,(y0+miny)/(float)texture.height);
        cuts.Add(new{texture=texture.name,column,row,uv=new[]{r.x,r.y,r.z,r.w}});return r;
    }
    static GameObject Card(string name,Vector3 foot,float h,int cell)
    {
        var r=Cut(props,cell%2,cell/2,2,2);var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name=name;g.transform.SetParent(root);g.transform.position=foot+Vector3.up*h*.5f;g.transform.localScale=new Vector3(h*r.x/r.y*props.width/props.height,h,1);UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());
        var m=UnityEngine.Object.Instantiate(propMat);m.name=name+" "+cuts.Count;m.mainTextureScale=new Vector2(r.x,r.y);m.mainTextureOffset=new Vector2(r.z,r.w);AssetDatabase.CreateAsset(m,Dir+"/Materials/"+m.name+".mat");var mr=g.GetComponent<MeshRenderer>();mr.sharedMaterial=m;mr.shadowCastingMode=ShadowCastingMode.TwoSided;return g;
    }
    static void ConvertCitizen(GameObject g,int row)
    {
        foreach(var child in g.GetComponentsInChildren<MeshRenderer>())UnityEngine.Object.DestroyImmediate(child.gameObject);
        var q=GameObject.CreatePrimitive(PrimitiveType.Quad);q.name="Pixel citizen";q.transform.SetParent(g.transform,false);UnityEngine.Object.DestroyImmediate(q.GetComponent<Collider>());var mr=q.GetComponent<MeshRenderer>();mr.sharedMaterial=npcMat;mr.shadowCastingMode=ShadowCastingMode.TwoSided;mr.lightProbeUsage=LightProbeUsage.BlendProbes;
        var a=g.AddComponent<CantonCitizenSprite>();a.plane=mr;a.atlasAspect=npcs.width/(float)npcs.height;a.frames=Enumerable.Range(0,4).Select(c=>Cut(npcs,c,row,4,4)).ToArray();q.transform.localPosition=Vector3.up*.85f;q.transform.localScale=new Vector3(.9f,1.65f,1);
    }
    static Material Mat(string n,Color c,bool glow=false){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.name=n;m.color=c;m.SetFloat("_Smoothness",.12f);if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*.8f);m.globalIlluminationFlags=MaterialGlobalIlluminationFlags.BakedEmissive;}AssetDatabase.CreateAsset(m,Dir+"/Materials/"+n+".mat");return m;}
    static Material Textured(string n,Texture2D t,Color c,bool alpha=false){var m=Mat(n,c);m.mainTexture=t;if(alpha){m.SetFloat("_AlphaClip",1);m.SetFloat("_Cutoff",.5f);m.SetFloat("_Cull",0);m.EnableKeyword("_ALPHATEST_ON");m.SetOverrideTag("RenderType","TransparentCutout");m.renderQueue=2450;}return m;}
    static GameObject Box(string n,Vector3 p,Vector3 s,Material m,bool collision=false){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root);g.transform.position=p;g.transform.localScale=s;g.GetComponent<MeshRenderer>().sharedMaterial=m;if(!collision)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
    static void Label(string text,Vector3 p,float size,Color c){var g=new GameObject(text);g.transform.SetParent(root);g.transform.position=p;var t=g.AddComponent<TextMesh>();t.text=text;t.fontSize=64;t.characterSize=size;t.anchor=TextAnchor.MiddleCenter;t.color=c;var label=g.AddComponent<WorldLabel>();label.content=text;label.fallback="KOWLOON";}
    public static string PrepareBake()
    {
        if(EditorSceneManager.GetActiveScene().path!=Scene)throw new Exception("Wrong scene");
        if(File.Exists(Dir+"/AfternoonLighting.lighting"))throw new Exception("Bake setup already exists; revise explicitly, then use Bake()");
        // Static meshes share secondary UVs by source mesh. Small detail uses probes, large surfaces use lightmaps.
        var meshes=new Dictionary<Mesh,Mesh>();int count=0;
        foreach(var mr in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            GameObjectUtility.SetStaticEditorFlags(mr.gameObject,0);mr.lightProbeUsage=LightProbeUsage.BlendProbes;
            if(mr.GetComponentInParent<StreetWalker>() || mr.GetComponentInParent<CantonPedestrian>() || mr.GetComponentInParent<CantonBus>() || mr.GetComponentInParent<CantonDelivery>() || mr.GetComponentInParent<CantonCitizenSprite>() || mr.GetComponent<TextMesh>() || mr.transform.root.name.Contains("passenger"))continue;
            if(mr.sharedMaterial && mr.sharedMaterial.renderQueue>=3000)continue;
            var mf=mr.GetComponent<MeshFilter>();if(!mf || !mf.sharedMesh)continue;
            float size=mr.bounds.size.sqrMagnitude;if(size<3)continue;
            if(!meshes.TryGetValue(mf.sharedMesh,out var baked))
            {baked=UnityEngine.Object.Instantiate(mf.sharedMesh);baked.name="UV2 "+meshes.Count;Unwrapping.GenerateSecondaryUVSet(baked);AssetDatabase.CreateAsset(baked,AssetDatabase.GenerateUniqueAssetPath(Dir+"/Meshes/"+baked.name+".asset"));meshes[mf.sharedMesh]=baked;}
            mf.sharedMesh=baked;GameObjectUtility.SetStaticEditorFlags(mr.gameObject,StaticEditorFlags.ContributeGI);mr.receiveGI=ReceiveGI.Lightmaps;mr.scaleInLightmap=mr.bounds.center.y>15?.15f:.55f;count++;
        }
        var probe=UnityEngine.Object.FindFirstObjectByType<LightProbeGroup>();if(!probe)probe=new GameObject("Walkable volume light probes").AddComponent<LightProbeGroup>();var pts=new List<Vector3>();
        for(int x=-32;x<=32;x+=4)foreach(float z in new[]{-6f,.4f,3f,6f})foreach(float y in new[]{.5f,1.8f,4.5f})pts.Add(new Vector3(x,y,z));
        for(int z=10;z<=30;z+=4)foreach(float x in new[]{23f,26f,29f})foreach(float y in new[]{.5f,1.8f,4.5f})pts.Add(new Vector3(x,y,z));probe.probePositions=pts.ToArray();
        var ls=new LightingSettings();ls.name="Afternoon mixed GI";ls.bakedGI=true;ls.realtimeGI=false;ls.lightmapper=LightingSettings.Lightmapper.ProgressiveGPU;ls.mixedBakeMode=MixedLightingMode.IndirectOnly;ls.lightmapResolution=8;ls.lightmapMaxSize=1024;ls.lightmapPadding=4;ls.directSampleCount=32;ls.indirectSampleCount=128;ls.environmentSampleCount=64;ls.maxBounces=3;ls.ao=true;ls.aoMaxDistance=.7f;ls.aoExponentIndirect=.5f;ls.directionalityMode=LightmapsMode.CombinedDirectional;
        AssetDatabase.CreateAsset(ls,Dir+"/AfternoonLighting.lighting");Lightmapping.lightingSettings=ls;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());return "Static renderers="+count+" probes="+pts.Count;
    }
    public static string Bake()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Scene)throw new Exception("Open polished scene in Edit mode");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());if(!Lightmapping.BakeAsync())throw new Exception("Bake did not start");return "BakeAsync started; poll and validate persisted output";
    }
    public static string RefreshCitizenAtlas()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Scene)throw new Exception("Wrong scene or mode");
        npcs=Import("pedestrians.png",true);cuts.Clear();
        foreach(var a in UnityEngine.Object.FindObjectsByType<CantonCitizenSprite>(FindObjectsSortMode.None))
        {
            var mover=a.GetComponent<CantonPedestrian>();int row=mover?Mathf.RoundToInt(mover.offset/9)%4:1;
            a.frames=Enumerable.Range(0,4).Select(c=>Cut(npcs,c,row,4,4)).ToArray();a.atlasAspect=npcs.width/(float)npcs.height;
        }
        File.WriteAllText(Dir+"/citizen-cuts-v2.json",Newtonsoft.Json.JsonConvert.SerializeObject(cuts,Newtonsoft.Json.Formatting.Indented));
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();return "Symmetric citizen atlas re-cut";
    }
    public static object BakeStatus()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Scene)throw new Exception("Open polished scene in Edit mode");
        if(!Lightmapping.isRunning){EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();}
        var report=new{running=Lightmapping.isRunning,progress=Lightmapping.buildProgress,lightmaps=LightmapSettings.lightmaps.Length,probes=LightmapSettings.lightProbes?LightmapSettings.lightProbes.count:0,lightingData=AssetDatabase.GetAssetPath(Lightmapping.lightingDataAsset),maps=LightmapSettings.lightmaps.Select(x=>new{color=AssetDatabase.GetAssetPath(x.lightmapColor),direction=AssetDatabase.GetAssetPath(x.lightmapDir)}).ToArray()};
        if(!report.running)File.WriteAllText(Dir+"/bake-result.json",Newtonsoft.Json.JsonConvert.SerializeObject(report,Newtonsoft.Json.Formatting.Indented));return report;
    }
}
