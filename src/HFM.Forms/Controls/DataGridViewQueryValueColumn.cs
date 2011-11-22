/*
 * HFM.NET - DataGridView Query Value Column Class
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
   [CoverageExclude]
   internal class DataGridViewQueryValueColumn : DataGridViewColumn
   {
      public DataGridViewQueryValueColumn()
         : base(new DataGridViewQueryValueTextBoxCell())
      {

      }

      public override DataGridViewCell CellTemplate
      {
         get { return base.CellTemplate; }
         set
         {
            // Ensure that the cell used for the template is a DataGridViewQueryValueTextBoxCell.
            if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewQueryValueTextBoxCell)))
            {
               throw new InvalidCastException("Must be a ValueCell");
            }
            base.CellTemplate = value;
         }
      }
   }
}
