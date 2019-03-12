using System;

namespace Nex.Timing
{
    public class NDateEventArgs:EventArgs
    {
        public object Item { get; private set; }
        public DateTime Last { get; private set; }
        public DateTime Current { get; private set; }

        public NDateEventArgs(object item, DateTime last, DateTime current)
        {
            Item = item;
            Last = last;
            Current = current;
        }
          
    }
}
