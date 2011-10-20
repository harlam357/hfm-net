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
using System.IO;
using System.Linq;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Forms.Models;
using HFM.Framework;
using HFM.Framework.DataTypes;
using HFM.Instances;

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
      /// Holds current Sort Column Name
      /// </summary>
      private string SortColumnName { get; set; }

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      private SortOrder SortColumnOrder { get; set; }

      /// <summary>
      /// Denotes the Saved State of the Current Client Configuration (false == saved, true == unsaved)
      /// </summary>
      private bool ChangedAfterSave { get; set; }

      private ClientInstance SelectedClientInstance { get; set; }

      private IDisplayInstance SelectedDisplayInstance { get; set; }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Client Files' is Visbile
      /// </summary>
      private bool ClientFilesMenuItemVisible
      {
         get
         {
            return SelectedClientInstance == null ||
                   SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance);
         }
      }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Cached Log File' is Visbile
      /// </summary>
      private bool CachedLogMenuItemVisible
      {
         get
         {
            return SelectedClientInstance == null ||
                 !(SelectedClientInstance.Settings.ExternalInstance);
         }
      }

      /// <summary>
      /// Tells the SortableBindingList whether to sort Offline Clients Last
      /// </summary>
      private bool OfflineClientsLast
      {
         set
         {
            if (_displayCollection.OfflineClientsLast != value)
            {
               _displayCollection.OfflineClientsLast = value;
               ApplySort();
            }
         }
      }
      
      #endregion
      
      #region Fields

      private HistoryPresenter _historyPresenter;

      private readonly BindingSource _displayBindingSource;
   
      #region Views
   
      private readonly IMainView _view;
      private readonly IMessagesView _messagesView;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IOpenFileDialogView _openFileDialogView;
      private readonly ISaveFileDialogView _saveFileDialogView;

      #endregion

      #region Collections

      private readonly InstanceCollection _instanceCollection;
      /// <summary>
      /// Display instance collection (this is bound to the DataGridView)
      /// </summary>
      private readonly IDisplayInstanceCollection _displayCollection;
      private readonly IProteinCollection _proteinCollection;
      private readonly IUnitInfoContainer _unitInfoContainer;
      private readonly IProteinBenchmarkContainer _benchmarkContainer;

      #endregion

      #region Logic Services

      private readonly IUpdateLogic _updateLogic;
      private readonly RetrievalLogic _retrievalLogic;
      private readonly IExternalProcessStarter _processStarter;
      private readonly IClientInstanceFactory _instanceFactory;

      #endregion

      #region Data Services

      private readonly IPreferenceSet _prefs;
      private readonly IXmlStatsDataContainer _statsData;
      private readonly InstanceConfigurationManager _configurationManager;
      
      #endregion

      #endregion

      #region Constructor

      public MainPresenter(IMainView view, IMessagesView messagesView, IMessageBoxView messageBoxView,
                           IOpenFileDialogView openFileDialogView, ISaveFileDialogView saveFileDialogView,
                           InstanceCollection instanceCollection, IDisplayInstanceCollection displayCollection, 
                           IProteinCollection proteinCollection, IUnitInfoContainer unitInfoContainer,
                           IProteinBenchmarkContainer benchmarkContainer, IUpdateLogic updateLogic, 
                           RetrievalLogic retrievalLogic, IExternalProcessStarter processStarter, 
                           IClientInstanceFactory instanceFactory, IPreferenceSet prefs, 
                           IXmlStatsDataContainer statsData, InstanceConfigurationManager configurationManager)
      {
         // Views
         _view = view;
         _messagesView = messagesView;
         _messageBoxView = messageBoxView;
         _openFileDialogView = openFileDialogView;
         _saveFileDialogView = saveFileDialogView;
         // Collections
         _instanceCollection = instanceCollection;
         _displayCollection = displayCollection;
         _proteinCollection = proteinCollection;
         _unitInfoContainer = unitInfoContainer;
         _benchmarkContainer = benchmarkContainer;
         // Logic Services
         _updateLogic = updateLogic;
         _updateLogic.Owner = _view;
         _retrievalLogic = retrievalLogic;
         _retrievalLogic.Initialize(this);
         _processStarter = processStarter;
         _instanceFactory = instanceFactory;
         // Data Services
         _prefs = prefs;
         _statsData = statsData;
         _configurationManager = configurationManager;

         _displayBindingSource = new BindingSource();

         SortColumnName = String.Empty;
         SortColumnOrder = SortOrder.None;

         // Hook-up Event Handlers
         _proteinCollection.Downloader.ProjectInfoUpdated += delegate { _retrievalLogic.QueueNewRetrieval(); };
      }
      
      #endregion

      #region Initialize

      public void Initialize()
      {
         // Restore Form Preferences (must be done AFTER DataGridView columns are setup)
         RestoreViewPreferences();
         // 
         BindToDataGridView();
         // 
         SubscribeToPreferenceSetEvents();
         // Apply User Stats Enabled Selection
         UserStatsEnabledChanged();
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

      private void BindToDataGridView()
      {
         _view.DataGridView.AutoGenerateColumns = false;
         _displayBindingSource.DataSource = _displayCollection;
         _view.DataGridView.DataSource = _displayBindingSource;
      }

      private void SubscribeToPreferenceSetEvents()
      {
         _prefs.FormShowStyleSettingsChanged += delegate { SetViewShowStyle(); };
         _prefs.MessageLevelChanged += delegate { ApplyMessageLevelIfChanged(); };
         _prefs.ColorLogFileChanged += delegate { ApplyColorLogFileSetting(); };
         _prefs.PpdCalculationChanged += delegate { RefreshDisplay(); };
         _prefs.DecimalPlacesChanged += delegate { RefreshDisplay(); };
         _prefs.CalculateBonusChanged += delegate { RefreshDisplay(); };
         _prefs.ShowUserStatsChanged += delegate { UserStatsEnabledChanged(); };

         // Set Offline Clients Sort Flag
         OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast);
         _prefs.OfflineLastChanged += delegate { OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast); };
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
         _view.SplitContainer.SplitterMoved += delegate { UpdateMainSplitContainerDistance(_view.SplitContainer.SplitterDistance); };

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
               ApplySort();
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

      private void DisplaySelectedInstance()
      {
         if (SelectedDisplayInstance != null)
         {
            _view.SetClientMenuItemsVisible(ClientFilesMenuItemVisible,
                                            CachedLogMenuItemVisible,
                                            ClientFilesMenuItemVisible ||
                                            CachedLogMenuItemVisible);
         
            _view.StatusLabelLeftText = SelectedDisplayInstance.ClientPathAndArguments;

            _view.QueueControl.SetQueue(SelectedDisplayInstance.Queue,
                                        SelectedDisplayInstance.TypeOfClient,
                                        SelectedDisplayInstance.ClientIsOnVirtualMachine);

            // if we've got a good queue read, let queueControl_QueueIndexChanged()
            // handle populating the log lines.
            if (SelectedDisplayInstance.Queue != null) return;

            // otherwise, load up the CurrentLogLines
            SetLogLines(SelectedDisplayInstance, SelectedDisplayInstance.CurrentLogLines);
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

         if (SelectedDisplayInstance != null)
         {
            //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Changed Queue Index ({0} - {1})", InstanceName, e.Index));

            // Check the UnitLogLines array against the requested Queue Index - Issue 171
            try
            {
               var logLines = SelectedDisplayInstance.GetLogLinesForQueueIndex(index);
               if (logLines == null && index == SelectedDisplayInstance.Queue.CurrentIndex)
               {
                  logLines = SelectedDisplayInstance.CurrentLogLines;
               }

               SetLogLines(SelectedDisplayInstance, logLines);
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
               _view.LogFileViewer.SetLogLines(logLines, instance.Name, _prefs.GetPreference<bool>(Preference.ColorLogFile));

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
         _prefs.SetPreference(Preference.FormSortColumn, SortColumnName);
         _prefs.SetPreference(Preference.FormSortOrder, SortColumnOrder);
      }

      private void UpdateMainSplitContainerDistance(int splitterDistance)
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

         SelectCurrentRowKey();
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
               if (SelectedClientInstance == null) return;

               _view.SetGridContextMenuItemsVisible(ClientFilesMenuItemVisible,
                                                    CachedLogMenuItemVisible,
                                                    ClientFilesMenuItemVisible ||
                                                    CachedLogMenuItemVisible);

               _view.ShowGridContextMenuStrip(_view.DataGridView.PointToScreen(new Point(coordX, coordY)));
            }
         }
         if (button == MouseButtons.Left && clicks == 2)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               // Check for SelectedClientInstance, and get out if not found
               if (SelectedClientInstance == null) return;

               if (SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance))
               {
                  HandleProcessStartResult(_processStarter.ShowFileExplorer(SelectedClientInstance.Settings.Path));
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
         if (_retrievalLogic.RetrievalInProgress)
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
               // clear the clients and UI
               Clear();
               // 
               LoadConfigFile(_openFileDialogView.FileName, _openFileDialogView.FilterIndex);
            }
         }
      }

      private void LoadConfigFile(string filePath, int filterIndex = 1)
      {
         Debug.Assert(filePath != null);

         try
         {
            // Read the config file
            ReadConfigFile(filePath, filterIndex);

            if (!_instanceCollection.HasInstances())
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

      /// <summary>
      /// Reads a collection of Client Instance Settings from file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      private void ReadConfigFile(string filePath, int filterIndex)
      {
         Debug.Assert(filePath != null);

         if (filterIndex > _configurationManager.SettingsPluginsCount)
         {
            throw new ArgumentOutOfRangeException("filterIndex", String.Format(CultureInfo.CurrentCulture,
               "Argument 'filterIndex' must be between 1 and {0}.", _configurationManager.SettingsPluginsCount));
         }

         ICollection<ClientInstance> instances = _configurationManager.ReadConfigFile(filePath, filterIndex);
         _instanceCollection.LoadInstances(instances);

         if (_instanceCollection.HasInstances())
         {
            RefreshDisplay();
            // Get client logs         
            _retrievalLogic.QueueNewRetrieval();
            // Start Retrieval and Web Generation Timers
            _retrievalLogic.SetTimerState();
         }
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
         if (!_configurationManager.HasConfigFilename)
         {
            FileSaveAsClick();
         }
         else
         {
            try
            {
               WriteConfigFile();
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

      private void WriteConfigFile()
      {
         WriteConfigFile(_configurationManager.ConfigFilename, _configurationManager.SettingsPluginIndex);
      }

      public void FileSaveAsClick()
      {
         // No Config File and no Instances, stub out
         if (!_configurationManager.HasConfigFilename && !_instanceCollection.HasInstances()) return;

         _saveFileDialogView.DefaultExt = _configurationManager.ConfigFileExtension;
         _saveFileDialogView.Filter = _configurationManager.FileTypeFilters;
         if (_saveFileDialogView.ShowDialog() == DialogResult.OK)
         {
            try
            {
               WriteConfigFile(_saveFileDialogView.FileName, _saveFileDialogView.FilterIndex); // Issue 75
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

      /// <summary>
      /// Saves the current collection of Client Instances to file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      private void WriteConfigFile(string filePath, int filterIndex)
      {
         if (String.IsNullOrEmpty(filePath))
         {
            throw new ArgumentException("Argument 'filePath' cannot be a null or empty string.", "filePath");
         }

         if (filterIndex > _configurationManager.SettingsPluginsCount)
         {
            throw new ArgumentOutOfRangeException("filterIndex", String.Format(CultureInfo.CurrentCulture,
               "Argument 'filterIndex' must be between 1 and {0}.", _configurationManager.SettingsPluginsCount));
         }

         _configurationManager.WriteConfigFile(GetCurrentInstanceArray(), filePath, filterIndex);

         ChangedAfterSave = false;
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
         var settings = new ClientInstanceSettings();
         var newHost = InstanceProvider.GetInstance<IInstanceSettingsPresenter>();
         newHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (newHost.ShowDialog(_view).Equals(DialogResult.OK))
         {
            try
            {
               Add(newHost.SettingsModel.Settings);
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
         if (SelectedClientInstance == null) return;

         var settings = SelectedClientInstance.Settings.DeepClone();
         string previousName = settings.InstanceName;
         string previousPath = settings.Path;
         var editHost = InstanceProvider.GetInstance<IInstanceSettingsPresenter>();
         editHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (editHost.ShowDialog(_view).Equals(DialogResult.OK))
         {
            try
            {
               Edit(previousName, previousPath, editHost.SettingsModel.Settings);
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
         if (SelectedClientInstance == null) return;

         Remove(SelectedClientInstance.Settings.InstanceName);
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
               Add(newHost.SettingsModel.Settings);
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
         if (SelectedClientInstance == null) return;

         _retrievalLogic.RetrieveSingleClient(SelectedClientInstance.Settings.InstanceName);
      }
      
      public void ClientsRefreshAllClick()
      {
         _retrievalLogic.QueueNewRetrieval();
      }

      public void ClientsViewCachedLogClick()
      {
         // Check for SelectedClientInstance, and get out if not found
         if (SelectedClientInstance == null) return;

         string logFilePath = Path.Combine(_prefs.CacheDirectory, SelectedClientInstance.Settings.CachedFahLogName);
         if (File.Exists(logFilePath))
         {
            HandleProcessStartResult(_processStarter.ShowCachedLogFile(logFilePath));
         }
         else
         {
            string message = String.Format(CultureInfo.CurrentCulture, "The FAHlog.txt file for '{0}' does not exist.",
                                           SelectedClientInstance.Settings.InstanceName);
            _messageBoxView.ShowInformation(_view, message, _view.Text);
         }
      }

      public void ClientsViewClientFilesClick()
      {
         // Check for SelectedClientInstance, and get out if not found
         if (SelectedClientInstance == null) return;

         if (SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance))
         {
            HandleProcessStartResult(_processStarter.ShowFileExplorer(SelectedClientInstance.Settings.Path));
         }
      }
      
      #endregion

      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="settings">Client Instance Settings</param>
      private void Add(ClientInstanceSettings settings)
      {
         Debug.Assert(settings != null);

         ClientInstance instance = _instanceFactory.Create(settings);
         if (_instanceCollection.ContainsKey(instance.Settings.InstanceName))
         {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
               "Client Name '{0}' already exists.", instance.Settings.InstanceName));
         }

         // Issue 230
         bool hasInstances = _instanceCollection.HasInstances();

         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            _instanceCollection.Add(instance.Settings.InstanceName, instance);
         }
         RefreshDisplay();

         _retrievalLogic.RetrieveSingleClient(instance);

         ChangedAfterSave = true;
         AutoSaveConfig();

         // Issue 230
         if (!hasInstances)
         {
            _retrievalLogic.SetTimerState();
         }
      }

      /// <summary>
      /// Edit the ClientInstance Name and Path
      /// </summary>
      /// <param name="previousName"></param>
      /// <param name="previousPath"></param>
      /// <param name="settings">Client Instance Settings</param>
      private void Edit(string previousName, string previousPath, ClientInstanceSettings settings)
      {
         Debug.Assert(previousName != null);
         Debug.Assert(previousPath != null);
         Debug.Assert(settings != null);

         Debug.Assert(_instanceCollection.ContainsKey(previousName));

         // if the host key changed
         if (previousName != settings.InstanceName)
         {
            // check for a duplicate name
            if (_instanceCollection.ContainsKey(settings.InstanceName))
            {
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "Client Name '{0}' already exists.", settings.InstanceName));
            }
         }

         var instance = _instanceCollection[previousName];
         // load the new settings
         instance.Settings = settings;

         // Instance Name changed but isn't an already existing key
         if (previousName != instance.Settings.InstanceName)
         {
            // lock added here - 9/28/10
            lock (_instanceCollection)
            {
               // update InstanceCollection
               _instanceCollection.Remove(previousName);
               _instanceCollection.Add(instance.Settings.InstanceName, instance);
            }

            // Issue 79 - 9/28/10
            if (!instance.Settings.ExternalInstance)
            {
               // update the Names in the BenchmarkContainer
               _benchmarkContainer.UpdateInstanceName(new BenchmarkClient(previousName, instance.Settings.Path), instance.Settings.InstanceName);
            }
         }
         // the path changed
         if (!Paths.Equal(previousPath, instance.Settings.Path))
         {
            // Issue 79 - 9/28/10
            if (!instance.Settings.ExternalInstance)
            {
               // update the Paths in the BenchmarkContainer
               _benchmarkContainer.UpdateInstancePath(new BenchmarkClient(instance.Settings.InstanceName, previousPath), instance.Settings.Path);
            }
         }

         _retrievalLogic.RetrieveSingleClient(instance);

         ChangedAfterSave = true;
         AutoSaveConfig();
      }

      /// <summary>
      /// Remove an Instance by Name
      /// </summary>
      /// <param name="instanceName">Instance Name</param>
      private void Remove(string instanceName)
      {
         Debug.Assert(instanceName != null);

         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            _instanceCollection.Remove(instanceName);
            var findInstance = FindDisplayInstance(instanceName);
            if (findInstance != null)
            {
               _displayCollection.Remove(findInstance);
            }
         }
         RefreshDisplay();

         ChangedAfterSave = true;
         AutoSaveConfig();

         FindDuplicates();
      }

      /// <summary>
      /// Clear All Instance Data
      /// </summary>
      private void Clear()
      {
         // new config filename
         _configurationManager.ClearConfigFilename();
         // collection has not changed
         ChangedAfterSave = false;

         if (_instanceCollection.HasInstances())
         {
            SaveCurrentUnitInfo();
         }

         _instanceCollection.Clear();
         _displayCollection.Clear();

         // This will disable the timers, we have no hosts
         _retrievalLogic.SetTimerState();

         RefreshDisplay();
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
         if (SelectedDisplayInstance != null)
         {
            projectId = SelectedDisplayInstance.ProjectID;
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
         Debug.Assert(_view.WorkUnitHistoryMenuEnabled);
      
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
         RefreshUserStatsData(true);
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

      #region User Stats Handling Methods

      public void ShowUserStatsClick()
      {
         _prefs.SetPreference(Preference.ShowTeamStats, false);
         _view.SetStatsControlsVisible(true);
         _view.RefreshUserStatsControls(_statsData.Data);
      }

      public void ShowTeamStatsClick()
      {
         _prefs.SetPreference(Preference.ShowTeamStats, true);
         _view.SetStatsControlsVisible(true);
         _view.RefreshUserStatsControls(_statsData.Data);
      }

      private void UserStatsEnabledChanged()
      {
         var showXmlStats = _prefs.GetPreference<bool>(Preference.ShowXmlStats);
         _view.SetStatsControlsVisible(showXmlStats);
         if (showXmlStats)
         {
            RefreshUserStatsData(false);
         }
      }

      /// <summary>
      /// Refresh Stats Data from EOC
      /// </summary>
      /// <param name="forceRefresh">If true, ignore last refresh time stamps and update.</param>
      public void RefreshUserStatsData(bool forceRefresh)
      {
         _statsData.GetEocXmlData(forceRefresh);
         _view.RefreshUserStatsControls(_statsData.Data);
      }

      #endregion

      #region Other Handling Methods

      private void ApplyMessageLevelIfChanged()
      {
         var newLevel = (TraceLevel)_prefs.GetPreference<int>(Preference.MessageLevel);
         if (newLevel != TraceLevelSwitch.Instance.Level)
         {
            TraceLevelSwitch.Instance.Level = newLevel;
            HfmTrace.WriteToHfmConsole(String.Format("Debug Message Level Changed: {0}", newLevel));
         }
      }

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

      public void SetSelectedInstance(string instanceName)
      {
         lock (_instanceCollection)
         {
            var previousClient = SelectedDisplayInstance;
            if (instanceName != null)
            {
               SelectedDisplayInstance = _displayCollection.FirstOrDefault(x => x.Name == instanceName);
               SelectedClientInstance = SelectedDisplayInstance.ExternalInstanceName != null ?
                  _instanceCollection[SelectedDisplayInstance.ExternalInstanceName] : _instanceCollection[instanceName];
            }
            else
            {
               SelectedDisplayInstance = null;
               SelectedClientInstance = null;
            }

            if (previousClient != SelectedDisplayInstance)
            {
               DisplaySelectedInstance();
            }
         }
      }

      public void RefreshDisplay()
      {
         try
         {
            if (_view.DataGridView.DataSource != null)
            {
               // Freeze the SelectionChanged Event when doing currency refresh
               _view.DataGridView.FreezeSelectionChanged = true;

               if (_view.InvokeRequired)
               {
                  _view.Invoke(new MethodInvoker(RefreshDisplayCollection));
                  // sort BEFORE resetting data bindings
                  ApplySort();
                  _view.Invoke(new Action<bool>(_displayBindingSource.ResetBindings), false);
               }
               else
               {
                  RefreshDisplayCollection();
                  // sort BEFORE resetting data bindings
                  ApplySort();
                  _displayBindingSource.ResetBindings(false);
               }

               // not AFTER
               //ApplySort();

               // Unfreeze the SelectionChanged Event after refresh
               // This removes the "stuttering log" effect seen as each client is refreshed
               _view.DataGridView.FreezeSelectionChanged = false;

               _view.Invoke(new MethodInvoker(DoSetSelectedInstance));
            }

            _view.RefreshControlsWithTotalsData(GetCurrentDisplayInstanceArray().GetInstanceTotals());
         }
         // This can happen when exiting the app in the middle of a retrieval process.
         // TODO: When implementing retrieval thread cancelling, cancel the the retrieval
         // thread on shutdown before allowing the app to exit.  Should be able to remove
         // this catch at that point.
         catch (ObjectDisposedException ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }

      private void DoSetSelectedInstance()
      {
         if (_view.DataGridView.CurrentCell != null)
         {
            if (_prefs.GetPreference<bool>(Preference.MaintainSelectedClient))
            {
               SelectCurrentRowKey();
            }
            else
            {
               _view.DataGridView.Rows[_view.DataGridView.CurrentCell.RowIndex].Selected = true;
            }
            // If Mono, go ahead and set the CurrentInstance here.  Under .NET the selection
            // setter above causes this same operation, but since Mono won't fire the 
            // DataGridView.SelectionChanged Event, the result of that event needs to be
            // forced here instead.
            if (PlatformOps.IsRunningOnMono())
            {
               SetSelectedInstance(_view.GetSelectedRowInstanceName(_view.DataGridView.SelectedRows));
            }
            DisplaySelectedInstance();
         }
      }

      private void SelectCurrentRowKey()
      {
         int row = _displayBindingSource.Find("Name", _view.DataGridView.CurrentRowKey);
         if (row > -1 && row < _view.DataGridView.Rows.Count)
         {
            _displayBindingSource.Position = row;
            _view.DataGridView.Rows[row].Selected = true;
         }
      }

      /// <summary>
      /// Apply Sort to Data Grid View
      /// </summary>
      private void ApplySort()
      {
         if (_view.InvokeRequired)
         {
            _view.Invoke(new MethodInvoker(ApplySort), null);
         }
         else
         {
            _view.DataGridView.FreezeSorted = true;

            // if we have a column name and a valid sort order
            if (!(String.IsNullOrEmpty(SortColumnName)) &&
                !(SortColumnOrder.Equals(SortOrder.None)))
            {
               // check for the column
               DataGridViewColumn column = _view.DataGridView.Columns[SortColumnName];
               if (column != null)
               {
                  ListSortDirection sortDirection = SortColumnOrder.Equals(SortOrder.Ascending)
                                                       ? ListSortDirection.Ascending
                                                       : ListSortDirection.Descending;

                  _view.DataGridView.Sort(column, sortDirection);
                  _view.DataGridView.SortedColumn.HeaderCell.SortGlyphDirection = SortColumnOrder;
               }
            }

            _view.DataGridView.FreezeSorted = false;
         }
      }

      /// <summary>
      /// Refresh the Display Collection from the Instance Collection
      /// </summary>
      private void RefreshDisplayCollection()
      {
         lock (_instanceCollection)
         {
            _displayCollection.RaiseListChangedEvents = false;
            RefreshDisplayInstances();
            _displayCollection.RaiseListChangedEvents = true;
         }
         //_displayCollection.ResetBindings();
      }

      /// <summary>
      /// Call only from RefreshDisplayCollection()
      /// </summary>
      private void RefreshDisplayInstances()
      {
         _displayCollection.Clear();

         foreach (var instance in _instanceCollection.Values)
         {
            foreach (var displayInstance in instance.DisplayInstances.Values)
            {
               _displayCollection.Add(displayInstance);
            }
         }
      }

      /// <summary>
      /// Get Array Representation of Current Client Instance objects in Collection
      /// </summary>
      private ClientInstance[] GetCurrentInstanceArray()
      {
         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            return _instanceCollection.Values.ToArray();
         }
      }

      /// <summary>
      /// Get Array Representation of Current Display Instance objects in Collection
      /// </summary>
      public IDisplayInstance[] GetCurrentDisplayInstanceArray()
      {
         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            return _displayCollection.ToArray();
         }
      }

      /// <summary>
      /// Finds the DisplayInstance by Key (Instance Name)
      /// </summary>
      /// <param name="key">Instance Name</param>
      public IDisplayInstance FindDisplayInstance(string key)
      {
         return _displayCollection.FirstOrDefault(displayInstance => displayInstance.Name == key);
      }

      private void SaveCurrentUnitInfo()
      {
         // If no clients loaded, stub out
         if (!_instanceCollection.HasInstances()) return;

         _unitInfoContainer.Clear();

         lock (_instanceCollection)
         {
            foreach (ClientInstance instance in _instanceCollection.Values)
            {
               foreach (var displayInstance in instance.DisplayInstances.Values)
               {
                  // Don't save the UnitInfo object if the contained Project is Unknown
                  if (displayInstance.CurrentUnitInfo.UnitInfoData.ProjectIsUnknown() == false)
                  {
                     _unitInfoContainer.Add((UnitInfo)displayInstance.CurrentUnitInfo.UnitInfoData);
                  }
               }
            }
         }

         _unitInfoContainer.Write();
      }

      public void FindDuplicates()
      {
         // check for clients with duplicate Project (Run, Clone, Gen) or UserID
         _displayCollection.FindDuplicates();
         _view.DataGridView.Invalidate();
      }
   }
}
