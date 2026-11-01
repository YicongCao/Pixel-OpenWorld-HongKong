using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;
using HongKong.Walk;

public static class TuneHD2DComposition
{
    public const string Scene="Assets/HongKong/Generated/Scenes/NathanHD2DStudy.unity";
    const string Art="Assets/HongKong/Generated/Art";
    public static string Create()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().isDirty)throw new Exception("Save scene and leave owned Play mode first");
        if(File.Exists(Scene))throw new Exception("Study exists; use explicit refinement, do not overwrite");
        if(!AssetDatabase.CopyAsset("Assets/HongKong/Generated/Scenes/NathanArtSample.unity",Scene))throw new Exception("Scene copy failed");
        if(!AssetDatabase.CopyAsset(Art+"/Atmosphere.asset",Art+"/HD2DStudyAtmosphere.asset"))throw new Exception("Profile copy failed");
        EditorSceneManager.OpenScene(Scene);
        var volume=UnityEngine.Object.FindFirstObjectByType<Volume>();volume.sharedProfile=AssetDatabase.LoadAssetAtPath<VolumeProfile>(Art+"/HD2DStudyAtmosphere.asset");
        var walker=UnityEngine.Object.FindFirstObjectByType<StreetWalker>();walker.cameraRelativeInput=false;walker.inputReferenceYaw=-95;
        UnityEngine.Object.FindFirstObjectByType<StreetHud>().stage="V1-03 / fixed-view study / unfinished art";
        Apply(2);EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        return Scene;
    }
    public static string Oblique(){Apply(0);return "A: oblique baseline";}
    public static string Axial(){Apply(1);return "B: axial, same lens";}
    public static string WeakPerspective(){Apply(2);return "C: axial weak perspective";}
    static void Apply(int variant)
    {
        var walker=UnityEngine.Object.FindFirstObjectByType<StreetWalker>();
        var cine=UnityEngine.Object.FindFirstObjectByType<CinemachineCamera>();
        float distance=variant==2?55:34;
        cine.transform.rotation=Quaternion.Euler(variant==0?28:25,variant==0?-110:-95,0);
        cine.Lens.FieldOfView=variant==2?20:32;
        var composer=cine.GetComponent<CinemachinePositionComposer>();composer.CameraDistance=distance;composer.TargetOffset=new Vector3(0,2.7f,0);
        walker.viewCamera.transform.SetPositionAndRotation(walker.transform.position+composer.TargetOffset-cine.transform.forward*distance,cine.transform.rotation);
        var volume=UnityEngine.Object.FindFirstObjectByType<Volume>();
        var profile=Application.isPlaying?volume.profile:volume.sharedProfile;
        if(profile.TryGet<DepthOfField>(out var dof)){dof.focusDistance.value=distance;if(!Application.isPlaying)EditorUtility.SetDirty(dof);}
        if(!Application.isPlaying)
        {
            EditorUtility.SetDirty(cine);EditorUtility.SetDirty(composer);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        }
    }
    public static string Adopt()
    {
        if(Application.isPlaying || EditorSceneManager.GetActiveScene().path!=Scene)throw new Exception("Study scene in edit mode required");
        Apply(2);
        var pipeline=GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        pipeline.shadowDistance=90;EditorUtility.SetDirty(pipeline);
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};AssetDatabase.SaveAssets();return "Study adopted; 90 m shadow range preserves lighting with distant camera; baseline scene retained";
    }
}
