using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public sealed class WorkUnitHistoryModel : ViewModelBase
    {
        public IPreferences Preferences { get; }
        public WorkUnitQueryDataContainer QueryContainer { get; }
        public IWorkUnitRepository Repository { get; }
        public BindingSource QueryBindingSource { get; }
        public BindingSource HistoryBindingSource { get; }

        private readonly List<WorkUnitQuery> _queryList;
        private readonly WorkUnitRowSortableBindingList _workUnitList;
        private Page<WorkUnitRow> _page;

        public WorkUnitHistoryModel(IPreferences preferences, WorkUnitQueryDataContainer queryContainer, IWorkUnitRepository repository)
        {
            Preferences = preferences;
            QueryContainer = queryContainer;
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _queryList = new List<WorkUnitQuery> { WorkUnitQuery.SelectAll };
            QueryBindingSource = new BindingSource();
            QueryBindingSource.DataSource = _queryList;
            QueryBindingSource.CurrentItemChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(EditAndDeleteButtonsEnabled));
                _currentPage = 1;
                ResetBindings(true);
            };

            _workUnitList = new WorkUnitRowSortableBindingList();
            _workUnitList.RaiseListChangedEvents = false;
            _workUnitList.Sorted += (s, e) =>
            {
                SortColumnName = e.Name;
                SortOrder = e.Direction;
            };
            HistoryBindingSource = new BindingSource();
            HistoryBindingSource.DataSource = _workUnitList;

            _page = new Page<WorkUnitRow> { Items = new List<WorkUnitRow>() };
        }

        public override void Load()
        {
            _bonusCalculation = Preferences.Get<BonusCalculation>(Preference.HistoryBonusCalculation);
            _showEntriesValue = Preferences.Get<int>(Preference.ShowEntriesValue);
            FormLocation = Preferences.Get<Point>(Preference.HistoryFormLocation);
            FormSize = Preferences.Get<Size>(Preference.HistoryFormSize);
            SortColumnName = Preferences.Get<string>(Preference.HistorySortColumnName);
            SortOrder = Preferences.Get<ListSortDirection>(Preference.HistorySortOrder);
            FormColumns = Preferences.Get<ICollection<string>>(Preference.HistoryFormColumns);

            _queryList.Clear();
            _queryList.Add(WorkUnitQuery.SelectAll);
            _queryList.AddRange(QueryContainer.Data.Where(q => q != WorkUnitQuery.SelectAll));
            _queryList.Sort();
            ResetBindings(true);
        }

        public override void Save()
        {
            Preferences.Set(Preference.HistoryBonusCalculation, _bonusCalculation);
            Preferences.Set(Preference.ShowEntriesValue, _showEntriesValue);
            Preferences.Set(Preference.HistoryFormLocation, FormLocation);
            Preferences.Set(Preference.HistoryFormSize, FormSize);
            Preferences.Set(Preference.HistorySortColumnName, SortColumnName);
            Preferences.Set(Preference.HistorySortOrder, SortOrder);
            Preferences.Set(Preference.HistoryFormColumns, FormColumns);
            Preferences.Save();

            QueryContainer.Data.Clear();
            QueryContainer.Data.AddRange(_queryList.Where(q => q != WorkUnitQuery.SelectAll));
            QueryContainer.Write();
        }

        public void AddQuery(WorkUnitQuery query)
        {
            CheckQueryParametersForAddOrReplace(query);

            if (_queryList.FirstOrDefault(x => x.Name == query.Name) != null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                   "A query with name '{0}' already exists.", query.Name));
            }

            _queryList.Add(query);
            _queryList.Sort();
            QueryBindingSource.ResetBindings(false);
            QueryBindingSource.Position = QueryBindingSource.IndexOf(query);
        }

        public void ReplaceQuery(WorkUnitQuery query)
        {
            if (SelectedWorkUnitQuery == WorkUnitQuery.SelectAll)
            {
                throw new ArgumentException($"Cannot replace the '{WorkUnitQuery.SelectAll}' query.");
            }

            CheckQueryParametersForAddOrReplace(query);

            var existing = _queryList.FirstOrDefault(x => x.Name == query.Name);
            if (existing != null && existing.Name != SelectedWorkUnitQuery.Name)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                   "A query with name '{0}' already exists.", query.Name));
            }

            _queryList.Remove(SelectedWorkUnitQuery);
            _queryList.Add(query);
            _queryList.Sort();
            QueryBindingSource.ResetBindings(false);
            QueryBindingSource.Position = QueryBindingSource.IndexOf(query);
        }

        private static void CheckQueryParametersForAddOrReplace(WorkUnitQuery query)
        {
            if (query == WorkUnitQuery.SelectAll)
            {
                throw new ArgumentException($"Query name cannot be '{WorkUnitQuery.SelectAll}'.");
            }

            if (query.Parameters.Count == 0)
            {
                throw new ArgumentException("No query fields defined.");
            }

            for (int i = 0; i < query.Parameters.Count; i++)
            {
                if (query.Parameters[i].Value == null)
                {
                    throw new ArgumentException($"Parameter {i + 1} must have a value.");
                }
            }
        }

        public void RemoveQuery(WorkUnitQuery query)
        {
            if (query == WorkUnitQuery.SelectAll)
            {
                throw new ArgumentException($"Cannot remove '{WorkUnitQuery.SelectAll}' query.");
            }

            _queryList.Remove(query);
            QueryBindingSource.ResetBindings(false);
        }

        public void DeleteHistoryEntry(WorkUnitRow row)
        {
            if (Repository.Delete(row) != 0)
            {
                _page.Items.Remove(row);
                _page.TotalItems--;
                ResetBindings(false);
            }
        }

        public void ResetBindings(bool executeQuery)
        {
            Debug.Assert(SelectedWorkUnitQuery != null);

            if (executeQuery)
            {
                _page = Repository.Page(CurrentPage, ShowEntriesValue, SelectedWorkUnitQuery, BonusCalculation);
            }
            if (_page == null)
            {
                return;
            }

            // refresh the underlying binding list
            RefreshHistoryList(_page.Items);

            // sort the list
            HistoryBindingSource.Sort = null;
            if (!String.IsNullOrEmpty(SortColumnName))
            {
                HistoryBindingSource.Sort = SortColumnName + " " + SortOrder.ToBindingSourceSortString();
                _workUnitList.ApplySort(_workUnitList.SortDescriptions);
            }

            HistoryBindingSource.ResetBindings(false);

            OnPropertyChanged(nameof(TotalEntries));
            OnPropertyChanged(nameof(CurrentPage));
        }

        private void RefreshHistoryList(IEnumerable<WorkUnitRow> historyEntries)
        {
            HistoryBindingSource.Clear();
            if (historyEntries != null)
            {
                foreach (var entry in historyEntries)
                {
                    HistoryBindingSource.Add(entry);
                }
            }
        }

        #region Properties

        public WorkUnitQuery SelectedWorkUnitQuery
        {
            get
            {
                if (QueryBindingSource.Current != null)
                {
                    return (WorkUnitQuery)QueryBindingSource.Current;
                }
                return null;
            }
        }

        public WorkUnitRow SelectedWorkUnitRow
        {
            get
            {
                if (HistoryBindingSource.Current != null)
                {
                    return (WorkUnitRow)HistoryBindingSource.Current;
                }
                return null;
            }
        }

        public bool EditAndDeleteButtonsEnabled
        {
            get { return SelectedWorkUnitQuery.Name != WorkUnitQuery.SelectAll.Name; }
        }

        private BonusCalculation _bonusCalculation;

        public BonusCalculation BonusCalculation
        {
            get { return _bonusCalculation; }
            set
            {
                if (_bonusCalculation != value)
                {
                    _bonusCalculation = value;
                    ResetBindings(true);
                }
            }
        }

        public long TotalEntries
        {
            get { return _page.TotalItems; }
        }

        private long _currentPage = 1;

        public long CurrentPage
        {
            get { return _currentPage; }
            set
            {
                long newPage = value;
                if (newPage < 1)
                {
                    newPage = TotalPages;
                }
                else if (newPage > TotalPages)
                {
                    newPage = 1;
                }

                if (_currentPage != newPage)
                {
                    _currentPage = newPage;
                    ResetBindings(true);
                }
            }
        }

        public long TotalPages
        {
            get { return _page.TotalPages > 0 ? _page.TotalPages : 1; }
        }

        private int _showEntriesValue;

        public int ShowEntriesValue
        {
            get { return _showEntriesValue; }
            set
            {
                if (_showEntriesValue != value)
                {
                    _showEntriesValue = value;
                    OnPropertyChanged();
                    _currentPage = 1;
                    ResetBindings(true);
                }
            }
        }

        public Point FormLocation { get; set; }

        public Size FormSize { get; set; }

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

        // SortColumnName stores the actual WorkUnitEntityRow property name
        // this method guards against WorkUnitEntityRow property name changes
        private string ValidateSortColumnNameOrDefault(string name)
        {
            var properties = HistoryBindingSource.CurrencyManager.GetItemProperties();
            var property = properties.Find(name, true);
            return property is null ? DefaultSortColumnName : name;
        }

        private string DefaultSortColumnName { get; } = String.Empty;

        public ListSortDirection SortOrder { get; set; }

        public ICollection<string> FormColumns { get; set; }

        #endregion

        public static string[] GetColumnNames()
        {
            // Indexes Must Match WorkUnitRowColumn enum
            return new[]
            {
                "ProjectID",
                "Run",
                "Clone",
                "Gen",
                "Client / Slot Name",
                "Connection String",
                "Donor Name",
                "Donor Team",
                "Core Version",
                "Frames Completed",
                "Frame Time",
                "Unit Result",
                "Assigned (UTC)",
                "Finished (UTC)",
                "Removed (Work Unit Name)",
                "KFactor",
                "Core Name",
                "Total Frames",
                "Atoms",
                "Slot Type",
                "PPD",
                "Credit",
                "Base Credit",
                "Client Version",
                "Operating System",
                "Platform",
                "Processor",
                "Threads",
                "Driver Version",
                "Compute Version",
                "CUDA Version"
            };
        }
    }
}
