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

      #region SlotStatus

      /// <summary>
      /// Gets Status Color Pen Object
      /// </summary>
      public static Pen GetDrawingPen(this SlotStatus status)
      {
         return new Pen(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Color Brush Object
      /// </summary>
      public static SolidBrush GetDrawingBrush(this SlotStatus status)
      {
         return new SolidBrush(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Color String
      /// </summary>
      public static string GetHtmlColor(this SlotStatus status)
      {
         return ColorTranslator.ToHtml(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Font Color String
      /// </summary>
      public static string GetHtmlFontColor(this SlotStatus status)
      {
         switch (status)
         {
            case SlotStatus.Running:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.RunningAsync:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.RunningNoFrameTimes:
               return ColorTranslator.ToHtml(Color.Black);
            case SlotStatus.Stopped:
            case SlotStatus.EuePause:
            case SlotStatus.Hung:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.Paused:
               return ColorTranslator.ToHtml(Color.Black);
            case SlotStatus.SendingWorkPacket:
            case SlotStatus.GettingWorkPacket:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.Offline:
               return ColorTranslator.ToHtml(Color.Black);
            default:
               return ColorTranslator.ToHtml(Color.Black);
         }
      }

      /// <summary>
      /// Gets Status Color Object
      /// </summary>
      private static Color GetStatusColor(SlotStatus status)
      {
         switch (status)
         {
            case SlotStatus.Running:
               return Color.Green;
            case SlotStatus.RunningAsync:
               return Color.Blue;
            case SlotStatus.RunningNoFrameTimes:
               return Color.Yellow;
            case SlotStatus.Stopped:
            case SlotStatus.EuePause:
            case SlotStatus.Hung:
               return Color.DarkRed;
            case SlotStatus.Paused:
               return Color.Orange;
            case SlotStatus.SendingWorkPacket:
            case SlotStatus.GettingWorkPacket:
               return Color.Purple;
            case SlotStatus.Offline:
               return Color.Gray;
            default:
               return Color.Gray;
         }
      }

      #endregion
   }
}
