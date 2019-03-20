using UnityEngine;
using System.Collections;
using nexcode.nwcore;
using Newtonsoft.Json.Linq;
using Components;

namespace Shine.Apps
{
    public class AppPhotosScript : MonoBehaviour
    {
        public GalleryMediaSource view;

        // Use this for initialization
        void Start()
        {
            var hub = NWCoreBase.hub;

            hub.Subscribe("_gallery")
                .On("open", (msg) =>
                {
                    Debug.Log("Gallery open");
                    view.GalleryOpen();
                })
                .On("pan", (msg) =>
                {
                    var panX = msg.Value<float>("panX");
                    var panY = msg.Value<float>("panY");

                    view.GalleryPan(panX, panY);
                })
                .On("scale", (msg) =>
                {
                    var scale = msg.Value<float>("scale");
                    view.GalleryScale(scale);
                })
                .On("tap", (msg) =>
                {
                    view.GalleryTap();
                })
                .On("play", (msg) =>
                {
                    var list = msg["list"] as JArray;
                    view.GalleryPlay(list);
                })
                .On("stop", (msg) =>
                {
                    view.GalleryStopPlaylist();
                })
                .On("goto", (msg) =>
                {
                    view.GalleryGoToPhoto(msg["photo"].ToString());
                })
                .On("delete", (msg) =>
                {
                    var list = msg["list"] as JArray;
                    view.GalleryDelete(list);
                })
                .On("list", (msg) =>
                {
                    view.SendAllImages();
                });


            hub.Subscribe("_gallery")
                .On("loadPlay", (msg) =>
                {
                    var list = msg["list"] as JArray;
                    view.GalleryLoadPlay(list);
                })
                .On("loadView", (msg) =>
                {
                    var list = msg["list"] as JArray;
                    view.GalleryLoadView(list);
                });


            hub.Subscribe(AppSettingsScript.Instance.FileuploadInstanceName)
                .On("onFileUploaded", (msg) =>
                {
                    view.GalleryEnqueueImage(msg["path"].ToString());
                });

        }
    }
}