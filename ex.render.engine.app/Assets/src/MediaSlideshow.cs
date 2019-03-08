using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace nexcode.nwcore
{

    [Serializable]
    public class MediaSlideshowProps : ComponentProps
    {
        public string[] slides;
        public float slideTime;
        public float slideFadeTime;
        public int width = 1920, height = 1080;
    }

    public class MediaSlideshow : TextureComponentBase<MediaSlideshowProps>
    {
        public RenderTexture targetTexture;
        public int curSlideIndex;
        public float nextTextureAlpha;

        public Texture curTexture, nextTexture;

        public Material mat;

        private void Start()
        {
            targetTexture = new RenderTexture(1920, 1080, 0);
            mat = Resources.Load<Material>("MatUnlitTextureFade");

            OnPropsChanged += (s, e) =>
            {
                foreach (var prop in e.PropsChanged)
                {
                    if (prop == "all")
                    {
                    }
                }
            };

            SetProps(props);
        }

        public void Play()
        {
            Debug.Log("Play slideshow with " + JsonConvert.SerializeObject(props));
            StartCoroutine(CoPlaySlideshow());
        }

        IEnumerator CoPlaySlideshow()
        {
            if (props.slides.Length == 0)
                yield break;

            if (curTexture)
                Destroy(curTexture);

            curSlideIndex = 0;

            var www = new WWW(props.slides[curSlideIndex]);
            yield return www;
            curTexture = www.texture;
            curTexture.wrapMode = TextureWrapMode.Clamp;

            while (true)
            {
                // load next texture
                curSlideIndex++;
                curSlideIndex %= props.slides.Length;
                www = new WWW(props.slides[curSlideIndex]);
                yield return www;
                nextTexture = www.texture;
                nextTexture.wrapMode = TextureWrapMode.Clamp;

                yield return new WaitForSeconds(props.slideTime);

                // do fade between curTexture and nextTexture
                var startFade = DateTime.UtcNow;

                if (props.slideFadeTime != 0)
                {
                    while ((DateTime.UtcNow - startFade).TotalSeconds <= props.slideFadeTime)
                    {
                        nextTextureAlpha = Mathf.Clamp01((float) (DateTime.UtcNow - startFade).TotalSeconds / props.slideFadeTime);
                        yield return null;
                    }
                }

                // nextTexture becomes curTexture
                if (curTexture)
                    Destroy(curTexture);

                curTexture = nextTexture;
                nextTextureAlpha = 0;
            }
        }


        public void Stop()
        {
            StopAllCoroutines();

            if (curTexture)
                Destroy(curTexture);

            if (nextTexture)
                Destroy(nextTexture);

            curTexture = null;
            nextTexture = null;
        }

        public void Close()
        {
            StopAllCoroutines();
        }

        public void Rewind(bool pause=false)
        {
        }

        public void Pause()
        {
        }

        public float GetPositionMillis()
        {
            return -1;
        }

        public float GetDurationMillis()
        {
            return -1;
        }

        public override Texture Texture
        {
            get
            {
                return targetTexture;
            }
        }

        public override void OnUpdate()
        {
            // Compose texture
            var oldRT = RenderTexture.active;

            Graphics.SetRenderTarget(targetTexture);

            GL.Clear(true, true, new Color(0, 0, 0, 0));
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.LoadPixelMatrix(0, 20000, 20000, 0);

            var uvRect = new Rect(0, 0, 1, 1);
            var posRect = new Rect(0, 0, 1, 1);

            if (curTexture)
                DrawTexture(curTexture, 1f, posRect, uvRect, 20000, 20000);

            if (nextTexture)
                DrawTexture(nextTexture, nextTextureAlpha, posRect, uvRect, 20000, 20000);

            GL.PopMatrix();
            Graphics.SetRenderTarget(oldRT);


            texture = targetTexture;
        }

        protected void DrawTexture(Texture texture, float alpha, Rect posRect, Rect uvRect, float width, float height)
        {
            if (texture == null || alpha < 0.0002f) return;

            var needsFlipY = texture is INeedsVerticalFlip ? (texture as INeedsVerticalFlip).NeedsVerticalFlip : false;

            mat.color = new Color(1, 1, 1, alpha);
            var rtRect = new Rect(posRect.x * width, posRect.y * height, posRect.width * width, posRect.height * height);

            if (!needsFlipY)
                Graphics.DrawTexture(rtRect, texture, uvRect, 0, 0, 0, 0, mat);
            else
                Graphics.DrawTexture(rtRect, texture, new Rect(uvRect.x, 1 - uvRect.y, uvRect.width, -uvRect.height), 0, 0, 0, 0, mat);
        }

    }
}