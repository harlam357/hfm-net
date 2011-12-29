/*
 * HFM.NET - Legacy Client Settings Model
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
using System.ComponentModel;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms.Models
{
   public sealed class LegacyClientSettingsModel : INotifyPropertyChanged
   {
      public bool Error
      {
         get
         {
            return (NameError ||
                    //ExternalFilenameError ||
                    ClientProcessorMegahertzError ||
                    FahLogFileNameError ||
                    UnitInfoFileNameError ||
                    QueueFileNameError ||
                    PathError ||
                    ServerError ||
                    PortError ||
                    CredentialsError ||
                    ClientTimeOffsetError);
         }
      }

      public LegacyClientSettingsModel()
      {
         _legacyClientSubType = LegacyClientSubType.Path;
         _name = String.Empty;
         _clientProcessorMegahertz = 1;
         _fahLogFileName = Default.FahLogFileName;
         _unitInfoFileName = Default.UnitInfoFileName;
         _queueFileName = Default.QueueFileName;
         _path = String.Empty;
         _server = String.Empty;
         _port = Default.FtpPort;
         _username = String.Empty;
         _password = String.Empty;
      }

      private LegacyClientSubType _legacyClientSubType;
      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      public LegacyClientSubType LegacyClientSubType
      {
         get { return _legacyClientSubType; }
         set
         {
            if (_legacyClientSubType != value &&
                !value.Equals(LegacyClientSubType.None))
            {
               _legacyClientSubType = value;
               ClearAccessSettings();
               OnPropertyChanged("Dummy");
            }
         }
      }

      private string _name;
      /// <summary>
      /// The name assigned to this client.
      /// </summary>
      public string Name
      {
         get { return _name; }
         set
         {
            if (_name != value)
            {
               _name = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Name");
            }
         }
      }
      
      public bool NameEmpty
      {
         get { return Name.Length == 0; }
      }
      
      public bool NameError
      {
         get { return !Validate.ClientName(Name); }
      }

      #region External Client (commented)

      //public bool ExternalClient { get; set; }

      //private string _externalFilename;
      ///// <summary>
      ///// External data file name
      ///// </summary>
      //public string ExternalFilename
      //{
      //   get { return _externalFilename; }
      //   set
      //   {
      //      if (_externalFilename != value)
      //      {
      //         _externalFilename = value == null ? String.Empty : value.Trim();
      //         OnPropertyChanged("ExternalFilename");
      //      }
      //   }
      //}

      //public bool ExternalFilenameError
      //{
      //   get { return !Validate.FileName(ExternalFilename); }
      //}

      #endregion

      private int _clientProcessorMegahertz;
      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      public int ClientProcessorMegahertz
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

      private string _fahLogFileName;
      /// <summary>
      /// Remote client log file name
      /// </summary>
      public string FahLogFileName
      {
         get { return _fahLogFileName; }
         set
         {
            if (_fahLogFileName != value)
            {
               _fahLogFileName = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("FahLogFileName");
            }
         }
      }
      
      public bool FahLogFileNameError
      {
         get { return !Validate.FileName(FahLogFileName); }
      }

      private string _unitInfoFileName;
      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      public string UnitInfoFileName
      {
         get { return _unitInfoFileName; }
         set
         {
            if (_unitInfoFileName != value)
            {
               _unitInfoFileName = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("UnitInfoFileName");
            }
         }
      }

      public bool UnitInfoFileNameError
      {
         get { return !Validate.FileName(UnitInfoFileName); }
      }

      private string _queueFileName;
      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      public string QueueFileName
      {
         get { return _queueFileName; }
         set
         {
            if (_queueFileName != value)
            {
               _queueFileName = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("QueueFileName");
            }
         }
      }

      public bool QueueFileNameError
      {
         get { return !Validate.FileName(QueueFileName); }
      }

      private string _path;
      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      public string Path
      {
         get { return _path; }
         set
         {
            if (_path != value)
            {
               string path = value == null ? String.Empty : value.Trim();
               path = StripFahClientFileNames(path);
               _path = path;
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
            switch (LegacyClientSubType)
            {
               case LegacyClientSubType.Path:
                  if (Path.Length < 2)
                  {
                     return true;
                  }
                  return !Validate.Path(Path);
               case LegacyClientSubType.Http:
                  return !Validate.HttpUrl(Path);
               case LegacyClientSubType.Ftp:
                  return !Validate.FtpPath(Path);
               default:
                  return true;
            }
         }
      }

      private string _server;
      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
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
            switch (LegacyClientSubType)
            {
               case LegacyClientSubType.Ftp:
                  return !Validate.ServerName(Server);
               default:
                  return false;
            }
         }
      }

      private int _port;
      /// <summary>
      /// FTP Server Port
      /// </summary>
      public int Port
      {
         get { return _port; }
         set
         {
            if (_port != value)
            {
               _port = value;
               OnPropertyChanged("Port");
            }
         }
      }

      public bool PortError
      {
         get
         {
            switch (LegacyClientSubType)
            {
               case LegacyClientSubType.Ftp:
                  return !Validate.ServerPort(Port);
               default:
                  return false;
            }
         }
      }

      private string _username;
      /// <summary>
      /// Username on remote server
      /// </summary>
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
            switch (LegacyClientSubType)
            {
               case LegacyClientSubType.Http:
                  return !ValidateCredentials(false);
               case LegacyClientSubType.Ftp:
                  return !ValidateCredentials(true);
               default:
                  return false;
            }
         }
      }
      
      private bool ValidateCredentials(bool throwOnEmpty)
      {
         try
         {
            // This will violate FxCop rule (rule ID)
            Validate.UsernamePasswordPair(Username, Password, throwOnEmpty);
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
      public bool UtcOffsetIsZero { get; set; }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      public int ClientTimeOffset { get; set; }
      
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

      private void ClearAccessSettings()
      {
         Path = String.Empty;
         Server = String.Empty;
         Port = Default.FtpPort;
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
