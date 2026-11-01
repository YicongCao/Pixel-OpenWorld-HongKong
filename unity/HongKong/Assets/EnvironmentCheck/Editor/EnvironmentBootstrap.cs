using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HongKong.EnvironmentCheck
{
    public static class EnvironmentBootstrap
    {
        public const string ScenePath = "Assets/EnvironmentCheck/EnvironmentCheck.unity";

        [MenuItem("Hong Kong/Environment Check/Create Scene")]
        public static string CreateScene()
        {
            // Never rebuild over an existing scene: keep manual edits intact.
            if (File.Exists(ScenePath))
                throw new InvalidOperationException("Environment check scene already exists; open it instead.");
            if (GraphicsSettings.defaultRenderPipeline is not UniversalRenderPipelineAsset)
                throw new InvalidOperationException("URP must be configured before creating the probe.");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("Environment Camera", typeof(Camera), typeof(AudioListener));
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(7, 6, -10);
            camera.transform.LookAt(new Vector3(0, 0.8f, 0));
            var cam = camera.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.11f, 0.16f, 0.20f);
            cam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
            var light = new GameObject("Environment Light", typeof(Light));
            light.transform.rotation = Quaternion.Euler(50, -30, 0);
            var sun = light.GetComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 2f;
            sun.shadows = LightShadows.Soft;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.45f, 0.45f);
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetColor("_BaseColor", new Color(0.1f, 0.65f, 0.68f));
            AssetDatabase.CreateAsset(material, "Assets/EnvironmentCheck/Probe.mat");
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Probe Ground";
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Environment Probe";
            cube.transform.position = Vector3.up;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            cube.AddComponent<EnvironmentProbe>();
            PlayerSettings.productName = "Hong Kong Environment Check";
            PlayerSettings.companyName = "PixelOpenWorldHongKong";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.runInBackground = true;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetArchitecture(NamedBuildTarget.Standalone, 1); // macOS Apple Silicon
            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            return ScenePath;
        }

        [MenuItem("Hong Kong/Environment Check/Build Mac Player")]
        public static string BuildMac()
        {
            string buildPath = Path.GetFullPath("../../.local/builds/HongKongEnvironment.app");
            Directory.CreateDirectory(Path.GetDirectoryName(buildPath));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = buildPath,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.Development
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Mac build failed: " + report.summary.result);
            Debug.Log("[EnvironmentCheck] Mac build succeeded: " + report.summary.totalSize + " bytes");
            return buildPath;
        }
    }
}
