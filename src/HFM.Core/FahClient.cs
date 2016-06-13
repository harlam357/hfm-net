/*
 * HFM.NET - Fah Client Class
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
using System.Text;
using System.Threading;

using HFM.Client;
using HFM.Client.DataTypes;
using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   public interface IFahClientFactory
   {
      FahClient Create();

      void Release(FahClient fahClient);
   }

   public interface IFahClient : IClient
   {
      /// <summary>
      /// Sends the Fold command to the FAH client.
      /// </summary>
      /// <param name="slotId">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
      void Fold(int? slotId);

      /// <summary>
      /// Sends the Pause command to the FAH client.
      /// </summary>
      /// <param name="slotId">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
      void Pause(int? slotId);

      /// <summary>
      /// Sends the Finish command to the FAH client.
      /// </summary>
      /// <param name="slotId">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
      void Finish(int? slotId);
   }

   public sealed class FahClient : Client, IFahClient
   {
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
               if (_settings.Server != value.Server ||
                   _settings.Port != value.Port ||
                   _settings.Password != value.Password)
               {
                  // connection settings have changed
                  // reset settings BEFORE Close()
                  _settings = value;
                  // Close this client and allow retrieval
                  // to open a new connection
                  _messageConnection.Close();
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
         // will fail.  if the connection is closed in the middle of an
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

      private readonly IMessageConnection _messageConnection;
      private readonly List<SlotModel> _slots;
      private readonly ReaderWriterLockSlim _slotsLock;
      private readonly FahLog _fahLog;
      private MessageReceiver _messages;

      public FahClient(IMessageConnection messageConnection)
      {
         if (messageConnection == null) throw new ArgumentNullException("messageConnection");

         _messageConnection = messageConnection;
         _slots = new List<SlotModel>();
         _slotsLock = new ReaderWriterLockSlim();
         _fahLog = FahLog.Create(FahLogType.FahClient);
         _messages = new MessageReceiver();

         _messageConnection.MessageReceived += MessageConnectionMessageReceived;
         _messageConnection.UpdateFinished += MessageConnectionUpdateFinished;
         _messageConnection.ConnectedChanged += MessageConnectionConnectedChanged;
      }

      private void MessageConnectionMessageReceived(object sender, MessageReceivedEventArgs e)
      {
         if (AbortFlag) return;

         Logger.DebugFormat(Constants.ClientNameFormat, Settings.Name, e.JsonMessage.GetHeader());

         _messages.Add(e);
         if (e.DataType == typeof(SlotCollection))
         {
            foreach (var slot in _messages.SlotCollection)
            {
               _messageConnection.SendCommand(String.Format(CultureInfo.InvariantCulture, Constants.FahClientSlotOptions, slot.Id));
            }
         }
         else if (e.DataType == typeof(LogRestart) ||
                  e.DataType == typeof(LogUpdate))
         {
            var logFragment = (LogFragment)e.TypedMessage;
            IEnumerable<char[]> chunks = logFragment.Value.GetChunks();

            bool createNew = e.DataType == typeof(LogRestart);
            // clear
            if (createNew)
            {
               _fahLog.Clear();
            }
            WriteToLocalFahLogCache(chunks, createNew);
            AppendToLogBuffer(chunks, logFragment.Value.Length);

            if (_messages.LogRetrieved)
            {
               _messageConnection.SendCommand("queue-info");
            }
         }
      }

      private void WriteToLocalFahLogCache(IEnumerable<char[]> chunks, bool createNew)
      {
         string fahLogPath = Path.Combine(Prefs.CacheDirectory, Settings.CachedFahLogFileName());
         if (createNew)
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

         var logText = new StringBuilder();
         foreach (var chunk in chunks)
         {
            logText.Append(chunk);
         }
         _fahLog.AddRange(logText.Split('\n').Where(x => x.Length != 0));
      }

      private void MessageConnectionUpdateFinished(object sender, EventArgs e)
      {
         if (AbortFlag) return;

         var result = _messages.Process();
         if (result.SlotsUpdated)
         {
            RefreshSlots();
         }
         if (!DefaultSlotActive && result.ExecuteRetrieval)
         {
            // Process the retrieved logs
            Retrieve();
         }
      }

      private void MessageConnectionConnectedChanged(object sender, ConnectedChangedEventArgs e)
      {
         if (!e.Connected)
         {
            // clear the local log buffer
            _fahLog.Clear();
            // reset messages
            _messages = new MessageReceiver();
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
               // itterate through slot collection
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

         if (_messageConnection.Connected)
         {
            _messageConnection.Close();
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
         if (!_messageConnection.Connected)
         {
            try
            {
               _messageConnection.Connect(Settings.Server, Settings.Port, Settings.Password);
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
      //private const int QueueInfoInterval = 30;

      private void SetUpdateCommands()
      {
         _messageConnection.SendCommand("updates clear");
         _messageConnection.SendCommand("log-updates restart");
         _messageConnection.SendCommand(String.Format(CultureInfo.InvariantCulture, "updates add 0 {0} $heartbeat", HeartbeatInterval));
         _messageConnection.SendCommand("updates add 1 1 $info");
         _messageConnection.SendCommand("updates add 2 1 $(options -a)");
         _messageConnection.SendCommand("updates add 3 1 $slot-info");
         //_messageConnection.SendCommand(String.Format(CultureInfo.InvariantCulture, "updates add 4 {0} $queue-info", QueueInfoInterval));
      }

      private void Process()
      {
         DateTime start = Instrumentation.ExecStart;

         // Set successful Last Retrieval Time
         LastRetrievalTime = DateTime.Now;

         var options = _messages.Options;
         var info = _messages.Info;

         _slotsLock.EnterReadLock();
         try
         {
            foreach (var slotModel in _slots)
            {
               // Re-Init Slot Level Members Before Processing
               slotModel.Initialize();

               #region Run the Aggregator

               var dataAggregator = new FahClientDataAggregator { Logger = Logger };
               dataAggregator.ClientName = slotModel.Name;
               IDictionary<int, UnitInfo> units = dataAggregator.AggregateData(_fahLog.ClientRuns.FirstOrDefault(), _messages.UnitCollection, info, options,
                                                                               slotModel.SlotOptions, slotModel.UnitInfo, slotModel.SlotId);
               PopulateRunLevelData(dataAggregator.CurrentClientRun, info, slotModel);

               slotModel.Queue = dataAggregator.Queue;
               slotModel.CurrentLogLines = dataAggregator.CurrentLogLines;
               //slotModel.UnitLogLines = dataAggregator.UnitLogLines;

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
               UpdateBenchmarkData(slotModel.UnitInfoLogic, parsedUnits.Values, dataAggregator.CurrentUnitIndex);

               // Update the UnitInfoLogic if we have a current unit index
               if (dataAggregator.CurrentUnitIndex != -1 && parsedUnits.ContainsKey(dataAggregator.CurrentUnitIndex))
               {
                  slotModel.UnitInfoLogic = parsedUnits[dataAggregator.CurrentUnitIndex];
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

         Protein protein = ProteinService.Get(unitInfo.ProjectID, true) ?? new Protein();

         // update the data
         unitInfo.UnitRetrievalTime = LastRetrievalTime;
         unitInfo.OwningClientName = Settings.Name;
         unitInfo.OwningClientPath = Settings.DataPath();
         unitInfo.OwningSlotId = slotModel.SlotId;
         if (unitInfo.SlotType == SlotType.Unknown)
         {
            unitInfo.SlotType = protein.Core.ToSlotType();
            if (unitInfo.SlotType == SlotType.Unknown)
            {
               unitInfo.SlotType = unitInfo.CoreID.ToSlotType();
            }
         }
         // build unit info logic
         var unitInfoLogic = new UnitInfoLogic(BenchmarkCollection);
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

      private void PopulateRunLevelData(ClientRun run, Info info, SlotModel slotModel)
      {
         Debug.Assert(slotModel != null);

         if (info != null)
         {
            slotModel.ClientVersion = info.Build.Version;
         }
         //if (run != null)
         //{
         //   slotModel.TotalRunCompletedUnits = run.CompletedUnits;
         //   slotModel.TotalRunFailedUnits = run.FailedUnits;
         //}
         if (UnitInfoDatabase.Connected)
         {
            slotModel.TotalRunCompletedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Completed, run.Data.StartTime);
            slotModel.TotalCompletedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Completed);
            slotModel.TotalRunFailedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Failed, run.Data.StartTime);
            slotModel.TotalFailedUnits = (int)UnitInfoDatabase.Count(slotModel.Name, CountType.Failed);
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

            if (currentUnitInfo.UnitInfoData.IsSameUnitAs(unitInfoLogic.UnitInfoData))
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

      private class MessageReceiver
      {
         private readonly IEqualityComparer<UnitCollection> _unitCollectionEqualityComparer = new UnitCollectionEqualityComparer();

         private Heartbeat _heartbeat;

         public bool HeartbeatOverdue
         {
            get
            {
               if (_heartbeat == null)
               {
                  return false;
               }
               return DateTime.UtcNow.Subtract(_heartbeat.Received).TotalMinutes >
                      TimeSpan.FromSeconds(HeartbeatInterval * 3).TotalMinutes;
            }
         }

         public Info Info { get; private set; }

         public Options Options { get; private set; }

         public SlotCollection SlotCollection { get; private set; }

         private readonly List<SlotOptions> _slotOptions = new List<SlotOptions>();

         private UnitCollection _previousUnitCollection;

         public UnitCollection UnitCollection { get; private set; }

         public bool LogRetrieved { get; private set; }

         public void Add(MessageReceivedEventArgs args)
         {
            if (args.DataType == typeof(Heartbeat))
            {
               _heartbeat = (Heartbeat)args.TypedMessage;
            }
            else if (args.DataType == typeof(Info))
            {
               Info = (Info)args.TypedMessage;
            }
            else if (args.DataType == typeof(Options))
            {
               Options = (Options)args.TypedMessage;
            }
            else if (args.DataType == typeof(SlotCollection))
            {
               SlotCollection = (SlotCollection)args.TypedMessage;
            }
            else if (args.DataType == typeof(SlotOptions))
            {
               _slotOptions.Add((SlotOptions)args.TypedMessage);
            }
            else if (args.DataType == typeof(UnitCollection))
            {
               UnitCollection = (UnitCollection)args.TypedMessage;
            }
            else if (!LogRetrieved &&
                    (args.DataType == typeof(LogRestart) ||
                     args.DataType == typeof(LogUpdate)))
            {
               var logFragment = (LogFragment)args.TypedMessage;
               if (logFragment.Value.Length < 65536)
               {
                  LogRetrieved = true;
               }
            }
         }

         public Result Process()
         {
            var result = new Result();
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

               result.SlotsUpdated = true;
            }
            if (LogRetrieved)
            {
               if (result.SlotsUpdated)
               {
                  result.ExecuteRetrieval = true;
               }
               if (UnitCollection != null && !_unitCollectionEqualityComparer.Equals(_previousUnitCollection, UnitCollection))
               {
                  _previousUnitCollection = UnitCollection;
                  result.ExecuteRetrieval = true;
               }
            }
            return result;
         }

         public sealed class Result
         {
            public bool SlotsUpdated { get; set; }

            public bool ExecuteRetrieval { get; set; }
         }

         // Compare the work unit collections for equality, ignoring point and frame time properties.
         // This equality compare is used to determine when a unit's progress changes, state changes,
         // or when a work units are added/removed from the queue.
         private class UnitCollectionEqualityComparer : IEqualityComparer<UnitCollection>
         {
            public bool Equals(UnitCollection x, UnitCollection y)
            {
               return x == null ? y == null : y != null && x.SequenceEqual(y, new UnitEqualityComparer());
            }

            public int GetHashCode(UnitCollection obj)
            {
               throw new NotImplementedException();
            }

            private class UnitEqualityComparer : IEqualityComparer<Unit>
            {
               public bool Equals(Unit x, Unit y)
               {
                  return x == null ? y == null : y != null && EqualsInternal(x, y);
               }

               private static bool EqualsInternal(Unit x, Unit y)
               {
                  Debug.Assert(x != null);
                  Debug.Assert(y != null);

                  return x.Id == y.Id &&
                         Equals(x.State, y.State) &&
                         x.Project == y.Project &&
                         x.Run == y.Run &&
                         x.Clone == y.Clone &&
                         x.Gen == y.Gen &&
                         Equals(x.Core, y.Core) &&
                         Equals(x.UnitId, y.UnitId) &&
                         x.TotalFrames == y.TotalFrames &&
                         x.FramesDone == y.FramesDone &&
                         Equals(x.Assigned, y.Assigned) &&
                         Equals(x.Timeout, y.Timeout) &&
                         Equals(x.Deadline, y.Deadline) &&
                         Equals(x.WorkServer, y.WorkServer) &&
                         Equals(x.CollectionServer, y.CollectionServer) &&
                         Equals(x.WaitingOn, y.WaitingOn) &&
                         x.Attempts == y.Attempts &&
                         Equals(x.NextAttempt, y.NextAttempt) &&
                         x.Slot == y.Slot;
               }

               public int GetHashCode(Unit obj)
               {
                  throw new NotImplementedException();
               }
            }
         }
      }

      public void Fold(int? slotId)
      {
         if (!_messageConnection.Connected)
         {
            return;
         }
         string command = slotId.HasValue ? "unpause " + slotId.Value : "unpause";
         _messageConnection.SendCommand(command);
      }

      public void Pause(int? slotId)
      {
         if (!_messageConnection.Connected)
         {
            return;
         }
         string command = slotId.HasValue ? "pause " + slotId.Value : "pause";
         _messageConnection.SendCommand(command);
      }

      public void Finish(int? slotId)
      {
         if (!_messageConnection.Connected)
         {
            return;
         }
         string command = slotId.HasValue ? "finish " + slotId.Value : "finish";
         _messageConnection.SendCommand(command);
      }
   }
}
