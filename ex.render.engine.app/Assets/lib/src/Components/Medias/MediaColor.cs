using System;
using UnityEngine;

namespace nexcode.nwcore
{
    [Serializable]
    public class MediaColorProps : ComponentProps
    {
        public Color color;
    }

    public class MediaColor : TextureComponentBase<MediaColorProps>
    {
        #region [ constructor  ]
        void Start()
        {
            OnPropsChanged += (s, e) =>
            {
                var colorTex = new Texture2D(4, 4);
                Color[] texColor = new Color[4 * 4];
                for (int i = 0; i < texColor.Length; ++i) texColor[i] = props.color;
                colorTex.SetPixels(texColor);
                colorTex.Apply();

                texture = colorTex;
            };

            SetProps(props);
        }
        #endregion 
    }
}
