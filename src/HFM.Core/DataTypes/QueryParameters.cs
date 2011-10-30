/*
 * HFM.NET - Query Parameters Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

using ProtoBuf;

namespace HFM.Core.DataTypes
{
   [ProtoContract]
   public class QueryParameters : IComparable<QueryParameters>
   {
      public const string SelectAll = "*** SELECT ALL ***";

      public QueryParameters()
      {
         Name = SelectAll;
      }

      public QueryParameters DeepCopy()
      {
         return Serializer.DeepClone(this);
      }

      [ProtoMember(1)]
      public string Name { get; set; }
      [ProtoMember(2)]
      private readonly List<QueryField> _fields = new List<QueryField>();

      public List<QueryField> Fields
      {
         get { return _fields; }
      }

      #region IComparable<QueryParameters> Members

      public int CompareTo(QueryParameters other)
      {
         // other null, this is greater
         if (other == null)
         {
            return 1;
         }

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

         if (Name == SelectAll)
         {
            if (other.Name == SelectAll)
            {
               // both SelectAll, equal
               return 0;
            }

            // Name is SelectAll, this is less
            return -1;
         }

         if (other.Name == SelectAll)
         {
            // other.Name is SelectAll, this is greater
            return 1;
         }

         // finally, just compare
         return Name.CompareTo(other.Name);
      }

      #endregion
   }

   [ProtoContract]
   public class QueryField
   {
      public QueryField()
      {
         Name = QueryFieldName.ProjectID;
         Type = QueryFieldType.Equal;
      }

      [ProtoMember(1)]
      public QueryFieldName Name { get; set; }
      [ProtoMember(2)]
      [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
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
