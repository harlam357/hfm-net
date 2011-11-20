/*
 * HFM.NET - Work Unit History Query UI Form
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms
{
   public interface IQueryView : IWin32Window
   {
      QueryParameters Query { get; set; }
      
      bool Visible { get; set; }

      DialogResult ShowDialog(IWin32Window owner);

      void Close();
   }

   // ReSharper disable InconsistentNaming
   public partial class frmQuery : Form, IQueryView
   // ReSharper restore InconsistentNaming
   {
      private QueryParameters _query;
      private BindingList<QueryField> _queryFieldList;

      public frmQuery()
      {
         InitializeComponent();
         SetupDataGridViewColumns();
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
            _queryFieldList = new BindingList<QueryField>(_query.Fields);
            BindNameTextBox(_query);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _queryFieldList;
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
         dataGridView1.AutoGenerateColumns = false;

         var queryColumn = new DataGridViewComboBoxColumn();
         var columnChoices = GetQueryFieldChoices();
         queryColumn.Name = "Name";
         queryColumn.HeaderText = "Name";
         queryColumn.DataSource = columnChoices;
         queryColumn.DisplayMember = "Display";
         queryColumn.ValueMember = "Value";
         queryColumn.DataPropertyName = "Name";
         queryColumn.Width = 150;
         dataGridView1.Columns.Add(queryColumn);

         queryColumn = new DataGridViewComboBoxColumn();
         columnChoices = GetOperatorFieldChoices();
         queryColumn.Name = "Operator";
         queryColumn.HeaderText = "Operator";
         queryColumn.DataSource = columnChoices;
         queryColumn.DisplayMember = "Display";
         queryColumn.ValueMember = "Value";
         queryColumn.DataPropertyName = "Type";
         queryColumn.Width = 175;
         dataGridView1.Columns.Add(queryColumn);

         var valueColumn = new ValueColumn();
         valueColumn.Name = "Value";
         valueColumn.HeaderText = "Value";
         valueColumn.DataPropertyName = "Value";
         valueColumn.DefaultCellStyle.DataSourceNullValue = null;
         //valueColumn.Width = 200;
         valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         dataGridView1.Columns.Add(valueColumn);
      }
      
      private static List<Choice> GetQueryFieldChoices()
      {
         var columnChoices = new List<Choice>();
         
         // On Mono the ComboBox choices must exactly match the enumeration name - LAME!!!
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new Choice(QueryFieldName.ProjectID.ToString(), QueryFieldName.ProjectID));
            columnChoices.Add(new Choice(QueryFieldName.WorkUnitName.ToString(), QueryFieldName.WorkUnitName));
            columnChoices.Add(new Choice(QueryFieldName.InstanceName.ToString(), QueryFieldName.InstanceName));
            columnChoices.Add(new Choice(QueryFieldName.InstancePath.ToString(), QueryFieldName.InstancePath));
            columnChoices.Add(new Choice(QueryFieldName.Username.ToString(), QueryFieldName.Username));
            columnChoices.Add(new Choice(QueryFieldName.Team.ToString(), QueryFieldName.Team));
            columnChoices.Add(new Choice(QueryFieldName.ClientType.ToString(), QueryFieldName.ClientType));
            columnChoices.Add(new Choice(QueryFieldName.Core.ToString(), QueryFieldName.Core));
            columnChoices.Add(new Choice(QueryFieldName.CoreVersion.ToString(), QueryFieldName.CoreVersion));
            columnChoices.Add(new Choice(QueryFieldName.FrameTime.ToString(), QueryFieldName.FrameTime));
            columnChoices.Add(new Choice(QueryFieldName.KFactor.ToString(), QueryFieldName.KFactor));
            columnChoices.Add(new Choice(QueryFieldName.PPD.ToString(), QueryFieldName.PPD));
            columnChoices.Add(new Choice(QueryFieldName.DownloadDateTime.ToString(), QueryFieldName.DownloadDateTime));
            columnChoices.Add(new Choice(QueryFieldName.CompletionDateTime.ToString(), QueryFieldName.CompletionDateTime));
            columnChoices.Add(new Choice(QueryFieldName.Credit.ToString(), QueryFieldName.Credit));
            columnChoices.Add(new Choice(QueryFieldName.Frames.ToString(), QueryFieldName.Frames));
            columnChoices.Add(new Choice(QueryFieldName.FramesCompleted.ToString(), QueryFieldName.FramesCompleted));
            columnChoices.Add(new Choice(QueryFieldName.Result.ToString(), QueryFieldName.Result));
            columnChoices.Add(new Choice(QueryFieldName.Atoms.ToString(), QueryFieldName.Atoms));
            columnChoices.Add(new Choice(QueryFieldName.ProjectRun.ToString(), QueryFieldName.ProjectRun));
            columnChoices.Add(new Choice(QueryFieldName.ProjectClone.ToString(), QueryFieldName.ProjectClone));
            columnChoices.Add(new Choice(QueryFieldName.ProjectGen.ToString(), QueryFieldName.ProjectGen));
         }
         else
         {
            var names = HistoryPresenter.GetQueryFieldColumnNames();
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
         }

         return columnChoices;
      }
      
      private static List<Choice> GetOperatorFieldChoices()
      {
         var columnChoices = new List<Choice>();
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new Choice(QueryFieldType.Equal.ToString(), QueryFieldType.Equal));
            columnChoices.Add(new Choice(QueryFieldType.GreaterThan.ToString(), QueryFieldType.GreaterThan));
            columnChoices.Add(new Choice(QueryFieldType.GreaterThanOrEqual.ToString(), QueryFieldType.GreaterThanOrEqual));
            columnChoices.Add(new Choice(QueryFieldType.LessThan.ToString(), QueryFieldType.LessThan));
            columnChoices.Add(new Choice(QueryFieldType.LessThanOrEqual.ToString(), QueryFieldType.LessThanOrEqual));
         }
         else
         {
            columnChoices.Add(new Choice("Equal", QueryFieldType.Equal));
            columnChoices.Add(new Choice("Greater Than", QueryFieldType.GreaterThan));
            columnChoices.Add(new Choice("Greater Than Or Equal", QueryFieldType.GreaterThanOrEqual));
            columnChoices.Add(new Choice("Less Than", QueryFieldType.LessThan));
            columnChoices.Add(new Choice("Less Than Or Equal", QueryFieldType.LessThanOrEqual));
         }

         return columnChoices;
      }

      void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         // column is query field
         if (e.ColumnIndex == 0)
         {
            // clear the value cell (this works for .NET and Mono)
            dataGridView1["Value", e.RowIndex].Value = null;
         }
      }

      private void btnAdd_Click(object sender, EventArgs e)
      {
         _query.Fields.Add(new QueryField());
         RefreshDisplay();
      }

      private void btnRemove_Click(object sender, EventArgs e)
      {
         Debug.Assert(dataGridView1.SelectedCells.Count == 1);
         foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
         {
            _query.Fields.RemoveAt(cell.OwningRow.Index);
         }
         RefreshDisplay();
      }

      private void RefreshDisplay()
      {
         //if (dataGridView1.DataSource != null)
         //{
         //   var cm = (CurrencyManager)dataGridView1.BindingContext[dataGridView1.DataSource];
         //   cm.Refresh();
         //}
         _queryFieldList.ResetBindings();
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
   }

   internal class Choice : IComparable<Choice>
   {
      public Choice(string display, object value)
      {
         Display = display;
         Value = value;
      }

      public string Display { get; private set; }
      public object Value { get; private set; }

      #region IComparable<Choice> Members

      public int CompareTo(Choice other)
      {
         return Display.CompareTo(other.Display);
      }

      #endregion
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
            var ctl = DataGridView.EditingControl as CalendarEditingControl;

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
   }

   internal class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
   {
      private bool _valueChanged;

      public CalendarEditingControl()
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
