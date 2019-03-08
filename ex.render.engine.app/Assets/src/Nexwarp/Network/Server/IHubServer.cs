using System.Collections.Generic;

namespace Nexwarp.Network
{
    public interface IHubServer : IHubSubscriber, IHubPublisher
    {
        #region [ properties ]
        bool IsListening { get; set; }
        List<IHubServerConnection> Connections { get; }
        #endregion 

        #region [ public methods ]
        IHubServerConnectionListener Start(HubServerSettings settings);
        void Stop();
        void Dispose();
        #endregion

        #region [ File ]
        IStreamCollectionTransfer Files();
        #endregion
    }
}
