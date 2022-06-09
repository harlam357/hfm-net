using System.ComponentModel;
using System.Diagnostics;

using HFM.Core.Client;
using HFM.Forms.Internal;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public sealed class SlotCollectionModel : ViewModelBase
    {
        private readonly ISynchronizeInvoke _synchronizeInvoke;
        private readonly ClientDataSortableBindingList _clientDataList;

        public IPreferences Preferences { get; }

        public ClientConfiguration ClientConfiguration { get; }

        public BindingSource BindingSource { get; }

        public SlotCollectionModel(ISynchronizeInvoke synchronizeInvoke, IPreferences preferences, ClientConfiguration clientConfiguration)
        {
            _synchronizeInvoke = synchronizeInvoke ?? throw new ArgumentNullException(nameof(synchronizeInvoke));
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            ClientConfiguration = clientConfiguration ?? throw new ArgumentNullException(nameof(clientConfiguration));

            _clientDataList = new ClientDataSortableBindingList();
            _clientDataList.RaiseListChangedEvents = false;
            BindingSource = new BindingSource();
        }

        public override void Load()
        {
            SortColumnName = Preferences.Get<string>(Preference.FormSortColumn);
            SortColumnOrder = Preferences.Get<ListSortDirection>(Preference.FormSortOrder);

            _clientDataList.OfflineClientsLast = Preferences.Get<bool>(Preference.OfflineLast);
            _clientDataList.Sorted += (sender, e) =>
            {
                SortColumnName = e.Name;
                Preferences.Set(Preference.FormSortColumn, SortColumnName);
                SortColumnOrder = e.Direction;
                Preferences.Set(Preference.FormSortOrder, SortColumnOrder);
            };
            BindingSource.DataSource = _clientDataList;
            BindingSource.CurrentItemChanged += (sender, args) => SelectedClient = (IClientData)BindingSource.Current;

            // subscribe to services raising events that require a view action
            Preferences.PreferenceChanged += (s, e) => OnPreferenceChanged(e);
            ClientConfiguration.ClientConfigurationChanged += (s, e) => ResetBindings();
        }

        private string _sortColumnName;

        public string SortColumnName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_sortColumnName))
                {
                    _sortColumnName = DefaultSortColumnName;
                }
                return _sortColumnName;
            }
            set
            {
                if (_sortColumnName != value)
                {
                    _sortColumnName = String.IsNullOrWhiteSpace(value)
                        ? DefaultSortColumnName
                        : ValidateSortColumnNameOrDefault(value);
                }
            }
        }

        // SortColumnName stores the actual IClientData property name
        // this method guards against IClientData property name changes
        private string ValidateSortColumnNameOrDefault(string name)
        {
            var properties = TypeDescriptor.GetProperties(typeof(IClientData));
            var property = properties.Find(name, true);
            return property is null ? DefaultSortColumnName : name;
        }

        private string DefaultSortColumnName => nameof(IClientData.Name);

        public ListSortDirection SortColumnOrder { get; set; }

        private IClientData _selectedClient;

        public IClientData SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (!ReferenceEquals(_selectedClient, value))
                {
                    _selectedClient = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPreferenceChanged(PreferenceChangedEventArgs e)
        {
            switch (e.Preference)
            {
                case Preference.OfflineLast:
                    _clientDataList.OfflineClientsLast = Preferences.Get<bool>(Preference.OfflineLast);
                    Sort();
                    break;
                case Preference.PPDCalculation:
                case Preference.DecimalPlaces:
                case Preference.BonusCalculation:
                case Preference.HideInactiveSlots:
                    ResetBindings();
                    break;
            }
        }

        private readonly object _resetBindingsLock = new();

        private void ResetBindings()
        {
            if (!Monitor.TryEnter(_resetBindingsLock))
            {
                Debug.WriteLine("Reset already in progress...");
                return;
            }
            try
            {
                ResetBindingsInternal();
            }
            finally
            {
                Monitor.Exit(_resetBindingsLock);
            }
        }

        private readonly object _slotsListLock = new();

        private SlotTotals _slotTotals;

        private SlotTotals SlotTotals
        {
            get => _slotTotals;
            set
            {
                _slotTotals = value;
                OnPropertyChanged();
            }
        }

        private void ResetBindingsInternal()
        {
            if (_synchronizeInvoke is Control control && control.IsDisposed)
            {
                return;
            }
            if (_synchronizeInvoke.InvokeRequired)
            {
                _synchronizeInvoke.BeginInvoke(new Action(ResetBindingsInternal), null);
                return;
            }

            lock (_slotsListLock)
            {
                var collection = FilterCollection(ClientConfiguration.GetClientDataCollection());

                // refresh the underlying binding list
                BindingSource.Clear();
                foreach (var slot in collection)
                {
                    BindingSource.Add(slot);
                }
                Debug.WriteLine("Number of slots: {0}", BindingSource.Count);

                SortInternal();
                ResetSelectedSlot();
                ClientData.ValidateRules(collection, Preferences);
                SlotTotals = SlotTotals.Create(collection);

                BindingSource.ResetBindings(false);
            }
            OnReset(new SlotCollectionModelResetEventArgs(SelectedClient,
                                                          (SelectedClient as FahClientData)?.WorkUnitQueue,
                                                          SelectedClient?.CurrentLogLines,
                                                          SlotTotals));
        }

        public bool HideInactiveSlots
        {
            get => Preferences.Get<bool>(Preference.HideInactiveSlots);
            set => Preferences.Set(Preference.HideInactiveSlots, value);
        }

        private ICollection<IClientData> FilterCollection(ICollection<IClientData> collection)
        {
            if (HideInactiveSlots)
            {
                collection = collection.Where(x => x.Status != SlotStatus.Offline && x.Status != SlotStatus.Paused).ToList();
            }
            return collection;
        }

        private void Sort()
        {
            lock (_slotsListLock)
            {
                SortInternal();
            }
        }

        private void SortInternal()
        {
            BindingSource.Sort = $"{SortColumnName} {SortColumnOrder.ToBindingSourceSortString()}";
            if (_clientDataList is IBindingList bindingList)
            {
                bindingList.ApplySort(bindingList.SortProperty, bindingList.SortDirection);
            }
        }

        public void ResetSelectedSlot()
        {
            if (SelectedClient == null) return;

            int row = BindingSource.Find(nameof(IClientData.Name), SelectedClient.Name);
            if (row > -1)
            {
                BindingSource.Position = row;
            }
        }

        public event EventHandler<SlotCollectionModelResetEventArgs> Reset;

        private void OnReset(SlotCollectionModelResetEventArgs e) => Reset?.Invoke(this, e);
    }

    public class SlotCollectionModelResetEventArgs : EventArgs
    {
        public SlotCollectionModelResetEventArgs(IClientData selectedClient,
                                                 WorkUnitQueueItemCollection workUnitQueue,
                                                 IReadOnlyCollection<LogLine> logLines,
                                                 SlotTotals slotTotals)
        {
            SelectedClient = selectedClient;
            WorkUnitQueue = workUnitQueue;
            LogLines = logLines;
            SlotTotals = slotTotals;
        }

        public IClientData SelectedClient { get; }
        public WorkUnitQueueItemCollection WorkUnitQueue { get; }
        public IReadOnlyCollection<LogLine> LogLines { get; }
        public SlotTotals SlotTotals { get; }
    }
}
