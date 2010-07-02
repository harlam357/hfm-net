/*
 * HFM.NET - Platform Operations Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace HFM.Framework
{
   public static class PlatformOps
   {
      public static bool IsRunningOnMono()
      {
         return Type.GetType("Mono.Runtime") != null;
      }

      /// <summary>
      /// Get the DateTimeStyle for the given Client Instance.
      /// </summary>
      public static System.Globalization.DateTimeStyles GetDateTimeStyle()
      {
         System.Globalization.DateTimeStyles style;

         if (IsRunningOnMono())
         {
            style = System.Globalization.DateTimeStyles.AssumeUniversal |
                    System.Globalization.DateTimeStyles.AdjustToUniversal;
         }
         else
         {
            // set parse style to parse local
            style = System.Globalization.DateTimeStyles.NoCurrentDateDefault |
                    System.Globalization.DateTimeStyles.AssumeUniversal |
                    System.Globalization.DateTimeStyles.AdjustToUniversal;
         }

         return style;
      }
      
      /// <summary>
      /// Major.Minor.Build
      /// </summary>
      public static string ApplicationVersion
      {
         get
         {
            return GetVersionString("{0}.{1}.{2}");
         }
      }

      /// <summary>
      /// Major.Minor.Build
      /// </summary>
      public static string ApplicationNameAndVersion
      {
         get
         {
            return String.Concat("HFM.NET v", GetVersionString("{0}.{1}.{2}"));
         }
      }

      /// <summary>
      /// Major.Minor.Build.Revision
      /// </summary>
      public static string ApplicationNameAndVersionWithRevision
      {
         get
         {
            return String.Concat("HFM.NET v", GetVersionString("{0}.{1}.{2}.{3}"));
         }
      }

      /// <summary>
      /// Major.Minor.Build.Revision
      /// </summary>
      public static string ApplicationVersionWithRevision
      {
         get
         {
            return GetVersionString("{0}.{1}.{2}.{3}");
         }
      }

      /// <summary>
      /// Formatted vMajor.Minor.Build.Revision
      /// </summary>
      public static string ShortFormattedApplicationVersionWithRevision
      {
         get
         {
            return GetVersionString("v{0}.{1}.{2}.{3}");
         }
      }

      /// <summary>
      /// Formatted Version Major.Minor.Build - Revision
      /// </summary>
      public static string LongFormattedApplicationVersionWithRevision
      {
         get
         {
            return GetVersionString("Version {0}.{1}.{2} - Revision {3}");
         }
      }
      
      public static long VersionNumber
      {
         get
         {
            // Example: 0.3.1.50 == 30010045 / 1.3.4.75 == 1030040075
            FileVersionInfo fileVersionInfo =
               FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return GetVersionLongFromArray(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                           fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
         }
      }
      
      /// <summary>
      /// 
      /// </summary>
      /// <param name="version"></param>
      /// <exception cref="FormatException">Throws when numbers cannot be parsed.</exception>
      public static long GetVersionLongFromString(string version)
      {
         int[] versionNumbers = new int[4];
      
         string[] split = version.Split(new[] { '.' }, 4, StringSplitOptions.None);
         for (int i = 0; i < split.Length; i++)
         {
            versionNumbers[i] = Int32.Parse(split[i]);
         }
         
         return GetVersionLongFromArray(versionNumbers);
      }
      
      private static long GetVersionLongFromArray(params int[] versionNumbers)
      {
         return (versionNumbers[0] * 1000000000) + (versionNumbers[1] * 10000000) +
                (versionNumbers[2] * 10000) + versionNumbers[3];
      }

      private static string GetVersionString(string format)
      {
         FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
         return String.Format(format, fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                              fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
      }

      public static string AssemblyGuid
      {
         get
         {
            object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false);
            if (attributes.Length == 0)
            {
               return String.Empty;
            }
            return ((System.Runtime.InteropServices.GuidAttribute)attributes[0]).Value;
         }
      }

      public static string AssemblyTitle
      {
         get
         {
            object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
               AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
               if (String.IsNullOrEmpty(titleAttribute.Title) == false)
               {
                  return titleAttribute.Title;
               }
            }
            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
         }
      }

      #region Status Color Helper Functions
      /// <summary>
      /// Gets Status Color Pen Object
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Color (Pen)</returns>
      public static Pen GetStatusPen(ClientStatus status)
      {
         return new Pen(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Color Brush Object
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Color (Brush)</returns>
      public static SolidBrush GetStatusBrush(ClientStatus status)
      {
         return new SolidBrush(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Color String
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Html Color (String)</returns>
      public static string GetStatusHtmlColor(ClientStatus status)
      {
         return ColorTranslator.ToHtml(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Font Color String
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Html Font Color (String)</returns>
      public static string GetStatusHtmlFontColor(ClientStatus status)
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
      /// <param name="status">Client Status</param>
      /// <returns>Status Color (Color)</returns>
      public static Color GetStatusColor(ClientStatus status)
      {
         switch (status)
         {
            case ClientStatus.Running:
               return Color.Green; // Issue 45
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
