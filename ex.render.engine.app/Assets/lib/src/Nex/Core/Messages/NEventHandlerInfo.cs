using System;

namespace Nex.Core
{
    internal class NEventHandlerInfo
    {       
        #region [ PROPERTIES ]
        public object Source { get; internal set;}
        public EventHandler<NMessage> Handler { get; internal set; }
        #endregion

        #region [ CONSTRUCTOR ]
        public NEventHandlerInfo(object source, EventHandler<NMessage> handler)
        {
            Source = source;
            Handler = handler;
        }
        #endregion
    }
}
