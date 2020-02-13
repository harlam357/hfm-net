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
    public enum WorkUnitHistoryQueryOperator
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
    public class WorkUnitHistoryQuery : IComparable<WorkUnitHistoryQuery>, IEquatable<WorkUnitHistoryQuery>
    {
        private static readonly WorkUnitHistoryQuery SelectAllValue = new WorkUnitHistoryQuery();

        public static WorkUnitHistoryQuery SelectAll => SelectAllValue;

        private const string SelectAllName = "*** SELECT ALL ***";

        public WorkUnitHistoryQuery()
        {
            Name = SelectAllName;
        }

        public WorkUnitHistoryQuery(string name)
        {
            Name = name;
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }
        [DataMember(Order = 2)]
        private readonly List<WorkUnitHistoryQueryParameter> _parameters = new List<WorkUnitHistoryQueryParameter>();

        public List<WorkUnitHistoryQueryParameter> Parameters => _parameters;

        public WorkUnitHistoryQuery AddParameter(WorkUnitHistoryRowColumn column, WorkUnitHistoryQueryOperator queryOperator, object value)
        {
            return AddParameter(new WorkUnitHistoryQueryParameter(column, queryOperator, value));
        }

        public WorkUnitHistoryQuery AddParameter(WorkUnitHistoryQueryParameter parameter)
        {
            Parameters.Add(parameter);
            return this;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(WorkUnitHistoryQuery)) return false;
            return Equals((WorkUnitHistoryQuery)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(WorkUnitHistoryQuery left, WorkUnitHistoryQuery right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WorkUnitHistoryQuery left, WorkUnitHistoryQuery right)
        {
            return !Equals(left, right);
        }

        #region IEquatable<WorkUnitHistoryQuery> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(WorkUnitHistoryQuery other)
        {
            //if (ReferenceEquals(null, other)) return false;
            //if (ReferenceEquals(this, other)) return true;
            return CompareTo(other) == 0;
        }

        #endregion

        public static bool operator <(WorkUnitHistoryQuery left, WorkUnitHistoryQuery right)
        {
            return left == null ? right != null : left.CompareTo(right) < 0;
        }

        public static bool operator >(WorkUnitHistoryQuery left, WorkUnitHistoryQuery right)
        {
            return right == null ? left != null : right.CompareTo(left) < 0;
        }

        #region IComparable<WorkUnitHistoryQuery> Members

        public int CompareTo(WorkUnitHistoryQuery other)
        {
            if (ReferenceEquals(null, other)) return 1;
            if (ReferenceEquals(this, other)) return 0;

            // other not null, check this Name
            if (Name == null)
            {
                // if null, check other.Name
                if (other.Name == null)
                {
                    // if other.Name is null, equal
                    return 0;
                }

                // other.Name NOT null, this is less
                return -1;
            }

            if (Name == SelectAllName)
            {
                if (other.Name == SelectAllName)
                {
                    // both SelectAll, equal
                    return 0;
                }

                // Name is SelectAll, this is less
                return -1;
            }

            if (other.Name == SelectAllName)
            {
                // other.Name is SelectAll, this is greater
                return 1;
            }

            // finally, just compare
            return Name.CompareTo(other.Name);
        }

        #endregion

        public WorkUnitHistoryQuery DeepClone()
        {
            return ProtoBuf.Serializer.DeepClone(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    internal static class WorkUnitHistoryQueryExtensions
    {
        internal static PetaPoco.Sql Append(this PetaPoco.Sql sql, WorkUnitHistoryQuery query)
        {
            return sql.Append(query.ToSql());
        }

        internal static PetaPoco.Sql ToSql(this WorkUnitHistoryQuery query)
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

        private static PetaPoco.Sql BuildWhereCondition(PetaPoco.Sql sql, WorkUnitHistoryQueryParameter parameter)
        {
            string format = "[{0}] {1} @0";
            if (parameter.Column.Equals(WorkUnitHistoryRowColumn.DownloadDateTime) ||
                parameter.Column.Equals(WorkUnitHistoryRowColumn.CompletionDateTime))
            {
                format = "datetime([{0}]) {1} datetime(@0)";
            }
            sql = sql.Append(String.Format(CultureInfo.InvariantCulture, format,
                ColumnNameOverrides.ContainsKey(parameter.Column) ? ColumnNameOverrides[parameter.Column] : parameter.Column.ToString(),
                parameter.OperatorString), parameter.Value);
            return sql;
        }

        private static readonly Dictionary<WorkUnitHistoryRowColumn, string> ColumnNameOverrides = new Dictionary<WorkUnitHistoryRowColumn, string>
        {
            { WorkUnitHistoryRowColumn.Name, "InstanceName" },
            { WorkUnitHistoryRowColumn.Path, "InstancePath" },
            { WorkUnitHistoryRowColumn.Credit, "CalcCredit" },
        };
    }

    [DataContract]
    public class WorkUnitHistoryQueryParameter
    {
        public WorkUnitHistoryQueryParameter()
        {
            Column = WorkUnitHistoryRowColumn.ProjectID;
            Operator = WorkUnitHistoryQueryOperator.Equal;
        }

        public WorkUnitHistoryQueryParameter(WorkUnitHistoryRowColumn column, WorkUnitHistoryQueryOperator queryOperator, object value)
        {
            Column = column;
            Operator = queryOperator;
            Value = value;
        }

        [DataMember(Order = 1)]
        public WorkUnitHistoryRowColumn Column { get; set; }
        [DataMember(Order = 2)]
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public WorkUnitHistoryQueryOperator Operator { get; set; }

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

        internal string OperatorString => GetOperator(Operator);

        private static string GetOperator(WorkUnitHistoryQueryOperator type)
        {
            switch (type)
            {
                case WorkUnitHistoryQueryOperator.Equal:
                    return "=";
                case WorkUnitHistoryQueryOperator.GreaterThan:
                    return ">";
                case WorkUnitHistoryQueryOperator.GreaterThanOrEqual:
                    return ">=";
                case WorkUnitHistoryQueryOperator.LessThan:
                    return "<";
                case WorkUnitHistoryQueryOperator.LessThanOrEqual:
                    return "<=";
                case WorkUnitHistoryQueryOperator.Like:
                    return "LIKE";
                case WorkUnitHistoryQueryOperator.NotLike:
                    return "NOT LIKE";
                case WorkUnitHistoryQueryOperator.NotEqual:
                    return "!=";
                default:
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                       "Query Field Type '{0}' is not implemented.", type));
            }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Column, OperatorString, Value);
        }

        public static string[] GetColumnNames()
        {
            // Indexes Must Match WorkUnitHistoryRowColumn enum defined in Enumerations.cs
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
