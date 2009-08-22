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

using HFM.Helpers;
using HFM.Instrumentation;
using HFM.Proteins;
using HFM.Preferences;

namespace HFM.Instances
{
   #region Enum
   public enum ClientStatus
   {
      Unknown,
      Offline,
      Stopped,
      EuePause,
      Hung,
      Paused,
      RunningNoFrameTimes,
      Running
   }

   public enum InstanceType
   {
      PathInstance,
      FTPInstance,
      HTTPInstance
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
      
      public const string DefaultUserID = "";
      public const int DefaultMachineID = 0; 
      #endregion
      
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
      /// Log File Cache Directory
      /// </summary>
      public string BaseDirectory
      {
         get { return System.IO.Path.Combine(PreferenceSet.Instance.AppDataPath, PreferenceSet.Instance.CacheFolder); }
      }

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
      #endregion

      #region Public Properties and Related Private Members

      #region Retrieval In Progress Flag
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
            if (value == String.Empty)
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
            if (value == String.Empty)
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

      #region Log Retrieval Timestamps
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
         set { _Status = value; }
      }

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
      private UnitInfo _UnitInfo;
      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      public UnitInfo CurrentUnitInfo
      {
         get { return _UnitInfo; }
         set 
         { 
            _UnitInfo = value;
         }
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
         InstanceHostTypeChanged += ClearUserSpecifiedValues;
         // When Client is on VM Changes, Clear the Unit Frame Data
         // The captured TimeOfFrame values will no longer be valid
         ClientIsOnVirtualMachineChanged += ClearFrameData;
         
         // Set the Host Type
         _InstanceHostType = type;
         // Clear Instance Specific Values
         Clear();
         // Create a fresh UnitInfo
         _UnitInfo = new UnitInfo(InstanceName, Path);
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

      /// <summary>
      /// Clear the Unit Frame Data from the Current Unit Info
      /// </summary>
      private void ClearFrameData(object sender, EventArgs e)
      {
         CurrentUnitInfo.ClearFrameData();
      }
      
      /// <summary>
      /// 
      /// </summary>
      private bool IsUnitInfoCurrentUnitInfo(UnitInfo parsedUnitInfo)
      {
         // if the Project is the same and either the Name or Path matches
         if (parsedUnitInfo.ProjectRunCloneGen == CurrentUnitInfo.ProjectRunCloneGen)
         {
            return true;
         }
         
         return false;
      }
      
      /// <summary>
      /// Sets the time based values on the Current Unit Info
      /// </summary>
      public void SetTimeBasedValues()
      {
         SetTimeBasedValues(CurrentUnitInfo);
      }

      /// <summary>
      /// Sets the time based values (FramesComplete, PercentComplete, TimePerFrame, UPD, PPD, ETA)
      /// </summary>
      public void SetTimeBasedValues(UnitInfo unit)
      {
         //if ((unit.RawFramesTotal != 0) && (unit.RawFramesComplete != 0))
         //{
            if (unit.RawTimePerSection != 0)
            {
               try
               {
                  Int32 FramesTotal = ProteinCollection.Instance[unit.ProjectID].Frames;
                  Int32 RawScaleFactor = unit.RawFramesTotal / FramesTotal;

                  //TODO: FramesComplete is pretty isolated here... can it be moved into this routine and not exposed at the class level?
                  unit.FramesComplete = unit.RawFramesComplete / RawScaleFactor;
                  unit.PercentComplete = unit.FramesComplete * 100 / FramesTotal;
                  unit.TimePerFrame = new TimeSpan(0, 0, Convert.ToInt32(unit.RawTimePerSection));

                  unit.UPD = ProteinCollection.Instance[unit.ProjectID].GetUPD(unit.TimePerFrame);
                  unit.PPD = ProteinCollection.Instance[unit.ProjectID].GetPPD(unit.TimePerFrame);
                  unit.ETA = new TimeSpan((100 - unit.PercentComplete) * unit.TimePerFrame.Ticks);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
            else if (Status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               // If we have frames but no section time, try pulling the percent complete from the UnitFrame data
               if (unit.PercentComplete == 0)
               {
                  // Only if we didn't get a reading from the unitinfo.txt parse
                  unit.PercentComplete = unit.LastUnitFramePercent;
               }

               TimeSpan benchmarkAverageTimePerFrame = ProteinBenchmarkCollection.Instance.GetBenchmarkAverageFrameTime(unit);
               if (benchmarkAverageTimePerFrame.Equals(TimeSpan.Zero) == false)
               {
                  unit.TimePerFrame = benchmarkAverageTimePerFrame;
                  unit.UPD = ProteinCollection.Instance[unit.ProjectID].GetUPD(unit.TimePerFrame);
                  unit.PPD = ProteinCollection.Instance[unit.ProjectID].GetPPD(unit.TimePerFrame);
                  unit.ETA = new TimeSpan((100 - unit.PercentComplete) * unit.TimePerFrame.Ticks);
               }
            }
         //}
      }

      /// <summary>
      /// Clear the user specified values that define this instance
      /// </summary>
      private void ClearUserSpecifiedValues(object sender, EventArgs e)
      {
         InstanceName = String.Empty;
         ClientProcessorMegahertz = 1;
         RemoteFAHLogFilename = String.Empty;
         RemoteUnitInfoFilename = String.Empty;
         ClientIsOnVirtualMachine = false;
         ClientTimeOffset = 0;

         Path = String.Empty;
         Server = String.Empty;
         Username = String.Empty;
         Password = String.Empty;
      }

      /// <summary>
      /// Determines what values to feed the DetermineStatus routine and then calls it
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      private static void DetermineStatusWrapper(ClientInstance Instance)
      {
         // if we have a frame time, use it
         if (Instance.CurrentUnitInfo.RawTimePerSection > 0)
         {
            Instance.Status = DetermineStatus(Instance, Instance.CurrentUnitInfo.TimeOfLastFrame, Instance.CurrentUnitInfo.RawTimePerSection);
         }

         // no frame time based on the current PPD calculation selection ('LastFrame', 'LastThreeFrames', etc)
         // this section attempts to give DetermineStats values to detect Hung clients before they have a valid
         // frame time - Issue 10
         else 
         {
            // if we have a frame time stamp, use it
            TimeSpan frameTime = Instance.CurrentUnitInfo.TimeOfLastFrame;
            if (frameTime == TimeSpan.Zero)
            {
               // otherwise, use the unit start time
               frameTime = Instance.CurrentUnitInfo.UnitStartTime;
            }

            // get the average frame time for this client and project id
            TimeSpan averageFrameTime = ProteinBenchmarkCollection.Instance.GetBenchmarkAverageFrameTime(Instance.CurrentUnitInfo);
            if (averageFrameTime > TimeSpan.Zero)
            {
               if (DetermineStatus(Instance, frameTime, Convert.ToInt32(averageFrameTime.TotalSeconds)).Equals(ClientStatus.Hung))
               {
                  Instance.Status = ClientStatus.Hung;
               }
            }

            // no benchmarked average frame time, use some arbitrary (and large) values for the frame time
            // we want to give the client plenty of time to show progress but don't want it to sit idle for days
            else 
            {
               if (Instance.CurrentUnitInfo.TypeOfClient.Equals(ClientType.GPU))
               {
                  // GPU: use 5 minutes (300 seconds) as a base frame time
                  if (DetermineStatus(Instance, frameTime, 300).Equals(ClientStatus.Hung))
                  {
                     Instance.Status = ClientStatus.Hung;
                  }
               }
               else
               {
                  // CPU: use 1 hour (3600 seconds) as a base frame time
                  if (DetermineStatus(Instance, frameTime, 3600).Equals(ClientStatus.Hung))
                  {
                     Instance.Status = ClientStatus.Hung;
                  }
               } 
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
         if (TraceLevelSwitch.GetTraceLevelSwitch().TraceVerbose)
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
      public void ProcessExisting()
      {
         DateTime Start = HfmTrace.ExecStart;

         UnitInfo parsedUnitInfo = null;

         LogReader lr = new LogReader();
         lr.ReadLogText(this, System.IO.Path.Combine(BaseDirectory, CachedFAHLogName));
         lr.ScanFAHLog(this);
         
         LogParser lp = new LogParser();
         
         IList<LogLine> logLines = lr.PreviousWorkUnitLogLines;
         if (logLines != null)
         {
            parsedUnitInfo = new UnitInfo(InstanceName, Path, FoldingID, Team);
            lp.ParseFAHLog(this, logLines, parsedUnitInfo);
            
            // check this against the CurrentUnitInfo
            if (IsUnitInfoCurrentUnitInfo(parsedUnitInfo))
            {
               // current frame has already been recorded, increment to the next frame
               int previousFramePercent = CurrentUnitInfo.LastUnitFramePercent + 1;

               // update the UnitFrames
               CurrentUnitInfo.UnitFrames = parsedUnitInfo.UnitFrames;
               CurrentUnitInfo.CurrentFrame = parsedUnitInfo.CurrentFrame;
               CurrentUnitInfo.FramesObserved = parsedUnitInfo.FramesObserved;

               // set the frame times and calculate values
               CurrentUnitInfo.SetFrameTimes();
               SetTimeBasedValues();
               ProteinBenchmarkCollection.Instance.UpdateBenchmarkData(CurrentUnitInfo, previousFramePercent, CurrentUnitInfo.LastUnitFramePercent);
            }
         }
         
         logLines = lr.CurrentWorkUnitLogLines;
         if (logLines != null)
         {
            _CurrentLogLines = logLines;
            
            Status = ClientStatus.RunningNoFrameTimes;

            parsedUnitInfo = new UnitInfo(InstanceName, Path, FoldingID, Team);
            if (lp.ParseUnitInfoFile(System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName), this, parsedUnitInfo) == false)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} ({1}) unitinfo parse failed.", HfmTrace.FunctionName, InstanceName));
            }
            lp.ParseFAHLog(this, logLines, parsedUnitInfo);

            int currentFrames = 0;
            // check this against the CurrentUnitInfo
            if (IsUnitInfoCurrentUnitInfo(parsedUnitInfo))
            {
               // current frame has already been recorded, increment to the next frame
               currentFrames = CurrentUnitInfo.LastUnitFramePercent + 1;
            }

            // set the frame times and calculate values
            parsedUnitInfo.SetFrameTimes();
            SetTimeBasedValues(parsedUnitInfo);
            ProteinBenchmarkCollection.Instance.UpdateBenchmarkData(parsedUnitInfo, currentFrames, parsedUnitInfo.LastUnitFramePercent);
         }

         if (parsedUnitInfo != null)
         {
            // Parsed is now Current
            CurrentUnitInfo = parsedUnitInfo;

            if (Status == ClientStatus.Stopped ||
                Status == ClientStatus.EuePause ||
                Status == ClientStatus.Paused)
            {
               // client is stopped or paused, clear PPD values
               CurrentUnitInfo.ClearTimeBasedValues();
            }
            else
            {
               DetermineStatusWrapper(this);
               if (Status.Equals(ClientStatus.Hung))
               {
                  // client is hung, clear PPD values
                  CurrentUnitInfo.ClearTimeBasedValues();
               }
            }
         }
         else
         {
            // Clear the time based values when log parsing fails
            CurrentUnitInfo.ClearTimeBasedValues();
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, InstanceName, Start);
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
            ProcessExisting();
         }
         catch (Exception ex)
         {
            Status = ClientStatus.Offline;
            HfmTrace.WriteToHfmConsole(InstanceName, ex);

            // Clear the time based values when log retrieval fails
            CurrentUnitInfo.ClearTimeBasedValues();
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
            string FAHLog_txt = System.IO.Path.Combine(BaseDirectory, CachedFAHLogName);
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
            string UnitInfo_txt = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);

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
            string LocalFile = System.IO.Path.Combine(BaseDirectory, CachedFAHLogName);
            NetworkOps.HttpDownloadHelper(HttpPath, LocalFile, InstanceName, Username, Password, DownloadType.FAHLog);
            
            try
            {
               HttpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Path, "/", RemoteUnitInfoFilename);
               LocalFile = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);
               NetworkOps.HttpDownloadHelper(HttpPath, LocalFile, InstanceName, Username, Password, DownloadType.UnitInfo);
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) Unitinfo Download Threw Exception: {2}.", HfmTrace.FunctionName, InstanceName, ex.Message));
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
            string LocalFilePath = System.IO.Path.Combine(BaseDirectory, CachedFAHLogName);
            NetworkOps.FtpDownloadHelper(Server, Path, RemoteFAHLogFilename, LocalFilePath, Username, Password);
            
            try
            {
               LocalFilePath = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);
               NetworkOps.FtpDownloadHelper(Server, Path, RemoteUnitInfoFilename, LocalFilePath, Username, Password);
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) Unitinfo Download Threw Exception: {2}.", HfmTrace.FunctionName, InstanceName, ex.Message));
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
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientMHz, ClientProcessorMegahertz.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientVM, ClientIsOnVirtualMachine.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientOffset, ClientTimeOffset.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropType, InstanceHostType.ToString()));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropPath, Path));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropServ, Server));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropUser, Username));
            xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropPass, Password));
            
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
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Remote FAH unitinfo Filename."));
            RemoteUnitInfoFilename = LocalUnitInfo;
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
         
         try
         {
            Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
         }
         catch (NullReferenceException)
         {
            Password = String.Empty;
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
      #endregion
   }
}
