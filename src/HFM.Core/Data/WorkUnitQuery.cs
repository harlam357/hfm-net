﻿using System.Globalization;
using System.Runtime.Serialization;

namespace HFM.Core.Data;

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
    public const string NewQueryName = "* New Query *";
    private const string SelectAllName = "*** SELECT ALL ***";

    public static WorkUnitQuery SelectAll { get; } = new() { _ordinal = 0, Name = SelectAllName };

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
    public List<WorkUnitQueryParameter> Parameters { get; } = new();

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

    public override string ToString()
    {
        return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Column, Operator, Value);
    }
}
