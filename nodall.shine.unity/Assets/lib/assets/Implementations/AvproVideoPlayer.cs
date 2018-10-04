using UnityEngine;
using System.Collections;
using nexcode.nwcore;
using RenderHeads.Media.AVProVideo;
using System;

public class AvproVideoPlayer : MediaPlayer, IVideoPlayer
{
    private void Awake()
    {
        if (hint == "ds")
            PlatformOptionsWindows.videoApi = RenderHeads.Media.AVProVideo.Windows.VideoApi.DirectShow;
        else
            PlatformOptionsWindows.videoApi = RenderHeads.Media.AVProVideo.Windows.VideoApi.MediaFoundation;
    }

    string hint;

    public float Opacity { get; set; }
    public string Hint { get { return hint; }
        set
        {
            hint = value;
            if (hint == "ds")
                PlatformOptionsWindows.videoApi = RenderHeads.Media.AVProVideo.Windows.VideoApi.DirectShow;
            else
                PlatformOptionsWindows.videoApi = RenderHeads.Media.AVProVideo.Windows.VideoApi.MediaFoundation;
        }
    }

    float IVideoPlayer.DisplayFrameRate
    {
        get
        {
            if (Info != null)
                return Info.GetVideoDisplayRate();
            return -1;
        }
    }

    float IVideoPlayer.DurationMs
    {
        get
        {
            if (Info != null)
                return Info.GetDurationMs();
            return -1;
        }
    }

    float IVideoPlayer.FrameRate
    {
        get
        {
            if (Info != null)
                return Info.GetVideoFrameRate();
            return -1;
        }
    }

    bool IVideoPlayer.HasAudio
    {
        get
        {
            if (Info != null)
                return Info.HasAudio();
            return false;
        }
    }

    bool IVideoPlayer.HasVideo
    {
        get
        {
            if (Info != null)
                return Info.HasVideo();
            return false;
        }
    }

    bool IVideoPlayer.IsMuted
    {
        get
        {
            if (Control != null)
                return Control.IsMuted();
            return false;
        }

        set
        {
            m_Muted = value;
            if (Control != null)
                Control.MuteAudio(value);
        }
    }

    bool IVideoPlayer.Loop
    {
        get
        {
            if (Control != null)
                return Control.IsLooping();
            return false;
        }
        set
        {
            m_Loop = value;
            if (Control != null)
                Control.SetLooping(value);
        }
    }

    float IVideoPlayer.PlaybackRate
    {
        get
        {
            if (Control != null)
                return Control.GetPlaybackRate();
            return 1;
        }

        set
        {
            m_PlaybackRate = value;
            if (Control != null)
                Control.SetPlaybackRate(value);
        }
    }

    Texture ITexture.Texture
    {
        get
        {
            if (TextureProducer != null)
                return TextureProducer.GetTexture();
            return null;
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
            if (Info != null)
                return Info.GetCurrentVideoTrackBitrate();
            return -1;
        }
    }

    string IVideoPlayer.VideoDescription
    {
        get
        {
            if (Info != null)
                return Info.GetPlayerDescription();
            return "";
        }
    }

    float IVideoPlayer.VideoHeight
    {
        get
        {
            if (Info != null)
                return Info.GetVideoHeight();
            return -1;
        }
    }

    float IVideoPlayer.VideoWidth
    {
        get
        {
            if (Info != null)
                return Info.GetVideoWidth();
            return -1;
        }
    }

    float IVideoPlayer.Volume
    {
        get
        {
            return Control == null ? 1 : Control.GetVolume();
        }

        set
        {
            m_Volume = value;
            if (Control != null)
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

    bool IVideoPlayer.CanPlay()
    {
        if (Control != null)
            return Control.CanPlay();
        return false;
    }

    bool IVideoPlayer.IsPlaying()
    {
        if (Control != null)
            return Control.IsPlaying() && !Control.IsPaused();
        return false;
    }

    float IVideoPlayer.GetCurrentTimeMs()
    {
        if (Control != null)
            return Control.GetCurrentTimeMs();
        return -1;
    }

    void IVideoPlayer.Load()
    {
        OpenVideoFromFile(m_VideoLocation, m_VideoPath, m_AutoStart);
    }

    void IVideoPlayer.Seek(float ms)
    {
        if (Control != null)
            Control.Seek(ms);
    }

    void IVideoPlayer.Unload()
    {
        CloseVideo();
    }

    bool IVideoPlayer.HasFinished()
    {
        return Control != null && (!Control.IsLooping() && Control.CanPlay() && Control.IsFinished())
                    || (Control.GetCurrentTimeMs() > Info.GetDurationMs());
    }

}
