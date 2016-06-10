/*
 * HFM.NET - Legacy Data Aggregator Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Linq;

using AutoMapper;
using Castle.Core.Logging;

using HFM.Core.DataTypes;
using HFM.Log;
using HFM.Queue;

namespace HFM.Core
{
   internal class LegacyDataAggregator
   {
      private FahLog _fahLog;

      /// <summary>
      /// Instance Name
      /// </summary>
      public string ClientName { get; set; }

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

      private ClientQueue _clientQueue;
      /// <summary>
      /// Client Queue
      /// </summary>
      public ClientQueue Queue
      {
         get { return _clientQueue; }
      }

      /// <summary>
      /// Current Index in List of returned UnitInfo and UnitLogLines
      /// </summary>
      public int CurrentUnitIndex
      {
         get
         {
            if (_clientQueue != null)
            {
               return _clientQueue.CurrentIndex;
            }

            // default Unit Index if only parsing logs
            return 1;
         }
      }

      /// <summary>
      /// Client Run Data for the Current Run
      /// </summary>
      public ClientRun2 CurrentClientRun
      {
         get { return _fahLog != null ? _fahLog.ClientRuns.PeekOrDefault() : null; }
      }

      /// <summary>
      /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
      /// </summary>
      public IList<LogLine> CurrentLogLines
      {
         get
         {
            if (_unitLogLines == null || _unitLogLines[CurrentUnitIndex] == null)
            {
               if (_fahLog == null)
               {
                  return new List<LogLine>();
               }
               var clientRun = CurrentClientRun;
               if (clientRun == null)
               {
                  return new List<LogLine>();
               }
               return clientRun.LogLines;
            }

            return _unitLogLines[CurrentUnitIndex];
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

      private ILogger _logger;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger ?? (_logger = NullLogger.Instance); }
         [CoverageExclude]
         set { _logger = value; }
      }

      #region Aggregation Logic

      /// <summary>
      /// Aggregate Data and return UnitInfo List
      /// </summary>
      public IList<UnitInfo> AggregateData()
      {
         _fahLog = FahLog.Read(File.ReadLines(FahLogFilePath), LogFileType.Legacy);

         // TODO: report errors that came back from log parsing
         //foreach (var s in _logInterpreterLegacy.LogLineParsingErrors)
         //{
         //   Logger.Debug(Constants.ClientNameFormat, ClientName, s);
         //}

         IList<UnitInfo> parsedUnits;
         // Decision Time: If Queue Read fails parse from logs only
         QueueData queueData = ReadQueueFile();
         if (queueData != null)
         {
            parsedUnits = GenerateUnitInfoDataFromQueue(_fahLog, queueData);
            _clientQueue = BuildClientQueue(queueData);
         }
         else
         {
            Logger.Warn(Constants.ClientNameFormat, ClientName,
               "Queue unavailable or failed read.  Parsing logs without queue.");

            parsedUnits = GenerateUnitInfoDataFromLogs(_fahLog);
            _clientQueue = null;
         }

         return parsedUnits;
      }

      private static ClientQueue BuildClientQueue(QueueData q)
      {
         Debug.Assert(q != null);

         var cq = Mapper.Map<QueueData, ClientQueue>(q);
         for (int i = 0; i < 10; i++)
         {
            cq.Add(i, Mapper.Map<QueueEntry, ClientQueueEntry>(q.GetQueueEntry((uint)i)));
         }

         return cq;
      }

      /// <summary>
      /// Read the queue.dat file
      /// </summary>
      private QueueData ReadQueueFile()
      {
         // Make sure the queue file exists first.  Would like to avoid the exception overhead.
         if (File.Exists(QueueFilePath))
         {
            // queue.dat is not required to get a reading
            // if something goes wrong just catch and log
            try
            {
               return QueueReader.ReadQueue(QueueFilePath);
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, Constants.ClientNameFormat, ex.Message);
            }
         }

         return null;
      }

      private IList<UnitInfo> GenerateUnitInfoDataFromLogs(FahLog fahLog)
      {
         var parsedUnits = new UnitInfo[2];
         _unitLogLines = new IList<LogLine>[2];

         var currentClientRun = GetCurrentClientRun(fahLog);
         var previousUnitRun = GetPreviousUnitRun(fahLog);
         if (previousUnitRun != null)
         {
            _unitLogLines[0] = previousUnitRun.LogLines;
            parsedUnits[0] = BuildUnitInfo(null, currentClientRun.Data, previousUnitRun, null);
         }

         bool matchOverride = false;
         var currentUnitRun = GetCurrentUnitRun(fahLog);
         if (currentUnitRun != null)
         {
            _unitLogLines[1] = currentUnitRun.LogLines;
         }
         else
         {
            matchOverride = true;
            _unitLogLines[1] = currentClientRun.LogLines;
         }
         parsedUnits[1] = BuildUnitInfo(null, currentClientRun.Data, currentUnitRun, GetUnitInfoLogData(), matchOverride);

         return parsedUnits;
      }

      private static ClientRun2 GetCurrentClientRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         return fahLog.ClientRuns.PeekOrDefault();
      }

      private static SlotRun GetCurrentSlotRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         var clientRun = GetCurrentClientRun(fahLog);
         return clientRun != null ? clientRun.SlotRuns[0] : null;
      }

      private static UnitRun GetPreviousUnitRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         var slotRun = GetCurrentSlotRun(fahLog);
         return slotRun != null && slotRun.UnitRuns.Count > 1 ? slotRun.UnitRuns.ElementAt(1) : null;
      }

      private static UnitRun GetCurrentUnitRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         var slotRun = GetCurrentSlotRun(fahLog);
         return slotRun != null ? slotRun.UnitRuns.PeekOrDefault() : null;
      }

      private UnitInfo[] GenerateUnitInfoDataFromQueue(FahLog fahLog, QueueData q)
      {
         Debug.Assert(q != null);

         var parsedUnits = new UnitInfo[10];
         _unitLogLines = new IList<LogLine>[10];

         var clientRun = GetCurrentClientRun(fahLog);
         for (int queueIndex = 0; queueIndex < parsedUnits.Length; queueIndex++)
         {
            var unitRun = GetUnitRunForQueueIndex(fahLog, queueIndex);
            // Get the Log Lines for this queue position from the reader
            _unitLogLines[queueIndex] = unitRun != null ? unitRun.LogLines : null;

            UnitInfoLogData unitInfoLogData = null;
            // On the Current Queue Index
            if (queueIndex == q.CurrentIndex)
            {
               // Get the UnitInfo Log Data
               unitInfoLogData = GetUnitInfoLogData();
            }

            parsedUnits[queueIndex] = BuildUnitInfo(q.GetQueueEntry((uint)queueIndex), clientRun.Data, unitRun, unitInfoLogData);
            if (parsedUnits[queueIndex] == null)
            {
               if (queueIndex == q.CurrentIndex)
               {
                  string message = String.Format(CultureInfo.CurrentCulture,
                     "Could not verify log section for current queue entry {0}. Trying to parse with most recent log section.", queueIndex);
                  Logger.Warn(Constants.ClientNameFormat, ClientName, message);

                  unitRun = GetCurrentUnitRun(fahLog);
                  // If got no Work Unit Log Lines based on Current Work Unit Log Lines
                  // then take the entire Current Client Run Log Lines - likely the run
                  // was short and never contained any Work Unit Data.
                  if (unitRun != null)
                  {
                     _unitLogLines[queueIndex] = unitRun.LogLines;
                  }
                  else
                  {
                     _unitLogLines[queueIndex] = clientRun.LogLines;
                  }

                  var slotRun = GetCurrentSlotRun(fahLog);
                  if (slotRun != null && slotRun.Data.Status == SlotStatus.GettingWorkPacket)
                  {
                     // Use either the current Work Unit log lines or current Client Run log lines
                     // as decided upon above... don't clear it here and show the user nothing - 10/9/10
                     //_unitLogLines[queueIndex] = null;
                     unitRun = null;
                     unitInfoLogData = new UnitInfoLogData();
                  }
                  parsedUnits[queueIndex] = BuildUnitInfo(q.GetQueueEntry((uint)queueIndex), clientRun.Data, unitRun, unitInfoLogData, true);
               }
               else
               {
                  // Just skip this unit and continue
                  string message = String.Format(CultureInfo.CurrentCulture,
                     "Could not find or verify log section for queue entry {0} (this is not a problem).", queueIndex);
                  Logger.Debug(Constants.ClientNameFormat, ClientName, message);
               }
            }
         }

         return parsedUnits;
      }

      private static UnitRun GetUnitRunForQueueIndex(FahLog fahLog, int queueIndex)
      {
         foreach (var clientRun in fahLog.ClientRuns)
         {
            var slotRun = clientRun.SlotRuns[0];
            var unitRun = slotRun.UnitRuns.FirstOrDefault(x => x.QueueIndex == queueIndex);
            if (unitRun != null)
            {
               return unitRun;
            }
         }
         return null;
      }

      private UnitInfoLogData GetUnitInfoLogData()
      {
         try
         {
            return UnitInfoLog.Read(UnitInfoLogFilePath);
         }
         catch (Exception ex)
         {
            Logger.WarnFormat(ex, Constants.ClientNameFormat, ClientName, ex.Message);
            return null;
         }
      }

      private UnitInfo BuildUnitInfo(QueueEntry queueEntry, ClientRun2Data clientRunData, UnitRun unitRun, UnitInfoLogData unitInfoLogData)
      {
         return BuildUnitInfo(queueEntry, clientRunData, unitRun, unitInfoLogData, false);
      }

      private UnitInfo BuildUnitInfo(QueueEntry queueEntry, ClientRun2Data clientRunData, UnitRun unitRun, UnitInfoLogData unitInfoLogData, bool matchOverride)
      {
         // queueEntry can be null
         Debug.Assert(clientRunData != null);
         // unitInfoLogData can be null

         UnitRunData unitRunData;
         if (unitRun == null)
         {
            if (matchOverride)
            {
               unitRunData = new UnitRunData();
            }
            else
            {
               return null;
            }
         }
         else
         {
            unitRunData = unitRun.Data;
         }
         var unit = new UnitInfo();
         unit.UnitStartTimeStamp = unitRunData.UnitStartTimeStamp ?? TimeSpan.MinValue;
         unit.FramesObserved = unitRunData.FramesObserved;
         unit.CoreVersion = unitRunData.CoreVersion;
         unit.UnitResult = unitRunData.WorkUnitResult;

         if (queueEntry != null)
         {
            PopulateUnitInfoFromQueueEntry(queueEntry, unit);
            SearchFahLogUnitDataProjects(unit, unitRunData);
            PopulateUnitInfoFromLogs(clientRunData, unitRunData, unitInfoLogData, unit);

            if (ProjectsMatch(unit, unitRunData) ||
                ProjectsMatch(unit, unitInfoLogData) ||
                matchOverride)
            {
               if (unitRun != null)
               {
                  // continue parsing the frame data
                  ParseFrameData(unitRun.LogLines, unit);
               }
            }
            else
            {
               return null;
            }
         }
         else
         {
            PopulateUnitInfoFromLogs(clientRunData, unitRunData, unitInfoLogData, unit);
            if (unitRun != null)
            {
               ParseFrameData(unitRun.LogLines, unit);
            }
         }

         return unit;
      }

      private static void SearchFahLogUnitDataProjects(UnitInfo unit, UnitRunData unitRunData)
      {
         Debug.Assert(unit != null);
         Debug.Assert(unitRunData != null);

         for (int i = 0; i < unitRunData.ProjectInfoList.Count; i++)
         {
            if (ProjectsMatch(unit, unitRunData.ProjectInfoList[i]))
            {
               unitRunData.ProjectInfoIndex = i;
            }
         }
      }

      private static bool ProjectsMatch(UnitInfo unit, IProjectInfo projectInfo)
      {
         Debug.Assert(unit != null);

         if (unit.ProjectIsUnknown() || projectInfo == null) return false;

         return (unit.ProjectID == projectInfo.ProjectID &&
                 unit.ProjectRun == projectInfo.ProjectRun &&
                 unit.ProjectClone == projectInfo.ProjectClone &&
                 unit.ProjectGen == projectInfo.ProjectGen);
      }

      #region Unit Population Methods

      private static void PopulateUnitInfoFromQueueEntry(QueueEntry entry, UnitInfo unit)
      {
         Debug.Assert(entry != null);
         Debug.Assert(unit != null);

         // convert to enum
         var queueEntryStatus = (QueueEntryStatus)entry.EntryStatus;

         if ((queueEntryStatus.Equals(QueueEntryStatus.Unknown) ||
              queueEntryStatus.Equals(QueueEntryStatus.Empty) ||
              queueEntryStatus.Equals(QueueEntryStatus.Garbage) ||
              queueEntryStatus.Equals(QueueEntryStatus.Abandonded)) == false)
         {
            /* Tag (Could be read here or through the unitinfo.txt file) */
            unit.ProteinTag = entry.WorkUnitTag;

            /* DownloadTime (Could be read here or through the unitinfo.txt file) */
            unit.DownloadTime = entry.BeginTimeUtc;

            /* DueTime (Could be read here or through the unitinfo.txt file) */
            unit.DueTime = entry.DueTimeUtc;

            /* FinishedTime */
            if (queueEntryStatus.Equals(QueueEntryStatus.Finished) ||
                queueEntryStatus.Equals(QueueEntryStatus.ReadyForUpload))
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
            unit.CoreID = entry.CoreNumberHex.ToUpperInvariant();
         }
      }

      private static void PopulateUnitInfoFromLogs(ClientRun2Data clientRunData,
         UnitRunData unitRunData, UnitInfoLogData unitInfoLogData, UnitInfo unit)
      {
         Debug.Assert(clientRunData != null);
         Debug.Assert(unitRunData != null);
         // unitInfoLogData can be null
         Debug.Assert(unit != null);

         /* Project (R/C/G) (Could have already been read through Queue) */
         if (unit.ProjectIsUnknown())
         {
            unit.ProjectID = unitRunData.ProjectID;
            unit.ProjectRun = unitRunData.ProjectRun;
            unit.ProjectClone = unitRunData.ProjectClone;
            unit.ProjectGen = unitRunData.ProjectGen;
         }

         if (unitRunData.Threads > 1)
         {
            unit.SlotType = SlotType.CPU;
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
            if (unit.DownloadTime.IsUnknown())
            {
               unit.DownloadTime = unitInfoLogData.DownloadTime;
            }

            /* DueTime (Could have already been read through Queue) */
            if (unit.DueTime.IsUnknown())
            {
               unit.DueTime = unitInfoLogData.DueTime;
            }

            /* FinishedTime (Not available in unitinfo log) */

            /* Project (R/C/G) (Could have already been read through Queue) */
            if (unit.ProjectIsUnknown())
            {
               unit.ProjectID = unitInfoLogData.ProjectID;
               unit.ProjectRun = unitInfoLogData.ProjectRun;
               unit.ProjectClone = unitInfoLogData.ProjectClone;
               unit.ProjectGen = unitInfoLogData.ProjectGen;
            }
         }

         /* FoldingID and Team from Last Client Run (Could have already been read through Queue) */
         if (unit.FoldingID.Equals(Constants.DefaultFoldingID) &&
            !String.IsNullOrEmpty(clientRunData.FoldingID))
         {
            unit.FoldingID = clientRunData.FoldingID;
         }
         if (unit.Team == Constants.DefaultTeam)
         {
            unit.Team = clientRunData.Team;
         }

         // Possibly check the currentClientRun from the log file.
         // The queue will have the ID and Team that was set when the work unit was received.
         //if (unit.FoldingID.Equals(Default.FoldingID) ||
         //   !unit.FoldingID.Equals(currentClientRun.FoldingID))
         //{
         //   unit.FoldingID = currentClientRun.FoldingID;
         //}
         //if (unit.Team == Default.Team ||
         //    unit.Team != currentClientRun.Team)
         //{
         //   unit.Team = currentClientRun.Team;
         //}
      }

      private static void ParseFrameData(IEnumerable<LogLine> logLines, UnitInfo unit)
      {
         Debug.Assert(logLines != null);
         Debug.Assert(unit != null);

         foreach (var logLine in logLines.Where(x => x.LineType == LogLineType.WorkUnitFrame))
         {
            // Check for FrameData
            var frame = logLine.LineData as UnitFrame;
            if (frame == null)
            {
               // If not found, clear the LineType and get out
               logLine.LineType = LogLineType.Unknown;
               continue;
            }

            unit.SetUnitFrame(frame);
         }
      }

      #endregion

      #endregion
   }
}
