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

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace HFM.Classes
{
   public partial class DataGridViewWrapper : DataGridView
   {
      private bool _FreezeRowEnter;
      
      public bool FreezeRowEnter
      {
         get { return _FreezeRowEnter; }
         set { _FreezeRowEnter = value; }
      }

      public DataGridViewWrapper()
      {
         InitializeComponent();
      }

      [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
      public void FireRowEnter(int columnIndex, int rowIndex)
      {
         OnRowEnter(new DataGridViewCellEventArgs(columnIndex, rowIndex));
      }

      ///<summary>
      ///Raises the <see cref="E:System.Windows.Forms.DataGridView.RowEnter"></see> event.
      ///</summary>
      ///<param name="e">A <see cref="T:System.Windows.Forms.DataGridViewCellEventArgs"></see> that contains the event data.</param>
      protected override void OnRowEnter(DataGridViewCellEventArgs e)
      {
         if (FreezeRowEnter) return;
      
         base.OnRowEnter(e);
      }
   }
}
