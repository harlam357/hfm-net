/*
 * HFM.NET - DataGridView Wrapper Class
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
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
   public partial class DataGridViewWrapper : DataGridView
   {
      /// <summary>
      /// Local Flag That Halts the SelectionChanged Event
      /// </summary>
      public bool FreezeSelectionChanged { get; set; }

      /// <summary>
      /// Local Flag That Halts the Sorted Event
      /// </summary>
      public bool FreezeSorted { get; set; }

      /// <summary>
      /// Key for the Currently Selected Row
      /// </summary>
      public string CurrentRowKey { get; private set; }

      /// <summary>
      /// Constructor
      /// </summary>
      public DataGridViewWrapper()
      {
         InitializeComponent();
      }

      protected override void OnSelectionChanged(EventArgs e)
      {
         if (FreezeSelectionChanged) return;

         // Code moved here from OnCellMouseDown
         if (SelectedRows.Count != 0)
         {
            CurrentRowKey = SelectedRows[0].Cells["Name"].Value.ToString();
         }
         else
         {
            CurrentRowKey = String.Empty;
         }
      
         base.OnSelectionChanged(e);
      }

      protected override void OnSorted(EventArgs e)
      {
         if (FreezeSorted) return;
      
         base.OnSorted(e);
      }

      //protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
      //{
      //   if (e.RowIndex == -1)
      //   {
      //      if (SelectedRows.Count != 0)
      //      {
      //         CurrentRowKey = SelectedRows[0].Cells["Name"].Value.ToString();
      //      }
      //      else
      //      {
      //         CurrentRowKey = String.Empty;
      //      }
      //   }
      
      //   base.OnCellMouseDown(e);
      //}

      public const int NumberOfDisplayFields = 19;

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
         dgv.Columns.Add("ClientType", "Client Type");
         dgv.Columns["ClientType"].DataPropertyName = "ClientType";
         dgv.Columns.Add("TPF", "TPF");
         dgv.Columns["TPF"].DataPropertyName = "TPF";
         dgv.Columns.Add("PPD", "PPD");
         dgv.Columns["PPD"].DataPropertyName = "PPD";
         dgv.Columns.Add("MHz", "MHz");
         dgv.Columns["MHz"].DataPropertyName = "MHz";
         dgv.Columns.Add("PpdMHz", "PPD/MHz");
         dgv.Columns["PpdMHz"].DataPropertyName = "PpdMHz";
         dgv.Columns.Add("ETA", "ETA");
         dgv.Columns["ETA"].DataPropertyName = "ETA";
         dgv.Columns.Add("Core", "Core");
         dgv.Columns["Core"].DataPropertyName = "Core";
         dgv.Columns.Add("CoreID", "Core ID");
         dgv.Columns["CoreID"].DataPropertyName = "CoreID";
         dgv.Columns.Add("ProjectRunCloneGen", "Project (Run, Clone, Gen)");
         dgv.Columns["ProjectRunCloneGen"].DataPropertyName = "ProjectRunCloneGen";
         dgv.Columns.Add("Credit", "Credit");
         dgv.Columns["Credit"].DataPropertyName = "Credit";
         dgv.Columns.Add("Complete", "Complete");
         dgv.Columns["Complete"].DataPropertyName = "Complete";
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

   #region DataGridViewProgressColumn

   // Code from: http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/769ca9d6-1e9d-4d76-8c23-db535b2f19c2

   public class DataGridViewProgressColumn : DataGridViewImageColumn
   {
      public DataGridViewProgressColumn()
      {
         CellTemplate = new DataGridViewProgressCell();
      }
   }

   class DataGridViewProgressCell : DataGridViewImageCell
   {
      // Used to make custom cell consistent with a DataGridViewImageCell
      static readonly Image EmptyImage;
      static DataGridViewProgressCell()
      {
         EmptyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      }
      public DataGridViewProgressCell()
      {
         ValueType = typeof(int);
      }
      // Method required to make the Progress Cell consistent with the default Image Cell.
      // The default Image Cell assumes an Image as a value, although the value of the Progress Cell is an int.
      protected override object GetFormattedValue(object value,
                           int rowIndex, ref DataGridViewCellStyle cellStyle,
                           TypeConverter valueTypeConverter,
                           TypeConverter formattedValueTypeConverter,
                           DataGridViewDataErrorContexts context)
      {
         return EmptyImage;
      }

      protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
                                    object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
                                    DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
      {
         var percentage = (float)value;
         var progressVal = (int)(percentage * 100);
         Brush foreColorBrush = new SolidBrush(cellStyle.ForeColor);

         // Draws the cell grid
         base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText,
            cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

         if (percentage > 0.0)
         {
            // Draw the progress bar and the text
            Color progressBarColor = Color.FromArgb(163, 189, 242);
            g.FillRectangle(new SolidBrush(progressBarColor), cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32((percentage * cellBounds.Width - 4)), cellBounds.Height - 4);
            g.DrawString(progressVal + "%", cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
         }
         else
         {
            // draw the text
            if (DataGridView.CurrentRow != null && DataGridView.CurrentRow.Index == rowIndex)
            {
               g.DrawString(progressVal + "%", cellStyle.Font, new SolidBrush(cellStyle.SelectionForeColor), cellBounds.X + 6, cellBounds.Y + 2);
            }
            else
            {
               g.DrawString(progressVal + "%", cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
            }
         }
      }
   }

   #endregion
}
