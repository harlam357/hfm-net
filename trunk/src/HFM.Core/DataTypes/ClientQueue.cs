/*
 * HFM.NET - Client Queue Class
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
using System.Linq;

namespace HFM.Core.DataTypes
{
   [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
   public class ClientQueue : IDictionary<int, ClientQueueEntry>
   {
      #region Fields
   
      private readonly Dictionary<int, ClientQueueEntry> _entries;
      
      #endregion

      #region Constructor

      public ClientQueue()
      {
         _entries = new Dictionary<int, ClientQueueEntry>();
      }
      
      #endregion

      #region queue.dat Properties

      /// <summary>
      /// Client type that generated this queue
      /// </summary>
      public ClientType ClientType { get; set; }

      /// <summary>
      /// Current index number
      /// </summary>
      public int CurrentIndex { get; set; }

      /// <summary>
      /// Performance fraction
      /// </summary>
      public float PerformanceFraction { get; set; }

      /// <summary>
      /// Performance fraction unit weight
      /// </summary>
      public int PerformanceFractionUnitWeight { get; set; }

      /// <summary>
      /// Download rate sliding average
      /// </summary>
      public float DownloadRateAverage { get; set; }

      /// <summary>
      /// Download rate unit weight
      /// </summary>
      public int DownloadRateUnitWeight { get; set; }

      /// <summary>
      /// Upload rate sliding average
      /// </summary>
      public float UploadRateAverage { get; set; }

      /// <summary>
      /// Upload rate unit weight
      /// </summary>
      public int UploadRateUnitWeight { get; set; }

      #endregion
      
      #region Entry Accessors

      /// <summary>
      /// Get the Current QueueEntry.
      /// </summary>
      public ClientQueueEntry CurrentQueueEntry
      {
         get { return _entries[CurrentIndex]; }
      }

      #endregion

      /// <summary>
      /// Collection used to populate UI Controls
      /// </summary>
      public IEnumerable<ListItem> EntryNameCollection
      {
         get
         {
            return _entries.Select(kvp => new ListItem
                                          {
                                             DisplayMember = String.Format(CultureInfo.InvariantCulture, "{0} - {1}", kvp.Key, kvp.Value.ProjectRunCloneGen()), ValueMember = kvp.Key
                                          }).ToList().AsReadOnly();
         }
      }

      #region IDictionary<int,ClientQueueEntry> Members

      public void Add(int key, ClientQueueEntry value)
      {
         _entries.Add(key, value);
      }

      public bool ContainsKey(int key)
      {
         return _entries.ContainsKey(key);
      }

      public ICollection<int> Keys
      {
         get { return _entries.Keys; }
      }

      public bool Remove(int key)
      {
         return _entries.Remove(key);
      }

      public bool TryGetValue(int key, out ClientQueueEntry value)
      {
         return _entries.TryGetValue(key, out value);
      }

      public ICollection<ClientQueueEntry> Values
      {
         get { return _entries.Values; }
      }

      public ClientQueueEntry this[int key]
      {
         get { return _entries[key]; }
         set { _entries[key] = value; }
      }

      #endregion

      #region ICollection<KeyValuePair<int,ClientQueueEntry>> Members

      void ICollection<KeyValuePair<int, ClientQueueEntry>>.Add(KeyValuePair<int, ClientQueueEntry> item)
      {
         throw new NotImplementedException();
      }

      public void Clear()
      {
         _entries.Clear();
      }

      bool ICollection<KeyValuePair<int, ClientQueueEntry>>.Contains(KeyValuePair<int, ClientQueueEntry> item)
      {
         throw new NotImplementedException();
      }

      void ICollection<KeyValuePair<int, ClientQueueEntry>>.CopyTo(KeyValuePair<int, ClientQueueEntry>[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         get { return _entries.Count; }
      }

      bool ICollection<KeyValuePair<int, ClientQueueEntry>>.IsReadOnly
      {
         get { return false; }
      }

      bool ICollection<KeyValuePair<int, ClientQueueEntry>>.Remove(KeyValuePair<int, ClientQueueEntry> item)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<KeyValuePair<int,ClientQueueEntry>> Members

      public IEnumerator<KeyValuePair<int, ClientQueueEntry>> GetEnumerator()
      {
         return _entries.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
