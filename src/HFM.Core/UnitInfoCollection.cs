/*
 * HFM.NET - UnitInfo Collection Class
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

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IUnitInfoCollection : ICollection<UnitInfo>
   {
      ///// <summary>
      ///// Retrieve from the Container
      ///// </summary>
      //UnitInfo RetrieveUnitInfo(DisplayInstance displayInstance);

      #region ICollection<UnitInfo> Members

      // Override Default Interface Documentation

      /// <summary>
      /// Adds a UnitInfo to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The UnitInfo to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="item"/> is null.</exception>
      new void Add(UnitInfo item);

      #endregion

      #region DataContainer<T>

      void Read();

      List<UnitInfo> Read(string filePath, Plugins.IFileSerializer<List<UnitInfo>> serializer);

      void Write();

      void Write(string filePath, Plugins.IFileSerializer<List<UnitInfo>> serializer);

      #endregion
   }

   public sealed class UnitInfoCollection : DataContainer<List<UnitInfo>>, IUnitInfoCollection
   {
      #region Properties

      public override Plugins.IFileSerializer<List<UnitInfo>> DefaultSerializer
      {
         get { return new Serializers.ProtoBufFileSerializer<List<UnitInfo>>(); }
      }

      #endregion

      public UnitInfoCollection()
         : this(null)
      {

      }

      public UnitInfoCollection(IPreferenceSet prefs)
      {
         if (prefs != null && !String.IsNullOrEmpty(prefs.ApplicationDataFolderPath))
         {
            FileName = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, Constants.UnitInfoCacheFileName);
         }
      }

      #region Methods
      
      ///// <summary>
      ///// Retrieve from the Container
      ///// </summary>
      ///// <param name="displayInstance"></param>
      //public UnitInfo RetrieveUnitInfo(DisplayInstance displayInstance)
      //{
      //   return _collection.UnitInfoList.Find(displayInstance.Owns);
      //}
      
      #endregion

      #region ICollection<UnitInfo> Members

      public void Add(UnitInfo item)
      {
         if (item == null) throw new ArgumentNullException("item");

         Data.Add(item);
      }

      [CoverageExclude]
      public void Clear()
      {
         Data.Clear();
      }

      public bool Contains(UnitInfo item)
      {
         return item != null && Data.Contains(item);
      }

      [CoverageExclude]
      void ICollection<UnitInfo>.CopyTo(UnitInfo[] array, int arrayIndex)
      {
         Data.CopyTo(array, arrayIndex);
      }

      public int Count
      {
         [CoverageExclude]
         get { return Data.Count; }
      }

      bool ICollection<UnitInfo>.IsReadOnly
      {
         [CoverageExclude]
         get { return false; }
      }

      public bool Remove(UnitInfo item)
      {
         return item != null && Data.Remove(item);
      }

      #endregion

      #region IEnumerable<UnitInfo> Members

      [CoverageExclude]
      public IEnumerator<UnitInfo> GetEnumerator()
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
