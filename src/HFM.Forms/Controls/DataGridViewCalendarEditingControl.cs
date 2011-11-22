/*
 * HFM.NET - DataGridView Calendar Editing Control Class
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
using System.Globalization;
using System.Windows.Forms;

using HFM.Core;

namespace HFM.Forms.Controls
{
   [CoverageExclude]
   internal class DataGridViewCalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
   {
      private bool _valueChanged;

      public DataGridViewCalendarEditingControl()
      {
         Format = DateTimePickerFormat.Custom;
         CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
      }

      #region IDataGridViewEditingControl Members

      // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property.
      public object EditingControlFormattedValue
      {
         // calling ToShortDateString() will reset the time portion of the
         // value back to 12:00:00 AM and we need the time portion of the value
         //get { return Value.ToShortDateString(); }

         // return the entire value using Invariant Culture rules
         // not tested on cultures other than en-US as of 10/7/10
         get { return Value.ToString(CultureInfo.InvariantCulture); }
         set
         {
            if (value is String)
            {
               try
               {
                  // This will throw an exception of the string is 
                  // null, empty, or not in the format of a date.
                  Value = DateTime.Parse((String)value);
               }
               catch
               {
                  // In the case of an exception, just use the 
                  // default value so we're not left with a null value.
                  Value = DateTime.Now;
               }
            }
         }
      }

      // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method.
      public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
      {
         return EditingControlFormattedValue;
      }

      // Implements the IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
      public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
      {
         Font = dataGridViewCellStyle.Font;
         CalendarForeColor = dataGridViewCellStyle.ForeColor;
         CalendarMonthBackground = dataGridViewCellStyle.BackColor;
      }

      // Implements the IDataGridViewEditingControl.EditingControlRowIndex property.
      public int EditingControlRowIndex { get; set; }

      // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey method.
      public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
      {
         // Let the DateTimePicker handle the keys listed.
         switch (key & Keys.KeyCode)
         {
            case Keys.Left:
            case Keys.Up:
            case Keys.Down:
            case Keys.Right:
            case Keys.Home:
            case Keys.End:
            case Keys.PageDown:
            case Keys.PageUp:
               return true;
            default:
               return !dataGridViewWantsInputKey;
         }
      }

      // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit method.
      public void PrepareEditingControlForEdit(bool selectAll)
      {
         // No preparation needs to be done.
      }

      // Implements the IDataGridViewEditingControl.RepositionEditingControlOnValueChange property.
      public bool RepositionEditingControlOnValueChange
      {
         get { return false; }
      }

      // Implements the IDataGridViewEditingControl.EditingControlDataGridView property.
      public DataGridView EditingControlDataGridView { get; set; }

      // Implements the IDataGridViewEditingControl.EditingControlValueChanged property.
      public bool EditingControlValueChanged
      {
         get { return _valueChanged; }
         set { _valueChanged = value; }
      }

      // Implements the IDataGridViewEditingControl.EditingPanelCursor property.
      public Cursor EditingPanelCursor
      {
         get { return base.Cursor; }
      }

      #endregion

      protected override void OnValueChanged(EventArgs eventargs)
      {
         // Notify the DataGridView that the contents of the cell have changed.
         _valueChanged = true;
         EditingControlDataGridView.NotifyCurrentCellDirty(true);
         base.OnValueChanged(eventargs);
      }
   }
}
