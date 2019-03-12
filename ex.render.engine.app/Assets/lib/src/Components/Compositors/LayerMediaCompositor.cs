using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nexcode.nwcore
{
    public class LayerMediaCompositor : LayerMediaCompositor<MediaCompositorProps>
    {

    }

    public abstract class LayerMediaCompositor<TProps> : MediaCompositorBase<TProps>, IUpdateable where TProps : MediaCompositorProps
    {
        public bool isBlackout;

        Material matUnlitTextureFade;

        public float[] opacities;
        public List<Rect> posRects = new List<Rect>(), uvRects = new List<Rect>();

        public new void Start()
        {
            Debug.Log("[LayerMediaCompositor] Start");
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

            var oldRT = RenderTexture.active;
            var width = renderTexture.width;
            var height = renderTexture.height;

            Graphics.SetRenderTarget(renderTexture);

            GL.Clear(true, true, new Color(0, 0, 0, 0));
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.LoadPixelMatrix(0, 20000, 20000, 0);

            for (int i = 0; i < inputs.Length; ++i)
            {
                var input = inputs[i] as ITexture;
                if (input != null)
                {
                    var posRect = posRects.Count > i ? posRects[i] : new Rect(0, 0, 1, 1);
                    var uvRect = uvRects.Count > i ? uvRects[i] : new Rect(0, 0, 1, 1);
                    DrawTexture(input, input.Opacity, posRect, uvRect, 20000, 20000);
                    opacities[i] = input.Opacity;
                }
            }

            if (isBlackout)
            {
                matUnlitTextureFade.color = new Color(0, 0, 0, 1);
                Graphics.DrawTexture(new Rect(0, 0, 1, 1), Texture2D.whiteTexture, matUnlitTextureFade);
            }

            GL.PopMatrix();
            Graphics.SetRenderTarget(oldRT);
        }

        public void SetPosRectOf(ComponentBase comp, Rect rect)
        {
            for (int i = 0; i < inputs.Length; ++i)
                if (comp == inputs[i])
                    SetPosRectOf(i, rect);
        }

        public void SetPosRectOf(int rectIdx, Rect rect)
        {
            if (posRects.Count > rectIdx)
                posRects[rectIdx] = rect;
            else
                posRects.Insert(rectIdx, rect);
        }

        public void SetUVRectOf(ComponentBase comp, Rect rect)
        {
            for (int i = 0; i < inputs.Length; ++i)
                if (comp == inputs[i])
                    SetUVRectOf(i, rect);
        }

        public void SetUVRectOf(int rectIdx, Rect rect)
        {
            if (uvRects.Count > rectIdx)
                uvRects[rectIdx] = rect;
            else
                uvRects.Insert(rectIdx, rect);
        }

        protected void DrawTexture(ITexture texture, float alpha, Rect posRect, Rect uvRect, float width, float height)
        {
            if (texture == null || alpha < 0.0002f) return;

            var needsFlipY = texture is INeedsVerticalFlip ? (texture as INeedsVerticalFlip).NeedsVerticalFlip : false;
            var mat = matUnlitTextureFade;

            var renderer = (texture as MonoBehaviour).GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
                mat = renderer.material;

            mat.color = new Color(1, 1, 1, alpha);
            var rtRect = new Rect(posRect.x * width, posRect.y * height, posRect.width * width, posRect.height * height);

            var tex = texture.Texture;
            if (tex != null)
            {
                if (!needsFlipY)
                    Graphics.DrawTexture(rtRect, tex, uvRect, 0, 0, 0, 0, mat);
                else
                    Graphics.DrawTexture(rtRect, tex, new Rect(uvRect.x, 1 - uvRect.y, uvRect.width, -uvRect.height), 0, 0, 0, 0, mat);
            }
        }

    }

}   