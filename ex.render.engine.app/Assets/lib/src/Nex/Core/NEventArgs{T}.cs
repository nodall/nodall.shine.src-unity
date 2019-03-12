namespace Nex.Core
{
    public class NEventArgs<TValue>:NEventArgs
    {
        public TValue Value { get; private set; }

        public NEventArgs(object source, TValue value)
            :base(source)
        {
            Value = value;
        }
    }
}
