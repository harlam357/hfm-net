using System;
using System.Drawing;
using System.Windows.Forms;

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
            MinimizeTo = Preferences.Get<MinimizeToOption>(Preference.MinimizeTo);

            Preferences.PreferenceChanged += (s, e) =>
            {
                switch (e.Preference)
                {
                    case Preference.MinimizeTo:
                        MinimizeTo = Preferences.Get<MinimizeToOption>(e.Preference);
                        break;
                }
            };
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

        public void GridModelSelectedSlotChanged(object sender, EventArgs e)
        {
            var selectedSlot = (sender as MainGridModel)?.SelectedSlot;
            if (selectedSlot != null)
            {
                ClientDetails = selectedSlot.SlotIdentifier.ClientIdentifier.ToServerPortString();
            }
        }

        /// <summary>
        /// Holds the state of the window before it is hidden (minimize to tray behaviour)
        /// </summary>
        public FormWindowState OriginalWindowState { get; private set; }

        private FormWindowState _windowState;

        public FormWindowState WindowState
        {
            get => _windowState;
            set
            {
                if (_windowState != value)
                {
                    OriginalWindowState = _windowState;
                    _windowState = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ShowInTaskbar));
                }
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
                if (_formLogWindowVisible != value)
                {
                    _formLogWindowVisible = value;
                    OnPropertyChanged();
                }
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
                if (_queueWindowVisible != value)
                {
                    _queueWindowVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _followLog;

        public bool FollowLog
        {
            get => _followLog;
            set
            {
                if (_followLog != value)
                {
                    _followLog = value;
                    OnPropertyChanged();
                }
            }
        }

        private MinimizeToOption _minimizeTo;

        public MinimizeToOption MinimizeTo
        {
            get => _minimizeTo;
            set
            {
                if (_minimizeTo != value)
                {
                    _minimizeTo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NotifyIconVisible));
                    OnPropertyChanged(nameof(ShowInTaskbar));
                }
            }
        }

        public bool NotifyIconVisible => MinimizeTo == MinimizeToOption.SystemTray || MinimizeTo == MinimizeToOption.Both;

        public bool ShowInTaskbar
        {
            get
            {
                if (WindowState != FormWindowState.Minimized) return true;
                return MinimizeTo == MinimizeToOption.TaskBar || MinimizeTo == MinimizeToOption.Both;
            }
        }

        private string _clientDetails;

        public string ClientDetails
        {
            get => _clientDetails;
            set
            {
                if (_clientDetails != value)
                {
                    _clientDetails = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
