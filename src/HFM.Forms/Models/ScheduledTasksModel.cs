/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using HFM.Preferences;
using HFM.Preferences.Data;

namespace HFM.Forms.Models
{
   internal class ScheduledTasksModel : INotifyPropertyChanged
   {
      public bool Error
      {
         get
         {
            return SyncTimeMinutesError ||
                   GenerateIntervalError ||
                   WebRootError ||
                   WebGenServerError ||
                   WebGenPortError ||
                   WebGenUsernameError ||
                   WebGenPasswordError;
         }
      }

      public ScheduledTasksModel(IPreferenceSet prefs)
      {
         Load(prefs);
      }

      public void Load(IPreferenceSet prefs)
      {
         var clientRetrievalTask = prefs.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
         SyncTimeMinutes = clientRetrievalTask.Interval;
         SyncOnSchedule = clientRetrievalTask.Enabled;
         SyncOnLoad = clientRetrievalTask.ProcessingMode == ProcessingMode.Serial;
         AllowRunningAsync = prefs.Get<bool>(Preference.AllowRunningAsync);

         var webGenerationTask = prefs.Get<WebGenerationTask>(Preference.WebGenerationTask);
         GenerateWeb = webGenerationTask.Enabled;
         GenerateInterval = webGenerationTask.Interval;
         WebGenAfterRefresh = webGenerationTask.AfterClientRetrieval;

         WebGenType = prefs.Get<WebDeploymentType>(Preference.WebDeploymentType);
         WebRoot = prefs.Get<string>(Preference.WebDeploymentRoot);
         WebGenServer = prefs.Get<string>(Preference.WebGenServer);
         WebGenPort = prefs.Get<int>(Preference.WebGenPort);
         WebGenUsername = prefs.Get<string>(Preference.WebGenUsername);
         WebGenPassword = prefs.Get<string>(Preference.WebGenPassword);
         CopyHtml = prefs.Get<bool>(Preference.WebGenCopyHtml);
         CopyXml = prefs.Get<bool>(Preference.WebGenCopyXml);
         CopyFAHlog = prefs.Get<bool>(Preference.WebGenCopyFAHlog);
         FtpMode = prefs.Get<FtpMode>(Preference.WebGenFtpMode);
         LimitLogSize = prefs.Get<bool>(Preference.WebGenLimitLogSize);
         LimitLogSizeLength = prefs.Get<int>(Preference.WebGenLimitLogSizeLength);
      }

      public void Update(IPreferenceSet prefs)
      {
         var clientRetrievalTask = new ClientRetrievalTask
         {
            Enabled = SyncOnSchedule,
            Interval = SyncTimeMinutes,
            ProcessingMode = SyncOnLoad ? ProcessingMode.Serial : ProcessingMode.Parallel
         };
         prefs.Set(Preference.ClientRetrievalTask, clientRetrievalTask);
         prefs.Set(Preference.AllowRunningAsync, AllowRunningAsync);

         var webGenerationTask = new WebGenerationTask
         {
            Enabled = GenerateWeb,
            Interval = GenerateInterval,
            AfterClientRetrieval = WebGenAfterRefresh
         };
         prefs.Set(Preference.WebGenerationTask, webGenerationTask);

         prefs.Set(Preference.WebDeploymentType, WebGenType);
         if (WebGenType == WebDeploymentType.Ftp)
         {
            prefs.Set(Preference.WebDeploymentRoot, FileSystemPath.AddUnixTrailingSlash(WebRoot));
         }
         else
         {
            prefs.Set(Preference.WebDeploymentRoot, FileSystemPath.AddTrailingSlash(WebRoot));
         }
         prefs.Set(Preference.WebGenServer, WebGenServer);
         prefs.Set(Preference.WebGenPort, WebGenPort);
         prefs.Set(Preference.WebGenUsername, WebGenUsername);
         prefs.Set(Preference.WebGenPassword, WebGenPassword);
         prefs.Set(Preference.WebGenCopyHtml, CopyHtml);
         prefs.Set(Preference.WebGenCopyXml, CopyXml);
         prefs.Set(Preference.WebGenCopyFAHlog, CopyFAHlog);
         prefs.Set(Preference.WebGenFtpMode, FtpMode);
         prefs.Set(Preference.WebGenLimitLogSize, LimitLogSize);
         prefs.Set(Preference.WebGenLimitLogSizeLength, LimitLogSizeLength);
      }

      #region Refresh Data

      private int _syncTimeMinutes;

      public int SyncTimeMinutes
      {
         get { return _syncTimeMinutes; }
         set
         {
            if (SyncTimeMinutes != value)
            {
               _syncTimeMinutes = value;
               OnPropertyChanged("SyncTimeMinutes");
            }
         }
      }

      public bool SyncTimeMinutesError
      {
         get
         {
            if (SyncOnSchedule == false) return false;
            return !Validate.Minutes(SyncTimeMinutes);
         }
      }

      private bool _syncOnSchedule;

      public bool SyncOnSchedule
      {
         get { return _syncOnSchedule; }
         set
         {
            if (SyncOnSchedule != value)
            {
               _syncOnSchedule = value;
               OnPropertyChanged("SyncOnSchedule");
            }
         }
      }

      private bool _syncLoad;

      public bool SyncOnLoad
      {
         get { return _syncLoad; }
         set
         {
            if (SyncOnLoad != value)
            {
               _syncLoad = value;
               OnPropertyChanged("SyncOnLoad");
            }
         }
      }

      private bool _allowRunningAsync;

      public bool AllowRunningAsync
      {
         get { return _allowRunningAsync; }
         set
         {
            if (AllowRunningAsync != value)
            {
               _allowRunningAsync = value;
               OnPropertyChanged("AllowRunningAsync");
            }
         }
      }

      #endregion

      #region Web Generation

      private WebDeploymentType _webGenType;

      public WebDeploymentType WebGenType
      {
         get { return _webGenType; }
         set
         {
            if (WebGenType != value)
            {
               _webGenType = value;
               OnPropertyChanged("WebGenType");
               OnPropertyChanged("FtpModeEnabled");
               OnPropertyChanged("BrowseLocalPathEnabled");
               OnPropertyChanged("LimitLogSizeEnabled");
               OnPropertyChanged("LimitLogSizeLengthEnabled");
            }
         }
      }

      private int _generateInterval;

      public int GenerateInterval
      {
         get { return _generateInterval; }
         set
         {
            if (GenerateInterval != value)
            {
               _generateInterval = value;
               OnPropertyChanged("GenerateInterval");
            }
         }
      }

      public bool GenerateIntervalEnabled
      {
         get { return GenerateWeb && WebGenAfterRefresh == false; }
      }

      public bool GenerateIntervalError
      {
         get
         {
            if (GenerateIntervalEnabled == false) return false;
            return !Validate.Minutes(GenerateInterval);
         }
      }

      private bool _webGenAfterRefresh;

      public bool WebGenAfterRefresh
      {
         get { return _webGenAfterRefresh; }
         set
         {
            if (WebGenAfterRefresh != value)
            {
               _webGenAfterRefresh = value;
               OnPropertyChanged("WebGenAfterRefresh");
               OnPropertyChanged("GenerateIntervalEnabled");
            }
         }
      }

      private string _webRoot;

      public string WebRoot
      {
         get { return _webRoot; }
         set
         {
            if (WebRoot != value)
            {
               _webRoot = value == null ? String.Empty : FileSystemPath.AddTrailingSlash(value.Trim());
               OnPropertyChanged("WebRoot");
            }
         }
      }

      public bool WebRootError
      {
         get
         {
            if (GenerateWeb == false) return false;

            switch (WebGenType)
            {
               case WebDeploymentType.Path:
                  if (WebRoot.Length < 2)
                  {
                     return true;
                  }
                  return !Validate.Path(WebRoot);
               case WebDeploymentType.Ftp:
                  return !Validate.FtpPath(WebRoot);
               default:
                  return true;
            }
         }
      }

      private string _webGenServer;

      public string WebGenServer
      {
         get { return _webGenServer; }
         set
         {
            if (WebGenServer != value)
            {
               _webGenServer = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("WebGenServer");
            }
         }
      }

      public bool WebGenServerError
      {
         get
         {
            switch (WebGenType)
            {
               case WebDeploymentType.Ftp:
                  return !Validate.ServerName(WebGenServer);
               default:
                  return false;
            }
         }
      }

      private int _webGenPort;

      public int WebGenPort
      {
         get { return _webGenPort; }
         set
         {
            if (WebGenPort != value)
            {
               _webGenPort = value;
               OnPropertyChanged("WebGenPort");
            }
         }
      }

      public bool WebGenPortError
      {
         get
         {
            switch (WebGenType)
            {
               case WebDeploymentType.Ftp:
                  return !Validate.ServerPort(WebGenPort);
               default:
                  return false;
            }
         }
      }

      private string _webGenUsername;

      public string WebGenUsername
      {
         get { return _webGenUsername; }
         set
         {
            if (WebGenUsername != value)
            {
               _webGenUsername = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("WebGenPassword");
               OnPropertyChanged("WebGenUsername");
            }
         }
      }

      public bool WebGenUsernameError
      {
         get { return CredentialsError; }
      }

      private string _webGenPassword;

      public string WebGenPassword
      {
         get { return _webGenPassword; }
         set
         {
            if (WebGenPassword != value)
            {
               _webGenPassword = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("WebGenUsername");
               OnPropertyChanged("WebGenPassword");
            }
         }
      }

      public bool WebGenPasswordError
      {
         get { return CredentialsError; }
      }

      public bool CredentialsError
      {
         get
         {
            switch (WebGenType)
            {
               case WebDeploymentType.Ftp:
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
            Validate.UsernamePasswordPair(WebGenUsername, WebGenPassword, throwOnEmpty);
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

      private bool _copyHtml;

      public bool CopyHtml
      {
         get { return _copyHtml; }
         set
         {
            if (CopyHtml != value)
            {
               if (value == false && CopyXml == false)
               {
                  return;
               }
               _copyHtml = value;
               OnPropertyChanged("CopyHtml");
            }
         }
      }

      private bool _copyXml;

      public bool CopyXml
      {
         get { return _copyXml; }
         set
         {
            if (CopyXml != value)
            {
               if (value == false && CopyHtml == false)
               {
                  CopyHtml = true;
               }
               _copyXml = value;
               OnPropertyChanged("CopyXml");
            }
         }
      }

      // ReSharper disable InconsistentNaming
      private bool _copyFAHlog;

      public bool CopyFAHlog
      // ReSharper restore InconsistentNaming
      {
         get { return _copyFAHlog; }
         set
         {
            if (CopyFAHlog != value)
            {
               _copyFAHlog = value;
               OnPropertyChanged("CopyFAHlog");
               OnPropertyChanged("LimitLogSizeEnabled");
               OnPropertyChanged("LimitLogSizeLengthEnabled");
            }
         }
      }

      private FtpMode _ftpMode;

      public FtpMode FtpMode
      {
         get { return _ftpMode; }
         set
         {
            if (FtpMode != value)
            {
               _ftpMode = value;
               OnPropertyChanged("FtpMode");
            }
         }
      }

      public bool FtpModeEnabled
      {
         get { return GenerateWeb && WebGenType == WebDeploymentType.Ftp; }
      }

      public bool BrowseLocalPathEnabled
      {
         get { return GenerateWeb && WebGenType == WebDeploymentType.Path; }
      }

      private bool _limitLogSize;

      public bool LimitLogSize
      {
         get { return _limitLogSize; }
         set
         {
            if (LimitLogSize != value)
            {
               _limitLogSize = value;
               OnPropertyChanged("LimitLogSize");
               OnPropertyChanged("LimitLogSizeEnabled");
               OnPropertyChanged("LimitLogSizeLengthEnabled");
            }
         }
      }

      public bool LimitLogSizeEnabled
      {
         get { return GenerateWeb && WebGenType == WebDeploymentType.Ftp && CopyFAHlog; }
      }

      private int _limitLogSizeLength;

      public int LimitLogSizeLength
      {
         get { return _limitLogSizeLength; }
         set
         {
            if (LimitLogSizeLength != value)
            {
               _limitLogSizeLength = value;
               OnPropertyChanged("LimitLogSizeLength");
            }
         }
      }

      public bool LimitLogSizeLengthEnabled
      {
         get { return LimitLogSizeEnabled && LimitLogSize; }
      }

      private bool _generateWeb;

      public bool GenerateWeb
      {
         get { return _generateWeb; }
         set
         {
            if (GenerateWeb != value)
            {
               _generateWeb = value;
               OnPropertyChanged("GenerateWeb");
               OnPropertyChanged("GenerateIntervalEnabled");
               OnPropertyChanged("FtpModeEnabled");
               OnPropertyChanged("BrowseLocalPathEnabled");
               OnPropertyChanged("LimitLogSizeEnabled");
               OnPropertyChanged("LimitLogSizeLengthEnabled");
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
