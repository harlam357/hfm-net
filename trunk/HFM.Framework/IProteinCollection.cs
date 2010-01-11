/*
 * HFM.NET - Protein Collection Interface
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
   public interface IProteinCollection
   {
      /// <summary>
      /// ProjectInfo.tab File Location
      /// </summary>
      string ProjectInfoLocation { get; }

      /// <summary>
      /// Project Summary HTML File Location
      /// </summary>
      Uri ProjectSummaryLocation { get; set; }

      /// <summary>
      /// Project (Protein) Data has been Updated
      /// </summary>
      event EventHandler ProjectInfoUpdated;

      /// <summary>
      /// Execute Primary Collection Load Sequence
      /// </summary>
      void Load();

      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      bool LoadFromTabDelimitedFile();

      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      /// <param name="ProjectInfoFilePath">Path to File</param>
      bool LoadFromTabDelimitedFile(string ProjectInfoFilePath);

      /// <summary>
      /// Download project information from Stanford University (psummary.html)
      /// </summary>
      IAsyncResult BeginDownloadFromStanford();

      /// <summary>
      /// Download project information from Stanford University (psummary.html)
      /// </summary>
      void DownloadFromStanford();

      /// <summary>
      /// Read Project Information from HTML (psummary.html)
      /// </summary>
      void ReadFromProjectSummaryHtml(Uri location);

      /// <summary>
      /// Get Protein (should be called from worker thread)
      /// </summary>
      /// <param name="ProjectID">Project ID</param>
      IProtein GetProtein(int ProjectID);

      /// <summary>
      /// Get a New Protein from the Collection
      /// </summary>
      IProtein GetNewProtein();

      void Add(int key, IProtein value);
      bool Remove(int key);
      void Clear();
      
      bool ContainsKey(int key);
      bool ContainsValue(IProtein value);
      
      bool TryGetValue(int key, out IProtein value);
      
      IProtein this[int key] { get; set; }
      int Count { get; }
   }
}