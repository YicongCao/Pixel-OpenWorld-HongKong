using UnityEngine;
using UnityHFSM;
namespace HongKong.Walk
{
    // Designed demo stop, not a claim about an actual Canton Road service or timetable.
    public sealed class CantonBus : MonoBehaviour
    {
        public static CantonBus Instance {get;private set;}
        public Transform door, passenger;
        public int cycles, boarded, violations;
        public string StateName=>fsm==null?"Starting":fsm.ActiveStateName;
        StateMachine fsm; StreetWalker player; float entered; Vector3 doorClosed;
        void Start()
        {
            Instance=this;player=FindFirstObjectByType<StreetWalker>();doorClosed=door.localPosition;fsm=new StateMachine();
            fsm.AddState("Approach",onEnter:s=>{entered=0;transform.position=new Vector3(-39,.2f,-4);passenger.gameObject.SetActive(true);passenger.position=new Vector3(3,.22f,.1f);},onLogic:s=>Move(0));
            fsm.AddState("Open",onEnter:s=>{entered=0;door.localPosition=doorClosed+Vector3.right*.8f;});
            fsm.AddState("Board",onEnter:s=>entered=0,onLogic:s=>{if(door.localPosition==doorClosed)violations++;passenger.position=Vector3.MoveTowards(passenger.position,new Vector3(2,.22f,-3),Time.deltaTime*2);});
            fsm.AddState("Close",onEnter:s=>{entered=0;boarded++;passenger.gameObject.SetActive(false);door.localPosition=doorClosed;});
            fsm.AddState("Depart",onEnter:s=>entered=0,onLogic:s=>Move(39));
            fsm.AddTransition("Approach","Open",t=>Mathf.Abs(transform.position.x)<.01f);
            fsm.AddTransition("Open","Board",t=>entered>1);
            fsm.AddTransition("Board","Close",t=>entered>2);
            fsm.AddTransition("Close","Depart",t=>entered>1);
            fsm.AddTransition("Depart","Approach",t=>{if(transform.position.x<38.99f)return false;cycles++;return true;});
            fsm.SetStartState("Approach");fsm.Init();
        }
        void Move(float x){transform.position=Vector3.MoveTowards(transform.position,new Vector3(x,.2f,-4),Time.deltaTime*8);}
        void Update(){if(player && player.paused)return;entered+=Time.deltaTime;fsm?.OnLogic();}
        void OnDestroy(){if(Instance==this)Instance=null;}
    }
}
