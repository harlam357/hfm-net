/*
 * HFM.NET - Legacy Client Class
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
using System.Globalization;
using System.IO;
using System.Linq;

using HFM.Core.DataTypes;
using HFM.Log;
using HFM.Queue;

namespace HFM.Core
{
   public interface ILegacyClientFactory
   {
      LegacyClient Create();

      void Release(LegacyClient legacyClient);
   }

   public sealed class LegacyClient : Client
   {
      #region Injection Properties

      public IStatusLogic StatusLogic { get; set; }

      public IDataRetriever DataRetriever { get; set; }

      #endregion

      #region Properties

      private ClientSettings _settings;
      private SlotModel _slotModel;

      public override ClientSettings Settings
      {
         get { return _settings; }
         set
         {
            Debug.Assert(value.IsLegacy());

            _settings = value;
            if (_slotModel == null)
            {
               // default slot model
               _slotModel = new SlotModel { Prefs = Prefs };
               // set settings
               _slotModel.Settings = _settings;
            }
            else
            {
               // just change settings
               _slotModel.Settings = _settings;
            }
         }
      }

      public override IEnumerable<SlotModel> Slots
      {
         get { return new[] { _slotModel }; }
      }

      #endregion

      #region Retrieval Methods

      protected override void RetrieveInternal()
      {
         try
         {
            DataRetriever.Execute(Settings);
            if (!AbortFlag)
            {
               // Process the retrieved logs
               Process();
            }
            else
            {
               _slotModel.Status = SlotStatus.Offline;
               //Logger.Info(Constants.ClientNameFormat, Settings.Name, "Retrieval Aborted...");
            }
         }
         catch (Exception ex)
         {
            _slotModel.Status = SlotStatus.Offline;
            Logger.ErrorFormat(ex, Constants.ClientNameFormat, Settings.Name, ex.Message);
         }
         finally
         {
            if (!AbortFlag) OnRetrievalFinished(EventArgs.Empty);
         }
      }

      #endregion

      #region Queue and Log Processing Functions

      /// <summary>
      /// Process the cached log files that exist on this machine
      /// </summary>
      private void Process()
      {
         DateTime start = Instrumentation.ExecStart;

         // Set successful Last Retrieval Time
         LastRetrievalTime = DateTime.Now;
         // Re-Init Slot Level Members Before Processing
         _slotModel.Initialize();

         #region Setup Aggregator

         var dataAggregator = new LegacyDataAggregator { Logger = Logger };
         dataAggregator.ClientName = Settings.Name;
         string queueFilePath = Path.Combine(Prefs.CacheDirectory, Settings.CachedQueueFileName());
         string fahLogFilePath = Path.Combine(Prefs.CacheDirectory, Settings.CachedFahLogFileName());
         string unitInfoLogFilePath = Path.Combine(Prefs.CacheDirectory, Settings.CachedUnitInfoFileName());

         #endregion

         #region Run the Aggregator

         var queue = ReadQueueFile(queueFilePath);
         var fahLog = FahLog.Read(File.ReadLines(fahLogFilePath), FahLogType.Legacy);
         var unitInfo = ReadUnitInfoFile(unitInfoLogFilePath);

         var result = dataAggregator.AggregateData(fahLog, queue, unitInfo);
         // Issue 126 - Use the Folding ID, Team, User ID, and Machine ID from the FAHlog data.
         // Use the Current Queue Entry as a backup data source.
         PopulateRunLevelData(result, _slotModel);
         if (result.Queue != null)
         {
            PopulateRunLevelData(result.Queue.Current, _slotModel);
         }

         _slotModel.Queue = result.Queue;
         _slotModel.CurrentLogLines = result.CurrentLogLines;
         _slotModel.UnitLogLines = result.UnitInfos.OrderBy(x => x.Key).Select(x => x.Value != null ? x.Value.LogLines : null).ToArray();

         #endregion

         var parsedUnits = new UnitInfoModel[result.UnitInfos.Count];
         for (int i = 0; i < result.UnitInfos.Count; i++)
         {
            if (result.UnitInfos[i] != null)
            {
               parsedUnits[i] = BuildUnitInfoLogic(result.UnitInfos[i], true);
            }
         }

         // *** THIS HAS TO BE DONE BEFORE UPDATING SlotModel.UnitInfoLogic ***
         UpdateBenchmarkData(_slotModel.UnitInfoModel, parsedUnits, result.CurrentUnitIndex);

         // Update the UnitInfoLogic if we have a Status
         if (result.Status != SlotStatus.Unknown)
         {
            _slotModel.UnitInfoModel = parsedUnits[result.CurrentUnitIndex];
         }

         HandleReturnedStatus(result.Status, _slotModel);

         _slotModel.UnitInfoModel.ShowPPDTrace(Logger, _slotModel.Name, _slotModel.Status,
            Prefs.Get<PpdCalculationType>(Preference.PpdCalculation),
            Prefs.Get<BonusCalculationType>(Preference.BonusCalculation));

         string statusMessage = String.Format(CultureInfo.CurrentCulture, "Client Status: {0}", _slotModel.Status);
         Logger.InfoFormat(Constants.ClientNameFormat, _slotModel.Name, statusMessage);

         string message = String.Format(CultureInfo.CurrentCulture, "Retrieval finished: {0}", Instrumentation.GetExecTime(start));
         Logger.InfoFormat(Constants.ClientNameFormat, Settings.Name, message);
      }

      private QueueData ReadQueueFile(string path)
      {
         // Make sure the queue file exists first.  Would like to avoid the exception overhead.
         if (File.Exists(path))
         {
            // queue.dat is not required to get a reading
            // if something goes wrong just catch and log
            try
            {
               return QueueReader.ReadQueue(path);
            }
            catch (Exception ex)
            {
               Logger.WarnFormat(ex, Constants.ClientNameFormat, Settings.Name, ex.Message);
            }
         }
         return null;
      }

      private UnitInfoLogData ReadUnitInfoFile(string path)
      {
         if (File.Exists(path))
         {
            try
            {
               return UnitInfoLog.Read(path);
            }
            catch (Exception ex)
            {
               Logger.WarnFormat(ex, Constants.ClientNameFormat, Settings.Name, ex.Message);
            }
         }
         return null;
      }

      private UnitInfoModel BuildUnitInfoLogic(UnitInfo unitInfo, bool updateUnitInfo)
      {
         Debug.Assert(unitInfo != null);

         Protein protein = ProteinService.Get(unitInfo.ProjectID, true) ?? new Protein();

         if (updateUnitInfo)
         {
            // update the data
            unitInfo.UnitRetrievalTime = LastRetrievalTime;
            unitInfo.OwningClientName = Settings.Name;
            unitInfo.OwningClientPath = Settings.DataPath();
            if (unitInfo.SlotType == SlotType.Unknown)
            {
               unitInfo.SlotType = protein.Core.ToSlotType();
               if (unitInfo.SlotType == SlotType.Unknown)
               {
                  unitInfo.SlotType = unitInfo.CoreID.ToSlotType();
               }
            }
         }
         // build unit info logic
         var unitInfoLogic = new UnitInfoModel(BenchmarkService);
         unitInfoLogic.CurrentProtein = protein;
         unitInfoLogic.UnitInfoData = unitInfo;
         return unitInfoLogic;
      }

      private void PopulateRunLevelData(DataAggregatorResult result, SlotModel slotModel)
      {
         slotModel.Arguments = result.Arguments;
         slotModel.ClientVersion = result.ClientVersion;

         slotModel.UserId = result.UserID;
         slotModel.MachineId = result.MachineID;

         //slotModel.TotalRunCompletedUnits = run.CompletedUnits;
         //slotModel.TotalRunFailedUnits = run.FailedUnits;
         //slotModel.TotalCompletedUnits = run.TotalCompletedUnits;
         if (UnitInfoDatabase.Connected)
         {
            slotModel.TotalRunCompletedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Completed, result.StartTime);
            slotModel.TotalCompletedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Completed);
            slotModel.TotalRunFailedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Failed, result.StartTime);
            slotModel.TotalFailedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Failed);
         }
      }

      private static void PopulateRunLevelData(QueueUnitItem queueUnitItem, SlotModel slotModel)
      {
         if (slotModel.UserId == Constants.DefaultUserID)
         {
            slotModel.UserId = queueUnitItem.UserID;
         }
         if (slotModel.MachineId == Constants.DefaultMachineID)
         {
            slotModel.MachineId = queueUnitItem.MachineID;
         }
      }

      internal void UpdateBenchmarkData(UnitInfoModel currentUnitInfo, UnitInfoModel[] parsedUnits, int currentUnitIndex)
      {
         var foundCurrent = false;
         var processUpdates = false;

         foreach (int index in UnitIndexIterator(currentUnitIndex, parsedUnits.Length))
         {
            var unitInfoModel = parsedUnits[index];

            // If Current has not been found, check the nextUnitIndex
            // or try to match the Current Project and Raw Download Time
            if (unitInfoModel != null && processUpdates == false && (index == currentUnitIndex || currentUnitInfo.UnitInfoData.IsSameUnitAs(unitInfoModel.UnitInfoData)))
            {
               foundCurrent = true;
               processUpdates = true;
            }

            if (processUpdates)
            {
               int previousFramesComplete = 0;
               if (foundCurrent)
               {
                  // current frame has already been recorded, increment to the next frame
                  previousFramesComplete = currentUnitInfo.FramesComplete + 1;
                  foundCurrent = false;
               }

               // Even though the current UnitInfoLogic has been found in the parsed UnitInfoLogic array doesn't
               // mean that all entries in the array will be present.  See TestFiles\SMP_12\FAHlog.txt.
               if (unitInfoModel != null)
               {
                  // Update benchmarks
                  BenchmarkService.UpdateData(unitInfoModel.UnitInfoData, previousFramesComplete, unitInfoModel.FramesComplete);
                  // Update history database
                  UpdateUnitInfoDatabase(unitInfoModel);
               }
            }
         }
      }

      private static IEnumerable<int> UnitIndexIterator(int currentUnitIndex, int numberOfUnits)
      {
         int i;
         for (i = GetNextIndex(currentUnitIndex, numberOfUnits); i != currentUnitIndex; i = GetNextIndex(i, numberOfUnits))
         {
            yield return i;
         }
         yield return i;
      }

      private static int GetNextIndex(int index, int numberOfUnits)
      {
         return index == numberOfUnits - 1 ? 0 : index + 1;
      }

      #endregion

      #region Status Handling and Determination

      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determines what values to feed the DetermineStatus routine.
      /// </summary>
      private void HandleReturnedStatus(SlotStatus returnedStatus, SlotModel slot)
      {
         var statusData = new StatusData
                          {
                             ClientName = Settings.Name,
                             SlotType = slot.UnitInfoModel.UnitInfoData.SlotType,
                             UnitRetrievalTime = slot.UnitInfoModel.UnitInfoData.UnitRetrievalTime,
                             UtcOffsetIsZero = Settings.UtcOffsetIsZero,
                             UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now),
                             ClientTimeOffset = Settings.ClientTimeOffset,
                             TimeOfLastUnitStart = slot.TimeOfLastUnitStart,
                             TimeOfLastFrameProgress = slot.TimeOfLastFrameProgress,
                             CurrentStatus = slot.Status,
                             ReturnedStatus = returnedStatus,
                             FrameTime = slot.UnitInfoModel.GetRawTime(Prefs.Get<PpdCalculationType>(Preference.PpdCalculation)),
                             BenchmarkAverageFrameTime = GetBenchmarkAverageFrameTimeOrDefault(slot.UnitInfo),
                             TimeOfLastFrame = slot.UnitInfoModel.UnitInfoData.CurrentFrame == null
                                                  ? TimeSpan.Zero
                                                  : slot.UnitInfoModel.UnitInfoData.CurrentFrame.TimeOfFrame,
                             UnitStartTimeStamp = slot.UnitInfoModel.UnitInfoData.UnitStartTimeStamp,
                             AllowRunningAsync = Prefs.Get<bool>(Preference.AllowRunningAsync)
                          };

         SlotStatus computedStatus = StatusLogic.HandleStatusData(statusData);

         // If the returned status is EuePause and current status is not
         if (computedStatus.Equals(SlotStatus.EuePause) && statusData.CurrentStatus.Equals(SlotStatus.EuePause) == false)
         {
            if (Prefs.Get<bool>(Preference.EmailReportingEnabled) &&
                Prefs.Get<bool>(Preference.ReportEuePause))
            {
               SendEuePauseEmail(statusData.ClientName);
            }
         }

         // If the returned status is Hung and current status is not
         if (computedStatus.Equals(SlotStatus.Hung) && statusData.CurrentStatus.Equals(SlotStatus.Hung) == false)
         {
            if (Prefs.Get<bool>(Preference.EmailReportingEnabled) &&
                Prefs.Get<bool>(Preference.ReportHung))
            {
               SendHungEmail(statusData.ClientName);
            }
         }

         slot.Status = computedStatus;
      }

      private TimeSpan GetBenchmarkAverageFrameTimeOrDefault(UnitInfo unitInfo)
      {
         var benchmark = BenchmarkService.GetBenchmark(unitInfo);
         return benchmark != null ? benchmark.AverageFrameTime : TimeSpan.Zero;
      }

      /// <summary>
      /// Send EuePause Status Email
      /// </summary>
      private void SendEuePauseEmail(string name)
      {
         string messageBody = String.Format("HFM.NET detected that Client '{0}' has entered a 24 hour EUE Pause state.", name);
         try
         {
            NetworkOps.SendEmail(Prefs.Get<bool>(Preference.EmailReportingServerSecure),
                                 Prefs.Get<string>(Preference.EmailReportingFromAddress),
                                 Prefs.Get<string>(Preference.EmailReportingToAddress),
                                 "HFM.NET - Client EUE Pause Error", messageBody,
                                 Prefs.Get<string>(Preference.EmailReportingServerAddress),
                                 Prefs.Get<int>(Preference.EmailReportingServerPort),
                                 Prefs.Get<string>(Preference.EmailReportingServerUsername),
                                 Prefs.Get<string>(Preference.EmailReportingServerPassword));
         }
         catch (Exception ex)
         {
            Logger.ErrorFormat(ex, "{0}", ex.Message);
         }
      }

      /// <summary>
      /// Send Hung Status Email
      /// </summary>
      private void SendHungEmail(string name)
      {
         string messageBody = String.Format("HFM.NET detected that Client '{0}' has entered a Hung state.", name);
         try
         {
            NetworkOps.SendEmail(Prefs.Get<bool>(Preference.EmailReportingServerSecure),
                                 Prefs.Get<string>(Preference.EmailReportingFromAddress),
                                 Prefs.Get<string>(Preference.EmailReportingToAddress),
                                 "HFM.NET - Client Hung Error", messageBody,
                                 Prefs.Get<string>(Preference.EmailReportingServerAddress),
                                 Prefs.Get<int>(Preference.EmailReportingServerPort),
                                 Prefs.Get<string>(Preference.EmailReportingServerUsername),
                                 Prefs.Get<string>(Preference.EmailReportingServerPassword));
         }
         catch (Exception ex)
         {
            Logger.ErrorFormat(ex, "{0}", ex.Message);
         }
      }

      #endregion
   }
}
