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
using System.Globalization;
using System.Threading;

using HFM.Framework;
using HFM.Instances;

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
               _retrieveExecStart = HfmTrace.ExecStart;
               _retrievalInProgress = true;
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Total Retrieval Execution Time: {0}", HfmTrace.GetExecTime(_retrieveExecStart)));
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

      private IMarkupGenerator _markupGenerator;

      private IWebsiteDeployer _websiteDeployer;

      private readonly IPreferenceSet _prefs;

      private readonly IMainView _mainView;

      private readonly InstanceCollection _instanceCollection;

      private readonly IXmlStatsDataContainer _statsData;

      private readonly IProteinBenchmarkContainer _benchmarkContainer;

      #endregion

      #endregion

      public RetrievalLogic(IPreferenceSet prefs, IMainView mainView, InstanceCollection instanceCollection, IXmlStatsDataContainer statsData,
                            IProteinBenchmarkContainer benchmarkContainer)
      {
         _prefs = prefs;
         _mainView = mainView;
         _instanceCollection = instanceCollection;
         _statsData = statsData;
         _benchmarkContainer = benchmarkContainer;
      }

      public void Initialize()
      {
         // Hook up Retrieval Timer Event Handler
         _workTimer.Elapsed += WorkTimerTick;
         // Hook up Web Generation Timer Event Handler
         _webTimer.Elapsed += WebGenTimerTick;

         //// Set Offline Clients Sort Flag
         //OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast);

         // Hook-up PreferenceSet Event Handlers
         //_prefs.OfflineLastChanged += delegate { OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast); };
         _prefs.TimerSettingsChanged += delegate { SetTimerState(); };
      }

      #region Retrieval Logic

      /// <summary>
      /// When the host refresh timer expires, refresh all the hosts
      /// </summary>
      private void WorkTimerTick(object sender, EventArgs e)
      {
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Running Retrieval Process...");
         QueueNewRetrieval();
      }

      /// <summary>
      /// When the web gen timer expires, refresh the website files
      /// </summary>
      private void WebGenTimerTick(object sender, EventArgs e)
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Web Generation Timer");
         _webTimer.Stop();

         DoWebGeneration();

         StartWebGenTimer();
      }

      private void DoWebGeneration()
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));

         // lazy initialize
         if (_markupGenerator == null) _markupGenerator = InstanceProvider.GetInstance<IMarkupGenerator>();
         if (_websiteDeployer == null) _websiteDeployer = InstanceProvider.GetInstance<IWebsiteDeployer>();

         try
         {
            if (_markupGenerator.GenerationInProgress)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Web Generation already in progress...");
            }
            else
            {
               DateTime start = HfmTrace.ExecStart;

               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Starting Web Generation...");

               ICollection<IDisplayInstance> displayInstances = _instanceCollection.GetCurrentDisplayInstanceArray();
               ICollection<ClientInstance> clientInstances = _instanceCollection.GetCurrentInstanceArray();

               _markupGenerator.Generate(displayInstances, clientInstances);
               _websiteDeployer.DeployWebsite(_markupGenerator.HtmlFilePaths, _markupGenerator.XmlFilePaths, _markupGenerator.ClientDataFilePath, displayInstances);

               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture,
                  "Total Web Generation Execution Time: {0}", HfmTrace.GetExecTime(start)));
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
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
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Retrieval Already In Progress...", HfmTrace.FunctionName));
            return;
         }

         // only fire if there are Hosts
         if (_instanceCollection.HasInstances)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Retrieval Timer Loop");
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
            if (_prefs.GetPreference<bool>(Preference.GenerateWeb) &&
                _prefs.GetPreference<bool>(Preference.WebGenAfterRefresh))
            {
               // do a web gen (on another thread)
               new Action(DoWebGeneration).BeginInvoke(null, null);
            }

            if (_prefs.GetPreference<bool>(Preference.ShowXmlStats))
            {
               RefreshUserStatsData(false);
            }

            // Enable the data retrieval timer
            if (_prefs.GetPreference<bool>(Preference.SyncOnSchedule))
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
         bool synchronous = _prefs.GetPreference<bool>(Preference.SyncOnLoad);

         // copy the current instance keys into local array
         int numInstances = _instanceCollection.Count;
         string[] instanceKeys = new string[numInstances];
         _instanceCollection.Keys.CopyTo(instanceKeys, 0);

         List<WaitHandle> waitHandleList = new List<WaitHandle>();
         for (int i = 0; i < numInstances; )
         {
            waitHandleList.Clear();
            // loop through the instances (can only handle up to 64 wait handles at a time)
            // limiting to 20 to reduce threadpool starvation
            for (int j = 0; j < 20 && i < numInstances; j++)
            {
               // try to get the key value from the collection, if the value is not found then
               // the user removed a client in the middle of a retrieve process, ignore the key
               ClientInstance instance;
               if (_instanceCollection.TryGetValue(instanceKeys[i], out instance))
               {
                  if (synchronous) // do the individual retrieves on a single thread
                  {
                     RetrieveInstance(instance);
                  }
                  else // fire individual threads to do the their own retrieve simultaneously
                  {
                     IAsyncResult async = QueueNewRetrieval(instance);

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

         // check for clients with duplicate Project (Run, Clone, Gen) or UserID
         DuplicateFinder.FindDuplicates(_instanceCollection.DisplayCollection);
         _mainView.DataGridView.Invalidate();

         // Save the benchmark collection
         _benchmarkContainer.Write();
      }

      /// <summary>
      /// Stick this Instance in the background thread queue to retrieve the info for the given Instance
      /// </summary>
      private IAsyncResult QueueNewRetrieval(ClientInstance instance)
      {
         return new Action<ClientInstance>(RetrieveInstance).BeginInvoke(instance, null, null);
      }

      /// <summary>
      /// Stub to execute retrieve and refresh display
      /// </summary>
      /// <param name="instance"></param>
      private void RetrieveInstance(ClientInstance instance)
      {
         if (instance.RetrievalInProgress == false)
         {
            instance.Retrieve();
            // signal the UI to update
            _mainView.RefreshDisplay();
         }
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      /// <param name="instanceName">Client Instance Name</param>
      public void RetrieveSingleClient(string instanceName)
      {
         RetrieveSingleClient(_instanceCollection.GetClientInstance(instanceName));
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      /// <param name="instance">Client Instance</param>
      public void RetrieveSingleClient(ClientInstance instance)
      {
         // fire the actual retrieval thread
         new Action<ClientInstance>(DoSingleClientRetieval).BeginInvoke(instance, null, null);
      }

      /// <summary>
      /// Do a single retrieval operation on the given Client Instance
      /// </summary>
      /// <param name="instance">Client Instance</param>
      private void DoSingleClientRetieval(ClientInstance instance)
      {
         if (instance.RetrievalInProgress == false)
         {
            IAsyncResult async = QueueNewRetrieval(instance);
            async.AsyncWaitHandle.WaitOne();

            // check for clients with duplicate Project (Run, Clone, Gen) or UserID
            DuplicateFinder.FindDuplicates(_instanceCollection.DisplayCollection);
            _mainView.DataGridView.Invalidate();

            // Save the benchmark collection
            _benchmarkContainer.Write();
         }
      }

      /// <summary>
      /// Disable and enable the background work timers
      /// </summary>
      public void SetTimerState()
      {
         // Disable timers if no hosts
         if (_instanceCollection.HasInstances == false)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, "No Hosts - Stopping All Background Timer Loops");
            _workTimer.Stop();
            _webTimer.Stop();
            return;
         }

         // Enable the data retrieval timer
         if (_prefs.GetPreference<bool>(Preference.SyncOnSchedule))
         {
            if (RetrievalInProgress == false)
            {
               StartBackgroundTimer();
            }
         }
         else
         {
            if (_workTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Retrieval Timer Loop");
               _workTimer.Stop();
            }
         }

         // Enable the web generation timer
         if (_prefs.GetPreference<bool>(Preference.GenerateWeb) &&
             _prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false)
         {
            StartWebGenTimer();
         }
         else
         {
            if (_webTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Web Generation Timer Loop");
               _webTimer.Stop();
            }
         }
      }

      /// <summary>
      /// Starts Retrieval Timer Loop
      /// </summary>
      private void StartBackgroundTimer()
      {
         var syncTimeMinutes = _prefs.GetPreference<int>(Preference.SyncTimeMinutes);

         _workTimer.Interval = syncTimeMinutes * Constants.MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Starting Retrieval Timer Loop: {0} Minutes", syncTimeMinutes));
         _workTimer.Start();
      }

      /// <summary>
      /// Start the Web Generation Timer
      /// </summary>
      private void StartWebGenTimer()
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);

         var generateInterval = _prefs.GetPreference<int>(Preference.GenerateInterval);

         _webTimer.Interval = generateInterval * Constants.MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture,
            "Starting Web Generation Timer Loop: {0} Minutes", generateInterval));
         _webTimer.Start();
      }

      /// <summary>
      /// Refresh Stats Data from EOC
      /// </summary>
      /// <param name="forceRefresh">If true, ignore last refresh time stamps and update.</param>
      public void RefreshUserStatsData(bool forceRefresh)
      {
         _statsData.GetEocXmlData(forceRefresh);
         _mainView.RefreshUserStatsControls();
      }

      #endregion
   }
}
