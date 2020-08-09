
using System;

namespace HFM.Preferences.Internal
{
    internal static class ObjectExtensions
    {
        internal static T Copy<T>(this T value)
        {
            if (null == (object)value)
            {
                return value;
            }
            return Copy(value, value.GetType());
        }

        internal static T Copy<T>(this T value, Type dataType)
        {
            if (dataType.IsValueType || dataType == typeof(string) || null == (object)value)
            {
                return value;
            }
            return (T)Activator.CreateInstance(dataType, value);
        }
    }
}
