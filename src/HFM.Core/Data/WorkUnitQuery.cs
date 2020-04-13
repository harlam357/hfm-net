/*
 * HFM.NET - Query Parameters Class
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace HFM.Core.Data
{
    /// <summary>
    /// Represents the operators for a work unit history query.
    /// </summary>
    public enum WorkUnitQueryOperator
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Like,
        NotLike,
        NotEqual
    }

    [DataContract]
    public class WorkUnitQuery : IEquatable<WorkUnitQuery>, IComparable<WorkUnitQuery>, IComparable
    {
        private const string SelectAllName = "*** SELECT ALL ***";

        public static WorkUnitQuery SelectAll { get; } = new WorkUnitQuery { _ordinal = 0, Name = SelectAllName };

        public WorkUnitQuery()
        {
            Name = String.Empty;
        }

        public WorkUnitQuery(string name)
        {
            Name = name;
        }

        private int _ordinal = Int32.MaxValue;

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [field: DataMember(Order = 2)]
        public List<WorkUnitQueryParameter> Parameters { get; } = new List<WorkUnitQueryParameter>();

        public WorkUnitQuery AddParameter(WorkUnitRowColumn column, WorkUnitQueryOperator queryOperator, object value)
        {
            return AddParameter(new WorkUnitQueryParameter(column, queryOperator, value));
        }

        public WorkUnitQuery AddParameter(WorkUnitQueryParameter parameter)
        {
            Parameters.Add(parameter);
            return this;
        }

        public WorkUnitQuery DeepClone()
        {
            return ProtoBuf.Serializer.DeepClone(this);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(WorkUnitQuery other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((WorkUnitQuery) obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return (Name != null ? Name.GetHashCode() : 0);
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        public static bool operator ==(WorkUnitQuery left, WorkUnitQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WorkUnitQuery left, WorkUnitQuery right)
        {
            return !Equals(left, right);
        }

        public int CompareTo(WorkUnitQuery other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var ordinalComparison = _ordinal.CompareTo(other._ordinal);
            if (ordinalComparison != 0) return ordinalComparison;
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is WorkUnitQuery other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(WorkUnitQuery)}");
        }

        public static bool operator <(WorkUnitQuery left, WorkUnitQuery right)
        {
            return Comparer<WorkUnitQuery>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(WorkUnitQuery left, WorkUnitQuery right)
        {
            return Comparer<WorkUnitQuery>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(WorkUnitQuery left, WorkUnitQuery right)
        {
            return Comparer<WorkUnitQuery>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(WorkUnitQuery left, WorkUnitQuery right)
        {
            return Comparer<WorkUnitQuery>.Default.Compare(left, right) >= 0;
        }
    }

    internal static class WorkUnitQueryExtensions
    {
        internal static PetaPoco.Sql Append(this PetaPoco.Sql sql, WorkUnitQuery query)
        {
            return sql.Append(query.ToSql());
        }

        internal static PetaPoco.Sql ToSql(this WorkUnitQuery query)
        {
            if (query.Parameters.Count == 0)
            {
                return null;
            }

            bool appendAnd = false;

            PetaPoco.Sql sql = PetaPoco.Sql.Builder.Append("WHERE ");
            foreach (var field in query.Parameters)
            {
                sql = sql.Append(appendAnd ? "AND " : String.Empty);
                sql = BuildWhereCondition(sql, field);
                appendAnd = true;
            }

            return appendAnd ? sql.Append(" ORDER BY [ID] ASC") : null;
        }

        private static PetaPoco.Sql BuildWhereCondition(PetaPoco.Sql sql, WorkUnitQueryParameter parameter)
        {
            string format = "[{0}] {1} @0";
            if (parameter.Column.Equals(WorkUnitRowColumn.DownloadDateTime) ||
                parameter.Column.Equals(WorkUnitRowColumn.CompletionDateTime))
            {
                format = "datetime([{0}]) {1} datetime(@0)";
            }
            sql = sql.Append(String.Format(CultureInfo.InvariantCulture, format,
                ColumnNameOverrides.ContainsKey(parameter.Column) ? ColumnNameOverrides[parameter.Column] : parameter.Column.ToString(),
                parameter.GetOperatorString()), parameter.Value);
            return sql;
        }

        private static readonly Dictionary<WorkUnitRowColumn, string> ColumnNameOverrides = new Dictionary<WorkUnitRowColumn, string>
        {
            { WorkUnitRowColumn.Name, "InstanceName" },
            { WorkUnitRowColumn.Path, "InstancePath" },
            { WorkUnitRowColumn.Credit, "CalcCredit" },
        };
    }

    [DataContract]
    public class WorkUnitQueryParameter
    {
        public WorkUnitQueryParameter()
        {
            Column = WorkUnitRowColumn.ProjectID;
            Operator = WorkUnitQueryOperator.Equal;
        }

        public WorkUnitQueryParameter(WorkUnitRowColumn column, WorkUnitQueryOperator queryOperator, object value)
        {
            Column = column;
            Operator = queryOperator;
            Value = value;
        }

        [DataMember(Order = 1)]
        public WorkUnitRowColumn Column { get; set; }
        [DataMember(Order = 2)]
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public WorkUnitQueryOperator Operator { get; set; }

        public object Value
        {
            get
            {
                if (_dateTimeValue.HasValue)
                {
                    return _dateTimeValue.Value;
                }
                return _stringValue;
            }
            set
            {
                if (value == null)
                {
                    _dateTimeValue = null;
                    _stringValue = null;
                }
                else if (value is DateTime dateTime)
                {
                    _dateTimeValue = dateTime;
                    _stringValue = null;
                }
                else
                {
                    _dateTimeValue = null;
                    _stringValue = value.ToString();
                }
            }
        }

        [DataMember(Order = 3)]
        private DateTime? _dateTimeValue;
        [DataMember(Order = 4)]
        private string _stringValue;

        internal string GetOperatorString()
        {
            switch (Operator)
            {
                case WorkUnitQueryOperator.Equal:
                    return "=";
                case WorkUnitQueryOperator.GreaterThan:
                    return ">";
                case WorkUnitQueryOperator.GreaterThanOrEqual:
                    return ">=";
                case WorkUnitQueryOperator.LessThan:
                    return "<";
                case WorkUnitQueryOperator.LessThanOrEqual:
                    return "<=";
                case WorkUnitQueryOperator.Like:
                    return "LIKE";
                case WorkUnitQueryOperator.NotLike:
                    return "NOT LIKE";
                case WorkUnitQueryOperator.NotEqual:
                    return "!=";
                default:
                    throw new InvalidOperationException($"Operator {Operator} is not supported.");
            }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Column, GetOperatorString(), Value);
        }

        public static string[] GetColumnNames()
        {
            // Indexes Must Match WorkUnitRowColumn enum defined in Enumerations.cs
            return new[]
            {
                "ProjectID",
                "Run",
                "Clone",
                "Gen",
                "Name",
                "Path",
                "Username",
                "Team",
                "Core Version",
                "Frames Completed",
                "Frame Time (Seconds)",
                "Unit Result",
                "Download Date (UTC)",
                "Completion Date (UTC)",
                "Work Unit Name",
                "KFactor",
                "Core Name",
                "Total Frames",
                "Atoms",
                "Slot Type",
                "PPD",
                "Credit"
            };
        }
    }
}
