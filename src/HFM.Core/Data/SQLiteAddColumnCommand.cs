using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;

using Microsoft.Data.Sqlite;

namespace HFM.Core.Data
{
    internal class SQLiteAddColumnCommand : IDisposable
    {
        private readonly string _tableName;
        private readonly SqliteConnection _connection;

        public SQLiteAddColumnCommand(string tableName, SqliteConnection connection)
        {
            _tableName = tableName;
            _connection = connection;

            Debug.Assert(_connection.State == ConnectionState.Open);
        }

        private readonly List<DbCommand> _commands = new List<DbCommand>();
        private EnumerableRowCollection<DataRow> _rows;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input.")]
        public void AddColumn(string name, string dataType)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (dataType == null) throw new ArgumentNullException(nameof(dataType));

            if (_rows == null)
            {
                using var command = _connection.CreateCommand();
                using var table = command.GetSchema(_tableName);
                _rows = table.AsEnumerable();
            }

            bool columnExists = _rows.Any(row => row.Field<string>(1) == name);
            if (!columnExists)
            {
                string commandText = String.Format(CultureInfo.InvariantCulture,
                    "ALTER TABLE [{0}] ADD COLUMN [{1}] {2} DEFAULT {3} NOT NULL", _tableName, name, dataType, GetDefaultValue(dataType));
                var command = _connection.CreateCommand();
                command.CommandText = commandText;
                _commands.Add(command);
            }
        }

        public static object GetDefaultValue(string dataType)
        {
            if (dataType.Contains("TEXT"))
            {
                return "''";
            }
            if (dataType.Contains("INTEGER"))
            {
                return 0;
            }
            if (dataType.Contains("REAL"))
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
