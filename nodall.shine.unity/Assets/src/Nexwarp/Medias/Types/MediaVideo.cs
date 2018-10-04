using System;

namespace Nexwarp.Medias
{
    public class MediaVideo : MediaTextureBase
    {
        #region [ properties ]
        public float AudioVolume { get; set; }
        public float AudioBalance { get; set; }
        public virtual TimeSpan Position { get; set; }
        #endregion

        #region [ constructor ]
        public MediaVideo(string url, bool isLoop)
        { }
        #endregion 
    }
    
}
