/*
 * HFM.NET - Preferences - Web Settings Tab - Binding Model
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
   
      public WebSettingsModel(IPreferenceSet prefs)
      {
         Load(prefs);
      }

      public void Load(IPreferenceSet prefs)
      {
         EocUserId = prefs.Get<int>(Preference.EocUserId);
         StanfordId = prefs.Get<string>(Preference.StanfordId);
         TeamId = prefs.Get<int>(Preference.TeamId);
         ProjectDownloadUrl = prefs.Get<string>(Preference.ProjectDownloadUrl);
         ProxyServer = prefs.Get<string>(Preference.ProxyServer);
         ProxyPort = prefs.Get<int>(Preference.ProxyPort);
         UseProxy = prefs.Get<bool>(Preference.UseProxy);
         ProxyUser = prefs.Get<string>(Preference.ProxyUser);
         ProxyPass = prefs.Get<string>(Preference.ProxyPass);
         UseProxyAuth = prefs.Get<bool>(Preference.UseProxyAuth);
      }

      public void Update(IPreferenceSet prefs)
      {
         prefs.Set(Preference.EocUserId, EocUserId);
         prefs.Set(Preference.StanfordId, StanfordId);
         prefs.Set(Preference.TeamId, TeamId);
         prefs.Set(Preference.ProjectDownloadUrl, ProjectDownloadUrl);
         prefs.Set(Preference.ProxyServer, ProxyServer);
         prefs.Set(Preference.ProxyPort, ProxyPort);
         prefs.Set(Preference.UseProxy, UseProxy);
         prefs.Set(Preference.ProxyUser, ProxyUser);
         prefs.Set(Preference.ProxyPass, ProxyPass);
         prefs.Set(Preference.UseProxyAuth, UseProxyAuth);
      }

      #region Web Statistics

      private int _eocUserId;

      public int EocUserId
      {
         get { return _eocUserId; }
         set
         {
            if (EocUserId != value)
            {
               _eocUserId = value;
               OnPropertyChanged("EocUserId");
            }
         }
      }
      
      public bool EocUserIdError
      {
         get { return EocUserId < 0; }
      }

      private string _stanfordId;

      public string StanfordId
      {
         get { return _stanfordId; }
         set
         {
            if (StanfordId != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _stanfordId = newValue;
               OnPropertyChanged("StanfordId");
            }
         }
      }

      public bool StanfordIdError
      {
         get { return StanfordId.Length == 0; }
      }

      private int _teamId;

      public int TeamId
      {
         get { return _teamId; }
         set
         {
            if (TeamId != value)
            {
               _teamId = value;
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

      private string _projectDownloadUrl;

      public string ProjectDownloadUrl
      {
         get { return _projectDownloadUrl; }
         set
         {
            if (ProjectDownloadUrl != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _projectDownloadUrl = newValue;
               OnPropertyChanged("ProjectDownloadUrl");
            }
         }
      }

      public bool ProjectDownloadUrlError
      {
         get { return !Validate.HttpUrl(ProjectDownloadUrl); }
      }
      
      #endregion

      #region Web Proxy Settings

      private string _proxyServer;

      public string ProxyServer
      {
         get { return _proxyServer; }
         set
         {
            if (ProxyServer != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _proxyServer = newValue;
               OnPropertyChanged("ProxyPort");
               OnPropertyChanged("ProxyServer");
            }
         }
      }

      public bool ProxyServerError
      {
         get { return ServerPortPairError; }
      }

      private int _proxyPort;

      public int ProxyPort
      {
         get { return _proxyPort; }
         set
         {
            if (ProxyPort != value)
            {
               _proxyPort = value;
               OnPropertyChanged("ProxyServer");
               OnPropertyChanged("ProxyPort");
            }
         }
      }

      public bool ProxyPortError
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
               Validate.ServerPortPair(ProxyServer, ProxyPort.ToString());
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

      private bool _useProxy;

      public bool UseProxy
      {
         get { return _useProxy; }
         set
         {
            if (UseProxy != value)
            {
               _useProxy = value;
               OnPropertyChanged("UseProxy");
               OnPropertyChanged("ProxyAuthEnabled");
            }
         }
      }

      private string _proxyUser;

      public string ProxyUser
      {
         get { return _proxyUser; }
         set
         {
            if (ProxyUser != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _proxyUser = newValue;
               OnPropertyChanged("ProxyPass");
               OnPropertyChanged("ProxyUser");
            }
         }
      }

      public bool ProxyUserError
      {
         get { return UsernamePasswordPairError; }
      }

      private string _proxyPass;

      public string ProxyPass
      {
         get { return _proxyPass; }
         set
         {
            if (ProxyPass != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _proxyPass = newValue;
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
               Validate.UsernamePasswordPair(ProxyUser, ProxyPass, true);
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

      private bool _useProxyAuth;

      public bool UseProxyAuth
      {
         get { return _useProxyAuth; }
         set
         {
            if (UseProxyAuth != value)
            {
               _useProxyAuth = value;
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
