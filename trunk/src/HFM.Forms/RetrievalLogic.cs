/*
 * HFM.NET - Retrieval Logic Class
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
               _logger.Info(String.Format("Total Retrieval Execution Time: {0}", Instrumentation.GetExecTime(_retrieveExecStart)));
               _retrievalInProgress = false;
            }
         }
      }

      private volatile bool _generationInProgress;

      public bool GenerationInProgress
      {
         get { return _generationInProgress; }
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

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
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
         _clientConfiguration.DictionaryChanged += (sender, e) => SetTimerState();
         _clientConfiguration.ClientDataDirty += (sender, e) =>
                                                 {
                                                    if (e.Name == null)
                                                    {
                                                       QueueNewRetrieval();
                                                    }
                                                    else
                                                    {
                                                       RetrieveSingleClient(e.Name);
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

         _prefs.TimerSettingsChanged += delegate
                                        {
                                           // stop
                                           _workTimer.Stop();
                                           _webTimer.Stop();
                                           // then reset
                                           SetTimerState();
                                        };
      }

      #region Retrieval Logic

      /// <summary>
      /// When the host refresh timer expires, refresh all the hosts
      /// </summary>
      private void WorkTimerTick(object sender, EventArgs e)
      {
         _logger.Info("Running Retrieval Process...");
         QueueNewRetrieval();
      }

      /// <summary>
      /// When the web gen timer expires, refresh the website files
      /// </summary>
      private void WebGenTimerTick(object sender, EventArgs e)
      {
         Debug.Assert(_prefs.Get<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.Get<bool>(Preference.WebGenAfterRefresh) == false);

         _logger.Info("Stopping web generation timer loop.");
         _webTimer.Stop();

         DoWebGeneration();

         StartWebGenTimer();
      }

      private void DoWebGeneration()
      {
         Debug.Assert(_prefs.Get<bool>(Preference.GenerateWeb));

         if (_generationInProgress)
         {
            _logger.Warn("Web Generation already in progress...");
            return;
         }

         _generationInProgress = true;
         try
         {
            DateTime start = Instrumentation.ExecStart;

            // lazy initialize
            if (_markupGenerator == null) _markupGenerator = ServiceLocator.Resolve<IMarkupGenerator>();
            if (_websiteDeployer == null) _websiteDeployer = ServiceLocator.Resolve<IWebsiteDeployer>();

            _logger.Info("Starting Web Generation...");

            var slots = _mainGridModel.SlotCollection;
            _markupGenerator.Generate(slots);
            _websiteDeployer.DeployWebsite(_markupGenerator.HtmlFilePaths, _markupGenerator.XmlFilePaths, slots);

            _logger.Info("Total Web Generation Execution Time: {0}", Instrumentation.GetExecTime(start));
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }
         finally
         {
            _generationInProgress = false;
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
            _logger.Warn("Retrieval already in progress...");
            return;
         }

         // only fire if there are Hosts
         if (_clientConfiguration.Count != 0)
         {
            _logger.Info("Stopping retrieval timer loop.");
            _workTimer.Stop();

            // fire the retrieval wrapper thread (basically a wait thread off the UI thread)
            _doRetrievalThread = new Thread(DoRetrievalWrapper) { IsBackground = true, Name = "DoRetrievalWrapper" };
            _doRetrievalThread.Start();
         }
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
            if (_prefs.Get<bool>(Preference.GenerateWeb) &&
                _prefs.Get<bool>(Preference.WebGenAfterRefresh))
            {
               // do a web gen (on another thread)
               var webGen = new Thread(DoWebGeneration) { IsBackground = true, Name = "DoWebGeneration" };
               webGen.Start();
            }

            // Enable the data retrieval timer
            if (_prefs.Get<bool>(Preference.SyncOnSchedule))
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
         var synchronous = _prefs.Get<bool>(Preference.SyncOnLoad);

         var clients = _clientConfiguration.GetClients().ToList();
         if (synchronous) // do the individual retrieves on a single thread
         {
            clients.ForEach(RetrieveInstance);
         }
         else // fire individual threads to do the their own retrieve simultaneously
         {
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
         Task.Factory.StartNew(() => RetrieveInstance(_clientConfiguration.Get(name)));
      }

      /// <summary>
      /// Disable and enable the background work timers
      /// </summary>
      private void SetTimerState()
      {
         // Disable timers if no hosts
         if (_clientConfiguration.Count == 0)
         {
            if (_workTimer.Enabled || _webTimer.Enabled)
            {
               _logger.Info("No Hosts - Stopping All Background Timer Loops");
            }
            _workTimer.Stop();
            _webTimer.Stop();
            return;
         }

         // Enable the data retrieval timer
         if (_prefs.Get<bool>(Preference.SyncOnSchedule))
         {
            if (!RetrievalInProgress)
            {
               StartBackgroundTimer();
            }
         }
         else
         {
            if (_workTimer.Enabled)
            {
               _logger.Info("Stopping Retrieval Timer Loop");
               _workTimer.Stop();
            }
         }

         // Enable the web generation timer
         if (_prefs.Get<bool>(Preference.GenerateWeb) &&
             _prefs.Get<bool>(Preference.WebGenAfterRefresh) == false)
         {
            StartWebGenTimer();
         }
         else
         {
            if (_webTimer.Enabled)
            {
               _logger.Info("Stopping Web Generation Timer Loop");
               _webTimer.Stop();
            }
         }
      }

      /// <summary>
      /// Starts Retrieval Timer Loop
      /// </summary>
      private void StartBackgroundTimer()
      {
         // don't start if already started
         if (_workTimer.Enabled) return;

         var syncTimeMinutes = _prefs.Get<int>(Preference.SyncTimeMinutes);

         _workTimer.Interval = syncTimeMinutes * Constants.MinToMillisec;
         _logger.Info("Starting Retrieval Timer Loop: {0} Minutes", syncTimeMinutes);
         _workTimer.Start();
      }

      /// <summary>
      /// Start the Web Generation Timer
      /// </summary>
      private void StartWebGenTimer()
      {
         // don't start if already started
         if (_webTimer.Enabled) return;

         Debug.Assert(_prefs.Get<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.Get<bool>(Preference.WebGenAfterRefresh) == false);

         var generateInterval = _prefs.Get<int>(Preference.GenerateInterval);

         _webTimer.Interval = generateInterval * Constants.MinToMillisec;
         _logger.Info("Starting Web Generation Timer Loop: {0} Minutes", generateInterval);
         _webTimer.Start();
      }

      #endregion
   }
}
