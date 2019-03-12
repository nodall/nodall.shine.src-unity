using System.Collections.Generic;
using System.Linq;

namespace Nex.Collections
{
    public class NUnion<TKeyValue,TValueKey>
    {
        public TKeyValue[] Keys { get { return _keyValue.Keys.ToArray(); } }
        public TValueKey[] Values { get { return _keyValue.Values.ToArray(); } }

        Dictionary<TKeyValue, TValueKey> _keyValue = new Dictionary<TKeyValue, TValueKey>();
        Dictionary<TValueKey, TKeyValue> _valueKey = new Dictionary<TValueKey, TKeyValue>();

        public void Add(TKeyValue key, TValueKey value)
        {
            _keyValue.Add(key, value);
            _valueKey.Add(value, key);
        }
        public void Add( TValueKey value, TKeyValue key)
        {
            Add(key, value);
        }
        public void Remove(TKeyValue key)
        {
            var value = _keyValue[key];
            _keyValue.Remove(key);
            _valueKey.Remove(value);
        }
        public void Remove(TValueKey value)
        {
            Remove(_valueKey[value]);
        }
        public void Clear()
        {
            _keyValue.Clear();
            _valueKey.Clear();
        }
        public TValueKey this[ TKeyValue key]
        {
            get { return _keyValue[key]; }
        }
        public TKeyValue this [TValueKey value]
        {
            get { return _valueKey[value]; }
        }
    }
}
