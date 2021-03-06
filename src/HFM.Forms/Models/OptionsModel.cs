﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class OptionsModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferences Preferences { get; }
        public IAutoRunConfiguration AutoRunConfiguration { get; }

        public OptionsModel(IPreferences preferences, IAutoRunConfiguration autoRunConfiguration)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            AutoRunConfiguration = autoRunConfiguration ?? new InMemoryAutoRunConfiguration();
        }

        public override void Load()
        {
            AutoRun = AutoRunConfiguration.IsEnabled();
            RunMinimized = Preferences.Get<bool>(Preference.RunMinimized);
            StartupCheckForUpdate = Preferences.Get<bool>(Preference.StartupCheckForUpdate);
            EocUserID = Preferences.Get<int>(Preference.EocUserId);
            FahUserID = Preferences.Get<string>(Preference.StanfordId);
            TeamID = Preferences.Get<int>(Preference.TeamId);
            EocUserStatsEnabled = Preferences.Get<bool>(Preference.EnableUserStats);
            LogFileViewer = Preferences.Get<string>(Preference.LogFileViewer);
            FileExplorer = Preferences.Get<string>(Preference.FileExplorer);
            MessageLevel = (LoggerLevel)Preferences.Get<int>(Preference.MessageLevel);
            MinimizeToOption = Preferences.Get<MinimizeToOption>(Preference.MinimizeTo);
        }

        public override void Save()
        {
            Preferences.Set(Preference.RunMinimized, RunMinimized);
            Preferences.Set(Preference.StartupCheckForUpdate, StartupCheckForUpdate);
            Preferences.Set(Preference.EocUserId, EocUserID);
            Preferences.Set(Preference.StanfordId, FahUserID);
            Preferences.Set(Preference.TeamId, TeamID);
            Preferences.Set(Preference.EnableUserStats, EocUserStatsEnabled);
            Preferences.Set(Preference.LogFileViewer, LogFileViewer);
            Preferences.Set(Preference.FileExplorer, FileExplorer);
            Preferences.Set(Preference.MessageLevel, (int)MessageLevel);
            Preferences.Set(Preference.MinimizeTo, MinimizeToOption);
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
                    case nameof(EocUserID):
                        return ValidateEocUserID() ? null : EocUserIDError;
                    case nameof(FahUserID):
                        return ValidateFahUserID() ? null : FahUserIDError;
                    case nameof(TeamID):
                        return ValidateTeamID() ? null : TeamIDError;
                    default:
                        return null;
                }
            }
        }

        public override string Error
        {
            get
            {
                var names = new[]
                {
                    nameof(EocUserID),
                    nameof(FahUserID),
                    nameof(TeamID)
                };
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
            get => _runMinimized;
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
            get => _startupCheckForUpdate;
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

        #region Identity

        private int _eocUserID;

        public int EocUserID
        {
            get => _eocUserID;
            set
            {
                if (EocUserID != value)
                {
                    _eocUserID = value;
                    OnPropertyChanged();
                }
            }
        }

        private const string EocUserIDError = "Provide EOC user ID.";

        private bool ValidateEocUserID()
        {
            return EocUserID >= 0;
        }

        private string _fahUserID;

        public string FahUserID
        {
            get => _fahUserID;
            set
            {
                if (FahUserID != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _fahUserID = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private const string FahUserIDError = "Provide FAH user ID.";

        private bool ValidateFahUserID()
        {
            return FahUserID is null || FahUserID.Length != 0;
        }

        private int _teamID;

        public int TeamID
        {
            get => _teamID;
            set
            {
                if (TeamID != value)
                {
                    _teamID = value;
                    OnPropertyChanged();
                }
            }
        }

        private const string TeamIDError = "Provide FAH team number.";

        private bool ValidateTeamID()
        {
            return TeamID >= 0;
        }

        private bool _eocUserStatsEnabled;

        public bool EocUserStatsEnabled
        {
            get => _eocUserStatsEnabled;
            set
            {
                if (EocUserStatsEnabled != value)
                {
                    _eocUserStatsEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region External Programs

        private string _logFileViewer;

        public string LogFileViewer
        {
            get => _logFileViewer;
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
            get => _fileExplorer;
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
            get => _messageLevel;
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

        private MinimizeToOption _minimizeToOption;

        public MinimizeToOption MinimizeToOption
        {
            get => _minimizeToOption;
            set
            {
                if (MinimizeToOption != value)
                {
                    _minimizeToOption = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public static IReadOnlyCollection<ListItem> DebugList { get; } = new List<ListItem>
        {
            new ListItem(LoggerLevel.Info.ToString(), LoggerLevel.Info),
            new ListItem(LoggerLevel.Debug.ToString(), LoggerLevel.Debug)
        }.AsReadOnly();

        public static IReadOnlyCollection<ListItem> DockingStyleList { get; } = new List<ListItem>
        {
            new ListItem("System Tray", MinimizeToOption.SystemTray),
            new ListItem("Task Bar", MinimizeToOption.TaskBar),
            new ListItem("Both", MinimizeToOption.Both)
        }.AsReadOnly();
    }
}
