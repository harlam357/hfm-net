/*
 * HFM.NET - Fah Client Data Aggregator Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Linq;

using Castle.Core.Logging;

using HFM.Client.DataTypes;
using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   public class FahClientDataAggregator : IFahClientDataAggregator
   {
      private LogInterpreter _logInterpreter;

      /// <summary>
      /// Client name.
      /// </summary>
      public string ClientName { get; set; }

      private ClientQueue _clientQueue;
      /// <summary>
      /// Client Queue
      /// </summary>
      public ClientQueue Queue
      {
         get { return _clientQueue; }
      }

      private int _currentUnitIndex;
      /// <summary>
      /// Current Index in List of returned UnitInfo
      /// </summary>
      public int CurrentUnitIndex
      {
         get { return _currentUnitIndex; }
      }

      private ClientRun _currentClientRun;
      /// <summary>
      /// Client Run Data for the Current Run
      /// </summary>
      public ClientRun CurrentClientRun
      {
         get { return _currentClientRun; }
      }

      private IList<LogLine> _currentLogLines;
      /// <summary>
      /// Current Log Lines
      /// </summary>
      public IList<LogLine> CurrentLogLines
      {
         get
         {
            if (_unitLogLines.ContainsKey(_currentUnitIndex))
            {
               return _unitLogLines[_currentUnitIndex];
            }
            
            if (_currentLogLines == null)
            {
               return new List<LogLine>();
            }

            return _currentLogLines;
         }
      }

      private IDictionary<int, IList<LogLine>> _unitLogLines;
      /// <summary>
      /// Array of LogLine Lists
      /// </summary>
      public IDictionary<int, IList<LogLine>> UnitLogLines
      {
         get { return _unitLogLines; }
      }

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      #region Aggregation Logic

      /// <summary>
      /// Aggregate Data and return UnitInfo List
      /// </summary>
      public IDictionary<int, UnitInfo> AggregateData(IList<LogLine> logLines, UnitCollection unitCollection, Options options, 
                                                      SlotOptions slotOptions, UnitInfo currentUnifInfo, int slotId)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");
         if (unitCollection == null) throw new ArgumentNullException("unitCollection");
         if (options == null) throw new ArgumentNullException("options");
         if (slotOptions == null) throw new ArgumentNullException("slotOptions");
         if (currentUnifInfo == null) throw new ArgumentNullException("currentUnifInfo");

         _currentUnitIndex = -1;
         // only take up to the last MaxDisplayableLogLines
         _currentLogLines = logLines.Skip(Math.Max(0, logLines.Count - Constants.MaxDisplayableLogLines)).ToList();
         _logInterpreter = new LogInterpreter(logLines, LogReader.GetClientRuns(logLines, LogFileType.FahClient));
         _currentClientRun = _logInterpreter.CurrentClientRun;

         // report errors that came back from log parsing
         foreach (var s in _logInterpreter.LogLineParsingErrors)
         {
            _logger.Debug(Constants.ClientNameFormat, ClientName, s);
         }

         IDictionary<int, UnitInfo> parsedUnits = GenerateUnitInfoDataFromQueue(unitCollection, options, slotOptions, currentUnifInfo, slotId);
         _clientQueue = null; // BuildClientQueue(qData);
         _logInterpreter = null;

         return parsedUnits;
      }

      //private static ClientQueue BuildClientQueue(QueueData q)
      //{
      //   Debug.Assert(q != null);
      //
      //   var cq = Mapper.Map<QueueData, ClientQueue>(q);
      //   for (int i = 0; i < 10; i++)
      //   {
      //      Mapper.Map(q.GetQueueEntry((uint)i), cq.GetQueueEntry(i));
      //   }
      //
      //   return cq;
      //}

      private IDictionary<int, UnitInfo> GenerateUnitInfoDataFromQueue(IEnumerable<Unit> unitCollection, Options options, 
                                                                       SlotOptions slotOptions, UnitInfo currentUnitInfo, int slotId)
      {
         Debug.Assert(unitCollection != null);
         Debug.Assert(options != null);
         Debug.Assert(slotOptions != null);
         Debug.Assert(currentUnitInfo != null);

         var parsedUnits = new Dictionary<int, UnitInfo>();
         _unitLogLines = new Dictionary<int, IList<LogLine>>();

         bool foundCurrentUnitInfo = false;

         foreach (var unit in unitCollection)
         {
            if (unit.Slot != slotId)
            {
               // does not match requested slot
               continue;
            }

            var projectInfo = new ProjectInfo { ProjectID = unit.Project, ProjectRun = unit.Run, 
                                                ProjectClone = unit.Clone, ProjectGen = unit.Gen };
            if (projectInfo.EqualsProject(currentUnitInfo) &&
                unit.AssignedDateTime.GetValueOrDefault().Equals(currentUnitInfo.DownloadTime))
            {
               foundCurrentUnitInfo = true;
            }

            // Get the Log Lines for this queue position from the reader
            var logLines = _logInterpreter.GetLogLinesForQueueIndex(unit.Id, projectInfo);
            if (logLines == null)
            {
               // no log lines matching this unit
               continue;
            }

            // Get the FAH Log Data from the Log Lines
            FahLogUnitData fahLogUnitData = LogReader.GetFahLogDataFromLogLines(logLines);

            UnitInfo unitInfo = BuildUnitInfo(unit, options, slotOptions, fahLogUnitData);
            if (unitInfo != null)
            {
               parsedUnits.Add(unit.Id, unitInfo);
               _unitLogLines.Add(unit.Id, logLines);
               if (unit.StateEnum.Equals(FahSlotStatus.Running) ||
                   unit.StateEnum.Equals(FahSlotStatus.Finishing))
               {
                  _currentUnitIndex = unit.Id;
               }
            }
         }

         // if the current unit has already left the UnitCollection then find the log section and update here
         if (!foundCurrentUnitInfo)
         {
            // Get the Log Lines for this queue position from the reader
            var logLines = _logInterpreter.GetLogLinesForQueueIndex(currentUnitInfo.QueueIndex, currentUnitInfo);
            if (logLines != null)
            {
               // Get the FAH Log Data from the Log Lines
               FahLogUnitData fahLogUnitData = LogReader.GetFahLogDataFromLogLines(logLines);

               UpdateUnitInfo(currentUnitInfo, fahLogUnitData);
               parsedUnits.Add(currentUnitInfo.QueueIndex, currentUnitInfo);
               _unitLogLines.Add(currentUnitInfo.QueueIndex, logLines);
            }
         }

         return parsedUnits;
      }

      private static UnitInfo BuildUnitInfo(Unit queueEntry, Options options, SlotOptions slotOptions, FahLogUnitData fahLogUnitData)
      {
         Debug.Assert(queueEntry != null);
         Debug.Assert(options != null);
         Debug.Assert(slotOptions != null);
         Debug.Assert(fahLogUnitData != null);

         var unit = new UnitInfo();
         unit.QueueIndex = queueEntry.Id;
         unit.UnitStartTimeStamp = fahLogUnitData.UnitStartTimeStamp;
         unit.FramesObserved = fahLogUnitData.FramesObserved;
         unit.CoreVersion = fahLogUnitData.CoreVersion;
         unit.UnitResult = fahLogUnitData.UnitResult;
         // there is no finished time available from the client API
         // since the unit history database won't write the same
         // result twice, the first time this hits use the local UTC
         // value for the finished time... not as good as what was
         // available with v6.
         if (unit.UnitResult.Equals(WorkUnitResult.FinishedUnit))
         {
            unit.FinishedTime = DateTime.UtcNow;
         }

         PopulateUnitInfoFromQueueEntry(queueEntry, options, slotOptions, unit);
         // parse the frame data
         ParseFrameData(fahLogUnitData.FrameDataList, unit);

         return unit;
      }

      private static void UpdateUnitInfo(UnitInfo unit, FahLogUnitData fahLogUnitData)
      {
         Debug.Assert(unit != null);
         Debug.Assert(fahLogUnitData != null);

         unit.UnitStartTimeStamp = fahLogUnitData.UnitStartTimeStamp;
         unit.FramesObserved = fahLogUnitData.FramesObserved;
         unit.CoreVersion = fahLogUnitData.CoreVersion;
         unit.UnitResult = fahLogUnitData.UnitResult;
         // there is no finished time available from the client API
         // since the unit history database won't write the same
         // result twice, the first time this hits use the local UTC
         // value for the finished time... not as good as what was
         // available with v6.
         if (unit.UnitResult.Equals(WorkUnitResult.FinishedUnit))
         {
            unit.FinishedTime = DateTime.UtcNow;
         }

         // parse the frame data
         ParseFrameData(fahLogUnitData.FrameDataList, unit);
      }

      #region Unit Population Methods

      private static void PopulateUnitInfoFromQueueEntry(Unit entry, Options options, SlotOptions slotOptions, UnitInfo unit)
      {
         Debug.Assert(entry != null);
         Debug.Assert(options != null);
         Debug.Assert(slotOptions != null);
         Debug.Assert(unit != null);

         /* DownloadTime (AssignedDateTime from HFM.Client API) */
         unit.DownloadTime = entry.AssignedDateTime.GetValueOrDefault();

         /* DueTime (TimeoutDateTime from HFM.Client API) */
         unit.DueTime = entry.TimeoutDateTime.GetValueOrDefault();

         /* FinishedTime */
         //if (queueEntryStatus.Equals(QueueEntryStatus.Finished) ||
         //    queueEntryStatus.Equals(QueueEntryStatus.ReadyForUpload))
         //{
         //   unit.FinishedTime = entry.EndTimeUtc;
         //}

         /* Project (R/C/G) */
         unit.ProjectID = entry.Project;
         unit.ProjectRun = entry.Run;
         unit.ProjectClone = entry.Clone;
         unit.ProjectGen = entry.Gen;

         /* FoldingID and Team from Queue Entry */
         unit.FoldingID = options.User ?? Default.FoldingID;
         unit.Team = options.Team ?? Default.Team;
         unit.SlotType = (SlotType)slotOptions.FahClientSubTypeEnum;

         /* Core ID */
         unit.CoreID = entry.Core.Replace("0x", String.Empty).ToUpperInvariant();
      }

      private static void ParseFrameData(IEnumerable<LogLine> frameData, UnitInfo unit)
      {
         Debug.Assert(frameData != null);
         Debug.Assert(unit != null);

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
