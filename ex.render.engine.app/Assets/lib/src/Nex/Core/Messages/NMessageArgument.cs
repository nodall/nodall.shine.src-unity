using System;

namespace Nex.Core
{
    public class NMessageArgument: IName
    {
        #region [ FIELDS ]
        string _name;
        Type _type;
        #endregion

        #region [ PROPERTIES ]
        public string Name { get { return _name; } }
        public Type Type { get { return _type; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NMessageArgument(string name, Type type)
        {
            _name = name;
            _type = type;
        }
        #endregion

        #region [ ToString() ]
        public override string ToString()
        {
            return String.Format("['{0}', {1}]", _name, Type);
        }
        #endregion
    }
    public class NMessageArgument<T> : NMessageArgument
    {
        #region [ CONSTRUCTOR ]
        public NMessageArgument(string name)
            :base(name, typeof(T))
        {
        }
        #endregion
    }
}
