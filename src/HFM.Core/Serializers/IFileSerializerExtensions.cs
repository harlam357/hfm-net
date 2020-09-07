using System.Collections.Generic;
using System.Text;

namespace HFM.Core.Serializers
{
    public static class IFileSerializerExtensions
    {
        public static string GetFileTypeFilters<T>(this IEnumerable<IFileSerializer<T>> serializers) where T : class, new()
        {
            var sb = new StringBuilder();
            foreach (var s in serializers)
            {
                sb.Append(s.FileTypeFilter);
                sb.Append("|");
            }

            sb.Length = sb.Length - 1;
            return sb.ToString();
        }
    }
}
