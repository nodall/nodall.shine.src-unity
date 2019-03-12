using System;
using UnityEngine;

namespace nexcode.nwcore
{
    [Serializable]
    public class MediaDesktopDuplicationProps : ComponentProps
    {
        public int monitorId;
    }

    public class MediaDesktopDuplication : TextureComponentBase<MediaDesktopDuplicationProps>, INeedsVerticalFlip
    {
        public uDesktopDuplication.Texture ddTexture;

        public bool NeedsVerticalFlip
        {
            get
            {
                return true;
            }
        }

        #region [ init  ]
        private void Start()
        {
            Debug.Log("Start at MediaDesktopDuplication");
            var renderer = gameObject.AddComponent<MeshRenderer>();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            ddTexture = gameObject.AddComponent<uDesktopDuplication.Texture>();

            OnPropsChanged += (s, e) =>
            {
                ddTexture.monitorId = props.monitorId;
            };            

            renderer.material = Resources.Load("MatUnlitNoAlphaFade") as Material;

            SetProps(props);
        }
        #endregion

        public override void OnUpdate()
        {
            if (ddTexture != null && ddTexture.monitor != null)
                texture = ddTexture.monitor.texture;
        }
    }
}
