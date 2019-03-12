using System;

namespace Nex.Logs
{
    public class NLogEventArgs:EventArgs
    {
        public DateTime Date { get; private set; }
        public NLogType Type { get; private set; }
        public string Message { get; private set; }
        public Exception  Exception { get; private set; }

        public NLogEventArgs(DateTime date, NLogType type, string message, Exception ex = null)
        {
            Date = date;
            Type = type;
            Message = message;
            Exception = ex;
        }
    }
}
