/*
 * HFM.NET - Query Parameters Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

namespace HFM.Core.DataTypes
{
   [DataContract]
   public class QueryParameters : IComparable<QueryParameters>, IEquatable<QueryParameters>
   {
      private static readonly QueryParameters SelectAllValue = new QueryParameters();

      public static QueryParameters SelectAll
      {
         get { return SelectAllValue; }
      }

      private const string SelectAllName = "*** SELECT ALL ***";

      public QueryParameters()
      {
         Name = SelectAllName;
      }

      public QueryParameters DeepClone()
      {
         return ProtoBuf.Serializer.DeepClone(this);
      }

      [DataMember(Order = 1)]
      public string Name { get; set; }
      [DataMember(Order = 2)]
      private readonly List<QueryField> _fields = new List<QueryField>();

      public List<QueryField> Fields
      {
         get { return _fields; }
      }

      /// <summary>
      /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
      /// </returns>
      /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
      /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
      /// <filterpriority>2</filterpriority>
      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         if (obj.GetType() != typeof(QueryParameters)) return false;
         return Equals((QueryParameters)obj);
      }

      /// <summary>
      /// Serves as a hash function for a particular type. 
      /// </summary>
      /// <returns>
      /// A hash code for the current <see cref="T:System.Object"/>.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override int GetHashCode()
      {
         return (Name != null ? Name.GetHashCode() : 0);
      }

      public static bool operator == (QueryParameters left, QueryParameters right)
      {
         return Equals(left, right);
      }

      public static bool operator != (QueryParameters left, QueryParameters right)
      {
         return !Equals(left, right);
      }

      #region IEquatable<QueryParameters> Members

      /// <summary>
      /// Indicates whether the current object is equal to another object of the same type.
      /// </summary>
      /// <returns>
      /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
      /// </returns>
      /// <param name="other">An object to compare with this object.</param>
      public bool Equals(QueryParameters other)
      {
         //if (ReferenceEquals(null, other)) return false;
         //if (ReferenceEquals(this, other)) return true;
         return CompareTo(other) == 0;
      }

      #endregion

      public static bool operator < (QueryParameters left, QueryParameters right)
      {
         return left == null ? right != null : left.CompareTo(right) < 0;
      }

      public static bool operator > (QueryParameters left, QueryParameters right)
      {
         return right == null ? left != null : right.CompareTo(left) < 0;
      }

      #region IComparable<QueryParameters> Members

      public int CompareTo(QueryParameters other)
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

      public override string ToString()
      {
         return Name;
      }
   }

   [DataContract]
   public class QueryField
   {
      public QueryField()
      {
         Name = QueryFieldName.ProjectID;
         Type = QueryFieldType.Equal;
      }

      [DataMember(Order = 1)]
      public QueryFieldName Name { get; set; }
      [DataMember(Order = 2)]
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

      [DataMember(Order = 3)]
      private DateTime? _dateTimeValue;
      [DataMember(Order = 4)]
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
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "Query Field Type '{0}' is not implemented.", type));
         }
      }
   }
}
