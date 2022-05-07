using System.Globalization;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
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

            PpdCalculationBonusDownloadTimeRadioButton.Tag = (int)BonusCalculation.DownloadTime;
            PpdCalculationBonusFrameTimeRadioButton.Tag = (int)BonusCalculation.FrameTime;
            PpdCalculationStandardRadioButton.Tag = (int)BonusCalculation.None;

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

            dgv.DataSource = model.HistoryBindingSource;

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

            foreach (DataGridViewColumn column in dgv.Columns)
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
                if (dgv.Columns.Count > index)
                {
                    dgv.Columns[index].DisplayIndex = Int32.Parse(tokens[0]);
                    dgv.Columns[index].Width = Int32.Parse(tokens[1]);
                    dgv.Columns[index].Visible = Boolean.Parse(tokens[2]);
                }
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
            dgv.AutoResizeColumns();
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

        private DataGridViewColumnSelector _columnSelector;

        private void SetupDataGridView(IPreferences preferences)
        {
            // Add Column Selector
            _columnSelector = new DataGridViewColumnSelector(dgv);

            string[] names = WorkUnitHistoryModel.GetColumnNames();
            string numberFormat = NumberFormat.Get(preferences.Get<int>(Preference.DecimalPlaces));

            dgv.AutoGenerateColumns = false;
            // ReSharper disable PossibleNullReferenceException
            dgv.Columns.Add(WorkUnitRowColumn.ProjectID.ToString(), names[(int)WorkUnitRowColumn.ProjectID]);
            dgv.Columns[WorkUnitRowColumn.ProjectID.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectID.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.SlotName.ToString(), names[(int)WorkUnitRowColumn.SlotName]);
            dgv.Columns[WorkUnitRowColumn.SlotName.ToString()].DataPropertyName = WorkUnitRowColumn.SlotName.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.ConnectionString.ToString(), names[(int)WorkUnitRowColumn.ConnectionString]);
            dgv.Columns[WorkUnitRowColumn.ConnectionString.ToString()].DataPropertyName = WorkUnitRowColumn.ConnectionString.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.DonorName.ToString(), names[(int)WorkUnitRowColumn.DonorName]);
            dgv.Columns[WorkUnitRowColumn.DonorName.ToString()].DataPropertyName = WorkUnitRowColumn.DonorName.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.DonorTeam.ToString(), names[(int)WorkUnitRowColumn.DonorTeam]);
            dgv.Columns[WorkUnitRowColumn.DonorTeam.ToString()].DataPropertyName = WorkUnitRowColumn.DonorTeam.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.SlotType.ToString(), names[(int)WorkUnitRowColumn.SlotType]);
            dgv.Columns[WorkUnitRowColumn.SlotType.ToString()].DataPropertyName = WorkUnitRowColumn.SlotType.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.Core.ToString(), names[(int)WorkUnitRowColumn.Core]);
            dgv.Columns[WorkUnitRowColumn.Core.ToString()].DataPropertyName = WorkUnitRowColumn.Core.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.CoreVersion.ToString(), names[(int)WorkUnitRowColumn.CoreVersion]);
            dgv.Columns[WorkUnitRowColumn.CoreVersion.ToString()].DataPropertyName = WorkUnitRowColumn.CoreVersion.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.FrameTime.ToString(), names[(int)WorkUnitRowColumn.FrameTime]);
            dgv.Columns[WorkUnitRowColumn.FrameTime.ToString()].DataPropertyName = WorkUnitRowColumn.FrameTime.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.KFactor.ToString(), names[(int)WorkUnitRowColumn.KFactor]);
            dgv.Columns[WorkUnitRowColumn.KFactor.ToString()].DataPropertyName = WorkUnitRowColumn.KFactor.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.PPD.ToString(), names[(int)WorkUnitRowColumn.PPD]);
            dgv.Columns[WorkUnitRowColumn.PPD.ToString()].DataPropertyName = WorkUnitRowColumn.PPD.ToString();
            dgv.Columns[WorkUnitRowColumn.PPD.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = numberFormat };
            dgv.Columns.Add(WorkUnitRowColumn.Assigned.ToString(), names[(int)WorkUnitRowColumn.Assigned]);
            dgv.Columns[WorkUnitRowColumn.Assigned.ToString()].DataPropertyName = WorkUnitRowColumn.Assigned.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.Finished.ToString(), names[(int)WorkUnitRowColumn.Finished]);
            dgv.Columns[WorkUnitRowColumn.Finished.ToString()].DataPropertyName = WorkUnitRowColumn.Finished.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.Credit.ToString(), names[(int)WorkUnitRowColumn.Credit]);
            dgv.Columns[WorkUnitRowColumn.Credit.ToString()].DataPropertyName = WorkUnitRowColumn.Credit.ToString();
            dgv.Columns[WorkUnitRowColumn.Credit.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = numberFormat };
            dgv.Columns.Add(WorkUnitRowColumn.BaseCredit.ToString(), names[(int)WorkUnitRowColumn.BaseCredit]);
            dgv.Columns[WorkUnitRowColumn.BaseCredit.ToString()].DataPropertyName = WorkUnitRowColumn.BaseCredit.ToString();
            dgv.Columns[WorkUnitRowColumn.BaseCredit.ToString()].DefaultCellStyle = new DataGridViewCellStyle { Format = numberFormat };
            dgv.Columns.Add(WorkUnitRowColumn.Frames.ToString(), names[(int)WorkUnitRowColumn.Frames]);
            dgv.Columns[WorkUnitRowColumn.Frames.ToString()].DataPropertyName = WorkUnitRowColumn.Frames.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.FramesCompleted.ToString(), names[(int)WorkUnitRowColumn.FramesCompleted]);
            dgv.Columns[WorkUnitRowColumn.FramesCompleted.ToString()].DataPropertyName = WorkUnitRowColumn.FramesCompleted.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.Result.ToString(), names[(int)WorkUnitRowColumn.Result]);
            dgv.Columns[WorkUnitRowColumn.Result.ToString()].DataPropertyName = WorkUnitRowColumn.Result.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.Atoms.ToString(), names[(int)WorkUnitRowColumn.Atoms]);
            dgv.Columns[WorkUnitRowColumn.Atoms.ToString()].DataPropertyName = WorkUnitRowColumn.Atoms.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.ProjectRun.ToString(), names[(int)WorkUnitRowColumn.ProjectRun]);
            dgv.Columns[WorkUnitRowColumn.ProjectRun.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectRun.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.ProjectClone.ToString(), names[(int)WorkUnitRowColumn.ProjectClone]);
            dgv.Columns[WorkUnitRowColumn.ProjectClone.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectClone.ToString();
            dgv.Columns.Add(WorkUnitRowColumn.ProjectGen.ToString(), names[(int)WorkUnitRowColumn.ProjectGen]);
            dgv.Columns[WorkUnitRowColumn.ProjectGen.ToString()].DataPropertyName = WorkUnitRowColumn.ProjectGen.ToString();
            // ReSharper restore PossibleNullReferenceException
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                if (e.ColumnIndex < 0 && e.RowIndex >= 0 && e.RowIndex < dgv.Rows.Count)
                {
                    e.PaintBackground(e.ClipBounds, true);
                    e.Graphics.DrawString((e.RowIndex + 1).ToString(), Font, Brushes.Black, e.CellBounds, sf);
                    e.Handled = true;
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti = dgv.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                if (hti.Type == DataGridViewHitTestType.RowHeader ||
                    hti.Type == DataGridViewHitTestType.Cell)
                {
                    int columnIndex = hti.ColumnIndex < 0 ? 0 : hti.ColumnIndex;
                    if (dgv.Rows[hti.RowIndex].Cells[columnIndex].Selected == false)
                    {
                        dgv.Rows[hti.RowIndex].Cells[columnIndex].Selected = true;
                    }
                    dataGridMenuStrip.Show(dgv.PointToScreen(new Point(e.X, e.Y)));
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _columnSelector?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
