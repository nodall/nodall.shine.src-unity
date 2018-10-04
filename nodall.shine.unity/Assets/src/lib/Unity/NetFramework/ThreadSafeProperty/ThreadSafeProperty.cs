using System;


[System.Serializable]
public abstract class ThreadSafeProperty<T>
{
    #region [ events ]
    public event EventHandler<PropertyEventArgs<T>> EventPropertyChanged;
    #endregion

    #region [ properties ]
    public T Value
    {
        get { return _value; }
        set { _pendingValue = value; }
    }
    #endregion

    #region [ fields ]
    public T _value;
    private T _pendingValue;
    #endregion

    #region [ constructor ]
    public ThreadSafeProperty()
    {
        _value = default(T);
        Value = default(T);
    }
    public ThreadSafeProperty(T initialValue)
    {
        _value = initialValue;
        _pendingValue = initialValue;
    }
    #endregion

    #region [ public method ]
    public void Initialize(T value)
    {
        _value = value;
        _pendingValue = value;
    }
    public void Update()
    {
        if (!IsEqual(_pendingValue, _value))
        {
            var oldValue = _value;
            _value = _pendingValue;
            if (EventPropertyChanged != null)
                EventPropertyChanged(this, new PropertyEventArgs<T>(oldValue, Value));
        }
    }
    #endregion

    #region [ abstract ]
    public abstract bool IsEqual(T a, T b);
    #endregion
}