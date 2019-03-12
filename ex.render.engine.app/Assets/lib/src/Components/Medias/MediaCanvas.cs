using Newtonsoft.Json;
using System;
using UnityEngine;

namespace nexcode.nwcore
{
    [Serializable]
    public class MediaCanvasProps : ComponentProps
    {
        [JsonIgnore]
        public Canvas canvas;
        public Vector2 size;
        public Color backgroundColor = new Color(0, 0, 0, 0f);
    }

    public class MediaCanvas : TextureComponentBase<MediaCanvasProps>
    {
        static int globalCanvasLayer = 10;


        #region [ fields ]
        GameObject _gameObject;
        int canvasLayer;
        Canvas canvas;
        new Camera camera;
        RenderTexture renderTexture;
        #endregion


        #region [ constructor ]
        public void Start()
        {
            if (Props.size == Vector2.zero)
                Props.size = new Vector2(Screen.width, Screen.height);

            OnPropsChanged += (s, e) =>
            {
                foreach (var prop in e.PropsChanged)
                {
                    if (prop == "size" || prop == "all")
                    {
                        if (camera == null || (renderTexture != null && (renderTexture.width != props.size.x || renderTexture.height != props.size.y)))
                        {
                            camera = GetComponent<Camera>();
                            var newLayer = false;
                            if (camera == null)
                            {
                                camera = gameObject.AddComponent<Camera>();
                                newLayer = true;
                            }

                            renderTexture = new RenderTexture((int)Props.size.x, (int)Props.size.y, 24);
                            camera.targetTexture = renderTexture;

                            if (newLayer)
                            {
                                camera.backgroundColor = Props.backgroundColor;
                                camera.clearFlags = CameraClearFlags.SolidColor;
                                camera.cullingMask = 1 << globalCanvasLayer;
                                canvasLayer = globalCanvasLayer;

                                ++MediaCanvas.globalCanvasLayer;
                            }
                            texture = renderTexture;
                        }
                    }
                    if (prop == "canvas" || prop == "all")
                    {
                        canvas = props.canvas;

                        if (canvas != null)
                        {
                            canvas.renderMode = RenderMode.ScreenSpaceCamera;
                            canvas.worldCamera = camera;
                            canvas.gameObject.layer = canvasLayer;
                        }
                    }

                    if (prop == "backgroundColor" || prop == "all")
                    {
                        camera.backgroundColor = Props.backgroundColor;
                    }
                }
            };

            SetProps(props);
        }

        public override void OnUpdate()
        {
            if (camera != null)
                camera.backgroundColor = Props.backgroundColor;
        }

        /*public void Update()
        {
            if (camera != null) {
                Debug.Log("Camera rendering of canvas");
                camera.Render();
            }
            IsUpdated = true;
        }*/
        #endregion
    }
}
