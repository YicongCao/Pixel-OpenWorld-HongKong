using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using HongKong.Walk;
using HongKong.Rendering;
using Newtonsoft.Json;
public static class BuildCantonDemo
{
    const string Dir="Assets/HongKong/Generated/CantonDemo";
    public const string Scene=Dir+"/CantonRoadDemo.unity";
    static Transform group;
    static Material stone,glass,dark,steel,gold,red,green,wood;
    static List<Light> lamps;
    class Layout {public float designLengthM;public Vector3[] route;}
    public static string Create()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().isDirty)throw new Exception("Save current scene and stop Play mode first");
        if(File.Exists(Scene))throw new Exception("Existing demo preserved; use explicit refinement");
        Directory.CreateDirectory(Dir);AssetDatabase.Refresh();
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        group=new GameObject("Canton Road | rectified demo design").transform;lamps=new List<Light>();
        stone=Mat("Stone",new Color(.69f,.66f,.57f));glass=Mat("Glazing",new Color(.16f,.35f,.39f));dark=Mat("Graphite",new Color(.07f,.11f,.14f));steel=Mat("Silver",new Color(.67f,.73f,.72f));gold=Mat("Warm light",new Color(1,.71f,.3f),true);red=Mat("Bus red",new Color(.65f,.08f,.055f));green=Mat("Foliage",new Color(.17f,.31f,.18f));wood=Mat("Wood",new Color(.51f,.31f,.13f));
        var layout=JsonConvert.DeserializeObject<Layout>(File.ReadAllText("Assets/HongKong/Generated/Data/CantonDemo.json"));float half=layout.designLengthM/2;
        Box("Ground",new Vector3(0,-.3f,3),new Vector3(100,.4f,70),stone,true);
        Box("Canton carriageway",new Vector3(0,-.07f,-5),new Vector3(86,.1f,11),Existing("Asphalt"));
        Box("East sidewalk",new Vector3(0,.08f,2),new Vector3(78,.2f,5.5f),Existing("Paving"),true);
        Box("Haiphong connecting street",new Vector3(26,.08f,19),new Vector3(9,.2f,34),Existing("Paving"),true);
        for(int i=-16;i<=16;i++){Box("Pavement seam",new Vector3(i*2.2f,.19f,2),new Vector3(.035f,.012f,5.4f),stone);}
        for(int i=-8;i<=8;i++){Box("Lane dash",new Vector3(i*5,.005f,-6),new Vector3(2,.015f,.12f),steel);Box("Yellow curb",new Vector3(i*5,.02f,-.6f),new Vector3(4.9f,.02f,.1f),gold);}
        MakeApple(-13);MakeShop(9,22,"CANTON PLAZA",new Color(.45f,.36f,.29f));MakeShop(-33,12,"CANTON ROAD",new Color(.42f,.48f,.44f));MakeShop(37,12,"TSIM SHA TSUI",new Color(.56f,.43f,.32f));
        // Side returns along the perpendicular connector provide real depth; rear massing is deliberately simpler.
        for(int i=0;i<3;i++){Box("Alley side wall",new Vector3(18,5,15+i*8),new Vector3(5,10,7.7f),Existing("Plaster"),true);Box("Alley awning",new Vector3(21,3,15+i*8),new Vector3(2,.2f,7),dark);}
        for(int i=0;i<7;i++){Box("Background block",new Vector3(-42+i*14,18+i%3*4,43),new Vector3(12,36+i%3*8,10),Mat("Haze "+i,new Color(.30f+i*.02f,.39f+i*.015f,.40f+i*.015f)),true);}
        foreach(float x in new[]{-28f,-1f,19f,34f}){Tree(new Vector3(x,.2f,4));Lamp(new Vector3(x,.2f,-.4f));}
        Label("廣東道  Canton Road",new Vector3(1,3.3f,4.4f),.10f,Color.white);Box("Street plate",new Vector3(1,3.3f,4.5f),new Vector3(5.6f,.7f,.12f),green);
        Label("海防道  Haiphong Road →",new Vector3(20,4.5f,8),.10f,Color.white);
        Label("海港城  HARBOUR CITY",new Vector3(-15,1.4f,-12),.13f,new Color(.85f,.73f,.40f));
        Box("Harbour City foreground low wall",new Vector3(-15,.6f,-12.3f),new Vector3(34,1.2f,.6f),dark,true);
        for(int i=0;i<12;i++){Box("Bollard",new Vector3(-36+i*6,.6f,-.1f),new Vector3(.12f,1.2f,.12f),steel);}
        // Explicit edge barriers; no fake roads or exits beyond the demo.
        Box("North demo edge",new Vector3(-half-1,1,3),new Vector3(.3f,2,60),dark,true);Box("South demo edge",new Vector3(half+1,1,3),new Vector3(.3f,2,60),dark,true);Box("Rear demo edge",new Vector3(0,1,32),new Vector3(90,2,.3f),dark,true);Box("Front demo edge",new Vector3(0,1,-10),new Vector3(90,2,.3f),dark,true);
        var hero=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HongKong/Generated/Art/Prefabs/WhiteHatPrototype.prefab"));hero.name="White Hat | player";hero.transform.position=new Vector3(-5,.22f,2);var walker=hero.GetComponent<StreetWalker>();walker.route=layout.route;walker.cameraRelativeInput=false;walker.inputReferenceYaw=0;
        var camera=new GameObject("Main Camera").AddComponent<Camera>();camera.tag="MainCamera";camera.nearClipPlane=.1f;camera.farClipPlane=220;camera.fieldOfView=24;camera.gameObject.AddComponent<AudioListener>();var cd=camera.GetUniversalAdditionalCameraData();cd.renderPostProcessing=true;camera.gameObject.AddComponent<CinemachineBrain>();camera.gameObject.AddComponent<CantonPixelView>();
        walker.viewCamera=camera;hero.GetComponent<PixelActor>().viewCamera=camera;
        var cine=new GameObject("Fixed exploration camera").AddComponent<CinemachineCamera>();cine.Follow=hero.transform;cine.transform.rotation=Quaternion.Euler(25,0,0);cine.Lens.FieldOfView=24;var comp=cine.gameObject.AddComponent<CinemachinePositionComposer>();comp.CameraDistance=55;comp.TargetOffset=new Vector3(0,3.2f,0);comp.Damping=new Vector3(.3f,.3f,.3f);camera.transform.SetPositionAndRotation(hero.transform.position+comp.TargetOffset-cine.transform.forward*55,cine.transform.rotation);
        var sun=new GameObject("Late afternoon sun").AddComponent<Light>();sun.type=LightType.Directional;sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(48,-28,0);RenderSettings.sun=sun;RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Exponential;RenderSettings.fogDensity=.009f;
        AssetDatabase.CopyAsset("Assets/HongKong/Generated/Art/HD2DStudyAtmosphere.asset",Dir+"/Atmosphere.asset");var volume=new GameObject("URP atmosphere").AddComponent<Volume>();volume.isGlobal=true;volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Dir+"/Atmosphere.asset");var at=volume.gameObject.AddComponent<StreetAtmosphere>();at.walker=walker;at.sun=sun;at.streetLights=lamps.ToArray();at.volume=volume;
        var hud=new GameObject("HUD").AddComponent<StreetHud>();hud.walker=walker;hud.location="Canton Road / Haiphong connector";hud.stage="Playable demo / compressed layout";
        var validation=new GameObject("Route verification").AddComponent<StreetValidation>();validation.walker=walker;
        MakeBus();for(int i=0;i<10;i++){var p=Person("Pedestrian "+i,new Vector3(-30+i*6,.22f,1),Mat("Coat "+i,Color.HSVToRGB(i*.11f,.38f,.55f)));var mover=p.AddComponent<CantonPedestrian>();mover.a=new Vector3(-30,.22f,.6f+i%2*2.5f);mover.b=new Vector3(30,.22f,mover.a.z);mover.speed=.7f+i*.06f;mover.offset=i*9;mover.leftLeg=p.transform.Find("Left leg");mover.rightLeg=p.transform.Find("Right leg");}
        var worker=Person("Delivery worker placeholder",new Vector3(17,.22f,7),red);Box("Handcart",new Vector3(16,.6f,6),new Vector3(1.1f,.1f,.7f),steel);for(int i=0;i<3;i++)Box("Delivery carton",new Vector3(16,.9f+i*.45f,6),new Vector3(.8f,.43f,.65f),wood);
        ConfigureRenderer(cd);QualitySettings.vSyncCount=1;PlayerSettings.runInBackground=true;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};AssetDatabase.SaveAssets();return Scene;
    }
    static void ConfigureRenderer(UniversalAdditionalCameraData camera)
    {
        var pipeline=GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        var so=new SerializedObject(pipeline);var list=so.FindProperty("m_RendererDataList");var original=list.GetArrayElementAtIndex(0).objectReferenceValue;
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(original),Dir+"/CantonRenderer.asset");var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(Dir+"/CantonRenderer.asset");
        var feature=ScriptableObject.CreateInstance<PixelRendererFeature>();feature.name="MIT pixel pipeline / Canton only";feature.settings.width=960;feature.settings.height=540;feature.settings.lineDarken=.22f;feature.settings.creaseBrighten=.08f;feature.settings.lineAlpha=.35f;
        feature.settings.outlineMaterial=ShaderMat("Outline");feature.settings.compositeMaterial=ShaderMat("Composite");feature.settings.upscaleMaterial=ShaderMat("SharpUpscale");AssetDatabase.AddObjectToAsset(feature,renderer);renderer.rendererFeatures.Add(feature);EditorUtility.SetDirty(renderer);
        int index=list.arraySize;list.arraySize++;list.GetArrayElementAtIndex(index).objectReferenceValue=renderer;so.ApplyModifiedPropertiesWithoutUndo();pipeline.shadowDistance=90;EditorUtility.SetDirty(pipeline);camera.SetRenderer(index);
    }
    static Material ShaderMat(string s){var m=new Material(Shader.Find("HongKong/Pixel/"+s));AssetDatabase.CreateAsset(m,Dir+"/"+s+".mat");return m;}
    static Material Existing(string name)=>AssetDatabase.LoadAssetAtPath<Material>("Assets/HongKong/Generated/Art/Materials/"+name+".mat");
    static Material Mat(string name,Color c,bool glow=false){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=c;m.SetFloat("_Smoothness",.15f);if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",c*.9f);}AssetDatabase.CreateAsset(m,Dir+"/"+name+".mat");return m;}
    static GameObject Box(string n,Vector3 p,Vector3 size,Material m,bool collide=false){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(group);g.transform.position=p;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=m;if(!collide)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
    static void Label(string text,Vector3 p,float size,Color c){var g=new GameObject(text);g.transform.parent=group;g.transform.position=p;var t=g.AddComponent<TextMesh>();t.text=text;t.fontSize=64;t.characterSize=size*.6f;t.anchor=TextAnchor.MiddleCenter;t.color=c;var label=g.AddComponent<WorldLabel>();label.content=text;label.fallback=text;}
    static void MakeApple(float x)
    {
        Box("Apple rear volume",new Vector3(x,7.5f,14),new Vector3(24,15,12),steel,true);
        Box("Glass frontage",new Vector3(x,6.5f,7.8f),new Vector3(23,12.5f,.1f),glass,true);
        for(int i=0;i<8;i++){float px=x-10.5f+i*3;Box("Glass mullion",new Vector3(px,6.5f,7.55f),new Vector3(.09f,12.8f,.15f),steel);Box("Interior light ribbon",new Vector3(px,12.4f,7.42f),new Vector3(.06f,.10f,.12f),gold);}
        Box("Apple mezzanine",new Vector3(x,6,7.3f),new Vector3(24,.3f,.5f),dark);Box("Apple canopy",new Vector3(x,13,7.6f),new Vector3(25,.4f,1.2f),steel);
        for(int floor=0;floor<2;floor++)for(int i=0;i<6;i++){Box("Display table",new Vector3(x-9+i*3.5f,1.3f+floor*6,7.32f),new Vector3(2.1f,.3f,.5f),wood);Box("Warm display",new Vector3(x-9+i*3.5f,2+floor*6,7.28f),new Vector3(.7f,.6f,.08f),gold);}
        Label("Apple",new Vector3(x,9.7f,7.28f),.42f,Color.white);
        for(int i=0;i<10;i++)Box("Visible stair cue",new Vector3(x+10,1+i*.48f,7.2f),new Vector3(2,.15f,.2f),steel);
        Box("Upper commercial mass",new Vector3(x,21,15),new Vector3(25,14,12),stone,true);for(int i=0;i<8;i++)Box("Upper dark window",new Vector3(x-10.5f+i*3,20,8.9f),new Vector3(1.8f,5,.1f),dark);
    }
    static void MakeShop(float x,float width,string name,Color color)
    {
        var wall=Mat(name,color);Box(name+" volume",new Vector3(x,11,14),new Vector3(width,22,12),wall,true);
        for(int floor=0;floor<5;floor++)for(int i=0;i<(int)(width/3);i++){float px=x-width/2+1.5f+i*3,y=6+floor*3;Box("Window",new Vector3(px,y,7.9f),new Vector3(1.9f,1.8f,.12f),glass);Box("Window sill",new Vector3(px,y-.95f,7.65f),new Vector3(2.1f,.12f,.6f),steel);if((i+floor)%3==0)Box("AC unit",new Vector3(px+.6f,y-.6f,7.35f),new Vector3(.6f,.5f,.6f),stone);}
        for(int i=0;i<(int)(width/5);i++)Box("Shop window",new Vector3(x-width/2+2.5f+i*5,1.9f,7.8f),new Vector3(4.4f,3.2f,.18f),glass);
        Box("Shop canopy",new Vector3(x,3.8f,6.9f),new Vector3(width,.3f,2.4f),dark);Label(name,new Vector3(x,4.7f,7.6f),.14f,new Color(1,.85f,.5f));
    }
    static void Lamp(Vector3 p){Box("Lamp post",p+Vector3.up*2.6f,new Vector3(.12f,5.2f,.12f),dark);Box("Lamp head",p+Vector3.up*5.2f,new Vector3(.7f,.2f,.45f),gold);var light=new GameObject("Warm lamp").AddComponent<Light>();light.transform.parent=group;light.transform.position=p+Vector3.up*5;light.type=LightType.Point;light.color=new Color(1,.63f,.28f);light.intensity=7;light.range=12;lamps.Add(light);}
    static void Tree(Vector3 p){Box("Tree trunk",p+Vector3.up*2.5f,new Vector3(.45f,5,.45f),wood);for(int i=0;i<3;i++)Box("Pixel canopy mass",p+new Vector3((i-1)*1.4f,5.5f+i%2,0),new Vector3(2.6f,2.5f,2.4f),green);}
    static GameObject Person(string name,Vector3 p,Material coat)
    {
        var old=group;var root=new GameObject(name);root.transform.position=p;group=root.transform;
        Box("Body",p+Vector3.up*.95f,new Vector3(.38f,.7f,.28f),coat);Box("Head",p+Vector3.up*1.49f,new Vector3(.31f,.32f,.3f),stone);Box("Hair",p+Vector3.up*1.68f,new Vector3(.33f,.12f,.31f),dark);Box("Left leg",p+new Vector3(-.11f,.32f,0),new Vector3(.13f,.55f,.17f),dark);Box("Right leg",p+new Vector3(.11f,.32f,0),new Vector3(.13f,.55f,.17f),dark);group=old;return root;
    }
    static void MakeBus()
    {
        var old=group;var bus=new GameObject("Double decker | HFSM demo stop");group=bus.transform;
        Box("Red body",new Vector3(0,1.9f,0),new Vector3(9,3.8f,2.4f),red);Box("Roof",new Vector3(0,3.85f,0),new Vector3(9.1f,.2f,2.5f),steel);
        foreach(float y in new[]{1.8f,3.1f})foreach(float z in new[]{-1.23f,1.23f})for(int i=0;i<7;i++)Box("Bus window",new Vector3(-3.5f+i*1.15f,y,z),new Vector3(.95f,.8f,.06f),glass);
        foreach(float x in new[]{-3f,3f})foreach(float z in new[]{-1.25f,1.25f})Box("Wheel",new Vector3(x,.45f,z),new Vector3(.8f,.9f,.25f),dark);
        var door=Box("Sliding door",new Vector3(2.5f,1.1f,1.3f),new Vector3(1.1f,2,.08f),dark);Label("DEMO",new Vector3(4,3.1f,1.3f),.08f,Color.yellow);
        group=old;var passenger=Person("Bounded queue / one passenger",new Vector3(3,.22f,.1f),steel);var behavior=bus.AddComponent<CantonBus>();behavior.door=door.transform;behavior.passenger=passenger.transform;
        Box("Demo stop pole",new Vector3(5,1.9f,-.2f),new Vector3(.1f,3.8f,.1f),red);Label("BUS\nDEMO",new Vector3(5,3,-.35f),.07f,Color.white);
    }
}
