using System.Drawing;

using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class MainModel : ViewModelBase
    {
        public IPreferences Preferences { get; }

        public MainModel(IPreferences preferences)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
        }

        public override void Load()
        {
            FormLocation = Preferences.Get<Point>(Preference.FormLocation);
            FormSize = Preferences.Get<Size>(Preference.FormSize);
        }

        public override void Save()
        {
            Preferences.Set(Preference.FormLocation, FormLocation);
            Preferences.Set(Preference.FormSize, FormSize);

            Preferences.Save();
        }

        public void GridModelSelectedSlotChanged(object sender, IndexChangedEventArgs e)
        {
            var selectedSlot = (sender as MainGridModel)?.SelectedSlot;
            if (selectedSlot != null)
            {
                ClientDetails = selectedSlot.SlotIdentifier.ClientIdentifier.ToServerPortString();
            }
        }

        public Point FormLocation { get; set; }

        public Size FormSize { get; set; }

        private string _clientDetails;

        public string ClientDetails
        {
            get => _clientDetails;
            set
            {
                _clientDetails = value;
                OnPropertyChanged();
            }
        }
    }
}
