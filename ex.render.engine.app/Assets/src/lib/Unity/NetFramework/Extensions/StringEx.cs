using System;

public static class StringEx
{
    public static string Add(this string value, params object[] args)
    {
        return String.Format(value, args);       
    }
}