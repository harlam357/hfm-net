
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HFM.Core.Logging;
using HFM.Core.ScheduledTasks;
using HFM.Core.SlotXml;
using HFM.Preferences;

namespace HFM.Core.Client
{
    public enum ProcessingMode
    {
        Parallel,
        Serial
    }

    public delegate ClientScheduledTasks ClientScheduledTasksFactory(ILogger logger, IPreferenceSet preferences, ClientConfiguration clientConfiguration);

    public class ClientScheduledTasks
    {
        private const string ClientTaskKey = "Client Retrieval";
        private const string WebTaskKey = "Web Generation";

        public ILogger Logger { get; }
        public IPreferenceSet Preferences { get; }
        public ClientConfiguration ClientConfiguration { get; }

        private readonly DelegateScheduledTask _clientRetrievalTask;
        private readonly DelegateScheduledTask _webGenerationTask;

        public static ClientScheduledTasksFactory Factory => (l, p, c) => new ClientScheduledTasks(l, p, c);

        public ClientScheduledTasks(ILogger logger, IPreferenceSet preferences, ClientConfiguration clientConfiguration)
        {
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences;
            ClientConfiguration = clientConfiguration;

            Preferences.PreferenceChanged += OnPreferenceChanged;
            ClientConfiguration.ClientConfigurationChanged += OnClientConfigurationChanged;
            
            _clientRetrievalTask = new DelegateScheduledTask(ClientTaskKey, ClientRetrievalAction, ClientInterval);
            _clientRetrievalTask.Changed += TaskChanged;
            _webGenerationTask = new DelegateScheduledTask(WebTaskKey, WebGenerationAction, WebInterval);
            _webGenerationTask.Changed += TaskChanged;
        }

        protected virtual void OnPreferenceChanged(object sender, PreferenceChangedEventArgs e)
        {
            switch (e.Preference)
            {
                case Preference.ClientRetrievalTask:
                    if (Preferences.Get<bool>(Preference.ClientRetrievalTaskEnabled))
                    {
                        if (ClientConfiguration.Count != 0)
                        {
                            _clientRetrievalTask.Interval = ClientInterval;
                            _clientRetrievalTask.Restart();
                        }
                    }
                    else
                    {
                        _clientRetrievalTask.Stop();
                    }
                    break;
                case Preference.WebGenerationTask:
                    if (Preferences.Get<bool>(Preference.WebGenerationTaskEnabled) &&
                        Preferences.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval) == false)
                    {
                        if (ClientConfiguration.Count != 0)
                        {
                            _webGenerationTask.Interval = WebInterval;
                            _webGenerationTask.Restart();
                        }
                    }
                    else
                    {
                        _webGenerationTask.Stop();
                    }
                    break;
            }
        }

        protected virtual void OnClientConfigurationChanged(object sender, ClientConfigurationChangedEventArgs e)
        {
            if (e.Action == ClientConfigurationChangedAction.Remove ||
                e.Action == ClientConfigurationChangedAction.Clear)
            {
                // Disable timers if no hosts
                if ((_clientRetrievalTask.Enabled || _webGenerationTask.Enabled) && ClientConfiguration.Count == 0)
                {
                    Logger.Info("No clients... stopping all scheduled tasks");
                    _clientRetrievalTask.Cancel();
                    _webGenerationTask.Cancel();
                }
            }
            else if (e.Action == ClientConfigurationChangedAction.Add)
            {
                var clientTaskEnabled = Preferences.Get<bool>(Preference.ClientRetrievalTaskEnabled);
                if (e.Client == null)
                {
                    // no client specified - retrieve all
                    _clientRetrievalTask.Interval = ClientInterval;
                    _clientRetrievalTask.Run(clientTaskEnabled);
                }
                else
                {
                    Task.Run(() => e.Client.Retrieve());
                    if (clientTaskEnabled)
                    {
                        _clientRetrievalTask.Interval = ClientInterval;
                        _clientRetrievalTask.Start();
                    }
                }

                if (Preferences.Get<bool>(Preference.WebGenerationTaskEnabled) &&
                    Preferences.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval) == false)
                {
                    _webGenerationTask.Interval = WebInterval;
                    _webGenerationTask.Start();
                }
            }
            else if (e.Action == ClientConfigurationChangedAction.Edit)
            {
                Task.Run(() => e.Client.Retrieve());
            }
        }

        private void TaskChanged(object sender, ScheduledTaskChangedEventArgs e)
        {
            switch (e.Action)
            {
                case ScheduledTaskChangedAction.Started:
                    Logger.Info(e.ToString(i => $"{(int)TimeSpan.FromMilliseconds(i.GetValueOrDefault()).TotalMinutes} minutes"));
                    break;
                case ScheduledTaskChangedAction.Faulted:
                    Logger.Error(e.ToString());
                    break;
                case ScheduledTaskChangedAction.AlreadyInProgress:
                    Logger.Warn(e.ToString());
                    break;
                default:
                    Logger.Info(e.ToString());
                    break;
            }
        }

        private double ClientInterval => TimeSpan.FromMinutes(Preferences.Get<int>(Preference.ClientRetrievalTaskInterval)).TotalMilliseconds;

        private double WebInterval => TimeSpan.FromMinutes(Preferences.Get<int>(Preference.WebGenerationTaskInterval)).TotalMilliseconds;

        public void RetrieveAll()
        {
            _clientRetrievalTask.Run(false);
        }

        //public void RunWebGeneration()
        //{
        //   _webGenerationTask.Run(false);
        //}

        private void ClientRetrievalAction(CancellationToken ct)
        {
            // get flag synchronous or asynchronous - we don't want this flag to change on us
            // in the middle of a retrieve, so grab it now and use the local copy
            var mode = Preferences.Get<ProcessingMode>(Preference.ClientRetrievalTaskType);

            ct.ThrowIfCancellationRequested();

            var clientsEnumerable = ClientConfiguration.GetClients();
            var clients = clientsEnumerable as IList<IClient> ?? clientsEnumerable.ToList();
            if (mode == ProcessingMode.Serial)
            {
                // do the individual retrieves on a single thread
                foreach (var client in clients)
                {
                    ct.ThrowIfCancellationRequested();
                    client.Retrieve();
                }
            }
            else
            {
                // fire individual threads to do the their own retrieve simultaneously
                Parallel.ForEach(clients, x =>
                {
                    ct.ThrowIfCancellationRequested();
                    x.Retrieve();
                });
            }

            if (Preferences.Get<bool>(Preference.WebGenerationTaskEnabled) &&
                Preferences.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval))
            {
                ct.ThrowIfCancellationRequested();
                _webGenerationTask.Run(false);
            }
        }

        private void WebGenerationAction(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            string artifactPath = null;
            try
            {
                var slots = ClientConfiguration.Slots as IList<SlotModel> ?? ClientConfiguration.Slots.ToList();
                var artifactBuilder = new WebArtifactBuilder(Logger, Preferences);
                artifactPath = artifactBuilder.Build(slots);

                ct.ThrowIfCancellationRequested();
                var deploymentType = Preferences.Get<WebDeploymentType>(Preference.WebDeploymentType);
                var deployment = WebArtifactDeployment.Create(deploymentType, Logger, Preferences);
                deployment.Deploy(artifactPath);
            }
            finally
            {
                TryDeleteDirectory(artifactPath);
            }
        }

        private static void TryDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        public static bool ValidateInterval(int interval)
        {
            return interval <= MaxInterval && interval >= MinInterval;
        }

        public const int MinInterval = 1;
        public const int MaxInterval = 180;
    }
}
