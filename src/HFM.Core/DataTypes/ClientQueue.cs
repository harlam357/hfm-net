/*
 * HFM.NET - Client Queue Class
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

/*
 * This class is currently populated by the queue.dat data only.
 * 
 * The reason for its existance is to provide a buffer for the
 * queue.dat data coming from v6 and below clients as well as a
 * buffer for the data (presumably different) coming from v7 clients.
 * 
 * This class also provides a concrete type that can be serailized
 * into a client data file.  Previously the queue information that
 * was available to a DisplayInstance was of type IQueueBase which
 * is not serializable by protobuf-net.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HFM.Core.DataTypes
{
   [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
   public class ClientQueue
   {
      #region Fields
   
      private readonly ClientQueueEntry[] _entries;
      
      #endregion

      #region Constructor

      public ClientQueue()
      {
         _entries = new ClientQueueEntry[10]; //TODO: Fix Hard Coded 10
         for (int i = 0; i < _entries.Length; i++)
         {
            _entries[i] = new ClientQueueEntry();
         }
      }
      
      #endregion

      #region queue.dat Properties

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

      /// <summary>
      /// Get the QueueEntry at the specified Index.
      /// </summary>
      /// <param name="index">Queue Entry Index</param>
      public ClientQueueEntry GetQueueEntry(int index)
      {
         return _entries[index];
      }
      
      #endregion

      /// <summary>
      /// Collection used to populate UI Controls
      /// </summary>
      public ICollection<string> EntryNameCollection
      {
         get
         {
            var list = new List<string>(10);

            for (int i = 0; i < 10; i++)
            {
               list.Add(String.Format(CultureInfo.InvariantCulture,
                  "{0} - {1}", i, GetQueueEntry(i).ProjectRunCloneGen()));
            }

            return list;
         }
      }
   }
}
