
using System;

namespace HFM.Core.WorkUnits
{
    public static class DateTimeExtensions
    {
        public static bool IsMinValue(this DateTime dateTime)
        {
            return dateTime.Equals(DateTime.MinValue);
        }

        public static string ToShortStringOrEmpty(this DateTime dateTime)
        {
            return ToStringOrEmpty(dateTime, $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}");
        }

        private static string ToStringOrEmpty(this IEquatable<DateTime> date, string formattedValue)
        {
            return date.Equals(DateTime.MinValue) ? String.Empty : formattedValue;
        }
    }
}
