using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Forms.Controls;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;
using HFM.Preferences;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms.Views
{
    public partial class WorkUnitHistoryForm : FormBase, IWin32Form
    {
        private readonly WorkUnitHistoryPresenter _presenter;

        public WorkUnitHistoryForm(WorkUnitHistoryPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

            InitializeComponent();
            EscapeKeyReturnsCancelDialogResult();

            // split container does not scale when
            // there is a fixed panel
            using (var myGraphics = CreateGraphics())
            {
                var dpi = myGraphics.DpiX;
                var scaleFactor = dpi / 96;
                var distance = splitContainerWrapper1.SplitterDistance * scaleFactor;
                splitContainerWrapper1.SplitterDistance = (int)distance;
            }

            SetupDataGridView(_presenter.Model.Preferences);
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            LoadData(_presenter.Model);
        }

        private void LoadData(WorkUnitHistoryModel model)
        {
            DataViewComboBox.DataSource = model.QueryBindingSource;
            DataViewEditButton.BindEnabled(model, nameof(WorkUnitHistoryModel.EditAndDeleteButtonsEnabled));
            DataViewDeleteButton.BindEnabled(model, nameof(WorkUnitHistoryModel.EditAndDeleteButtonsEnabled));

            rdoPanelProduction.DataSource = model;
            rdoPanelProduction.ValueMember = "BonusCalculation";
            ResultsTextBox.BindText(model, nameof(WorkUnitHistoryModel.TotalEntries));
            PageNumberTextBox.BindText(model, nameof(WorkUnitHistoryModel.CurrentPage));
            ResultNumberUpDownControl.DataBindings.Add("Value", model, nameof(WorkUnitHistoryModel.ShowEntriesValue), false, DataSourceUpdateMode.OnPropertyChanged);

            dataGridView1.DataSource = model.HistoryBindingSource;

            Location = model.FormLocation;
            LocationChanged += (s, e) => model.FormLocation = WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            Size = model.FormSize;
            SizeChanged += (s, e) => model.FormSize = WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;
            RestoreColumnSettings(model.FormColumns);
        }

        private void HistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _presenter.Model.FormColumns = GetColumnSettings();
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (var scope = _presenter.ServiceScopeFactory.CreateScope())
            {
                _presenter.NewQueryClick(scope.ServiceProvider.GetRequiredService<WorkUnitQueryPresenter>());
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            using (var scope = _presenter.ServiceScopeFactory.CreateScope())
            {
                _presenter.EditQueryClick(scope.ServiceProvider.GetRequiredService<WorkUnitQueryPresenter>());
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            _presenter.DeleteQueryClick();
        }

        private void mnuFileExport_Click(object sender, EventArgs e)
        {
            using (var saveFile = DefaultFileDialogPresenter.SaveFile())
            {
                _presenter.ExportClick(saveFile);
            }
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuViewAutoSizeGrid_Click(object sender, EventArgs e)
        {
            // do this manually... with a ton of items in the grid
            // this can take quite a while... should limit it to
            // the visible entries if possible... if not then some
            // set number like 100 entries.
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

        private void SetupDataGridView(IPreferences preferences)
        {
            // Add Column Selector
            new DataGridViewColumnSelector(dataGridView1);

            string[] names = WorkUnitQueryParameter.GetColumnNames();
            string numberFormat = NumberFormat.Get(preferences.Get<int>(Preference.DecimalPlaces));

            dataGridView1.AutoGenerateColumns = false;
            // ReSharper disable PossibleNullReferenceException
            dataGridView1.Columns.Add(WorkUnitRowColumn.ProjectID.ToString(), names[(int)WorkUnitRowColumn.ProjectID]);
            dataGridView1.Columns[WorkUnitRowColumn.ProjectID.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectID.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.WorkUnitName.ToString(), names[(int)WorkUnitRowColumn.WorkUnitName]);
            dataGridView1.Columns[WorkUnitRowColumn.WorkUnitName.ToString()].DataPropertyName = WorkUnitRowColumn.WorkUnitName.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Name.ToString(), names[(int)WorkUnitRowColumn.Name]);
            dataGridView1.Columns[WorkUnitRowColumn.Name.ToString()].DataPropertyName = WorkUnitRowColumn.Name.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Path.ToString(), names[(int)WorkUnitRowColumn.Path]);
            dataGridView1.Columns[WorkUnitRowColumn.Path.ToString()].DataPropertyName = WorkUnitRowColumn.Path.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Username.ToString(), names[(int)WorkUnitRowColumn.Username]);
            dataGridView1.Columns[WorkUnitRowColumn.Username.ToString()].DataPropertyName = WorkUnitRowColumn.Username.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Team.ToString(), names[(int)WorkUnitRowColumn.Team]);
            dataGridView1.Columns[WorkUnitRowColumn.Team.ToString()].DataPropertyName = WorkUnitRowColumn.Team.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.SlotType.ToString(), names[(int)WorkUnitRowColumn.SlotType]);
            dataGridView1.Columns[WorkUnitRowColumn.SlotType.ToString()].DataPropertyName = WorkUnitRowColumn.SlotType.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Core.ToString(), names[(int)WorkUnitRowColumn.Core]);
            dataGridView1.Columns[WorkUnitRowColumn.Core.ToString()].DataPropertyName = WorkUnitRowColumn.Core.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.CoreVersion.ToString(), names[(int)WorkUnitRowColumn.CoreVersion]);
            dataGridView1.Columns[WorkUnitRowColumn.CoreVersion.ToString()].DataPropertyName = WorkUnitRowColumn.CoreVersion.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.FrameTime.ToString(), names[(int)WorkUnitRowColumn.FrameTime]);
            dataGridView1.Columns[WorkUnitRowColumn.FrameTime.ToString()].DataPropertyName = WorkUnitRowColumn.FrameTime.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.KFactor.ToString(), names[(int)WorkUnitRowColumn.KFactor]);
            dataGridView1.Columns[WorkUnitRowColumn.KFactor.ToString()].DataPropertyName = WorkUnitRowColumn.KFactor.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.PPD.ToString(), names[(int)WorkUnitRowColumn.PPD]);
            dataGridView1.Columns[WorkUnitRowColumn.PPD.ToString()].DataPropertyName = WorkUnitRowColumn.PPD.ToString();
            dataGridView1.Columns[WorkUnitRowColumn.PPD.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = numberFormat };
            dataGridView1.Columns.Add(WorkUnitRowColumn.Assigned.ToString(), names[(int)WorkUnitRowColumn.Assigned]);
            dataGridView1.Columns[WorkUnitRowColumn.Assigned.ToString()].DataPropertyName = WorkUnitRowColumn.Assigned.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Finished.ToString(), names[(int)WorkUnitRowColumn.Finished]);
            dataGridView1.Columns[WorkUnitRowColumn.Finished.ToString()].DataPropertyName = WorkUnitRowColumn.Finished.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Credit.ToString(), names[(int)WorkUnitRowColumn.Credit]);
            dataGridView1.Columns[WorkUnitRowColumn.Credit.ToString()].DataPropertyName = WorkUnitRowColumn.Credit.ToString();
            dataGridView1.Columns[WorkUnitRowColumn.Credit.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = numberFormat };
            dataGridView1.Columns.Add(WorkUnitRowColumn.Frames.ToString(), names[(int)WorkUnitRowColumn.Frames]);
            dataGridView1.Columns[WorkUnitRowColumn.Frames.ToString()].DataPropertyName = WorkUnitRowColumn.Frames.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.FramesCompleted.ToString(), names[(int)WorkUnitRowColumn.FramesCompleted]);
            dataGridView1.Columns[WorkUnitRowColumn.FramesCompleted.ToString()].DataPropertyName = WorkUnitRowColumn.FramesCompleted.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Result.ToString(), names[(int)WorkUnitRowColumn.Result]);
            dataGridView1.Columns[WorkUnitRowColumn.Result.ToString()].DataPropertyName = WorkUnitRowColumn.Result.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.Atoms.ToString(), names[(int)WorkUnitRowColumn.Atoms]);
            dataGridView1.Columns[WorkUnitRowColumn.Atoms.ToString()].DataPropertyName = WorkUnitRowColumn.Atoms.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.ProjectRun.ToString(), names[(int)WorkUnitRowColumn.ProjectRun]);
            dataGridView1.Columns[WorkUnitRowColumn.ProjectRun.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectRun.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.ProjectClone.ToString(), names[(int)WorkUnitRowColumn.ProjectClone]);
            dataGridView1.Columns[WorkUnitRowColumn.ProjectClone.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectClone.ToString();
            dataGridView1.Columns.Add(WorkUnitRowColumn.ProjectGen.ToString(), names[(int)WorkUnitRowColumn.ProjectGen]);
            dataGridView1.Columns[WorkUnitRowColumn.ProjectGen.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectGen.ToString();
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
                    dataGridMenuStrip.Show(dataGridView1.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        private void DeleteWorkUnit_Click(object sender, EventArgs e)
        {
            _presenter.DeleteWorkUnitClick();
        }

        private void CopyPRCGToClipboard_Click(object sender, EventArgs e)
        {
            _presenter.CopyPRCGToClipboardClicked();
        }
    }
}
