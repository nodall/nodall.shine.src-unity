using System;

[Serializable]
public class IntThreadSafeProperty : ThreadSafeProperty<int>
{
    #region [ Constructor ]
    public IntThreadSafeProperty(int value)
        : base(value) { }
    public IntThreadSafeProperty()
        : base() { }
    #endregion

    #region [ abstract ]
    public override bool IsEqual(int a, int b)
    {
        return a == b;
    }
    #endregion
}