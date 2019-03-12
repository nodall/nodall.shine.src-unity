using System;
using UnityEngine;

namespace nexcode.nwcore
{
    [Serializable]
    public class MediaTextureUnityProps : ComponentProps
    {
        public Texture texture;

        [ReadOnly]
        public Vector2 size;
    }

    public class MediaTextureUnity : TextureComponentBase<MediaTextureUnityProps>
    {
        #region [ constructor  ]
        void Start()
        {
            OnPropsChanged += (s, e) =>
            {
                foreach (var prop in e.PropsChanged)
                {
                    if (prop == "texture" || prop == "all")
                    {
                        texture = props.texture;
                        if (texture == null && props.size != Vector2.one)
                            SetProp("size", Vector2.one);
                        else
                            SetProp("size", new Vector2(texture.width, texture.height));
                    }
                }
            };

            SetProps(props);
        }
        #endregion 
    }
}
