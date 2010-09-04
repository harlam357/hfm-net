/*
 * HFM.NET - Work Unit History Query UI Form
 * Copyright (C) 2010 Ryan Harlamert (harlam357)
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
using System.Windows.Forms;

using HFM.Framework;
using HFM.Instances;

namespace HFM.Forms
{
   public interface IQueryView : IWin32Window
   {
      QueryParameters Query { get; set; }
      
      bool Visible { get; set; }

      DialogResult ShowDialog(IWin32Window owner);

      void Close();
   }

   public partial class frmQuery : Form, IQueryView
   {
      private QueryParameters _query;

      public frmQuery()
      {
         InitializeComponent();
         SetupDataGridViewColumns();
         //dataGridView1.DataError += dataGridView1_DataError;
         dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

         Query = new QueryParameters();
      }

      #region IQueryView Members

      public QueryParameters Query
      {
         get { return _query; }
         set
         {
            _query = value;
            BindNameTextBox(_query);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _query.Fields;
         }
      }

      #endregion

      public void BindNameTextBox(QueryParameters parameters)
      {
         txtName.DataBindings.Clear();
         txtName.DataBindings.Add("Text", parameters, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
      }

      private void SetupDataGridViewColumns()
      {
         var names = PlatformOps.GetQueryFieldColumnNames();
      
         dataGridView1.AutoGenerateColumns = false;

         var queryFieldColumn = new DataGridViewComboBoxColumn();
         var columnChoices = new List<Choice>();
         columnChoices.Add(new Choice(names[(int)QueryFieldName.ProjectID], QueryFieldName.ProjectID));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.WorkUnitName], QueryFieldName.WorkUnitName));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.InstanceName], QueryFieldName.InstanceName));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.InstancePath], QueryFieldName.InstancePath));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.Username], QueryFieldName.Username));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.Team], QueryFieldName.Team));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.ClientType], QueryFieldName.ClientType));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.Core], QueryFieldName.Core));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.CoreVersion], QueryFieldName.CoreVersion));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.FrameTime], QueryFieldName.FrameTime));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.KFactor], QueryFieldName.KFactor));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.PPD], QueryFieldName.PPD));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.DownloadDateTime], QueryFieldName.DownloadDateTime));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.CompletionDateTime], QueryFieldName.CompletionDateTime));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.Credit], QueryFieldName.Credit));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.Frames], QueryFieldName.Frames));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.FramesCompleted], QueryFieldName.FramesCompleted));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.Result], QueryFieldName.Result));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.Atoms], QueryFieldName.Atoms));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.ProjectRun], QueryFieldName.ProjectRun));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.ProjectClone], QueryFieldName.ProjectClone));
         columnChoices.Add(new Choice(names[(int)QueryFieldName.ProjectGen], QueryFieldName.ProjectGen));
         
         queryFieldColumn.Name = "Name";
         queryFieldColumn.HeaderText = "Name";
         queryFieldColumn.DataSource = columnChoices;
         queryFieldColumn.DisplayMember = "Display";
         queryFieldColumn.ValueMember = "Value";
         queryFieldColumn.DataPropertyName = "Name";
         queryFieldColumn.Width = 125;
         dataGridView1.Columns.Add(queryFieldColumn);

         var queryTypeColumn = new DataGridViewComboBoxColumn();
         columnChoices = new List<Choice>();
         columnChoices.Add(new Choice("Equal", QueryFieldType.Equal));
         columnChoices.Add(new Choice("Greater Than", QueryFieldType.GreaterThan));
         columnChoices.Add(new Choice("Greater Than Or Equal", QueryFieldType.GreaterThanOrEqual));
         columnChoices.Add(new Choice("Less Than", QueryFieldType.LessThan));
         columnChoices.Add(new Choice("Less Than Or Equal", QueryFieldType.LessThanOrEqual));
         queryTypeColumn.Name = "Operator";
         queryTypeColumn.HeaderText = "Operator";
         queryTypeColumn.DataSource = columnChoices;
         queryTypeColumn.DisplayMember = "Display";
         queryTypeColumn.ValueMember = "Value";
         queryTypeColumn.DataPropertyName = "Type";
         queryTypeColumn.Width = 150;
         dataGridView1.Columns.Add(queryTypeColumn);

         var valueColumn = new ValueColumn();
         valueColumn.Name = "Value";
         valueColumn.HeaderText = "Value";
         valueColumn.DataPropertyName = "Value";
         //valueColumn.Width = 200;
         valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         dataGridView1.Columns.Add(valueColumn);
      }

      void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         //var cell = dataGridView1[e.ColumnIndex, e.RowIndex];
         //if (cell.OwningColumn.Name == "Name")
         //{
         //   var valueCell = (ValueCell)cell.OwningRow.Cells["Value"];
         //   if (cell.Value.Equals(QueryFieldName.DownloadDateTime) ||
         //       cell.Value.Equals(QueryFieldName.CompletionDateTime))
         //   {
         //      valueCell.CalendarEdit = true;
         //   }
         //   else
         //   {
         //      valueCell.CalendarEdit = false;
         //   }
         //}
      }

      private void btnAdd_Click(object sender, EventArgs e)
      {
         _query.Fields.Add(new QueryField());
         RefreshDisplay();
      }

      private void btnRemove_Click(object sender, EventArgs e)
      {
         foreach (DataGridViewRow row in dataGridView1.SelectedRows)
         {
            _query.Fields.RemoveAt(row.Index);
         }
         RefreshDisplay();
      }

      private void RefreshDisplay()
      {
         if (dataGridView1.DataSource != null)
         {
            var cm = (CurrencyManager)dataGridView1.BindingContext[dataGridView1.DataSource];
            cm.Refresh();
         }
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      //private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
      //{
      
      //}
   }

   internal class Choice
   {
      public Choice(string display, object value)
      {
         Display = display;
         Value = value;
      }

      public string Display { get; private set; }
      public object Value { get; private set; }
   }

   public class ValueColumn : DataGridViewColumn
   {
      public ValueColumn()
         : base(new ValueCell())
      {
      
      }

      public override DataGridViewCell CellTemplate
      {
         get { return base.CellTemplate; }
         set
         {
            // Ensure that the cell used for the template is a ValueCell.
            if (value != null && !value.GetType().IsAssignableFrom(typeof(ValueCell)))
            {
               throw new InvalidCastException("Must be a ValueCell");
            }
            base.CellTemplate = value;
         }
      }
   }

   public class ValueCell : DataGridViewTextBoxCell
   {
      //private bool _calendarEdit;
      
      //public bool CalendarEdit
      //{
      //   get { return _calendarEdit; }
      //   set
      //   {
      //      _calendarEdit = value;
      //      if (_calendarEdit)
      //      {
      //         // Use the short date format.
      //         Style.Format = "d";
      //      }
      //      else
      //      {
      //         Style.Format = String.Empty;
      //      }
      //   }
      //}
      
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

      public override Type EditType
      {
         get
         {
            if (CalendarEdit)
            {
               // Return the type of the editing control that ValueCell uses.
               return typeof(CalendarEditingControl);
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

      public override void InitializeEditingControl(int rowIndex, object initialFormattedValue,
                                                    DataGridViewCellStyle dataGridViewCellStyle)
      {
         // Set the value of the editing control to the current cell value.
         base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
         
         if (CalendarEdit)
         {
            var ctl = DataGridView.EditingControl as CalendarEditingControl;

            // Use the default row value when Value property is null.
            if (Value == null)
            {
               ctl.Value = (DateTime)DefaultNewRowValue;
            }
            else
            {
               ctl.Value = (DateTime)Value;
            }
         }
      }
   }

   internal class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
   {
      private bool _valueChanged;

      public CalendarEditingControl()
      {
         Format = DateTimePickerFormat.Short;
      }

      #region IDataGridViewEditingControl Members

      // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property.
      public object EditingControlFormattedValue
      {
         get { return Value.ToShortDateString(); }
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
