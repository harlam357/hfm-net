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
            AddColumn(WorkUnitRowColumn.ProjectID, names);
            AddColumn(WorkUnitRowColumn.SlotName, names);
            AddColumn(WorkUnitRowColumn.ConnectionString, names);
            AddColumn(WorkUnitRowColumn.DonorName, names);
            AddColumn(WorkUnitRowColumn.DonorTeam, names);
            AddColumn(WorkUnitRowColumn.SlotType, names);
            AddColumn(WorkUnitRowColumn.Core, names);
            AddColumn(WorkUnitRowColumn.CoreVersion, names);
            AddColumn(WorkUnitRowColumn.FrameTime, names);
            AddColumn(WorkUnitRowColumn.KFactor, names);
            AddColumn(WorkUnitRowColumn.PPD, names, numberFormat);
            AddColumn(WorkUnitRowColumn.Assigned, names);
            AddColumn(WorkUnitRowColumn.Finished, names);
            AddColumn(WorkUnitRowColumn.Credit, names, numberFormat);
            AddColumn(WorkUnitRowColumn.BaseCredit, names, numberFormat);
            AddColumn(WorkUnitRowColumn.Frames, names);
            AddColumn(WorkUnitRowColumn.FramesCompleted, names);
            AddColumn(WorkUnitRowColumn.Result, names);
            AddColumn(WorkUnitRowColumn.Atoms, names);
            AddColumn(WorkUnitRowColumn.ProjectRun, names);
            AddColumn(WorkUnitRowColumn.ProjectClone, names);
            AddColumn(WorkUnitRowColumn.ProjectGen, names);
            AddColumn(WorkUnitRowColumn.ClientVersion, names);
            AddColumn(WorkUnitRowColumn.OperatingSystem, names);
            AddColumn(WorkUnitRowColumn.PlatformImplementation, names);
            AddColumn(WorkUnitRowColumn.PlatformProcessor, names);
            AddColumn(WorkUnitRowColumn.PlatformThreads, names);
            AddColumn(WorkUnitRowColumn.DriverVersion, names);
            AddColumn(WorkUnitRowColumn.ComputeVersion, names);
            AddColumn(WorkUnitRowColumn.CUDAVersion, names);
        }

        private void AddColumn(WorkUnitRowColumn column, IReadOnlyList<string> names, string numberFormat = null)
        {
            string columnName = column.ToString();
            dgv.Columns.Add(columnName, names[(int)column]);
            // ReSharper disable PossibleNullReferenceException
            dgv.Columns[columnName].DataPropertyName = columnName;
            if (!String.IsNullOrEmpty(numberFormat))
            {
                dgv.Columns[columnName].DefaultCellStyle = new DataGridViewCellStyle { Format = numberFormat };
            }
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
