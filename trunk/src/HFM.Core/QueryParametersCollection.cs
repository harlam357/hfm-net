/*
 * HFM.NET - Query Parameters Collection Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IQueryParametersCollection : ICollection<QueryParameters>
   {
      #region DataContainer<T>

      void Read();

      List<QueryParameters> Read(string filePath, Plugins.IFileSerializer<List<QueryParameters>> serializer);

      void Write();

      void Write(string filePath, Plugins.IFileSerializer<List<QueryParameters>> serializer);

      #endregion
   }

   public class QueryParametersCollection : DataContainer<List<QueryParameters>>, IQueryParametersCollection
   {
      public QueryParametersCollection()
         : this(null)
      {
         
      }

      public QueryParametersCollection(IPreferenceSet prefs)
      {
         Data.Add(new QueryParameters());

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

      /// <summary>
      /// Adds a QueryParameters to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The QueryParameters to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="item"/> is null.</exception>
      public void Add(QueryParameters item)
      {
         if (item == null) throw new ArgumentNullException("item");

         Data.Add(item);
      }

      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      public void Clear()
      {
         Data.Clear();
      }

      /// <summary>
      /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
      /// </returns>
      /// <param name="item">The QueryParameters to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      public bool Contains(QueryParameters item)
      {
         return item != null && Data.Contains(item);
      }

      /// <summary>
      /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
      /// </summary>
      /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
      /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
      /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
      /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
      ///     -or-
      ///     <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
      ///     -or-
      ///     The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
      /// </exception>
      void ICollection<QueryParameters>.CopyTo(QueryParameters[] array, int arrayIndex)
      {
         Data.CopyTo(array, arrayIndex);
      }

      /// <summary>
      /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <returns>
      /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      public int Count
      {
         get { return Data.Count; }
      }

      /// <summary>
      /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
      /// </summary>
      /// <returns>
      /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
      /// </returns>
      bool ICollection<QueryParameters>.IsReadOnly
      {
         get { return false; }
      }

      /// <summary>
      /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      /// <param name="item">The QueryParameters to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      public bool Remove(QueryParameters item)
      {
         return item != null && Data.Remove(item);
      }

      #endregion

      #region IEnumerable<QueryParameters> Members

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public IEnumerator<QueryParameters> GetEnumerator()
      {
         return Data.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
