/*
 * HFM.NET - Main UI DataGrid Handlers
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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Controls;

namespace HFM.Forms
{
   enum PaintCell
   {
      Status,
      Time,
      Warning,
      EtaDate
   }

   // ReSharper disable InconsistentNaming
   public partial class MainForm
   // ReSharper restore InconsistentNaming
   {
      /// <summary>
      /// Holds Current Mouse Over Row Index
      /// </summary>
      private int _currentMouseOverRow = -1;

      /// <summary>
      /// Holds Current Mouse Over Column Index
      /// </summary>
      private int _currentMouseOverColumn = -1;
   
      private void DataGridViewMouseMove(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo info = dataGridView1.HitTest(e.X, e.Y);

         // Only draw Tooltips if we've actually changed cells - Issue 99
         if (info.RowIndex == _currentMouseOverRow && info.ColumnIndex == _currentMouseOverColumn)
         {
            return;
         }
         
         // Update the current cell indexes
         _currentMouseOverRow = info.RowIndex;
         _currentMouseOverColumn = info.ColumnIndex;

         #region Draw or Hide the Tooltip
         if (info.RowIndex > -1)
         {
            var slotModel = _presenter.FindSlotModel(dataGridView1.Rows[info.RowIndex].Cells["Name"].Value.ToString());
            if (slotModel == null)
            {
               return;
            }

            // ReSharper disable PossibleNullReferenceException
            if (dataGridView1.Columns["Status"].Index == info.ColumnIndex)
            {
               toolTipGrid.Show(slotModel.Status.ToString(), dataGridView1, e.X + 15, e.Y);
               return;
            }
            
            if (dataGridView1.Columns["Username"].Index == info.ColumnIndex)
            {
               if (slotModel.UsernameOk == false)
               {
                  toolTipGrid.Show("Client's User Name does not match the configured User Name", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
            else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == info.ColumnIndex)
            {
               if (_prefs.Get<bool>(Preference.DuplicateProjectCheck) && slotModel.ProjectIsDuplicate)
               {
                  toolTipGrid.Show("Client is working on the same work unit as another client", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
            else if (dataGridView1.Columns["Name"].Index == info.ColumnIndex)
            {
               if (_prefs.Get<bool>(Preference.DuplicateUserIdCheck) && slotModel.UserIdIsDuplicate)
               {
                  toolTipGrid.Show("Client is working with the same User and Machine ID as another client", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
            // ReSharper restore PossibleNullReferenceException
         }

         toolTipGrid.Hide(dataGridView1);
         #endregion
      }

      /// <summary>
      /// Override Cell Painting in the DataGridView
      /// </summary>
      private void DataGridViewCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
      {
         if (e.Value == null) return;

         if (e.RowIndex >= 0)
         {
            // ReSharper disable PossibleNullReferenceException
            if (dataGridView1.Columns["Status"].Index == e.ColumnIndex)
            {
               PaintGridCell(PaintCell.Status, e);
            }
            else if (dataGridView1.Columns["Name"].Index == e.ColumnIndex)
            {
               #region Duplicate User and Machine ID Custom Paint
               if (_prefs.Get<bool>(Preference.DuplicateUserIdCheck))
               {
                  var slotModel = _presenter.FindSlotModel(dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString());
                  if (slotModel != null && slotModel.UserIdIsDuplicate)
                  {
                     PaintGridCell(PaintCell.Warning, e);
                  }
               }
               #endregion
            }
            else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == e.ColumnIndex)
            {
               #region Duplicate Project Custom Paint
               
               if (_prefs.Get<bool>(Preference.DuplicateProjectCheck))
               {
                  var slotModel = _presenter.FindSlotModel(dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString());
                  if (slotModel != null && slotModel.ProjectIsDuplicate)
                  {
                     PaintGridCell(PaintCell.Warning, e);
                  }
               }
               #endregion
            }
            else if (dataGridView1.Columns["Username"].Index == e.ColumnIndex)
            {
               #region Username Incorrect Custom Paint
               var slotModel = _presenter.FindSlotModel(dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString());
               if (slotModel != null && !slotModel.UsernameOk)
               {
                  PaintGridCell(PaintCell.Warning, e);
               }
               #endregion
            }
            else if ((dataGridView1.Columns["TPF"].Index == e.ColumnIndex ||
                      dataGridView1.Columns["ETA"].Index == e.ColumnIndex ||
                      dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
                      dataGridView1.Columns["Deadline"].Index == e.ColumnIndex) &&
                      _prefs.Get<TimeStyleType>(Preference.TimeStyle).Equals(TimeStyleType.Formatted))
            {
               PaintGridCell(PaintCell.Time, e);
            }
            else if (dataGridView1.Columns["ETA"].Index == e.ColumnIndex)
            {
               #region ETA as Date Custom Paint
               if (_prefs.Get<bool>(Preference.EtaDate))
               {
                  var slotModel = _presenter.FindSlotModel(dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString());
                  if (slotModel != null)
                  {
                     PaintGridCell(PaintCell.EtaDate, slotModel.ETADate, e);
                  }
               }
               #endregion
            }
            else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex ||
                     dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
            {
               var date = (DateTime)e.Value;
               if (date.Equals(DateTime.MinValue))
               {
                  PaintGridCell(PaintCell.Time, e);
               }
            }
            // ReSharper restore PossibleNullReferenceException
         }
      }

      /// <summary>
      /// Custom Paint Grid Cells
      /// </summary>
      private void PaintGridCell(PaintCell paint, DataGridViewCellPaintingEventArgs e)
      {
         PaintGridCell(paint, null, e);
      }
      
      /// <summary>
      /// Custom Paint Grid Cells
      /// </summary>
      private void PaintGridCell(PaintCell paint, object data, DataGridViewCellPaintingEventArgs e)
      {
         using (Brush gridBrush = new SolidBrush(dataGridView1.GridColor),
                      backColorBrush = new SolidBrush(e.CellStyle.BackColor),
                      selectionColorBrush = new SolidBrush(e.CellStyle.SelectionBackColor))
         {
            using (var gridLinePen = new Pen(gridBrush))
            {
               #region Erase (Set BackColor) the Cell and Choose Text Color
               
               Color textColor = Color.Black;
               if (paint.Equals(PaintCell.Status))
               {
                  e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
               }
               else if (paint.Equals(PaintCell.Time) ||
                        paint.Equals(PaintCell.EtaDate))
               {
                  if (dataGridView1.Rows[e.RowIndex].Selected)
                  {
                     e.Graphics.FillRectangle(selectionColorBrush, e.CellBounds);
                     textColor = Color.White;
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
                     textColor = Color.White;
                  }
                  else
                  {
                     e.Graphics.FillRectangle(Brushes.Orange, e.CellBounds);
                  }
               }
               else
               {
                  throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                     "PaintCell Type '{0}' is not implemented", paint));
               }
               
               #endregion

               #region Draw the bottom grid line
               
               //if (Core.Application.IsRunningOnMono)
               //{
               //   e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
               //                       e.CellBounds.Top, e.CellBounds.Right,
               //                       e.CellBounds.Top);
               //}
               //else
               //{
                  e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                                      e.CellBounds.Bottom - 1, e.CellBounds.Right,
                                      e.CellBounds.Bottom - 1);
               //}

               #endregion

               #region Draw Cell Content (Text or Shapes)
               
               if (paint.Equals(PaintCell.Status))
               {
                  //// this is new... sometimes being passed a null
                  //// value here.  check and skip if so. - 1/28/12
                  //if (e.Value != null)
                  //{
                     // Draw the inset highlight box.
                     var newRect = new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4,
                                                 e.CellBounds.Width - 10, e.CellBounds.Height - 10);

                     var status = (SlotStatus)e.Value;
                     e.Graphics.DrawRectangle(status.GetDrawingPen(), newRect);
                     e.Graphics.FillRectangle(status.GetDrawingBrush(), newRect);
                  //}
                  //else
                  //{
                  //   Debug.WriteLine("Status value is null.");
                  //}
               }
               else if (paint.Equals(PaintCell.Time))
               {
                  //if (e.Value != null)
                  //{
                     PaintTimeBasedCellValue(textColor, e);
                  //}
               }
               else if (paint.Equals(PaintCell.EtaDate))
               {
                  //if (e.Value != null)
                  //{
                     Debug.Assert(data != null);
                     DrawText(((DateTime)data).ToDateString(), textColor, e);
                  //}
               }
               else if (paint.Equals(PaintCell.Warning))
               {
                  // Draw the text content of the cell
                  //if (e.Value != null)
                  //{
                     DrawText(e.Value.ToString(), textColor, e);
                  //}
               }
               else
               {
                  throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
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
      private void PaintTimeBasedCellValue(Color textColor, DataGridViewCellPaintingEventArgs e)
      {
         string drawString = String.Empty;

         // ReSharper disable PossibleNullReferenceException
         if (dataGridView1.Columns["TPF"].Index == e.ColumnIndex)
         {
            var span = (TimeSpan)e.Value;
            drawString = GetFormattedTpfString(span);
         }
         else if (dataGridView1.Columns["ETA"].Index == e.ColumnIndex)
         {
            var span = (TimeSpan)e.Value;
            drawString = GetFormattedEtaString(span);
         }
         else if (dataGridView1.Columns["DownloadTime"].Index == e.ColumnIndex)
         {
            var date = (DateTime)e.Value;
            drawString = GetFormattedDownloadTimeString(date);
         }
         else if (dataGridView1.Columns["Deadline"].Index == e.ColumnIndex)
         {
            var date = (DateTime)e.Value;
            drawString = GetFormattedDeadlineString(date);
         }
         // ReSharper restore PossibleNullReferenceException

         if (drawString.Length != 0)
         {
            DrawText(drawString, textColor, e);
         }
      }

      private static void DrawText(string value, Color color, DataGridViewCellPaintingEventArgs e)
      {
         TextRenderer.DrawText(e.Graphics, value, e.CellStyle.Font, new Point(e.CellBounds.X, e.CellBounds.Y + 2), color);
      }

      /// <summary>
      /// Measure Text and set Column Width on Double-Click
      /// </summary>
      private void DataGridViewColumnDividerDoubleClick(object sender, DataGridViewColumnDividerDoubleClickEventArgs e)
      {
         AutoSizeColumn(e.ColumnIndex);
         e.Handled = true;
      }
      
      private void AutoSizeColumn(int columnIndex)
      {
         var font = new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Regular);
         Graphics g = dataGridView1.CreateGraphics();

         SizeF s;
         int width = 0;

         for (int i = 0; i < dataGridView1.Rows.Count; i++)
         {
            if (dataGridView1.Rows[i].Cells[columnIndex].Value != null)
            {
               // ReSharper disable PossibleNullReferenceException
               if (dataGridView1.Columns["Status"].Index == columnIndex)
               {
                  dataGridView1.Columns[columnIndex].Width = (int)(50 * g.GetDpiScale());
                  return;
               }
               if (dataGridView1.Columns["Progress"].Index == columnIndex)
               {
                  dataGridView1.Columns[columnIndex].Width = (int)(60 * g.GetDpiScale());
                  return;
               }

               string formattedString = String.Empty;
               if ((dataGridView1.Columns["TPF"].Index == columnIndex ||
                    dataGridView1.Columns["ETA"].Index == columnIndex ||
                    dataGridView1.Columns["DownloadTime"].Index == columnIndex ||
                    dataGridView1.Columns["Deadline"].Index == columnIndex) &&
                    _prefs.Get<TimeStyleType>(Preference.TimeStyle).Equals(TimeStyleType.Formatted))
               {
                  if (dataGridView1.Columns["TPF"].Index == columnIndex)
                  {
                     formattedString = GetFormattedTpfString((TimeSpan)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
                  else if (dataGridView1.Columns["ETA"].Index == columnIndex)
                  {
                     formattedString = GetFormattedEtaString((TimeSpan)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
                  else if (dataGridView1.Columns["DownloadTime"].Index == columnIndex)
                  {
                     formattedString = GetFormattedDownloadTimeString((DateTime)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
                  else if (dataGridView1.Columns["Deadline"].Index == columnIndex)
                  {
                     formattedString = GetFormattedDeadlineString((DateTime)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
               }
               else if (dataGridView1.Columns["ETA"].Index == columnIndex && _prefs.Get<bool>(Preference.EtaDate))
               {
                  var slotModel = _presenter.FindSlotModel(dataGridView1.Rows[i].Cells["Name"].Value.ToString());
                  formattedString = slotModel.ETADate.ToDateString();
               }
               else if (dataGridView1.Columns["DownloadTime"].Index == columnIndex ||
                        dataGridView1.Columns["Deadline"].Index == columnIndex)
               {
                  formattedString =
                     ((DateTime)dataGridView1.Rows[i].Cells[columnIndex].Value).ToDateString(
                     dataGridView1.Rows[i].Cells[columnIndex].FormattedValue.ToString());
               }
               else
               {
                  formattedString = dataGridView1.Rows[i].Cells[columnIndex].FormattedValue.ToString();
               }
               // ReSharper restore PossibleNullReferenceException

               s = TextRenderer.MeasureText(formattedString, font);
               //s = g.MeasureString(formattedString, font);

               if (width < s.Width)
               {
                  width = (int)(s.Width + (int)(10 * g.GetDpiScale()));
               }
            }
         }

         if (width >= dataGridView1.Columns[columnIndex].MinimumWidth)
         {
            dataGridView1.Columns[columnIndex].Width = width;
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

      public const int NumberOfDisplayFields = 17;

      public static void SetupDataGridViewColumns(DataGridView dgv)
      {
         // ReSharper disable PossibleNullReferenceException
         dgv.Columns.Add("Status", "Status");
         dgv.Columns["Status"].DataPropertyName = "Status";
         var progressColumn = new DataGridViewProgressColumn();
         progressColumn.Name = "Progress";
         progressColumn.HeaderText = "Progress";
         dgv.Columns.Add(progressColumn);
         dgv.Columns["Progress"].DataPropertyName = "Progress";
         dgv.Columns.Add("Name", "Name");
         dgv.Columns["Name"].DataPropertyName = "Name";
         dgv.Columns.Add("SlotType", "Slot Type");
         dgv.Columns["SlotType"].DataPropertyName = "SlotType";
         dgv.Columns.Add("TPF", "TPF");
         dgv.Columns["TPF"].DataPropertyName = "TPF";
         dgv.Columns.Add("PPD", "PPD");
         dgv.Columns["PPD"].DataPropertyName = "PPD";
         //dgv.Columns.Add("MHz", "MHz");
         //dgv.Columns["MHz"].DataPropertyName = "MHz";
         //dgv.Columns.Add("PPDMHz", "PPD/MHz");
         //dgv.Columns["PPDMHz"].DataPropertyName = "PPDMHz";
         dgv.Columns.Add("ETA", "ETA");
         dgv.Columns["ETA"].DataPropertyName = "ETA";
         dgv.Columns.Add("Core", "Core");
         dgv.Columns["Core"].DataPropertyName = "Core";
         dgv.Columns.Add("CoreId", "Core ID");
         dgv.Columns["CoreId"].DataPropertyName = "CoreId";
         dgv.Columns.Add("ProjectRunCloneGen", "Project (Run, Clone, Gen)");
         dgv.Columns["ProjectRunCloneGen"].DataPropertyName = "ProjectRunCloneGen";
         dgv.Columns.Add("Credit", "Credit");
         dgv.Columns["Credit"].DataPropertyName = "Credit";
         dgv.Columns.Add("Completed", "Completed");
         dgv.Columns["Completed"].DataPropertyName = "Completed";
         dgv.Columns.Add("Failed", "Failed");
         dgv.Columns["Failed"].DataPropertyName = "TotalRunFailedUnits";
         dgv.Columns.Add("Username", "User Name");
         dgv.Columns["Username"].DataPropertyName = "Username";
         dgv.Columns.Add("DownloadTime", "Download Time");
         dgv.Columns["DownloadTime"].DataPropertyName = "DownloadTime";
         dgv.Columns.Add("Deadline", "Deadline");
         dgv.Columns["Deadline"].DataPropertyName = "PreferredDeadline";
         dgv.Columns.Add("Dummy", String.Empty);
         //dataGridView1.Columns["Dummy"].DataPropertyName = "Dummy";
         // ReSharper restore PossibleNullReferenceException
      }
   }
}
