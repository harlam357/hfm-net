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
            BindingSource.CurrentItemChanged += (sender, args) => SelectedSlot = (SlotModel)BindingSource.Current;

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

        // SortColumnName stores the actual SlotModel property name
        // this method guards against SlotModel property name changes
        private string ValidateSortColumnNameOrDefault(string name)
        {
            var properties = TypeDescriptor.GetProperties(typeof(SlotModel));
            var property = properties.Find(name, true);
            return property is null ? DefaultSortColumnName : name;
        }

        private string DefaultSortColumnName { get; } = nameof(SlotModel.Name);

        public ListSortDirection SortColumnOrder { get; set; }

        private SlotModel _selectedSlot;

        public SlotModel SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                if (!ReferenceEquals(_selectedSlot, value))
                {
                    _selectedSlot = value;
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

        private readonly object _resetBindingsLock = new object();

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

        private readonly object _slotsListLock = new object();

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
                SlotModel.ValidateRules(collection, Preferences);
                SlotTotals = SlotTotals.Create(collection);

                BindingSource.ResetBindings(false);
            }
            OnReset(new SlotCollectionModelResetEventArgs(SelectedSlot,
                                                          SelectedSlot?.WorkUnitQueue,
                                                          SelectedSlot?.CurrentLogLines,
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
            if (SelectedSlot == null) return;

            int row = BindingSource.Find(nameof(SlotModel.Name), SelectedSlot.Name);
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
        public SlotCollectionModelResetEventArgs(SlotModel selectedSlot,
                                                 WorkUnitQueueItemCollection workUnitQueue,
                                                 IReadOnlyCollection<LogLine> logLines,
                                                 SlotTotals slotTotals)
        {
            SelectedSlot = selectedSlot;
            WorkUnitQueue = workUnitQueue;
            LogLines = logLines;
            SlotTotals = slotTotals;
        }

        public SlotModel SelectedSlot { get; }
        public WorkUnitQueueItemCollection WorkUnitQueue { get; }
        public IReadOnlyCollection<LogLine> LogLines { get; }
        public SlotTotals SlotTotals { get; }
    }
}
