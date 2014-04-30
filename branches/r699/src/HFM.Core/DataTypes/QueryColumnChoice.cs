/*
 * HFM.NET - Query Column Choice Class
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

namespace HFM.Core.DataTypes
{
   /// <summary>
   /// Class used to bind column choice data to the work unit history query dialog.
   /// </summary>
   public sealed class QueryColumnChoice : IComparable<QueryColumnChoice>, IEquatable<QueryColumnChoice>
   {
      private readonly string _display;

      public string Display
      {
         get { return _display; }
      }

      private readonly object _value;

      public object Value
      {
         get { return _value; }
      }

      public QueryColumnChoice(string display, object value)
      {
         if (display == null) throw new ArgumentNullException("display");
         if (value == null) throw new ArgumentNullException("value");

         _display = display;
         _value = value;
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
         if (obj.GetType() != typeof(QueryColumnChoice)) return false;
         return Equals((QueryColumnChoice)obj);
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
         return _display.GetHashCode();
      }

      public static bool operator == (QueryColumnChoice left, QueryColumnChoice right)
      {
         return Equals(left, right);
      }

      public static bool operator != (QueryColumnChoice left, QueryColumnChoice right)
      {
         return !Equals(left, right);
      }

      #region IEquatable<QueryColumnChoice> Members

      /// <summary>
      /// Indicates whether the current object is equal to another object of the same type.
      /// </summary>
      /// <returns>
      /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
      /// </returns>
      /// <param name="other">An object to compare with this object.</param>
      public bool Equals(QueryColumnChoice other)
      {
         //if (ReferenceEquals(null, other)) return false;
         //if (ReferenceEquals(this, other)) return true;
         return CompareTo(other) == 0;
      }

      #endregion

      public static bool operator < (QueryColumnChoice left, QueryColumnChoice right)
      {
         return left == null ? right != null : left.CompareTo(right) < 0;
      }

      public static bool operator > (QueryColumnChoice left, QueryColumnChoice right)
      {
         return right == null ? left != null : right.CompareTo(left) < 0;
      }

      #region IComparable<QueryColumnChoice> Members

      /// <summary>
      /// Compares the current object with another object of the same type.
      /// </summary>
      /// <returns>
      /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: 
      /// Less than zero - This object is less than the <paramref name="other"/> parameter.
      /// Zero - This object is equal to <paramref name="other"/>. 
      /// Greater than zero - This object is greater than <paramref name="other"/>. 
      /// </returns>
      /// <param name="other">An object to compare with this object.</param>
      public int CompareTo(QueryColumnChoice other)
      {
         if (ReferenceEquals(null, other)) return 1;
         if (ReferenceEquals(this, other)) return 0;
         return _display.CompareTo(other.Display);
      }

      #endregion
   }
}
