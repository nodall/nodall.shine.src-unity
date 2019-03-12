using System;


namespace Nex.Core
{
    public abstract class NPropertyMessage:NMessage
    {
        #region [ PORPERTIES ]
        public object OldValue { get { return this[NPropertyMessageDescriptor.OLD_VALUE]; } }
        public object NewValue { get { return this[NPropertyMessageDescriptor.NEW_VALUE]; } }
        public object[] Index { get; internal set; }
        public new NPropertyMessageDescriptor Descriptor { get { return (NPropertyMessageDescriptor)base.Descriptor; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NPropertyMessage(object source, NPropertyMessageDescriptor descriptor, object oldValue, object newValue, object[] index = null)
            : base(source, descriptor, oldValue, newValue, index)
        {           
            if (index != null)
            {
                if (index.Length != Descriptor.Index.Length)
                    throw  new Exception("Index Parameters not match");

                for (int i = 0; i < index.Length; i++)
                    if (index[i].GetType() != Descriptor.Index[i])
                        throw new Exception("Index Parameters not match");
            }

            Index = index;
        }
        #endregion
    }

    public class NPropertyMessage<T> : NPropertyMessage
    {
        #region [ PORPERTIES ]
        public new T OldValue { get { return (T)base.OldValue; } }
        public new T NewValue { get { return (T)base.NewValue; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NPropertyMessage(object source, NPropertyMessageDescriptor<T> descriptor, T oldValue, T newValue, object[] index = null)
            : base(source, descriptor, oldValue, newValue, index)
        {
        }
        #endregion
    }
}
