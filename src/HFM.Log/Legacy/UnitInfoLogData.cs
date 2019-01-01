/*
 * HFM.NET - unitinfo.txt Log Data Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;

namespace HFM.Log.Legacy
{
   /// <summary>
   /// Represents data read from a Folding@Home v6 or prior client unitinfo.txt log.
   /// </summary>
   public class UnitInfoLogData
   {
      /// <summary>
      /// Gets or sets the protein name.
      /// </summary>
      public string ProteinName { get; set; }

      /// <summary>
      /// Gets or sets the protein tag.
      /// </summary>
      public string ProteinTag { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Number).
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Run).
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Clone).
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Gen).
      /// </summary>
      public int ProjectGen { get; set; }

      /// <summary>
      /// Gets or sets the download time.
      /// </summary>
      public DateTime DownloadTime { get; set; }

      /// <summary>
      /// Gets or sets the due time.
      /// </summary>
      public DateTime DueTime { get; set; }

      /// <summary>
      /// Gets or sets the progress percentage.
      /// </summary>
      public int Progress { get; set; }
   }
}
