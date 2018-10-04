using System;

namespace Nexwarp.Medias
{
    public abstract class MediaBase
    {
        #region [ events ]
        public event EventHandler<MediaStateEventArgs> EventStateChanged;
        #endregion

        #region [ fields ]
        public MediaState _state;
        #endregion

        #region [ properties ]
        public MediaState State
        {
            get { return _state; }
            set
            {
                var oldValue = _state;
                _state = value;
                if (EventStateChanged != null)
                    EventStateChanged(this, new MediaStateEventArgs(oldValue, _state));
            }
        }
        public float Opacity { get; set; }
        #endregion

        #region [ public methods ]
        public virtual void Load()
        { }
        public virtual void Play()
        { }
        public virtual void Pause()
        { }
        public virtual void Stop()
        { }
        #endregion
    }
}
