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
                  // reset settings BEFORE Close()
                  _settings = value;
                  // Close this client and allow retrieval
                  // to open a new connection
                  _fahClient.Close();
               }
               else
               {
                  // reset settings BEFORE slot refresh
                  _settings = value;
                  // refresh the slots with the updated settings
                  RefreshSlots();
               }
            }
            else
            {
               // no existing settings, just set the value
               _settings = value;
            }
         }
      }

      private bool DefaultSlotActive
      {
         //get { return (!_fahClient.Connected || _slots.Count == 0); }

         // based on only the slot count, otherwise if a connection is
         // closed while the Slots property is being enumerated by a
         // consumer the collection will be changed and the enumeration
         // will fail.  if the connection is closed in the midle of an
         // enumeration the slim lock will halt the call to RefreshSlots()
         // until the Slots property is no longer being enumerated.
         get { return _slots.Count == 0; }
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
               return _slots.ToArray();
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
      private readonly FahClientMessages _messages;

      public FahClient(IFahClientInterface fahClient)
      {
         _fahClient = fahClient;
         _slots = new List<SlotModel>();
         _slotsLock = new ReaderWriterLockSlim();
         _logText = new StringBuilder();
         _messages = new FahClientMessages();

         //_fahClient.CacheMessage<Info>(true);
         //_fahClient.CacheMessage<Options>(true);
         _fahClient.MessageUpdated += FahClientMessageUpdated;
         _fahClient.UpdateFinished += FahClientUpdateFinished;
         _fahClient.ConnectedChanged += FahClientConnectedChanged;
      }

      private void FahClientMessageUpdated(object sender, MessageUpdatedEventArgs e)
      {
         if (AbortFlag) return;

         _messages.Add(e);
         JsonMessage message = _fahClient.GetJsonMessage(e.Key);
         Logger.DebugFormat(Constants.ClientNameFormat, Settings.Name, message.GetMessageHeader());

         if (e.DataType == typeof(Heartbeat))
         {
            _messages.SetHeartbeat(_fahClient.GetMessage<Heartbeat>());
         }
         else if (e.DataType == typeof(UnitCollection))
         {
            _messages.UnitCollection = _fahClient.GetMessage<UnitCollection>();
         }
         else if (e.DataType == typeof(SlotCollection))
         {
            _messages.SlotCollection = _fahClient.GetMessage<SlotCollection>();
            foreach (var slot in _messages.SlotCollection)
            {
               _fahClient.SendCommand(String.Format(CultureInfo.InvariantCulture, Constants.FahClientSlotOptions, slot.Id));
            }
         }
         else if (e.DataType == typeof(SlotOptions))
         {
            _messages.AddSlotOptions(_fahClient.GetMessage<SlotOptions>());
         }
         else if (e.DataType == typeof(LogRestart))
         {
            LogFragment logFragment = _fahClient.GetMessage<LogRestart>();
            IEnumerable<char[]> chunks = logFragment.Value.GetChunks();

            // clear
            _logText.Length = 0;
            WriteToLocalFahLogCache(chunks);
            AppendToLogBuffer(chunks, logFragment.Value.Length);
         }
         else if (e.DataType == typeof(LogUpdate))
         {
            LogFragment logFragment = _fahClient.GetMessage<LogUpdate>();
            IEnumerable<char[]> chunks = logFragment.Value.GetChunks();

            WriteToLocalFahLogCache(chunks);
            AppendToLogBuffer(chunks, logFragment.Value.Length);
         }
      }

      private void WriteToLocalFahLogCache(IEnumerable<char[]> chunks)
      {
         string fahLogPath = Path.Combine(Prefs.CacheDirectory, Settings.CachedFahLogFileName());
         if (_logText.Length == 0)
         {
            int i = 0;
            foreach (var chunk in chunks)
            {
               if (i == 0)
               {
                  File.WriteAllText(fahLogPath, new string(chunk));
               }
               else
               {
                  File.AppendAllText(fahLogPath, new string(chunk));
               }
               i++;
            }
         }
         else
         {
            foreach (var chunk in chunks)
            {
               File.AppendAllText(fahLogPath, new string(chunk));
            }
         }
      }

      private void AppendToLogBuffer(IEnumerable<char[]> chunks, int length)
      {
         Debug.Assert(chunks != null);

         if (_logText.Length > (8000 * 2500)) // 20 Million Bytes
         {
            _logText.Remove(0, length);
         }
         foreach (var chunk in chunks)
         {
            _logText.Append(chunk);
         }
      }

      private void FahClientUpdateFinished(object sender, EventArgs e)
      {
         if (AbortFlag) return;

         if (_messages.PopulateSlotOptions())
         {
            RefreshSlots();
            // new slots, repopulate them
            Retrieve();
         }
         if (!DefaultSlotActive && _messages.ContainsUpdates)
         {
            // clear the new messages buffer
            _messages.Clear();
            // Process the retrieved logs
            Retrieve();
         }
      }

      private void FahClientConnectedChanged(object sender, ConnectedChangedEventArgs e)
      {
         if (!e.Connected)
         {
            // clear the local log buffer
            _logText.Length = 0;
            // reset messages
            _messages.Reset();
            // refresh (clear) the slots
            RefreshSlots();
         }
      }

      private void RefreshSlots()
      {
         _slotsLock.EnterWriteLock();
         try
         {
            _slots.Clear();
            if (_messages.SlotCollection != null)
            {
               // itterate through slot collection);
               foreach (var slot in _messages.SlotCollection)
               {
                  // add slot model to the collection
                  var slotModel = new SlotModel
                                  {
                                     Settings = _settings,
                                     Prefs = Prefs,
                                     Status = (SlotStatus)slot.StatusEnum,
                                     SlotId = slot.Id,
                                     SlotOptions = slot.SlotOptions
                                  };
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
         if (_messages.HeartbeatOverdue)
         {
            // haven't seen a heartbeat
            Abort();
         }

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

      private const int HeartbeatInterval = 60;
      private const int QueueInfoInterval = 30;

      private void SetUpdateCommands()
      {
         _fahClient.SendCommand("updates clear");
         _fahClient.SendCommand("log-updates restart");
         _fahClient.SendCommand(String.Format(CultureInfo.InvariantCulture, "updates add 0 {0} $heartbeat", HeartbeatInterval));
         _fahClient.SendCommand("updates add 1 1 $info");
         _fahClient.SendCommand("updates add 2 1 $(options -a)");
         _fahClient.SendCommand("updates add 3 1 $slot-info");
         _fahClient.SendCommand(String.Format(CultureInfo.InvariantCulture, "updates add 4 {0} $queue-info", QueueInfoInterval));
      }

      private void Process()
      {
         DateTime start = Instrumentation.ExecStart;

         // Set successful Last Retrieval Time
         LastRetrievalTime = DateTime.Now;

         var options = _fahClient.GetMessage<Options>();
         var info = _fahClient.GetMessage<Info>();

         _slotsLock.EnterReadLock();
         try
         {
            foreach (var slotModel in _slots)
            {
               // Re-Init Slot Level Members Before Processing
               slotModel.Initialize();

               #region Run the Aggregator

               DataAggregator.ClientName = slotModel.Name;
               var lines = LogReader.GetLogLines(_logText.Split('\n').Where(x => x.Length != 0), LogFileType.FahClient);
               lines = lines.Filter(LogFilterType.SlotAndNonIndexed, slotModel.SlotId).ToList();
               IDictionary<int, UnitInfo> units = DataAggregator.AggregateData(lines, _messages.UnitCollection, info, options,
                                                                               slotModel.SlotOptions, slotModel.UnitInfo, slotModel.SlotId);
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
               if (DataAggregator.CurrentUnitIndex != -1 && parsedUnits.ContainsKey(DataAggregator.CurrentUnitIndex))
               {
                  slotModel.UnitInfoLogic = parsedUnits[DataAggregator.CurrentUnitIndex];
               }

               SetSlotStatus(slotModel);

               slotModel.UnitInfoLogic.ShowPPDTrace(Logger, slotModel.Name, slotModel.Status,
                  Prefs.Get<PpdCalculationType>(Preference.PpdCalculation),
                  Prefs.Get<BonusCalculationType>(Preference.CalculateBonus));

               string statusMessage = String.Format(CultureInfo.CurrentCulture, "Slot Status: {0}", slotModel.Status);
               Logger.Info(Constants.ClientNameFormat, slotModel.Name, statusMessage);
            }
         }
         finally 
         {
            _slotsLock.ExitReadLock();
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
         unitInfo.OwningClientName = Settings.Name;
         unitInfo.OwningClientPath = Settings.DataPath();
         unitInfo.OwningSlotId = slotModel.SlotId;
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
         Debug.Assert(slotModel != null);

         if (info != null)
         {
            slotModel.ClientVersion = info.Build.Version;
         }
         if (run != null)
         {
            slotModel.TotalRunCompletedUnits = run.CompletedUnits;
            slotModel.TotalRunFailedUnits = run.FailedUnits;
         }
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

      private class FahClientMessages
      {
         #region Fields

         private Heartbeat _lastHeartbeat;

         private UnitCollection _unitCollection;
         private bool _unitCollectionUpdated;

         public UnitCollection UnitCollection
         {
            get { return _unitCollection; }
            set
            {
               if (value == null) return;

               if (_unitCollection == null)
               {
                  _unitCollection = value;
                  _unitCollectionUpdated = true;
               }
               else if (!_unitCollection.Equals(value))
               {
                  _unitCollection = value;
                  _unitCollectionUpdated = true;
               }
            }
         }

         private SlotCollection _slotCollection;

         public SlotCollection SlotCollection
         {
            get { return _slotCollection; }
            set { _slotCollection = value; }
         }

         private readonly IList<SlotOptions> _slotOptions;
         private readonly IList<MessageUpdatedEventArgs> _receivedMessages;

         #endregion

         #region Constructor

         public FahClientMessages()
         {
            _slotOptions = new List<SlotOptions>();        
            _receivedMessages = new List<MessageUpdatedEventArgs>();    
         }

         #endregion

         #region Methods and Properties

         public void Reset()
         {
            _lastHeartbeat = null;
            _unitCollection = null;
            _unitCollectionUpdated = false;
            _slotCollection = null;
            _slotOptions.Clear();
            _receivedMessages.Clear();
         }

         public void SetHeartbeat(Heartbeat heartbeat)
         {
            _lastHeartbeat = heartbeat;
         }

         public bool HeartbeatOverdue
         {
            get
            {
               if (_lastHeartbeat != null)
               {
                  if (DateTime.UtcNow.Subtract(_lastHeartbeat.Received).TotalMinutes >
                      TimeSpan.FromSeconds(HeartbeatInterval * 3).TotalMinutes)
                  {
                     return true;
                  }
               }
               return false;
            }
         }

         public void AddSlotOptions(SlotOptions item)
         {
            _slotOptions.Add(item);
         }

         public bool PopulateSlotOptions()
         {
            if (SlotCollection != null && _slotOptions.Count == SlotCollection.Count)
            {
               foreach (var options in _slotOptions)
               {
                  SlotOptions options1 = options;
                  if (options1.MachineId.HasValue)
                  {
                     int machineId = options1.MachineId.Value;
                     var slot = SlotCollection.First(x => x.Id == machineId);
                     slot.SlotOptions = options;
                  }
               }
               _slotOptions.Clear();

               return true;
            }

            return false;
         }

         public void Add(MessageUpdatedEventArgs item)
         {
            _receivedMessages.Add(item);
         }

         public bool ContainsUpdates
         {
            get
            {
               // possibly wait for a log message that is less 
               // than 65000 bytes before triggering an update
               return (ContainsMessage(typeof(LogRestart)) ||
                       ContainsMessage(typeof(LogUpdate))) && _unitCollectionUpdated;
            }
         }

         public void Clear()
         {
            _unitCollectionUpdated = false;
            _receivedMessages.Clear();
         }

         private bool ContainsMessage(Type type)
         {
            // use == for equality... DataType property could easily be null
            return _receivedMessages.FirstOrDefault(x => x.DataType == type) != null;
         }

         #endregion
      }
   }
}
