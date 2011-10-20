/*
 * HFM.NET - DataGridView Extended Class
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
using System.Windows.Forms;

using HFM.Framework;

namespace HFM.Forms.Controls
{
   public interface IDataGridView
   {
      #region System.Windows.Forms.DataGridView Properties

      bool AutoGenerateColumns { get; set; }

      DataGridViewCell CurrentCell { get; }

      object DataSource { get; set; }

      DataGridViewRowCollection Rows { get; }

      DataGridViewColumnCollection Columns { get; }

      DataGridViewSelectedRowCollection SelectedRows { get; }

      DataGridViewColumn SortedColumn { get; }

      SortOrder SortOrder { get; }

      #endregion

      #region System.Windows.Forms.DataGridView Events

      event DataGridViewColumnEventHandler ColumnDisplayIndexChanged;

      #endregion

      #region System.Windows.Forms.DataGridView Methods

      DataGridView.HitTestInfo HitTest(int x, int y);

      void Invalidate();

      void Sort(DataGridViewColumn dataGridViewColumn, System.ComponentModel.ListSortDirection direction);

      #endregion

      #region System.Windows.Forms.Control Methods

      System.Drawing.Point PointToScreen(System.Drawing.Point p);

      #endregion

      /// <summary>
      /// Local Flag That Halts the SelectionChanged Event
      /// </summary>
      bool FreezeSelectionChanged { get; set; }

      /// <summary>
      /// Local Flag That Halts the Sorted Event
      /// </summary>
      bool FreezeSorted { get; set; }

      /// <summary>
      /// Key for the Currently Selected Row
      /// </summary>
      string CurrentRowKey { get; }
   }

   [CoverageExclude]
   public partial class DataGridViewExt : DataGridView, IDataGridView
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
      public DataGridViewExt()
      {
         InitializeComponent();
      }

      protected override void OnSelectionChanged(EventArgs e)
      {
         if (FreezeSelectionChanged) return;

         CurrentRowKey = SelectedRows.Count != 0 ? SelectedRows[0].Cells["Name"].Value.ToString() : String.Empty;
      
         base.OnSelectionChanged(e);
      }

      protected override void OnSorted(EventArgs e)
      {
         if (FreezeSorted) return;
      
         base.OnSorted(e);
      }
   }
}
