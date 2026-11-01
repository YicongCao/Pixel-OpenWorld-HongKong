using System;
using UnityEngine;

namespace HongKong.Walk
{
    public sealed class PixelActor : MonoBehaviour
    {
        [Serializable] public class PixelRect { public float x,y,width,height; }
        [Serializable] public class Frame { public PixelRect rect; public string direction; public int column,durationMs; }
        [Serializable] public class Atlas { public int width,height,columns,rows; public float worldHeight; public Frame[] frames; }
        public TextAsset metadata;
        public StreetWalker walker;
        public Transform visual;
        public MeshRenderer plane;
        public Camera viewCamera;
        Atlas atlas;
        MaterialPropertyBlock props;
        float phase;
        int direction=0,lastFrame=-1;
        public int CurrentDirection=>direction;
        public int CurrentFrame=>lastFrame;
        void Awake()
        {
            if(!walker)walker=GetComponent<StreetWalker>();
            if(!viewCamera)viewCamera=Camera.main;
            atlas=JsonUtility.FromJson<Atlas>(metadata.text);props=new MaterialPropertyBlock();
        }
        void LateUpdate()
        {
            if(atlas==null || atlas.frames==null)return;
            Vector3 front=Vector3.ProjectOnPlane(-viewCamera.transform.forward,Vector3.up).normalized;
            walker.GetInputAxes(out var right,out var inward);
            float x=Vector3.Dot(walker.Facing,right),z=Vector3.Dot(walker.Facing,-inward);
            int wanted=Mathf.Abs(x)>Mathf.Abs(z)?(x>0?1:3):(z>0?0:2);
            // Avoid alternating adjacent rows at a diagonal boundary.
            if(Mathf.Abs(Mathf.Abs(x)-Mathf.Abs(z))>.08f)direction=wanted;
            float speed=Vector3.ProjectOnPlane(walker.Velocity,Vector3.up).magnitude;
            if(!walker.paused && speed>.08f)phase+=Time.deltaTime*speed/1.45f;
            int column=speed>.08f && !walker.paused?1+(int)(phase*6)%6:0;
            int index=direction*7+column;
            // Unity's Quad faces local -Z; +Z must point away from the viewer to preserve left/right.
            visual.rotation=Quaternion.LookRotation(-front,Vector3.up);
            if(index==lastFrame)return;
            lastFrame=index;
            var r=atlas.frames[index].rect;
            float pixelsPerMeter=atlas.frames[direction*7].rect.height/atlas.worldHeight;
            float h=r.height/pixelsPerMeter,w=r.width/pixelsPerMeter;
            visual.localScale=new Vector3(w,h,1);
            visual.position=transform.position+Vector3.up*(h*.5f+.025f);
            plane.GetPropertyBlock(props);
            props.SetVector("_BaseMap_ST",new Vector4(r.width/atlas.width,r.height/atlas.height,r.x/atlas.width,r.y/atlas.height));
            plane.SetPropertyBlock(props);
        }
    }
}
