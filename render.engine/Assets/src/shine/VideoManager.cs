using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Shine.Apps;
using nexcode.nwcore;

public class VideoManager : MonoBehaviour {
/*
    public NWLayer layerA, layerB;
    public VideoTextureSource videoA, videoB;

    public NWLayer curLayer;
    public VideoTextureSource curVideo;

    float fadeDuration = 1.5f;

	// Use this for initialization
	void Start () {

        curVideo = videoA;
        curLayer = layerA;

        var hub = NWCoreBase.hub;
        hub.Subscribe("_syscmd")
            .On("videoLoad", (msg) =>
            {
                FindObjectOfType<MoodMediaSource>().StopVideo();

                var layerToHide = curLayer;
                var videoToMute = curVideo;

                StartCoroutine(CoFadeVideoOut(curLayer, curVideo));
                StartCoroutine(CoPauseAndRewindIn(curVideo, fadeDuration));
                NextLayer();

                //var loop = msg.Data["loop"] != null && msg.Data.Value<bool>("loop");
                curVideo.VideoFile = msg["file"].ToString();
                curVideo.isUnloading = false;
                curVideo.Play();

                curVideo.avpro.Control.SetLooping(false); // loop);

                AppsManagerScript.Instance.HideLayer("Layer_Mood");

                // show video layer
                StartCoroutine(CoFadeVideoIn(curLayer, curVideo));


            })
            .On("videoPlay", (msg) =>
            {
                //curVideo.Play();

                //NodeManager.Instance.HideLayer("Layer_Mood");

                // show video layer
                //StartCoroutine(CoFadeVideoIn(curLayer, curVideo));

            })
            .On("videoPause", (msg) =>
            {
                curVideo.Pause();
            })
            .On("videoStop", (msg) =>
            {
                var layer = curLayer;
                var video = curVideo;
                DG.Tweening.DOTween.To(() => { return layer.masterOpacity; },
                    (float val) => { layer.masterOpacity = val; }, 0f, fadeDuration);

                DG.Tweening.DOTween.To(() => { return video.avpro.Control.GetVolume(); },
                    (float val) => { video.avpro.Control.SetVolume(val); }, 0f, fadeDuration);

                StartCoroutine(CoPauseAndRewindIn(curVideo, fadeDuration));
            })
            .On("videoHide", (msg) =>
            {
                // show video layer
                var layer = curLayer;
                var video = curVideo;
                DG.Tweening.DOTween.To(() => { return layer.masterOpacity; },
                    (float val) => { layer.masterOpacity = val; }, 0f, fadeDuration);

                DG.Tweening.DOTween.To(() => { return video.avpro.Control.GetVolume(); },
                    (float val) => { video.avpro.Control.SetVolume(val); }, 0f, fadeDuration);
            })
            .On("videoSetVolume", (msg) =>
            {
                curVideo.avpro.m_Volume = msg.Data.Value<float>("volume");
            })
            .On("videoPlaybackRate", (msg) =>
            {
                curVideo.avpro.m_PlaybackRate = msg.Data.Value<float>("rate");
            })
            .On("videoLoop", (msg) =>
            {
                curVideo.avpro.Control.SetLooping(true);
            });

    }

    IEnumerator CoFadeVideoOut(NWLayer layer, VideoTextureSource video)
    {
        yield return null;
        DOTween.To(() => { return layer.masterOpacity; },
            (float val) => { layer.masterOpacity = val; }, 0f, fadeDuration).SetEase(Ease.InQuad);

        DOTween.To(() => { return video.avpro.Control.GetVolume(); },
            (float val) => { video.avpro.Control.SetVolume(val); }, 0f, fadeDuration).SetEase(Ease.InCubic);
    }


    IEnumerator CoFadeVideoIn(NWLayer layer, VideoTextureSource video)
    {
        yield return null;
        DOTween.To(() => { return layer.masterOpacity; },
            (float val) => { layer.masterOpacity = val; }, 1f, fadeDuration).SetEase(Ease.OutQuad);

        DOTween.To(() => { return video.avpro.Control.GetVolume(); },
            (float val) => { video.avpro.Control.SetVolume(val); }, 1f, fadeDuration).SetEase(Ease.OutCubic);
    }

    IEnumerator CoPauseAndRewindIn(VideoTextureSource video, float secs)
    {
        video.isUnloading = true;
        yield return new WaitForSeconds(secs);
        if (video.isUnloading)
        {
            video.Pause();
            video.Rewind();
            video.isUnloading = false;
        }
    }


    public void NextLayer()
    {
        if (curLayer == layerA)
        {
            curLayer = layerB;
            curVideo = videoB;
        }
        else
        {
            curLayer = layerA;
            curVideo = videoA;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
