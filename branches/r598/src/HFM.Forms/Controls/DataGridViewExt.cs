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

using HFM.Core;

namespace HFM.Forms.Controls
{
   public interface IDataGridView
   {
      #region System.Windows.Forms.DataGridView Properties

      object DataSource { get; set; }

      DataGridViewRowCollection Rows { get; }

      DataGridViewColumnCollection Columns { get; }

      #endregion

      #region System.Windows.Forms.DataGridView Events

      event DataGridViewColumnEventHandler ColumnDisplayIndexChanged;

      #endregion

      #region System.Windows.Forms.DataGridView Methods

      DataGridView.HitTestInfo HitTest(int x, int y);

      void Invalidate();

      #endregion

      #region System.Windows.Forms.Control Methods

      System.Drawing.Point PointToScreen(System.Drawing.Point p);

      #endregion
   }

   [CoverageExclude]
   public partial class DataGridViewExt : DataGridView, IDataGridView
   {
      /// <summary>
      /// Constructor
      /// </summary>
      public DataGridViewExt()
      {
         InitializeComponent();
      }
   }
}
