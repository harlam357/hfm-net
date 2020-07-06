
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
            ShowXmlStats = Preferences.Get<bool>(Preference.EnableUserStats);
            MessageLevel = (LoggerLevel)Preferences.Get<int>(Preference.MessageLevel);
            FormShowStyle = Preferences.Get<MinimizeToOption>(Preference.MinimizeTo);
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
            Preferences.Set(Preference.EnableUserStats, ShowXmlStats);
            Preferences.Set(Preference.MessageLevel, (int)MessageLevel);
            Preferences.Set(Preference.MinimizeTo, FormShowStyle);
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

        private bool _showXmlStats;

        public bool ShowXmlStats
        {
            get { return _showXmlStats; }
            set
            {
                if (ShowXmlStats != value)
                {
                    _showXmlStats = value;
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

        public static ReadOnlyCollection<ListItem> DebugList
        {
            get
            {
                var list = new List<ListItem>
                       {
                          //new ListItem
                          //{ DisplayMember = LoggerLevel.Off.ToString(), ValueMember = LoggerLevel.Off },
                          //new ListItem
                          //{ DisplayMember = LoggerLevel.Fatal.ToString(), ValueMember = LoggerLevel.Fatal },
                          //new ListItem
                          //{ DisplayMember = LoggerLevel.Error.ToString(), ValueMember = LoggerLevel.Error },
                          //new ListItem
                          //{ DisplayMember = LoggerLevel.Warn.ToString(), ValueMember = LoggerLevel.Warn },
                          new ListItem
                          { DisplayMember = LoggerLevel.Info.ToString(), ValueMember = LoggerLevel.Info },
                          new ListItem
                          { DisplayMember = LoggerLevel.Debug.ToString(), ValueMember = LoggerLevel.Debug }
                       };
                return list.AsReadOnly();
            }
        }

        public static ReadOnlyCollection<ListItem> DockingStyleList
        {
            get
            {
                var list = new List<ListItem>
                       {
                          new ListItem
                          { DisplayMember = "System Tray", ValueMember = MinimizeToOption.SystemTray },
                          new ListItem
                          { DisplayMember = "Task Bar", ValueMember = MinimizeToOption.TaskBar },
                          new ListItem
                          { DisplayMember = "Both", ValueMember = MinimizeToOption.Both }
                       };
                return list.AsReadOnly();
            }
        }
    }
}
