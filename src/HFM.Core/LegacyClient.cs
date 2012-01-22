/*
 * HFM.NET - Legacy Client Class
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
using System.Globalization;
using System.IO;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public sealed class LegacyClient : Client
   {
      #region Injection Properties

      public IStatusLogic StatusLogic { get; set; }

      public IDataRetriever DataRetriever { get; set; }

      public ILegacyDataAggregator DataAggregator { get; set; }

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
               // restore unit info if available
               RestoreUnitInfo();
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

      private void RestoreUnitInfo()
      {
         if (UnitInfoCollection == null) return;

         foreach (var unitInfo in UnitInfoCollection)
         {
            if (_slotModel.Owns(unitInfo))
            {
               _slotModel.UnitInfoLogic = BuildUnitInfoLogic(unitInfo, false);
               string message = String.Format(CultureInfo.CurrentCulture, "Restored unit: {0}", unitInfo.ProjectRunCloneGen());
               Logger.Info(Constants.ClientNameFormat, _settings.Name, message);
               break;
            }
         }
      }

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

         DataAggregator.ClientName = Settings.Name;
         DataAggregator.QueueFilePath = Path.Combine(Prefs.CacheDirectory, Settings.CachedQueueFileName());
         DataAggregator.FahLogFilePath = Path.Combine(Prefs.CacheDirectory, Settings.CachedFahLogFileName());
         DataAggregator.UnitInfoLogFilePath = Path.Combine(Prefs.CacheDirectory, Settings.CachedUnitInfoFileName()); 
         
         #endregion
         
         #region Run the Aggregator
         
         IList<UnitInfo> units = DataAggregator.AggregateData();
         // Issue 126 - Use the Folding ID, Team, User ID, and Machine ID from the FAHlog data.
         // Use the Current Queue Entry as a backup data source.
         PopulateRunLevelData(DataAggregator.CurrentClientRun, _slotModel);
         if (DataAggregator.Queue != null)
         {
            PopulateRunLevelData(DataAggregator.Queue.CurrentQueueEntry, _slotModel);
         }

         _slotModel.Queue = DataAggregator.Queue;
         _slotModel.CurrentLogLines = DataAggregator.CurrentLogLines;
         _slotModel.UnitLogLines = DataAggregator.UnitLogLines;
         
         #endregion
         
         var parsedUnits = new UnitInfoLogic[units.Count];
         for (int i = 0; i < units.Count; i++)
         {
            if (units[i] != null)
            {
               parsedUnits[i] = BuildUnitInfoLogic(units[i], true);
            }
         }

         // *** THIS HAS TO BE DONE BEFORE UPDATING SlotModel.UnitInfoLogic ***
         UpdateBenchmarkData(_slotModel.UnitInfoLogic, parsedUnits, DataAggregator.CurrentUnitIndex);

         // Update the UnitInfoLogic if we have a Status
         SlotStatus currentWorkUnitStatus = DataAggregator.CurrentClientRun.Status;
         if (!currentWorkUnitStatus.Equals(SlotStatus.Unknown))
         {
            _slotModel.UnitInfoLogic = parsedUnits[DataAggregator.CurrentUnitIndex];
         }

         HandleReturnedStatus(currentWorkUnitStatus, _slotModel);
         
         _slotModel.UnitInfoLogic.ShowPPDTrace(Logger, _slotModel.Status, 
            Prefs.Get<PpdCalculationType>(Preference.PpdCalculation),
            Prefs.Get<bool>(Preference.CalculateBonus));

         string statusMessage = String.Format(CultureInfo.CurrentCulture, "Client Status: {0}", _slotModel.Status);
         Logger.Info(Constants.ClientNameFormat, _slotModel.Name, statusMessage);

         string message = String.Format(CultureInfo.CurrentCulture, "Retrieval finished in {0}", Instrumentation.GetExecTime(start));
         Logger.Info(Constants.ClientNameFormat, Settings.Name, message);
      }

      private UnitInfoLogic BuildUnitInfoLogic(UnitInfo unitInfo, bool updateUnitInfo)
      {
         Debug.Assert(unitInfo != null);

         Protein protein = ProteinDictionary.GetProteinOrDownload(unitInfo.ProjectID);

         if (updateUnitInfo)
         {
            // update the data
            unitInfo.UnitRetrievalTime = LastRetrievalTime;
            unitInfo.OwningSlotName = Settings.Name;
            unitInfo.OwningSlotPath = Settings.DataPath();
            unitInfo.SlotType = protein.Core.ToSlotType();
            if (unitInfo.SlotType.Equals(SlotType.Unknown))
            {
               unitInfo.SlotType = unitInfo.CoreID.ToSlotType();
            }
         }
         // build unit info logic
         var unitInfoLogic = ServiceLocator.Resolve<UnitInfoLogic>();
         unitInfoLogic.CurrentProtein = protein;
         unitInfoLogic.UnitInfoData = unitInfo;
         return unitInfoLogic;
      }

      private static void PopulateRunLevelData(ClientRun run, SlotModel slotModel)
      {
         slotModel.Arguments = run.Arguments;
         slotModel.ClientVersion = run.ClientVersion;

         slotModel.UserId = run.UserID;
         slotModel.MachineId = run.MachineID;

         slotModel.TotalRunCompletedUnits = run.CompletedUnits;
         slotModel.TotalRunFailedUnits = run.FailedUnits;
         slotModel.TotalCompletedUnits = run.TotalCompletedUnits;
      }

      private static void PopulateRunLevelData(ClientQueueEntry queueEntry, SlotModel slotModel)
      {
         if (slotModel.UserId == Constants.DefaultUserID)
         {
            slotModel.UserId = queueEntry.UserID;
         }
         if (slotModel.MachineId == Constants.DefaultMachineID)
         {
            slotModel.MachineId = queueEntry.MachineID;
         }
      }

      /// <summary>
      /// Update Project Benchmarks
      /// </summary>
      /// <param name="currentUnitInfo">Current UnitInfo</param>
      /// <param name="parsedUnits">Parsed UnitInfo Array</param>
      /// <param name="nextUnitIndex">Index of Current UnitInfo</param>
      internal void UpdateBenchmarkData(UnitInfoLogic currentUnitInfo, UnitInfoLogic[] parsedUnits, int nextUnitIndex)
      {
         var foundCurrent = false;
         var processUpdates = false;
         var index = GetStartingIndex(nextUnitIndex, parsedUnits.Length);

         while (index != -1)
         {
            // If Current has not been found, check the nextUnitIndex
            // or try to match the Current Project and Raw Download Time
            if (parsedUnits[index] != null && processUpdates == false && (index == nextUnitIndex || currentUnitInfo.UnitInfoData.Equals(parsedUnits[index].UnitInfoData)))
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
               if (parsedUnits[index] != null)
               {
                  // Update benchmarks
                  BenchmarkCollection.UpdateData(parsedUnits[index].UnitInfoData, previousFramesComplete, parsedUnits[index].FramesComplete);
                  // Update history database
                  UpdateUnitInfoDatabase(parsedUnits[index]);
               }
            }

            #region Increment to the next unit or set terminal value
            if (index == nextUnitIndex)
            {
               index = -1;
            }
            else if (index == parsedUnits.Length - 1)
            {
               index = 0;
            }
            else
            {
               index++;
            }
            #endregion
         }
      }

      private static int GetStartingIndex(int nextUnitIndex, int numberOfUnits)
      {
         if (nextUnitIndex == numberOfUnits - 1)
         {
            return 0;
         }

         return nextUnitIndex + 1;
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
                             InstanceName = Settings.Name,
                             SlotType = slot.UnitInfoLogic.UnitInfoData.SlotType,
                             LastRetrievalTime = slot.UnitInfoLogic.UnitInfoData.UnitRetrievalTime,
                             IgnoreUtcOffset = Settings.UtcOffsetIsZero,
                             UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now),
                             ClientTimeOffset = Settings.ClientTimeOffset,
                             TimeOfLastUnitStart = slot.TimeOfLastUnitStart,
                             TimeOfLastFrameProgress = slot.TimeOfLastFrameProgress,
                             CurrentStatus = slot.Status,
                             ReturnedStatus = returnedStatus,
                             FrameTime = slot.UnitInfoLogic.GetRawTime(Prefs.Get<PpdCalculationType>(Preference.PpdCalculation)),
                             AverageFrameTime = GetBenchmarkAverageFrameTimeOrDefault(slot.UnitInfo),
                             TimeOfLastFrame = slot.UnitInfoLogic.UnitInfoData.CurrentFrame == null
                                                  ? TimeSpan.Zero
                                                  : slot.UnitInfoLogic.UnitInfoData.CurrentFrame.TimeOfFrame,
                             UnitStartTimeStamp = slot.UnitInfoLogic.UnitInfoData.UnitStartTimeStamp,
                             AllowRunningAsync = Prefs.Get<bool>(Preference.AllowRunningAsync)
                          };

         // If the returned status is EuePause and current status is not
         if (statusData.ReturnedStatus.Equals(SlotStatus.EuePause) && statusData.CurrentStatus.Equals(SlotStatus.EuePause) == false)
         {
            if (Prefs.Get<bool>(Preference.EmailReportingEnabled) &&
                Prefs.Get<bool>(Preference.ReportEuePause))
            {
               SendEuePauseEmail(statusData.InstanceName);
            }
         }

         // If the returned status is Hung and current status is not
         if (statusData.ReturnedStatus.Equals(SlotStatus.Hung) && statusData.CurrentStatus.Equals(SlotStatus.Hung) == false)
         {
            if (Prefs.Get<bool>(Preference.EmailReportingEnabled) &&
                Prefs.Get<bool>(Preference.ReportHung))
            {
               SendHungEmail(statusData.InstanceName);
            }
         }

         slot.Status = StatusLogic.HandleStatusData(statusData);
      }

      private TimeSpan GetBenchmarkAverageFrameTimeOrDefault(UnitInfo unitInfo)
      {
         var benchmark = BenchmarkCollection.GetBenchmark(unitInfo);
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
