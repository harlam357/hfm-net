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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
   public interface IProteinCollection
   {
      /// <summary>
      /// ProjectInfo.tab File Location
      /// </summary>
      string ProjectInfoLocation { get; }

      /// <summary>
      /// Project Summary Downloader Interface
      /// </summary>
      IProjectSummaryDownloader Downloader { get; }

      /// <summary>
      /// Collection of Proteins
      /// </summary>
      ICollection<IProtein> Proteins { get; }

      /// <summary>
      /// Execute Primary Collection Read Sequence
      /// </summary>
      void Read();

      /// <summary>
      /// Read the Protein Collection from Tab Delimited File
      /// </summary>
      bool ReadFromTabDelimitedFile();

      /// <summary>
      /// Read the Protein Collection from Tab Delimited File
      /// </summary>
      /// <param name="projectInfoFilePath">Path to File</param>
      bool ReadFromTabDelimitedFile(string projectInfoFilePath);

      /// <summary>
      /// Clear the Projects not found cache
      /// </summary>
      void ClearProjectsNotFoundCache();

      /// <summary>
      /// Get Protein (should be called from worker thread)
      /// </summary>
      /// <param name="projectId">Project ID</param>
      IProtein GetProtein(int projectId);

      /// <summary>
      /// Get Protein from Collection (should be called from worker thread)
      /// </summary>
      /// <param name="projectId">Project ID</param>
      /// <param name="allowProteinDownload">Allow Download from psummary</param>
      IProtein GetProtein(int projectId, bool allowProteinDownload);

      /// <summary>
      /// Create a New Protein
      /// </summary>
      IProtein CreateProtein();

      #region Want to get rid of these direct collection accessors
      bool ContainsKey(int key);
      
      bool TryGetValue(int key, out IProtein value);
      
      IProtein this[int key] { get; set; }
      #endregion

      int Count { get; }
   }
}
