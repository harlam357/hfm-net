/*
 * HFM.NET - Preferences - Web Visual Styles Tab - Binding Model
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

using HFM.Core;

namespace HFM.Forms.Models
{
   class WebVisualStylesModel : INotifyPropertyChanged
   {
      private const string CssExtension = ".css";
   
      public WebVisualStylesModel(IPreferenceSet prefs)
      {
         Load(prefs);
      }

      public void Load(IPreferenceSet prefs)
      {
         ApplicationPath = prefs.ApplicationPath;
         CssFile = prefs.Get<string>(Preference.CssFile);
         WebOverview = prefs.Get<string>(Preference.WebOverview);
         WebMobileOverview = prefs.Get<string>(Preference.WebMobileOverview);
         WebSummary = prefs.Get<string>(Preference.WebSummary);
         WebMobileSummary = prefs.Get<string>(Preference.WebMobileSummary);
         WebSlot = prefs.Get<string>(Preference.WebSlot);
      }

      public void Update(IPreferenceSet prefs)
      {
         prefs.Set(Preference.CssFile, CssFile);
         prefs.Set(Preference.WebOverview, WebOverview);
         prefs.Set(Preference.WebMobileOverview, WebMobileOverview);
         prefs.Set(Preference.WebSummary, WebSummary);
         prefs.Set(Preference.WebMobileSummary, WebMobileSummary);
         prefs.Set(Preference.WebSlot, WebSlot);
      }

      private string ApplicationPath { get; set; }

      public ReadOnlyCollection<ListItem> CssFileList
      {
         get
         {
            var list = new List<ListItem>();
            var di = new DirectoryInfo(Path.Combine(ApplicationPath, Constants.CssFolderName));
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

      private string _cssFile;

      public string CssFile
      {
         get { return _cssFile; }
         set
         {
            if (CssFile != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _cssFile = newValue;
               OnPropertyChanged("CssFile");
            }
         }
      }

      private string _webOverview;

      public string WebOverview
      {
         get { return _webOverview; }
         set
         {
            if (WebOverview != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _webOverview = newValue;
               OnPropertyChanged("WebOverview");
            }
         }
      }

      private string _webMobileOverview;

      public string WebMobileOverview
      {
         get { return _webMobileOverview; }
         set
         {
            if (WebMobileOverview != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _webMobileOverview = newValue;
               OnPropertyChanged("WebMobileOverview");
            }
         }
      }

      private string _webSummary;

      public string WebSummary
      {
         get { return _webSummary; }
         set
         {
            if (WebSummary != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _webSummary = newValue;
               OnPropertyChanged("WebSummary");
            }
         }
      }

      private string _webMobileSummary;

      public string WebMobileSummary
      {
         get { return _webMobileSummary; }
         set
         {
            if (WebMobileSummary != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _webMobileSummary = newValue;
               OnPropertyChanged("WebMobileSummary");
            }
         }
      }

      private string _webSlot;

      public string WebSlot
      {
         get { return _webSlot; }
         set
         {
            if (WebSlot != value)
            {
               string newValue = value == null ? String.Empty : value.Trim();
               _webSlot = newValue;
               OnPropertyChanged("WebSlot");
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
