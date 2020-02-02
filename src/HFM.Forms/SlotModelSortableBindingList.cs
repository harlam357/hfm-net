﻿/*
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
using System.Collections.Generic;
using System.ComponentModel;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.Client;

namespace HFM.Forms
{
   internal class SlotModelSortableBindingList : SortableBindingList<SlotModel>
   {
      public bool OfflineClientsLast
      {
         get { return ((SlotModelSortComparer)SortComparer).OfflineClientsLast; }
         set { ((SlotModelSortComparer)SortComparer).OfflineClientsLast = value; }
      }

      public SlotModelSortableBindingList()
         : this(null)
      {

      }

      public SlotModelSortableBindingList(ISynchronizeInvoke syncObject)
         : base(syncObject)
      {
         SortComparer = new SlotModelSortComparer();
      }

      #region BindingList<T> Find Overrides

      protected override bool SupportsSearchingCore
      {
         get { return true; }
      }

      protected override int FindCore(PropertyDescriptor prop, object key)
      {
         // This key seems to always be null under Mono
         if (key == null) return -1;

         if (key is string)
         {
            var list = Items as List<SlotModel>;
            if ((null != list))
            {
               return list.FindIndex(item =>
                                     {
                                        if (prop.GetValue(item).Equals(key))
                                        {
                                           return true;
                                        }
                                        return false;
                                     });
            }

            return -1;
         }

         throw new NotSupportedException("Key must be of Type System.String.");
      }

      #endregion

      /// <summary>
      /// Sorts the items and optionally fires the ListChanged event.
      /// </summary>
      /// <param name="fireListChanged">true to fire the ListChanged event; otherwise, false.</param>
      protected override void ApplySortCoreInternal(bool fireListChanged)
      {
         base.ApplySortCoreInternal(false);
      }

      /// <summary>
      /// Gets a value indicating whether the data source supports filtering. 
      /// </summary>
      /// <returns>true if the data source supports filtering; otherwise, false.</returns>
      public override bool SupportsFiltering
      {
         get { return false; }
      }

      #region SlotModelSortComparer

      #endregion
   }
}
