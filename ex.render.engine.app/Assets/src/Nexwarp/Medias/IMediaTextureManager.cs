using Nexwarp.Medias;
using System;

namespace Nexwarp.Medias
{
    public interface IMediaTextureManager
    {
        TimeSpan TransitionInterval { get; set; }

        object CurrentOverlay { get; }
        MediaTextureBase CurrentMedia { get; }

        void ShowOverlay(object Texture);
        void HideOverlay();

        void ShowMediaTexture(MediaTextureBase media);
        void Blackout(bool value);
    }
}
