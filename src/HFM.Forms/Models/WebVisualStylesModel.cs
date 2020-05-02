
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

using HFM.Core;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    internal class WebVisualStylesModel : INotifyPropertyChanged
    {
        private const string CssExtension = ".css";

        public WebVisualStylesModel(IPreferenceSet prefs)
        {
            Load(prefs);
        }

        public void Load(IPreferenceSet prefs)
        {
            ApplicationPath = prefs.Get<string>(Preference.ApplicationPath);
            CssFile = prefs.Get<string>(Preference.CssFile);
            WebOverview = prefs.Get<string>(Preference.WebOverview);
            WebSummary = prefs.Get<string>(Preference.WebSummary);
            WebSlot = prefs.Get<string>(Preference.WebSlot);
        }

        public void Update(IPreferenceSet prefs)
        {
            prefs.Set(Preference.CssFile, CssFile);
            prefs.Set(Preference.WebOverview, WebOverview);
            prefs.Set(Preference.WebSummary, WebSummary);
            prefs.Set(Preference.WebSlot, WebSlot);
        }

        private string ApplicationPath { get; set; }

        public ReadOnlyCollection<ListItem> CssFileList
        {
            get
            {
                var list = new List<ListItem>();
                var di = new DirectoryInfo(Path.Combine(ApplicationPath, Application.CssFolderName));
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
