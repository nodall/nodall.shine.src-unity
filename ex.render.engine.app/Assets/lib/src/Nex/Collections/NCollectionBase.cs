using System.Collections;
using Nex.Core;

namespace Nex.Collections
{
    public abstract class NCollectionBase : NObservable, IEnumerable
    {
        #region [ Messages ]
        static public NMessageDescriptor PreviewAddMessage = new NMessageDescriptor(new NMessageArgument("Item", typeof(object)));
        static public NMessageDescriptor AddMessage = new NMessageDescriptor(new NMessageArgument("Item", typeof(object)));
        static public NMessageDescriptor PreviewRemoveMessage = new NMessageDescriptor(new NMessageArgument("Item", typeof(object)));
        static public NMessageDescriptor RemoveMessage = new NMessageDescriptor(new NMessageArgument("Item", typeof(object)));
        static public NMessageDescriptor ClearMessage = new NMessageDescriptor();
        static public NMessageDescriptor MoveBeforeMessage = new NMessageDescriptor(
            new NMessageArgument("ItemToMove", typeof(object)),
            new NMessageArgument("Item", typeof(object)));
        static public NMessageDescriptor MoveAfterMessage = new NMessageDescriptor(
            new NMessageArgument("ItemToMove", typeof(object)),
            new NMessageArgument("Item", typeof(object)));
        #endregion

        public NCollectionBase()
        {
        }
        public NCollectionBase(object parent)
            : base(parent)
        {
        }

        public IEnumerator GetEnumerator()
        {
            return OnGetEnumerator();
        }
        public abstract IEnumerator OnGetEnumerator();
        public virtual void MoveBefore(object itemToMove, object item)
        {
            NotifyMessage(new NMessage(this, MoveBeforeMessage, itemToMove, item));
        }
        public virtual void MoveAfter(object itemToMove, object item)
        {
            NotifyMessage(new NMessage(this, MoveAfterMessage, itemToMove, item));
        }

    }
}
