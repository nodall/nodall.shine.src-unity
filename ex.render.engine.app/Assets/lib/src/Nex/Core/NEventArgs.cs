using System;

namespace Nex.Core
{
    public class NEventArgs:EventArgs
    {
        public object Source { get; private set; }

        public NEventArgs(object source)
        {
            Source = source;
        }
    }
}
