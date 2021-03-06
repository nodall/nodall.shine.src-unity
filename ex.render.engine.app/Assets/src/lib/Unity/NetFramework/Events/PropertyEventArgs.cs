﻿using System;


public class PropertyEventArgs<T> : EventArgs
{
    public T OldValue { get; private set; }
    public T NewValue { get; private set; }
    public PropertyEventArgs(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}