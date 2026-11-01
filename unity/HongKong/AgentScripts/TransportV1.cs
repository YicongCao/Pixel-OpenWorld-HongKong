using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using HongKong.Walk;
public static class TransportV1
{
 const string Scene="Assets/HongKong/Authored/CantonPolish/CantonRoadPolish.unity";
 const string Dir="Assets/HongKong/Authored/TransportV1";
 static Material labelMat;
 static Material yellow,blue,black,glass,white,silver,red,seat,amber;
 static int meshId;
 static List<Transform> wheels=new List<Transform>();
 static Material Mat(string name,Color color,float smooth=.25f)
 {var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.name=name;m.color=color;m.SetFloat("_Smoothness",smooth);AssetDatabase.CreateAsset(m,Dir+"/"+name+".mat");return m;}
 static GameObject Part(Transform parent,string name,Vector3 pos,Vector3 size,Material mat,PrimitiveType kind=PrimitiveType.Cube)
 {var g=GameObject.CreatePrimitive(kind);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=size;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());var r=g.GetComponent<Renderer>();r.sharedMaterial=mat;r.lightProbeUsage=LightProbeUsage.BlendProbes;return g;}
 static GameObject Box(Transform p,string n,float x,float y,float z,float sx,float sy,float sz,Material m)=>Part(p,n,new Vector3(x,y,z),new Vector3(sx,sy,sz),m);
 static void Text(Transform p,string value,Vector3 pos,float size,Color color,bool reverse=false)
 {var g=new GameObject("Marking "+value.Replace("\n"," "));g.transform.SetParent(p,false);g.transform.localPosition=pos;if(reverse)g.transform.localRotation=Quaternion.Euler(0,180,0);var t=g.AddComponent<TextMesh>();t.text=value;t.fontSize=64;t.characterSize=size*.22f;t.anchor=TextAnchor.MiddleCenter;t.alignment=TextAlignment.Center;t.color=color;t.font=WorldLabel.ReadableFont;g.GetComponent<Renderer>().sharedMaterial=t.font.material;g.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;var l=g.AddComponent<WorldLabel>();l.content=value;l.fallback=value;l.depthMaterial=labelMat;g.GetComponent<Renderer>().sharedMaterial=labelMat;labelMat.mainTexture=t.font.material.mainTexture;}
 // Extruded side silhouette: custom cabin/hood profiles, not a stack of opaque full-size boxes.
 static GameObject Profile(Transform p,string name,Vector2[] shape,float width,Material mat)
 {
  int n=shape.Length;var v=new Vector3[n*2];for(int i=0;i<n;i++){v[i]=new Vector3(shape[i].x,shape[i].y,-width/2);v[i+n]=new Vector3(shape[i].x,shape[i].y,width/2);}
  var tris=new List<int>();for(int i=1;i<n-1;i++){tris.AddRange(new[]{0,i+1,i,n,n+i,n+i+1});}
  for(int i=0;i<n;i++){int j=(i+1)%n;tris.AddRange(new[]{i,j,j+n,i,j+n,i+n});}
  var mesh=new Mesh();mesh.name=name;mesh.vertices=v;mesh.triangles=tris.ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Dir+"/Mesh-"+(meshId++)+".asset");
  var g=new GameObject(name);g.transform.SetParent(p,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;var r=g.AddComponent<MeshRenderer>();r.sharedMaterial=mat;r.lightProbeUsage=LightProbeUsage.BlendProbes;return g;
 }
 static void Wheel(Transform p,float x,float z,float radius)
 {
  var g=Part(p,"Rolling wheel",new Vector3(x,radius,z),new Vector3(radius*2,.12f,radius*2),black,PrimitiveType.Cylinder);g.transform.localRotation=Quaternion.Euler(90,0,0);wheels.Add(g.transform);
  var hub=Part(g.transform,"Five spoke alloy",new Vector3(0,z>0?.99f:-.99f,0),new Vector3(.58f,.06f,.58f),silver,PrimitiveType.Cylinder);
  for(int i=0;i<5;i++){float a=i*Mathf.PI*2/5;var slot=Part(g.transform,"Rim recess",new Vector3(Mathf.Cos(a)*.18f,z>0?1.06f:-1.06f,Mathf.Sin(a)*.18f),new Vector3(.12f,.015f,.09f),black);}
 }
 static GameObject Taxi(string region,Color color)
 {
  var root=new GameObject("Taxi "+region);var p=root.transform;var body=Mat("Taxi "+region+" paint",color,.42f);var roof=region=="urban"?silver:white;wheels.Clear();
  Profile(p,"Comfort body silhouette",new[]{new Vector2(-2.35f,.45f),new Vector2(2.35f,.45f),new Vector2(2.35f,.83f),new Vector2(1.25f,1.01f),new Vector2(-1.28f,1.01f),new Vector2(-2.35f,.9f)},1.72f,body);
  Profile(p,"Silver or white cabin",new[]{new Vector2(-1.5f,.96f),new Vector2(1.28f,.96f),new Vector2(.66f,1.62f),new Vector2(-.95f,1.62f)},1.58f,roof);
  Profile(p,"Sloped windshield",new[]{new Vector2(.73f,1.57f),new Vector2(1.29f,1.03f),new Vector2(1.27f,1.02f),new Vector2(.70f,1.57f)},1.39f,glass);
  Profile(p,"Rear windscreen",new[]{new Vector2(-1.47f,1.02f),new Vector2(-.96f,1.58f),new Vector2(-.94f,1.58f),new Vector2(-1.44f,1.02f)},1.37f,glass);
  foreach(float z in new[]{-.798f,.798f})
  {
   var a=Profile(p,"Front side window",new[]{new Vector2(-.12f,1.07f),new Vector2(1.09f,1.07f),new Vector2(.61f,1.54f),new Vector2(-.12f,1.54f)},.012f,glass);a.transform.localPosition=new Vector3(0,0,z);
   var b=Profile(p,"Rear side window",new[]{new Vector2(-1.30f,1.07f),new Vector2(-.23f,1.07f),new Vector2(-.23f,1.54f),new Vector2(-.90f,1.54f)},.012f,glass);b.transform.localPosition=new Vector3(0,0,z);
   Box(p,"Black side protector",0,.65f,z*1.1f,4.52f,.075f,.035f,black);
   Box(p,"Door seam",-.20f,.84f,z*1.09f,.018f,.31f,.018f,black);
   foreach(float x in new[]{-.95f,.2f})Box(p,"Chrome handle",x,.94f,z*1.10f,.18f,.045f,.04f,silver);
   Box(p,"Wing mirror",1.05f,1.07f,z*1.23f,.23f,.14f,.17f,black);
   Text(p,"的士\nTAXI",new Vector3(-.8f,.79f,z*1.105f),.075f,Color.white,z>0);
   Wheel(p,1.47f,z*1.11f,.34f);Wheel(p,-1.47f,z*1.11f,.34f);
  }
  Box(p,"Roof TAXI lantern",-.05f,1.74f,0,.64f,.23f,.36f,white);
  foreach(float z in new[]{-.187f,.187f})Text(p,"TAXI",new Vector3(-.05f,1.75f,z),.09f,new Color(.75f,.08f,.08f),z>0);
  foreach(float x in new[]{-2.38f,2.38f})Box(p,"Impact bumper",x,.5f,0,.10f,.17f,1.75f,black);
  Box(p,"Front grille",2.405f,.78f,0,.025f,.24f,.78f,black);
  for(int j=0;j<4;j++)Box(p,"Grille chrome",2.42f,.69f+j*.055f,0,.02f,.012f,.73f,silver);
  foreach(float z in new[]{-.61f,.61f}){Box(p,"Headlamp",2.4f,.79f,z,.03f,.22f,.40f,white);Box(p,"Rear brake lamp",-2.40f,.8f,z,.03f,.24f,.36f,red);}
  Box(p,"White front plate",2.435f,.53f,0,.02f,.13f,.42f,white);Box(p,"Yellow rear plate",-2.435f,.53f,0,.02f,.13f,.42f,yellow);
  var bc=root.AddComponent<BoxCollider>();bc.center=new Vector3(0,.8f,0);bc.size=new Vector3(4.8f,1.5f,1.85f);
  PrefabUtility.SaveAsPrefabAsset(root,Dir+"/Taxi-"+region+".prefab");return root;
 }
 static GameObject Bus()
 {
  var root=new GameObject("H2 semi-open sightseeing bus");var p=root.transform;wheels.Clear();
  Box(p,"Lower yellow body",0,.88f,0,11.4f,.95f,2.5f,yellow);
  Box(p,"Lower deck floor",0,.53f,0,11.3f,.13f,2.4f,black);
  Box(p,"Upper deck floor",0,2.72f,0,11.4f,.14f,2.5f,black);
  foreach(float z in new[]{-1.26f,1.26f})
  {
   Box(p,"Blue tour wrap",-1.2f,1.03f,z,7.5f,.65f,.035f,blue);
   Box(p,"Upper yellow belt",0,3.02f,z,11.4f,.50f,.07f,yellow);
   Box(p,"Upper blue band",-1.0f,3.04f,z*1.03f,7.4f,.44f,.025f,blue);
   for(int i=0;i<9;i++){float x=-4.7f+i*1.12f;Box(p,"Lower window",x,1.98f,z,1.02f,1.2f,.055f,glass);Box(p,"Window pillar",x-.54f,2,z,.06f,1.35f,.10f,black);}
   Box(p,"Open deck safety rail",-1.55f,3.57f,z,8.0f,.045f,.06f,silver);
   for(int i=0;i<9;i++)Box(p,"Rail post",-5.4f+i*.95f,3.40f,z,.04f,.36f,.05f,silver);
   Text(p,"HOP ON HOP OFF",new Vector3(-.7f,3.05f,z*1.055f),.19f,Color.white,z>0);
   Text(p,"觀光城巴",new Vector3(4.2f,3.02f,z*1.06f),.17f,new Color(.7f,.08f,.05f),z>0);
   Text(p,"HK CITY SIGHTSEEING",new Vector3(-1.1f,.95f,z*1.055f),.105f,Color.white,z>0);
   for(int i=0;i<9;i++)Box(p,"Pixel skyline wrap",-4.7f+i*.8f,.71f,z*1.06f,.38f,.13f+(i%3)*.08f,.015f,i%2==0?white:yellow);
   Wheel(p,3.8f,z*.99f,.49f);Wheel(p,-2.75f,z*.99f,.49f);Wheel(p,-4.0f,z*.99f,.49f);
  }
  // Front upper roof section and open rear seats are visible from the locked exploration angle.
  Box(p,"Front sheltered roof",4.05f,4.24f,0,3.3f,.16f,2.58f,yellow);
  foreach(float z in new[]{-1.25f,1.25f}){Box(p,"Sheltered upper side window",4.05f,3.77f,z,3.1f,.84f,.05f,glass);foreach(float x in new[]{2.45f,4f,5.58f})Box(p,"Upper pillar",x,3.78f,z,.08f,.88f,.08f,black);}
  for(int i=0;i<7;i++)foreach(float z in new[]{-.77f,.77f})
  {float x=-4.7f+i*.94f;Box(p,"Blue upper seat cushion",x,3.02f,z,.53f,.13f,.48f,seat);Box(p,"Blue upper seat back",x-.26f,3.30f,z,.10f,.6f,.48f,seat);Box(p,"Seat leg",x,2.90f,z,.08f,.25f,.30f,silver);}
  Box(p,"Front lower windscreen",5.73f,1.95f,0,.06f,1.28f,2.25f,glass);
  Box(p,"Front upper windscreen",5.73f,3.75f,0,.06f,.78f,2.25f,glass);
  Box(p,"Rear panel",-5.73f,2.02f,0,.10f,1.4f,2.5f,yellow);
  for(int j=0;j<7;j++)Box(p,"Rear ventilation",-5.79f,.8f+j*.085f,0,.025f,.026f,1.7f,black);
  foreach(float z in new[]{-.94f,.94f}){Box(p,"Bus headlight",5.79f,.74f,z,.04f,.24f,.35f,white);Box(p,"Tail lamp",-5.79f,.7f,z,.04f,.35f,.18f,red);}
  var door=Box(p,"Front passenger door",4.62f,1.46f,1.32f,1.12f,2.05f,.07f,black);Box(door.transform,"Door glazing",0,.15f,0,.88f,1.5f,.09f,glass);
  Box(p,"Centre exit door",.75f,1.46f,1.32f,1.12f,2.05f,.08f,black);
  foreach(float z in new[]{-1.32f,1.36f}){Box(p,"LED destination backing",4.5f,2.43f,z,1.8f,.32f,.035f,black);Text(p,"H2  中環",new Vector3(4.5f,2.44f,z+(z>0?.03f:-.03f)),.12f,new Color(1,.65f,.15f),z>0);}
  var bc=root.AddComponent<BoxCollider>();bc.center=new Vector3(0,2.1f,0);bc.size=new Vector3(11.6f,4.2f,2.65f);
  PrefabUtility.SaveAsPrefabAsset(root,Dir+"/Sightseeing-H2.prefab");return root;
 }
 static GameObject Stop()
 {
  var g=new GameObject("KMB Gateway Hotel | design placement");var p=g.transform;
  Part(p,"Round steel pole",new Vector3(0,1.8f,0),new Vector3(.085f,1.8f,.085f),silver,PrimitiveType.Cylinder);
  var disc=Part(p,"Red roundel",new Vector3(0,3.08f,0),new Vector3(1.06f,.045f,1.06f),red,PrimitiveType.Cylinder);disc.transform.localRotation=Quaternion.Euler(90,0,0);
  Text(p,"KMB 九巴",new Vector3(0,3.38f,-.06f),.1f,Color.white);
  string[] routes={"271","271P","271B","271X","271A"};
  for(int i=0;i<5;i++){float x=(i%3-1)*.29f,y=3.13f-i/3*.22f;Box(p,"Route number tile",x,y,-.055f,.28f,.21f,.015f,white);Text(p,routes[i],new Vector3(x,y,-.069f),.081f,Color.black);}
  Text(p,"港威酒店",new Vector3(0,2.73f,-.06f),.082f,Color.white);
  Box(p,"Timetable red cabinet",0,1.55f,0,.69f,1.35f,.28f,red);
  Box(p,"Cabinet rain cap",0,2.27f,0,.80f,.06f,.40f,silver);
  Box(p,"White information panel",0,1.55f,-.15f,.60f,1.22f,.025f,white);
  Text(p,"廣東道\n港威酒店",new Vector3(0,2.04f,-.17f),.087f,Color.black);
  Text(p,"GATEWAY HOTEL",new Vector3(0,1.84f,-.17f),.044f,Color.black);
  for(int i=0;i<5;i++){float y=1.66f-i*.145f;Text(p,routes[i],new Vector3(-.17f,y,-.17f),.068f,Color.black);Box(p,"Information divider",.10f,y,-.17f,.26f,.013f,.01f,silver);}
  foreach(float x in new[]{-.29f,.29f})foreach(float y in new[]{.94f,2.14f})Part(p,"Cabinet fastener",new Vector3(x,y,-.18f),new Vector3(.035f,.035f,.016f),silver,PrimitiveType.Sphere);
  Box(p,"Weighted foot",0,.06f,0,.45f,.12f,.4f,black);
  var c=g.AddComponent<BoxCollider>();c.center=new Vector3(0,1.6f,0);c.size=new Vector3(.8f,3.5f,.4f);
  PrefabUtility.SaveAsPrefabAsset(g,Dir+"/KMB-GatewayHotel.prefab");return g;
 }
 public static object Apply()
 {
  if(Application.isPlaying || EditorSceneManager.GetActiveScene().isDirty)throw new Exception("Save scene and stop Play first");
  if(AssetDatabase.IsValidFolder(Dir))throw new Exception("Transport assets already exist; use explicit refinement");
  if(EditorSceneManager.GetActiveScene().path!=Scene)EditorSceneManager.OpenScene(Scene);
  Directory.CreateDirectory(Dir);AssetDatabase.Refresh();meshId=0;labelMat=new Material(Shader.Find("HongKong/WorldTextDepth"));AssetDatabase.CreateAsset(labelMat,Dir+"/Depth text.mat");
  yellow=Mat("Tour golden yellow",new Color(.97f,.68f,.06f));blue=Mat("Tour blue",new Color(.02f,.30f,.53f));black=Mat("Rubber and black trim",new Color(.025f,.035f,.04f),.13f);glass=Mat("Pixel cool glazing",new Color(.10f,.23f,.29f),.65f);white=Mat("Warm white",new Color(.86f,.88f,.84f));silver=Mat("Satin metal",new Color(.55f,.62f,.65f),.5f);red=Mat("KMB red",new Color(.68f,.035f,.07f));seat=Mat("Blue seats",new Color(.13f,.42f,.57f));amber=Mat("Amber lamp",new Color(1,.55f,.05f));
  var old=UnityEngine.Object.FindFirstObjectByType<CantonBus>();var passenger=old.passenger;UnityEngine.Object.DestroyImmediate(old.gameObject);passenger.gameObject.SetActive(false);
  foreach(var label in UnityEngine.Object.FindObjectsByType<TextMesh>(FindObjectsSortMode.None))if(label.text=="BUS\nDEMO")UnityEngine.Object.DestroyImmediate(label.gameObject);
  var pole=GameObject.Find("Demo stop pole");if(pole)UnityEngine.Object.DestroyImmediate(pole);
  var bus=Bus();var bw=wheels.ToArray();bus.transform.position=new Vector3(-12,.2f,-3);var behavior=bus.AddComponent<CantonBus>();behavior.passThrough=true;behavior.passenger=passenger;behavior.door=bus.transform.Find("Front passenger door");behavior.wheels=bw;
  var stop=Stop();stop.transform.position=new Vector3(5,.20f,.35f);
  foreach(var region in new[]{"urban","nt","lantau"})
  {
   var taxi=Taxi(region,region=="urban"?new Color(.66f,.055f,.07f):region=="nt"?new Color(.035f,.42f,.25f):new Color(.23f,.58f,.76f));
   if(region!="urban"){UnityEngine.Object.DestroyImmediate(taxi);continue;}
   var mover=taxi.AddComponent<CantonTrafficVehicle>();mover.wheels=wheels.ToArray();taxi.transform.position=new Vector3(-24,.02f,-6.8f);
   var second=UnityEngine.Object.Instantiate(taxi);second.name="Urban taxi second";second.transform.position=new Vector3(20,.02f,-6.8f);
  }
  // Signs are probe-lit, but removing the former pole changes the baked scene: rebake before delivery.
  EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  return new{scene=Scene,assets=Dir,redTaxis=2,excluded="green and blue remain prefabs",bus="H2 through only",rebakeRequired=true};
 }
 public static object RefineMarkings()
 {
  labelMat=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Depth text.mat");
  if(!labelMat){labelMat=new Material(Shader.Find("HongKong/WorldTextDepth"));AssetDatabase.CreateAsset(labelMat,Dir+"/Depth text.mat");}
  foreach(var path in AssetDatabase.FindAssets("t:Prefab",new[]{Dir}).Select(AssetDatabase.GUIDToAssetPath))
  {
   var root=PrefabUtility.LoadPrefabContents(path);FixLabels(root);PrefabUtility.SaveAsPrefabAsset(root,path);PrefabUtility.UnloadPrefabContents(root);
  }
  foreach(var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
   if(root.GetComponent<CantonBus>() || root.GetComponent<CantonTrafficVehicle>() || root.name.StartsWith("KMB Gateway"))FixLabels(root);
  EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();return "Fitted depth-tested markings";
 }
 static void FixLabels(GameObject root)
 {
  foreach(var label in root.GetComponentsInChildren<WorldLabel>(true))
  {
   var t=label.GetComponent<TextMesh>();if(!label.depthMaterial)t.characterSize*=.22f;
   label.depthMaterial=labelMat;label.GetComponent<Renderer>().sharedMaterial=labelMat;
   labelMat.mainTexture=WorldLabel.ReadableFont.material.mainTexture;
  }
 }
 public static object Gallery()
 {
  if(Application.isPlaying)throw new Exception("Stop play first");
  var group=new GameObject("Temporary transport inspection");
  var names=new[]{"Taxi-urban","Taxi-nt","Taxi-lantau","Sightseeing-H2","KMB-GatewayHotel"};
  var positions=new[]{new Vector3(-6,0,200),new Vector3(0,0,200),new Vector3(6,0,200),new Vector3(-2,0,207),new Vector3(7,0,206)};
  for(int i=0;i<names.Length;i++){var g=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Dir+"/"+names[i]+".prefab"));g.transform.SetParent(group.transform);g.transform.position=positions[i];}
  foreach(var t in group.GetComponentsInChildren<TextMesh>()){t.font=WorldLabel.ReadableFont;t.font.RequestCharactersInTexture(t.text,64);t.GetComponent<Renderer>().sharedMaterial=t.GetComponent<WorldLabel>().depthMaterial;}
  labelMat=AssetDatabase.LoadAssetAtPath<Material>(Dir+"/Depth text.mat");labelMat.mainTexture=WorldLabel.ReadableFont.material.mainTexture;
  var cg=new GameObject("Inspection camera");var cam=cg.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=6.5f;cam.transform.position=new Vector3(9,12,182);cam.transform.LookAt(new Vector3(0,1.4f,204));cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.19f,.23f,.25f);
  var rt=new RenderTexture(1800,1100,24);cam.targetTexture=rt;cam.Render();var previous=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(1800,1100,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1800,1100),0,0);tex.Apply();
  var path=Path.GetFullPath("../../docs/08-blocks/tst-canton-demo/transport-validation/asset-gallery.png");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllBytes(path,tex.EncodeToPNG());RenderTexture.active=previous;cam.targetTexture=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(cg);UnityEngine.Object.DestroyImmediate(group);
  return path;
 }
 static float[] Pos(Vector3 v)=>new[]{v.x,v.y,v.z};
 public static object Audit()
 {
  if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Scene)throw new Exception("Wrong scene/state");
  var taxis=UnityEngine.Object.FindObjectsByType<CantonTrafficVehicle>(FindObjectsSortMode.None);
  var bus=UnityEngine.Object.FindFirstObjectByType<CantonBus>();
  if(taxis.Length!=2 || taxis.Any(t=>t.service!="urban") || !bus.passThrough)throw new Exception("Transport deployment rule failed");
  if(GameObject.Find("Taxi nt") || GameObject.Find("Taxi lantau"))throw new Exception("Restricted taxi region in scene");
  float gap=Mathf.Abs(taxis[0].transform.position.z-bus.transform.position.z)-(taxis[0].GetComponent<BoxCollider>().size.z+bus.GetComponent<BoxCollider>().size.z)/2;
  if(gap<1)throw new Exception("Insufficient lane clearance");
  var stop=GameObject.Find("KMB Gateway Hotel | design placement");if(!stop)throw new Exception("Stop missing");
  return new{passed=true,urbanTaxis=taxis.Select(t=>new{name=t.name,position=Pos(t.transform.position),rotation=Pos(t.transform.eulerAngles),scale=Pos(t.transform.localScale),asset="Taxi-urban",source_or_design="design"}).ToArray(),bus=new{position=Pos(bus.transform.position),asset="Sightseeing-H2",source_or_design="design; through service"},stop=new{position=Pos(stop.transform.position),asset="KMB-GatewayHotel",place_id="td:BUS:stop:13364",source_or_design="design placement; TD identity"},laneColliderGapM=gap,prefabs=AssetDatabase.FindAssets("t:Prefab",new[]{Dir}).Length,excludedRegions=new[]{"nt","lantau"}};
 }
 public static object Bake(){if(EditorSceneManager.GetActiveScene().path!=Scene || Application.isPlaying)throw new Exception("Wrong scene/state");return new{started=Lightmapping.BakeAsync()};}
 public static object BakeStatus(){if(!Lightmapping.isRunning){EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();}return new{running=Lightmapping.isRunning,maps=LightmapSettings.lightmaps.Length,probes=LightmapSettings.lightProbes?LightmapSettings.lightProbes.count:0};}
}
