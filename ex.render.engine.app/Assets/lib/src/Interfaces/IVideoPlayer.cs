using System;

namespace nexcode.nwcore
{
    public interface IVideoPlayer : ITexture, INeedsVerticalFlip
    {        
        string Url { get; set; }
        float Volume { get; set; }
        float PlaybackRate { get; set; }
        bool IsMuted { get; set; }
        bool Loop { get; set; }

        void Load();
        void Play();
        void Pause();
        void Stop();
        void Rewind(bool pause);
        void Unload();
        bool HasFinished();

        bool CanPlay();
        bool IsPlaying();

        void Seek(float ms);
        float GetCurrentTimeMs();

        /* Info */
        float DurationMs { get; }
        float VideoWidth { get; }
        float VideoHeight { get; }
        float FrameRate { get; }
        float DisplayFrameRate { get; }
        string Hint { get; set; }
        bool HasVideo { get; }
        bool HasAudio { get; }
        float VideoBitrate { get; }
        string VideoDescription { get; }
    }
}