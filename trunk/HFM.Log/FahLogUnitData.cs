/*
 * HFM.NET - FAHlog.txt Unit Data Class
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

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   public class FahLogUnitData : IProjectInfo
   {
      public FahLogUnitData()
      {
         UnitStartTimeStamp = TimeSpan.MinValue;
         FrameDataList = new List<LogLine>(101);
         CoreVersion = String.Empty;
         ProjectInfoIndex = -1;
         ProjectInfoList = new List<IProjectInfo>();
         UnitResult = WorkUnitResult.Unknown;
      }

      /// <summary>
      /// Unit Start Time Stamp
      /// </summary>
      public TimeSpan UnitStartTimeStamp { get; set; }

      /// <summary>
      /// List of Log Lines containing Frame Data
      /// </summary>
      public IList<LogLine> FrameDataList { get; set; }

      /// <summary>
      /// Number of Frames Observed since Last Unit Start
      /// </summary>
      public int FramesObserved { get; set; }

      /// <summary>
      /// Core Version
      /// </summary>
      public string CoreVersion { get; set; }

      /// <summary>
      /// Project Info List Current Index
      /// </summary>
      public int ProjectInfoIndex { get; set; }

      /// <summary>
      /// Project Info List
      /// </summary>
      public IList<IProjectInfo> ProjectInfoList { get; private set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public int ProjectID
      {
         get 
         { 
            if (ProjectInfoIndex == -1)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectID;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex].ProjectID;
         }
      }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public int ProjectRun
      {
         get
         {
            if (ProjectInfoIndex == -1)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectRun;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex].ProjectRun;
         }
      }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public int ProjectClone
      {
         get
         {
            if (ProjectInfoIndex == -1)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectClone;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex].ProjectClone;
         }
      }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public int ProjectGen
      {
         get
         {
            if (ProjectInfoIndex == -1)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectGen;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex].ProjectGen;
         }
      }

      /// <summary>
      /// Work Unit Result
      /// </summary>
      public WorkUnitResult UnitResult { get; set; }
   }
}
