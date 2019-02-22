namespace Nexwarp.Network
{
    public interface IHubPublisher
    {
        void Publish(HubMessage msg);
    }
}
