using System;

public class BoolThreadSafeProperty : ThreadSafeProperty<bool>
{
    #region [ Constructor ]
    public BoolThreadSafeProperty(bool value)
        : base(value) { }
    public BoolThreadSafeProperty()
        : base() { }
    #endregion

    #region [ abstract ]
    public override bool IsEqual(bool a, bool b)
    {
        return a == b;
    }
    #endregion
}
