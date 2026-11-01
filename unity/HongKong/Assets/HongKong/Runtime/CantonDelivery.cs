using UnityEngine;

namespace HongKong.Walk
{
    // Small, bounded frontage delivery loop. This is authored staging, not a surveyed activity.
    public sealed class CantonDelivery : MonoBehaviour
    {
        public Vector3 pickup=new(4,.22f,6), dropoff=new(11,.22f,6);
        public GameObject parcel;
        public int deliveries;
        public string stage="Pick up";
        StreetWalker player;
        float wait=2;
        int state;
        void Start(){player=FindFirstObjectByType<StreetWalker>();transform.position=pickup;parcel.SetActive(false);}
        void Update()
        {
            if(player && player.paused)return;
            if(state==0 || state==2)
            {
                wait-=Time.deltaTime;if(wait>0)return;
                if(state==0){parcel.SetActive(true);state=1;stage="Carry to shop";}
                else{parcel.SetActive(false);deliveries++;state=3;stage="Return empty";}
            }
            else
            {
                var target=state==1?dropoff:pickup;
                transform.position=Vector3.MoveTowards(transform.position,target,Time.deltaTime*.75f);
                parcel.transform.localPosition=new Vector3(state==1?.25f:-.25f,.9f,-.08f);
                if(Vector3.Distance(transform.position,target)<.01f){state=state==1?2:0;wait=2;stage=state==2?"Hand over":"Pick up";}
            }
        }
    }
}
