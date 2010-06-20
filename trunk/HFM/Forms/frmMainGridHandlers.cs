/*
 * HFM.NET - Main UI DataGrid Handlers
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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using HFM.Framework;

namespace HFM.Forms
{
   enum PaintCell
   {
      Status,
      Time,
      Warning,
      ClientVersion,
      CoreVersion,
      CompleteStyle,
      EtaDate
   }

   // ReSharper disable InconsistentNaming
   public partial class frmMain
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
   
      private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
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
            IClientInstance instance =
               _clientInstances.Instances[dataGridView1.Rows[info.RowIndex].Cells["Name"].Value.ToString()];

            // ReSharper disable PossibleNullReferenceException
            if (dataGridView1.Columns["Status"].Index == info.ColumnIndex)
            {
               toolTipGrid.Show(instance.Status.ToString(), dataGridView1, e.X + 15, e.Y);
               return;
            }
            
            if (dataGridView1.Columns["Username"].Index == info.ColumnIndex)
            {
               if (instance.IsUsernameOk() == false)
               {
                  toolTipGrid.Show("Client's User Name does not match the configured User Name", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
            else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == info.ColumnIndex)
            {
               if (_Prefs.GetPreference<bool>(Preference.DuplicateProjectCheck) && instance.CurrentUnitInfo.ProjectIsDuplicate)
               {
                  toolTipGrid.Show("Client is working on the same work unit as another client", dataGridView1, e.X + 15, e.Y);
                  return;
               }
            }
            else if (dataGridView1.Columns["Name"].Index == info.ColumnIndex)
            {
               if (_Prefs.GetPreference<bool>(Preference.DuplicateUserIdCheck) && instance.UserIdIsDuplicate)
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
      private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
      {
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
               IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<bool>(Preference.DuplicateUserIdCheck) && instance.UserIdIsDuplicate)
               {
                  PaintGridCell(PaintCell.Warning, e);
               }
               #endregion
            }
            else if (dataGridView1.Columns["ClientType"].Index == e.ColumnIndex)
            {
               IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<bool>(Preference.ShowVersions) && String.IsNullOrEmpty(instance.ClientVersion) == false)
               {
                  PaintGridCell(PaintCell.ClientVersion, instance.ClientVersion, e);
               }
            }
            else if (dataGridView1.Columns["Core"].Index == e.ColumnIndex)
            {
               IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<bool>(Preference.ShowVersions) && String.IsNullOrEmpty(instance.CurrentUnitInfo.CoreVersion) == false)
               {
                  PaintGridCell(PaintCell.CoreVersion, instance.CurrentUnitInfo.CoreVersion, e);
               }
            }
            else if (dataGridView1.Columns["ProjectRunCloneGen"].Index == e.ColumnIndex)
            {
               #region Duplicate Project Custom Paint
               IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<bool>(Preference.DuplicateProjectCheck) && instance.CurrentUnitInfo.ProjectIsDuplicate)
               {
                  PaintGridCell(PaintCell.Warning, e);
               }
               #endregion
            }
            
            else if (dataGridView1.Columns["Complete"].Index == e.ColumnIndex)
            {
               IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay).Equals(CompletedCountDisplayType.ClientTotal))
               {
                  PaintGridCell(PaintCell.CompleteStyle, instance.TotalClientCompletedUnits.ToString(), e);
               }
            }
            else if (dataGridView1.Columns["Username"].Index == e.ColumnIndex)
            {
               #region Username Incorrect Custom Paint
               IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (instance.IsUsernameOk() == false)
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
               var date = (DateTime)e.Value;
               if (date.Equals(DateTime.MinValue))
               {
                  PaintGridCell(PaintCell.Time, e);
               }
            }
            else if (dataGridView1.Columns["ETA"].Index == e.ColumnIndex)
            {
               IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[e.RowIndex].Cells["Name"].Value.ToString()];

               if (_Prefs.GetPreference<bool>(Preference.EtaDate))
               {
                  PaintGridCell(PaintCell.EtaDate, instance.EtaDate, e);
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
               Brush textColor = Brushes.Black;
               
               if (paint.Equals(PaintCell.Status))
               {
                  e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
               }
               else if (paint.Equals(PaintCell.Time) ||
                        paint.Equals(PaintCell.ClientVersion) ||
                        paint.Equals(PaintCell.CoreVersion) ||
                        paint.Equals(PaintCell.CompleteStyle) ||
                        paint.Equals(PaintCell.EtaDate))
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
                  var newRect = new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4,
                                              e.CellBounds.Width - 10, e.CellBounds.Height - 10);

                  // Draw the inset highlight box.
                  var status = (ClientStatus)e.Value;
                  e.Graphics.DrawRectangle(PlatformOps.GetStatusPen(status), newRect);
                  e.Graphics.FillRectangle(PlatformOps.GetStatusBrush(status), newRect);
               }
               else if (paint.Equals(PaintCell.Time))
               {
                  if (e.Value != null)
                  {
                     PaintTimeBasedCellValue(textColor, e);
                  }
               }
               else if (paint.Equals(PaintCell.ClientVersion) ||
                        paint.Equals(PaintCell.CoreVersion))
               {
                  if (e.Value != null)
                  {
                     Debug.Assert(data != null);
                     string value = GetVersionString(e.Value.ToString(), data.ToString());
                     e.Graphics.DrawString(value, e.CellStyle.Font,
                                           textColor, e.CellBounds.X + 1,
                                           e.CellBounds.Y + 2, StringFormat.GenericDefault);
                  }
               }
               else if (paint.Equals(PaintCell.CompleteStyle))
               {
                  if (e.Value != null)
                  {
                     Debug.Assert(data != null);
                     e.Graphics.DrawString(data.ToString(), e.CellStyle.Font,
                                           textColor, e.CellBounds.X + 1,
                                           e.CellBounds.Y + 2, StringFormat.GenericDefault);
                  }
               }
               else if (paint.Equals(PaintCell.EtaDate))
               {
                  if (e.Value != null)
                  {
                     Debug.Assert(data != null);
                     var dateTime = (DateTime)data;
                     string value = GetFormattedDateStringForMeasurement(dateTime, String.Format(CultureInfo.CurrentCulture,
                        "{0} {1}", dateTime.ToShortDateString(), dateTime.ToShortTimeString()));
                     e.Graphics.DrawString(value, e.CellStyle.Font,
                                           textColor, e.CellBounds.X + 1,
                                           e.CellBounds.Y + 2, StringFormat.GenericDefault);
                  }
               }
               else if (paint.Equals(PaintCell.Warning))
               {
                  // Draw the text content of the cell, ignoring alignment.
                  if (e.Value != null)
                  {
                     e.Graphics.DrawString(e.Value.ToString(), e.CellStyle.Font,
                                           textColor, e.CellBounds.X + 1,
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
            e.Graphics.DrawString(drawString, e.CellStyle.Font,
                  textColor, e.CellBounds.X + 1,
                  e.CellBounds.Y + 2, StringFormat.GenericDefault);
         }
      }

      /// <summary>
      /// Measure Text and set Column Width on Double-Click
      /// </summary>
      private void dataGridView1_ColumnDividerDoubleClick(object sender, DataGridViewColumnDividerDoubleClickEventArgs e)
      {
         AutoSizeColumn(e.ColumnIndex);
         e.Handled = true;
      }
      
      private void AutoSizeColumn(int columnIndex)
      {
         var font = new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Regular);
         //Graphics g = dataGridView1.CreateGraphics();

         SizeF s;
         int width = 0;

         for (int i = 0; i < dataGridView1.Rows.Count; i++)
         {
            if (dataGridView1.Rows[i].Cells[columnIndex].Value != null)
            {
               string formattedString = String.Empty;
               // ReSharper disable PossibleNullReferenceException
               if ((dataGridView1.Columns["TPF"].Index == columnIndex ||
                    dataGridView1.Columns["ETA"].Index == columnIndex ||
                    dataGridView1.Columns["DownloadTime"].Index == columnIndex ||
                    dataGridView1.Columns["Deadline"].Index == columnIndex) &&
                    _Prefs.GetPreference<TimeStyleType>(Preference.TimeStyle).Equals(TimeStyleType.Formatted))
               {
                  if (dataGridView1.Columns["TPF"].Index == columnIndex)
                  {
                     formattedString =
                        GetFormattedTpfString((TimeSpan)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
                  else if (dataGridView1.Columns["ETA"].Index == columnIndex)
                  {
                     formattedString =
                        GetFormattedEtaString((TimeSpan)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
                  else if (dataGridView1.Columns["DownloadTime"].Index == columnIndex)
                  {
                     formattedString =
                        GetFormattedDownloadTimeString((DateTime)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
                  else if (dataGridView1.Columns["Deadline"].Index == columnIndex)
                  {
                     formattedString =
                        GetFormattedDeadlineString((DateTime)dataGridView1.Rows[i].Cells[columnIndex].Value);
                  }
               }
               else if (dataGridView1.Columns["DownloadTime"].Index == columnIndex ||
                        dataGridView1.Columns["Deadline"].Index == columnIndex)
               {
                  formattedString =
                     GetFormattedDateStringForMeasurement((DateTime)dataGridView1.Rows[i].Cells[columnIndex].Value,
                                                                    dataGridView1.Rows[i].Cells[columnIndex].FormattedValue.ToString());
               }
               else if (dataGridView1.Columns["ETA"].Index == columnIndex &&
                        _Prefs.GetPreference<bool>(Preference.EtaDate))
               {
                  IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[i].Cells["Name"].Value.ToString()];
                  formattedString = GetFormattedDateStringForMeasurement(instance.EtaDate, String.Format(CultureInfo.CurrentCulture, 
                     "{0} {1}", instance.EtaDate.ToShortDateString(), instance.EtaDate.ToShortTimeString()));
               }
               else if (dataGridView1.Columns["ClientType"].Index == columnIndex &&
                        _Prefs.GetPreference<bool>(Preference.ShowVersions))
               {
                  IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[i].Cells["Name"].Value.ToString()];
                  formattedString = GetVersionString(dataGridView1.Rows[i].Cells[columnIndex].Value.ToString(), instance.ClientVersion);
               }
               else if (dataGridView1.Columns["Core"].Index == columnIndex &&
                        _Prefs.GetPreference<bool>(Preference.ShowVersions))
               {
                  IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[i].Cells["Name"].Value.ToString()];
                  formattedString = GetVersionString(dataGridView1.Rows[i].Cells[columnIndex].Value.ToString(), instance.CurrentUnitInfo.CoreVersion);
               }
               else if (dataGridView1.Columns["Complete"].Index == columnIndex &&
                        _Prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay).Equals(CompletedCountDisplayType.ClientTotal))
               {
                  IClientInstance instance = _clientInstances.Instances[dataGridView1.Rows[i].Cells["Name"].Value.ToString()];
                  formattedString = instance.TotalClientCompletedUnits.ToString(CultureInfo.CurrentCulture);
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
                  width = (int)(s.Width + 3);
               }
            }
         }

         dataGridView1.Columns[columnIndex].Width = width;
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
         return date.Equals(DateTime.MinValue) ? "Unknown" : formattedValue;
      }

      private static string GetVersionString(string clientType, string version)
      {
         return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", clientType, version);
      }
      
      #endregion
   }
}
