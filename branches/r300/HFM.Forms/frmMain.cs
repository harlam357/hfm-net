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
 
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   public interface IMainView : IWin32Window
   {
      #region System.Windows.Forms Properties

      FormWindowState WindowState { get; set; }

      bool ShowInTaskbar { get; set; }

      string Text { get; set; }

      bool Visible { get; set; }

      Point Location { get; set; }

      Size Size { get; set; }

      Rectangle RestoreBounds { get; }

      bool InvokeRequired { get; }
      
      #endregion

      #region System.Windows.Forms Methods

      object Invoke(Delegate method);

      object Invoke(Delegate method, params object[] args);
      
      #endregion
      
      #region Properties

      string StatusLabelLeftText { get; set; }

      ILogFileViewer LogFileViewer { get; }

      DataGridView DataGridView { get; }

      SplitContainer SplitContainer { get; }

      SplitContainer SplitContainer2 { get; }

      QueueControl QueueControl { get; }
      
      #endregion
      
      #region Methods

      void Initialize(MainPresenter presenter, IProteinCollection proteinCollection);

      void SetManualStartPosition();

      void SetNotifyIconVisible(bool visible);

      void SetClientMenuItemsVisible(bool filesMenu, bool cachedLog, bool seperator);

      void SetGridContextMenuItemsVisible(bool filesMenu, bool cachedLog, bool seperator);

      void ShowGridContextMenuStrip(Point screenLocation);

      void DisableViewResizeEvent();

      void EnableViewResizeEvent();

      void SetQueueButtonText(string text);

      void ShowNotifyToolTip(string text);
      
      #region Background Processing Methods
      
      void ApplySort();

      void SelectCurrentRowKey();
      
      #endregion
      
      #endregion
   }

   // ReSharper disable InconsistentNaming
   public partial class frmMain : FormWrapper, IMainView
   {
      #region Properties
   
      public string StatusLabelLeftText
      {
         get { return statusLabelLeft.Text; }
         set { statusLabelLeft.Text = value; }
      }

      public ILogFileViewer LogFileViewer { get { return txtLogFile; } }

      public DataGridView DataGridView { get { return dataGridView1; } }
      
      public SplitContainer SplitContainer { get { return splitContainer1; } }

      public SplitContainer SplitContainer2 { get { return splitContainer2; } }

      public QueueControl QueueControl { get { return queueControl; } }
      
      #endregion
      
      #region Fields

      private static readonly string FormTitle = String.Format("HFM.NET v{0} - Beta", PlatformOps.ApplicationVersion);

      private MainPresenter _presenter;

      private NotifyIcon _notifyIcon;

      private readonly BindingSource _displayBindingSource;
      
      #region Service Interfaces
      
      private readonly IPreferenceSet _prefs;

      private readonly IXmlStatsDataContainer _statsData;

      private readonly IInstanceCollection _instanceCollection;

      #endregion

      #endregion

      #region Constructor

      /// <summary>
      /// Main Form constructor
      /// </summary>
      public frmMain(IPreferenceSet prefs, IXmlStatsDataContainer statsData, IInstanceCollection instanceCollection)
      {
         _prefs = prefs;
         _statsData = statsData;
         _instanceCollection = instanceCollection;
         _displayBindingSource = new BindingSource();

         // This call is Required by the Windows Form Designer
         InitializeComponent();

         // Set Main Form Text
         base.Text = FormTitle;
      }
      
      #endregion
      
      #region Initialize

      public void Initialize(MainPresenter presenter, IProteinCollection proteinCollection)
      {
         _presenter = presenter;
         // Resize can be invoked when InitializeComponent() is call
         // if the DPI is not set to Normal (96 DPI).  The frmMain_Resize
         // method depends on _presenter being NOT NULL... wait to hook
         // up this event until after _presenter has been set (above).
         Resize += frmMain_Resize;
      
         // Manually Create the Columns - Issue 41
         DataGridViewWrapper.SetupDataGridViewColumns(dataGridView1);
         // Restore Form Preferences (MUST BE DONE AFTER DataGridView Columns are Setup)
         _presenter.RestoreFormPreferences();
         SetupDataGridView();

         // Give the Queue Control access to the Protein Collection
         queueControl.SetProteinCollection(proteinCollection);

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
               _instanceCollection.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
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
         _instanceCollection.RefreshGrid += delegate { RefreshDisplay(); };
         _instanceCollection.InvalidateGrid += delegate { dataGridView1.Invalidate(); };
         _instanceCollection.OfflineLastChanged += delegate { ApplySort(); };
         _instanceCollection.InstanceDataChanged += delegate { _presenter.AutoSaveConfig(); };
         _instanceCollection.SelectedInstanceChanged += delegate { _presenter.DisplaySelectedInstance(); };
         _instanceCollection.RefreshUserStatsControls += delegate { RefreshUserStatsControls(); };
      }

      private void SubscribeToPreferenceSetEvents()
      {
         _prefs.FormShowStyleSettingsChanged += delegate { _presenter.SetViewShowStyle(); };
         _prefs.ShowUserStatsChanged += delegate { UserStatsEnabledChanged(); };
         _prefs.MessageLevelChanged += delegate { _presenter.ApplyMessageLevelIfChanged(); };
         _prefs.ColorLogFileChanged += delegate { _presenter.ApplyColorLogFileSetting(); };
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
      
      #region Form Handlers

      public void SecondInstanceStarted(string[] args)
      {
         if (InvokeRequired)
         {
            BeginInvoke((MethodInvoker)(() => SecondInstanceStarted(args)));
            return;
         }
      
         if (WindowState == FormWindowState.Minimized)
         {
            WindowState = _presenter.OriginalWindowState;
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
         // Add the Index Changed Handler here after everything is shown
         dataGridView1.ColumnDisplayIndexChanged += delegate { _presenter.DataGridViewColumnDisplayIndexChanged(); };
         // Then run it once to ensure the last column is set to Fill
         _presenter.DataGridViewColumnDisplayIndexChanged();
         // Add the Splitter Moved Handler here after everything is shown - Issue 8
         // When the log file window (Panel2) is visible, this event will fire.
         // Update the split location directly from the split panel control. - Issue 8
         splitContainer1.SplitterMoved += delegate { _presenter.UpdateMainSplitContainerDistance(splitContainer1.SplitterDistance); };
      
         _notifyIcon = new NotifyIcon(components);
         _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
         _notifyIcon.ContextMenuStrip = notifyMenu;
         _notifyIcon.Icon = Icon;
         _notifyIcon.Text = Text;
         _notifyIcon.DoubleClick += delegate { _presenter.NotifyIconDoubleClick(); };
         
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

      #region Other IMainView Interface Methods

      public void SetManualStartPosition()
      {
         StartPosition = FormStartPosition.Manual;
      }

      public void SetNotifyIconVisible(bool visible)
      {
         if (_notifyIcon != null) _notifyIcon.Visible = visible;
      }

      public void DisableViewResizeEvent()
      {
         Resize -= frmMain_Resize;
      }

      public void EnableViewResizeEvent()
      {
         Resize += frmMain_Resize;
      }

      public void SetQueueButtonText(string text)
      {
         btnQueue.Text = text;
      }

      #endregion
      
      #region Data Grid View Handlers
      
      private void dataGridView1_SelectionChanged(object sender, EventArgs e)
      {
         _instanceCollection.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
      }

      public void SetClientMenuItemsVisible(bool filesMenu, bool cachedLog, bool seperator)
      {
         mnuClientsViewClientFiles.Visible = filesMenu;
         mnuClientsViewCachedLog.Visible = cachedLog;
         mnuClientsSep4.Visible = seperator;
      }

      private void queueControl_QueueIndexChanged(object sender, QueueIndexChangedEventArgs e)
      {
         _presenter.QueueIndexChanged(e.Index);
      }

      private void dataGridView1_Sorted(object sender, EventArgs e)
      {
         _presenter.DataGridViewSorted();
      }

      private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
      {
         _presenter.DataGridViewMouseDown(e.X, e.Y, e.Button, e.Clicks);
      }
      
      public void SetGridContextMenuItemsVisible(bool filesMenu, bool cachedLog, bool seperator)
      {
         mnuContextClientsViewClientFiles.Visible = filesMenu;
         mnuContextClientsViewCachedLog.Visible = cachedLog;
         mnuContextClientsSep2.Visible = seperator;
      }
      
      public void ShowGridContextMenuStrip(Point screenLocation)
      {
         gridContextMenuStrip.Show(screenLocation);
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
         _presenter.ShowHfmLogFile();
      }

      private void mnuHelpHfmDataFiles_Click(object sender, EventArgs e)
      {
         _presenter.ShowHfmDataFiles();
      }

      private void mnuHelpHfmGroup_Click(object sender, EventArgs e)
      {
         _presenter.ShowHfmGoogleGroup();
      }
      
      private void mnuHelpCheckForUpdate_Click(object sender, EventArgs e)
      {
         _presenter.CheckForUpdateClick();
      }
      
      private void mnuHelpAbout_Click(object sender, EventArgs e)
      {
         var about = new frmAbout();
         about.ShowDialog(this);
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
         _presenter.ClientsAddClick();
      }

      private void mnuClientsEdit_Click(object sender, EventArgs e)
      {
         _presenter.ClientsEditClick();
      }

      private void mnuClientsDelete_Click(object sender, EventArgs e)
      {
         _presenter.ClientsDeleteClick();
      }

      private void mnuClientsMerge_Click(object sender, EventArgs e)
      {
         _presenter.ClientsMergeClick();
      }

      private void mnuClientsRefreshSelected_Click(object sender, EventArgs e)
      {
         _presenter.ClientsRefreshSelectedClick();
      }

      private void mnuClientsRefreshAll_Click(object sender, EventArgs e)
      {
         _presenter.ClientsRefreshAllClick();
      }

      private void mnuClientsViewCachedLog_Click(object sender, EventArgs e)
      {
         _presenter.ClientsViewCachedLogClick();
      }

      private void mnuClientsViewClientFiles_Click(object sender, EventArgs e)
      {
         _presenter.ClientsViewClientFilesClick();
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
      }

      private void mnuViewToggleCompletedCountStyle_Click(object sender, EventArgs e)
      {
         _presenter.ViewToggleCompletedCountStyleClick();
      }

      private void mnuViewToggleVersionInformation_Click(object sender, EventArgs e)
      {
         _presenter.ViewToggleVersionInformationClick();
      }

      private void mnuViewToggleBonusCalculation_Click(object sender, EventArgs e)
      {
         _presenter.ViewToggleBonusCalculationClick();
      }

      private void mnuViewCycleCalculation_Click(object sender, EventArgs e)
      {
         _presenter.ViewCycleCalculationClick();
      }
      
      public void ShowNotifyToolTip(string text)
      {
         toolTipNotify.Show(text, this, Size.Width - 150, 8, 2000);
      }
      
      #endregion

      #region Tools Menu Click Handlers

      private void mnuToolsDownloadProjects_Click(object sender, EventArgs e)
      {
         _presenter.ToolsDownloadProjectsClick();
      }

      private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
      {
         _presenter.ToolsBenchmarksClick();
      }

      private void mnuToolsHistory_Click(object sender, EventArgs e)
      {
         _presenter.ToolsHistoryClick();
      }

      #endregion

      #region Web Menu Click Handlers

      private void mnuWebEOCUser_Click(object sender, EventArgs e)
      {
         _presenter.ShowEocUserPage();
      }

      private void mnuWebStanfordUser_Click(object sender, EventArgs e)
      {
         _presenter.ShowStanfordUserPage();
      }

      private void mnuWebEOCTeam_Click(object sender, EventArgs e)
      {
         _presenter.ShowEocTeamPage();
      }

      private void mnuWebRefreshUserStats_Click(object sender, EventArgs e)
      {
         _presenter.RefreshUserStatsClick();
      }

      private void mnuWebHFMGoogleCode_Click(object sender, EventArgs e)
      {
         _presenter.ShowHfmGoogleCode();
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
                  Invoke(new MethodInvoker(_instanceCollection.RefreshDisplayCollection));
               }
               else
               {
                  _instanceCollection.RefreshDisplayCollection();
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
               _instanceCollection.SetSelectedInstance(GetSelectedRowInstanceName(dataGridView1.SelectedRows));
            }
            _instanceCollection.RaiseSelectedInstanceChanged();
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
         InstanceTotals totals = _instanceCollection.GetInstanceTotals();

         double totalPPD = totals.PPD;
         int goodHosts = totals.WorkingClients;

         SetNotifyIconText(String.Format("{0} Working Clients{3}{1} Non-Working Clients{3}{2:" + _prefs.PpdFormatString + "} PPD",
                                         goodHosts, totals.NonWorkingClients, totalPPD, Environment.NewLine));
         RefreshClientStatusBarLabels(goodHosts, totalPPD);
      }

      private void RefreshClientStatusBarLabels(int goodHosts, double totalPPD)
      {
         string clientLabel = goodHosts == 1 ? "Client" : "Clients";
         SetStatusLabelHostsText(String.Format(CultureInfo.CurrentCulture, "{0} {1}", goodHosts, clientLabel));
         SetStatusLabelPPDText(String.Format(CultureInfo.CurrentCulture, "{0:" + _prefs.PpdFormatString + "} PPD", totalPPD));
      }

      private void SetNotifyIconText(string val)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<string>(SetNotifyIconText), val);
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

      #endregion

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
      
      private void RefreshUserStatsControls()
      {
         var data = _statsData.Data;
         if (_prefs.GetPreference<bool>(Preference.ShowTeamStats))
         {
            ApplyUserStats(data.TeamRank, 0, data.TeamTwentyFourHourAvgerage, data.TeamPointsToday, 
                           data.TeamPointsWeek, data.TeamPointsTotal, data.TeamWorkUnitsTotal);
         }
         else
         {
            ApplyUserStats(data.UserTeamRank, data.UserOverallRank, data.UserTwentyFourHourAvgerage, data.UserPointsToday,
                           data.UserPointsWeek, data.UserPointsTotal, data.UserWorkUnitsTotal);
         }
      }
      
      private void ApplyUserStats(int teamRank, int projectRank, long twentyFourHour, long today, long week, long totalPoints, long totalWUs)
      {
         statusUserTeamRank.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Team: ", Constants.EocStatsFormat), teamRank);
         statusUserProjectRank.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Project: ", Constants.EocStatsFormat), projectRank);
         statusUser24hr.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("24hr: ", Constants.EocStatsFormat), twentyFourHour);
         statusUserToday.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Today: ", Constants.EocStatsFormat), today);
         statusUserWeek.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Week: ", Constants.EocStatsFormat), week);
         statusUserTotal.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("Total: ", Constants.EocStatsFormat), totalPoints);
         statusUserWUs.Text = String.Format(CultureInfo.CurrentCulture, String.Concat("WUs: ", Constants.EocStatsFormat), totalWUs);
      }
      
      private void UserStatsEnabledChanged()
      {
         var showXmlStats = _prefs.GetPreference<bool>(Preference.ShowXmlStats);
         SetStatsControlsVisible(showXmlStats);
         if (showXmlStats)
         {
            _instanceCollection.RefreshUserStatsData(false);
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
   }
   // ReSharper restore InconsistentNaming
}
