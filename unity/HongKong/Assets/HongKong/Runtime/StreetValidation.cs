using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

namespace HongKong.Walk
{
    public sealed class StreetValidation : MonoBehaviour
    {
        public StreetWalker walker;
        public int reached;
        public bool completed, failed;
        string output;
        readonly List<float> frameMs = new();
        readonly List<string> errors = new();
        float begin;
        void Start()
        {
            var args=Environment.GetCommandLineArgs();
            int index=Array.IndexOf(args,"-streetOutput");
            if(Array.IndexOf(args,"-streetSmokeTest")>=0)
                Begin(index>=0 && index+1<args.Length ? args[index+1] : Path.Combine(Application.persistentDataPath,"validation"));
        }
        public void Begin(string folder)
        {
            if (output!=null) return;
            output=folder; Directory.CreateDirectory(folder);
            Application.logMessageReceived += Log;
            StartCoroutine(Traverse());
        }
        void Log(string message,string trace,LogType type)
        { if(type==LogType.Exception || type==LogType.Error || type==LogType.Assert) errors.Add(message); }
        void Update() { if(output!=null && !completed) frameMs.Add(Time.unscaledDeltaTime*1000); }
        IEnumerator Traverse()
        {
            begin=Time.realtimeSinceStartup;
            walker.automatic=true;
            walker.automaticTarget=walker.transform.position;
            yield return new WaitForSeconds(1);
            yield return Capture("start.png");
            var pixel=walker.viewCamera.GetComponent<HongKong.Rendering.CantonPixelView>();
            if(pixel)
            {
                int originalMode=pixel.mode;
                foreach(int mode in new[]{0,1,2,3})
                {pixel.mode=mode;yield return new WaitForSeconds(.3f);yield return Capture("pixel-mode-"+mode+".png");}
                pixel.mode=originalMode;
            }
            var atmosphere=FindFirstObjectByType<StreetAtmosphere>();
            if(atmosphere)
            {
                if(!atmosphere.volume.profile.TryGet<UnityEngine.Rendering.Universal.DepthOfField>(out var effect))
                    errors.Add("Depth of field profile missing after load");
                atmosphere.dofEnabled=true;atmosphere.Apply();yield return new WaitForSeconds(.5f);
                yield return Capture("day-dof.png");
                atmosphere.night=true;atmosphere.Apply();yield return new WaitForSeconds(.5f);
                yield return Capture("night-dof.png");
                atmosphere.night=false;atmosphere.dofEnabled=false;atmosphere.Apply();
            }
            for(int pass=0;pass<2;pass++)
            {
                for(int j=0;j<walker.route.Length;j++)
                {
                    int i=pass==0 ? j : walker.route.Length-1-j;
                    walker.automaticTarget=walker.route[i];
                    float deadline=Time.realtimeSinceStartup+40;
                    while(Vector3.ProjectOnPlane(walker.transform.position-walker.route[i],Vector3.up).magnitude>.17f)
                    {
                        if(Time.realtimeSinceStartup>deadline){failed=true;errors.Add("Route stalled at "+i+" pass "+pass);break;}
                        yield return null;
                    }
                    if(failed)break;
                    reached++;
                    if(pass==0 && (i==0 || i==walker.route.Length/2 || i==walker.route.Length-1))
                        yield return Capture("route-"+i+".png");
                }
                if(failed)break;
            }
            walker.automatic=false;
            var bus=FindFirstObjectByType<CantonBus>();
            if(bus){float until=Time.realtimeSinceStartup+50;while(bus.cycles<3 && Time.realtimeSinceStartup<until)yield return null;
                if(bus.cycles<3 || bus.violations>0)errors.Add("Bus did not complete three safe boarding cycles");}
            yield return Capture("returned.png");
            frameMs.Sort(); completed=true;
            var result=new Result { stage=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                busCycles=bus?bus.cycles:0, boardingViolations=bus?bus.violations:0,
                passed=!failed && errors.Count==0 && walker.fallRecoveries==0, fallRecoveries=walker.fallRecoveries,
                unityVersion=Application.unityVersion,graphics=SystemInfo.graphicsDeviceName,
                elapsedSeconds=Time.realtimeSinceStartup-begin, frames=frameMs.Count, waypointsReached=reached,
                waypointsExpected=walker.route.Length*2, distanceM=walker.distanceWalked,
                p95FrameMs=frameMs[(int)((frameMs.Count-1)*.95f)],p99FrameMs=frameMs[(int)((frameMs.Count-1)*.99f)],
                width=Screen.width,height=Screen.height,memoryBytes=Profiler.GetTotalAllocatedMemoryLong(),
                finalPosition=walker.transform.position,errors=errors.ToArray(),
                limitations="Automated world-space intents use the same CharacterController as keyboard input. Short traversal, not v0.1 10-minute performance or art acceptance."};
            File.WriteAllText(Path.Combine(output,"result.json"),JsonUtility.ToJson(result,true));
            Debug.Log("[StreetValidation] "+(result.passed?"PASS":"FAIL")+" reached="+reached+" distance="+walker.distanceWalked);
            Application.logMessageReceived-=Log;
            if(!Application.isEditor)Application.Quit(result.passed?0:1);
        }
        IEnumerator Capture(string file)
        {
            yield return new WaitForEndOfFrame();
            var tex=ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(output,file),tex.EncodeToPNG()); Destroy(tex);
        }
        void OnDestroy(){Application.logMessageReceived-=Log;}
        [Serializable] sealed class Result
        {
            public string stage,unityVersion,graphics,limitations;
            public bool passed;
            public int busCycles,boardingViolations;
            public int frames,waypointsReached,waypointsExpected,width,height,fallRecoveries;
            public float elapsedSeconds,distanceM,p95FrameMs,p99FrameMs;
            public long memoryBytes;
            public Vector3 finalPosition;
            public string[] errors;
        }
    }
}
