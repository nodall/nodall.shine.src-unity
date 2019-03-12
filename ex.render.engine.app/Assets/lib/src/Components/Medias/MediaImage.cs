using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

namespace nexcode.nwcore
{
    [Serializable]
    public class MediaImageProps : ComponentProps
    {
        public string url;
        [ReadOnly]
        public Vector2 size;
    }

    [Serializable]
    public class MediaImage : TextureComponentBase<MediaImageProps> 
    {
        #region [ fields ]
        Action whenLoaded;
        #endregion

        #region [ constructor ]
        private void Start()
        {
            OnPropsChanged += (s, e) =>
            {
                foreach (var prop in e.PropsChanged)
                {
                    if (prop == "url" || prop == "all")
                        StartCoroutine(CoLoadImage(Props.url));
                }
            };

            SetProps(props);
        }
        #endregion

        IEnumerator CoLoadImage(string url)
        {
            if (!url.Contains("://"))
                url = "file:///" + url;

            var www = new WWW(url);
            yield return www;

            texture = www.texture;
            texture.wrapMode = TextureWrapMode.Clamp;

            SetProp("size", new Vector2(texture.width, texture.height));

            if (texture != null && whenLoaded != null)
                whenLoaded();
        }

        public void WhenLoaded(Action action)
        {
            whenLoaded = action;
        }

        public override Texture Texture
        {
            get
            {
                return texture;
            }
        }
    }
}
