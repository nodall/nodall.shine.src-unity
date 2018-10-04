using Nexwarp.Medias;
using System;

namespace Nexwarp.Medias
{
    public interface IMediaAudioManager
    {
        TimeSpan MuteInterval { get; set; }
        MediaAudioBase[] Audios { get; }

        void Add(MediaAudioBase audio);
        void Remove(MediaAudioBase audio);
        void Mute(bool value);
    }
}
