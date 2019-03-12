using UnityEngine;
using System.Collections;

namespace nexcode.nwcore
{
    public class FadeMediaCompositor : FadeMediaCompositor<MediaCompositorProps>
    {

    }

    public abstract class FadeMediaCompositor<TProps> : MediaCompositorBase<TProps>, IUpdateable where TProps : MediaCompositorProps
    {
        public bool isBlackout;

        public Material matUnlitTextureFade;
        public Texture lastDrawnTexture;

        public float[] opacities;

        public new void Start()
        {
            Debug.Log("[FadeMediaCompositor] Start");
            base.Start();
            matUnlitTextureFade = Resources.Load("MatUnlitTextureFade") as Material;
        }

        public override void OnUpdate()
        {
            if (renderTexture == null)
                return;

            base.OnUpdate();

            if (opacities == null || opacities.Length != inputs.Length)
                opacities = new float[inputs.Length];

            for (int i = 0; i < inputs.Length; ++i) {
                var tex = inputs[i] as TextureComponentBase<ComponentProps>;
                opacities[i] = tex == null ? 0 : tex.opacity;
            }

            var oldRT = RenderTexture.active;

            /*
            if (_renderTexture == null || _renderTexture.width != width || _renderTexture.height != height)
            {
                _renderTexture = new RenderTexture(width, height, 0);
            }*/

            var width = renderTexture.width;
            var height = renderTexture.height;

            Graphics.SetRenderTarget(renderTexture);

            GL.Clear(true, true, new Color(0, 0, 0, 0));
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, 1, 1, 0);

            for (int i = 0; i < inputs.Length; ++i)
            {
                var input = inputs[i] as ITexture;
                if (input != null)
                    DrawTexture(input, input.Opacity);
            }

            if (isBlackout)
            {
                matUnlitTextureFade.color = new Color(0, 0, 0, 1);
                Graphics.DrawTexture(new Rect(0, 0, 1, 1), Texture2D.whiteTexture, matUnlitTextureFade);
            }

            GL.PopMatrix();
            Graphics.SetRenderTarget(oldRT);
        }

        protected void DrawTexture(ITexture texture, float alpha)
        {
            if (texture == null || alpha < 0.0001f) return;

            var needsFlipY = texture is INeedsVerticalFlip ? (texture as INeedsVerticalFlip).NeedsVerticalFlip : false;
            var mat = matUnlitTextureFade;

            var renderer = (texture as MonoBehaviour).GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
                mat = renderer.material;

            mat.color = new Color(1, 1, 1, alpha);

            var tex = texture.Texture;
            if (tex != null)
            {
                //Graphics.DrawTexture(new Rect(0, 0, 1, 1), tex, mat);
                if (!needsFlipY)
                    Graphics.DrawTexture(new Rect(0, 0, 1, 1), tex, new Rect(0, 0, 1, 1), 0, 0, 0, 0, mat);
                else
                    Graphics.DrawTexture(new Rect(0, 0, 1, 1), tex, new Rect(0, 1, 1, -1), 0, 0, 0, 0, mat);
                lastDrawnTexture = tex;
            }
        }

    }

}   