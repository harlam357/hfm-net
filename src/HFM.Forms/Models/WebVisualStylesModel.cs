using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

using HFM.Core;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class WebVisualStylesModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }

        public WebVisualStylesModel(IPreferenceSet preferences)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
        }

        public override void Load()
        {
            ApplicationPath = Preferences.Get<string>(Preference.ApplicationPath);
            CssFile = Preferences.Get<string>(Preference.CssFile);
            OverviewXsltPath = Preferences.Get<string>(Preference.WebOverview);
            SummaryXsltPath = Preferences.Get<string>(Preference.WebSummary);
            SlotXsltPath = Preferences.Get<string>(Preference.WebSlot);
        }

        public override void Save()
        {
            Preferences.Set(Preference.CssFile, CssFile);
            Preferences.Set(Preference.WebOverview, OverviewXsltPath);
            Preferences.Set(Preference.WebSummary, SummaryXsltPath);
            Preferences.Set(Preference.WebSlot, SlotXsltPath);
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

        public override string Error
        {
            get
            {
                var names = new string[0];
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        private string ApplicationPath { get; set; }

        private const string CssPattern = "*.css";

        public ReadOnlyCollection<ListItem> CssFileList
        {
            get
            {
                var list = new List<ListItem>();
                var di = new DirectoryInfo(Path.Combine(ApplicationPath, Application.CssFolderName));
                if (di.Exists)
                {
                    list.AddRange(di.EnumerateFiles(CssPattern)
                        .Select(fi => new ListItem { DisplayMember = Path.GetFileNameWithoutExtension(fi.Name), ValueMember = fi.Name }));
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

        private string _overviewXsltPath;

        public string OverviewXsltPath
        {
            get { return _overviewXsltPath; }
            set
            {
                if (OverviewXsltPath != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _overviewXsltPath = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private string _summaryXsltPath;

        public string SummaryXsltPath
        {
            get { return _summaryXsltPath; }
            set
            {
                if (SummaryXsltPath != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _summaryXsltPath = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private string _slotXsltPath;

        public string SlotXsltPath
        {
            get { return _slotXsltPath; }
            set
            {
                if (SlotXsltPath != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _slotXsltPath = newValue;
                    OnPropertyChanged();
                }
            }
        }
    }
}
