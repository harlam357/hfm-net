/*
 * HFM.NET - DataGridView Progress Column Class
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using HFM.Core;

namespace HFM.Forms.Controls
{
   // Code from: http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/769ca9d6-1e9d-4d76-8c23-db535b2f19c2

   [CoverageExclude]
   internal class DataGridViewProgressColumn : DataGridViewColumn
   {
      public DataGridViewProgressColumn()
      {
         CellTemplate = new DataGridViewProgressCell();
         SortMode = DataGridViewColumnSortMode.Automatic;
      }
   }

   [CoverageExclude]
   internal class DataGridViewProgressCell : DataGridViewTextBoxCell
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
         // this is new... sometimes being passed a null
         // value here.  check and get out if so. - 1/28/12
         if (value == null) return;
         
         var percentage = (float)value;
         var progressVal = Convert.ToInt32(percentage * 100);
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
}
