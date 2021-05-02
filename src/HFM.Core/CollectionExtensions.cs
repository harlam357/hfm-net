using System.Collections.Generic;
using System.Linq;

namespace HFM.Core
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes all existing items from the <see cref="ICollection{T}" /> and adds the <paramref name="items" />.
        /// </summary>
        public static void Reset<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            source.Clear();
            if (items != null)
            {
                // optimize for source List<T>
                if (source is List<T> list)
                {
                    list.AddRange(items);
                }
                else
                {
                    foreach (var item in items)
                    {
                        source.Add(item);
                    }
                }
            }
        }

        public static double MaxOrDefault(this ICollection<double> source) => source.Count > 0 ? source.Max() : default;
    }
}
