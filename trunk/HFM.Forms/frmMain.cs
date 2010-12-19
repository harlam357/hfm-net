/*
 * HFM.NET - Main UI Form
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
 * Form and DataGridView save state code by Ron Dunant, modified by harlam357.
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
using HFM.Forms.Controls;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   // ReSharper disable InconsistentNaming
   public partial class frmMain : FormWrapper
   // ReSharper restore InconsistentNaming
   {
      #region Fields
      
      private static readonly string FormTitle = String.Format("HFM.NET v{0} - Beta", PlatformOps.ApplicationVersion);
      
      /// <summary>
      /// Holds the state of the window before it is hidden (minimise to tray behaviour)
      /// </summary>
      private FormWindowState _originalState;

      /// <summary>
      /// Holds current Sort Column Name
      /// </summary>
      private string _sortColumnName = String.Empty;

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      private SortOrder _sortColumnOrder = SortOrder.None;

      /// <summary>
      /// Notify Icon for frmMain
      /// </summary>
      private NotifyIcon _notifyIcon;
      
      /// <summary>
      /// Update Logic Class
      /// </summary>
      private UpdateLogic _updateLogic;

      /// <summary>
      /// Path to the update file (MSI) to run on exit
      /// </summary>
      private string _updateFilePath;
      
      /// <summary>
      /// Work Unit History Presentation
      /// </summary>
      private HistoryPresenter _historyPresenter;
      
      #region Service Interfaces
      
      private readonly IPreferenceSet _prefs;

      private readonly IMessagesView _frmMessages;

      private readonly IXmlStatsDataContainer _statsData;

      private readonly IInstanceCollection _clientInstances;

      private readonly IProteinCollection _proteinCollection;

      private readonly IProteinBenchmarkContainer _benchmarkContainer;

      private readonly IInstanceConfigurationManager _configurationManager;

      private readonly IExternalProcessStarter _processStarter;
      
      #endregion

      private readonly BindingSource _displayBindingSource;

      #endregion

      #region Constructor / Initialize

      /// <summary>
      /// Main form constructor
      /// </summary>
      public frmMain(IPreferenceSet prefs, IMessagesView messagesView, IXmlStatsDataContainer statsData,
                     IInstanceCollection instanceCollection, IProteinCollection proteinCollection, 
                     IProteinBenchmarkContainer benchmarkContainer, IInstanceConfigurationManager configurationManager,
                     IExternalProcessStarter processStarter)
      {
         _prefs = prefs;
         _frmMessages = messagesView;
         _statsData = statsData;
         _clientInstances = instanceCollection;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         _configurationManager = configurationManager;
         _processStarter = processStarter;
         _displayBindingSource = new BindingSource();

         // This call is Required by the Windows Form Designer
         InitializeComponent();

         // Set Main Form Text
         base.Text = FormTitle;
      }

      public void Initialize()
      {
         // Read the User/Team stats data from disk
         _statsData.Read();
         // Apply User Stats Enabled Selection
         UserStatsEnabledChanged();
         // Initialize the Instance Collection
         _clientInstances.Initialize();
      
         // Manually Create the Columns - Issue 41
         DataGridViewWrapper.SetupDataGridViewColumns(dataGridView1);
         // Restore Form Preferences (MUST BE DONE AFTER DataGridView Columns are Setup)
         RestoreFormPreferences();
         SetupDataGridView();

         // Read the Protein Collection from disk
         _proteinCollection.Read();
         // Give the Queue Control access to the Protein Collection
         queueControl.SetProteinCollection(_proteinCollection);

         // Read the Benchmarks from disk
         _benchmarkContainer.Read();

         SubscribeToInstanceCollectionEvents();
         SubscribeToPreferenceSetEvents();
         SubscribeToStatsLabelEvents();

         // If Mono, use the RowEnter Event (which was what 0.3.0 and prior used)
         // to set the CurrentInstance selection.  Obviously Mono doesn't fire the
         // DataGridView.SelectionChanged Event.
         if (PlatformOps.IsRunningOnMono())
         {
            HfmTrace.WriteToHfmConsole("Running on Mono...");
            dataGridView1.RowEnter += delegate
            {
               _clientInstances.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
            };
         }
      }

      private void SetupDataGridView()
      {
         // Add Column Selector
         new DataGridViewColumnSelector(dataGridView1);

         dataGridView1.AutoGenerateColumns = false;
         _displayBindingSource.DataSource = InstanceProvider.GetInstance<IDisplayInstanceCollection>();
         dataGridView1.DataSource = _displayBindingSource;
      }

      private void SubscribeToInstanceCollectionEvents()
      {
         _clientInstances.RefreshUserStatsData += delegate { RefreshUserStatsData(false); };

         // refactored events
         _clientInstances.RefreshGrid += delegate { RefreshDisplay(); };
         _clientInstances.InvalidateGrid += delegate { dataGridView1.Invalidate(); };
         _clientInstances.OfflineLastChanged += delegate { ApplySort(); };
         _clientInstances.InstanceDataChanged += InstanceDataChanged;
         _clientInstances.SelectedInstanceChanged += SelectedInstanceChanged;
      }

      private void InstanceDataChanged(object sender, EventArgs e)
      {
         if (_prefs.GetPreference<bool>(Preference.AutoSaveConfig))
         {
            mnuFileSave_Click(sender, e);
         }
      }

      private void SubscribeToPreferenceSetEvents()
      {
         _prefs.FormShowStyleSettingsChanged += PreferenceSet_FormShowStyleSettingsChanged;
         _prefs.ShowUserStatsChanged += PreferenceSetShowUserStatsChanged;
         _prefs.MessageLevelChanged += PreferenceSet_MessageLevelChanged;
         _prefs.ColorLogFileChanged += PreferenceSet_ColorLogFileChanged;
         _prefs.PpdCalculationChanged += delegate { RefreshDisplay(); };
         _prefs.DecimalPlacesChanged += delegate { RefreshDisplay(); };
         _prefs.CalculateBonusChanged += delegate { RefreshDisplay(); };
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

      #endregion
      
      #region Other Handlers

      private void StatsLabelMouseDown(object sender, MouseEventArgs e)
      {
         if (e.Button.Equals(MouseButtons.Right))
         {
            var statusLabel = (ToolStripStatusLabel)sender;
            // Issue 235
            if (PlatformOps.IsRunningOnMono())
            {
               statsContextMenuStrip.Show(statusStrip, e.X, e.Y);
            }
            else
            {
               statsContextMenuStrip.Show(statusStrip, statusLabel.Bounds.X + e.X, statusLabel.Bounds.Y + e.Y);
            }
         }
      }

      public void SecondInstanceStarted(string[] args)
      {
         if (InvokeRequired)
         {
            BeginInvoke((MethodInvoker)(() => SecondInstanceStarted(args)));
            return;
         }
      
         if (WindowState == FormWindowState.Minimized)
         {
            WindowState = _originalState;
         }
         else
         {
            if (PlatformOps.IsRunningOnMono())
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
         if (_prefs.GetPreference<bool>(Preference.RunMinimized))
         {
            WindowState = FormWindowState.Minimized;
         }
      
         string fileName = String.Empty;

         if (_prefs.GetPreference<bool>(Preference.UseDefaultConfigFile))
         {
            fileName = _prefs.GetPreference<string>(Preference.DefaultConfigFile);
         }

         //TODO: Handle command line arguments
         //if (Program.Args.Length > 0)
         //{
         //   // Filename on command line - probably from Explorer
         //   fileName = Program.Args[0];
         //}

         if (String.IsNullOrEmpty(fileName) == false)
         {
            LoadFile(fileName);
         }
         
         // Add the Index Changed Handler here after everything is shown
         dataGridView1.ColumnDisplayIndexChanged += DataGridViewColumnDisplayIndexChanged;
         // Then run it once to ensure the last column is set to Fill
         DataGridViewColumnDisplayIndexChanged(null, null);
         // Add the Splitter Moved Handler here after everything is shown - Issue 8
         splitContainer1.SplitterMoved += SplitContainerSplitterMoved;

         _notifyIcon = new NotifyIcon(components);
         
         _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
         _notifyIcon.ContextMenuStrip = notifyMenu;
         _notifyIcon.Icon = Icon;
         _notifyIcon.Text = base.Text;
         _notifyIcon.DoubleClick += NotifyIconDoubleClick;
         SetFormShowStyle();

         if (_prefs.GetPreference<bool>(Preference.StartupCheckForUpdate))
         {
            CheckForUpdate();
         }
      }
      
      private void CheckForUpdate()
      {
         CheckForUpdate(false);
      }
      
      private void CheckForUpdate(bool userInvoked)
      {
         if (_updateLogic == null) _updateLogic = new UpdateLogic(this, new MessageBoxView(), new NetworkOps(_prefs));
         if (_updateLogic.CheckInProgress == false)
         {
            HfmTrace.WriteToHfmConsole("Checking for update...");
            _updateLogic.BeginCheckForUpdate(ShowUpdate, userInvoked);
         }
      }
      
      private void ShowUpdate(ApplicationUpdate update)
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(() => ShowUpdate(update)));
            return;
         }

         var updatePresenter = new UpdatePresenter(HfmTrace.WriteToHfmConsole,
            update, new NetworkOps(_prefs).GetProxy(), Constants.ApplicationName, PlatformOps.ApplicationVersionWithRevision);
         updatePresenter.Show(this);
         HandleUpdatePresenterResults(updatePresenter);
      }
      
      private void HandleUpdatePresenterResults(UpdatePresenter presenter)
      {
         if (presenter.UpdateReady && 
             presenter.SelectedUpdate.UpdateType == 0 &&
             PlatformOps.IsRunningOnMono() == false)
         {
            string message = String.Format(CultureInfo.CurrentCulture,
                                           "{0} will install the new version when you exit the application.",
                                           Constants.ApplicationName);
            MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            _updateFilePath = presenter.LocalFilePath;
         }
      }

      private void frmMain_Resize(object sender, EventArgs e)
      {
         if (WindowState != FormWindowState.Minimized)
         {
            _originalState = WindowState;
            // ReApply Sort when restoring from the sys tray - Issue 32
            if (ShowInTaskbar == false)
            {
               ApplySort();
            }
         }
         
         SetFormShowStyle();
         
         // When the log file window (panel) is collapsed, get the split location
         // changes based on the height of Panel1 - Issue 8
         if (Visible && splitContainer1.Panel2Collapsed)
         {
            _prefs.SetPreference(Preference.FormSplitLocation, splitContainer1.Panel1.Height);
         }
      }

      private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (CanContinueDestructiveOp(sender, e) == false)
         {
            e.Cancel = true;
            return;
         }

         SaveColumnSettings();
         SaveSortColumn();

         // Save location and size data
         // RestoreBounds remembers normal position if minimized or maximized
         if (WindowState == FormWindowState.Normal)
         {
            _prefs.SetPreference(Preference.FormLocation, Location);
            _prefs.SetPreference(Preference.FormSize, Size);
         }
         else
         {
            _prefs.SetPreference(Preference.FormLocation, RestoreBounds.Location);
            _prefs.SetPreference(Preference.FormSize, RestoreBounds.Size);
         }

         _prefs.SetPreference(Preference.FormLogVisible, txtLogFile.Visible);
         _prefs.SetPreference(Preference.QueueViewerVisible, queueControl.Visible);

         // Save the data
         _prefs.Save();
         
         // Save the data on current WUs in progress
         _clientInstances.SaveCurrentUnitInfo();
         // Save the benchmark collection
         _benchmarkContainer.Write();

         CheckForAndFireUpdateProcess();

         HfmTrace.WriteToHfmConsole("----------");
         HfmTrace.WriteToHfmConsole("Exiting...");
         HfmTrace.WriteToHfmConsole(String.Empty);
      }
      
      private void CheckForAndFireUpdateProcess()
      {
         if (String.IsNullOrEmpty(_updateFilePath) == false)
         {
            HfmTrace.WriteToHfmConsole(String.Format(CultureInfo.CurrentCulture,
               "Firing update file '{0}'...", _updateFilePath));
            try
            {
               Process.Start(_updateFilePath);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               string message = String.Format(CultureInfo.CurrentCulture,
                                              "Update process failed to start with the following error:{0}{0}{1}",
                                              Environment.NewLine, ex.Message);
               MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
      }

      private void SplitContainerSplitterMoved(object sender, SplitterEventArgs e)
      {
         // When the log file window (Panel2) is visible, this event will fire.
         // Update the split location directly from the split panel control. - Issue 8
         _prefs.SetPreference(Preference.FormSplitLocation, splitContainer1.SplitterDistance);
         _prefs.Save();
      }
      
      #endregion
      
      #region Data Grid View Handlers
      
      private void DataGridViewColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
      {
         if (dataGridView1.Columns.Count == DataGridViewWrapper.NumberOfDisplayFields)
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

            SaveColumnSettings(); // Save Column Settings - Issue 73
            _prefs.Save();
         }
      }

      private void dataGridView1_SelectionChanged(object sender, EventArgs e)
      {
         _clientInstances.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
      }
      
      private void SelectedInstanceChanged(object sender, EventArgs e)
      {
         if (_clientInstances.SelectedDisplayInstance != null)
         {
            mnuClientsViewClientFiles.Visible = _clientInstances.ClientFilesMenuItemVisible;
            mnuClientsViewCachedLog.Visible = _clientInstances.CachedLogMenuItemVisible;
            mnuClientsSep4.Visible = _clientInstances.ClientFilesMenuItemVisible ||
                                     _clientInstances.CachedLogMenuItemVisible;
         
            statusLabelLeft.Text = _clientInstances.SelectedDisplayInstance.ClientPathAndArguments;
            
            queueControl.SetQueue(_clientInstances.SelectedDisplayInstance.Queue, 
               _clientInstances.SelectedDisplayInstance.TypeOfClient, 
               _clientInstances.SelectedDisplayInstance.ClientIsOnVirtualMachine);

            // if we've got a good queue read, let queueControl_QueueIndexChanged()
            // handle populating the log lines.
            ClientQueue qBase = _clientInstances.SelectedDisplayInstance.Queue;
            if (qBase != null) return;

            // otherwise, load up the CurrentLogLines
            SetLogLines(_clientInstances.SelectedDisplayInstance, _clientInstances.SelectedDisplayInstance.CurrentLogLines);
         }
         else
         {
            ClearLogAndQueueViewer();
         }
      }

      private void queueControl_QueueIndexChanged(object sender, QueueIndexChangedEventArgs e)
      {
         if (e.Index == -1)
         {
            txtLogFile.SetNoLogLines();
            return;
         }

         if (_clientInstances.SelectedDisplayInstance != null)
         {
            //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Changed Queue Index ({0} - {1})", InstanceName, e.Index));

            // Check the UnitLogLines array against the requested Queue Index - Issue 171
            try
            {
               var logLines = _clientInstances.SelectedDisplayInstance.GetLogLinesForQueueIndex(e.Index);
               if (logLines == null && e.Index == _clientInstances.SelectedDisplayInstance.Queue.CurrentIndex)
               {
                  logLines = _clientInstances.SelectedDisplayInstance.CurrentLogLines;
               }

               SetLogLines(_clientInstances.SelectedDisplayInstance, logLines);
            }
            catch (ArgumentOutOfRangeException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               txtLogFile.SetNoLogLines();
            }
         }
         else
         {
            ClearLogAndQueueViewer();
         }
      }

      private void SetLogLines(IDisplayInstance instance, IList<LogLine> logLines)
      {
         /*** Checked LogLine Count ***/
         if (logLines != null && logLines.Count > 0) 
         {
            // Different Client... Load LogLines
            if (txtLogFile.LogOwnedByInstanceName.Equals(instance.Name) == false)
            {
               txtLogFile.SetLogLines(logLines, instance.Name);
               PreferenceSet_ColorLogFileChanged(null, EventArgs.Empty);
               
               //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Set Log Lines (Changed Client - {0})", instance.InstanceName));
            }
            // Textbox has text lines
            else if (txtLogFile.Lines.Length > 0)
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
               if (txtLogFile.Lines[txtLogFile.Lines.Length - 1].Equals(lastLogLine) == false)
               {
                  txtLogFile.SetLogLines(logLines, instance.Name);
                  PreferenceSet_ColorLogFileChanged(null, EventArgs.Empty);
                  
                  //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Set Log Lines (log lines different)");
               }
            }
            // Nothing in the Textbox... Load LogLines
            else
            {
               txtLogFile.SetLogLines(logLines, instance.Name);
               PreferenceSet_ColorLogFileChanged(null, EventArgs.Empty);
            }
         }
         else
         {
            txtLogFile.SetNoLogLines();
         }

         txtLogFile.ScrollToBottom();
      }

      private void dataGridView1_Sorted(object sender, EventArgs e)
      {
         _sortColumnName = dataGridView1.SortedColumn.Name;
         _sortColumnOrder = dataGridView1.SortOrder;

         SaveSortColumn(); // Save Column Sort Order - Issue 73
         _prefs.Save();

         SelectCurrentRowKey();
      }

      private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo hti = dataGridView1.HitTest(e.X, e.Y);
         if (e.Button == MouseButtons.Right)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               if (dataGridView1.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected == false)
               {
                  dataGridView1.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected = true;
               }

               // Check for SelectedClientInstance, and get out if not found
               if (_clientInstances.SelectedClientInstance == null) return;

               mnuContextClientsViewClientFiles.Visible = _clientInstances.ClientFilesMenuItemVisible;
               mnuContextClientsViewCachedLog.Visible = _clientInstances.CachedLogMenuItemVisible;
               mnuContextClientsSep2.Visible = _clientInstances.ClientFilesMenuItemVisible ||
                                               _clientInstances.CachedLogMenuItemVisible;
               
               gridContextMenuStrip.Show(dataGridView1.PointToScreen(new Point(e.X, e.Y)));
            }
         }
         if (e.Button == MouseButtons.Left && e.Clicks == 2)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               // Check for SelectedClientInstance, and get out if not found
               if (_clientInstances.SelectedClientInstance == null) return;

               if (_clientInstances.SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance))
               {
                  HandleProcessStartResult(_processStarter.ShowFileExplorer(_clientInstances.SelectedClientInstance.Settings.Path));
               }
            }
         }
      }
      
      #endregion

      #region File Menu Click Handlers
      
      private void mnuFileNew_Click(object sender, EventArgs e)
      {
         if (_clientInstances.RetrievalInProgress)
         {
            MessageBox.Show(this, "Retrieval in progress... please wait to create a new config file.", 
               Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (CanContinueDestructiveOp(sender, e))
         {
            _clientInstances.Clear();
         }
      }

      private void mnuFileOpen_Click(object sender, EventArgs e)
      {
         if (_clientInstances.RetrievalInProgress)
         {
            MessageBox.Show(this, "Retrieval in progress... please wait to open another config file.",
               Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (CanContinueDestructiveOp(sender, e))
         {
            openConfigDialog.DefaultExt = _configurationManager.ConfigFileExtension;
            openConfigDialog.Filter = _configurationManager.FileTypeFilters;
            openConfigDialog.FileName = _configurationManager.ConfigFilename;
            openConfigDialog.RestoreDirectory = true;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
            {
               LoadFile(openConfigDialog.FileName, openConfigDialog.FilterIndex);
            }
         }
      }

      private void mnuFileSave_Click(object sender, EventArgs e)
      {
         if (_configurationManager.HasConfigFilename == false)
         {
            mnuFileSaveas_Click(sender, e);
         }
         else
         {
            try
            {
               _clientInstances.WriteConfigFile();
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
                  Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
      }

      private void mnuFileSaveas_Click(object sender, EventArgs e)
      {
         // No Config File and no Instances, stub out
         if (_configurationManager.HasConfigFilename == false && _clientInstances.HasInstances == false) return;
      
         saveConfigDialog.DefaultExt = _configurationManager.ConfigFileExtension;
         saveConfigDialog.Filter = _configurationManager.FileTypeFilters;
         if (saveConfigDialog.ShowDialog() == DialogResult.OK)
         {
            try
            {
               _clientInstances.WriteConfigFile(saveConfigDialog.FileName, saveConfigDialog.FilterIndex); // Issue 75
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
                  Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
      }

      private void mnuFileQuit_Click(object sender, EventArgs e)
      {
         // CanContinueDestructiveOp code moved into frmMainNew_FormClosing() - Issue 1
         Close();
      }

      #endregion

      #region Edit Menu Click Handlers
      /// <summary>
      /// Displays and handles the User Preferences dialog
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuEditPreferences_Click(object sender, EventArgs e)
      {
         var prefDialog = new frmPreferences(_prefs);
         prefDialog.ShowDialog();
         
         dataGridView1.Invalidate();
      }
      #endregion

      #region Help Menu Click Handlers

      private void mnuHelpHfmLogFile_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowHfmLogFile());
      }

      private void mnuHelpHfmDataFiles_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowFileExplorer(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath)));
      }

      private void mnuHelpHfmGroup_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowHfmGoogleGroup());
      }
      
      private void mnuHelpCheckForUpdate_Click(object sender, EventArgs e)
      {
         CheckForUpdate(true);
      }
      
      private void mnuHelpAbout_Click(object sender, EventArgs e)
      {
         var newAbout = new frmAbout();
         newAbout.ShowDialog(this);
      }

      private void mnuHelpContents_Click(object sender, EventArgs e)
      {
         //Help.ShowHelp(this, helpProvider.HelpNamespace);
      }

      private void mnuHelpIndex_Click(object sender, EventArgs e)
      {
         //Help.ShowHelpIndex(this, helpProvider.HelpNamespace);
      }

      #endregion

      #region Clients Menu Click Handlers
      
      private void mnuClientsAdd_Click(object sender, EventArgs e)
      {
         var settings = new ClientInstanceSettings();
         var newHost = InstanceProvider.GetInstance<InstanceSettingsPresenter>();
         newHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (newHost.ShowDialog(this).Equals(DialogResult.OK))
         {
            try
            {
               _clientInstances.Add(newHost.SettingsModel.Settings);
               break;
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
         }
      }

      private void mnuClientsEdit_Click(object sender, EventArgs e)
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_clientInstances.SelectedClientInstance == null) return;

         var settings = _clientInstances.SelectedClientInstance.Settings.DeepCopy();
         string previousName = settings.InstanceName;
         string previousPath = settings.Path;
         var editHost = InstanceProvider.GetInstance<InstanceSettingsPresenter>();
         editHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (editHost.ShowDialog(this).Equals(DialogResult.OK))
         {
            try
            {
               _clientInstances.Edit(previousName, previousPath, editHost.SettingsModel.Settings);
               break;
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
         }
      }

      private void mnuClientsDelete_Click(object sender, EventArgs e)
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_clientInstances.SelectedClientInstance == null) return;

         _clientInstances.Remove(_clientInstances.SelectedClientInstance.Settings.InstanceName);
      }

      private void mnuClientsMerge_Click(object sender, EventArgs e)
      {
         var settings = new ClientInstanceSettings { ExternalInstance = true };
         var newHost = InstanceProvider.GetInstance<InstanceSettingsPresenter>();
         newHost.SettingsModel = new ClientInstanceSettingsModel(settings);
         while (newHost.ShowDialog(this).Equals(DialogResult.OK))
         {
            try
            {
               _clientInstances.Add(newHost.SettingsModel.Settings);
               break;
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
      }

      private void mnuClientsRefreshSelected_Click(object sender, EventArgs e)
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_clientInstances.SelectedClientInstance == null) return;

         _clientInstances.RetrieveSingleClient(_clientInstances.SelectedClientInstance.Settings.InstanceName);
      }

      private void mnuClientsRefreshAll_Click(object sender, EventArgs e)
      {
         _clientInstances.QueueNewRetrieval();
      }

      private void mnuClientsViewCachedLog_Click(object sender, EventArgs e)
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_clientInstances.SelectedClientInstance == null) return;

         string logFilePath = Path.Combine(_prefs.CacheDirectory, _clientInstances.SelectedClientInstance.Settings.CachedFahLogName);
         if (File.Exists(logFilePath))
         {
            HandleProcessStartResult(_processStarter.ShowCachedLogFile(logFilePath));
         }
         else
         {
            string message = String.Format(CultureInfo.CurrentCulture, "The FAHlog.txt file for '{0}' does not exist.",
                                           _clientInstances.SelectedClientInstance.Settings.InstanceName);
            MessageBox.Show(this, message, PlatformOps.ApplicationNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
      }

      private void mnuClientsViewClientFiles_Click(object sender, EventArgs e)
      {
         // Check for SelectedClientInstance, and get out if not found
         if (_clientInstances.SelectedClientInstance == null) return;

         if (_clientInstances.SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance))
         {
            HandleProcessStartResult(_processStarter.ShowFileExplorer(_clientInstances.SelectedClientInstance.Settings.Path));
         }
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
         if (_frmMessages.Visible)
         {
            _frmMessages.Close();
         }
         else
         {
            // Restore state data
            var location = _prefs.GetPreference<Point>(Preference.MessagesFormLocation);
            var size = _prefs.GetPreference<Size>(Preference.MessagesFormSize);

            if (location.X != 0 && location.Y != 0)
            {
               _frmMessages.SetManualStartPosition();
               _frmMessages.SetLocation(location.X, location.Y);
            }

            if (size.Width != 0 && size.Height != 0)
            {
               _frmMessages.SetSize(size.Width, size.Height);
            }

            _frmMessages.Show();
            _frmMessages.ScrollToEnd();
         }
      }
      
      /// <summary>
      /// Show or Hide the FAH Log Window
      /// </summary>
      private void mnuViewShowHideLog_Click(object sender, EventArgs e)
      {
         ShowHideLog(!txtLogFile.Visible);
      }

      /// <summary>
      /// Show or Hide the Queue Viewer
      /// </summary>
      private void btnQueue_Click(object sender, EventArgs e)
      {
         ShowHideQueue(!queueControl.Visible);
      }
      
      private void ShowHideQueue(bool show)
      {
         if (show == false)
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

      /// <summary>
      /// Toggle the Date/Time Style
      /// </summary>
      private void mnuViewToggleDateTime_Click(object sender, EventArgs e)
      {
         if (_prefs.GetPreference<TimeStyleType>(Preference.TimeStyle).Equals(TimeStyleType.Standard))
         {
            _prefs.SetPreference(Preference.TimeStyle, TimeStyleType.Formatted);
         }
         else
         {
            _prefs.SetPreference(Preference.TimeStyle, TimeStyleType.Standard);
         }

         dataGridView1.Invalidate();
      }

      private void mnuViewToggleCompletedCountStyle_Click(object sender, EventArgs e)
      {
         if (_prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay).Equals(CompletedCountDisplayType.ClientTotal))
         {
            _prefs.SetPreference(Preference.CompletedCountDisplay, CompletedCountDisplayType.ClientRunTotal);
         }
         else
         {
            _prefs.SetPreference(Preference.CompletedCountDisplay, CompletedCountDisplayType.ClientTotal);
         }

         dataGridView1.Invalidate();
      }

      private void mnuViewToggleVersionInformation_Click(object sender, EventArgs e)
      {
         _prefs.SetPreference(Preference.ShowVersions, !_prefs.GetPreference<bool>(Preference.ShowVersions));
         dataGridView1.Invalidate();
      }

      private void mnuViewToggleBonusCalculation_Click(object sender, EventArgs e)
      {
         bool value = !_prefs.GetPreference<bool>(Preference.CalculateBonus);
         _prefs.SetPreference(Preference.CalculateBonus, value);
         toolTipNotify.Show(value ? "Bonus On" : "Bonus Off", this, Size.Width - 150, 8, 2000);
         dataGridView1.Invalidate();
      }

      private void mnuViewCycleCalculation_Click(object sender, EventArgs e)
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
         string calculationTypeString = (from item in Models.OptionsModel.PpdCalculationList
                                         where ((PpdCalculationType)item.ValueMember) == calculationType
                                         select item.DisplayMember).First();
         toolTipNotify.Show(calculationTypeString, this, Size.Width - 150, 8, 2000);
         dataGridView1.Invalidate();
      }
      
      #endregion

      #region Tools Menu Click Handlers
      /// <summary>
      /// Download Project Info From Stanford
      /// </summary>
      private void mnuToolsDownloadProjects_Click(object sender, EventArgs e)
      {
         // Clear the Project Not Found Cache and Last Download Time
         _proteinCollection.ClearProjectsNotFoundCache();
         _proteinCollection.Downloader.ResetLastDownloadTime();
         // Execute Asynchronous Download
         var projectDownloadView = InstanceProvider.GetInstance<IProgressDialogView>("projectDownloadView");
         projectDownloadView.OwnerWindow = this;
         projectDownloadView.ProcessRunner = _proteinCollection.Downloader;
         projectDownloadView.UpdateMessage(_proteinCollection.Downloader.Prefs.GetPreference<string>(Preference.ProjectDownloadUrl));
         projectDownloadView.Process();
      }

      /// <summary>
      /// Show the Benchmarks Dialog
      /// </summary>
      private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
      {
         int projectId = 0;
      
         // Check for SelectedDisplayInstance, and if found... load its ProjectID.
         if (_clientInstances.SelectedDisplayInstance != null)
         {
            projectId = _clientInstances.SelectedDisplayInstance.ProjectID;
         }

         var frm = new frmBenchmarks(_prefs, _proteinCollection, _benchmarkContainer, _clientInstances, projectId);
         frm.StartPosition = FormStartPosition.Manual;

         // Restore state data
         var location = _prefs.GetPreference<Point>(Preference.BenchmarksFormLocation);
         var size = _prefs.GetPreference<Size>(Preference.BenchmarksFormSize);

         if (location.X != 0 && location.Y != 0)
         {
            frm.Location = location;
         }
         else
         {
            frm.Location = new Point(Location.X + 50, Location.Y + 50);
         }
         
         if (size.Width != 0 && size.Height != 0)
         {
            frm.Size = size;
         }
         
         frm.Show();
      }

      private void mnuToolsHistory_Click(object sender, EventArgs e)
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

      #region Web Menu Click Handlers

      private void mnuWebEOCUser_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowEocUserPage());
      }

      private void mnuWebStanfordUser_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowStanfordUserPage());
      }

      private void mnuWebEOCTeam_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowEocTeamPage());
      }

      private void mnuWebRefreshUserStats_Click(object sender, EventArgs e)
      {
         RefreshUserStatsData(true);
      }

      private void mnuWebHFMGoogleCode_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowHfmGoogleCode());
      }

      #endregion

      #region Background Work Routines
      /// <summary>
      /// Refresh the Form
      /// </summary>
      private void RefreshDisplay()
      {
         try 
         {
            //_clientInstances.RefreshDisplayCollection();
            if (dataGridView1.DataSource != null)
            {
               // Freeze the SelectionChanged Event when doing currency refresh
               dataGridView1.FreezeSelectionChanged = true;
            
               //var cm = (CurrencyManager)dataGridView1.BindingContext[dataGridView1.DataSource];
               //if (cm != null)
               //{
               //   if (InvokeRequired)
               //   {
               //      Invoke(new MethodInvoker(cm.Refresh));
               //   }
               //   else
               //   {
               //      cm.Refresh();
               //   }
               //}

               if (InvokeRequired)
               {
                  Invoke(new MethodInvoker(_clientInstances.RefreshDisplayCollection));
               }
               else
               {
                  _clientInstances.RefreshDisplayCollection();
               }

               ApplySort();

               // Unfreeze the SelectionChanged Event after refresh
               // This removes the "stuttering log" effect seen as each client is refreshed
               dataGridView1.FreezeSelectionChanged = false;
               
               Invoke(new MethodInvoker(DoSetSelectedInstance));
            }

            RefreshControls();
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
         if (dataGridView1.CurrentCell != null)
         {
            if (_prefs.GetPreference<bool>(Preference.MaintainSelectedClient))
            {
               SelectCurrentRowKey();
            }
            else
            {
               dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Selected = true;
            }
            // If Mono, go ahead and set the CurrentInstance here.  Under .NET the selection
            // setter above causes this same operation, but since Mono won't fire the 
            // DataGridView.SelectionChanged Event, the result of that event needs to be
            // forced here instead.
            if (PlatformOps.IsRunningOnMono())
            {
               _clientInstances.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
            }
            _clientInstances.RaiseSelectedInstanceChanged();
         }
      }
      
      private static string GetSelectedRowInstanceName(System.Collections.IList selectedRows)
      {
         if (selectedRows.Count > 0)
         {
            Debug.Assert(selectedRows.Count == 1);

            var selectedClient = selectedRows[0] as DataGridViewRow;
            if (selectedClient != null)
            {
               var nameColumnValue = selectedClient.Cells["Name"].Value;
               if (nameColumnValue != null)
               {
                  return nameColumnValue.ToString();
               }
            }
         }

         return null;
      }
      
      private void SelectCurrentRowKey()
      {
         int row = _displayBindingSource.Find("Name", dataGridView1.CurrentRowKey);
         if (row > -1 && row < dataGridView1.Rows.Count)
         {
            _displayBindingSource.Position = row;
            dataGridView1.Rows[row].Selected = true;
         }
      }

      /// <summary>
      /// Apply Sort to Data Grid View
      /// </summary>
      private void ApplySort()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(ApplySort), null);
         }
         else
         {
            dataGridView1.FreezeSorted = true;
            
            // if we have a column name and a valid sort order
            if (!(String.IsNullOrEmpty(_sortColumnName)) && 
                !(_sortColumnOrder.Equals(SortOrder.None)))
            {
               // check for the column
               DataGridViewColumn column = dataGridView1.Columns[_sortColumnName];
               if (column != null)
               {
                  ListSortDirection sortDirection = _sortColumnOrder.Equals(SortOrder.Ascending)
                                                       ? ListSortDirection.Ascending
                                                       : ListSortDirection.Descending;

                  dataGridView1.Sort(column, sortDirection);
                  dataGridView1.SortedColumn.HeaderCell.SortGlyphDirection = _sortColumnOrder;
               }
            }

            dataGridView1.FreezeSorted = false;
         }
      }

      /// <summary>
      /// Refresh remaining UI controls with current data
      /// </summary>
      private void RefreshControls()
      {
         InstanceTotals totals = _clientInstances.GetInstanceTotals();

         double totalPPD = totals.PPD;
         int goodHosts = totals.WorkingClients;

         SetNotifyIconText(String.Format("{0} Working Clients{3}{1} Non-Working Clients{3}{2:" + _prefs.PpdFormatString + "} PPD",
                                         goodHosts, totals.NonWorkingClients, totalPPD, Environment.NewLine));
         RefreshStatusLabels(goodHosts, totalPPD);
      }

      /// <summary>
      /// Delegate used for Post-Message Invoke to UI Thread
      /// </summary>
      /// <param name="val">String Value</param>
      private delegate void SimpleVoidStringDelegate(string val);
      
      /// <summary>
      /// Sets the Notify Icon Text (Thread Safe)
      /// </summary>
      /// <param name="val">Text to set</param>
      private void SetNotifyIconText(string val)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new SimpleVoidStringDelegate(SetNotifyIconText), new object[] { val });
            return;
         }
         
         // make sure the object has been created
         if (_notifyIcon != null)
         {
            if (val.Length > 64)
            {
               //if string is too long, remove the word Clients
               val = val.Replace("Clients", String.Empty);
            }
            _notifyIcon.Text = val;
         }
      }

      /// <summary>
      /// Update the Status Labels with Client and PPD info
      /// </summary>
      private void RefreshStatusLabels(int goodHosts, double totalPPD)
      {
         if (goodHosts == 1)
         {
            SetStatusLabelHostsText(String.Format("{0} Client", goodHosts));
         }
         else
         {
            SetStatusLabelHostsText(String.Format("{0} Clients", goodHosts));
         }

         SetStatusLabelPPDText(String.Format("{0:" + _prefs.PpdFormatString + "} PPD", totalPPD));
      }

      /// <summary>
      /// Sets the Hosts Status Label Text (Thread Safe)
      /// </summary>
      /// <param name="val">Text to set</param>
      private void SetStatusLabelHostsText(string val)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new SimpleVoidStringDelegate(SetStatusLabelHostsText), new object[] { val });
            return;
         }
         
         statusLabelHosts.Text = val;
      }

      /// <summary>
      /// Sets the PPD Status Label Text (Thread Safe)
      /// </summary>
      /// <param name="val">Text to set</param>
      private void SetStatusLabelPPDText(string val)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new SimpleVoidStringDelegate(SetStatusLabelPPDText), new object[] { val });
            return;
         }
         
         statusLabelPPW.Text = val;
      }
      #endregion

      #region System Tray Icon Routines
      /// <summary>
      /// Action of the double-clicked notification icon (min/restore)
      /// </summary>
      private void NotifyIconDoubleClick(object sender, EventArgs e)
      {
         if (WindowState == FormWindowState.Minimized)
         {
            WindowState = _originalState;
         }
         else
         {
            _originalState = WindowState;
            WindowState = FormWindowState.Minimized;
         }
      }

      /// <summary>
      /// Action of the clicked notification icon restore option
      /// </summary>
      private void mnuNotifyRst_Click(object sender, EventArgs e)
      {
         if (WindowState == FormWindowState.Minimized)
         {
            WindowState = _originalState;
         }
         else if (WindowState == FormWindowState.Maximized)
         {
            WindowState = FormWindowState.Normal;
         }
      }

      /// <summary>
      /// Action of the clicked notification icon minimize option
      /// </summary>
      private void mnuNotifyMin_Click(object sender, EventArgs e)
      {
         if (WindowState != FormWindowState.Minimized)
         {
            _originalState = WindowState;
            WindowState = FormWindowState.Minimized;
         }
      }

      /// <summary>
      /// Action of the clicked notification icon maximize option
      /// </summary>
      private void mnuNotifyMax_Click(object sender, EventArgs e)
      {
         if (WindowState != FormWindowState.Maximized)
         {
            WindowState = FormWindowState.Maximized;
            _originalState = WindowState;
         }
      }
      #endregion

      #region Helper Routines
      
      /// <summary>
      /// Restore Form State
      /// </summary>
      private void RestoreFormPreferences()
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
            StartPosition = FormStartPosition.Manual;
            Location = location;
         }
         if (size.Width != 0 && size.Height != 0)
         {
            if (_prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
            {
               size = new Size(size.Width, size.Height + _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            }
            Size = size;
            splitContainer1.SplitterDistance = _prefs.GetPreference<int>(Preference.FormSplitLocation);
         }

         if (_prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
         {
            ShowHideLog(false);
         }

         if (_prefs.GetPreference<bool>(Preference.QueueViewerVisible) == false)
         {
            ShowHideQueue(false);
         }

         //if (Prefs.FormSortColumn != String.Empty &&
         //    Prefs.FormSortOrder != SortOrder.None)
         //{
            _sortColumnName = _prefs.GetPreference<string>(Preference.FormSortColumn);
            _sortColumnOrder = _prefs.GetPreference<SortOrder>(Preference.FormSortOrder);
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
               dataGridView1.Columns[index].DisplayIndex = Int16.Parse(a[0]);
               if (dataGridView1.Columns[index].AutoSizeMode.Equals(DataGridViewAutoSizeColumnMode.Fill) == false)
               {
                  dataGridView1.Columns[index].Width = Int16.Parse(a[1]);
               }
               dataGridView1.Columns[index].Visible = bool.Parse(a[2]);
            }
         }
         catch (NullReferenceException)
         {
            // This happens when the FormColumns setting is empty
         }
      }

      /// <summary>
      /// Save Column Index, Width, and Visibility
      /// </summary>
      private void SaveColumnSettings()
      {
         // Save column state data
         // including order, column width and whether or not the column is visible
         var stringCollection = new StringCollection();
         int i = 0;

         foreach (DataGridViewColumn column in dataGridView1.Columns)
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

      /// <summary>
      /// Save Sorted Column Name and Order
      /// </summary>
      private void SaveSortColumn()
      {
         _prefs.SetPreference(Preference.FormSortColumn, _sortColumnName);
         _prefs.SetPreference(Preference.FormSortOrder, _sortColumnOrder);
      }

      /// <summary>
      /// Test current application status for changes; ask for confirmation if necessary.
      /// </summary>
      private bool CanContinueDestructiveOp(object sender, EventArgs e)
      {
         if (_clientInstances.ChangedAfterSave)
         {
            DialogResult qResult = MessageBox.Show(this, String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes / No - Continue and do not save the changes / Cancel - Do not continue", Environment.NewLine), base.Text, MessageBoxButtons.YesNoCancel);
            switch (qResult)
            {
               case DialogResult.Yes:
                  mnuFileSave_Click(sender, e);
                  return !_clientInstances.ChangedAfterSave;
               case DialogResult.No:
                  return true;
               case DialogResult.Cancel:
                  return false;
            }
            return false;
         }
         
         return true;
      }

      /// <summary>
      /// Clear the Log File and Queue Viewers
      /// </summary>
      private void ClearLogAndQueueViewer()
      {
         // clear the log text
         txtLogFile.SetNoLogLines();
         // clear the queue control
         queueControl.SetQueue(null);
      }

      /// <summary>
      /// Loads a configuration file into memory
      /// </summary>
      /// <param name="filePath">File to load</param>
      private void LoadFile(string filePath)
      {
         LoadFile(filePath, 1);
      }

      /// <summary>
      /// Loads a configuration file into memory
      /// </summary>
      /// <param name="filePath">File to load</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      private void LoadFile(string filePath, int filterIndex)
      {
         try
         {
            // Read the config file
            _clientInstances.ReadConfigFile(filePath, filterIndex);

            if (_clientInstances.HasInstances == false)
            {
               MessageBox.Show(this, "No client configurations were loaded from the given config file.",
                  Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
               "No client configurations were loaded from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message),
               Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      /// <summary>
      /// Show or Hide the Log File Control
      /// </summary>
      /// <param name="show"></param>
      private void ShowHideLog(bool show)
      {
         if (show == false)
         {
            txtLogFile.Visible = false;
            splitContainer1.Panel2Collapsed = true;
            _prefs.SetPreference(Preference.FormLogWindowHeight, (splitContainer1.Height - splitContainer1.SplitterDistance));
            Size = new Size(Size.Width, Size.Height - _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
         }
         else
         {
            txtLogFile.Visible = true;
            Resize -= frmMain_Resize; // disable Form resize event for this operation
            Size = new Size(Size.Width, Size.Height + _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            Resize += frmMain_Resize; // re-enable
            splitContainer1.Panel2Collapsed = false;
         }
      }

      private void HandleProcessStartResult(string message)
      {
         if (message != null)
         {
            MessageBox.Show(this, message, PlatformOps.ApplicationNameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      #region User Stats Data Methods
      
      private void RefreshUserStatsData(bool forceRefresh)
      {
         _statsData.GetEocXmlData(forceRefresh);
         RefreshUserStatsControls();
      }
      
      /// <summary>
      /// Refresh User Stats from Xml Data Source
      /// </summary>
      private void RefreshUserStatsControls()
      {
         try
         {
            if (_prefs.GetPreference<bool>(Preference.ShowTeamStats))
            {
               statusUserTeamRank.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Team: ", Constants.EocStatsFormat), _statsData.Data.TeamRank);
               //statusUserProjectRank.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Project: ", Constants.EocStatsFormat), _statsData.Data.UserOverallRank);
               statusUser24hr.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("24hr: ", Constants.EocStatsFormat), _statsData.Data.TeamTwentyFourHourAvgerage);
               statusUserToday.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Today: ", Constants.EocStatsFormat), _statsData.Data.TeamPointsToday);
               statusUserWeek.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Week: ", Constants.EocStatsFormat), _statsData.Data.TeamPointsWeek);
               statusUserTotal.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Total: ", Constants.EocStatsFormat), _statsData.Data.TeamPointsTotal);
               statusUserWUs.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("WUs: ", Constants.EocStatsFormat), _statsData.Data.TeamWorkUnitsTotal);
            }
            else
            {
               statusUserTeamRank.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Team: ", Constants.EocStatsFormat), _statsData.Data.UserTeamRank);
               statusUserProjectRank.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Project: ", Constants.EocStatsFormat), _statsData.Data.UserOverallRank);
               statusUser24hr.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("24hr: ", Constants.EocStatsFormat), _statsData.Data.UserTwentyFourHourAvgerage);
               statusUserToday.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Today: ", Constants.EocStatsFormat), _statsData.Data.UserPointsToday);
               statusUserWeek.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Week: ", Constants.EocStatsFormat), _statsData.Data.UserPointsWeek);
               statusUserTotal.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Total: ", Constants.EocStatsFormat), _statsData.Data.UserPointsTotal);
               statusUserWUs.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("WUs: ", Constants.EocStatsFormat), _statsData.Data.UserWorkUnitsTotal);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }
      
      #endregion

      #region PreferenceSet Event Handlers
      private void PreferenceSet_FormShowStyleSettingsChanged(object sender, EventArgs e)
      {
         SetFormShowStyle();
      }

      private void SetFormShowStyle()
      {
         switch (_prefs.GetPreference<FormShowStyleType>(Preference.FormShowStyle))
         {
            case FormShowStyleType.SystemTray:
               if (_notifyIcon != null) _notifyIcon.Visible = true;
               ShowInTaskbar = (WindowState != FormWindowState.Minimized);
               break;
            case FormShowStyleType.TaskBar:
               if (_notifyIcon != null) _notifyIcon.Visible = false;
               ShowInTaskbar = true;
               break;
            case FormShowStyleType.Both:
               if (_notifyIcon != null) _notifyIcon.Visible = true;
               ShowInTaskbar = true;
               break;
         }
      }

      /// <summary>
      /// User Stats Preference Changed
      /// </summary>
      private void PreferenceSetShowUserStatsChanged(object sender, EventArgs e)
      {
         UserStatsEnabledChanged();
      }

      /// <summary>
      /// Show or Hide User Stats Controls based on user setting
      /// </summary>
      private void UserStatsEnabledChanged()
      {
         var showXmlStats = _prefs.GetPreference<bool>(Preference.ShowXmlStats);
         SetStatsControlsVisible(showXmlStats);
         if (showXmlStats)
         {
            RefreshUserStatsData(false);
         }
      }

      private void mnuContextShowUserStats_Click(object sender, EventArgs e)
      {
         _prefs.SetPreference(Preference.ShowTeamStats, false);
         SetStatsControlsVisible(true);
         RefreshUserStatsControls();
      }
      
      private void mnuContextShowTeamStats_Click(object sender, EventArgs e)
      {
         _prefs.SetPreference(Preference.ShowTeamStats, true);
         SetStatsControlsVisible(true);
         RefreshUserStatsControls();
      }
      
      private void SetStatsControlsVisible(bool visible)
      {
         mnuWebRefreshUserStats.Visible = visible;
         mnuWebSep2.Visible = visible;
         statusUserTeamRank.Visible = visible;
         statusUserProjectRank.Visible = !_prefs.GetPreference<bool>(Preference.ShowTeamStats) && visible;
         statusUser24hr.Visible = visible;
         statusUserToday.Visible = visible;
         statusUserWeek.Visible = visible;
         statusUserTotal.Visible = visible;
         statusUserWUs.Visible = visible;
         statusLabelMiddle.Visible = visible;
      }

      /// <summary>
      /// Sets Debug Message Level on Trace Level Switch
      /// </summary>
      private void PreferenceSet_MessageLevelChanged(object sender, EventArgs e)
      {
         var newLevel = (TraceLevel)_prefs.GetPreference<int>(Preference.MessageLevel);
         if (newLevel != TraceLevelSwitch.Instance.Level)
         {
            TraceLevelSwitch.Instance.Level = newLevel;
            HfmTrace.WriteToHfmConsole(String.Format("Debug Message Level Changed: {0}", newLevel));
         }
      }

      /// <summary>
      /// Sets the Log Color option after change
      /// </summary>
      private void PreferenceSet_ColorLogFileChanged(object sender, EventArgs e)
      {
         if (_prefs.GetPreference<bool>(Preference.ColorLogFile))
         {
            txtLogFile.HighlightLines();
         }
         else
         {
            txtLogFile.RemoveHighlight();
         }
      }
      #endregion
      
      #endregion
   }
}
