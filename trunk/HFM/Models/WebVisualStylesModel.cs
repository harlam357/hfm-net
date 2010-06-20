/*
 * HFM.NET - Preferences - Web Visual Styles Tab - Binding Model
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

using HFM.Framework;

namespace HFM.Models
{
   class WebVisualStylesModel : INotifyPropertyChanged
   {
      private const string CssExtension = ".css";
   
      private readonly IPreferenceSet _prefs;

      public WebVisualStylesModel(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      public ReadOnlyCollection<ListItem> CssFileList
      {
         get
         {
            var list = new List<ListItem>();
            var di = new DirectoryInfo(Path.Combine(_prefs.ApplicationPath, Constants.CssFolderName));
            if (di.Exists)
            {
               foreach (FileInfo fi in di.GetFiles())
               {
                  if (fi.Extension.Equals(CssExtension))
                  {
                     list.Add(new ListItem { DisplayMember = Path.GetFileNameWithoutExtension(fi.Name), ValueMember = fi.Name });
                  }
               }
            }

            return list.AsReadOnly();
         }
      }

      public string CssFile
      {
         get { return _prefs.GetPreference<string>(Preference.CssFile); }
         set
         {
            if (CssFile != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.CssFile, newValue);
               OnPropertyChanged("CssFile");
            }
         }
      }

      public string WebOverview
      {
         get { return _prefs.GetPreference<string>(Preference.WebOverview); }
         set
         {
            if (WebOverview != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.WebOverview, newValue);
               OnPropertyChanged("WebOverview");
            }
         }
      }

      public string WebMobileOverview
      {
         get { return _prefs.GetPreference<string>(Preference.WebMobileOverview); }
         set
         {
            if (WebMobileOverview != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.WebMobileOverview, newValue);
               OnPropertyChanged("WebMobileOverview");
            }
         }
      }

      public string WebSummary
      {
         get { return _prefs.GetPreference<string>(Preference.WebSummary); }
         set
         {
            if (WebSummary != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.WebSummary, newValue);
               OnPropertyChanged("WebSummary");
            }
         }
      }

      public string WebMobileSummary
      {
         get { return _prefs.GetPreference<string>(Preference.WebMobileSummary); }
         set
         {
            if (WebMobileSummary != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.WebMobileSummary, newValue);
               OnPropertyChanged("WebMobileSummary");
            }
         }
      }

      public string WebInstance
      {
         get { return _prefs.GetPreference<string>(Preference.WebInstance); }
         set
         {
            if (WebInstance != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _prefs.SetPreference(Preference.WebInstance, newValue);
               OnPropertyChanged("WebInstance");
            }
         }
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
