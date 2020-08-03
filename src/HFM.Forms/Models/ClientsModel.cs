
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public IPreferenceSet Preferences { get; }

        public ClientsModel(IPreferenceSet preferences)
        {
            Preferences = preferences;
        }

        public override void Load()
        {
            DefaultConfigFile = Preferences.Get<string>(Preference.DefaultConfigFile);
            UseDefaultConfigFile = Preferences.Get<bool>(Preference.UseDefaultConfigFile);

            var clientRetrievalTask = Preferences.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
            SyncTimeMinutes = clientRetrievalTask.Interval;
            SyncOnSchedule = clientRetrievalTask.Enabled;
            SyncOnLoad = clientRetrievalTask.ProcessingMode == ProcessingMode.Serial.ToString();

            OfflineLast = Preferences.Get<bool>(Preference.OfflineLast);
            ColorLogFile = Preferences.Get<bool>(Preference.ColorLogFile);
            AutoSaveConfig = Preferences.Get<bool>(Preference.AutoSaveConfig);
            PpdCalculation = Preferences.Get<PPDCalculation>(Preference.PPDCalculation);
            DecimalPlaces = Preferences.Get<int>(Preference.DecimalPlaces);
            CalculateBonus = Preferences.Get<BonusCalculation>(Preference.BonusCalculation);
            EtaDate = Preferences.Get<bool>(Preference.DisplayEtaAsDate);
            DuplicateProjectCheck = Preferences.Get<bool>(Preference.DuplicateProjectCheck);
        }

        public override void Save()
        {
            Preferences.Set(Preference.DefaultConfigFile, DefaultConfigFile);
            Preferences.Set(Preference.UseDefaultConfigFile, UseDefaultConfigFile);

            var clientRetrievalTask = new ClientRetrievalTask
            {
                Enabled = SyncOnSchedule,
                Interval = SyncTimeMinutes,
                ProcessingMode = (SyncOnLoad ? ProcessingMode.Serial : ProcessingMode.Parallel).ToString()
            };
            Preferences.Set(Preference.ClientRetrievalTask, clientRetrievalTask);

            Preferences.Set(Preference.OfflineLast, OfflineLast);
            Preferences.Set(Preference.ColorLogFile, ColorLogFile);
            Preferences.Set(Preference.AutoSaveConfig, AutoSaveConfig);
            Preferences.Set(Preference.PPDCalculation, PpdCalculation);
            Preferences.Set(Preference.DecimalPlaces, DecimalPlaces);
            Preferences.Set(Preference.BonusCalculation, CalculateBonus);
            Preferences.Set(Preference.DisplayEtaAsDate, EtaDate);
            Preferences.Set(Preference.DuplicateProjectCheck, DuplicateProjectCheck);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(SyncTimeMinutes):
                        return ValidateSyncTimeMinutes() ? null : SyncTimeMinutesError;
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
                    nameof(SyncTimeMinutes)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

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

        #region Refresh Client Data

        private int _syncTimeMinutes;

        public int SyncTimeMinutes
        {
            get { return _syncTimeMinutes; }
            set
            {
                if (SyncTimeMinutes != value)
                {
                    _syncTimeMinutes = value;
                    OnPropertyChanged();
                }
            }
        }

        private static string SyncTimeMinutesError { get; } = String.Format("Minutes must be a value from {0} to {1}.", ClientScheduledTasks.MinInterval, ClientScheduledTasks.MaxInterval);

        private bool ValidateSyncTimeMinutes()
        {
            return !SyncOnSchedule || ClientScheduledTasks.ValidateInterval(SyncTimeMinutes);
        }

        private bool _syncOnSchedule;

        public bool SyncOnSchedule
        {
            get { return _syncOnSchedule; }
            set
            {
                if (SyncOnSchedule != value)
                {
                    _syncOnSchedule = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _syncLoad;

        public bool SyncOnLoad
        {
            get { return _syncLoad; }
            set
            {
                if (SyncOnLoad != value)
                {
                    _syncLoad = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Display / Production Options

        private bool _offlineLast;

        public bool OfflineLast
        {
            get { return _offlineLast; }
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
            get { return _colorLogFile; }
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
            get { return _autoSaveConfig; }
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

        public PPDCalculation PpdCalculation
        {
            get { return _ppdCalculation; }
            set
            {
                if (PpdCalculation != value)
                {
                    _ppdCalculation = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _decimalPlaces;

        public int DecimalPlaces
        {
            get { return _decimalPlaces; }
            set
            {
                if (DecimalPlaces != value)
                {
                    _decimalPlaces = value;
                    OnPropertyChanged();
                }
            }
        }

        private BonusCalculation _calculateBonus;

        public BonusCalculation CalculateBonus
        {
            get { return _calculateBonus; }
            set
            {
                if (CalculateBonus != value)
                {
                    _calculateBonus = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _etaDate;

        public bool EtaDate
        {
            get { return _etaDate; }
            set
            {
                if (EtaDate != value)
                {
                    _etaDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _duplicateProjectCheck;

        public bool DuplicateProjectCheck
        {
            get { return _duplicateProjectCheck; }
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

        public static ReadOnlyCollection<ListItem> PpdCalculationList { get; } = new List<ListItem>
        {
            new ListItem { DisplayMember = "Last Frame", ValueMember = PPDCalculation.LastFrame },
            new ListItem { DisplayMember = "Last Three Frames", ValueMember = PPDCalculation.LastThreeFrames },
            new ListItem { DisplayMember = "All Frames", ValueMember = PPDCalculation.AllFrames },
            new ListItem { DisplayMember = "Effective Rate", ValueMember = PPDCalculation.EffectiveRate }
        }.AsReadOnly();

        public static ReadOnlyCollection<ListItem> BonusCalculationList { get; } = new List<ListItem>
        {
            new ListItem { DisplayMember = "Download Time", ValueMember = BonusCalculation.DownloadTime },
            new ListItem { DisplayMember = "Frame Time", ValueMember = BonusCalculation.FrameTime },
            new ListItem { DisplayMember = "None", ValueMember = BonusCalculation.None },
        }.AsReadOnly();
    }
}
