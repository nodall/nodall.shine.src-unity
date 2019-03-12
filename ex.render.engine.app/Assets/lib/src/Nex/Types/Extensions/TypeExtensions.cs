using System;
using System.Globalization;

namespace Nex.Types
{
    public static class TypeExtensions
    {
        #region [ToString Methods ]
        public static string ToInvariantString(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvariantString(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        #endregion


        #region [ Parse Methods ]

        public static double ToDouble(this string value)
        {
            return Double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        public static Single ToSingle(this string value)
        {
            return Single.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        public static Int32 ToInt32(this string value)
        {
            return Int32.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        public static bool ToBoolean(this string value)
        {
            return Boolean.Parse(value);
        }
        public static T ToEnum<T>(this string value) where T : struct
        {
            return (T)Enum.Parse(typeof(T), value, false);
        }

        #endregion        

        #region [ Try Parse ]
        public static bool TryToDouble(this string value, out double result)
        {
            return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
        public static bool TryToSingle(this string value, out float result)
        {
            return float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
        public static bool TryToInt32(this string value, out int result)
        {
            return int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }
        public static bool TryToBoolean(this string value, out bool result)
        {
            return bool.TryParse(value, out result);
        }
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
        public static bool TryToEnum<T>(this string value, out T result) where T : struct
        {
            return Enum.TryParse<T>(value, false, out result);
        }
#endif
        #endregion

        // helpers for UNITY (NET 3.5)
#if !UNITY_EDITOR && !UNITY_STANDALONE && !UNITY_ANDROID && !UNITY_IOS
#else
        // helpers for 3.5 reflection
        public static Type GetType(this Type t)
        {
            return t;
        }

        public static Type GetTypeInfo(this Type t)
        {
            return t;
        }

        public static System.Reflection.PropertyInfo[] GetRuntimeProperties(this Type t)
        {
            return t.GetProperties();
        }

        public static System.Reflection.MethodInfo[] GetRuntimeMethods(this Type t)
        {
            return t.GetMethods();
        }

        public static System.IO.FileInfo[] EnumerateFiles(this System.IO.DirectoryInfo dir)
        {
            return dir.GetFiles();
        }
#endif
    }
}
