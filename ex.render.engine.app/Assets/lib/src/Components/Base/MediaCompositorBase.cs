using UnityEngine;
using System.Collections;
using System;

namespace nexcode.nwcore
{
    public class MediaCompositorProps : ComponentProps
    {
        public int width, height;
    }

    public class MediaCompositorBase<TProps> : TextureComponentBase<TProps> where TProps : MediaCompositorProps
    {
        public RenderTexture renderTexture;

        public void Start()
        {
            Debug.Log("[MediaCompositorBase] Start");
            renderTexture = new RenderTexture(Props.width, Props.height, 0);
        }

        public override void OnUpdate()
        {
            texture = renderTexture;
        }

        public override Texture Texture { get
        {
            return renderTexture;
        } }
    }
}