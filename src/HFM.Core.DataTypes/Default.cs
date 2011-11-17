/*
 * HFM.NET - Default Value Class
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

namespace HFM.Core.DataTypes
{
   public static class Default
   {
      // Folding ID and Team Defaults
      public const string FoldingID = "Unknown";
      public const int Team = 0;
      public const string CoreID = "Unknown";

      // Log Filename Constants
      public const string FahLogFileName = "FAHlog.txt";
      public const string UnitInfoFileName = "unitinfo.txt";
      public const string QueueFileName = "queue.dat";
      //public const string ExternalDataFileName = "ClientData.dat";

      public const int MaxDecimalPlaces = 5;

      private static readonly bool IsRunningOnMono = Type.GetType("Mono.Runtime") != null;

      /// <summary>
      /// Get the DateTimeStyle for the given Client Instance.
      /// </summary>
      public static System.Globalization.DateTimeStyles DateTimeStyle
      {
         get
         {
            System.Globalization.DateTimeStyles style;

            if (IsRunningOnMono)
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
      }

      /// <summary>
      /// String Comparison for Paths (case sensetive on Mono / case insensetive on .NET)
      /// </summary>
      public static StringComparison PathComparison
      {
         get { return IsRunningOnMono ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase; }
      }
   }
}
