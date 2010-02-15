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

using HFM.Framework;

namespace HFM.Log
{
   public class FahLogUnitData : IFahLogUnitData
   {
      private TimeSpan _UnitStartTimeStamp = TimeSpan.MinValue;
      /// <summary>
      /// Unit Starting Time Stamp
      /// </summary>
      public TimeSpan UnitStartTimeStamp
      {
         get { return _UnitStartTimeStamp; }
         set { _UnitStartTimeStamp = value; }
      }

      private IList<ILogLine> _FrameDataList = new List<ILogLine>(101);
      /// <summary>
      /// List of Log Lines containing Frame Data
      /// </summary>
      public IList<ILogLine> FrameDataList
      {
         get { return _FrameDataList; }
         set { _FrameDataList = value; }
      }

      private Int32 _FramesObserved;
      /// <summary>
      /// Number of Frames Observed since Last Unit Start
      /// </summary>
      public Int32 FramesObserved
      {
         get { return _FramesObserved; }
         set { _FramesObserved = value; }
      }
   
      private string _CoreVersion = String.Empty;
      /// <summary>
      /// Core Version
      /// </summary>
      public string CoreVersion
      {
         get { return _CoreVersion; }
         set { _CoreVersion = value; }
      }
      
      private Int32 _ProjectInfoIndex = -1;
      /// <summary>
      /// Project Info List Current Index
      /// </summary>
      public Int32 ProjectInfoIndex
      {
         get { return _ProjectInfoIndex; }
         set { _ProjectInfoIndex = value; }
      }
      
      private readonly IList<IProjectInfo> _ProjectInfoList = new List<IProjectInfo>();
      /// <summary>
      /// Project Info List
      /// </summary>
      public IList<IProjectInfo> ProjectInfoList
      {
         get { return _ProjectInfoList; }
      }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public Int32 ProjectID
      {
         get 
         { 
            if (_ProjectInfoIndex == -1)
            {
               if (_ProjectInfoList.Count > 0)
               {
                  return _ProjectInfoList[_ProjectInfoList.Count - 1].ProjectID;
               }
               return 0;
            }

            return _ProjectInfoList[_ProjectInfoIndex].ProjectID;
         }
      }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public Int32 ProjectRun
      {
         get
         {
            if (_ProjectInfoIndex == -1)
            {
               if (_ProjectInfoList.Count > 0)
               {
                  return _ProjectInfoList[_ProjectInfoList.Count - 1].ProjectRun;
               }
               return 0;
            }

            return _ProjectInfoList[_ProjectInfoIndex].ProjectRun;
         }
      }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public Int32 ProjectClone
      {
         get
         {
            if (_ProjectInfoIndex == -1)
            {
               if (_ProjectInfoList.Count > 0)
               {
                  return _ProjectInfoList[_ProjectInfoList.Count - 1].ProjectClone;
               }
               return 0;
            }

            return _ProjectInfoList[_ProjectInfoIndex].ProjectClone;
         }
      }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public Int32 ProjectGen
      {
         get
         {
            if (_ProjectInfoIndex == -1)
            {
               if (_ProjectInfoList.Count > 0)
               {
                  return _ProjectInfoList[_ProjectInfoList.Count - 1].ProjectGen;
               }
               return 0;
            }

            return _ProjectInfoList[_ProjectInfoIndex].ProjectGen;
         }
      }
   
      private WorkUnitResult _UnitResult = WorkUnitResult.Unknown;
      /// <summary>
      /// Work Unit Result
      /// </summary>
      public WorkUnitResult UnitResult
      {
         get { return _UnitResult; }
         set { _UnitResult = value; }
      }

      private ClientStatus _Status = ClientStatus.Unknown;
      /// <summary>
      /// Client Status
      /// </summary>
      public ClientStatus Status
      {
         get { return _Status; }
         set { _Status = value; }
      }
   }
}