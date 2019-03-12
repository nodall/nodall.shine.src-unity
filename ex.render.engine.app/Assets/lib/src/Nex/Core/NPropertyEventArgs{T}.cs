using System;

namespace Nex.Core
{
    public class NPropertyEventArgs<TValue>:NEventArgs
    {
        public TValue NewValue { get; private set; }
        public TValue OldValue { get; private set; }

        public NPropertyEventArgs(object source, TValue oldValue, TValue newValue)
            :base(source)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
