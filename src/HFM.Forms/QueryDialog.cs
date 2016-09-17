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

using HFM.Core.DataTypes;
using HFM.Forms.Controls;

namespace HFM.Forms
{
   public interface IQueryView : IWin32Window
   {
      QueryParameters Query { get; set; }
      
      bool Visible { get; set; }

      DialogResult ShowDialog(IWin32Window owner);

      void Close();
   }

   public partial class QueryDialog : Form, IQueryView
   {
      private QueryParameters _query;
      private BindingList<QueryField> _queryFieldList;

      public QueryDialog()
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
            columnChoices.Add(new ListItem(QueryFieldName.ProjectID.ToString(), QueryFieldName.ProjectID));
            columnChoices.Add(new ListItem(QueryFieldName.WorkUnitName.ToString(), QueryFieldName.WorkUnitName));
            columnChoices.Add(new ListItem(QueryFieldName.Name.ToString(), QueryFieldName.Name));
            columnChoices.Add(new ListItem(QueryFieldName.Path.ToString(), QueryFieldName.Path));
            columnChoices.Add(new ListItem(QueryFieldName.Username.ToString(), QueryFieldName.Username));
            columnChoices.Add(new ListItem(QueryFieldName.Team.ToString(), QueryFieldName.Team));
            columnChoices.Add(new ListItem(QueryFieldName.SlotType.ToString(), QueryFieldName.SlotType));
            columnChoices.Add(new ListItem(QueryFieldName.Core.ToString(), QueryFieldName.Core));
            columnChoices.Add(new ListItem(QueryFieldName.CoreVersion.ToString(), QueryFieldName.CoreVersion));
            columnChoices.Add(new ListItem(QueryFieldName.FrameTime.ToString(), QueryFieldName.FrameTime));
            columnChoices.Add(new ListItem(QueryFieldName.KFactor.ToString(), QueryFieldName.KFactor));
            columnChoices.Add(new ListItem(QueryFieldName.PPD.ToString(), QueryFieldName.PPD));
            columnChoices.Add(new ListItem(QueryFieldName.DownloadDateTime.ToString(), QueryFieldName.DownloadDateTime));
            columnChoices.Add(new ListItem(QueryFieldName.CompletionDateTime.ToString(), QueryFieldName.CompletionDateTime));
            columnChoices.Add(new ListItem(QueryFieldName.Credit.ToString(), QueryFieldName.Credit));
            columnChoices.Add(new ListItem(QueryFieldName.Frames.ToString(), QueryFieldName.Frames));
            columnChoices.Add(new ListItem(QueryFieldName.FramesCompleted.ToString(), QueryFieldName.FramesCompleted));
            columnChoices.Add(new ListItem(QueryFieldName.Result.ToString(), QueryFieldName.Result));
            columnChoices.Add(new ListItem(QueryFieldName.Atoms.ToString(), QueryFieldName.Atoms));
            columnChoices.Add(new ListItem(QueryFieldName.ProjectRun.ToString(), QueryFieldName.ProjectRun));
            columnChoices.Add(new ListItem(QueryFieldName.ProjectClone.ToString(), QueryFieldName.ProjectClone));
            columnChoices.Add(new ListItem(QueryFieldName.ProjectGen.ToString(), QueryFieldName.ProjectGen));
         }
         else
         {
            string[] names = QueryField.GetColumnNames();
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.ProjectID], QueryFieldName.ProjectID));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.WorkUnitName], QueryFieldName.WorkUnitName));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Name], QueryFieldName.Name));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Path], QueryFieldName.Path));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Username], QueryFieldName.Username));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Team], QueryFieldName.Team));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.SlotType], QueryFieldName.SlotType));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Core], QueryFieldName.Core));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.CoreVersion], QueryFieldName.CoreVersion));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.FrameTime], QueryFieldName.FrameTime));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.KFactor], QueryFieldName.KFactor));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.PPD], QueryFieldName.PPD));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.DownloadDateTime], QueryFieldName.DownloadDateTime));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.CompletionDateTime], QueryFieldName.CompletionDateTime));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Credit], QueryFieldName.Credit));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Frames], QueryFieldName.Frames));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.FramesCompleted], QueryFieldName.FramesCompleted));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Result], QueryFieldName.Result));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.Atoms], QueryFieldName.Atoms));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.ProjectRun], QueryFieldName.ProjectRun));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.ProjectClone], QueryFieldName.ProjectClone));
            columnChoices.Add(new ListItem(names[(int)QueryFieldName.ProjectGen], QueryFieldName.ProjectGen));
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
}
