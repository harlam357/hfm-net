/*
 * HFM.NET - Core Constants Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

namespace HFM.Core
{
   public static class Constants
   {
      public const string EOCUserBaseUrl = "http://folding.extremeoverclocking.com/user_summary.php?s=&u=";
      public const string EOCTeamBaseUrl = "http://folding.extremeoverclocking.com/team_summary.php?s=&t=";
      public const string StanfordBaseUrl = "http://fah-web.stanford.edu/cgi-bin/main.py?qtype=userpage&username=";
      public const string GitHubUrl = "https://github.com/harlam357/hfm-net";
      public const string GoogleGroupUrl = "http://groups.google.com/group/hfm-net/";

      public const string EocStatsFormat = "{0:###,###,##0}";

      // Folding ID and Team Defaults
      public const string DefaultFoldingID = "Unknown";
      public const int DefaultTeam = 0;
      public const string DefaultCoreID = "Unknown";

      // Log Filename Constants
      public const string FahClientLogFileName = "log.txt";

      /// <summary>
      /// Conversion factor - minutes to milli-seconds
      /// </summary>
      public const int MinToMillisec = 60000;

      public const int MaxDecimalPlaces = 5;

      // Default ID Constants
      public const int DefaultMachineID = 0;

      public const int MaxDisplayableLogLines = 500;

      public const string ClientNameFormat = "({0}) {1}";

      public const string FahClientSlotOptions = "slot-options {0} cpus client-type client-subtype cpu-usage machine-id max-packet-size core-priority next-unit-percentage max-units checkpoint pause-on-start gpu-index gpu-usage";
   }
}
