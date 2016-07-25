/*
 * HFM.NET - Retrieval Logic Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Castle.Core.Logging;

using HFM.Core;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public sealed class RetrievalLogic
   {
      #region Fields

      /// <summary>
      /// Local time that denotes when a full retrieve started (only accessed by the RetrieveInProgress property)
      /// </summary>
      private DateTime _retrieveExecStart;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress (only accessed by the RetrieveInProgress property)
      /// </summary>
      private volatile bool _retrievalInProgress;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress
      /// </summary>
      public bool RetrievalInProgress
      {
         get { return _retrievalInProgress; }
         private set
         {
            if (value)
            {
               _retrieveExecStart = Instrumentation.ExecStart;
               _retrievalInProgress = true;
            }
            else
            {
               Logger.Info(String.Format("Total Retrieval Execution Time: {0}", Instrumentation.GetExecTime(_retrieveExecStart)));
               _retrievalInProgress = false;
            }
         }
      }

      private volatile bool _generationInProgress;

      public bool GenerationInProgress
      {
         get { return _generationInProgress; }
         private set { _generationInProgress = value; }
      }

      /// <summary>
      /// Retrieval Timer Object (init 10 minutes)
      /// </summary>
      private readonly System.Timers.Timer _workTimer = new System.Timers.Timer(600000);

      /// <summary>
      /// Web Generation Timer Object (init 15 minutes)
      /// </summary>
      private readonly System.Timers.Timer _webTimer = new System.Timers.Timer(900000);

      #region Service Interfaces

      private ILogger _logger;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger ?? (_logger = NullLogger.Instance); }
         [CoverageExclude]
         set { _logger = value; }
      }

      private IMarkupGenerator _markupGenerator;
      private IWebsiteDeployer _websiteDeployer;

      private readonly IPreferenceSet _prefs;
      private readonly IClientConfiguration _clientConfiguration;
      private readonly MainGridModel _mainGridModel;

      private Thread _doRetrievalThread;

      #endregion

      #endregion

      public RetrievalLogic(IPreferenceSet prefs, IClientConfiguration clientConfiguration, MainGridModel mainGridModel)
      {
         _prefs = prefs;
         _clientConfiguration = clientConfiguration;
         _clientConfiguration.DictionaryChanged += (s, e) =>
                                                   {
                                                      if (e.ChangedType == ConfigurationChangedType.Remove ||
                                                          e.ChangedType == ConfigurationChangedType.Clear)
                                                      {
                                                         // Disable timers if no hosts
                                                         if (_clientConfiguration.Count == 0)
                                                         {
                                                            if (_workTimer.Enabled || _webTimer.Enabled)
                                                            {
                                                               Logger.Info("No Hosts - Stopping All Scheduled Tasks");
                                                               _workTimer.Stop();
                                                               _webTimer.Stop();
                                                            }
                                                         }
                                                      }
                                                      else if (e.ChangedType == ConfigurationChangedType.Add)
                                                      {
                                                         if (e.Client == null)
                                                         {
                                                            // no client specified - retrieve all
                                                            // this method will start the client retrieval timer when finished
                                                            QueueNewRetrieval();
                                                         }
                                                         else
                                                         {
                                                            // retrieve specified client
                                                            RetrieveSingleClient(e.Client);
                                                            // starts the client retrieval timer if necessary
                                                            SetClientRetrievalTimerState();
                                                         }
                                                         // starts the web generation timer if necessary
                                                         SetWebGenerationTimerState();
                                                      }
                                                      else if (e.ChangedType == ConfigurationChangedType.Edit)
                                                      {
                                                         Debug.Assert(e.Client != null);
                                                         RetrieveSingleClient(e.Client);
                                                      }
                                                   };
         _mainGridModel = mainGridModel;
      }

      public void Initialize()
      {
         // Hook up Retrieval Timer Event Handler
         _workTimer.Elapsed += WorkTimerTick;
         // Hook up Web Generation Timer Event Handler
         _webTimer.Elapsed += WebGenTimerTick;

         _prefs.PreferenceChanged += (s, e) =>
         {
            switch (e.Preference)
            {
               case Preference.ClientRetrievalTask:
                  SetClientRetrievalTimerState(true);
                  break;
               case Preference.WebGenerationTask:
                  SetWebGenerationTimerState(true);
                  break;
            }
         };
      }

      #region Retrieval Logic

      /// <summary>
      /// When the host refresh timer expires, refresh all the hosts
      /// </summary>
      private void WorkTimerTick(object sender, EventArgs e)
      {
         Logger.Info("Running Client Retrieval Task...");
         QueueNewRetrieval();
      }

      /// <summary>
      /// When the web gen timer expires, refresh the website files
      /// </summary>
      private void WebGenTimerTick(object sender, EventArgs e)
      {
         Debug.Assert(WebGenerationTaskEnabled);
         //Logger.Info("Stopping web generation timer loop.");
         _webTimer.Stop();

         DoWebGeneration();
         if (WebGenerationTaskEnabled)
         {
            StartWebGenTimer();
         }
      }

      private void DoWebGeneration()
      {
         Debug.Assert(_prefs.Get<bool>(Preference.WebGenerationTaskEnabled));

         if (GenerationInProgress)
         {
            Logger.Warn("Web Generation already in progress...");
            return;
         }

         GenerationInProgress = true;
         try
         {
            DateTime start = Instrumentation.ExecStart;

            // lazy initialize
            if (_markupGenerator == null) _markupGenerator = ServiceLocator.Resolve<IMarkupGenerator>();
            if (_websiteDeployer == null) _websiteDeployer = ServiceLocator.Resolve<IWebsiteDeployer>();

            Logger.Info("Running Web Generation Task...");

            var slots = _mainGridModel.SlotCollection;
            _markupGenerator.Generate(slots);
            _websiteDeployer.DeployWebsite(_markupGenerator.HtmlFilePaths, _markupGenerator.XmlFilePaths, slots);

            Logger.Info("Total Web Generation Execution Time: {0}", Instrumentation.GetExecTime(start));
         }
         catch (Exception ex)
         {
            Logger.ErrorFormat(ex, "{0}", ex.Message);
         }
         finally
         {
            GenerationInProgress = false;
         }
      }

      /// <summary>
      /// Stick each Instance in the background thread queue to retrieve the info for a given Instance
      /// </summary>
      public void QueueNewRetrieval()
      {
         // don't fire this process twice
         if (RetrievalInProgress)
         {
            Logger.Warn("Client Retrieval already in progress...");
            return;
         }

         // only fire if there are Hosts
         if (_clientConfiguration.Count == 0)
         {
            return;
         }

         //Logger.Info("Stopping retrieval timer loop.");
         _workTimer.Stop();

         // fire the retrieval wrapper thread (basically a wait thread off the UI thread)
         _doRetrievalThread = new Thread(DoRetrievalWrapper) { IsBackground = true, Name = "DoRetrievalWrapper" };
         _doRetrievalThread.Start();
      }

      public void Abort()
      {
         if (RetrievalInProgress)
         {
            _doRetrievalThread.Abort();
         }
      }

      /// <summary>
      /// Wraps the DoRetrieval function on a seperate thread and fires post retrieval processes
      /// </summary>
      private void DoRetrievalWrapper()
      {
         try
         {
            // set full retrieval flag
            RetrievalInProgress = true;

            // fire the actual retrieval thread
            var doRetrieval = new Thread(DoRetrieval) { IsBackground = true, Name = "DoRetrieval" };
            doRetrieval.Start();
            // wait for completion
            doRetrieval.Join();

            // run post retrieval processes
            if (_prefs.Get<bool>(Preference.WebGenerationTaskEnabled) &&
                _prefs.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval))
            {
               // do a web gen (on another thread)
               var webGen = new Thread(DoWebGeneration) { IsBackground = true, Name = "DoWebGeneration" };
               webGen.Start();
            }

            // Enable the data retrieval timer
            if (_prefs.Get<bool>(Preference.ClientRetrievalTaskEnabled))
            {
               StartBackgroundTimer();
            }
         }
         catch (ThreadAbortException)
         {
            // do nothing, just let this thread die
         }
         finally
         {
            // clear full retrieval flag
            RetrievalInProgress = false;
         }
      }

      /// <summary>
      /// Do a full retrieval operation
      /// </summary>
      private void DoRetrieval()
      {
         // get flag synchronous or asynchronous - we don't want this flag to change on us
         // in the middle of a retrieve, so grab it now and use the local copy
         var type = _prefs.Get<ProcessingMode>(Preference.ClientRetrievalTaskType);

         var clients = _clientConfiguration.GetClients().ToList();
         if (type == ProcessingMode.Serial)
         {
            // do the individual retrieves on a single thread
            clients.ForEach(RetrieveInstance);
         }
         else
         {
            // fire individual threads to do the their own retrieve simultaneously
            Parallel.ForEach(clients, RetrieveInstance);
         }
      }

      /// <summary>
      /// Stub to execute retrieve and refresh display
      /// </summary>
      private static void RetrieveInstance(IClient client)
      {
         if (client.RetrievalInProgress == false)
         {
            client.Retrieve();
         }
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      public void RetrieveSingleClient(string name)
      {
         RetrieveSingleClient(_clientConfiguration.Get(name));
      }

      private static void RetrieveSingleClient(IClient client)
      {
         Task.Factory.StartNew(() => RetrieveInstance(client));
      }

      private void SetClientRetrievalTimerState(bool reset = false)
      {
         // Enable the data retrieval timer
         if (_prefs.Get<bool>(Preference.ClientRetrievalTaskEnabled))
         {
            if (!RetrievalInProgress)
            {
               if (reset)
               {
                  _workTimer.Stop();
               }
               if (!_workTimer.Enabled)
               {
                  StartBackgroundTimer();
               }
            }
         }
         else if (_workTimer.Enabled)
         {
            Logger.Info("Stopping Client Retrieval Scheduled Task");
            _workTimer.Stop();
         }
      }

      private void StartBackgroundTimer()
      {
         Debug.Assert(!_workTimer.Enabled);

         if (_clientConfiguration.Count == 0)
         {
            return;
         }

         var syncTimeMinutes = _prefs.Get<int>(Preference.ClientRetrievalTaskInterval);

         _workTimer.Interval = syncTimeMinutes * Constants.MinToMillisec;
         Logger.Info("Scheduling Client Retrieval Task: {0} Minutes", syncTimeMinutes);
         _workTimer.Start();
      }

      private bool WebGenerationTaskEnabled
      {
         get
         {
            return _prefs.Get<bool>(Preference.WebGenerationTaskEnabled) &&
                   _prefs.Get<bool>(Preference.WebGenerationTaskAfterClientRetrieval) == false;
         }
      }

      private void SetWebGenerationTimerState(bool reset = false)
      {
         // Enable the web generation timer
         if (WebGenerationTaskEnabled)
         {
            if (reset)
            {
               _webTimer.Stop();
            }
            if (!_webTimer.Enabled)
            {
               StartWebGenTimer();
            }
         }
         else if (_webTimer.Enabled)
         {
            Logger.Info("Stopping Web Generation Scheduled Task");
            _webTimer.Stop();
         }
      }

      private void StartWebGenTimer()
      {
         Debug.Assert(!_webTimer.Enabled);
         Debug.Assert(WebGenerationTaskEnabled);

         if (_clientConfiguration.Count == 0)
         {
            return;
         }

         var generateInterval = _prefs.Get<int>(Preference.WebGenerationTaskInterval);

         _webTimer.Interval = generateInterval * Constants.MinToMillisec;
         Logger.Info("Scheduling Web Generation Task: {0} Minutes", generateInterval);
         _webTimer.Start();
      }

      #endregion
   }
}
