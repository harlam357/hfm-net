namespace HFM.Core.WorkUnits;

public static class DateTimeExtensions
{
    public static bool IsMinValue(this DateTime dateTime) => dateTime.Equals(DateTime.MinValue);

    public static string ToShortStringOrEmpty(this DateTime dateTime) =>
        ToStringOrEmpty(dateTime, $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}");

    private static string ToStringOrEmpty(this IEquatable<DateTime> date, string formattedValue) =>
        date.Equals(DateTime.MinValue) ? String.Empty : formattedValue;

    public static DateTime Normalize(this DateTime dateTime) => new(dateTime.Ticks / 10_000_000 * 10_000_000);
}
