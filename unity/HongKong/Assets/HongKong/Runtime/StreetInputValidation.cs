using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace HongKong.Walk
{
    // Opt-in test only: a virtual keyboard exercises the real InputAction bindings without OS input.
    public sealed class StreetInputValidation : MonoBehaviour
    {
        public bool checkNathanRoadBoundary=true;
        InputSettings originalSettings, testSettings;
        Keyboard keyboard;
        StreetWalker walker;
        readonly List<string> checks=new();
        readonly List<string> failures=new();
        public void Begin(string folder){StartCoroutine(Run(folder));}
        IEnumerator KeyFor(Key key,float seconds)
        {
            InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));
            yield return new WaitForSecondsRealtime(seconds);
            InputSystem.QueueStateEvent(keyboard,new KeyboardState());
            yield return null;
        }
        void Check(string name,bool ok){checks.Add(name+": "+(ok?"PASS":"FAIL"));if(!ok)failures.Add(name);}
        IEnumerator Run(string folder)
        {
            Directory.CreateDirectory(folder);
            originalSettings=InputSystem.settings;testSettings=Instantiate(originalSettings);
            testSettings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            testSettings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings=testSettings;
            walker=FindFirstObjectByType<StreetWalker>();
            var atmosphere=FindFirstObjectByType<StreetAtmosphere>();
            keyboard=InputSystem.AddDevice<Keyboard>("Street validation keyboard");
            var body=walker.GetComponent<CharacterController>();
            body.enabled=false;walker.transform.position=walker.route[4]+Vector3.up*.1f;body.enabled=true;
            walker.automatic=false;walker.paused=false;
            yield return new WaitForSeconds(.5f);
            var initial=walker.transform.position;
            yield return KeyFor(Key.D,.4f);
            var delta=walker.transform.position-initial;
            Check("D drives real input toward screen right",Vector3.Dot(delta,walker.viewCamera.transform.right)>.4f);
            var actor=walker.GetComponent<PixelActor>();
            walker.GetInputAxes(out var right,out var inward);
            Check("D does not drift across the declared street axis",Mathf.Abs(Vector3.Dot(delta,inward))<.08f);
            Check("D selects right-facing sprite",actor.CurrentDirection==1);
            foreach(var pair in new[]{(Key.W,2),(Key.A,3),(Key.S,0)})
            {
                yield return KeyFor(pair.Item1,.15f);
                Check(pair.Item1+" selects matching sprite direction",actor.CurrentDirection==pair.Item2);
            }
            yield return null;yield return null;
            Check("Stopping returns to idle frame",actor.CurrentFrame%7==0);
            yield return KeyFor(Key.Escape,.08f);
            Check("Escape pauses",walker.paused);
            initial=walker.transform.position;
            yield return KeyFor(Key.W,.3f);
            Check("Movement blocked while paused",Vector3.Distance(initial,walker.transform.position)<.01f);
            yield return KeyFor(Key.Escape,.08f);Check("Escape resumes",!walker.paused);
            yield return KeyFor(Key.N,.08f);Check("N enables night",atmosphere.night);
            yield return KeyFor(Key.F,.08f);Check("F enables depth of field",atmosphere.dofEnabled);
            Check("DOF profile is present and active",atmosphere.volume.profile.TryGet<UnityEngine.Rendering.Universal.DepthOfField>(out var dof) && dof.active);
            yield return KeyFor(Key.N,.08f);yield return KeyFor(Key.F,.08f);
            Check("N and F restore day / no DOF",!atmosphere.night && !atmosphere.dofEnabled);
            if(checkNathanRoadBoundary)
            {
            initial=walker.transform.position;
            walker.automatic=true;walker.automaticTarget=initial+Vector3.right*20;
            yield return new WaitForSeconds(1.2f);
            Check("Side boundary blocks road access",walker.transform.position.x-initial.x<2.4f);
            walker.automatic=false;
            }
            var pixel=walker.viewCamera.GetComponent<HongKong.Rendering.CantonPixelView>();
            if(pixel){int before=pixel.mode;yield return KeyFor(Key.P,.08f);Check("P cycles pixel pipeline",pixel.mode==(before+1)%4);pixel.mode=before;}
            var atlas=JsonUtility.FromJson<PixelActor.Atlas>(actor.metadata.text);
            bool valid=atlas.frames.Length==28;
            foreach(var f in atlas.frames)valid &= f.rect.width>0 && f.rect.height>0 && f.rect.x>=0 && f.rect.y>=0 && f.rect.x+f.rect.width<=atlas.width && f.rect.y+f.rect.height<=atlas.height;
            Check("28 valid runtime frame rectangles",valid);
            Check("No fall recovery",walker.fallRecoveries==0);
            walker.ResetToStart();InputSystem.RemoveDevice(keyboard);keyboard=null;RestoreSettings();
            var result=new Result{passed=failures.Count==0,checks=checks.ToArray(),failures=failures.ToArray(),method="Virtual keyboard through Input System; actual controller collision. Editor test, not physical keyboard or full UI acceptance."};
            File.WriteAllText(Path.Combine(folder,"input-result.json"),JsonUtility.ToJson(result,true));
            Debug.Log("[StreetInputValidation] "+(result.passed?"PASS":"FAIL"));
        }
        void RestoreSettings(){if(originalSettings){InputSystem.settings=originalSettings;originalSettings=null;}if(testSettings){Destroy(testSettings);testSettings=null;}}
        void OnDestroy(){RestoreSettings();if(keyboard!=null)InputSystem.RemoveDevice(keyboard);}
        [Serializable] sealed class Result{public bool passed;public string method;public string[] checks,failures;}
    }
}
