using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Nex.Core;

namespace Nex.Collections
{
    public class NList<T>:NCollectionBase, IList<T>
    {
        #region [ FIELDS ]
        List<T> _list = new List<T>();
        #endregion

        #region [ Constructors ]
        public NList(object owner = null)
            : base(owner)
        {
        }
        #endregion

        #region [ IList<T> ]
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (_list.Contains(item))
                throw new ArgumentException("item is already added");

            NotifyMessage(new NMessage(this, PreviewAddMessage, item));
            _list.Insert(index, item);

            if (item is NObservable)
                (item as NObservable).Parent = this;
            NotifyMessage(new NMessage(this, AddMessage, item));
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            NotifyMessage(new NMessage(this, PreviewRemoveMessage, item));
            _list.Remove(item);
            NotifyMessage(new NMessage(this, RemoveMessage, item));
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                var oldValue = this[index];
                RemoveAt(index);
                Insert(index, value);

                if (oldValue is NObservable)
                    (oldValue as NObservable).Parent = null;
            }
        }

        public void Add(T item)
        {
            if (_list.Contains(item))
                throw new ArgumentException("item is already added");

            NotifyMessage(new NMessage(this, PreviewAddMessage, item));
            _list.Add(item);

            if (item is NObservable)
                (item as NObservable).Parent = this;

            NotifyMessage(new NMessage(this, AddMessage, item));
        }

        public void Clear()
        {
            T[] itemsToRemove = _list.ToArray();
            foreach (var item in itemsToRemove)
                Remove(item);

            NotifyMessage(new NMessage(this, ClearMessage));
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            NotifyMessage(new NMessage(this, PreviewRemoveMessage, item));
            bool result = _list.Remove(item);

            if (item is NObservable)
                (item as NObservable).Parent = null;

            NotifyMessage(new NMessage(this, RemoveMessage, item));
            return result;
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }   
        #endregion

        #region [ Generic Methods ]
        public K[] GetItems<K>() where K : T
        {
            var result = from item in _list where item is K select (K)item;
            return result.ToArray();
        }
        public bool Contains<K>() where K : T, IName
        {
            var result = from item in _list where item is K select (K)item;
            return result.Count() != 0;
        }
      
        
        #endregion

        #region [ IName Methods ]
        public K GetItem<K>(string name) where K : T, IName
        {
            var result = from item in _list where item is K && ((K)item).Name == name select (K)item;
            return result.First();
        }
        public K[] GetItems<K>(string name) where K : T, IName
        {
            var result = from item in _list where item is K && ((K)item).Name == name select (K)item;
            return result.ToArray();
        }
        public bool Contains<K>(string name) where K : T, IName
        {
            var result = from item in _list where item is K && ((K)item).Name == name select (K)item;
            return result.Count() != 0;
        }
        #endregion

        public override IEnumerator OnGetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void MoveBefore(T itemToMove, T item )
        {
            if (!(_list.Contains(item) &&
                 _list.Contains(itemToMove)))
                throw new Exception("NList must contains all items");

            _list.Remove(itemToMove);
            int index = _list.IndexOf(item);

            _list.Insert(index, itemToMove);

            base.MoveBefore(itemToMove, item);
        }
        public  void MoveAfter(T itemToMove, T item)
        {
            if (!(_list.Contains(item) &&
                 _list.Contains(itemToMove)))
                throw new Exception("NList must contains all items");

            _list.Remove(itemToMove);
            int index = _list.IndexOf(item);

            if (index + 1 == _list.Count)
                _list.Add(itemToMove);
            else
                _list.Insert(index + 1, itemToMove);
            base.MoveAfter(itemToMove, item);
        }
    }
}
