/*
 * HFM.NET - Preferences - Reporting Tab - Binding Model
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.ComponentModel;

using HFM.Core;

namespace HFM.Forms.Models
{
   class ReportingModel : INotifyPropertyChanged
   {
      public bool Error
      {
         get
         {
            return ToAddressError ||
                   FromAddressError ||
                   ServerPortPairError ||
                   UsernamePasswordPairError;
         }
      }
   
      public ReportingModel(IPreferenceSet prefs)
      {
         Load(prefs);
      }

      public void Load(IPreferenceSet prefs)
      {
         ServerSecure = prefs.Get<bool>(Preference.EmailReportingServerSecure);
         ToAddress = prefs.Get<string>(Preference.EmailReportingToAddress);
         FromAddress = prefs.Get<string>(Preference.EmailReportingFromAddress);
         ServerAddress = prefs.Get<string>(Preference.EmailReportingServerAddress);
         ServerPort = prefs.Get<int>(Preference.EmailReportingServerPort);
         ServerUsername = prefs.Get<string>(Preference.EmailReportingServerUsername);
         ServerPassword = prefs.Get<string>(Preference.EmailReportingServerPassword);
         ReportingEnabled = prefs.Get<bool>(Preference.EmailReportingEnabled);
         ReportEuePause = prefs.Get<bool>(Preference.ReportEuePause);
         ReportHung = prefs.Get<bool>(Preference.ReportHung);
      }

      public void Update(IPreferenceSet prefs)
      {
         prefs.Set(Preference.EmailReportingServerSecure, ServerSecure);
         prefs.Set(Preference.EmailReportingToAddress, ToAddress);
         prefs.Set(Preference.EmailReportingFromAddress, FromAddress);
         prefs.Set(Preference.EmailReportingServerAddress, ServerAddress);
         prefs.Set(Preference.EmailReportingServerPort, ServerPort);
         prefs.Set(Preference.EmailReportingServerUsername, ServerUsername);
         prefs.Set(Preference.EmailReportingServerPassword, ServerPassword);
         prefs.Set(Preference.EmailReportingEnabled, ReportingEnabled);
         prefs.Set(Preference.ReportEuePause, ReportEuePause);
         prefs.Set(Preference.ReportHung, ReportHung);
      }

      #region Email Settings

      private bool _serverSecure;

      public bool ServerSecure
      {
         get { return _serverSecure; }
         set
         {
            if (ServerSecure != value)
            {
               _serverSecure = value;
               OnPropertyChanged("ServerSecure");
            }
         }
      }

      private string _toAddress;

      public string ToAddress
      {
         get { return _toAddress; }
         set
         {
            if (ToAddress != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _toAddress = newValue;
               OnPropertyChanged("ToAddress");
            }
         }
      }
      
      public bool ToAddressError
      {
         get
         {
            if (ReportingEnabled == false) return false;
            if (ToAddress.Length == 0) return true;

            return !Validate.EmailAddress(ToAddress);
         }
      }

      private string _fromAddress;

      public string FromAddress
      {
         get { return _fromAddress; }
         set
         {
            if (FromAddress != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _fromAddress = newValue;
               OnPropertyChanged("FromAddress");
            }
         }
      }

      public bool FromAddressError
      {
         get
         {
            if (ReportingEnabled == false) return false;
            if (FromAddress.Length == 0) return true;

            return !StringOps.ValidateEmailAddress(FromAddress);
         }
      }

      private string _serverAddress;

      public string ServerAddress
      {
         get { return _serverAddress; }
         set
         {
            if (ServerAddress != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _serverAddress = newValue;
               OnPropertyChanged("ServerPort");
               OnPropertyChanged("ServerAddress");
            }
         }
      }

      public bool ServerAddressError
      {
         get { return ServerPortPairError; }
      }

      private int _serverPort;

      public int ServerPort
      {
         get { return _serverPort; }
         set
         {
            if (ServerPort != value)
            {
               _serverPort = value;
               OnPropertyChanged("ServerAddress");
               OnPropertyChanged("ServerPort");
            }
         }
      }

      public bool ServerPortError
      {
         get { return ServerPortPairError; }
      }
      
      private bool ServerPortPairError
      {
         get
         {
            if (ReportingEnabled == false) return false;
         
            try
            {
               // This will violate FxCop rule (rule ID)
               StringOps.ValidateServerPortPair(ServerAddress, ServerPort.ToString());
               ServerPortPairErrorMessage = String.Empty;
               return false;
            }
            catch (ArgumentException ex)
            {
               ServerPortPairErrorMessage = ex.Message;
               return true;
            }
         }
      }
      
      public string ServerPortPairErrorMessage { get; private set; }

      private string _serverUsername;

      public string ServerUsername
      {
         get { return _serverUsername; }
         set
         {
            if (ServerUsername != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _serverUsername = newValue;
               OnPropertyChanged("ServerPassword");
               OnPropertyChanged("ServerUsername");
            }
         }
      }

      public bool ServerUsernameError
      {
         get { return UsernamePasswordPairError; }
      }

      private string _serverPassword;

      public string ServerPassword
      {
         get { return _serverPassword; }
         set
         {
            if (ServerPassword != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _serverPassword = newValue;
               OnPropertyChanged("ServerUsername");
               OnPropertyChanged("ServerPassword");
            }
         }
      }

      public bool ServerPasswordError
      {
         get { return UsernamePasswordPairError; }
      }

      private bool UsernamePasswordPairError
      {
         get
         {
            if (ReportingEnabled == false) return false;
         
            try
            {
               // This will violate FxCop rule (rule ID)
               StringOps.ValidateUsernamePasswordPair(ServerUsername, ServerPassword);
               UsernamePasswordPairErrorMessage = String.Empty;
               return false;
            }
            catch (ArgumentException ex)
            {
               UsernamePasswordPairErrorMessage = ex.Message;
               return true;
            }
         }
      }

      public string UsernamePasswordPairErrorMessage { get; private set; }

      private bool _reportingEnabled;

      public bool ReportingEnabled
      {
         get { return _reportingEnabled; }
         set
         {
            if (ReportingEnabled != value)
            {
               _reportingEnabled = value;
               OnPropertyChanged("ReportingEnabled");
            }
         }
      }
      
      #endregion

      #region Report Selections

      private bool _reportEuePause;

      public bool ReportEuePause
      {
         get { return _reportEuePause; }
         set
         {
            if (ReportEuePause != value)
            {
               _reportEuePause = value;
               OnPropertyChanged("ReportEuePause");
            }
         }
      }

      private bool _reportHung;

      public bool ReportHung
      {
         get { return _reportHung; }
         set
         {
            if (ReportHung != value)
            {
               _reportHung = value;
               OnPropertyChanged("ReportHung");
            }
         }
      }
      
      #endregion
   
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
