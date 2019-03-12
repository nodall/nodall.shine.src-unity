using System;


namespace Nex.Core
{
    public abstract class NPropertyMessageDescriptor: NMessageDescriptor
    {
        #region [ CONST ]
        public const string OLD_VALUE = "OldValue";
        public const string NEW_VALUE = "NewValue";
        public const string INDEX_VALUE = "Index";
        #endregion

        #region [ PROPERTIES ]
        public Type PropertyType { get { return Arguments[0].Type; } }
        public Type[] Index { get ; internal set; }
        #endregion

        #region [ CONSTRUCTOR ]
        public NPropertyMessageDescriptor(string name, Type valueType, Type[] index = null)
            : base(
             new NMessageArgument(OLD_VALUE, valueType),
             new NMessageArgument(NEW_VALUE, valueType),
             new NMessageArgument(INDEX_VALUE, typeof(Object[])))
        {			
            Index = index;
        }
        #endregion		
    }

    public class NPropertyMessageDescriptor<T> : NPropertyMessageDescriptor
    {
        #region [ CONSTRUTOR ]
        public NPropertyMessageDescriptor(string name, Type[] index = null) :
            base(name, typeof(T), index)
        {
        }
        #endregion
    }
}
