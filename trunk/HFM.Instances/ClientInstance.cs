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
using HFM.Helpers;
using HFM.Instrumentation;

namespace HFM.Instances
{
   [CLSCompliant(false)]
   public interface IClientInstance : IDisplayInstance
   {
      /// <summary>
      /// Client Instance Settings
      /// </summary>
      IClientInstanceSettings Settings { get; }

      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      IUnitInfoLogic CurrentUnitInfo { get; }

      bool Owns(IOwnedByClientInstance value);
   }

   public class ClientInstance : IClientInstance
   {
      #region Fields

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
      
      private readonly ClientInstanceSettings _settings;
      /// <summary>
      /// Client Instance Settings
      /// </summary>
      public IClientInstanceSettings Settings
      {
         get { return _settings; }
      }

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
         _settings = instanceSettings ?? new ClientInstanceSettings(InstanceType.PathInstance);
         
         // Init Client Level Members
         Init();
         // Create a fresh UnitInfo
         _currentUnitInfo = new UnitInfoLogic(_prefs, _proteinCollection, _benchmarkContainer, new UnitInfo(), this);
      }
      #endregion

      #region Client Level Members

      /// <summary>
      /// Client Startup Arguments
      /// </summary>
      public string Arguments { get; set; }

      /// <summary>
      /// User ID associated with this client
      /// </summary>
      public string UserId { get; set; }
      
      /// <summary>
      /// True if User ID is Unknown
      /// </summary>
      public bool UserIdUnknown
      {
         get { return UserId.Length == 0; }
      }

      /// <summary>
      /// Machine ID associated with this client
      /// </summary>
      public int MachineId { get; set; }

      /// <summary>
      /// Combined User ID and Machine ID String
      /// </summary>
      public string UserAndMachineId
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", UserId, MachineId); }
      }

      /// <summary>
      /// The Folding ID (Username) attached to this client
      /// </summary>
      public string FoldingID { get; set; }

      /// <summary>
      /// The Team number attached to this client
      /// </summary>
      public int Team { get; set; }

      /// <summary>
      /// Combined Folding ID and Team String
      /// </summary>
      public string FoldingIDAndTeam
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", FoldingID, Team); }
      }

      private UnitInfoLogic _currentUnitInfo;
      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      private UnitInfoLogic CurrentUnitInfoConcrete
      {
         get { return _currentUnitInfo; }
         set
         {
            UpdateTimeOfLastProgress(value);
            _currentUnitInfo = value;
         }
      }
      
      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      public IUnitInfoLogic CurrentUnitInfo
      {
         get { return _currentUnitInfo; }
      }

      /// <summary>
      /// Init Client Level Members
      /// </summary>
      private void Init()
      {
         Arguments = String.Empty;
         UserId = Constants.DefaultUserID;
         MachineId = Constants.DefaultMachineID;
         FoldingID = Constants.FoldingIDDefault;
         Team = Constants.TeamDefault;
         TotalRunCompletedUnits = 0;
         TotalRunFailedUnits = 0;
         TotalClientCompletedUnits = 0;
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
         protected set 
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

            _dataRetriever.Settings = Settings;
            switch (Settings.InstanceHostType)
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
                     "Instance Type '{0}' is not implemented", Settings.InstanceHostType));
            }

            // Set successful Last Retrieval Time
            LastRetrievalTime = DateTime.Now;
            // Re-Init Client Level Members Before Processing
            Init();
            // Process the retrieved logs
            ClientStatus returnedStatus = ProcessExisting();

            // Handle the status retured from the log parse
            HandleReturnedStatus(returnedStatus);
         }
         catch (Exception ex)
         {
            Status = ClientStatus.Offline;
            HfmTrace.WriteToHfmConsole(Settings.InstanceName, ex);
         }
         finally
         {
            RetrievalInProgress = false;
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Client Status: {2}", HfmTrace.FunctionName, Settings.InstanceName, Status));
      }
      
      #endregion

      #region Queue and Log Processing Functions
      
      /// <summary>
      /// Process the cached log files that exist on this machine
      /// </summary>
      public ClientStatus ProcessExisting()
      {
         // Exec Start
         DateTime start = HfmTrace.ExecStart;

         #region Setup UnitInfo Aggregator
         _dataAggregator.InstanceName = Settings.InstanceName;
         _dataAggregator.QueueFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueName);
         _dataAggregator.FahLogFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogName);
         _dataAggregator.UnitInfoLogFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoName); 
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
         #endregion
         
         var parsedUnits = new UnitInfoLogic[units.Count];
         for (int i = 0; i < units.Count; i++)
         {
            if (units[i] != null)
            {
               parsedUnits[i] = new UnitInfoLogic(_prefs, _proteinCollection, _benchmarkContainer, units[i], this);
            }
         }

         // *** THIS HAS TO BE DONE BEFORE UPDATING THE CurrentUnitInfo ***
         // Update Benchmarks from parsedUnits array 
         UpdateBenchmarkData(parsedUnits, _dataAggregator.CurrentUnitIndex);

         // Update the CurrentUnitInfo if we have a Status
         ClientStatus currentWorkUnitStatus = _dataAggregator.CurrentWorkUnitStatus;
         if (currentWorkUnitStatus.Equals(ClientStatus.Unknown) == false)
         {
            CurrentUnitInfoConcrete = parsedUnits[_dataAggregator.CurrentUnitIndex];
         }
         
         CurrentUnitInfoConcrete.ShowPPDTrace();
         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, start);

         // Return the Status
         return currentWorkUnitStatus;
      }

      private void PopulateRunLevelData(IClientRun run)
      {
         ClientVersion = run.ClientVersion;
         Arguments = run.Arguments;
      
         FoldingID = run.FoldingID;
         Team = run.Team;
         
         UserId = run.UserID;
         MachineId = run.MachineID;

         TotalRunCompletedUnits = run.NumberOfCompletedUnits;
         TotalRunFailedUnits = run.NumberOfFailedUnits;
         TotalClientCompletedUnits = run.NumberOfTotalUnitsCompleted;
      }

      private void PopulateRunLevelData(IQueueEntry queueEntry)
      {
         if (FoldingID == Constants.FoldingIDDefault)
         {
            FoldingID = queueEntry.FoldingID;
         }
         if (Team == Constants.TeamDefault)
         {
            Team = (int)queueEntry.TeamNumber;
         }
         if (UserId == Constants.DefaultUserID)
         {
            UserId = queueEntry.UserID;
         }
         if (MachineId == Constants.DefaultMachineID)
         {
            MachineId = (int)queueEntry.MachineID;
         }
      }

      /// <summary>
      /// Update Project Benchmarks
      /// </summary>
      /// <param name="parsedUnits">Parsed UnitInfo Array</param>
      /// <param name="benchmarkUpdateIndex">Index of Current UnitInfo</param>
      private void UpdateBenchmarkData(UnitInfoLogic[] parsedUnits, int benchmarkUpdateIndex)
      {
         bool foundCurrent = false;
         bool processUpdates = false;
         int index = benchmarkUpdateIndex;
         
         #region Set index for the oldest unit in the array
         if (index == parsedUnits.Length - 1)
         {
            index = 0;
         }
         else
         {
            index++;
         }
         #endregion

         while (index != -1)
         {
            // If Current has not been found, check the benchmarkUpdateIndex
            // or try to match the Current Project and Raw Download Time
            if (processUpdates == false && (index == benchmarkUpdateIndex || IsUnitInfoCurrentUnitInfo(parsedUnits[index])))
            {
               foundCurrent = true;
               processUpdates = true;
            }

            if (processUpdates)
            {
               int previousFrameID = 0;
               if (foundCurrent)
               {
                  // current frame has already been recorded, increment to the next frame
                  previousFrameID = CurrentUnitInfo.LastUnitFrameID + 1;
                  foundCurrent = false;
               }

               // Even though the CurrentUnitInfo has been found in the parsed UnitInfoLogic array doesn't
               // mean that all entries in the array will be present.  See TestFiles\SMP_12\FAHlog.txt.
               if (parsedUnits[index] != null)
               {
                  // Update benchmarks
                  _benchmarkContainer.UpdateBenchmarkData(parsedUnits[index], previousFrameID, parsedUnits[index].LastUnitFrameID);

                  // Write Completed Unit Info only for units that are NOT current (i.e. have moved into history)
                  // For some WUs (typically bigadv) all frames could be complete but the FinishedTime read from
                  // the queue.dat is not yet populated.  To write this units production using an accurate bonus
                  // multiplier that FinishedTime needs to be populated.
                  if (index != benchmarkUpdateIndex)
                  {
                     // Make sure all Frames have been completed (not necessarily observed, but completed)
                     if (parsedUnits[index].AllFramesAreCompleted)
                     {
                        UnitInfoContainer.WriteCompletedUnitInfo(parsedUnits[index]);
                     }
                  }
               }
            }

            #region Increment to the next unit or set terminal value
            if (index == benchmarkUpdateIndex)
            {
               index = -1;
            }
            else if (index == parsedUnits.Length - 1)
            {
               index = 0;
            }
            else
            {
               index++;
            }
            #endregion
         }
      }

      /// <summary>
      /// Update Time of Last Frame Progress based on Current and Parsed UnitInfo
      /// </summary>
      private void UpdateTimeOfLastProgress(IUnitInfoLogic parsedUnitInfo)
      {
         // Matches the Current Project and Raw Download Time
         if (IsUnitInfoCurrentUnitInfo(parsedUnitInfo))
         {
            // If the Unit Start Time Stamp is no longer the same as the CurrentUnitInfo
            if (parsedUnitInfo.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                CurrentUnitInfo.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                parsedUnitInfo.UnitStartTimeStamp.Equals(CurrentUnitInfo.UnitStartTimeStamp) == false)
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

      /// <summary>
      /// Does the given UnitInfo.ProjectRunCloneGen match the CurrentUnitInfo.ProjectRunCloneGen?
      /// </summary>
      private bool IsUnitInfoCurrentUnitInfo(IUnitInfoLogic parsedUnitInfo)
      {
         Debug.Assert(CurrentUnitInfo != null);
      
         // if the parsed Project is known
         if (parsedUnitInfo != null && parsedUnitInfo.ProjectIsUnknown == false)
         {
            // Matches the Current Project and Raw Download Time
            // DownloadTime check should be made on the Raw DownloadTime
            // value from the internal UnitInfoData data source object
            if (ProjectsMatch(parsedUnitInfo, CurrentUnitInfo) &&
                parsedUnitInfo.UnitInfoData.DownloadTime.Equals(CurrentUnitInfo.UnitInfoData.DownloadTime))
            {
               return true;
            }
         }

         return false;
      }

      private static bool ProjectsMatch(IProjectInfo project1, IProjectInfo project2)
      {
         if (project1 == null || project2 == null) return false;

         return (project1.ProjectID == project2.ProjectID &&
                 project1.ProjectRun == project2.ProjectRun &&
                 project1.ProjectClone == project2.ProjectClone &&
                 project1.ProjectGen == project2.ProjectGen);
      }
      
      #endregion

      #region Status Handling and Determination
      
      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determines what values to feed the DetermineStatus routine.
      /// </summary>
      /// <param name="returnedStatus">Client Status</param>
      private void HandleReturnedStatus(ClientStatus returnedStatus)
      {
         var statusData = new StatusData();
         statusData.InstanceName = Settings.InstanceName;
         statusData.TypeOfClient = CurrentUnitInfo.TypeOfClient;
         statusData.LastRetrievalTime = LastRetrievalTime;
         statusData.IgnoreUtcOffset = Settings.ClientIsOnVirtualMachine;
         statusData.UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
         statusData.ClientTimeOffset = Settings.ClientTimeOffset;
         statusData.TimeOfLastUnitStart = TimeOfLastUnitStart;
         statusData.TimeOfLastFrameProgress = TimeOfLastFrameProgress;
         statusData.CurrentStatus = Status;
         statusData.ReturnedStatus = returnedStatus;
         statusData.FrameTime = CurrentUnitInfo.RawTimePerSection;
         statusData.AverageFrameTime = _benchmarkContainer.GetBenchmarkAverageFrameTime(CurrentUnitInfo);
         statusData.TimeOfLastFrame = CurrentUnitInfo.TimeOfLastFrame;
         statusData.UnitStartTimeStamp = CurrentUnitInfo.UnitStartTimeStamp;
         statusData.AllowRunningAsync = _prefs.GetPreference<bool>(Preference.AllowRunningAsync);

         // If the returned status is EuePause and current status is not
         if (statusData.ReturnedStatus.Equals(ClientStatus.EuePause) && statusData.CurrentStatus.Equals(ClientStatus.EuePause) == false)
         {
            if (_prefs.GetPreference<bool>(Preference.EmailReportingEnabled) &&
                _prefs.GetPreference<bool>(Preference.ReportEuePause))
            {
               SendEuePauseEmail(statusData.InstanceName, _prefs);
            }
         }
      
         Status = _statusLogic.HandleStatusData(statusData);
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

      #endregion

      #region Other Helper Functions
      
      /// <summary>
      /// Restore the given UnitInfo into this Client Instance
      /// </summary>
      /// <param name="unitInfo">UnitInfo Object to Restore</param>
      public void RestoreUnitInfo(IUnitInfo unitInfo)
      {
         CurrentUnitInfoConcrete = new UnitInfoLogic(_prefs, _proteinCollection, _benchmarkContainer, unitInfo, this, false);
      }
      
      public bool Owns(IOwnedByClientInstance value)
      {
         if (value.OwningInstanceName.Equals(Settings.InstanceName) &&
             StringOps.PathsEqual(value.OwningInstancePath, Settings.Path))
         {
            return true;
         }
         
         return false;
      }
      
      #endregion

      #region IDisplayInstance Members

      #region Grid Properties

      private ClientStatus _status;
      /// <summary>
      /// Status of this client
      /// </summary>
      public ClientStatus Status
      {
         get { return _status; }
         set
         {
            if (_status != value)
            {
               _status = value;
               //OnStatusChanged(EventArgs.Empty);
            }
         }
      }

      float IDisplayInstance.Progress
      {
         get { return ((float)PercentComplete) / 100; }
      }

      string IDisplayInstance.Name
      {
         get { return Settings.InstanceName; }
      }

      string IDisplayInstance.ClientType
      {
         get
         {
            if (_prefs.GetPreference<bool>(Preference.ShowVersions) && String.IsNullOrEmpty(ClientVersion) == false)
            {
               return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", CurrentUnitInfo.TypeOfClient, ClientVersion);
            }
            return CurrentUnitInfo.TypeOfClient.ToString();
         }
      }

      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan TPF
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.TimePerFrame;
            }

            return TimeSpan.Zero;
         }
      }

      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      public double PPD
      {
         get
         {
            if (ProductionValuesOk)
            {
               return Math.Round(CurrentUnitInfo.PPD, _prefs.GetPreference<int>(Preference.DecimalPlaces));
            }

            return 0;
         }
      }

      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      public double UPD
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.UPD;
            }

            return 0;
         }
      }

      int IDisplayInstance.MHz
      {
         get { return Settings.ClientProcessorMegahertz; }
      }

      double IDisplayInstance.PPD_MHz
      {
         get { return Math.Round(PPD / Settings.ClientProcessorMegahertz, 3); }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public TimeSpan ETA
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.ETA;
            }

            return TimeSpan.Zero;
         }
      }

      string IDisplayInstance.Core
      {
         get
         {
            if (_prefs.GetPreference<bool>(Preference.ShowVersions) && String.IsNullOrEmpty(CurrentUnitInfo.CoreVersion) == false)
            {
               return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", CurrentUnitInfo.Core, CurrentUnitInfo.CoreVersion);
            }
            return CurrentUnitInfo.Core;
         }
      }

      string IDisplayInstance.CoreId
      {
         get { return CurrentUnitInfo.CoreId; }
      }

      string IDisplayInstance.ProjectRunCloneGen
      {
         get { return CurrentUnitInfo.ProjectRunCloneGen; }
      }

      public double Credit
      {
         get
         {
            // Issue 125
            if (ProductionValuesOk && _prefs.GetPreference<bool>(Preference.CalculateBonus))
            {
               return CurrentUnitInfo.GetBonusCredit();
            }

            return CurrentUnitInfo.Credit;
         }
      }

      int IDisplayInstance.Complete
      {
         get
         {
            if (_prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay).Equals(CompletedCountDisplayType.ClientTotal))
            {
               return TotalClientCompletedUnits;
            }
            return TotalRunCompletedUnits;
         }
      }

      /// <summary>
      /// Number of failed units since the last client start
      /// </summary>
      public int TotalRunFailedUnits { get; set; }
      
      string IDisplayInstance.Username
      {
         get { return FoldingIDAndTeam; }
      }

      DateTime IDisplayInstance.DownloadTime
      {
         get { return CurrentUnitInfo.DownloadTime; }
      }

      DateTime IDisplayInstance.PreferredDeadline
      {
         get { return CurrentUnitInfo.PreferredDeadline; }
      }
      
      #endregion

      #region Complex Interfaces

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      IProtein IDisplayInstance.CurrentProtein
      {
         get { return CurrentUnitInfo.CurrentProtein; }
      }

      /// <summary>
      /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
      /// </summary>
      IList<ILogLine> IDisplayInstance.CurrentLogLines
      {
         get { return _dataAggregator.CurrentLogLines; }
      }

      /// <summary>
      /// Return LogLine List for Specified Queue Index
      /// </summary>
      /// <param name="queueIndex">Index in Queue</param>
      /// <exception cref="ArgumentOutOfRangeException">If queueIndex is outside the bounds of the Log Lines Array</exception>
      public IList<ILogLine> GetLogLinesForQueueIndex(int queueIndex)
      {
         if (_dataAggregator.UnitLogLines == null) return null;

         // Check the UnitLogLines array against the requested Queue Index - Issue 171
         if (queueIndex < 0 || queueIndex > _dataAggregator.UnitLogLines.Length - 1)
         {
            throw new ArgumentOutOfRangeException("QueueIndex", String.Format(CultureInfo.CurrentCulture,
               "Index is out of range.  Requested Index: {0}.  Array Length: {1}", queueIndex, _dataAggregator.UnitLogLines.Length));
         }

         if (_dataAggregator.UnitLogLines[queueIndex] != null)
         {
            return _dataAggregator.UnitLogLines[queueIndex];
         }

         return null;
      }

      /// <summary>
      /// Queue Base Interface
      /// </summary>
      IQueueBase IDisplayInstance.Queue
      {
         get { return _dataAggregator.Queue; }
      }
      
      #endregion

      private DateTime _lastRetrievalTime = DateTime.MinValue;
      /// <summary>
      /// When the log files were last successfully retrieved
      /// </summary>
      public DateTime LastRetrievalTime
      {
         get { return _lastRetrievalTime; }
         protected set
         {
            _lastRetrievalTime = value;
         }
      }

      /// <summary>
      /// Flag denoting if Progress, Production, and Time based values are OK to Display
      /// </summary>
      public bool ProductionValuesOk
      {
         get
         {
            if (Status.Equals(ClientStatus.Running) ||
                Status.Equals(ClientStatus.RunningAsync) ||
                Status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               return true;
            }

            return false;
         }
      }

      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      InstanceType IDisplayInstance.InstanceHostType
      {
         get { return Settings.InstanceHostType; }
      }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      string IDisplayInstance.Path
      {
         get { return Settings.Path; }
      }

      /// <summary>
      /// Client Path and Arguments (If Arguments Exist)
      /// </summary>
      public string ClientPathAndArguments
      {
         get
         {
            if (Arguments.Length == 0)
            {
               return Settings.Path;
            }

            return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Settings.Path, Arguments);
         }
      }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      bool IDisplayInstance.ClientIsOnVirtualMachine
      {
         get { return Settings.ClientIsOnVirtualMachine; }
      }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public int FramesComplete
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.FramesComplete;
            }

            return 0;
         }
      }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public int PercentComplete
      {
         get
         {
            if (ProductionValuesOk ||
                Status.Equals(ClientStatus.Paused))
            {
               return CurrentUnitInfo.PercentComplete;
            }

            return 0;
         }
      }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      ClientType IDisplayInstance.TypeOfClient
      {
         get { return CurrentUnitInfo.TypeOfClient; }
      }

      /// <summary>
      /// Client Version
      /// </summary>
      public string ClientVersion { get; set; }

      string IDisplayInstance.CoreName
      {
         get { return CurrentUnitInfo.Core; }
      }

      string IDisplayInstance.CoreVersion
      {
         get { return CurrentUnitInfo.CoreVersion; }
      }

      /// <summary>
      /// Project ID Number
      /// </summary>
      int IDisplayInstance.ProjectID
      {
         get { return CurrentUnitInfo.ProjectID; }
      }

      /// <summary>
      /// User ID is a Duplicate of another Client's User ID
      /// </summary>
      public bool UserIdIsDuplicate { get; set; }

      /// <summary>
      /// Project (R/C/G) is a Duplicate of another Client's Project (R/C/G)
      /// </summary>
      public bool ProjectIsDuplicate { get; set; }

      public bool UsernameOk
      {
         get
         {
            // if these are the default assigned values, don't check otherwise and just return true
            if (FoldingID == Constants.FoldingIDDefault && Team == Constants.TeamDefault)
            {
               return true;
            }

            if ((FoldingID != _prefs.GetPreference<string>(Preference.StanfordId) ||
                      Team != _prefs.GetPreference<int>(Preference.TeamId)) &&
                (Status.Equals(ClientStatus.Unknown) == false && Status.Equals(ClientStatus.Offline) == false))
            {
               return false;
            }

            return true;
         }
      }

      /// <summary>
      /// Number of completed units since the last client start
      /// </summary>
      public int TotalRunCompletedUnits { get; set; }

      /// <summary>
      /// Total Units Completed for lifetime of the client (read from log file)
      /// </summary>
      public int TotalClientCompletedUnits { get; set; }

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      string IDisplayInstance.CachedFahLogName
      {
         get { return Settings.CachedFahLogName; }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public DateTime EtaDate
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.EtaDate;
            }

            return DateTime.MinValue;
         }
      }

      /// <summary>
      /// Flag specifying if EtaDate is Unknown
      /// </summary>
      bool IDisplayInstance.EtaDateUnknown
      {
         get { return CurrentUnitInfo.EtaDateUnknown; }
      }

      /// <summary>
      /// Flag specifying if Download Time is Unknown
      /// </summary>
      bool IDisplayInstance.DownloadTimeUnknown
      {
         get { return CurrentUnitInfo.DownloadTimeUnknown; }
      }

      /// <summary>
      /// Flag specifying if Preferred Deadline is Unknown
      /// </summary>
      bool IDisplayInstance.PreferredDeadlineUnknown
      {
         get { return CurrentUnitInfo.PreferredDeadlineUnknown; }
      }

      #endregion
   }
}
