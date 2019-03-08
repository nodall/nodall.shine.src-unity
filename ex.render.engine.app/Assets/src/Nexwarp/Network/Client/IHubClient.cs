namespace Nexwarp.Network
{
    public interface IHubClient : IHubSubscriber, IHubPublisher
    {
        IHubClientConnection Connect(string url, string sourceType = "Unknow");      
        IStreamCollectionTransfer Transfer(params StreamTransferModel[] files);
        void Close();
    }
}
