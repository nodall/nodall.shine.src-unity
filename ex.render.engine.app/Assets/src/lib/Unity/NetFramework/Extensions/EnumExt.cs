using System;

public static class EnumEx
{
    public static bool HasFlag(this Enum value, Enum flag)
    {
        ulong num = Convert.ToUInt64(flag);
        ulong num2 = Convert.ToUInt64(value);
        return (num2 & num) == num;
    }
}
