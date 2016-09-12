/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Globalization;

namespace HFM.Core.DataTypes
{
   public struct ProteinBenchmarkSlotIdentifier : IComparable<ProteinBenchmarkSlotIdentifier>, IEquatable<ProteinBenchmarkSlotIdentifier>
   {
      /// <summary>
      /// Gets a value that indicates if this identifier represents 'All Slots'
      /// </summary>
      public bool AllSlots
      {
         get { return _value == null; }
      }

      private readonly string _value;

      public string Value
      {
         get { return _value ?? "All Slots"; }
      }

      public ProteinBenchmarkSlotIdentifier(string name, string path)
      {
         if (String.IsNullOrEmpty(name)) throw new ArgumentException("Argument 'name' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(path)) throw new ArgumentException("Argument 'path' cannot be a null or empty string.");

         _value = String.Format(CultureInfo.InvariantCulture, "{0} ({1})", name, path.TrimEnd('\\', '/'));
      }

      public static bool operator == (ProteinBenchmarkSlotIdentifier left, ProteinBenchmarkSlotIdentifier right)
      {
         return Equals(left, right);
      }

      public static bool operator != (ProteinBenchmarkSlotIdentifier left, ProteinBenchmarkSlotIdentifier right)
      {
         return !Equals(left, right);
      }

      public bool Equals(ProteinBenchmarkSlotIdentifier other)
      {
         return string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj)) return false;
         return obj is ProteinBenchmarkSlotIdentifier && Equals((ProteinBenchmarkSlotIdentifier)obj);
      }

      public override int GetHashCode()
      {
         return (_value != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(_value) : 0);
      }

      public static bool operator < (ProteinBenchmarkSlotIdentifier left, ProteinBenchmarkSlotIdentifier right)
      {
         return left.CompareTo(right) < 0;
      }

      public static bool operator > (ProteinBenchmarkSlotIdentifier left, ProteinBenchmarkSlotIdentifier right)
      {
         return right.CompareTo(left) < 0;
      }
      
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
      public int CompareTo(ProteinBenchmarkSlotIdentifier other)
      {
         // both AllSlots true (equal)
         if (AllSlots && other.AllSlots)
         {
            return 0;
         }

         // both AllSlots false (check properties)
         if (AllSlots == other.AllSlots)
         {
            if (Value == other.Value)
            {
               return 0;
            }
            return String.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);
         }

         // this AllSlots true (this is less than)
         if (AllSlots)
         {
            return -1;
         }
         // other AllSlots true (this is greater than)
         return 1;
      }
   }
}
