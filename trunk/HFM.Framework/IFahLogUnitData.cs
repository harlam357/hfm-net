/*
 * HFM.NET - FAHlog.txt Unit Data Interface
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
using System.Collections.Generic;

namespace HFM.Framework
{
   public interface IFahLogUnitData : IProjectInfo
   {
      /// <summary>
      /// Client Status
      /// </summary>
      TimeSpan UnitStartTimeStamp { get; }
      
      /// <summary>
      /// List of Log Lines containing Frame Data
      /// </summary>
      IList<LogLine> FrameDataList { get; }
      
      /// <summary>
      /// Number of Frames Observed since Last Unit Start
      /// </summary>
      Int32 FramesObserved { get; }

      /// <summary>
      /// Core Version
      /// </summary>
      string CoreVersion { get; }

      /// <summary>
      /// Project Info List Current Index
      /// </summary>
      Int32 ProjectInfoIndex { get; set; }

      /// <summary>
      /// Project Info List
      /// </summary>
      IList<IProjectInfo> ProjectInfoList { get; }

      /// <summary>
      /// Work Unit Result
      /// </summary>
      WorkUnitResult UnitResult { get; }

      /// <summary>
      /// Client Status
      /// </summary>
      ClientStatus Status { get; }
   }
}