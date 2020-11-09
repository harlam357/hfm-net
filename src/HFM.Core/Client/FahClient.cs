using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Client
{
    public interface IFahClient : IClient
    {
        FahClientConnection Connection { get; }
    }

    public class FahClient : Client, IFahClient, IFahClientCommand
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

        public FahClient(ILogger logger, IPreferences preferences, IProteinBenchmarkService benchmarkService,
                         IProteinService proteinService, IWorkUnitRepository workUnitRepository)
            : base(logger, preferences, benchmarkService)
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

        internal void RefreshSlots()
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
                        var slotDescription = ParseSlotDescription(slot.Description);
                        // add slot model to the collection
                        var slotModel = new SlotModel(this)
                        {
                            Status = (SlotStatus)Enum.Parse(typeof(SlotStatus), slot.Status, true),
                            SlotID = slot.ID.GetValueOrDefault(),
                            SlotType = slotDescription.SlotType,
                            SlotThreads = slotDescription.CPUThreads,
                            SlotProcessor = slotDescription.GPU
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

        private static (SlotType SlotType, int? CPUThreads, string GPU, int? GPUBus, int? GPUSlot) ParseSlotDescription(string description)
        {
            if (description is null) return (SlotType.CPU, null, null, null, null);

            var slotType = ConvertToSlotType.FromSlotDescription(description);

            string gpu = null;
            int? gpuBus = null;
            int? gpuSlot = null;
            int? cpuThreads = null;

            if (slotType == SlotType.GPU)
            {
                gpu = GetGPUFromDescription(description);
                (gpuBus, gpuSlot) = GetGPUBusAndSlotFromDescription(description);
            }
            else
            {
                Debug.Assert(slotType == SlotType.CPU);
                cpuThreads = GetCPUThreadsFromDescription(description);
            }

            return (slotType, cpuThreads, gpu, gpuBus, gpuSlot);
        }

        private static string GetGPUFromDescription(string description)
        {
            var match = Regex.Match(description, "\\[(?<GPU>.+)\\]", RegexOptions.Singleline);
            return match.Success ? match.Groups["GPU"].Value : null;
        }

        private static (int? GPUBus, int? GPUSlot) GetGPUBusAndSlotFromDescription(string description)
        {
            int? gpuBus = null;
            int? gpuSlot = null;

            var match = Regex.Match(description, @"gpu\:(?<GPUBus>\d+)\:(?<GPUSlot>\d+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success &&
                Int32.TryParse(match.Groups["GPUBus"].Value, out var bus) &&
                Int32.TryParse(match.Groups["GPUSlot"].Value, out var slot))
            {
                gpuBus = bus;
                gpuSlot = slot;
            }

            return (gpuBus, gpuSlot);
        }

        private static int? GetCPUThreadsFromDescription(string description)
        {
            var match = Regex.Match(description, @"[cpu|smp]\:(?<CPUThreads>\d+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return match.Success && Int32.TryParse(match.Groups["CPUThreads"].Value, out var threads)
                ? (int?)threads
                : null;
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
                    catch (Exception ex)
                    {
                        // connection died
                        Logger.Error(String.Format(Logging.Logger.NameFormat, Settings.Name, ex.Message), ex);
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
                var workUnitsBuilder = new WorkUnitCollectionBuilder(
                    Logger, Settings, Messages.UnitCollection, Messages.Options, Messages.GetClientRun(), LastRetrievalTime);
                var workUnitQueueBuilder = new WorkUnitQueueItemCollectionBuilder(
                    Messages.UnitCollection, Messages.Info?.System);

                foreach (var slotModel in _slots)
                {
                    var previousWorkUnitModel = slotModel.WorkUnitModel;
                    var workUnits = workUnitsBuilder.BuildForSlot(slotModel.SlotID, previousWorkUnitModel.WorkUnit);
                    var workUnitModels = new WorkUnitModelCollection(workUnits.Select(x => BuildWorkUnitModel(slotModel, x)));

                    PopulateSlotModel(slotModel, workUnits, workUnitModels, workUnitQueueBuilder);
                    UpdateWorkUnitBenchmarkAndRepository(workUnitModels, previousWorkUnitModel);
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

        private static string GetSlotProcessor(SystemInfo systemInfo, SlotModel slotModel) =>
            slotModel.SlotType == SlotType.GPU
                ? slotModel.SlotProcessor
                : systemInfo.CPU;

        private IEnumerable<LogLine> EnumerateSlotModelLogLines(int slotID, WorkUnitCollection workUnits)
        {
            if (workUnits.Current?.LogLines != null)
            {
                return workUnits.Current.LogLines;
            }

            var slotRun = Messages.GetSlotRun(slotID);
            if (slotRun != null)
            {
                return LogLineEnumerable.Create(slotRun);
            }

            var clientRun = Messages.GetClientRun();
            if (clientRun != null)
            {
                return LogLineEnumerable.Create(clientRun);
            }

            return Array.Empty<LogLine>();
        }

        private WorkUnitModel BuildWorkUnitModel(SlotModel slotModel, WorkUnit workUnit)
        {
            Debug.Assert(slotModel != null);
            Debug.Assert(workUnit != null);

            Protein protein = ProteinService.GetOrRefresh(workUnit.ProjectID) ?? new Protein();

            // update the data
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

        private void PopulateSlotModel(SlotModel slotModel, WorkUnitCollection workUnits,
            WorkUnitModelCollection workUnitModels, WorkUnitQueueItemCollectionBuilder workUnitQueueBuilder)
        {
            Debug.Assert(slotModel != null);
            Debug.Assert(workUnits != null);
            Debug.Assert(workUnitModels != null);
            Debug.Assert(workUnitQueueBuilder != null);

            var slotProcessor = GetSlotProcessor(Messages.Info?.System, slotModel);

            slotModel.ClientVersion = Messages.Info?.Client.Version;
            slotModel.SlotProcessor = slotProcessor;
            slotModel.WorkUnitQueue = workUnitQueueBuilder.BuildForSlot(slotModel.SlotID, slotProcessor);
            slotModel.CurrentLogLines.Reset(EnumerateSlotModelLogLines(slotModel.SlotID, workUnits));

            var clientRun = Messages.GetClientRun();
            if (WorkUnitRepository.Connected && clientRun != null)
            {
                slotModel.TotalRunCompletedUnits = (int)WorkUnitRepository.CountCompleted(slotModel.Name, clientRun.Data.StartTime);
                slotModel.TotalCompletedUnits = (int)WorkUnitRepository.CountCompleted(slotModel.Name, null);
                slotModel.TotalRunFailedUnits = (int)WorkUnitRepository.CountFailed(slotModel.Name, clientRun.Data.StartTime);
                slotModel.TotalFailedUnits = (int)WorkUnitRepository.CountFailed(slotModel.Name, null);
            }

            // Update the WorkUnitModel if we have a current unit index
            if (workUnits.CurrentID != WorkUnitCollection.NoID && workUnitModels.ContainsID(workUnits.CurrentID))
            {
                slotModel.WorkUnitModel = workUnitModels[workUnits.CurrentID];
            }
        }

        internal void UpdateWorkUnitBenchmarkAndRepository(IEnumerable<WorkUnitModel> workUnitModels, WorkUnitModel previousWorkUnitModel)
        {
            foreach (var m in workUnitModels)
            {
                // find the WorkUnit in workUnitModels that matches the previousWorkUnitModel
                if (previousWorkUnitModel.WorkUnit.EqualsProjectAndDownloadTime(m.WorkUnit))
                {
                    UpdateBenchmarkFrameTimes(previousWorkUnitModel, m);
                }
                if (m.WorkUnit.UnitResult != WorkUnitResult.Unknown)
                {
                    InsertCompletedWorkUnitIntoRepository(m);
                }
            }
        }

        internal void UpdateBenchmarkFrameTimes(WorkUnitModel previousWorkUnitModel, WorkUnitModel workUnitModel)
        {
            // current frame has already been recorded, increment to the next frame
            int nextFrame = previousWorkUnitModel.FramesComplete + 1;
            int count = workUnitModel.FramesComplete - previousWorkUnitModel.FramesComplete;
            var frameTimes = GetFrameTimes(workUnitModel.WorkUnit, nextFrame, count);

            if (frameTimes.Count > 0)
            {
                var slotIdentifier = workUnitModel.SlotModel.SlotIdentifier;
                var benchmarkIdentifier = workUnitModel.BenchmarkIdentifier;
                BenchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);
            }
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

        private static ICollection<TimeSpan> GetFrameTimes(WorkUnit workUnit, int nextFrame, int count)
        {
            return Enumerable.Range(nextFrame, count)
                .Select(workUnit.GetFrame)
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
