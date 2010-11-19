/*
 * HFM.NET - Benchmark Client Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

namespace HFM.Framework.DataTypes
{
   /// <summary>
   /// Container Class used to Bind Benchmark Client Data to the Benchmarks Form
   /// </summary>
   public class BenchmarkClient : IComparable<BenchmarkClient>, IEquatable<BenchmarkClient>
   {
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
         get
         {
            if (_allClients)
            {
               return "All Clients";
            }

            return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, Path);
         }
      }

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
      public BenchmarkClient(string clientName, string clientPath)
      {
         _name = clientName;
         _path = clientPath;
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
         if (client != null)
         {
            return Equals(client);
         }

         return base.Equals(obj);
      }

      ///<summary>
      ///Determines whether the specified <see cref="T:HFM.Instances.BenchmarkClient"></see> is equal to the current <see cref="T:HFM.Instances.BenchmarkClient"></see>.
      ///</summary>
      ///<returns>
      ///true if the specified <see cref="T:HFM.Instances.BenchmarkClient"></see> is equal to the current <see cref="T:HFM.Instances.BenchmarkClient"></see>; otherwise, false.
      ///</returns>
      ///<param name="client">The <see cref="T:HFM.Instances.BenchmarkClient"></see> to compare with the current <see cref="T:HFM.Instances.BenchmarkClient"></see>.</param>
      public bool Equals(BenchmarkClient client)
      {
         if (client == null)
         {
            return false;
         }

         if (Name.Equals(client.Name) &&
             StringOps.PathsEqual(Path, client.Path) &&
             AllClients.Equals(client.AllClients))
         {
            return true;
         }

         return false;
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

         if (AllClients.Equals(other.AllClients))
         {
            if (Name.Equals(other.Name))
            {
               if (StringOps.PathsEqual(Path, other.Path))
               {
                  return 0;
               }

               return Path.CompareTo(other.Path);
            }

            return Name.CompareTo(other.Name);
         }

         if (AllClients) return -1;

         return 1;
      }

      public static bool operator < (BenchmarkClient bc1, BenchmarkClient bc2)
      {
         return (bc1.CompareTo(bc2) < 0);
      }

      public static bool operator > (BenchmarkClient bc1, BenchmarkClient bc2)
      {
         return (bc1.CompareTo(bc2) > 0);
      }
   }
}
