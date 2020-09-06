using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Client;
using HFM.Core.Services;
using HFM.Core.WorkUnits;
using HFM.Forms.Controls;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;
using HFM.Log;
using HFM.Preferences;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms.Views
{
    public partial class MainForm : FormBase, IWin32Form
    {
        public bool WorkUnitHistoryMenuEnabled
        {
            get { return mnuToolsHistory.Enabled; }
            set { mnuToolsHistory.Enabled = value; }
        }

        private readonly MainPresenter _presenter;
        private NotifyIcon _notifyIcon;

        public MainForm(MainPresenter presenter)
        {
            _presenter = presenter;

            // This call is Required by the Windows Form Designer
            InitializeComponent();

            SetupDataGridView();
            queueControl.SetProteinService(_presenter.ProteinService);
            base.Text = String.Format("HFM.NET v{0}", Core.Application.Version);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadData(_presenter.Model);
            LoadGridData(_presenter.GridModel);
        }

        private void LoadData(MainModel model)
        {
            // keep the model value synchronized with the form
            Resize += (s, e) => model.WindowState = WindowState;
            DataBindings.Add(nameof(ShowInTaskbar), model, nameof(MainModel.ShowInTaskbar), false, DataSourceUpdateMode.OnPropertyChanged);

            var (location, size) = WindowPosition.Normalize(this, model.FormLocation, model.FormSize);

            Location = location;
            LocationChanged += (s, e) => model.FormLocation = WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;

            if (size.Width != 0 && size.Height != 0)
            {
                if (!model.FormLogWindowVisible)
                {
                    size = new Size(size.Width, size.Height + model.FormLogWindowHeight);
                }
                Size = size;
                SizeChanged += (s, e) => model.FormSize = WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;

                // make sure split location is at least the minimum panel size
                if (model.FormSplitterLocation < splitContainer1.Panel2MinSize)
                {
                    model.FormSplitterLocation = splitContainer1.Panel2MinSize;
                }
                splitContainer1.SplitterDistance = model.FormSplitterLocation;
                splitContainer1.SplitterMoved += (s, e) => model.FormSplitterLocation = splitContainer1.SplitterDistance;
            }

            if (!model.FormLogWindowVisible)
            {
                ShowHideLogWindow(false);
            }
            if (!model.QueueWindowVisible)
            {
                ShowHideQueue(false);
            }

            ViewToggleFollowLogFileMenuItem.BindChecked(model, nameof(MainModel.FollowLog));
            statusLabelLeft.BindText(model, nameof(MainModel.ClientDetails));

            model.PropertyChanged += ModelPropertyChanged;

            RestoreViewPreferences(model);
            //
            dataGridView1.DataSource = _presenter.GridModel.BindingSource;
            //
            _presenter.Model.Preferences.PreferenceChanged += (s, e) =>
            {
                switch (e.Preference)
                {
                    case Preference.ColorLogFile:
                        txtLogFile.HighlightLines(_presenter.Model.Preferences.Get<bool>(Preference.ColorLogFile));
                        break;
                }
            };

            BindToUserStatsDataModel(_presenter.UserStatsDataModel);
            // Hook-up Status Label Event Handlers
            SubscribeToStatsLabelEvents();

            #region Hook-up DataGridView Event Handlers for Mono

            // If Mono, use the RowEnter Event (which was what 0.3.0 and prior used)
            // to set the CurrentInstance selection.  Obviously Mono doesn't fire the
            // DataGridView.SelectionChanged Event.
            if (Core.Application.IsRunningOnMono)
            {
                //dataGridView1.RowEnter += delegate
                //{
                //   _presenter.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
                //};
                //// Use RowLeave to clear data grid when selecting New file under Mono
                //dataGridView1.RowLeave += delegate
                //{
                //   _presenter.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
                //};
            }

            #endregion
        }

        private void LoadGridData(MainGridModel gridModel)
        {
            gridModel.AfterResetBindings += (s, e) =>
            {
                // run asynchronously so binding operation can finish
                BeginInvoke(new Action(() =>
                {
                    DisplaySelectedSlot(gridModel.SelectedSlot);
                    RefreshControlsWithTotalsData(gridModel.SlotTotals);
                }));
            };

            gridModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(MainGridModel.SelectedSlot):
                        // run asynchronously so binding operation can finish
                        BeginInvoke(new Action(() => DisplaySelectedSlot(gridModel.SelectedSlot)));
                        break;
                }
            };
        }

        private void ShowHideLogWindow(bool show)
        {
            if (!show)
            {
                txtLogFile.Visible = false;
                splitContainer1.Panel2Collapsed = true;
                _presenter.Model.FormLogWindowHeight = splitContainer1.Height - splitContainer1.SplitterDistance;
                Size = new Size(Size.Width, Size.Height - _presenter.Model.FormLogWindowHeight);
            }
            else
            {
                txtLogFile.Visible = true;
                Size = new Size(Size.Width, Size.Height + _presenter.Model.FormLogWindowHeight);
                splitContainer1.Panel2Collapsed = false;
            }
        }

        private void ShowHideQueue(bool show)
        {
            if (!show)
            {
                queueControl.Visible = false;
                btnQueue.Text = String.Format(CultureInfo.CurrentCulture, "S{0}h{0}o{0}w{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine);
                splitContainer2.SplitterDistance = 27;
            }
            else
            {
                queueControl.Visible = true;
                btnQueue.Text = String.Format(CultureInfo.CurrentCulture, "H{0}i{0}d{0}e{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine);
                splitContainer2.SplitterDistance = 289;
            }
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var model = _presenter.Model;
            switch (e.PropertyName)
            {
                case nameof(MainModel.FormLogWindowVisible):
                    ShowHideLogWindow(model.FormLogWindowVisible);
                    break;
                case nameof(MainModel.QueueWindowVisible):
                    ShowHideQueue(model.QueueWindowVisible);
                    break;
                case nameof(MainModel.NotifyIconVisible):
                    if (_notifyIcon != null)
                    {
                        _notifyIcon.Visible = model.NotifyIconVisible;
                    }
                    break;
                case nameof(MainModel.NotifyToolTip):
                    toolTipNotify.Show(model.NotifyToolTip, this, Size.Width - 150, 8, 2000);
                    break;
            }
        }

        private void RestoreViewPreferences(MainModel model)
        {
            var columns = model.FormColumns;
            if (columns != null)
            {
                var columnsList = columns.ToList();
                columnsList.Sort();

                for (int i = 0; i < columnsList.Count && i < NumberOfDisplayFields; i++)
                {
                    string[] tokens = columnsList[i].Split(',');
                    int index = Int32.Parse(tokens[3]);
                    dataGridView1.Columns[index].DisplayIndex = Int32.Parse(tokens[0]);
                    if (dataGridView1.Columns[index].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                    {
                        dataGridView1.Columns[index].Width = Int32.Parse(tokens[1]);
                    }
                    dataGridView1.Columns[index].Visible = Boolean.Parse(tokens[2]);
                }
            }
        }

        private void SaveColumnSettings()
        {
            var columns = new List<string>();

            int i = 0;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                columns.Add(String.Format(CultureInfo.InvariantCulture,
                    "{0},{1},{2},{3}",
                    column.DisplayIndex.ToString("D2"),
                    column.Width,
                    column.Visible,
                    i++));
            }

            _presenter.Model.FormColumns = columns;
        }

        private void DisplaySelectedSlot(SlotModel selectedSlot)
        {
            if (selectedSlot != null)
            {
                queueControl.SetWorkUnitInfos(selectedSlot.WorkUnitInfos, selectedSlot.SlotType);
                SetLogLines(selectedSlot, selectedSlot.CurrentLogLines);
            }
            else
            {
                txtLogFile.SetNoLogLines();
                queueControl.SetWorkUnitInfos(null, SlotType.Unknown);
            }
        }

        private void SetLogLines(SlotModel selectedSlot, IList<LogLine> logLines)
        {
            var preferences = _presenter.Model.Preferences;

            if (logLines != null && logLines.Count > 0)
            {
                // Different Client... Load LogLines
                if (txtLogFile.LogOwnedByInstanceName.Equals(selectedSlot.Name) == false)
                {
                    txtLogFile.SetLogLines(logLines, selectedSlot.Name, preferences.Get<bool>(Preference.ColorLogFile));
                }
                // Textbox has text lines
                else if (txtLogFile.Lines.Length > 0)
                {
                    string lastLogLine = String.Empty;

                    try // to get the last LogLine from the instance
                    {
                        lastLogLine = logLines[logLines.Count - 1].ToString();
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // eat it
                    }

                    // If the last text line in the textbox DOES NOT equal the last LogLine Text... Load LogLines.
                    // Otherwise, the log has not changed, don't update and perform the log "flicker".
                    if (txtLogFile.Lines[txtLogFile.Lines.Length - 1].Equals(lastLogLine) == false)
                    {
                        txtLogFile.SetLogLines(logLines, selectedSlot.Name, preferences.Get<bool>(Preference.ColorLogFile));
                    }
                }
                // Nothing in the Textbox... Load LogLines
                else
                {
                    txtLogFile.SetLogLines(logLines, selectedSlot.Name, preferences.Get<bool>(Preference.ColorLogFile));
                }
            }
            else
            {
                txtLogFile.SetNoLogLines();
            }

            if (_presenter.Model.FollowLog)
            {
                txtLogFile.ScrollToBottom();
            }
        }

        private void BindToUserStatsDataModel(UserStatsDataModel userStatsDataModel)
        {
            statusUserTeamRank.DataBindings.Add("Text", userStatsDataModel, "Rank", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserProjectRank.DataBindings.Add("Text", userStatsDataModel, "OverallRank", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUser24hr.DataBindings.Add("Text", userStatsDataModel, "TwentyFourHourAverage", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserToday.DataBindings.Add("Text", userStatsDataModel, "PointsToday", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserWeek.DataBindings.Add("Text", userStatsDataModel, "PointsWeek", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserTotal.DataBindings.Add("Text", userStatsDataModel, "PointsTotal", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserWUs.DataBindings.Add("Text", userStatsDataModel, "WorkUnitsTotal", false, DataSourceUpdateMode.OnPropertyChanged);

            statusUserTeamRank.DataBindings.Add("Visible", userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserProjectRank.DataBindings.Add("Visible", userStatsDataModel, "OverallRankVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUser24hr.DataBindings.Add("Visible", userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserToday.DataBindings.Add("Visible", userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserWeek.DataBindings.Add("Visible", userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserTotal.DataBindings.Add("Visible", userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
            statusUserWUs.DataBindings.Add("Visible", userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);

            if (Core.Application.IsRunningOnMono)
            {
                userStatsDataModel.PropertyChanged += (s, e) => UserStatsDataModelPropertyChangedForMono(userStatsDataModel);
            }
        }

        private void UserStatsDataModelPropertyChangedForMono(UserStatsDataModel userStatsDataModel)
        {
            statusUserTeamRank.Text = userStatsDataModel.Rank;
            statusUserProjectRank.Text = userStatsDataModel.OverallRank;
            statusUser24hr.Text = userStatsDataModel.TwentyFourHourAverage;
            statusUserToday.Text = userStatsDataModel.PointsToday;
            statusUserWeek.Text = userStatsDataModel.PointsWeek;
            statusUserTotal.Text = userStatsDataModel.PointsTotal;
            statusUserWUs.Text = userStatsDataModel.WorkUnitsTotal;

            statusUserTeamRank.Visible = userStatsDataModel.ControlsVisible;
            statusUserProjectRank.Visible = userStatsDataModel.OverallRankVisible;
            statusUser24hr.Visible = userStatsDataModel.ControlsVisible;
            statusUserToday.Visible = userStatsDataModel.ControlsVisible;
            statusUserWeek.Visible = userStatsDataModel.ControlsVisible;
            statusUserTotal.Visible = userStatsDataModel.ControlsVisible;
            statusUserWUs.Visible = userStatsDataModel.ControlsVisible;
        }

        private void SubscribeToStatsLabelEvents()
        {
            statusUserTeamRank.MouseDown += StatsLabelMouseDown;
            statusUserProjectRank.MouseDown += StatsLabelMouseDown;
            statusUser24hr.MouseDown += StatsLabelMouseDown;
            statusUserToday.MouseDown += StatsLabelMouseDown;
            statusUserWeek.MouseDown += StatsLabelMouseDown;
            statusUserTotal.MouseDown += StatsLabelMouseDown;
            statusUserWUs.MouseDown += StatsLabelMouseDown;
        }

        #region Form Handlers

        public void SecondInstanceStarted(string[] args)
        {
            if (InvokeRequired)
            {
                // make sure to use new object[] for the params array.  not doing so will cause
                // the invoke to use the args array as the params array which can easily cause
                // a TargetParameterCountException
                BeginInvoke(new Action<string[]>(SecondInstanceStarted), new object[] { args });
                return;
            }

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = _presenter.Model.OriginalWindowState;
            }
            else
            {
                if (Core.Application.IsRunningOnMono)
                {
                    Activate();
                }
                else
                {
                    NativeMethods.SetForegroundWindow(Handle);
                }
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            _notifyIcon = new NotifyIcon(components);
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.ContextMenuStrip = notifyMenu;
            _notifyIcon.Icon = Icon;
            _notifyIcon.Text = Text;
            _notifyIcon.DoubleClick += delegate { _presenter.NotifyIconDoubleClick(); };
            _notifyIcon.Visible = _presenter.Model.NotifyIconVisible;

            // Add the Index Changed Handler here after everything is shown
            dataGridView1.ColumnDisplayIndexChanged += delegate { DataGridViewColumnDisplayIndexChanged(); };
            // Then run it once to ensure the last column is set to Fill
            DataGridViewColumnDisplayIndexChanged();

            _presenter.ViewShown();
            _presenter.CheckForUpdateOnStartup(new ApplicationUpdateService(_presenter.Model.Preferences));
        }

        private void DataGridViewColumnDisplayIndexChanged()
        {
            if (dataGridView1.Columns.Count == NumberOfDisplayFields)
            {
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    if (column.DisplayIndex < dataGridView1.Columns.Count - 1)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    }
                    else
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }

                SaveColumnSettings();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveColumnSettings();

            e.Cancel = _presenter.ViewClosing();
        }

        #endregion

        #region Data Grid View Handlers

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            _presenter.DataGridViewSorted();
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            var hitTest = dataGridView1.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                if (hitTest.Type == DataGridViewHitTestType.Cell)
                {
                    if (dataGridView1.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex].Selected == false)
                    {
                        dataGridView1.Rows[hitTest.RowIndex].Cells[hitTest.ColumnIndex].Selected = true;
                    }
                    gridContextMenuStrip.Show(dataGridView1.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        #endregion

        #region File Menu Click Handlers

        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            _presenter.FileNewClick();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            using (var openFile = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.FileOpenClick(openFile);
            }
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            _presenter.FileSaveClick();
        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            using (var saveFile = DefaultFileDialogPresenter.SaveFile())
            {
                _presenter.FileSaveAsClick(saveFile);
            }
        }

        private void mnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Edit Menu Click Handlers

        private void mnuEditPreferences_Click(object sender, EventArgs e)
        {
            using (var scope = _presenter.ServiceScopeFactory.CreateScope())
            {
                using (var presenter = scope.ServiceProvider.GetRequiredService<PreferencesPresenter>())
                {
                    presenter.ShowDialog(_presenter.Form);
                    dataGridView1.Invalidate();
                }
            }
        }

        #endregion

        #region Help Menu Click Handlers

        private void mnuHelpHfmLogFile_Click(object sender, EventArgs e)
        {
            _presenter.ShowHfmLogFile(LocalProcessService.Default);
        }

        private void mnuHelpHfmDataFiles_Click(object sender, EventArgs e)
        {
            _presenter.ShowHfmDataFiles(LocalProcessService.Default);
        }

        private void mnuHelpHfmGroup_Click(object sender, EventArgs e)
        {
            _presenter.ShowHfmGoogleGroup(LocalProcessService.Default);
        }

        private void mnuHelpCheckForUpdate_Click(object sender, EventArgs e)
        {
            _presenter.CheckForUpdateClick(new ApplicationUpdateService(_presenter.Model.Preferences));
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            _presenter.AboutClicked();
        }

        #endregion

        #region Clients Menu Click Handlers

        private void AddClient_Click(object sender, EventArgs e)
        {
            _presenter.ClientsAddClick();
        }

        private void EditClient_Click(object sender, EventArgs e)
        {
            _presenter.ClientsEditClick();
        }

        private void DeleteClient_Click(object sender, EventArgs e)
        {
            _presenter.ClientsDeleteClick();
        }

        private void RefreshSelectedSlot_Click(object sender, EventArgs e)
        {
            _presenter.ClientsRefreshSelectedClick();
        }

        private void RefreshAllSlots_Click(object sender, EventArgs e)
        {
            _presenter.ClientsRefreshAllClick();
        }

        private void ViewCachedLog_Click(object sender, EventArgs e)
        {
            _presenter.ClientsViewCachedLogClick(LocalProcessService.Default);
        }

        #endregion

        #region Grid Context Menu Handlers

        private void FoldSlot_Click(object sender, EventArgs e)
        {
            _presenter.ClientsFoldSlotClick();
        }

        private void PauseSlot_Click(object sender, EventArgs e)
        {
            _presenter.ClientsPauseSlotClick();
        }

        private void FinishSlot_Click(object sender, EventArgs e)
        {
            _presenter.ClientsFinishSlotClick();
        }

        private void CopyPRCGToClipboard_Click(object sender, EventArgs e)
        {
            _presenter.CopyPRCGToClipboardClicked();
        }

        #endregion

        #region View Menu Click Handlers

        private void mnuViewAutoSizeGridColumns_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < dataGridView1.Columns.Count; i++)
            {
                AutoSizeColumn(i);
            }
        }

        private void mnuViewMessages_Click(object sender, EventArgs e)
        {
            _presenter.ViewMessagesClick();
        }

        private void mnuViewShowHideLog_Click(object sender, EventArgs e)
        {
            _presenter.ShowHideLogWindow();
        }

        private void btnQueue_Click(object sender, EventArgs e)
        {
            _presenter.ShowHideQueue();
        }

        private void mnuViewToggleDateTime_Click(object sender, EventArgs e)
        {
            _presenter.ViewToggleDateTimeClick();
            dataGridView1.Invalidate();
        }

        private void mnuViewToggleCompletedCountStyle_Click(object sender, EventArgs e)
        {
            _presenter.ViewToggleCompletedCountStyleClick();
            dataGridView1.Invalidate();
        }

        private void mnuViewToggleVersionInformation_Click(object sender, EventArgs e)
        {
            _presenter.ViewToggleVersionInformationClick();
            dataGridView1.Invalidate();
        }

        private void mnuViewToggleBonusCalculation_Click(object sender, EventArgs e)
        {
            _presenter.ViewCycleBonusCalculationClick();
            dataGridView1.Invalidate();
        }

        private void mnuViewCycleCalculation_Click(object sender, EventArgs e)
        {
            _presenter.ViewCycleCalculationClick();
            dataGridView1.Invalidate();
        }

        #endregion

        #region Tools Menu Click Handlers

        private void mnuToolsDownloadProjects_Click(object sender, EventArgs e)
        {
            using (var scope = _presenter.ServiceScopeFactory.CreateScope())
            {
                _presenter.ToolsDownloadProjectsClick(scope.ServiceProvider.GetRequiredService<IProteinService>());
            }
        }

        private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
        {
            _presenter.ToolsBenchmarksClick();
        }

        private void mnuToolsPointsCalculator_Click(object sender, EventArgs e)
        {
            _presenter.ToolsPointsCalculatorClick();
        }

        private void mnuToolsHistory_Click(object sender, EventArgs e)
        {
            _presenter.ToolsHistoryClick();
        }

        #endregion

        #region Web Menu Click Handlers

        private void mnuWebEOCUser_Click(object sender, EventArgs e)
        {
            _presenter.ShowEocUserPage(LocalProcessService.Default);
        }

        private void mnuWebStanfordUser_Click(object sender, EventArgs e)
        {
            _presenter.ShowStanfordUserPage(LocalProcessService.Default);
        }

        private void mnuWebEOCTeam_Click(object sender, EventArgs e)
        {
            _presenter.ShowEocTeamPage(LocalProcessService.Default);
        }

        private void mnuWebRefreshUserStats_Click(object sender, EventArgs e)
        {
            _presenter.RefreshUserStatsData();
        }

        private void mnuWebHFMGoogleCode_Click(object sender, EventArgs e)
        {
            _presenter.ShowHfmGitHub(LocalProcessService.Default);
        }

        #endregion

        private void RefreshControlsWithTotalsData(SlotTotals totals)
        {
            var preferences = _presenter.Model.Preferences;
            string numberFormat = NumberFormat.Get(preferences.Get<int>(Preference.DecimalPlaces));

            SetNotifyIconText(String.Format("{0} Working Slots{3}{1} Idle Slots{3}{2} PPD",
                totals.WorkingSlots, totals.NonWorkingSlots, totals.PPD.ToString(numberFormat), Environment.NewLine));

            string slots = "Slots";
            if (totals.TotalSlots == 1)
            {
                slots = "Slot";
            }

            int percentWorking = 0;
            if (totals.TotalSlots > 0)
            {
                percentWorking = ((totals.WorkingSlots * 200) + totals.TotalSlots) / (totals.TotalSlots * 2);
            }

            SetStatusLabelHostsText($"{totals.WorkingSlots} of {totals.TotalSlots} {slots} ({percentWorking}%)");
            SetStatusLabelPPDText($"{totals.PPD.ToString(numberFormat)} PPD");
        }

        private void SetNotifyIconText(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetNotifyIconText), text);
                return;
            }

            // make sure the object has been created
            if (_notifyIcon != null)
            {
                if (text.Length > 64)
                {
                    // if string is too long, remove the word Slots
                    text = text.Replace("Slots", String.Empty);
                }
                _notifyIcon.Text = text;
            }
        }

        private void SetStatusLabelHostsText(string val)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatusLabelHostsText), val);
                return;
            }

            statusLabelHosts.Text = val;
        }

        private void SetStatusLabelPPDText(string val)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetStatusLabelPPDText), val);
                return;
            }

            statusLabelPPW.Text = val;
        }

        #region System Tray Icon Click Handlers

        private void mnuNotifyRestore_Click(object sender, EventArgs e)
        {
            _presenter.NotifyIconRestoreClick();
        }

        private void mnuNotifyMinimize_Click(object sender, EventArgs e)
        {
            _presenter.NotifyIconMinimizeClick();
        }

        private void mnuNotifyMaximize_Click(object sender, EventArgs e)
        {
            _presenter.NotifyIconMaximizeClick();
        }

        #endregion

        #region User Stats Data Methods

        private void StatsLabelMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var statusLabel = (ToolStripStatusLabel)sender;
                // Issue 235
                if (Core.Application.IsRunningOnMono)
                {
                    statsContextMenuStrip.Show(statusStrip, e.X, e.Y);
                }
                else
                {
                    statsContextMenuStrip.Show(statusStrip, statusLabel.Bounds.X + e.X, statusLabel.Bounds.Y + e.Y);
                }
            }
        }

        private void mnuContextShowUserStats_Click(object sender, EventArgs e)
        {
            _presenter.SetUserStatsDataViewStyle(false);
        }

        private void mnuContextShowTeamStats_Click(object sender, EventArgs e)
        {
            _presenter.SetUserStatsDataViewStyle(true);
        }

        #endregion

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            SetupDataGridViewColumns(dataGridView1);

            new DataGridViewColumnSelector(dataGridView1);
        }
    }
}
