using System;

[Serializable]
public class ExceptionThreadSafeProperty : ThreadSafeProperty<Exception>
{
    public override bool IsEqual(Exception a, Exception b)
    {
        return a == b;
    }
}
