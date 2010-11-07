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
using HFM.Framework.DataTypes;
using HFM.Log;
using HFM.Framework;

namespace HFM.DataAggregator
{
   public class DataAggregator : IDataAggregator
   {
      private readonly IQueueReader _queueReader;

      private ILogReader _logReader;

      /// <summary>
      /// Instance Name
      /// </summary>
      public string InstanceName { get; set; }

      /// <summary>
      /// queue.dat File Path
      /// </summary>
      public string QueueFilePath { get; set; }

      /// <summary>
      /// FAHlog.txt File Path
      /// </summary>
      public string FahLogFilePath { get; set; }

      /// <summary>
      /// unitinfo.txt File Path
      /// </summary>
      public string UnitInfoLogFilePath { get; set; }

      /// <summary>
      /// Queue Base Interface
      /// </summary>
      [CLSCompliant(false)]
      public IQueueBase Queue
      {
         get
         {
            return _queueReader.QueueReadOk ? _queueReader.Queue : null;
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

      private ClientRun _currentClientRun;
      /// <summary>
      /// Client Run Data for the Current Run
      /// </summary>
      public ClientRun CurrentClientRun
      {
         get { return _currentClientRun; }
      }

      private FahLogUnitData _currentFahLogUnitData;
      /// <summary>
      /// Current Work Unit Status based on LogReader CurrentWorkUnitLogLines
      /// </summary>
      public ClientStatus CurrentWorkUnitStatus
      {
         get { return _currentFahLogUnitData.Status; }
      }

      /// <summary>
      /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
      /// </summary>
      public IList<LogLine> CurrentLogLines
      {
         get
         {
            return _unitLogLines == null ? new List<LogLine>() : _unitLogLines[CurrentUnitIndex];
         }
      }

      private IList<LogLine>[] _unitLogLines;
      /// <summary>
      /// Array of LogLine Lists
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public IList<LogLine>[] UnitLogLines
      {
         get { return _unitLogLines; }
      }

      [CLSCompliant(false)]
      public DataAggregator(IQueueReader queueReader)
      {
         _queueReader = queueReader;
      }

      #region Aggregation Logic
      
      /// <summary>
      /// Aggregate Data and return UnitInfo List
      /// </summary>
      public IList<IUnitInfo> AggregateData()
      {
         _logReader = new LogReader();
      
         IList<IUnitInfo> parsedUnits;

         _logReader.ScanFahLog(InstanceName, FahLogFilePath);
         _currentClientRun = _logReader.CurrentClientRun;

         // Decision Time: If Queue Read fails parse from logs only
         if (ReadQueueFile())
         {
            parsedUnits = GenerateUnitInfoDataFromQueue();
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, 
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
         if (File.Exists(QueueFilePath))
         {
            // queue.dat is not required to get a reading 
            // if something goes wrong just catch and log
            try
            {
               _queueReader.ReadQueue(QueueFilePath);
               if (_queueReader.QueueReadOk)
               {
                  success = true;
               }
               else
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, 
                     String.Format("{0} read failed.", _queueReader.QueueFilePath));
               }
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(InstanceName, ex);
            }
         }

         // If read is unsuccessful, clear the queue - Issue 171
         if (success == false)
         {
            _queueReader.ClearQueue();
         }

         return success;
      }

      private IUnitInfo[] GenerateUnitInfoDataFromLogs()
      {
         var parsedUnits = new IUnitInfo[2];
         _unitLogLines = new IList<LogLine>[2];

         if (_logReader.PreviousWorkUnitLogLines != null)
         {
            _unitLogLines[0] = _logReader.PreviousWorkUnitLogLines;
            parsedUnits[0] = BuildUnitInfo(null, _logReader.GetFahLogDataFromLogLines(_logReader.PreviousWorkUnitLogLines), null);
         }

         bool matchOverride = false;
         _unitLogLines[1] = _logReader.CurrentWorkUnitLogLines;
         if (_unitLogLines[1] == null)
         {
            matchOverride = true;
            _unitLogLines[1] = _logReader.CurrentClientRunLogLines;
         }
         
         _currentFahLogUnitData = _logReader.GetFahLogDataFromLogLines(_unitLogLines[1]);
         parsedUnits[1] = BuildUnitInfo(null, _currentFahLogUnitData, _logReader.GetUnitInfoLogData(InstanceName, UnitInfoLogFilePath), matchOverride);

         return parsedUnits;
      }

      private IUnitInfo[] GenerateUnitInfoDataFromQueue()
      {
         var parsedUnits = new IUnitInfo[10];
         _unitLogLines = new IList<LogLine>[10];

         for (int queueIndex = 0; queueIndex < parsedUnits.Length; queueIndex++)
         {
            // Get the Log Lines for this queue position from the reader
            _unitLogLines[queueIndex] = _logReader.GetLogLinesFromQueueIndex(queueIndex);
            // Get the FAH Log Data from the Log Lines
            FahLogUnitData fahLogUnitData = _logReader.GetFahLogDataFromLogLines(_unitLogLines[queueIndex]);
            UnitInfoLogData unitInfoLogData = null;
            // On the Current Queue Index
            if (queueIndex == _queueReader.Queue.CurrentIndex)
            {
               // Get the UnitInfo Log Data
               unitInfoLogData = _logReader.GetUnitInfoLogData(InstanceName, UnitInfoLogFilePath);
               _currentFahLogUnitData = fahLogUnitData;
            }

            parsedUnits[queueIndex] = BuildUnitInfo(_queueReader.Queue.GetQueueEntry((uint)queueIndex), fahLogUnitData, unitInfoLogData);
            if (parsedUnits[queueIndex] == null)
            {
               if (queueIndex == _queueReader.Queue.CurrentIndex)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, String.Format(CultureInfo.CurrentCulture,
                     "Could not verify log section for current queue entry ({0}). Trying to parse with most recent log section.", queueIndex));

                  _unitLogLines[queueIndex] = _logReader.CurrentWorkUnitLogLines;
                  // If got no Work Unit Log Lines based on Current Work Unit Log Lines
                  // then take the entire Current Client Run Log Lines - likely the run
                  // was short and never contained any Work Unit Data.
                  if (_unitLogLines[queueIndex] == null)
                  {
                     _unitLogLines[queueIndex] = _logReader.CurrentClientRunLogLines;
                  }
                  fahLogUnitData = _logReader.GetFahLogDataFromLogLines(_unitLogLines[queueIndex]);
                  _currentFahLogUnitData = fahLogUnitData;

                  if (_currentFahLogUnitData.Status.Equals(ClientStatus.GettingWorkPacket))
                  {
                     // Use either the current Work Unit log lines or current Client Run log lines
                     // as decided upon above... don't clear it here and show the user nothing - 10/9/10
                     //_unitLogLines[queueIndex] = null;
                     fahLogUnitData = new FahLogUnitData();
                     unitInfoLogData = new UnitInfoLogData();
                  }
                  parsedUnits[queueIndex] = BuildUnitInfo(_queueReader.Queue.GetQueueEntry((uint) queueIndex), fahLogUnitData, unitInfoLogData, true);
               }
               else
               {
                  // Just skip this unit and continue
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, InstanceName, String.Format(CultureInfo.CurrentCulture,
                     "Could not find or verify log section for queue entry {0} (this is not a problem).", queueIndex));
               }
            }
         }

         return parsedUnits;
      }
      
      private IUnitInfo BuildUnitInfo(IQueueEntry queueEntry, FahLogUnitData fahLogUnitData, UnitInfoLogData unitInfoLogData)
      {
         return BuildUnitInfo(queueEntry, fahLogUnitData, unitInfoLogData, false);
      }

      private IUnitInfo BuildUnitInfo(IQueueEntry queueEntry, FahLogUnitData fahLogUnitData, UnitInfoLogData unitInfoLogData, bool matchOverride)
      {
         IUnitInfo unit = new UnitInfo();
         unit.UnitStartTimeStamp = fahLogUnitData.UnitStartTimeStamp;
         unit.FramesObserved = fahLogUnitData.FramesObserved;
         unit.CoreVersion = fahLogUnitData.CoreVersion;
         unit.UnitResult = fahLogUnitData.UnitResult;

         if (queueEntry != null)
         {
            PopulateUnitInfoFromQueueEntry(queueEntry, unit);
            SearchFahLogUnitDataProjects(unit, fahLogUnitData);
            PopulateUnitInfoFromLogs(CurrentClientRun, fahLogUnitData, unitInfoLogData, unit);

            if (ProjectsMatch(unit, fahLogUnitData) ||
                ProjectsMatch(unit, unitInfoLogData) ||
                matchOverride)
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

      private static void SearchFahLogUnitDataProjects(IUnitInfo unit, FahLogUnitData fahLogUnitData)
      {
         Debug.Assert(unit != null);
         for (int i = 0; i < fahLogUnitData.ProjectInfoList.Count; i++)
         {
            if (ProjectsMatch(unit, fahLogUnitData.ProjectInfoList[i]))
            {
               fahLogUnitData.ProjectInfoIndex = i;
            }
         }
      }

      private static bool ProjectsMatch(IUnitInfo unit, IProjectInfo projectInfo)
      {
         Debug.Assert(unit != null);
         if (unit.ProjectIsUnknown || projectInfo == null) return false;
      
         return (unit.ProjectID == projectInfo.ProjectID &&
                 unit.ProjectRun == projectInfo.ProjectRun &&
                 unit.ProjectClone == projectInfo.ProjectClone &&
                 unit.ProjectGen == projectInfo.ProjectGen);
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
            if (entry.EntryStatus.Equals(QueueEntryStatus.Finished) ||
                entry.EntryStatus.Equals(QueueEntryStatus.ReadyForUpload))
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
            
            /* Core ID */
            unit.CoreID = entry.CoreNumber.ToUpperInvariant();
         }
      }

      private static void PopulateUnitInfoFromLogs(ClientRun currentClientRun, FahLogUnitData fahLogUnitData, 
                                                   UnitInfoLogData unitInfoLogData, IUnitInfo unit)
      {
         Debug.Assert(currentClientRun != null);
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
            unit.FoldingID = currentClientRun.FoldingID;
         }
         if (unit.Team == Constants.TeamDefault)
         {
            unit.Team = currentClientRun.Team;
         }
      }

      private static void ParseFrameData(IEnumerable<LogLine> frameData, IUnitInfo unit)
      {
         foreach (var logLine in frameData)
         {
            // Check for FrameData
            var frame = logLine.LineData as UnitFrame;
            if (frame == null)
            {
               // If not found, clear the LineType and get out
               logLine.LineType = LogLineType.Unknown;
               continue;
            }
            
            unit.SetCurrentFrame(frame);
         }
      }
      #endregion
      
      #endregion
   }
}
