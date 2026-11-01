using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.Cinemachine;
using HongKong.Walk;
public static class RefineCantonDemo
{
    public static string Apply()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!="Assets/HongKong/Generated/CantonDemo/CantonRoadDemo.unity")throw new System.Exception("Canton scene in Edit mode required");
        var cine=Object.FindFirstObjectByType<CinemachineCamera>();cine.Lens.FieldOfView=24;cine.GetComponent<CinemachinePositionComposer>().TargetOffset=new Vector3(0,3.2f,0);
        var walker=Object.FindFirstObjectByType<StreetWalker>();walker.viewCamera.GetComponent<HongKong.Rendering.CantonPixelView>().mode=1;
        foreach(var label in Object.FindObjectsByType<WorldLabel>(FindObjectsSortMode.None))if(label.content.StartsWith("海防道"))label.transform.position=new Vector3(20,4.5f,8);walker.transform.position=new Vector3(-5,.22f,2);walker.viewCamera.fieldOfView=24;
        foreach(var text in Object.FindObjectsByType<TextMesh>(FindObjectsSortMode.None))text.characterSize=text.text=="Apple"?.252f:text.text.StartsWith("海港城")?.078f:text.text.StartsWith("海防道")||text.text.StartsWith("廣東道")?.06f:text.text=="DEMO"?.048f:text.text.StartsWith("BUS")?.042f:.084f;
        foreach(var name in new[]{"East sidewalk","Haiphong connecting street"}){var floor=GameObject.Find(name);if(floor && !floor.GetComponent<BoxCollider>())floor.AddComponent<BoxCollider>();}
        var bus=Object.FindFirstObjectByType<CantonBus>();if(!bus.GetComponent<BoxCollider>()){var collider=bus.gameObject.AddComponent<BoxCollider>();collider.center=new Vector3(0,2,0);collider.size=new Vector3(9,4,2.4f);}
        foreach(var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {var m=r.sharedMaterial;if(m && m.name.StartsWith("CANTON") && m.HasProperty("_BaseMap")){var source=AssetDatabase.LoadAssetAtPath<Material>("Assets/HongKong/Generated/Art/Materials/Plaster.mat");m.SetTexture("_BaseMap",source.GetTexture("_BaseMap"));EditorUtility.SetDirty(m);}}
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();return "Wider 24 degree scene framing, legible text, bus collision; existing sample retained";
    }
}
