/*
 * HFM.NET - Client Instance Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
   [CLSCompliant(false)]
   public interface IClientInstance
   {
      /// <summary>
      /// Client Instance Settings
      /// </summary>
      IClientInstanceSettings Settings { get; }
      
      IDisplayInstance DisplayInstance { get; }

      IList<IDisplayInstance> ExternalDisplayInstances { get; }
   }

   public sealed class ClientInstance : IClientInstance
   {
      #region Fields

      #region ReadOnly

      /// <summary>
      /// PreferenceSet Interface
      /// </summary>
      private readonly IPreferenceSet _prefs;
      
      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      private readonly IProteinCollection _proteinCollection;
      
      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      private readonly IProteinBenchmarkContainer _benchmarkContainer;
      
      /// <summary>
      /// Status Logic Interface
      /// </summary>
      private readonly IStatusLogic _statusLogic;

      /// <summary>
      /// Data Retriever Interface
      /// </summary>
      private readonly IDataRetriever _dataRetriever;

      /// <summary>
      /// Data Aggregator Interface
      /// </summary>
      private readonly IDataAggregator _dataAggregator;
      
      #endregion
      
      private readonly DisplayInstance _displayInstance;
      
      public IClientInstanceSettings Settings
      {
         get { return _displayInstance.Settings; }
      }
      
      [CLSCompliant(false)]
      public IDisplayInstance DisplayInstance
      {
         get { return _displayInstance; }
      }

      [CLSCompliant(false)]
      public IList<IDisplayInstance> ExternalDisplayInstances { get; private set; }

      #endregion
      
      #region Constructor
      /// <summary>
      /// Primary Constructor
      /// </summary>
      [CLSCompliant(false)]
      public ClientInstance(IPreferenceSet prefs, IProteinCollection proteinCollection, IProteinBenchmarkContainer benchmarkContainer,
                            IStatusLogic statusLogic, IDataRetriever dataRetriever, IDataAggregator dataAggregator)
         : this(prefs, proteinCollection, benchmarkContainer, statusLogic, dataRetriever, dataAggregator, null)
      {
         
      }
      
      /// <summary>
      /// Primary Constructor
      /// </summary>
      [CLSCompliant(false)]
      public ClientInstance(IPreferenceSet prefs, IProteinCollection proteinCollection, IProteinBenchmarkContainer benchmarkContainer,
                            IStatusLogic statusLogic, IDataRetriever dataRetriever, IDataAggregator dataAggregator, ClientInstanceSettings instanceSettings)
      {
         _prefs = prefs;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         _statusLogic = statusLogic;
         _dataRetriever = dataRetriever;
         _dataAggregator = dataAggregator;
         
         // Init User Specified Client Level Members
         _displayInstance = new DisplayInstance
                            {
                               Prefs = _prefs,
                               ProteinCollection = _proteinCollection,
                               BenchmarkContainer = _benchmarkContainer,
                               SettingsConcrete = instanceSettings ?? new ClientInstanceSettings(InstanceType.PathInstance),
                               UnitInfo = new UnitInfo()
                            };
         _displayInstance.BuildUnitInfoLogic();
         
         // Init Client Level Members
         Init();
      }
      #endregion

      #region Client Level Members

      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      public IUnitInfoLogic CurrentUnitInfo
      {
         get { return _displayInstance.CurrentUnitInfo; }
         private set
         {
            UpdateTimeOfLastProgress(value);
            _displayInstance.CurrentUnitInfo = value;
         }
      }

      /// <summary>
      /// Init Client Level Members
      /// </summary>
      private void Init()
      {
         _displayInstance.Arguments = String.Empty;
         _displayInstance.UserId = Constants.DefaultUserID;
         _displayInstance.MachineId = Constants.DefaultMachineID;
         //_displayInstance.FoldingID = Constants.FoldingIDDefault;
         //_displayInstance.Team = Constants.TeamDefault;
         _displayInstance.TotalRunCompletedUnits = 0;
         _displayInstance.TotalRunFailedUnits = 0;
         _displayInstance.TotalClientCompletedUnits = 0;
      }
      
      #endregion

      #region Unit Progress Client Level Members
      
      private DateTime _timeOfLastUnitStart = DateTime.MinValue;
      /// <summary>
      /// Local Time when this Client last detected Frame Progress
      /// </summary>
      internal DateTime TimeOfLastUnitStart
      {
         get { return _timeOfLastUnitStart; }
         set { _timeOfLastUnitStart = value; }
      }

      private DateTime _timeOfLastFrameProgress = DateTime.MinValue;
      /// <summary>
      /// Local Time when this Client last detected Frame Progress
      /// </summary>
      internal DateTime TimeOfLastFrameProgress
      {
         get { return _timeOfLastFrameProgress; }
         set { _timeOfLastFrameProgress = value; }
      } 
      
      #endregion

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

            _dataRetriever.Settings = _displayInstance.Settings;
            switch (_displayInstance.Settings.InstanceHostType)
            {
               case InstanceType.PathInstance:
                  _dataRetriever.RetrievePathInstance();
                  break;
               case InstanceType.HttpInstance:
                  _dataRetriever.RetrieveHttpInstance();
                  break;
               case InstanceType.FtpInstance:
                  _dataRetriever.RetrieveFtpInstance();
                  break;
               default:
                  throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
                     "Instance Type '{0}' is not implemented", _displayInstance.Settings.InstanceHostType));
            }

            // Set successful Last Retrieval Time
            _displayInstance.LastRetrievalTime = DateTime.Now;
            // Re-Init Client Level Members Before Processing
            Init();
            // Process the retrieved logs
            if (Settings.ExternalInstance)
            {
               ReadExternalDataFile(Path.Combine(_prefs.CacheDirectory, _displayInstance.Settings.CachedExternalName));
               //_displayInstance.Status = ClientStatus.Running;
            }
            else
            {
               ClientStatus returnedStatus = ProcessExisting();
               // Handle the status retured from the log parse
               HandleReturnedStatus(returnedStatus);
            }
         }
         catch (Exception ex)
         {
            _displayInstance.Status = ClientStatus.Offline;
            HfmTrace.WriteToHfmConsole(_displayInstance.Settings.InstanceName, ex);
         }
         finally
         {
            RetrievalInProgress = false;
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Client Status: {2}", HfmTrace.FunctionName, _displayInstance.Settings.InstanceName, _displayInstance.Status));
      }

      private void ReadExternalDataFile(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         ExternalDisplayInstances = null;
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
                  instance.ProteinCollection = _proteinCollection;
                  instance.BuildUnitInfoLogic();
               }
               ExternalDisplayInstances = list.ConvertAll(x => (IDisplayInstance)x);
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
      public ClientStatus ProcessExisting()
      {
         DateTime start = HfmTrace.ExecStart;

         #region Setup UnitInfo Aggregator

         _dataAggregator.InstanceName = _displayInstance.Settings.InstanceName;
         _dataAggregator.QueueFilePath = Path.Combine(_prefs.CacheDirectory, _displayInstance.Settings.CachedQueueName);
         _dataAggregator.FahLogFilePath = Path.Combine(_prefs.CacheDirectory, _displayInstance.Settings.CachedFahLogName);
         _dataAggregator.UnitInfoLogFilePath = Path.Combine(_prefs.CacheDirectory, _displayInstance.Settings.CachedUnitInfoName); 
         
         #endregion
         
         #region Run the Aggregator and Set ClientInstance Level Results
         
         IList<IUnitInfo> units = _dataAggregator.AggregateData();
         // Issue 126 - Use the Folding ID, Team, User ID, and Machine ID from the FAHlog data.
         // Use the Current Queue Entry as a backup data source.
         PopulateRunLevelData(_dataAggregator.CurrentClientRun);
         if (_dataAggregator.Queue != null)
         {
            PopulateRunLevelData(_dataAggregator.Queue.CurrentQueueEntry);
         }

         _displayInstance.Queue = _dataAggregator.Queue;
         _displayInstance.CurrentLogLines = _dataAggregator.CurrentLogLines;
         _displayInstance.UnitLogLines = _dataAggregator.UnitLogLines;
         
         #endregion
         
         var parsedUnits = new UnitInfoLogic[units.Count];
         for (int i = 0; i < units.Count; i++)
         {
            if (units[i] != null)
            {
               IProtein protein = _proteinCollection.GetProtein(units[i].ProjectID);
               parsedUnits[i] = new UnitInfoLogic(_prefs, protein, _benchmarkContainer, units[i], DisplayInstance.Settings, DisplayInstance);
            }
         }

         // *** THIS HAS TO BE DONE BEFORE UPDATING THE CurrentUnitInfo ***
         // Update Benchmarks from parsedUnits array 
         _benchmarkContainer.UpdateBenchmarkData(CurrentUnitInfo, parsedUnits, _dataAggregator.CurrentUnitIndex);

         // Update the CurrentUnitInfo if we have a Status
         ClientStatus currentWorkUnitStatus = _dataAggregator.CurrentWorkUnitStatus;
         if (currentWorkUnitStatus.Equals(ClientStatus.Unknown) == false)
         {
            CurrentUnitInfo = parsedUnits[_dataAggregator.CurrentUnitIndex];
         }
         
         CurrentUnitInfo.ShowPPDTrace();
         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, DisplayInstance.Settings.InstanceName, start);

         // Return the Status
         return currentWorkUnitStatus;
      }

      private void PopulateRunLevelData(IClientRun run)
      {
         _displayInstance.ClientVersion = run.ClientVersion;
         _displayInstance.Arguments = run.Arguments;

         //_displayInstance.FoldingID = run.FoldingID;
         //_displayInstance.Team = run.Team;

         _displayInstance.UserId = run.UserID;
         _displayInstance.MachineId = run.MachineID;

         _displayInstance.TotalRunCompletedUnits = run.NumberOfCompletedUnits;
         _displayInstance.TotalRunFailedUnits = run.NumberOfFailedUnits;
         _displayInstance.TotalClientCompletedUnits = run.NumberOfTotalUnitsCompleted;
      }

      private void PopulateRunLevelData(IQueueEntry queueEntry)
      {
         //if (_displayInstance.FoldingID == Constants.FoldingIDDefault)
         //{
         //   _displayInstance.FoldingID = queueEntry.FoldingID;
         //}
         //if (_displayInstance.Team == Constants.TeamDefault)
         //{
         //   _displayInstance.Team = (int)queueEntry.TeamNumber;
         //}
         if (_displayInstance.UserId == Constants.DefaultUserID)
         {
            _displayInstance.UserId = queueEntry.UserID;
         }
         if (_displayInstance.MachineId == Constants.DefaultMachineID)
         {
            _displayInstance.MachineId = (int)queueEntry.MachineID;
         }
      }

      /// <summary>
      /// Update Time of Last Frame Progress based on Current and Parsed UnitInfo
      /// </summary>
      private void UpdateTimeOfLastProgress(IUnitInfoLogic parsedUnitInfo)
      {
         // Matches the Current Project and Raw Download Time
         if (PlatformOps.IsUnitInfoCurrentUnitInfo(CurrentUnitInfo, parsedUnitInfo))
         {
            // If the Unit Start Time Stamp is no longer the same as the CurrentUnitInfo
            if (parsedUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                parsedUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp) == false)
            {
               TimeOfLastUnitStart = DateTime.Now;
            }
         
            // If the Last Unit Frame ID is greater than the CurrentUnitInfo Last Unit Frame ID
            if (parsedUnitInfo.LastUnitFrameID > CurrentUnitInfo.LastUnitFrameID)
            {
               // Update the Time Of Last Frame Progress
               TimeOfLastFrameProgress = DateTime.Now;
            }
         }
         else // Different UnitInfo - Update the Time Of Last 
              // Unit Start and Clear Frame Progress Value
         {
            TimeOfLastUnitStart = DateTime.Now;
            TimeOfLastFrameProgress = DateTime.MinValue;
         }
      }
      
      #endregion

      #region Status Handling and Determination
      
      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determines what values to feed the DetermineStatus routine.
      /// </summary>
      /// <param name="returnedStatus">Client Status</param>
      private void HandleReturnedStatus(ClientStatus returnedStatus)
      {
         var statusData = new StatusData
                          {
                             InstanceName = _displayInstance.Settings.InstanceName,
                             TypeOfClient = CurrentUnitInfo.UnitInfoData.TypeOfClient,
                             LastRetrievalTime = _displayInstance.LastRetrievalTime,
                             IgnoreUtcOffset = _displayInstance.Settings.ClientIsOnVirtualMachine,
                             UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now),
                             ClientTimeOffset = _displayInstance.Settings.ClientTimeOffset,
                             TimeOfLastUnitStart = TimeOfLastUnitStart,
                             TimeOfLastFrameProgress = TimeOfLastFrameProgress,
                             CurrentStatus = _displayInstance.Status,
                             ReturnedStatus = returnedStatus,
                             FrameTime = CurrentUnitInfo.RawTimePerSection,
                             AverageFrameTime = _benchmarkContainer.GetBenchmarkAverageFrameTime(CurrentUnitInfo),
                             TimeOfLastFrame = CurrentUnitInfo.TimeOfLastFrame,
                             UnitStartTimeStamp = CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp,
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

         _displayInstance.Status = _statusLogic.HandleStatusData(statusData);
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

      #region Other Helper Functions
      
      /// <summary>
      /// Restore the given UnitInfo into this Client Instance
      /// </summary>
      /// <param name="unitInfo">UnitInfo Object to Restore</param>
      public void RestoreUnitInfo(UnitInfo unitInfo)
      {
         _displayInstance.UnitInfo = unitInfo;
         _displayInstance.BuildUnitInfoLogic();
      }
      
      #endregion
   }
}
