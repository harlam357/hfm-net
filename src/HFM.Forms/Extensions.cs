/*
 * HFM.NET - Forms Extensions Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms
{
   internal static class Extensions
   {
      #region Color

      public static Color FindNearestKnown(this Color c)
      {
         var best = new ColorName { Name = null };

         foreach (string colorName in Enum.GetNames(typeof(KnownColor)))
         {
            Color known = Color.FromName(colorName);
            int dist = Math.Abs(c.R - known.R) + Math.Abs(c.G - known.G) + Math.Abs(c.B - known.B);

            if (best.Name == null || dist < best.Distance)
            {
               best.Color = known;
               best.Name = colorName;
               best.Distance = dist;
            }
         }

         return best.Color;
      }

      struct ColorName
      {
         public Color Color;
         public string Name;
         public int Distance;
      }

      #endregion

      #region SlotStatus

      /// <summary>
      /// Gets Status Color Pen Object
      /// </summary>
      public static Pen GetDrawingPen(this SlotStatus status)
      {
         return new Pen(status.GetStatusColor());
      }

      /// <summary>
      /// Gets Status Color Brush Object
      /// </summary>
      public static SolidBrush GetDrawingBrush(this SlotStatus status)
      {
         return new SolidBrush(status.GetStatusColor());
      }

      #endregion

      #region ListSortDirection

      public static string ToDirectionString(this ListSortDirection direction)
      {
         return direction.Equals(ListSortDirection.Descending) ? "DESC" : "ASC";
      }

      #endregion

      #region Controls

      public static void BindText(this Control control, object dataSource, string dataMember)
      {
         if (Core.Application.IsRunningOnMono)
         {
            control.DataBindings.Add("Text", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
         }
         else
         {
            control.DataBindings.Add("Text", dataSource, dataMember, false, DataSourceUpdateMode.OnValidation);
         }
      }

      //public static void BindText(this Control control, object dataSource, string dataMember, DataSourceUpdateMode updateMode)
      //{
      //   control.DataBindings.Add("Text", dataSource, dataMember, false, updateMode);
      //}

      public static void BindEnabled(this Control control, object dataSource, string dataMember)
      {
         control.DataBindings.Add("Enabled", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
      }

      //public static void BindEnabled(this Control control, object dataSource, string dataMember, DataSourceUpdateMode updateMode)
      //{
      //   control.DataBindings.Add("Enabled", dataSource, dataMember, false, updateMode);
      //}

      public static void BindChecked(this CheckBox control, object dataSource, string dataMember)
      {
         control.DataBindings.Add("Checked", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
      }

      public static void BindChecked(this RadioButton control, object dataSource, string dataMember)
      {
         control.DataBindings.Add("Checked", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
      }

      #endregion

      #region Graphics

      public static double GetDpiScale(this Graphics g)
      {
         return g.DpiX / 96;
      }

      #endregion
   }
}
