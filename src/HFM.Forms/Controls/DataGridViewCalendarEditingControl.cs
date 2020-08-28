using System;
using System.Globalization;
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
    internal class DataGridViewCalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        public DataGridViewCalendarEditingControl()
        {
            Format = DateTimePickerFormat.Custom;
            CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
        }

        public object EditingControlFormattedValue
        {
            // calling ToShortDateString() will reset the time portion of the
            // value back to 12:00:00 AM and we need the time portion of the value
            //get { return Value.ToShortDateString(); }

            // return the entire value using Invariant Culture rules
            // not tested on cultures other than en-US as of 10/7/10
            get => Value.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (value is string stringValue)
                {
                    if (DateTime.TryParse(stringValue, out var result))
                    {
                        Value = result;
                    }
                    else
                    {
                        // just use the default value so we're not left with a null value.
                        Value = DateTime.Now;
                    }
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => EditingControlFormattedValue;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            CalendarForeColor = dataGridViewCellStyle.ForeColor;
            CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex { get; set; }

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

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        public bool RepositionEditingControlOnValueChange => false;

        public DataGridView EditingControlDataGridView { get; set; }

        public bool EditingControlValueChanged { get; set; }

        public Cursor EditingPanelCursor => base.Cursor;

        protected override void OnValueChanged(EventArgs e)
        {
            // Notify the DataGridView that the contents of the cell have changed.
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(e);
        }
    }
}
