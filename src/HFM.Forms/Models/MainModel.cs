using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Preferences;

namespace HFM.Forms.Models;

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
        QueueSplitterLocation = Preferences.Get<int>(Preference.QueueSplitterLocation);
        FollowLog = Preferences.Get<bool>(Preference.FollowLog);
        FormColumns.Reset(Preferences.Get<ICollection<string>>(Preference.FormColumns));

        // changed by preferences dialog
        MinimizeTo = Preferences.Get<MinimizeToOption>(Preference.MinimizeTo);
        ColorLogFile = Preferences.Get<bool>(Preference.ColorLogFile);

        Preferences.PreferenceChanged += (s, e) =>
        {
            string toolTip;
            switch (e.Preference)
            {
                case Preference.MinimizeTo:
                    MinimizeTo = Preferences.Get<MinimizeToOption>(e.Preference);
                    break;
                case Preference.ColorLogFile:
                    ColorLogFile = Preferences.Get<bool>(e.Preference);
                    break;
                case Preference.BonusCalculation:
                    toolTip = ClientsModel.BonusCalculationList
                        .FirstOrDefault(x => x.GetValue<BonusCalculation>() == Preferences.Get<BonusCalculation>(Preference.BonusCalculation)).DisplayMember;
                    NotifyToolTip = toolTip;
                    break;
                case Preference.PPDCalculation:
                    toolTip = ClientsModel.PPDCalculationList
                        .FirstOrDefault(x => x.GetValue<PPDCalculation>() == Preferences.Get<PPDCalculation>(Preference.PPDCalculation)).DisplayMember;
                    NotifyToolTip = toolTip;
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
        Preferences.Set(Preference.QueueSplitterLocation, QueueSplitterLocation);
        Preferences.Set(Preference.FollowLog, FollowLog);
        Preferences.Set(Preference.FormColumns, FormColumns);

        Preferences.Save();
    }

    public void GridModelSelectedClientChanged(IClientData selectedClient)
    {
        if (selectedClient is null)
        {
            ClientDetails = null;
        }
        else
        {
            ClientDetails = selectedClient.SlotIdentifier.ClientIdentifier.ToConnectionString();
            var settings = selectedClient.Settings;
            if (settings != null && settings.Disabled)
            {
                ClientDetails += " (Disabled)";
            }
        }
    }

    public void GridModelSlotTotalsChanged(SlotTotals totals)
    {
        string numberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces));

        NotifyIconText = String.Format("{0} Working Slots{3}{1} Idle Slots{3}{2} PPD",
            totals.WorkingSlots, totals.NonWorkingSlots, totals.PPD.ToString(numberFormat), Environment.NewLine);

        string slots = "Slots";
        if (totals.TotalSlots == 1)
        {
            slots = "Slot";
        }

        int percentWorking = 0;
        if (totals.TotalSlots > 0)
        {
            percentWorking = ((totals.WorkingSlots * 200) + totals.TotalSlots) / (totals.TotalSlots * 2);
        }

        WorkingSlotsText = $"{totals.WorkingSlots} of {totals.TotalSlots} {slots} ({percentWorking}%)";
        TotalProductionText = $"{totals.PPD.ToString(numberFormat)} PPD";
    }

    /// <summary>
    /// Gets the previous state of the window.
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

    private int _queueWindowLocation;

    public int QueueSplitterLocation
    {
        get => _queueWindowLocation;
        set
        {
            if (_queueWindowLocation != value)
            {
                _queueWindowLocation = value;
                OnPropertyChanged();
            }
        }
    }

    public ICollection<string> FormColumns { get; } = new List<string>();

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

    private bool _colorLogFile;

    public bool ColorLogFile
    {
        get => _colorLogFile;
        set
        {
            if (_colorLogFile != value)
            {
                _colorLogFile = value;
                OnPropertyChanged();
            }
        }
    }

    public bool NotifyIconVisible => MinimizeTo == MinimizeToOption.SystemTray || MinimizeTo == MinimizeToOption.Both;

    private string _notifyIconText;

    public string NotifyIconText
    {
        get => _notifyIconText;
        set
        {
            _notifyIconText = value;
            OnPropertyChanged();
        }
    }

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

    private string _workingSlotsText;

    public string WorkingSlotsText
    {
        get => _workingSlotsText;
        set
        {
            _workingSlotsText = value;
            OnPropertyChanged();
        }
    }

    private string _totalProductionText;

    public string TotalProductionText
    {
        get => _totalProductionText;
        set
        {
            _totalProductionText = value;
            OnPropertyChanged();
        }
    }

    private string _notifyToolTip;

    public string NotifyToolTip
    {
        get => _notifyToolTip;
        set
        {
            _notifyToolTip = value;
            OnPropertyChanged();
        }
    }
}
