
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class OptionsModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }
        public IAutoRunConfiguration AutoRunConfiguration { get; }

        public OptionsModel(IPreferenceSet preferences, IAutoRunConfiguration autoRunConfiguration)
        {
            Preferences = preferences;
            AutoRunConfiguration = autoRunConfiguration;
        }

        public override void Load()
        {
            AutoRun = AutoRunConfiguration.IsEnabled();
            RunMinimized = Preferences.Get<bool>(Preference.RunMinimized);
            StartupCheckForUpdate = Preferences.Get<bool>(Preference.StartupCheckForUpdate);
            LogFileViewer = Preferences.Get<string>(Preference.LogFileViewer);
            FileExplorer = Preferences.Get<string>(Preference.FileExplorer);
            MessageLevel = (LoggerLevel)Preferences.Get<int>(Preference.MessageLevel);
            FormShowStyle = Preferences.Get<MinimizeToOption>(Preference.MinimizeTo);
        }

        public override void Save()
        {
            Preferences.Set(Preference.RunMinimized, RunMinimized);
            Preferences.Set(Preference.StartupCheckForUpdate, StartupCheckForUpdate);
            Preferences.Set(Preference.LogFileViewer, LogFileViewer);
            Preferences.Set(Preference.FileExplorer, FileExplorer);
            Preferences.Set(Preference.MessageLevel, (int)MessageLevel);
            Preferences.Set(Preference.MinimizeTo, FormShowStyle);
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

        #region Debug Message Level

        private LoggerLevel _messageLevel;

        public LoggerLevel MessageLevel
        {
            get { return _messageLevel; }
            set
            {
                if (MessageLevel != value)
                {
                    _messageLevel = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Form Docking Style

        private MinimizeToOption _formShowStyle;

        public MinimizeToOption FormShowStyle
        {
            get { return _formShowStyle; }
            set
            {
                if (FormShowStyle != value)
                {
                    _formShowStyle = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public static ReadOnlyCollection<ListItem> DebugList { get; } = new List<ListItem>
        {
            new ListItem { DisplayMember = LoggerLevel.Info.ToString(), ValueMember = LoggerLevel.Info },
            new ListItem { DisplayMember = LoggerLevel.Debug.ToString(), ValueMember = LoggerLevel.Debug }
        }.AsReadOnly();

        public static ReadOnlyCollection<ListItem> DockingStyleList { get; } = new List<ListItem>
        {
            new ListItem { DisplayMember = "System Tray", ValueMember = MinimizeToOption.SystemTray },
            new ListItem { DisplayMember = "Task Bar", ValueMember = MinimizeToOption.TaskBar },
            new ListItem { DisplayMember = "Both", ValueMember = MinimizeToOption.Both }
        }.AsReadOnly();
    }
}
