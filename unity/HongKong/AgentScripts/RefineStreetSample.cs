using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using HongKong.Walk;
using Unity.Cinemachine;

public static class RefineStreetSample
{
    const string A="Assets/HongKong/Generated/Art";
    public static string Apply()
    {
        if(Application.isPlaying)throw new Exception("Edit mode required");
        if(EditorSceneManager.GetActiveScene().path!=BuildScene)throw new Exception("Open the art sample first");
        if(GameObject.Find("09 | Surface refinement"))throw new Exception("Refinement already applied");
        var root=new GameObject("09 | Surface refinement");
        var path=GameObject.Find("Continuous measured-grade path").GetComponent<MeshFilter>().sharedMesh;
        var vertices=path.vertices;
        var vs=new List<Vector3>();var ts=new List<int>();var uv=new List<Vector2>();
        // Route mesh stores one left/right vertex pair per waypoint.
        for(int i=0;i<vertices.Length-2;i+=2)
        {
            AddEdge(vertices[i],vertices[i+2],vs,ts,uv);
            AddEdge(vertices[i+1],vertices[i+3],vs,ts,uv);
        }
        var mesh=new Mesh{name="Sidewalk vertical edge"};mesh.SetVertices(vs);mesh.SetTriangles(ts,0);mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh,A+"/Meshes/SidewalkEdge.asset");
        var edge=new GameObject("Path skirt / grade visualization");edge.transform.parent=root.transform;
        edge.AddComponent<MeshFilter>().sharedMesh=mesh;edge.AddComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(A+"/Materials/Stone.mat");
        var apron=GameObject.CreatePrimitive(PrimitiveType.Cube);apron.name="Station apron / visual infill only";apron.transform.parent=root.transform;
        apron.transform.position=new Vector3(-14.2f,-.08f,8);apron.transform.localScale=new Vector3(15.5f,.28f,20);
        UnityEngine.Object.DestroyImmediate(apron.GetComponent<Collider>());
        var paving=new Material(AssetDatabase.LoadAssetAtPath<Material>(A+"/Materials/Paving.mat"));paving.mainTextureScale=new Vector2(7.75f,10);
        AssetDatabase.CreateAsset(paving,A+"/Materials/Apron paving.mat");apron.GetComponent<Renderer>().sharedMaterial=paving;
        // Lower imported sampling resolution is a prototype pixel-density experiment; source PNGs stay unchanged.
        foreach(var name in new[]{"Paving","Plaster","Terracotta","Asphalt","Glass","Stone","Metal","Foliage"})
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(A+"/Textures/"+name+".png");
            importer.maxTextureSize=128;importer.filterMode=FilterMode.Point;importer.SaveAndReimport();
        }
        Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing=true;
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(BuildScene,true)};
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return "Surface edges, pixel sampling, post-processing and default build scene updated";
    }
    public const string BuildScene="Assets/HongKong/Generated/Scenes/NathanArtSample.unity";
    public static string CloseCamera()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=BuildScene)throw new Exception("Art scene in edit mode required");
        var walker=UnityEngine.Object.FindFirstObjectByType<StreetWalker>();
        var cine=UnityEngine.Object.FindFirstObjectByType<CinemachineCamera>();
        var composer=cine.GetComponent<CinemachinePositionComposer>();composer.CameraDistance=34;composer.TargetOffset=new Vector3(0,2.6f,0);
        walker.viewCamera.transform.SetPositionAndRotation(walker.transform.position+composer.TargetOffset-cine.transform.forward*34,cine.transform.rotation);
        var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(A+"/Atmosphere.asset");
        if(profile.TryGet<DepthOfField>(out var dof)){dof.focusDistance.value=34;EditorUtility.SetDirty(dof);}
        EditorUtility.SetDirty(composer);EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return "User requested closer framing: 34 m distance, 2.6 m target offset; orientation and FOV preserved";
    }
    public static string RepairProfileAndEdges()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=BuildScene)throw new Exception("Art scene in edit mode required");
        var profile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(A+"/Atmosphere.asset");
        if(profile.components.Count==0)
        {
            var tone=profile.Add<Tonemapping>(true);tone.mode.Override(TonemappingMode.ACES);
            var bloom=profile.Add<Bloom>(true);bloom.intensity.Override(.18f);bloom.threshold.Override(1.1f);
            var color=profile.Add<ColorAdjustments>(true);color.postExposure.Override(.25f);color.contrast.Override(7);color.saturation.Override(-5);
            var dof=profile.Add<DepthOfField>(true);dof.mode.Override(DepthOfFieldMode.Bokeh);dof.focusDistance.Override(43);dof.focalLength.Override(170);dof.aperture.Override(2);dof.active=false;
            foreach(var component in profile.components)AssetDatabase.AddObjectToAsset(component,profile);
            EditorUtility.SetDirty(profile);
        }
        var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(A+"/Meshes/SidewalkEdge.asset");
        var triangles=new List<int>();for(int i=0;i<mesh.vertexCount;i+=4)foreach(int n in new[]{0,1,2,2,1,3})triangles.Add(i+n);
        mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();EditorUtility.SetDirty(mesh);
        var edgeMaterial=AssetDatabase.LoadAssetAtPath<Material>(A+"/Materials/Sidewalk edge.mat");
        if(!edgeMaterial){edgeMaterial=new Material(AssetDatabase.LoadAssetAtPath<Material>(A+"/Materials/Stone.mat"));AssetDatabase.CreateAsset(edgeMaterial,A+"/Materials/Sidewalk edge.mat");}
        edgeMaterial.SetFloat("_Cull",0);GameObject.Find("Path skirt / grade visualization").GetComponent<MeshRenderer>().sharedMaterial=edgeMaterial;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return "Volume components persisted as sub-assets; curb normals repaired";
    }
    static void AddEdge(Vector3 a,Vector3 b,List<Vector3> v,List<int> t,List<Vector2> uv)
    {
        int n=v.Count;v.Add(a);v.Add(b);v.Add(new Vector3(a.x,-.45f,a.z));v.Add(new Vector3(b.x,-.45f,b.z));
        foreach(int i in new[]{0,1,2,2,1,3,2,1,0,3,1,2})t.Add(n+i);
        uv.Add(Vector2.zero);uv.Add(Vector2.right*Vector3.Distance(a,b));uv.Add(Vector2.up);uv.Add(new Vector2(Vector3.Distance(a,b),1));
    }
}
