using System;

namespace Nexwarp.Network
{
    public interface IHubServerConnectionListener
    {
        IHubServerConnectionListener OnOpen(Action<IHubServerConnection> action);
        IHubServerConnectionListener OnClose(Action<IHubServerConnection, string> action);
        IHubServerConnectionListener OnException(Action<IHubServerConnection, Exception> action);
    }
}
