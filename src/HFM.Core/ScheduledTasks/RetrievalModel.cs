
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Castle.Core.Logging;

using HFM.Core.Client;
using HFM.Preferences;

namespace HFM.Core.ScheduledTasks
{
   public class RetrievalModel
   {
      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      private const string ClientTaskKey = "Client Retrieval";
      private const string WebTaskKey = "Web Generation";

      private readonly IPreferenceSet _prefs;
      private readonly IClientConfiguration _clientConfiguration;
      private readonly Lazy<IMarkupGenerator> _markupGenerator;
      private readonly Lazy<IWebsiteDeployer> _websiteDeployer;
      private readonly TaskManager _taskManager;

      public RetrievalModel(IPreferenceSet prefs, IClientConfiguration clientConfiguration, 
                            Lazy<IMarkupGenerator> markupGenerator, Lazy<IWebsiteDeployer> websiteDeployer)
      {
         _prefs = prefs;
         _clientConfiguration = clientConfiguration;
         _markupGenerator = markupGenerator;
         _websiteDeployer = websiteDeployer;
         _taskManager = new TaskManager();
         _taskManager.Changed += (s, e) => ReportAction(e);

         _prefs.PreferenceChanged += (s, e) =>
         {
            switch (e.Preference)
            {
               case Preference.ClientRetrievalTask:
                  if (_prefs.Get<bool>(Preference.ClientRetrievalTaskEnabled))
                  {
                     if (_clientConfiguration.Count != 0)
                     {
                        _taskManager.Restart(ClientTaskKey, ClientInterval);
                     }
                  }
                  else
                  {
                     _taskManager.Stop(ClientTaskKey);
                  }
                  break;
               case Preference.WebGenerationTask:
                  if (_prefs.Get<bool>(Preference.WebGenerationTaskEnabled) &&
                      _prefs.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval) == false)
                  {
                     if (_clientConfiguration.Count != 0)
                     {
                        _taskManager.Restart(WebTaskKey, WebInterval);
                     }
                  }
                  else
                  {
                     _taskManager.Stop(WebTaskKey);
                  }
                  break;
            }
         };

         _clientConfiguration.ConfigurationChanged += (s, e) =>
         {
            if (e.ChangedType == ConfigurationChangedType.Remove ||
                e.ChangedType == ConfigurationChangedType.Clear)
            {
               // Disable timers if no hosts
               if (_taskManager.Enabled && _clientConfiguration.Count == 0)
               {
                  Logger.Info("No clients... stopping all scheduled tasks");
                  //_taskManager.Stop();
                  _taskManager.Cancel();
               }
            }
            else if (e.ChangedType == ConfigurationChangedType.Add)
            {
               var clientTaskEnabled = _prefs.Get<bool>(Preference.ClientRetrievalTaskEnabled);
               if (e.Client == null)
               {
                  // no client specified - retrieve all
                  _taskManager.Run(ClientTaskKey, ClientInterval, clientTaskEnabled);
               }
               else if (clientTaskEnabled)
               {
                  _taskManager.Start(ClientTaskKey, ClientInterval);
               }

               if (_prefs.Get<bool>(Preference.WebGenerationTaskEnabled) &&
                   _prefs.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval) == false)
               {
                  _taskManager.Start(WebTaskKey, WebInterval);
               }
            }
         };

         _taskManager.Add(ClientTaskKey, ClientRetrievalAction, ClientInterval);
         _taskManager.Add(WebTaskKey, WebGenerationAction, WebInterval);
      }

      private void ReportAction(ScheduledTaskChangedEventArgs e)
      {
         switch (e.Action)
         {
            case ScheduledTaskChangedAction.Started:
               Logger.InfoFormat("{0} task scheduled: {1} minutes", e.Key, (int)(e.Interval / Constants.MinToMillisec));
               break;
            case ScheduledTaskChangedAction.Stopped:
               Logger.InfoFormat("{0} task stopped", e.Key);
               break;
            case ScheduledTaskChangedAction.Running:
               Logger.InfoFormat("{0} task running", e.Key);
               break;
            case ScheduledTaskChangedAction.Finished:
               Logger.InfoFormat("{0} task finished: {1:#,##0} ms", e.Key, e.Interval);
               break;
            case ScheduledTaskChangedAction.AlreadyInProgress:
               Logger.WarnFormat("{0} task already in progress", e.Key);
               break;
         }
      }

      private double ClientInterval
      {
         get { return _prefs.Get<int>(Preference.ClientRetrievalTaskInterval) * Constants.MinToMillisec; }
      }

      private double WebInterval
      {
         get { return _prefs.Get<int>(Preference.WebGenerationTaskInterval) * Constants.MinToMillisec; }
      }

      public void RunClientRetrieval()
      {
         _taskManager.Run(ClientTaskKey, false);
      }

      //public void RunWebGeneration()
      //{
      //   _taskManager.Run(WebTaskKey, false);
      //}

      private void ClientRetrievalAction(CancellationToken ct)
      {
         // get flag synchronous or asynchronous - we don't want this flag to change on us
         // in the middle of a retrieve, so grab it now and use the local copy
         var mode = _prefs.Get<ProcessingMode>(Preference.ClientRetrievalTaskType);

         ct.ThrowIfCancellationRequested();

         var clientsEnumerable = _clientConfiguration.GetClients();
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

         if (_prefs.Get<bool>(Preference.WebGenerationTaskEnabled) &&
             _prefs.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval))
         {
            ct.ThrowIfCancellationRequested();
            _taskManager.Run(WebTaskKey, false);
         }
      }

      private void WebGenerationAction(CancellationToken ct)
      {
         ct.ThrowIfCancellationRequested();
         var slots = _clientConfiguration.Slots as IList<SlotModel> ?? _clientConfiguration.Slots.ToList();
         _markupGenerator.Value.Generate(slots);

         ct.ThrowIfCancellationRequested();
         _websiteDeployer.Value.DeployWebsite(_markupGenerator.Value.HtmlFilePaths, _markupGenerator.Value.XmlFilePaths, slots);
      }
   }
}
