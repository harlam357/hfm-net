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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Forms.Models;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   public class MainPresenter
   {
      #region Properties
   
      /// <summary>
      /// Holds the state of the window before it is hidden (minimise to tray behaviour)
      /// </summary>
      public FormWindowState OriginalWindowState { get; private set; }

      /// <summary>
      /// Holds current Sort Column Name
      /// </summary>
      public string SortColumnName { get; private set; }

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      public SortOrder SortColumnOrder { get; private set; }
      
      #endregion
      
      #region Fields

      private HistoryPresenter _historyPresenter;
   
      #region Service Interfaces
   
      private readonly IMainView _view;
      private readonly IPreferenceSet _prefs;
      private readonly IInstanceCollection _instanceCollection;
      private readonly IInstanceConfigurationManager _configurationManager;
      private readonly IProteinCollection _proteinCollection;
      private readonly IUpdateLogic _updateLogic;
      private readonly IMessagesView _messagesView;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IOpenFileDialogView _openFileDialogView;
      private readonly ISaveFileDialogView _saveFileDialogView;
      private readonly IExternalProcessStarter _processStarter;
      
      #endregion
      
      #endregion

      #region Constructor

      public MainPresenter(IMainView view, IPreferenceSet prefs, IInstanceCollection instanceCollection,
                           IInstanceConfigurationManager configurationManager, IProteinCollection proteinCollection,
                           IUpdateLogic updateLogic, IMessagesView messagesView, IMessageBoxView messageBoxView, 
                           IOpenFileDialogView openFileDialogView, ISaveFileDialogView saveFileDialogView, 
                           IExternalProcessStarter processStarter)
      {
         _view = view;
         _prefs = prefs;
         _instanceCollection = instanceCollection;
         _configurationManager = configurationManager;
         _proteinCollection = proteinCollection;
         _updateLogic = updateLogic;
         _messagesView = messagesView;
         _messageBoxView = messageBoxView;
         _openFileDialogView = openFileDialogView;
         _saveFileDialogView = saveFileDialogView;
         _processStarter = processStarter;

         // set owner
         _updateLogic.Owner = _view;

         SortColumnName = String.Empty;
         SortColumnOrder = SortOrder.None;
      }
      
      #endregion
      
      #region View Handling Methods
   
      public void ViewShown()
      {
         if (_prefs.GetPreference<bool>(Preference.RunMinimized))
         {
            _view.WindowState = FormWindowState.Minimized;
         }

         string fileName = String.Empty;

         if (_prefs.GetPreference<bool>(Preference.UseDefaultConfigFile))
         {
            fileName = _prefs.GetPreference<string>(Preference.DefaultConfigFile);
         }

         if (String.IsNullOrEmpty(fileName) == false)
         {
            LoadConfigFile(fileName);
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
               _view.ApplySort();
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
         if (CheckForConfigurationChanges() == false)
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

      public void RestoreFormPreferences()
      {
         //TODO: Would like to do this here in lieu of in frmMain_Shown() event.
         // There is some drawing error that if Minimized here, the first time the
         // Form is restored from the system tray, the DataGridView is drawn with
         // a big black box on the right hand side.  Like it didn't get initialized
         // properly when the Form was created.
         //if (Prefs.RunMinimized)
         //{
         //   WindowState = FormWindowState.Minimized;
         //}

         // Restore state data
         var location = _prefs.GetPreference<Point>(Preference.FormLocation);
         var size = _prefs.GetPreference<Size>(Preference.FormSize);

         if (location.X != 0 && location.Y != 0)
         {
            // Set StartPosition to manual
            _view.SetManualStartPosition();
            _view.Location = location;
         }
         if (size.Width != 0 && size.Height != 0)
         {
            if (_prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
            {
               size = new Size(size.Width, size.Height + _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            }
            _view.Size = size;
            _view.SplitContainer.SplitterDistance = _prefs.GetPreference<int>(Preference.FormSplitLocation);
         }

         if (_prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
         {
            ShowHideLogWindow(false);
         }

         if (_prefs.GetPreference<bool>(Preference.QueueViewerVisible) == false)
         {
            ShowHideQueue(false);
         }

         //if (Prefs.FormSortColumn != String.Empty &&
         //    Prefs.FormSortOrder != SortOrder.None)
         //{
            SortColumnName = _prefs.GetPreference<string>(Preference.FormSortColumn);
            SortColumnOrder = _prefs.GetPreference<SortOrder>(Preference.FormSortOrder);
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
               _view.DataGridView.Columns[index].DisplayIndex = Int16.Parse(a[0]);
               if (_view.DataGridView.Columns[index].AutoSizeMode.Equals(DataGridViewAutoSizeColumnMode.Fill) == false)
               {
                  _view.DataGridView.Columns[index].Width = Int16.Parse(a[1]);
               }
               _view.DataGridView.Columns[index].Visible = bool.Parse(a[2]);
            }
         }
         catch (NullReferenceException)
         {
            // This happens when the FormColumns setting is empty
         }
      }

      private void CheckForAndFireUpdateProcess()
      {
         if (String.IsNullOrEmpty(_updateLogic.UpdateFilePath) == false)
         {
            HfmTrace.WriteToHfmConsole(String.Format(CultureInfo.CurrentCulture,
               "Firing update file '{0}'...", _updateLogic.UpdateFilePath));
            try
            {
               Process.Start(_updateLogic.UpdateFilePath);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               string message = String.Format(CultureInfo.CurrentCulture,
                                              "Update process failed to start with the following error:{0}{0}{1}",
                                              Environment.NewLine, ex.Message);
               _messageBoxView.ShowError(_view, message, _view.Text);
            }
         }
      }
      
      #endregion
      
      #region Data Grid View Handling Methods

      public void DisplaySelectedInstance()
      {
         if (_instanceCollection.SelectedDisplayInstance != null)
         {
            _view.SetClientMenuItemsVisible(_instanceCollection.ClientFilesMenuItemVisible,
                                            _instanceCollection.CachedLogMenuItemVisible,
                                            _instanceCollection.ClientFilesMenuItemVisible ||
                                            _instanceCollection.CachedLogMenuItemVisible);
         
            _view.StatusLabelLeftText = _instanceCollection.SelectedDisplayInstance.ClientPathAndArguments;

            _view.QueueControl.SetQueue(_instanceCollection.SelectedDisplayInstance.Queue,
               _instanceCollection.SelectedDisplayInstance.TypeOfClient,
               _instanceCollection.SelectedDisplayInstance.ClientIsOnVirtualMachine);

            // if we've got a good queue read, let queueControl_QueueIndexChanged()
            // handle populating the log lines.
            ClientQueue qBase = _instanceCollection.SelectedDisplayInstance.Queue;
            if (qBase != null) return;

            // otherwise, load up the CurrentLogLines
            SetLogLines(_instanceCollection.SelectedDisplayInstance, _instanceCollection.SelectedDisplayInstance.CurrentLogLines);
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

         if (_instanceCollection.SelectedDisplayInstance != null)
         {
            //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Changed Queue Index ({0} - {1})", InstanceName, e.Index));

            // Check the UnitLogLines array against the requested Queue Index - Issue 171
            try
            {
               var logLines = _instanceCollection.SelectedDisplayInstance.GetLogLinesForQueueIndex(index);
               if (logLines == null && index == _instanceCollection.SelectedDisplayInstance.Queue.CurrentIndex)
               {
                  logLines = _instanceCollection.SelectedDisplayInstance.CurrentLogLines;
               }

               SetLogLines(_instanceCollection.SelectedDisplayInstance, logLines);
            }
            catch (ArgumentOutOfRangeException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
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

      private void SetLogLines(IDisplayInstance instance, IList<LogLine> logLines)
      {
         /*** Checked LogLine Count ***/
         if (logLines != null && logLines.Count > 0)
         {
            // Different Client... Load LogLines
            if (_view.LogFileViewer.LogOwnedByInstanceName.Equals(instance.Name) == false)
            {
               _view.LogFileViewer.SetLogLines(logLines, instance.Name);
               ApplyColorLogFileSetting();

               //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Set Log Lines (Changed Client - {0})", instance.InstanceName));
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
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, instance.Name, ex);
               }

               // If the last text line in the textbox DOES NOT equal the last LogLine Text... Load LogLines.
               // Otherwise, the log has not changed, don't update and perform the log "flicker".
               if (_view.LogFileViewer.Lines[_view.LogFileViewer.Lines.Length - 1].Equals(lastLogLine) == false)
               {
                  _view.LogFileViewer.SetLogLines(logLines, instance.Name);
                  ApplyColorLogFileSetting();

                  //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Set Log Lines (log lines different)");
               }
            }
            // Nothing in the Textbox... Load LogLines
            else
            {
               _view.LogFileViewer.SetLogLines(logLines, instance.Name);
               ApplyColorLogFileSetting();
            }
         }
         else
         {
            _view.LogFileViewer.SetNoLogLines();
         }

         _view.LogFileViewer.ScrollToBottom();
      }

      public void DataGridViewColumnDisplayIndexChanged()
      {
         if (_view.DataGridView.Columns.Count == DataGridViewWrapper.NumberOfDisplayFields)
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
         _prefs.SetPreference(Preference.FormSortColumn, SortColumnName);
         _prefs.SetPreference(Preference.FormSortOrder, SortColumnOrder);
      }

      public void UpdateMainSplitContainerDistance(int splitterDistance)
      {
         _prefs.SetPreference(Preference.FormSplitLocation, splitterDistance);
         _prefs.Save();
      }

      public void DataGridViewSorted()
      {
         SortColumnName = _view.DataGridView.SortedColumn.Name;
         SortColumnOrder = _view.DataGridView.SortOrder;

         SaveSortColumn(); // Save Column Sort Order - Issue 73
         _prefs.Save();

         _view.SelectCurrentRowKey();
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

               // Check for SelectedClientInstance, and get out if not found
               if (_instanceCollection.SelectedClientInstance == null) return;

               _view.SetGridContextMenuItemsVisible(_instanceCollection.ClientFilesMenuItemVisible,
                                                    _instanceCollection.CachedLogMenuItemVisible,
                                                    _instanceCollection.ClientFilesMenuItemVisible ||
                                                    _instanceCollection.CachedLogMenuItemVisible);

               _view.ShowGridContextMenuStrip(_view.DataGridView.PointToScreen(new Point(coordX, coordY)));
            }
         }
         if (button == MouseButtons.Left && clicks == 2)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               // Check for SelectedClientInstance, and get out if not found
               if (_instanceCollection.SelectedClientInstance == null) return;

               if (_instanceCollection.SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance))
               {
                  HandleProcessStartResult(_processStarter.ShowFileExplorer(_instanceCollection.SelectedClientInstance.Settings.Path));
               }
            }
         }
      }
      
      #endregion
      
      #region File Handling Methods

      public void AutoSaveConfig()
      {
         if (_prefs.GetPreference<bool>(Preference.AutoSaveConfig))
         {
            FileSaveClick();
         }
      }

      public void FileNewClick()
      {
         if (_instanceCollection.RetrievalInProgress)
         {
            _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to create a new config file.", _view.Text);
            return;
         }

         if (CheckForConfigurationChanges())
         {
            _instanceCollection.Clear();
         }
      }

      public void FileOpenClick()
      {
         if (_instanceCollection.RetrievalInProgress)
         {
            _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to open another config file.", _view.Text);
            return;
         }

         if (CheckForConfigurationChanges())
         {
            _openFileDialogView.DefaultExt = _configurationManager.ConfigFileExtension;
            _openFileDialogView.Filter = _configurationManager.FileTypeFilters;
            _openFileDialogView.FileName = _configurationManager.ConfigFilename;
            _openFileDialogView.RestoreDirectory = true;
            if (_openFileDialogView.ShowDialog() == DialogResult.OK)
            {
               LoadConfigFile(_openFileDialogView.FileName, _openFileDialogView.FilterIndex);
            }
         }
      }

      private bool CheckForConfigurationChanges()
      {
         if (_instanceCollection.ChangedAfterSave)
         {
            DialogResult result = _messageBoxView.AskYesNoCancelQuestion(_view, 
               String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes / No - Continue and do not save the changes / Cancel - Do not continue", Environment.NewLine),
               _view.Text);
               
            switch (result)
            {
               case DialogResult.Yes:
                  FileSaveClick();
                  return !(_instanceCollection.ChangedAfterSave);
               case DialogResult.No:
                  return true;
               case DialogResult.Cancel:
                  return false;
            }
            return false;
         }

         return true;
      }

      public void FileSaveClick()
      {
         if (_configurationManager.HasConfigFilename == false)
         {
            FileSaveAsClick();
         }
         else
         {
            try
            {
               _instanceCollection.WriteConfigFile();
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
                  _view.Text);
            }
         }
      }

      public void FileSaveAsClick()
      {
         // No Config File and no Instances, stub out
         if (_configurationManager.HasConfigFilename == false && _instanceCollection.HasInstances == false) return;

         _saveFileDialogView.DefaultExt = _configurationManager.ConfigFileExtension;
         _saveFileDialogView.Filter = _configurationManager.FileTypeFilters;
         if (_saveFileDialogView.ShowDialog() == DialogResult.OK)
         {
            try
            {
               _instanceCollection.WriteConfigFile(_saveFileDialogView.FileName, _saveFileDialogView.FilterIndex); // Issue 75
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
                  _view.Text);
            }
         }
      }

      private void LoadConfigFile(string filePath)
      {
         LoadConfigFile(filePath, 1);
      }

      private void LoadConfigFile(string filePath, int filterIndex)
      {
         try
         {
            // Read the config file
            _instanceCollection.ReadConfigFile(filePath, filterIndex);

            if (_instanceCollection.HasInstances == false)
            {
               _messageBoxView.ShowError(_view, "No client configurations were loaded from the given config file.", _view.Text);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
               "No client configurations were loaded from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message),
               _view.Text);
         }
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
         var settings = new ClientInstanceSettings();
         var newHost = InstanceProvider.GetInstance<IInstanceSettingsPresenter>();
         newHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (newHost.ShowDialog(_view).Equals(DialogResult.OK))
         {
            try
            {
               _instanceCollection.Add(newHost.SettingsModel.Settings);
               break;
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, ex.Message, _view.Text);
            }
         }
      }

      public void ClientsEditClick()
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_instanceCollection.SelectedClientInstance == null) return;

         var settings = _instanceCollection.SelectedClientInstance.Settings.DeepCopy();
         string previousName = settings.InstanceName;
         string previousPath = settings.Path;
         var editHost = InstanceProvider.GetInstance<IInstanceSettingsPresenter>();
         editHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (editHost.ShowDialog(_view).Equals(DialogResult.OK))
         {
            try
            {
               _instanceCollection.Edit(previousName, previousPath, editHost.SettingsModel.Settings);
               break;
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, ex.Message, _view.Text);
            }
         }
      }

      public void ClientsDeleteClick()
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_instanceCollection.SelectedClientInstance == null) return;

         //TODO: Add _instanceCollection.RemoveSelected()
         _instanceCollection.Remove(_instanceCollection.SelectedClientInstance.Settings.InstanceName);
      }

      public void ClientsMergeClick()
      {
         var settings = new ClientInstanceSettings { ExternalInstance = true };
         var newHost = InstanceProvider.GetInstance<IInstanceSettingsPresenter>();
         newHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (newHost.ShowDialog(_view).Equals(DialogResult.OK))
         {
            try
            {
               _instanceCollection.Add(newHost.SettingsModel.Settings);
               break;
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, ex.Message, _view.Text);
            }
         }
      }

      public void ClientsRefreshSelectedClick()
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_instanceCollection.SelectedClientInstance == null) return;

         _instanceCollection.RetrieveSingleClient(_instanceCollection.SelectedClientInstance.Settings.InstanceName);
      }
      
      public void ClientsRefreshAllClick()
      {
         _instanceCollection.QueueNewRetrieval();
      }

      public void ClientsViewCachedLogClick()
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_instanceCollection.SelectedClientInstance == null) return;

         string logFilePath = Path.Combine(_prefs.CacheDirectory, _instanceCollection.SelectedClientInstance.Settings.CachedFahLogName);
         if (File.Exists(logFilePath))
         {
            HandleProcessStartResult(_processStarter.ShowCachedLogFile(logFilePath));
         }
         else
         {
            string message = String.Format(CultureInfo.CurrentCulture, "The FAHlog.txt file for '{0}' does not exist.",
                                           _instanceCollection.SelectedClientInstance.Settings.InstanceName);
            _messageBoxView.ShowInformation(_view, message, PlatformOps.ApplicationNameAndVersion);
         }
      }

      public void ClientsViewClientFilesClick()
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_instanceCollection.SelectedClientInstance == null) return;

         if (_instanceCollection.SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance))
         {
            HandleProcessStartResult(_processStarter.ShowFileExplorer(_instanceCollection.SelectedClientInstance.Settings.Path));
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
         if (show == false)
         {
            _view.LogFileViewer.Visible = false;
            _view.SplitContainer.Panel2Collapsed = true;
            _prefs.SetPreference(Preference.FormLogWindowHeight, (_view.SplitContainer.Height - _view.SplitContainer.SplitterDistance));
            _view.Size = new Size(_view.Size.Width, _view.Size.Height - _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
         }
         else
         {
            _view.LogFileViewer.Visible = true;
            //Resize -= frmMain_Resize; // disable Form resize event for this operation
            _view.DisableViewResizeEvent();
            _view.Size = new Size(_view.Size.Width, _view.Size.Height + _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            //Resize += frmMain_Resize; // re-enable
            _view.EnableViewResizeEvent();
            _view.SplitContainer.Panel2Collapsed = false;
         }
      }
      
      public void ShowHideQueue()
      {
         ShowHideQueue(!_view.QueueControl.Visible);
      }

      private void ShowHideQueue(bool show)
      {
         if (show == false)
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
         // Clear the Project Not Found Cache and Last Download Time
         _proteinCollection.ClearProjectsNotFoundCache();
         _proteinCollection.Downloader.ResetLastDownloadTime();
         // Execute Asynchronous Download
         var projectDownloadView = InstanceProvider.GetInstance<IProgressDialogView>("projectDownloadView");
         projectDownloadView.OwnerWindow = _view;
         projectDownloadView.ProcessRunner = _proteinCollection.Downloader;
         projectDownloadView.UpdateMessage(_proteinCollection.Downloader.Prefs.GetPreference<string>(Preference.ProjectDownloadUrl));
         projectDownloadView.Process();
      }

      public void ToolsBenchmarksClick()
      {
         int projectId = 0;

         // Check for SelectedDisplayInstance, and if found... load its ProjectID.
         if (_instanceCollection.SelectedDisplayInstance != null)
         {
            projectId = _instanceCollection.SelectedDisplayInstance.ProjectID;
         }

         var frm = InstanceProvider.GetInstance<IBenchmarksView>();
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
         if (_historyPresenter == null)
         {
            _historyPresenter = InstanceProvider.GetInstance<HistoryPresenter>();
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

      public void RefreshUserStatsClick()
      {
         _instanceCollection.RefreshUserStatsData(true);
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

      public void ApplyMessageLevelIfChanged()
      {
         var newLevel = (TraceLevel)_prefs.GetPreference<int>(Preference.MessageLevel);
         if (newLevel != TraceLevelSwitch.Instance.Level)
         {
            TraceLevelSwitch.Instance.Level = newLevel;
            HfmTrace.WriteToHfmConsole(String.Format("Debug Message Level Changed: {0}", newLevel));
         }
      }

      public void ApplyColorLogFileSetting()
      {
         if (_prefs.GetPreference<bool>(Preference.ColorLogFile))
         {
            _view.LogFileViewer.HighlightLines();
         }
         else
         {
            _view.LogFileViewer.RemoveHighlight();
         }
      }

      private void HandleProcessStartResult(string message)
      {
         if (message != null)
         {
            _messageBoxView.ShowError(_view, message, PlatformOps.ApplicationNameAndVersion);
         }
      }
      
      #endregion
   }
}
