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
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Client
{
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

    public class FahClient : Client, IFahClient
    {
        #region Properties

        private ClientSettings _settings;

        public override ClientSettings Settings
        {
            get { return _settings; }
            set
            {
                Debug.Assert(value.ClientType == ClientType.FahClient);

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

        private readonly IMessageConnection _messageConnection;
        private readonly List<SlotModel> _slots;
        private readonly ReaderWriterLockSlim _slotsLock;
        private readonly FahLog _fahLog;
        private MessageReceiver _messages;

        public const string DefaultSlotOptions = "slot-options {0} cpus client-type client-subtype cpu-usage machine-id max-packet-size core-priority next-unit-percentage max-units checkpoint pause-on-start gpu-index gpu-usage";
        
        public FahClient() : this(new TypedMessageConnection())
        {

        }

        public FahClient(IMessageConnection messageConnection)
        {
            if (messageConnection == null) throw new ArgumentNullException("messageConnection");

            _messageConnection = messageConnection;
            _slots = new List<SlotModel>();
            _slotsLock = new ReaderWriterLockSlim();
            _fahLog = new Log.FahClient.FahClientLog();
            _messages = new MessageReceiver();

            _messageConnection.MessageReceived += MessageConnectionMessageReceived;
            _messageConnection.UpdateFinished += MessageConnectionUpdateFinished;
            _messageConnection.ConnectedChanged += MessageConnectionConnectedChanged;
            _messageConnection.StatusMessage += MessageConnectionStatusMessage;
        }

        private void MessageConnectionMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (AbortFlag) return;

            Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, e.JsonMessage.GetHeader()));

            _messages.Add(e);
            if (e.DataType == typeof(SlotCollection))
            {
                foreach (var slot in _messages.SlotCollection)
                {
                    _messageConnection.SendCommand(String.Format(CultureInfo.InvariantCulture, DefaultSlotOptions, slot.Id));
                }
            }
            else if (e.DataType == typeof(LogRestart) ||
                     e.DataType == typeof(LogUpdate))
            {
                bool createNew = e.DataType == typeof(LogRestart);
                if (createNew)
                {
                    _fahLog.Clear();
                }

                var logFragment = (LogFragment)e.TypedMessage;
                using (var textReader = new StringBuilderReader(logFragment.Value))
                using (var reader = new Log.FahClient.FahClientLogTextReader(textReader))
                {
                    _fahLog.Read(reader);
                }
                WriteToLocalFahLogCache(logFragment.Value, createNew);

                if (_messages.LogRetrieved)
                {
                    _messageConnection.SendCommand("queue-info");
                }
            }
        }

        private void WriteToLocalFahLogCache(StringBuilder logText, bool createNew)
        {
            const int sleep = 100;
            const int timeout = 60 * 1000;
            string fahLogPath = Path.Combine(Prefs.Get<string>(Preference.CacheDirectory), Settings.ClientLogFileName);

            try
            {
                using (var stream = Internal.FileSystem.TryFileOpen(fahLogPath, createNew ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read, sleep, timeout))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var chunk in logText.GetChunks())
                    {
                        writer.Write(chunk);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to write to {fahLogPath}", ex);
            }
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

        private void MessageConnectionStatusMessage(object sender, StatusMessageEventArgs e)
        {
            if (e.Exception != null)
            {
                Logger.Warn(e.Status, e.Exception);
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
                    // iterate through slot collection
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
                    Logger.Error(String.Format(Logging.Logger.NameFormat, Settings.Name, ex.Message), ex);
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
                Logger.Error(String.Format(Logging.Logger.NameFormat, Settings.Name, ex.Message), ex);
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
            // get an initial queue reading
            _messageConnection.SendCommand("queue-info");
        }

        private void Process()
        {
            var sw = Stopwatch.StartNew();

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

                    // Run the Aggregator
                    var dataAggregator = new FahClientDataAggregator { Logger = Logger };
                    dataAggregator.ClientName = slotModel.Name;
                    DataAggregatorResult result = dataAggregator.AggregateData(_fahLog.ClientRuns.LastOrDefault(),
                                                                               _messages.UnitCollection,
                                                                               info,
                                                                               options,
                                                                               slotModel.SlotOptions,
                                                                               slotModel.WorkUnit,
                                                                               slotModel.SlotId);
                    PopulateRunLevelData(result, info, slotModel);

                    slotModel.WorkUnitInfos = result.WorkUnitInfos;
                    slotModel.CurrentLogLines = result.CurrentLogLines;
                    //slotModel.UnitLogLines = result.UnitLogLines;

                    var parsedUnits = new Dictionary<int, WorkUnitModel>(result.WorkUnits.Count);
                    foreach (int key in result.WorkUnits.Keys)
                    {
                        if (result.WorkUnits[key] != null)
                        {
                            parsedUnits[key] = BuildWorkUnitModel(slotModel, result.WorkUnits[key]);
                        }
                    }

                    // *** THIS HAS TO BE DONE BEFORE UPDATING SlotModel.WorkUnitModel ***
                    UpdateBenchmarkData(slotModel.WorkUnitModel, parsedUnits.Values);

                    // Update the WorkUnitModel if we have a current unit index
                    if (result.CurrentUnitIndex != -1 && parsedUnits.ContainsKey(result.CurrentUnitIndex))
                    {
                        slotModel.WorkUnitModel = parsedUnits[result.CurrentUnitIndex];
                    }

                    SetSlotStatus(slotModel);

                    slotModel.WorkUnitModel.ShowProductionTrace(Logger, slotModel.Name, slotModel.Status,
                       Prefs.Get<PPDCalculation>(Preference.PPDCalculation),
                       Prefs.Get<BonusCalculation>(Preference.BonusCalculation));

                    string statusMessage = String.Format(CultureInfo.CurrentCulture, "Slot Status: {0}", slotModel.Status);
                    Logger.Info(String.Format(Logging.Logger.NameFormat, slotModel.Name, statusMessage));
                }
            }
            finally
            {
                _slotsLock.ExitReadLock();
            }

            string message = String.Format(CultureInfo.CurrentCulture, "Retrieval finished: {0}", sw.GetExecTime());
            Logger.Info(String.Format(Logging.Logger.NameFormat, Settings.Name, message));
        }

        private WorkUnitModel BuildWorkUnitModel(SlotModel slotModel, WorkUnit workUnit)
        {
            Debug.Assert(slotModel != null);
            Debug.Assert(workUnit != null);

            Protein protein = ProteinService.GetOrRefresh(workUnit.ProjectID) ?? new Protein();

            // update the data
            workUnit.UnitRetrievalTime = LastRetrievalTime;
            workUnit.OwningClientName = Settings.Name;
            workUnit.OwningClientPath = Settings.ClientPath;
            workUnit.OwningSlotId = slotModel.SlotId;
            if (workUnit.SlotType == SlotType.Unknown)
            {
                workUnit.SlotType = protein.Core.ToSlotType();
                if (workUnit.SlotType == SlotType.Unknown)
                {
                    workUnit.SlotType = workUnit.CoreID.ToSlotType();
                }
            }
            
            var workUnitModel = new WorkUnitModel(BenchmarkService);
            workUnitModel.CurrentProtein = protein;
            workUnitModel.Data = workUnit;
            return workUnitModel;
        }

        private static void SetSlotStatus(SlotModel slotModel)
        {
            if (slotModel.Status == SlotStatus.Running ||
                slotModel.Status == SlotStatus.RunningNoFrameTimes)
            {
                slotModel.Status = slotModel.IsUsingBenchmarkFrameTime ? SlotStatus.RunningNoFrameTimes : SlotStatus.Running;
            }
        }

        private void PopulateRunLevelData(DataAggregatorResult result, Info info, SlotModel slotModel)
        {
            Debug.Assert(slotModel != null);

            if (info != null)
            {
                slotModel.ClientVersion = info.Build.Version;
            }
            if (WorkUnitRepository.Connected)
            {
                slotModel.TotalRunCompletedUnits = (int)WorkUnitRepository.CountCompleted(slotModel.Name, result.StartTime);
                slotModel.TotalCompletedUnits = (int)WorkUnitRepository.CountCompleted(slotModel.Name, null);
                slotModel.TotalRunFailedUnits = (int)WorkUnitRepository.CountFailed(slotModel.Name, result.StartTime);
                slotModel.TotalFailedUnits = (int)WorkUnitRepository.CountFailed(slotModel.Name, null);
            }
        }

        internal void UpdateBenchmarkData(WorkUnitModel currentWorkUnit, IEnumerable<WorkUnitModel> parsedUnits)
        {
            foreach (var model in parsedUnits.Where(x => x != null))
            {
                if (currentWorkUnit.Data.EqualsProjectAndDownloadTime(model.Data))
                {
                    // found the current unit
                    // current frame has already been recorded, increment to the next frame
                    int nextFrame = currentWorkUnit.FramesComplete + 1;
                    int count = model.FramesComplete - currentWorkUnit.FramesComplete;
                    var frameTimes = GetFrameTimes(model.Data, nextFrame, count);
                    // Update benchmarks
                    BenchmarkService.Update(model.Data.SlotIdentifier, model.Data.ProjectID, frameTimes);
                }
                // Update history database
                if (model.Data.UnitResult != WorkUnitResult.Unknown)
                {
                    InsertCompletedWorkUnit(model);
                }
            }
        }

        private static IEnumerable<TimeSpan> GetFrameTimes(WorkUnit workUnit, int nextFrame, int count)
        {
            return Enumerable.Range(nextFrame, count)
                .Select(workUnit.GetFrameData)
                .Where(f => f != null)
                .Select(f => f.Duration)
                .ToList();
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

            // TODO: use ValueTuple
            internal struct Result
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
                    return x == null ? y == null : y != null && (ReferenceEquals(x, y) || x.SequenceEqual(y, new UnitEqualityComparer()));
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

                        int xPercentDone = GetPercentDone(x.PercentDone);
                        int yPercentDone = GetPercentDone(y.PercentDone);

                        return x.Id == y.Id &&
                               x.State == y.State &&
                               x.Project == y.Project &&
                               x.Run == y.Run &&
                               x.Clone == y.Clone &&
                               x.Gen == y.Gen &&
                               x.Core == y.Core &&
                               x.UnitId == y.UnitId &&
                               xPercentDone == yPercentDone &&
                               x.TotalFrames == y.TotalFrames &&
                               x.FramesDone == y.FramesDone &&
                               x.Assigned == y.Assigned &&
                               x.Timeout == y.Timeout &&
                               x.Deadline == y.Deadline &&
                               x.WorkServer == y.WorkServer &&
                               x.CollectionServer == y.CollectionServer &&
                               x.WaitingOn == y.WaitingOn &&
                               x.Attempts == y.Attempts &&
                               x.NextAttempt == y.NextAttempt &&
                               x.Slot == y.Slot;
                    }

                    private static int GetPercentDone(string value)
                    {
                        if (value == null)
                        {
                            return 0;
                        }
                        double result;
                        if (Double.TryParse(value.TrimEnd('%'), out result))
                        {
                            return (int)Math.Round(result, MidpointRounding.AwayFromZero);
                        }
                        return 0;
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
