
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace HFM.Core.Data
{
    internal sealed class SQLiteAddColumnCommand : IDisposable
    {
        private readonly string _tableName;
        private readonly SQLiteConnection _connection;

        public SQLiteAddColumnCommand(string tableName, SQLiteConnection connection)
        {
            _tableName = tableName;
            _connection = connection;

            Debug.Assert(_connection.State == ConnectionState.Open);
        }

        private readonly List<DbCommand> _commands = new List<DbCommand>();
        private EnumerableRowCollection<DataRow> _rows;

        public void AddColumn(string name, string dataType)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (dataType == null) throw new ArgumentNullException(nameof(dataType));

            if (_rows == null)
            {
                using (var adapter = new SQLiteDataAdapter("PRAGMA table_info(WuHistory);", _connection))
                using (var table = new DataTable())
                {
                    adapter.Fill(table);
                    _rows = table.AsEnumerable();
                }
            }

            bool columnExists = _rows.Any(row => row.Field<string>(1) == name);
            if (!columnExists)
            {
                string commandText = String.Format(CultureInfo.InvariantCulture,
                    "ALTER TABLE [{0}] ADD COLUMN [{1}] {2} DEFAULT {3} NOT NULL", _tableName, name, dataType, GetDefaultValue(dataType));
                _commands.Add(new SQLiteCommand(_connection) { CommandText = commandText });
            }
        }

        public static object GetDefaultValue(string dataType)
        {
            if (dataType.Contains("VARCHAR"))
            {
                return "''";
            }
            if (dataType.Contains("INT"))
            {
                return 0;
            }
            if (dataType.Contains("FLOAT"))
            {
                return 0.0f;
            }

            string message = String.Format(CultureInfo.CurrentCulture, "Data type {0} is not valid.", dataType);
            throw new ArgumentException(message, nameof(dataType));
        }

        public void Execute()
        {
            foreach (var command in _commands)
            {
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            foreach (var command in _commands)
            {
                command.Dispose();
            }
        }
    }
}