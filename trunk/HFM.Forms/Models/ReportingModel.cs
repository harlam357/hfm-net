/*
 * HFM.NET - Preferences - Reporting Tab - Binding Model
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.ComponentModel;

using HFM.Framework;

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
   
      private readonly IPreferenceSet _prefs;
      
      public ReportingModel(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      #region Email Settings

      public bool ServerSecure
      {
         get { return _prefs.GetPreference<bool>(Preference.EmailReportingServerSecure); }
         set
         {
            if (ServerSecure != value)
            {
               _prefs.SetPreference(Preference.EmailReportingServerSecure, value);
               OnPropertyChanged("ServerSecure");
            }
         }
      }

      public string ToAddress
      {
         get { return _prefs.GetPreference<string>(Preference.EmailReportingToAddress); }
         set
         {
            if (ToAddress != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.EmailReportingToAddress, newValue);
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

            return !StringOps.ValidateEmailAddress(ToAddress);
         }
      }

      public string FromAddress
      {
         get { return _prefs.GetPreference<string>(Preference.EmailReportingFromAddress); }
         set
         {
            if (FromAddress != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.EmailReportingFromAddress, newValue);
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

      public string ServerAddress
      {
         get { return _prefs.GetPreference<string>(Preference.EmailReportingServerAddress); }
         set
         {
            if (ServerAddress != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.EmailReportingServerAddress, newValue);
               OnPropertyChanged("ServerPort");
               OnPropertyChanged("ServerAddress");
            }
         }
      }

      public bool ServerAddressError
      {
         get { return ServerPortPairError; }
      }

      public int ServerPort
      {
         get { return _prefs.GetPreference<int>(Preference.EmailReportingServerPort); }
         set
         {
            if (ServerPort != value)
            {
               _prefs.SetPreference(Preference.EmailReportingServerPort, value);
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

      public string ServerUsername
      {
         get { return _prefs.GetPreference<string>(Preference.EmailReportingServerUsername); }
         set
         {
            if (ServerUsername != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.EmailReportingServerUsername, newValue);
               OnPropertyChanged("ServerPassword");
               OnPropertyChanged("ServerUsername");
            }
         }
      }

      public bool ServerUsernameError
      {
         get { return UsernamePasswordPairError; }
      }

      public string ServerPassword
      {
         get { return _prefs.GetPreference<string>(Preference.EmailReportingServerPassword); }
         set
         {
            if (ServerPassword != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.EmailReportingServerPassword, newValue);
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

      public bool ReportingEnabled
      {
         get { return _prefs.GetPreference<bool>(Preference.EmailReportingEnabled); }
         set
         {
            if (ReportingEnabled != value)
            {
               _prefs.SetPreference(Preference.EmailReportingEnabled, value);
               OnPropertyChanged("ReportingEnabled");
            }
         }
      }
      
      #endregion

      #region Report Selections

      public bool ReportEuePause
      {
         get { return _prefs.GetPreference<bool>(Preference.ReportEuePause); }
         set
         {
            if (ReportEuePause != value)
            {
               _prefs.SetPreference(Preference.ReportEuePause, value);
               OnPropertyChanged("ReportEuePause");
            }
         }
      }

      public bool ReportHung
      {
         get { return _prefs.GetPreference<bool>(Preference.ReportHung); }
         set
         {
            if (ReportHung != value)
            {
               _prefs.SetPreference(Preference.ReportHung, value);
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
