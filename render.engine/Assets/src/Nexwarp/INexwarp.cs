using Nexwarp.Medias;
using Nexwarp.Network;

namespace Nexwarp
{
    public interface INexwarp
    {
         IMediaTextureManager Texture { get; }
         IMediaAudioManager Audio { get; }
         IHubServer Hub { get; set; }
    }
}
