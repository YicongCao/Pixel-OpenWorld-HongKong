using System;
using UnityEngine;

namespace HongKong.Walk
{
    [RequireComponent(typeof(TextMesh))]
    public sealed class WorldLabel : MonoBehaviour
    {
        public string content;
        public string fallback;
        static Font shared;
        public static Font ReadableFont
        {
            get
            {
                if(shared)return shared;
                var names=Font.GetOSInstalledFontNames();
                foreach(var candidate in new[]{"PingFang TC","PingFang SC","Heiti TC"})
                    if(Array.IndexOf(names,candidate)>=0){shared=Font.CreateDynamicFontFromOSFont(candidate,64);return shared;}
                return shared=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }
        void Awake()
        {
            var mesh=GetComponent<TextMesh>();mesh.font=ReadableFont;
            mesh.text=ReadableFont.HasCharacter('香')?content:fallback;
            GetComponent<Renderer>().sharedMaterial=ReadableFont.material;
        }
    }
}
