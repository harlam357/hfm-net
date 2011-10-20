/*
 * HFM.NET - Preferences - Scheduled Tasks Tab - Binding Model
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
using System.IO;
using System.ComponentModel;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms.Models
{
   class ScheduledTasksModel : INotifyPropertyChanged
   {
      public bool Error
      {
         get
         {
            return SyncTimeMinutesError ||
                   GenerateIntervalError ||
                   WebRootError;
         }
      }

      public ScheduledTasksModel(IPreferenceSet prefs)
      {
         Load(prefs);
      }
   
      public void Load(IPreferenceSet prefs)
      {
         SyncOnLoad = prefs.Get<bool>(Preference.SyncOnLoad);
         DuplicateProjectCheck = prefs.Get<bool>(Preference.DuplicateProjectCheck);
         DuplicateUserIdCheck = prefs.Get<bool>(Preference.DuplicateUserIdCheck);
         SyncTimeMinutes = prefs.Get<int>(Preference.SyncTimeMinutes);
         SyncOnSchedule = prefs.Get<bool>(Preference.SyncOnSchedule);
         AllowRunningAsync = prefs.Get<bool>(Preference.AllowRunningAsync);
         ShowXmlStats = prefs.Get<bool>(Preference.ShowXmlStats);
         GenerateInterval = prefs.Get<int>(Preference.GenerateInterval);
         WebGenAfterRefresh = prefs.Get<bool>(Preference.WebGenAfterRefresh);
         WebRoot = prefs.Get<string>(Preference.WebRoot);
         CopyHtml = prefs.Get<bool>(Preference.WebGenCopyHtml);
         CopyXml = prefs.Get<bool>(Preference.WebGenCopyXml);
         CopyClientData = prefs.Get<bool>(Preference.WebGenCopyClientData);
         CopyFAHlog = prefs.Get<bool>(Preference.WebGenCopyFAHlog);
         FtpMode = prefs.Get<FtpType>(Preference.WebGenFtpMode);
         LimitLogSize = prefs.Get<bool>(Preference.WebGenLimitLogSize);
         LimitLogSizeLength = prefs.Get<int>(Preference.WebGenLimitLogSizeLength);
         GenerateWeb = prefs.Get<bool>(Preference.GenerateWeb);
      }

      public void Update(IPreferenceSet prefs)
      {
         prefs.Set(Preference.SyncOnLoad, SyncOnLoad);
         prefs.Set(Preference.DuplicateProjectCheck, DuplicateProjectCheck);
         prefs.Set(Preference.DuplicateUserIdCheck, DuplicateUserIdCheck);
         prefs.Set(Preference.SyncTimeMinutes, SyncTimeMinutes);
         prefs.Set(Preference.SyncOnSchedule, SyncOnSchedule);
         prefs.Set(Preference.AllowRunningAsync, AllowRunningAsync);
         prefs.Set(Preference.ShowXmlStats, ShowXmlStats);
         prefs.Set(Preference.GenerateInterval, GenerateInterval);
         prefs.Set(Preference.WebGenAfterRefresh, WebGenAfterRefresh);
         prefs.Set(Preference.WebRoot, WebRoot);
         prefs.Set(Preference.WebGenCopyHtml, CopyHtml);
         prefs.Set(Preference.WebGenCopyXml, CopyXml);
         prefs.Set(Preference.WebGenCopyClientData, CopyClientData);
         prefs.Set(Preference.WebGenCopyFAHlog, CopyFAHlog);
         prefs.Set(Preference.WebGenFtpMode, FtpMode);
         prefs.Set(Preference.WebGenLimitLogSize, LimitLogSize);
         prefs.Set(Preference.WebGenLimitLogSizeLength, LimitLogSizeLength);
         prefs.Set(Preference.GenerateWeb, GenerateWeb);
      }

      #region Refresh Data

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

      private bool _duplicateProjectCheck;

      public bool DuplicateProjectCheck
      {
         get { return _duplicateProjectCheck; }
         set
         {
            if (DuplicateProjectCheck != value)
            {
               _duplicateProjectCheck = value;
               OnPropertyChanged("DuplicateProjectCheck");
            }
         }
      }

      private bool _duplicateUserIdCheck;

      public bool DuplicateUserIdCheck
      {
         get { return _duplicateUserIdCheck; }
         set
         {
            if (DuplicateUserIdCheck != value)
            {
               _duplicateUserIdCheck = value;
               OnPropertyChanged("DuplicateUserIdCheck");
            }
         }
      }

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
            return !StringOps.ValidateMinutes(SyncTimeMinutes);
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

      private bool _showXmlStats;

      public bool ShowXmlStats
      {
         get { return _showXmlStats; }
         set
         {
            if (ShowXmlStats != value)
            {
               _showXmlStats = value;
               OnPropertyChanged("ShowXmlStats");
            }
         }
      }
      
      #endregion

      #region Web Generation

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
            return !StringOps.ValidateMinutes(GenerateInterval);
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
               string newValue = value == null ? String.Empty : value.Trim();
               if (newValue.Length > 1)
               {
                  if ((StringOps.ValidatePathInstancePath(newValue) || StringOps.ValidatePathInstancePath(newValue + Path.DirectorySeparatorChar)) &&
                       newValue.EndsWith(Path.DirectorySeparatorChar.ToString()) == false)
                  {
                     newValue += Path.DirectorySeparatorChar;
                  }
                  else if ((StringOps.ValidateFtpWithUserPassUrl(newValue) || StringOps.ValidateFtpWithUserPassUrl(newValue + "/")) && 
                            newValue.EndsWith("/") == false)
                  {
                     newValue += "/";
                  }
               }
               _webRoot = newValue;
               OnPropertyChanged("WebRoot");
               OnPropertyChanged("FtpModeEnabled");
               OnPropertyChanged("LimitLogSizeEnabled");
               OnPropertyChanged("LimitLogSizeLengthEnabled");
            }
         }
      }

      public bool WebRootError
      {
         get
         {
            if (GenerateWeb == false) return false;
            if (WebRoot.Length == 0) return true;
         
            if (IsWebRootPath || IsWebRootFtp)
            {
               return false;
            }
            return true;
         }
      }
      
      private bool IsWebRootPath
      {
         get { return StringOps.ValidatePathInstancePath(WebRoot); }
      }

      private bool IsWebRootFtp
      {
         get { return StringOps.ValidateFtpWithUserPassUrl(WebRoot); }
      }

      private bool _copyHtml;

      public bool CopyHtml
      {
         get { return _copyHtml; }
         set
         {
            if (CopyHtml != value)
            {
               if (value == false && 
                   CopyXml == false &&
                   CopyClientData == false)
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
               if (value == false && 
                   CopyHtml == false &&
                   CopyClientData == false)
               {
                  CopyHtml = true;
               }
               _copyXml = value;
               OnPropertyChanged("CopyXml");
            }
         }
      }

      private bool _copyClientData;
      
      public bool CopyClientData
      {
         get { return _copyClientData; }
         set
         {
            if (CopyClientData != value)
            {
               if (value == false && 
                   CopyHtml == false &&
                   CopyXml == false)
               {
                  CopyHtml = true;
               }
               _copyClientData = value;
               OnPropertyChanged("CopyClientData");
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

      private FtpType _ftpMode;

      public FtpType FtpMode
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
         get { return GenerateWeb && IsWebRootFtp; }
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
         get { return GenerateWeb && IsWebRootFtp && CopyFAHlog; }
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
