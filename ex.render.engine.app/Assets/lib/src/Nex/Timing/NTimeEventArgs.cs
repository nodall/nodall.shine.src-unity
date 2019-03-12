using System;

namespace Nex.Timing
{
    public class NTimeEventArgs:EventArgs
    {
        public object Item { get; private set; }
        public TimeSpan Last { get; private set; }
        public TimeSpan Current { get; private set; }

        public NTimeEventArgs(object item, TimeSpan last, TimeSpan current)
        {
            Item = item;
            Last = last;
            Current = current;
        }
          
    }
}
