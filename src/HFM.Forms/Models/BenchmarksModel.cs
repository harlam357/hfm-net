using System.Collections.Generic;
using System.Drawing;

using HFM.Core.WorkUnits;
using HFM.Forms.Views;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms.Models
{
    public class BenchmarksModel : ViewModelBase
    {
        public IPreferenceSet Preferences { get; }

        public BenchmarksModel(IPreferenceSet preferences)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
        }

        public int ProjectID { get; set; }

        public override void Load()
        {
            FormLocation = Preferences.Get<Point>(Preference.BenchmarksFormLocation);
            FormSize = Preferences.Get<Size>(Preference.BenchmarksFormSize);
            GraphLayoutType = Preferences.Get<GraphLayoutType>(Preference.BenchmarksGraphLayoutType);
            ClientsPerGraph = Preferences.Get<int>(Preference.BenchmarksClientsPerGraph);
            GraphColors = Preferences.Get<List<Color>>(Preference.GraphColors);
        }

        public override void Save()
        {
            Preferences.Set(Preference.BenchmarksFormLocation, FormLocation);
            Preferences.Set(Preference.BenchmarksFormSize, FormSize);
            Preferences.Set(Preference.BenchmarksGraphLayoutType, GraphLayoutType);
            Preferences.Set(Preference.BenchmarksClientsPerGraph, ClientsPerGraph);
            Preferences.Set(Preference.GraphColors, GraphColors);
            Preferences.Save();
        }

        public int DecimalPlaces => Preferences.Get<int>(Preference.DecimalPlaces);

        public BonusCalculation BonusCalculation => Preferences.Get<BonusCalculation>(Preference.BonusCalculation);

        public Point FormLocation { get; set; }

        public Size FormSize { get; set; }

        public GraphLayoutType GraphLayoutType { get; set; }

        public List<Color> GraphColors { get; private set; }

        public int ClientsPerGraph { get; set; }

        private Protein _protein;

        public Protein Protein
        {
            get => _protein;
            set
            {
                if (_protein != value)
                {
                    _protein = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(WorkUnitName));
                    OnPropertyChanged(nameof(Credit));
                    OnPropertyChanged(nameof(KFactor));
                    OnPropertyChanged(nameof(Frames));
                    OnPropertyChanged(nameof(NumberOfAtoms));
                    OnPropertyChanged(nameof(Core));
                    OnPropertyChanged(nameof(DescriptionUrl));
                    OnPropertyChanged(nameof(PreferredDays));
                    OnPropertyChanged(nameof(MaximumDays));
                    OnPropertyChanged(nameof(Contact));
                    OnPropertyChanged(nameof(ServerIP));
                }
            }
        }

        public string WorkUnitName => Protein?.WorkUnitName;

        public double? Credit => Protein?.Credit;

        public double? KFactor => Protein?.KFactor;

        public int? Frames => Protein?.Frames;

        public int? NumberOfAtoms => Protein?.NumberOfAtoms;

        public string Core => Protein?.Core;

        public string DescriptionUrl => Protein?.Description;

        public double? PreferredDays => Protein?.PreferredDays;

        public double? MaximumDays => Protein?.MaximumDays;

        public string Contact => Protein?.Contact;

        public string ServerIP => Protein?.ServerIP;
    }
}
