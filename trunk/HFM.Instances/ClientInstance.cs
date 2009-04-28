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
   public enum InstanceType
   {
      PathInstance,
      FTPInstance,
      HTTPInstance
   }

   public class ClientInstance
   {
      #region Constants
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
      
      private const string LocalFAHLog = "FAHLog.txt";
      private const string LocalUnitInfo = "UnitInfo.txt";
      #endregion
      
      public event EventHandler InstanceHostTypeChanged;

      #region Public Properties and Related Private Variables
      private string _InstanceName;
      public string Name
      {
         get { return _InstanceName; }
         set { _InstanceName = value; }
      }

      private Protein _CurrentProtein;
      public Protein CurrentProtein
      {
         get { return _CurrentProtein; }
         set { _CurrentProtein = value; }
      }

      private readonly UnitInfo _UnitInfo;
      public UnitInfo UnitInfo
      {
         get { return _UnitInfo; }
      }

      protected DateTime _LastRetrieved = DateTime.MinValue;
      public DateTime LastRetrievalTime
      {
         get { return _LastRetrieved; }
      }

      private Int32 _TotalUnits;
      public Int32 TotalUnits
      {
         get { return _TotalUnits; }
         set { _TotalUnits = value; }
      }

      private string _RemoteFAHLogFilename = LocalFAHLog;
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

      private string _RemoteUnitInfoFilename = LocalUnitInfo;
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

      public string BaseDirectory
      {
         get { return System.IO.Path.Combine(PreferenceSet.Instance.AppDataPath, PreferenceSet.Instance.CacheFolder); }
      }

      private readonly List<string> _CurrentLogText = new List<string>();
      public List<string> CurrentLogText
      {
         get { return _CurrentLogText; }
      }

      private bool _RetrievalInProgress = false;
      protected bool RetrievalInProgress
      {
         get { return _RetrievalInProgress; }
         set { _RetrievalInProgress = value; }
      }

      private Int32 _NumberOfCompletedUnitsSinceLastStart;
      public Int32 NumberOfCompletedUnitsSinceLastStart
      {
         get { return _NumberOfCompletedUnitsSinceLastStart; }
         set { _NumberOfCompletedUnitsSinceLastStart = value; }
      }

      private Int32 _NumberOfFailedUnitsSinceLastStart;
      public Int32 NumberOfFailedUnitsSinceLastStart
      {
         get { return _NumberOfFailedUnitsSinceLastStart; }
         set { _NumberOfFailedUnitsSinceLastStart = value; }
      }
      
      private Int32 _ClientProcessorMegahertz;
      public Int32 ClientProcessorMegahertz
      {
         get { return _ClientProcessorMegahertz; }
         set { _ClientProcessorMegahertz = value; }
      }
      
      private bool _ClientIsOnVirtualMachine;
      public bool ClientIsOnVirtualMachine
      {
         get { return _ClientIsOnVirtualMachine; }
         set { _ClientIsOnVirtualMachine = value; }
      }

      private Int32 _ClientTimeOffset;
      public Int32 ClientTimeOffset
      {
         get { return _ClientTimeOffset; }
         set { _ClientTimeOffset = value; }
      }

      private InstanceType _InstanceHostType;
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
      /// Private variable storing location of log files for this instance
      /// </summary>
      private string _Path;
      /// <summary>
      /// Public property storing location of log files for this instance
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
      /// FTP username on remote server
      /// </summary>
      private string _Username;
      public string Username
      {
         get { return _Username; }
         set { _Username = value; }
      }

      /// <summary>
      /// FTP password on remote server
      /// </summary>
      private string _Password;
      public string Password
      {
         get { return _Password; }
         set { _Password = value; }
      }

      public string CachedFAHLogName
      {
         get { return String.Format("{0}-{1}", Name, LocalFAHLog); }
      }

      public string CachedUnitInfoName
      {
         get { return String.Format("{0}-{1}", Name, LocalUnitInfo); }
      }
      #endregion

      #region Protected Variables
      protected ProteinCollection _Proteins;
      #endregion

      #region Constructor
      /// <summary>
      /// Class constructor
      /// </summary>
      public ClientInstance(InstanceType type)
      {
         InstanceHostTypeChanged += ClearInstanceValues;
         _InstanceHostType = type;
      
         _UnitInfo = new UnitInfo();
         Clear();
         
         _Proteins = ProteinCollection.Instance;
      }
      #endregion

      #region Data Processing
      private void Clear()
      {
         // clear the instance log holder
         CurrentLogText.Clear();
         // reset completed and failed values
         NumberOfCompletedUnitsSinceLastStart = 0;
         NumberOfFailedUnitsSinceLastStart = 0;
      
         _UnitInfo.ClientType = eClientType.Unknown;
         _UnitInfo.CoreVersion = String.Empty;
         _UnitInfo.DownloadTime = DateTime.MinValue;
         _UnitInfo.DueTime = DateTime.MinValue;
         _UnitInfo.FramesComplete = 0;
         _UnitInfo.PercentComplete = 0;
         _UnitInfo.ProjectID = 0;
         _UnitInfo.ProjectRun = 0;
         _UnitInfo.ProjectClone = 0;
         _UnitInfo.ProjectGen = 0;
         _UnitInfo.ProteinName = String.Empty;
         _UnitInfo.ProteinTag = String.Empty;
         _UnitInfo.RawFramesComplete = 0;
         _UnitInfo.RawFramesTotal = 0;
         //_UnitInfo.Status = eClientStatus.Unknown;
         _UnitInfo.TimeOfLastFrame = TimeSpan.Zero;
         
         ClearTimeBasedValues();

         _CurrentProtein = new Protein();
         _CurrentProtein.Contact = "Unassigned Contact";
         _CurrentProtein.Core = "Unassigned Core";
         _CurrentProtein.Credit = 0;
         _CurrentProtein.Description = "Unassigned Description";
         _CurrentProtein.Frames = 100;
         _CurrentProtein.MaxDays = 0;
         _CurrentProtein.NumAtoms = 0;
         _CurrentProtein.PreferredDays = 0;
         _CurrentProtein.ProjectNumber = 0;
         _CurrentProtein.ServerIP = "0.0.0.0";
         _CurrentProtein.WorkUnitName = "Unassigned Protein";
      }
      
      private void ClearTimeBasedValues()
      {
         _UnitInfo.ETA = TimeSpan.Zero;
         _UnitInfo.PPD = 0.0;
         _UnitInfo.RawTimePerLastSection = 0;
         _UnitInfo.RawTimePerThreeSections = 0;
         _UnitInfo.TimePerFrame = TimeSpan.Zero;
         _UnitInfo.UPD = 0.0;
      }
      
      private void ClearInstanceValues(object sender, EventArgs e)
      {
         Name = String.Empty;
         RemoteFAHLogFilename = String.Empty;
         RemoteUnitInfoFilename = String.Empty;
         ClientProcessorMegahertz = 1;
         ClientIsOnVirtualMachine = false;
         ClientTimeOffset = 0;
         
         Path = String.Empty;
         Server = String.Empty;
         Username = String.Empty;
         Password = String.Empty;
      }

      public void ProcessExisting()
      {
         DateTime Start = Debug.ExecStart;

         Clear();

         Boolean allGood;

         LogParser lp = new LogParser();
         allGood = lp.ParseUnitInfo(System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName), this) &&
                   lp.ParseFAHLog(System.IO.Path.Combine(BaseDirectory, CachedFAHLogName), this);

         if (allGood)
         {
            UnitInfo.SetTimeBasedValues(Name);
         }
         else
         {
            // Clear the time based values when log parsing fails
            ClearTimeBasedValues();
         }
         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Name, Debug.GetExecTime(Start)));
      }
      
      public void SetTimeBasedValues()
      {
         UnitInfo.SetTimeBasedValues(Name);
      }

      /// <summary>
      /// Virtual method - override to define appropriate retrieval semantics for
      /// the subclass. Note: Call base class method from override to correctly
      /// update internal structures.
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
      }

      /// <summary>
      /// Retrieve the instance's log files
      /// </summary>
      public bool RetrievePathInstance()
      {
         if (RetrievalInProgress)
         {
            return false;
         }

         DateTime Start = Debug.ExecStart;

         try
         {
            RetrievalInProgress = true;

            FileInfo fiLog = new FileInfo(System.IO.Path.Combine(Path, RemoteFAHLogFilename));
            string FAHLog_txt = System.IO.Path.Combine(BaseDirectory, CachedFAHLogName);
            try
            {
               if (fiLog.Exists)
               {
                  Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) FAHLog copy (start).", Debug.FunctionName, Name));
                  fiLog.CopyTo(FAHLog_txt, true);
                  Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) FAHLog copy (success).", Debug.FunctionName, Name));
               }
               else
               {
                  UnitInfo.Status = eClientStatus.Offline;
                  Debug.WriteToHfmConsole(TraceLevel.Error,
                                          String.Format("{0} ({1}) The path {2} is inaccessible.", Debug.FunctionName, Name, fiLog.FullName));
                  return false;
               }
            }
            catch (Exception ex)
            {
               UnitInfo.Status = eClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, Name, ex.Message));

               return false;
            }

            // Retrieve UnitInfo.txt (or equivalent)
            FileInfo fiUI = new FileInfo(System.IO.Path.Combine(Path, RemoteUnitInfoFilename));
            string UnitInfo_txt = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);
            try
            {
               if (fiUI.Exists)
               {
                  Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) UnitInfo copy (start).", Debug.FunctionName, Name));
                  fiUI.CopyTo(UnitInfo_txt, true);
                  Debug.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format("{0} ({1}) UnitInfo copy (success).", Debug.FunctionName, Name));
               }
               else
               {
                  UnitInfo.Status = eClientStatus.Offline;
                  Debug.WriteToHfmConsole(TraceLevel.Error,
                                          String.Format("{0} ({1}) The path {2} is inaccessible.", Debug.FunctionName, Name, fiUI.FullName));
                  return false;
               }
            }
            catch (Exception ex)
            {
               UnitInfo.Status = eClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, Name, ex.Message));
               return false;
            }

            _LastRetrieved = DateTime.Now;
         }
         finally
         {
            RetrievalInProgress = false;
         }
         
         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Name, Debug.GetExecTime(Start)));

         return true;
      }

      /// <summary>
      /// 
      /// </summary>
      public bool RetrieveHTTPInstance()
      {
         if (RetrievalInProgress)
         {
            // Don't allow this to fire more than once at a time
            return false;
         }

         DateTime Start = Debug.ExecStart;

         try
         {
            RetrievalInProgress = true;

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
               UnitInfo.Status = eClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, Name,
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

               sw2.Write(sr2.ReadToEnd());
               sw2.Flush();
               sw2.Close();
               sr2.Close();
            }
            catch (Exception ex)
            {
               UnitInfo.Status = eClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, Name,
                                                     ex.Message));
               return false;
            }

            //TODO: Check _LastRetrieved here.  Moved this in from the Base class.
            _LastRetrieved = DateTime.Now;
         }
         finally
         {
            RetrievalInProgress = false;
         }

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Name, Debug.GetExecTime(Start)));
         
         return true;
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured FTP location
      /// </summary>
      public bool RetrieveFTPInstance()
      {
         if (RetrievalInProgress)
         {
            // Don't allow this to fire more than once at a time
            return false;
         }

         DateTime Start = Debug.ExecStart;

         try
         {
            RetrievalInProgress = true;

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
               UnitInfo.Status = eClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, Name,
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
               UnitInfo.Status = eClientStatus.Offline;
               Debug.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} ({1}) threw exception {2}.", Debug.FunctionName, Name,
                                                     ex.Message));
               return false;
            }
            String UnitInfo_txt = System.IO.Path.Combine(BaseDirectory, CachedUnitInfoName);
            StreamWriter sw2 = new StreamWriter(UnitInfo_txt, false);
            StreamReader sr2 = new StreamReader(ftpr2.GetResponseStream(), Encoding.ASCII);

            sw2.Write(sr2.ReadToEnd());
            sw2.Flush();
            sw2.Close();
            sr2.Close();

            //TODO: Check _LastRetrieved here.  Moved this in from the Base class.
            _LastRetrieved = DateTime.Now;
         }
         finally
         {
            RetrievalInProgress = false;
         }

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Name, Debug.GetExecTime(Start)));
         
         return true;
      }
      #endregion
      
      protected void OnInstanceHostTypeChanged(EventArgs e)
      {
         if (InstanceHostTypeChanged != null)
         {
            InstanceHostTypeChanged(this, e);
         }
      }

      #region XML Serialization
      /// <summary>
      /// Virtual method - override to define appropriate save to XML semantics for
      /// the subclass. Note: Call base class method from override to correctly
      /// save common elements.
      /// </summary>
      public virtual System.Xml.XmlDocument ToXml()
      {
         DateTime Start = Debug.ExecStart;

         try
         {
            System.Xml.XmlDocument xmlData = new System.Xml.XmlDocument();

            System.Xml.XmlElement xmlRoot = xmlData.CreateElement(xmlNodeInstance);
            xmlRoot.SetAttribute(xmlAttrName, Name);
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
            
            Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
            return xmlData;
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
         }
         
         return null;
      }

      /// <summary>
      /// Virtual method - override to define appropriate load from XML semantics for
      /// the subclass. Note: Call base class method from override to correctly
      /// load common elements.
      /// </summary>
      /// <param name="xmlData">Xml containing the Instance configuration.
      /// Should be identical in structure and scope to the output of the ToXml
      /// method in the same class.</param>
      public virtual void FromXml(System.Xml.XmlNode xmlData)
      {
         DateTime Start = Debug.ExecStart;
         
         Name = xmlData.Attributes[xmlAttrName].ChildNodes[0].Value;
         try
         {
            RemoteFAHLogFilename = xmlData.SelectSingleNode(xmlNodeFAHLog).InnerText;
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Remote FAH Log Filename."));
            RemoteFAHLogFilename = LocalFAHLog;
         }
         
         try
         {
            RemoteUnitInfoFilename = xmlData.SelectSingleNode(xmlNodeUnitInfo).InnerText;
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Remote FAH UnitInfo Filename."));
            RemoteUnitInfoFilename = LocalUnitInfo;
         }
         
         try
         {
            ClientProcessorMegahertz = int.Parse(xmlData.SelectSingleNode(xmlNodeClientMHz).InnerText);
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client MHz, defaulting to 1 MHz."));
            ClientProcessorMegahertz = 1;
         }
         catch (FormatException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Could not parse Client MHz, defaulting to 1 MHz."));
            ClientProcessorMegahertz = 1;
         }

         try
         {
            ClientIsOnVirtualMachine = Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeClientVM).InnerText);
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client VM Flag, defaulting to false."));
            ClientIsOnVirtualMachine = false;
         }
         catch (InvalidCastException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Could not parse Client VM Flag, defaulting to false."));
            ClientIsOnVirtualMachine = false;
         }

         try
         {
            ClientTimeOffset = int.Parse(xmlData.SelectSingleNode(xmlNodeClientOffset).InnerText);
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client Time Offset, defaulting to 0."));
            ClientTimeOffset = 0;
         }
         catch (FormatException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Could not parse Client Time Offset, defaulting to 0."));
            ClientTimeOffset = 0;
         }

         try
         {
            Path = xmlData.SelectSingleNode(xmlPropPath).InnerText;
         }
         catch (NullReferenceException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client Path."));
         }

         try
         {
            Server = xmlData.SelectSingleNode(xmlPropServ).InnerText;
         }
         catch (NullReferenceException)
         {
            Server = String.Empty;
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Client Server."));
         }
         
         try
         {
            Username = xmlData.SelectSingleNode(xmlPropUser).InnerText;
         }
         catch (NullReferenceException)
         {
            Username = String.Empty;
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Server Username."));
         }
         
         try
         {
            Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
         }
         catch (NullReferenceException)
         {
            Password = String.Empty;
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, "Cannot load Server Password."));
         }
         
         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
      }
      #endregion
   }
}
