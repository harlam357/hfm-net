/*
 * HFM.NET - Main View Presenter
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
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Castle.Core.Logging;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public sealed class MainPresenter
   {
      #region Properties

      /// <summary>
      /// Command Line Arguments.
      /// </summary>
      public IEnumerable<Argument> Arguments { get; set; }

      /// <summary>
      /// Holds the state of the window before it is hidden (minimize to tray behaviour)
      /// </summary>
      public FormWindowState OriginalWindowState { get; private set; }

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
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
      private readonly IProteinBenchmarkCollection _benchmarkCollection;
      private readonly IProteinDictionary _proteinDictionary;
      private readonly IUnitInfoCollection _unitInfoCollection;

      #endregion

      #region Logic Services

      private readonly IUpdateLogic _updateLogic;
      private readonly RetrievalLogic _retrievalLogic;
      private readonly IExternalProcessStarter _processStarter;

      #endregion

      #region Data Services

      private readonly IPreferenceSet _prefs;
      private readonly IClientSettingsManager _settingsManager;
      
      #endregion

      #endregion

      #region Constructor

      public MainPresenter(MainGridModel mainGridModel, IMainView view, IMessagesView messagesView, IMessageBoxView messageBoxView,
                           IOpenFileDialogView openFileDialogView, ISaveFileDialogView saveFileDialogView,
                           IClientDictionary clientDictionary, IProteinBenchmarkCollection benchmarkCollection,
                           IProteinDictionary proteinDictionary, IUnitInfoCollection unitInfoCollection, IUpdateLogic updateLogic, 
                           RetrievalLogic retrievalLogic, IExternalProcessStarter processStarter, 
                           IPreferenceSet prefs, IClientSettingsManager settingsManager)
      {
         _gridModel = mainGridModel;
         //_gridModel.BeforeResetBindings += delegate { _view.DataGridView.FreezeSelectionChanged = true; };
         _gridModel.AfterResetBindings += delegate
                                          {
                                             //_view.DataGridView.FreezeSelectionChanged = false;
                                             DisplaySelectedSlotData();
                                             _view.RefreshControlsWithTotalsData(_gridModel.SlotTotals);
                                          };
         _gridModel.SelectedSlotChanged += (sender, e) =>
                                           {
                                              if (e.Index >=0 && e.Index < _view.DataGridView.Rows.Count)
                                              {
                                                 _view.DataGridView.Rows[e.Index].Selected = true;
                                                 DisplaySelectedSlotData();
                                              }
                                           };

         // Views
         _view = view;
         _messagesView = messagesView;
         _messageBoxView = messageBoxView;
         _openFileDialogView = openFileDialogView;
         _saveFileDialogView = saveFileDialogView;
         // Collections
         _clientDictionary = clientDictionary;
         _benchmarkCollection = benchmarkCollection;
         _proteinDictionary = proteinDictionary;
         _unitInfoCollection = unitInfoCollection;
         // Logic Services
         _updateLogic = updateLogic;
         _updateLogic.Owner = _view;
         _retrievalLogic = retrievalLogic;
         _retrievalLogic.Initialize();
         _processStarter = processStarter;
         // Data Services
         _prefs = prefs;
         _settingsManager = settingsManager;

         _clientDictionary.ClientEdited += HandleClientEdit;
         _clientDictionary.DictionaryChanged += delegate { AutoSaveConfig(); };
      }
      
      #endregion

      #region Initialize

      public void Initialize()
      {
         // Restore View Preferences (must be done AFTER DataGridView columns are setup)
         RestoreViewPreferences();
         // 
         _view.SetGridDataSource(_gridModel.BindingSource);
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
         var location = _prefs.Get<Point>(Preference.FormLocation);
         if (location.X != 0 && location.Y != 0)
         {
            _view.SetManualStartPosition();
            _view.Location = location;
         }
         // Look for view size
         var size = _prefs.Get<Size>(Preference.FormSize);
         if (size.Width != 0 && size.Height != 0)
         {
            if (!_prefs.Get<bool>(Preference.FormLogVisible))
            {
               size = new Size(size.Width, size.Height + _prefs.Get<int>(Preference.FormLogWindowHeight));
            }
            _view.Size = size;
            _view.SplitContainer.SplitterDistance = _prefs.Get<int>(Preference.FormSplitLocation);
         }

         if (!_prefs.Get<bool>(Preference.FormLogVisible))
         {
            ShowHideLogWindow(false);
         }
         if (!_prefs.Get<bool>(Preference.QueueViewerVisible))
         {
            ShowHideQueue(false);
         }
         _view.FollowLogFileChecked = _prefs.Get<bool>(Preference.FollowLogFile);

         //if (Prefs.FormSortColumn != String.Empty &&
         //    Prefs.FormSortOrder != SortOrder.None)
         //{
            _gridModel.SortColumnName = _prefs.Get<string>(Preference.FormSortColumn);
            _gridModel.SortColumnOrder = _prefs.Get<ListSortDirection>(Preference.FormSortOrder);
         //}

         try
         {
            // Restore the columns' state
            var cols = _prefs.Get<StringCollection>(Preference.FormColumns);
            var colsArray = new string[cols.Count];

            cols.CopyTo(colsArray, 0);
            Array.Sort(colsArray);

            for (int i = 0; i < colsArray.Length && i < MainForm.NumberOfDisplayFields; i++)
            {
               string[] a = colsArray[i].Split(',');
               int index = Int32.Parse(a[3]);
               _view.DataGridView.Columns[index].DisplayIndex = Int32.Parse(a[0]);
               if (_view.DataGridView.Columns[index].AutoSizeMode.Equals(DataGridViewAutoSizeColumnMode.Fill) == false)
               {
                  _view.DataGridView.Columns[index].Width = Int32.Parse(a[1]);
               }
               _view.DataGridView.Columns[index].Visible = Boolean.Parse(a[2]);
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

         if (_prefs.Get<bool>(Preference.RunMinimized))
         {
            _view.WindowState = FormWindowState.Minimized;
         }

         Debug.Assert(Arguments != null);
         var openFile = Arguments.FirstOrDefault(x => x.Type.Equals(ArgumentType.OpenFile));
         if (openFile != null)
         {
            if (!String.IsNullOrEmpty(openFile.Data))
            {
               LoadConfigFile(openFile.Data);
            }
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
            _prefs.Set(Preference.FormSplitLocation, _view.SplitContainer.Panel1.Height);
         }
      }

      public bool ViewClosing()
      {
         if (!CheckForConfigurationChanges())
         {
            return true;
         }

         SaveColumnSettings();
         SaveCurrentUnitInfo();

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

         _prefs.Set(Preference.FormLogVisible, _view.LogFileViewer.Visible);
         _prefs.Set(Preference.QueueViewerVisible, _view.QueueControlVisible);

         CheckForAndFireUpdateProcess();

         return false;
      }
      
      public void SetViewShowStyle()
      {
         switch (_prefs.Get<FormShowStyleType>(Preference.FormShowStyle))
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

            _view.SetQueue(_gridModel.SelectedSlot.Queue,
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
               // show the current log even if not the current unit index - 2/17/12
               if (logLines == null) // && index == _gridModel.SelectedSlot.Queue.CurrentIndex)
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
         _view.SetQueue(null);
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
                  _logger.WarnFormat(ex, Constants.ClientNameFormat, instance.Name, ex.Message);
               }

               // If the last text line in the textbox DOES NOT equal the last LogLine Text... Load LogLines.
               // Otherwise, the log has not changed, don't update and perform the log "flicker".
               if (_view.LogFileViewer.Lines[_view.LogFileViewer.Lines.Length - 1].Equals(lastLogLine) == false)
               {
                  _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.Get<bool>(Preference.ColorLogFile));

                  //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Set Log Lines (log lines different)");
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

         if (_prefs.Get<bool>(Preference.FollowLogFile))
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

         _prefs.Set(Preference.FormColumns, stringCollection);
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
         //if (_retrievalLogic.RetrievalInProgress)
         //{
         //   _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to create a new config file.", _view.Text);
         //   return;
         //}

         if (CheckForConfigurationChanges())
         {
            ClearConfiguration();
         }
      }

      public void FileOpenClick()
      {
         //if (_retrievalLogic.RetrievalInProgress)
         //{
         //   _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to open another config file.", _view.Text);
         //   return;
         //}

         if (CheckForConfigurationChanges())
         {
            _openFileDialogView.DefaultExt = _settingsManager.FileExtension;
            _openFileDialogView.Filter = _settingsManager.FileTypeFilters;
            _openFileDialogView.FileName = _settingsManager.FileName;
            _openFileDialogView.RestoreDirectory = true;
            if (_openFileDialogView.ShowDialog().Equals(DialogResult.OK))
            {
               ClearConfiguration();
               // 
               LoadConfigFile(_openFileDialogView.FileName, _openFileDialogView.FilterIndex);
            }
         }
      }

      private void ClearConfiguration()
      {
         // abort current retrieval
         if (_retrievalLogic.RetrievalInProgress)
         {
            _retrievalLogic.Abort();
         }

         // clear the clients and UI
         _settingsManager.ClearFileName();
         // 
         if (_clientDictionary.Count != 0)
         {
            SaveCurrentUnitInfo();
         }
         _clientDictionary.Clear();
      }

      private void SaveCurrentUnitInfo()
      {
         // If no clients loaded, stub out
         if (_clientDictionary.Count == 0) return;

         _unitInfoCollection.Clear();

         foreach (var slotModel in _clientDictionary.Slots)
         {
            // Don't save the UnitInfo object if the contained Project is Unknown
            if (!slotModel.UnitInfo.ProjectIsUnknown())
            {
               _unitInfoCollection.Add(slotModel.UnitInfo);
            }
         }

         _unitInfoCollection.Write();
      }

      private void LoadConfigFile(string filePath, int filterIndex = 1)
      {
         Debug.Assert(filePath != null);

         try
         {
            // Read the config file
            _clientDictionary.Load(_settingsManager.Read(filePath, filterIndex));

            if (_clientDictionary.Count == 0)
            {
               _messageBoxView.ShowError(_view, "No client configurations were loaded from the given config file.", _view.Text);
            }
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
            _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
               "No client configurations were loaded from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message), _view.Text);
         }
      }

      private void AutoSaveConfig()
      {
         if (_prefs.Get<bool>(Preference.AutoSaveConfig) &&
             _clientDictionary.IsDirty)
         {
            FileSaveClick();
         }
      }

      public void FileSaveClick()
      {
         // no clients, stub out
         if (_clientDictionary.Count == 0) return;

         // index 2 is hard coded to legacy serializer
         if (_settingsManager.FilterIndex == 2)
         {
            _settingsManager.ClearFileName();
         }

         if (_settingsManager.FileName.Length == 0)
         {
            FileSaveAsClick();
         }
         else
         {
            try
            {
               _settingsManager.Write(_clientDictionary.Values.Select(x => x.Settings), _settingsManager.FileName, 
                                      _settingsManager.FilterIndex == 2 ? 1 : _settingsManager.FilterIndex);
               _clientDictionary.IsDirty = false;
            }
            catch (Exception ex)
            {
               _logger.ErrorFormat(ex, "{0}", ex.Message);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message), _view.Text);
            }
         }
      }

      public void FileSaveAsClick()
      {
         // no clients, stub out
         if (_clientDictionary.Count == 0) return;

         _saveFileDialogView.DefaultExt = _settingsManager.FileExtension;
         _saveFileDialogView.Filter = _settingsManager.FileTypeFilters;
         if (_saveFileDialogView.ShowDialog().Equals(DialogResult.OK))
         {
            try
            {
               // Issue 75
               _settingsManager.Write(_clientDictionary.Values.Select(x => x.Settings), _saveFileDialogView.FileName, _saveFileDialogView.FilterIndex);
               _clientDictionary.IsDirty = false;
            }
            catch (Exception ex)
            {
               _logger.ErrorFormat(ex, "{0}", ex.Message);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message), _view.Text);
            }
         }
      }

      private bool CheckForConfigurationChanges()
      {
         if (_clientDictionary.Count != 0 && _clientDictionary.IsDirty)
         {
            DialogResult result = _messageBoxView.AskYesNoCancelQuestion(_view,
               String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes / No - Continue and do not save the changes / Cancel - Do not continue", Environment.NewLine),
               _view.Text);

            switch (result)
            {
               case DialogResult.Yes:
                  FileSaveClick();
                  return !(_clientDictionary.IsDirty);
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

      internal void ClientsAddClick()
      {
         using (var dialog = ServiceLocator.Resolve<IFahClientSetupPresenter>())
         {
            dialog.SettingsModel = new FahClientSettingsModel(_view);
            while (dialog.ShowDialog(_view).Equals(DialogResult.OK))
            {
               var settings = AutoMapper.Mapper.Map<FahClientSettingsModel, ClientSettings>(dialog.SettingsModel);
               //if (_clientDictionary.ContainsKey(settings.Name))
               //{
               //   string message = String.Format(CultureInfo.CurrentCulture, "Client name '{0}' already exists.", settings.Name);
               //   _messageBoxView.ShowError(_view, Core.Application.NameAndVersion, message);
               //   continue;
               //}
               // perform the add
               try
               {
                  _clientDictionary.Add(settings);
                  break;
               }
               catch (ArgumentException ex)
               {
                  _logger.ErrorFormat(ex, "{0}", ex.Message);
                  _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
               }
            }
         }
      }

      internal void ClientsAddLegacyClick()
      {
         using (var dialog = ServiceLocator.Resolve<ILegacyClientSetupPresenter>())
         {
            dialog.SettingsModel = new LegacyClientSettingsModel();
            while (dialog.ShowDialog(_view).Equals(DialogResult.OK))
            {
               var settings = AutoMapper.Mapper.Map<LegacyClientSettingsModel, ClientSettings>(dialog.SettingsModel);
               // perform the add
               try
               {
                  _clientDictionary.Add(settings);
                  break;
               }
               catch (ArgumentException ex)
               {
                  _logger.ErrorFormat(ex, "{0}", ex.Message);
                  _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
               }
            }
         }
      }

      public void ClientsEditClick()
      {
         // Check for SelectedSlot, and get out if not found
         if (_gridModel.SelectedSlot == null) return;

         if (_gridModel.SelectedSlot.Settings.IsFahClient())
         {
            EditFahClient();
         }
         else if (_gridModel.SelectedSlot.Settings.IsLegacy())
         {
            EditLegacyClient();
         }
         else
         {
            // no External support yet
            throw new InvalidOperationException("Client type is not supported.");
         }
      }

      private void EditFahClient()
      {
         Debug.Assert(_gridModel.SelectedSlot != null);
         IClient client = _clientDictionary[_gridModel.SelectedSlot.Settings.Name];
         ClientSettings originalSettings = client.Settings;
         Debug.Assert(originalSettings.IsFahClient());

         using (var dialog = ServiceLocator.Resolve<IFahClientSetupPresenter>())
         {
            dialog.SettingsModel = AutoMapper.Mapper.Map<ClientSettings, FahClientSettingsModel>(originalSettings);
            while (dialog.ShowDialog(_view).Equals(DialogResult.OK))
            {
               var newSettings = AutoMapper.Mapper.Map<FahClientSettingsModel, ClientSettings>(dialog.SettingsModel);
               // perform the edit
               try
               {
                  _clientDictionary.Edit(originalSettings.Name, newSettings);
                  break;
               }
               catch (ArgumentException ex)
               {
                  _logger.ErrorFormat(ex, "{0}", ex.Message);
                  _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
               }
            }
         }
      }

      private void EditLegacyClient()
      {
         Debug.Assert(_gridModel.SelectedSlot != null);
         IClient client = _clientDictionary[_gridModel.SelectedSlot.Settings.Name];
         ClientSettings originalSettings = client.Settings;
         Debug.Assert(originalSettings.IsLegacy());

         using (var dialog = ServiceLocator.Resolve<ILegacyClientSetupPresenter>())
         {
            dialog.SettingsModel = AutoMapper.Mapper.Map<ClientSettings, LegacyClientSettingsModel>(originalSettings);
            while (dialog.ShowDialog(_view).Equals(DialogResult.OK))
            {
               var newSettings = AutoMapper.Mapper.Map<LegacyClientSettingsModel, ClientSettings>(dialog.SettingsModel);
               // perform the edit
               try
               {
                  _clientDictionary.Edit(originalSettings.Name, newSettings);
                  break;
               }
               catch (ArgumentException ex)
               {
                  _logger.ErrorFormat(ex, "{0}", ex.Message);
                  _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
               }
            }
         }
      }

      private void HandleClientEdit(object sender, ClientEditedEventArgs e)
      {
         // the name changed
         if (e.PreviousName != e.NewName)
         {
            // update the Names in the benchmark collection
            _benchmarkCollection.UpdateOwnerName(e.PreviousName, e.PreviousPath, e.NewName);
         }
         // the path changed
         if (!Paths.Equal(e.PreviousPath, e.NewPath))
         {
            // update the Paths in the benchmark collection
            _benchmarkCollection.UpdateOwnerPath(e.NewName, e.PreviousPath, e.NewPath);
         }
      }

      public void ClientsDeleteClick()
      {
         // Check for SelectedSlot, and get out if not found
         if (_gridModel.SelectedSlot == null) return;

         _clientDictionary.Remove(_gridModel.SelectedSlot.Settings.Name);
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
         // Check for SelectedSlot, and get out if not found
         if (_gridModel.SelectedSlot == null) return;

         _retrievalLogic.RetrieveSingleClient(_gridModel.SelectedSlot.Settings.Name);
      }
      
      public void ClientsRefreshAllClick()
      {
         _retrievalLogic.QueueNewRetrieval();
      }

      public void ClientsViewCachedLogClick()
      {
         // Check for SelectedSlot, and get out if not found
         if (_gridModel.SelectedSlot == null) return;

         string logFilePath = Path.Combine(_prefs.CacheDirectory, _gridModel.SelectedSlot.Settings.CachedFahLogFileName());
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

      public void ClientsViewClientFilesClick()
      {
         // Check for SelectedSlot, and get out if not found);
         if (_gridModel.SelectedSlot == null) return;
         Debug.Assert(_gridModel.SelectedSlot.Settings.IsLegacy());

         if (_gridModel.SelectedSlot.Settings.LegacyClientSubType.Equals(LegacyClientSubType.Path))
         {
            HandleProcessStartResult(_processStarter.ShowFileExplorer(_gridModel.SelectedSlot.Settings.DataPath()));
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
         var style = _prefs.Get<TimeStyleType>(Preference.TimeStyle);
         _prefs.Set(Preference.TimeStyle, style.Equals(TimeStyleType.Standard) 
                                 ? TimeStyleType.Formatted 
                                 : TimeStyleType.Standard);
         _prefs.Save();
         _view.DataGridView.Invalidate();
      }

      public void ViewToggleCompletedCountStyleClick()
      {
         var style = _prefs.Get<CompletedCountDisplayType>(Preference.CompletedCountDisplay);
         _prefs.Set(Preference.CompletedCountDisplay, style.Equals(CompletedCountDisplayType.ClientTotal)
                                 ? CompletedCountDisplayType.ClientRunTotal
                                 : CompletedCountDisplayType.ClientTotal);
         _prefs.Save();
         _view.DataGridView.Invalidate();
      }

      public void ViewToggleVersionInformationClick()
      {
         _prefs.Set(Preference.ShowVersions, !_prefs.Get<bool>(Preference.ShowVersions));
         _prefs.Save();
         _view.DataGridView.Invalidate();
      }

      public void ViewCycleBonusCalculationClick()
      {
         var calculationType = _prefs.Get<BonusCalculationType>(Preference.CalculateBonus);
         int typeIndex = 0;
         // None is always LAST entry
         if (calculationType.Equals(BonusCalculationType.None) == false)
         {
            typeIndex = (int)calculationType;
            typeIndex++;
         }

         calculationType = (BonusCalculationType)typeIndex;
         _prefs.Set(Preference.CalculateBonus, calculationType);
         _prefs.Save();

         string calculationTypeString = (from item in OptionsModel.BonusCalculationList
                                         where ((BonusCalculationType)item.ValueMember) == calculationType
                                         select item.DisplayMember).First();
         _view.ShowNotifyToolTip(calculationTypeString);
         _view.DataGridView.Invalidate();
      }

      public void ViewCycleCalculationClick()
      {
         var calculationType = _prefs.Get<PpdCalculationType>(Preference.PpdCalculation);
         int typeIndex = 0;
         // EffectiveRate is always LAST entry
         if (calculationType.Equals(PpdCalculationType.EffectiveRate) == false)
         {
            typeIndex = (int)calculationType;
            typeIndex++;
         }

         calculationType = (PpdCalculationType)typeIndex;
         _prefs.Set(Preference.PpdCalculation, calculationType);
         _prefs.Save();

         string calculationTypeString = (from item in OptionsModel.PpdCalculationList
                                         where ((PpdCalculationType)item.ValueMember) == calculationType
                                         select item.DisplayMember).First();
         _view.ShowNotifyToolTip(calculationTypeString);
         _view.DataGridView.Invalidate();
      }

      internal void ViewToggleFollowLogFile()
      {
         _prefs.Set(Preference.FollowLogFile, !_prefs.Get<bool>(Preference.FollowLogFile));
      }

      #endregion

      #region Tools Menu Handling Methods

      public void ToolsDownloadProjectsClick()
      {
         var downloader = ServiceLocator.Resolve<IProjectSummaryDownloader>();
         // Clear the Project Not Found Cache and Last Download Time
         _proteinDictionary.ClearProjectsNotFoundCache();
         downloader.ResetLastDownloadTime();
         // Execute Asynchronous Download
         var projectDownloadView = ServiceLocator.Resolve<IProgressDialogView>();
         projectDownloadView.OwnerWindow = _view;
         projectDownloadView.ProcessRunner = downloader;
         projectDownloadView.UpdateMessage(_prefs.Get<string>(Preference.ProjectDownloadUrl));
         projectDownloadView.Process();

         IEnumerable<ProteinLoadInfo> loadInfo;
         try
         {
            loadInfo = _proteinDictionary.Load(downloader.DownloadFilePath);
            foreach (var info in loadInfo.Where(info => !info.Result.Equals(ProteinLoadResult.NoChange)))
            {
               Logger.Info(info.ToString());
            }
            _proteinDictionary.Write();
            //_proteinDictionary.Write(Path.Combine(_prefs.ApplicationDataFolderPath, "ProjectInfo.xml"), new Core.Serializers.XmlFileSerializer<List<Protein>>());
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
            return;
         }

         if (loadInfo.Where(x => !x.Result.Equals(ProteinLoadResult.NoChange)).Count() != 0)
         {
            _retrievalLogic.QueueNewRetrieval();
            using (var dlg = new ProteinLoadResultsDialog())
            {
               dlg.DataBind(loadInfo);
               dlg.ShowDialog(_view);
            }
         }
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
         var location = _prefs.Get<Point>(Preference.BenchmarksFormLocation);
         var size = _prefs.Get<Size>(Preference.BenchmarksFormSize);

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

      internal void ToolsPointsCalculatorClick()
      {
         var dlg = ServiceLocator.Resolve<ProteinCalculatorForm>();
         dlg.Show(_view);
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
         _view.LogFileViewer.HighlightLines(_prefs.Get<bool>(Preference.ColorLogFile));
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
      public SlotModel FindSlotModel(string key)
      {
         return _clientDictionary.Slots.FirstOrDefault(slot => slot.Name == key);
      }
   }
}
