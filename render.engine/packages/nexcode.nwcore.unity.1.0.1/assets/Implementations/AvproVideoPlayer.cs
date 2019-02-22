using UnityEngine;
using System.Collections;
using nexcode.nwcore;
using RenderHeads.Media.AVProVideo;
using System;

public class AvproVideoPlayer : MediaPlayer, IVideoPlayer
{
    private void Awake()
    {
        PlatformOptionsWindows.videoApi = RenderHeads.Media.AVProVideo.Windows.VideoApi.DirectShow;
    }

    public float Opacity { get; set; }

    float IVideoPlayer.DisplayFrameRate
    {
        get
        {
            return Info.GetVideoDisplayRate();
        }
    }

    float IVideoPlayer.DurationMs
    {
        get
        {
            return Info.GetDurationMs();
        }
    }

    float IVideoPlayer.FrameRate
    {
        get
        {
            return Info.GetVideoFrameRate();
        }
    }

    bool IVideoPlayer.HasAudio
    {
        get
        {
            return Info.HasAudio();
        }
    }

    bool IVideoPlayer.HasVideo
    {
        get
        {
            return Info.HasVideo();
        }
    }

    bool IVideoPlayer.IsMuted
    {
        get
        {
            return Control.IsMuted();
        }

        set
        {
            m_Muted = value;
            Control.MuteAudio(value);
        }
    }

    bool IVideoPlayer.Loop
    {
        get
        {
            return Control.IsLooping();
        }
        set
        {
            m_Loop = value;
            Control.SetLooping(value);
        }
    }

    float IVideoPlayer.PlaybackRate
    {
        get
        {
            return Control.GetPlaybackRate();
        }

        set
        {
            Control.SetPlaybackRate(value);
        }
    }

    Texture ITexture.Texture
    {
        get
        {
            return TextureProducer.GetTexture();
        }
    }

    string IVideoPlayer.Url
    {
        get
        {
            return m_VideoPath;
        }

        set
        {
            m_VideoPath = value;
            OpenVideoFromFile(FileLocation.AbsolutePathOrURL, m_VideoPath, false);
        }
    }

    float IVideoPlayer.VideoBitrate
    {
        get
        {
            return Info.GetCurrentVideoTrackBitrate();
        }
    }

    string IVideoPlayer.VideoDescription
    {
        get
        {
            return Info.GetPlayerDescription();
        }
    }

    float IVideoPlayer.VideoHeight
    {
        get
        {
            return Info.GetVideoHeight();
        }
    }

    float IVideoPlayer.VideoWidth
    {
        get
        {
            return Info.GetVideoWidth();
        }
    }

    float IVideoPlayer.Volume
    {
        get
        {
            return Control.GetVolume();
        }

        set
        {
            m_Volume = value;
            Control.SetVolume(value);
        }
    }

    bool INeedsVerticalFlip.NeedsVerticalFlip
    {
        get
        {
            return TextureProducer.RequiresVerticalFlip();
        }
    }

    float IVideoPlayer.GetCurrentTimeMs()
    {
        return Control.GetCurrentTimeMs();
    }

    void IVideoPlayer.Load()
    {
        OpenVideoFromFile(m_VideoLocation, m_VideoPath, m_AutoStart);
    }

    void IVideoPlayer.Seek(float ms)
    {
        Control.Seek(ms);
    }

    void IVideoPlayer.Unload()
    {
        CloseVideo();
    }

    bool IVideoPlayer.HasFinished()
    {
        return (!Control.IsLooping() && Control.CanPlay() && Control.IsFinished())
                    || (Control.GetCurrentTimeMs() > Info.GetDurationMs());
    }

}
