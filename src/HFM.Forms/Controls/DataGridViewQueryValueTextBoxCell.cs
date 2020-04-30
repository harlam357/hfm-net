
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using HFM.Core.Data;

namespace HFM.Forms.Controls
{
   [ExcludeFromCodeCoverage]
   internal class DataGridViewQueryValueTextBoxCell : DataGridViewTextBoxCell
   {
      private bool CalendarEdit
      {
         get
         {
            if (OwningRow != null && OwningRow.Index >= 0)
            {
               var nameCell = OwningRow.Cells["Name"];
               return nameCell.Value.Equals(WorkUnitRowColumn.Assigned) ||
                      nameCell.Value.Equals(WorkUnitRowColumn.Finished);
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
