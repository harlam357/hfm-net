/*
 * HFM.NET - DataGridView Query Value Text Box Cell Class
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
using System.Windows.Forms;

using HFM.Core;

namespace HFM.Forms.Controls
{
   [CoverageExclude]
   internal class DataGridViewQueryValueTextBoxCell : DataGridViewTextBoxCell
   {
      private bool CalendarEdit
      {
         get
         {
            if (OwningRow != null && OwningRow.Index >= 0)
            {
               var nameCell = OwningRow.Cells["Name"];
               return nameCell.Value.Equals(QueryFieldName.DownloadDateTime) ||
                      nameCell.Value.Equals(QueryFieldName.CompletionDateTime);
            }

            return false;
         }
      }

      public override void InitializeEditingControl(int rowIndex, object
          initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
      {
         if (Core.Application.IsRunningOnMono) return;

         // Set the value of the editing control to the current cell value.
         base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

         if (CalendarEdit)
         {
            var ctl = DataGridView.EditingControl as DataGridViewCalendarEditingControl;

            // if a DateTime Value is present
            if (Value != null && Value is DateTime)
            {
               ctl.Value = (DateTime)Value;
            }
            else
            {
               ctl.Value = (DateTime)DefaultNewRowValue;
            }
         }
      }

      public override Type EditType
      {
         get
         {
            if (CalendarEdit)
            {
               // Return the type of the editing control that ValueCell uses.
               return typeof(DataGridViewCalendarEditingControl);
            }
            return base.EditType;
         }
      }

      public override Type ValueType
      {
         get
         {
            if (CalendarEdit)
            {
               // Return the type of the value that ValueCell contains.
               return typeof(DateTime);
            }
            return base.ValueType;
         }
      }

      public override object DefaultNewRowValue
      {
         get
         {
            if (CalendarEdit)
            {
               // Use the current date and time as the default value.
               return DateTime.Now;
            }
            return base.DefaultNewRowValue;
         }
      }
   }
}
