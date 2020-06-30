
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class StartupAndExternalModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }
        public IAutoRun AutoRunConfiguration { get; }

        public StartupAndExternalModel(IPreferenceSet preferences, IAutoRun autoRunConfiguration)
        {
            Preferences = preferences;
            AutoRunConfiguration = autoRunConfiguration;
            Load();
        }

        public void Load()
        {
            AutoRun = AutoRunConfiguration.IsEnabled();
            RunMinimized = Preferences.Get<bool>(Preference.RunMinimized);
            StartupCheckForUpdate = Preferences.Get<bool>(Preference.StartupCheckForUpdate);
            DefaultConfigFile = Preferences.Get<string>(Preference.DefaultConfigFile);
            UseDefaultConfigFile = Preferences.Get<bool>(Preference.UseDefaultConfigFile);
            LogFileViewer = Preferences.Get<string>(Preference.LogFileViewer);
            FileExplorer = Preferences.Get<string>(Preference.FileExplorer);
        }

        public void Update()
        {
            Preferences.Set(Preference.RunMinimized, RunMinimized);
            Preferences.Set(Preference.StartupCheckForUpdate, StartupCheckForUpdate);
            Preferences.Set(Preference.DefaultConfigFile, DefaultConfigFile);
            Preferences.Set(Preference.UseDefaultConfigFile, UseDefaultConfigFile);
            Preferences.Set(Preference.LogFileViewer, LogFileViewer);
            Preferences.Set(Preference.FileExplorer, FileExplorer);
        }

        public void UpdateAutoRun()
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
