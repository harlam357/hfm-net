/*
 * View and DataGridView save state code based on code by Ron Dunant.
 * http://www.codeproject.com/KB/grid/PersistentDataGridView.aspx
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using AutoMapper;

using harlam357.Core;
using harlam357.Core.ComponentModel;
using harlam357.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Forms.Models;
using HFM.Log;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms
{
    public sealed class MainPresenter
    {
        #region Properties

        /// <summary>
        /// Holds the state of the window before it is hidden (minimize to tray behaviour)
        /// </summary>
        public FormWindowState OriginalWindowState { get; private set; }

        private ILogger _logger;

        public ILogger Logger
        {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }

        #endregion

        #region Fields

        private HistoryPresenter _historyPresenter;
        private readonly MainGridModel _gridModel;
        private readonly UserStatsDataModel _userStatsDataModel;

        private readonly IMainView _view;
        private readonly IMessagesView _messagesView;
        private readonly IMessageBoxView _messageBoxView;

        private readonly IViewFactory _viewFactory;
        private readonly IPresenterFactory _presenterFactory;

        private readonly ClientConfiguration _clientConfiguration;
        private readonly IProteinService _proteinService;

        private readonly IUpdateLogic _updateLogic;
        private readonly IExternalProcessStarter _processStarter;

        private readonly IPreferenceSet _prefs;
        private readonly ClientSettingsManager _settingsManager;

        private readonly IMapper _clientSettingsMapper;

        #endregion

        #region Constructor

        public MainPresenter(MainGridModel mainGridModel, IMainView view, IMessagesView messagesView, IViewFactory viewFactory,
                             IMessageBoxView messageBoxView, UserStatsDataModel userStatsDataModel, IPresenterFactory presenterFactory,
                             ClientConfiguration clientConfiguration, IProteinService proteinService, IUpdateLogic updateLogic,
                             IExternalProcessStarter processStarter,
                             IPreferenceSet prefs, ClientSettingsManager settingsManager)
        {
            _gridModel = mainGridModel;
            _gridModel.AfterResetBindings += (sender, e) =>
            {
                // run asynchronously so binding operation can finish
                _view.BeginInvoke(new Action(() =>
                {
                    DisplaySelectedSlotData();
                    _view.RefreshControlsWithTotalsData(_gridModel.SlotTotals);
                }), null);
            };
            _gridModel.SelectedSlotChanged += (sender, e) =>
            {
                if (e.Index >= 0 && e.Index < _view.DataGridView.Rows.Count)
                {
                    // run asynchronously so binding operation can finish
                    _view.BeginInvoke(new Action(() =>
                    {
                        _view.DataGridView.Rows[e.Index].Selected = true;
                        DisplaySelectedSlotData();
                    }), null);
                }
            };
            _userStatsDataModel = userStatsDataModel;

            // Views
            _view = view;
            _messagesView = messagesView;
            _messageBoxView = messageBoxView;
            //
            _viewFactory = viewFactory;
            _presenterFactory = presenterFactory;
            // Collections
            _clientConfiguration = clientConfiguration;
            _proteinService = proteinService;
            // Logic Services
            _updateLogic = updateLogic;
            _updateLogic.Owner = _view;
            _processStarter = processStarter;
            // Data Services
            _prefs = prefs;
            _settingsManager = settingsManager;

            _clientSettingsMapper = new MapperConfiguration(cfg => cfg.AddProfile<FahClientSettingsModelProfile>()).CreateMapper();

            _clientConfiguration.ClientConfigurationChanged += (s, e) => AutoSaveConfig();
        }

        #endregion

        #region Initialize

        private string _openFile;

        public void Initialize(string openFile)
        {
            _openFile = openFile;

            // Restore View Preferences (must be done AFTER DataGridView columns are setup)
            RestoreViewPreferences();
            //
            _view.SetGridDataSource(_gridModel.BindingSource);
            //
            _prefs.PreferenceChanged += (s, e) =>
            {
                switch (e.Preference)
                {
                    case Preference.MinimizeTo:
                        SetViewShowStyle();
                        break;
                    case Preference.ColorLogFile:
                        ApplyColorLogFileSetting();
                        break;
                    case Preference.EocUserId:
                        _userStatsDataModel.Refresh();
                        break;
                }
            };
        }

        private void RestoreViewPreferences()
        {
            // Would like to do this here in lieu of in frmMain_Shown() event.
            // There is some drawing error that if Minimized here, the first time the
            // Form is restored from the system tray, the DataGridView is drawn with
            // a big black box on the right hand side. Like it didn't get initialized
            // properly when the Form was created.
            //if (_prefs.Get<bool>(Preference.RunMinimized))
            //{
            //   WindowState = FormWindowState.Minimized;
            //}

            // Look for start position
            var location = _prefs.Get<Point>(Preference.FormLocation);
            var size = _prefs.Get<Size>(Preference.FormSize);
            if (location.X != 0 && location.Y != 0)
            {
                _view.SetManualStartPosition();
                _view.Location = WindowPosition.Normalize(location, size);
            }
            // Look for view size
            if (size.Width != 0 && size.Height != 0)
            {
                // make sure values coming from the prefs are at least the minimums - Issue 234
                if (size.Width < _view.MinimumSize.Width) size.Width = _view.MinimumSize.Width;
                if (size.Height < _view.MinimumSize.Height) size.Height = _view.MinimumSize.Height;

                if (!_prefs.Get<bool>(Preference.FormLogWindowVisible))
                {
                    size = new Size(size.Width, size.Height + _prefs.Get<int>(Preference.FormLogWindowHeight));
                }
                _view.Size = size;
                // make sure split location from the prefs is at least the minimum panel size - Issue 234
                var formSplitLocation = _prefs.Get<int>(Preference.FormSplitterLocation);
                if (formSplitLocation < _view.SplitContainer.Panel2MinSize) formSplitLocation = _view.SplitContainer.Panel2MinSize;
                _view.SplitContainer.SplitterDistance = formSplitLocation;
            }

            if (!_prefs.Get<bool>(Preference.FormLogWindowVisible))
            {
                ShowHideLogWindow(false);
            }
            if (!_prefs.Get<bool>(Preference.QueueWindowVisible))
            {
                ShowHideQueue(false);
            }
            _view.FollowLogFileChecked = _prefs.Get<bool>(Preference.FollowLog);

            _gridModel.SortColumnName = _prefs.Get<string>(Preference.FormSortColumn);
            _gridModel.SortColumnOrder = _prefs.Get<ListSortDirection>(Preference.FormSortOrder);

            try
            {
                // Restore the columns' state
                var columns = _prefs.Get<ICollection<string>>(Preference.FormColumns);
                if (columns != null)
                {
                    var colsList = columns.ToList();
                    colsList.Sort();

                    for (int i = 0; i < colsList.Count && i < MainForm.NumberOfDisplayFields; i++)
                    {
                        string[] tokens = colsList[i].Split(',');
                        int index = Int32.Parse(tokens[3]);
                        _view.DataGridView.Columns[index].DisplayIndex = Int32.Parse(tokens[0]);
                        if (_view.DataGridView.Columns[index].AutoSizeMode.Equals(DataGridViewAutoSizeColumnMode.Fill) == false)
                        {
                            _view.DataGridView.Columns[index].Width = Int32.Parse(tokens[1]);
                        }
                        _view.DataGridView.Columns[index].Visible = Boolean.Parse(tokens[2]);
                    }
                }
            }
            catch (NullReferenceException)
            {
                // This happens when the FormColumns setting is empty
            }
        }

        #endregion

        #region View Handling Methods

        public void ViewShown()
        {
            // Add the Index Changed Handler here after everything is shown
            _view.DataGridView.ColumnDisplayIndexChanged += delegate { DataGridViewColumnDisplayIndexChanged(); };
            // Then run it once to ensure the last column is set to Fill
            DataGridViewColumnDisplayIndexChanged();
            // Add the Splitter Moved Handler here after everything is shown - Issue 8
            // When the log file window (Panel2) is visible, this event will fire.
            // Update the split location directly from the split panel control. - Issue 8
            _view.SplitContainer.SplitterMoved += delegate
                                                  {
                                                      _prefs.Set(Preference.FormSplitterLocation, _view.SplitContainer.SplitterDistance);
                                                      _prefs.Save();
                                                  };

            if (_prefs.Get<bool>(Preference.RunMinimized))
            {
                _view.WindowState = FormWindowState.Minimized;
            }

            if (!String.IsNullOrEmpty(_openFile))
            {
                LoadConfigFile(_openFile);
            }
            else if (_prefs.Get<bool>(Preference.UseDefaultConfigFile))
            {
                var fileName = _prefs.Get<string>(Preference.DefaultConfigFile);
                if (!String.IsNullOrEmpty(fileName))
                {
                    LoadConfigFile(fileName);
                }
            }

            SetViewShowStyle();

            if (_prefs.Get<bool>(Preference.StartupCheckForUpdate))
            {
                _updateLogic.CheckForUpdate();
            }
        }

        public void ViewResize()
        {
            if (_view.WindowState != FormWindowState.Minimized)
            {
                OriginalWindowState = _view.WindowState;
                // ReApply Sort when restoring from the sys tray - Issue 32
                if (_view.ShowInTaskbar == false)
                {
                    _gridModel.Sort();
                }
            }

            SetViewShowStyle();

            // When the log file window (panel) is collapsed, get the split location
            // changes based on the height of Panel1 - Issue 8
            if (_view.Visible && _view.SplitContainer.Panel2Collapsed)
            {
                _prefs.Set(Preference.FormSplitterLocation, _view.SplitContainer.Panel1.Height);
            }
        }

        public bool ViewClosing()
        {
            if (!CheckForConfigurationChanges())
            {
                return true;
            }

            SaveColumnSettings();

            // Save location and size data
            // RestoreBounds remembers normal position if minimized or maximized
            if (_view.WindowState == FormWindowState.Normal)
            {
                _prefs.Set(Preference.FormLocation, _view.Location);
                _prefs.Set(Preference.FormSize, _view.Size);
            }
            else
            {
                _prefs.Set(Preference.FormLocation, _view.RestoreBounds.Location);
                _prefs.Set(Preference.FormSize, _view.RestoreBounds.Size);
            }

            _prefs.Set(Preference.FormLogWindowVisible, _view.LogFileViewer.Visible);
            _prefs.Set(Preference.QueueWindowVisible, _view.QueueControlVisible);

            CheckForAndFireUpdateProcess();

            return false;
        }

        public void SetViewShowStyle()
        {
            switch (_prefs.Get<MinimizeToOption>(Preference.MinimizeTo))
            {
                case MinimizeToOption.SystemTray:
                    _view.SetNotifyIconVisible(true);
                    _view.ShowInTaskbar = (_view.WindowState != FormWindowState.Minimized);
                    break;
                case MinimizeToOption.TaskBar:
                    _view.SetNotifyIconVisible(false);
                    _view.ShowInTaskbar = true;
                    break;
                case MinimizeToOption.Both:
                    _view.SetNotifyIconVisible(true);
                    _view.ShowInTaskbar = true;
                    break;
            }
        }

        private void CheckForAndFireUpdateProcess()
        {
            if (!String.IsNullOrEmpty(_updateLogic.UpdateFilePath))
            {
                Logger.Info($"Firing update file '{_updateLogic.UpdateFilePath}'...");
                try
                {
                    Process.Start(_updateLogic.UpdateFilePath);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    string message = String.Format(CultureInfo.CurrentCulture,
                                                   "Update process failed to start with the following error:{0}{0}{1}",
                                                   Environment.NewLine, ex.Message);
                    _messageBoxView.ShowError(_view, message, _view.Text);
                }
            }
        }

        #endregion

        #region Data Grid View Handling Methods

        private void DisplaySelectedSlotData()
        {
            if (_gridModel.SelectedSlot != null)
            {
                // TODO: StatusLabelLeftText for v7 client
                //_view.StatusLabelLeftText = _gridModel.SelectedSlot.ClientPathAndArguments;

                _view.SetWorkUnitInfos(_gridModel.SelectedSlot.WorkUnitInfos,
                                       _gridModel.SelectedSlot.WorkUnitModel.WorkUnit.SlotType);

                // if we've got a good queue read, let queueControl_QueueIndexChanged()
                // handle populating the log lines.
                if (_gridModel.SelectedSlot.WorkUnitInfos != null) return;

                // otherwise, load up the CurrentLogLines
                SetLogLines(_gridModel.SelectedSlot, _gridModel.SelectedSlot.CurrentLogLines);
            }
            else
            {
                ClearLogAndQueueViewer();
            }
        }

        public void QueueIndexChanged(int index)
        {
            if (index == -1)
            {
                _view.LogFileViewer.SetNoLogLines();
                return;
            }

            if (_gridModel.SelectedSlot != null)
            {
                // Check the UnitLogLines array against the requested Queue Index - Issue 171
                try
                {
                    var logLines = _gridModel.SelectedSlot.GetLogLinesForQueueIndex(index);
                    // show the current log even if not the current unit index - 2/17/12
                    if (logLines == null) // && index == _gridModel.SelectedSlot.Queue.CurrentWorkUnitKey)
                    {
                        logLines = _gridModel.SelectedSlot.CurrentLogLines;
                    }

                    SetLogLines(_gridModel.SelectedSlot, logLines);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Logger.Error(ex.Message, ex);
                    _view.LogFileViewer.SetNoLogLines();
                }
            }
            else
            {
                ClearLogAndQueueViewer();
            }
        }

        private void ClearLogAndQueueViewer()
        {
            // clear the log text
            _view.LogFileViewer.SetNoLogLines();
            // clear the queue control
            _view.SetWorkUnitInfos(null);
        }

        private void SetLogLines(SlotModel instance, IList<LogLine> logLines)
        {
            /*** Checked LogLine Count ***/
            if (logLines != null && logLines.Count > 0)
            {
                // Different Client... Load LogLines
                if (_view.LogFileViewer.LogOwnedByInstanceName.Equals(instance.Name) == false)
                {
                    _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.Get<bool>(Preference.ColorLogFile));
                }
                // Textbox has text lines
                else if (_view.LogFileViewer.Lines.Length > 0)
                {
                    string lastLogLine = String.Empty;

                    try // to get the last LogLine from the instance
                    {
                        lastLogLine = logLines[logLines.Count - 1].ToString();
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // even though i've checked the count above, it could have changed in between then
                        // and now... and if the count is 0 it will yield this exception.  Log It!!!
                        Logger.Warn(String.Format(Core.Logging.Logger.NameFormat, instance.Name, ex.Message), ex);
                    }

                    // If the last text line in the textbox DOES NOT equal the last LogLine Text... Load LogLines.
                    // Otherwise, the log has not changed, don't update and perform the log "flicker".
                    if (_view.LogFileViewer.Lines[_view.LogFileViewer.Lines.Length - 1].Equals(lastLogLine) == false)
                    {
                        _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.Get<bool>(Preference.ColorLogFile));
                    }
                }
                // Nothing in the Textbox... Load LogLines
                else
                {
                    _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.Get<bool>(Preference.ColorLogFile));
                }
            }
            else
            {
                _view.LogFileViewer.SetNoLogLines();
            }

            if (_prefs.Get<bool>(Preference.FollowLog))
            {
                _view.LogFileViewer.ScrollToBottom();
            }
        }

        private void DataGridViewColumnDisplayIndexChanged()
        {
            if (_view.DataGridView.Columns.Count == MainForm.NumberOfDisplayFields)
            {
                foreach (DataGridViewColumn column in _view.DataGridView.Columns)
                {
                    if (column.DisplayIndex < _view.DataGridView.Columns.Count - 1)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    }
                    else
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }

                SaveColumnSettings(); // Save Column Settings - Issue 73
                _prefs.Save();
            }
        }

        private void SaveColumnSettings()
        {
            // Save column state data
            // including order, column width and whether or not the column is visible
            var columns = new List<string>();
            int i = 0;

            foreach (DataGridViewColumn column in _view.DataGridView.Columns)
            {
                columns.Add(String.Format(CultureInfo.InvariantCulture,
                                        "{0},{1},{2},{3}",
                                        column.DisplayIndex.ToString("D2"),
                                        column.Width,
                                        column.Visible,
                                        i++));
            }

            _prefs.Set(Preference.FormColumns, columns);
        }

        public void DataGridViewSorted()
        {
            _gridModel.ResetSelectedSlot();
        }

        public void DataGridViewMouseDown(int coordX, int coordY, MouseButtons button, int clicks)
        {
            DataGridView.HitTestInfo hti = _view.DataGridView.HitTest(coordX, coordY);
            if (button == MouseButtons.Right)
            {
                if (hti.Type == DataGridViewHitTestType.Cell)
                {
                    if (_view.DataGridView.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected == false)
                    {
                        _view.DataGridView.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected = true;
                    }

                    // Check for SelectedSlot, and get out if not found
                    if (_gridModel.SelectedSlot == null) return;

                    _view.ShowGridContextMenuStrip(_view.DataGridView.PointToScreen(new Point(coordX, coordY)));
                }
            }
            if (button == MouseButtons.Left && clicks == 2)
            {
                if (hti.Type == DataGridViewHitTestType.Cell)
                {
                    // Check for SelectedSlot, and get out if not found
                    if (_gridModel.SelectedSlot == null) return;

                    // TODO: What to do on double left click on v7 client?
                }
            }
        }

        #endregion

        #region File Handling Methods

        public void FileNewClick()
        {
            if (CheckForConfigurationChanges())
            {
                ClearConfiguration();
            }
        }

        public void FileOpenClick()
        {
            if (CheckForConfigurationChanges())
            {
                var openFileDialogView = _viewFactory.GetOpenFileDialogView();
                openFileDialogView.DefaultExt = _settingsManager.FileExtension;
                openFileDialogView.Filter = _settingsManager.FileTypeFilters;
                openFileDialogView.FileName = _settingsManager.FileName;
                openFileDialogView.RestoreDirectory = true;
                if (openFileDialogView.ShowDialog() == DialogResult.OK)
                {
                    ClearConfiguration();
                    LoadConfigFile(openFileDialogView.FileName, openFileDialogView.FilterIndex);
                }
                _viewFactory.Release(openFileDialogView);
            }
        }

        private void ClearConfiguration()
        {
            // clear the clients and UI
            _settingsManager.ClearFileName();
            _clientConfiguration.Clear();
        }

        private void LoadConfigFile(string filePath, int filterIndex = 1)
        {
            Debug.Assert(filePath != null);

            try
            {
                // Read the config file
                _clientConfiguration.Load(_settingsManager.Read(filePath, filterIndex));

                if (_clientConfiguration.Count == 0)
                {
                    _messageBoxView.ShowError(_view, "No client configurations were loaded from the given config file.", _view.Text);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                   "No client configurations were loaded from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message), _view.Text);
            }
        }

        private void AutoSaveConfig()
        {
            if (_prefs.Get<bool>(Preference.AutoSaveConfig) &&
                _clientConfiguration.IsDirty)
            {
                FileSaveClick();
            }
        }

        public void FileSaveClick()
        {
            if (_clientConfiguration.Count == 0)
            {
                return;
            }

            if (String.IsNullOrEmpty(_settingsManager.FileName))
            {
                FileSaveAsClick();
            }
            else
            {
                try
                {
                    _settingsManager.Write(_clientConfiguration.GetClients().Select(x => x.Settings), _settingsManager.FileName, _settingsManager.FilterIndex);
                    _clientConfiguration.IsDirty = false;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                       "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message), _view.Text);
                }
            }
        }

        public void FileSaveAsClick()
        {
            if (_clientConfiguration.Count == 0)
            {
                return;
            }

            var saveFileDialogView = _viewFactory.GetSaveFileDialogView();
            saveFileDialogView.DefaultExt = _settingsManager.FileExtension;
            saveFileDialogView.Filter = _settingsManager.FileTypeFilters;
            if (saveFileDialogView.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Issue 75
                    _settingsManager.Write(_clientConfiguration.GetClients().Select(x => x.Settings), saveFileDialogView.FileName, saveFileDialogView.FilterIndex);
                    _clientConfiguration.IsDirty = false;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                       "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message), _view.Text);
                }
            }
            _viewFactory.Release(saveFileDialogView);
        }

        private bool CheckForConfigurationChanges()
        {
            if (_clientConfiguration.Count != 0 && _clientConfiguration.IsDirty)
            {
                DialogResult result = _messageBoxView.AskYesNoCancelQuestion(_view,
                   String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes / No - Continue and do not save the changes / Cancel - Do not continue", Environment.NewLine),
                   _view.Text);

                switch (result)
                {
                    case DialogResult.Yes:
                        FileSaveClick();
                        return !_clientConfiguration.IsDirty;
                    case DialogResult.No:
                        return true;
                    case DialogResult.Cancel:
                        return false;
                }
                return false;
            }

            return true;
        }

        #endregion

        #region Edit Menu Handling Methods

        public void EditPreferencesClick()
        {
            var prefDialog = _viewFactory.GetPreferencesDialog();
            prefDialog.ShowDialog(_view);
            _viewFactory.Release(prefDialog);

            _view.DataGridView.Invalidate();
        }

        #endregion

        #region Help Menu Handling Methods

        public void ShowHfmLogFile()
        {
            HandleProcessStartResult(_processStarter.ShowHfmLogFile());
        }

        public void ShowHfmDataFiles()
        {
            HandleProcessStartResult(_processStarter.ShowFileExplorer(_prefs.Get<string>(Preference.ApplicationDataFolderPath)));
        }

        public void ShowHfmGoogleGroup()
        {
            HandleProcessStartResult(_processStarter.ShowHfmGoogleGroup());
        }

        public void CheckForUpdateClick()
        {
            // if already in progress, stub out...
            if (_updateLogic.CheckInProgress) return;

            _updateLogic.CheckForUpdate(true);
        }

        #endregion

        #region Clients Menu Handling Methods

        internal void ClientsAddClick()
        {
            var dialog = _presenterFactory.GetFahClientSetupPresenter();
            dialog.SettingsModel = new FahClientSettingsModel();
            while (dialog.ShowDialog(_view) == DialogResult.OK)
            {
                var settings = _clientSettingsMapper.Map<FahClientSettingsModel, ClientSettings>(dialog.SettingsModel);
                //if (_clientDictionary.ContainsKey(settings.Name))
                //{
                //   string message = String.Format(CultureInfo.CurrentCulture, "Client name '{0}' already exists.", settings.Name);
                //   _messageBoxView.ShowError(_view, Core.Application.NameAndVersion, message);
                //   continue;
                //}
                // perform the add
                try
                {
                    _clientConfiguration.Add(settings);
                    break;
                }
                catch (ArgumentException ex)
                {
                    Logger.Error(ex.Message, ex);
                    _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
                }
            }
            _presenterFactory.Release(dialog);
        }

        public void ClientsEditClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (_gridModel.SelectedSlot == null) return;

            EditFahClient();
        }

        private void EditFahClient()
        {
            Debug.Assert(_gridModel.SelectedSlot != null);
            IClient client = _clientConfiguration.Get(_gridModel.SelectedSlot.Settings.Name);
            ClientSettings originalSettings = client.Settings;
            Debug.Assert(originalSettings.ClientType == ClientType.FahClient);

            var dialog = _presenterFactory.GetFahClientSetupPresenter();
            dialog.SettingsModel = _clientSettingsMapper.Map<ClientSettings, FahClientSettingsModel>(originalSettings);
            while (dialog.ShowDialog(_view) == DialogResult.OK)
            {
                var newSettings = _clientSettingsMapper.Map<FahClientSettingsModel, ClientSettings>(dialog.SettingsModel);
                // perform the edit
                try
                {
                    _clientConfiguration.Edit(originalSettings.Name, newSettings);
                    break;
                }
                catch (ArgumentException ex)
                {
                    Logger.Error(ex.Message, ex);
                    _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
                }
            }
            _presenterFactory.Release(dialog);
        }

        public void ClientsDeleteClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (_gridModel.SelectedSlot == null) return;

            _clientConfiguration.Remove(_gridModel.SelectedSlot.Settings.Name);
        }

        public void ClientsRefreshSelectedClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (_gridModel.SelectedSlot == null) return;

            var client = _clientConfiguration.Get(_gridModel.SelectedSlot.Settings.Name);
            Task.Factory.StartNew(client.Retrieve);
        }

        public void ClientsRefreshAllClick()
        {
            _clientConfiguration.ScheduledTasks.RetrieveAll();
        }

        public void ClientsViewCachedLogClick()
        {
            // Check for SelectedSlot, and get out if not found
            if (_gridModel.SelectedSlot == null) return;

            string logFilePath = Path.Combine(_prefs.Get<string>(Preference.CacheDirectory), _gridModel.SelectedSlot.Settings.ClientLogFileName);
            if (File.Exists(logFilePath))
            {
                HandleProcessStartResult(_processStarter.ShowCachedLogFile(logFilePath));
            }
            else
            {
                string message = String.Format(CultureInfo.CurrentCulture, "The log file for '{0}' does not exist.",
                                               _gridModel.SelectedSlot.Settings.Name);
                _messageBoxView.ShowInformation(_view, message, _view.Text);
            }
        }

        #endregion

        #region Grid Context Menu Handling Methods

        internal void ClientsFoldSlotClick()
        {
            if (_gridModel.SelectedSlot == null) return;

            for (var client = _clientConfiguration.Get(_gridModel.SelectedSlot.Settings.Name) as IFahClient; client != null; client = null)
            {
                client.Fold(_gridModel.SelectedSlot.SlotID);
            }
        }

        internal void ClientsPauseSlotClick()
        {
            if (_gridModel.SelectedSlot == null) return;

            for (var client = _clientConfiguration.Get(_gridModel.SelectedSlot.Settings.Name) as IFahClient; client != null; client = null)
            {
                client.Pause(_gridModel.SelectedSlot.SlotID);
            }
        }

        internal void ClientsFinishSlotClick()
        {
            if (_gridModel.SelectedSlot == null) return;

            for (var client = _clientConfiguration.Get(_gridModel.SelectedSlot.Settings.Name) as IFahClient; client != null; client = null)
            {
                client.Finish(_gridModel.SelectedSlot.SlotID);
            }
        }

        #endregion

        #region View Menu Handling Methods

        public void ViewMessagesClick()
        {
            if (_messagesView.Visible)
            {
                _messagesView.Close();
            }
            else
            {
                // Restore state data
                var location = _prefs.Get<Point>(Preference.MessagesFormLocation);
                var size = _prefs.Get<Size>(Preference.MessagesFormSize);
                location = WindowPosition.Normalize(location, size);

                if (location.X != 0 && location.Y != 0)
                {
                    _messagesView.SetManualStartPosition();
                    _messagesView.SetLocation(location.X, location.Y);
                }

                if (size.Width != 0 && size.Height != 0)
                {
                    _messagesView.SetSize(size.Width, size.Height);
                }

                _messagesView.Show();
                _messagesView.ScrollToEnd();
            }
        }

        public void ShowHideLogWindow()
        {
            ShowHideLogWindow(!_view.LogFileViewer.Visible);
        }

        private void ShowHideLogWindow(bool show)
        {
            if (!show)
            {
                _view.LogFileViewer.Visible = false;
                _view.SplitContainer.Panel2Collapsed = true;
                _prefs.Set(Preference.FormLogWindowHeight, (_view.SplitContainer.Height - _view.SplitContainer.SplitterDistance));
                _view.Size = new Size(_view.Size.Width, _view.Size.Height - _prefs.Get<int>(Preference.FormLogWindowHeight));
            }
            else
            {
                _view.LogFileViewer.Visible = true;
                _view.DisableViewResizeEvent();  // disable Form resize event for this operation
                _view.Size = new Size(_view.Size.Width, _view.Size.Height + _prefs.Get<int>(Preference.FormLogWindowHeight));
                _view.EnableViewResizeEvent();   // re-enable
                _view.SplitContainer.Panel2Collapsed = false;
            }
        }

        public void ShowHideQueue()
        {
            ShowHideQueue(!_view.QueueControlVisible);
        }

        private void ShowHideQueue(bool show)
        {
            if (!show)
            {
                _view.QueueControlVisible = false;
                _view.SetQueueButtonText(String.Format(CultureInfo.CurrentCulture, "S{0}h{0}o{0}w{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine));
                _view.SplitContainer2.SplitterDistance = 27;
            }
            else
            {
                _view.QueueControlVisible = true;
                _view.SetQueueButtonText(String.Format(CultureInfo.CurrentCulture, "H{0}i{0}d{0}e{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine));
                _view.SplitContainer2.SplitterDistance = 289;
            }
        }

        public void ViewToggleDateTimeClick()
        {
            var style = _prefs.Get<TimeFormatting>(Preference.TimeFormatting);
            _prefs.Set(Preference.TimeFormatting, style == TimeFormatting.None
                                    ? TimeFormatting.Format1
                                    : TimeFormatting.None);
            _prefs.Save();
            _view.DataGridView.Invalidate();
        }

        public void ViewToggleCompletedCountStyleClick()
        {
            var style = _prefs.Get<UnitTotalsType>(Preference.UnitTotals);
            _prefs.Set(Preference.UnitTotals, style == UnitTotalsType.All
                                    ? UnitTotalsType.ClientStart
                                    : UnitTotalsType.All);
            _prefs.Save();
            _view.DataGridView.Invalidate();
        }

        public void ViewToggleVersionInformationClick()
        {
            _prefs.Set(Preference.DisplayVersions, !_prefs.Get<bool>(Preference.DisplayVersions));
            _prefs.Save();
            _view.DataGridView.Invalidate();
        }

        public void ViewCycleBonusCalculationClick()
        {
            var calculationType = _prefs.Get<BonusCalculation>(Preference.BonusCalculation);
            int typeIndex = 0;
            // None is always LAST entry
            if (calculationType != BonusCalculation.None)
            {
                typeIndex = (int)calculationType;
                typeIndex++;
            }

            calculationType = (BonusCalculation)typeIndex;
            _prefs.Set(Preference.BonusCalculation, calculationType);
            _prefs.Save();

            string calculationTypeString = (from item in OptionsModel.BonusCalculationList
                                            where ((BonusCalculation)item.ValueMember) == calculationType
                                            select item.DisplayMember).First();
            _view.ShowNotifyToolTip(calculationTypeString);
            _view.DataGridView.Invalidate();
        }

        public void ViewCycleCalculationClick()
        {
            var calculationType = _prefs.Get<PPDCalculation>(Preference.PPDCalculation);
            int typeIndex = 0;
            // EffectiveRate is always LAST entry
            if (calculationType != PPDCalculation.EffectiveRate)
            {
                typeIndex = (int)calculationType;
                typeIndex++;
            }

            calculationType = (PPDCalculation)typeIndex;
            _prefs.Set(Preference.PPDCalculation, calculationType);
            _prefs.Save();

            string calculationTypeString = (from item in OptionsModel.PpdCalculationList
                                            where ((PPDCalculation)item.ValueMember) == calculationType
                                            select item.DisplayMember).First();
            _view.ShowNotifyToolTip(calculationTypeString);
            _view.DataGridView.Invalidate();
        }

        internal void ViewToggleFollowLogFile()
        {
            _prefs.Set(Preference.FollowLog, !_prefs.Get<bool>(Preference.FollowLog));
        }

        #endregion

        #region Tools Menu Handling Methods

        public async void ToolsDownloadProjectsClick()
        {
            try
            {
                var downloader = new ProjectDownloader(_proteinService);
                await downloader.ExecuteAsyncWithProgress(true);
                if (downloader.Result != null)
                {
                    var proteinChanges = downloader.Result.Where(x => x.Result != ProteinDictionaryChangeResult.NoChange).ToList();
                    if (proteinChanges.Count > 0)
                    {
                        if (_clientConfiguration.Count > 0)
                        {
                            _clientConfiguration.ScheduledTasks.RetrieveAll();
                        }
                        using (var dlg = new ProteinLoadResultsDialog())
                        {
                            dlg.DataBind(proteinChanges);
                            dlg.ShowDialog(_view);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _messageBoxView.ShowError(ex.Message, Core.Application.NameAndVersion);
            }
        }

        private sealed class ProjectDownloader : AsyncProcessorBase<IEnumerable<ProteinDictionaryChange>>
        {
            private readonly IProteinService _proteinService;

            public ProjectDownloader(IProteinService proteinService)
            {
                if (proteinService == null) throw new ArgumentNullException("proteinService");
                _proteinService = proteinService;
            }

            protected override async Task<IEnumerable<ProteinDictionaryChange>> OnExecuteAsync(IProgress<ProgressInfo> progress)
            {
                return await Task.Run(() => _proteinService.Refresh(progress)).ConfigureAwait(false);
            }
        }

        public void ToolsBenchmarksClick()
        {
            int projectId = 0;

            // Check for SelectedSlot, and if found... load its ProjectID.
            if (_gridModel.SelectedSlot != null)
            {
                projectId = _gridModel.SelectedSlot.WorkUnitModel.WorkUnit.ProjectID;
            }

            var benchmarksView = _viewFactory.GetBenchmarksForm();
            benchmarksView.Closed += (s, e) => _viewFactory.Release(benchmarksView);
            benchmarksView.ProjectId = projectId;

            // Restore state data
            var location = _prefs.Get<Point>(Preference.BenchmarksFormLocation);
            var size = _prefs.Get<Size>(Preference.BenchmarksFormSize);
            location = WindowPosition.Normalize(location, size);

            if (location.X != 0 && location.Y != 0)
            {
                benchmarksView.Location = location;
            }
            else
            {
                benchmarksView.Location = WindowPosition.CenterOnPrimaryScreen(size);
            }

            if (size.Width != 0 && size.Height != 0)
            {
                benchmarksView.Size = size;
            }

            benchmarksView.Show();
        }

        public void ToolsHistoryClick()
        {
            Debug.Assert(_view.WorkUnitHistoryMenuEnabled);

            if (_historyPresenter == null)
            {
                _historyPresenter = _presenterFactory.GetHistoryPresenter();
                _historyPresenter.Initialize();
                _historyPresenter.PresenterClosed += (sender, args) =>
                {
                    _presenterFactory.Release(_historyPresenter);
                    _historyPresenter = null;
                };
            }

            if (_historyPresenter != null)
            {
                _historyPresenter.Show();
            }
        }

        internal void ToolsPointsCalculatorClick()
        {
            var calculatorForm = _viewFactory.GetProteinCalculatorForm();
            calculatorForm.Closed += (s, e) => _viewFactory.Release(calculatorForm);
            calculatorForm.Show(_view);
        }

        #endregion

        #region Web Menu Handling Methods

        public void ShowEocUserPage()
        {
            HandleProcessStartResult(_processStarter.ShowEocUserPage());
        }

        public void ShowStanfordUserPage()
        {
            HandleProcessStartResult(_processStarter.ShowStanfordUserPage());
        }

        public void ShowEocTeamPage()
        {
            HandleProcessStartResult(_processStarter.ShowEocTeamPage());
        }

        public void RefreshUserStatsData()
        {
            _userStatsDataModel.Refresh();
        }

        public void ShowHfmGitHub()
        {
            HandleProcessStartResult(_processStarter.ShowHfmGitHub());
        }

        #endregion

        #region System Tray Icon Handling Methods

        public void NotifyIconDoubleClick()
        {
            if (_view.WindowState == FormWindowState.Minimized)
            {
                _view.WindowState = OriginalWindowState;
            }
            else
            {
                OriginalWindowState = _view.WindowState;
                _view.WindowState = FormWindowState.Minimized;
            }
        }

        public void NotifyIconRestoreClick()
        {
            if (_view.WindowState == FormWindowState.Minimized)
            {
                _view.WindowState = OriginalWindowState;
            }
            else if (_view.WindowState == FormWindowState.Maximized)
            {
                _view.WindowState = FormWindowState.Normal;
            }
        }

        public void NotifyIconMinimizeClick()
        {
            if (_view.WindowState != FormWindowState.Minimized)
            {
                OriginalWindowState = _view.WindowState;
                _view.WindowState = FormWindowState.Minimized;
            }
        }

        public void NotifyIconMaximizeClick()
        {
            if (_view.WindowState != FormWindowState.Maximized)
            {
                _view.WindowState = FormWindowState.Maximized;
                OriginalWindowState = _view.WindowState;
            }
        }

        #endregion

        #region Other Handling Methods

        private void ApplyColorLogFileSetting()
        {
            _view.LogFileViewer.HighlightLines(_prefs.Get<bool>(Preference.ColorLogFile));
        }

        private void HandleProcessStartResult(string message)
        {
            if (message != null)
            {
                _messageBoxView.ShowError(_view, message, _view.Text);
            }
        }

        public void SetUserStatsDataViewStyle(bool showTeamStats)
        {
            _userStatsDataModel.SetViewStyle(showTeamStats);
        }

        #endregion
    }
}
