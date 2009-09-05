/*
 * HFM.NET - Main UI Form
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
 * 
 * Form and DataGridView save state code by Ron Dunant, modified by harlam357.
 * http://www.codeproject.com/KB/grid/PersistentDataGridView.aspx
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using HFM.Classes;
using HFM.Helpers;
using HFM.Instances;
using HFM.Instrumentation;
using HFM.Proteins;
using HFM.Preferences;

namespace HFM.Forms
{
   public partial class frmMain : FormWrapper
   {
      #region Private Variables
      /// <summary>
      /// Collection of host instances
      /// </summary>
      private readonly FoldingInstanceCollection HostInstances = new FoldingInstanceCollection();
      
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
      /// True when in the ApplySort() routine
      /// </summary>
      private bool InApplySort = false;
      
      /// <summary>
      /// Messages Form
      /// </summary>
      private readonly frmMessages _frmMessages = null;

      /// <summary>
      /// Notify Icon for frmMain
      /// </summary>
      private NotifyIcon notifyIcon = null;
      #endregion

      #region Form Constructor / functionality
      /// <summary>
      /// Main form constructor
      /// </summary>
      public frmMain()
      {
         // This call is Required by the Windows Form Designer
         InitializeComponent();

         // Set Main Form Text
         base.Text = String.Format("HFM.NET {0} - Beta", PlatformOps.ApplicationVersionString);

         // Create Messages Window
         _frmMessages = new frmMessages();
         
         // Setup Log File and Messages Window handlers
         SetupTraceListeners();
         
         // Hook up Protein Collection Updated Event Handler
         ProteinCollection.Instance.ProjectInfoUpdated += Instance_ProjectInfoUpdated;
         
         // Clear the Log File Cache Folder
         ClearCacheFolder();
         // Manually Create the Columns - Issue 41
         DisplayInstance.SetupDataGridViewColumns(dataGridView1);
         // Clear the UI
         ClearUI();
         // Restore Form Preferences
         RestoreFormPreferences();
         // Add Column Selector
         new DataGridViewColumnSelector(dataGridView1);

         // Hook-up Instance Collection Event Handlers
         dataGridView1.AutoGenerateColumns = false;
         dataGridView1.DataSource = HostInstances.GetDisplayCollection();
         HostInstances.CollectionChanged += HostInstances_CollectionChanged;
         //HostInstances.CollectionLoaded += HostInstances_CollectionLoaded;
         //HostInstances.CollectionSaved += HostInstances_CollectionSaved;
         HostInstances.InstanceAdded += HostInstances_InstanceDataChanged;
         HostInstances.InstanceEdited += HostInstances_InstanceDataChanged;
         HostInstances.InstanceRemoved += HostInstances_InstanceDataChanged;
         HostInstances.InstanceRetrieved += HostInstances_InstanceRetrieved;
         HostInstances.DuplicatesFoundOrChanged += HostInstances_DuplicatesFoundOrChanged;
         HostInstances.RefreshUserStatsData += HostInstances_RefreshUserStatsData;

         // Hook-up PreferenceSet Event Handlers
         PreferenceSet Prefs = PreferenceSet.Instance;
         Prefs.ShowUserStatsChanged += PreferenceSet_ShowUserStatsChanged;
         PreferenceSet_ShowUserStatsChanged(this, EventArgs.Empty);
         Prefs.OfflineLastChanged += PreferenceSet_OfflineLastChanged;
         PreferenceSet_OfflineLastChanged(this, EventArgs.Empty);
         Prefs.MessageLevelChanged += PreferenceSet_MessageLevelChanged;
         Prefs.DuplicateCheckChanged += PreferenceSet_DuplicateCheckChanged;
         Prefs.ColorLogFileChanged += PreferenceSet_ColorLogFileChanged;
      }

      /// <summary>
      /// Creates Trace Listener for Log File writing and Message Window output
      /// </summary>
      private void SetupTraceListeners()
      {
         FileInfo fi = new FileInfo("HFM.log");
         if (fi.Exists && fi.Length > 512000)
         {
            FileInfo fi2 = new FileInfo("HFM-prev.log");
            if (fi2.Exists)
            {
               fi2.Delete();
            }
            fi.MoveTo("HFM-prev.log");
         }

         TextWriterTraceListener listener = new TextWriterTraceListener("HFM.log");
         Trace.Listeners.Add(listener);
         Trace.AutoFlush = true;
         
         // Set Level to Warning to catch any errors that come from loading the preferences
         TraceLevelSwitch.Instance.Level = TraceLevel.Warning;

         HfmTrace.Instance.TextMessage += HfmTrace_TextMessage;
         HfmTrace.WriteToHfmConsole(String.Empty);
         HfmTrace.WriteToHfmConsole(String.Format("Starting - HFM.NET {0}", PlatformOps.ApplicationVersionStringWithRevision));
         HfmTrace.WriteToHfmConsole(String.Empty);
         
         // Get the actual TraceLevel from the preferences
         TraceLevelSwitch.Instance.Level = (TraceLevel)PreferenceSet.Instance.MessageLevel;
      }

      /// <summary>
      /// Loads the appropriate file on initialisation - ie when showing the form
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void frmMain_Shown(object sender, EventArgs e)
      {
         PreferenceSet Prefs = PreferenceSet.Instance;
      
         String Filename = String.Empty;

         if (Program.cmdArgs.Length > 0)
         {
            // Filename on command line - probably from Explorer
            Filename = Program.cmdArgs[0];
         }
         else if (Prefs.UseDefaultConfigFile)
         {
            Filename = Prefs.DefaultConfigFile;
         }

         if (String.IsNullOrEmpty(Filename) == false)
         {
            DoLoadFile(Filename);
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
         notifyIcon.Visible = true;
      }

      /// <summary>
      /// Hides the taskbar icon if the app is minimized
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void frmMain_Resize(object sender, EventArgs e)
      {
         if (WindowState != FormWindowState.Minimized)
         {
            originalState = WindowState;
            // ReApply Sort when restoring from the sys tray - Issue 32
            if (ShowInTaskbar == false)
            {
               ShowInTaskbar = true;
               ApplySort();
            }
         }
         else
         {
            ShowInTaskbar = false;
         }
         
         // When the log file window (panel) is collapsed, get the split location
         // changes based on the height of Panel1 - Issue 8
         if (Visible && splitContainer1.Panel2Collapsed)
         {
            PreferenceSet.Instance.FormSplitLocation = splitContainer1.Panel1.Height;
         }
      }

      /// <summary>
      /// Save Form State on before closing
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (CanContinueDestructiveOp(sender, e) == false)
         {
            e.Cancel = true;
            return;
         }
      
         PreferenceSet Prefs = PreferenceSet.Instance;

         SaveColumnSettings(Prefs);
         SaveSortColumn(Prefs);

         // Save location and size data
         // RestoreBounds remembers normal position if minimized or maximized
         if (WindowState == FormWindowState.Normal)
         {
            Prefs.FormLocation = Location;
            Prefs.FormSize = Size;
         }
         else
         {
            Prefs.FormLocation = RestoreBounds.Location;
            Prefs.FormSize = RestoreBounds.Size;
         }

         Prefs.FormLogVisible = txtLogFile.Visible;

         // Save the data
         Prefs.Save();
         
         // Save the data on current WUs in progress
         HostInstances.SaveCurrentUnitInfo();
         // Save the benchmark collection
         ProteinBenchmarkCollection.Instance.Serialize();

         HfmTrace.WriteToHfmConsole("----------");
         HfmTrace.WriteToHfmConsole("Exiting...");
         HfmTrace.WriteToHfmConsole(String.Empty);
      }

      /// <summary>
      /// Update Split Location in Preferences
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
      {
         PreferenceSet Prefs = PreferenceSet.Instance;

         // When the log file window (Panel2) is visible, this event will fire.
         // Update the split location directly from the split panel control. - Issue 8
         Prefs.FormSplitLocation = splitContainer1.SplitterDistance;
         Prefs.Save();
      }

      /// <summary>
      /// Event Handler - adds messages to the frmMessages window
      /// </summary>
      private void HfmTrace_TextMessage(object sender, TextMessageEventArgs e)
      {
         _frmMessages.AddMessage(e.Message);
         //Application.DoEvents();
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
         if (dataGridView1.Columns.Count == FoldingInstanceCollection.NumberOfDisplayFields)
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

            PreferenceSet Prefs = PreferenceSet.Instance;

            SaveColumnSettings(Prefs); // Save Column Settings - Issue 73
            Prefs.Save();
         }
      }

      /// <summary>
      /// When entering row, show the FAH Log text if available.
      /// </summary>
      private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
      {
         if (HostInstances.HasInstances)
         {
            string InstanceName = dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString();
         
            ClientInstance Instance;
            if (HostInstances.InstanceCollection.TryGetValue(InstanceName, out Instance))
            {
               statusLabelLeft.Text = Instance.Path;

               if (Instance.CurrentLogLines.Count > 0) /*** Checked LogLine Count ***/
               {
                  // Different Client... Load LogLines.
                  if (txtLogFile.LogOwnedByInstanceName.Equals(Instance.InstanceName) == false)
                  {
                     txtLogFile.SetLogLines(Instance.CurrentLogLines, Instance.InstanceName);
                     PreferenceSet_ColorLogFileChanged(sender, EventArgs.Empty);
                  }
                  // Textbox has text lines
                  else if (txtLogFile.Lines.Length > 0)
                  {
                     string lastLogLine = String.Empty;
                     
                     try // to get the last LogLine from the Instance
                     {
                        lastLogLine = Instance.CurrentLogLines[Instance.CurrentLogLines.Count - 1].ToString();
                     }
                     catch (ArgumentOutOfRangeException ex)
                     {
                        // even though i've checked the count above, it could have changed in between then
                        // and now... and if the count is 0 it will yield this exception.  Log It!!!
                        HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Instance.InstanceName, ex);
                     } 

                     // If the last text line in the textbox DOES NOT equal the last LogLine Text... Load LogLines.
                     // Otherwise, the log has not changed, don't update and perform the log "flicker".
                     if (txtLogFile.Lines[txtLogFile.Lines.Length - 1].Equals(lastLogLine) == false)
                     {
                        txtLogFile.SetLogLines(Instance.CurrentLogLines, Instance.InstanceName);
                        PreferenceSet_ColorLogFileChanged(sender, EventArgs.Empty);
                     }
                  }
                  // Nothing in the Textbox... Load LogLines.
                  else
                  {
                     txtLogFile.SetLogLines(Instance.CurrentLogLines, Instance.InstanceName);
                     PreferenceSet_ColorLogFileChanged(sender, EventArgs.Empty);
                  }
               }
               else
               {
                  txtLogFile.SetNoLogLines();
               }

               txtLogFile.ScrollToBottom();
            }
            else // this should only happen when this fires in the middle of an 'Edit' operation that changes the Client Name
            {
               Debug.WriteLine(String.Format("{0} could not find Client Name '{1}'.", HfmTrace.FunctionName, InstanceName));
            }
         }
      }

      /// <summary>
      /// Update Form Level Sorting Fields
      /// </summary>
      private void dataGridView1_Sorted(object sender, EventArgs e)
      {
         if (InApplySort == false)
         {
            SortColumnName = dataGridView1.SortedColumn.Name;
            SortColumnOrder = dataGridView1.SortOrder;

            PreferenceSet Prefs = PreferenceSet.Instance;

            SaveSortColumn(Prefs); // Save Column Sort Order - Issue 73
            Prefs.Save();
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo info = dataGridView1.HitTest(e.X, e.Y);

         if (dataGridView1.Columns["Status"].Index == info.ColumnIndex && info.RowIndex > -1)
         {
            ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[info.RowIndex].Cells["Name"].Value.ToString()];

            toolTipGrid.Show(Instance.Status.ToString(), dataGridView1, e.X + 15, e.Y);
         }
         else if (dataGridView1.Columns["Username"].Index == info.ColumnIndex && info.RowIndex > -1)
         {
            ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[info.RowIndex].Cells["Name"].Value.ToString()];
            if (Instance.IsUsernameOk() == false)
            {
               toolTipGrid.Show("Client's User Name does not match the configured User Name", dataGridView1, e.X + 15, e.Y);
            }
            else
            {
               toolTipGrid.Hide(dataGridView1);
            }
         }
         else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == info.ColumnIndex && info.RowIndex > -1)
         {
            ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[info.RowIndex].Cells["Name"].Value.ToString()];
            if (HostInstances.IsDuplicateProject(Instance.CurrentUnitInfo.ProjectRunCloneGen))
            {
               toolTipGrid.Show("Client is working on the same work unit as another client", dataGridView1, e.X + 15, e.Y);
            }
            else
            {
               toolTipGrid.Hide(dataGridView1);
            }
         }
         else if (dataGridView1.Columns["Name"].Index == info.ColumnIndex && info.RowIndex > -1)
         {
            ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[info.RowIndex].Cells["Name"].Value.ToString()];
            if (HostInstances.IsDuplicateUserAndMachineID(Instance.UserAndMachineID))
            {
               toolTipGrid.Show("Client is working with the same User and Machine ID as another client", dataGridView1, e.X + 15, e.Y);
            }
            else
            {
               toolTipGrid.Hide(dataGridView1);
            }
         }
         else
         {
            toolTipGrid.Hide(dataGridView1);
         }
      }

      /// <summary>
      /// Override painting in the Status column
      /// </summary>
      private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
      {
         if (e.RowIndex >= 0)
         {
            if (dataGridView1.Columns["Status"].Index == e.ColumnIndex)
            {
               #region Status Column Custom Paint
               Rectangle newRect = new Rectangle(e.CellBounds.X + 4,
                                                 e.CellBounds.Y + 4, e.CellBounds.Width - 10,
                                                 e.CellBounds.Height - 10);

               using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor),
                            backColorBrush = new SolidBrush(e.CellStyle.BackColor))
               {
                  using (Pen gridLinePen = new Pen(gridBrush))
                  {
                     // Erase the cell.
                     e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                     // Draw the bottom grid line
                     e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                         e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                         e.CellBounds.Bottom - 1);

                     // Draw the inset highlight box.
                     ClientStatus status = (ClientStatus)e.Value;
                     e.Graphics.DrawRectangle(ClientInstance.GetStatusPen(status), newRect);
                     e.Graphics.FillRectangle(ClientInstance.GetStatusBrush(status), newRect);

                     //Draw the text content of the cell, ignoring alignment.
                     //if (e.Value != null)
                     //{
                     //   e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                     //                         Brushes.Black, e.CellBounds.X + 2,
                     //                         e.CellBounds.Y + 2, StringFormat.GenericDefault);
                     //}

                     e.Handled = true;
                  }
               }
               #endregion
            }
            else if (dataGridView1.Columns["Username"].Index == e.ColumnIndex)
            {
               #region Username Incorrect Custom Paint
               ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];
               if (Instance.IsUsernameOk() == false)
               {
                  using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor))
                  {
                     using (Pen gridLinePen = new Pen(gridBrush))
                     {
                        // Erase the cell.
                        e.Graphics.FillRectangle(Brushes.Orange, e.CellBounds);

                        // Draw the bottom grid line
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                            e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                            e.CellBounds.Bottom - 1);

                        // Draw the text content of the cell, ignoring alignment.
                        if (e.Value != null)
                        {
                           e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                                                 Brushes.Black, e.CellBounds.X + 2,
                                                 e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }

                        e.Handled = true;
                     }
                  }
               }
               #endregion
            }
            else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == e.ColumnIndex)
            {
               #region Username Incorrect Custom Paint
               ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];
               if (HostInstances.IsDuplicateProject(Instance.CurrentUnitInfo.ProjectRunCloneGen))
               {
                  using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor))
                  {
                     using (Pen gridLinePen = new Pen(gridBrush))
                     {
                        // Erase the cell.
                        e.Graphics.FillRectangle(Brushes.Orange, e.CellBounds);

                        // Draw the bottom grid line
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                            e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                            e.CellBounds.Bottom - 1);

                        // Draw the text content of the cell, ignoring alignment.
                        if (e.Value != null)
                        {
                           e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                                                 Brushes.Black, e.CellBounds.X + 2,
                                                 e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }

                        e.Handled = true;
                     }
                  }
               }
               #endregion
            }
            else if (dataGridView1.Columns["Name"].Index == e.ColumnIndex)
            {
               #region Username Incorrect Custom Paint
               ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];
               if (HostInstances.IsDuplicateUserAndMachineID(Instance.UserAndMachineID))
               {
                  using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor))
                  {
                     using (Pen gridLinePen = new Pen(gridBrush))
                     {
                        // Erase the cell.
                        e.Graphics.FillRectangle(Brushes.Orange, e.CellBounds);

                        // Draw the bottom grid line
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                            e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                            e.CellBounds.Bottom - 1);

                        // Draw the text content of the cell, ignoring alignment.
                        if (e.Value != null)
                        {
                           e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                                                 Brushes.Black, e.CellBounds.X + 2,
                                                 e.CellBounds.Y + 2, StringFormat.GenericDefault);
                        }

                        e.Handled = true;
                     }
                  }
               }
               #endregion
            }
            else
            {
               PaintTimeBasedCell(e);
            }
         }
      }

      /// <summary>
      /// Paint the Time based cells with the custom time format
      /// </summary>
      /// <param name="e"></param>
      private void PaintTimeBasedCell(DataGridViewCellPaintingEventArgs e)
      {
         if ((dataGridView1.Columns["TPF"].Index == e.ColumnIndex || 
              dataGridView1.Columns["ETA"].Index == e.ColumnIndex ||
              dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
              dataGridView1.Columns["Deadline"].Index == e.ColumnIndex) && 
              PreferenceSet.Instance.TimeStyle.Equals(eTimeStyle.Formatted) &&
              e.Value != null)
         {
            using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor),
                         backColorBrush = new SolidBrush(e.CellStyle.BackColor),
                         selectionColorBrush = new SolidBrush(e.CellStyle.SelectionBackColor))
            {
               using (Pen gridLinePen = new Pen(gridBrush))
               {
                  // Erase the cell.
                  e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                  Brush textColor = Brushes.Black;

                  if (dataGridView1.Rows[e.RowIndex].Selected)
                  {
                     e.Graphics.FillRectangle(selectionColorBrush, e.CellBounds);
                     textColor = Brushes.White;
                  }

                  // Draw the bottom grid line
                  e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                      e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                      e.CellBounds.Bottom - 1);
                                      
                  if (dataGridView1.Columns["TPF"].Index == e.ColumnIndex)
                  {
                     TimeSpan span = (TimeSpan)e.Value;
                     if (span.Equals(TimeSpan.MinValue) == false)
                     {
                        e.Graphics.DrawString(GetFormattedTpfString(span), e.CellStyle.Font,
                           textColor, e.CellBounds.X + 2,
                           e.CellBounds.Y + 2, StringFormat.GenericDefault);

                        e.Handled = true;
                     }
                  }
                  else if (dataGridView1.Columns["ETA"].Index == e.ColumnIndex)
                  {
                     TimeSpan span = (TimeSpan)e.Value;
                     if (span.Equals(TimeSpan.MinValue) == false)
                     {
                        e.Graphics.DrawString(GetFormattedEtaString(span), e.CellStyle.Font,
                           textColor, e.CellBounds.X + 2,
                           e.CellBounds.Y + 2, StringFormat.GenericDefault);

                        e.Handled = true;
                     }
                  }
                  else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex)
                  {
                     DateTime date = (DateTime)e.Value;
                     //if (date.Equals(DateTime.MinValue) == false)
                     //{
                        e.Graphics.DrawString(GetFormattedDownloadTimeString(date), e.CellStyle.Font,
                           textColor, e.CellBounds.X + 2,
                           e.CellBounds.Y + 2, StringFormat.GenericDefault);

                        e.Handled = true;
                     //}
                  }
                  else if (dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
                  {
                     DateTime date = (DateTime)e.Value;
                     //if (date.Equals(DateTime.MinValue) == false)
                     //{
                        e.Graphics.DrawString(GetFormattedDeadlineString(date), e.CellStyle.Font,
                           textColor, e.CellBounds.X + 2,
                           e.CellBounds.Y + 2, StringFormat.GenericDefault);

                        e.Handled = true;
                     //}
                  }
               }
            }
         }
         else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
                  dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
         {
         
            using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor),
                         backColorBrush = new SolidBrush(e.CellStyle.BackColor),
                         selectionColorBrush = new SolidBrush(e.CellStyle.SelectionBackColor))
            {
               using (Pen gridLinePen = new Pen(gridBrush))
               {
                  if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex)
                  {
                     // Erase the cell.
                     e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                     Brush textColor = Brushes.Black;

                     if (dataGridView1.Rows[e.RowIndex].Selected)
                     {
                        e.Graphics.FillRectangle(selectionColorBrush, e.CellBounds);
                        textColor = Brushes.White;
                     }

                     // Draw the bottom grid line
                     e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                         e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                         e.CellBounds.Bottom - 1);
                                         
                     DateTime date = (DateTime)e.Value;
                     if (date.Equals(DateTime.MinValue))
                     {
                        e.Graphics.DrawString(GetFormattedDownloadTimeString(date), e.CellStyle.Font,
                           textColor, e.CellBounds.X + 2,
                           e.CellBounds.Y + 2, StringFormat.GenericDefault);

                        e.Handled = true;
                     }
                  }
                  else if (dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
                  {
                     // Erase the cell.
                     e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                     Brush textColor = Brushes.Black;

                     if (dataGridView1.Rows[e.RowIndex].Selected)
                     {
                        e.Graphics.FillRectangle(selectionColorBrush, e.CellBounds);
                        textColor = Brushes.White;
                     }

                     // Draw the bottom grid line
                     e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                         e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                         e.CellBounds.Bottom - 1);
                  
                     DateTime date = (DateTime)e.Value;
                     if (date.Equals(DateTime.MinValue))
                     {
                        e.Graphics.DrawString(GetFormattedDeadlineString(date), e.CellStyle.Font,
                           textColor, e.CellBounds.X + 2,
                           e.CellBounds.Y + 2, StringFormat.GenericDefault);

                        e.Handled = true;
                     }
                  }
               }
            }
         }
      }

      /// <summary>
      /// Measure Text and set Column Width on Double-Click
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGridView1_ColumnDividerDoubleClick(object sender, DataGridViewColumnDividerDoubleClickEventArgs e)
      {
         Font font = new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Regular);
         //Graphics g = dataGridView1.CreateGraphics();

         SizeF s;
         int width = 0;

         if ((dataGridView1.Columns["TPF"].Index == e.ColumnIndex || 
              dataGridView1.Columns["ETA"].Index == e.ColumnIndex ||
              dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
              dataGridView1.Columns["Deadline"].Index == e.ColumnIndex) &&
              PreferenceSet.Instance.TimeStyle.Equals(eTimeStyle.Formatted))
         {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
               if (dataGridView1.Rows[i].Cells[e.ColumnIndex].Value != null)
               {
                  string formattedString = String.Empty;
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

                  //s = g.MeasureString(formattedString, font);
                  s = TextRenderer.MeasureText(formattedString, font);

                  if (width < s.Width)
                  {
                     width = (int)(s.Width + 1);
                  }
               }
            }

            dataGridView1.Columns[e.ColumnIndex].Width = width;

            e.Handled = true;
         }
         else
         {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
               s = TextRenderer.MeasureText(dataGridView1.Rows[i].Cells[e.ColumnIndex].FormattedValue.ToString(), font);

               if (width < s.Width)
               {
                  width = (int)(s.Width + 1);
               }
            }

            dataGridView1.Columns[e.ColumnIndex].Width = width;

            e.Handled = true;
         }
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
      #endregion

      /// <summary>
      /// Handle Right-Click (Contest Menu) and Left Double-Click (File Browser)
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo hti = dataGridView1.HitTest(e.X, e.Y);
         if (e.Button == System.Windows.Forms.MouseButtons.Right)
         {
            if (hti.Type == DataGridViewHitTestType.Cell)
            {
               dataGridView1.ClearSelection();

               dataGridView1.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected = true;

               ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["Name"].Value.ToString()];
               if (Instance.InstanceHostType.Equals(InstanceType.PathInstance))
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
         if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 2)
         {
            if (hti.Type == DataGridViewHitTestType.Cell && dataGridView1.SelectedRows.Count > 0)
            {
               ClientInstance Instance =
                  HostInstances.InstanceCollection[dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString()];

               if (Instance.InstanceHostType.Equals(InstanceType.PathInstance))
               {
                  try
                  {
                     StartFileBrowser(Instance.Path);
                  }
                  catch (Exception ex)
                  {
                     HfmTrace.WriteToHfmConsole(ex);
                     
                     MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, String.Format("client '{0}' files.{1}{1}Please check the current File Explorer defined in the Preferences", Instance.InstanceName, Environment.NewLine)));
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
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileNew_Click(object sender, EventArgs e)
      {
         if (HostInstances.RetrievalInProgress)
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
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileOpen_Click(object sender, EventArgs e)
      {
         if (HostInstances.RetrievalInProgress)
         {
            MessageBox.Show(this, "Retrieval in progress... please wait to open another config file.",
               Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (CanContinueDestructiveOp(sender, e))
         {
            openConfigDialog.DefaultExt = "hfm";
            openConfigDialog.Filter = "HFM Configuration Files|*.hfm";
            openConfigDialog.FileName = HostInstances.ConfigFilename;
            openConfigDialog.RestoreDirectory = true;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
            {
               DoLoadFile(openConfigDialog.FileName);
            }
         }
      }

      /// <summary>
      /// Save the current host configuration
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileSave_Click(object sender, EventArgs e)
      {
         if (HostInstances.HasConfigFilename == false)
         {
            mnuFileSaveas_Click(sender, e);
         }
         else
         {
            HostInstances.ToXml();
         }
      }

      /// <summary>
      /// Save the current host configuration as a new file
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileSaveas_Click(object sender, EventArgs e)
      {
         // No Config File and no Instances, stub out
         if (HostInstances.HasConfigFilename == false && HostInstances.HasInstances == false) return;
      
         if (saveConfigDialog.ShowDialog() == DialogResult.OK)
         {
            HostInstances.ToXml(saveConfigDialog.FileName); // Issue 75
         }
      }

      /// <summary>
      /// Import FahMon clientstab.txt configuration
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileImportFahMon_Click(object sender, EventArgs e)
      {
         if (HostInstances.RetrievalInProgress)
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
         ePpdCalculation currentPpdCalc = PreferenceSet.Instance.PpdCalculation;
      
         frmPreferences prefDialog = new frmPreferences();
         if (prefDialog.ShowDialog() == DialogResult.OK)
         {
            HostInstances.SetTimerState();
            
            // If the PPD Calculation style changed, we need to do a new Retrieval
            if (currentPpdCalc.Equals(PreferenceSet.Instance.PpdCalculation))
            {
               RefreshDisplay();
            }
            else
            {
               HostInstances.QueueNewRetrieval();
            }
         }
      }
      #endregion

      #region Help Menu Click Handlers
      /// <summary>
      /// Show the About dialog
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuHelpAbout_Click(object sender, EventArgs e)
      {
         frmAbout newAbout = new frmAbout();
         newAbout.ShowDialog(this);
      }

      /// <summary>
      /// Show the help file at the contents tab
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuHelpContents_Click(object sender, EventArgs e)
      {
         //Help.ShowHelp(this, helpProvider.HelpNamespace);
      }

      /// <summary>
      /// Show the help file at the index tab
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuHelpIndex_Click(object sender, EventArgs e)
      {
         //Help.ShowHelpIndex(this, helpProvider.HelpNamespace);
      }
      #endregion

      #region Clients Menu Click Handlers
      /// <summary>
      /// Add a new host to the configuration
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuClientsAdd_Click(object sender, EventArgs e)
      {
         frmHost newHost = new frmHost();
         newHost.radioLocal.Checked = true;
         newHost.chkClientVM.Checked = false;

         if (newHost.ShowDialog() == DialogResult.OK)
         {
            if (HostInstances.ContainsName(newHost.txtName.Text))
            {
               MessageBox.Show(String.Format("Client Name '{0}' already exists.", newHost.txtName.Text));
               return;
            }

            ClientInstance xHost = null;
            if (newHost.radioLocal.Checked)
            {
               xHost = new ClientInstance(InstanceType.PathInstance);
               SetInstanceBasicInfo(newHost, xHost);
               
               xHost.Path = newHost.txtLocalPath.Text;
            }
            else if (newHost.radioHTTP.Checked)
            {
               xHost = new ClientInstance(InstanceType.HTTPInstance);
               SetInstanceBasicInfo(newHost, xHost);
               
               xHost.Path = newHost.txtWebURL.Text;
               xHost.Username = newHost.txtWebUser.Text;
               xHost.Password = newHost.txtWebPass.Text;
            }
            else if (newHost.radioFTP.Checked)
            {
               xHost = new ClientInstance(InstanceType.FTPInstance);
               SetInstanceBasicInfo(newHost, xHost);
               
               xHost.Server = newHost.txtFTPServer.Text;
               xHost.Path = newHost.txtFTPPath.Text;
               xHost.Username = newHost.txtFTPUser.Text;
               xHost.Password = newHost.txtFTPPass.Text;
            }

            // xHost should not be null at this point
            Debug.Assert(xHost != null);
            
            // Add the new Host Instance
            HostInstances.Add(xHost);
         }
      }

      /// <summary>
      /// Edits an existing host configuration
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuClientsEdit_Click(object sender, EventArgs e)
      {
         if (dataGridView1.SelectedRows.Count == 0) return;
      
         frmHost editHost = new frmHost();

         ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString()];

         editHost.txtName.Text = Instance.InstanceName;
         editHost.txtLogFileName.Text = Instance.RemoteFAHLogFilename;
         editHost.txtUnitFileName.Text = Instance.RemoteUnitInfoFilename;
         editHost.txtClientMegahertz.Text = Instance.ClientProcessorMegahertz.ToString();
         editHost.chkClientVM.Checked = Instance.ClientIsOnVirtualMachine;
         editHost.numOffset.Value = Instance.ClientTimeOffset;

         if (Instance.InstanceHostType.Equals(InstanceType.PathInstance))
         {
            editHost.radioLocal.Select();
            editHost.txtLocalPath.Text = Instance.Path;
         }
         else if (Instance.InstanceHostType.Equals(InstanceType.FTPInstance))
         {
            editHost.radioFTP.Select();
            editHost.txtFTPPath.Text = Instance.Path;
            editHost.txtFTPServer.Text = Instance.Server;
            editHost.txtFTPUser.Text = Instance.Username;
            editHost.txtFTPPass.Text = Instance.Password;
         }
         else if (Instance.InstanceHostType.Equals(InstanceType.HTTPInstance))
         {
            editHost.radioHTTP.Select();
            editHost.txtWebURL.Text = Instance.Path;
            editHost.txtWebUser.Text = Instance.Username;
            editHost.txtWebPass.Text = Instance.Password;
         }
         else
         {
            throw new NotImplementedException("The instance type was not located as expected");
         }

         if (editHost.ShowDialog() == DialogResult.OK)
         {
            if (Instance.InstanceName != editHost.txtName.Text)
            {
               if (HostInstances.ContainsName(editHost.txtName.Text))
               {
                  MessageBox.Show(String.Format("Client Name '{0}' already exists.", editHost.txtName.Text));
                  return;
               }
            }
            
            string oldName = Instance.InstanceName;
            string oldPath = Instance.Path;

            if (editHost.radioLocal.Checked)
            {
               if (Instance.InstanceHostType.Equals(InstanceType.PathInstance) == false)
               {
                  Instance.InstanceHostType = InstanceType.PathInstance;
               }

               SetInstanceBasicInfo(editHost, Instance);
               Instance.Path = editHost.txtLocalPath.Text;
            }
            else if (editHost.radioHTTP.Checked)
            {
               if (Instance.InstanceHostType.Equals(InstanceType.HTTPInstance) == false)
               {
                  Instance.InstanceHostType = InstanceType.HTTPInstance;
               }
            
               SetInstanceBasicInfo(editHost, Instance);
               Instance.Path = editHost.txtWebURL.Text;
               Instance.Username = editHost.txtWebUser.Text;
               Instance.Password = editHost.txtWebPass.Text;
            }
            else if (editHost.radioFTP.Checked)
            {
               if (Instance.InstanceHostType.Equals(InstanceType.FTPInstance) == false)
               {
                  Instance.InstanceHostType = InstanceType.FTPInstance;
               }
            
               SetInstanceBasicInfo(editHost, Instance);
               Instance.Path = editHost.txtFTPPath.Text;
               Instance.Server = editHost.txtFTPServer.Text;
               Instance.Username = editHost.txtFTPUser.Text;
               Instance.Password = editHost.txtFTPPass.Text;
            }

            HostInstances.Edit(oldName, oldPath, Instance);
         }
      }

      /// <summary>
      /// Gets basic client info from the frmHost and sets in the given Client Instance
      /// </summary>
      /// <param name="hostForm">frmHost object</param>
      /// <param name="Instance">Client Instance</param>
      private static void SetInstanceBasicInfo(frmHost hostForm, ClientInstance Instance)
      {
         Instance.InstanceName = hostForm.txtName.Text;
         Instance.RemoteFAHLogFilename = hostForm.txtLogFileName.Text;
         Instance.RemoteUnitInfoFilename = hostForm.txtUnitFileName.Text;
         int mhz;
         if (int.TryParse(hostForm.txtClientMegahertz.Text, out mhz))
         {
            Instance.ClientProcessorMegahertz = mhz;
         }
         else
         {
            Instance.ClientProcessorMegahertz = 1;
         }
         Instance.ClientIsOnVirtualMachine = hostForm.chkClientVM.Checked;
         Instance.ClientTimeOffset = Convert.ToInt32(hostForm.numOffset.Value);
      }

      /// <summary>
      /// Removes a host from the configuration
      /// </summary>
      private void mnuClientsDelete_Click(object sender, EventArgs e)
      {
         if (dataGridView1.SelectedRows.Count == 0) return;
         
         HostInstances.Remove(dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString());
      }

      /// <summary>
      /// Refreshes the currently selected client
      /// </summary>
      private void mnuClientsRefreshSelected_Click(object sender, EventArgs e)
      {
         if (dataGridView1.SelectedRows.Count == 0) return;
      
         HostInstances.RetrieveSingleClient(dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString());
      }

      /// <summary>
      /// Refreshes all clients
      /// </summary>
      private void mnuClientsRefreshAll_Click(object sender, EventArgs e)
      {
         HostInstances.QueueNewRetrieval();
      }

      /// <summary>
      /// Fire process to show the cached FAHLog file
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuClientsViewCachedLog_Click(object sender, EventArgs e)
      {
         if (dataGridView1.SelectedRows.Count == 0) return;
         
         ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString()];
         
         string logPath = Path.Combine(Instance.BaseDirectory, Instance.CachedFAHLogName);
         if (File.Exists(logPath))
         {
            try
            {
               Process.Start(PreferenceSet.Instance.LogFileViewer, logPath);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               
               MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, String.Format("client '{0}' FAHlog file.{1}{1}Please check the current Log File Viewer defined in the Preferences", Instance.InstanceName, Environment.NewLine)));
            }
         }
         else
         {
            MessageBox.Show(String.Format("Cannot find client '{0}' FAHlog file.", Instance.InstanceName));
         }
      }

      /// <summary>
      /// Fire process to show the current client file directory (Local Clients Only)
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuClientsViewClientFiles_Click(object sender, EventArgs e)
      {
         if (dataGridView1.SelectedRows.Count == 0) return;
         
         ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString()];

         if (Instance.InstanceHostType.Equals(InstanceType.PathInstance))
         {
            try
            {
               StartFileBrowser(Instance.Path);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               
               MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, String.Format("client '{0}' files.{1}{1}Please check the current File Explorer defined in the Preferences", Instance.InstanceName, Environment.NewLine)));
            }
         }
      }
      #endregion

      #region View Menu Click Handlers
      /// <summary>
      /// Show or Hide the FAH Log Window
      /// </summary>
      private void mnuViewShowHideLog_Click(object sender, EventArgs e)
      {
         ShowHideLog(!txtLogFile.Visible);
      }

      /// <summary>
      /// Toggle the Date/Time Style
      /// </summary>
      private void mnuViewToggleDateTime_Click(object sender, EventArgs e)
      {
         if (PreferenceSet.Instance.TimeStyle.Equals(eTimeStyle.Standard))
         {
            PreferenceSet.Instance.TimeStyle = eTimeStyle.Formatted;
         }
         else
         {
            PreferenceSet.Instance.TimeStyle = eTimeStyle.Standard;
         }

         dataGridView1.Invalidate();
      }
      #endregion

      #region Tools Menu Click Handlers
      /// <summary>
      /// Show or Hide the HFM Messages Window
      /// </summary>
      private void mnuToolsMessages_Click(object sender, EventArgs e)
      {
         if (_frmMessages.Visible)
         {
            _frmMessages.Hide();
         }
         else
         {
            _frmMessages.Show();
            _frmMessages.ScrollToEnd();
         }
      }

      /// <summary>
      /// Download Project Info From Stanford
      /// </summary>
      private void mnuToolsDownloadProjects_Click(object sender, EventArgs e)
      {
         ProteinCollection.Instance.BeginDownloadFromStanford();
      }

      /// <summary>
      /// Show the Benchmarks Dialog
      /// </summary>
      private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
      {
         int ProjectID = 0;
      
         // Make sure we have a selected client in the data grid view - Issue 33
         if (dataGridView1.SelectedRows.Count > 0)
         {
            ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString()];
            ProjectID = Instance.CurrentUnitInfo.ProjectID;
         }
         
         frmBenchmarks frm = new frmBenchmarks(HostInstances, ProjectID);
         frm.StartPosition = FormStartPosition.Manual;
         frm.Location = new Point(Location.X + 50, Location.Y + 50);
         frm.Show();
      }
      #endregion

      #region Web Menu Click Handlers
      private void mnuWebEOCUser_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start(PreferenceSet.Instance.EOCUserURL);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            
            MessageBox.Show(Properties.Resources.ProcessStartError, String.Format("EOC User Stats page"));
         }
      }

      private void mnuWebStanfordUser_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start(PreferenceSet.Instance.StanfordUserURL);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            
            MessageBox.Show(Properties.Resources.ProcessStartError, String.Format("Stanford User Stats page"));
         }
      }

      private void mnuWebEOCTeam_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start(PreferenceSet.Instance.EOCTeamURL);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            
            MessageBox.Show(Properties.Resources.ProcessStartError, String.Format("EOC Team Stats page"));
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
            
            MessageBox.Show(Properties.Resources.ProcessStartError, String.Format("HFM.NET Google Code page"));
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
            HostInstances.RefreshDisplayCollection();
            if (dataGridView1.DataSource != null)
            {
               CurrencyManager cm = (CurrencyManager) dataGridView1.BindingContext[dataGridView1.DataSource];
               if (cm != null)
               {
                  // Remove the RowEnter handler when doing currency refresh
                  dataGridView1.RowEnter -= dataGridView1_RowEnter;
                  if (InvokeRequired)
                  {
                     BeginInvoke(new MethodInvoker(cm.Refresh));
                  }
                  else
                  {
                     cm.Refresh();
                  }
                  // Add the RowEnter handler back after refresh
                  // This removes the "stuttering log" effect seen as each client is refreshed
                  dataGridView1.RowEnter += dataGridView1_RowEnter;
               }

               ApplySort();
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

      /// <summary>
      /// Apply Sort to Data Grid View
      /// </summary>
      private void ApplySort()
      {
         if (InvokeRequired)
         {
            BeginInvoke(new MethodInvoker(ApplySort), null);
         }
         else
         {
            InApplySort = true;
            
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
            
            InApplySort = false;
         }
      }

      /// <summary>
      /// Refresh remaining UI controls with current data
      /// </summary>
      private void RefreshControls()
      {
         InstanceTotals totals = HostInstances.GetInstanceTotals();

         double TotalPPD = totals.PPD;
         int GoodHosts = totals.WorkingClients;
         
         SetNotifyIconText(String.Format("{0} Working Clients{3}{1} Non-Working Clients{3}{2:" + PreferenceSet.GetPPDFormatString() + "} PPD",
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

         SetStatusLabelPPDText(String.Format("{0:" + PreferenceSet.GetPPDFormatString() + "} PPD", TotalPPD));
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

      /// <summary>
      /// Forces a log and screen refresh when the Stanford info is updated
      /// </summary>
      private void Instance_ProjectInfoUpdated(object sender, ProjectInfoUpdatedEventArgs e)
      {
         // Do Retrieve on all clients after Project Info is updated (this is confirmed needed)
         HostInstances.QueueNewRetrieval();
      }
      #endregion

      #region System Tray Icon Routines
      /// <summary>
      /// Action of the double-clicked notification icon (min/restore)
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
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
      /// <param name="sender"></param>
      /// <param name="e"></param>
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
      /// <param name="sender"></param>
      /// <param name="e"></param>
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
      /// <param name="sender"></param>
      /// <param name="e"></param>
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
      /// Clears the log cache folder specified by the CacheFolder setting
      /// </summary>
      private static void ClearCacheFolder()
      {
         DateTime Start = HfmTrace.ExecStart;
      
         string cacheFolder = Path.Combine(PreferenceSet.Instance.AppDataPath,
                                           PreferenceSet.Instance.CacheFolder);

         DirectoryInfo di = new DirectoryInfo(cacheFolder);
         if (di.Exists == false)
         {
            di.Create();
         }
         else
         {
            foreach (FileInfo fi in di.GetFiles())
            {
               try
               {
                  fi.Delete();
               }
               catch (IOException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} Failed to Clear Cache File '{1}'.", HfmTrace.FunctionName, fi.Name));
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
      }

      /// <summary>
      /// Restore Form State
      /// </summary>
      private void RestoreFormPreferences()
      {
         PreferenceSet Prefs = PreferenceSet.Instance;

         // Restore state data
         Point location = Prefs.FormLocation;
         Size size = Prefs.FormSize;

         if (location.X != 0 && location.Y != 0)
         {
            // Set StartPosition to manual
            StartPosition = FormStartPosition.Manual;
            Location = location;
         }
         if (size.Width != 0 && size.Height != 0)
         {
            if (Prefs.FormLogVisible == false)
            {
               size = new Size(size.Width, size.Height + Prefs.FormLogWindowHeight);
            }
            Size = size;
            splitContainer1.SplitterDistance = Prefs.FormSplitLocation;
         }

         if (Prefs.FormLogVisible == false)
         {
            ShowHideLog(false);
         }

         //if (Prefs.FormSortColumn != String.Empty &&
         //    Prefs.FormSortOrder != SortOrder.None)
         //{
            SortColumnName = Prefs.FormSortColumn;
            SortColumnOrder = Prefs.FormSortOrder;
         //}
         
         try
         {
            // Restore the columns' state
            StringCollection cols = PreferenceSet.Instance.FormColumns;
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
      /// <param name="Prefs">Preferences Set</param>
      private void SaveColumnSettings(PreferenceSet Prefs)
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

         Prefs.FormColumns = stringCollection;
      }

      /// <summary>
      /// Save Sorted Column Name and Order
      /// </summary>
      /// <param name="Prefs">Preferences Set</param>
      private void SaveSortColumn(PreferenceSet Prefs)
      {
         Prefs.FormSortColumn = SortColumnName;
         Prefs.FormSortOrder = SortColumnOrder;
      }

      /// <summary>
      /// Test current application status for changes; ask for confirmation if necessary.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      /// <returns>Whether or not a destructive operation can be performed</returns>
      private bool CanContinueDestructiveOp(object sender, EventArgs e)
      {
         if (HostInstances.ChangedAfterSave)
         {
            DialogResult qResult = MessageBox.Show(this, String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes / No - Continue and do not save the changes / Cancel - Do not continue", Environment.NewLine), base.Text, MessageBoxButtons.YesNoCancel);
            switch (qResult)
            {
               case DialogResult.Yes:
                  mnuFileSave_Click(sender, e);
                  if (HostInstances.ChangedAfterSave)
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
         // clear the log text
         txtLogFile.Text = String.Empty;
         // Clear the list of host instances
         HostInstances.Clear();
         // This will disable the timers, we have no hosts
         HostInstances.SetTimerState();
      }

      /// <summary>
      /// Call LoadFile() and handle any thrown exceptions
      /// </summary>
      /// <param name="Filename">File to load</param>
      private void DoLoadFile(string Filename)
      {
         try
         {
            LoadFile(Filename);
         }
         catch (FileNotFoundException fnfe)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, fnfe);

            MessageBox.Show(fnfe.Message, base.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
         catch (Exception ex)
         {
            // OK now this could be anything (even permissions)
            HfmTrace.WriteToHfmConsole(ex);

            MessageBox.Show(ex.Message, base.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      /// <summary>
      /// Loads a configuration file into memory
      /// </summary>
      /// <param name="Filename">File to load</param>
      private void LoadFile(string Filename)
      {
         // Clear the UI
         ClearUI();
         // Read the config file
         HostInstances.FromXml(Filename);

         if (HostInstances.HasInstances == false)
         {
            MessageBox.Show(this, "No client configurations were loaded from the given config file.",
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
         // Read the config file
         HostInstances.FromFahMonClientsTab(filename);

         if (HostInstances.HasInstances == false)
         {
            MessageBox.Show(this, String.Format("No client configurations were imported from the given config file.{0}{0}Possibly because the file is in an older FahMon format (not tab delimited).{0}{0}Later versions of FahMon write a clientstab.txt file in tab delimited format.", Environment.NewLine), 
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
            PreferenceSet.Instance.FormLogWindowHeight = (splitContainer1.Height - splitContainer1.SplitterDistance);
            Size = new Size(Size.Width, Size.Height - PreferenceSet.Instance.FormLogWindowHeight);
         }
         else
         {
            txtLogFile.Visible = true;
            Resize -= frmMain_Resize; // disable Form resize event for this operation
            Size = new Size(Size.Width, Size.Height + PreferenceSet.Instance.FormLogWindowHeight);
            Resize += frmMain_Resize; // re-enable
            splitContainer1.Panel2Collapsed = false;
         }
      }

      /// <summary>
      /// Fire File Browser Process (wrap calls to this function in try catch)
      /// </summary>
      /// <param name="path">The Folder Path to browse</param>
      private static void StartFileBrowser(string path)
      {
         Process.Start(PreferenceSet.Instance.FileExplorer, path);
      }

      #region User Stats Data Methods
      /// <summary>
      /// Refresh User Stats from external source
      /// </summary>
      private void HostInstances_RefreshUserStatsData(object sender, EventArgs e)
      {
         RefreshUserStatsData(false);
      }

      /// <summary>
      /// Refresh User Stats from external source
      /// </summary>
      private void RefreshUserStatsData(bool ForceRefresh)
      {
         try
         {
            UserStatsDataContainer UserStatsData = UserStatsDataContainer.Instance;
         
            XMLOps.GetEOCXmlData(UserStatsData, ForceRefresh);
            statusLabel24hr.Text = String.Format("24hr: {0:###,###,##0}", UserStatsData.User24hrAvg);
            statusLabelToday.Text = String.Format("Today: {0:###,###,##0}", UserStatsData.UserPointsToday);
            statusLabelWeek.Text = String.Format("Week: {0:###,###,##0}", UserStatsData.UserPointsWeek);
            statusLabelTotal.Text = String.Format("Total: {0:###,###,##0}", UserStatsData.UserPointsTotal);
            statusLabelWUs.Text = String.Format("WUs: {0:###,###,##0}", UserStatsData.UserWUsTotal);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }
      #endregion

      #region Instance Collection Event Handlers
      /// <summary>
      /// Forces a screen refresh when the instance collection is changed
      /// </summary>
      private void HostInstances_CollectionChanged(object sender, EventArgs e)
      {
         // Update the UI (this is confirmed needed)
         RefreshDisplay();
      }

      /// <summary>
      /// 
      /// </summary>
      private void HostInstances_InstanceDataChanged(object sender, EventArgs e)
      {
         if (PreferenceSet.Instance.AutoSaveConfig)
         {
            mnuFileSave_Click(sender, e);
         }
      }

      /// <summary>
      /// 
      /// </summary>
      private void HostInstances_InstanceRetrieved(object sender, EventArgs e)
      {
         // Update the UI (this is confirmed needed)
         RefreshDisplay();
      }

      /// <summary>
      /// 
      /// </summary>
      private void HostInstances_DuplicatesFoundOrChanged(object sender, EventArgs e)
      {
         // Update the UI (this is confirmed needed)
         RefreshDisplay();
      }
      #endregion

      #region PreferenceSet Event Handlers
      /// <summary>
      /// Show or Hide User Stats Controls based on user setting
      /// </summary>
      private void PreferenceSet_ShowUserStatsChanged(object sender, EventArgs e)
      {
         bool show = PreferenceSet.Instance.ShowUserStats;
         if (show)
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

         statusLabel24hr.Visible = show;
         statusLabelToday.Visible = show;
         statusLabelWeek.Visible = show;
         statusLabelTotal.Visible = show;
         statusLabelWUs.Visible = show;
         statusLabelMiddle.Visible = show;
      }

      /// <summary>
      /// Sets OfflineLast Property on HostInstances Collection
      /// </summary>
      private void PreferenceSet_OfflineLastChanged(object sender, EventArgs e)
      {
         HostInstances.OfflineClientsLast = PreferenceSet.Instance.OfflineLast;
      }

      /// <summary>
      /// Sets Debug Message Level on Trace Level Switch
      /// </summary>
      private static void PreferenceSet_MessageLevelChanged(object sender, EventArgs e)
      {
         TraceLevel newLevel = (TraceLevel)PreferenceSet.Instance.MessageLevel;
         if (newLevel != TraceLevelSwitch.Instance.Level)
         {
            TraceLevelSwitch.Instance.Level = newLevel;
            HfmTrace.WriteToHfmConsole(String.Format("Debug Message Level Changed: {0}", newLevel));
         }
      }

      /// <summary>
      /// Checks for Duplicates after Duplicate Check Preferences have changed
      /// </summary>
      private void PreferenceSet_DuplicateCheckChanged(object sender, EventArgs e)
      {
         HostInstances.FindDuplicates(); // Issue 81
      }
      
      /// <summary>
      /// Sets the Log Color option after change
      /// </summary>
      private void PreferenceSet_ColorLogFileChanged(object sender, EventArgs e)
      {
         if (PreferenceSet.Instance.ColorLogFile)
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
