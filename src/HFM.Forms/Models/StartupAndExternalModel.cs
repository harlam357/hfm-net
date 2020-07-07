
using System;
using System.ComponentModel;
using System.Linq;

using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class StartupAndExternalModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }
        public IAutoRunConfiguration AutoRunConfiguration { get; }

        public StartupAndExternalModel(IPreferenceSet preferences, IAutoRunConfiguration autoRunConfiguration)
        {
            Preferences = preferences;
            AutoRunConfiguration = autoRunConfiguration;
        }

        public override void Load()
        {
            AutoRun = AutoRunConfiguration.IsEnabled();
            RunMinimized = Preferences.Get<bool>(Preference.RunMinimized);
            StartupCheckForUpdate = Preferences.Get<bool>(Preference.StartupCheckForUpdate);
            DefaultConfigFile = Preferences.Get<string>(Preference.DefaultConfigFile);
            UseDefaultConfigFile = Preferences.Get<bool>(Preference.UseDefaultConfigFile);
            LogFileViewer = Preferences.Get<string>(Preference.LogFileViewer);
            FileExplorer = Preferences.Get<string>(Preference.FileExplorer);
        }

        public override void Save()
        {
            Preferences.Set(Preference.RunMinimized, RunMinimized);
            Preferences.Set(Preference.StartupCheckForUpdate, StartupCheckForUpdate);
            Preferences.Set(Preference.DefaultConfigFile, DefaultConfigFile);
            Preferences.Set(Preference.UseDefaultConfigFile, UseDefaultConfigFile);
            Preferences.Set(Preference.LogFileViewer, LogFileViewer);
            Preferences.Set(Preference.FileExplorer, FileExplorer);
        }

        public void SaveAutoRunConfiguration()
        {
            AutoRunConfiguration.SetFilePath(AutoRun ? System.Windows.Forms.Application.ExecutablePath : null);
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

        #region Startup

        private bool _autoRun;

        public bool AutoRun
        {
            get => _autoRun;
            set
            {
                if (_autoRun != value)
                {
                    _autoRun = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _runMinimized;

        public bool RunMinimized
        {
            get { return _runMinimized; }
            set
            {
                if (RunMinimized != value)
                {
                    _runMinimized = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();

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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}
