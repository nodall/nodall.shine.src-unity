using System;

namespace Nexwarp.Network
{
    public interface IHubClientConnection
    {
        IHubClientConnection OnOpen(Action action);
        IHubClientConnection OnClose(Action<string> action);
        IHubClientConnection OnException(Action<Exception> action);
    }
}
