using System;
using System.Collections.Generic;
using System.Linq;

namespace HFM.Core.Serializers
{
    public static class IFileSerializerExtensions
    {
        public static string GetFileTypeFilters<T>(this IEnumerable<IFileSerializer<T>> serializers) where T : class, new()
        {
            return String.Join("|", serializers.Select(x => x.FileTypeFilter));
        }
    }
}
