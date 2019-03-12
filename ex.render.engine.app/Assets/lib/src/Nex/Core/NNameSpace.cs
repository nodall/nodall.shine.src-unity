using System;
using System.Collections.Generic;
using System.Linq;


namespace Nex.Core
{
    public class NNameSpace:IComparable, IEnumerable<string>
    {
        #region [ STATIC ]
        static public bool IsCaseSensitive { get; set; }
        public static NNameSpace Empty
        {
            get
            {
                return new NNameSpace();
            }
        }
        static NNameSpace()
        {
            IsCaseSensitive = true;
        }
        public static NNameSpace Parse(String nameSpace, string separator)
        {
            if (nameSpace == null)
                throw new ArgumentNullException();

            string[] split = nameSpace.Replace(separator, new String((char)0, 1)).Split((char)0);

            return new NNameSpace(split);
        }
        #endregion 

        #region [ PROPERTIES ]
        String[] _items;

        public string[] Items { get { return (string[])_items.Clone(); } }
        public bool IsEmpty { get { return  _items.Count() == 0; } }
        public bool IsPattern { get { return _items.Count(x => x.Contains("*")) != 0; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NNameSpace(params String[] args)
        {
            if (args != null)
            {
                var list = new List<string>();
                foreach (var arg in args)
                {
                    if (!String.IsNullOrEmpty(arg))
                        list.Add(arg);
                }
                _items = list.ToArray();
            }
            else
                _items = new String[] { };
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public int Count() { return _items.Count(); } 
        public string First() { return _items[0]; } 
        public string Last() { return _items[_items.Length - 1]; } 
        public NNameSpace Next()
        {
            // {a.b.c.d}.Next() = {b.c.d}
            if (Count() == 0 || Count() == 1)
                return NNameSpace.Empty;            

            var newItems = new String[_items.Count() - 1];
            Array.Copy(_items, 1,  newItems, 0, _items.Count() - 1); 
            return new NNameSpace(newItems);
        }
        public NNameSpace Previous()
        {
            // {a.b.c.d}.Previous() = {a.b.c}
            if (Count() == 0 || Count() == 1)
                return NNameSpace.Empty;

            var newItems = new String[_items.Count() - 1];
            Array.Copy(_items, 0, newItems, 0, _items.Count() - 1);
            return new NNameSpace(newItems);
        }
        public string ToString(string separator)
        {
            String str = "";

            if (Count() != 0)
            {
                str = _items.Aggregate((workingSentence, next) => workingSentence + separator + next);

                if (str.EndsWith(separator))
                    str = str.Remove(str.Length - separator.Length, separator.Length);
            }
            return str;
        }
        #endregion
       
        #region [ OVERRIDE ]  
        public override string ToString()
        {
            return ToString(".");
        }
        public override bool Equals(object obj)
        {
            if (!(obj is NNameSpace))
                throw new ArgumentException("NameSpace type is required");
            return this == (NNameSpace)obj;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region [ OPERATORS ]
        public string this[int index]
        {
            get { return _items[index]; }
        }
        public static NNameSpace operator +(NNameSpace a, NNameSpace b)
        {
            // {a.b.c.d} + {e.f} = {a.b.c.d.e.f}
            return new NNameSpace(a._items.Concat(b._items).ToArray());
        }
        public static NNameSpace operator +(NNameSpace a, string b)
        {
            // {a.b.c.d} + e = {a.b.c.d.e}
            return new NNameSpace(a._items.Concat(new string[] { b }).ToArray());
        }
        public static NNameSpace operator +(string a, NNameSpace b)
        {
            // a + {b.c.d} = {a.b.c.d}
            return new NNameSpace(new string[] { a }.Concat(b._items).ToArray());
        }
        public static NNameSpace operator -(NNameSpace a, NNameSpace b)
        {
            // {a.b.c.d} - {a.b.c} = {d}
            // {a.b.c.d} - {b.c} = {}

            for (int i = 0; i < b._items.Count(); i++)
            {
                if (IsCaseSensitive && a[i] != b[i] ||
                    !IsCaseSensitive && a[i].ToLower() != b[i].ToLower())
                    return NNameSpace.Empty;
            }

            var length = a._items.Count() - b._items.Count();
            var newItems = new String[length];
            Array.Copy(a._items, b._items.Count(), newItems, 0, length);

            return new NNameSpace(newItems);
        }
        public static NNameSpace operator %(NNameSpace a, NNameSpace b)
        {
            // {a.b.c.d} % {b.c.d} = {a}

            for (int i = 0; i < b._items.Count(); i++)
            {
                var indexA = (a.Count() - 1) - i;
                var indexB = (b.Count() - 1) - i;
                if (IsCaseSensitive && a[indexA] != b[indexB] ||
                    !IsCaseSensitive && a[indexA].ToLower() != b[indexB].ToLower())
                    return NNameSpace.Empty;
            }

            var length = a._items.Count() - b._items.Count();
            var newItems = new String[length];
            Array.Copy(a._items, 0, newItems, 0, length);

            return new NNameSpace(newItems);
        }
       
        public static bool operator ==(NNameSpace a, NNameSpace b)
        {
            if (a.IsPattern == b.IsPattern)
            {
                if (a.Count() != b.Count())
                    return false;

                for (int i = 0; i < b._items.Count(); i++)
                {
                    if (IsCaseSensitive && a[i] != b[i] ||
                        !IsCaseSensitive && a[i].ToLower() != b[i].ToLower())
                        return false;
                }
                return true;
            }
            else
            {
                NNameSpace pattern = a.IsPattern ? a : b;
                NNameSpace nameSpace = a.IsPattern ? b : a;

                if (pattern.Count() > nameSpace.Count())
                    return false;

                for (int i = 0; i < pattern._items.Length; i++)
                {
                    // nameSpace Length is less than pattern
                    if (nameSpace._items.Length < i)
                        return false;

                    var patternNode = pattern._items[i];
                    var nameSpaceNode = nameSpace._items[i];

                    if (patternNode != "*")
                    {
                        if (NNameSpace.IsCaseSensitive)
                        {
                            if (patternNode != nameSpaceNode)
                                return false;
                        }
                        else
                        {
                            if (patternNode.ToLower() != nameSpaceNode.ToLower())
                                return false;
                        }
                    }
                }
                return true;
            }
        }
        public static bool operator !=(NNameSpace a, NNameSpace b)
        {
            return !(a == b);
        }        
        #endregion

        #region [ IComparable ]
        public int CompareTo(object obj)
        {
            if (!(obj is NNameSpace))
                throw new ArgumentException("NameSpace type is required");

            if (IsCaseSensitive)
                return this.ToString().ToLower().CompareTo(((NNameSpace)obj).ToString().ToLower());
            else
                return this.ToString().CompareTo(((NNameSpace)obj).ToString());
        }
        #endregion

        public IEnumerator<string> GetEnumerator()
        {
            return _items.ToList().GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
