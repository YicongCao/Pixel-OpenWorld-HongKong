using System;
using UnityEngine;

namespace HongKong.Walk
{
    [RequireComponent(typeof(TextMesh))]
    public sealed class WorldLabel : MonoBehaviour
    {
        public string content;
        public string fallback;
        public Material depthMaterial;
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
        void LateUpdate(){if(depthMaterial)depthMaterial.mainTexture=ReadableFont.material.mainTexture;}
        void Awake()
        {
            var mesh=GetComponent<TextMesh>();mesh.font=ReadableFont;
            mesh.text=ReadableFont.HasCharacter('香')?content:fallback;
            GetComponent<Renderer>().sharedMaterial=depthMaterial?depthMaterial:ReadableFont.material;
        }
    }
}
