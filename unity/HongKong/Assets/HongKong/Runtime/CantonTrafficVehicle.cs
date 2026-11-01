using UnityEngine;
namespace HongKong.Walk
{
    // Rectified street design: south is +X; only urban taxis are spawned here.
    public sealed class CantonTrafficVehicle : MonoBehaviour
    {
        public string service="urban";
        public float speed=6, laneZ=-6.8f, startX=-44, endX=44;
        public int passes;
        public Transform[] wheels;
        public float wheelRadius=.34f;
        StreetWalker player;
        void Start(){player=FindFirstObjectByType<StreetWalker>();Debug.Log("[CantonTraffic] service="+service+" lane="+laneZ);}
        void Update()
        {
            if(player && player.paused)return;
            float dx=speed*Time.deltaTime;
            transform.position+=Vector3.right*dx;
            foreach(var wheel in wheels)if(wheel)wheel.Rotate(Vector3.up,-dx/wheelRadius*Mathf.Rad2Deg,Space.Self);
            if(transform.position.x>endX){transform.position=new Vector3(startX,transform.position.y,laneZ);passes++;}
        }
    }
}
