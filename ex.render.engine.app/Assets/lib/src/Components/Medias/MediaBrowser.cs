using Newtonsoft.Json;
using System;
using UnityEngine;

namespace nexcode.nwcore
{

    [Serializable]
    public class MediaBrowserProps : ComponentProps
    {
        public string url = "about:blank";
        public Vector2 size = new Vector2(256, 256);
    }

    public class MediaBrowser : TextureComponentBase<MediaBrowserProps>
    {
        #region [ fields ]
        IBrowser _browser;
        #endregion

        #region [ Init ]
        private void Start()
        {
            _browser = ComponentManager.Add<IBrowser>(gameObject);

            WhenLoaded(null);

            OnPropsChanged += (s, e) =>
            {
                foreach (var prop in e.PropsChanged)
                {
                    if (prop == "url" || prop == "all")
                    {
                        _browser.Url = Props.url;
                    }
                    if (prop == "size" || prop == "all")
                    {
                        _browser.Size = Props.size;
                    }
                }
            };

            SetProps(Props);
        }
        #endregion

        public void WhenLoaded(Action onLoaded)
        {
            _browser.WhenLoaded(() => {
                texture = _browser.Texture;
                if (onLoaded != null)
                    onLoaded();
                });
        }

        public override Texture Texture
        {
            get
            {
                return _browser == null ? null : _browser.Texture;
            }
        }
    }
}
