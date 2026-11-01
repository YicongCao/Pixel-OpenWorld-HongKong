using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HongKong.Walk
{
    public sealed class StreetAtmosphere : MonoBehaviour
    {
        public Light sun;
        public Light[] streetLights;
        public Volume volume;
        public StreetWalker walker;
        public bool night,dofEnabled;
        DepthOfField dof;
        void Start(){volume.profile.TryGet(out dof);Apply();}
        void Update()
        {
            if(Keyboard.current!=null)
            {
                if(Keyboard.current.nKey.wasPressedThisFrame){night=!night;Apply();}
                if(Keyboard.current.fKey.wasPressedThisFrame){dofEnabled=!dofEnabled;Apply();}
            }
            if(dof!=null)
            {
                var cam=walker.viewCamera.transform;
                float depth=Vector3.Dot(walker.transform.position+Vector3.up-cam.position,cam.forward);
                dof.focusDistance.value=Mathf.Lerp(dof.focusDistance.value,Mathf.Max(1,depth),1-Mathf.Exp(-Time.deltaTime*3));
            }
        }
        public void Apply()
        {
            sun.intensity=night?.45f:2.2f;
            sun.color=night?new Color(.48f,.61f,.9f):new Color(1,.88f,.70f);
            RenderSettings.ambientLight=night?new Color(.25f,.32f,.42f):new Color(.55f,.61f,.65f);
            RenderSettings.fogColor=night?new Color(.045f,.08f,.15f):new Color(.53f,.65f,.67f);
            walker.viewCamera.backgroundColor=RenderSettings.fogColor;
            foreach(var lamp in streetLights)if(lamp)lamp.enabled=night;
            if(dof!=null)dof.active=dofEnabled;
            if(volume.profile.TryGet<ColorAdjustments>(out var color))color.postExposure.value=night?.8f:.25f;
        }
    }
}
