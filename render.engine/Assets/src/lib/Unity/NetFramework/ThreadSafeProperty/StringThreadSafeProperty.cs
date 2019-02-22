using System;


[System.Serializable]
public class StringThreadSafeProperty : ThreadSafeProperty<string>
{
    #region [ Constructor ]
    public StringThreadSafeProperty(string value)
        : base(value) { }
    public StringThreadSafeProperty()
        : base() { }
    #endregion

    #region [ abstract ]
    public override bool IsEqual(string a, string b)
    {
        return a == b;
    }
    #endregion
}
