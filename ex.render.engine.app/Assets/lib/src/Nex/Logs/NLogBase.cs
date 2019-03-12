
using Nex.Core;

namespace Nex.Logs
{
    public abstract class NLogBase
    {
        #region [ PROPERTY ]
        public bool IsOpened { get; internal set; }
        #endregion

        #region [ PUBLIC METODS ]
        public void Open()
        {
            if (!IsOpened)
            {
                OnOpen();
                NLog.Instance.AddEventHandler(NMessages.LogMessage, NObservable_MessageReceived);
                IsOpened = true;
            }
        }
        public void Close()
        {
            if (IsOpened)
            {
                OnClose();
                NLog.Instance.RemoveEventHandler(NMessages.LogMessage, NObservable_MessageReceived);
                IsOpened = false;
            }
        }
        public void Clear() { OnClear(); }
        #endregion

        #region [ ABSTRACT METHODS ]
        protected abstract void OnWrite(NLogEventArgs e);
        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        protected virtual void OnClear() { }
        #endregion

        #region [ PRIVATE METHODS ]
        void NObservable_MessageReceived(object sender, NMessage e)
        {
            OnWrite(e.Get<NLogEventArgs>("Value"));
        }
        #endregion
    }
}
