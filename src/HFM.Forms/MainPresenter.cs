/*
 * HFM.NET - Main View Presenter
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

/*
 * View and DataGridView save state code based on code by Ron Dunant.
 * http://www.codeproject.com/KB/grid/PersistentDataGridView.aspx
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using Castle.Core.Logging;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public class MainPresenter
   {
      #region Properties
   
      /// <summary>
      /// Holds the state of the window before it is hidden (minimize to tray behaviour)
      /// </summary>
      public FormWindowState OriginalWindowState { get; private set; }

      /// <summary>
      /// Denotes the Saved State of the Current Client Configuration (false == saved, true == unsaved)
      /// </summary>
      private bool ChangedAfterSave { get; set; }

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         get { return _logger; }
         set { _logger = value; }
      }
      
      #endregion
      
      #region Fields

      private HistoryPresenter _historyPresenter;
      private readonly MainGridModel _gridModel;
   
      #region Views
   
      private readonly IMainView _view;
      private readonly IMessagesView _messagesView;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IOpenFileDialogView _openFileDialogView;
      private readonly ISaveFileDialogView _saveFileDialogView;

      #endregion

      #region Collections

      private readonly IClientDictionary _clientDictionary;

      #endregion

      #region Logic Services

      private readonly IUpdateLogic _updateLogic;
      private readonly RetrievalLogic _retrievalLogic;
      private readonly IExternalProcessStarter _processStarter;

      #endregion

      #region Data Services

      private readonly IPreferenceSet _prefs;
      
      #endregion

      #endregion

      #region Constructor

      public MainPresenter(IMainView view, IMessagesView messagesView, IMessageBoxView messageBoxView,
                           IOpenFileDialogView openFileDialogView, ISaveFileDialogView saveFileDialogView,
                           IClientDictionary clientDictionary, IUpdateLogic updateLogic, 
                           RetrievalLogic retrievalLogic, IExternalProcessStarter processStarter, 
                           IPreferenceSet prefs)
      {
         _gridModel = new MainGridModel(prefs, clientDictionary);
         _gridModel.BeforeResetBindings += delegate { _view.DataGridView.FreezeSelectionChanged = true; };
         _gridModel.AfterResetBindings += delegate { _view.DataGridView.FreezeSelectionChanged = false; };
         _gridModel.SelectedSlotChanged += (sender, e) =>
                                           {
                                              _view.DataGridView.Rows[e.Index].Selected = true;
                                              DisplaySelectedSlotData();
                                              _view.RefreshControlsWithTotalsData(_clientDictionary.Slots.GetSlotTotals());
                                           };

         // Views
         _view = view;
         _messagesView = messagesView;
         _messageBoxView = messageBoxView;
         _openFileDialogView = openFileDialogView;
         _saveFileDialogView = saveFileDialogView;
         // Collections
         _clientDictionary = clientDictionary;
         // Logic Services
         _updateLogic = updateLogic;
         _updateLogic.Owner = _view;
         _retrievalLogic = retrievalLogic;
         _retrievalLogic.Initialize(this);
         _processStarter = processStarter;
         // Data Services
         _prefs = prefs;

         // Hook-up Event Handlers
         //_proteinCollection.Downloader.ProjectInfoUpdated += delegate { _retrievalLogic.QueueNewRetrieval(); };
      }
      
      #endregion

      #region Initialize

      public void Initialize()
      {
         // Restore Form Preferences (must be done AFTER DataGridView columns are setup)
         RestoreViewPreferences();
         // 
         _view.DataGridView.AutoGenerateColumns = false;
         _view.DataGridView.DataSource = _gridModel.BindingSource;
         // 
         _prefs.FormShowStyleSettingsChanged += delegate { SetViewShowStyle(); };
         _prefs.ColorLogFileChanged += delegate { ApplyColorLogFileSetting(); };
      }

      private void RestoreViewPreferences()
      {
         // Would like to do this here in lieu of in frmMain_Shown() event.
         // There is some drawing error that if Minimized here, the first time the
         // Form is restored from the system tray, the DataGridView is drawn with
         // a big black box on the right hand side. Like it didn't get initialized
         // properly when the Form was created.
         //if (Prefs.RunMinimized)
         //{
         //   WindowState = FormWindowState.Minimized;
         //}

         // Look for start position
         var location = _prefs.GetPreference<Point>(Preference.FormLocation);
         if (location.X != 0 && location.Y != 0)
         {
            _view.SetManualStartPosition();
            _view.Location = location;
         }
         // Look for view size
         var size = _prefs.GetPreference<Size>(Preference.FormSize);
         if (size.Width != 0 && size.Height != 0)
         {
            if (!_prefs.GetPreference<bool>(Preference.FormLogVisible))
            {
               size = new Size(size.Width, size.Height + _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            }
            _view.Size = size;
            _view.SplitContainer.SplitterDistance = _prefs.GetPreference<int>(Preference.FormSplitLocation);
         }

         if (!_prefs.GetPreference<bool>(Preference.FormLogVisible))
         {
            ShowHideLogWindow(false);
         }
         if (!_prefs.GetPreference<bool>(Preference.QueueViewerVisible))
         {
            ShowHideQueue(false);
         }

         //if (Prefs.FormSortColumn != String.Empty &&
         //    Prefs.FormSortOrder != SortOrder.None)
         //{
            _gridModel.SortColumnName = _prefs.GetPreference<string>(Preference.FormSortColumn);
            _gridModel.SortColumnOrder = _prefs.GetPreference<ListSortDirection>(Preference.FormSortOrder);
         //}

         try
         {
            // Restore the columns' state
            var cols = _prefs.GetPreference<StringCollection>(Preference.FormColumns);
            var colsArray = new string[cols.Count];

            cols.CopyTo(colsArray, 0);
            Array.Sort(colsArray);

            for (int i = 0; i < colsArray.Length; i++)
            {
               string[] a = colsArray[i].Split(',');
               int index = int.Parse(a[3]);
               _view.DataGridView.Columns[index].DisplayIndex = Int32.Parse(a[0]);
               if (_view.DataGridView.Columns[index].AutoSizeMode.Equals(DataGridViewAutoSizeColumnMode.Fill) == false)
               {
                  _view.DataGridView.Columns[index].Width = Int32.Parse(a[1]);
               }
               _view.DataGridView.Columns[index].Visible = bool.Parse(a[2]);
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
                                                  _prefs.Set(Preference.FormSplitLocation, _view.SplitContainer.SplitterDistance);
                                                  _prefs.Save();
                                               };

         if (_prefs.GetPreference<bool>(Preference.RunMinimized))
         {
            _view.WindowState = FormWindowState.Minimized;
         }

         if (_prefs.GetPreference<bool>(Preference.UseDefaultConfigFile))
         {
            var fileName = _prefs.GetPreference<string>(Preference.DefaultConfigFile);
            if (!String.IsNullOrEmpty(fileName))
            {
               LoadConfigFile(fileName);
            }
         }

         SetViewShowStyle();

         if (_prefs.GetPreference<bool>(Preference.StartupCheckForUpdate))
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
            _prefs.SetPreference(Preference.FormSplitLocation, _view.SplitContainer.Panel1.Height);
         }
      }

      public bool ViewClosing()
      {
         if (!CheckForConfigurationChanges())
         {
            return true;
         }

         SaveColumnSettings();
         SaveSortColumn();

         // Save location and size data
         // RestoreBounds remembers normal position if minimized or maximized
         if (_view.WindowState == FormWindowState.Normal)
         {
            _prefs.SetPreference(Preference.FormLocation, _view.Location);
            _prefs.SetPreference(Preference.FormSize, _view.Size);
         }
         else
         {
            _prefs.SetPreference(Preference.FormLocation, _view.RestoreBounds.Location);
            _prefs.SetPreference(Preference.FormSize, _view.RestoreBounds.Size);
         }

         _prefs.SetPreference(Preference.FormLogVisible, _view.LogFileViewer.Visible);
         _prefs.SetPreference(Preference.QueueViewerVisible, _view.QueueControl.Visible);

         CheckForAndFireUpdateProcess();

         return false;
      }
      
      public void SetViewShowStyle()
      {
         switch (_prefs.GetPreference<FormShowStyleType>(Preference.FormShowStyle))
         {
            case FormShowStyleType.SystemTray:
               _view.SetNotifyIconVisible(true);
               _view.ShowInTaskbar = (_view.WindowState != FormWindowState.Minimized);
               break;
            case FormShowStyleType.TaskBar:
               _view.SetNotifyIconVisible(false);
               _view.ShowInTaskbar = true;
               break;
            case FormShowStyleType.Both:
               _view.SetNotifyIconVisible(true);
               _view.ShowInTaskbar = true;
               break;
         }
      }

      private void CheckForAndFireUpdateProcess()
      {
         if (!String.IsNullOrEmpty(_updateLogic.UpdateFilePath))
         {
            _logger.Info("Firing update file '{0}'...", _updateLogic.UpdateFilePath);
            try
            {
               Process.Start(_updateLogic.UpdateFilePath);
            }
            catch (Exception ex)
            {
               _logger.ErrorFormat(ex, "{0}", ex.Message);
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
            _view.SetClientMenuItemsVisible(_gridModel.ClientFilesMenuItemVisible,
                                            _gridModel.CachedLogMenuItemVisible,
                                            _gridModel.ClientFilesMenuItemVisible ||
                                            _gridModel.CachedLogMenuItemVisible);

            _view.StatusLabelLeftText = _gridModel.SelectedSlot.ClientPathAndArguments;

            _view.QueueControl.SetQueue(_gridModel.SelectedSlot.Queue,
                                        _gridModel.SelectedSlot.UnitInfo.SlotType,
                                        _gridModel.SelectedSlot.Settings.UtcOffsetIsZero);

            // if we've got a good queue read, let queueControl_QueueIndexChanged()
            // handle populating the log lines.
            if (_gridModel.SelectedSlot.Queue != null) return;

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
            //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Changed Queue Index ({0} - {1})", Name, e.Index));

            // Check the UnitLogLines array against the requested Queue Index - Issue 171
            try
            {
               var logLines = _gridModel.SelectedSlot.GetLogLinesForQueueIndex(index);
               if (logLines == null && index == _gridModel.SelectedSlot.Queue.CurrentIndex)
               {
                  logLines = _gridModel.SelectedSlot.CurrentLogLines;
               }

               SetLogLines(_gridModel.SelectedSlot, logLines);
            }
            catch (ArgumentOutOfRangeException ex)
            {
               _logger.ErrorFormat(ex, "{0}", ex.Message);
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
         _view.QueueControl.SetQueue(null);
      }

      private void SetLogLines(SlotModel instance, IList<LogLine> logLines)
      {
         /*** Checked LogLine Count ***/
         if (logLines != null && logLines.Count > 0)
         {
            // Different Client... Load LogLines
            if (_view.LogFileViewer.LogOwnedByInstanceName.Equals(instance.Name) == false)
            {
               _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.GetPreference<bool>(Preference.ColorLogFile));

               //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Set Log Lines (Changed Client - {0})", instance.Name));
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
                  _logger.WarnFormat(ex, Constants.InstanceNameFormat, instance.Name, ex.Message);
               }

               // If the last text line in the textbox DOES NOT equal the last LogLine Text... Load LogLines.
               // Otherwise, the log has not changed, don't update and perform the log "flicker".
               if (_view.LogFileViewer.Lines[_view.LogFileViewer.Lines.Length - 1].Equals(lastLogLine) == false)
               {
                  _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.GetPreference<bool>(Preference.ColorLogFile));

                  //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Set Log Lines (log lines different)");
               }
            }
            // Nothing in the Textbox... Load LogLines
            else
            {
               _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.GetPreference<bool>(Preference.ColorLogFile));
            }
         }
         else
         {
            _view.LogFileViewer.SetNoLogLines();
         }

         _view.LogFileViewer.ScrollToBottom();
      }

      private void DataGridViewColumnDisplayIndexChanged()
      {
         if (_view.DataGridView.Columns.Count == frmMain.NumberOfDisplayFields)
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
         var stringCollection = new StringCollection();
         int i = 0;

         foreach (DataGridViewColumn column in _view.DataGridView.Columns)
         {
            stringCollection.Add(String.Format(CultureInfo.InvariantCulture,
                                    "{0},{1},{2},{3}",
                                    column.DisplayIndex.ToString("D2"),
                                    column.Width,
                                    column.Visible,
                                    i++));
         }

         _prefs.SetPreference(Preference.FormColumns, stringCollection);
      }

      private void SaveSortColumn()
      {
         _prefs.Set(Preference.FormSortColumn, _gridModel.SortColumnName);
         _prefs.Set(Preference.FormSortOrder, _gridModel.SortColumnOrder);
      }

      public void DataGridViewSorted()
      {
         //SaveSortColumn(); // Save Column Sort Order - Issue 73
         //_prefs.Save();

         //SelectCurrentRowKey();
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

               _view.SetGridContextMenuItemsVisible(_gridModel.ClientFilesMenuItemVisible,
                                                    _gridModel.CachedLogMenuItemVisible,
                                                    _gridModel.ClientFilesMenuItemVisible ||
                                                    _gridModel.CachedLogMenuItemVisible);

               _view.ShowGridContextMenuStrip(_view.DataGridView.PointToScreen(new Point(coordX, coordY)));
            }
         }
         if (button == MouseButtons.Left && clicks == 2)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               // Check for SelectedSlot, and get out if not found
               if (_gridModel.SelectedSlot == null) return;

               if (_gridModel.SelectedSlot.Settings.LegacyClientSubType.Equals(LegacyClientSubType.Path))
               {
                  HandleProcessStartResult(_processStarter.ShowFileExplorer(_gridModel.SelectedSlot.Settings.Path));
               }
            }
         }
      }
      
      #endregion
      
      #region File Handling Methods

      public void FileNewClick()
      {
         if (_retrievalLogic.RetrievalInProgress)
         {
            _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to create a new config file.", _view.Text);
            return;
         }

         if (CheckForConfigurationChanges())
         {
            // clear the clients and UI
            Clear();
         }
      }

      public void FileOpenClick()
      {
         //if (_retrievalLogic.RetrievalInProgress)
         //{
         //   _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to open another config file.", _view.Text);
         //   return;
         //}

         //if (CheckForConfigurationChanges())
         //{
         //   _openFileDialogView.DefaultExt = _configurationManager.ConfigFileExtension;
         //   _openFileDialogView.Filter = _configurationManager.FileTypeFilters;
         //   _openFileDialogView.FileName = _configurationManager.ConfigFilename;
         //   _openFileDialogView.RestoreDirectory = true;
         //   if (_openFileDialogView.ShowDialog() == DialogResult.OK)
         //   {
         //      // clear the clients and UI
         //      Clear();
         //      // 
         //      LoadConfigFile(_openFileDialogView.FileName, _openFileDialogView.FilterIndex);
         //   }
         //}
      }

      private void LoadConfigFile(string filePath, int filterIndex = 1)
      {
         //Debug.Assert(filePath != null);

         //try
         //{
         //   // Read the config file
         //   ReadConfigFile(filePath, filterIndex);

         //   if (!_clientDictionary.HasInstances())
         //   {
         //      _messageBoxView.ShowError(_view, "No client configurations were loaded from the given config file.", _view.Text);
         //   }
         //}
         //catch (Exception ex)
         //{
         //   HfmTrace.WriteToHfmConsole(ex);
         //   _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
         //      "No client configurations were loaded from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message),
         //      _view.Text);
         //}
      }

      /// <summary>
      /// Reads a collection of Client Instance Settings from file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      private void ReadConfigFile(string filePath, int filterIndex)
      {
         //Debug.Assert(filePath != null);

         //if (filterIndex > _configurationManager.SettingsPluginsCount)
         //{
         //   throw new ArgumentOutOfRangeException("filterIndex", String.Format(CultureInfo.CurrentCulture,
         //      "Argument 'filterIndex' must be between 1 and {0}.", _configurationManager.SettingsPluginsCount));
         //}

         //ICollection<ClientInstance> instances = _configurationManager.ReadConfigFile(filePath, filterIndex);
         //_clientDictionary.LoadInstances(instances);

         //if (_clientDictionary.HasInstances())
         //{
         //   RefreshDisplay();
         //   // Get client logs         
         //   _retrievalLogic.QueueNewRetrieval();
         //   // Start Retrieval and Web Generation Timers
         //   _retrievalLogic.SetTimerState();
         //}
      }

      private void AutoSaveConfig()
      {
         if (_prefs.GetPreference<bool>(Preference.AutoSaveConfig))
         {
            FileSaveClick();
         }
      }

      public void FileSaveClick()
      {
         //if (!_configurationManager.HasConfigFilename)
         //{
         //   FileSaveAsClick();
         //}
         //else
         //{
         //   try
         //   {
         //      WriteConfigFile();
         //   }
         //   catch (Exception ex)
         //   {
         //      HfmTrace.WriteToHfmConsole(ex);
         //      _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
         //         "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
         //         _view.Text);
         //   }
         //}
      }

      private void WriteConfigFile()
      {
         //WriteConfigFile(_configurationManager.ConfigFilename, _configurationManager.SettingsPluginIndex);
      }

      public void FileSaveAsClick()
      {
         //// No Config File and no Instances, stub out
         //if (!_configurationManager.HasConfigFilename && !_clientDictionary.HasInstances()) return;

         //_saveFileDialogView.DefaultExt = _configurationManager.ConfigFileExtension;
         //_saveFileDialogView.Filter = _configurationManager.FileTypeFilters;
         //if (_saveFileDialogView.ShowDialog() == DialogResult.OK)
         //{
         //   try
         //   {
         //      WriteConfigFile(_saveFileDialogView.FileName, _saveFileDialogView.FilterIndex); // Issue 75
         //   }
         //   catch (Exception ex)
         //   {
         //      HfmTrace.WriteToHfmConsole(ex);
         //      _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
         //         "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
         //         _view.Text);
         //   }
         //}
      }

      /// <summary>
      /// Saves the current collection of Client Instances to file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      private void WriteConfigFile(string filePath, int filterIndex)
      {
         //if (String.IsNullOrEmpty(filePath))
         //{
         //   throw new ArgumentException("Argument 'filePath' cannot be a null or empty string.", "filePath");
         //}

         //if (filterIndex > _configurationManager.SettingsPluginsCount)
         //{
         //   throw new ArgumentOutOfRangeException("filterIndex", String.Format(CultureInfo.CurrentCulture,
         //      "Argument 'filterIndex' must be between 1 and {0}.", _configurationManager.SettingsPluginsCount));
         //}

         //_configurationManager.WriteConfigFile(GetCurrentInstanceArray(), filePath, filterIndex);

         //ChangedAfterSave = false;
      }

      private bool CheckForConfigurationChanges()
      {
         if (ChangedAfterSave)
         {
            DialogResult result = _messageBoxView.AskYesNoCancelQuestion(_view,
               String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes / No - Continue and do not save the changes / Cancel - Do not continue", Environment.NewLine),
               _view.Text);

            switch (result)
            {
               case DialogResult.Yes:
                  FileSaveClick();
                  return !(ChangedAfterSave);
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

      #region Help Menu Handling Methods

      public void ShowHfmLogFile()
      {
         HandleProcessStartResult(_processStarter.ShowHfmLogFile());
      }

      public void ShowHfmDataFiles()
      {
         HandleProcessStartResult(_processStarter.ShowFileExplorer(_prefs.ApplicationDataFolderPath));
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

      public void ClientsAddClick()
      {
         //var settings = new ClientInstanceSettings();
         //var newHost = InstanceProvider.GetInstance<ILegacyClientSetupPresenter>();
         //newHost.SettingsModel = new LegacyClientSettingsModel(settings);
         //while (newHost.ShowDialog(_view).Equals(DialogResult.OK))
         //{
         //   try
         //   {
         //      Add(newHost.SettingsModel.Settings);
         //      break;
         //   }
         //   catch (InvalidOperationException ex)
         //   {
         //      HfmTrace.WriteToHfmConsole(ex);
         //      _messageBoxView.ShowError(_view, ex.Message, _view.Text);
         //   }
         //}
      }

      public void ClientsEditClick()
      {
         //// Check for SelectedClientInstance, and get out if not found
         //if (SelectedClientInstance == null) return;

         //var settings = SelectedClientInstance.Settings.DeepClone();
         //string previousName = settings.InstanceName;
         //string previousPath = settings.Path;
         //var editHost = InstanceProvider.GetInstance<ILegacyClientSetupPresenter>();
         //editHost.SettingsModel = new LegacyClientSettingsModel(settings);
         //while (editHost.ShowDialog(_view).Equals(DialogResult.OK))
         //{
         //   try
         //   {
         //      Edit(previousName, previousPath, editHost.SettingsModel.Settings);
         //      break;
         //   }
         //   catch (InvalidOperationException ex)
         //   {
         //      HfmTrace.WriteToHfmConsole(ex);
         //      _messageBoxView.ShowError(_view, ex.Message, _view.Text);
         //   }
         //}
      }

      public void ClientsDeleteClick()
      {
         //// Check for SelectedClientInstance, and get out if not found
         //if (SelectedClientInstance == null) return;

         //Remove(SelectedClientInstance.Settings.InstanceName);
      }

      public void ClientsMergeClick()
      {
         //var settings = new ClientInstanceSettings { ExternalInstance = true };
         //var newHost = InstanceProvider.GetInstance<ILegacyClientSetupPresenter>();
         //newHost.SettingsModel = new LegacyClientSettingsModel(settings);
         //while (newHost.ShowDialog(_view).Equals(DialogResult.OK))
         //{
         //   try
         //   {
         //      Add(newHost.SettingsModel.Settings);
         //      break;
         //   }
         //   catch (InvalidOperationException ex)
         //   {
         //      HfmTrace.WriteToHfmConsole(ex);
         //      _messageBoxView.ShowError(_view, ex.Message, _view.Text);
         //   }
         //}
      }

      public void ClientsRefreshSelectedClick()
      {
         //// Check for SelectedClientInstance, and get out if not found
         //if (SelectedClientInstance == null) return;

         //_retrievalLogic.RetrieveSingleClient(SelectedClientInstance.Settings.InstanceName);
      }
      
      public void ClientsRefreshAllClick()
      {
         _retrievalLogic.QueueNewRetrieval();
      }

      public void ClientsViewCachedLogClick()
      {
         //// Check for SelectedClientInstance, and get out if not found
         //if (SelectedClientInstance == null) return;

         //string logFilePath = Path.Combine(_prefs.CacheDirectory, SelectedClientInstance.Settings.CachedFahLogName);
         //if (File.Exists(logFilePath))
         //{
         //   HandleProcessStartResult(_processStarter.ShowCachedLogFile(logFilePath));
         //}
         //else
         //{
         //   string message = String.Format(CultureInfo.CurrentCulture, "The FAHlog.txt file for '{0}' does not exist.",
         //                                  SelectedClientInstance.Settings.InstanceName);
         //   _messageBoxView.ShowInformation(_view, message, _view.Text);
         //}
      }

      public void ClientsViewClientFilesClick()
      {
         //// Check for SelectedClientInstance, and get out if not found
         //if (SelectedClientInstance == null) return;

         //if (SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance))
         //{
         //   HandleProcessStartResult(_processStarter.ShowFileExplorer(SelectedClientInstance.Settings.Path));
         //}
      }
      
      #endregion

      ///// <summary>
      ///// Add an Instance
      ///// </summary>
      ///// <param name="settings">Client Instance Settings</param>
      //private void Add(ClientInstanceSettings settings)
      //{
      //   Debug.Assert(settings != null);

      //   ClientInstance instance = _instanceFactory.Create(settings);
      //   if (_clientDictionary.ContainsKey(instance.Settings.InstanceName))
      //   {
      //      throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
      //         "Client Name '{0}' already exists.", instance.Settings.InstanceName));
      //   }

      //   // Issue 230
      //   bool hasInstances = _clientDictionary.HasInstances();

      //   // lock added here - 9/28/10
      //   lock (_clientDictionary)
      //   {
      //      _clientDictionary.Add(instance.Settings.InstanceName, instance);
      //   }
      //   RefreshDisplay();

      //   _retrievalLogic.RetrieveSingleClient(instance);

      //   ChangedAfterSave = true;
      //   AutoSaveConfig();

      //   // Issue 230
      //   if (!hasInstances)
      //   {
      //      _retrievalLogic.SetTimerState();
      //   }
      //}

      ///// <summary>
      ///// Edit the ClientInstance Name and Path
      ///// </summary>
      ///// <param name="previousName"></param>
      ///// <param name="previousPath"></param>
      ///// <param name="settings">Client Instance Settings</param>
      //private void Edit(string previousName, string previousPath, ClientInstanceSettings settings)
      //{
      //   Debug.Assert(previousName != null);
      //   Debug.Assert(previousPath != null);
      //   Debug.Assert(settings != null);

      //   Debug.Assert(_clientDictionary.ContainsKey(previousName));

      //   // if the host key changed
      //   if (previousName != settings.InstanceName)
      //   {
      //      // check for a duplicate name
      //      if (_clientDictionary.ContainsKey(settings.InstanceName))
      //      {
      //         throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
      //            "Client Name '{0}' already exists.", settings.InstanceName));
      //      }
      //   }

      //   var instance = _clientDictionary[previousName];
      //   // load the new settings
      //   instance.Settings = settings;

      //   // Instance Name changed but isn't an already existing key
      //   if (previousName != instance.Settings.InstanceName)
      //   {
      //      // lock added here - 9/28/10
      //      lock (_clientDictionary)
      //      {
      //         // update InstanceCollection
      //         _clientDictionary.Remove(previousName);
      //         _clientDictionary.Add(instance.Settings.InstanceName, instance);
      //      }

      //      // Issue 79 - 9/28/10
      //      if (!instance.Settings.ExternalInstance)
      //      {
      //         // update the Names in the BenchmarkContainer
      //         _benchmarkContainer.UpdateInstanceName(new BenchmarkClient(previousName, instance.Settings.Path), instance.Settings.InstanceName);
      //      }
      //   }
      //   // the path changed
      //   if (!Paths.Equal(previousPath, instance.Settings.Path))
      //   {
      //      // Issue 79 - 9/28/10
      //      if (!instance.Settings.ExternalInstance)
      //      {
      //         // update the Paths in the BenchmarkContainer
      //         _benchmarkContainer.UpdateInstancePath(new BenchmarkClient(instance.Settings.InstanceName, previousPath), instance.Settings.Path);
      //      }
      //   }

      //   _retrievalLogic.RetrieveSingleClient(instance);

      //   ChangedAfterSave = true;
      //   AutoSaveConfig();
      //}

      ///// <summary>
      ///// Remove an Instance by Name
      ///// </summary>
      ///// <param name="instanceName">Instance Name</param>
      //private void Remove(string instanceName)
      //{
      //   Debug.Assert(instanceName != null);

      //   // lock added here - 9/28/10
      //   lock (_clientDictionary)
      //   {
      //      _clientDictionary.Remove(instanceName);
      //      var findInstance = FindDisplayInstance(instanceName);
      //      if (findInstance != null)
      //      {
      //         _displayCollection.Remove(findInstance);
      //      }
      //   }
      //   RefreshDisplay();

      //   ChangedAfterSave = true;
      //   AutoSaveConfig();

      //   FindDuplicates();
      //}

      /// <summary>
      /// Clear All Instance Data
      /// </summary>
      private void Clear()
      {
         //// new config filename
         //_configurationManager.ClearConfigFilename();
         //// collection has not changed
         //ChangedAfterSave = false;

         //if (_clientDictionary.HasInstances())
         //{
         //   SaveCurrentUnitInfo();
         //}

         //_clientDictionary.Clear();
         //_displayCollection.Clear();

         //// This will disable the timers, we have no hosts
         //_retrievalLogic.SetTimerState();

         //RefreshDisplay();
      }

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
            var location = _prefs.GetPreference<Point>(Preference.MessagesFormLocation);
            var size = _prefs.GetPreference<Size>(Preference.MessagesFormSize);

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
            _prefs.SetPreference(Preference.FormLogWindowHeight, (_view.SplitContainer.Height - _view.SplitContainer.SplitterDistance));
            _view.Size = new Size(_view.Size.Width, _view.Size.Height - _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
         }
         else
         {
            _view.LogFileViewer.Visible = true;
            _view.DisableViewResizeEvent();  // disable Form resize event for this operation
            _view.Size = new Size(_view.Size.Width, _view.Size.Height + _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            _view.EnableViewResizeEvent();   // re-enable
            _view.SplitContainer.Panel2Collapsed = false;
         }
      }
      
      public void ShowHideQueue()
      {
         ShowHideQueue(!_view.QueueControl.Visible);
      }

      private void ShowHideQueue(bool show)
      {
         if (!show)
         {
            _view.QueueControl.Visible = false;
            _view.SetQueueButtonText(String.Format(CultureInfo.CurrentCulture, "S{0}h{0}o{0}w{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine));
            _view.SplitContainer2.SplitterDistance = 27;
         }
         else
         {
            _view.QueueControl.Visible = true;
            _view.SetQueueButtonText(String.Format(CultureInfo.CurrentCulture, "H{0}i{0}d{0}e{0}{0}Q{0}u{0}e{0}u{0}e", Environment.NewLine));
            _view.SplitContainer2.SplitterDistance = 289;
         }
      }

      public void ViewToggleDateTimeClick()
      {
         var style = _prefs.GetPreference<TimeStyleType>(Preference.TimeStyle);
         _prefs.SetPreference(Preference.TimeStyle, style.Equals(TimeStyleType.Standard) 
                                 ? TimeStyleType.Formatted 
                                 : TimeStyleType.Standard);

         _view.DataGridView.Invalidate();
      }

      public void ViewToggleCompletedCountStyleClick()
      {
         var style = _prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay);
         _prefs.SetPreference(Preference.CompletedCountDisplay, style.Equals(CompletedCountDisplayType.ClientTotal)
                                 ? CompletedCountDisplayType.ClientRunTotal
                                 : CompletedCountDisplayType.ClientTotal);

         _view.DataGridView.Invalidate();
      }

      public void ViewToggleVersionInformationClick()
      {
         _prefs.SetPreference(Preference.ShowVersions, !_prefs.GetPreference<bool>(Preference.ShowVersions));
         _view.DataGridView.Invalidate();
      }

      public void ViewToggleBonusCalculationClick()
      {
         bool value = !_prefs.GetPreference<bool>(Preference.CalculateBonus);
         _prefs.SetPreference(Preference.CalculateBonus, value);
         _view.ShowNotifyToolTip(value ? "Bonus On" : "Bonus Off");
         _view.DataGridView.Invalidate();
      }

      public void ViewCycleCalculationClick()
      {
         var calculationType = _prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation);
         int typeIndex = 0;
         // EffectiveRate is always LAST entry
         if (calculationType.Equals(PpdCalculationType.EffectiveRate) == false)
         {
            typeIndex = (int)calculationType;
            typeIndex++;
         }

         calculationType = (PpdCalculationType)typeIndex;
         _prefs.SetPreference(Preference.PpdCalculation, calculationType);
         string calculationTypeString = (from item in OptionsModel.PpdCalculationList
                                         where ((PpdCalculationType)item.ValueMember) == calculationType
                                         select item.DisplayMember).First();
         _view.ShowNotifyToolTip(calculationTypeString);
         _view.DataGridView.Invalidate();
      }

      #endregion

      #region Tools Menu Handling Methods

      public void ToolsDownloadProjectsClick()
      {
         //// Clear the Project Not Found Cache and Last Download Time
         //_proteinCollection.ClearProjectsNotFoundCache();
         //_proteinCollection.Downloader.ResetLastDownloadTime();
         //// Execute Asynchronous Download
         //var projectDownloadView = InstanceProvider.GetInstance<IProgressDialogView>("projectDownloadView");
         //projectDownloadView.OwnerWindow = _view;
         //projectDownloadView.ProcessRunner = _proteinCollection.Downloader;
         //projectDownloadView.UpdateMessage(_proteinCollection.Downloader.Prefs.GetPreference<string>(Preference.ProjectDownloadUrl));
         //projectDownloadView.Process();
      }

      public void ToolsBenchmarksClick()
      {
         int projectId = 0;

         // Check for SelectedSlot, and if found... load its ProjectID.
         if (_gridModel.SelectedSlot != null)
         {
            projectId = _gridModel.SelectedSlot.UnitInfo.ProjectID;
         }

         var frm = ServiceLocator.Resolve<IBenchmarksView>();
         frm.SetManualStartPosition();
         frm.LoadProjectID = projectId;

         // Restore state data
         var location = _prefs.GetPreference<Point>(Preference.BenchmarksFormLocation);
         var size = _prefs.GetPreference<Size>(Preference.BenchmarksFormSize);

         if (location.X != 0 && location.Y != 0)
         {
            frm.Location = location;
         }
         else
         {
            frm.Location = new Point(_view.Location.X + 50, _view.Location.Y + 50);
         }

         if (size.Width != 0 && size.Height != 0)
         {
            frm.Size = size;
         }

         frm.Show();
      }

      public void ToolsHistoryClick()
      {
         Debug.Assert(_view.WorkUnitHistoryMenuEnabled);
      
         if (_historyPresenter == null)
         {
            _historyPresenter = ServiceLocator.Resolve<HistoryPresenter>();
            _historyPresenter.Initialize();
            _historyPresenter.PresenterClosed += delegate { _historyPresenter = null; };
         }

         _historyPresenter.Show();
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

      public void ShowHfmGoogleCode()
      {
         HandleProcessStartResult(_processStarter.ShowHfmGoogleCode());
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
         _view.LogFileViewer.HighlightLines(_prefs.GetPreference<bool>(Preference.ColorLogFile));
      }

      private void HandleProcessStartResult(string message)
      {
         if (message != null)
         {
            _messageBoxView.ShowError(_view, message, _view.Text);
         }
      }
      
      #endregion

      /// <summary>
      /// Finds the SlotModel by key (Name).
      /// </summary>
      public SlotModel FindDisplayInstance(string key)
      {
         return _clientDictionary.Slots.FirstOrDefault(slot => slot.Name == key);
      }

      //private void SaveCurrentUnitInfo()
      //{
      //   // If no clients loaded, stub out
      //   if (!_clientDictionary.HasInstances()) return;

      //   _unitInfoContainer.Clear();

      //   lock (_clientDictionary)
      //   {
      //      foreach (ClientInstance instance in _clientDictionary.Values)
      //      {
      //         foreach (var displayInstance in instance.DisplayInstances.Values)
      //         {
      //            // Don't save the UnitInfo object if the contained Project is Unknown
      //            if (displayInstance.CurrentUnitInfo.UnitInfoData.ProjectIsUnknown() == false)
      //            {
      //               _unitInfoContainer.Add((UnitInfo)displayInstance.CurrentUnitInfo.UnitInfoData);
      //            }
      //         }
      //      }
      //   }

      //   _unitInfoContainer.Write();
      //}

      public void FindDuplicates()
      {
         // check for clients with duplicate Project (Run, Clone, Gen) or UserID
         _clientDictionary.Slots.FindDuplicates();
         _view.DataGridView.Invalidate();
      }
   }
}
