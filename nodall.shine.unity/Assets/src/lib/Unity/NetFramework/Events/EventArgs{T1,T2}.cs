using System;


public class EventArgs<T1, T2> : EventArgs
{
    public T1 Value1 { get; private set; }
    public T2 Value2 { get; private set; }
    public EventArgs(T1 value1, T2 value2)
    {
        Value1 = value1;
        Value2 = value2;
    }
}
