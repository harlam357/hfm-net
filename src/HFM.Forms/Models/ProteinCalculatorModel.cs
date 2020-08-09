using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms.Models
{
    public class ProteinCalculatorModel : INotifyPropertyChanged
    {
        private readonly IPreferenceSet _preferences;
        private readonly IProteinService _proteinService;

        public ProteinCalculatorModel(IPreferenceSet preferences, IProteinService proteinService)
        {
            _preferences = preferences ?? new InMemoryPreferenceSet();
            _proteinService = proteinService ?? NullProteinService.Instance;
        }

        public void Calculate()
        {
            Protein value = _proteinService.Get(SelectedProject);
            if (value == null)
            {
                return;
            }

            Protein protein = value.Copy();
            if (PreferredDeadlineChecked) protein.PreferredDays = PreferredDeadline;
            if (FinalDeadlineChecked) protein.MaximumDays = FinalDeadline;
            if (KFactorChecked) protein.KFactor = KFactor;

            TimeSpan frameTime = TimeSpan.FromMinutes(TpfMinutes).Add(TimeSpan.FromSeconds(TpfSeconds));
            TimeSpan totalTimeByFrame = TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames);
            TimeSpan totalTimeByUser = totalTimeByFrame;
            if (TotalWuTimeEnabled)
            {
                totalTimeByUser = TimeSpan.FromMinutes(TotalWuTimeMinutes).Add(TimeSpan.FromSeconds(TotalWuTimeSeconds));
                // user time is less than total time by frame, not permitted
                if (totalTimeByUser < totalTimeByFrame)
                {
                    totalTimeByUser = totalTimeByFrame;
                }
            }

            var decimalPlaces = _preferences.Get<int>(Preference.DecimalPlaces);
            var noBonusValues = protein.GetProductionValues(frameTime, TimeSpan.Zero);
            var bonusByUserSpecifiedTimeValues = protein.GetProductionValues(frameTime, totalTimeByUser);
            var bonusByFrameTimeValues = protein.GetProductionValues(frameTime, totalTimeByFrame);
            CoreName = protein.Core;
            SlotType = SlotTypeConvert.FromCoreName(protein.Core).ToString();
            NumberOfAtoms = protein.NumberOfAtoms;
            CompletionTime = Math.Round(TotalWuTimeEnabled ? totalTimeByUser.TotalDays : totalTimeByFrame.TotalDays, decimalPlaces);
            PreferredDeadline = protein.PreferredDays;
            FinalDeadline = protein.MaximumDays;
            KFactor = protein.KFactor;
            BonusMultiplier = Math.Round(TotalWuTimeEnabled ? bonusByUserSpecifiedTimeValues.Multiplier : bonusByFrameTimeValues.Multiplier, decimalPlaces);
            BaseCredit = noBonusValues.Credit;
            TotalCredit = Math.Round(TotalWuTimeEnabled ? bonusByUserSpecifiedTimeValues.Credit : bonusByFrameTimeValues.Credit, decimalPlaces);
            BasePpd = noBonusValues.PPD;
            TotalPpd = Math.Round(TotalWuTimeEnabled ? bonusByUserSpecifiedTimeValues.PPD : bonusByFrameTimeValues.PPD, decimalPlaces);
        }

        public ICollection<int> Projects => _proteinService.GetProjects().OrderBy(x => x).ToList();

        private int _selectedProject;

        public int SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (_selectedProject != value)
                {
                    _selectedProject = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _tpfMinutes;

        public int TpfMinutes
        {
            get => _tpfMinutes;
            set
            {
                if (_tpfMinutes != value)
                {
                    _tpfMinutes = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _tpfSeconds;

        public int TpfSeconds
        {
            get => _tpfSeconds;
            set
            {
                if (_tpfSeconds != value)
                {
                    _tpfSeconds = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _totalWuTimeEnabled;

        public bool TotalWuTimeEnabled
        {
            get => _totalWuTimeEnabled;
            set
            {
                if (_totalWuTimeEnabled != value)
                {
                    _totalWuTimeEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalWuTimeMinutes;

        public int TotalWuTimeMinutes
        {
            get => _totalWuTimeMinutes;
            set
            {
                if (_totalWuTimeMinutes != value)
                {
                    _totalWuTimeMinutes = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalWuTimeSeconds;

        public int TotalWuTimeSeconds
        {
            get => _totalWuTimeSeconds;
            set
            {
                if (_totalWuTimeSeconds != value)
                {
                    _totalWuTimeSeconds = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _coreName;

        public string CoreName
        {
            get => _coreName;
            set
            {
                if (_coreName != value)
                {
                    _coreName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _slotType;

        public string SlotType
        {
            get => _slotType;
            set
            {
                if (_slotType != value)
                {
                    _slotType = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _numberOfAtoms;

        public int NumberOfAtoms
        {
            get => _numberOfAtoms;
            set
            {
                if (_numberOfAtoms != value)
                {
                    _numberOfAtoms = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _completionTime;

        public double CompletionTime
        {
            get => _completionTime;
            set
            {
                if (_completionTime != value)
                {
                    _completionTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _preferredDeadline;

        public double PreferredDeadline
        {
            get => _preferredDeadline;
            set
            {
                if (_preferredDeadline != value)
                {
                    _preferredDeadline = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _preferredDeadlineChecked;

        public bool PreferredDeadlineChecked
        {
            get => _preferredDeadlineChecked;
            set
            {
                if (_preferredDeadlineChecked != value)
                {
                    _preferredDeadlineChecked = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PreferredDeadlineIsReadOnly));
                }
            }
        }

        public bool PreferredDeadlineIsReadOnly => !PreferredDeadlineChecked;

        private double _finalDeadline;

        public double FinalDeadline
        {
            get => _finalDeadline;
            set
            {
                if (_finalDeadline != value)
                {
                    _finalDeadline = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _finalDeadlineChecked;

        public bool FinalDeadlineChecked
        {
            get => _finalDeadlineChecked;
            set
            {
                if (_finalDeadlineChecked != value)
                {
                    _finalDeadlineChecked = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FinalDeadlineIsReadOnly));
                }
            }
        }

        public bool FinalDeadlineIsReadOnly => !FinalDeadlineChecked;

        private double _kFactor;

        public double KFactor
        {
            get => _kFactor;
            set
            {
                if (_kFactor != value)
                {
                    _kFactor = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _kFactorChecked;

        public bool KFactorChecked
        {
            get => _kFactorChecked;
            set
            {
                if (_kFactorChecked != value)
                {
                    _kFactorChecked = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(KFactorIsReadOnly));
                }
            }
        }

        public bool KFactorIsReadOnly => !KFactorChecked;

        private double _bonusMultiplier;

        public double BonusMultiplier
        {
            get => _bonusMultiplier;
            set
            {
                if (_bonusMultiplier != value)
                {
                    _bonusMultiplier = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _baseCredit;

        public double BaseCredit
        {
            get => _baseCredit;
            set
            {
                if (_baseCredit != value)
                {
                    _baseCredit = value;
                    OnPropertyChanged();
                }
            }
        }

        public double BonusCredit => TotalCredit - BaseCredit;

        private double _totalCredit;

        public double TotalCredit
        {
            get => _totalCredit;
            set
            {
                if (_totalCredit != value)
                {
                    _totalCredit = value;
                    OnPropertyChanged(nameof(BonusCredit));
                    OnPropertyChanged();
                }
            }
        }

        private double _basePpd;

        public double BasePpd
        {
            get => _basePpd;
            set
            {
                if (_basePpd != value)
                {
                    _basePpd = value;
                    OnPropertyChanged();
                }
            }
        }

        public double BonusPpd => TotalPpd - BasePpd;

        private double _totalPpd;

        public double TotalPpd
        {
            get => _totalPpd;
            set
            {
                if (_totalPpd != value)
                {
                    _totalPpd = value;
                    OnPropertyChanged(nameof(BonusPpd));
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
