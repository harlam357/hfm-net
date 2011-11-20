/*
 * HFM.NET - Core Constants Class
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

namespace HFM.Core
{
   public static class Constants
   {
      public const string ExeName = "HFM";
      public const string ApplicationName = "HFM.NET";

      public const string HfmLogFileName = "HFM.log";
      public const string HfmPrevLogFileName = "HFM-prev.log";

      public const string SqLiteFilename = "WuHistory.db3";
      public const string ProjectInfoFileName = "ProjectInfo.tab";
      public const string UnitInfoCacheFileName = "UnitInfoCache.dat";
      public const string BenchmarkCacheFileName = "BenchmarkCache.dat";
      //public const string CompletedUnitsCsvFileName = "CompletedUnits.csv";
      public const string UserStatsCacheFileName = "UserStatsCache.dat";
      public const string QueryCacheFileName = "WuHistoryQuery.dat";

      // Plugins Folder Constants
      public const string PluginsFolderName = "Plugins";
      public const string PluginsProteinsFolderName = "Proteins";
      public const string PluginsBenchmarksFolderName = "Benchmarks";

      public const string CssFolderName = "CSS";
      public const string XmlFolderName = "XML";
      public const string XsltFolderName = "XSL";

      public const string EOCUserXmlUrl = "http://folding.extremeoverclocking.com/xml/user_summary.php?u=";
      public const string EOCUserBaseUrl = "http://folding.extremeoverclocking.com/user_summary.php?s=&u=";
      public const string EOCTeamBaseUrl = "http://folding.extremeoverclocking.com/team_summary.php?s=&t=";
      public const string StanfordBaseUrl = "http://fah-web.stanford.edu/cgi-bin/main.py?qtype=userpage&username=";
      public const string GoogleCodeUrl = "http://code.google.com/p/hfm-net/";
      public const string GoogleGroupUrl = "http://groups.google.com/group/hfm-net/";

      public const string EocStatsFormat = "{0:###,###,##0}";

      /// <summary>
      /// Conversion factor - minutes to milli-seconds
      /// </summary>
      public const int MinToMillisec = 60000;

      /// <summary>
      /// UnitInfo Log File Maximum Download Size
      /// </summary>
      public const int UnitInfoMax = 1048576; // 1 Megabyte

      public const int MinMinutes = 1;
      public const int MaxMinutes = 180;

      public const int MinOffsetMinutes = -720;
      public const int MaxOffsetMinutes = 720;

      public const int MinutesDefault = 15;
      public const int ProxyPortDefault = 8080;

      // Default ID Constants
      public const string DefaultUserID = "";
      public const int DefaultMachineID = 0;

      public const int MaxDisplayableLogLines = 500;

      public const string InstanceNameFormat = "({0}) {1}";
   }
}
