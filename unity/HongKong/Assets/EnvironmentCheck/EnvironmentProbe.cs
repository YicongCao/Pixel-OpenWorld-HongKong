using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace HongKong.EnvironmentCheck
{
    // Diagnostic only: this scene is not the game's movement controller or art baseline.
    public sealed class EnvironmentProbe : MonoBehaviour
    {
        public float degreesPerSecond = 35f;
        public int observedFrames;
        private Vector3 initialPosition;

        private void Awake()
        {
            initialPosition = transform.position;
            Debug.Log("[EnvironmentProbe] Started " + Application.unityVersion);
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-envSmokeTest") >= 0)
                StartCoroutine(RecordPlayerResult());
        }

        private void Update()
        {
            observedFrames++;
            transform.Rotate(Vector3.up, degreesPerSecond * Time.deltaTime);
            transform.position = initialPosition + Vector3.right * Mathf.Sin(Time.time) * 0.5f;
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                Application.Quit();
        }

        private IEnumerator RecordPlayerResult()
        {
            while (observedFrames < 120) yield return null;
            yield return new WaitForEndOfFrame();
            string[] args = Environment.GetCommandLineArgs();
            int outputIndex = Array.IndexOf(args, "-envOutput");
            string output = outputIndex >= 0 && outputIndex + 1 < args.Length
                ? args[outputIndex + 1] : Application.persistentDataPath;
            Directory.CreateDirectory(output);
            Texture2D capture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(output, "player.png"), capture.EncodeToPNG());
            Destroy(capture);
            var result = new Result
            {
                unityVersion = Application.unityVersion,
                frames = observedFrames,
                graphicsDevice = SystemInfo.graphicsDeviceName,
                graphicsApi = SystemInfo.graphicsDeviceType.ToString(),
                renderPipeline = GraphicsSettings.currentRenderPipeline != null
                    ? GraphicsSettings.currentRenderPipeline.GetType().Name : "Built-in",
                keyboardDetected = Keyboard.current != null,
                width = Screen.width,
                height = Screen.height
            };
            File.WriteAllText(Path.Combine(output, "player.json"), JsonUtility.ToJson(result, true));
            Debug.Log("[EnvironmentProbe] Completed: " + observedFrames + " frames");
            yield return null;
            Application.Quit(0);
        }

        [Serializable]
        private sealed class Result
        {
            public string unityVersion, graphicsDevice, graphicsApi, renderPipeline;
            public int frames, width, height;
            public bool keyboardDetected;
        }
    }
}
