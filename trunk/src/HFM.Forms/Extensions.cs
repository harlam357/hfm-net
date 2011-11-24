/*
 * HFM.NET - Forms Extensions Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Drawing;

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

      #region ClientStatus

      /// <summary>
      /// Gets Status Color Pen Object
      /// </summary>
      public static Pen GetDrawingPen(this ClientStatus status)
      {
         return new Pen(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Color Brush Object
      /// </summary>
      public static SolidBrush GetDrawingBrush(this ClientStatus status)
      {
         return new SolidBrush(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Color String
      /// </summary>
      public static string GetHtmlColor(this ClientStatus status)
      {
         return ColorTranslator.ToHtml(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Font Color String
      /// </summary>
      public static string GetHtmlFontColor(this ClientStatus status)
      {
         switch (status)
         {
            case ClientStatus.Running:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.RunningAsync:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.RunningNoFrameTimes:
               return ColorTranslator.ToHtml(Color.Black);
            case ClientStatus.Stopped:
            case ClientStatus.EuePause:
            case ClientStatus.Hung:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.Paused:
               return ColorTranslator.ToHtml(Color.Black);
            case ClientStatus.SendingWorkPacket:
            case ClientStatus.GettingWorkPacket:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.Offline:
               return ColorTranslator.ToHtml(Color.Black);
            default:
               return ColorTranslator.ToHtml(Color.Black);
         }
      }

      /// <summary>
      /// Gets Status Color Object
      /// </summary>
      private static Color GetStatusColor(ClientStatus status)
      {
         switch (status)
         {
            case ClientStatus.Running:
               return Color.Green;
            case ClientStatus.RunningAsync:
               return Color.Blue;
            case ClientStatus.RunningNoFrameTimes:
               return Color.Yellow;
            case ClientStatus.Stopped:
            case ClientStatus.EuePause:
            case ClientStatus.Hung:
               return Color.DarkRed;
            case ClientStatus.Paused:
               return Color.Orange;
            case ClientStatus.SendingWorkPacket:
            case ClientStatus.GettingWorkPacket:
               return Color.Purple;
            case ClientStatus.Offline:
               return Color.Gray;
            default:
               return Color.Gray;
         }
      }

      #endregion
   }
}
