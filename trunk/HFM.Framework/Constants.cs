/*
 * HFM.NET - Framework Constants Class
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

namespace HFM.Framework
{
   public static class Constants
   {
      #region Public Const
      
      public const string ExeName = "HFM";
      public const string ApplicationName = "HFM.NET";

      public const string HfmLogFileName = "HFM.log";
      public const string HfmPrevLogFileName = "HFM-prev.log";

      public const string SqLiteFilename = "WuHistory.db3";
      public const string ProjectInfoFileName = "ProjectInfo.tab";
      public const string UnitInfoCacheFileName = "UnitInfoCache.dat";
      public const string BenchmarkCacheFileName = "BenchmarkCache.dat";
      public const string CompletedUnitsCsvFileName = "CompletedUnits.csv";

      public const string CssFolderName = "CSS";
      public const string XmlFolderName = "XML";
      public const string XsltFolderName = "XSL";

      public const string EOCUserXmlUrl = "http://folding.extremeoverclocking.com/xml/user_summary.php?u=";
      public const string EOCUserBaseUrl = "http://folding.extremeoverclocking.com/user_summary.php?s=&u=";
      public const string EOCTeamBaseUrl = "http://folding.extremeoverclocking.com/team_summary.php?s=&t=";
      public const string StanfordBaseUrl = "http://fah-web.stanford.edu/cgi-bin/main.py?qtype=userpage&username=";
      public const string GoogleGroupUrl = "http://groups.google.com/group/hfm-net";

      public const string EocStatsFormat = "{0:###,###,##0}";

      /// <summary>
      /// Conversion factor - minutes to milli-seconds
      /// </summary>
      public const int MinToMillisec = 60000;

      /// <summary>
      /// UnitInfo Log File Maximum Download Size
      /// </summary>
      public const int UnitInfoMax = 1048576; // 1 Megabyte

      public const int MinDecimalPlaces = 0;
      public const int MaxDecimalPlaces = 5;

      public const int MinMinutes = 1;
      public const int MaxMinutes = 180;

      public const int MinOffsetMinutes = -720;
      public const int MaxOffsetMinutes = 720;

      public const int MinutesDefault = 15;
      public const int ProxyPortDefault = 8080;

      public const string UnassignedDescription = "Unassigned Description";

      // Default ID Constants
      public const string DefaultUserID = "";
      public const int DefaultMachineID = 0;

      // Folding ID and Team Defaults
      public const string FoldingIDDefault = "Unknown";
      public const int TeamDefault = 0;
      public const string CoreIDDefault = "Unknown";

      // Log Filename Constants
      public const string LocalFahLog = "FAHlog.txt";
      public const string LocalUnitInfo = "unitinfo.txt";
      public const string LocalQueue = "queue.dat";
      public const string LocalExternal = "ClientData.dat";

      #endregion
   }
}
