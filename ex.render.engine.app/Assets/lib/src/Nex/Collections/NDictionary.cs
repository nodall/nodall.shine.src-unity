using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Nex.Core;

namespace Nex.Collections
{

    public class NDictionary<TKey, TValue> : NCollectionBase, IDictionary<TKey, TValue>
    {
        #region [ Messages ]
        static public new NMessageDescriptor PreviewAddMessage = new NMessageDescriptor(
            new NMessageArgument("Key", typeof(TKey)),
            new NMessageArgument("Value", typeof(TValue)));
        static public new NMessageDescriptor AddMessage = new NMessageDescriptor(
            new NMessageArgument("Key", typeof(TKey)), 
            new NMessageArgument("Value", typeof(TValue)));
        static public new NMessageDescriptor PreviewRemoveMessage = new NMessageDescriptor(
           new NMessageArgument("Key", typeof(TKey)),
           new NMessageArgument("Value", typeof(TValue)));
        static public new NMessageDescriptor RemoveMessage = new NMessageDescriptor(
            new NMessageArgument("Key", typeof(TKey)),
            new NMessageArgument("Value", typeof(TValue)));
        #endregion

        #region [ FIELDS ]
        Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();
        List<TValue> _list = new List<TValue>();
        #endregion

        #region [ Constructors ]
        public NDictionary()
        {
        }
        public NDictionary(object owner)
            : base(owner)
        {
        }
        #endregion

        #region [ IDictionary<TKey, TValue> ]
        public void Add(TKey key, TValue value)
        {
            NotifyMessage(new NMessage(this, PreviewAddMessage, key, value));
            NotifyMessage(new NMessage(this, NCollectionBase.AddMessage, value));

            _items.Add(key, value);
            _list.Add(value);

            if (value is NObservable)
                (value as NObservable).Parent = this;

            NotifyMessage(new NMessage(this, AddMessage, key, value));
            NotifyMessage(new NMessage(this, NCollectionBase.AddMessage, value));
        }

        public bool ContainsKey(TKey key)
        {
            return _items.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _items.Keys; }
        }

        public bool Remove(TKey key)
        {
            var value = _items[key];
            NotifyMessage(new NMessage(this, PreviewAddMessage, key, value));
            _items.Remove(key);
            _list.Remove(value);

            if (value is NObservable)
                (value as NObservable).Parent = null;

            NotifyMessage(new NMessage(this, RemoveMessage, key, value));
            NotifyMessage(new NMessage(this, NCollectionBase.RemoveMessage, value));

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _items.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _list; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _items[key];
            }
            set
            {
                var oldValue = _items[key];
                var index = _list.IndexOf(oldValue);
                
                _list.RemoveAt(index);
                _list.Insert(index, value);
                _items.Add(key, value);

                if (oldValue is NObservable)
                    (oldValue as NObservable).Parent = null;

                if (value is NObservable)
                    (value as NObservable).Parent = this;

                NotifyMessage(new NMessage(this, RemoveMessage, key, value));
                NotifyMessage(new NMessage(this, NCollectionBase.RemoveMessage, value));
                NotifyMessage(new NMessage(this, AddMessage, key, value));
                NotifyMessage(new NMessage(this, NCollectionBase.AddMessage, value));
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            TKey[] itemsToRemove = _items.Keys.ToArray();
            foreach (var item in itemsToRemove)
                Remove(item);

            NotifyMessage(new NMessage(this, ClearMessage));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            (_items as IDictionary<TKey,TValue>).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return (_items as IDictionary<TKey, TValue>).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region [ PUBLIC METHODS ]
        public TValue this[int index]
        {
            get { return _list[index]; }
        }
        #endregion

        public override IEnumerator OnGetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        public void MoveBefore(TValue item, TValue itemToMove)
        {
            if (!(_list.Contains((TValue)item) &&
                 _list.Contains((TValue)itemToMove)))
                throw new Exception("NList must contains all items");

            _list.Remove((TValue)itemToMove);
            int index = _list.IndexOf((TValue)item);

            _list.Insert(index, (TValue)itemToMove);
            base.MoveBefore(itemToMove, item);
        }
        public void MoveAfter(TValue item, TValue itemToMove)
        {
            if (!(_list.Contains((TValue)item) &&
                 _list.Contains((TValue)itemToMove)))
                throw new Exception("NList must contains all items");

            _list.Remove((TValue)itemToMove);
            int index = _list.IndexOf((TValue)item);

            if (index + 1 == _list.Count)
                _list.Add((TValue)itemToMove);
            else
                _list.Insert(index + 1, (TValue)itemToMove);
            base.MoveAfter(itemToMove, item);
        }
    }
}
