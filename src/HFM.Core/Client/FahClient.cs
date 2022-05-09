using System.Diagnostics;
using System.Globalization;

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
                         IProteinBenchmarkRepository benchmarks,
                         IProteinService proteinService,
                         IWorkUnitRepository workUnitRepository)
            : base(logger, preferences, benchmarks)
        {
            ProteinService = proteinService;
            WorkUnitRepository = workUnitRepository;
            Messages = new FahClientMessages(this);
            _messageActions = new List<FahClientMessageAction>
            {
                new DelegateFahClientMessageAction(FahClientMessageType.SlotInfo, RefreshSlots),
                new DelegateFahClientMessageAction(FahClientMessageType.Info, RefreshClientPlatform),
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
                    var slotModel = new FahClientSlotModel(this, status, slotID);
                    slotModel.Description = slotDescription;
                    slots.Add(slotModel);
                }
            }
            else
            {
                base.OnRefreshSlots(slots);
            }
        }

        private void RefreshClientPlatform()
        {
            var info = Messages.Info;
            if (info is not null)
            {
                Platform = new ClientPlatform(info.Client.Version, info.System.OS);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();

            if (Connected)
            {
                try
                {
                    Connection.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(String.Format(Logging.Logger.NameFormat, Settings.Name, ex.Message), ex);
                }
            }

            // reset messages
            Messages.Clear();
            // refresh (clear) the slots
            RefreshSlots();
        }

        public override bool Connected => Connection is { Connected: true };

        protected override async Task OnConnect()
        {
            await CreateAndOpenConnection().ConfigureAwait(false);

            _ = Task.Run(async () => await ReadMessagesFromConnection().ConfigureAwait(false))
                .ContinueWith(_ => Close(),
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
            if (Connected)
            {
                await SetupClientToSendMessageUpdatesAsync().ConfigureAwait(false);
            }
        }

        internal async Task SetupClientToSendMessageUpdatesAsync()
        {
            var heartbeatCommandText = String.Format(CultureInfo.InvariantCulture, "updates add 0 {0} $heartbeat", FahClientMessages.HeartbeatInterval);

            await Connection.CreateCommand("updates clear").ExecuteAsync().ConfigureAwait(false);
            await Connection.CreateCommand("log-updates restart").ExecuteAsync().ConfigureAwait(false);
            await Connection.CreateCommand(heartbeatCommandText).ExecuteAsync().ConfigureAwait(false);
            await Connection.CreateCommand("updates add 1 1 $info").ExecuteAsync().ConfigureAwait(false);
            await Connection.CreateCommand("updates add 2 1 $(options -a)").ExecuteAsync().ConfigureAwait(false);
            await Connection.CreateCommand("updates add 3 1 $slot-info").ExecuteAsync().ConfigureAwait(false);
            // get an initial queue reading
            await Connection.CreateCommand("queue-info").ExecuteAsync().ConfigureAwait(false);
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

        protected override async Task OnRetrieve()
        {
            if (Messages.IsHeartbeatOverdue())
            {
                Close();
            }

            await Process().ConfigureAwait(false);
        }

        protected override void OnRetrieveFinished()
        {
            if (!IsCancellationRequested) base.OnRetrieveFinished();
        }

        private async Task Process()
        {
            var sw = Stopwatch.StartNew();

            var workUnitsBuilder = new WorkUnitCollectionBuilder(Logger, Settings, Messages, LastRetrieveTime);
            var workUnitQueueBuilder = new WorkUnitQueueItemCollectionBuilder(
                Messages.UnitCollection, Messages.Info?.System);

            foreach (var slotModel in Slots.OfType<FahClientSlotModel>())
            {
                var previousWorkUnitModel = slotModel.WorkUnitModel;
                var workUnits = workUnitsBuilder.BuildForSlot(slotModel.SlotID, slotModel.Description, previousWorkUnitModel.WorkUnit);
                var workUnitModels = new WorkUnitModelCollection(workUnits.Select(x => BuildWorkUnitModel(slotModel, x)));

                await PopulateSlotModel(slotModel, workUnits, workUnitModels, workUnitQueueBuilder).ConfigureAwait(false);
                UpdateWorkUnitBenchmarkAndRepository(workUnitModels);

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
                var clientRun = Messages.ClientRun;
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

        private async Task PopulateSlotModel(FahClientSlotModel slotModel,
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
                slotModel.Description.Processor = Messages.Info?.System?.CPU;
            }
            slotModel.WorkUnitQueue = workUnitQueueBuilder.BuildForSlot(slotModel.SlotID);
            slotModel.CurrentLogLines = EnumerateSlotModelLogLines(slotModel.SlotID, workUnits);

            var clientRun = Messages.ClientRun;
            if (WorkUnitRepository is not null && clientRun is not null)
            {
                slotModel.TotalRunCompletedUnits = (int)await WorkUnitRepository.CountCompletedAsync(slotModel.Name, clientRun.Data.StartTime).ConfigureAwait(false);
                slotModel.TotalCompletedUnits = (int)await WorkUnitRepository.CountCompletedAsync(slotModel.Name, null).ConfigureAwait(false);
                slotModel.TotalRunFailedUnits = (int)await WorkUnitRepository.CountFailedAsync(slotModel.Name, clientRun.Data.StartTime).ConfigureAwait(false);
                slotModel.TotalFailedUnits = (int)await WorkUnitRepository.CountFailedAsync(slotModel.Name, null).ConfigureAwait(false);
            }

            // Update the WorkUnitModel if we have a current unit index
            if (workUnits.CurrentID != WorkUnitCollection.NoID && workUnitModels.ContainsID(workUnits.CurrentID))
            {
                slotModel.WorkUnitModel = workUnitModels[workUnits.CurrentID];
            }
        }

        private void UpdateWorkUnitBenchmarkAndRepository(IEnumerable<WorkUnitModel> workUnitModels)
        {
            foreach (var m in workUnitModels)
            {
                UpdateWorkUnitRepository(m);
            }
        }

        private void UpdateWorkUnitRepository(WorkUnitModel workUnitModel)
        {
            if (WorkUnitRepository is not null)
            {
                try
                {
                    if (WorkUnitRepository.Update(workUnitModel) > 0)
                    {
                        if (Logger.IsDebugEnabled)
                        {
                            string message = $"Updated {workUnitModel.WorkUnit.ToProjectString()} in database.";
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
            if (!Connected)
            {
                return;
            }
            string command = slotId.HasValue ? "unpause " + slotId.Value : "unpause";
            Connection.CreateCommand(command).Execute();
        }

        public void Pause(int? slotId)
        {
            if (!Connected)
            {
                return;
            }
            string command = slotId.HasValue ? "pause " + slotId.Value : "pause";
            Connection.CreateCommand(command).Execute();
        }

        public void Finish(int? slotId)
        {
            if (!Connected)
            {
                return;
            }
            string command = slotId.HasValue ? "finish " + slotId.Value : "finish";
            Connection.CreateCommand(command).Execute();
        }
    }
}
