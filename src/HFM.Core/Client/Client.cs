using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

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
        /// Enumeration of client slots.
        /// </summary>
        IEnumerable<SlotModel> Slots { get; }

        /// <summary>
        /// Cancels the retrieval process.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Starts the retrieval process.
        /// </summary>
        void Retrieve();
    }

    public abstract class Client : IClient
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

        // Slots
        private readonly List<SlotModel> _slots = new List<SlotModel>();
        private readonly ReaderWriterLockSlim _slotsLock = new ReaderWriterLockSlim();

        public IEnumerable<SlotModel> Slots
        {
            get
            {
                _slotsLock.EnterReadLock();
                try
                {
                    return _slots.ToArray();
                }
                finally
                {
                    _slotsLock.ExitReadLock();
                }
            }
        }

        public void RefreshSlots()
        {
            _slotsLock.EnterWriteLock();
            try
            {
                _slots.Clear();
                OnRefreshSlots(_slots);
            }
            finally
            {
                _slotsLock.ExitWriteLock();
            }

            OnSlotsChanged();
        }

        protected virtual void OnRefreshSlots(ICollection<SlotModel> slots) =>
            slots.Add(SlotModel.CreateOfflineSlotModel(this));

        // Retrieve
        public DateTime LastRetrieveTime { get; protected set; } = DateTime.MinValue;

        public bool IsCancellationRequested { get; private set; }

        public void Cancel() => OnCancel();

        protected virtual void OnCancel() => IsCancellationRequested = true;

        private readonly object _retrieveLock = new object();

        public void Retrieve()
        {
            if (!Monitor.TryEnter(_retrieveLock))
            {
                OnRetrieveInProgress();
                return;
            }
            try
            {
                IsCancellationRequested = false;

                LastRetrieveTime = DateTime.Now;
                OnRetrieve();
            }
            finally
            {
                OnRetrieveFinished();

                Monitor.Exit(_retrieveLock);
            }
        }

        protected virtual void OnRetrieveInProgress() =>
            Debug.WriteLine(Logging.Logger.NameFormat, Settings.Name, "Retrieval already in progress...");

        protected abstract void OnRetrieve();
    }
}
