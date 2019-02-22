using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

namespace nexcode.nwcore
{
    [Serializable]
    public class MediaAudioProps : ComponentProps
    {
        public string url;
        public bool isLoop;
    }

    public class UnityAudioState
    {
        public float durationMillis;
        public bool isPlaying;
        public float positionMillis;
    }

    [Serializable]
    public class MediaAudio : TextureComponentBase<MediaAudioProps>, IMediaPlaybackControls, IGetState<UnityAudioState>
    {
        #region [ fields ]
        Action _onEnd;

        AudioSource audioSource;
        AudioClip audioClip;
        bool wasPlaying = false;
        #endregion

        #region [ constructor ]
        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            OnPropsChanged += (s, e) =>
            {
                foreach (var prop in e.PropsChanged)
                {
                    if (prop == "loop" || prop == "all")
                        audioSource.loop = props.isLoop;

                    if (prop == "url" || prop == "all")
                    {
                        audioClip = LoadAudio(props.url);
                        audioSource.transform.SetParent(transform, false);
                        audioSource.volume = 0;
                        audioSource.clip = audioClip;

                        wasPlaying = true;
                        audioSource.Play();
                    }
                }
            };

            SetProps(props);
        }
        #endregion

        AudioClip LoadAudio(string urlAudio)
        {
            //var urlAudio = string.Format("file:///{0}" + audio, settings.audiosFolder);
            Debug.Log("[MediaAudio] Loading " + urlAudio);
            var www = new WWW(urlAudio);
            while (!www.isDone)
                System.Threading.Thread.Sleep(0);
            var audioClip = www.GetAudioClip();
            return audioClip;
        }

        new void Update()
        {
            base.Update();
            if (audioSource != null)
            {
                audioSource.volume = volume;

                if (wasPlaying && !audioSource.isPlaying)
                {
                    if (_onEnd != null)
                        _onEnd();

                    _onEnd = null;
                }
            }
        }

        public object GetState()
        {
            return new UnityAudioState
            {
                isPlaying = IsPlaying(),
                durationMillis = audioClip != null ? audioClip.length * 1000 : 0,
                positionMillis = GetPositionMillis()
            };
        }

        public bool IsPlaying()
        {
            return audioSource != null && audioSource.isPlaying;
        }

        public float GetPositionMillis()
        {
            return audioSource != null ? audioSource.time * 1000 : 0;
        }

        public void Pause()
        {
            if (audioSource != null)
                audioSource.Pause();

            wasPlaying = false;
        }

        public void Play()
        {
            if (audioSource != null)
            {
                audioSource.UnPause();
                wasPlaying = true;
            }
        }

        public void Seek(float millis)
        {
            if (audioSource != null)
                audioSource.time = millis / 1000f;
        }

        public void OnEnd(Action endAction)
        {
            _onEnd = endAction;
        }

        public override Texture Texture
        {
            get
            {
                return texture;
            }
        }
    }
}
