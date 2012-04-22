/*
 * HFM.NET - Slot Model Sortable Binding List Class
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
using System.ComponentModel;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms
{
   [CoverageExclude]
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

      private class SlotModelSortComparer : SortComparer<SlotModel>
      {
         public bool OfflineClientsLast { get; set; }

         public override bool SupportsAdvancedSorting
         {
            get { return false; }
         }

         protected override int CompareInternal(SlotModel xVal, SlotModel yVal)
         {
            /* Get property values */
            object xValue = GetPropertyValue(xVal, Property);
            object yValue = GetPropertyValue(yVal, Property);
            SlotStatus xStatusValue = xVal.Status;
            SlotStatus yStatusValue = yVal.Status;
            object xNameValue = xVal.Name;
            object yNameValue = yVal.Name;

            // check for offline clients first
            if (OfflineClientsLast)
            {
               if ((xStatusValue).Equals(SlotStatus.Offline) &&
                   (yStatusValue).Equals(SlotStatus.Offline) == false)
               {
                  return 1;
               }
               if ((yStatusValue).Equals(SlotStatus.Offline) &&
                   (xStatusValue).Equals(SlotStatus.Offline) == false)
               {
                  return -1;
               }
            }

            int returnValue;

            /* Determine sort order */
            if (Direction == ListSortDirection.Ascending)
            {
               returnValue = CompareAscending(xValue, yValue);
            }
            else
            {
               returnValue = CompareDescending(xValue, yValue);
            }

            // if values are equal, sort via the client name (asc)
            if (returnValue == 0)
            {
               returnValue = CompareAscending(xNameValue, yNameValue);
            }

            return returnValue;
         }
      }

      #endregion
   }
}
