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
using System.Globalization;

namespace HFM.Framework.DataTypes
{
   public static class Extensions
   {
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

      /// <summary>
      /// Returns true if Project (R/C/G) has not been identified
      /// </summary>
      public static bool ProjectIsUnknown(this IProjectInfo projectInfo)
      {
         return projectInfo.ProjectID == 0 &&
                projectInfo.ProjectRun == 0 &&
                projectInfo.ProjectClone == 0 &&
                projectInfo.ProjectGen == 0;
      }

      /// <summary>
      /// Formatted Project (Run, Clone, Gen) Information
      /// </summary>
      public static string ProjectRunCloneGen(this IProjectInfo projectInfo)
      {
         return String.Format(CultureInfo.InvariantCulture, "P{0} (R{1}, C{2}, G{3})", 
            projectInfo.ProjectID, projectInfo.ProjectRun, projectInfo.ProjectClone, projectInfo.ProjectGen);
      }
   }
}
