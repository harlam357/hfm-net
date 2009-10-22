/*
 * HFM.NET - Base Instance Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using harlam357.Security;
using harlam357.Security.Encryption;

using HFM.Helpers;
using HFM.Instrumentation;
using HFM.Proteins;
using HFM.Preferences;

namespace HFM.Instances
{
   #region Enum
   //public enum ClientStatus
   //{
   //   Unknown,
   //   Offline,
   //   Stopped,
   //   EuePause,
   //   Hung,
   //   Paused,
   //   RunningNoFrameTimes,
   //   Running
   //}

   public enum InstanceType
   {
      PathInstance,
      FTPInstance,
      HTTPInstance
   }

   public enum ClientType
   {
      Unknown,
      Standard,
      SMP,
      GPU
   } 
   #endregion

   public class ClientInstance
   {
      #region Constants
      // Xml Serialization Constants
      private const string xmlNodeInstance = "Instance";
      private const string xmlAttrName = "Name";
      private const string xmlNodeFAHLog = "FAHLogFile";
      private const string xmlNodeUnitInfo = "UnitInfoFile";
      private const string xmlNodeQueue = "QueueFile";
      private const string xmlNodeClientMHz = "ClientMHz";
      private const string xmlNodeClientVM = "ClientVM";
      private const string xmlNodeClientOffset = "ClientOffset";
      private const string xmlPropType = "HostType";
      private const string xmlPropPath = "Path";
      private const string xmlPropServ = "Server";
      private const string xmlPropUser = "Username";
      private const string xmlPropPass = "Password";

      // Log Filename Constants
      public const string LocalFAHLog = "FAHlog.txt";
      public const string LocalUnitInfo = "unitinfo.txt";
      public const string LocalQueue = "queue.dat";
      
      public const string DefaultUserID = "";
      public const int DefaultMachineID = 0;

      private readonly Data IV = new Data("zX!1=D,^7K@u33+d");
      private readonly Data SymmetricKey = new Data("cNx/7+,?%ubm*?j8");
      #endregion

      private readonly QueueReader _qr = new QueueReader();
      
      [CLSCompliant(false)]
      public QueueReader ClientQueue
      {
         get { return _qr; }
      }
      
      #region Public Events
      /// <summary>
      /// Raised when Instance Host Type is Changed
      /// </summary>
      public event EventHandler InstanceHostTypeChanged;

      /// <summary>
      /// Raised when Client is on VM flag is Changed
      /// </summary>
      public event EventHandler ClientIsOnVirtualMachineChanged;
      #endregion

      #region Public Readonly Properties
      

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      public string CachedFAHLogName
      {
         get { return String.Format("{0}-{1}", InstanceName, LocalFAHLog); }
      }

      /// <summary>
      /// Cached UnitInfo Filename for this instance
      /// </summary>
      public string CachedUnitInfoName
      {
         get { return String.Format("{0}-{1}", InstanceName, LocalUnitInfo); }
      } 
      
      /// <summary>
      /// Cached Queue Filename for this instance
      /// </summary>
      public string CachedQueueName
      {
         get { return String.Format("{0}-{1}", InstanceName, LocalQueue); }
      } 
      
      /// <summary>
      /// Combined UserID and MachineID string
      /// </summary>
      public string UserAndMachineID
      {
         get { return String.Format("{0} ({1})", UserID, MachineID); }
      }
      
      /// <summary>
      /// Returns true if UserID is unknown
      /// </summary>
      public bool UserIDUnknown
      {
         get { return UserID.Length == 0; }
      }
      
      /// <summary>
      /// Client Path and Arguments (if arguments exist)
      /// </summary>
      public string ClientPathAndArguments
      {
         get 
         {
            if (Arguments.Length == 0)
            {
               return Path;   
            }

            return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", Path, Arguments);
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
                Status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               return true;
            }
            
            return false;
         }
      }
      
      #region CurrentUnitInfo Values
      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public Int32 FramesComplete
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
      public Int32 PercentComplete
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.PercentComplete;
            }

            return 0;
         }
      }

      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan TimePerFrame
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
      /// Units per day (UPD) rating for this instance
      /// </summary>
      public Double UPD
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

      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      public Double PPD
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.PPD;
            }

            return 0;
         }
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
      #endregion

      #endregion

      #region Public Properties and Related Private Members

      #region Retrieval Properties
      /// <summary>
      /// Local flag set when retrieval acts on the status returned from parse
      /// </summary>
      private bool _HandleStatusOnRetrieve = true;
      /// <summary>
      /// Local flag set when retrieval acts on the status returned from parse
      /// </summary>
      public bool HandleStatusOnRetrieve
      {
         get { return _HandleStatusOnRetrieve; }
         set { _HandleStatusOnRetrieve = value; }
      } 
      
      /// <summary>
      /// Local flag set when log retrieval is in progress
      /// </summary>
      private volatile bool _RetrievalInProgress = false;
      /// <summary>
      /// Local flag set when log retrieval is in progress
      /// </summary>
      public bool RetrievalInProgress
      {
         get { return _RetrievalInProgress; }
      }

      /// <summary>
      /// When the log files were last successfully retrieved
      /// </summary>
      private DateTime _LastRetrievalTime = DateTime.MinValue;
      /// <summary>
      /// When the log files were last successfully retrieved
      /// </summary>
      public DateTime LastRetrievalTime
      {
         get { return _LastRetrievalTime; }
      }
      #endregion
      
      #region User Specified Values (from the frmHost dialog)
      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      private string _InstanceName;
      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      public string InstanceName
      {
         get { return _InstanceName; }
         set { _InstanceName = value; }
      }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      private Int32 _ClientProcessorMegahertz = 1;
      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      public Int32 ClientProcessorMegahertz
      {
         get { return _ClientProcessorMegahertz; }
         set { _ClientProcessorMegahertz = value; }
      }

      /// <summary>
      /// Remote client log file name
      /// </summary>
      private string _RemoteFAHLogFilename = LocalFAHLog;
      /// <summary>
      /// Remote client log file name
      /// </summary>
      public string RemoteFAHLogFilename
      {
         get { return _RemoteFAHLogFilename; }
         set
         {
            if (String.IsNullOrEmpty(value))
            {
               _RemoteFAHLogFilename = LocalFAHLog;
            }
            else
            {
               _RemoteFAHLogFilename = value;
            }

         }
      }

      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      private string _RemoteUnitInfoFilename = LocalUnitInfo;
      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      public string RemoteUnitInfoFilename
      {
         get { return _RemoteUnitInfoFilename; }
         set
         {
            if (String.IsNullOrEmpty(value))
            {
               _RemoteUnitInfoFilename = LocalUnitInfo;
            }
            else
            {
               _RemoteUnitInfoFilename = value;
            }
         }
      }

      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      private string _RemoteQueueFilename = LocalQueue;
      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      public string RemoteQueueFilename
      {
         get { return _RemoteQueueFilename; }
         set
         {
            if (String.IsNullOrEmpty(value))
            {
               _RemoteQueueFilename = LocalQueue;
            }
            else
            {
               _RemoteQueueFilename = value;
            }
         }
      }

      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      private InstanceType _InstanceHostType;
      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      public InstanceType InstanceHostType
      {
         get { return _InstanceHostType; }
         set
         {
            if (_InstanceHostType != value)
            {
               _InstanceHostType = value;
               OnInstanceHostTypeChanged(EventArgs.Empty);
            }
         }
      }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      private string _Path;
      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      public string Path
      {
         get { return _Path; }
         set { _Path = value; }
      }

      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      private string _Server;
      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      public string Server
      {
         get { return _Server; }
         set { _Server = value; }
      }

      /// <summary>
      /// Username on remote server
      /// </summary>
      private string _Username;
      /// <summary>
      /// Username on remote server
      /// </summary>
      public string Username
      {
         get { return _Username; }
         set { _Username = value; }
      }

      /// <summary>
      /// Password on remote server
      /// </summary>
      private string _Password;
      /// <summary>
      /// Password on remote server
      /// </summary>
      public string Password
      {
         get { return _Password; }
         set { _Password = value; }
      }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      private bool _ClientIsOnVirtualMachine;
      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      public bool ClientIsOnVirtualMachine
      {
         get { return _ClientIsOnVirtualMachine; }
         set 
         {
            if (_ClientIsOnVirtualMachine != value)
            {
               _ClientIsOnVirtualMachine = value;
               OnClientIsOnVirtualMachineChanged(EventArgs.Empty);
            }
         }
      }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      private Int32 _ClientTimeOffset;
      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      public Int32 ClientTimeOffset
      {
         get { return _ClientTimeOffset; }
         set { _ClientTimeOffset = value; }
      } 
      #endregion

      #region Values captured during log file parse
      /// <summary>
      /// Status of this client
      /// </summary>
      private ClientStatus _Status;
      /// <summary>
      /// Status of this client
      /// </summary>
      public ClientStatus Status
      {
         get { return _Status; }
         set 
         { 
            if (_Status != value)
            {
               _Status = value;
               //OnStatusChanged(EventArgs.Empty);
            }
         }
      }

      /// <summary>
      /// Client Startup Arguments
      /// </summary>
      private string _Arguments = String.Empty;
      /// <summary>
      /// Client Startup Arguments
      /// </summary>
      public string Arguments
      {
         get { return _Arguments; }
         set { _Arguments = value; }
      }

      /// <summary>
      /// User ID associated with this client
      /// </summary>
      private string _UserID;
      /// <summary>
      /// User ID associated with this client
      /// </summary>
      public string UserID
      {
         get { return _UserID; }
         set { _UserID = value; }
      }

      /// <summary>
      /// Machine ID associated with this client
      /// </summary>
      private int _MachineID;
      /// <summary>
      /// Machine ID associated with this client
      /// </summary>
      public int MachineID
      {
         get { return _MachineID; }
         set { _MachineID = value; }
      }
      
      /// <summary>
      /// Total Units Completed for lifetime of the client (read from log file)
      /// </summary>
      private Int32 _TotalUnits;
      /// <summary>
      /// Total Units Completed for lifetime of the client (read from log file)
      /// </summary>
      public Int32 TotalUnits
      {
         get { return _TotalUnits; }
         set { _TotalUnits = value; }
      }

      /// <summary>
      /// The Folding ID (Username) attached to this client
      /// </summary>
      private string _FoldingID;
      /// <summary>
      /// The Folding ID (Username) attached to this client
      /// </summary>
      public string FoldingID
      {
         get { return _FoldingID; }
         set { _FoldingID = value; }
      }

      /// <summary>
      /// The Team number attached to this client
      /// </summary>
      private Int32 _Team;
      /// <summary>
      /// The Team number attached to this client
      /// </summary>
      public Int32 Team
      {
         get { return _Team; }
         set { _Team = value; }
      }

      /// <summary>
      /// Number of completed units since the last client start
      /// </summary>
      private Int32 _NumberOfCompletedUnitsSinceLastStart;
      /// <summary>
      /// Number of completed units since the last client start
      /// </summary>
      public Int32 NumberOfCompletedUnitsSinceLastStart
      {
         get { return _NumberOfCompletedUnitsSinceLastStart; }
         set { _NumberOfCompletedUnitsSinceLastStart = value; }
      }

      /// <summary>
      /// Number of failed units since the last client start
      /// </summary>
      private Int32 _NumberOfFailedUnitsSinceLastStart;
      /// <summary>
      /// Number of failed units since the last client start
      /// </summary>
      public Int32 NumberOfFailedUnitsSinceLastStart
      {
         get { return _NumberOfFailedUnitsSinceLastStart; }
         set { _NumberOfFailedUnitsSinceLastStart = value; }
      }

      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      private UnitInfo _CurrentUnitInfo;
      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      public UnitInfo CurrentUnitInfo
      {
         get { return _CurrentUnitInfo; }
         //set 
         //{ 
         //   _CurrentUnitInfo = value;
         //}
      } 
      
      /// <summary>
      /// Array of LogLine Lists - Used to hold QueueEntry LogLines
      /// </summary>
      private IList<LogLine>[] _QueueLogLines;

      /// <summary>
      /// List of current log file lines
      /// </summary>
      private IList<LogLine> _CurrentLogLines = new List<LogLine>();
      /// <summary>
      /// List of current log file text lines
      /// </summary>
      public IList<LogLine> CurrentLogLines
      {
         get { return _CurrentLogLines; }
         //set { _CurrentLogLines = value; }
      }
      #endregion

      #endregion

      #region Constructor
      /// <summary>
      /// Primary Constructor
      /// </summary>
      public ClientInstance(InstanceType type)
      {
         // When Instance Host Type Changes, Clear the User Specified Values
         InstanceHostTypeChanged += ClientInstance_InstanceHostTypeChanged;
         // When Client is on VM Changes, Clear the Unit Frame Data
         // The captured TimeOfFrame values will no longer be valid
         ClientIsOnVirtualMachineChanged += ClientInstance_ClientIsOnVirtualMachineChanged;
         
         // Set the Host Type
         _InstanceHostType = type;
         // Clear (Init) Instance Values
         Clear();
         // Clear (Init) User Specified Instance Values
         ClearUserSpecifiedValues();
         // Create a fresh UnitInfo
         _CurrentUnitInfo = new UnitInfo(InstanceName, Path, DateTime.Now);
      }
      #endregion

      #region Protected Event Wrappers
      /// <summary>
      /// Call when changing Host Type
      /// </summary>
      protected void OnInstanceHostTypeChanged(EventArgs e)
      {
         if (InstanceHostTypeChanged != null)
         {
            InstanceHostTypeChanged(this, e);
         }
      }

      /// <summary>
      /// Call when changing Client is on VM
      /// </summary>
      protected void OnClientIsOnVirtualMachineChanged(EventArgs e)
      {
         if (ClientIsOnVirtualMachineChanged != null)
         {
            ClientIsOnVirtualMachineChanged(this, e);
         }
      } 
      #endregion

      #region Data Processing
      /// <summary>
      /// Clear Client Instance and UnitInfo Values
      /// </summary>
      private void Clear()
      {
         // reset client level values
         _CurrentLogLines = new List<LogLine>();
         
         FoldingID = UnitInfo.UsernameDefault;
         Team = UnitInfo.TeamDefault;
         UserID = DefaultUserID;
         MachineID = DefaultMachineID;

         TotalUnits = 0;
         NumberOfCompletedUnitsSinceLastStart = 0;
         NumberOfFailedUnitsSinceLastStart = 0;
      }
      
      public void RestoreUnitInfo(UnitInfo unit)
      {
         _CurrentUnitInfo = unit;
      }

      public IList<LogLine> GetLogLinesForQueueIndex(int QueueIndex)
      {
         if (_QueueLogLines != null && _QueueLogLines[QueueIndex] != null)
         {
            return _QueueLogLines[QueueIndex];
         }

         return null;
      }

      private void ClientInstance_ClientIsOnVirtualMachineChanged(object sender, EventArgs e)
      {
         ClearUnitFrameData();
      }

      /// <summary>
      /// Clear the Unit Frame Data from the Current Unit Info
      /// </summary>
      private void ClearUnitFrameData()
      {
         CurrentUnitInfo.ClearUnitFrameData();
      }
      
      /// <summary>
      /// 
      /// </summary>
      private bool IsUnitInfoCurrentUnitInfo(UnitInfo parsedUnitInfo)
      {
         // if the parsed Project is known
         if (parsedUnitInfo.ProjectIsUnknown == false)
         {
            // and is the same as the current Project
            if (parsedUnitInfo.ProjectRunCloneGen == CurrentUnitInfo.ProjectRunCloneGen)
            {
               return true;
            }
         }

         return false;
      }
      
      /// <summary>
      /// Handles the InstanceHostTypeChanged Event
      /// </summary>
      private void ClientInstance_InstanceHostTypeChanged(object sender, EventArgs e)
      {
         ClearUserSpecifiedValues();
      }

      /// <summary>
      /// Clear the user specified values that define this instance
      /// </summary>
      private void ClearUserSpecifiedValues()
      {
         InstanceName = String.Empty;
         ClientProcessorMegahertz = 1;
         RemoteFAHLogFilename = LocalFAHLog;
         RemoteUnitInfoFilename = LocalUnitInfo;
         RemoteQueueFilename = LocalQueue;
         ClientIsOnVirtualMachine = false;
         ClientTimeOffset = 0;

         Path = String.Empty;
         Server = String.Empty;
         Username = String.Empty;
         Password = String.Empty;
      }

      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determines what values to feed the DetermineStatus routine.
      /// </summary>
      /// <param name="returnedStatus">Client Status</param>
      private void HandleReturnedStatus(ClientStatus returnedStatus)
      {
         // If the returned status is EuePause and current status is not
         if (returnedStatus.Equals(ClientStatus.EuePause) && Status.Equals(ClientStatus.EuePause) == false)
         {
            SendEuePauseEmail();
         }

         switch (returnedStatus)
         {
            case ClientStatus.Running:
            case ClientStatus.RunningNoFrameTimes:
               break;
            case ClientStatus.Unknown:
            case ClientStatus.Offline:
            case ClientStatus.Stopped:
            case ClientStatus.EuePause:
            case ClientStatus.Hung:
            case ClientStatus.Paused:
            case ClientStatus.GettingWorkPacket:
               // Update Client Status - don't call Determine Status
               Status = returnedStatus;
               return;
         }
      
         // if we have a frame time, use it
         if (CurrentUnitInfo.RawTimePerSection > 0)
         {
            Status = DetermineStatus(this, CurrentUnitInfo.TimeOfLastFrame, CurrentUnitInfo.RawTimePerSection);
         }

         // no frame time based on the current PPD calculation selection ('LastFrame', 'LastThreeFrames', etc)
         // this section attempts to give DetermineStats values to detect Hung clients before they have a valid
         // frame time - Issue 10
         else 
         {
            // if we have a frame time stamp, use it
            TimeSpan frameTime = CurrentUnitInfo.TimeOfLastFrame;
            if (frameTime == TimeSpan.Zero)
            {
               // otherwise, use the unit start time
               frameTime = CurrentUnitInfo.UnitStartTime;
            }

            // get the average frame time for this client and project id
            TimeSpan averageFrameTime = ProteinBenchmarkCollection.Instance.GetBenchmarkAverageFrameTime(CurrentUnitInfo);
            if (averageFrameTime > TimeSpan.Zero)
            {
               if (DetermineStatus(this, frameTime, Convert.ToInt32(averageFrameTime.TotalSeconds)).Equals(ClientStatus.Hung))
               {
                  Status = ClientStatus.Hung;
               }
               else
               {
                  Status = returnedStatus;
               }
            }

            // no benchmarked average frame time, use some arbitrary (and large) values for the frame time
            // we want to give the client plenty of time to show progress but don't want it to sit idle for days
            else 
            {
               // CPU: use 1 hour (3600 seconds) as a base frame time
               int SectionTime = 3600;
               if (CurrentUnitInfo.TypeOfClient.Equals(ClientType.GPU))
               {
                  // GPU: use 10 minutes (600 seconds) as a base frame time
                  SectionTime = 600;
               }

               if (DetermineStatus(this, frameTime, SectionTime).Equals(ClientStatus.Hung))
               {
                  Status = ClientStatus.Hung;
               }
               else
               {
                  Status = returnedStatus;
               }
            }
         }
      }

      private void SendEuePauseEmail()
      {
         PreferenceSet Prefs = PreferenceSet.Instance;

         if (Prefs.EmailReportingEnabled && Prefs.ReportEuePause) 
         {
            string messageBody = String.Format("HFM.NET detected that Client '{0}' has entered a 24 hour EUE Pause state.", InstanceName);
            try
            {
               NetworkOps.SendEmail(Prefs.EmailReportingFromAddress, Prefs.EmailReportingToAddress,
                                    "HFM.NET - Client EUE Pause Error", messageBody, Prefs.EmailReportingServerAddress, 
                                    Prefs.EmailReportingServerUsername, Prefs.EmailReportingServerPassword);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
         }
   }
      
      /// <summary>
      /// Determine Client Status
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="TimeOfLastFrame">Time Stamp from Last Recorded Frame</param>
      /// <param name="SectionTime">The Current Frame Time (in seconds)</param>
      private static ClientStatus DetermineStatus(ClientInstance Instance, TimeSpan TimeOfLastFrame, int SectionTime)
      {
         #region Get Terminal Time
         // Terminal Time - defined as last retrieval time minus twice (7 times for GPU) the current Raw Time per Section.
         // if a new frame has not completed in twice the amount of time it should take to complete we should deem this client Hung.
         DateTime terminalDateTime;

         if (Instance.CurrentUnitInfo.TypeOfClient.Equals(ClientType.GPU))
         {
            terminalDateTime = Instance.LastRetrievalTime.Subtract(TimeSpan.FromSeconds(SectionTime * 7));
         }
         else
         {
            terminalDateTime = Instance.LastRetrievalTime.Subtract(TimeSpan.FromSeconds(SectionTime * 2));
         }
         #endregion

         #region Get Last Retrieval Time Date
         DateTime currentFrameDateTime;

         if (Instance.ClientIsOnVirtualMachine)
         {
            // get only the date from the last retrieval time (in universal), we'll add the current time below
            currentFrameDateTime = new DateTime(Instance.LastRetrievalTime.Date.Ticks, DateTimeKind.Utc);
         }
         else
         {
            // get only the date from the last retrieval time, we'll add the current time below
            currentFrameDateTime = Instance.LastRetrievalTime.Date;
         }
         #endregion

         #region Apply Frame Time Offset and Set Current Frame Time Date
         TimeSpan offset = TimeSpan.FromMinutes(Instance.ClientTimeOffset);
         TimeSpan adjustedFrameTime = TimeOfLastFrame.Subtract(offset);

         // client time has already rolled over to the next day. the offset correction has 
         // caused the adjusted frame time span to be negetive.  take the that negetive span
         // and add it to a full 24 hours to correct.
         if (adjustedFrameTime < TimeSpan.Zero)
         {
            adjustedFrameTime = TimeSpan.FromDays(1).Add(adjustedFrameTime);
         }

         // the offset correction has caused the frame time span to be greater than 24 hours.
         // subtract the extra day from the adjusted frame time span.
         else if (adjustedFrameTime > TimeSpan.FromDays(1))
         {
            adjustedFrameTime = adjustedFrameTime.Subtract(TimeSpan.FromDays(1));
         }

         // add adjusted Time of Last Frame (TimeSpan) to the DateTime with the correct date
         currentFrameDateTime = currentFrameDateTime.Add(adjustedFrameTime);
         #endregion

         #region Check For Frame from Prior Day (Midnight Rollover on Local Machine)
         bool priorDayAdjust = false;

         // if the current (and adjusted) frame time hours is greater than the last retrieval time hours, 
         // and the time difference is greater than an hour, then frame is from the day prior.
         // this should only happen after midnight time on the machine running HFM when the monitored client has 
         // not completed a frame since the local machine time rolled over to the next day, otherwise the time
         // stamps between HFM and the client are too far off, a positive offset should be set to correct.
         if (currentFrameDateTime.TimeOfDay.Hours > Instance.LastRetrievalTime.TimeOfDay.Hours &&
             currentFrameDateTime.TimeOfDay.Subtract(Instance.LastRetrievalTime.TimeOfDay).Hours > 0)
         {
            priorDayAdjust = true;

            // subtract 1 day from today's date
            currentFrameDateTime = currentFrameDateTime.Subtract(TimeSpan.FromDays(1));
         }
         #endregion

         #region Write Verbose Trace
         if (TraceLevelSwitch.Switch.TraceVerbose)
         {
            List<string> messages = new List<string>(10);

            messages.Add(String.Format("{0} ({1})", HfmTrace.FunctionName, Instance.InstanceName));
            messages.Add(String.Format(" - Retrieval Time (Date) ------- : {0}", Instance.LastRetrievalTime));
            messages.Add(String.Format(" - Time Of Last Frame (TimeSpan) : {0}", TimeOfLastFrame));
            messages.Add(String.Format(" - Offset (Minutes) ------------ : {0}", Instance.ClientTimeOffset));
            messages.Add(String.Format(" - Time Of Last Frame (Adjusted) : {0}", adjustedFrameTime));
            messages.Add(String.Format(" - Prior Day Adjustment -------- : {0}", priorDayAdjust));
            messages.Add(String.Format(" - Time Of Last Frame (Date) --- : {0}", currentFrameDateTime));
            messages.Add(String.Format(" - Terminal Time (Date) -------- : {0}", terminalDateTime));

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, messages);
         }
         #endregion

         if (currentFrameDateTime > terminalDateTime)
         {
            return ClientStatus.Running;
         }
         else // current frame is less than terminal time
         {
            return ClientStatus.Hung;
         }
      }

      /// <summary>
      /// Process the cached log files that exist on this machine
      /// </summary>
      public ClientStatus ProcessExisting()
      {
         DateTime Start = HfmTrace.ExecStart;
         
         UnitInfo[] parsedUnits = ParseQueueFile();
         ClientStatus returnedStatus = ClientStatus.Unknown;

         LogReader lr = new LogReader();
         lr.ScanFAHLog(this, System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedFAHLogName));
         
         lr.PopulateClientStartupArgumentData(this);
         lr.PopulateWorkUnitCountData(this);

         if (parsedUnits == null)
         {
            parsedUnits = new UnitInfo[2];
            returnedStatus = ParseCurrentAndPreviousUnitsFromLogsOnly(parsedUnits, lr);
         }
         else
         {
            #region Parse Log Data Into UnitInfo Created From Queue Data
            _qr.PopulateUserAndMachineData(this);

            int outputArrayIndex = 0;
            _QueueLogLines = new IList<LogLine>[10];
            UnitInfo[] units = new UnitInfo[10];

            int queueIndex = (int)_qr.CurrentIndex;
            // Set index for the oldest unit in the queue
            if (queueIndex == 9)
            {
               queueIndex = 0;
            }
            else
            {
               queueIndex++;
            }

            while (queueIndex != -1)
            {
               // Get the Log Lines for this queue position from the reader
               IList<LogLine> logLines = lr.GetLogLinesFromQueueIndex(queueIndex);
               _QueueLogLines[queueIndex] = logLines;

               // Make sure we have lines and that the Project (R/C/G) from the Queue matches what is found in the log lines
               if (_qr.CurrentIndex == queueIndex)
               {
                  if (ValidateQueueEntryMatchesLogLines(logLines, parsedUnits[queueIndex]))
                  {
                     returnedStatus = ProcessCurrentWorkUnitLogLines(logLines, parsedUnits[queueIndex]);
                  }
                  else
                  {
                     HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, String.Format("Could not find or verify log section for queue entry {0} (current - parsing logs without queue).", queueIndex));

                     parsedUnits = new UnitInfo[2];
                     returnedStatus = ParseCurrentAndPreviousUnitsFromLogsOnly(parsedUnits, lr);

                     units = null;
                     _qr.ClearQueue();
                     break;
                  }
               }
               else
               {
                  if (ValidateQueueEntryMatchesLogLines(logLines, parsedUnits[queueIndex]))
                  {
                     ProcessPreviousWorkUnitLogLines(logLines, parsedUnits[queueIndex]);
                  }
                  else
                  {
                     HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, InstanceName, String.Format("Could not find or verify log section for queue entry {0} (this is not a problem).", queueIndex));
                  }
               }

               units[outputArrayIndex] = parsedUnits[queueIndex];
               outputArrayIndex++;

               if (queueIndex == (int)_qr.CurrentIndex)
               {
                  queueIndex = -1;
               }
               else if (queueIndex == 9)
               {
                  queueIndex = 0;
               }
               else
               {
                  queueIndex++;
               }
            }

            if (units != null)
            {
               // update parsed units with reordered array
               parsedUnits = units; 
            }
            #endregion
         }

         // *** THIS HAS TO BE DONE BEFORE UPDATING THE _CurrentUnitInfo ***
         // Update Benchmarks from units array 
         UpdateBenchmarkData(parsedUnits);

         if (returnedStatus.Equals(ClientStatus.Unknown) == false)
         {
            _CurrentUnitInfo = parsedUnits[parsedUnits.Length - 1];
         }

         _CurrentLogLines = lr.CurrentWorkUnitLogLines;
         
         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, InstanceName, Start);
         
         return returnedStatus;
      }

      private UnitInfo[] ParseQueueFile()
      {
         UnitInfo[] units = null;
         
         // queue.dat is not required to get a reading, if something goes wrong
         // just catch, log, and continue with parsing log files
         try
         {
            _qr.ReadQueue(System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedQueueName));
            if (_qr.QueueReadOk)
            {
               units = new UnitInfo[10];
            
               // process entries
               for (int i = 0; i < 10; i++)
               {
                  UnitInfo parsedUnitInfo = new UnitInfo(InstanceName, Path, LastRetrievalTime);
                  QueueParser.ParseQueueEntry(_qr.GetQueueEntry((uint)i), parsedUnitInfo, ClientIsOnVirtualMachine);
                  units[i] = parsedUnitInfo;
               }
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, String.Format("{0} read failed.", _qr.QueueFilePath));
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Error, InstanceName, ex);
         }
         
         return units;
      }

      private ClientStatus ParseCurrentAndPreviousUnitsFromLogsOnly(UnitInfo[] parsedUnits, LogReader lr)
      {
         // Make sure we've got an array of 2
         Debug.Assert(parsedUnits.Length == 2);

         _QueueLogLines = null;

         lr.PopulateUserAndMachineData(this);

         parsedUnits[0] = new UnitInfo(InstanceName, Path, LastRetrievalTime, FoldingID, Team);
         parsedUnits[1] = new UnitInfo(InstanceName, Path, LastRetrievalTime, FoldingID, Team);

         ProcessPreviousWorkUnitLogLines(lr.PreviousWorkUnitLogLines, parsedUnits[0]);
         return ProcessCurrentWorkUnitLogLines(lr.CurrentWorkUnitLogLines, parsedUnits[1]);
      }
      
      private static bool ValidateQueueEntryMatchesLogLines(ICollection<LogLine> logLines, UnitInfo parsedUnitInfo)
      {
         if (logLines != null &&
             parsedUnitInfo.ProjectIsUnknown == false &&
             parsedUnitInfo.ProjectRunCloneGen.Equals(LogReader.GetProjectFromLogLines(logLines)))
         {
            return true;
         }
         
         return false;
      }

      private void ProcessPreviousWorkUnitLogLines(IList<LogLine> logLines, UnitInfo parsedUnitInfo)
      {
         // Make sure we have a previous log lines to parse. 
         // We don't want to write trace errors if there are no lines.
         if (logLines != null)
         {
            ProcessWorkUnitLogLines(logLines, parsedUnitInfo, false);
         }
      }

      private ClientStatus ProcessCurrentWorkUnitLogLines(IList<LogLine> logLines, UnitInfo parsedUnitInfo)
      {
         // We have to have a FAHlog to parse and current log lines by this point
         Debug.Assert(logLines != null);
      
         return ProcessWorkUnitLogLines(logLines, parsedUnitInfo, true);
      }

      private ClientStatus ProcessWorkUnitLogLines(IList<LogLine> logLines, UnitInfo parsedUnitInfo, bool ReadUnitInfoFile)
      {
         // There should always be a list of current work unit log lines
         ClientStatus returnedStatus = ParseWorkUnitLogLines(logLines, parsedUnitInfo, ReadUnitInfoFile);

         if (returnedStatus.Equals(ClientStatus.Unknown))
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("Unable to Parse Work Unit for Client '{0}'", InstanceName), true);
         }
         return returnedStatus;
      }

      private ClientStatus ParseWorkUnitLogLines(IList<LogLine> LogLines, UnitInfo parsedUnitInfo, bool ReadUnitInfoFile)
      {
         LogParser lp = new LogParser(this, parsedUnitInfo);
         if (ReadUnitInfoFile)
         {
            if (lp.ParseUnitInfoFile(System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedUnitInfoName)) == false)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, "unitinfo parse failed.");
            }
         }

         return lp.ParseFAHLog(LogLines);
      }

      /// <summary>
      /// Update Project Benchmarks
      /// </summary>
      /// <param name="parsedUnits">Array of parsed UnitInfo objects.  Must be in order from oldest (0) to newest (9) work unit.</param>
      private void UpdateBenchmarkData(UnitInfo[] parsedUnits)
      {
         bool FoundCurrent = false;
         
         for (int i = 0; i < parsedUnits.Length; i++)
         {
            if (FoundCurrent == false && IsUnitInfoCurrentUnitInfo(parsedUnits[i]))
            {
               FoundCurrent = true;
            }
            
            if (FoundCurrent || i == (parsedUnits.Length - 1))
            {
               int previousFramePercent = 0;
               // check this against the CurrentUnitInfo
               if (IsUnitInfoCurrentUnitInfo(parsedUnits[i]))
               {
                  // current frame has already been recorded, increment to the next frame
                  previousFramePercent = CurrentUnitInfo.LastUnitFramePercent + 1;
               }

               // Update benchmarks
               ProteinBenchmarkCollection.Instance.UpdateBenchmarkData(parsedUnits[i], previousFramePercent,
                                                                       parsedUnits[i].LastUnitFramePercent);
            }
         }
      }

      /// <summary>
      /// Attempts to Set Project ID with given Match.  If Project cannot be found in local cache, download again.
      /// </summary>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="match">Regex Match containing Project data</param>
      internal static void DoProjectIDMatch(UnitInfo parsedUnitInfo, Match match)
      {
         List<int> ProjectID = new List<int>(4);
      
         ProjectID.Add(Int32.Parse(match.Result("${ProjectNumber}")));
         ProjectID.Add(Int32.Parse(match.Result("${Run}")));
         ProjectID.Add(Int32.Parse(match.Result("${Clone}")));
         ProjectID.Add(Int32.Parse(match.Result("${Gen}")));
         
         DoProjectIDMatch(parsedUnitInfo, ProjectID);
      }

      internal static void DoProjectIDMatch(UnitInfo parsedUnitInfo, IList<int> ProjectID)
      {
         if (ProteinCollection.Instance.ContainsKey(ProjectID[0]))
         {
            SetProjectAndClientType(parsedUnitInfo, ProjectID);
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} Project ID '{1}' not found in Protein Collection.",
                                                     HfmTrace.FunctionName, ProjectID[0]));

            // If a Project cannot be identified using the local Project data, update Project data from Stanford. - Issue 4
            HfmTrace.WriteToHfmConsole(TraceLevel.Info,
                                       String.Format("{0} Attempting to download new Project data...", HfmTrace.FunctionName));
            ProteinCollection.Instance.DownloadFromStanford();

            if (ProteinCollection.Instance.ContainsKey(ProjectID[0]))
            {
               SetProjectAndClientType(parsedUnitInfo, ProjectID);
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Error,
                                          String.Format("{0} Project ID '{1}' not found on Stanford Web Project Summary.",
                                                        HfmTrace.FunctionName, ProjectID[0]));
            }
         }
      }

      /// <summary>
      /// Sets the ProjectID and gets the Protein info from the Protein Collection (from Stanford)
      /// </summary>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="ProjectID">List of ProjectID values</param>
      /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when Project ID cannot be found in Protein Collection.</exception>
      private static void SetProjectAndClientType(UnitInfo parsedUnitInfo, IList<int> ProjectID)
      {
         Debug.Assert(ProjectID.Count == 4);

         parsedUnitInfo.ProjectID = ProjectID[0];
         parsedUnitInfo.ProjectRun = ProjectID[1];
         parsedUnitInfo.ProjectClone = ProjectID[2];
         parsedUnitInfo.ProjectGen = ProjectID[3];

         parsedUnitInfo.CurrentProtein = ProteinCollection.Instance[parsedUnitInfo.ProjectID];
         parsedUnitInfo.TypeOfClient = UnitInfo.GetClientTypeFromProtein(parsedUnitInfo.CurrentProtein);
      }

      /// <summary>
      /// Retrieve Instance Log Files based on Instance Type
      /// </summary>
      public void Retrieve()
      {
         // Don't allow this to fire more than once at a time
         if (_RetrievalInProgress) return;

         try
         {
            _RetrievalInProgress = true;
         
            switch (InstanceHostType)
            {
               case InstanceType.PathInstance:
                  RetrievePathInstance();
                  break;
               case InstanceType.HTTPInstance:
                  RetrieveHTTPInstance();
                  break;
               case InstanceType.FTPInstance:
                  RetrieveFTPInstance();
                  break;
               default:
                  throw new NotImplementedException(String.Format("Instance Type '{0}' is not implemented", InstanceHostType));
            }

            // Clear the Instance Level values before processing
            Clear();
            // Process the retrieved logs
            ClientStatus returnedStatus = ProcessExisting();

            /*** Setting this flag false aids in Unit Test since the results of *
             *   determining status are relative to the current time of day. ***/
            if (HandleStatusOnRetrieve)
            {
               // Handle the status retured from the log parse
               HandleReturnedStatus(returnedStatus);
            }
            else
            {
               Status = returnedStatus;
            }
         }
         catch (Exception ex)
         {
            Status = ClientStatus.Offline;
            HfmTrace.WriteToHfmConsole(InstanceName, ex);
         }
         finally
         {
            _RetrievalInProgress = false;
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Client Status: {2}", HfmTrace.FunctionName, InstanceName, Status));
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured Local path
      /// </summary>
      private void RetrievePathInstance()
      {
         DateTime Start = HfmTrace.ExecStart;

         try
         {
            FileInfo fiLog = new FileInfo(System.IO.Path.Combine(Path, RemoteFAHLogFilename));
            string FAHLog_txt = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedFAHLogName);
            FileInfo fiCachedLog = new FileInfo(FAHLog_txt);

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                       String.Format("{0} ({1}) FAHlog copy (start)", HfmTrace.FunctionName, InstanceName));
            if (fiLog.Exists)
            {
               if (fiCachedLog.Exists == false || fiLog.Length != fiCachedLog.Length)
               {
                  fiLog.CopyTo(FAHLog_txt, true);
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                             String.Format("{0} ({1}) FAHlog copy (success)", HfmTrace.FunctionName, InstanceName));
               }
               else
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                             String.Format("{0} ({1}) FAHlog copy (file has not changed)", HfmTrace.FunctionName, InstanceName));
               }
            }
            else
            {
               //Status = ClientStatus.Offline;
               //HfmTrace.WriteToHfmConsole(TraceLevel.Error,
               //                           String.Format("{0} ({1}) The path {2} is inaccessible.", HfmTrace.FunctionName, InstanceName, fiLog.FullName));
               //return false;
               
               throw new FileNotFoundException(String.Format("The path {0} is inaccessible.", fiLog.FullName));
            }

            // Retrieve unitinfo.txt (or equivalent)
            FileInfo fiUI = new FileInfo(System.IO.Path.Combine(Path, RemoteUnitInfoFilename));
            string UnitInfo_txt = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedUnitInfoName);

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                       String.Format("{0} ({1}) UnitInfo copy (start)", HfmTrace.FunctionName, InstanceName));
            if (fiUI.Exists)
            {
               // If file size is too large, do not copy it and delete the current cached copy - Issue 2
               if (fiUI.Length < NetworkOps.UnitInfoMax)
               {
                  fiUI.CopyTo(UnitInfo_txt, true);
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                             String.Format("{0} ({1}) UnitInfo copy (success)", HfmTrace.FunctionName, InstanceName));
               }
               else
               {
                  if (File.Exists(UnitInfo_txt))
                  {
                     File.Delete(UnitInfo_txt);
                  }
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                             String.Format("{0} ({1}) UnitInfo copy (file is too big: {2} bytes)", HfmTrace.FunctionName, InstanceName, fiUI.Length));
               }
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            //else
            //{
            //   Status = ClientStatus.Offline;
            //   HfmTrace.WriteToHfmConsole(TraceLevel.Error,
            //                              String.Format("{0} ({1}) The path {2} is inaccessible.", HfmTrace.FunctionName, InstanceName, fiUI.FullName));
            //   return false;
            //}
            else
            {
               if (File.Exists(UnitInfo_txt))
               {
                  File.Delete(UnitInfo_txt);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) The path {2} is inaccessible.", HfmTrace.FunctionName, InstanceName, fiUI.FullName));
            }

            // Retrieve queue.dat (or equivalent)
            FileInfo fiQueue = new FileInfo(System.IO.Path.Combine(Path, RemoteQueueFilename));
            string Queue_dat = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedQueueName);

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                       String.Format("{0} ({1}) Queue copy (start)", HfmTrace.FunctionName, InstanceName));
            if (fiQueue.Exists)
            {
               fiQueue.CopyTo(Queue_dat, true);
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) Queue copy (success)", HfmTrace.FunctionName, InstanceName));
            }
            /*** Remove Requirement for Queue to be Present ***/
            //else
            //{
            //   Status = ClientStatus.Offline;
            //   HfmTrace.WriteToHfmConsole(TraceLevel.Error,
            //                              String.Format("{0} ({1}) The path {2} is inaccessible.", HfmTrace.FunctionName, InstanceName, fiUI.FullName));
            //   return false;
            //}
            else
            {
               if (File.Exists(Queue_dat))
               {
                  File.Delete(Queue_dat);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) The path {2} is inaccessible.", HfmTrace.FunctionName, InstanceName, fiUI.FullName));
            }

            _LastRetrievalTime = DateTime.Now;
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, InstanceName, Start);
         }
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured HTTP location
      /// </summary>
      private void RetrieveHTTPInstance()
      {
         DateTime Start = HfmTrace.ExecStart;

         try
         {
            string HttpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Path, "/", RemoteFAHLogFilename);
            string LocalFile = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedFAHLogName);
            NetworkOps.HttpDownloadHelper(HttpPath, LocalFile, InstanceName, Username, Password, DownloadType.FAHLog);
            
            try
            {
               HttpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Path, "/", RemoteUnitInfoFilename);
               LocalFile = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedUnitInfoName);
               NetworkOps.HttpDownloadHelper(HttpPath, LocalFile, InstanceName, Username, Password, DownloadType.UnitInfo);
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(LocalFile))
               {
                  File.Delete(LocalFile);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) Unitinfo Download Threw Exception: {2}.", HfmTrace.FunctionName, InstanceName, ex.Message));
            }

            try
            {
               HttpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Path, "/", RemoteQueueFilename);
               LocalFile = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedQueueName);
               NetworkOps.HttpDownloadHelper(HttpPath, LocalFile, InstanceName, Username, Password, DownloadType.Queue);
            }
            /*** Remove Requirement for Queue to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(LocalFile))
               {
                  File.Delete(LocalFile);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) Queue Download Threw Exception: {2}.", HfmTrace.FunctionName, InstanceName, ex.Message));
            }

            _LastRetrievalTime = DateTime.Now;
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, InstanceName, Start);
         }
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured FTP location
      /// </summary>
      private void RetrieveFTPInstance()
      {
         DateTime Start = HfmTrace.ExecStart;

         try
         {
            string LocalFilePath = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedFAHLogName);
            NetworkOps.FtpDownloadHelper(Server, Path, RemoteFAHLogFilename, LocalFilePath, Username, Password, DownloadType.FAHLog);
            
            try
            {
               LocalFilePath = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedUnitInfoName);
               NetworkOps.FtpDownloadHelper(Server, Path, RemoteUnitInfoFilename, LocalFilePath, Username, Password, DownloadType.UnitInfo);
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(LocalFilePath))
               {
                  File.Delete(LocalFilePath);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) Unitinfo Download Threw Exception: {2}.", HfmTrace.FunctionName, InstanceName, ex.Message));
            }

            try
            {
               LocalFilePath = System.IO.Path.Combine(PreferenceSet.CacheDirectory, CachedQueueName);
               NetworkOps.FtpDownloadHelper(Server, Path, RemoteQueueFilename, LocalFilePath, Username, Password, DownloadType.Queue);
            }
            /*** Remove Requirement for Queue to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(LocalFilePath))
               {
                  File.Delete(LocalFilePath);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) Queue Download Threw Exception: {2}.", HfmTrace.FunctionName, InstanceName, ex.Message));
            }

            _LastRetrievalTime = DateTime.Now;
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, InstanceName, Start);
         }
      }
      #endregion

      #region XML Serialization
      /// <summary>
      /// Serialize this client instance to Xml
      /// </summary>
      public System.Xml.XmlDocument ToXml()
      {
         DateTime Start = HfmTrace.ExecStart;

         try
         {
            System.Xml.XmlDocument xmlData = new System.Xml.XmlDocument();

            System.Xml.XmlElement xmlRoot = xmlData.CreateElement(xmlNodeInstance);
            xmlRoot.SetAttribute(xmlAttrName, InstanceName);
            xmlData.AppendChild(xmlRoot);
            
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeFAHLog, RemoteFAHLogFilename));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeUnitInfo, RemoteUnitInfoFilename));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeQueue, RemoteQueueFilename));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientMHz, ClientProcessorMegahertz.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientVM, ClientIsOnVirtualMachine.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientOffset, ClientTimeOffset.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropType, InstanceHostType.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropPath, Path));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropServ, Server));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropUser, Username));

            Symmetric SymetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);
            
            string encryptedPassword = String.Empty;
            if (Password.Length > 0)
            {
               try
               {
                  SymetricProvider.IntializationVector = IV;
                  encryptedPassword = SymetricProvider.Encrypt(new Data(Password), SymmetricKey).ToBase64();
               }
               catch (CryptographicException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, "Failed to encrypt Server Password... saving clear value.");
                  encryptedPassword = Password;
               }
            }
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropPass, encryptedPassword));
            
            return xmlData;
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, InstanceName, Start);
         }
         
         return null;
      }

      /// <summary>
      /// Deserialize into this instance based on the given XmlNode data
      /// </summary>
      /// <param name="xmlData">XmlNode containing the client instance data</param>
      public void FromXml(System.Xml.XmlNode xmlData)
      {
         DateTime Start = HfmTrace.ExecStart;
         
         InstanceName = xmlData.Attributes[xmlAttrName].ChildNodes[0].Value;
         try
         {
            RemoteFAHLogFilename = xmlData.SelectSingleNode(xmlNodeFAHLog).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Remote FAHlog Filename."));
            RemoteFAHLogFilename = LocalFAHLog;
         }
         
         try
         {
            RemoteUnitInfoFilename = xmlData.SelectSingleNode(xmlNodeUnitInfo).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Remote Unitinfo Filename."));
            RemoteUnitInfoFilename = LocalUnitInfo;
         }
         
         try
         {
            RemoteQueueFilename = xmlData.SelectSingleNode(xmlNodeQueue).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Remote Queue Filename."));
            RemoteQueueFilename = LocalQueue;
         }
         
         try
         {
            ClientProcessorMegahertz = int.Parse(xmlData.SelectSingleNode(xmlNodeClientMHz).InnerText);
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client MHz, defaulting to 1 MHz."));
            ClientProcessorMegahertz = 1;
         }
         catch (FormatException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Could not parse Client MHz, defaulting to 1 MHz."));
            ClientProcessorMegahertz = 1;
         }

         try
         {
            ClientIsOnVirtualMachine = Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeClientVM).InnerText);
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client VM Flag, defaulting to false."));
            ClientIsOnVirtualMachine = false;
         }
         catch (InvalidCastException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Could not parse Client VM Flag, defaulting to false."));
            ClientIsOnVirtualMachine = false;
         }

         try
         {
            ClientTimeOffset = int.Parse(xmlData.SelectSingleNode(xmlNodeClientOffset).InnerText);
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client Time Offset, defaulting to 0."));
            ClientTimeOffset = 0;
         }
         catch (FormatException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Could not parse Client Time Offset, defaulting to 0."));
            ClientTimeOffset = 0;
         }

         try
         {
            Path = xmlData.SelectSingleNode(xmlPropPath).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client Path."));
         }

         try
         {
            Server = xmlData.SelectSingleNode(xmlPropServ).InnerText;
         }
         catch (NullReferenceException)
         {
            Server = String.Empty;
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client Server."));
         }
         
         try
         {
            Username = xmlData.SelectSingleNode(xmlPropUser).InnerText;
         }
         catch (NullReferenceException)
         {
            Username = String.Empty;
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Server Username."));
         }
         
         Symmetric SymetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);
         
         try
         {
            Password = String.Empty;
            if (xmlData.SelectSingleNode(xmlPropPass).InnerText.Length > 0)
            {
               try
               {
                  SymetricProvider.IntializationVector = IV;
                  Password = SymetricProvider.Decrypt(new Data(Utils.FromBase64(xmlData.SelectSingleNode(xmlPropPass).InnerText)), SymmetricKey).ToString();
               }
               catch (FormatException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, "Server Password is not Base64 encoded... loading clear value.");
                  Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
               }
               catch (CryptographicException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, "Cannot decrypt Server Password... loading clear value.");
                  Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
               }
            }
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Server Password."));
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, InstanceName, Start);
      }
      #endregion
      
      #region Status Color Helper Functions
      /// <summary>
      /// Gets Status Color Pen Object
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Color (Pen)</returns>
      public static Pen GetStatusPen(ClientStatus status)
      {
         return new Pen(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Color Brush Object
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Color (Brush)</returns>
      public static SolidBrush GetStatusBrush(ClientStatus status)
      {
         return new SolidBrush(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Color String
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Html Color (String)</returns>
      public static string GetStatusHtmlColor(ClientStatus status)
      {
         return ColorTranslator.ToHtml(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Font Color String
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Html Font Color (String)</returns>
      public static string GetStatusHtmlFontColor(ClientStatus status)
      {
         switch (status)
         {
            case ClientStatus.Running:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.RunningNoFrameTimes:
               return ColorTranslator.ToHtml(Color.Black);
            case ClientStatus.Stopped:
            case ClientStatus.EuePause:
            case ClientStatus.Hung:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.Paused:
               return ColorTranslator.ToHtml(Color.Black);
            case ClientStatus.GettingWorkPacket:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.Offline:
               return ColorTranslator.ToHtml(Color.Black);
            default:
               return ColorTranslator.ToHtml(Color.Black);
         }
      }

      /// <summary>
      /// Gets Status Color Object
      /// </summary>
      /// <param name="status">Client Status</param>
      /// <returns>Status Color (Color)</returns>
      public static Color GetStatusColor(ClientStatus status)
      {
         switch (status)
         {
            case ClientStatus.Running:
               return Color.Green; // Issue 45
            case ClientStatus.RunningNoFrameTimes:
               return Color.Yellow;
            case ClientStatus.Stopped:
            case ClientStatus.EuePause:
            case ClientStatus.Hung:
               return Color.DarkRed;
            case ClientStatus.Paused:
               return Color.Orange;
            case ClientStatus.GettingWorkPacket:
               return Color.Purple;
            case ClientStatus.Offline:
               return Color.Gray;
            default:
               return Color.Gray;
         }
      }
      #endregion

      #region Other Helper Functions
      public bool IsUsernameOk()
      {
         // if these are the default assigned values, don't check otherwise and just return true
         if (CurrentUnitInfo.FoldingID == UnitInfo.UsernameDefault && CurrentUnitInfo.Team == UnitInfo.TeamDefault)
         {
            return true;
         }

         PreferenceSet Prefs = PreferenceSet.Instance;

         if ((CurrentUnitInfo.FoldingID != Prefs.StanfordID || CurrentUnitInfo.Team != Prefs.TeamID) &&
             (Status.Equals(ClientStatus.Unknown) == false && Status.Equals(ClientStatus.Offline) == false))
         {
            return false;
         }

         return true;
      }
      
      public bool Owns(IOwnedByClientInstance value)
      {
         if (value.OwningInstanceName.Equals(InstanceName) &&
             value.OwningInstancePath.Equals(Path))
         {
            return true;
         }
         
         return false;
      }
      #endregion
   }
}
