/*
 * HFM.NET - Project Info Interface
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

namespace HFM.Framework
{
   public interface IProjectInfo
   {
      /// <summary>
      /// Project ID Number
      /// </summary>
      Int32 ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      Int32 ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      Int32 ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      Int32 ProjectGen { get; set; }
   }
}
