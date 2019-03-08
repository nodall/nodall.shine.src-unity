using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using nexcode.nwcore;
using System;

namespace Components
{
    public class AudioPlayer : MonoBehaviour
    {
        #region [ Singleton ]
        public static AudioPlayer Instance { get; private set; }
        #endregion

        [System.Serializable]
        public class Settings
        {
            public float fadeInDuration, fadeOutDuration;
            public string audiosFolder;
            public List<string> audios;
        }

        public Settings settings;

        //public Dictionary<string, AudioClip> audios = new Dictionary<string, AudioClip>();
        //public List<AudioClip> audiosList;

        // Use this for initialization
        void Start()
        {
            settings = AppSettingsScript.Instance.Get<Settings>("Components", "AudioPlayer");
            //StartCoroutine(CoLoadAudios());

            var hub = NWCoreBase.hub;
            
            NWCoreBase.hub.Subscribe("audioPlayer").
                On("play", (msg) =>
                {
                    var audio = msg.Value<string>("audio").ToLowerInvariant();
                    Debug.Log("[audioPlayer] play "+audio);
                    var audioTrans = transform.Find(audio.Replace('/', ':'));
                    var audioSource = audioTrans == null ? null : audioTrans.GetComponent<AudioSource>();

                    Debug.Log(audioTrans);

                    var fadeInDur = settings.fadeInDuration;
                    if (msg["fadeIn"] != null || msg.Value<float>("fadeIn") >= 0)
                        fadeInDur = msg.Value<float>("fadeIn");

                    var fadeOutDur = settings.fadeOutDuration;
                    if (msg["fadeOut"] != null || msg.Value<float>("fadeOut") >= 0)
                        fadeOutDur = msg.Value<float>("fadeOut");

                    var loop = msg["loop"] != null || msg.Value<bool>("loop");
                    var replay = msg["replay"] != null || msg.Value<bool>("replay");

                    if (audioSource != null)
                    {
                        if (audioSource.isPlaying)
                        {
                            if (replay)
                            {
                                audioSource.DOFade(0, fadeOutDur).OnComplete(() => {
                                    audioSource.Stop();
                                    audioSource.Play();
                                    audioSource.DOFade(1, fadeInDur);
                                });
                            }
                        }
                        else
                        {
                            audioSource.Play();
                            audioSource.DOFade(1, fadeInDur);
                        }
                    }
                    else
                    {
                        try
                        {
                            var audioClip = LoadAudio(audio); // audios[audio];
                            audioSource = new GameObject(audio.Replace('/', ':')).AddComponent<AudioSource>();
                            audioSource.transform.SetParent(transform, false);
                            audioSource.volume = 0;
                            audioSource.clip = audioClip;

                            audioSource.Play();
                            audioSource.DOFade(1, fadeInDur);

                        } catch (System.Exception e)
                        {
                            Debug.LogError("[AudioPlayer] Trying to play missing: " + audio);
                        }
                    }

                    audioSource.loop = loop;

                })
                .On("stop", (msg) =>
                {
                    Debug.Log("[audioPlayer] stop");
                    var audio = msg.Value<string>("audio").ToLowerInvariant();
                    var audioTrans = transform.Find(audio);
                    var audioSource = audioTrans == null ? null : audioTrans.GetComponent<AudioSource>();

                    var fadeOutDur = settings.fadeOutDuration;
                    if (msg["fadeOut"] != null || msg.Value<float>("fadeOut") >= 0)
                        fadeOutDur = msg.Value<float>("fadeOut");

                    if (audioSource != null)
                    {
                        if (audioSource.isPlaying)
                            audioSource.DOFade(0, fadeOutDur).OnComplete(() => Destroy(audioSource.gameObject));
                        else
                            Destroy(audioSource.gameObject);
                    }
                })
                .On("stopAll", (msg) =>
                {
                    StopAll();
                });

            #region [ Singleton ]
            if (Instance == null)
                Instance = this;
            else
                throw new Exception("Only One Instance is allowed");
            #endregion
        }

        public void StopAll()
        {
            foreach (Transform child in transform)
            {
                var audioSource = child.GetComponent<AudioSource>();
                if (audioSource != null)
                    audioSource.DOFade(0, settings.fadeOutDuration).OnComplete(() => Destroy(audioSource.gameObject));
            }
        }

        AudioClip LoadAudio(string audio)
        {
            var urlAudio = string.Format("file:///{0}" + audio, settings.audiosFolder);
            Debug.Log("[AudioPlayer] Loading " + urlAudio);
            var www = new WWW(urlAudio);
            while (!www.isDone)
                System.Threading.Thread.Sleep(0);
            var audioClip = www.GetAudioClip();
            return audioClip;
        }

        //IEnumerator CoLoadAudios()
        //{
        //    foreach (var audio in settings.audios)
        //    {
        //        var urlAudio = string.Format("file:///{0}" + audio, settings.audiosFolder);
        //        Debug.Log("[AudioPlayer] Loading " + urlAudio);
        //        var www = new WWW(urlAudio);
        //        yield return www;
        //        var audioClip = www.audioClip;

        //        audios.Add(audio.ToLowerInvariant(), audioClip);
        //        audiosList.Add(audioClip);
        //    }
        //}
    }
}