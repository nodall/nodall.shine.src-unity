using UnityEngine;
using System.Collections;
using Components;
using nexcode.nwcore;
using System.IO;
using System;
using System.Linq;

namespace Shine.Apps
{
    [Serializable]
    public class UnityColorsState
    {
        public bool isPlaying;
        public string[] startCorners, endCorners, currentCorners;
        public float loopTime;
        public float positionInLoop;
    }

    public class UnityColorsArgs {
        public float loopTime;
        public string[] startCorners, endCorners;
        public bool fadeColors;
    }

    public class ColorsView : TextureComponentBase<ComponentProps>, IMediaPlaybackControls, IGetState<UnityColorsState>
    {
        public static ColorsView Instance;

        bool isShowingWebview = false;

        Action _onEnd;

        public Texture2D colorsTexture;

        public float lightness = 50, hue = 0, saturation = 70;
        public Color color;
        public Color[] colorsGradient = new Color[4];
        public bool isColorFading = false;

        Coroutine coroutineFade = null;

        public bool isPlaying = false;

        public float startTime;
        public float curTime;

        public float positionInLoop;
        public float loopTime;
        public Color[] startColors, endColors, currentColors;

        public Action onStateChange;

        void Awake()
        {
            Instance = this;        
        }

        void Start()
        {
            startColors = new Color[4];
            endColors = new Color[4];
            currentColors = new Color[4];

            NWCore.hub.Subscribe("colors").On("setColors", (msg) =>
            {
                var args = msg.ToObject<UnityColorsArgs>();
                SetColorsWithFade(args);
                if (onStateChange != null)
                    onStateChange.Invoke();
            });

            colorsTexture = new Texture2D(2, 2);
            colorsTexture.wrapMode = TextureWrapMode.Clamp;
            texture = colorsTexture;

            gameObject.SetActive(false);
        }

        protected void UpdateTextureWithGradient(Color[] cols)
        {
            colorsTexture.SetPixel(0, 1 - 0, cols[0]);
            colorsTexture.SetPixel(1, 1 - 0, cols[1]);
            colorsTexture.SetPixel(1, 1 - 1, cols[2]);
            colorsTexture.SetPixel(0, 1 - 1, cols[3]);
            colorsTexture.Apply();
        }

        public void PlayColors(UnityColorsArgs args)
        {
            gameObject.SetActive(true);
            Seek(0);
            Play();
            SetColors(args);
            StartCoroutine(CoStartColors());
        }

        public void SetColors(UnityColorsArgs args)
        {
            loopTime = args.loopTime;
            if (loopTime == 0)
                isPlaying = false;

            for (int i = 0; i < 4; ++i)
            {
                ColorUtility.TryParseHtmlString(args.startCorners[i], out startColors[i]);
                ColorUtility.TryParseHtmlString(args.endCorners[i], out endColors[i]);
            }
        }

        public void SetColorsWithFade(UnityColorsArgs args)
        {
            Color[] targetStart = new Color[4];
            Color[] targetEnd = new Color[4];
            loopTime = args.loopTime;
            if (loopTime == 0)
                isPlaying = false;

            for (int i = 0; i < 4; ++i)
            {
                ColorUtility.TryParseHtmlString(args.startCorners[i], out targetStart[i]);
                ColorUtility.TryParseHtmlString(args.endCorners[i], out targetEnd[i]);
            }

            if (args.fadeColors)
            {
                StartCoroutine(CoFadeColors(targetStart, targetEnd));
            }
            else
            {
                for (int i = 0; i < 4; ++i)
                {
                    startColors[i] = targetStart[i];
                    endColors[i] = targetEnd[i];
                }
            }
        }

        IEnumerator CoFadeColors(Color[] targetStart, Color[] targetEnd)
        {
            for (int k = 0; k < 50; ++k)
            {
                for (int i = 0; i < 4; ++i)
                {
                    startColors[i] = Color.Lerp(startColors[i], targetStart[i], 0.1f + k * 0.01f);
                    endColors[i] = Color.Lerp(endColors[i], targetEnd[i], 0.1f + k * 0.01f);
                }
                yield return null;
            }
            if (onStateChange != null)
                onStateChange.Invoke();
        }

        IEnumerator CoStartColors()
        {
            isColorFading = true;

            startTime = Time.time;

            while (true)
            {
                if (isPlaying)
                    curTime += Time.deltaTime;

                var loopTime = this.loopTime;

                if (loopTime < 0.01f)
                {
                    loopTime = 1f;
                    positionInLoop = 0;
                }
                else
                {
                    if (positionInLoop > curTime % (loopTime * 2))
                    {
                        if (_onEnd != null)
                            _onEnd();

                        _onEnd = null;
                    }

                    positionInLoop = curTime % (loopTime * 2);
                }

                if (positionInLoop < 0)
                {
                    positionInLoop += loopTime * 2;
                }

                for (int i = 0; i < 4; ++i)
                {
                    if (positionInLoop < loopTime)
                        currentColors[i] = Color.Lerp(startColors[i], endColors[i], positionInLoop / loopTime);
                    else
                        currentColors[i] = Color.Lerp(endColors[i], startColors[i], (positionInLoop - loopTime) / loopTime);
                }

                UpdateTextureWithGradient(currentColors);
                yield return null;
            }
        }

        public object GetState()
        {
            return new UnityColorsState
            {
                isPlaying = isPlaying,
                loopTime = loopTime,
                positionInLoop = positionInLoop,
                currentCorners = currentColors.Select(c => "#" + ColorUtility.ToHtmlStringRGBA(c)).ToArray(),
                startCorners = startColors.Select(c => "#" + ColorUtility.ToHtmlStringRGBA(c)).ToArray(),
                endCorners = endColors.Select(c => "#" + ColorUtility.ToHtmlStringRGBA(c)).ToArray(),
            };
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public float GetPositionMillis()
        {
            return positionInLoop;
        }

        public void Pause()
        {
            isPlaying = false;
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Seek(float millis)
        {
            curTime = millis / 1000f;
            positionInLoop = curTime % (loopTime * 2);
        }

        public void OnEnd(Action endAction)
        {
            _onEnd = endAction;
        }
    }
}   