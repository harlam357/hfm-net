/*
 * HFM.NET - Benchmark Client Class
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
using System.Globalization;

namespace HFM.Core.DataTypes
{
   /// <summary>
   /// Container Class used to Bind Benchmark Client Data to the Benchmarks Form
   /// </summary>
   public class BenchmarkClient : IComparable<BenchmarkClient>, IEquatable<BenchmarkClient>
   {
      #region Fields and Properties

      /// <summary>
      /// Self Referencing Property
      /// </summary>
      public BenchmarkClient Client
      {
         get { return this; }
      }

      private readonly string _name = String.Empty;
      /// <summary>
      /// Client Name
      /// </summary>
      public string Name
      {
         get { return _name; }
      }

      private readonly string _path = String.Empty;
      /// <summary>
      /// Client Path
      /// </summary>
      public string Path
      {
         get { return _path; }
      }

      private readonly bool _allClients;
      /// <summary>
      /// Value Indicates if this Benchmark Client represents 'All Clients'
      /// </summary>
      public bool AllClients
      {
         get { return _allClients; }
      }

      /// <summary>
      /// Concatenated Name and Path Value
      /// </summary>
      public string NameAndPath
      {
         get { return _allClients ? "All Clients" : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, Path); }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Create BenchmarkClient Instance (All Clients)
      /// </summary>
      public BenchmarkClient()
      {
         _allClients = true;
      }

      /// <summary>
      /// Create BenchmarkClient Instance (Individual Clients)
      /// </summary>
      public BenchmarkClient(string name, string path)
      {
         if (String.IsNullOrEmpty(name)) throw new ArgumentException("Argument 'name' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(path)) throw new ArgumentException("Argument 'path' cannot be a null or empty string.");

         _name = name;
         _path = path;
      }

      #endregion

      ///<summary>
      ///Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
      ///</summary>
      ///<returns>
      ///A hash code for the current <see cref="T:System.Object"></see>.
      ///</returns>
      ///<filterpriority>2</filterpriority>
      public override int GetHashCode()
      {
         return Name.GetHashCode() ^
                Path.GetHashCode() ^
                AllClients.GetHashCode();
      }

      ///<summary>
      ///Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
      ///</summary>
      ///<returns>
      ///true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
      ///</returns>
      ///<param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
      ///<filterpriority>2</filterpriority>
      public override bool Equals(object obj)
      {
         var client = obj as BenchmarkClient;
         return client != null ? Equals(client) : base.Equals(obj);
      }

      ///<summary>
      ///Determines whether the specified <see cref="T:HFM.Core.DataTypes.BenchmarkClient"></see> is equal to the current <see cref="T:HFM.Core.DataTypes.BenchmarkClient"></see>.
      ///</summary>
      ///<returns>
      ///true if the specified <see cref="T:HFM.Core.DataTypes.BenchmarkClient"></see> is equal to the current <see cref="T:HFM.Core.DataTypes.BenchmarkClient"></see>; otherwise, false.
      ///</returns>
      ///<param name="other">The <see cref="T:HFM.Core.DataTypes.BenchmarkClient"></see> to compare with the current <see cref="T:HFM.Core.DataTypes.BenchmarkClient"></see>.</param>
      public bool Equals(BenchmarkClient other)
      {
         if (other == null) return false;

         return Name.Equals(other.Name) &&
                Paths.Equal(Path, other.Path) &&
                AllClients.Equals(other.AllClients);
      }

      public static bool operator == (BenchmarkClient bc1, BenchmarkClient bc2)
      {
         return ReferenceEquals(bc1, null) ? ReferenceEquals(bc2, null) : bc1.Equals(bc2);
      }

      public static bool operator != (BenchmarkClient bc1, BenchmarkClient bc2)
      {
         return !(bc1 == bc2);
      }

      ///<summary>
      ///Compares the current object with another object of the same type.
      ///</summary>
      ///<returns>
      ///A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other. 
      ///</returns>
      ///<param name="other">An object to compare with this object.</param>
      public int CompareTo(BenchmarkClient other)
      {
         if (other == null)
         {
            return 1;
         }

         // both All Clients true (equal)
         if (AllClients && other.AllClients) return 0;

         // both All Client false (check properties)
         if (AllClients.Equals(other.AllClients))
         {
            if (Name.Equals(other.Name))
            {
               if (Paths.Equal(Path, other.Path))
               {
                  return 0;
               }

               return String.Compare(Path, other.Path, Default.PathComparison);
            }

            return String.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
         }

         // this All Clients true (this is less than)
         if (AllClients) return -1;

         // other All Clients true (this is greater than)
         return 1;
      }

      public static bool operator < (BenchmarkClient bc1, BenchmarkClient bc2)
      {
         return bc1 == null ? bc2 != null : bc1.CompareTo(bc2) < 0;
      }

      public static bool operator > (BenchmarkClient bc1, BenchmarkClient bc2)
      {
         return bc2 == null ? bc1 != null : bc2.CompareTo(bc1) < 0;
      }
   }
}
