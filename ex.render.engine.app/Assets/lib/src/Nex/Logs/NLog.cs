using System;
using System.Linq;

using Nex.Core;

namespace Nex.Logs
{
    public class NLog: NObservable, ILog
    {
        #region [ SINGLETON ]
        private static NLog instance;
        private NLog() { }
        public static NLog Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NLog();
                }
                return instance;
            }
        }        
        #endregion

        #region [Static Methods ]
        public static string Convert(DateTime date)
        {
			return string.Format("{0:dd/MM/yy HH:mm:ss.fff}", date);
        }
        public static string Convert(NLogType type)
        {
            var max = Enum.GetNames(typeof(NLogType)).Max((a) => a.Length);
            return string.Format("{0}", type.ToString().PadRight(max, '.'));
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public void Write(string format, params object[] args)
        {
            NotifyMessage(new NMessage(this, NMessages.LogMessage, new NLogEventArgs( DateTime.Now, NLogType.Info, string.Format(format, args))));
        }
        public void Write(NLogType type, string format, params object[] args)
        {
            NotifyMessage(new NMessage(this, NMessages.LogMessage, new NLogEventArgs(DateTime.Now, type, string.Format(format, args))));
        }
        public void Write(Exception ex, string format = "", params object[] args)
        {
            NotifyMessage(new NMessage(this, NMessages.LogMessage, new NLogEventArgs(DateTime.Now, NLogType.Exception, string.Format(format, args), ex)));
        }
        #endregion
    }
}
