/*
 * HFM.NET - Work Unit History Query UI Form
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
using System.Windows.Forms;

using HFM.Core.Data;
using HFM.Forms.Controls;

namespace HFM.Forms
{
   public interface IQueryView : IWin32Window
   {
      WorkUnitHistoryQuery Query { get; set; }
      
      bool Visible { get; set; }

      DialogResult ShowDialog(IWin32Window owner);

      void Close();
   }

   public partial class QueryDialog : Form, IQueryView
   {
      private WorkUnitHistoryQuery _workUnitHistoryQuery;
      private BindingList<WorkUnitHistoryQueryParameter> _parametersList;

      public QueryDialog()
      {
         InitializeComponent();
         SetupDataGridViewColumns();
         dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

         Query = new WorkUnitHistoryQuery();
      }

      #region IQueryView Members

      public WorkUnitHistoryQuery Query
      {
         get { return _workUnitHistoryQuery; }
         set
         {
            _workUnitHistoryQuery = value;
            _parametersList = new BindingList<WorkUnitHistoryQueryParameter>(_workUnitHistoryQuery.Parameters);
            BindNameTextBox(_workUnitHistoryQuery);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _parametersList;
         }
      }

      #endregion

      public void BindNameTextBox(WorkUnitHistoryQuery parameters)
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
         queryColumn.DisplayMember = "DisplayMember";
         queryColumn.ValueMember = "ValueMember";
         queryColumn.DataPropertyName = "Name";
         queryColumn.Width = 150;
         dataGridView1.Columns.Add(queryColumn);

         queryColumn = new DataGridViewComboBoxColumn();
         columnChoices = GetOperatorFieldChoices();
         queryColumn.Name = "Operator";
         queryColumn.HeaderText = "Operator";
         queryColumn.DataSource = columnChoices;
         queryColumn.DisplayMember = "DisplayMember";
         queryColumn.ValueMember = "ValueMember";
         queryColumn.DataPropertyName = "Type";
         queryColumn.Width = 175;
         dataGridView1.Columns.Add(queryColumn);

         var valueColumn = new DataGridViewQueryValueColumn();
         valueColumn.Name = "Value";
         valueColumn.HeaderText = "Value";
         valueColumn.DataPropertyName = "Value";
         valueColumn.DefaultCellStyle.DataSourceNullValue = null;
         //valueColumn.Width = 200;
         valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         dataGridView1.Columns.Add(valueColumn);
      }

      private static List<ListItem> GetQueryFieldChoices()
      {
         var columnChoices = new List<ListItem>();
         
         // On Mono the ComboBox choices must exactly match the enumeration name - LAME!!!
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.ProjectID.ToString(), WorkUnitHistoryRowColumn.ProjectID));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.WorkUnitName.ToString(), WorkUnitHistoryRowColumn.WorkUnitName));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Name.ToString(), WorkUnitHistoryRowColumn.Name));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Path.ToString(), WorkUnitHistoryRowColumn.Path));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Username.ToString(), WorkUnitHistoryRowColumn.Username));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Team.ToString(), WorkUnitHistoryRowColumn.Team));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.SlotType.ToString(), WorkUnitHistoryRowColumn.SlotType));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Core.ToString(), WorkUnitHistoryRowColumn.Core));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.CoreVersion.ToString(), WorkUnitHistoryRowColumn.CoreVersion));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.FrameTime.ToString(), WorkUnitHistoryRowColumn.FrameTime));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.KFactor.ToString(), WorkUnitHistoryRowColumn.KFactor));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.PPD.ToString(), WorkUnitHistoryRowColumn.PPD));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.DownloadDateTime.ToString(), WorkUnitHistoryRowColumn.DownloadDateTime));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.CompletionDateTime.ToString(), WorkUnitHistoryRowColumn.CompletionDateTime));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Credit.ToString(), WorkUnitHistoryRowColumn.Credit));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Frames.ToString(), WorkUnitHistoryRowColumn.Frames));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.FramesCompleted.ToString(), WorkUnitHistoryRowColumn.FramesCompleted));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Result.ToString(), WorkUnitHistoryRowColumn.Result));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.Atoms.ToString(), WorkUnitHistoryRowColumn.Atoms));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.ProjectRun.ToString(), WorkUnitHistoryRowColumn.ProjectRun));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.ProjectClone.ToString(), WorkUnitHistoryRowColumn.ProjectClone));
            columnChoices.Add(new ListItem(WorkUnitHistoryRowColumn.ProjectGen.ToString(), WorkUnitHistoryRowColumn.ProjectGen));
         }
         else
         {
            string[] names = WorkUnitHistoryQueryParameter.GetColumnNames();
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.ProjectID], WorkUnitHistoryRowColumn.ProjectID));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.WorkUnitName], WorkUnitHistoryRowColumn.WorkUnitName));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Name], WorkUnitHistoryRowColumn.Name));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Path], WorkUnitHistoryRowColumn.Path));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Username], WorkUnitHistoryRowColumn.Username));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Team], WorkUnitHistoryRowColumn.Team));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.SlotType], WorkUnitHistoryRowColumn.SlotType));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Core], WorkUnitHistoryRowColumn.Core));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.CoreVersion], WorkUnitHistoryRowColumn.CoreVersion));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.FrameTime], WorkUnitHistoryRowColumn.FrameTime));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.KFactor], WorkUnitHistoryRowColumn.KFactor));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.PPD], WorkUnitHistoryRowColumn.PPD));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.DownloadDateTime], WorkUnitHistoryRowColumn.DownloadDateTime));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.CompletionDateTime], WorkUnitHistoryRowColumn.CompletionDateTime));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Credit], WorkUnitHistoryRowColumn.Credit));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Frames], WorkUnitHistoryRowColumn.Frames));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.FramesCompleted], WorkUnitHistoryRowColumn.FramesCompleted));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Result], WorkUnitHistoryRowColumn.Result));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.Atoms], WorkUnitHistoryRowColumn.Atoms));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.ProjectRun], WorkUnitHistoryRowColumn.ProjectRun));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.ProjectClone], WorkUnitHistoryRowColumn.ProjectClone));
            columnChoices.Add(new ListItem(names[(int)WorkUnitHistoryRowColumn.ProjectGen], WorkUnitHistoryRowColumn.ProjectGen));
         }

         return columnChoices;
      }
      
      private static List<ListItem> GetOperatorFieldChoices()
      {
         var columnChoices = new List<ListItem>();
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new ListItem(QueryFieldType.Equal.ToString(), QueryFieldType.Equal));
            columnChoices.Add(new ListItem(QueryFieldType.NotEqual.ToString(), QueryFieldType.NotEqual));
            columnChoices.Add(new ListItem(QueryFieldType.GreaterThan.ToString(), QueryFieldType.GreaterThan));
            columnChoices.Add(new ListItem(QueryFieldType.GreaterThanOrEqual.ToString(), QueryFieldType.GreaterThanOrEqual));
            columnChoices.Add(new ListItem(QueryFieldType.LessThan.ToString(), QueryFieldType.LessThan));
            columnChoices.Add(new ListItem(QueryFieldType.LessThanOrEqual.ToString(), QueryFieldType.LessThanOrEqual));
            columnChoices.Add(new ListItem(QueryFieldType.Like.ToString(), QueryFieldType.Like));
            columnChoices.Add(new ListItem(QueryFieldType.NotLike.ToString(), QueryFieldType.NotLike));
         }
         else
         {
            columnChoices.Add(new ListItem("Equal", QueryFieldType.Equal));
            columnChoices.Add(new ListItem("Not Equal", QueryFieldType.NotEqual));
            columnChoices.Add(new ListItem("Greater Than", QueryFieldType.GreaterThan));
            columnChoices.Add(new ListItem("Greater Than Or Equal", QueryFieldType.GreaterThanOrEqual));
            columnChoices.Add(new ListItem("Less Than", QueryFieldType.LessThan));
            columnChoices.Add(new ListItem("Less Than Or Equal", QueryFieldType.LessThanOrEqual));
            columnChoices.Add(new ListItem("Like", QueryFieldType.Like));
            columnChoices.Add(new ListItem("Not Like", QueryFieldType.NotLike));
         }

         return columnChoices;
      }

      private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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
         _workUnitHistoryQuery.Parameters.Add(new WorkUnitHistoryQueryParameter());
         RefreshDisplay();
      }

      private void btnRemove_Click(object sender, EventArgs e)
      {
         Debug.Assert(dataGridView1.SelectedCells.Count == 1);
         foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
         {
            _workUnitHistoryQuery.Parameters.RemoveAt(cell.OwningRow.Index);
         }
         RefreshDisplay();
      }

      private void RefreshDisplay()
      {
         _parametersList.ResetBindings();
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
}
