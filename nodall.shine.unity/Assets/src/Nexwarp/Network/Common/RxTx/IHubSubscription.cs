using Newtonsoft.Json.Linq;
using System;

namespace Nexwarp.Network
{
    public interface IHubSubscription
    {
        IHubSubscription On(string message, Action<JToken> action);
        IHubSubscription On<T>(string message, Action<T> action);
    }
}
