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
      private const string LocalFAHLog = "FAHLog.txt";
      private const string LocalUnitInfo = "UnitInfo.txt";
      #endregion
      
      #region Public Events
      /// <summary>
      /// Raised when Instance Host Type is Changed
      /// </summary>
      public event EventHandler InstanceHostTypeChanged;
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
      /// Cached FAHLog Filename for this instance
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
      
      #region User specified values (from the frmHost dialog)
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
      private Int32 _ClientProcessorMegahertz;
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
            _InstanceHostType = value;
            OnInstanceHostTypeChanged(EventArgs.Empty);
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
         set { _ClientIsOnVirtualMachine = value; }
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
            //_PreviousLastRetrievalTime = _LastRetrievalTime;
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
      /// List of current log file text lines
      /// </summary>
      private readonly List<string> _CurrentLogText = new List<string>();
      /// <summary>
      /// List of current log file text lines
      /// </summary>
      public List<string> CurrentLogText
      {
         get { return _CurrentLogText; }
      }

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      private Protein _CurrentProtein;
      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      public Protein CurrentProtein
      {
         get { return _CurrentProtein; }
         set { _CurrentProtein = value; }
      }

      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      private readonly UnitInfo _UnitInfo;
      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      public UnitInfo UnitInfo
      {
         get { return _UnitInfo; }
      } 
      #endregion
      
      #endregion

      #region Constructor
      /// <summary>
      /// Primary Constructor
      /// </summary>
      public ClientInstance(InstanceType type)
      {
         InstanceHostTypeChanged += ClearInstanceValues;
         _InstanceHostType = type;

         _UnitInfo = new UnitInfo();
         Clear();
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
      #endregion

      #region Data Processing
      /// <summary>
      /// Clear Client Instance and UnitInfo Values
      /// </summary>
      private void Clear()
      {
         // reset total, completed, and failed values
         TotalUnits = 0;
         NumberOfCompletedUnitsSinceLastStart = 0;
         NumberOfFailedUnitsSinceLastStart = 0;
         // clear the instance log holder
         CurrentLogText.Clear();
      
         UnitInfo.TypeOfClient = ClientType.Unknown;
         UnitInfo.CoreVersion = String.Empty;
         UnitInfo.DownloadTime = DateTime.MinValue;
         UnitInfo.DueTime = DateTime.MinValue;
         UnitInfo.FramesComplete = 0;
         UnitInfo.PercentComplete = 0;
         UnitInfo.ProjectID = 0;
         UnitInfo.ProjectRun = 0;
         UnitInfo.ProjectClone = 0;
         UnitInfo.ProjectGen = 0;
         UnitInfo.ProteinName = String.Empty;
         UnitInfo.ProteinTag = String.Empty;
         UnitInfo.RawFramesComplete = 0;
         UnitInfo.RawFramesTotal = 0;
         UnitInfo.TimeOfLastFrame = TimeSpan.Zero;
         
         ClearTimeBasedValues();

         _CurrentProtein = new Protein();
      }
      
      /// <summary>
      /// Clear only the time based values for this instance
      /// </summary>
      private void ClearTimeBasedValues()
      {
         // Set in SetTimeBasedValues()
         UnitInfo.TimePerFrame = TimeSpan.Zero;
         UnitInfo.UPD = 0.0;
         UnitInfo.PPD = 0.0;
         UnitInfo.ETA = TimeSpan.Zero;

         // Set in LogParser.SetTimeStamp()
         UnitInfo.RawTimePerLastSection = 0;
         UnitInfo.RawTimePerThreeSections = 0;
      }

      /// <summary>
      /// Sets the time based values (FramesComplete, PercentComplete, TimePerFrame, UPD, PPD, ETA)
      /// </summary>
      public void SetTimeBasedValues()
      {
         if ((UnitInfo.RawFramesTotal != 0) && (UnitInfo.RawFramesComplete != 0) && (UnitInfo.RawTimePerSection != 0))
         {
            try
            {
               Int32 FramesTotal = ProteinCollection.Instance[UnitInfo.ProjectID].Frames;
               Int32 RawScaleFactor = UnitInfo.RawFramesTotal / FramesTotal;

               UnitInfo.FramesComplete = UnitInfo.RawFramesComplete / RawScaleFactor;
               UnitInfo.PercentComplete = UnitInfo.FramesComplete * 100 / FramesTotal;
               UnitInfo.TimePerFrame = new TimeSpan(0, 0, Convert.ToInt32(UnitInfo.RawTimePerSection));

               UnitInfo.UPD = 86400 / (UnitInfo.TimePerFrame.TotalSeconds * FramesTotal);
               UnitInfo.PPD = Math.Round(UnitInfo.UPD * ProteinCollection.Instance[UnitInfo.ProjectID].Credit, 5);
               UnitInfo.ETA = new TimeSpan((100 - UnitInfo.PercentComplete) * UnitInfo.TimePerFrame.Ticks);
            }
            catch (Exception ex)
            {
               Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            }
         }
      }
      
      /// <summary>
      /// Clear the user specified values that define this instance
      /// </summary>
      private void ClearInstanceValues(object sender, EventArgs e)
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
      /// Process the cached log files that exist on this machine
      /// </summary>
      public void ProcessExisting()
      {
         DateTime Start = Debug.ExecStart;

         Clear();

         Boolean allGood;

         LogParser lp = new LogParser();
         if (lp.ParseUnitInfo(System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName), this) == false)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} ({1}) UnitInfo parse failed.", Debug.FunctionName, InstanceName));
         }
         allGood = lp.ParseFAHLog(System.IO.Path.Combine(BaseDirectory, CachedFAHLogName), this);

         if (allGood)
         {
            SetTimeBasedValues();
         }
         else
         {
            // Clear the time based values when log parsing fails
            ClearTimeBasedValues();
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

         if (success)
         {
            ProcessExisting();
         }
         else
         {
            // Clear the time based values when log retrieval fails
            ClearTimeBasedValues();
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
            try
            {
               Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                       String.Format("{0} ({1}) FAHLog copy (start).", Debug.FunctionName,
                                                     InstanceName));
               if (fiLog.Exists)
               {
                  if (fiCachedLog.Exists == false || fiLog.Length != fiCachedLog.Length)
                  {
                     fiLog.CopyTo(FAHLog_txt, true);
                     Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                             String.Format("{0} ({1}) FAHLog copy (success).", Debug.FunctionName,
                                                           InstanceName));
                  }
                  else
                  {
                     Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                             String.Format("{0} ({1}) FAHLog copy (file has not changed).", Debug.FunctionName,
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
            }
            catch (Exception ex)
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName, ex.Message));

               return false;
            }

            // Retrieve UnitInfo.txt (or equivalent)
            FileInfo fiUI = new FileInfo(System.IO.Path.Combine(Path, RemoteUnitInfoFilename));
            string UnitInfo_txt = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);
            try
            {
               Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                       String.Format("{0} ({1}) UnitInfo copy (start).", Debug.FunctionName, InstanceName));
               if (fiUI.Exists)
               {
                  // If file size is too large, do not copy it and delete the current cached copy - Issue 2
                  if (fiUI.Length < 1024)
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
            }
            catch (Exception ex)
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName, ex.Message));
               return false;
            }

            LastRetrievalTime = DateTime.Now;
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

            PreferenceSet Prefs = PreferenceSet.Instance;

            // Download FAHlog.txt
            WebRequest httpc1 = WebRequest.Create(Path + "/" + RemoteFAHLogFilename);
            httpc1.Credentials = new NetworkCredential(Username, Password);
            httpc1.Method = WebRequestMethods.Http.Get;
            httpc1.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
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

            try
            {
               WebResponse r1 = httpc1.GetResponse();
               String FAHLog_txt = System.IO.Path.Combine(BaseDirectory, CachedFAHLogName);
               StreamWriter sw1 = new StreamWriter(FAHLog_txt, false);
               StreamReader sr1 = new StreamReader(r1.GetResponseStream(), Encoding.ASCII);

               sw1.Write(sr1.ReadToEnd());
               sw1.Flush();
               sw1.Close();
               sr1.Close();
            }
            catch (Exception ex)
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName,
                                                     ex.Message));
               return false;
            }

            // Download unitinfo.txt
            WebRequest httpc2 = WebRequest.Create(Path + "/" + RemoteUnitInfoFilename);
            httpc2.Credentials = new NetworkCredential(Username, Password);
            httpc2.Method = WebRequestMethods.Http.Get;
            httpc2.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            if (Prefs.UseProxy)
            {
               httpc2.Proxy = new WebProxy(Prefs.ProxyServer, Prefs.ProxyPort);
               if (Prefs.UseProxyAuth)
               {
                  httpc2.Proxy.Credentials = new NetworkCredential(Prefs.ProxyUser, Prefs.ProxyPass);
               }
            }
            else
            {
               httpc2.Proxy = null;
            }

            try
            {
               WebResponse r2 = httpc2.GetResponse();
               String UnitInfo_txt = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);
               StreamWriter sw2 = new StreamWriter(UnitInfo_txt, false);
               StreamReader sr2 = new StreamReader(r2.GetResponseStream(), Encoding.ASCII);
               
               //TODO: (HTTP) If this download begins getting too big we likely have a bad UnitInfo.txt file. - Issue 2
               //      Stop the download and delete the current cached copy, as done for Path Instances.

               sw2.Write(sr2.ReadToEnd());
               sw2.Flush();
               sw2.Close();
               sr2.Close();
            }
            catch (Exception ex)
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName,
                                                     ex.Message));
               return false;
            }

            LastRetrievalTime = DateTime.Now;
         }
         finally
         {
            _RetrievalInProgress = false;
         }

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
         
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

            PreferenceSet Prefs = PreferenceSet.Instance;

            // Download FAHlog.txt
            FtpWebRequest ftpc1 = (FtpWebRequest) FtpWebRequest.Create("ftp://" + Server + Path + RemoteFAHLogFilename);
            ftpc1.Method = WebRequestMethods.Ftp.DownloadFile;
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
            ftpc1.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
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

            FtpWebResponse ftpr1;
            try
            {
               ftpr1 = (FtpWebResponse) ftpc1.GetResponse();
            }
            catch (Exception ex)
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName,
                                                     ex.Message));
               return false;
            }
            String FAHLog_txt = System.IO.Path.Combine(BaseDirectory, CachedFAHLogName);
            StreamWriter sw1 = new StreamWriter(FAHLog_txt, false);
            StreamReader sr1 = new StreamReader(ftpr1.GetResponseStream(), Encoding.ASCII);

            sw1.Write(sr1.ReadToEnd());
            sw1.Flush();
            sw1.Close();
            sr1.Close();

            // Download unitinfo.txt
            FtpWebRequest ftpc2 =
               (FtpWebRequest) FtpWebRequest.Create("ftp://" + Server + Path + RemoteUnitInfoFilename);
            if ((Username != "") && (Username != null))
            {
               if (Username.Contains("\\"))
               {
                  String[] UserParts = Username.Split('\\');
                  ftpc2.Credentials = new NetworkCredential(UserParts[1], Password, UserParts[0]);
               }
               else
               {
                  ftpc2.Credentials = new NetworkCredential(Username, Password);
               }
            }
            ftpc2.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpc2.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            if (Prefs.UseProxy)
            {
               ftpc2.Proxy = new WebProxy(Prefs.ProxyServer, Prefs.ProxyPort);
               if (Prefs.UseProxyAuth)
               {
                  ftpc2.Proxy.Credentials = new NetworkCredential(Prefs.ProxyUser, Prefs.ProxyPass);
               }
            }
            else
            {
               ftpc2.Proxy = null;
            }

            FtpWebResponse ftpr2;
            try
            {
               ftpr2 = (FtpWebResponse) ftpc2.GetResponse();
            }
            catch (Exception ex)
            {
               Status = ClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, InstanceName,
                                                     ex.Message));
               return false;
            }
            String UnitInfo_txt = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);
            StreamWriter sw2 = new StreamWriter(UnitInfo_txt, false);
            StreamReader sr2 = new StreamReader(ftpr2.GetResponseStream(), Encoding.ASCII);

            //TODO: (FTP) If this download begins getting too big we likely have a bad UnitInfo.txt file. - Issue 2
            //      Stop the download and delete the current cached copy, as done for Path Instances.

            sw2.Write(sr2.ReadToEnd());
            sw2.Flush();
            sw2.Close();
            sr2.Close();

            LastRetrievalTime = DateTime.Now;
         }
         finally
         {
            _RetrievalInProgress = false;
         }

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
         
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
   }
}
