
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using HFM.Core;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class WebVisualStylesModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private const string CssExtension = ".css";

        public IPreferenceSet Preferences { get; }

        public WebVisualStylesModel(IPreferenceSet preferences)
        {
            Preferences = preferences;
            Load();
        }

        public void Load()
        {
            ApplicationPath = Preferences.Get<string>(Preference.ApplicationPath);
            CssFile = Preferences.Get<string>(Preference.CssFile);
            WebOverview = Preferences.Get<string>(Preference.WebOverview);
            WebSummary = Preferences.Get<string>(Preference.WebSummary);
            WebSlot = Preferences.Get<string>(Preference.WebSlot);
        }

        public void Update()
        {
            Preferences.Set(Preference.CssFile, CssFile);
            Preferences.Set(Preference.WebOverview, WebOverview);
            Preferences.Set(Preference.WebSummary, WebSummary);
            Preferences.Set(Preference.WebSlot, WebSlot);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    default:
                        return null;
                }
            }
        }

        public string Error
        {
            get
            {
                var names = new string[0];
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        public bool HasError => !String.IsNullOrWhiteSpace(Error);

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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
