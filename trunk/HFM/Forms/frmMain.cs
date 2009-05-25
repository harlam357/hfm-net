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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using HFM.Classes;
using HFM.Instances;
using HFM.Instrumentation;
using HFM.Proteins;
using HFM.Preferences;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Forms
{
   public partial class frmMain : Form
   {
      #region Private Variables
      /// <summary>
      /// Conversion factor - minutes to milli-seconds
      /// </summary>
      private const int MinToMillisec = 60000;

      /// <summary>
      /// Collection of host instances
      /// </summary>
      private FoldingInstanceCollection HostInstances;
      
      /// <summary>
      /// Internal filename
      /// </summary>
      private string ConfigFilename = String.Empty;

      /// <summary>
      /// Internal variable storing whether New, Open, Quit should prompt for saving the config first
      /// </summary>
      private bool ChangedAfterSave = false;

      /// <summary>
      /// Private PPD counter for tooltip
      /// </summary>
      private Double TotalPPD;

      /// <summary>
      /// Private Good Host counter for tooltip
      /// </summary>
      private Int32 GoodHosts;
      
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
      /// Messages Form
      /// </summary>
      private readonly frmMessages _frmMessages = null;
      
      /// <summary>
      /// Local flag that forces web gen to run after a full retrieve process
      /// </summary>
      private bool ExecuteWebGenAfterRetrieve = false;

      /// <summary>
      /// Local time that denotes when a full retrieve started (only accessed by the RetrieveInProgress property)
      /// </summary>
      private DateTime _RetrieveExecStart;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress (only accessed by the RetrieveInProgress property)
      /// </summary>
      private bool _RetrievalInProgress = false;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress
      /// </summary>
      private bool RetrievalInProgress
      {
         get { return _RetrievalInProgress; }
         set 
         { 
            if (value)
            {
               _RetrieveExecStart = Debug.ExecStart;
               _RetrievalInProgress = value;
            }
            else
            {
               Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("Total Retrieval Execution Time: {0}", Debug.GetExecTime(_RetrieveExecStart)));
               _RetrievalInProgress = value;
            }
         }
      }
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
         FileVersionInfo fileVersionInfo =
            FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
         base.Text += String.Format(" v{0}.{1}.{2} - Build {3} - Beta", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                                                        fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);

         // Create Messages Window
         _frmMessages = new frmMessages();
         
         // Setup Log File and Messages Window handlers
         SetupTraceListeners();
         
         // Hook up Protein Collection Updated Event Handler
         ProteinCollection.Instance.ProjectInfoUpdated += Instance_ProjectInfoUpdated;
         
         // Clear the Log File Cache Folder
         ClearCacheFolder();
         // Clear the UI
         ClearUI();
         // Add Dummy Column
         dataGridView1.Columns["Dummy"].HeaderText = String.Empty;
         // Restore Form Preferences
         RestoreFormPreferences();
         // Add Column Selector
         new DataGridViewColumnSelector(dataGridView1);
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

         TextWriterTraceListenerWithDateTime listener = new TextWriterTraceListenerWithDateTime("HFM.log");

         Trace.Listeners.Add(listener);
         Trace.AutoFlush = true;
         
         TraceLevelSwitch.Instance.Level = (TraceLevel)PreferenceSet.Instance.MessageLevel;

         listener.TextMessage += Debug_TextMessage;

         Debug.WriteToHfmConsole(String.Format("Starting - {0}", base.Text));
         Debug.WriteToHfmConsole(String.Empty);
      }

      /// <summary>
      /// Loads the appropriate file on initialisation - ie when showing the form
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void frmMain_Shown(object sender, EventArgs e)
      {
         String sFilename = String.Empty;

         if (Program.cmdArgs.Length > 0)
         {
            // Filename on command line - probably from Explorer
            sFilename = Program.cmdArgs[0];
         }
         else if (PreferenceSet.Instance.UseDefaultConfigFile)
         {
            sFilename = PreferenceSet.Instance.DefaultConfigFile;
         }

         if (sFilename != String.Empty)
         {
            try
            {
               LoadFile(sFilename);
            }
            catch (Exception ex)
            {
               // OK now this could be anything (even permissions), but we'll just write to the log and continue.
               Debug.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            }
         }
         
         // Add the Index Changed Handler here after everything is shown
         dataGridView1.ColumnDisplayIndexChanged += dataGridView1_ColumnDisplayIndexChanged;
         // Then run it once to ensure the last column is set to Fill
         dataGridView1_ColumnDisplayIndexChanged(null, null);
         // Add the Splitter Moved Handler here after everything is shown - Issue 8
         splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;
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
            ShowInTaskbar = true;
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

         PreferenceSet.Instance.FormColumns = stringCollection;
         if (dataGridView1.SortedColumn != null)
         {
            PreferenceSet.Instance.FormSortColumn = dataGridView1.SortedColumn.Name;
         }
         PreferenceSet.Instance.FormSortOrder = dataGridView1.SortOrder;

         // Save location and size data
         // RestoreBounds remembers normal position if minimized or maximized
         if (WindowState == FormWindowState.Normal)
         {
            PreferenceSet.Instance.FormLocation = Location;
            PreferenceSet.Instance.FormSize = Size;
         }
         else
         {
            PreferenceSet.Instance.FormLocation = RestoreBounds.Location;
            PreferenceSet.Instance.FormSize = RestoreBounds.Size;
         }

         PreferenceSet.Instance.FormLogVisible = txtLogFile.Visible;

         // Save the data
         PreferenceSet.Instance.Save();
         
         // Save the data on current WUs in progress
         HostInstances.SaveCurrentUnitInfo();
         // Save the benchmark collection
         ProteinBenchmarkCollection.Instance.Serialize();

         Debug.WriteToHfmConsole("----------");
         Debug.WriteToHfmConsole("Exiting...");
         Debug.WriteToHfmConsole(String.Empty);
      }

      /// <summary>
      /// Update Split Location in Preferences
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
      {
         // When the log file window (Panel2) is visible, this event will fire.
         // Update the split location directly from the split panel control. - Issue 8
         PreferenceSet.Instance.FormSplitLocation = splitContainer1.SplitterDistance;
      }

      /// <summary>
      /// Event Handler - adds messages to the frmMessages window
      /// </summary>
      /// <param name="e"></param>
      private void Debug_TextMessage(TextMessageEventArgs e)
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
         }
      }

      /// <summary>
      /// When entering row, show the FAH Log text if available.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
      {
         if (HostInstances.Count > 0)
         {
            ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];
         
            string[] logText = Instance.CurrentUnitInfo.CurrentLogText.ToArray();
               
            statusLabelLeft.Text = Instance.Path;

            if (logText.Length > 0)
            {
               txtLogFile.Lines = logText;
               txtLogFile.SelectionStart = txtLogFile.Text.Length;
               txtLogFile.ScrollToCaret();
            }
            else
            {
               txtLogFile.Text = "No Log Available";
            }
         }
      }

      /// <summary>
      /// Update Form Level Sorting Fields
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGridView1_Sorted(object sender, EventArgs e)
      {
         SortColumnName = dataGridView1.SortedColumn.Name;
         SortColumnOrder = dataGridView1.SortOrder;
      }

      /// <summary>
      /// Override painting in the Status column
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
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
                     Pen fillPen;
                     Brush fillBrush;
                     switch (((ClientStatus) e.Value))
                     {
                        case ClientStatus.Running:
                           fillPen = Pens.DarkGreen;
                           fillBrush = Brushes.DarkGreen;
                           break;
                        case ClientStatus.RunningNoFrameTimes:
                           fillPen = Pens.Yellow;
                           fillBrush = Brushes.Yellow;
                           break;
                        case ClientStatus.Stopped:
                        case ClientStatus.Hung:
                           fillPen = Pens.DarkRed;
                           fillBrush = Brushes.DarkRed;
                           break;
                        case ClientStatus.Paused:
                           fillPen = Pens.Orange;
                           fillBrush = Brushes.Orange;
                           break;
                        case ClientStatus.Offline:
                           fillPen = Pens.Gray;
                           fillBrush = Brushes.Gray;
                           break;
                        default:
                           fillPen = Pens.Gray;
                           fillBrush = Brushes.Gray;
                           break;
                     }
                     e.Graphics.DrawRectangle(fillPen, newRect);
                     e.Graphics.FillRectangle(fillBrush, newRect);

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
                     if (date.Equals(DateTime.MinValue) == false)
                     {
                        e.Graphics.DrawString(GetFormattedDownloadTimeString(date), e.CellStyle.Font,
                           textColor, e.CellBounds.X + 2,
                           e.CellBounds.Y + 2, StringFormat.GenericDefault);

                        e.Handled = true;
                     }
                  }
                  else if (dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
                  {
                     DateTime date = (DateTime)e.Value;
                     if (date.Equals(DateTime.MinValue) == false)
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
         string formatString = "{1}min {2}sec";
         if (span.Hours > 0)
         {
            formatString = "{0}hr {1}min {2}sec";
         }

         return String.Format(formatString, span.Hours, span.Minutes, span.Seconds);
      }

      private static string GetFormattedEtaString(TimeSpan span)
      {
         string formatString = "{1}hr {2}min";
         if (span.Days > 0)
         {
            formatString = "{0}d {1}hr {2}min";
         }

         return String.Format(formatString, span.Days, span.Hours, span.Minutes);
      }

      private static string GetFormattedDownloadTimeString(DateTime date)
      {
         if (date.Equals(DateTime.MinValue))
         {
            return String.Empty;
         }

         TimeSpan span = DateTime.Now.Subtract(date);
         string formatString = "{1}hr {2}min ago";
         if (span.Days > 0)
         {
            formatString = "{0}d {1}hr {2}min ago";
         }

         return String.Format(formatString, span.Days, span.Hours, span.Minutes);
      }

      private static string GetFormattedDeadlineString(DateTime date)
      {
         if (date.Equals(DateTime.MinValue))
         {
            return String.Empty;
         }

         TimeSpan span = date.Subtract(DateTime.Now);
         string formatString = "In {1}hr {2}min";
         if (span.Days > 0)
         {
            formatString = "In {0}d {1}hr {2}min";
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
            if (hti.Type == DataGridViewHitTestType.Cell)
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
                     Debug.WriteToHfmConsole(TraceLevel.Error,
                                             String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
                     MessageBox.Show(
                        String.Format(
                           "Failed to show client '{0}' files.\n\nPlease check the current File Explorer defined in the Preferences.",
                           Instance.InstanceName));
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
         if (RetrievalInProgress)
         {
            MessageBox.Show(this, "Retrieval in progress... please wait to create a new config file.", 
               Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (CanContinueDestructiveOp(sender, e))
         {
            HostInstances.SaveCurrentUnitInfo();
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
         if (RetrievalInProgress)
         {
            MessageBox.Show(this, "Retrieval in progress... please wait to open another config file.",
               Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (CanContinueDestructiveOp(sender, e))
         {
            openConfigDialog.DefaultExt = "hfm";
            openConfigDialog.Filter = "HFM Configuration Files|*.hfm";
            openConfigDialog.FileName = ConfigFilename;
            openConfigDialog.RestoreDirectory = true;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
            {
               HostInstances.SaveCurrentUnitInfo();
               LoadFile(openConfigDialog.FileName);
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
         if (ConfigFilename == String.Empty)
         {
            mnuFileSaveas_Click(sender, e);
         }
         else
         {
            HostInstances.ToXml(ConfigFilename);
            ChangedAfterSave = false;
         }
      }

      /// <summary>
      /// Save the current host configuration as a new file
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileSaveas_Click(object sender, EventArgs e)
      {
         if (saveConfigDialog.ShowDialog() == DialogResult.OK)
         {
            ConfigFilename = saveConfigDialog.FileName;
            mnuFileSave_Click(sender, e);
         }
      }

      /// <summary>
      /// Import FahMon clientstab.txt configuration
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void mnuFileImportFahMon_Click(object sender, EventArgs e)
      {
         if (RetrievalInProgress)
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
            SetTimerState();
            HostInstances.OfflineClientsLast = PreferenceSet.Instance.OfflineLast;
            TraceLevel newLevel = (TraceLevel)PreferenceSet.Instance.MessageLevel;
            if (newLevel != TraceLevelSwitch.Instance.Level)
            {
               TraceLevelSwitch.Instance.Level = newLevel;
               Debug.WriteToHfmConsole(String.Format("Debug Message Level Changed: {0}", newLevel));
            }

            // If the PPD Calculation style changed, we need to do a new Retrieval
            if (currentPpdCalc.Equals(PreferenceSet.Instance.PpdCalculation))
            {
               RefreshDisplay();
            }
            else
            {
               QueueNewRetrieval();
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
               MessageBox.Show(String.Format("Host Name '{0}' already exists.", newHost.txtName.Text));
            }
            else
            {
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
               System.Diagnostics.Debug.Assert(xHost != null);
               
               // Add the new Host Instance and Queue Retrieval
               HostInstances.Add(xHost);
               QueueNewRetrieval(xHost);
               ChangedAfterSave = true;
            }
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
            
            // if the host key changed
            if (oldName != Instance.InstanceName)
            {
               HostInstances.UpdateDisplayInstanceName(oldName, Instance.InstanceName);
               HostInstances.InstanceCollection.Remove(oldName);
               HostInstances.Add(Instance);
               
               ProteinBenchmarkCollection.Instance.UpdateInstanceName(oldName, Instance.InstanceName);
            }
            // if the path changed
            if (oldPath != Instance.Path)
            {
               ProteinBenchmarkCollection.Instance.UpdateInstancePath(Instance.InstanceName, Instance.Path);
            }
            QueueNewRetrieval(Instance);
            ChangedAfterSave = true;
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
         ChangedAfterSave = true;
      }

      /// <summary>
      /// Refreshes the currently selected client
      /// </summary>
      private void mnuClientsRefreshSelected_Click(object sender, EventArgs e)
      {
         if (dataGridView1.SelectedRows.Count == 0) return;
      
         QueueNewRetrieval(HostInstances.InstanceCollection[dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString()]);
      }

      /// <summary>
      /// Refreshes all clients
      /// </summary>
      private void mnuClientsRefreshAll_Click(object sender, EventArgs e)
      {
         QueueNewRetrieval();
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
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
               MessageBox.Show(String.Format("Failed to show client '{0}' FAHLog file.\n\nPlease check the current Log File Viewer defined in the Preferences.", Instance.InstanceName));
            }
         }
         else
         {
            MessageBox.Show(String.Format("Cannot find client '{0}' FAHLog file.", Instance.InstanceName));
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
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
               MessageBox.Show(String.Format("Failed to show client '{0}' files.\n\nPlease check the current File Explorer defined in the Preferences.", Instance.InstanceName));
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

         //RefreshDisplay();
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
         ProteinCollection.Instance.DownloadFromStanford();
      }

      /// <summary>
      /// Show the Benchmarks Dialog
      /// </summary>
      private void mnuToolsBenchmarks_Click(object sender, EventArgs e)
      {
         ClientInstance Instance = HostInstances.InstanceCollection[dataGridView1.SelectedRows[0].Cells["Name"].Value.ToString()];
         
         frmBenchmarks frm = new frmBenchmarks(HostInstances, Instance.CurrentUnitInfo.ProjectID);
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
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show("Failed to show EOC User Stats page.");
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
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show("Failed to show Stanford User Stats page.");
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
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show("Failed to show EOC Team Stats page.");
         }
      }

      private void mnuWebHFMGoogleCode_Click(object sender, EventArgs e)
      {
         try
         {
            Process.Start("http://code.google.com/p/hfm-net/");
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            MessageBox.Show("Failed to show HFM.NET Google Code page.");
         }
      }
      #endregion

      #region Background Work Routines
      /// <summary>
      /// Disable and enable the background work timers
      /// </summary>
      private void SetTimerState()
      {
         // Disable timers if no hosts
         if (HostInstances.Count < 1)
         {
            Debug.WriteToHfmConsole(TraceLevel.Info, "No Hosts - Stopping Both Background Timer Loops");
            bgWorkTimer.Stop();
            webGenTimer.Stop();
            return;
         }

         // Enable the data retrieval timer
         if (PreferenceSet.Instance.SyncOnSchedule)
         {
            StartBackgroundTimer();
         }
         else
         {
            Debug.WriteToHfmConsole(TraceLevel.Info, "Stopping Background Timer Loop");
            bgWorkTimer.Stop();
         }

         // Enable the web generation timer
         if (PreferenceSet.Instance.GenerateWeb && PreferenceSet.Instance.WebGenAfterRefresh == false)
         {
            webGenTimer.Interval = Convert.ToInt32(PreferenceSet.Instance.GenerateInterval) * MinToMillisec;
            webGenTimer.Start();
         }
         else
         {
            webGenTimer.Stop();
         }
      }

      /// <summary>
      /// When the host refresh timer expires, refresh all the hosts
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void bgWorkTimer_Tick(object sender, EventArgs e)
      {
         Debug.WriteToHfmConsole(TraceLevel.Info, "Running Background Timer...");
         QueueNewRetrieval();
      }

      /// <summary>
      /// When the web gen timer expires, refresh the website files
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void webGenTimer_Tick(object sender, EventArgs e)
      {
         if (PreferenceSet.Instance.GenerateWeb == false) return;
      
         //webGenTimer.Stop();
         StopWebGenTimer();

         Debug.WriteToHfmConsole(TraceLevel.Info,
                                 String.Format("{0} Starting webGen.", Debug.FunctionName));
         
         StreamWriter sw;

         // Create the web folder (just in case)
         if (Directory.Exists(PreferenceSet.Instance.WebRoot) == false)
         {
            Directory.CreateDirectory(PreferenceSet.Instance.WebRoot);
         }

         // Copy the CSS file to the output directory
         string sAppPath = Path.GetDirectoryName(Application.ExecutablePath);
         string sCSSFileName = Path.Combine(Path.Combine(sAppPath, "CSS"), PreferenceSet.Instance.CSSFileName);
         if (File.Exists(sCSSFileName))
         {
            File.Copy(sCSSFileName, Path.Combine(PreferenceSet.Instance.WebRoot, PreferenceSet.Instance.CSSFileName), true);
         }

         // Generate the index page
         sw = new StreamWriter(Path.Combine(PreferenceSet.Instance.WebRoot, "index.html"), false);
         sw.Write(XMLGen.OverviewXml("WebOverview.xslt", HostInstances));
         sw.Close();

         // Generate the summary page
         sw = new StreamWriter(Path.Combine(PreferenceSet.Instance.WebRoot, "summary.html"), false);
         sw.Write(XMLGen.SummaryXml("WebSummary.xslt", HostInstances));
         sw.Close();

         // Generate a page per instance
         foreach (KeyValuePair<String, ClientInstance> kvp in HostInstances.InstanceCollection)
         {
            sw = new StreamWriter(Path.Combine(PreferenceSet.Instance.WebRoot, Path.ChangeExtension(kvp.Value.InstanceName, ".html")), false);
            sw.Write(XMLGen.InstanceXml("WebInstance.xslt", kvp.Value));
            sw.Close();
         }

         //webGenTimer.Start();
         StartWebGenTimer();
      }
      
      private void StopWebGenTimer()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(StopWebGenTimer));
         }
         else
         {
            webGenTimer.Stop();
         }
      }

      private void StartWebGenTimer()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(StartWebGenTimer));
         }
         else
         {
            if (PreferenceSet.Instance.GenerateWeb && PreferenceSet.Instance.WebGenAfterRefresh == false)
            {
               webGenTimer.Start();
            }
         }
      }

      /// <summary>
      /// Stick each Instance in the background thread queue to retrieve the info for a given Instance
      /// </summary>
      private void QueueNewRetrieval()
      {
         // don't fire this process twice
         if (RetrievalInProgress)
         {
            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Retrieval Already In Progress...", Debug.FunctionName));
            return;
         }
         
         // only fire if there are Hosts
         if (HostInstances.Count > 0)
         {
            if (PreferenceSet.Instance.GenerateWeb && PreferenceSet.Instance.WebGenAfterRefresh)
            {
               ExecuteWebGenAfterRetrieve = true;
            }

            Debug.WriteToHfmConsole(TraceLevel.Info, "Stopping Background Timer Loop");
            bgWorkTimer.Stop();
         
            // set full retrieval flag
            RetrievalInProgress = true;

            // fire the retrieval wrapper thread (basically a wait thread off the UI thread)
            new MethodInvoker(DoRetrievalWrapper).BeginInvoke(null, null);
         }
      }
      
      /// <summary>
      /// Wraps the DoRetrieval function on a seperate thread and fires post retrieval processes
      /// </summary>
      private void DoRetrievalWrapper()
      {
         // fire the actual retrieval thread
         IAsyncResult async = new MethodInvoker(DoRetrieval).BeginInvoke(null, null);
         // wait for completion
         async.AsyncWaitHandle.WaitOne();
         
         // run post retrieval processes
         if (ExecuteWebGenAfterRetrieve)
         {
            ExecuteWebGenAfterRetrieve = false;
            // do a web gen
            webGenTimer_Tick(null, null);
         }

         // Enable the data retrieval timer
         if (PreferenceSet.Instance.SyncOnSchedule)
         {
            StartBackgroundTimer();
         }

         // clear full retrieval flag
         RetrievalInProgress = false;
         // Save the benchmark collection
         ProteinBenchmarkCollection.Instance.Serialize();
      }
      
      private void StartBackgroundTimer()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(StartBackgroundTimer));
         }
         else
         {
            bgWorkTimer.Interval = Convert.ToInt32(PreferenceSet.Instance.SyncTimeMinutes) * MinToMillisec;
            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("Starting Background Timer Loop: {0} Minutes",
                                                                    PreferenceSet.Instance.SyncTimeMinutes));
            bgWorkTimer.Start();
         }
      }

      /// <summary>
      /// Do a full retrieval operation
      /// </summary>
      private void DoRetrieval()
      {
         // get flag synchronous or asynchronous - we don't want this flag to change on us
         // in the middle of a retrieve, so grab it now and use the local copy
         bool Synchronous = PreferenceSet.Instance.SyncOnLoad;

         // copy the current instance keys into local array
         int numInstances = HostInstances.Count;
         string[] instanceKeys = new string[numInstances];
         HostInstances.InstanceCollection.Keys.CopyTo(instanceKeys, 0);

         List<WaitHandle> waitHandleList = new List<WaitHandle>();
         for (int i = 0; i < numInstances; )
         {
            waitHandleList.Clear();
            // loop through the instances (can only handle up to 64 wait handles at a time)
            for (int j = 0; j < 64 && i < numInstances; j++)
            {
               // try to get the key value from the collection, if the value is not found then
               // the user removed a client in the middle of a retrieve process, ignore the key
               ClientInstance Instance;
               if (HostInstances.InstanceCollection.TryGetValue(instanceKeys[i], out Instance))
               {
                  if (Synchronous) // do the individual retrieves on a single thread
                  {
                     RetrieveInstance(Instance);
                  }
                  else // fire individual threads to do the their own retrieve simultaneously
                  {
                     //ThreadPool.QueueUserWorkItem(new WaitCallback(RetrieveInstance), Instance);
                     RetrieveInstanceDelegate del = RetrieveInstance;
                     IAsyncResult async = del.BeginInvoke(Instance, null, null);

                     // get the wait handle for each invoked delegate
                     waitHandleList.Add(async.AsyncWaitHandle);
                  }
               }

               i++; // increment the outer loop counter
               //Application.DoEvents();
            }
            
            if (Synchronous == false)
            {
               WaitHandle[] waitHandles = waitHandleList.ToArray();
               // wait for all invoked threads to complete
               WaitHandle.WaitAll(waitHandles);
            }
         }
      }

      /// <summary>
      /// Stick this Instance in the background thread queue to retrieve the info for the given Instance
      /// </summary>
      private void QueueNewRetrieval(ClientInstance Instance)
      {
         new RetrieveInstanceDelegate(RetrieveInstance).BeginInvoke(Instance, null, null);
      }

      /// <summary>
      /// Delegate used for asynchronous instance retrieval
      /// </summary>
      /// <param name="Instance"></param>
      private delegate void RetrieveInstanceDelegate(ClientInstance Instance);

      /// <summary>
      /// Stub to execute retrieve and refresh display
      /// </summary>
      /// <param name="Instance"></param>
      private void RetrieveInstance(ClientInstance Instance)
      {
         Instance.Retrieve();
         // Update the UI (this is confirmed needed)
         RefreshDisplay();
      }
      
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
                  if (InvokeRequired)
                  {
                     Invoke(new MethodInvoker(cm.Refresh));
                  }
                  else
                  {
                     cm.Refresh();
                     //dataGridView1.Invalidate();
                  }
               }

               ApplySort(SortColumnOrder);
            }

            RefreshControls();
         }
         catch (ObjectDisposedException ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
         }
      }

      /// <summary>
      /// Delegate used for Post-Message Invoke to UI Thread
      /// </summary>
      /// <param name="order"></param>
      private delegate void ApplySortDelegate(SortOrder order);
      
      /// <summary>
      /// Apply Sort to Data Grid View
      /// </summary>
      /// <param name="order"></param>
      private void ApplySort(SortOrder order)
      {
         if (InvokeRequired)
         {
            Invoke(new ApplySortDelegate(ApplySort), new object[] {order});
         }
         else
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
      }

      /// <summary>
      /// Refresh remaining UI controls with current data
      /// </summary>
      private void RefreshControls()
      {
         Double newTotalPPD = 0;
         Int32 newGoodHosts = 0;
         //Int32 newPausedHosts = 0;
         //Int32 newHungHosts = 0;
         //Int32 newStoppedHosts = 0;
         //Int32 newOfflineUnknownHosts = 0;
         foreach (KeyValuePair<String, ClientInstance> kvp in HostInstances.InstanceCollection)
         {
            newTotalPPD += kvp.Value.CurrentUnitInfo.PPD;
            
            switch (kvp.Value.Status)
            {
               case ClientStatus.Running:
               case ClientStatus.RunningNoFrameTimes:
                  newGoodHosts++;
                  break;
               //case ClientStatus.Paused:
               //   newPausedHosts++;
               //   break;
               //case ClientStatus.Hung:
               //   newHungHosts++;
               //   break;
               //case ClientStatus.Stopped:
               //   newStoppedHosts++;
               //   break;
               //case ClientStatus.Offline:
               //case ClientStatus.Unknown:
               //   newOfflineUnknownHosts++;
               //   break;
            }
         }

         TotalPPD = newTotalPPD;
         GoodHosts = newGoodHosts;
         
         SetNotifyIconText(String.Format("{0} Clients Working\n{1} Clients Offline\n{2:###,###,##0.00} PPD (Est)",
                                         GoodHosts, (HostInstances.InstanceCollection.Count - GoodHosts), TotalPPD));
         RefreshStatusLabels();                        
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
            Invoke(new SimpleVoidStringDelegate(SetNotifyIconText), new object[] { val });
         }
         else
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
      private void RefreshStatusLabels()
      {
         if (GoodHosts == 1)
         {
            SetStatusLabelHostsText(String.Format("{0} Clients", GoodHosts));
         }
         else
         {
            SetStatusLabelHostsText(String.Format("{0} Clients", GoodHosts));
         }

         SetStatusLabelPPDText(String.Format("{0:###,###,##0.00} PPD", TotalPPD));
      }

      /// <summary>
      /// Sets the Hosts Status Label Text (Thread Safe)
      /// </summary>
      /// <param name="val">Text to set</param>
      private void SetStatusLabelHostsText(string val)
      {
         if (InvokeRequired)
         {
            Invoke(new SimpleVoidStringDelegate(SetStatusLabelHostsText), new object[] { val });
         }
         else
         {
            statusLabelHosts.Text = val;
         }
      }

      /// <summary>
      /// Sets the PPD Status Label Text (Thread Safe)
      /// </summary>
      /// <param name="val">Text to set</param>
      private void SetStatusLabelPPDText(string val)
      {
         if (InvokeRequired)
         {
            Invoke(new SimpleVoidStringDelegate(SetStatusLabelPPDText), new object[] { val });
         }
         else
         {
            statusLabelPPW.Text = val;
         }
      }

      /// <summary>
      /// Forces a log and screen refresh when the Stanford info is updated
      /// </summary>
      private void Instance_ProjectInfoUpdated(object sender, ProjectInfoUpdatedEventArgs e)
      {
         // Do Retrieve on all clients after Project Info is updated (this is confirmed needed)
         QueueNewRetrieval();
      }

      /// <summary>
      /// Forces a screen refresh when the instance collection is updated
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void HostInstances_RefreshCollection(object sender, EventArgs e)
      {
         // Update the UI (this is confirmed needed)
         RefreshDisplay();
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
         DateTime Start = Debug.ExecStart;
      
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
                  Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} Failed to clear cache file '{1}'.", Debug.FunctionName, fi.Name));
               }
            }
         }

         Debug.WriteToHfmConsole(TraceLevel.Info,
                                 String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
      }

      /// <summary>
      /// Restore Form State
      /// </summary>
      private void RestoreFormPreferences()
      {
         // Restore state data
         Point location = PreferenceSet.Instance.FormLocation;
         Size size = PreferenceSet.Instance.FormSize;

         if (location.X != 0 && location.Y != 0)
         {
            // Set StartPosition to manual
            StartPosition = FormStartPosition.Manual;
            Location = location;
         }
         if (size.Width != 0 && size.Height != 0)
         {
            if (PreferenceSet.Instance.FormLogVisible == false)
            {
               size = new Size(size.Width, size.Height + PreferenceSet.Instance.FormLogWindowHeight);
            }
            Size = size;
            splitContainer1.SplitterDistance = PreferenceSet.Instance.FormSplitLocation;
         }

         if (PreferenceSet.Instance.FormLogVisible == false)
         {
            ShowHideLog(false);
         }
         
         if (PreferenceSet.Instance.FormSortColumn != String.Empty &&
             PreferenceSet.Instance.FormSortOrder != SortOrder.None)
         {
            SortColumnName = PreferenceSet.Instance.FormSortColumn;
            SortColumnOrder = PreferenceSet.Instance.FormSortOrder;
         }
         
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
      /// Test current application status for changes; ask for confirmation if necessary.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      /// <returns>Whether or not a destructive operation can be performed</returns>
      private bool CanContinueDestructiveOp(object sender, EventArgs e)
      {
         if (ChangedAfterSave)
         {
            DialogResult qResult = MessageBox.Show("There are changes to the configuration that have not been saved. Would you like to save these changes?\n\nYes - Continue and save the changes\nNo - Continue and do not save the changes\nCancel - Do not continue", "Warning", MessageBoxButtons.YesNoCancel);
            switch (qResult)
            {
               case DialogResult.Yes:
                  mnuFileSave_Click(sender, e);
                  if (ChangedAfterSave)
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

         // Re-initialise the list of host instances
         HostInstances = new FoldingInstanceCollection();
         dataGridView1.DataSource = HostInstances.GetDisplayCollection();
         HostInstances.RefreshCollection += HostInstances_RefreshCollection;
         HostInstances.OfflineClientsLast = PreferenceSet.Instance.OfflineLast;

         // and the config filename - this is a new one now
         ConfigFilename = String.Empty;

         // Technically there are now no changes after a save
         ChangedAfterSave = false;

         // This will disable the timers, we have no hosts
         SetTimerState();

         // Update the UI (this is confirmed needed)
         RefreshDisplay();
      }

      /// <summary>
      /// Loads a configuration file into memory
      /// </summary>
      /// <param name="filename"></param>
      private void LoadFile(string filename)
      {
         // Clear the UI
         ClearUI();
         // Set config file name
         ConfigFilename = filename;
         // Read the config file
         HostInstances.FromXml(filename);
         // Get client logs         
         QueueNewRetrieval();
         // Start Retrieval and WebGen Timers
         SetTimerState();
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
         // Collection imported from external source
         ChangedAfterSave = true;
         // Get client logs         
         QueueNewRetrieval();
         // Start Retrieval and WebGen Timers
         SetTimerState();
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
      /// Fire File Browser Process
      /// </summary>
      /// <param name="path"></param>
      private static void StartFileBrowser(string path)
      {
         Process.Start(PreferenceSet.Instance.FileExplorer, path);
      }
      #endregion
   }
}