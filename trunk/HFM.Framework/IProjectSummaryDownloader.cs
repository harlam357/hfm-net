/*
 * HFM.NET - Project Summary Downloader Interface
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
using System.Collections.Generic;

namespace HFM.Framework
{
   public interface IProjectSummaryDownloader
   {
      /// <summary>
      /// Time of Last Successful Download
      /// </summary>
      DateTime LastDownloadTime { get; }

      /// <summary>
      /// Project Summary HTML File Location
      /// </summary>
      Uri ProjectSummaryLocation { get; set; }

      /// <summary>
      /// Local Project Info Tab File Location
      /// </summary>
      string ProjectInfoLocation { get; set; }

      /// <summary>
      /// Protein Storage Dictionary
      /// </summary>
      SortedDictionary<Int32, IProtein> Dictionary { get; set; }

      /// <summary>
      /// Preferences Interface
      /// </summary>
      IPreferenceSet Prefs { get; set; }

      /// <summary>
      /// Project (Protein) Data has been Updated
      /// </summary>
      event EventHandler ProjectInfoUpdated;

      /// <summary>
      /// Reset the Last Download Time
      /// </summary>
      void ResetLastDownloadTime();

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      void DownloadFromStanford();

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      void DownloadFromStanford(Uri ProjectDownloadUrl, bool SaveToFile);

      /// <summary>
      /// Read Project Information from HTML (psummary.html)
      /// </summary>
      void ReadFromProjectSummaryHtml(Uri location);
   }
}
