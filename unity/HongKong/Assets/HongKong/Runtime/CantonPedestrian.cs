using UnityEngine;
namespace HongKong.Walk
{
    public sealed class CantonPedestrian : MonoBehaviour
    {
        public Vector3 a,b; public float speed=1,offset;public Transform leftLeg,rightLeg;
        StreetWalker player;float clock;
        void Start(){player=FindFirstObjectByType<StreetWalker>();}
        void Update()
        {
            if(player && player.paused)return;clock+=Time.deltaTime;
            float length=Vector3.Distance(a,b);float t=Mathf.PingPong(clock*speed+offset,length)/length;
            transform.position=Vector3.Lerp(a,b,t);
            float stride=Mathf.Sin((clock*speed+offset)*8)*20;
            if(leftLeg)leftLeg.localRotation=Quaternion.Euler(stride,0,0);
            if(rightLeg)rightLeg.localRotation=Quaternion.Euler(-stride,0,0);
        }
    }
}
