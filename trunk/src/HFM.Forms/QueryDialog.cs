/*
 * HFM.NET - Work Unit History Query UI Form
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
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

using HFM.Core;
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

         var valueColumn = new DataGridViewQueryValueColumn();
         valueColumn.Name = "Value";
         valueColumn.HeaderText = "Value";
         valueColumn.DataPropertyName = "Value";
         valueColumn.DefaultCellStyle.DataSourceNullValue = null;
         //valueColumn.Width = 200;
         valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         dataGridView1.Columns.Add(valueColumn);
      }
      
      private static List<QueryColumnChoice> GetQueryFieldChoices()
      {
         var columnChoices = new List<QueryColumnChoice>();
         
         // On Mono the ComboBox choices must exactly match the enumeration name - LAME!!!
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.ProjectID.ToString(), QueryFieldName.ProjectID));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.WorkUnitName.ToString(), QueryFieldName.WorkUnitName));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Name.ToString(), QueryFieldName.Name));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Path.ToString(), QueryFieldName.Path));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Username.ToString(), QueryFieldName.Username));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Team.ToString(), QueryFieldName.Team));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.SlotType.ToString(), QueryFieldName.SlotType));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Core.ToString(), QueryFieldName.Core));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.CoreVersion.ToString(), QueryFieldName.CoreVersion));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.FrameTime.ToString(), QueryFieldName.FrameTime));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.KFactor.ToString(), QueryFieldName.KFactor));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.PPD.ToString(), QueryFieldName.PPD));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.DownloadDateTime.ToString(), QueryFieldName.DownloadDateTime));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.CompletionDateTime.ToString(), QueryFieldName.CompletionDateTime));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Credit.ToString(), QueryFieldName.Credit));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Frames.ToString(), QueryFieldName.Frames));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.FramesCompleted.ToString(), QueryFieldName.FramesCompleted));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Result.ToString(), QueryFieldName.Result));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.Atoms.ToString(), QueryFieldName.Atoms));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.ProjectRun.ToString(), QueryFieldName.ProjectRun));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.ProjectClone.ToString(), QueryFieldName.ProjectClone));
            columnChoices.Add(new QueryColumnChoice(QueryFieldName.ProjectGen.ToString(), QueryFieldName.ProjectGen));
         }
         else
         {
            var names = HistoryPresenter.GetQueryFieldColumnNames();
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.ProjectID], QueryFieldName.ProjectID));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.WorkUnitName], QueryFieldName.WorkUnitName));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Name], QueryFieldName.Name));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Path], QueryFieldName.Path));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Username], QueryFieldName.Username));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Team], QueryFieldName.Team));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.SlotType], QueryFieldName.SlotType));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Core], QueryFieldName.Core));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.CoreVersion], QueryFieldName.CoreVersion));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.FrameTime], QueryFieldName.FrameTime));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.KFactor], QueryFieldName.KFactor));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.PPD], QueryFieldName.PPD));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.DownloadDateTime], QueryFieldName.DownloadDateTime));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.CompletionDateTime], QueryFieldName.CompletionDateTime));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Credit], QueryFieldName.Credit));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Frames], QueryFieldName.Frames));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.FramesCompleted], QueryFieldName.FramesCompleted));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Result], QueryFieldName.Result));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.Atoms], QueryFieldName.Atoms));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.ProjectRun], QueryFieldName.ProjectRun));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.ProjectClone], QueryFieldName.ProjectClone));
            columnChoices.Add(new QueryColumnChoice(names[(int)QueryFieldName.ProjectGen], QueryFieldName.ProjectGen));
         }

         return columnChoices;
      }
      
      private static List<QueryColumnChoice> GetOperatorFieldChoices()
      {
         var columnChoices = new List<QueryColumnChoice>();
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new QueryColumnChoice(QueryFieldType.Equal.ToString(), QueryFieldType.Equal));
            columnChoices.Add(new QueryColumnChoice(QueryFieldType.GreaterThan.ToString(), QueryFieldType.GreaterThan));
            columnChoices.Add(new QueryColumnChoice(QueryFieldType.GreaterThanOrEqual.ToString(), QueryFieldType.GreaterThanOrEqual));
            columnChoices.Add(new QueryColumnChoice(QueryFieldType.LessThan.ToString(), QueryFieldType.LessThan));
            columnChoices.Add(new QueryColumnChoice(QueryFieldType.LessThanOrEqual.ToString(), QueryFieldType.LessThanOrEqual));
            columnChoices.Add(new QueryColumnChoice(QueryFieldType.Like.ToString(), QueryFieldType.Like));
         }
         else
         {
            columnChoices.Add(new QueryColumnChoice("Equal", QueryFieldType.Equal));
            columnChoices.Add(new QueryColumnChoice("Greater Than", QueryFieldType.GreaterThan));
            columnChoices.Add(new QueryColumnChoice("Greater Than Or Equal", QueryFieldType.GreaterThanOrEqual));
            columnChoices.Add(new QueryColumnChoice("Less Than", QueryFieldType.LessThan));
            columnChoices.Add(new QueryColumnChoice("Less Than Or Equal", QueryFieldType.LessThanOrEqual));
            columnChoices.Add(new QueryColumnChoice("Like", QueryFieldType.Like));
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
}
