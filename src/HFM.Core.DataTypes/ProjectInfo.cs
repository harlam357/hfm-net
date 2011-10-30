/*
 * HFM.NET - Project Info Class
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

namespace HFM.Core.DataTypes
{
   public interface IProjectInfo
   {
      /// <summary>
      /// Project ID Number
      /// </summary>
      int ProjectID { get; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      int ProjectRun { get; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      int ProjectClone { get; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      int ProjectGen { get; }
   }

   public class ProjectInfo : IProjectInfo
   {
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
   }
}
