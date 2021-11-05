using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Client
{
    public interface IClient
    {
        /// <summary>
        /// Fired when the client slot layout has changed.
        /// </summary>
        event EventHandler SlotsChanged;

        /// <summary>
        /// Fired when the Retrieve method finishes.
        /// </summary>
        event EventHandler RetrieveFinished;

        ILogger Logger { get; }

        IPreferences Preferences { get; }

        IProteinBenchmarkService BenchmarkService { get; }

        /// <summary>
        /// Settings that define this client's behavior.
        /// </summary>
        ClientSettings Settings { get; set; }

        /// <summary>
        /// Gets the client version.
        /// </summary>
        string ClientVersion { get; }

        /// <summary>
        /// Enumeration of client slots.
        /// </summary>
        IEnumerable<SlotModel> Slots { get; }

        /// <summary>
        /// Cancels the retrieval process.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Gets a value indicating whether the client is connected.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Asynchronously connects the client to the resources defined by the <see cref="ClientSettings"/>.
        /// </summary>
        Task Connect();

        /// <summary>
        /// Starts the retrieval process.
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
        public IProteinBenchmarkService BenchmarkService { get; }

        protected Client(ILogger logger, IPreferences preferences, IProteinBenchmarkService benchmarkService)
        {
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            BenchmarkService = benchmarkService ?? NullProteinBenchmarkService.Instance;

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
        private List<SlotModel> _slots = new List<SlotModel>();

        public IEnumerable<SlotModel> Slots => _slots.ToArray();

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

        public void Cancel() => OnCancel();

        protected virtual void OnCancel() => IsCancellationRequested = true;

        public virtual bool Connected => false;

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

        private bool ClientCannotConnectOrRetrieve() => Settings != null && Settings.Disabled;

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
