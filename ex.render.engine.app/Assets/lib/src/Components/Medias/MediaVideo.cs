using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace nexcode.nwcore
{
    public class SyncedLately
    {
        public string channel;
        public DateTime date;
    }

    [Serializable]
    public class MediaVideoProps : ComponentProps
    {
        public string url;
        public bool isLoop;
        public string syncChannel;
    }

    [Serializable]
    public class UnityVideoState
    {
        public bool isPlaying;
        public float positionMillis;
        public float durationMillis;
    }

    public class MediaVideo : TextureComponentBase<MediaVideoProps>, INeedsVerticalFlip, IMediaPlaybackControls, IGetState<UnityVideoState>
    {
        public static RecentList recentSyncs = new RecentList(1);

        public IVideoPlayer videoPlayer;

        #region [ fields ]
        Action _onEnd;
        #endregion

        static public void SyncChannel(string syncChannel)
        {
            if (syncChannel != null && syncChannel != "" && !recentSyncs.IsInList(syncChannel))
            {
                // sync all videos in channel
                foreach (var mv in ComponentManager.GetAll<MediaVideo>())
                {
                    if (mv.props.syncChannel == syncChannel)
                    {
                        mv.Rewind(false);
                        mv.Play();
                    }
                }

                recentSyncs.Add(syncChannel);
            }
        }

        private void Start()
        {
            videoPlayer = ComponentManager.Add<IVideoPlayer>(gameObject);
            videoPlayer.Hint = "mf";

            OnPropsChanged += (s, e) =>
            {
                foreach (var prop in e.PropsChanged)
                {
                    if (prop == "url" || prop == "all")
                    {
                        videoPlayer.Url = props.url;
                    }
                    if (prop == "isLoop" || prop == "all")
                    {
                        videoPlayer.Loop = props.isLoop;
                    }
                }
            };

            volume = 1;

            SetProps(props);
        }

        public void Play()
        {
            if (videoPlayer == null || !videoPlayer.CanPlay())
            {
                StartCoroutine(CoPlayWhenAvailable());
            }
            else
            {
                videoPlayer.Loop = Props.isLoop;
                videoPlayer.Play();
            }
        }

        IEnumerator CoPlayWhenAvailable()
        {
            while (videoPlayer == null)
                yield return null;

            videoPlayer.Loop = Props.isLoop;

            if (videoPlayer != null)
                while (!videoPlayer.CanPlay())
                    yield return null;
            videoPlayer.Play();
        }

        public void Stop()
        {
            videoPlayer.Stop();
        }

        public void Close()
        {
            videoPlayer.Stop();
        }

        public void Rewind(bool pause = false)
        {
            if (videoPlayer != null)
                videoPlayer.Rewind(pause);
        }

        public void Pause()
        {
            if (videoPlayer != null)
                videoPlayer.Pause();
        }

        public float GetPositionMillis()
        {
            if (videoPlayer != null)
                return videoPlayer.GetCurrentTimeMs();

            return -1;
        }

        public float GetDurationMillis()
        {
            if (videoPlayer != null)
                return videoPlayer.DurationMs;

            return -1;
        }

        public void Seek(float millis)
        {
            if (videoPlayer != null)
                videoPlayer.Seek(millis);
        }

        public void OnEnd(Action onEnd)
        {
            _onEnd = onEnd;
        }

        public override Texture Texture
        {
            get
            {
                return videoPlayer.Texture;
            }
        }

        public bool NeedsVerticalFlip
        {
            get
            {
                if (videoPlayer == null)
                    return true;
                return videoPlayer.NeedsVerticalFlip;
            }
        }

        public bool IsPlaying()
        {
            if (videoPlayer != null)
                return videoPlayer.IsPlaying();
            return false;
        }

        public override void OnUpdate()
        {
            if (videoPlayer == null)
                return;

            texture = videoPlayer.Texture;
            if (videoPlayer.HasFinished())
            {
                if (_onEnd != null)
                    _onEnd();

                // sync channel
                var syncChannel = props.syncChannel;
                if (syncChannel != null && syncChannel != "")
                    SyncChannel(syncChannel);

                _onEnd = null;
            }

            if (videoPlayer != null)
                videoPlayer.Volume = volume;
        }

        public object GetState()
        {
            return new UnityVideoState {
                isPlaying = IsPlaying(),
                positionMillis = GetPositionMillis(),
                durationMillis = GetDurationMillis()
            };
        }
    }

}