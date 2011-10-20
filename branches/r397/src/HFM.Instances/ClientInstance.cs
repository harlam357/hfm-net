/*
 * HFM.NET - Client Instance Class
 * Copyright (C) 2006 David Rawling
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public sealed class ClientInstance
   {
      #region Fields

      #region Injection Properties

      private IPreferenceSet _prefs;
      /// <summary>
      /// PreferenceSet Interface
      /// </summary>
      public IPreferenceSet Prefs
      {
         set { _prefs = value; }
      }
      
      private IProteinCollection _proteinCollection;
      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      public IProteinCollection ProteinCollection
      {
         set { _proteinCollection = value; }
      }
      
      private IProteinBenchmarkContainer _benchmarkContainer;
      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      public IProteinBenchmarkContainer BenchmarkContainer
      {
         set { _benchmarkContainer = value; }
      }
      
      private IStatusLogic _statusLogic;
      /// <summary>
      /// Status Logic Interface
      /// </summary>
      public IStatusLogic StatusLogic
      {
         set { _statusLogic = value; }
      }

      private IDataRetriever _dataRetriever;
      /// <summary>
      /// Data Retriever Interface
      /// </summary>
      public IDataRetriever DataRetriever
      {
         set { _dataRetriever = value; }
      }

      private IDataAggregator _dataAggregator;
      /// <summary>
      /// Data Aggregator Interface
      /// </summary>
      public IDataAggregator DataAggregator
      {
         set { _dataAggregator = value; }
      }

      #endregion

      private ClientInstanceSettings _settings;
      
      public ClientInstanceSettings Settings
      {
         get { return _settings; }
         set
         {
            Debug.Assert(value != null);

            if (_settings == null)
            {
               // add default instance for all client types
               DisplayInstances.Add(0, CreateDisplayInstance(value));
            }
            _settings = value;
         }
      }
      
      public IDictionary<int, DisplayInstance> DisplayInstances { get; private set; }

      #endregion

      public ClientInstance()
      {
         DisplayInstances = new Dictionary<int, DisplayInstance>();
      }
      
      #region Retrieval Properties
      
      private volatile bool _retrievalInProgress;
      /// <summary>
      /// Local flag set when log retrieval is in progress
      /// </summary>
      public bool RetrievalInProgress
      {
         get { return _retrievalInProgress; }
         private set 
         { 
            _retrievalInProgress = value;
         }
      }

      #endregion

      #region Retrieval Methods

      private DisplayInstance CreateDisplayInstance(ClientInstanceSettings settings)
      {
         return CreateDisplayInstance(settings, new Protein());
      }

      private DisplayInstance CreateDisplayInstance(ClientInstanceSettings settings, IProtein protein)
      {
         // Init User Specified Client Level Members
         var displayInstance = new DisplayInstance
                               {
                                  Prefs = _prefs,
                                  BenchmarkContainer = _benchmarkContainer,
                                  Settings = settings,
                                  UnitInfo = new UnitInfo()
                               };
         displayInstance.BuildUnitInfoLogic(protein ?? _proteinCollection.GetProtein(displayInstance.UnitInfo.ProjectID, false));
         displayInstance.InitClientLevelMembers();

         return displayInstance;
      }
      
      /// <summary>
      /// Retrieve Instance Log Files based on Instance Type
      /// </summary>
      public void Retrieve()
      {
         // Don't allow this to fire more than once at a time
         if (RetrievalInProgress) return;

         try
         {
            RetrievalInProgress = true;
            
            if (Settings.InstanceHostType.IsLegacyType())
            {
               if (Settings.ExternalInstance)
               {
                  DisplayInstances.Clear();

                  try
                  {
                     _dataRetriever.Execute(Settings);
                     ReadExternalDataFile(Path.Combine(_prefs.CacheDirectory, Settings.CachedExternalName));
                  }
                  catch (Exception ex)
                  {
                     // problem retrieving or reading the external file
                     // create a default display instance so we have something
                     // to show on the data grid
                     DisplayInstance displayInstance = CreateDisplayInstance(Settings);
                     DisplayInstances.Add(0, displayInstance);

                     displayInstance.Status = ClientStatus.Offline;
                     HfmTrace.WriteToHfmConsole(displayInstance.Settings.InstanceName, ex);
                  }
               }
               else
               {
                  Debug.Assert(DisplayInstances.Count == 1);
                  DisplayInstance displayInstance = DisplayInstances[0];

                  try
                  {
                     _dataRetriever.Execute(Settings);
                     // Set successful Last Retrieval Time
                     displayInstance.LastRetrievalTime = DateTime.Now;
                     // Re-Init Client Level Members Before Processing
                     displayInstance.InitClientLevelMembers();
                     // Process the retrieved logs

                     ClientStatus returnedStatus = ProcessLegacy(displayInstance);
                     // Handle the status retured from the log parse
                     HandleReturnedStatus(returnedStatus, displayInstance);

                     HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Client Status: {2}", HfmTrace.FunctionName, displayInstance.Settings.InstanceName, displayInstance.Status));
                  }
                  catch (Exception ex)
                  {
                     displayInstance.Status = ClientStatus.Offline;
                     HfmTrace.WriteToHfmConsole(displayInstance.Settings.InstanceName, ex);
                  }
               }
            }
            else
            {
               // v7 client
            }
         }
         finally
         {
            RetrievalInProgress = false;
         }
      }

      private void ReadExternalDataFile(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         try
         {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
               var list = ProtoBuf.Serializer.Deserialize<List<DisplayInstance>>(fileStream);
               foreach (var instance in list)
               {
                  // set external instance key - also used to identify display instances
                  // that are bound to an external client instance
                  instance.ExternalInstanceName = Settings.InstanceName;
                  // update the instance name to specify the merged source name
                  instance.Settings.InstanceName = String.Format(CultureInfo.InvariantCulture,
                     "{0} ({1})", instance.Name, instance.ExternalInstanceName);
                  instance.Prefs = _prefs;
                  instance.BuildUnitInfoLogic(_proteinCollection.GetProtein(instance.UnitInfo.ProjectID, false));
               }
               
               for (int i = 0; i < list.Count; i++)
               {
                  DisplayInstances.Add(i, list[i]);
               }
            }
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
         }
      }
      
      #endregion

      #region Queue and Log Processing Functions
      
      /// <summary>
      /// Process the cached log files that exist on this machine
      /// </summary>
      private ClientStatus ProcessLegacy(DisplayInstance displayInstance)
      {
         DateTime start = HfmTrace.ExecStart;

         #region Setup UnitInfo Aggregator

         _dataAggregator.InstanceName = Settings.InstanceName;
         _dataAggregator.QueueFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueName);
         _dataAggregator.FahLogFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogName);
         _dataAggregator.UnitInfoLogFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoName); 
         
         #endregion
         
         #region Run the Aggregator and Set ClientInstance Level Results
         
         IList<UnitInfo> units = _dataAggregator.AggregateData();
         // Issue 126 - Use the Folding ID, Team, User ID, and Machine ID from the FAHlog data.
         // Use the Current Queue Entry as a backup data source.
         PopulateRunLevelData(_dataAggregator.CurrentClientRun, displayInstance);
         if (_dataAggregator.Queue != null)
         {
            PopulateRunLevelData(_dataAggregator.Queue.CurrentQueueEntry, displayInstance);
         }

         displayInstance.Queue = _dataAggregator.Queue;
         displayInstance.CurrentLogLines = _dataAggregator.CurrentLogLines;
         displayInstance.UnitLogLines = _dataAggregator.UnitLogLines;
         
         #endregion
         
         var parsedUnits = new UnitInfoLogic[units.Count];
         for (int i = 0; i < units.Count; i++)
         {
            if (units[i] != null)
            {
               IProtein protein = _proteinCollection.GetProtein(units[i].ProjectID);
               units[i].UnitRetrievalTime = displayInstance.LastRetrievalTime;
               parsedUnits[i] = new UnitInfoLogic(protein, _benchmarkContainer, units[i], Settings);
            }
         }

         // *** THIS HAS TO BE DONE BEFORE UPDATING THE CurrentUnitInfo ***
         // Update Benchmarks from parsedUnits array 
         _benchmarkContainer.UpdateBenchmarkData(displayInstance.CurrentUnitInfo, parsedUnits, _dataAggregator.CurrentUnitIndex);

         // Update the CurrentUnitInfo if we have a Status
         ClientStatus currentWorkUnitStatus = _dataAggregator.CurrentClientRun.Status;
         if (currentWorkUnitStatus.Equals(ClientStatus.Unknown) == false)
         {
            displayInstance.CurrentUnitInfo = parsedUnits[_dataAggregator.CurrentUnitIndex];
         }
         
         displayInstance.CurrentUnitInfo.ShowPPDTrace(displayInstance.Status, 
            _prefs.Get<PpdCalculationType>(Preference.PpdCalculation),
            _prefs.Get<bool>(Preference.CalculateBonus));
         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, start);

         // Return the Status
         return currentWorkUnitStatus;
      }

      private static void PopulateRunLevelData(ClientRun run, DisplayInstance displayInstance)
      {
         displayInstance.ClientVersion = run.ClientVersion;
         displayInstance.Arguments = run.Arguments;

         //displayInstance.FoldingID = run.FoldingID;
         //displayInstance.Team = run.Team;

         displayInstance.UserId = run.UserID;
         displayInstance.MachineId = run.MachineID;

         displayInstance.TotalRunCompletedUnits = run.CompletedUnits;
         displayInstance.TotalRunFailedUnits = run.FailedUnits;
         displayInstance.TotalClientCompletedUnits = run.TotalCompletedUnits;
      }

      private static void PopulateRunLevelData(ClientQueueEntry queueEntry, DisplayInstance displayInstance)
      {
         //if (_displayInstance.FoldingID == Constants.FoldingIDDefault)
         //{
         //   _displayInstance.FoldingID = queueEntry.FoldingID;
         //}
         //if (_displayInstance.Team == Constants.TeamDefault)
         //{
         //   _displayInstance.Team = (int)queueEntry.TeamNumber;
         //}
         if (displayInstance.UserId == Constants.DefaultUserID)
         {
            displayInstance.UserId = queueEntry.UserID;
         }
         if (displayInstance.MachineId == Constants.DefaultMachineID)
         {
            displayInstance.MachineId = queueEntry.MachineID;
         }
      }
      
      #endregion

      #region Status Handling and Determination
      
      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determines what values to feed the DetermineStatus routine.
      /// </summary>
      private void HandleReturnedStatus(ClientStatus returnedStatus, DisplayInstance displayInstance)
      {
         var statusData = new StatusData
                          {
                             InstanceName = Settings.InstanceName,
                             TypeOfClient = displayInstance.CurrentUnitInfo.UnitInfoData.TypeOfClient,
                             LastRetrievalTime = displayInstance.LastRetrievalTime,
                             IgnoreUtcOffset = Settings.ClientIsOnVirtualMachine,
                             UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now),
                             ClientTimeOffset = Settings.ClientTimeOffset,
                             TimeOfLastUnitStart = displayInstance.TimeOfLastUnitStart,
                             TimeOfLastFrameProgress = displayInstance.TimeOfLastFrameProgress,
                             CurrentStatus = displayInstance.Status,
                             ReturnedStatus = returnedStatus,
                             FrameTime = displayInstance.CurrentUnitInfo.GetRawTime(_prefs.Get<PpdCalculationType>(Preference.PpdCalculation)),
                             AverageFrameTime = _benchmarkContainer.GetBenchmarkAverageFrameTime(displayInstance.CurrentUnitInfo),
                             TimeOfLastFrame = displayInstance.CurrentUnitInfo.UnitInfoData.CurrentFrame == null
                                                  ? TimeSpan.Zero
                                                  : displayInstance.CurrentUnitInfo.UnitInfoData.CurrentFrame.TimeOfFrame,
                             UnitStartTimeStamp = displayInstance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp,
                             AllowRunningAsync = _prefs.GetPreference<bool>(Preference.AllowRunningAsync)
                          };

         // If the returned status is EuePause and current status is not
         if (statusData.ReturnedStatus.Equals(ClientStatus.EuePause) && statusData.CurrentStatus.Equals(ClientStatus.EuePause) == false)
         {
            if (_prefs.GetPreference<bool>(Preference.EmailReportingEnabled) &&
                _prefs.GetPreference<bool>(Preference.ReportEuePause))
            {
               SendEuePauseEmail(statusData.InstanceName, _prefs);
            }
         }

         // If the returned status is Hung and current status is not
         if (statusData.ReturnedStatus.Equals(ClientStatus.Hung) && statusData.CurrentStatus.Equals(ClientStatus.Hung) == false)
         {
            if (_prefs.GetPreference<bool>(Preference.EmailReportingEnabled) &&
                _prefs.GetPreference<bool>(Preference.ReportHung))
            {
               SendHungEmail(statusData.InstanceName, _prefs);
            }
         }

         displayInstance.Status = _statusLogic.HandleStatusData(statusData);
      }

      /// <summary>
      /// Send EuePause Status Email
      /// </summary>
      private static void SendEuePauseEmail(string instanceName, IPreferenceSet prefs)
      {
         string messageBody = String.Format("HFM.NET detected that Client '{0}' has entered a 24 hour EUE Pause state.", instanceName);
         try
         {
            NetworkOps.SendEmail(prefs.GetPreference<bool>(Preference.EmailReportingServerSecure), 
                                 prefs.GetPreference<string>(Preference.EmailReportingFromAddress), 
                                 prefs.GetPreference<string>(Preference.EmailReportingToAddress),
                                 "HFM.NET - Client EUE Pause Error", messageBody, 
                                 prefs.GetPreference<string>(Preference.EmailReportingServerAddress),
                                 prefs.GetPreference<int>(Preference.EmailReportingServerPort),
                                 prefs.GetPreference<string>(Preference.EmailReportingServerUsername), 
                                 prefs.GetPreference<string>(Preference.EmailReportingServerPassword));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }

      /// <summary>
      /// Send Hung Status Email
      /// </summary>
      private static void SendHungEmail(string instanceName, IPreferenceSet prefs)
      {
         string messageBody = String.Format("HFM.NET detected that Client '{0}' has entered a Hung state.", instanceName);
         try
         {
            NetworkOps.SendEmail(prefs.GetPreference<bool>(Preference.EmailReportingServerSecure),
                                 prefs.GetPreference<string>(Preference.EmailReportingFromAddress),
                                 prefs.GetPreference<string>(Preference.EmailReportingToAddress),
                                 "HFM.NET - Client Hung Error", messageBody,
                                 prefs.GetPreference<string>(Preference.EmailReportingServerAddress),
                                 prefs.GetPreference<int>(Preference.EmailReportingServerPort),
                                 prefs.GetPreference<string>(Preference.EmailReportingServerUsername),
                                 prefs.GetPreference<string>(Preference.EmailReportingServerPassword));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }

      #endregion
   }
}
