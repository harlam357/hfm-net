using System.Diagnostics;

using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Client;

public record ClientPlatform(string ClientVersion, string OperatingSystem);

public interface IClient
{
    /// <summary>
    /// Raised when the client slot collection has changed.
    /// </summary>
    event EventHandler SlotsChanged;

    /// <summary>
    /// Raised when the client data retrieval process is finished.
    /// </summary>
    event EventHandler RetrieveFinished;

    /// <summary>
    /// Gets or sets the settings that define this client's behavior.
    /// </summary>
    ClientSettings Settings { get; set; }

    /// <summary>
    /// Gets the values that define this client's platform (version, operating system, etc).
    /// </summary>
    ClientPlatform Platform { get; }

    /// <summary>
    /// Gets the collection of client slots.
    /// </summary>
    IReadOnlyCollection<SlotModel> Slots { get; }

    /// <summary>
    /// Closes the client connection.
    /// </summary>
    void Close();

    /// <summary>
    /// Gets the value indicating if the client is connected.
    /// </summary>
    bool Connected { get; }

    /// <summary>
    /// Asynchronously connects the client using the defined <see cref="Settings"/>.
    /// </summary>
    Task Connect();

    /// <summary>
    /// Asynchronously retrieves data from the client.
    /// </summary>
    Task Retrieve();
}

public abstract class Client : IClient
{
    public event EventHandler SlotsChanged;

    protected virtual void OnSlotsChanged() => SlotsChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler RetrieveFinished;

    protected virtual void OnRetrieveFinished() => RetrieveFinished?.Invoke(this, EventArgs.Empty);

    public ILogger Logger { get; }

    public IPreferences Preferences { get; }

    protected Client(ILogger logger, IPreferences preferences)
    {
        Logger = logger ?? NullLogger.Instance;
        Preferences = preferences ?? new InMemoryPreferencesProvider();

        RefreshSlots();
    }

    // ClientSettings
    private ClientSettings _settings;

    public ClientSettings Settings
    {
        get => _settings;
        set
        {
            if (_settings != value)
            {
                var oldSettings = _settings;
                _settings = value;
                OnSettingsChanged(oldSettings, value);
            }
        }
    }

    protected virtual void OnSettingsChanged(ClientSettings oldSettings, ClientSettings newSettings)
    {
    }

    public ClientPlatform Platform { get; set; }

    // Slots
    private List<SlotModel> _slots = new();

    public IReadOnlyCollection<SlotModel> Slots => _slots.ToArray();

    public void RefreshSlots()
    {
        var slots = new List<SlotModel>();
        OnRefreshSlots(slots);
        Interlocked.Exchange(ref _slots, slots);

        OnSlotsChanged();
    }

    protected virtual void OnRefreshSlots(ICollection<SlotModel> slots) =>
        slots.Add(SlotModel.CreateOfflineSlotModel(this));

    // Retrieve
    public DateTime LastRetrieveTime { get; protected set; } = DateTime.MinValue;

    public bool IsCancellationRequested { get; private set; }

    public void Close() => OnClose();

    protected virtual void OnClose() => IsCancellationRequested = true;

    public virtual bool Connected { get; protected set; }

    public async Task Connect()
    {
        if (ClientCannotConnectOrRetrieve())
        {
            return;
        }

        await OnConnect().ConfigureAwait(false);
    }

    protected virtual Task OnConnect() => Task.CompletedTask;

    private int _retrieveLock;

    public async Task Retrieve()
    {
        if (ClientCannotConnectOrRetrieve())
        {
            return;
        }

        if (Interlocked.CompareExchange(ref _retrieveLock, 1, 0) != 0)
        {
            OnRetrieveInProgress();
            return;
        }

        try
        {
            IsCancellationRequested = false;
            LastRetrieveTime = DateTime.Now;

            if (!Connected)
            {
                await Connect().ConfigureAwait(false);
                return;
            }

            await OnRetrieve().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger.Error(String.Format(Logging.Logger.NameFormat, Settings?.Name, ex.Message), ex);
        }
        finally
        {
            OnRetrieveFinished();

            Interlocked.Exchange(ref _retrieveLock, 0);
        }
    }

    private bool ClientCannotConnectOrRetrieve() => Settings is { Disabled: true };

    protected virtual void OnRetrieveInProgress() =>
        Debug.WriteLine(Logging.Logger.NameFormat, Settings?.Name, "Retrieval already in progress...");

    protected abstract Task OnRetrieve();
}
