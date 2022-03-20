using System.Data;

using Microsoft.Data.Sqlite;

namespace HFM.Core.Data.Internal;

internal static class SqliteCommandExtensions
{
    internal static DataTable GetSchema(this SqliteCommand command, string tableName)
    {
        command.CommandText = $"PRAGMA table_info({tableName});";
        using var reader = command.ExecuteReader();

        var table = new DataTable();
        var columnType = typeof(object);
        table.Columns.Add(new DataColumn("cid", columnType));
        table.Columns.Add(new DataColumn("name", columnType));
        table.Columns.Add(new DataColumn("type", columnType));
        table.Columns.Add(new DataColumn("notnull", columnType));
        table.Columns.Add(new DataColumn("dflt_value", columnType));
        table.Columns.Add(new DataColumn("pk", columnType));

        while (reader.Read())
        {
            var row = table.NewRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.GetValue(i);
            }
            table.Rows.Add(row);
        }

        return table;
    }
}
