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

using System;

namespace HFM.Framework
{
   public static class Constants
   {
      #region Public Const
      public const string ExeName = "HFM";

      public const string HfmLogFileName = "HFM.log";
      public const string HfmPrevLogFileName = "HFM-prev.log";

      public const string ProjectInfoFileName = "ProjectInfo.tab";

      public const string CssFolderName = "CSS";
      public const string XmlFolderName = "XML";
      public const string XsltFolderName = "XSL";

      public const string EOCUserXmlUrl = "http://folding.extremeoverclocking.com/xml/user_summary.php?u=";
      public const string EOCUserBaseUrl = "http://folding.extremeoverclocking.com/user_summary.php?s=&u=";
      public const string EOCTeamBaseUrl = "http://folding.extremeoverclocking.com/team_summary.php?s=&t=";
      public const string StanfordBaseUrl = "http://fah-web.stanford.edu/cgi-bin/main.py?qtype=userpage&username=";

      public const Int32 MinDecimalPlaces = 0;
      public const Int32 MaxDecimalPlaces = 5;

      public const Int32 MinMinutes = 1;
      public const Int32 MaxMinutes = 180;

      public const Int32 MinutesDefault = 15;
      public const Int32 ProxyPortDefault = 8080;

      public const string UnassignedDescription = "Unassigned Description";

      // Folding ID and Team Defaults
      public const string FoldingIDDefault = "Unknown";
      public const int TeamDefault = 0;

      // Log Filename Constants
      public const string LocalFAHLog = "FAHlog.txt";
      public const string LocalUnitInfo = "unitinfo.txt";
      public const string LocalQueue = "queue.dat";
      #endregion
   }
}
