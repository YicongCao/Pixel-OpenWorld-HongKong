using UnityEngine;

namespace HongKong.Walk
{
    // Four side-walk frames. Only use for routes whose visible movement is lateral.
    public sealed class CantonCitizenSprite : MonoBehaviour
    {
        public MeshRenderer plane;
        public Vector4[] frames;
        public float worldHeight=1.65f, atlasAspect=1;
        MaterialPropertyBlock block;
        Vector3 previous;
        float distance;
        bool left;
        void OnEnable(){previous=transform.position;}
        void LateUpdate()
        {
            if(!plane || frames==null || frames.Length!=4)return;
            block??=new MaterialPropertyBlock();
            var delta=transform.position-previous;previous=transform.position;
            if(Mathf.Abs(delta.x)>.001f)left=delta.x<0;
            float travel=Vector3.ProjectOnPlane(delta,Vector3.up).magnitude;
            // Ignore loop teleports, which must not advance gait.
            if(travel<.5f)distance+=travel;
            int frame=travel>.001f?(int)(distance*4/1.25f)%4:1;
            var r=frames[frame];
            float h=worldHeight*r.y/frames[1].y,w=h*r.x/r.y*atlasAspect;
            plane.transform.localPosition=Vector3.up*(h*.5f+.025f);
            plane.transform.localScale=new Vector3(w,h,1);
            plane.GetPropertyBlock(block);
            block.SetVector("_BaseMap_ST",new Vector4(left?-r.x:r.x,r.y,left?r.z+r.x:r.z,r.w));
            plane.SetPropertyBlock(block);
        }
    }
}
