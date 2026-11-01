using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using HongKong.Walk;
public static class BrandCanton
{
    const string Dir="Assets/HongKong/Authored/CantonPolish";
    static Transform root;
    public static string Apply()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Dir+"/CantonRoadPolish.unity")throw new Exception("Open Canton polish in Edit mode");
        if(GameObject.Find("Verified brand signs v1"))throw new Exception("Brand revision already applied; preserve authored changes");
        root=new GameObject("Verified brand signs v1").transform;
        GameObject.Find("Apple").SetActive(false);
        Sign("Apple bitten-apple sign", "apple-sign.png",new Vector3(-13,9.5f,7.32f),2.7f,256);
        // Sa Sa: official locator confirms Manley House, 86-98 Canton Road, shops 4-5.
        // These two bays remain an explicit compressed layout, not surveyed shop geometry.
        GameObject.Find("日常百貨").SetActive(false);GameObject.Find("茶 · 食").SetActive(false);
        var entry=GameObject.Find("九龍商行");var wl=entry.GetComponent<WorldLabel>();wl.content="文利大廈";wl.fallback="MANLEY HOUSE";var tm=entry.GetComponent<TextMesh>();tm.text=wl.content;tm.characterSize=.10f;tm.color=new Color(.8f,.82f,.79f);entry.name="Manley House entry label";
        var silver=Material("Sa Sa pale metallic fascia",new Color(.50f,.65f,.68f));
        var charcoal=Material("Sa Sa dark framing",new Color(.08f,.11f,.12f));
        var ivory=Material("Sa Sa retail white",new Color(.82f,.83f,.77f));
        var pink=Material("Sa Sa magenta trim",new Color(.82f,.015f,.38f));
        Box("Sa Sa storefront fascia",new Vector3(2.9f,4.2f,5.65f),new Vector3(9.35f,1.05f,.2f),silver,true);
        Box("Sa Sa lower illuminated trim",new Vector3(2.9f,3.65f,5.58f),new Vector3(9.35f,.07f,.08f),pink,true);
        Sign("Sa Sa official wordmark", "sasa-official.png",new Vector3(4.8f,4.22f,5.51f),.9f,256);
        foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            var p=r.transform.position;
            if(r.name=="Canvas awning" && p.x<8 && p.x>-2)r.sharedMaterial=silver;
            if((r.name=="Awning valance" || r.name=="Recessed shop jamb") && p.x<8 && p.x>-2)r.sharedMaterial=charcoal;
            if(r.name=="Potted shrub" && p.x<5 && p.x>-2)r.gameObject.SetActive(false);
        }
        foreach(float x in new[]{.5f,5.3f})
        {
            Box("Sa Sa cosmetic counter",new Vector3(x,1.0f,7.4f),new Vector3(3.8f,.8f,.25f),ivory,true);
            for(int i=0;i<8;i++)Box("Cosmetic display carton",new Vector3(x-1.5f+i*.42f,1.55f,7.2f),new Vector3(.19f,.30f+(i%3)*.08f,.15f),i%3==0?pink:ivory,false);
        }
        Label("MAKE UP",new Vector3(.5f,2.8f,7.4f),.055f,Color.white);
        Label("SKIN CARE",new Vector3(5.3f,2.8f,7.4f),.055f,Color.white);
        // Material/geometry changes require rebaking; the existing settings/probes are retained.
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return "Apple logo + verified Sa Sa fascia; fictional shop labels removed; rebake required";
    }
    public static string AdjustVisibility()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Dir+"/CantonRoadPolish.unity")throw new Exception("Open Canton polish in Edit mode");
        // Camera inspection found the existing street tree hid the left half of the mark.
        // Shift only the non-GI sign within the verified store's compressed fascia.
        GameObject.Find("Sa Sa official wordmark").transform.position=new Vector3(4.8f,4.22f,5.51f);
        string path=Dir+"/sasa-official.png.sign.json";
        var record=Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(path));
        record["position"]=new Newtonsoft.Json.Linq.JArray(4.8f,4.22f,5.51f);
        File.WriteAllText(path,record.ToString());
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        return "Sa Sa mark moved clear of the tree; no baked geometry changed";
    }
    static void Sign(string name,string file,Vector3 position,float height,int pixels)
    {
        string path="Assets/HongKong/Generated/CantonPolish/BrandTextures/"+file;
        AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var imp=(TextureImporter)AssetImporter.GetAtPath(path);imp.isReadable=true;imp.alphaIsTransparency=true;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.filterMode=FilterMode.Point;imp.mipmapEnabled=true;imp.maxTextureSize=2048;imp.npotScale=TextureImporterNPOTScale.None;imp.SaveAndReimport();
        var t=AssetDatabase.LoadAssetAtPath<Texture2D>(path);int minx=t.width,miny=t.height,maxx=0,maxy=0;
        var colors=t.GetPixels32();for(int y=0;y<t.height;y++)for(int x=0;x<t.width;x++)if(colors[y*t.width+x].a>128){minx=Math.Min(x,minx);maxx=Math.Max(x,maxx);miny=Math.Min(y,miny);maxy=Math.Max(y,maxy);}
        // Official Sa Sa header artwork includes a grey slogan below the magenta mark.
        // Clip that separate slogan via UVs; keep the official wordmark pixels unchanged.
        if(file.StartsWith("sasa"))
        {minx=t.width;miny=t.height;maxx=0;maxy=0;for(int y=0;y<t.height;y++)for(int x=0;x<t.width;x++){var c=colors[y*t.width+x];if(c.a>128 && c.r>150 && c.g<80 && c.b>60){minx=Math.Min(x,minx);maxx=Math.Max(x,maxx);miny=Math.Min(y,miny);maxy=Math.Max(y,maxy);}}}
        var uv=new Vector4((maxx-minx+1f)/t.width,(maxy-miny+1f)/t.height,minx/(float)t.width,miny/(float)t.height);
        var m=Material(name,Color.white);m.mainTexture=t;m.mainTextureScale=new Vector2(uv.x,uv.y);m.mainTextureOffset=new Vector2(uv.z,uv.w);m.SetFloat("_AlphaClip",1);m.EnableKeyword("_ALPHATEST_ON");m.SetFloat("_Cutoff",.5f);m.SetFloat("_Cull",0);m.SetOverrideTag("RenderType","TransparentCutout");m.renderQueue=2450;
        m.EnableKeyword("_EMISSION");m.SetTexture("_EmissionMap",t);m.SetColor("_EmissionColor",Color.white*.65f);m.globalIlluminationFlags=MaterialGlobalIlluminationFlags.None;
        var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name=name;g.transform.SetParent(root);g.transform.position=position;g.transform.localScale=new Vector3(height*(maxx-minx+1f)/(maxy-miny+1f),height,1);UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());g.GetComponent<Renderer>().sharedMaterial=m;g.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
        imp.maxTextureSize=pixels;imp.isReadable=false;imp.SaveAndReimport();
        File.WriteAllText(Dir+"/"+file+".sign.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{file,uv=new[]{uv.x,uv.y,uv.z,uv.w},height,position=new[]{position.x,position.y,position.z}},Newtonsoft.Json.Formatting.Indented));
    }
    static Material Material(string name,Color c){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.name=name;m.color=c;m.SetFloat("_Smoothness",.18f);AssetDatabase.CreateAsset(m,Dir+"/Materials/"+name+".mat");return m;}
    static void Box(string name,Vector3 p,Vector3 scale,Material m,bool gi)
    {
        var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root);g.transform.position=p;g.transform.localScale=scale;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());g.GetComponent<Renderer>().sharedMaterial=m;
        if(gi){g.GetComponent<MeshFilter>().sharedMesh=AssetDatabase.LoadAssetAtPath<Mesh>(Dir+"/Meshes/UV2 0.asset");GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.ContributeGI);var r=g.GetComponent<MeshRenderer>();r.receiveGI=ReceiveGI.Lightmaps;r.scaleInLightmap=.55f;}
    }
    static void Label(string text,Vector3 p,float size,Color c){var g=new GameObject(text);g.transform.SetParent(root);g.transform.position=p;var t=g.AddComponent<TextMesh>();t.text=text;t.fontSize=64;t.characterSize=size;t.anchor=TextAnchor.MiddleCenter;t.color=c;var w=g.AddComponent<WorldLabel>();w.content=text;w.fallback=text;}
}
