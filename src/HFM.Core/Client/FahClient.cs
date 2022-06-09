using System.Diagnostics;
using System.Globalization;

using HFM.Client;
using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Client;

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

    public IProteinBenchmarkRepository Benchmarks { get; }
    public IProteinService ProteinService { get; }
    public IWorkUnitRepository WorkUnitRepository { get; }
    public FahClientMessages Messages { get; protected set; }
    public FahClientConnection Connection { get; protected set; }

    public FahClient(ILogger logger,
                     IPreferences preferences,
                     IProteinBenchmarkRepository benchmarks,
                     IProteinService proteinService,
                     IWorkUnitRepository workUnitRepository)
        : base(logger, preferences)
    {
        Benchmarks = benchmarks;
        ProteinService = proteinService;
        WorkUnitRepository = workUnitRepository;
        Messages = new FahClientMessages(Logger, Preferences);
        _messageActions = new List<FahClientMessageAction>
        {
            new DelegateFahClientMessageAction(FahClientMessageType.SlotInfo, RefreshSlots),
            new DelegateFahClientMessageAction(FahClientMessageType.Info, RefreshClientPlatform),
            new ExecuteRetrieveMessageAction(Messages, async () => await Retrieve().ConfigureAwait(false))
        };

        RefreshSlots();
    }

    private readonly List<FahClientMessageAction> _messageActions;

    protected virtual async Task OnMessageRead(FahClientMessage message)
    {
        if (IsCancellationRequested) return;

        Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, $"{message.Identifier} - Length: {message.MessageText.Length}"));

        bool updated = await Messages.UpdateMessageAsync(message, this).ConfigureAwait(false);
        if (updated)
        {
            _messageActions.ForEach(x => x.Execute(message.Identifier.MessageType));
        }
    }

    private List<IClientData> _clientData = new();

    public void RefreshSlots()
    {
        var slots = new List<IClientData>();
        OnRefreshSlots(slots);
        Interlocked.Exchange(ref _clientData, slots);

        OnClientDataCollectionChanged();
    }

    protected virtual void OnRefreshSlots(ICollection<IClientData> collection)
    {
        var slotCollection = Messages?.SlotCollection;
        if (slotCollection is { Count: > 0 })
        {
            foreach (var slot in slotCollection)
            {
                var slotDescription = SlotDescription.Parse(slot.Description);
                var status = (SlotStatus)Enum.Parse(typeof(SlotStatus), slot.Status, true);
                var slotID = slot.ID.GetValueOrDefault();
                var clientData = new FahClientData(Preferences, this, status, slotID)
                {
                    Description = slotDescription
                };
                collection.Add(clientData);
            }
        }
    }

    protected override IReadOnlyCollection<IClientData> OnGetClientDataCollection() =>
        _clientData.Count > 0
            ? _clientData
            : base.OnGetClientDataCollection();

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

        foreach (var clientData in ClientDataCollection.Cast<FahClientData>())
        {
            var previousWorkUnitModel = clientData.WorkUnitModel;
            var workUnits = workUnitsBuilder.BuildForSlot(clientData.SlotID, clientData.Description, previousWorkUnitModel.WorkUnit);
            var workUnitModels = new WorkUnitModelCollection(workUnits.Select(x => BuildWorkUnitModel(clientData, x)));

            await PopulateClientData(clientData, workUnits, workUnitModels, workUnitQueueBuilder).ConfigureAwait(false);
            foreach (var m in workUnitModels)
            {
                await UpdateWorkUnitRepository(m).ConfigureAwait(false);
            }

            clientData.WorkUnitModel.ShowProductionTrace(Logger, clientData.Name, clientData.Status,
                Preferences.Get<PPDCalculation>(Preference.PPDCalculation),
                Preferences.Get<BonusCalculation>(Preference.BonusCalculation));

            string statusMessage = String.Format(CultureInfo.CurrentCulture, "Slot Status: {0}", clientData.Status);
            Logger.Info(String.Format(Logging.Logger.NameFormat, clientData.Name, statusMessage));
        }

        string message = String.Format(CultureInfo.CurrentCulture, "Retrieval finished: {0}", sw.GetExecTime());
        Logger.Info(String.Format(Logging.Logger.NameFormat, Settings.Name, message));
    }

    private IReadOnlyCollection<LogLine> EnumerateLogLines(int slotID, WorkUnitCollection workUnits)
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

    private WorkUnitModel BuildWorkUnitModel(IClientData clientData, WorkUnit workUnit)
    {
        Debug.Assert(clientData != null);
        Debug.Assert(workUnit != null);

        var protein = ProteinService?.GetOrRefresh(workUnit.ProjectID) ?? new Protein();
        return new WorkUnitModel(clientData, workUnit, Benchmarks)
        {
            CurrentProtein = protein
        };
    }

    private async Task PopulateClientData(FahClientData clientData,
                                          WorkUnitCollection workUnits,
                                          WorkUnitModelCollection workUnitModels,
                                          WorkUnitQueueItemCollectionBuilder workUnitQueueBuilder)
    {
        Debug.Assert(clientData != null);
        Debug.Assert(workUnits != null);
        Debug.Assert(workUnitModels != null);
        Debug.Assert(workUnitQueueBuilder != null);

        if (clientData.SlotType == SlotType.CPU)
        {
            clientData.Description.Processor = Messages.Info?.System?.CPU;
        }
        clientData.WorkUnitQueue = workUnitQueueBuilder.BuildForSlot(clientData.SlotID);
        clientData.CurrentLogLines = EnumerateLogLines(clientData.SlotID, workUnits);

        if (WorkUnitRepository is not null && Messages.ClientRun is not null)
        {
            var r = WorkUnitRepository;
            var slotIdentifier = clientData.SlotIdentifier;
            var clientStartTime = Messages.ClientRun.Data.StartTime;

            clientData.TotalRunCompletedUnits = (int)await r.CountCompletedAsync(slotIdentifier, clientStartTime).ConfigureAwait(false);
            clientData.TotalCompletedUnits = (int)await r.CountCompletedAsync(slotIdentifier, null).ConfigureAwait(false);
            clientData.TotalRunFailedUnits = (int)await r.CountFailedAsync(slotIdentifier, clientStartTime).ConfigureAwait(false);
            clientData.TotalFailedUnits = (int)await r.CountFailedAsync(slotIdentifier, null).ConfigureAwait(false);
        }

        // Update the WorkUnitModel if we have a current unit index
        if (workUnits.CurrentID != WorkUnitCollection.NoID && workUnitModels.ContainsID(workUnits.CurrentID))
        {
            clientData.WorkUnitModel = workUnitModels[workUnits.CurrentID];
        }
    }

    private async Task UpdateWorkUnitRepository(WorkUnitModel workUnitModel)
    {
        if (WorkUnitRepository is not null)
        {
            try
            {
                if (await WorkUnitRepository.UpdateAsync(workUnitModel).ConfigureAwait(false) > 0)
                {
                    if (Logger.IsDebugEnabled)
                    {
                        string message = $"Updated {workUnitModel.WorkUnit.ToProjectString()} in database.";
                        Logger.Debug(String.Format(Logging.Logger.NameFormat, workUnitModel.ClientData.SlotIdentifier.Name, message));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }
    }

    public void Fold(int? slotID)
    {
        if (!Connected)
        {
            return;
        }
        string command = slotID.HasValue ? "unpause " + slotID.Value : "unpause";
        Connection.CreateCommand(command).Execute();
    }

    public void Pause(int? slotID)
    {
        if (!Connected)
        {
            return;
        }
        string command = slotID.HasValue ? "pause " + slotID.Value : "pause";
        Connection.CreateCommand(command).Execute();
    }

    public void Finish(int? slotID)
    {
        if (!Connected)
        {
            return;
        }
        string command = slotID.HasValue ? "finish " + slotID.Value : "finish";
        Connection.CreateCommand(command).Execute();
    }
}
