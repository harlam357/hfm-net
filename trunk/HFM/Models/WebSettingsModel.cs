/*
 * HFM.NET - Preferences - Web Settings Tab - Binding Model
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

namespace HFM.Models
{
   class WebSettingsModel : INotifyPropertyChanged
   {
      public bool Error
      {
         get
         {
            return EocUserIdError ||
                   StanfordIdError ||
                   TeamIdError ||
                   ProjectDownloadUrlError ||
                   ServerPortPairError ||
                   UsernamePasswordPairError;
         }
      }
   
      private readonly IPreferenceSet _prefs;

      public WebSettingsModel(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      #region Web Statistics

      public int EocUserId
      {
         get { return _prefs.GetPreference<int>(Preference.EocUserId); }
         set
         {
            if (EocUserId != value)
            {
               _prefs.SetPreference(Preference.EocUserId, value);
               OnPropertyChanged("EocUserId");
            }
         }
      }
      
      public bool EocUserIdError
      {
         get { return EocUserId < 0; }
      }

      public string StanfordId
      {
         get { return _prefs.GetPreference<string>(Preference.StanfordId); }
         set
         {
            if (StanfordId != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.StanfordId, newValue);
               OnPropertyChanged("StanfordId");
            }
         }
      }

      public bool StanfordIdError
      {
         get { return StanfordId.Length == 0; }
      }

      public int TeamId
      {
         get { return _prefs.GetPreference<int>(Preference.TeamId); }
         set
         {
            if (TeamId != value)
            {
               _prefs.SetPreference(Preference.TeamId, value);
               OnPropertyChanged("TeamId");
            }
         }
      }

      public bool TeamIdError
      {
         get { return TeamId < 0; }
      }
      
      #endregion

      #region Project Download URL

      public string ProjectDownloadUrl
      {
         get { return _prefs.GetPreference<string>(Preference.ProjectDownloadUrl); }
         set
         {
            if (ProjectDownloadUrl != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.ProjectDownloadUrl, newValue);
               OnPropertyChanged("ProjectDownloadUrl");
            }
         }
      }

      public bool ProjectDownloadUrlError
      {
         get { return !StringOps.ValidateHttpUrl(ProjectDownloadUrl); }
      }
      
      #endregion

      #region Web Proxy Settings

      public string ProxyServer
      {
         get { return _prefs.GetPreference<string>(Preference.ProxyServer); }
         set
         {
            if (ProxyServer != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.ProxyServer, newValue);
               OnPropertyChanged("ProxyPort");
               OnPropertyChanged("ProxyServer");
            }
         }
      }

      public bool ProxyServerError
      {
         get { return ServerPortPairError; }
      }

      public int ProxyPort
      {
         get { return _prefs.GetPreference<int>(Preference.ProxyPort); }
         set
         {
            if (ProxyPort != value)
            {
               _prefs.SetPreference(Preference.ProxyPort, value);
               OnPropertyChanged("ProxyServer");
               OnPropertyChanged("ProxyPort");
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
            if (UseProxy == false) return false;
         
            try
            {
               // This will violate FxCop rule (rule ID)
               StringOps.ValidateServerPortPair(ProxyServer, ProxyPort.ToString());
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

      public bool UseProxy
      {
         get { return _prefs.GetPreference<bool>(Preference.UseProxy); }
         set
         {
            if (UseProxy != value)
            {
               _prefs.SetPreference(Preference.UseProxy, value);
               OnPropertyChanged("UseProxy");
               OnPropertyChanged("ProxyAuthEnabled");
            }
         }
      }

      public string ProxyUser
      {
         get { return _prefs.GetPreference<string>(Preference.ProxyUser); }
         set
         {
            if (ProxyUser != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.ProxyUser, newValue);
               OnPropertyChanged("ProxyPass");
               OnPropertyChanged("ProxyUser");
            }
         }
      }

      public bool ProxyUserError
      {
         get { return UsernamePasswordPairError; }
      }

      public string ProxyPass
      {
         get { return _prefs.GetPreference<string>(Preference.ProxyPass); }
         set
         {
            if (ProxyPass != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.ProxyPass, newValue);
               OnPropertyChanged("ProxyUser");
               OnPropertyChanged("ProxyPass");
            }
         }
      }

      public bool ProxyPassError
      {
         get { return UsernamePasswordPairError; }
      }

      private bool UsernamePasswordPairError
      {
         get
         {
            if (ProxyAuthEnabled == false) return false;
         
            try
            {
               // This will violate FxCop rule (rule ID)
               StringOps.ValidateUsernamePasswordPair(ProxyUser, ProxyPass, true);
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

      public bool UseProxyAuth
      {
         get { return _prefs.GetPreference<bool>(Preference.UseProxyAuth); }
         set
         {
            if (UseProxyAuth != value)
            {
               _prefs.SetPreference(Preference.UseProxyAuth, value);
               OnPropertyChanged("UseProxyAuth");
               OnPropertyChanged("ProxyAuthEnabled");
            }
         }
      }
      
      public bool ProxyAuthEnabled
      {
         get { return UseProxy && UseProxyAuth; }
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
