using System;

namespace Nexwarp.Medias
{
    public class MediaStateEventArgs : EventArgs
    {
        #region [ properties ]
        public MediaState OldValue { get; private set; }
        public MediaState NewValue { get; private set; }
        #endregion

        #region [ constructor ]
        public MediaStateEventArgs(MediaState oldValue, MediaState newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        #endregion
    }

}
