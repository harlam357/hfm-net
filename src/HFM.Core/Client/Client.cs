using System.Diagnostics;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Client
{
    public interface IClient
    {
        /// <summary>
        /// Fired when the client slot collection has changed.
        /// </summary>
        event EventHandler SlotsChanged;

        /// <summary>
        /// Fired when the client data retrieval process is finished.
        /// </summary>
        event EventHandler RetrieveFinished;

        ILogger Logger { get; }

        IPreferences Preferences { get; }

        IProteinBenchmarkRepository Benchmarks { get; }

        /// <summary>
        /// Settings that define this client's behavior.
        /// </summary>
        ClientSettings Settings { get; set; }

        /// <summary>
        /// Gets the client version.
        /// </summary>
        string ClientVersion { get; }

        /// <summary>
        /// Gets the collection of client slots.
        /// </summary>
        IReadOnlyCollection<SlotModel> Slots { get; }

        void Close();

        /// <summary>
        /// Gets a value indicating whether the client is connected.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Asynchronously connects the client to the resources defined by <see cref="Settings"/>.
        /// </summary>
        Task Connect();

        /// <summary>
        /// Starts the client data retrieval process.
        /// </summary>
        Task Retrieve();
    }

    public abstract class Client : IClient, IDisposable
    {
        public event EventHandler SlotsChanged;

        protected virtual void OnSlotsChanged() => SlotsChanged?.Invoke(this, EventArgs.Empty);

        public event EventHandler RetrieveFinished;

        protected virtual void OnRetrieveFinished() => RetrieveFinished?.Invoke(this, EventArgs.Empty);

        public ILogger Logger { get; }
        public IPreferences Preferences { get; }
        public IProteinBenchmarkRepository Benchmarks { get; }

        protected Client(ILogger logger, IPreferences preferences, IProteinBenchmarkRepository benchmarks)
        {
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            Benchmarks = benchmarks ?? NullProteinBenchmarkRepository.Instance;

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

        public string ClientVersion { get; set; }

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

        // Dispose
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
