/*
 * HFM.NET - DataGridView Wrapper Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

namespace HFM.Classes
{
   public partial class DataGridViewWrapper : DataGridView
   {
      private bool _FreezeSelectionChanged;
      /// <summary>
      /// Local Flag That Halts the SelectionChanged Event
      /// </summary>
      public bool FreezeSelectionChanged
      {
         get { return _FreezeSelectionChanged; }
         set { _FreezeSelectionChanged = value; }
      }

      private bool _FreezeSorted;
      /// <summary>
      /// Local Flag That Halts the Sorted Event
      /// </summary>
      public bool FreezeSorted
      {
         get { return _FreezeSorted; }
         set { _FreezeSorted = value; }
      }
      
      private string _CurrentRowKey;
      /// <summary>
      /// Key for the Currently Selected Row
      /// </summary>
      public string CurrentRowKey
      {
         get { return _CurrentRowKey; }
      }

      /// <summary>
      /// Constructor
      /// </summary>
      public DataGridViewWrapper()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Raises the System.Windows.Forms.DataGridView.SelectionChanged event.
      /// </summary>
      protected override void OnSelectionChanged(EventArgs e)
      {
         if (FreezeSelectionChanged) return;
      
         base.OnSelectionChanged(e);
      }

      protected override void OnSorted(EventArgs e)
      {
         if (FreezeSorted) return;
      
         base.OnSorted(e);
      }

      protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
      {
         if (e.RowIndex == -1)
         {
            _CurrentRowKey = SelectedRows[0].Cells["Name"].Value.ToString();
         }
      
         base.OnCellMouseDown(e);
      }
   }
}
