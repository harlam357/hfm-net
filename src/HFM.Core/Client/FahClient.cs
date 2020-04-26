
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Client
{
    public interface IFahClient : IClient
    {
        FahClientConnection Connection { get; }

        FahClientMessages Messages { get; }

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
        protected override void OnSettingsChanged(ClientSettings oldSettings, ClientSettings newSettings)
        {
            Debug.Assert(newSettings.ClientType == ClientType.FahClient);

            // settings already exist
            if (oldSettings != null)
            {
                if (oldSettings.Server != newSettings.Server ||
                    oldSettings.Port != newSettings.Port ||
                    oldSettings.Password != newSettings.Password)
                {
                    // close existing connection and allow retrieval to open a new connection
                    Connection?.Close();
                }
                else
                {
                    // refresh the slots with the updated settings
                    RefreshSlots();
                }
            }
        }

        protected override IEnumerable<SlotModel> OnEnumerateSlots()
        {
            _slotsLock.EnterReadLock();
            try
            {
                // not connected or no slots
                if (_slots.Count == 0)
                {
                    // return default slot (for grid binding)
                    return new[] { new SlotModel(this) { Status = SlotStatus.Offline } };
                }
                return _slots.ToArray();
            }
            finally
            {
                _slotsLock.ExitReadLock();
            }
        }

        public IProteinService ProteinService { get; }
        public IWorkUnitRepository WorkUnitRepository { get; }
        public FahClientMessages Messages { get; }
        public FahClientConnection Connection { get; private set; }

        private readonly List<SlotModel> _slots;
        private readonly ReaderWriterLockSlim _slotsLock;

        public FahClient(ILogger logger, IPreferenceSet preferences, IProteinBenchmarkService benchmarkService, 
            IProteinService proteinService, IWorkUnitRepository workUnitRepository) : base(logger, preferences, benchmarkService)
        {
            ProteinService = proteinService;
            WorkUnitRepository = workUnitRepository;
            Messages = new FahClientMessages(this);

            _slots = new List<SlotModel>();
            _slotsLock = new ReaderWriterLockSlim();
        }

        protected virtual async Task OnMessageRead(FahClientMessage message)
        {
            if (AbortFlag) return;

            Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, $"{message.Identifier} - Length: {message.MessageText.Length}"));

            var result = await Messages.UpdateMessageAsync(message).ConfigureAwait(false);
            if (result.SlotsUpdated)
            {
                RefreshSlots();
            }
            if (result.ExecuteRetrieval)
            {
                // Process the retrieved logs
                Retrieve();
            }
        }

        protected virtual async Task OnConnectedChanged(bool connected)
        {
            if (connected)
            {
                await Messages.SetupClientToSendMessageUpdatesAsync().ConfigureAwait(false);
            }
            else
            {
                // reset messages
                Messages.Clear();
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
                if (Messages.SlotCollection != null)
                {
                    // iterate through slot collection
                    foreach (var slot in Messages.SlotCollection)
                    {
                        // add slot model to the collection
                        var slotModel = new SlotModel(this)
                        {
                            Status = (SlotStatus)Enum.Parse(typeof(SlotStatus), slot.Status, true),
                            SlotID = slot.ID.GetValueOrDefault()
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

            if (Connection != null && Connection.Connected)
            {
                Connection.Close();
            }
        }

        protected override async void OnRetrieve()
        {
            if (Messages.IsHeartbeatOverdue())
            {
                // haven't seen a heartbeat
                Abort();
            }

            // connect if not connected
            if (Connection is null || !Connection.Connected)
            {
                try
                {
                    Connection = new FahClientConnection(Settings.Server, Settings.Port);
                    await Connection.OpenAsync().ConfigureAwait(false);
                    if (!String.IsNullOrWhiteSpace(Settings.Password))
                    {
                        await Connection.CreateCommand("auth " + Settings.Password).ExecuteAsync().ConfigureAwait(false);
                    }
                    await OnConnectedChanged(Connection.Connected).ConfigureAwait(false);

                    var reader = Connection.CreateReader();
                    try
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            await OnMessageRead(reader.Message).ConfigureAwait(false);
                        }
                    }
                    catch (Exception)
                    {
                        // connection died
                    }
                    Connection.Close();
                    await OnConnectedChanged(Connection.Connected).ConfigureAwait(false);
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

        private void Process()
        {
            var sw = Stopwatch.StartNew();

            // Set successful Last Retrieval Time
            LastRetrievalTime = DateTime.Now;

            _slotsLock.EnterReadLock();
            try
            {
                foreach (var slotModel in _slots)
                {
                    // Re-Init Slot Level Members Before Processing
                    slotModel.Initialize();

                    var aggregator = new FahClientMessageAggregator(this, slotModel);
                    var result = aggregator.AggregateData();
                    PopulateRunLevelData(result, Messages.Info, slotModel);

                    slotModel.WorkUnitInfos = result.WorkUnitInfos;
                    slotModel.CurrentLogLines = result.CurrentLogLines;
                    //slotModel.UnitLogLines = result.UnitLogLines;

                    var newWorkUnitModels = new Dictionary<int, WorkUnitModel>(result.WorkUnits.Count);
                    foreach (int key in result.WorkUnits.Keys)
                    {
                        if (result.WorkUnits[key] != null)
                        {
                            newWorkUnitModels[key] = BuildWorkUnitModel(slotModel, result.WorkUnits[key]);
                        }
                    }

                    // *** THIS HAS TO BE DONE BEFORE UPDATING SlotModel.WorkUnitModel ***
                    UpdateWorkUnitBenchmarkAndRepository(slotModel.WorkUnitModel, newWorkUnitModels.Values);

                    // Update the WorkUnitModel if we have a current unit index
                    if (result.CurrentUnitIndex != -1 && newWorkUnitModels.ContainsKey(result.CurrentUnitIndex))
                    {
                        slotModel.WorkUnitModel = newWorkUnitModels[result.CurrentUnitIndex];
                    }

                    SetSlotStatus(slotModel);

                    slotModel.WorkUnitModel.ShowProductionTrace(Logger, slotModel.Name, slotModel.Status,
                       Preferences.Get<PPDCalculation>(Preference.PPDCalculation),
                       Preferences.Get<BonusCalculation>(Preference.BonusCalculation));

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
            if (workUnit.SlotType == SlotType.Unknown)
            {
                workUnit.SlotType = SlotTypeConvert.FromCoreName(protein.Core);
                if (workUnit.SlotType == SlotType.Unknown)
                {
                    workUnit.SlotType = SlotTypeConvert.FromCoreId(workUnit.CoreID);
                }
            }

            var workUnitModel = new WorkUnitModel(slotModel, workUnit);
            workUnitModel.CurrentProtein = protein;
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

        private void PopulateRunLevelData(ClientMessageAggregatorResult result, Info info, SlotModel slotModel)
        {
            Debug.Assert(slotModel != null);

            if (info != null)
            {
                // TODO: Surface client arguments?
                //slotModel.Arguments = info.Client.Args;
                slotModel.ClientVersion = info.Client.Version;
            }
            if (WorkUnitRepository.Connected)
            {
                slotModel.TotalRunCompletedUnits = (int)WorkUnitRepository.CountCompleted(slotModel.Name, result.StartTime);
                slotModel.TotalCompletedUnits = (int)WorkUnitRepository.CountCompleted(slotModel.Name, null);
                slotModel.TotalRunFailedUnits = (int)WorkUnitRepository.CountFailed(slotModel.Name, result.StartTime);
                slotModel.TotalFailedUnits = (int)WorkUnitRepository.CountFailed(slotModel.Name, null);
            }
        }

        internal void UpdateWorkUnitBenchmarkAndRepository(WorkUnitModel workUnitModel, IEnumerable<WorkUnitModel> newWorkUnitModels)
        {
            foreach (var m in newWorkUnitModels.Where(x => x != null))
            {
                // find the WorkUnit in newWorkUnitModels that matches the current WorkUnit
                if (workUnitModel.WorkUnit.EqualsProjectAndDownloadTime(m.WorkUnit))
                {
                    UpdateBenchmarkFrameTimes(workUnitModel, m);
                }
                if (m.WorkUnit.UnitResult != WorkUnitResult.Unknown)
                {
                    InsertCompletedWorkUnitIntoRepository(m);
                }
            }
        }

        private void UpdateBenchmarkFrameTimes(WorkUnitModel workUnitModel, WorkUnitModel newWorkUnitModel)
        {
            // current frame has already been recorded, increment to the next frame
            int nextFrame = workUnitModel.FramesComplete + 1;
            int count = newWorkUnitModel.FramesComplete - workUnitModel.FramesComplete;
            var frameTimes = GetFrameTimes(newWorkUnitModel.WorkUnit, nextFrame, count);
            
            BenchmarkService.Update(newWorkUnitModel.SlotModel.SlotIdentifier, newWorkUnitModel.WorkUnit.ProjectID, frameTimes);
        }

        private void InsertCompletedWorkUnitIntoRepository(WorkUnitModel workUnitModel)
        {
            if (WorkUnitRepository != null && WorkUnitRepository.Connected)
            {
                try
                {
                    if (WorkUnitRepository.Insert(workUnitModel))
                    {
                        if (Logger.IsDebugEnabled)
                        {
                            string message = $"Inserted {workUnitModel.WorkUnit.ToProjectString()} into database.";
                            Logger.Debug(String.Format(Logging.Logger.NameFormat, workUnitModel.SlotModel.SlotIdentifier.Name, message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
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

        public void Fold(int? slotId)
        {
            if (!Connection.Connected)
            {
                return;
            }
            string command = slotId.HasValue ? "unpause " + slotId.Value : "unpause";
            Connection.CreateCommand(command).Execute();
        }

        public void Pause(int? slotId)
        {
            if (!Connection.Connected)
            {
                return;
            }
            string command = slotId.HasValue ? "pause " + slotId.Value : "pause";
            Connection.CreateCommand(command).Execute();
        }

        public void Finish(int? slotId)
        {
            if (!Connection.Connected)
            {
                return;
            }
            string command = slotId.HasValue ? "finish " + slotId.Value : "finish";
            Connection.CreateCommand(command).Execute();
        }
    }
}
