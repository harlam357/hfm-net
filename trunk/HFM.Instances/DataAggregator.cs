/*
 * HFM.NET - Data Aggregator Class
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public class DataAggregator : IDataAggregator
   {
      private readonly IQueueReader _queueReader;
      private readonly ILogReaderFactory _logReaderFactory;
      private readonly IUnitInfoFactory _unitInfoFactory;

      private ILogReader _logReader = null;

      private string _InstanceName;
      /// <summary>
      /// Instance Name
      /// </summary>
      public string InstanceName
      {
         get { return _InstanceName; }
         set { _InstanceName = value; }
      }

      private string _queueFilePath;
      /// <summary>
      /// queue.dat File Path
      /// </summary>
      public string QueueFilePath
      {
         get { return _queueFilePath; }
         set { _queueFilePath = value; }
      }

      private string _fahLogFilePath;
      /// <summary>
      /// FAHlog.txt File Path
      /// </summary>
      public string FahLogFilePath
      {
         get { return _fahLogFilePath; }
         set { _fahLogFilePath = value; }
      }

      private string _unitInfoLogFilePath;
      /// <summary>
      /// unitinfo.txt File Path
      /// </summary>
      public string UnitInfoLogFilePath
      {
         get { return _unitInfoLogFilePath; }
         set { _unitInfoLogFilePath = value; }
      }

      /// <summary>
      /// Queue Base Interface
      /// </summary>
      [CLSCompliant(false)]
      public IQueueBase Queue
      {
         get
         {
            if (_queueReader.QueueReadOk)
            {
               return _queueReader.Queue;
            }

            return null;
         }
      }

      /// <summary>
      /// Current Index in List of returned UnitInfo and UnitLogLines
      /// </summary>
      public int CurrentUnitIndex
      {
         get
         {
            if (_queueReader.QueueReadOk)
            {
               return (int) _queueReader.Queue.CurrentIndex;
            }

            // default Unit Index if only parsing logs
            return 1;
         }
      }

      private IClientRun _CurrentClientRun;
      /// <summary>
      /// Client Run Data for the Current Run
      /// </summary>
      public IClientRun CurrentClientRun
      {
         get { return _CurrentClientRun; }
      }

      private IFahLogUnitData _CurrentFahLogUnitData;
      /// <summary>
      /// Current Work Unit Status based on LogReader CurrentWorkUnitLogLines
      /// </summary>
      public ClientStatus CurrentWorkUnitStatus
      {
         get { return _CurrentFahLogUnitData.Status; }
      }

      /// <summary>
      /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
      /// </summary>
      public IList<ILogLine> CurrentLogLines
      {
         get 
         { 
            if (_UnitLogLines == null)
            {
               return new List<ILogLine>();
            }
            
            return _UnitLogLines[CurrentUnitIndex]; 
         }
      }

      private IList<ILogLine>[] _UnitLogLines;
      /// <summary>
      /// Array of LogLine Lists
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public IList<ILogLine>[] UnitLogLines
      {
         get { return _UnitLogLines; }
      }

      [CLSCompliant(false)]
      public DataAggregator(IQueueReader queueReader, ILogReaderFactory logReaderFactory, IUnitInfoFactory unitInfoFactory)
      {
         _queueReader = queueReader;
         _logReaderFactory = logReaderFactory;
         _unitInfoFactory = unitInfoFactory;
      }

      #region Aggregation Logic
      
      /// <summary>
      /// Aggregate Data and return UnitInfo List
      /// </summary>
      public IList<IUnitInfo> AggregateData()
      {
         _logReader = _logReaderFactory.Create();
      
         IList<IUnitInfo> parsedUnits;

         _logReader.ScanFAHLog(_InstanceName, _fahLogFilePath);
         _CurrentClientRun = _logReader.CurrentClientRun;
         _CurrentFahLogUnitData = _logReader.GetFahLogDataFromLogLines(_logReader.CurrentWorkUnitLogLines);

         // Decision Time: If Queue Read fails parse from logs only
         if (ReadQueueFile())
         {
            parsedUnits = GenerateUnitInfoDataFromQueue();
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _InstanceName, 
               "Queue unavailable or failed read.  Parsing logs without queue.");

            parsedUnits = GenerateUnitInfoDataFromLogs();
         }
         
         _logReader = null;

         return parsedUnits;
      }

      /// <summary>
      /// Read the queue.dat file
      /// </summary>
      private bool ReadQueueFile()
      {
         bool success = false;

         // Make sure the queue file exists first.  Would like to avoid the exception overhead.
         if (File.Exists(_queueFilePath))
         {
            // queue.dat is not required to get a reading 
            // if something goes wrong just catch and log
            try
            {
               _queueReader.ReadQueue(_queueFilePath);
               if (_queueReader.QueueReadOk)
               {
                  success = true;
               }
               else
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _InstanceName, 
                     String.Format("{0} read failed.", _queueReader.QueueFilePath));
               }
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(_InstanceName, ex);
            }
         }

         return success;
      }

      private IUnitInfo[] GenerateUnitInfoDataFromLogs()
      {
         IUnitInfo[] parsedUnits = new IUnitInfo[2];
         _UnitLogLines = new IList<ILogLine>[2];

         if (_logReader.PreviousWorkUnitLogLines != null)
         {
            _UnitLogLines[0] = _logReader.PreviousWorkUnitLogLines;
            parsedUnits[0] = BuildUnitInfo(null, _logReader.GetFahLogDataFromLogLines(_logReader.PreviousWorkUnitLogLines), null);
         }
         _UnitLogLines[1] = _logReader.CurrentWorkUnitLogLines;
         parsedUnits[1] = BuildUnitInfo(null, _CurrentFahLogUnitData, _logReader.GetUnitInfoLogData(_InstanceName, _unitInfoLogFilePath));

         return parsedUnits;
      }

      private IUnitInfo[] GenerateUnitInfoDataFromQueue()
      {
         IUnitInfo[] parsedUnits = new IUnitInfo[10];
         _UnitLogLines = new IList<ILogLine>[10];

         for (int queueIndex = 0; queueIndex < parsedUnits.Length; queueIndex++)
         {
            // Get the Log Lines for this queue position from the reader
            _UnitLogLines[queueIndex] = _logReader.GetLogLinesFromQueueIndex(queueIndex);
            // Get the FAH Log Data from the Log Lines
            IFahLogUnitData fahLogUnitData = _logReader.GetFahLogDataFromLogLines(_UnitLogLines[queueIndex]);

            parsedUnits[queueIndex] = BuildUnitInfo(_queueReader.Queue.GetQueueEntry((uint) queueIndex), fahLogUnitData, null);
            if (parsedUnits[queueIndex] == null)
            {
               if (queueIndex == _queueReader.Queue.CurrentIndex)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _InstanceName, String.Format(CultureInfo.CurrentCulture,
                     "Could not verify log section for current queue entry ({0}). Trying to parse with most recent log section.", queueIndex));

                  _UnitLogLines[queueIndex] = _logReader.CurrentWorkUnitLogLines;
                  fahLogUnitData = _CurrentFahLogUnitData;
                  if (_CurrentFahLogUnitData.Status.Equals(ClientStatus.GettingWorkPacket))
                  {
                     _UnitLogLines[queueIndex] = null;
                     fahLogUnitData = _logReader.GetEmptyFahLogData();
                  }
                  parsedUnits[queueIndex] = BuildUnitInfo(_queueReader.Queue.GetQueueEntry((uint) queueIndex), fahLogUnitData,
                                                          _logReader.GetUnitInfoLogData(_InstanceName, _unitInfoLogFilePath), true);
               }
               else
               {
                  // Just skip this unit and continue
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _InstanceName, String.Format(CultureInfo.CurrentCulture,
                     "Could not find or verify log section for queue entry {0} (this is not a problem).", queueIndex));
               }
            }
         }

         return parsedUnits;
      }
      
      private IUnitInfo BuildUnitInfo(IQueueEntry queueEntry, IFahLogUnitData fahLogUnitData, IUnitInfoLogData unitInfoLogData)
      {
         return BuildUnitInfo(queueEntry, fahLogUnitData, unitInfoLogData, false);
      }

      private IUnitInfo BuildUnitInfo(IQueueEntry queueEntry, IFahLogUnitData fahLogUnitData, IUnitInfoLogData unitInfoLogData, bool MatchOverride)
      {
         IUnitInfo unit = _unitInfoFactory.Create();
         unit.UnitStartTimeStamp = fahLogUnitData.UnitStartTimeStamp;
         unit.FramesObserved = fahLogUnitData.FramesObserved;
         unit.CoreVersion = fahLogUnitData.CoreVersion;
         unit.UnitResult = fahLogUnitData.UnitResult;

         if (queueEntry != null)
         {
            PopulateUnitInfoFromQueueEntry(queueEntry, unit);
            PopulateUnitInfoFromLogs(CurrentClientRun, fahLogUnitData, unitInfoLogData, unit);

            if (ProjectsMatch(unit, fahLogUnitData) ||
                ProjectsMatch(unit, unitInfoLogData) ||
                MatchOverride)
            {
               // continue parsing the frame data
               ParseFrameData(fahLogUnitData.FrameDataList, unit);
            }
            else
            {
               return null;
            }
         }
         else
         {
            PopulateUnitInfoFromLogs(CurrentClientRun, fahLogUnitData, unitInfoLogData, unit);
            ParseFrameData(fahLogUnitData.FrameDataList, unit);
         }

         return unit;
      }

      private static bool ProjectsMatch(IProjectInfo project1, IProjectInfo project2)
      {
         if (project1 == null || project2 == null) return false;
      
         return (project1.ProjectID == project2.ProjectID &&
                 project1.ProjectRun == project2.ProjectRun &&
                 project1.ProjectClone == project2.ProjectClone &&
                 project1.ProjectGen == project2.ProjectGen);
      }

      #region Unit Population Methods
      private static void PopulateUnitInfoFromQueueEntry(IQueueEntry entry, IUnitInfo unit)
      {
         if ((entry.EntryStatus.Equals(QueueEntryStatus.Unknown) ||
              entry.EntryStatus.Equals(QueueEntryStatus.Empty) ||
              entry.EntryStatus.Equals(QueueEntryStatus.Garbage) ||
              entry.EntryStatus.Equals(QueueEntryStatus.Abandonded)) == false)
         {
            /* Tag (Could be read here or through the unitinfo.txt file) */
            unit.ProteinTag = entry.WorkUnitTag;

            /* DownloadTime (Could be read here or through the unitinfo.txt file) */
            unit.DownloadTime = entry.BeginTimeUtc;

            /* DueTime (Could be read here or through the unitinfo.txt file) */
            unit.DueTime = entry.DueTimeUtc;

            /* FinishedTime */
            if (entry.EntryStatus.Equals(QueueEntryStatus.Finished))
            {
               unit.FinishedTime = entry.EndTimeUtc;
            }

            /* Project (R/C/G) */
            unit.ProjectID = entry.ProjectID;
            unit.ProjectRun = entry.ProjectRun;
            unit.ProjectClone = entry.ProjectClone;
            unit.ProjectGen = entry.ProjectGen;

            /* FoldingID and Team from Queue Entry */
            unit.FoldingID = entry.FoldingID;
            unit.Team = (int) entry.TeamNumber;
         }
      }

      private static void PopulateUnitInfoFromLogs(IClientRun CurrentClientRun, IFahLogUnitData fahLogUnitData, 
                                                   IUnitInfoLogData unitInfoLogData, IUnitInfo unit)
      {
         Debug.Assert(CurrentClientRun != null);
         Debug.Assert(fahLogUnitData != null);
         Debug.Assert(unit != null);

         /* Project (R/C/G) (Could have already been read through Queue) */
         if (unit.ProjectIsUnknown)
         {
            unit.ProjectID = fahLogUnitData.ProjectID;
            unit.ProjectRun = fahLogUnitData.ProjectRun;
            unit.ProjectClone = fahLogUnitData.ProjectClone;
            unit.ProjectGen = fahLogUnitData.ProjectGen;
         }

         if (unitInfoLogData != null)
         {
            unit.ProteinName = unitInfoLogData.ProteinName;

            /* Tag (Could have already been read through Queue) */
            if (unit.ProteinTag.Length == 0)
            {
               unit.ProteinTag = unitInfoLogData.ProteinTag;
            }

            /* DownloadTime (Could have already been read through Queue) */
            if (unit.DownloadTimeUnknown)
            {
               unit.DownloadTime = unitInfoLogData.DownloadTime;
            }

            /* DueTime (Could have already been read through Queue) */
            if (unit.DueTimeUnknown)
            {
               unit.DueTime = unitInfoLogData.DueTime;
            }

            /* FinishedTime (Not available in unitinfo log) */

            /* Project (R/C/G) (Could have already been read through Queue) */
            if (unit.ProjectIsUnknown)
            {
               unit.ProjectID = unitInfoLogData.ProjectID;
               unit.ProjectRun = unitInfoLogData.ProjectRun;
               unit.ProjectClone = unitInfoLogData.ProjectClone;
               unit.ProjectGen = unitInfoLogData.ProjectGen;
            }
         }

         /* FoldingID and Team from Last Client Run (Could have already been read through Queue) */
         if (unit.FoldingID.Equals(Constants.FoldingIDDefault))
         {
            unit.FoldingID = CurrentClientRun.FoldingID;
         }
         if (unit.Team == Constants.TeamDefault)
         {
            unit.Team = CurrentClientRun.Team;
         }
      }

      private static void ParseFrameData(IEnumerable<ILogLine> frameData, IUnitInfo unit)
      {
         foreach (ILogLine frame in frameData)
         {
            unit.SetCurrentFrame(frame);
         }
      }
      #endregion
      
      #endregion
   }
}