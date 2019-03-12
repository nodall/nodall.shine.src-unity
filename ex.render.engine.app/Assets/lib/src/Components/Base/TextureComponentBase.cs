using UnityEngine;
using System.Collections;
using System;

namespace nexcode.nwcore
{
    public abstract class TextureComponentBase<TProps> : ComponentBase<TProps>, ITexture where TProps : ComponentProps
    {
        [Range(0, 1)]
        public float opacity;

        public float Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        public override bool HasTexture
        {
            get
            {
                return true;
            }
        }

        public virtual Texture Texture
        {
            get
            {
                return this.texture;
            }
        }
    }
}