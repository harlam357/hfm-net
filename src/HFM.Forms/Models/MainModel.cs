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
            FormLogWindowVisible = Preferences.Get<bool>(Preference.FormLogWindowVisible);
            FormLogWindowHeight = Preferences.Get<int>(Preference.FormLogWindowHeight);
            FormSplitterLocation = Preferences.Get<int>(Preference.FormSplitterLocation);
            QueueWindowVisible = Preferences.Get<bool>(Preference.QueueWindowVisible);
            FollowLog = Preferences.Get<bool>(Preference.FollowLog);
        }

        public override void Save()
        {
            Preferences.Set(Preference.FormLocation, FormLocation);
            Preferences.Set(Preference.FormSize, FormSize);
            Preferences.Set(Preference.FormLogWindowVisible, FormLogWindowVisible);
            Preferences.Set(Preference.FormLogWindowHeight, FormLogWindowHeight);
            Preferences.Set(Preference.FormSplitterLocation, FormSplitterLocation);
            Preferences.Set(Preference.QueueWindowVisible, QueueWindowVisible);
            Preferences.Set(Preference.FollowLog, FollowLog);

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

        private bool _formLogWindowVisible;

        public bool FormLogWindowVisible
        {
            get => _formLogWindowVisible;
            set
            {
                _formLogWindowVisible = value;
                OnPropertyChanged();
            }
        }

        public int FormLogWindowHeight { get; set; }

        public int FormSplitterLocation { get; set; }

        private bool _queueWindowVisible;

        public bool QueueWindowVisible
        {
            get => _queueWindowVisible;
            set
            {
                _queueWindowVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _followLog;

        public bool FollowLog
        {
            get => _followLog;
            set
            {
                _followLog = value;
                OnPropertyChanged();
            }
        }

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
