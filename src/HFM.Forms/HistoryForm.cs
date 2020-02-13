/*
 * HFM.NET - Work Unit History UI Form
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Data;
using HFM.Forms.Controls;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM.Forms
{
   public interface IHistoryView : IWin32Window
   {
      void AttachPresenter(HistoryPresenter presenter);

      void DataBindModel(HistoryPresenterModel model);

      ICollection<string> GetColumnSettings();

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

   public partial class HistoryForm : FormWrapper, IHistoryView
   {
      private HistoryPresenter _presenter;

      public HistoryForm(IPreferenceSet prefs)
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

      public void DataBindModel(HistoryPresenterModel model)
      {
         DataViewComboBox.DataSource = model.QueryBindingSource;
         DataViewEditButton.DataBindings.Add("Enabled", model, "EditAndDeleteButtonsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         DataViewDeleteButton.DataBindings.Add("Enabled", model, "EditAndDeleteButtonsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

         rdoPanelProduction.DataSource = model;
         rdoPanelProduction.ValueMember = "BonusCalculation";
         ResultsTextBox.DataBindings.Add("Text", model, "TotalEntries", false, DataSourceUpdateMode.OnPropertyChanged);
         PageNumberTextBox.DataBindings.Add("Text", model, "CurrentPage", false, DataSourceUpdateMode.OnValidation); //OnPropertyChanged);
         ResultNumberUpDownControl.DataBindings.Add("Value", model, "ShowEntriesValue", false, DataSourceUpdateMode.OnPropertyChanged);

         dataGridView1.DataSource = model.HistoryBindingSource;

         Location = model.FormLocation;
         Size = model.FormSize;
         RestoreColumnSettings(model.FormColumns);
      }

      /// <summary>
      /// Save Column Index, Width, and Visibility
      /// </summary>
      public ICollection<string> GetColumnSettings()
      {
         // Save column state data
         // including order, column width and whether or not the column is visible
         var formColumns = new List<string>();
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

      private void RestoreColumnSettings(IEnumerable<string> formColumns)
      {
         if (formColumns == null) return;

         // Restore the columns' state
         var colsList = formColumns.ToList();
         colsList.Sort();

         foreach (string col in colsList)
         {
            string[] tokens = col.Split(',');
            int index = Int32.Parse(tokens[3]);
            dataGridView1.Columns[index].DisplayIndex = Int32.Parse(tokens[0]);
            dataGridView1.Columns[index].Width = Int32.Parse(tokens[1]);
            dataGridView1.Columns[index].Visible = Boolean.Parse(tokens[2]);
         }
      }

      #endregion

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

      private void mnuFileExport_Click(object sender, EventArgs e)
      {
         _presenter.ExportClick();
      }

      private void mnuFileExit_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void mnuViewAutoSizeGrid_Click(object sender, EventArgs e)
      {
         // do this manually... with a ton of items in the grid
         // this can take quite a while... should limit it to
         // the visible entries if posssible... if not then some
         // set number like 100 entires.
         dataGridView1.AutoResizeColumns();
      }

      private void FirstPageButton_Click(object sender, EventArgs e)
      {
         _presenter.FirstPageClicked();
      }

      private void PreviousPageButton_Click(object sender, EventArgs e)
      {
         _presenter.PreviousPageClicked();
      }

      private void NextPageButton_Click(object sender, EventArgs e)
      {
         _presenter.NextPageClicked();
      }

      private void LastPageButton_Click(object sender, EventArgs e)
      {
         _presenter.LastPageClicked();
      }

      private void RefreshAllMenuItem_Click(object sender, EventArgs e)
      {
         _presenter.RefreshAllProjectDataClick();
      }

      private void RefreshUnknownMenuItem_Click(object sender, EventArgs e)
      {
         _presenter.RefreshUnknownProjectDataClick();
      }

      private void RefreshProjectMenuItem_Click(object sender, EventArgs e)
      {
         _presenter.RefreshDataByProjectClick();
      }

      private void RefreshEntryMenuItem_Click(object sender, EventArgs e)
      {
         _presenter.RefreshDataByIdClick();
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

         string[] names = WorkUnitHistoryQueryParameter.GetColumnNames();

         dataGridView1.AutoGenerateColumns = false;
         // ReSharper disable PossibleNullReferenceException
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.ProjectID.ToString(), names[(int)WorkUnitHistoryRowColumn.ProjectID]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.ProjectID.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.ProjectID.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.WorkUnitName.ToString(), names[(int)WorkUnitHistoryRowColumn.WorkUnitName]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.WorkUnitName.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.WorkUnitName.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Name.ToString(), names[(int)WorkUnitHistoryRowColumn.Name]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Name.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Name.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Path.ToString(), names[(int)WorkUnitHistoryRowColumn.Path]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Path.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Path.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Username.ToString(), names[(int)WorkUnitHistoryRowColumn.Username]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Username.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Username.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Team.ToString(), names[(int)WorkUnitHistoryRowColumn.Team]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Team.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Team.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.SlotType.ToString(), names[(int)WorkUnitHistoryRowColumn.SlotType]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.SlotType.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.SlotType.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Core.ToString(), names[(int)WorkUnitHistoryRowColumn.Core]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Core.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Core.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.CoreVersion.ToString(), names[(int)WorkUnitHistoryRowColumn.CoreVersion]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.CoreVersion.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.CoreVersion.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.FrameTime.ToString(), names[(int)WorkUnitHistoryRowColumn.FrameTime]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.FrameTime.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.FrameTime.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.KFactor.ToString(), names[(int)WorkUnitHistoryRowColumn.KFactor]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.KFactor.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.KFactor.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.PPD.ToString(), names[(int)WorkUnitHistoryRowColumn.PPD]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.PPD.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.PPD.ToString();
         dataGridView1.Columns[WorkUnitHistoryRowColumn.PPD.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = prefs.GetPpdFormatString() };
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.DownloadDateTime.ToString(), names[(int)WorkUnitHistoryRowColumn.DownloadDateTime]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.DownloadDateTime.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.DownloadDateTime.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.CompletionDateTime.ToString(), names[(int)WorkUnitHistoryRowColumn.CompletionDateTime]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.CompletionDateTime.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.CompletionDateTime.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Credit.ToString(), names[(int)WorkUnitHistoryRowColumn.Credit]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Credit.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Credit.ToString();
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Credit.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = prefs.GetPpdFormatString() };
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Frames.ToString(), names[(int)WorkUnitHistoryRowColumn.Frames]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Frames.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Frames.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.FramesCompleted.ToString(), names[(int)WorkUnitHistoryRowColumn.FramesCompleted]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.FramesCompleted.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.FramesCompleted.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Result.ToString(), names[(int)WorkUnitHistoryRowColumn.Result]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Result.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Result.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.Atoms.ToString(), names[(int)WorkUnitHistoryRowColumn.Atoms]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.Atoms.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.Atoms.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.ProjectRun.ToString(), names[(int)WorkUnitHistoryRowColumn.ProjectRun]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.ProjectRun.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.ProjectRun.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.ProjectClone.ToString(), names[(int)WorkUnitHistoryRowColumn.ProjectClone]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.ProjectClone.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.ProjectClone.ToString();
         dataGridView1.Columns.Add(WorkUnitHistoryRowColumn.ProjectGen.ToString(), names[(int)WorkUnitHistoryRowColumn.ProjectGen]);
         dataGridView1.Columns[WorkUnitHistoryRowColumn.ProjectGen.ToString()].DataPropertyName = WorkUnitHistoryRowColumn.ProjectGen.ToString();
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
