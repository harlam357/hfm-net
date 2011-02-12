/*
 * HFM.NET - Preferences - Startup and External Tab - Binding Model
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
   class StartupAndExternalModel : INotifyPropertyChanged
   {
      private readonly IPreferenceSet _prefs;
      
      public StartupAndExternalModel(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      #region Startup

      public bool RunMinimized
      {
         get { return _prefs.GetPreference<bool>(Preference.RunMinimized); }
         set
         {
            if (RunMinimized != value)
            {
               _prefs.SetPreference(Preference.RunMinimized, value);
               OnPropertyChanged("RunMinimized");
            }
         }
      }

      public bool StartupCheckForUpdate
      {
         get { return _prefs.GetPreference<bool>(Preference.StartupCheckForUpdate); }
         set
         {
            if (StartupCheckForUpdate != value)
            {
               _prefs.SetPreference(Preference.StartupCheckForUpdate, value);
               OnPropertyChanged("StartupCheckForUpdate");
            }
         }
      }
      
      #endregion

      #region Configuration File

      public string DefaultConfigFile
      {
         get { return _prefs.GetPreference<string>(Preference.DefaultConfigFile); }
         set
         {
            if (DefaultConfigFile != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.DefaultConfigFile, newValue);
               OnPropertyChanged("DefaultConfigFile");
               
               if (newValue.Length == 0)
               {
                  UseDefaultConfigFile = false;
               }
            }
         }
      }

      public bool UseDefaultConfigFile
      {
         get { return _prefs.GetPreference<bool>(Preference.UseDefaultConfigFile); }
         set
         {
            if (UseDefaultConfigFile != value)
            {
               _prefs.SetPreference(Preference.UseDefaultConfigFile, value);
               OnPropertyChanged("UseDefaultConfigFile");
            }
         }
      }
      
      #endregion

      #region External Programs

      public string LogFileViewer
      {
         get { return _prefs.GetPreference<string>(Preference.LogFileViewer); }
         set
         {
            if (LogFileViewer != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.LogFileViewer, newValue);
               OnPropertyChanged("LogFileViewer");
            }
         }
      }

      public string FileExplorer
      {
         get { return _prefs.GetPreference<string>(Preference.FileExplorer); }
         set
         {
            if (FileExplorer != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.FileExplorer, newValue);
               OnPropertyChanged("FileExplorer");
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
