/*
 * HFM.NET - Fah Client Class
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
using System.Linq;
using System.Text;
using System.Threading;

using HFM.Client;
using HFM.Client.DataTypes;
using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   public sealed class FahClient : Client
   {
      #region Injection Properties

      public IFahClientDataAggregator DataAggregator { get; set; }

      #endregion

      #region Properties

      private ClientSettings _settings;
      
      public override ClientSettings Settings
      {
         get { return _settings; }
         set
         {
            Debug.Assert(value.IsFahClient());

            // settings already exist
            if (_settings != null)
            {
               if (!_settings.Server.Equals(value.Server) ||
                   !_settings.Port.Equals(value.Port) ||
                   !_settings.Password.Equals(value.Password))
               {
                  // connection settings have changed
                  _fahClient.Close();
               }
               else
               {
                  RefreshSlots();
               }
            }
            _settings = value;
         }
      }

      private bool DefaultSlotActive
      {
         get { return (!_fahClient.Connected || _slots.Count == 0); }
      }

      public override IEnumerable<SlotModel> Slots
      {
         get
         {
            _slotsLock.EnterReadLock();
            try
            {
               // not connected or no slots
               if (DefaultSlotActive)
               {
                  // return default slot (for grid binding)
                  return new[] { new SlotModel { Settings = _settings, Prefs = Prefs, Status = SlotStatus.Offline } };
               }
               return _slots.AsReadOnly();
            }
            finally
            {
               _slotsLock.ExitReadLock();
            }
         }
      }

      #endregion

      //private void RestoreUnitInfo()
      //{
      //   if (UnitInfoCollection == null) return;
      //
      //   _slotsLock.EnterReadLock();
      //   try
      //   {
      //      foreach (var slotModel in _slots)
      //      {
      //         foreach (var unitInfo in UnitInfoCollection)
      //         {
      //            if (slotModel.Owns(unitInfo))
      //            {
      //               slotModel.UnitInfoLogic = BuildUnitInfoLogic(slotModel, unitInfo);
      //               break;
      //            }
      //         }
      //      }
      //   }
      //   finally
      //   {
      //      _slotsLock.ExitReadLock();
      //   }
      //}

      private readonly IFahClientInterface _fahClient;
      private readonly List<SlotModel> _slots;
      private readonly ReaderWriterLockSlim _slotsLock;
      private readonly StringBuilder _logText;
      private readonly List<MessageUpdatedEventArgs> _newMessages;

      private bool _unitCollectionUpdated;
      private UnitCollection _unitCollection;
      private SlotCollection _slotCollection;
      private readonly List<SlotOptions> _slotOptions;

      public FahClient(IFahClientInterface fahClient)
      {
         _fahClient = fahClient;
         _slots = new List<SlotModel>();
         _slotsLock = new ReaderWriterLockSlim();
         _logText = new StringBuilder();
         _newMessages = new List<MessageUpdatedEventArgs>();

         _slotOptions = new List<SlotOptions>();

         _fahClient.MessageUpdated += FahClientMessageUpdated;
         _fahClient.UpdateFinished += FahClientUpdateFinished;
         _fahClient.ConnectedChanged += FahClientConnectedChanged;
      }

      private void FahClientMessageUpdated(object sender, MessageUpdatedEventArgs e)
      {
         if (AbortFlag) return;

         _newMessages.Add(e);

         if (e.DataType == typeof(UnitCollection))
         {
            var unitCollection = _fahClient.GetMessage<UnitCollection>();
            if (_unitCollection == null)
            {
               _unitCollection = unitCollection;
               _unitCollectionUpdated = true;
            }
            else if (!_unitCollection.Equals(unitCollection))
            {
               _unitCollection = unitCollection;
               _unitCollectionUpdated = true;
            }
         }
         else if (e.DataType == typeof(SlotCollection))
         {
            _slotCollection = _fahClient.GetMessage<SlotCollection>();
            foreach (var slot in _slotCollection)
            {
               _fahClient.SendCommand(String.Format(CultureInfo.InvariantCulture, Constants.FahClientSlotOptions, slot.Id));
            }
         }
         else if (e.DataType == typeof(SlotOptions))
         {
            _slotOptions.Add(_fahClient.GetMessage<SlotOptions>());
         }
         else if (e.DataType == typeof(LogRestart))
         {
            string logRestart = _fahClient.GetMessage<LogRestart>().Value;
            // clear
            _logText.Length = 0;
            WriteToLocalFahLogCache(logRestart);
            AppendToLogBuffer(logRestart);
         }
         else if (e.DataType == typeof(LogUpdate))
         {
            string logUpdate = _fahClient.GetMessage<LogUpdate>().Value;
            WriteToLocalFahLogCache(logUpdate);
            AppendToLogBuffer(logUpdate);
         }
      }

      private void WriteToLocalFahLogCache(string value)
      {
         string fahLogPath = Path.Combine(Prefs.CacheDirectory, Settings.CachedFahLogFileName());
         if (_logText.Length == 0)
         {
            File.WriteAllText(fahLogPath, value);
         }
         else
         {
            File.AppendAllText(fahLogPath, value);
         }
      }

      private void AppendToLogBuffer(string value)
      {
         Debug.Assert(value != null);

         if (_logText.Length > 450000)
         {
            _logText.Remove(0, value.Length);
         }
         _logText.Append(value);
      }

      private void FahClientUpdateFinished(object sender, EventArgs e)
      {
         if (AbortFlag) return;

         if (_slotCollection != null && _slotOptions.Count == _slotCollection.Count)
         {
            foreach (var options in _slotOptions)
            {
               SlotOptions options1 = options;
               if (options1.MachineId.HasValue)
               {
                  int machineId = options1.MachineId.Value;
                  var slot = _slotCollection.First(x => x.Id == machineId);
                  slot.SlotOptions = options;
               }
            }
            _slotOptions.Clear();
            RefreshSlots();
         }
         if (!DefaultSlotActive && _newMessages.ContainsUpdates() && _unitCollectionUpdated)
         {
            // clear the new messages buffer
            _newMessages.Clear();
            // clear collection updated flag
            _unitCollectionUpdated = false;
            // Process the retrieved logs
            Retrieve();
         }
      }

      private void FahClientConnectedChanged(object sender, ConnectedChangedEventArgs e)
      {
         if (!e.Connected)
         {
            // clear
            _logText.Length = 0;
            _slotCollection = null;
            RefreshSlots();
         }
      }

      private void RefreshSlots()
      {
         _slotsLock.EnterWriteLock();
         try
         {
            _slots.Clear();
            if (_slotCollection != null)
            {
               // itterate through slot collection
               foreach (var slot in _slotCollection)
               {
                  // add slot model to the collection
                  var slotModel = new SlotModel { Settings = _settings, Prefs = Prefs };
                  slotModel.Status = (SlotStatus)slot.StatusEnum;
                  slotModel.SlotId = slot.Id;
                  slotModel.SlotOptions = slot.SlotOptions;
                  _slots.Add(slotModel);
               }
            }
         }
         finally
         {
            _slotsLock.ExitWriteLock();
         }

         OnSlotsChanged(EventArgs.Empty);
      }

      public override void Abort()
      {
         base.Abort();

         if (_fahClient.Connected)
         {
            _fahClient.Close();
         }
      }

      protected override void RetrieveInternal()
      {
         // connect if not connected
         if (!_fahClient.Connected)
         {
            try
            {
               _fahClient.Connect(Settings.Server, Settings.Port, Settings.Password);
               SetUpdateCommands();
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, Constants.ClientNameFormat, Settings.Name, ex.Message);
            }
            return;
         }

         try
         {
            // Process the retrieved data
            Process();
         }
         catch (Exception ex)
         {
            Logger.ErrorFormat(ex, Constants.ClientNameFormat, Settings.Name, ex.Message);
         }
         finally
         {
            if (!AbortFlag) OnRetrievalFinished(EventArgs.Empty);
         }
      }

      private void SetUpdateCommands()
      {
         _fahClient.SendCommand("updates clear");
         //_fahClient.SendCommand("log-updates restart");
         _fahClient.SendCommand("log-updates start");
         _fahClient.SendCommand("updates add 0 5 $heartbeat");
         _fahClient.SendCommand("updates add 1 1 $info");
         _fahClient.SendCommand("updates add 2 1 $(options -a)");
         _fahClient.SendCommand("updates add 3 4 $queue-info");
         _fahClient.SendCommand("updates add 4 1 $slot-info");
      }

      private void Process()
      {
         DateTime start = Instrumentation.ExecStart;

         // Set successful Last Retrieval Time
         LastRetrievalTime = DateTime.Now;

         var options = _fahClient.GetMessage<Options>();
         var info = _fahClient.GetMessage<Info>();

         foreach (var slotModel in Slots)
         {
            // Re-Init Slot Level Members Before Processing
            slotModel.Initialize();

            #region Run the Aggregator
            
            DataAggregator.ClientName = slotModel.Name;
            var lines = LogReader.GetLogLines(_logText.ToString().Split('\n').Where(x => x.Length != 0).ToList(), LogFileType.FahClient);
            lines = lines.Filter(LogFilterType.SlotAndNonIndexed, slotModel.SlotId).ToList();
            IDictionary<int, UnitInfo> units = DataAggregator.AggregateData(lines, _unitCollection, options, slotModel.SlotOptions, slotModel.UnitInfo, slotModel.SlotId);
            PopulateRunLevelData(DataAggregator.CurrentClientRun, info, slotModel);

            slotModel.Queue = DataAggregator.Queue;
            slotModel.CurrentLogLines = DataAggregator.CurrentLogLines;
            //slotModel.UnitLogLines = DataAggregator.UnitLogLines;

            #endregion

            var parsedUnits = new Dictionary<int, UnitInfoLogic>(units.Count);
            foreach (int key in units.Keys)
            {
               if (units[key] != null)
               {
                  parsedUnits[key] = BuildUnitInfoLogic(slotModel, units[key]);
               }
            }

            // *** THIS HAS TO BE DONE BEFORE UPDATING SlotModel.UnitInfoLogic ***
            UpdateBenchmarkData(slotModel.UnitInfoLogic, parsedUnits.Values, DataAggregator.CurrentUnitIndex);

            // Update the UnitInfoLogic if we have a current unit index
            if (DataAggregator.CurrentUnitIndex != -1)
            {
               slotModel.UnitInfoLogic = parsedUnits[DataAggregator.CurrentUnitIndex];
            }

            SetSlotStatus(slotModel);

            slotModel.UnitInfoLogic.ShowPPDTrace(Logger, slotModel.Status,
               Prefs.Get<PpdCalculationType>(Preference.PpdCalculation),
               Prefs.Get<BonusCalculationType>(Preference.CalculateBonus));

            string statusMessage = String.Format(CultureInfo.CurrentCulture, "Slot Status: {0}", slotModel.Status);
            Logger.Info(Constants.ClientNameFormat, slotModel.Name, statusMessage);
         }

         string message = String.Format(CultureInfo.CurrentCulture, "Retrieval finished in {0}", Instrumentation.GetExecTime(start));
         Logger.Info(Constants.ClientNameFormat, Settings.Name, message);
      }

      private UnitInfoLogic BuildUnitInfoLogic(SlotModel slotModel, UnitInfo unitInfo)
      {
         Debug.Assert(slotModel != null);
         Debug.Assert(unitInfo != null);

         Protein protein = ProteinDictionary.GetProteinOrDownload(unitInfo.ProjectID);

         // update the data
         unitInfo.UnitRetrievalTime = LastRetrievalTime;
         unitInfo.OwningSlotName = slotModel.Name; // this will return ClientSettings.Name - SlotModel.SlotId
         unitInfo.OwningSlotPath = Settings.DataPath();
         //unitInfo.SlotType = UnitInfo.DetermineSlotType(protein.Core, unitInfo.CoreID);
         // build unit info logic
         var unitInfoLogic = ServiceLocator.Resolve<UnitInfoLogic>();
         unitInfoLogic.CurrentProtein = protein;
         unitInfoLogic.UnitInfoData = unitInfo;
         return unitInfoLogic;
      }

      private static void SetSlotStatus(SlotModel slotModel)
      {
         if (slotModel.Status.Equals(SlotStatus.Running) ||
             slotModel.Status.Equals(SlotStatus.RunningNoFrameTimes))
         {
            slotModel.Status = slotModel.IsUsingBenchmarkFrameTime ? SlotStatus.RunningNoFrameTimes : SlotStatus.Running;
         }
      }

      private static void PopulateRunLevelData(ClientRun run, Info info, SlotModel slotModel)
      {
         //slotModel.Arguments = run.Arguments;
         slotModel.ClientVersion = info.Build.Version;

         //slotModel.UserId = run.UserID;
         //slotModel.MachineId = run.MachineID;

         slotModel.TotalRunCompletedUnits = run.CompletedUnits;
         slotModel.TotalRunFailedUnits = run.FailedUnits;
         //slotModel.TotalCompletedUnits = run.TotalCompletedUnits;
      }

      internal void UpdateBenchmarkData(UnitInfoLogic currentUnitInfo, IEnumerable<UnitInfoLogic> parsedUnits, int currentUnitIndex)
      {
         foreach (var unitInfoLogic in parsedUnits)
         {
            if (unitInfoLogic == null)
            {
               continue;
            }

            if (currentUnitInfo.UnitInfoData.Equals(unitInfoLogic.UnitInfoData))
            {
               // found the current unit
               // current frame has already been recorded, increment to the next frame
               int previousFramesComplete = currentUnitInfo.FramesComplete + 1;
               // Update benchmarks
               BenchmarkCollection.UpdateData(unitInfoLogic.UnitInfoData, previousFramesComplete, unitInfoLogic.FramesComplete);
               // Update history database
               if (!unitInfoLogic.FinishedTime.Equals(DateTime.MinValue))
               {
                  UpdateUnitInfoDatabase(unitInfoLogic);
               }
            }
            //// used when there is no currentUnitInfo
            //else if (unitInfoLogic.UnitInfoData.QueueIndex == currentUnitIndex)
            //{
            //   BenchmarkCollection.UpdateData(unitInfoLogic.UnitInfoData, 0, unitInfoLogic.FramesComplete);
            //}
         }
      }
   }

   public static class FahClientExtensions
   {
      public static bool Contains(this IEnumerable<MessageUpdatedEventArgs> messages, Type type)
      {
         // use == for equality... DataType property could easily be null
         return messages.FirstOrDefault(x => x.DataType == type) != null;
      }

      public static bool ContainsUpdates(this IEnumerable<MessageUpdatedEventArgs> messages)
      {
         return (messages.Contains(typeof(LogRestart)) || 
                 messages.Contains(typeof(LogUpdate))) &&
                 messages.Contains(typeof(UnitCollection));
      }
   }
}
