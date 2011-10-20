/*
 * HFM.NET - Client Instance Settings Model
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
using System.ComponentModel;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms.Models
{
   public interface IClientInstanceSettingsModel : INotifyPropertyChanged
   {
      bool Error { get; }

      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      InstanceType InstanceHostType { get; set; }

      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      string InstanceName { get; set; }

      bool InstanceNameEmpty { get; }
      
      bool InstanceNameError { get; }

      bool ExternalInstance { get; }

      /// <summary>
      /// External data file name
      /// </summary>
      string RemoteExternalFilename { get; set; }

      bool RemoteExternalFilenameError { get; }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      int ClientProcessorMegahertz { get; set; }

      bool ClientProcessorMegahertzError { get; }

      /// <summary>
      /// Remote client log file name
      /// </summary>
      string RemoteFAHLogFilename { get; set; }

      bool RemoteFAHLogFilenameError { get; }

      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      string RemoteUnitInfoFilename { get; set; }

      bool RemoteUnitInfoFilenameError { get; }

      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      string RemoteQueueFilename { get; set; }

      bool RemoteQueueFilenameError { get; }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      string Path { get; set; }

      bool PathEmpty { get; }
      
      bool PathError { get; }

      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      string Server { get; set; }

      bool ServerError { get; }

      /// <summary>
      /// Username on remote server
      /// </summary>
      string Username { get; set; }

      bool UsernameError { get; }

      /// <summary>
      /// Password on remote server
      /// </summary>
      string Password { get; set; }

      bool PasswordError { get; }
      
      bool CredentialsError { get; }
      
      string CredentialsErrorMessage { get; }

      /// <summary>
      /// Specifies the FTP Communication Mode for this client
      /// </summary>
      FtpType FtpMode { get; set; }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      bool ClientIsOnVirtualMachine { get; set; }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      int ClientTimeOffset { get; set; }

      bool ClientTimeOffsetError { get; }
      
      string Dummy { get; }
      
      ClientInstanceSettings Settings { get; }
   }

   internal sealed class ClientInstanceSettingsModel : IClientInstanceSettingsModel
   {
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

      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      public InstanceType InstanceHostType
      {
         get { return _settings.InstanceHostType; }
         set
         {
            if (_settings.InstanceHostType != value)
            {
               _settings.InstanceHostType = value;
               ClearAccessSettings();
               OnPropertyChanged("Dummy");
            }
         }
      }

      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      public string InstanceName
      {
         get { return _settings.InstanceName; }
         set
         {
            if (_settings.InstanceName != value)
            {
               _settings.InstanceName = value == null ? String.Empty : value.Trim();
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

      public bool ExternalInstance
      {
         get { return _settings.ExternalInstance; }
         //private set { _settings.ExternalInstance = value; }
      }

      /// <summary>
      /// External data file name
      /// </summary>
      public string RemoteExternalFilename
      {
         get { return _settings.RemoteExternalFilename; }
         set
         {
            if (_settings.RemoteExternalFilename != value)
            {
               _settings.RemoteExternalFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteExternalFilename");
            }
         }
      }

      public bool RemoteExternalFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteExternalFilename); }
      }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      public int ClientProcessorMegahertz
      {
         get { return _settings.ClientProcessorMegahertz; }
         set
         {
            if (_settings.ClientProcessorMegahertz != value)
            {
               _settings.ClientProcessorMegahertz = value;
               OnPropertyChanged("ClientProcessorMegahertz");
            }
         }
      }
      
      public bool ClientProcessorMegahertzError
      {
         get { return ClientProcessorMegahertz < 1; }
      }

      /// <summary>
      /// Remote client log file name
      /// </summary>
      public string RemoteFAHLogFilename
      {
         get { return _settings.RemoteFAHLogFilename; }
         set
         {
            if (_settings.RemoteFAHLogFilename != value)
            {
               _settings.RemoteFAHLogFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteFAHLogFilename");
            }
         }
      }
      
      public bool RemoteFAHLogFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteFAHLogFilename); }
      }

      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      public string RemoteUnitInfoFilename
      {
         get { return _settings.RemoteUnitInfoFilename; }
         set
         {
            if (_settings.RemoteUnitInfoFilename != value)
            {
               _settings.RemoteUnitInfoFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteUnitInfoFilename");
            }
         }
      }

      public bool RemoteUnitInfoFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteUnitInfoFilename); }
      }

      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      public string RemoteQueueFilename
      {
         get { return _settings.RemoteQueueFilename; }
         set
         {
            if (_settings.RemoteQueueFilename != value)
            {
               _settings.RemoteQueueFilename = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("RemoteQueueFilename");
            }
         }
      }

      public bool RemoteQueueFilenameError
      {
         get { return !StringOps.ValidateFileName(RemoteQueueFilename); }
      }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      public string Path
      {
         get { return _settings.Path; }
         set
         {
            if (_settings.Path != value)
            {
               string path = value == null ? String.Empty : value.Trim();
               path = StripFahClientFileNames(path);
               _settings.Path = path;
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

      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      public string Server
      {
         get { return _settings.Server; }
         set
         {
            if (_settings.Server != value)
            {
               _settings.Server = value == null ? String.Empty : value.Trim();
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

      /// <summary>
      /// Username on remote server
      /// </summary>
      public string Username
      {
         get { return _settings.Username; } 
         set
         {
            if (_settings.Username != value)
            {
               _settings.Username = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Password");
               OnPropertyChanged("Username");
            }
         }
      }
      
      public bool UsernameError
      {
         get { return CredentialsError; }
      }

      /// <summary>
      /// Password on remote server
      /// </summary>
      public string Password
      {
         get { return _settings.Password; }
         set
         {
            if (_settings.Password != value)
            {
               _settings.Password = value == null ? String.Empty : value.Trim();
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

      /// <summary>
      /// Specifies the FTP Communication Mode for this client
      /// </summary>
      public FtpType FtpMode
      {
         get { return _settings.FtpMode; }
         set
         {
            if (_settings.FtpMode != value)
            {
               _settings.FtpMode = value;
               OnPropertyChanged("FtpMode");
            }
         }
      }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      public bool ClientIsOnVirtualMachine
      {
         get { return _settings.ClientIsOnVirtualMachine; } 
         set { _settings.ClientIsOnVirtualMachine = value; }
      }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      public int ClientTimeOffset
      {
         get { return _settings.ClientTimeOffset; }
         set { _settings.ClientTimeOffset = value; }
      }
      
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

      private readonly ClientInstanceSettings _settings;
      
      public ClientInstanceSettings Settings
      {
         get { return _settings; }
      }

      public ClientInstanceSettingsModel()
      {
         _settings = new ClientInstanceSettings();
      }

      public ClientInstanceSettingsModel(ClientInstanceSettings settings)
      {
         _settings = settings;
      }

      private void ClearAccessSettings()
      {
         Path = String.Empty;
         Server = String.Empty;
         Username = String.Empty;
         Password = String.Empty;
         CredentialsErrorMessage = String.Empty;
         FtpMode = FtpType.Passive;
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
   }
}