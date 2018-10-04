namespace Nexwarp.Network
{
    public class HubMessage
    {
        string Channel { get; set; }
        string Command { get; set; }
        object Args { get; set; }
    }
}
