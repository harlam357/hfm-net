using System.Data.Common;

namespace HFM.Core.Data.Internal;

internal static class DbDataReaderExtensions
{
    internal static T GetFieldValueOrDefault<T>(this DbDataReader reader, string name, T defaultValue = default)
    {
        int ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal)
            ? defaultValue
            : reader.GetFieldValue<T>(ordinal);
    }

}
