using System;
namespace Nex.Core
{
    public interface IObservable
    {
        object[] Children { get; }
        bool IsNotifyParendEnabled { get; set; }
        object Parent { get; set; }
        object Root { get; }

        void AddEventHandler(NMessageDescriptor descriptor, EventHandler<NMessage> handler);
        void RemoveEventHandler(NMessageDescriptor descriptor, EventHandler<NMessage> handler);
        bool ContainsHandler(NMessageDescriptor descriptor, EventHandler<NMessage> handler);
        void EnableEventHandler(EventHandler<NMessage> handler);
        void DisableEventHandler(EventHandler<NMessage> handler);

        T GetFirstParent<T>();
        NMessageDescriptor[] GetMessages();
        
        void NotifyMessage(NMessage msg);
        void NotifyMessageToChildren(NMessage msg); 
    }
}
