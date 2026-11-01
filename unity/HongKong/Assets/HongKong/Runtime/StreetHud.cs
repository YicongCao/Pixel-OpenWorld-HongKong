using UnityEngine;

namespace HongKong.Walk
{
    public sealed class StreetHud : MonoBehaviour
    {
        public StreetWalker walker;
        public string location = "Nathan Road  ×  Haiphong Road  ·  Tsim Sha Tsui";
        public string stage = "V1-02 / spatial prototype";
        GUIStyle title, detail;
        void OnGUI()
        {
            if (title == null)
            {
                title = new GUIStyle(GUI.skin.label) { fontSize = 23, fontStyle=FontStyle.Bold };
                detail = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            }
            GUI.color = new Color(.04f,.09f,.12f,.88f);
            GUI.DrawTexture(new Rect(22,20,440,98),Texture2D.whiteTexture);
            GUI.color = new Color(.96f,.92f,.8f);
            GUI.Label(new Rect(38,30,420,30),"HONG KONG / PIXEL RAMBLE", title);
            GUI.Label(new Rect(38,65,420,26),location,detail);
            GUI.Label(new Rect(38,88,420,24),stage,detail);
            GUI.color = Color.white;
            GUI.Label(new Rect(25,Screen.height-38,950,28),"WASD / Arrows   Walk     Shift   Fast walk     Esc   Pause     N   Day / Night     F   Depth of field",detail);
            if (walker.paused)
            {
                GUI.Box(new Rect(Screen.width/2-150,Screen.height/2-85,300,170),"PAUSED");
                if (GUI.Button(new Rect(Screen.width/2-120,Screen.height/2-45,240,38),"Continue walking")) walker.paused=false;
                if (GUI.Button(new Rect(Screen.width/2-120,Screen.height/2+5,240,38),"Return to starting point")) {walker.ResetToStart();walker.paused=false;}
                if (GUI.Button(new Rect(Screen.width/2-120,Screen.height/2+50,240,28),"Quit")) Application.Quit();
            }
        }
    }
}
