
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using HFM.Core.Data;
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
        event EventHandler RetrievalFinished;

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
        /// Last successful retrieval time.
        /// </summary>
        DateTime LastRetrievalTime { get; }

        /// <summary>
        /// Abort retrieval processes.
        /// </summary>
        void Abort();

        /// <summary>
        /// Start retrieval processes.
        /// </summary>
        void Retrieve();
    }

    public abstract class Client : IClient
    {
        public event EventHandler SlotsChanged;

        protected virtual void OnSlotsChanged(EventArgs e)
        {
            SlotsChanged?.Invoke(this, e);
        }

        public event EventHandler RetrievalFinished;

        protected virtual void OnRetrievalFinished(EventArgs e)
        {
            RetrievalFinished?.Invoke(this, e);
        }

        public ILogger Logger { get; }
        public IPreferences Preferences { get; }
        public IProteinBenchmarkService BenchmarkService { get; }
        
        protected Client(ILogger logger, IPreferences preferences, IProteinBenchmarkService benchmarkService)
        {
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            BenchmarkService = benchmarkService ?? new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
        }

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

        public IEnumerable<SlotModel> Slots => OnEnumerateSlots();

        protected virtual IEnumerable<SlotModel> OnEnumerateSlots()
        {
            return Array.Empty<SlotModel>();
        }

        public DateTime LastRetrievalTime { get; protected set; } = DateTime.MinValue;

        protected bool AbortFlag { get; private set; }

        public virtual void Abort()
        {
            AbortFlag = true;
        }

        private readonly object _retrieveLock = new object();

        public void Retrieve()
        {
            if (!Monitor.TryEnter(_retrieveLock))
            {
                Debug.WriteLine(Logging.Logger.NameFormat, Settings.Name, "Retrieval already in progress...");
                return;
            }
            try
            {
                AbortFlag = false;

                // perform the client specific retrieval
                OnRetrieve();
            }
            finally
            {
                AbortFlag = false;
                Monitor.Exit(_retrieveLock);
            }
        }

        protected abstract void OnRetrieve();
    }
}
