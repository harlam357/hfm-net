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

using HFM.Framework;

namespace HFM.Instances
{
   /// <summary>
   /// Container Class used to Bind Benchmark Client Data to the Benchmarks Form
   /// </summary>
   public class BenchmarkClient : IBenchmarkClient
   {
      /// <summary>
      /// Self Referencing Property
      /// </summary>
      public IBenchmarkClient Client
      {
         get { return this; }
      }

      private readonly string _Name = String.Empty;
      /// <summary>
      /// Client Name
      /// </summary>
      public string Name
      {
         get { return _Name; }
      }

      private readonly string _Path = String.Empty;
      /// <summary>
      /// Client Path
      /// </summary>
      public string Path
      {
         get { return _Path; }
      }

      private readonly bool _AllClients;
      /// <summary>
      /// Value Indicates if this Benchmark Client represents 'All Clients'
      /// </summary>
      public bool AllClients
      {
         get { return _AllClients; }
      }

      /// <summary>
      /// Concatenated Name and Path Value
      /// </summary>
      public string NameAndPath
      {
         get
         {
            if (_AllClients)
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
         _AllClients = true;
      }

      /// <summary>
      /// Create BenchmarkClient Instance (Individual Clients)
      /// </summary>
      public BenchmarkClient(string ClientName, string ClientPath)
      {
         _Name = ClientName;
         _Path = ClientPath;
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
         IBenchmarkClient client = obj as BenchmarkClient;
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
      public bool Equals(IBenchmarkClient client)
      {
         if (client == null)
         {
            return false;
         }

         if (Name.Equals(client.Name) &&
             Path.Equals(client.Path) &&
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
      public int CompareTo(IBenchmarkClient other)
      {
         if (other == null)
         {
            return 1;
         }

         if (AllClients.Equals(other.AllClients))
         {
            if (Name.Equals(other.Name))
            {
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
