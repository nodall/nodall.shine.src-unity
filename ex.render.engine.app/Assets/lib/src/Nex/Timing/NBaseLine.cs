
using Nex.Core;
using Nex.Collections;

namespace Nex.Timing
{
    public abstract class NBaseLine<T>:NList<T>, IRunnable
    {
        #region [ MESSAGE ]
        public static NPropertyMessageDescriptor<NClock> ClockProperty = new NPropertyMessageDescriptor<NClock>("Clock");
        #endregion

        #region [ FIELDS ]
        NClock _clock;
        #endregion

        #region [ IClock IMPLEMENTS ]
        public NClock Clock
        {
            get { return _clock; }
            set
            {
                var oldValue = _clock;
                _clock = value;
                NotifyMessage(new NPropertyMessage<NClock>(this, ClockProperty, oldValue, value));
            }
        }
        #endregion

        #region [ CONSTRUCTOR ]
        public NBaseLine(object parent = null)
            :base(parent)
        {
            _clock = new NClock(this);
            AddEventHandler(NList<T>.AddMessage,  Message_Received);
            AddEventHandler(NList<T>.RemoveMessage, Message_Received);
        }
        #endregion

        #region [ MESSAGE_RECIEVED ]
        void Message_Received(object sender, NMessage e)
        {
            if (e.Descriptor == NList<T>.AddMessage && e.Source == this)
                OnItemAdded(e.Get<T>("Value"));

            if (e.Descriptor == NList<T>.RemoveMessage && e.Source == this)
                OnItemRemoved(e.Get<T>("Value"));
        }
        #endregion

        public abstract void Run();
        protected virtual void OnItemAdded(T item) { }
        protected virtual void OnItemRemoved(T item) { }
    }
}
