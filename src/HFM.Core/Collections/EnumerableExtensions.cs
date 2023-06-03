namespace HFM.Core.Collections;

public static class EnumerableExtensions
{
    public static ICollection<T> CastOrToCollection<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source as ICollection<T> ?? source.ToList();
    }
}
