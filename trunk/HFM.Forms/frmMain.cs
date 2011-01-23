/*
 * HFM.NET - Main UI Form
 * Copyright (C) 2006-2007 David Rawling
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
 * Form and DataGridView save state code by Ron Dunant, modified by harlam357.
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
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Forms.Models;
using HFM.Forms.Controls;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   public interface IMainView : IWin32Window
   {
      FormWindowState WindowState { get; set; }

      bool ShowInTaskbar { get; set; }

      Icon Icon { get; }

      string Text { get; set; }

      bool Visible { get; set; }

      Point Location { get; set; }

      FormStartPosition StartPosition { get; set; }

      Size Size { get; set; }

      Rectangle RestoreBounds { get; }

      bool LogFileVisible { get; }

      bool QueueVisible { get; }

      DataGridView DataGridView { get; }

      SplitContainer SplitContainer { get; }

      NotifyIcon NotifyIcon { get; }

      ContextMenuStrip NotifyMenu { get; }

      void Initialize(MainPresenter presenter);
      
      //TODO: Move these methods to presenter
      void ApplySort();

      void SelectCurrentRowKey();

      void ShowHideLogWindow(bool show);

      void ShowHideQueue(bool show);
   }

   // ReSharper disable InconsistentNaming
   public partial class frmMain : FormWrapper, IMainView
   {
      #region Fields
      
      private static readonly string FormTitle = String.Format("HFM.NET v{0} - Beta", PlatformOps.ApplicationVersion);

      private MainPresenter _presenter;
      
      public bool LogFileVisible
      {
         get { return txtLogFile.Visible; }
      }
      
      public bool QueueVisible
      {
         get { return queueControl.Visible; }
      }

      public DataGridView DataGridView { get { return dataGridView1; } }
      
      public SplitContainer SplitContainer { get { return splitContainer1; } }
      
      public NotifyIcon NotifyIcon { get; private set; }

      public ContextMenuStrip NotifyMenu { get { return notifyMenu; } }
      
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

      private readonly IExternalProcessStarter _processStarter;

      private readonly IUpdateLogic _updateLogic;
      
      #endregion

      private readonly BindingSource _displayBindingSource;

      #endregion

      #region Constructor / Initialize

      /// <summary>
      /// Main form constructor
      /// </summary>
      public frmMain(IPreferenceSet prefs, IMessagesView messagesView, IXmlStatsDataContainer statsData,
                     IInstanceCollection instanceCollection, IProteinCollection proteinCollection, 
                     IProteinBenchmarkContainer benchmarkContainer, 
                     IExternalProcessStarter processStarter, IUpdateLogic updateLogic)
      {
         _prefs = prefs;
         _frmMessages = messagesView;
         _statsData = statsData;
         _clientInstances = instanceCollection;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         _processStarter = processStarter;
         _updateLogic = updateLogic;
         _updateLogic.Owner = this;
         _displayBindingSource = new BindingSource();

         // This call is Required by the Windows Form Designer
         InitializeComponent();

         // Set Main Form Text
         base.Text = FormTitle;
      }

      public void Initialize(MainPresenter presenter)
      {
         _presenter = presenter;
      
         // Manually Create the Columns - Issue 41
         DataGridViewWrapper.SetupDataGridViewColumns(dataGridView1);
         // Restore Form Preferences (MUST BE DONE AFTER DataGridView Columns are Setup)
         _presenter.RestoreFormPreferences();
         SetupDataGridView();

         // Give the Queue Control access to the Protein Collection
         queueControl.SetProteinCollection(_proteinCollection);

         SubscribeToInstanceCollectionEvents();
         SubscribeToPreferenceSetEvents();
         SubscribeToStatsLabelEvents();

         // Apply User Stats Enabled Selection
         UserStatsEnabledChanged();

         // If Mono, use the RowEnter Event (which was what 0.3.0 and prior used)
         // to set the CurrentInstance selection.  Obviously Mono doesn't fire the
         // DataGridView.SelectionChanged Event.
         if (PlatformOps.IsRunningOnMono())
         {
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
         // refactored events
         _clientInstances.RefreshGrid += delegate { RefreshDisplay(); };
         _clientInstances.InvalidateGrid += delegate { dataGridView1.Invalidate(); };
         _clientInstances.OfflineLastChanged += delegate { ApplySort(); };
         _clientInstances.InstanceDataChanged += InstanceDataChanged;
         _clientInstances.SelectedInstanceChanged += SelectedInstanceChanged;
         _clientInstances.RefreshUserStatsControls += delegate { RefreshUserStatsControls(); };
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
         _prefs.FormShowStyleSettingsChanged += delegate { _presenter.SetViewShowStyle(); };
         _prefs.ShowUserStatsChanged += delegate { UserStatsEnabledChanged(); };
         _prefs.MessageLevelChanged += MessageLevelChanged;
         _prefs.ColorLogFileChanged += ColorLogFileChanged;
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

      //TODO: Move to Presenter
      public void SecondInstanceStarted(string[] args)
      {
         if (InvokeRequired)
         {
            BeginInvoke((MethodInvoker)(() => SecondInstanceStarted(args)));
            return;
         }
      
         if (WindowState == FormWindowState.Minimized)
         {
            WindowState = _presenter.OriginalState;
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
         NotifyIcon = new NotifyIcon(components);
         
         _presenter.ViewShown();
      }

      private void frmMain_Resize(object sender, EventArgs e)
      {
         _presenter.ViewResize();
      }

      private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
      {
         e.Cancel = _presenter.ViewClosing();
      }
      
      #endregion
      
      #region Data Grid View Handlers
      
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
               ColorLogFileChanged(null, EventArgs.Empty);
               
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
                  ColorLogFileChanged(null, EventArgs.Empty);
                  
                  //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Set Log Lines (log lines different)");
               }
            }
            // Nothing in the Textbox... Load LogLines
            else
            {
               txtLogFile.SetLogLines(logLines, instance.Name);
               ColorLogFileChanged(null, EventArgs.Empty);
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
         _presenter.DataGridViewSorted();
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
         _presenter.FileNewClick();
      }

      private void mnuFileOpen_Click(object sender, EventArgs e)
      {
         _presenter.FileOpenClick();
      }

      private void mnuFileSave_Click(object sender, EventArgs e)
      {
         _presenter.FileSaveClick();
      }

      private void mnuFileSaveas_Click(object sender, EventArgs e)
      {
         _presenter.FileSaveAsClick();
      }

      private void mnuFileQuit_Click(object sender, EventArgs e)
      {
         // CanContinueDestructiveOp code moved into frmMainNew_FormClosing() - Issue 1
         Close();
      }

      #endregion

      #region Edit Menu Click Handlers

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
         HandleProcessStartResult(_processStarter.ShowFileExplorer(_prefs.ApplicationDataFolderPath));
      }

      private void mnuHelpHfmGroup_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowHfmGoogleGroup());
      }
      
      private void mnuHelpCheckForUpdate_Click(object sender, EventArgs e)
      {
         // if already in progress, stub out...
         if (_updateLogic.CheckInProgress) return;
         
         _updateLogic.CheckForUpdate(true);
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
         ShowHideLogWindow(!txtLogFile.Visible);
      }

      /// <summary>
      /// Show or Hide the Queue Viewer
      /// </summary>
      private void btnQueue_Click(object sender, EventArgs e)
      {
         ShowHideQueue(!queueControl.Visible);
      }
      
      public void ShowHideQueue(bool show)
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
         string calculationTypeString = (from item in OptionsModel.PpdCalculationList
                                         where ((PpdCalculationType)item.ValueMember) == calculationType
                                         select item.DisplayMember).First();
         toolTipNotify.Show(calculationTypeString, this, Size.Width - 150, 8, 2000);
         dataGridView1.Invalidate();
      }
      
      #endregion

      #region Tools Menu Click Handlers

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

      private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
      {
         int projectId = 0;
      
         // Check for SelectedDisplayInstance, and if found... load its ProjectID.
         if (_clientInstances.SelectedDisplayInstance != null)
         {
            projectId = _clientInstances.SelectedDisplayInstance.ProjectID;
         }

         var frm = new frmBenchmarks(_prefs, _proteinCollection, _benchmarkContainer, _clientInstances, projectId)
                   {
                      StartPosition = FormStartPosition.Manual
                   };

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
         _clientInstances.RefreshUserStatsData(true);
      }

      private void mnuWebHFMGoogleCode_Click(object sender, EventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowHfmGoogleCode());
      }

      #endregion

      #region Background Work Routines

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

            RefreshControlsWithTotalsData();
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
      
      public void SelectCurrentRowKey()
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
      public void ApplySort()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(ApplySort), null);
         }
         else
         {
            dataGridView1.FreezeSorted = true;
            
            // if we have a column name and a valid sort order
            if (!(String.IsNullOrEmpty(_presenter.SortColumnName)) &&
                !(_presenter.SortColumnOrder.Equals(SortOrder.None)))
            {
               // check for the column
               DataGridViewColumn column = dataGridView1.Columns[_presenter.SortColumnName];
               if (column != null)
               {
                  ListSortDirection sortDirection = _presenter.SortColumnOrder.Equals(SortOrder.Ascending)
                                                       ? ListSortDirection.Ascending
                                                       : ListSortDirection.Descending;

                  dataGridView1.Sort(column, sortDirection);
                  dataGridView1.SortedColumn.HeaderCell.SortGlyphDirection = _presenter.SortColumnOrder;
               }
            }

            dataGridView1.FreezeSorted = false;
         }
      }

      private void RefreshControlsWithTotalsData()
      {
         InstanceTotals totals = _clientInstances.GetInstanceTotals();

         double totalPPD = totals.PPD;
         int goodHosts = totals.WorkingClients;

         SetNotifyIconText(String.Format("{0} Working Clients{3}{1} Non-Working Clients{3}{2:" + _prefs.PpdFormatString + "} PPD",
                                         goodHosts, totals.NonWorkingClients, totalPPD, Environment.NewLine));
         RefreshClientStatusBarLabels(goodHosts, totalPPD);
      }

      private void RefreshClientStatusBarLabels(int goodHosts, double totalPPD)
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

      private void SetNotifyIconText(string val)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<string>(SetNotifyIconText), val);
            return;
         }
         
         // make sure the object has been created
         if (NotifyIcon != null)
         {
            if (val.Length > 64)
            {
               //if string is too long, remove the word Clients
               val = val.Replace("Clients", String.Empty);
            }
            NotifyIcon.Text = val;
         }
      }

      /// <summary>
      /// Sets the Hosts Status Label Text (Thread Safe)
      /// </summary>
      /// <param name="val">Text to set</param>
      private void SetStatusLabelHostsText(string val)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<string>(SetStatusLabelHostsText), val);
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
            BeginInvoke(new Action<string>(SetStatusLabelPPDText), val);
            return;
         }
         
         statusLabelPPW.Text = val;
      }

      #endregion

      #region System Tray Icon Routines

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

      #region Helper Routines

      private void ClearLogAndQueueViewer()
      {
         // clear the log text
         txtLogFile.SetNoLogLines();
         // clear the queue control
         queueControl.SetQueue(null);
      }

      public void ShowHideLogWindow(bool show)
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
      
      /// <summary>
      /// Refresh User Stats from Xml Data Source
      /// </summary>
      private void RefreshUserStatsControls()
      {
         //try
         //{
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
         //}
         //catch (Exception ex)
         //{
         //   HfmTrace.WriteToHfmConsole(ex);
         //}
      }
      
      #endregion

      #region PreferenceSet Event Handlers

      private void UserStatsEnabledChanged()
      {
         var showXmlStats = _prefs.GetPreference<bool>(Preference.ShowXmlStats);
         SetStatsControlsVisible(showXmlStats);
         if (showXmlStats)
         {
            _clientInstances.RefreshUserStatsData(false);
         }
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

      private void MessageLevelChanged(object sender, EventArgs e)
      {
         var newLevel = (TraceLevel)_prefs.GetPreference<int>(Preference.MessageLevel);
         if (newLevel != TraceLevelSwitch.Instance.Level)
         {
            TraceLevelSwitch.Instance.Level = newLevel;
            HfmTrace.WriteToHfmConsole(String.Format("Debug Message Level Changed: {0}", newLevel));
         }
      }

      private void ColorLogFileChanged(object sender, EventArgs e)
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

      #region Status Bar Context Menu Event Handlers

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

      #endregion
      
      #endregion
   }
   // ReSharper restore InconsistentNaming
}
