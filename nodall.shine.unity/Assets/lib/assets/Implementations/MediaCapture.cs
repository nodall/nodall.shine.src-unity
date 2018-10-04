using RenderHeads.Media.AVProLiveCamera;
using System;
using UnityEngine;

namespace nexcode.nwcore
{
    [Serializable]
    public class MediaCaptureProps : ComponentProps
    {
        public string path;

        [ReadOnly]
        public Vector2 size;
    }

    public class MediaCapture : TextureComponentBase<MediaCaptureProps>
    {
        AVProLiveCamera cam;

        #region [ constructor  ]
        void Start()
        {
            OnPropsChanged += (s, e) =>
            {
                cam = gameObject.GetComponent<AVProLiveCamera>();
                if (cam == null)
                {
                    var indices = props.path.ToLowerInvariant().Split('.')[0].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var idx in indices)
                        Debug.Log("indices: " + idx);

                    cam = gameObject.AddComponent<AVProLiveCamera>();

                    if (indices.Length > 0)
                    {
                        var camIdx = int.Parse(indices[0].Replace("/", ""));

                        cam._deviceSelection = AVProLiveCamera.SelectDeviceBy.Index;
                        cam._desiredDeviceIndex = camIdx;

                        if (indices.Length > 1)
                        {
                            int resWidth = -1, resHeight = -1;
                            string modeStr = null;

                            for (int i = 1; i < indices.Length; ++i) {
                                var modeOrRes = indices[i];
                                if (modeOrRes.Contains("x"))
                                {
                                    var resSpl = modeOrRes.Split('x');
                                    if (resSpl.Length == 2)
                                    {
                                        resWidth = int.Parse(resSpl[0]);
                                        resHeight = int.Parse(resSpl[1]);
                                    }
                                }
                                else
                                {
                                    modeStr = modeOrRes;
                                }
                            }

                            //Debug.Log("Desired res " + resWidth + "x" + resHeight + " fromat " + modeStr);

                            var avMgr = FindObjectOfType<AVProLiveCameraManager>();
                            var device = avMgr.GetDevice(camIdx);

                            for (int idx = 0; idx < device.NumModes; ++idx)
                            {
                                var mode = device.GetMode(idx);
                                Debug.Log(idx + " " + device.Name + ": " + mode.Format + " " + mode.Width + "x" + mode.Height);
                                if (mode.Width == resWidth && mode.Height == resHeight && modeStr == mode.Format.ToLowerInvariant())
                                {
                                    cam._modeSelection = AVProLiveCamera.SelectModeBy.Index;
                                    cam._desiredModeIndex =  idx;
                                }
                            }
                        }
                    }
                }

                //var indices = props.path.Split('.')[0].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                //if (indices.Length > 0)
                //{
                //    cam._deviceSelection = AVProLiveCamera.SelectDeviceBy.Index;
                //    try
                //    {
                //        cam._desiredDeviceIndex = int.Parse(indices[0]);
                //    }
                //    catch (Exception) { }
                //}

            };

            SetProps(props);
        }
        #endregion 

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (cam != null)
                texture = cam.OutputTexture;
        }

        public float DisplayFrameRate()
        {
            if (cam != null && cam.Device != null)
            {
                return cam.Device.DisplayFPS;
            }

            return 0;
        }

        public string DisplaySize()
        {
            if (cam != null && cam.Device != null)
            {
                return cam.Device.CurrentWidth + "x" + cam.Device.CurrentHeight;
            }
            return "";
        }
    }

}
