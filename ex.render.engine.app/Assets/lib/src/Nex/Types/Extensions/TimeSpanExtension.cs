using System;

namespace Nex.Types
{
    public static class TimeSpanExtension
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
        #region [ TimeSpan ]
        public const string StringFormatWhithMilliseconds = @"hh\:mm\:ss\.fff";
        public const string StringFormat = @"hh\:mm\:ss";
        public static TimeSpan ToFormattedTimeSpan(this string value, bool useMilliseconds = true)
        {
            if (value.StartsWith("-"))
                return -TimeSpan.ParseExact(value.Remove(0, 1), useMilliseconds ? StringFormatWhithMilliseconds : StringFormat, null);
            else
                return TimeSpan.ParseExact(value, useMilliseconds ? StringFormatWhithMilliseconds : StringFormat, null);
        }
        public static TimeSpan ToTimeSpan(this string value)
        {
            return TimeSpan.Parse(value);
        }
        public static string ToFormattedString(this TimeSpan value, bool useMilliseconds = true)
        {
            if (value >= TimeSpan.Zero)
                return value.ToString(useMilliseconds ? StringFormatWhithMilliseconds : StringFormat);
            else
                return "-" + value.ToString(useMilliseconds ? StringFormatWhithMilliseconds : StringFormat);
        }
        #endregion
#else
        #region [ TimeSpan ]
        public const string StringFormatWhithMilliseconds = @"hh\:mm\:ss\.fff";
        public const string StringFormat = @"hh\:mm\:ss";
        public static TimeSpan ToFormattedTimeSpan(this string value, bool useMilliseconds = true)
        {
            if (value.StartsWith("-"))
                return -TimeSpan.Parse(value.Remove(0, 1));
            else
                return TimeSpan.Parse(value);
        }
        public static TimeSpan ToTimeSpan(this string value)
        {
            return TimeSpan.Parse(value);
        }
        public static string ToFormattedString(this TimeSpan value, bool useMilliseconds = true)
        {
            if (value >= TimeSpan.Zero)
                return value.ToString();
            else
                return "-" + value.ToString();
        }
        #endregion
#endif
    }
}
