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
         get { return _currentLogLines; }
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
      public IList<UnitInfo> AggregateData(IList<LogLine> logLines, UnitCollection unitCollection, Options options, SlotOptions slotOptions, int slotId)
      {
         _currentUnitIndex = -1;
         _currentLogLines = logLines;
         _logInterpreter = new LogInterpreter(_currentLogLines, LogReader.GetClientRuns(_currentLogLines, LogFileType.FahClient));
         _currentClientRun = _logInterpreter.CurrentClientRun;

         // report errors that came back from log parsing
         foreach (var s in _logInterpreter.LogLineParsingErrors)
         {
            _logger.Debug(Constants.ClientNameFormat, ClientName, s);
         }

         IList<UnitInfo> parsedUnits = GenerateUnitInfoDataFromQueue(unitCollection, options, slotOptions, slotId);
         _clientQueue = null; // BuildClientQueue(qData);
         _logInterpreter = null;

         return parsedUnits;
      }

      //private static ClientQueue BuildClientQueue(QueueData q)
      //{
      //   var cq = Mapper.Map<QueueData, ClientQueue>(q);
      //   for (int i = 0; i < 10; i++)
      //   {
      //      Mapper.Map(q.GetQueueEntry((uint)i), cq.GetQueueEntry(i));
      //   }
      //
      //   return cq;
      //}

      private IList<UnitInfo> GenerateUnitInfoDataFromQueue(IEnumerable<Unit> unitCollection, Options options, SlotOptions slotOptions, int slotId)
      {
         var parsedUnits = new List<UnitInfo>();

         foreach (var unit in unitCollection)
         {
            if (unit.Slot != slotId)
            {
               // does not match requested slot
               continue;
            }

            // Get the Log Lines for this queue position from the reader
            var logLines = _logInterpreter.GetLogLinesForQueueIndex(unit.Id, 
               new ProjectInfo { ProjectID = unit.Project, ProjectRun = unit.Run, 
                                 ProjectClone = unit.Clone, ProjectGen = unit.Gen });

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
               parsedUnits.Add(unitInfo);
               if (unit.StateEnum.Equals(FahSlotStatus.Running))
               {
                  _currentUnitIndex = parsedUnits.Count - 1;
               }
            }
         }

         return parsedUnits.AsReadOnly();
      }

      private static UnitInfo BuildUnitInfo(Unit queueEntry, Options options, SlotOptions slotOptions, FahLogUnitData fahLogUnitData)
      {
         Debug.Assert(queueEntry != null);
         Debug.Assert(fahLogUnitData != null);

         var unit = new UnitInfo();
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

      #region Unit Population Methods

      private static void PopulateUnitInfoFromQueueEntry(Unit entry, Options options, SlotOptions slotOptions, UnitInfo unit)
      {
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
         unit.FoldingID = options.User;
         unit.Team = options.Team.GetValueOrDefault();
         unit.SlotType = (SlotType)slotOptions.FahClientSubTypeEnum;

         /* Core ID */
         unit.CoreID = entry.Core.Replace("0x", String.Empty).ToUpperInvariant();
      }

      private static void ParseFrameData(IEnumerable<LogLine> frameData, UnitInfo unit)
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
