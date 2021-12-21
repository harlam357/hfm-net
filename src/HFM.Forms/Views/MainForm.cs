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

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms.Views
{
    public partial class MainForm : FormBase, IWin32Form
    {
        private readonly MainPresenter _presenter;
        private NotifyIcon _notifyIcon;

        public MainForm(MainPresenter presenter)
        {
            _presenter = presenter;

            // This call is Required by the Windows Form Designer
            InitializeComponent();

            SetupDataGridView();
            SubscribeToUserStatsControlEvents();
            SubscribeToFileMenuControlEvents();
            SubscribeToEditMenuControlEvents();
            SubscribeToHelpMenuControlEvents();
            SubscribeToClientsMenuControlEvents();
            SubscribeToGridContextMenuControlEvents();
            SubscribeToWebMenuControlEvents();
            SubscribeToNotifyIconMenuControlEvents();
            SubscribeToUserStatsContextMenuControlEvents();
            queueControl.ProteinService = _presenter.ProteinService;
            base.Text = $@"{Core.Application.Name} v{Core.Application.Version}";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadData(_presenter.Model);
            LoadSlotsData(_presenter.SlotsModel);
            LoadUserStatsData(_presenter.UserStatsDataModel);
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
                splitContainer2.SplitterMoved += (s, e) =>
                {
                    if (queueControl.Visible)
                    {
                        model.QueueSplitterLocation = splitContainer2.SplitterDistance;
                    }
                };
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
            clientDetailsStatusLabel.BindText(model, nameof(MainModel.ClientDetails));
            workingSlotsStatusLabel.BindText(model, nameof(MainModel.WorkingSlotsText));
            totalProductionStatusLabel.BindText(model, nameof(MainModel.TotalProductionText));
            txtLogFile.DataBindings.Add(nameof(txtLogFile.ColorLogFile), model, nameof(MainModel.ColorLogFile), false, DataSourceUpdateMode.OnPropertyChanged);

            model.PropertyChanged += ModelPropertyChanged;

            RestoreFormColumns(model.FormColumns);
        }

        private void LoadSlotsData(SlotCollectionModel slotsModel)
        {
            dataGridView1.DataSource = slotsModel.BindingSource;
            dataGridView1.Sorted += (s, e) => slotsModel.ResetSelectedSlot();

            slotsModel.AfterResetBindings += (s, e) =>
            {
                // Create a local reference before handing off to BeginInvoke.
                // This ensures that the BeginInvoke action uses the state of SlotsModel properties available now,
                // not the state of SlotsModel properties when the BeginInvoke action is executed (at a later time).
                var selectedSlot = slotsModel.SelectedSlot;
                var workUnitQueue = selectedSlot?.WorkUnitQueue;
                var logLines = selectedSlot?.CurrentLogLines?.ToList();

                // run asynchronously so binding operation can finish
                BeginInvoke(new Action(() => LoadSelectedSlot(selectedSlot, workUnitQueue, logLines)));
            };

            slotsModel.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(SlotCollectionModel.SelectedSlot):
                        // Create a local reference before handing off to BeginInvoke.
                        // This ensures that the BeginInvoke action uses the state of SlotsModel properties available now,
                        // not the state of SlotsModel properties when the BeginInvoke action is executed (at a later time).
                        var selectedSlot = slotsModel.SelectedSlot;
                        var workUnitQueue = selectedSlot?.WorkUnitQueue;
                        var logLines = selectedSlot?.CurrentLogLines?.ToList();

                        // run asynchronously so binding operation can finish
                        BeginInvoke(new Action(() => LoadSelectedSlot(selectedSlot, workUnitQueue, logLines)));
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

        private void ShowHideQueue(bool show, int? splitterDistance = null)
        {
            if (show)
            {
                queueControl.Visible = true;
                btnQueue.Text = String.Format(CultureInfo.CurrentCulture, "H{0}i{0}d{0}e{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine);
                splitContainer2.SplitterDistance = splitterDistance.GetValueOrDefault(289);
            }
            else
            {
                queueControl.Visible = false;
                btnQueue.Text = String.Format(CultureInfo.CurrentCulture, "S{0}h{0}o{0}w{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine);
                splitContainer2.SplitterDistance = 32;
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
                    ShowHideQueue(model.QueueWindowVisible, model.QueueSplitterLocation);
                    break;
                case nameof(MainModel.NotifyIconVisible):
                    if (_notifyIcon != null)
                    {
                        _notifyIcon.Visible = model.NotifyIconVisible;
                    }
                    break;
                case nameof(MainModel.NotifyIconText):
                    SetNotifyIconText(model.NotifyIconText);
                    break;
                case nameof(MainModel.NotifyToolTip):
                    toolTipNotify.Show(model.NotifyToolTip, this, Size.Width - 150, 8, 2000);
                    break;
            }

            void SetNotifyIconText(string text)
            {
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
        }

        private void RestoreFormColumns(ICollection<string> formColumns)
        {
            if (formColumns != null)
            {
                var columnsList = formColumns.ToList();
                columnsList.Sort();

                for (int i = 0; i < columnsList.Count && i < NumberOfDisplayFields; i++)
                {
                    var columnSettings = Preferences.FormColumnPreference.Parse(columnsList[i]);
                    if (columnSettings.HasValue)
                    {
                        int index = columnSettings.Value.Index;
                        dataGridView1.Columns[index].DisplayIndex = columnSettings.Value.DisplayIndex;
                        if (dataGridView1.Columns[index].AutoSizeMode != DataGridViewAutoSizeColumnMode.Fill)
                        {
                            dataGridView1.Columns[index].Width = columnSettings.Value.Width;
                        }
                        dataGridView1.Columns[index].Visible = columnSettings.Value.Visible;
                    }
                }
            }
        }

        private ICollection<string> GetFormColumns()
        {
            return dataGridView1.Columns.Cast<DataGridViewColumn>()
                .Select((column, i) => Preferences.FormColumnPreference.Format(
                    column.DisplayIndex,
                    column.Width,
                    column.Visible,
                    i))
                .ToList();
        }

        private void LoadSelectedSlot(SlotModel selectedSlot, WorkUnitQueueItemCollection workUnitQueue, IList<LogLine> logLines)
        {
            queueControl.SetWorkUnitQueue(workUnitQueue);
            txtLogFile.SetLogLines(selectedSlot, logLines);
            if (_presenter.Model.FollowLog)
            {
                txtLogFile.ScrollToBottom();
            }
        }

        private void LoadUserStatsData(UserStatsDataModel userStatsDataModel)
        {
            statusUserTeamRank.BindText(userStatsDataModel, nameof(UserStatsDataModel.Rank));
            statusUserProjectRank.BindText(userStatsDataModel, nameof(UserStatsDataModel.OverallRank));
            statusUser24hr.BindText(userStatsDataModel, nameof(UserStatsDataModel.TwentyFourHourAverage));
            statusUserToday.BindText(userStatsDataModel, nameof(UserStatsDataModel.PointsToday));
            statusUserWeek.BindText(userStatsDataModel, nameof(UserStatsDataModel.PointsWeek));
            statusUserTotal.BindText(userStatsDataModel, nameof(UserStatsDataModel.PointsTotal));
            statusUserWUs.BindText(userStatsDataModel, nameof(UserStatsDataModel.WorkUnitsTotal));

            statusUserTeamRank.BindVisible(userStatsDataModel, nameof(UserStatsDataModel.ControlsVisible));
            statusUserProjectRank.BindVisible(userStatsDataModel, nameof(UserStatsDataModel.OverallRankVisible));
            statusUser24hr.BindVisible(userStatsDataModel, nameof(UserStatsDataModel.ControlsVisible));
            statusUserToday.BindVisible(userStatsDataModel, nameof(UserStatsDataModel.ControlsVisible));
            statusUserWeek.BindVisible(userStatsDataModel, nameof(UserStatsDataModel.ControlsVisible));
            statusUserTotal.BindVisible(userStatsDataModel, nameof(UserStatsDataModel.ControlsVisible));
            statusUserWUs.BindVisible(userStatsDataModel, nameof(UserStatsDataModel.ControlsVisible));
        }

        private void SubscribeToUserStatsControlEvents()
        {
            statusUserTeamRank.MouseDown += StatsLabelMouseDown;
            statusUserProjectRank.MouseDown += StatsLabelMouseDown;
            statusUser24hr.MouseDown += StatsLabelMouseDown;
            statusUserToday.MouseDown += StatsLabelMouseDown;
            statusUserWeek.MouseDown += StatsLabelMouseDown;
            statusUserTotal.MouseDown += StatsLabelMouseDown;
            statusUserWUs.MouseDown += StatsLabelMouseDown;

            void StatsLabelMouseDown(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var statusLabel = (ToolStripStatusLabel)sender;
                    statsContextMenuStrip.Show(statusStrip, statusLabel.Bounds.X + e.X, statusLabel.Bounds.Y + e.Y);
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
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

            _presenter.FormShown();
            using (var scope = _presenter.ServiceScopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<ApplicationUpdateService>();
                var presenterFactory = scope.ServiceProvider.GetRequiredService<ApplicationUpdatePresenterFactory>();
                _presenter.CheckForUpdateOnStartup(service, presenterFactory);
            }
        }

        private void DataGridViewColumnDisplayIndexChanged()
        {
            // don't execute until all the columns are populated
            if (dataGridView1.Columns.Count != NumberOfDisplayFields) return;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = column.DisplayIndex < dataGridView1.Columns.Count - 1
                    ? DataGridViewAutoSizeColumnMode.None
                    : DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var formColumns = GetFormColumns();
            e.Cancel = _presenter.FormClosing(formColumns);
        }

        // Data Grid View Handlers
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

        private void SubscribeToFileMenuControlEvents()
        {
            mnuFileNew.Click += (s, e) => _presenter.FileNewClick();
            mnuFileOpen.Click += (s, e) =>
            {
                using (var openFile = DefaultFileDialogPresenter.OpenFile())
                {
                    _presenter.FileOpenClick(openFile);
                }
            };
            mnuFileSave.Click += (s, e) => _presenter.FileSaveClick();
            mnuFileSaveas.Click += (s, e) =>
            {
                using (var saveFile = DefaultFileDialogPresenter.SaveFile())
                {
                    _presenter.FileSaveAsClick(saveFile);
                }
            };
            mnuFileQuit.Click += (s, e) => Close();
        }

        private void SubscribeToEditMenuControlEvents()
        {
            mnuEditPreferences.Click += (s, e) =>
            {
                using (var scope = _presenter.ServiceScopeFactory.CreateScope())
                using (var presenter = scope.ServiceProvider.GetRequiredService<PreferencesPresenter>())
                {
                    presenter.ShowDialog(_presenter.Form);
                    dataGridView1.Invalidate();
                }
            };
        }

        private void SubscribeToHelpMenuControlEvents()
        {
            mnuHelpHfmLogFile.Click += (s, e) => _presenter.ShowHfmLogFile(LocalProcessService.Default);
            mnuHelpHfmDataFiles.Click += (s, e) => _presenter.ShowHfmDataFiles(LocalProcessService.Default);
            mnuHelpHfmGroup.Click += (s, e) => _presenter.ShowHfmGoogleGroup(LocalProcessService.Default);
            mnuHelpCheckForUpdate.Click += (s, e) =>
            {
                using (var scope = _presenter.ServiceScopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<ApplicationUpdateService>();
                    var presenterFactory = scope.ServiceProvider.GetRequiredService<ApplicationUpdatePresenterFactory>();
                    _presenter.CheckForUpdateClick(service, presenterFactory);
                }
            };
            mnuHelpAbout.Click += (s, e) =>
            {
                using (var scope = _presenter.ServiceScopeFactory.CreateScope())
                using (var dialog = scope.ServiceProvider.GetRequiredService<AboutDialog>())
                {
                    dialog.ShowDialog(_presenter.Form);
                }
            };
        }

        private void SubscribeToClientsMenuControlEvents()
        {
            appMenuClientsAddClient.Click += (s, e) =>
            {
                using (var scope = _presenter.ServiceScopeFactory.CreateScope())
                {
                    var presenterFactory = scope.ServiceProvider.GetRequiredService<FahClientSettingsPresenterFactory>();
                    _presenter.ClientsAddClick(presenterFactory);
                }
            };
            appMenuClientsEditClient.Click += (s, e) =>
            {
                using (var scope = _presenter.ServiceScopeFactory.CreateScope())
                {
                    var presenterFactory = scope.ServiceProvider.GetRequiredService<FahClientSettingsPresenterFactory>();
                    _presenter.ClientsEditClick(presenterFactory);
                }
            };
            appMenuClientsDeleteClient.Click += (s, e) => _presenter.ClientsDeleteClick();
            appMenuClientsRefreshSelectedSlot.Click += (s, e) => _presenter.ClientsRefreshSelectedClick();
            appMenuClientsRefreshAllSlots.Click += (s, e) => _presenter.ClientsRefreshAllClick();
            appMenuClientsViewCachedLog.Click += (s, e) => _presenter.ClientsViewCachedLogClick(LocalProcessService.Default);
        }

        private void SubscribeToGridContextMenuControlEvents()
        {
            gridContextMenuItemEditClient.Click += (s, e) =>
            {
                using (var scope = _presenter.ServiceScopeFactory.CreateScope())
                {
                    var presenterFactory = scope.ServiceProvider.GetRequiredService<FahClientSettingsPresenterFactory>();
                    _presenter.ClientsEditClick(presenterFactory);
                }
            };
            gridContextMenuItemDeleteClient.Click += (s, e) => _presenter.ClientsDeleteClick();
            gridContextMenuItemRefreshSelectedSlot.Click += (s, e) => _presenter.ClientsRefreshSelectedClick();
            gridContextMenuItemViewCachedLog.Click += (s, e) => _presenter.ClientsViewCachedLogClick(LocalProcessService.Default);
            gridContextMenuItemFoldSlot.Click += (s, e) => _presenter.ClientsFoldSlotClick();
            gridContextMenuItemPauseSlot.Click += (s, e) => _presenter.ClientsPauseSlotClick();
            gridContextMenuItemFinishSlot.Click += (s, e) => _presenter.ClientsFinishSlotClick();
            gridContextMenuItemCopyPRCG.Click += (s, e) => _presenter.CopyPRCGToClipboardClicked();
        }

        // View Menu Click Handlers
        private void mnuViewAutoSizeGridColumns_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < dataGridView1.Columns.Count; i++)
            {
                AutoSizeColumn(i);
            }
        }

        private void mnuViewInactiveSlots_Click(object sender, EventArgs e)
        {
            _presenter.ShowHideInactiveSlots();
        }

        private void mnuViewMessages_Click(object sender, EventArgs e)
        {
            var presenterFactory = new Func<MessagesPresenter>(() =>
            {
                var scope = _presenter.ServiceScopeFactory.CreateScope();
                var presenter = scope.ServiceProvider.GetRequiredService<MessagesPresenter>();
                presenter.Closed += delegate { scope.Dispose(); };
                return presenter;
            });

            _presenter.ViewMessagesClick(presenterFactory);
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

        // Tools Menu Click Handlers
        private void mnuToolsDownloadProjects_Click(object sender, EventArgs e)
        {
            using (var scope = _presenter.ServiceScopeFactory.CreateScope())
            {
                _presenter.ToolsDownloadProjectsClick(scope.ServiceProvider.GetRequiredService<IProteinService>());
            }
        }

        private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
        {
            var scope = _presenter.ServiceScopeFactory.CreateScope();
            var presenter = scope.ServiceProvider.GetRequiredService<BenchmarksPresenter>();
            presenter.Closed += delegate { scope.Dispose(); };
            _presenter.ToolsBenchmarksClick(presenter);
        }

        private void mnuToolsPointsCalculator_Click(object sender, EventArgs e)
        {
            var scope = _presenter.ServiceScopeFactory.CreateScope();
            var calculatorForm = scope.ServiceProvider.GetRequiredService<ProteinCalculatorForm>();
            calculatorForm.Closed += delegate { scope.Dispose(); };
            calculatorForm.Show();
        }

        public bool WorkUnitHistoryMenuItemEnabled
        {
            get => mnuToolsHistory.Enabled;
            set => mnuToolsHistory.Enabled = value;
        }

        private void mnuToolsHistory_Click(object sender, EventArgs e)
        {
            var presenterFactory = new Func<WorkUnitHistoryPresenter>(() =>
            {
                var scope = _presenter.ServiceScopeFactory.CreateScope();
                var presenter = scope.ServiceProvider.GetRequiredService<WorkUnitHistoryPresenter>();
                presenter.Closed += delegate { scope.Dispose(); };
                return presenter;
            });

            _presenter.ToolsHistoryClick(presenterFactory);
        }

        private void SubscribeToWebMenuControlEvents()
        {
            mnuWebEocUser.Click += (s, e) => _presenter.ShowEocUserPage(LocalProcessService.Default);
            mnuWebFahUser.Click += (s, e) => _presenter.ShowFahUserPage(LocalProcessService.Default, FahUserService.Default);
            mnuWebEocTeam.Click += (s, e) => _presenter.ShowEocTeamPage(LocalProcessService.Default);
            mnuWebRefreshUserStats.Click += (s, e) => _presenter.RefreshUserStatsData();
            mnuWebHfmGitHub.Click += (s, e) => _presenter.ShowHfmGitHub(LocalProcessService.Default);
        }

        private void SubscribeToNotifyIconMenuControlEvents()
        {
            mnuNotifyRst.Click += (s, e) => _presenter.NotifyIconRestoreClick();
            mnuNotifyMin.Click += (s, e) => _presenter.NotifyIconMinimizeClick();
            mnuNotifyMax.Click += (s, e) => _presenter.NotifyIconMaximizeClick();
            mnuNotifyQuit.Click += (s, e) => Close();
        }

        private void SubscribeToUserStatsContextMenuControlEvents()
        {
            mnuContextShowUserStats.Click += (s, e) => _presenter.SetUserStatsDataViewStyle(false);
            mnuContextShowTeamStats.Click += (s, e) => _presenter.SetUserStatsDataViewStyle(true);
            mnuContextForceRefreshEocStats.Click += (s, e) => _presenter.RefreshUserStatsData();
        }

        private DataGridViewColumnSelector _columnSelector;

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            SetupDataGridViewColumns(dataGridView1);

            _columnSelector = new DataGridViewColumnSelector(dataGridView1);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _notifyIcon?.Dispose();
                _columnSelector?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
