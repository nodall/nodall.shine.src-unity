using System;
using System.Collections.Generic;
using System.Linq;

namespace Nex.Core
{
    public class NObservable : IObservable
    {
        #region [ Messages ]
        static public NMessageDescriptor AnyMessage = new NMessageDescriptor();
        #endregion

        #region [ FIELDS ]
        object _parent;
        List<object> _children;
        Dictionary<NMessageDescriptor, List<EventHandler<NMessage>>> _handlers = new Dictionary<NMessageDescriptor, List<EventHandler<NMessage>>>();
        List<EventHandler<NMessage>> _disabledEventHandlers = new List<EventHandler<NMessage>>();
        #endregion

        #region [ PROPERTIES ]
        public object Parent
        {
            get { return _parent; }
            set
            {
                var oldParent = _parent;
                if (oldParent is NObservable)
                    (oldParent as NObservable)._children.Remove(this);

                var newParent = value;
                if (newParent is NObservable && (newParent as NObservable).Children.Contains(this))
                    throw new Exception("CRITICAL EXCEPTION. Parent can't contains this child.");

                _parent = newParent;
                if (newParent is NObservable && !(newParent as NObservable).Children.Contains(this))
                    (newParent as NObservable)._children.Add(this);
            }
        }
        public object[] Children { get { return _children.ToArray(); } }
        public bool IsNotifyParendEnabled { get; set; }
        public bool IsNotifyEnabled { get; set; }
        public bool IsNotifyOnlyNewValueEnabled { get; set; }
        public object Root
        {
            get
            {
                var parent = _parent;

                while ((parent is NObservable))
                {
                    if ((parent as NObservable).Parent != null)
                        parent = (parent as NObservable).Parent;
                    else
                        break;
                }

                return parent;
            }
        }
        #endregion

        #region [ Constructor ]
        public NObservable(object parent = null)
        {
            // PRECONDITION 
            if (parent is NObservable && 
                (parent as NObservable).Children.Contains(this))
                throw new Exception("Parent has already this child");

            _parent = parent;

            if (_parent is NObservable)
                (_parent as NObservable)._children.Add(this);

            _children = new List<object>();
            IsNotifyParendEnabled = true;
            IsNotifyEnabled = true;
        }
        #endregion

        void Invoke(EventHandler<NMessage> handler, NMessage e)
        {
           if (!_disabledEventHandlers.Contains(handler))
                handler(this, e);
        }

        #region [ EventHandler Methods ]
        public NMessageDescriptor[] GetMessages()
        {
            return NMessageDescriptor.Get(this.GetType());
        }
        public void EnableEventHandler(EventHandler<NMessage> handler)
        {
            if (_disabledEventHandlers.Contains(handler))
                _disabledEventHandlers.Remove(handler);
        }
        public void DisableEventHandler(EventHandler<NMessage> handler)
        {
            if (!_disabledEventHandlers.Contains(handler))
                _disabledEventHandlers.Add(handler);
        }
        public bool ContainsHandler(NMessageDescriptor descriptor, EventHandler<NMessage> handler)
        {
            if (!_handlers.ContainsKey(descriptor))
                return false;

            var result = from item in _handlers[descriptor] where item == handler select item;

            return result.Count() != 0;
        }
      
        public void AddEventHandler(NMessageDescriptor descriptor, EventHandler<NMessage> handler)
        {
            if (!_handlers.ContainsKey(descriptor))
                _handlers.Add(descriptor, new List<EventHandler<NMessage>>());

            if (ContainsHandler(descriptor, handler))
                throw new Exception("Handler exists");

            _handlers[descriptor].Add(handler);
        }
        public void RemoveEventHandler(NMessageDescriptor descriptor, EventHandler<NMessage> handler)
        {
            if (!_handlers.ContainsKey(descriptor))
                throw new Exception(descriptor + " not found");

            var result = (from item in _handlers[descriptor] where item == handler select item).ToArray();
            foreach (var item in (EventHandler<NMessage>[])result.Clone())
                _handlers[descriptor].Remove(item);

            if (_handlers[descriptor].Count == 0)
                _handlers.Remove(descriptor);
        }
        #endregion

        #region [ public Methods  ]
        public void NotifyProperty<T>(IObservable source, ref T property, T newValue, NPropertyMessageDescriptor<T> descriptor, object[] index = null)
        {
            if (IsNotifyOnlyNewValueEnabled && 
                property != null &&
                property.Equals(newValue))
                return;

            var oldValue = property;
            property = newValue;
            source.NotifyMessage(new NPropertyMessage<T>(this, descriptor, oldValue, property, index));
        }
        public void NotifyMessage(NMessage msg)
        {
            if (!IsNotifyEnabled)
                return;

            if (_handlers.ContainsKey(NObservable.AnyMessage))
            {
                int count = _handlers[NObservable.AnyMessage].Count;
                for (int i = 0; i < count; i++)
                    Invoke(_handlers[NObservable.AnyMessage][i], msg);
            }
            if (_handlers.ContainsKey(msg.Descriptor))
            {
                int count = _handlers[msg.Descriptor].Count;
                for (int i = 0; i < count; i++)
                    if (i < _handlers[msg.Descriptor].Count)
                        Invoke(_handlers[msg.Descriptor][i], msg);
            }

            if (Parent != null && Parent is NObservable && IsNotifyParendEnabled)
                (Parent as NObservable).NotifyMessage(msg);

        }
        public T GetFirstParent<T>() 
        {
            var parent = _parent;

            while(!(parent is T) && parent is NObservable && parent != null)
                parent = (parent as NObservable).Parent;
            
            return (T)parent;
        }
        
        public void NotifyMessageToChildren(NMessage msg)
        {         
            foreach (var child in Children)
            {
                if (child is NObservable)
                {
                    if ((child as NObservable)._handlers.ContainsKey(msg.Descriptor))
                    {
                        for (int i = 0; i < (child as NObservable)._handlers[msg.Descriptor].Count; i++)
                            (child as NObservable)._handlers[msg.Descriptor][i]((child as NObservable), msg);
                    }
                    (child as NObservable).NotifyMessageToChildren(msg);
                }
            }
        }
        #endregion
    }
}
