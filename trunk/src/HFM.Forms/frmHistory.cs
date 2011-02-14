/*
 * HFM.NET - Work Unit History UI Form
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using HFM.Forms.Models;
using HFM.Forms.Controls;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   public interface IHistoryView : IWin32Window
   {
      void AttachPresenter(HistoryPresenter presenter);

      void DataBindModel(IHistoryPresenterModel model);

      void QueryComboRefreshList(IList<QueryParameters> queryList);
      
      int QueryComboSelectedIndex { get; set; }

      QueryParameters QueryComboSelectedValue { get; }

      bool EditButtonEnabled { get; set; }
      
      bool DeleteButtonEnabled { get; set; }
      
      HistoryEntry DataGridSelectedHistoryEntry { get; }
      
      void DataGridSetDataSource(IList<HistoryEntry> list);

      void DataGridSetDataSource(int totalResults, IList<HistoryEntry> list);

      StringCollection GetColumnSettings();

      void ApplySort(string sortColumnName, SortOrder sortOrder);

      #region System.Windows.Forms.Form Exposure

      void Show();

      void Close();

      void BringToFront();
      
      FormWindowState WindowState { get; set; }
      
      Point Location { get; set; }
      
      Size Size { get; set; }
      
      Rectangle RestoreBounds { get; }

      bool Visible { get; set; }
      
      #endregion
   }

   // ReSharper disable InconsistentNaming
   public partial class frmHistory : FormWrapper, IHistoryView
   // ReSharper restore InconsistentNaming
   {
      private HistoryPresenter _presenter;
   
      public frmHistory(IPreferenceSet prefs)
      {
         InitializeComponent();

         // split container does not scale when
         // there is a fixed panel
         using (var myGraphics = CreateGraphics())
         {
            var dpi = myGraphics.DpiX;
            var scaleFactor = dpi / 96;
            var distance = splitContainerWrapper1.SplitterDistance * scaleFactor;
            splitContainerWrapper1.SplitterDistance = (int)distance;
         }
         
         SetupDataGridView(prefs);
      }

      #region IHistoryView Members

      public void AttachPresenter(HistoryPresenter presenter)
      {
         _presenter = presenter;
      }
      
      public void DataBindModel(IHistoryPresenterModel model)
      {
         rdoPanelProduction.DataSource = model;
         rdoPanelProduction.ValueMember = "ProductionView";
         chkFirst.DataBindings.Add("Checked", model, "ShowFirstChecked", false, DataSourceUpdateMode.OnPropertyChanged);
         chkLast.DataBindings.Add("Checked", model, "ShowLastChecked", false, DataSourceUpdateMode.OnPropertyChanged);
         numericUpDown1.DataBindings.Add("Value", model, "ShowEntriesValue", false, DataSourceUpdateMode.OnPropertyChanged);

         Location = model.FormLocation;
         Size = model.FormSize;
         RestoreColumnSettings(model.FormColumns);
      }
      
      public void QueryComboRefreshList(IList<QueryParameters> queryList)
      {
         if (queryList.Count == 0)
         {
            throw new ArgumentException("Query list must have at least one query.");
         }
      
         var selectedIndex = cboSortView.SelectedIndex;

         var names = new List<Choice>();
         foreach (var query in queryList)
         {
            names.Add(new Choice(query.Name, query));
         }
         cboSortView.DataSource = names;
         cboSortView.DisplayMember = "Display";
         cboSortView.ValueMember = "Value";

         if (selectedIndex >= 0 && selectedIndex < cboSortView.Items.Count)
         {
            cboSortView.SelectedIndex = selectedIndex;
         }
         else
         {
            cboSortView.SelectedIndex = 0;
         }
      }

      public int QueryComboSelectedIndex
      {
         get { return cboSortView.SelectedIndex; }
         set { cboSortView.SelectedIndex = value; }
      }
      
      public QueryParameters QueryComboSelectedValue
      {
         get { return (QueryParameters)cboSortView.SelectedValue; }
      }

      public bool EditButtonEnabled
      {
         get { return btnEdit.Enabled; }
         set { btnEdit.Enabled = value; }
      }
      
      public bool DeleteButtonEnabled
      {
         get { return btnDelete.Enabled; }
         set { btnDelete.Enabled = value; }
      }
      
      public HistoryEntry DataGridSelectedHistoryEntry
      {
         get
         {
            if (dataGridView1.DataSource != null)
            {
               var cm = (CurrencyManager)dataGridView1.BindingContext[dataGridView1.DataSource];
               return cm.Current as HistoryEntry;
            }
            
            return null;
         }
      }
      
      public void DataGridSetDataSource(IList<HistoryEntry> list)
      {
         DataGridSetDataSource(list.Count, list);
      }

      public void DataGridSetDataSource(int totalResults, IList<HistoryEntry> list)
      {
         txtResults.Text = totalResults.ToString();
         txtShown.Text = list.Count.ToString();
         dataGridView1.DataSource = new HistoryEntrySortableBindingList(list);
      }
      
      public void ApplySort(string sortColumnName, SortOrder sortOrder)
      {
         if (String.IsNullOrEmpty(sortColumnName) == false &&
             dataGridView1.Columns.Contains(sortColumnName) &&
             sortOrder.Equals(SortOrder.None) == false)
         {
            // ReSharper disable AssignNullToNotNullAttribute
            if (sortOrder.Equals(SortOrder.Ascending))
            {
               dataGridView1.Sort(dataGridView1.Columns[sortColumnName], ListSortDirection.Ascending);
               dataGridView1.SortedColumn.HeaderCell.SortGlyphDirection = sortOrder;
            }
            else if (sortOrder.Equals(SortOrder.Descending))
            {
               dataGridView1.Sort(dataGridView1.Columns[sortColumnName], ListSortDirection.Descending);
               dataGridView1.SortedColumn.HeaderCell.SortGlyphDirection = sortOrder;
            }
            // ReSharper restore AssignNullToNotNullAttribute
         }
      }

      /// <summary>
      /// Save Column Index, Width, and Visibility
      /// </summary>
      public StringCollection GetColumnSettings()
      {
         // Save column state data
         // including order, column width and whether or not the column is visible
         var formColumns = new StringCollection();
         int i = 0;

         foreach (DataGridViewColumn column in dataGridView1.Columns)
         {
            formColumns.Add(String.Format(CultureInfo.InvariantCulture,
                                    "{0},{1},{2},{3}",
                                    column.DisplayIndex.ToString("D2"),
                                    column.Width,
                                    column.Visible,
                                    i++));
         }

         return formColumns;
      }

      private void RestoreColumnSettings(StringCollection formColumns)
      {
         if (formColumns == null) return;

         // Restore the columns' state
         var colsArray = new string[formColumns.Count];

         formColumns.CopyTo(colsArray, 0);
         Array.Sort(colsArray);

         for (int i = 0; i < colsArray.Length; i++)
         {
            string[] a = colsArray[i].Split(',');
            int index = Int32.Parse(a[3]);
            dataGridView1.Columns[index].DisplayIndex = Int32.Parse(a[0]);
            dataGridView1.Columns[index].Width = Int32.Parse(a[1]);
            dataGridView1.Columns[index].Visible = Boolean.Parse(a[2]);
         }
      }

      #endregion
      
      private void cboSortView_SelectedIndexChanged(object sender, EventArgs e)
      {
         _presenter.SelectQuery(cboSortView.SelectedIndex);
      }

      private void btnNew_Click(object sender, EventArgs e)
      {
         _presenter.NewQueryClick();
      }

      private void btnEdit_Click(object sender, EventArgs e)
      {
         _presenter.EditQueryClick();
      }

      private void btnDelete_Click(object sender, EventArgs e)
      {
         _presenter.DeleteQueryClick();
      }

      private void mnuFileImportCompletedUnits_Click(object sender, EventArgs e)
      {
         _presenter.ImportCompletedUnitsClick();
      }

      private void mnuFileExit_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void mnuViewAutoSizeGrid_Click(object sender, EventArgs e)
      {
         dataGridView1.AutoResizeColumns();
      }

      private void mnuViewRefresh_Click(object sender, EventArgs e)
      {
         _presenter.RefreshClicked();
      }

      private void btnRefresh_Click(object sender, EventArgs e)
      {
         _presenter.RefreshClicked();
      }

      private void frmHistory_FormClosing(object sender, FormClosingEventArgs e)
      {
         _presenter.OnViewClosing();
      }

      private void frmHistory_FormClosed(object sender, FormClosedEventArgs e)
      {
         _presenter.Close();
      }

      private void SetupDataGridView(IPreferenceSet prefs)
      {
         // Add Column Selector
         new DataGridViewColumnSelector(dataGridView1);
      
         var names = PlatformOps.GetQueryFieldColumnNames();
      
         dataGridView1.AutoGenerateColumns = false;
         // ReSharper disable PossibleNullReferenceException
         dataGridView1.Columns.Add(QueryFieldName.ProjectID.ToString(), names[(int)QueryFieldName.ProjectID]);
         dataGridView1.Columns[QueryFieldName.ProjectID.ToString()].DataPropertyName = QueryFieldName.ProjectID.ToString();
         dataGridView1.Columns.Add(QueryFieldName.WorkUnitName.ToString(), names[(int)QueryFieldName.WorkUnitName]);
         dataGridView1.Columns[QueryFieldName.WorkUnitName.ToString()].DataPropertyName = QueryFieldName.WorkUnitName.ToString();
         dataGridView1.Columns.Add(QueryFieldName.InstanceName.ToString(), names[(int)QueryFieldName.InstanceName]);
         dataGridView1.Columns[QueryFieldName.InstanceName.ToString()].DataPropertyName = QueryFieldName.InstanceName.ToString();
         dataGridView1.Columns.Add(QueryFieldName.InstancePath.ToString(), names[(int)QueryFieldName.InstancePath]);
         dataGridView1.Columns[QueryFieldName.InstancePath.ToString()].DataPropertyName = QueryFieldName.InstancePath.ToString();
         dataGridView1.Columns.Add(QueryFieldName.Username.ToString(), names[(int)QueryFieldName.Username]);
         dataGridView1.Columns[QueryFieldName.Username.ToString()].DataPropertyName = QueryFieldName.Username.ToString();
         dataGridView1.Columns.Add(QueryFieldName.Team.ToString(), names[(int)QueryFieldName.Team]);
         dataGridView1.Columns[QueryFieldName.Team.ToString()].DataPropertyName = QueryFieldName.Team.ToString();
         dataGridView1.Columns.Add(QueryFieldName.ClientType.ToString(), names[(int)QueryFieldName.ClientType]);
         dataGridView1.Columns[QueryFieldName.ClientType.ToString()].DataPropertyName = QueryFieldName.ClientType.ToString();
         dataGridView1.Columns.Add(QueryFieldName.Core.ToString(), names[(int)QueryFieldName.Core]);
         dataGridView1.Columns[QueryFieldName.Core.ToString()].DataPropertyName = QueryFieldName.Core.ToString();
         dataGridView1.Columns.Add(QueryFieldName.CoreVersion.ToString(), names[(int)QueryFieldName.CoreVersion]);
         dataGridView1.Columns[QueryFieldName.CoreVersion.ToString()].DataPropertyName = QueryFieldName.CoreVersion.ToString();
         dataGridView1.Columns.Add(QueryFieldName.FrameTime.ToString(), names[(int)QueryFieldName.FrameTime]);
         dataGridView1.Columns[QueryFieldName.FrameTime.ToString()].DataPropertyName = QueryFieldName.FrameTime.ToString();
         dataGridView1.Columns.Add(QueryFieldName.KFactor.ToString(), names[(int)QueryFieldName.KFactor]);
         dataGridView1.Columns[QueryFieldName.KFactor.ToString()].DataPropertyName = QueryFieldName.KFactor.ToString();
         dataGridView1.Columns.Add(QueryFieldName.PPD.ToString(), names[(int)QueryFieldName.PPD]);
         dataGridView1.Columns[QueryFieldName.PPD.ToString()].DataPropertyName = QueryFieldName.PPD.ToString();
         dataGridView1.Columns[QueryFieldName.PPD.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = prefs.PpdFormatString };
         dataGridView1.Columns.Add(QueryFieldName.DownloadDateTime.ToString(), names[(int)QueryFieldName.DownloadDateTime]);
         dataGridView1.Columns[QueryFieldName.DownloadDateTime.ToString()].DataPropertyName = QueryFieldName.DownloadDateTime.ToString();
         dataGridView1.Columns.Add(QueryFieldName.CompletionDateTime.ToString(), names[(int)QueryFieldName.CompletionDateTime]);
         dataGridView1.Columns[QueryFieldName.CompletionDateTime.ToString()].DataPropertyName = QueryFieldName.CompletionDateTime.ToString();
         dataGridView1.Columns.Add(QueryFieldName.Credit.ToString(), names[(int)QueryFieldName.Credit]);
         dataGridView1.Columns[QueryFieldName.Credit.ToString()].DataPropertyName = QueryFieldName.Credit.ToString();
         dataGridView1.Columns.Add(QueryFieldName.Frames.ToString(), names[(int)QueryFieldName.Frames]);
         dataGridView1.Columns[QueryFieldName.Frames.ToString()].DataPropertyName = QueryFieldName.Frames.ToString();
         dataGridView1.Columns.Add(QueryFieldName.FramesCompleted.ToString(), names[(int)QueryFieldName.FramesCompleted]);
         dataGridView1.Columns[QueryFieldName.FramesCompleted.ToString()].DataPropertyName = QueryFieldName.FramesCompleted.ToString();
         dataGridView1.Columns.Add(QueryFieldName.Result.ToString(), names[(int)QueryFieldName.Result]);
         dataGridView1.Columns[QueryFieldName.Result.ToString()].DataPropertyName = QueryFieldName.Result.ToString();
         dataGridView1.Columns.Add(QueryFieldName.Atoms.ToString(), names[(int)QueryFieldName.Atoms]);
         dataGridView1.Columns[QueryFieldName.Atoms.ToString()].DataPropertyName = QueryFieldName.Atoms.ToString();
         dataGridView1.Columns.Add(QueryFieldName.ProjectRun.ToString(), names[(int)QueryFieldName.ProjectRun]);
         dataGridView1.Columns[QueryFieldName.ProjectRun.ToString()].DataPropertyName = QueryFieldName.ProjectRun.ToString();
         dataGridView1.Columns.Add(QueryFieldName.ProjectClone.ToString(), names[(int)QueryFieldName.ProjectClone]);
         dataGridView1.Columns[QueryFieldName.ProjectClone.ToString()].DataPropertyName = QueryFieldName.ProjectClone.ToString();
         dataGridView1.Columns.Add(QueryFieldName.ProjectGen.ToString(), names[(int)QueryFieldName.ProjectGen]);
         dataGridView1.Columns[QueryFieldName.ProjectGen.ToString()].DataPropertyName = QueryFieldName.ProjectGen.ToString();
         // ReSharper restore PossibleNullReferenceException
      }
      
      private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
      {
         var sf = new StringFormat { Alignment = StringAlignment.Center };
         if (e.ColumnIndex < 0 && e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
         {
            e.PaintBackground(e.ClipBounds, true);
            e.Graphics.DrawString((e.RowIndex + 1).ToString(), Font, Brushes.Black, e.CellBounds, sf);
            e.Handled = true;
         }
      }

      /// <summary>
      /// Update Form Level Sorting Fields
      /// </summary>
      private void dataGridView1_Sorted(object sender, EventArgs e)
      {
         _presenter.SaveSortSettings(dataGridView1.SortedColumn.Name, dataGridView1.SortOrder);
      }

      private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo hti = dataGridView1.HitTest(e.X, e.Y);
         if (e.Button == MouseButtons.Right)
         {
            if (hti.Type == DataGridViewHitTestType.RowHeader ||
                hti.Type == DataGridViewHitTestType.Cell)
            {
               int columnIndex = hti.ColumnIndex < 0 ? 0 : hti.ColumnIndex;
               if (dataGridView1.Rows[hti.RowIndex].Cells[columnIndex].Selected == false)
               {
                  dataGridView1.Rows[hti.RowIndex].Cells[columnIndex].Selected = true;
               }
            }
         
            if (hti.Type == DataGridViewHitTestType.RowHeader)
            {
               dataGridMenuStrip.Show(dataGridView1.PointToScreen(new Point(e.X, e.Y)));
            }
         }
      }

      private void dataGridDeleteWorkUnitMenuItem_Click(object sender, EventArgs e)
      {
         _presenter.DeleteWorkUnitClick();
      }
   }
}
