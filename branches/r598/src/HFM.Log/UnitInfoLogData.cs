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

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public class UnitInfoLogData : IProjectInfo
   {
      /// <summary>
      /// Protein Name
      /// </summary>
      public string ProteinName { get; set; }

      /// <summary>
      /// Protein Tag
      /// </summary>
      public string ProteinTag { get; set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public int ProjectGen { get; set; }

      /// <summary>
      /// Download Time
      /// </summary>
      public DateTime DownloadTime { get; set; }

      /// <summary>
      /// Due Time
      /// </summary>
      public DateTime DueTime { get; set; }

      /// <summary>
      /// Progress Percentage
      /// </summary>
      public int Progress { get; set; }
   }
}