/*
 * HFM.NET - History Entry Sortable Binding List Class
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

using System.ComponentModel;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms
{
   [CoverageExclude]
   internal class HistoryEntrySortableBindingList : SortableBindingList<HistoryEntry>
   {
      public HistoryEntrySortableBindingList()
      {
         SortComparer = new HistoryEntrySortComparer();
      }

      #region HistoryEntrySortComparer

      [CoverageExclude]
      private class HistoryEntrySortComparer : SortComparer<HistoryEntry>
      {
         protected override int CompareInternal(HistoryEntry xVal, HistoryEntry yVal)
         {
            /* Get property values */
            object xValue = GetPropertyValue(xVal, Property);
            object yValue = GetPropertyValue(yVal, Property);
            long xIdValue = xVal.ID;
            long yIdValue = yVal.ID;

            int result;

            /* Determine sort order */
            if (Direction == ListSortDirection.Ascending)
            {
               result = CompareAscending(xValue, yValue);
            }
            else
            {
               result = CompareDescending(xValue, yValue);
            }

            // if values are equal, sort via the entry id (asc)
            if (result == 0)
            {
               result = CompareAscending(xIdValue, yIdValue);
            }

            return result;
         }

         protected override int RecursiveCompareInternal(HistoryEntry xVal, HistoryEntry yVal, int index)
         {
            if (index >= SortDescriptions.Count)
            {
               return 0; // termination condition
            }

            /* Get property values */
            ListSortDescription listSortDesc = SortDescriptions[index];
            object xValue = listSortDesc.PropertyDescriptor.GetValue(xVal);
            object yValue = listSortDesc.PropertyDescriptor.GetValue(yVal);
            long xIdValue = xVal.ID;
            long yIdValue = yVal.ID;

            int result;
            /* Determine sort order */
            if (listSortDesc.SortDirection == ListSortDirection.Ascending)
            {
               result = CompareAscending(xValue, yValue);
            }
            else
            {
               result = CompareDescending(xValue, yValue);
            }

            /* If the properties are equal, compare the next property */
            if (result == 0)
            {
               result = RecursiveCompareInternal(xVal, yVal, ++index);
            }

            // if values are equal, sort via the entry id (asc)
            if (result == 0)
            {
               result = CompareAscending(xIdValue, yIdValue);
            }

            return result;
         }
      }

      #endregion
   }
}
