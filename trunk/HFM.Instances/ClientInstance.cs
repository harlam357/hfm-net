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
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using HFM.Helpers;
using HFM.Proteins;
using HFM.Preferences;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Instances
{
   #region Enum
   public enum ClientStatus
   {
      Unknown,
      Offline,
      Stopped,
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

   public enum DownloadType
   {
      FAHLog = 0,
      UnitInfo
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
      
      // Log File Size Constants
      private const int UnitInfoMax = 1048576; // 1 Megabyte
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

      #region Private Members
      /// <summary>
      /// Local flag set when log retrieval is in progress
      /// </summary>
      private bool _RetrievalInProgress = false;
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
      #endregion

      #region Public Properties and Related Private Members
      
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
         private set
         {
            _LastRetrievalTime = value;
         }
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
         // reset total, completed, and failed values
         UserID = String.Empty;
         MachineID = 0;
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
                  Debug.WriteToHfmConsole(TraceLevel.Error,
                                          String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
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
         if (HFM.Instrumentation.TraceLevelSwitch.GetTraceLevelSwitch().TraceVerbose)
         {
            List<string> messages = new List<string>(10);

            messages.Add(String.Format("{0} ({1})", Debug.FunctionName, Instance.InstanceName));
            messages.Add(String.Format(" - Retrieval Time (Date) ------- : {0}", Instance.LastRetrievalTime));
            messages.Add(String.Format(" - Time Of Last Frame (TimeSpan) : {0}", TimeOfLastFrame));
            messages.Add(String.Format(" - Offset (Minutes) ------------ : {0}", Instance.ClientTimeOffset));
            messages.Add(String.Format(" - Time Of Last Frame (Adjusted) : {0}", adjustedFrameTime));
            messages.Add(String.Format(" - Prior Day Adjustment -------- : {0}", priorDayAdjust));
            messages.Add(String.Format(" - Time Of Last Frame (Date) --- : {0}", currentFrameDateTime));
            messages.Add(String.Format(" - Terminal Time (Date) -------- : {0}", terminalDateTime));

            Debug.WriteToHfmConsole(TraceLevel.Verbose, messages.ToArray());
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
         DateTime Start = Debug.ExecStart;

         UnitInfo parsedUnitInfo = null;

         LogParser lp = new LogParser();
         if (lp.ReadLogText(System.IO.Path.Combine(BaseDirectory, CachedFAHLogName)))
         {
            if (lp.Previous1UnitStartPosition > 0)
            {
               parsedUnitInfo = lp.ParseFAHLog(this, UnitToRead.Previous1);
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
            if (lp.LastUnitStartPosition > 0)
            {
               Status = ClientStatus.RunningNoFrameTimes;
            
               parsedUnitInfo = lp.ParseFAHLog(this, UnitToRead.Last);

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
         }

         if (parsedUnitInfo != null)
         {
            // Parsed is now Current
            CurrentUnitInfo = parsedUnitInfo;

            if (Status == ClientStatus.Stopped ||
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

         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
      }

      /// <summary>
      /// Retrieve Instance Log Files based on Instance Type
      /// </summary>
      public void Retrieve()
      {
         bool success;
         
         switch (InstanceHostType)
         {
            case InstanceType.PathInstance:
               success = RetrievePathInstance();
               break;
            case InstanceType.HTTPInstance:
               success = RetrieveHTTPInstance();
               break;
            case InstanceType.FTPInstance:
               success = RetrieveFTPInstance();
               break;
            default:
               throw new NotImplementedException(String.Format("Instance Type '{0}' is not implemented", InstanceHostType));
         }

         // Clear the instance before
         Clear();

         if (success)
         {
            ProcessExisting();
         }
         else
         {
            // Clear the time based values when log retrieval fails
            CurrentUnitInfo.ClearTimeBasedValues();
         }
         
         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Client Status: {2}", Debug.FunctionName, InstanceName, Status));
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured Local path
      /// </summary>
      public bool RetrievePathInstance()
      {
         if (_RetrievalInProgress)
         {
            return false;
         }

         DateTime Start = Debug.ExecStart;

         try
         {
            _RetrievalInProgress = true;

            FileInfo fiLog = new FileInfo(System.IO.Path.Combine(Path, RemoteFAHLogFilename));
            string FAHLog_txt = System.IO.Path.Combine(BaseDirectory, CachedFAHLogName);
            FileInfo fiCachedLog = new FileInfo(FAHLog_txt);

            Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                    String.Format("{0} ({1}) FAHlog copy (start).", Debug.FunctionName,
                                                  InstanceName));
            if (fiLog.Exists)
            {
               if (fiCachedLog.Exists == false || fiLog.Length != fiCachedLog.Length)
               {
                  fiLog.CopyTo(FAHLog_txt, true);
                  Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) FAHlog copy (success).", Debug.FunctionName,
                                                        InstanceName));
               }
               else
               {
                  Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) FAHlog copy (file has not changed).", Debug.FunctionName,
                                                        InstanceName));
               }
            }
            else
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) The path {2} is inaccessible.", Debug.FunctionName, InstanceName, fiLog.FullName));
               return false;
            }

            // Retrieve unitinfo.txt (or equivalent)
            FileInfo fiUI = new FileInfo(System.IO.Path.Combine(Path, RemoteUnitInfoFilename));
            string UnitInfo_txt = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);

            Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                    String.Format("{0} ({1}) UnitInfo copy (start).", Debug.FunctionName, InstanceName));
            if (fiUI.Exists)
            {
               // If file size is too large, do not copy it and delete the current cached copy - Issue 2
               if (fiUI.Length < UnitInfoMax)
               {
                  fiUI.CopyTo(UnitInfo_txt, true);
                  Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) UnitInfo copy (success).", Debug.FunctionName,
                                                        InstanceName));
               }
               else
               {
                  if (File.Exists(UnitInfo_txt))
                  {
                     File.Delete(UnitInfo_txt);
                  }
                  Debug.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) UnitInfo copy (file is too big: {2} bytes).", Debug.FunctionName,
                                                        InstanceName, fiUI.Length));
               }
            }
            else
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) The path {2} is inaccessible.", Debug.FunctionName, InstanceName, fiUI.FullName));
               return false;
            }

            LastRetrievalTime = DateTime.Now;
         }
         catch (Exception ex)
         {
            Status = ClientStatus.Offline;
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName, ex.Message));

            return false;
         }
         finally
         {
            _RetrievalInProgress = false;
         }
         
         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));

         return true;
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured HTTP location
      /// </summary>
      public bool RetrieveHTTPInstance()
      {
         if (_RetrievalInProgress)
         {
            // Don't allow this to fire more than once at a time
            return false;
         }

         DateTime Start = Debug.ExecStart;

         try
         {
            _RetrievalInProgress = true;

            bool bFAHLog = HttpDownloadHelper(RemoteFAHLogFilename, CachedFAHLogName, DownloadType.FAHLog);
            bool bUnitInfo = false;
            if (bFAHLog)
            {
               bUnitInfo = HttpDownloadHelper(RemoteUnitInfoFilename, CachedUnitInfoName, DownloadType.UnitInfo);
            }
            
            if ((bFAHLog && bUnitInfo) == false)
            {
               return false;
            }

            LastRetrievalTime = DateTime.Now;
         }
         catch (Exception ex)
         {
            Status = ClientStatus.Offline;
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName, ex.Message));
            return false;
         }
         finally
         {
            _RetrievalInProgress = false;
         }

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
         
         return true;
      }
      
      /// <summary>
      /// Makes the Http connection and downloads the specified files
      /// </summary>
      /// <param name="RemoteLogFilename">Remote filename</param>
      /// <param name="CachedLogFilename">Local Cached filename</param>
      /// <param name="type">Type of Download (FAHLog or UnitInfo)</param>
      private bool HttpDownloadHelper(string RemoteLogFilename, string CachedLogFilename, DownloadType type)
      {
         PreferenceSet Prefs = PreferenceSet.Instance;
      
         WebRequest httpc1 = WebRequest.Create(String.Format("{0}{1}{2}", Path, "/", RemoteLogFilename));
         httpc1.Method = WebRequestMethods.Http.Get;
         httpc1.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         httpc1.Credentials = new NetworkCredential(Username, Password);
         
         if (Prefs.UseProxy)
         {
            httpc1.Proxy = new WebProxy(Prefs.ProxyServer, Prefs.ProxyPort);
            if (Prefs.UseProxyAuth)
            {
               httpc1.Proxy.Credentials = new NetworkCredential(Prefs.ProxyUser, Prefs.ProxyPass);
            }
         }
         else
         {
            httpc1.Proxy = null;
         }

         string FAHLog_txt = System.IO.Path.Combine(BaseDirectory, CachedLogFilename);

         StreamWriter sw1 = null;
         StreamReader sr1 = null;
         try
         {
            WebResponse r1 = httpc1.GetResponse();
            if (type.Equals(DownloadType.UnitInfo) && r1.ContentLength >= UnitInfoMax)
            {
               if (File.Exists(FAHLog_txt))
               {
                  File.Delete(FAHLog_txt);
               }
               Debug.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} ({1}) UnitInfo HTTP download (file is too big: {2} bytes).", Debug.FunctionName,
                                                     InstanceName, r1.ContentLength));
            }
            else
            {
               sr1 = new StreamReader(r1.GetResponseStream(), Encoding.ASCII);
               sw1 = new StreamWriter(FAHLog_txt, false);
               sw1.Write(sr1.ReadToEnd());
            }
         }
         finally
         {
            if (sr1 != null)
            {
               sr1.Close();
            }
            if (sw1 != null)
            {
               sw1.Flush();
               sw1.Close();
            }
         }
         
         return true;
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured FTP location
      /// </summary>
      public bool RetrieveFTPInstance()
      {
         if (_RetrievalInProgress)
         {
            // Don't allow this to fire more than once at a time
            return false;
         }

         DateTime Start = Debug.ExecStart;

         try
         {
            _RetrievalInProgress = true;

            bool bFAHLog = FtpDownloadHelper(RemoteFAHLogFilename, CachedFAHLogName);
            bool bUnitInfo = false;
            if (bFAHLog)
            {
               bUnitInfo = FtpDownloadHelper(RemoteUnitInfoFilename, CachedUnitInfoName);
            }

            if ((bFAHLog && bUnitInfo) == false)
            {
               return false;
            }
            
            LastRetrievalTime = DateTime.Now;
         }
         catch (Exception ex)
         {
            Status = ClientStatus.Offline;
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName, ex.Message));
            return false;
         }
         finally
         {
            _RetrievalInProgress = false;
         }

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
         
         return true;
      }

      /// <summary>
      /// Makes the Ftp connection and downloads the specified files
      /// </summary>
      /// <param name="RemoteLogFilename">Remote filename</param>
      /// <param name="CachedLogFilename">Local Cached filename</param>
      private bool FtpDownloadHelper(string RemoteLogFilename, string CachedLogFilename)
      {
         PreferenceSet Prefs = PreferenceSet.Instance;

         FtpWebRequest ftpc1 = (FtpWebRequest)FtpWebRequest.Create(String.Format("ftp://{0}{1}{2}", Server, Path, RemoteLogFilename));
         ftpc1.Method = WebRequestMethods.Ftp.DownloadFile;
         ftpc1.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         
         if ((Username != String.Empty) && (Username != null))
         {
            if (Username.Contains("\\"))
            {
               String[] UserParts = Username.Split('\\');
               ftpc1.Credentials = new NetworkCredential(UserParts[1], Password, UserParts[0]);
            }
            else
            {
               ftpc1.Credentials = new NetworkCredential(Username, Password);
            }
         }
         
         if (Prefs.UseProxy)
         {
            ftpc1.Proxy = new WebProxy(Prefs.ProxyServer, Prefs.ProxyPort);
            if (Prefs.UseProxyAuth)
            {
               ftpc1.Proxy.Credentials = new NetworkCredential(Prefs.ProxyUser, Prefs.ProxyPass);
            }
         }
         else
         {
            ftpc1.Proxy = null;
         }

         string FAHLog_txt = System.IO.Path.Combine(BaseDirectory, CachedLogFilename);
         
         StreamReader sr1 = null;
         StreamWriter sw1 = null;
         try
         {
            FtpWebResponse ftpr1 = (FtpWebResponse)ftpc1.GetResponse();
            sr1 = new StreamReader(ftpr1.GetResponseStream(), Encoding.ASCII);
            sw1 = new StreamWriter(FAHLog_txt, false);
            sw1.Write(sr1.ReadToEnd());
         }
         finally
         {
            if (sr1 != null)
            {
               sr1.Close();
            }
            if (sw1 != null)
            {
               sw1.Flush();
               sw1.Close();
            }
         }

         return true;
      }
      #endregion

      #region XML Serialization
      /// <summary>
      /// Serialize this client instance to Xml
      /// </summary>
      public System.Xml.XmlDocument ToXml()
      {
         DateTime Start = Debug.ExecStart;

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
            
            Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
            return xmlData;
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
         }
         
         return null;
      }

      /// <summary>
      /// Deserialize into this instance based on the given XmlNode data
      /// </summary>
      /// <param name="xmlData">XmlNode containing the client instance data</param>
      public void FromXml(System.Xml.XmlNode xmlData)
      {
         DateTime Start = Debug.ExecStart;
         
         InstanceName = xmlData.Attributes[xmlAttrName].ChildNodes[0].Value;
         try
         {
            RemoteFAHLogFilename = xmlData.SelectSingleNode(xmlNodeFAHLog).InnerText;
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Remote FAH Log Filename."));
            RemoteFAHLogFilename = LocalFAHLog;
         }
         
         try
         {
            RemoteUnitInfoFilename = xmlData.SelectSingleNode(xmlNodeUnitInfo).InnerText;
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Remote FAH UnitInfo Filename."));
            RemoteUnitInfoFilename = LocalUnitInfo;
         }
         
         try
         {
            ClientProcessorMegahertz = int.Parse(xmlData.SelectSingleNode(xmlNodeClientMHz).InnerText);
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client MHz, defaulting to 1 MHz."));
            ClientProcessorMegahertz = 1;
         }
         catch (FormatException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Could not parse Client MHz, defaulting to 1 MHz."));
            ClientProcessorMegahertz = 1;
         }

         try
         {
            ClientIsOnVirtualMachine = Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeClientVM).InnerText);
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client VM Flag, defaulting to false."));
            ClientIsOnVirtualMachine = false;
         }
         catch (InvalidCastException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Could not parse Client VM Flag, defaulting to false."));
            ClientIsOnVirtualMachine = false;
         }

         try
         {
            ClientTimeOffset = int.Parse(xmlData.SelectSingleNode(xmlNodeClientOffset).InnerText);
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client Time Offset, defaulting to 0."));
            ClientTimeOffset = 0;
         }
         catch (FormatException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Could not parse Client Time Offset, defaulting to 0."));
            ClientTimeOffset = 0;
         }

         try
         {
            Path = xmlData.SelectSingleNode(xmlPropPath).InnerText;
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client Path."));
         }

         try
         {
            Server = xmlData.SelectSingleNode(xmlPropServ).InnerText;
         }
         catch (NullReferenceException)
         {
            Server = String.Empty;
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client Server."));
         }
         
         try
         {
            Username = xmlData.SelectSingleNode(xmlPropUser).InnerText;
         }
         catch (NullReferenceException)
         {
            Username = String.Empty;
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Server Username."));
         }
         
         try
         {
            Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
         }
         catch (NullReferenceException)
         {
            Password = String.Empty;
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Server Password."));
         }

         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
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
               return Color.DarkGreen;
            case ClientStatus.RunningNoFrameTimes:
               return Color.Yellow;
            case ClientStatus.Stopped:
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
   }
}
