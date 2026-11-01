using UnityEngine;
using UnityEngine.InputSystem;
namespace HongKong.Rendering
{
    public sealed class CantonPixelView : MonoBehaviour
    {
        [Range(0,3)] public int mode=1;
        void Update(){if(Keyboard.current!=null && Keyboard.current.pKey.wasPressedThisFrame)mode=(mode+1)%4;}
        void OnGUI(){GUI.Label(new Rect(25,Screen.height-65,900,26),"P  Pixel pipeline: "+new[]{"Native","1280 wide","960 wide","640 wide"}[mode]+"  |  Bus state: "+(HongKong.Walk.CantonBus.Instance?HongKong.Walk.CantonBus.Instance.StateName:"--"));}
    }
}
