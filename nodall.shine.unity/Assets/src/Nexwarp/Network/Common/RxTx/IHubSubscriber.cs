namespace Nexwarp.Network
{
    public interface IHubSubscriber
    {
        IHubSubscription Subscribe(string channel);
    }
}
