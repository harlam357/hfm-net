﻿
using System;
using System.Collections.Generic;
using System.Globalization;

using ProtoBuf;

namespace HFM.Framework.DataTypes
{
   [ProtoContract]
   public class QueryParameters
   {
      public const string SelectAll = "*** SELECT ALL ***";

      public QueryParameters()
      {
         Name = SelectAll;
      }

      public QueryParameters DeepCopy()
      {
         var parameters = new QueryParameters { Name = Name };
         foreach (var field in Fields)
         {
            parameters.Fields.Add(field.DeepCopy());
         }
         return parameters;
      }

      [ProtoMember(1)]
      public string Name { get; set; }
      [ProtoMember(2)]
      private readonly List<QueryField> _fields = new List<QueryField>();

      public List<QueryField> Fields
      {
         get { return _fields; }
      }
   }

   [ProtoContract]
   public class QueryField
   {
      public QueryField()
      {
         Name = QueryFieldName.ProjectID;
         Type = QueryFieldType.Equal;
      }

      public QueryField DeepCopy()
      {
         // Value is set and returned as an object but the underlying
         // types are either value or immutable (string), so we're ok
         // here with just an assignment.
         return new QueryField { Name = Name, Type = Type, Value = Value };
      }

      [ProtoMember(1)]
      public QueryFieldName Name { get; set; }
      [ProtoMember(2)]
      public QueryFieldType Type { get; set; }

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
            else if (value is DateTime)
            {
               _dateTimeValue = (DateTime)value;
               _stringValue = null;
            }
            else
            {
               _dateTimeValue = null;
               _stringValue = value.ToString();
            }
         }
      }

      [ProtoMember(3)]
      private DateTime? _dateTimeValue;
      [ProtoMember(4)]
      private string _stringValue;

      public string Operator
      {
         get { return GetOperator(Type); }
      }

      private static string GetOperator(QueryFieldType type)
      {
         switch (type)
         {
            //case QueryFieldType.All:
            //   return "*";
            case QueryFieldType.Equal:
               return "==";
            case QueryFieldType.GreaterThan:
               return ">";
            case QueryFieldType.GreaterThanOrEqual:
               return ">=";
            case QueryFieldType.LessThan:
               return "<";
            case QueryFieldType.LessThanOrEqual:
               return "<=";
            default:
               throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
                  "Query Field Type '{0}' is not implemented.", type));
         }
      }
   }
}