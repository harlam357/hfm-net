
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class OptionsModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }

        public OptionsModel(IPreferenceSet preferences)
        {
            Preferences = preferences;
        }

        public override void Load()
        {
            OfflineLast = Preferences.Get<bool>(Preference.OfflineLast);
            ColorLogFile = Preferences.Get<bool>(Preference.ColorLogFile);
            AutoSaveConfig = Preferences.Get<bool>(Preference.AutoSaveConfig);
            PpdCalculation = Preferences.Get<PPDCalculation>(Preference.PPDCalculation);
            DecimalPlaces = Preferences.Get<int>(Preference.DecimalPlaces);
            CalculateBonus = Preferences.Get<BonusCalculation>(Preference.BonusCalculation);
            EtaDate = Preferences.Get<bool>(Preference.DisplayEtaAsDate);
            DuplicateProjectCheck = Preferences.Get<bool>(Preference.DuplicateProjectCheck);
            DefaultConfigFile = Preferences.Get<string>(Preference.DefaultConfigFile);
            UseDefaultConfigFile = Preferences.Get<bool>(Preference.UseDefaultConfigFile);
        }

        public override void Save()
        {
            Preferences.Set(Preference.OfflineLast, OfflineLast);
            Preferences.Set(Preference.ColorLogFile, ColorLogFile);
            Preferences.Set(Preference.AutoSaveConfig, AutoSaveConfig);
            Preferences.Set(Preference.PPDCalculation, PpdCalculation);
            Preferences.Set(Preference.DecimalPlaces, DecimalPlaces);
            Preferences.Set(Preference.BonusCalculation, CalculateBonus);
            Preferences.Set(Preference.DisplayEtaAsDate, EtaDate);
            Preferences.Set(Preference.DuplicateProjectCheck, DuplicateProjectCheck);
            Preferences.Set(Preference.DefaultConfigFile, DefaultConfigFile);
            Preferences.Set(Preference.UseDefaultConfigFile, UseDefaultConfigFile);
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

        #region Interactive Options

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

        public static ReadOnlyCollection<ListItem> PpdCalculationList
        {
            get
            {
                var list = new List<ListItem>
                       {
                          new ListItem
                          { DisplayMember = "Last Frame", ValueMember = PPDCalculation.LastFrame },
                          new ListItem
                          { DisplayMember = "Last Three Frames", ValueMember = PPDCalculation.LastThreeFrames },
                          new ListItem
                          { DisplayMember = "All Frames", ValueMember = PPDCalculation.AllFrames },
                          new ListItem
                          { DisplayMember = "Effective Rate", ValueMember = PPDCalculation.EffectiveRate }
                       };
                return list.AsReadOnly();
            }
        }

        public static ReadOnlyCollection<ListItem> BonusCalculationList
        {
            get
            {
                var list = new List<ListItem>
                       {
                          new ListItem
                          { DisplayMember = "Download Time", ValueMember = BonusCalculation.DownloadTime },
                          new ListItem
                          { DisplayMember = "Frame Time", ValueMember = BonusCalculation.FrameTime },
                          new ListItem
                          { DisplayMember = "None", ValueMember = BonusCalculation.None },
                       };
                return list.AsReadOnly();
            }
        }
    }
}
