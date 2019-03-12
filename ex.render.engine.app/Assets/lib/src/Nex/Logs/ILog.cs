using System;
namespace Nex.Logs
{
    public interface ILog
    {
        void Write(NLogType type, string format, params object[] args);
        void Write(Exception ex, string format = "", params object[] args);
        void Write(string format, params object[] args);
    }
}
