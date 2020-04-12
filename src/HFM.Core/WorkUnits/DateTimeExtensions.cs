
using System;

namespace HFM.Core.WorkUnits
{
    public static class DateTimeExtensions
    {
        public static bool IsMinValue(this DateTime dateTime)
        {
            return dateTime.Equals(DateTime.MinValue);
        }

        public static string ToStringOrUnknown(this DateTime dateTime)
        {
            return ToStringOrUnknown(dateTime, $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}");
        }

        public static string ToStringOrUnknown(this IEquatable<DateTime> date, string formattedValue)
        {
            return date.Equals(DateTime.MinValue) ? Unknown.Value : formattedValue;
        }
    }
}
