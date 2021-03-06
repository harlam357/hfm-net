﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Preferences.Data;

namespace HFM.Forms.Models
{
    public class ClientsModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferences Preferences { get; }

        public ClientsModel(IPreferences preferences)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
        }

        public override void Load()
        {
            DefaultConfigFile = Preferences.Get<string>(Preference.DefaultConfigFile);
            DefaultConfigFileEnabled = Preferences.Get<bool>(Preference.UseDefaultConfigFile);
            AutoSaveConfig = Preferences.Get<bool>(Preference.AutoSaveConfig);

            var clientRetrievalTask = Preferences.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
            RetrievalInterval = clientRetrievalTask.Interval;
            RetrievalEnabled = clientRetrievalTask.Enabled;
            RetrievalIsSerial = clientRetrievalTask.ProcessingMode == ProcessingMode.Serial.ToString();

            OfflineLast = Preferences.Get<bool>(Preference.OfflineLast);
            ColorLogFile = Preferences.Get<bool>(Preference.ColorLogFile);
            PPDCalculation = Preferences.Get<PPDCalculation>(Preference.PPDCalculation);
            DecimalPlaces = Preferences.Get<int>(Preference.DecimalPlaces);
            BonusCalculation = Preferences.Get<BonusCalculation>(Preference.BonusCalculation);
            DisplayETADate = Preferences.Get<bool>(Preference.DisplayEtaAsDate);
            DuplicateProjectCheck = Preferences.Get<bool>(Preference.DuplicateProjectCheck);
        }

        public override void Save()
        {
            Preferences.Set(Preference.DefaultConfigFile, DefaultConfigFile);
            Preferences.Set(Preference.UseDefaultConfigFile, DefaultConfigFileEnabled);
            Preferences.Set(Preference.AutoSaveConfig, AutoSaveConfig);

            var clientRetrievalTask = new ClientRetrievalTask
            {
                Enabled = RetrievalEnabled,
                Interval = RetrievalInterval,
                ProcessingMode = (RetrievalIsSerial ? ProcessingMode.Serial : ProcessingMode.Parallel).ToString()
            };
            Preferences.Set(Preference.ClientRetrievalTask, clientRetrievalTask);

            Preferences.Set(Preference.OfflineLast, OfflineLast);
            Preferences.Set(Preference.ColorLogFile, ColorLogFile);
            Preferences.Set(Preference.PPDCalculation, PPDCalculation);
            Preferences.Set(Preference.DecimalPlaces, DecimalPlaces);
            Preferences.Set(Preference.BonusCalculation, BonusCalculation);
            Preferences.Set(Preference.DisplayEtaAsDate, DisplayETADate);
            Preferences.Set(Preference.DuplicateProjectCheck, DuplicateProjectCheck);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(RetrievalInterval):
                        return ValidateRetrievalInterval() ? null : RetrievalIntervalError;
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
                    nameof(RetrievalInterval)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        #region Configuration File

        private string _defaultConfigFile;

        public string DefaultConfigFile
        {
            get => _defaultConfigFile;
            set
            {
                if (DefaultConfigFile != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _defaultConfigFile = newValue;
                    OnPropertyChanged();

                    if (newValue.Length == 0)
                    {
                        DefaultConfigFileEnabled = false;
                    }
                }
            }
        }

        private bool _defaultConfigFileEnabled;

        public bool DefaultConfigFileEnabled
        {
            get => _defaultConfigFileEnabled;
            set
            {
                if (DefaultConfigFileEnabled != value)
                {
                    _defaultConfigFileEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Refresh Client Data

        private int _retrievalInterval;

        public int RetrievalInterval
        {
            get => _retrievalInterval;
            set
            {
                if (RetrievalInterval != value)
                {
                    _retrievalInterval = value;
                    OnPropertyChanged();
                }
            }
        }

        private static string RetrievalIntervalError { get; } = String.Format("Minutes must be a value from {0} to {1}.", ClientScheduledTasks.MinInterval, ClientScheduledTasks.MaxInterval);

        private bool ValidateRetrievalInterval()
        {
            return !RetrievalEnabled || ClientScheduledTasks.ValidateInterval(RetrievalInterval);
        }

        private bool _retrievalEnabled;

        public bool RetrievalEnabled
        {
            get => _retrievalEnabled;
            set
            {
                if (RetrievalEnabled != value)
                {
                    _retrievalEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _retrievalIsSerial;

        public bool RetrievalIsSerial
        {
            get => _retrievalIsSerial;
            set
            {
                if (RetrievalIsSerial != value)
                {
                    _retrievalIsSerial = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Display / Production Options

        private bool _offlineLast;

        public bool OfflineLast
        {
            get => _offlineLast;
            set
            {
                if (OfflineLast != value)
                {
                    _offlineLast = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _colorLogFile;

        public bool ColorLogFile
        {
            get => _colorLogFile;
            set
            {
                if (ColorLogFile != value)
                {
                    _colorLogFile = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _autoSaveConfig;

        public bool AutoSaveConfig
        {
            get => _autoSaveConfig;
            set
            {
                if (AutoSaveConfig != value)
                {
                    _autoSaveConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        private PPDCalculation _ppdCalculation;

        public PPDCalculation PPDCalculation
        {
            get => _ppdCalculation;
            set
            {
                if (PPDCalculation != value)
                {
                    _ppdCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _decimalPlaces;

        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                if (DecimalPlaces != value)
                {
                    _decimalPlaces = value;
                    OnPropertyChanged();
                }
            }
        }

        private BonusCalculation _bonusCalculation;

        public BonusCalculation BonusCalculation
        {
            get => _bonusCalculation;
            set
            {
                if (BonusCalculation != value)
                {
                    _bonusCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _displayETADate;

        public bool DisplayETADate
        {
            get => _displayETADate;
            set
            {
                if (DisplayETADate != value)
                {
                    _displayETADate = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _duplicateProjectCheck;

        public bool DuplicateProjectCheck
        {
            get => _duplicateProjectCheck;
            set
            {
                if (DuplicateProjectCheck != value)
                {
                    _duplicateProjectCheck = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public static IReadOnlyCollection<ListItem> PPDCalculationList { get; } = new List<ListItem>
        {
            new ListItem("Last Frame", PPDCalculation.LastFrame),
            new ListItem("Last Three Frames", PPDCalculation.LastThreeFrames),
            new ListItem("All Frames", PPDCalculation.AllFrames),
            new ListItem("Effective Rate", PPDCalculation.EffectiveRate)
        }.AsReadOnly();

        public static IReadOnlyCollection<ListItem> BonusCalculationList { get; } = new List<ListItem>
        {
            new ListItem("Download Time", BonusCalculation.DownloadTime),
            new ListItem("Frame Time", BonusCalculation.FrameTime),
            new ListItem("None", BonusCalculation.None)
        }.AsReadOnly();
    }
}
