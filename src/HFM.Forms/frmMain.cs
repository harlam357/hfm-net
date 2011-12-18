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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Controls;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public interface IMainView : IWin32Window, ISynchronizeInvoke
   {
      #region System.Windows.Forms.Form Properties

      FormWindowState WindowState { get; set; }

      bool ShowInTaskbar { get; set; }

      string Text { get; set; }

      bool Visible { get; set; }

      Point Location { get; set; }

      Size Size { get; set; }

      Rectangle RestoreBounds { get; }

      #endregion

      #region Properties

      string StatusLabelLeftText { get; set; }

      bool WorkUnitHistoryMenuEnabled { get; set; }

      ILogFileViewer LogFileViewer { get; }

      IDataGridView DataGridView { get; }

      SplitContainer SplitContainer { get; }

      SplitContainer SplitContainer2 { get; }

      bool QueueControlVisible { get; set; }

      #endregion
      
      #region Methods

      void Initialize(MainPresenter presenter);

      void SetGridDataSource(object dataSource);

      void SecondInstanceStarted(string[] args);

      void SetManualStartPosition();

      void SetNotifyIconVisible(bool visible);

      void SetClientMenuItemsVisible(bool filesMenu, bool cachedLog, bool seperator);

      void SetGridContextMenuItemsVisible(bool filesMenu, bool cachedLog, bool seperator);

      void ShowGridContextMenuStrip(Point screenLocation);

      void DisableViewResizeEvent();

      void EnableViewResizeEvent();

      void SetQueueButtonText(string text);

      void ShowNotifyToolTip(string text);

      void SetQueue(ClientQueue queue);

      void SetQueue(ClientQueue queue, SlotType slotType, bool utcOffsetIsZero);
      
      void RefreshControlsWithTotalsData(SlotTotals totals);
      
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
      
      public bool WorkUnitHistoryMenuEnabled
      {
         get { return mnuToolsHistory.Enabled; }
         set { mnuToolsHistory.Enabled = value; }
      }

      public ILogFileViewer LogFileViewer { get { return txtLogFile; } }

      public IDataGridView DataGridView { get { return dataGridView1; } }
      
      public SplitContainer SplitContainer { get { return splitContainer1; } }

      public SplitContainer SplitContainer2 { get { return splitContainer2; } }

      public bool QueueControlVisible
      {
         get { return queueControl.Visible; }
         set { queueControl.Visible = value; }
      }
      
      #endregion
      
      #region Fields

      private MainPresenter _presenter;
      private NotifyIcon _notifyIcon;

      private readonly IPreferenceSet _prefs;
      private readonly UserStatsDataModel _userStatsDataModel;

      #endregion

      #region Constructor

      /// <summary>
      /// Main Form Constructor
      /// </summary>
      public frmMain(IPreferenceSet prefs, UserStatsDataModel userStatsDataModel)
      {
         _prefs = prefs;
         _userStatsDataModel = userStatsDataModel;

         // This call is Required by the Windows Form Designer
         InitializeComponent();

         // Set Main Form Text
#if BETA
         base.Text = String.Format("HFM.NET v{0} - Beta", Core.Application.Version);
#else
         base.Text = String.Format("HFM.NET v{0}", Core.Application.Version);
#endif
      }
      
      #endregion
      
      #region Initialize

      public void Initialize(MainPresenter presenter)
      {
         _presenter = presenter;
         // Resize can be invoked when InitializeComponent() is call
         // if the DPI is not set to Normal (96 DPI).  The frmMain_Resize
         // method depends on _presenter being NOT NULL... wait to hook
         // up this event until after _presenter has been set (above).
         Resize += frmMain_Resize;
      
         #region Initialize Controls

         // Manually Create the Columns - Issue 41
         dataGridView1.AutoGenerateColumns = false;
         SetupDataGridViewColumns(dataGridView1);
         // Add Column Selector
         new DataGridViewColumnSelector(dataGridView1);
         // Give the Queue Control access to the Protein Collection
         queueControl.SetProteinCollection(ServiceLocator.Resolve<IProteinDictionary>());

         #endregion

         // Initialize the Presenter
         _presenter.Initialize();

         BindToUserStatsDataModel();
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

      private void BindToUserStatsDataModel()
      {
         statusUserTeamRank.DataBindings.Add("Text", _userStatsDataModel, "Rank", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserProjectRank.DataBindings.Add("Text", _userStatsDataModel, "OverallRank", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUser24hr.DataBindings.Add("Text", _userStatsDataModel, "TwentyFourHourAvgerage", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserToday.DataBindings.Add("Text", _userStatsDataModel, "PointsToday", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserWeek.DataBindings.Add("Text", _userStatsDataModel, "PointsWeek", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserTotal.DataBindings.Add("Text", _userStatsDataModel, "PointsTotal", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserWUs.DataBindings.Add("Text", _userStatsDataModel, "WorkUnitsTotal", false, DataSourceUpdateMode.OnPropertyChanged);

         statusUserTeamRank.DataBindings.Add("Visible", _userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserProjectRank.DataBindings.Add("Visible", _userStatsDataModel, "OverallRankVisible", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUser24hr.DataBindings.Add("Visible", _userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserToday.DataBindings.Add("Visible", _userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserWeek.DataBindings.Add("Visible", _userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserTotal.DataBindings.Add("Visible", _userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
         statusUserWUs.DataBindings.Add("Visible", _userStatsDataModel, "ControlsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
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

      public void SetGridDataSource(object dataSource)
      {
         dataGridView1.DataSource = dataSource;
      }

      #endregion
      
      #region Form Handlers

      public void SecondInstanceStarted(string[] args)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<string[]>(SecondInstanceStarted), args);
            return;
         }

         if (WindowState == FormWindowState.Minimized)
         {
            WindowState = _presenter.OriginalWindowState;
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
      
      public void SetClientMenuItemsVisible(bool filesMenu, bool cachedLog, bool seperator)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<bool, bool, bool>(SetClientMenuItemsVisible), filesMenu, cachedLog, seperator);
            return;
         }

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
         var prefDialog = ServiceLocator.Resolve<PreferencesDialog>();
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
         var about = new AboutDialog();
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

      private void mnuClientsAddLegacy_Click(object sender, EventArgs e)
      {
         _presenter.ClientsAddLegacyClick();
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

      public void SetQueue(ClientQueue queue)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<ClientQueue>(SetQueue), queue);
            return;
         }

         queueControl.SetQueue(queue);
      }

      public void SetQueue(ClientQueue queue, SlotType slotType, bool utcOffsetIsZero)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<ClientQueue, SlotType, bool>(SetQueue), queue, slotType, utcOffsetIsZero);
            return;
         }

         queueControl.SetQueue(queue, slotType, utcOffsetIsZero);
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
         _userStatsDataModel.Refresh(true);
      }

      private void mnuWebHFMGoogleCode_Click(object sender, EventArgs e)
      {
         _presenter.ShowHfmGoogleCode();
      }

      #endregion

      #region Background Work Routines

      public void RefreshControlsWithTotalsData(SlotTotals totals)
      {
         SetNotifyIconText(String.Format("{0} Working Clients{3}{1} Non-Working Clients{3}{2:" + _prefs.PpdFormatString + "} PPD",
                                         totals.WorkingClients, totals.NonWorkingClients, totals.PPD, Environment.NewLine));

         string clientLabel = totals.WorkingClients == 1 ? "Client" : "Clients";
         SetStatusLabelHostsText(String.Format(CultureInfo.CurrentCulture, "{0} {1}", totals.WorkingClients, clientLabel));
         SetStatusLabelPPDText(String.Format(CultureInfo.CurrentCulture, "{0:" + _prefs.PpdFormatString + "} PPD", totals.PPD));
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
         _userStatsDataModel.SetViewStyle(false);
      }

      private void mnuContextShowTeamStats_Click(object sender, EventArgs e)
      {
         _userStatsDataModel.SetViewStyle(true);
      }

      #endregion
   }

   // ReSharper restore InconsistentNaming
}
