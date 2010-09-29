/*
 * HFM.NET - Client Instance Settings Class
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

using ProtoBuf;

using HFM.Framework;

namespace HFM.Instances
{
   [ProtoContract]
   public class ClientInstanceSettings : IClientInstanceSettings
   {
      public string ImportError { get; set; }
   
      public bool Error
      {
         get
         {
            return (InstanceNameError ||
                    RemoteExternalFilenameError ||
                    ClientProcessorMegahertzError ||
                    RemoteFAHLogFilenameError ||
                    RemoteUnitInfoFilenameError ||
                    RemoteQueueFilenameError ||
                    PathError ||
                    ServerError ||
                    CredentialsError ||
                    ClientTimeOffsetError);
         }
      }

      private InstanceType _instanceHostType;
      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      [ProtoMember(1)]
      public InstanceType InstanceHostType
      {
         get { return _instanceHostType; }
         set
         {
            if (_instanceHostType != value)
            {
               _instanceHostType = value;
               ClearAccessSettings();
               OnPropertyChanged("Dummy");
            }
         }
      }

      private string _instanceName;
      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      [ProtoMember(2)]
      public string InstanceName
      {
         get { return _instanceName; }
         set
         {
            if (_instanceName != value)
            {
               _instanceName = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("InstanceName");
            }
         }
      }
      
      public bool InstanceNameEmpty
      {
         get { return InstanceName.Length == 0; }
      }
      
      public bool InstanceNameError
      {
         get { return !StringOps.ValidateInstanceName(InstanceName); }
      }

      [ProtoMember(14)]
      public bool ExternalInstance { get; set; }

      private string _remoteExternalFilename;
      /// <summary>
      /// External data file name
      /// </summary>
      [ProtoMember(15)]
      public string RemoteExternalFilename
      {
         get { return _remoteExternalFilename; }
         set
         {
            if (_remoteExternalFilename != value)
            {
               _remoteExternalFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteExternalFilename");
            }
         }
      }

      public bool RemoteExternalFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteExternalFilename); }
      }

      private Int32 _clientProcessorMegahertz;
      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      [ProtoMember(3)]
      public Int32 ClientProcessorMegahertz
      {
         get { return _clientProcessorMegahertz; }
         set
         {
            if (_clientProcessorMegahertz != value)
            {
               _clientProcessorMegahertz = value;
               OnPropertyChanged("ClientProcessorMegahertz");
            }
         }
      }
      
      public bool ClientProcessorMegahertzError
      {
         get { return ClientProcessorMegahertz < 1; }
      }

      private string _remoteFAHLogFilename;
      /// <summary>
      /// Remote client log file name
      /// </summary>
      [ProtoMember(4)]
      public string RemoteFAHLogFilename
      {
         get { return _remoteFAHLogFilename; }
         set
         {
            if (_remoteFAHLogFilename != value)
            {
               _remoteFAHLogFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteFAHLogFilename");
            }
         }
      }
      
      public bool RemoteFAHLogFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteFAHLogFilename); }
      }

      private string _remoteUnitInfoFilename;
      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      [ProtoMember(5)]
      public string RemoteUnitInfoFilename
      {
         get { return _remoteUnitInfoFilename; }
         set
         {
            if (_remoteUnitInfoFilename != value)
            {
               _remoteUnitInfoFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteUnitInfoFilename");
            }
         }
      }

      public bool RemoteUnitInfoFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteUnitInfoFilename); }
      }

      private string _remoteQueueFilename;
      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      [ProtoMember(6)]
      public string RemoteQueueFilename
      {
         get { return _remoteQueueFilename; }
         set
         {
            if (_remoteQueueFilename != value)
            {
               _remoteQueueFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteQueueFilename");
            }
         }
      }

      public bool RemoteQueueFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteQueueFilename); }
      }

      private string _path;
      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      [ProtoMember(7)]
      public string Path
      {
         get { return _path; }
         set
         {
            if (_path != value)
            {
               _path = value == null ? String.Empty : value.Trim();
               _path = StripFahClientFileNames(value);
               switch (InstanceHostType)
               {
                  case InstanceType.PathInstance:
                     if (_path.Length > 2 &&
                         _path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) == false)
                     {
                        _path = String.Concat(_path, System.IO.Path.DirectorySeparatorChar);
                     }
                     break;
                  case InstanceType.HttpInstance:
                  case InstanceType.FtpInstance:
                     if (_path.EndsWith("/") == false)
                     {
                        _path = String.Concat(_path, "/");
                     }
                     break;
               }
               OnPropertyChanged("Path");
            }
         }
      }

      private static string StripFahClientFileNames(string value)
      {
         if (value.ToUpperInvariant().EndsWith("FAHLOG.TXT"))
         {
            return value.Substring(0, value.Length - 10);
         }
         if (value.ToUpperInvariant().EndsWith("UNITINFO.TXT"))
         {
            return value.Substring(0, value.Length - 12);
         }
         if (value.ToUpperInvariant().EndsWith("QUEUE.DAT"))
         {
            return value.Substring(0, value.Length - 9);
         }

         return value;
      }
      
      public bool PathEmpty
      {
         get { return Path.Length == 0; }
      }
      
      public bool PathError
      {
         get
         {
            switch (InstanceHostType)
            {
               case InstanceType.PathInstance:
                  if (Path.Length < 2)
                  {
                     return true;
                  }
                  return !StringOps.ValidatePathInstancePath(Path);
               case InstanceType.HttpInstance:
                  return !StringOps.ValidateHttpUrl(Path);
               case InstanceType.FtpInstance:
                  if (Path == "/")
                  {
                     return false;
                  }
                  return !StringOps.ValidateFtpPath(Path);
               default:
                  return true;
            }
         }
      }

      private string _server;
      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      [ProtoMember(8)]
      public string Server
      {
         get { return _server; }
         set
         {
            if (_server != value)
            {
               _server = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Server");
            }
         }
      }

      public bool ServerError
      {
         get
         {
            switch (InstanceHostType)
            {
               case InstanceType.PathInstance:
               case InstanceType.HttpInstance:
                  return false;
               case InstanceType.FtpInstance:
                  return !StringOps.ValidateServerName(Server);
               default:
                  return true;
            }
         }
      }

      private string _username;
      /// <summary>
      /// Username on remote server
      /// </summary>
      [ProtoMember(9)]
      public string Username
      {
         get { return _username; } 
         set
         {
            if (_username != value)
            {
               _username = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Password");
               OnPropertyChanged("Username");
            }
         }
      }
      
      public bool UsernameError
      {
         get { return CredentialsError; }
      }

      private string _password;
      /// <summary>
      /// Password on remote server
      /// </summary>
      [ProtoMember(10)]
      public string Password
      {
         get { return _password; }
         set
         {
            if (_password != value)
            {
               _password = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Username");
               OnPropertyChanged("Password");
            }
         }
      }

      public bool PasswordError
      {
         get { return CredentialsError; }
      }

      public bool CredentialsError
      {
         get
         {
            switch (InstanceHostType)
            {
               case InstanceType.PathInstance:
                  return false;
               case InstanceType.HttpInstance:
                  return !ValidateCredentials(false);
               case InstanceType.FtpInstance:
                  return !ValidateCredentials(true);
               default:
                  return true;
            }
         }
      }
      
      private bool ValidateCredentials(bool throwOnEmpty)
      {
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(Username, Password, throwOnEmpty);
            CredentialsErrorMessage = String.Empty;
            return true;
         }
         catch (ArgumentException ex)
         {
            CredentialsErrorMessage = ex.Message;
            return false;
         }
      }

      public string CredentialsErrorMessage { get; private set; }

      private FtpType _ftpMode;
      /// <summary>
      /// Specifies the FTP Communication Mode for this client
      /// </summary>
      [ProtoMember(11)]
      public FtpType FtpMode
      {
         get { return _ftpMode; }
         set
         {
            if (_ftpMode != value)
            {
               _ftpMode = value;
               OnPropertyChanged("FtpMode");
            }
         }
      }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      [ProtoMember(12)]
      public bool ClientIsOnVirtualMachine { get; set; }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      [ProtoMember(13)]
      public Int32 ClientTimeOffset { get; set; }
      
      public bool ClientTimeOffsetError
      {
         get
         {
            return ClientTimeOffset < Constants.MinOffsetMinutes ||
                   ClientTimeOffset > Constants.MaxOffsetMinutes;
         }
      }
      
      public string Dummy
      {
         get { return String.Empty; }
      }

      public ClientInstanceSettings()
         : this(InstanceType.PathInstance)
      {
         
      }
      
      public ClientInstanceSettings(InstanceType hostType)
      {
         ImportError = String.Empty;
         InstanceHostType = hostType;
         InstanceName = String.Empty;
         ExternalInstance = false;
         RemoteExternalFilename = Constants.LocalExternal;
         ClientProcessorMegahertz = 1;
         RemoteFAHLogFilename = Constants.LocalFahLog;
         RemoteUnitInfoFilename = Constants.LocalUnitInfo;
         RemoteQueueFilename = Constants.LocalQueue;
         ClearAccessSettings();
         ClientIsOnVirtualMachine = false;
         ClientTimeOffset = 0;
      }

      private void ClearAccessSettings()
      {
         _path = String.Empty;
         _server = String.Empty;
         _username = String.Empty;
         _password = String.Empty;
         CredentialsErrorMessage = String.Empty;
         FtpMode = FtpType.Passive;
      }

      private ClientInstanceSettings(IClientInstanceSettings settings)
      {
         LoadSettings(settings);
      }

      public void LoadSettings(IClientInstanceSettings settings)
      {
         InstanceHostType = settings.InstanceHostType;
         InstanceName = settings.InstanceName;
         ExternalInstance = settings.ExternalInstance;
         RemoteExternalFilename = settings.RemoteExternalFilename;
         ClientProcessorMegahertz = settings.ClientProcessorMegahertz;
         RemoteFAHLogFilename = settings.RemoteFAHLogFilename;
         RemoteUnitInfoFilename = settings.RemoteUnitInfoFilename;
         RemoteQueueFilename = settings.RemoteQueueFilename;
         Path = settings.Path;
         Server = settings.Server;
         Username = settings.Username;
         Password = settings.Password;
         FtpMode = settings.FtpMode;
         ClientIsOnVirtualMachine = settings.ClientIsOnVirtualMachine;
         ClientTimeOffset = settings.ClientTimeOffset;
      }
      
      public ReadOnlyCollection<string> CleanupSettings()
      {
         var warnings = new List<string>();
      
         if (InstanceNameError)
         {
            // Remove illegal characters
            warnings.Add(String.Format(CultureInfo.CurrentCulture,
                                       "Instance Name '{0}' contained invalid characters and was cleaned.", InstanceName));
            InstanceName = StringOps.CleanInstanceName(InstanceName);
         }

         if (ClientProcessorMegahertzError)
         {
            warnings.Add("Client MHz is less than 1, defaulting to 1 MHz.");
            ClientProcessorMegahertz = 1;
         }
         
         if (RemoteFAHLogFilenameError)
         {
            warnings.Add("No remote FAHlog.txt filename, loading default.");
            RemoteFAHLogFilename = Constants.LocalFahLog;
         }

         if (RemoteUnitInfoFilenameError)
         {
            warnings.Add("No remote unitinfo.txt filename, loading default.");
            RemoteUnitInfoFilename = Constants.LocalUnitInfo;
         }

         if (RemoteQueueFilenameError)
         {
            warnings.Add("No remote queue.dat filename, loading default.");
            RemoteQueueFilename = Constants.LocalQueue;
         }
         
         if (ClientTimeOffsetError)
         {
            warnings.Add("Client time offset is out of range, defaulting to 0.");
            ClientTimeOffset = 0;
         }

         return warnings.AsReadOnly();
      }

      public IClientInstanceSettings Clone()
      {
         return new ClientInstanceSettings(this);
      }

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;
      
      private void OnPropertyChanged(string propertyName)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      #endregion

      #region Cached Log File Name Properties

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      public string CachedFahLogName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Constants.LocalFahLog); }
      }

      /// <summary>
      /// Cached UnitInfo Filename for this instance
      /// </summary>
      public string CachedUnitInfoName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Constants.LocalUnitInfo); }
      }

      /// <summary>
      /// Cached Queue Filename for this instance
      /// </summary>
      public string CachedQueueName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Constants.LocalQueue); }
      }

      /// <summary>
      /// Cached External Filename for this instance
      /// </summary>
      public string CachedExternalName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Constants.LocalExternal); }
      }

      #endregion
   }
}