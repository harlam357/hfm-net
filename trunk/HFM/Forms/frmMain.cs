/*
 * HFM.NET - Main UI Form
 * Copyright (C) 2006 David Rawling
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
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Classes;
using HFM.Framework;
using HFM.Helpers;
using HFM.Instances;
using HFM.Instrumentation;

namespace HFM.Forms
{
   internal enum PaintCell
   {
      Status,
      Time,
      Warning
   }

   public partial class frmMain : FormWrapper
   {
      #region Private Fields
      
      private static string FormTitle = String.Format("HFM.NET v{0} - Beta", PlatformOps.ApplicationVersion);
      
      /// <summary>
      /// Holds the state of the window before it is hidden (minimise to tray behaviour)
      /// </summary>
      private FormWindowState originalState;

      /// <summary>
      /// Holds current Sort Column Name
      /// </summary>
      private string SortColumnName = String.Empty;

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      private SortOrder SortColumnOrder = SortOrder.None;
      
      /// <summary>
      /// Holds Current Mouse Over Row Index
      /// </summary>
      private Int32 CurrentMouseOverRow = -1;
      
      /// <summary>
      /// Holds Current Mouse Over Column Index
      /// </summary>
      private Int32 CurrentMouseOverColumn = -1;

      /// <summary>
      /// Notify Icon for frmMain
      /// </summary>
      private NotifyIcon notifyIcon;
      
      /// <summary>
      /// Update Logic Class
      /// </summary>
      private UpdateLogic _updateLogic;

      /// <summary>
      /// Path to the update file (MSI) to run on exit
      /// </summary>
      private string _updateFilePath;
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _Prefs;

      /// <summary>
      /// Messages View
      /// </summary>
      private readonly IMessagesView _frmMessages;

      /// <summary>
      /// XML Stats Data Interface
      /// </summary>
      private readonly IXmlStatsDataContainer _statsData;

      /// <summary>
      /// Collection of Client Instances
      /// </summary>
      private readonly InstanceCollection _clientInstances;

      /// <summary>
      /// Display Collection Binding Source
      /// </summary>
      private readonly BindingSource _displayBindingSource;
      #endregion

      #region Form Constructor / Functionality
      /// <summary>
      /// Main form constructor
      /// </summary>
      public frmMain(IPreferenceSet prefs, IMessagesView messagesView)
      {
         _Prefs = prefs;
         _frmMessages = messagesView;
         _statsData = InstanceProvider.GetInstance<IXmlStatsDataContainer>();

         // Create Instance Collection
         _clientInstances = new InstanceCollection(_Prefs, InstanceProvider.GetInstance<IProteinCollection>(), 
                                                           InstanceProvider.GetInstance<IProteinBenchmarkContainer>());
         _displayBindingSource = new BindingSource();

         // This call is Required by the Windows Form Designer
         InitializeComponent();

         // Set Main Form Text
         base.Text = FormTitle;
      }

      public void Initialize()
      {
         // Manually Create the Columns - Issue 41
         DisplayInstance.SetupDataGridViewColumns(dataGridView1);
         // Restore Form Preferences (MUST BE DONE AFTER DataGridView Columns are Setup)
         RestoreFormPreferences();
         SetupDataGridView();

         // Read the Protein Collection from disk
         IProteinCollection proteinCollection = InstanceProvider.GetInstance<IProteinCollection>();
         proteinCollection.Read();
         // Give the Queue Control access to the Protein Collection
         queueControl.SetProteinCollection(proteinCollection);

         // Read the Benchmarks from disk
         IProteinBenchmarkContainer benchmarkContainer = InstanceProvider.GetInstance<IProteinBenchmarkContainer>();
         benchmarkContainer.Read();
         
         // Read the User/Team stats data from disk
         _statsData.Read();
         // Set Stats Visibility and Refresh if necessary
         XmlStatsVisible(_Prefs.GetPreference<bool>(Preference.ShowUserStats));

         SubscribeToInstanceCollectionEvents();
         SubscribeToPreferenceSetEvents();

         // If Mono, use the RowEnter Event (which was what 0.3.0 and prior used)
         // to set the CurrentInstance selection.  Obviously Mono doesn't fire the
         // DataGridView.SelectionChanged Event.
         if (PlatformOps.IsRunningOnMono())
         {
            HfmTrace.WriteToHfmConsole("Running on Mono...");
            dataGridView1.RowEnter += delegate
            {
               _clientInstances.SetCurrentInstance(dataGridView1.SelectedRows);
            };
         }
      }

      private void SetupDataGridView()
      {
         // Add Column Selector
         new DataGridViewColumnSelector(dataGridView1);

         dataGridView1.AutoGenerateColumns = false;
         _displayBindingSource.DataSource = _clientInstances.GetDisplayCollection();
         dataGridView1.DataSource = _displayBindingSource;
      }

      private void SubscribeToInstanceCollectionEvents()
      {
         _clientInstances.CollectionChanged += delegate { RefreshDisplay(); };
         //_clientInstances.CollectionLoaded += ClientInstances_CollectionLoaded;
         //_clientInstances.CollectionSaved += ClientInstances_CollectionSaved;
         _clientInstances.InstanceAdded += ClientInstances_InstanceDataChanged;
         _clientInstances.InstanceEdited += ClientInstances_InstanceDataChanged;
         _clientInstances.InstanceRemoved += ClientInstances_InstanceDataChanged;
         _clientInstances.InstanceRetrieved += delegate { RefreshDisplay(); };
         _clientInstances.SelectedInstanceChanged += ClientInstances_SelectedInstanceChanged;
         _clientInstances.FindDuplicatesComplete += delegate { RefreshDisplay(); };
         _clientInstances.OfflineLastChanged += delegate { ApplySort(); };
         _clientInstances.RefreshUserStatsData += delegate { RefreshUserStatsData(false); };
      }

      private void SubscribeToPreferenceSetEvents()
      {
         _Prefs.FormShowStyleSettingsChanged += PreferenceSet_FormShowStyleSettingsChanged;
         _Prefs.ShowUserStatsChanged += PreferenceSet_ShowUserStatsChanged;
         _Prefs.MessageLevelChanged += PreferenceSet_MessageLevelChanged;
         _Prefs.ColorLogFileChanged += PreferenceSet_ColorLogFileChanged;
         _Prefs.PpdCalculationChanged += delegate { RefreshDisplay(); };
         _Prefs.DecimalPlacesChanged += delegate { RefreshDisplay(); };
         _Prefs.CalculateBonusChanged += delegate { RefreshDisplay(); };
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
            WindowState = originalState;
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

      /// <summary>
      /// Loads the appropriate file on initialisation - ie when showing the form
      /// </summary>
      private void frmMain_Shown(object sender, EventArgs e)
      {
         if (_Prefs.GetPreference<bool>(Preference.RunMinimized))
         {
            WindowState = FormWindowState.Minimized;
         }
      
         string fileName = String.Empty;

         if (Program.cmdArgs.Length > 0)
         {
            // Filename on command line - probably from Explorer
            fileName = Program.cmdArgs[0];
         }
         else if (_Prefs.GetPreference<bool>(Preference.UseDefaultConfigFile))
         {
            fileName = _Prefs.GetPreference<string>(Preference.DefaultConfigFile);
         }

         if (String.IsNullOrEmpty(fileName) == false)
         {
            LoadFile(fileName);
         }
         
         // Add the Index Changed Handler here after everything is shown
         dataGridView1.ColumnDisplayIndexChanged += dataGridView1_ColumnDisplayIndexChanged;
         // Then run it once to ensure the last column is set to Fill
         dataGridView1_ColumnDisplayIndexChanged(null, null);
         // Add the Splitter Moved Handler here after everything is shown - Issue 8
         splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;

         notifyIcon = new NotifyIcon(components);
         
         notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
         notifyIcon.ContextMenuStrip = notifyMenu;
         notifyIcon.Icon = Icon;
         notifyIcon.Text = base.Text;
         notifyIcon.DoubleClick += notifyIcon_DoubleClick;
         SetFormShowStyle();

         if (_Prefs.GetPreference<bool>(Preference.StartupCheckForUpdate))
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
         if (_updateLogic == null) _updateLogic = new UpdateLogic(this, new MessageBoxView());
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

         UpdatePresenter updatePresenter = new UpdatePresenter(this, HfmTrace.WriteToHfmConsole,
            update, NetworkOps.GetProxy(), Constants.ApplicationName, PlatformOps.ApplicationVersionWithRevision);
         updatePresenter.ShowView();
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

      /// <summary>
      /// Hides the taskbar icon if the app is minimized
      /// </summary>
      private void frmMain_Resize(object sender, EventArgs e)
      {
         if (WindowState != FormWindowState.Minimized)
         {
            originalState = WindowState;
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
            _Prefs.SetPreference(Preference.FormSplitLocation, splitContainer1.Panel1.Height);
         }
      }

      /// <summary>
      /// Save Form State on before closing
      /// </summary>
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
            _Prefs.SetPreference(Preference.FormLocation, Location);
            _Prefs.SetPreference(Preference.FormSize, Size);
         }
         else
         {
            _Prefs.SetPreference(Preference.FormLocation, RestoreBounds.Location);
            _Prefs.SetPreference(Preference.FormSize, RestoreBounds.Size);
         }

         _Prefs.SetPreference(Preference.FormLogVisible, txtLogFile.Visible);
         _Prefs.SetPreference(Preference.QueueViewerVisible, queueControl.Visible);

         // Save the data
         _Prefs.Save();
         
         // Save the data on current WUs in progress
         _clientInstances.SaveCurrentUnitInfo();
         // Save the benchmark collection
         IProteinBenchmarkContainer benchmarkContainer = InstanceProvider.GetInstance<IProteinBenchmarkContainer>();
         benchmarkContainer.Write();

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

      /// <summary>
      /// Update Split Location in Preferences
      /// </summary>
      private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
      {
         // When the log file window (Panel2) is visible, this event will fire.
         // Update the split location directly from the split panel control. - Issue 8
         _Prefs.SetPreference(Preference.FormSplitLocation, splitContainer1.SplitterDistance);
         _Prefs.Save();
      }
      #endregion
      
      #region Data Grid View Handlers
      /// <summary>
      /// When Column Index changes, set the last column to AutoSize Fill, the others to AutoSize None
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGridView1_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
      {
         if (dataGridView1.Columns.Count == InstanceCollection.NumberOfDisplayFields)
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
            _Prefs.Save();
         }
      }

      /// <summary>
      /// When Grid Selection Changes
      /// </summary>
      private void dataGridView1_SelectionChanged(object sender, EventArgs e)
      {
         _clientInstances.SetCurrentInstance(dataGridView1.SelectedRows);
      }
      
      /// <summary>
      /// When _clientInstances SelectedInstance Changes
      /// </summary>
      private void ClientInstances_SelectedInstanceChanged(object sender, EventArgs e)
      {
         if (_clientInstances.SelectedInstance != null)
         {
            statusLabelLeft.Text = _clientInstances.SelectedInstance.ClientPathAndArguments;
            
            queueControl.SetQueue(_clientInstances.SelectedInstance.DataAggregator.Queue, 
               _clientInstances.SelectedInstance.CurrentUnitInfo.TypeOfClient, 
               _clientInstances.SelectedInstance.ClientIsOnVirtualMachine);

            // if we've got a good queue read, let queueControl_QueueIndexChanged()
            // handle populating the log lines.
            IQueueBase qBase = _clientInstances.SelectedInstance.DataAggregator.Queue;
            if (qBase != null && qBase.DataPopulated) return;

            // I'm not sure why I ever wrote this check here - it makes no sense - 4/6/10
            //if (_clientInstances.SelectedInstance.DataAggregator.UnitLogLines != null)
            //{
               SetLogLines(_clientInstances.SelectedInstance.Settings, _clientInstances.SelectedInstance.DataAggregator.CurrentLogLines);
            //}
         }
         else
         {
            ClearLogAndQueueViewer();
         }
      }

      /// <summary>
      /// When Queue Control QueueIndex Changes
      /// </summary>
      private void queueControl_QueueIndexChanged(object sender, QueueIndexChangedEventArgs e)
      {
         if (e.Index == -1)
         {
            txtLogFile.SetNoLogLines();
            return;
         }

         if (_clientInstances.SelectedInstance != null)
         {
            //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Changed Queue Index ({0} - {1})", InstanceName, e.Index));

            // Check the UnitLogLines array against the requested Queue Index - Issue 171
            try
            {
               SetLogLines(_clientInstances.SelectedInstance.Settings,
                           _clientInstances.SelectedInstance.GetLogLinesForQueueIndex(e.Index));
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

      /// <summary>
      /// Populates the Log Viewer with the given LogLine List.
      /// </summary>
      /// <param name="instance">Client Instance</param>
      /// <param name="logLines">List of LogLines</param>
      private void SetLogLines(IClientInstanceSettings instance, IList<ILogLine> logLines)
      {
         /*** Checked LogLine Count ***/
         if (logLines != null && logLines.Count > 0) 
         {
            // Different Client... Load LogLines
            if (txtLogFile.LogOwnedByInstanceName.Equals(instance.InstanceName) == false)
            {
               txtLogFile.SetLogLines(logLines, instance.InstanceName);
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
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, instance.InstanceName, ex);
               }

               // If the last text line in the textbox DOES NOT equal the last LogLine Text... Load LogLines.
               // Otherwise, the log has not changed, don't update and perform the log "flicker".
               if (txtLogFile.Lines[txtLogFile.Lines.Length - 1].Equals(lastLogLine) == false)
               {
                  txtLogFile.SetLogLines(logLines, instance.InstanceName);
                  PreferenceSet_ColorLogFileChanged(null, EventArgs.Empty);
                  
                  //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Set Log Lines (log lines different)");
               }
            }
            // Nothing in the Textbox... Load LogLines
            else
            {
               txtLogFile.SetLogLines(logLines, instance.InstanceName);
               PreferenceSet_ColorLogFileChanged(null, EventArgs.Empty);
            }
         }
         else
         {
            txtLogFile.SetNoLogLines();
         }

         txtLogFile.ScrollToBottom();
      }

      /// <summary>
      /// Update Form Level Sorting Fields
      /// </summary>
      private void dataGridView1_Sorted(object sender, EventArgs e)
      {
         SortColumnName = dataGridView1.SortedColumn.Name;
         SortColumnOrder = dataGridView1.SortOrder;

         SaveSortColumn(); // Save Column Sort Order - Issue 73
         _Prefs.Save();

         SelectCurrentRowKey();
      }

      /// <summary>
      /// 
      /// </summary>
      private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo info = dataGridView1.HitTest(e.X, e.Y);
         
         //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format(CultureInfo.CurrentCulture,
         //   "MouseMove x:{0} y:{1} / row:{2} column:{3}", e.X, e.Y, info.RowIndex, info.ColumnIndex));
         
         // Only draw Tooltips if we've actually changed cells - Issue 99
         if (info.RowIndex == CurrentMouseOverRow && info.ColumnIndex == CurrentMouseOverColumn)
         {
            return;
         }
         else
         {
            // Update the current cell indexes
            CurrentMouseOverRow = info.RowIndex;
            CurrentMouseOverColumn = info.ColumnIndex;
         }

         #region Draw or Hide the Tooltip
         if (info.RowIndex > -1)
         {
            ClientInstance Instance =
               _clientInstances.Instances[dataGridView1.Rows[info.RowIndex].Cells["Name"].Value.ToString()];

            if (dataGridView1.Columns["Status"].Index == info.ColumnIndex)
            {
               //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Drawing Status Tooltip...");
               toolTipGrid.Show(Instance.Status.ToString(), dataGridView1, e.X + 15, e.Y);
               return;
            }
            else if (dataGridView1.Columns["Username"].Index == info.ColumnIndex)
            {
               if (Instance.IsUsernameOk() == false)
               {
                  toolTipGrid.Show("Client's User Name does not match the configured User Name", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
            else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == info.ColumnIndex)
            {
               if (_Prefs.GetPreference<bool>(Preference.DuplicateProjectCheck) && Instance.CurrentUnitInfo.ProjectIsDuplicate)
               {
                  toolTipGrid.Show("Client is working on the same work unit as another client", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
            else if (dataGridView1.Columns["Name"].Index == info.ColumnIndex)
            {
               if (_Prefs.GetPreference<bool>(Preference.DuplicateUserIdCheck) && Instance.UserIdIsDuplicate)
               {
                  toolTipGrid.Show("Client is working with the same User and Machine ID as another client", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
         }

         toolTipGrid.Hide(dataGridView1);
         #endregion
      }

      /// <summary>
      /// Override Cell Painting in the DataGridView
      /// </summary>
      private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
      {
         if (e.RowIndex >= 0)
         {
            if (dataGridView1.Columns["Status"].Index == e.ColumnIndex)
            {
               PaintGridCell(PaintCell.Status, e);
            }
            else if (dataGridView1.Columns["Name"].Index == e.ColumnIndex)
            {
               #region Duplicate User and Machine ID Custom Paint
               ClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<bool>(Preference.DuplicateUserIdCheck) && instance.UserIdIsDuplicate)
               {
                  PaintGridCell(PaintCell.Warning, e);
               }
               #endregion
            }
            else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == e.ColumnIndex)
            {
               #region Duplicate Project Custom Paint
               ClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<bool>(Preference.DuplicateProjectCheck) && instance.CurrentUnitInfo.ProjectIsDuplicate)
               {
                  PaintGridCell(PaintCell.Warning, e);
               }
               #endregion
            }
            else if (dataGridView1.Columns["Username"].Index == e.ColumnIndex)
            {
               #region Username Incorrect Custom Paint
               ClientInstance Instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (Instance.IsUsernameOk() == false)
               {
                  PaintGridCell(PaintCell.Warning, e);
               }
               #endregion
            }
            else if ((dataGridView1.Columns["TPF"].Index == e.ColumnIndex ||
                      dataGridView1.Columns["ETA"].Index == e.ColumnIndex ||
                      dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
                      dataGridView1.Columns["Deadline"].Index == e.ColumnIndex) &&
                      _Prefs.GetPreference<TimeStyleType>(Preference.TimeStyle).Equals(TimeStyleType.Formatted))
            {
               PaintGridCell(PaintCell.Time, e);
            }
            else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
                     dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
            {
               DateTime date = (DateTime)e.Value;
               if (date.Equals(DateTime.MinValue))
               {
                  PaintGridCell(PaintCell.Time, e);
               }
            }
         }
      }

      /// <summary>
      /// Custom Paint Grid Cells
      /// </summary>
      private void PaintGridCell(PaintCell paint, DataGridViewCellPaintingEventArgs e)
      {
         using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor),
                      backColorBrush = new SolidBrush(e.CellStyle.BackColor),
                      selectionColorBrush = new SolidBrush(e.CellStyle.SelectionBackColor))
         {
            using (Pen gridLinePen = new Pen(gridBrush))
            {
               #region Erase (Set BackColor) the Cell and Choose Text Color
               Brush textColor = Brushes.Black;
               
               if (paint.Equals(PaintCell.Status))
               {
                  e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
               }
               else if (paint.Equals(PaintCell.Time))
               {
                  if (dataGridView1.Rows[e.RowIndex].Selected)
                  {
                     e.Graphics.FillRectangle(selectionColorBrush, e.CellBounds);
                     textColor = Brushes.White;
                  }
                  else
                  {
                     e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                  }
               }
               else if (paint.Equals(PaintCell.Warning))
               {
                  if (dataGridView1.Rows[e.RowIndex].Selected)
                  {
                     e.Graphics.FillRectangle(selectionColorBrush, e.CellBounds);
                     textColor = Brushes.White;
                  }
                  else
                  {
                     e.Graphics.FillRectangle(Brushes.Orange, e.CellBounds);
                  }
               }
               else
               {
                  throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
                     "PaintCell Type '{0}' is not implemented", paint));
               }
               #endregion

               #region Draw the bottom grid line
               e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                   e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                   e.CellBounds.Bottom - 1);
               #endregion

               #region Draw Cell Content (Text or Shapes)
               if (paint.Equals(PaintCell.Status))
               {
                  Rectangle newRect = new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4,
                                                    e.CellBounds.Width - 10, e.CellBounds.Height - 10);

                  // Draw the inset highlight box.
                  ClientStatus status = (ClientStatus)e.Value;
                  e.Graphics.DrawRectangle(ClientInstance.GetStatusPen(status), newRect);
                  e.Graphics.FillRectangle(ClientInstance.GetStatusBrush(status), newRect);
               }
               else if (paint.Equals(PaintCell.Time))
               {
                  if (e.Value != null)
                  {
                     PaintTimeBasedCellValue(textColor, e);
                  }
               }
               else if (paint.Equals(PaintCell.Warning))
               {
                  // Draw the text content of the cell, ignoring alignment.
                  if (e.Value != null)
                  {
                     e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                                           textColor, e.CellBounds.X + 2,
                                           e.CellBounds.Y + 2, StringFormat.GenericDefault);
                  }
               }
               else
               {
                  throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
                     "PaintCell Type '{0}' is not implemented", paint));
               }
               #endregion

               e.Handled = true;
            }
         }
      }
      
      /// <summary>
      /// Paint the Time based cells with the custom time format
      /// </summary>
      private void PaintTimeBasedCellValue(Brush textColor, DataGridViewCellPaintingEventArgs e)
      {
         string DrawString = String.Empty;
      
         if (dataGridView1.Columns["TPF"].Index == e.ColumnIndex)
         {
            TimeSpan span = (TimeSpan)e.Value;
            DrawString = GetFormattedTpfString(span);
         }
         else if (dataGridView1.Columns["ETA"].Index == e.ColumnIndex)
         {
            TimeSpan span = (TimeSpan)e.Value;
            DrawString = GetFormattedEtaString(span);
         }
         else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex)
         {
            DateTime date = (DateTime)e.Value;
            DrawString = GetFormattedDownloadTimeString(date);
         }
         else if (dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
         {
            DateTime date = (DateTime)e.Value;
            DrawString = GetFormattedDeadlineString(date);
         }
         
         if (DrawString.Length != 0)
         {
            e.Graphics.DrawString(DrawString, e.CellStyle.Font,
                  textColor, e.CellBounds.X + 2,
                  e.CellBounds.Y + 2, StringFormat.GenericDefault);
         }
      }

      /// <summary>
      /// Measure Text and set Column Width on Double-Click
      /// </summary>
      private void dataGridView1_ColumnDividerDoubleClick(object sender, DataGridViewColumnDividerDoubleClickEventArgs e)
      {
         Font font = new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Regular);
         //Graphics g = dataGridView1.CreateGraphics();

         SizeF s;
         int width = 0;

         for (int i = 0; i < dataGridView1.Rows.Count; i++)
         {
            if (dataGridView1.Rows[i].Cells[e.ColumnIndex].Value != null)
            {
               string formattedString = String.Empty;
               
               if ((dataGridView1.Columns["TPF"].Index == e.ColumnIndex || 
                    dataGridView1.Columns["ETA"].Index == e.ColumnIndex ||
                    dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
                    dataGridView1.Columns["Deadline"].Index == e.ColumnIndex) &&
                    _Prefs.GetPreference<TimeStyleType>(Preference.TimeStyle).Equals(TimeStyleType.Formatted))
               {
                  if (dataGridView1.Columns["TPF"].Index == e.ColumnIndex)
                  {
                     formattedString =
                        GetFormattedTpfString((TimeSpan)dataGridView1.Rows[i].Cells[e.ColumnIndex].Value);
                  }
                  else if (dataGridView1.Columns["ETA"].Index == e.ColumnIndex)
                  {
                     formattedString =
                        GetFormattedEtaString((TimeSpan)dataGridView1.Rows[i].Cells[e.ColumnIndex].Value);
                  }
                  else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex)
                  {
                     formattedString =
                        GetFormattedDownloadTimeString((DateTime)dataGridView1.Rows[i].Cells[e.ColumnIndex].Value);
                  }
                  else if (dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
                  {
                     formattedString =
                        GetFormattedDeadlineString((DateTime)dataGridView1.Rows[i].Cells[e.ColumnIndex].Value);
                  }
               }
               else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
                        dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
               {
                  formattedString = 
                     GetFormattedDateStringForMeasurement((DateTime)dataGridView1.Rows[i].Cells[e.ColumnIndex].Value,
                                                                    dataGridView1.Rows[i].Cells[e.ColumnIndex].FormattedValue.ToString());
               }
               else
               {
                  formattedString = dataGridView1.Rows[i].Cells[e.ColumnIndex].FormattedValue.ToString();
               }

               s = TextRenderer.MeasureText(formattedString, font);
               //s = g.MeasureString(formattedString, font);

               if (width < s.Width)
               {
                  width = (int)(s.Width + 1);
               }
            }
         }

         dataGridView1.Columns[e.ColumnIndex].Width = width;

         e.Handled = true;
      }

      #region Custom String Formatting Helpers
      private static string GetFormattedTpfString(TimeSpan span)
      {
         string formatString = "{1:00}min {2:00}sec";
         if (span.Hours > 0)
         {
            formatString = "{0:00}hr {1:00}min {2:00}sec";
         }

         return String.Format(formatString, span.Hours, span.Minutes, span.Seconds);
      }

      private static string GetFormattedEtaString(TimeSpan span)
      {
         string formatString = "{1:00}hr {2:00}min";
         if (span.Days > 0)
         {
            formatString = "{0}d {1:00}hr {2:00}min";
         }

         return String.Format(formatString, span.Days, span.Hours, span.Minutes);
      }

      private static string GetFormattedDownloadTimeString(DateTime date)
      {
         if (date.Equals(DateTime.MinValue))
         {
            return "Unknown";
         }

         TimeSpan span = DateTime.Now.Subtract(date);
         string formatString = "{1:00}hr {2:00}min ago";
         if (span.Days > 0)
         {
            formatString = "{0}d {1:00}hr {2:00}min ago";
         }

         return String.Format(formatString, span.Days, span.Hours, span.Minutes);
      }

      private static string GetFormattedDeadlineString(DateTime date)
      {
         if (date.Equals(DateTime.MinValue))
         {
            return "Unknown";
         }

         TimeSpan span = date.Subtract(DateTime.Now);
         string formatString = "In {1:00}hr {2:00}min";
         if (span.Days > 0)
         {
            formatString = "In {0}d {1:00}hr {2:00}min";
         }

         return String.Format(formatString, span.Days, span.Hours, span.Minutes);
      } 
      
      private static string GetFormattedDateStringForMeasurement(IEquatable<DateTime> date, string formattedValue)
      {
         if (date.Equals(DateTime.MinValue))
         {
            return "Unknown";
         }
         
         return formattedValue;
      }
      #endregion

      /// <summary>
      /// Handle Right-Click (Contest Menu) and Left Double-Click (File Browser)
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo hti = dataGridView1.HitTest(e.X, e.Y);
         if (e.Button == MouseButtons.Right)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               if (dataGridView1.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected == false)
               {
                  //dataGridView1.ClearSelection();
                  dataGridView1.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected = true;
               }

               // Check for SelectedInstance, and get out if not found
               if (_clientInstances.SelectedInstance == null) return;

               if (_clientInstances.SelectedInstance.InstanceHostType.Equals(InstanceType.PathInstance))
               {
                  mnuContextClientsViewClientFiles.Visible = true;
               }
               else
               {
                  mnuContextClientsViewClientFiles.Visible = false;
               }
               gridContextMenuStrip.Show(dataGridView1.PointToScreen(new Point(e.X, e.Y)));
            }
         }
         if (e.Button == MouseButtons.Left && e.Clicks == 2)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               // Check for SelectedInstance, and get out if not found
               if (_clientInstances.SelectedInstance == null) return;

               if (_clientInstances.SelectedInstance.InstanceHostType.Equals(InstanceType.PathInstance))
               {
                  try
                  {
                     StartFileBrowser(_clientInstances.SelectedInstance.Path);
                  }
                  catch (Exception ex)
                  {
                     HfmTrace.WriteToHfmConsole(ex);
                     
                     MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError,
                        String.Format(CultureInfo.CurrentCulture, "client '{0}' files.{1}{1}Please check the current File Explorer defined in the Preferences",
                        _clientInstances.SelectedInstance.InstanceName, Environment.NewLine)));
                  }
               }
            }
         }
      }
      #endregion

      #region File Menu Click Handlers
      /// <summary>
      /// Create a new host configuration
      /// </summary>
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
            ClearUI();
         }
      }

      /// <summary>
      /// Open an existing (saved) host configuration
      /// </summary>
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
            openConfigDialog.DefaultExt = "hfm";
            openConfigDialog.Filter = "HFM Configuration Files|*.hfm";
            openConfigDialog.FileName = _clientInstances.ConfigFilename;
            openConfigDialog.RestoreDirectory = true;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
            {
               LoadFile(openConfigDialog.FileName);
            }
         }
      }

      /// <summary>
      /// Save the current host configuration
      /// </summary>
      private void mnuFileSave_Click(object sender, EventArgs e)
      {
         if (_clientInstances.HasConfigFilename == false)
         {
            mnuFileSaveas_Click(sender, e);
         }
         else
         {
            try
            {
               _clientInstances.ToXml();
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

      /// <summary>
      /// Save the current host configuration as a new file
      /// </summary>
      private void mnuFileSaveas_Click(object sender, EventArgs e)
      {
         // No Config File and no Instances, stub out
         if (_clientInstances.HasConfigFilename == false && _clientInstances.HasInstances == false) return;
      
         if (saveConfigDialog.ShowDialog() == DialogResult.OK)
         {
            try
            {
               _clientInstances.ToXml(saveConfigDialog.FileName); // Issue 75
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

      /// <summary>
      /// Import FahMon clientstab.txt configuration
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileImportFahMon_Click(object sender, EventArgs e)
      {
         if (_clientInstances.RetrievalInProgress)
         {
            MessageBox.Show(this, "Retrieval in progress... please wait to open another config file.",
               Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (CanContinueDestructiveOp(sender, e))
         {
            openConfigDialog.DefaultExt = "txt";
            openConfigDialog.Filter = "Text Files|*.txt";
            openConfigDialog.FileName = "clientstab.txt";
            openConfigDialog.RestoreDirectory = true;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
            {
               ImportFahMonFile(openConfigDialog.FileName);
            }
         }
      }

      /// <summary>
      /// Exit the application
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
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
         frmPreferences prefDialog = new frmPreferences(_Prefs);
         prefDialog.ShowDialog();
         
         dataGridView1.Invalidate();
      }
      #endregion

      #region Help Menu Click Handlers
      private void mnuHelpHfmLogFile_Click(object sender, EventArgs e)
      {
         string logPath = Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath),
                                       Constants.HfmLogFileName);
         try
         {
            Process.Start(_Prefs.GetPreference<string>(Preference.LogFileViewer), logPath);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, 
               String.Format(CultureInfo.CurrentCulture, "the HFM.log file.{0}{0}Please check the current Log File Viewer defined in the Preferences",
               Environment.NewLine)));
         }
      }

      private void mnuHelpHfmDataFiles_Click(object sender, EventArgs e)
      {
         try
         {
            StartFileBrowser(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError,
               String.Format(CultureInfo.CurrentCulture, "the HFM.NET data files.{0}{0}Please check the current File Explorer defined in the Preferences",
               Environment.NewLine)));
         }
      }

      private void mnuHelpHfmGroup_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start("http://groups.google.com/group/hfm-net/");
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "HFM.NET Google Group"));
         }
      }
      
      private void mnuHelpCheckForUpdate_Click(object sender, EventArgs e)
      {
         CheckForUpdate(true);
      }
      
      /// <summary>
      /// Show the About dialog
      /// </summary>
      private void mnuHelpAbout_Click(object sender, EventArgs e)
      {
         frmAbout newAbout = new frmAbout();
         newAbout.ShowDialog(this);
      }

      /// <summary>
      /// Show the help file at the contents tab
      /// </summary>
      private void mnuHelpContents_Click(object sender, EventArgs e)
      {
         //Help.ShowHelp(this, helpProvider.HelpNamespace);
      }

      /// <summary>
      /// Show the help file at the index tab
      /// </summary>
      private void mnuHelpIndex_Click(object sender, EventArgs e)
      {
         //Help.ShowHelpIndex(this, helpProvider.HelpNamespace);
      }
      #endregion

      #region Clients Menu Click Handlers
      /// <summary>
      /// Add a new host to the configuration
      /// </summary>
      private void mnuClientsAdd_Click(object sender, EventArgs e)
      {
         var newHost = new frmHost(new ClientInstanceSettings(InstanceType.PathInstance));
         if (newHost.ShowDialog().Equals(DialogResult.OK))
         {
            try
            {
               _clientInstances.Add(newHost.Settings);
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
         }
      }

      /// <summary>
      /// Edits an existing host configuration
      /// </summary>
      private void mnuClientsEdit_Click(object sender, EventArgs e)
      {
         // Check for SelectedInstance, and get out if not found
         if (_clientInstances.SelectedInstance == null) return;

         string previousName = _clientInstances.SelectedInstance.InstanceName;
         string previousPath = _clientInstances.SelectedInstance.Path;

         var editHost = new frmHost(_clientInstances.SelectedInstance.Settings.Clone());
         if (editHost.ShowDialog().Equals(DialogResult.OK))
         {
            try
            {
               _clientInstances.Edit(previousName, previousPath, editHost.Settings);
            }
            catch (InvalidOperationException ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
         }
      }

      /// <summary>
      /// Removes a host from the configuration
      /// </summary>
      private void mnuClientsDelete_Click(object sender, EventArgs e)
      {
         // Check for SelectedInstance, and get out if not found
         if (_clientInstances.SelectedInstance == null) return;
         
         _clientInstances.Remove(_clientInstances.SelectedInstance.InstanceName);
      }

      /// <summary>
      /// Refreshes the currently selected client
      /// </summary>
      private void mnuClientsRefreshSelected_Click(object sender, EventArgs e)
      {
         // Check for SelectedInstance, and get out if not found
         if (_clientInstances.SelectedInstance == null) return;
      
         _clientInstances.RetrieveSingleClient(_clientInstances.SelectedInstance.InstanceName);
      }

      /// <summary>
      /// Refreshes all clients
      /// </summary>
      private void mnuClientsRefreshAll_Click(object sender, EventArgs e)
      {
         _clientInstances.QueueNewRetrieval();
      }

      /// <summary>
      /// Fire process to show the cached FAHLog file
      /// </summary>
      private void mnuClientsViewCachedLog_Click(object sender, EventArgs e)
      {
         // Check for SelectedInstance, and get out if not found
         if (_clientInstances.SelectedInstance == null) return;

         string logPath = Path.Combine(_Prefs.CacheDirectory, _clientInstances.SelectedInstance.CachedFAHLogName);
         if (File.Exists(logPath))
         {
            try
            {
               Process.Start(_Prefs.GetPreference<string>(Preference.LogFileViewer), logPath);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               
               MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError,
                  String.Format(CultureInfo.CurrentCulture, "client '{0}' FAHlog file.{1}{1}Please check the current Log File Viewer defined in the Preferences",
                  _clientInstances.SelectedInstance.InstanceName, Environment.NewLine)));
            }
         }
         else
         {
            MessageBox.Show(String.Format("Cannot find client '{0}' FAHlog.txt file.", _clientInstances.SelectedInstance.InstanceName));
         }
      }

      /// <summary>
      /// Fire process to show the current client file directory (Local Clients Only)
      /// </summary>
      private void mnuClientsViewClientFiles_Click(object sender, EventArgs e)
      {
         // Check for SelectedInstance, and get out if not found
         if (_clientInstances.SelectedInstance == null) return;

         if (_clientInstances.SelectedInstance.InstanceHostType.Equals(InstanceType.PathInstance))
         {
            try
            {
               StartFileBrowser(_clientInstances.SelectedInstance.Path);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               
               MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError,
                  String.Format(CultureInfo.CurrentCulture, "client '{0}' files.{1}{1}Please check the current File Explorer defined in the Preferences",
                  _clientInstances.SelectedInstance.InstanceName, Environment.NewLine)));
            }
         }
      }
      #endregion

      #region View Menu Click Handlers
      private void mnuViewMessages_Click(object sender, EventArgs e)
      {
         if (_frmMessages.Visible)
         {
            _frmMessages.Close();
         }
         else
         {
            // Restore state data
            Point location = _Prefs.GetPreference<Point>(Preference.MessagesFormLocation);
            Size size = _Prefs.GetPreference<Size>(Preference.MessagesFormSize);

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
         if (_Prefs.GetPreference<TimeStyleType>(Preference.TimeStyle).Equals(TimeStyleType.Standard))
         {
            _Prefs.SetPreference(Preference.TimeStyle, TimeStyleType.Formatted);
         }
         else
         {
            _Prefs.SetPreference(Preference.TimeStyle, TimeStyleType.Standard);
         }

         dataGridView1.Invalidate();
      }

      private void mnuViewToggleCompletedCountStyle_Click(object sender, EventArgs e)
      {
         if (_Prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay).Equals(CompletedCountDisplayType.ClientTotal))
         {
            _Prefs.SetPreference(Preference.CompletedCountDisplay, CompletedCountDisplayType.ClientRunTotal);
         }
         else
         {
            _Prefs.SetPreference(Preference.CompletedCountDisplay, CompletedCountDisplayType.ClientTotal);
         }

         RefreshDisplay();
      }
      #endregion

      #region Tools Menu Click Handlers
      /// <summary>
      /// Download Project Info From Stanford
      /// </summary>
      private void mnuToolsDownloadProjects_Click(object sender, EventArgs e)
      {
         IProteinCollection proteinCollection = InstanceProvider.GetInstance<IProteinCollection>();

         // Clear the Project Not Found Cache and Last Download Time
         proteinCollection.ClearProjectsNotFoundCache();
         proteinCollection.Downloader.ResetLastDownloadTime();
         // Execute Asynchronous Download
         proteinCollection.BeginDownloadFromStanford();
      }

      /// <summary>
      /// Show the Benchmarks Dialog
      /// </summary>
      private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
      {
         int projectId = 0;
      
         // Check for SelectedInstance, and if found... load its ProjectID.
         if (_clientInstances.SelectedInstance != null)
         {
            projectId = _clientInstances.SelectedInstance.CurrentUnitInfo.ProjectID;
         }

         frmBenchmarks frm = new frmBenchmarks(_Prefs, InstanceProvider.GetInstance<IProteinBenchmarkContainer>(), _clientInstances, projectId);
         frm.StartPosition = FormStartPosition.Manual;

         // Restore state data
         Point location = _Prefs.GetPreference<Point>(Preference.BenchmarksFormLocation);
         Size size = _Prefs.GetPreference<Size>(Preference.BenchmarksFormSize);

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
      #endregion

      #region Web Menu Click Handlers
      private void mnuWebEOCUser_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start(_Prefs.EocUserUrl.AbsoluteUri);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "EOC User Stats page"));
         }
      }

      private void mnuWebStanfordUser_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start(_Prefs.StanfordUserUrl.AbsoluteUri);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "Stanford User Stats page"));
         }
      }

      private void mnuWebEOCTeam_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start(_Prefs.EocTeamUrl.AbsoluteUri);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "EOC Team Stats page"));
         }
      }

      private void mnuWebRefreshUserStats_Click(object sender, EventArgs e)
      {
         RefreshUserStatsData(true);
      }

      private void mnuWebHFMGoogleCode_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start("http://code.google.com/p/hfm-net/");
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "HFM.NET Google Code page"));
         }
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
            _clientInstances.RefreshDisplayCollection();
            if (dataGridView1.DataSource != null)
            {
               // Freeze the SelectionChanged Event when doing currency refresh
               dataGridView1.FreezeSelectionChanged = true;
            
               CurrencyManager cm = (CurrencyManager) dataGridView1.BindingContext[dataGridView1.DataSource];
               if (cm != null)
               {
                  if (InvokeRequired)
                  {
                     Invoke(new MethodInvoker(cm.Refresh));
                  }
                  else
                  {
                     cm.Refresh();
                  }
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
            if (_Prefs.GetPreference<bool>(Preference.MaintainSelectedClient))
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
               _clientInstances.SetCurrentInstance(dataGridView1.SelectedRows);
            }
            _clientInstances.RaiseSelectedInstanceChanged();
         }
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
            
            if (String.IsNullOrEmpty(SortColumnName) == false && SortColumnOrder.Equals(SortOrder.None) == false)
            {
               if (SortColumnOrder.Equals(SortOrder.Ascending))
               {
                  dataGridView1.Sort(dataGridView1.Columns[SortColumnName], ListSortDirection.Ascending);
                  dataGridView1.SortedColumn.HeaderCell.SortGlyphDirection = SortColumnOrder;
               }
               else if (SortColumnOrder.Equals(SortOrder.Descending))
               {
                  dataGridView1.Sort(dataGridView1.Columns[SortColumnName], ListSortDirection.Descending);
                  dataGridView1.SortedColumn.HeaderCell.SortGlyphDirection = SortColumnOrder;
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

         double TotalPPD = totals.PPD;
         int GoodHosts = totals.WorkingClients;

         SetNotifyIconText(String.Format("{0} Working Clients{3}{1} Non-Working Clients{3}{2:" + _Prefs.PpdFormatString + "} PPD",
                                         GoodHosts, totals.NonWorkingClients, TotalPPD, Environment.NewLine));
         RefreshStatusLabels(GoodHosts, TotalPPD);
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
         if (notifyIcon != null)
         {
            if (val.Length > 64)
            {
               //if string is too long, remove the word Clients
               val = val.Replace("Clients", String.Empty);
            }
            notifyIcon.Text = val;
         }
      }

      /// <summary>
      /// Update the Status Labels with Client and PPD info
      /// </summary>
      private void RefreshStatusLabels(int GoodHosts, double TotalPPD)
      {
         if (GoodHosts == 1)
         {
            SetStatusLabelHostsText(String.Format("{0} Client", GoodHosts));
         }
         else
         {
            SetStatusLabelHostsText(String.Format("{0} Clients", GoodHosts));
         }

         SetStatusLabelPPDText(String.Format("{0:" + _Prefs.PpdFormatString + "} PPD", TotalPPD));
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
      private void notifyIcon_DoubleClick(object sender, EventArgs e)
      {
         if (WindowState == FormWindowState.Minimized)
         {
            WindowState = originalState;
         }
         else
         {
            originalState = WindowState;
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
            WindowState = originalState;
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
            originalState = WindowState;
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
            originalState = WindowState;
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
         Point location = _Prefs.GetPreference<Point>(Preference.FormLocation);
         Size size = _Prefs.GetPreference<Size>(Preference.FormSize);

         if (location.X != 0 && location.Y != 0)
         {
            // Set StartPosition to manual
            StartPosition = FormStartPosition.Manual;
            Location = location;
         }
         if (size.Width != 0 && size.Height != 0)
         {
            if (_Prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
            {
               size = new Size(size.Width, size.Height + _Prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            }
            Size = size;
            splitContainer1.SplitterDistance = _Prefs.GetPreference<int>(Preference.FormSplitLocation);
         }

         if (_Prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
         {
            ShowHideLog(false);
         }

         if (_Prefs.GetPreference<bool>(Preference.QueueViewerVisible) == false)
         {
            ShowHideQueue(false);
         }

         //if (Prefs.FormSortColumn != String.Empty &&
         //    Prefs.FormSortOrder != SortOrder.None)
         //{
            SortColumnName = _Prefs.GetPreference<string>(Preference.FormSortColumn);
            SortColumnOrder = _Prefs.GetPreference<SortOrder>(Preference.FormSortOrder);
         //}
         
         try
         {
            // Restore the columns' state
            StringCollection cols = _Prefs.GetPreference<StringCollection>(Preference.FormColumns);
            string[] colsArray = new string[cols.Count];
            
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
         StringCollection stringCollection = new StringCollection();
         int i = 0;

         foreach (DataGridViewColumn column in dataGridView1.Columns)
         {
            stringCollection.Add(String.Format(
                                    "{0},{1},{2},{3}",
                                    column.DisplayIndex.ToString("D2"),
                                    column.Width,
                                    column.Visible,
                                    i++));
         }

         _Prefs.SetPreference(Preference.FormColumns, stringCollection);
      }

      /// <summary>
      /// Save Sorted Column Name and Order
      /// </summary>
      private void SaveSortColumn()
      {
         _Prefs.SetPreference(Preference.FormSortColumn, SortColumnName);
         _Prefs.SetPreference(Preference.FormSortOrder, SortColumnOrder);
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
                  if (_clientInstances.ChangedAfterSave)
                  {
                     return false;
                  }
                  else
                  {
                     return true;
                  }
               case DialogResult.No:
                  return true;
               case DialogResult.Cancel:
                  return false;
            }
            return false;
         }
         else
         {
            return true;
         }
      }

      /// <summary>
      /// Set the UI to an empty (No Clients) state
      /// </summary>
      private void ClearUI()
      {
         // Clear the instances controller
         _clientInstances.Clear();
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
      /// <param name="Filename">File to load</param>
      private void LoadFile(string Filename)
      {
         // Clear the UI
         ClearUI();
      
         try
         {
            // Read the config file
            _clientInstances.FromXml(Filename);

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
      /// Imports a FahMon configuration file into memory
      /// </summary>
      /// <param name="filename"></param>
      private void ImportFahMonFile(string filename)
      {
         // Clear the UI
         ClearUI();
         
         try
         {
            // Read the config file
            _clientInstances.FromFahMonClientsTab(filename);

            if (_clientInstances.HasInstances == false)
            {
               MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, 
                  "No client configurations were imported from the given config file.{0}{0}Possibly because the file is in an older FahMon format (not tab delimited).{0}{0}Later versions of FahMon write a clientstab.txt file in tab delimited format.", Environment.NewLine),
                  Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
               "No client configurations were imported from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message),
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
            _Prefs.SetPreference(Preference.FormLogWindowHeight, (splitContainer1.Height - splitContainer1.SplitterDistance));
            Size = new Size(Size.Width, Size.Height - _Prefs.GetPreference<int>(Preference.FormLogWindowHeight));
         }
         else
         {
            txtLogFile.Visible = true;
            Resize -= frmMain_Resize; // disable Form resize event for this operation
            Size = new Size(Size.Width, Size.Height + _Prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            Resize += frmMain_Resize; // re-enable
            splitContainer1.Panel2Collapsed = false;
         }
      }

      /// <summary>
      /// Fire File Browser Process (wrap calls to this function in try catch)
      /// </summary>
      /// <param name="path">The Folder Path to browse</param>
      private void StartFileBrowser(string path)
      {
         Process.Start(_Prefs.GetPreference<string>(Preference.FileExplorer), path);
      }

      #region User Stats Data Methods
      /// <summary>
      /// Refresh User Stats from external source
      /// </summary>
      private void RefreshUserStatsData(bool forceRefresh)
      {
         try
         {
            _statsData.GetEocXmlData(forceRefresh);
            statusLabel24hr.Text = String.Format("24hr: {0:###,###,##0}", _statsData.Data.TwentyFourHourAvgerage);
            statusLabelToday.Text = String.Format("Today: {0:###,###,##0}", _statsData.Data.PointsToday);
            statusLabelWeek.Text = String.Format("Week: {0:###,###,##0}", _statsData.Data.PointsWeek);
            statusLabelTotal.Text = String.Format("Total: {0:###,###,##0}", _statsData.Data.PointsTotal);
            statusLabelWUs.Text = String.Format("WUs: {0:###,###,##0}", _statsData.Data.WorkUnitsTotal);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }
      #endregion

      #region Instance Collection Event Handlers
      private void ClientInstances_InstanceDataChanged(object sender, EventArgs e)
      {
         if (_Prefs.GetPreference<bool>(Preference.AutoSaveConfig))
         {
            mnuFileSave_Click(sender, e);
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
         switch (_Prefs.GetPreference<FormShowStyleType>(Preference.FormShowStyle))
         {
            case FormShowStyleType.SystemTray:
               if (notifyIcon != null) notifyIcon.Visible = true;
               ShowInTaskbar = (WindowState != FormWindowState.Minimized);
               break;
            case FormShowStyleType.TaskBar:
               if (notifyIcon != null) notifyIcon.Visible = false;
               ShowInTaskbar = true;
               break;
            case FormShowStyleType.Both:
               if (notifyIcon != null) notifyIcon.Visible = true;
               ShowInTaskbar = true;
               break;
         }
      }

      /// <summary>
      /// Show or Hide User Stats Controls based on user setting
      /// </summary>
      private void PreferenceSet_ShowUserStatsChanged(object sender, EventArgs e)
      {
         XmlStatsVisible(_Prefs.GetPreference<bool>(Preference.ShowUserStats));
      }

      private void XmlStatsVisible(bool visible)
      {
         if (visible)
         {
            mnuWebRefreshUserStats.Visible = true;
            mnuWebSep2.Visible = true;
            RefreshUserStatsData(false);
         }
         else
         {
            mnuWebRefreshUserStats.Visible = false;
            mnuWebSep2.Visible = false;
         }

         statusLabel24hr.Visible = visible;
         statusLabelToday.Visible = visible;
         statusLabelWeek.Visible = visible;
         statusLabelTotal.Visible = visible;
         statusLabelWUs.Visible = visible;
         statusLabelMiddle.Visible = visible;
      }

      /// <summary>
      /// Sets Debug Message Level on Trace Level Switch
      /// </summary>
      private void PreferenceSet_MessageLevelChanged(object sender, EventArgs e)
      {
         TraceLevel newLevel = (TraceLevel)_Prefs.GetPreference<int>(Preference.MessageLevel);
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
         if (_Prefs.GetPreference<bool>(Preference.ColorLogFile))
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
