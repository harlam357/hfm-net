/*
 * HFM.NET - DataGridViewColumnSelector ver 1.0 (2008)
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
 * 
 * Author - Vincenzo Rossi - Naples, Italy - redmaster@tiscali.it
 * http://www.codeproject.com/KB/grid/DGVColumnSelector.aspx
 * 
 * Modified for HFM by harlam357. 
 * Licenced under the The Code Project Open License (CPOL).
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
using System.Windows.Forms;
using System.Drawing;

namespace HFM.Classes
{
    /// <summary>
    /// Add column show/hide capability to a DataGridView. When user right-clicks 
    /// the cell origin a popup, containing a list of checkbox and column names, is
    /// shown. 
    /// </summary>
    class DataGridViewColumnSelector : IDisposable
    {
        // the DataGridView to which the DataGridViewColumnSelector is attached
        private DataGridView mDataGridView = null;
        // a CheckedListBox containing the column header text and checkboxes
        private readonly CheckedListBox mCheckedListBox;
        // a ToolStripDropDown object used to show the popup
        private readonly ToolStripDropDown mPopup;
        
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
            get { return mDataGridView; }
            set { 
                  // If any, remove handler from current DataGridView 
                  if (mDataGridView != null) mDataGridView.MouseUp -= mDataGridView_MouseUp;
                  // Set the new DataGridView
                  mDataGridView = value;
                  // Attach CellMouseClick handler to DataGridView
                  if (mDataGridView != null) mDataGridView.MouseUp += mDataGridView_MouseUp;
              }
        }

        // When user right-clicks the cell origin, it clears and fill the CheckedListBox with
        // columns header text. Then it shows the popup. 
        // In this way the CheckedListBox items are always refreshed to reflect changes occurred in 
        // DataGridView columns (column additions or name changes and so on).
        void mDataGridView_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hti = mDataGridView.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Right && hti.Type == DataGridViewHitTestType.ColumnHeader)
            {
                int textWidth = 0;
                mCheckedListBox.Items.Clear();
                foreach (DataGridViewColumn c in mDataGridView.Columns)
                {
                    if (c.HeaderText.Length == 0) // FxCop: CA1820
                    {
                       mCheckedListBox.Items.Add("(Dummy Column)", c.Visible);
                    }
                    else
                    {
                       mCheckedListBox.Items.Add(c.HeaderText, c.Visible);
                       
                    }
                    Size itemWidth = TextRenderer.MeasureText(c.HeaderText, mCheckedListBox.Font);
                    textWidth = (itemWidth.Width > textWidth) ? itemWidth.Width : textWidth;
                }
                int PreferredHeight = (mCheckedListBox.Items.Count * 16) + 7;
                mCheckedListBox.Height = (PreferredHeight < MaxHeight) ? PreferredHeight : MaxHeight;
                mCheckedListBox.Width = textWidth + 20; //Width;
                
                mPopup.Show(mDataGridView.PointToScreen(new Point (e.X, e.Y)));
            }
        }

        // The constructor creates an instance of CheckedListBox and ToolStripDropDown.
        // the CheckedListBox is hosted by ToolStripControlHost, which in turn is
        // added to ToolStripDropDown.
        public DataGridViewColumnSelector() 
        {
            mCheckedListBox = new CheckedListBox();
            mCheckedListBox.CheckOnClick = true;
            mCheckedListBox.ItemCheck += mCheckedListBox_ItemCheck;

            ToolStripControlHost mControlHost = new ToolStripControlHost(mCheckedListBox);
            mControlHost.Padding = Padding.Empty;
            mControlHost.Margin = Padding.Empty;
            mControlHost.AutoSize = false;

            mPopup = new ToolStripDropDown();
            mPopup.Padding = Padding.Empty;
            mPopup.Items.Add(mControlHost);
        }

        public DataGridViewColumnSelector(DataGridView dgv) : this() 
        {
            DataGridView = dgv;
        }

        // When user checks / unchecks a checkbox, the related column visibility is 
        // switched.
        void mCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            mDataGridView.Columns[e.Index].Visible = (e.NewValue == CheckState.Checked);
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
                 mCheckedListBox.Dispose();
                 mPopup.Dispose();
              }

              _disposed = true;
           }
        }

        private bool _disposed;
        #endregion
    }
}
