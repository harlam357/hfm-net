/*
 * HFM.NET - Fah Client Class
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
using System.Globalization;
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

         if (e.DataType == typeof(SlotCollection))
         {
            _slotCollection = _fahClient.GetMessage<SlotCollection>();
            foreach (var slot in _slotCollection)
            {
               _fahClient.SendCommand(String.Format(CultureInfo.InvariantCulture, Constants.FahClientSlotOptions, slot.Id));
            }
         }
         else if (e.DataType.Equals(typeof(SlotOptions)))
         {
            _slotOptions.Add(_fahClient.GetMessage<SlotOptions>());
         }
         else if (e.DataType.Equals(typeof(LogRestart)))
         {
            // clear
            _logText.Length = 0;
            _logText.Append(_fahClient.GetMessage<LogRestart>().Value);
            // write new local log file
         }
         else if (e.DataType.Equals(typeof(LogUpdate)))
         {
            _logText.Append(_fahClient.GetMessage<LogUpdate>().Value);
            // append to local log file
         }
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
         if (!DefaultSlotActive && _newMessages.ContainsUpdates())
         {
            // clear the new messages buffer
            _newMessages.Clear();
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
                  slotModel.MachineId = slot.SlotOptions.MachineId.GetValueOrDefault();
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
               Logger.ErrorFormat(ex, Constants.InstanceNameFormat, Settings.Name, ex.Message);
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
            Logger.ErrorFormat(ex, Constants.InstanceNameFormat, Settings.Name, ex.Message);
         }
         finally
         {
            if (!AbortFlag) OnRetrievalFinished(EventArgs.Empty);
         }
      }

      private void SetUpdateCommands()
      {
         _fahClient.SendCommand("updates clear");
         _fahClient.SendCommand("log-updates restart");
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

         foreach (var slotModel in Slots)
         {
            // Re-Init Slot Level Members Before Processing
            slotModel.Initialize();

            #region Run the Aggregator

            var unitCollection = _fahClient.GetMessage<UnitCollection>();
            var options = _fahClient.GetMessage<Options>();
            var info = _fahClient.GetMessage<Info>();

            var lines = _logText.ToString().Split('\n').Where(x => x.Length != 0).ToList();
            IList<UnitInfo> units = DataAggregator.AggregateData(LogReader.GetLogLines(lines, LogFileType.FahClient), unitCollection, options, slotModel.SlotOptions, slotModel.SlotId);
            // Issue 126 - Use the Folding ID, Team, User ID, and Machine ID from the FAHlog data.
            // Use the Current Queue Entry as a backup data source.
            PopulateRunLevelData(DataAggregator.CurrentClientRun, info, slotModel);

            slotModel.Queue = DataAggregator.Queue;
            slotModel.CurrentLogLines = DataAggregator.CurrentLogLines;
            //slotModel.UnitLogLines = DataAggregator.UnitLogLines;

            #endregion

            var parsedUnits = new UnitInfoLogic[units.Count];
            for (int i = 0; i < units.Count; i++)
            {
               if (units[i] != null)
               {
                  parsedUnits[i] = BuildUnitInfoLogic(slotModel, units[i]);
               }
            }

            if (DataAggregator.CurrentUnitIndex != -1)
            {
               // *** THIS HAS TO BE DONE BEFORE UPDATING SlotModel.UnitInfoLogic ***
               UpdateBenchmarkData(slotModel.UnitInfoLogic, parsedUnits[DataAggregator.CurrentUnitIndex]);
               // 
               slotModel.UnitInfoLogic = parsedUnits[DataAggregator.CurrentUnitIndex];
            }

            SetSlotStatus(slotModel);

            slotModel.UnitInfoLogic.ShowPPDTrace(Logger, slotModel.Status,
               Prefs.Get<PpdCalculationType>(Preference.PpdCalculation),
               Prefs.Get<bool>(Preference.CalculateBonus));

            string statusMessage = String.Format(CultureInfo.CurrentCulture, "Slot Status: {0}", slotModel.Status);
            Logger.Info(Constants.InstanceNameFormat, slotModel.Settings.Name, statusMessage);
         }

         string message = String.Format(CultureInfo.CurrentCulture, "Retrieval finished in {0}", Instrumentation.GetExecTime(start));
         Logger.Info(Constants.InstanceNameFormat, Settings.Name, message);
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
         //slotModel.TotalClientCompletedUnits = run.TotalCompletedUnits;
      }

      internal void UpdateBenchmarkData(UnitInfoLogic currentUnitInfo, UnitInfoLogic parsedUnitInfo)
      {
         if (parsedUnitInfo == null) return;

         // current frame has already been recorded, increment to the next frame
         int previousFramesComplete = currentUnitInfo.FramesComplete + 1;

         // Update benchmarks
         BenchmarkCollection.UpdateData(parsedUnitInfo.UnitInfoData, previousFramesComplete, parsedUnitInfo.FramesComplete);
         // Update history database
         if (UnitInfoDatabase != null && UnitInfoDatabase.Connected)
         {
            try
            {
               UnitInfoDatabase.WriteUnitInfo(parsedUnitInfo);
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, "{0}", ex.Message);
            }
         }
      }
   }

   public static class FahClientExtensions
   {
      public static bool Contains(this IEnumerable<MessageUpdatedEventArgs> messages, Type type)
      {
         return messages.FirstOrDefault(x => x.DataType.Equals(type)) != null;
      }

      public static bool ContainsUpdates(this IEnumerable<MessageUpdatedEventArgs> messages)
      {
         return (messages.Contains(typeof(LogRestart)) || 
                 messages.Contains(typeof(LogUpdate))) &&
                 messages.Contains(typeof(UnitCollection));
      }
   }
}
