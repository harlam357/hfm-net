/*
 * HFM.NET - DataGridViewColumnSelector ver 1.0 (2008)
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
 * Author - Vincenzo Rossi - Naples, Italy - redmaster@tiscali.it
 * http://www.codeproject.com/KB/grid/DGVColumnSelector.aspx
 * 
 * Modified for HFM by harlam357. 
 * Licenced under the The Code Project Open License (CPOL).
 */

using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

using HFM.Core;

namespace HFM.Forms.Controls
{
   /// <summary>
   /// Add column show/hide capability to a DataGridView. When user right-clicks 
   /// the cell origin a popup, containing a list of checkbox and column names, is
   /// shown. 
   /// </summary>
   [CoverageExclude]
   internal class DataGridViewColumnSelector : IDisposable
   {
      // the DataGridView to which the DataGridViewColumnSelector is attached
      private DataGridView _dataGridView;
      // a CheckedListBox containing the column header text and checkboxes
      private readonly CheckedListBox _checkedListBox;
      // a ToolStripDropDown object used to show the popup
      private readonly ToolStripDropDown _popUp;

      /// <summary>
      /// The max height of the popup
      /// </summary>
      public int MaxHeight = 300;

      ///// <summary>
      ///// The width of the popup
      ///// </summary>
      //public int Width = 200;

      /// <summary>
      /// Gets or sets the DataGridView to which the DataGridViewColumnSelector is attached
      /// </summary>
      public DataGridView DataGridView
      {
         get { return _dataGridView; }
         set
         {
            // If any, remove handler from current DataGridView 
            if (_dataGridView != null) _dataGridView.MouseUp -= DataGridViewMouseUp;
            // Set the new DataGridView
            _dataGridView = value;
            // Attach CellMouseClick handler to DataGridView
            if (_dataGridView != null) _dataGridView.MouseUp += DataGridViewMouseUp;
         }
      }

      // When user right-clicks the cell origin, it clears and fill the CheckedListBox with
      // columns header text. Then it shows the popup. 
      // In this way the CheckedListBox items are always refreshed to reflect changes occurred in 
      // DataGridView columns (column additions or name changes and so on).
      void DataGridViewMouseUp(object sender, MouseEventArgs e)
      {
         DataGridView.HitTestInfo hti = _dataGridView.HitTest(e.X, e.Y);
         if (e.Button == MouseButtons.Right && hti.Type == DataGridViewHitTestType.ColumnHeader)
         {
            int textWidth = 0;
            _checkedListBox.Items.Clear();
            foreach (DataGridViewColumn c in _dataGridView.Columns)
            {
               if (c.HeaderText.Length == 0) // FxCop: CA1820
               {
                  _checkedListBox.Items.Add("(Dummy Column)", c.Visible);
               }
               else
               {
                  _checkedListBox.Items.Add(c.HeaderText, c.Visible);

               }
               Size itemWidth = TextRenderer.MeasureText(c.HeaderText, _checkedListBox.Font);
               textWidth = (itemWidth.Width > textWidth) ? itemWidth.Width : textWidth;
            }
            int preferredHeight = (_checkedListBox.Items.Count * 16) + 7;
            _checkedListBox.Height = (preferredHeight < MaxHeight) ? preferredHeight : MaxHeight;
            _checkedListBox.Width = textWidth + 20; //Width;

            _popUp.Show(_dataGridView.PointToScreen(new Point(e.X, e.Y)));
         }
      }

      // The constructor creates an instance of CheckedListBox and ToolStripDropDown.
      // the CheckedListBox is hosted by ToolStripControlHost, which in turn is
      // added to ToolStripDropDown.
      public DataGridViewColumnSelector()
      {
         _checkedListBox = new CheckedListBox();
         _checkedListBox.CheckOnClick = true;
         _checkedListBox.ItemCheck += CheckedListBoxItemCheck;

         var controlHost = new ToolStripControlHost(_checkedListBox);
         controlHost.Padding = Padding.Empty;
         controlHost.Margin = Padding.Empty;
         controlHost.AutoSize = false;

         _popUp = new ToolStripDropDown();
         _popUp.Padding = Padding.Empty;
         _popUp.Items.Add(controlHost);
      }

      public DataGridViewColumnSelector(DataGridView dgv)
         : this()
      {
         DataGridView = dgv;
      }

      // When user checks / unchecks a checkbox, the related column visibility is 
      // switched.
      void CheckedListBoxItemCheck(object sender, ItemCheckEventArgs e)
      {
         if (NumberOfColumnsVisible() == 1)
         {
            e.NewValue = CheckState.Checked;
         }
         _dataGridView.Columns[e.Index].Visible = (e.NewValue == CheckState.Checked);
      }
      
      private int NumberOfColumnsVisible()
      {
         return _dataGridView.Columns.Cast<DataGridViewColumn>().Count(column => column.Visible);
      }

      #region IDisposable Members
      
      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or
      /// resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Disposes the object.
      /// </summary>
      private void Dispose(bool disposing)
      {
         if (false == _disposed)
         {
            // clean native resources        

            if (disposing)
            {
               // clean managed resources
               _checkedListBox.Dispose();
               _popUp.Dispose();
            }

            _disposed = true;
         }
      }

      private bool _disposed;
      
      #endregion
   }
}
