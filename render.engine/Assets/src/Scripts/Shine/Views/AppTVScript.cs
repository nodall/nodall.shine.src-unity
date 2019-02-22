using UnityEngine;
using System.Collections;
using System;
using Components;
using nexcode.nwcore;
using System.IO;
using UnityEngine.UI;

namespace Shine.Apps
{
    public class AppTVScript : MonoBehaviour
    {

        #region [ fields ]
        MediaTextureUnity mediaChannelChangeTexture;
        MediaCanvas mediaCanvas;
        #endregion

        #region [ class ]
        [Serializable]
        public class Settings
        {
            public float channelChangeDuration;
            public float channelChangeFadeDuration;
        }
        #endregion

        #region [ public fields ]
        public Settings settings;
        public Texture2D channelTextureChange;
        public Canvas tvCanvas;
        #endregion

        #region [ MonoBehaviour ]
        void Start()
        {
            settings = AppSettingsScript.Instance.Get<Settings>("Shine", "Apps", "AppTV");

            var hub = NWCoreBase.hub;

            hub.Subscribe("app-tv").On("channelChange", (msg) =>
            {
                Debug.Log("Channel change");
                ShowTVDesktopDuplication();
                StartCoroutine(CoFadeChangeChannels());
            })
            .On("resize", (msg) =>
            {
                    var x = float.Parse(msg["x"].ToString());
                    var y = float.Parse(msg["y"].ToString());
                    var w = float.Parse(msg["w"].ToString());
                    var h = float.Parse(msg["h"].ToString());

                /*
                 * Smooth
                var r = GetComponent<NWLayer>().rectangle;
                float t = 0.2f;

                x = Mathf.Lerp(x, r.x, t);
                y = Mathf.Lerp(y, r.x, t);
                w = Mathf.Lerp(w, r.width, t);
                h = Mathf.Lerp(h, r.height, t);
                */

                    ResizeCanvas(x, y, w, h);
                });

            channelTextureChange = new Texture2D(4, 4);
            //channelTextureChange.LoadImage(File.ReadAllBytes(AppsManagerScript.Instance.folders.medias + "app-tv/tv-change-ch.png"));
            mediaChannelChangeTexture = ComponentManager.New<MediaTextureUnity>(new MediaTextureUnityProps { texture = channelTextureChange });

            mediaCanvas = ComponentManager.New<MediaCanvas>(new MediaCanvasProps() { canvas = tvCanvas });
        }
        #endregion

        private void ShowTVDesktopDuplication()
        {
            //tvCanvas.GetComponentInChildren<RawImage>().texture = FindObjectOfType<MediaDesktopDuplication>().texture;
            //shineCompositor.ShowTexture(mediaCanvas);
        }

        private void ResizeCanvas(float x, float y, float w, float h)
        {
            var rect = tvCanvas.GetComponentInChildren<RawImage>().GetComponent<RectTransform>();
            var canvasSize = tvCanvas.GetComponent<RectTransform>().sizeDelta;

            rect.anchoredPosition = new Vector2(canvasSize.x * x, canvasSize.y * (1-y));
            rect.sizeDelta = new Vector2(canvasSize.x * w, canvasSize.y * h);
        }

        #region [ coroutines ]
        IEnumerator CoFadeChangeChannels()
        {
            //shineCompositor.ShowOverlay(mediaChannelChangeTexture, settings.channelChangeFadeDuration);
            yield return null;
            //yield return new WaitForSeconds(settings.channelChangeDuration);
            //ShowTVDesktopDuplication();
            //shineCompositor.HideOverlay(settings.channelChangeFadeDuration);
        }
        #endregion
    }
}
