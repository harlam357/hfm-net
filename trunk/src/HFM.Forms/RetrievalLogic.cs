/*
 * HFM.NET - Retrieval Logic Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Castle.Core.Logging;

using HFM.Core;

namespace HFM.Forms
{
   public class RetrievalLogic
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
      private readonly IClientDictionary _clientDictionary;

      #endregion

      #endregion

      public RetrievalLogic(IPreferenceSet prefs, IClientDictionary clientDictionary)
      {
         _prefs = prefs;
         _clientDictionary = clientDictionary;
         _clientDictionary.DictionaryChanged += delegate { SetTimerState(); };
         _clientDictionary.ClientDataDirty += (sender, e) =>
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

         // lazy initialize
         if (_markupGenerator == null) _markupGenerator = ServiceLocator.Resolve<IMarkupGenerator>();
         if (_websiteDeployer == null) _websiteDeployer = ServiceLocator.Resolve<IWebsiteDeployer>();

         try
         {
            if (_markupGenerator.GenerationInProgress)
            {
               _logger.Info("Web Generation already in progress...");
            }
            else
            {
               DateTime start = Instrumentation.ExecStart;

               _logger.Info("Starting Web Generation...");

               var slots = _clientDictionary.Slots.ToList();
               _markupGenerator.Generate(slots);
               _websiteDeployer.DeployWebsite(_markupGenerator.HtmlFilePaths, _markupGenerator.XmlFilePaths, slots);

               _logger.Info("Total Web Generation Execution Time: {0}", Instrumentation.GetExecTime(start));
            }
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
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
            _logger.Info("Retrieval already in progress...");
            return;
         }

         // only fire if there are Hosts
         if (_clientDictionary.Count != 0)
         {
            _logger.Info("Stopping retrieval timer loop.");
            _workTimer.Stop();

            // fire the retrieval wrapper thread (basically a wait thread off the UI thread)
            new Action(DoRetrievalWrapper).BeginInvoke(null, null);
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
            IAsyncResult async = new Action(DoRetrieval).BeginInvoke(null, null);
            // wait for completion
            async.AsyncWaitHandle.WaitOne();

            // run post retrieval processes
            if (_prefs.Get<bool>(Preference.GenerateWeb) &&
                _prefs.Get<bool>(Preference.WebGenAfterRefresh))
            {
               // do a web gen (on another thread)
               new Action(DoWebGeneration).BeginInvoke(null, null);
            }

            if (_prefs.Get<bool>(Preference.ShowXmlStats))
            {
               //_presenter.RefreshUserStatsData(false);
            }

            // Enable the data retrieval timer
            if (_prefs.Get<bool>(Preference.SyncOnSchedule))
            {
               StartBackgroundTimer();
            }
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
         bool synchronous = _prefs.Get<bool>(Preference.SyncOnLoad);

         // copy the current instance keys into local array
         int numInstances = _clientDictionary.Count;
         var instanceKeys = new string[numInstances];
         _clientDictionary.Keys.CopyTo(instanceKeys, 0);

         var waitHandleList = new List<WaitHandle>();
         for (int i = 0; i < numInstances; )
         {
            waitHandleList.Clear();
            // loop through the instances (can only handle up to 64 wait handles at a time)
            // limiting to 20 to reduce threadpool starvation
            for (int j = 0; j < 20 && i < numInstances; j++)
            {
               // try to get the key value from the collection, if the value is not found then
               // the user removed a client in the middle of a retrieve process, ignore the key
               IClient client;
               if (_clientDictionary.TryGetValue(instanceKeys[i], out client))
               {
                  if (synchronous) // do the individual retrieves on a single thread
                  {
                     RetrieveInstance(client);
                  }
                  else // fire individual threads to do the their own retrieve simultaneously
                  {
                     IAsyncResult async = QueueNewRetrieval(client);

                     // get the wait handle for each invoked delegate
                     waitHandleList.Add(async.AsyncWaitHandle);
                  }
               }

               i++; // increment the outer loop counter
            }

            if (synchronous == false)
            {
               WaitHandle[] waitHandles = waitHandleList.ToArray();
               // wait for all invoked threads to complete
               WaitHandle.WaitAll(waitHandles);
            }
         }
      }

      /// <summary>
      /// Stick this Instance in the background thread queue to retrieve the info for the given Instance
      /// </summary>
      private static IAsyncResult QueueNewRetrieval(IClient client)
      {
         return new Action<IClient>(RetrieveInstance).BeginInvoke(client, null, null);
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
         RetrieveSingleClient(_clientDictionary[name]);
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      public void RetrieveSingleClient(IClient client)
      {
         // fire the actual retrieval thread
         new Action<IClient>(DoSingleClientRetieval).BeginInvoke(client, null, null);
      }

      /// <summary>
      /// Do a single retrieval operation on the given Client Instance
      /// </summary>
      private static void DoSingleClientRetieval(IClient client)
      {
         if (client.RetrievalInProgress == false)
         {
            IAsyncResult async = QueueNewRetrieval(client);
            async.AsyncWaitHandle.WaitOne();
         }
      }

      /// <summary>
      /// Disable and enable the background work timers
      /// </summary>
      public void SetTimerState()
      {
         // Disable timers if no hosts
         if (_clientDictionary.Count == 0)
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
