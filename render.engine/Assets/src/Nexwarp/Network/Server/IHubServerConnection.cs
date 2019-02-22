namespace Nexwarp.Network
{
    public interface IHubServerConnection
    {
        object Info { get; set; }
        string SourceType { get; set; }
        void Close(string msg);
    }
}
