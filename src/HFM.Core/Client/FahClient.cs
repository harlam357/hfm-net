using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HFM.Client;
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

            if (oldSettings != null)
            {
                if (oldSettings.Server != newSettings.Server ||
                    oldSettings.Port != newSettings.Port ||
                    oldSettings.Password != newSettings.Password ||
                    oldSettings.Disabled != newSettings.Disabled)
                {
                    // close existing connection and allow retrieval to open a new connection
                    Connection?.Close();
                }
                else
                {
                    RefreshSlots();
                }
            }
        }

        public IProteinService ProteinService { get; }
        public IWorkUnitRepository WorkUnitRepository { get; }
        public FahClientMessages Messages { get; protected set; }
        public FahClientConnection Connection { get; protected set; }

        public FahClient(ILogger logger,
                         IPreferences preferences,
                         IProteinBenchmarkService benchmarkService,
                         IProteinService proteinService,
                         IWorkUnitRepository workUnitRepository)
            : base(logger, preferences, benchmarkService)
        {
            ProteinService = proteinService;
            WorkUnitRepository = workUnitRepository;
            Messages = new FahClientMessages(this);
            _messageActions = new List<FahClientMessageAction>
            {
                new DelegateFahClientMessageAction(FahClientMessageType.SlotInfo, RefreshSlots),
                new DelegateFahClientMessageAction(FahClientMessageType.Info, RefreshClientInfo),
                new ExecuteRetrieveMessageAction(Messages, async () => await Retrieve().ConfigureAwait(false))
            };
        }

        private readonly List<FahClientMessageAction> _messageActions;

        protected virtual async Task OnMessageRead(FahClientMessage message)
        {
            if (IsCancellationRequested) return;

            Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, $"{message.Identifier} - Length: {message.MessageText.Length}"));

            bool updated = await Messages.UpdateMessageAsync(message).ConfigureAwait(false);
            if (updated)
            {
                _messageActions.ForEach(x => x.Execute(message.Identifier.MessageType));
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

        protected override void OnRefreshSlots(ICollection<SlotModel> slots)
        {
            var slotCollection = Messages?.SlotCollection;
            if (slotCollection is { Count: > 0 })
            {
                foreach (var slot in slotCollection)
                {
                    var slotDescription = SlotDescription.Parse(slot.Description);
                    var status = (SlotStatus)Enum.Parse(typeof(SlotStatus), slot.Status, true);
                    var slotID = slot.ID.GetValueOrDefault();
                    var slotType = slotDescription?.SlotType ?? default;
                    var slotModel = new FahClientSlotModel(this, status, slotID, slotType);
                    switch (slotDescription)
                    {
                        case GPUSlotDescription g:
                            slotModel.Processor = g.GPU;
                            break;
                        case CPUSlotDescription c:
                            slotModel.Threads = c.CPUThreads;
                            break;
                    }
                    slots.Add(slotModel);
                }
            }
            else
            {
                base.OnRefreshSlots(slots);
            }
        }

        private void RefreshClientInfo() => ClientVersion = Messages.Info?.Client.Version;

        protected override void OnCancel()
        {
            base.OnCancel();

            if (Connected)
            {
                Connection.Close();
            }
        }

        public override bool Connected => Connection is { Connected: true };

        protected override async Task OnConnect()
        {
            await CreateAndOpenConnection().ConfigureAwait(false);

            _ = Task.Run(async () => await ReadMessagesFromConnection().ConfigureAwait(false))
                .ContinueWith(async _ => await CloseConnection().ConfigureAwait(false),
                    CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
        }

        private async Task CreateAndOpenConnection()
        {
            Connection = new FahClientConnection(Settings.Server, Settings.Port);
            await Connection.OpenAsync().ConfigureAwait(false);
            if (!String.IsNullOrWhiteSpace(Settings.Password))
            {
                await Connection.CreateCommand("auth " + Settings.Password).ExecuteAsync().ConfigureAwait(false);
            }
            await OnConnectedChanged(Connection.Connected).ConfigureAwait(false);
        }

        private async Task ReadMessagesFromConnection()
        {
            var reader = Connection.CreateReader();
            try
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    await OnMessageRead(reader.Message).ConfigureAwait(false);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, ex.Message), ex);
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format(Logging.Logger.NameFormat, Settings.Name, ex.Message), ex);
            }
        }

        private async Task CloseConnection()
        {
            try
            {
                Connection.Close();
                await OnConnectedChanged(Connection.Connected).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format(Logging.Logger.NameFormat, Settings.Name, ex.Message), ex);
            }
        }

        protected override Task OnRetrieve()
        {
            if (Messages.IsHeartbeatOverdue())
            {
                Cancel();
                return Task.CompletedTask;
            }

            Process();
            return Task.CompletedTask;
        }

        protected override void OnRetrieveFinished()
        {
            if (!IsCancellationRequested) base.OnRetrieveFinished();
        }

        private void Process()
        {
            var sw = Stopwatch.StartNew();

            var workUnitsBuilder = new WorkUnitCollectionBuilder(
                Logger, Settings, Messages.UnitCollection, Messages.Options, Messages.GetClientRun(), LastRetrieveTime);
            var workUnitQueueBuilder = new WorkUnitQueueItemCollectionBuilder(
                Messages.UnitCollection, Messages.Info?.System);

            foreach (var slotModel in Slots.OfType<FahClientSlotModel>())
            {
                var previousWorkUnitModel = slotModel.WorkUnitModel;
                var workUnits = workUnitsBuilder.BuildForSlot(slotModel.SlotID, previousWorkUnitModel.WorkUnit);
                var workUnitModels = new WorkUnitModelCollection(workUnits.Select(x => BuildWorkUnitModel(slotModel, x)));

                PopulateSlotModel(slotModel, workUnits, workUnitModels, workUnitQueueBuilder);
                UpdateWorkUnitBenchmarkAndRepository(workUnitModels, previousWorkUnitModel);

                slotModel.WorkUnitModel.ShowProductionTrace(Logger, slotModel.Name, slotModel.Status,
                   Preferences.Get<PPDCalculation>(Preference.PPDCalculation),
                   Preferences.Get<BonusCalculation>(Preference.BonusCalculation));

                string statusMessage = String.Format(CultureInfo.CurrentCulture, "Slot Status: {0}", slotModel.Status);
                Logger.Info(String.Format(Logging.Logger.NameFormat, slotModel.Name, statusMessage));
            }

            string message = String.Format(CultureInfo.CurrentCulture, "Retrieval finished: {0}", sw.GetExecTime());
            Logger.Info(String.Format(Logging.Logger.NameFormat, Settings.Name, message));
        }

        private IReadOnlyCollection<LogLine> EnumerateSlotModelLogLines(int slotID, WorkUnitCollection workUnits)
        {
            IEnumerable<LogLine> logLines = workUnits.Current?.LogLines;

            if (logLines is null)
            {
                var slotRun = Messages.GetSlotRun(slotID);
                if (slotRun != null)
                {
                    logLines = LogLineEnumerable.Create(slotRun);
                }
            }

            if (logLines is null)
            {
                var clientRun = Messages.GetClientRun();
                if (clientRun != null)
                {
                    logLines = LogLineEnumerable.Create(clientRun);
                }
            }

            return logLines is null ? Array.Empty<LogLine>() : logLines.ToList();
        }

        private WorkUnitModel BuildWorkUnitModel(SlotModel slotModel, WorkUnit workUnit)
        {
            Debug.Assert(slotModel != null);
            Debug.Assert(workUnit != null);

            var protein = ProteinService?.GetOrRefresh(workUnit.ProjectID) ?? new Protein();
            return new WorkUnitModel(slotModel, workUnit)
            {
                CurrentProtein = protein
            };
        }

        private void PopulateSlotModel(FahClientSlotModel slotModel,
                                       WorkUnitCollection workUnits,
                                       WorkUnitModelCollection workUnitModels,
                                       WorkUnitQueueItemCollectionBuilder workUnitQueueBuilder)
        {
            Debug.Assert(slotModel != null);
            Debug.Assert(workUnits != null);
            Debug.Assert(workUnitModels != null);
            Debug.Assert(workUnitQueueBuilder != null);

            if (slotModel.SlotType == SlotType.CPU)
            {
                slotModel.Processor = Messages.Info?.System?.CPU;
            }
            slotModel.WorkUnitQueue = workUnitQueueBuilder.BuildForSlot(slotModel.SlotID);
            slotModel.CurrentLogLines = EnumerateSlotModelLogLines(slotModel.SlotID, workUnits);

            var clientRun = Messages.GetClientRun();
            if (WorkUnitRepository is { Connected: true } && clientRun != null)
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
            if (WorkUnitRepository is { Connected: true })
            {
                try
                {
                    if (WorkUnitRepository.Insert(workUnitModel) > 0)
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

        private static ICollection<TimeSpan> GetFrameTimes(WorkUnit workUnit, int nextFrame, int count) =>
            Enumerable.Range(nextFrame, count)
                .Select(workUnit.GetFrame)
                .Where(f => f != null)
                .Select(f => f.Duration)
                .ToList();

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
