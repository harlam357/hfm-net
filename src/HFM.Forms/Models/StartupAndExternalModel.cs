/*
 * HFM.NET - Preferences - Startup and External Tab - Binding Model
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
   class StartupAndExternalModel : INotifyPropertyChanged
   {
      public StartupAndExternalModel(IPreferenceSet prefs)
      {
         Load(prefs);
      }

      public void Load(IPreferenceSet prefs)
      {
         RunMinimized = prefs.Get<bool>(Preference.RunMinimized);
         StartupCheckForUpdate = prefs.Get<bool>(Preference.StartupCheckForUpdate);
         DefaultConfigFile = prefs.Get<string>(Preference.DefaultConfigFile);
         UseDefaultConfigFile = prefs.Get<bool>(Preference.UseDefaultConfigFile);
         LogFileViewer = prefs.Get<string>(Preference.LogFileViewer);
         FileExplorer = prefs.Get<string>(Preference.FileExplorer);
      }

      public void Update(IPreferenceSet prefs)
      {
         prefs.Set(Preference.RunMinimized, RunMinimized);
         prefs.Set(Preference.StartupCheckForUpdate, StartupCheckForUpdate);
         prefs.Set(Preference.DefaultConfigFile, DefaultConfigFile);
         prefs.Set(Preference.UseDefaultConfigFile, UseDefaultConfigFile);
         prefs.Set(Preference.LogFileViewer, LogFileViewer);
         prefs.Set(Preference.FileExplorer, FileExplorer);
      }

      #region Startup

      private bool _runMinimized;

      public bool RunMinimized
      {
         get { return _runMinimized; }
         set
         {
            if (RunMinimized != value)
            {
               _runMinimized = value;
               OnPropertyChanged("RunMinimized");
            }
         }
      }

      private bool _startupCheckForUpdate;

      public bool StartupCheckForUpdate
      {
         get { return _startupCheckForUpdate; }
         set
         {
            if (StartupCheckForUpdate != value)
            {
               _startupCheckForUpdate = value;
               OnPropertyChanged("StartupCheckForUpdate");
            }
         }
      }
      
      #endregion

      #region Configuration File

      private string _defaultConfigFile;

      public string DefaultConfigFile
      {
         get { return _defaultConfigFile; }
         set
         {
            if (DefaultConfigFile != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _defaultConfigFile = newValue;
               OnPropertyChanged("DefaultConfigFile");
               
               if (newValue.Length == 0)
               {
                  UseDefaultConfigFile = false;
               }
            }
         }
      }

      private bool _useDefaultConfigFile;

      public bool UseDefaultConfigFile
      {
         get { return _useDefaultConfigFile; }
         set
         {
            if (UseDefaultConfigFile != value)
            {
               _useDefaultConfigFile = value;
               OnPropertyChanged("UseDefaultConfigFile");
            }
         }
      }
      
      #endregion

      #region External Programs

      private string _logFileViewer;

      public string LogFileViewer
      {
         get { return _logFileViewer; }
         set
         {
            if (LogFileViewer != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _logFileViewer = newValue;
               OnPropertyChanged("LogFileViewer");
            }
         }
      }

      private string _fileExplorer;

      public string FileExplorer
      {
         get { return _fileExplorer; }
         set
         {
            if (FileExplorer != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _fileExplorer = newValue;
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
