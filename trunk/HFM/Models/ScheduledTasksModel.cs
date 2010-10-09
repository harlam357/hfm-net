/*
 * HFM.NET - Preferences - Scheduled Tasks Tab - Binding Model
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
using System.IO;
using System.ComponentModel;

using HFM.Framework;

namespace HFM.Models
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
   
      private readonly IPreferenceSet _prefs;
   
      public ScheduledTasksModel(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      #region Refresh Data

      public bool SyncOnLoad
      {
         get { return _prefs.GetPreference<bool>(Preference.SyncOnLoad); }
         set
         {
            if (SyncOnLoad != value)
            {
               _prefs.SetPreference(Preference.SyncOnLoad, value);
               OnPropertyChanged("SyncOnLoad");
            }
         }
      }

      public bool DuplicateProjectCheck
      {
         get { return _prefs.GetPreference<bool>(Preference.DuplicateProjectCheck); }
         set
         {
            if (DuplicateProjectCheck != value)
            {
               _prefs.SetPreference(Preference.DuplicateProjectCheck, value);
               OnPropertyChanged("DuplicateProjectCheck");
            }
         }
      }

      public bool DuplicateUserIdCheck
      {
         get { return _prefs.GetPreference<bool>(Preference.DuplicateUserIdCheck); }
         set
         {
            if (DuplicateUserIdCheck != value)
            {
               _prefs.SetPreference(Preference.DuplicateUserIdCheck, value);
               OnPropertyChanged("DuplicateUserIdCheck");
            }
         }
      }

      public int SyncTimeMinutes
      {
         get { return _prefs.GetPreference<int>(Preference.SyncTimeMinutes); }
         set
         {
            if (SyncTimeMinutes != value)
            {
               _prefs.SetPreference(Preference.SyncTimeMinutes, value);
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

      public bool SyncOnSchedule
      {
         get { return _prefs.GetPreference<bool>(Preference.SyncOnSchedule); }
         set
         {
            if (SyncOnSchedule != value)
            {
               _prefs.SetPreference(Preference.SyncOnSchedule, value);
               OnPropertyChanged("SyncOnSchedule");
            }
         }
      }

      public bool AllowRunningAsync
      {
         get { return _prefs.GetPreference<bool>(Preference.AllowRunningAsync); }
         set
         {
            if (AllowRunningAsync != value)
            {
               _prefs.SetPreference(Preference.AllowRunningAsync, value);
               OnPropertyChanged("AllowRunningAsync");
            }
         }
      }

      public bool ShowXmlStats
      {
         get { return _prefs.GetPreference<bool>(Preference.ShowXmlStats); }
         set
         {
            if (ShowXmlStats != value)
            {
               _prefs.SetPreference(Preference.ShowXmlStats, value);
               OnPropertyChanged("ShowXmlStats");
            }
         }
      }
      
      #endregion

      #region Web Generation

      public int GenerateInterval
      {
         get { return _prefs.GetPreference<int>(Preference.GenerateInterval); }
         set
         {
            if (GenerateInterval != value)
            {
               _prefs.SetPreference(Preference.GenerateInterval, value);
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

      public bool WebGenAfterRefresh
      {
         get { return _prefs.GetPreference<bool>(Preference.WebGenAfterRefresh); }
         set
         {
            if (WebGenAfterRefresh != value)
            {
               _prefs.SetPreference(Preference.WebGenAfterRefresh, value);
               OnPropertyChanged("WebGenAfterRefresh");
               OnPropertyChanged("GenerateIntervalEnabled");
            }
         }
      }

      public string WebRoot
      {
         get { return _prefs.GetPreference<string>(Preference.WebRoot); }
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
               _prefs.SetPreference(Preference.WebRoot, newValue);
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

      public bool CopyHtml
      {
         get { return _prefs.GetPreference<bool>(Preference.WebGenCopyHtml); }
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
               _prefs.SetPreference(Preference.WebGenCopyHtml, value);
               OnPropertyChanged("CopyHtml");
            }
         }
      }

      public bool CopyXml
      {
         get { return _prefs.GetPreference<bool>(Preference.WebGenCopyXml); }
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
               _prefs.SetPreference(Preference.WebGenCopyXml, value);
               OnPropertyChanged("CopyXml");
            }
         }
      }
      
      public bool CopyClientData
      {
         get { return _prefs.GetPreference<bool>(Preference.WebGenCopyClientData); }
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
               _prefs.SetPreference(Preference.WebGenCopyClientData, value);
               OnPropertyChanged("CopyClientData");
            }
         }
      }

      // ReSharper disable InconsistentNaming
      public bool CopyFAHlog
      // ReSharper restore InconsistentNaming
      {
         get { return _prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog); }
         set
         {
            if (CopyFAHlog != value)
            {
               _prefs.SetPreference(Preference.WebGenCopyFAHlog, value);
               OnPropertyChanged("CopyFAHlog");
               OnPropertyChanged("LimitLogSizeEnabled");
               OnPropertyChanged("LimitLogSizeLengthEnabled");
            }
         }
      }

      public FtpType FtpMode
      {
         get { return _prefs.GetPreference<FtpType>(Preference.WebGenFtpMode); }
         set
         {
            if (FtpMode != value)
            {
               _prefs.SetPreference(Preference.WebGenFtpMode, value);
               OnPropertyChanged("FtpMode");
            }
         }
      }

      public bool FtpModeEnabled
      {
         get { return GenerateWeb && IsWebRootFtp; }
      }

      public bool LimitLogSize
      {
         get { return _prefs.GetPreference<bool>(Preference.WebGenLimitLogSize); }
         set
         {
            if (LimitLogSize != value)
            {
               _prefs.SetPreference(Preference.WebGenLimitLogSize, value);
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

      public int LimitLogSizeLength
      {
         get { return _prefs.GetPreference<int>(Preference.WebGenLimitLogSizeLength); }
         set
         {
            if (LimitLogSizeLength != value)
            {
               _prefs.SetPreference(Preference.WebGenLimitLogSizeLength, value);
               OnPropertyChanged("LimitLogSizeLength");
            }
         }
      }

      public bool LimitLogSizeLengthEnabled
      {
         get { return LimitLogSizeEnabled && LimitLogSize; }
      }

      public bool GenerateWeb
      {
         get { return _prefs.GetPreference<bool>(Preference.GenerateWeb); }
         set
         {
            if (GenerateWeb != value)
            {
               _prefs.SetPreference(Preference.GenerateWeb, value);
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
