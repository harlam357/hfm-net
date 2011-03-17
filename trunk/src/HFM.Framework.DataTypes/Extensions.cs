/*
 * HFM.NET - Data Type Extension Methods
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
using System.Globalization;

namespace HFM.Framework.DataTypes
{
   public static class Extensions
   {
      #region DateTime/TimeSpan

      public static bool IsKnown(this DateTime dateTime)
      {
         return !IsUnknown(dateTime);
      }
   
      public static bool IsUnknown(this DateTime dateTime)
      {
         return dateTime.Equals(DateTime.MinValue);
      }
      
      public static bool IsZero(this TimeSpan timeSpan)
      {
         return timeSpan.Equals(TimeSpan.Zero);
      }

      #endregion

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

      #region IProjectInfo

      /// <summary>
      /// Is Project Unknown?
      /// </summary>
      /// <returns>true if Project (R/C/G) has not been identified.</returns>
      public static bool ProjectIsUnknown(this IProjectInfo projectInfo)
      {
         if (projectInfo == null) return true;

         return projectInfo.ProjectID == 0 &&
                projectInfo.ProjectRun == 0 &&
                projectInfo.ProjectClone == 0 &&
                projectInfo.ProjectGen == 0;
      }

      /// <summary>
      /// Formatted Project (R/C/G) information.
      /// </summary>
      public static string ProjectRunCloneGen(this IProjectInfo projectInfo)
      {
         if (projectInfo == null) return String.Empty;

         return String.Format(CultureInfo.InvariantCulture, "P{0} (R{1}, C{2}, G{3})", 
            projectInfo.ProjectID, projectInfo.ProjectRun, projectInfo.ProjectClone, projectInfo.ProjectGen);
      }

      /// <summary>
      /// Equals Project (R/C/G)
      /// </summary>
      /// <returns>true if Project (R/C/G) are equal.</returns>
      public static bool EqualsProject(this IProjectInfo projectInfo1, IProjectInfo projectInfo2)
      {
         if (projectInfo1 == null || projectInfo2 == null) return false;

         return (projectInfo1.ProjectID == projectInfo2.ProjectID &&
                 projectInfo1.ProjectRun == projectInfo2.ProjectRun &&
                 projectInfo1.ProjectClone == projectInfo2.ProjectClone &&
                 projectInfo1.ProjectGen == projectInfo2.ProjectGen);
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
