/*
 * HFM.NET - Query Parameters Collection Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IQueryParametersCollection : ICollection<QueryParameters>
   {
      #region ICollection<QueryParameter> Members

      // Override Default Interface Documentation

      /// <summary>
      /// Adds a QueryParameters to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The QueryParameters to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="item"/> is null.</exception>
      new void Add(QueryParameters item);

      #endregion

      #region DataContainer<T>

      void Read();

      List<QueryParameters> Read(string filePath, Plugins.IFileSerializer<List<QueryParameters>> serializer);

      void Write();

      void Write(string filePath, Plugins.IFileSerializer<List<QueryParameters>> serializer);

      #endregion
   }

   public sealed class QueryParametersCollection : DataContainer<List<QueryParameters>>, IQueryParametersCollection
   {
      public QueryParametersCollection()
         : this(null)
      {
         
      }

      public QueryParametersCollection(IPreferenceSet prefs)
      {
         if (prefs != null && !String.IsNullOrEmpty(prefs.ApplicationDataFolderPath))
         {
            FileName = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, Constants.QueryCacheFileName);
         }
      }

      #region Properties

      public override Plugins.IFileSerializer<List<QueryParameters>> DefaultSerializer
      {
         get { return new Serializers.ProtoBufFileSerializer<List<QueryParameters>>(); }
      }

      #endregion

      #region ICollection<QueryParameter> Members

      public void Add(QueryParameters item)
      {
         if (item == null) throw new ArgumentNullException("item");

         Data.Add(item);
      }

      [CoverageExclude]
      public void Clear()
      {
         Data.Clear();
      }

      public bool Contains(QueryParameters item)
      {
         return item != null && Data.Contains(item);
      }

      [CoverageExclude]
      void ICollection<QueryParameters>.CopyTo(QueryParameters[] array, int arrayIndex)
      {
         Data.CopyTo(array, arrayIndex);
      }

      public int Count
      {
         [CoverageExclude]
         get { return Data.Count; }
      }

      bool ICollection<QueryParameters>.IsReadOnly
      {
         [CoverageExclude]
         get { return false; }
      }

      public bool Remove(QueryParameters item)
      {
         return item != null && Data.Remove(item);
      }

      #endregion

      #region IEnumerable<QueryParameters> Members

      [CoverageExclude]
      public IEnumerator<QueryParameters> GetEnumerator()
      {
         return Data.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      [CoverageExclude]
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
