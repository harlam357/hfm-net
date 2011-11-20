/*
 * HFM.NET - Client Instance Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.IO;

using Castle.Core.Logging;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public sealed class LegacyClient : IClient
   {
      #region Fields

      #region Injection Properties

      private IPreferenceSet _prefs;
      /// <summary>
      /// PreferenceSet Interface
      /// </summary>
      public IPreferenceSet Prefs
      {
         set { _prefs = value; }
      }

      private ILogger _logger = NullLogger.Instance;
      /// <summary>
      /// Logging Interface
      /// </summary>
      public ILogger Logger
      {
         set { _logger = value; }
      }

      private IDictionary<int, Protein> _proteinDictionary;
      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      public IDictionary<int, Protein> ProteinDictionary
      {
         set { _proteinDictionary = value; }
      }

      private IProteinBenchmarkCollection _benchmarkCollection;
      /// <summary>
      /// Benchmark Collection Interface
      /// </summary>
      public IProteinBenchmarkCollection BenchmarkCollection
      {
         set { _benchmarkCollection = value; }
      }

      private IStatusLogic _statusLogic;
      /// <summary>
      /// Status Logic Interface
      /// </summary>
      public IStatusLogic StatusLogic
      {
         set { _statusLogic = value; }
      }

      private IDataRetriever _dataRetriever;
      /// <summary>
      /// Data Retriever Interface
      /// </summary>
      public IDataRetriever DataRetriever
      {
         set { _dataRetriever = value; }
      }

      private IDataAggregator _dataAggregator;
      /// <summary>
      /// Data Aggregator Interface
      /// </summary>
      public IDataAggregator DataAggregator
      {
         set { _dataAggregator = value; }
      }

      #endregion

      public ClientSettings Settings { get; set; }

      private readonly IDictionary<int, SlotModel> _slots;
      
      public IDictionary<int, SlotModel> Slots
      {
         get { return _slots; }
      }

      #endregion

      public LegacyClient()
      {
         _slots = new Dictionary<int, SlotModel> { { 0, new SlotModel() } };

         LastRetrievalTime = DateTime.MinValue;
      }
      
      #region Retrieval Properties

      private volatile bool _retrievalInProgress;
      /// <summary>
      /// Local flag set when log retrieval is in progress
      /// </summary>
      public bool RetrievalInProgress
      {
         get { return _retrievalInProgress; }
      }

      private bool _abort;

      /// <summary>
      /// When the data was last successfully retrieved
      /// </summary>
      public DateTime LastRetrievalTime { get; private set; } // should be init to DateTime.MinValue

      #endregion

      #region Retrieval Methods

      public void Abort()
      {
         _abort = true;
      }

      private bool AbortRetrieve
      {
         get { return _retrievalInProgress && _abort; }
      }

      /// <summary>
      /// Retrieve Instance Log Files based on Instance Type
      /// </summary>
      public void Retrieve()
      {
         Debug.Assert(Slots.Count == 1);
         RetrieveInternal(Slots[0]);
      }

      /// <summary>
      /// Retrieve Instance Log Files based on Instance Type
      /// </summary>
      private void RetrieveInternal(SlotModel defaultSlotModel)
      {
         try
         {
            // Don't allow this to fire more than once at a time
            if (_retrievalInProgress) return;
         
            _retrievalInProgress = true;
            _abort = false;

            _dataRetriever.Execute(Settings);
            if (!AbortRetrieve)
            {
               // Set successful Last Retrieval Time
               LastRetrievalTime = DateTime.Now;
               // Re-Init Client Level Members Before Processing
               defaultSlotModel.Initialize();
               // Process the retrieved logs

               ClientStatus returnedStatus = ProcessLegacy(defaultSlotModel);
               // Handle the status retured from the log parse
               HandleReturnedStatus(returnedStatus, defaultSlotModel);

               _logger.Info("{0} ({1}) Client Status: {2}", Instrumentation.FunctionName, defaultSlotModel.Settings.Name, defaultSlotModel.Status);
            }
            else
            {
               defaultSlotModel.Status = ClientStatus.Offline;
               _logger.Info(Constants.InstanceNameFormat, defaultSlotModel.Settings.Name, "Retrieval Aborted...");
            }
         }
         catch (Exception ex)
         {
            defaultSlotModel.Status = ClientStatus.Offline;
            _logger.ErrorFormat(ex, Constants.InstanceNameFormat, defaultSlotModel.Settings.Name, ex.Message);
         }
         finally
         {
            _retrievalInProgress = false;
            _abort = false;
         }
      }
      
      #endregion

      #region Queue and Log Processing Functions
      
      /// <summary>
      /// Process the cached log files that exist on this machine
      /// </summary>
      private ClientStatus ProcessLegacy(SlotModel defaultSlotModel)
      {
         DateTime start = Instrumentation.ExecStart;

         #region Setup UnitInfo Aggregator

         _dataAggregator.InstanceName = Settings.Name;
         _dataAggregator.QueueFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueFileName());
         _dataAggregator.FahLogFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogFileName());
         _dataAggregator.UnitInfoLogFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoFileName()); 
         
         #endregion
         
         #region Run the Aggregator and Set LegacyClient Level Results
         
         IList<UnitInfo> units = _dataAggregator.AggregateData();
         // Issue 126 - Use the Folding ID, Team, User ID, and Machine ID from the FAHlog data.
         // Use the Current Queue Entry as a backup data source.
         PopulateRunLevelData(_dataAggregator.CurrentClientRun, defaultSlotModel);
         if (_dataAggregator.Queue != null)
         {
            PopulateRunLevelData(_dataAggregator.Queue.CurrentQueueEntry, defaultSlotModel);
         }

         defaultSlotModel.Queue = _dataAggregator.Queue;
         defaultSlotModel.CurrentLogLines = _dataAggregator.CurrentLogLines;
         defaultSlotModel.UnitLogLines = _dataAggregator.UnitLogLines;
         
         #endregion
         
         var parsedUnits = new UnitInfoLogic[units.Count];
         for (int i = 0; i < units.Count; i++)
         {
            if (units[i] != null)
            {
               parsedUnits[i] = BuildUnitInfoLogic(units[i]);
            }
         }

         // *** THIS HAS TO BE DONE BEFORE UPDATING THE UnitInfoLogic ***
         // Update Benchmarks from parsedUnits array 
         _benchmarkCollection.UpdateData(defaultSlotModel.UnitInfoLogic, parsedUnits, _dataAggregator.CurrentUnitIndex);

         // Update the UnitInfoLogic if we have a Status
         ClientStatus currentWorkUnitStatus = _dataAggregator.CurrentClientRun.Status;
         if (currentWorkUnitStatus.Equals(ClientStatus.Unknown) == false)
         {
            defaultSlotModel.UnitInfoLogic = parsedUnits[_dataAggregator.CurrentUnitIndex];
         }
         
         defaultSlotModel.UnitInfoLogic.ShowPPDTrace(_logger, defaultSlotModel.Status, 
            _prefs.Get<PpdCalculationType>(Preference.PpdCalculation),
            _prefs.Get<bool>(Preference.CalculateBonus));
         _logger.Debug(Constants.InstanceNameFormat, Settings.Name, Instrumentation.GetExecTime(start));

         // Return the Status
         return currentWorkUnitStatus;
      }

      private UnitInfoLogic BuildUnitInfoLogic(UnitInfo unitInfo)
      {
         Debug.Assert(unitInfo != null);

         Protein protein = _proteinDictionary.ContainsKey(unitInfo.ProjectID)
                              ? _proteinDictionary[unitInfo.ProjectID]
                              : new Protein();

         // update the data
         unitInfo.UnitRetrievalTime = LastRetrievalTime;
         unitInfo.OwningInstanceName = Settings.Name;
         unitInfo.OwningInstancePath = Settings.Path;
         unitInfo.SlotType = UnitInfo.DetermineSlotType(protein.Core, unitInfo.CoreID);
         // build unit info logic
         var unitInfoLogic = ServiceLocator.Resolve<UnitInfoLogic>();
         unitInfoLogic.CurrentProtein = protein;
         unitInfoLogic.UnitInfoData = unitInfo;
         return unitInfoLogic;
      }

      private static void PopulateRunLevelData(ClientRun run, SlotModel displayInstance)
      {
         displayInstance.Arguments = run.Arguments;
         displayInstance.ClientVersion = run.ClientVersion;

         displayInstance.UserId = run.UserID;
         displayInstance.MachineId = run.MachineID;

         displayInstance.TotalRunCompletedUnits = run.CompletedUnits;
         displayInstance.TotalRunFailedUnits = run.FailedUnits;
         displayInstance.TotalClientCompletedUnits = run.TotalCompletedUnits;
      }

      private static void PopulateRunLevelData(ClientQueueEntry queueEntry, SlotModel displayInstance)
      {
         if (displayInstance.UserId == Constants.DefaultUserID)
         {
            displayInstance.UserId = queueEntry.UserID;
         }
         if (displayInstance.MachineId == Constants.DefaultMachineID)
         {
            displayInstance.MachineId = queueEntry.MachineID;
         }
      }
      
      #endregion

      #region Status Handling and Determination
      
      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determines what values to feed the DetermineStatus routine.
      /// </summary>
      private void HandleReturnedStatus(ClientStatus returnedStatus, SlotModel slot)
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
                             FrameTime = slot.UnitInfoLogic.GetRawTime(_prefs.Get<PpdCalculationType>(Preference.PpdCalculation)),
                             AverageFrameTime = _benchmarkCollection.GetBenchmark(slot.UnitInfo).AverageFrameTime,
                             TimeOfLastFrame = slot.UnitInfoLogic.UnitInfoData.CurrentFrame == null
                                                  ? TimeSpan.Zero
                                                  : slot.UnitInfoLogic.UnitInfoData.CurrentFrame.TimeOfFrame,
                             UnitStartTimeStamp = slot.UnitInfoLogic.UnitInfoData.UnitStartTimeStamp,
                             AllowRunningAsync = _prefs.Get<bool>(Preference.AllowRunningAsync)
                          };

         // If the returned status is EuePause and current status is not
         if (statusData.ReturnedStatus.Equals(ClientStatus.EuePause) && statusData.CurrentStatus.Equals(ClientStatus.EuePause) == false)
         {
            if (_prefs.Get<bool>(Preference.EmailReportingEnabled) &&
                _prefs.Get<bool>(Preference.ReportEuePause))
            {
               SendEuePauseEmail(statusData.InstanceName);
            }
         }

         // If the returned status is Hung and current status is not
         if (statusData.ReturnedStatus.Equals(ClientStatus.Hung) && statusData.CurrentStatus.Equals(ClientStatus.Hung) == false)
         {
            if (_prefs.Get<bool>(Preference.EmailReportingEnabled) &&
                _prefs.Get<bool>(Preference.ReportHung))
            {
               SendHungEmail(statusData.InstanceName);
            }
         }

         slot.Status = _statusLogic.HandleStatusData(statusData);
      }

      /// <summary>
      /// Send EuePause Status Email
      /// </summary>
      private void SendEuePauseEmail(string name)
      {
         string messageBody = String.Format("HFM.NET detected that Client '{0}' has entered a 24 hour EUE Pause state.", name);
         try
         {
            NetworkOps.SendEmail(_prefs.Get<bool>(Preference.EmailReportingServerSecure),
                                 _prefs.Get<string>(Preference.EmailReportingFromAddress),
                                 _prefs.Get<string>(Preference.EmailReportingToAddress),
                                 "HFM.NET - Client EUE Pause Error", messageBody,
                                 _prefs.Get<string>(Preference.EmailReportingServerAddress),
                                 _prefs.Get<int>(Preference.EmailReportingServerPort),
                                 _prefs.Get<string>(Preference.EmailReportingServerUsername),
                                 _prefs.Get<string>(Preference.EmailReportingServerPassword));
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
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
            NetworkOps.SendEmail(_prefs.Get<bool>(Preference.EmailReportingServerSecure),
                                 _prefs.Get<string>(Preference.EmailReportingFromAddress),
                                 _prefs.Get<string>(Preference.EmailReportingToAddress),
                                 "HFM.NET - Client Hung Error", messageBody,
                                 _prefs.Get<string>(Preference.EmailReportingServerAddress),
                                 _prefs.Get<int>(Preference.EmailReportingServerPort),
                                 _prefs.Get<string>(Preference.EmailReportingServerUsername),
                                 _prefs.Get<string>(Preference.EmailReportingServerPassword));
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }
      }

      #endregion
   }
}
