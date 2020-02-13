/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public sealed class HistoryPresenterModel : INotifyPropertyChanged
    {
        private readonly IUnitInfoDatabase _database;

        private readonly List<WorkUnitHistoryQuery> _queryList;
        private readonly BindingSource _queryBindingSource;
        public BindingSource QueryBindingSource
        {
            get { return _queryBindingSource; }
        }

        private readonly WorkUnitHistoryRowSortableBindingList _workUnitHistoryList;
        private readonly BindingSource _historyBindingSource;
        public BindingSource HistoryBindingSource
        {
            get { return _historyBindingSource; }
        }

        private PetaPoco.Page<WorkUnitHistoryRow> _page;

        public HistoryPresenterModel(IUnitInfoDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));

            Debug.Assert(_database.Connected);

            _queryList = new List<WorkUnitHistoryQuery>();
            _queryList.Add(new WorkUnitHistoryQuery());
            _queryList.Sort();
            _queryBindingSource = new BindingSource();
            _queryBindingSource.DataSource = _queryList;
            _queryBindingSource.CurrentItemChanged += (s, e) =>
                                                      {
                                                          OnPropertyChanged("EditAndDeleteButtonsEnabled");
                                                          _currentPage = 1;
                                                          ResetBindings(true);
                                                      };

            _workUnitHistoryList = new WorkUnitHistoryRowSortableBindingList();
            _workUnitHistoryList.Sorted += (s, e) =>
            {
                SortColumnName = e.Name;
                SortOrder = e.Direction;
            };
            _historyBindingSource = new BindingSource();
            _historyBindingSource.DataSource = _workUnitHistoryList;

            _page = new PetaPoco.Page<WorkUnitHistoryRow> { Items = new List<WorkUnitHistoryRow>() };
        }

        public void Load(IPreferenceSet prefs, WorkUnitHistoryQueryDataContainer queryContainer)
        {
            _bonusCalculation = prefs.Get<BonusCalculationType>(Preference.HistoryBonusCalculation);
            _showEntriesValue = prefs.Get<int>(Preference.ShowEntriesValue);
            FormLocation = prefs.Get<Point>(Preference.HistoryFormLocation);
            FormSize = prefs.Get<Size>(Preference.HistoryFormSize);
            SortColumnName = prefs.Get<string>(Preference.HistorySortColumnName);
            SortOrder = prefs.Get<ListSortDirection>(Preference.HistorySortOrder);
            FormColumns = prefs.Get<ICollection<string>>(Preference.HistoryFormColumns);

            _queryList.Clear();
            _queryList.Add(new WorkUnitHistoryQuery());
            foreach (var query in queryContainer.Data)
            {
                // don't load Select All twice
                if (query.Name != WorkUnitHistoryQuery.SelectAll.Name)
                {
                    _queryList.Add(query);
                }
            }
            _queryList.Sort();
            ResetBindings(true);
        }

        public void Update(IPreferenceSet prefs, WorkUnitHistoryQueryDataContainer queryContainer)
        {
            prefs.Set(Preference.HistoryBonusCalculation, _bonusCalculation);
            prefs.Set(Preference.ShowEntriesValue, _showEntriesValue);
            prefs.Set(Preference.HistoryFormLocation, FormLocation);
            prefs.Set(Preference.HistoryFormSize, FormSize);
            prefs.Set(Preference.HistorySortColumnName, SortColumnName);
            prefs.Set(Preference.HistorySortOrder, SortOrder);
            prefs.Set(Preference.HistoryFormColumns, FormColumns);
            prefs.Save();

            queryContainer.Data.Clear();
            queryContainer.Data.AddRange(_queryList.Where(x => x.Name != WorkUnitHistoryQuery.SelectAll.Name));
            queryContainer.Write();
        }

        public void AddQuery(WorkUnitHistoryQuery query)
        {
            CheckQueryParametersForAddOrReplace(query);

            if (_queryList.FirstOrDefault(x => x.Name == query.Name) != null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                   "A query with name '{0}' already exists.", query.Name));
            }

            _queryList.Add(query);
            _queryList.Sort();
            _queryBindingSource.ResetBindings(false);
            _queryBindingSource.Position = _queryBindingSource.IndexOf(query);
        }

        public void ReplaceQuery(WorkUnitHistoryQuery query)
        {
            if (SelectedWorkUnitHistoryQuery.Name == WorkUnitHistoryQuery.SelectAll.Name)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Cannot replace the '{0}' query.", WorkUnitHistoryQuery.SelectAll));
            }

            CheckQueryParametersForAddOrReplace(query);

            var existing = _queryList.FirstOrDefault(x => x.Name == query.Name);
            if (existing != null && existing.Name != SelectedWorkUnitHistoryQuery.Name)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                   "A query with name '{0}' already exists.", query.Name));
            }

            _queryList.Remove(SelectedWorkUnitHistoryQuery);
            _queryList.Add(query);
            _queryList.Sort();
            _queryBindingSource.ResetBindings(false);
            _queryBindingSource.Position = _queryBindingSource.IndexOf(query);
        }

        private static void CheckQueryParametersForAddOrReplace(WorkUnitHistoryQuery query)
        {
            if (query.Name == WorkUnitHistoryQuery.SelectAll.Name)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Query name cannot be '{0}'.", WorkUnitHistoryQuery.SelectAll));
            }

            if (query.Parameters.Count == 0)
            {
                throw new ArgumentException("No query fields defined.");
            }

            for (int i = 0; i < query.Parameters.Count; i++)
            {
                if (query.Parameters[i].Value == null)
                {
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Field index {0} must have a query value.", (i + 1)));
                }
            }
        }

        public void RemoveQuery(WorkUnitHistoryQuery query)
        {
            if (query.Name == WorkUnitHistoryQuery.SelectAll.Name)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Cannot remove '{0}' query.", WorkUnitHistoryQuery.SelectAll));
            }

            _queryList.Remove(query);
            _queryBindingSource.ResetBindings(false);
        }

        public void DeleteHistoryEntry(WorkUnitHistoryRow row)
        {
            if (_database.Delete(row) != 0)
            {
                _page.Items.Remove(row);
                _page.TotalItems--;
                ResetBindings(false);
            }
        }

        public void ResetBindings(bool executeQuery)
        {
            Debug.Assert(SelectedWorkUnitHistoryQuery != null);

            if (executeQuery)
            {
                _page = _database.Page(CurrentPage, ShowEntriesValue, SelectedWorkUnitHistoryQuery, BonusCalculation);
            }
            if (_page == null)
            {
                return;
            }

            // halt binding source updates
            _historyBindingSource.RaiseListChangedEvents = false;
            _workUnitHistoryList.RaiseListChangedEvents = false;
            // refresh the underlying binding list
            RefreshHistoryList(_page.Items);
            // sort the list
            _historyBindingSource.Sort = null;
            if (!String.IsNullOrEmpty(SortColumnName))
            {
                _historyBindingSource.Sort = SortColumnName + " " + SortOrder.ToDirectionString();
            }
            // enable binding source updates
            _historyBindingSource.RaiseListChangedEvents = true;
            _workUnitHistoryList.RaiseListChangedEvents = true;
            // reset AFTER RaiseListChangedEvents is enabled
            _historyBindingSource.ResetBindings(false);

            OnPropertyChanged("TotalEntries");
            OnPropertyChanged("CurrentPage");
        }

        private void RefreshHistoryList(IEnumerable<WorkUnitHistoryRow> historyEntries)
        {
            _historyBindingSource.Clear();
            if (historyEntries != null)
            {
                foreach (var entry in historyEntries)
                {
                    _historyBindingSource.Add(entry);
                }
            }
        }

        public IList<WorkUnitHistoryRow> FetchSelectedQuery()
        {
            return _database.Fetch(SelectedWorkUnitHistoryQuery, BonusCalculation);
        }

        #region Properties

        public WorkUnitHistoryQuery SelectedWorkUnitHistoryQuery
        {
            get
            {
                if (_queryBindingSource.Current != null)
                {
                    return (WorkUnitHistoryQuery)_queryBindingSource.Current;
                }
                return null;
            }
        }

        public WorkUnitHistoryRow SelectedWorkUnitHistoryRow
        {
            get
            {
                if (_historyBindingSource.Current != null)
                {
                    return (WorkUnitHistoryRow)_historyBindingSource.Current;
                }
                return null;
            }
        }

        public bool EditAndDeleteButtonsEnabled
        {
            get { return SelectedWorkUnitHistoryQuery.Name != WorkUnitHistoryQuery.SelectAll.Name; }
        }

        private BonusCalculationType _bonusCalculation;

        public BonusCalculationType BonusCalculation
        {
            get { return _bonusCalculation; }
            set
            {
                if (_bonusCalculation != value)
                {
                    _bonusCalculation = value;
                    OnPropertyChanged("ProductionView");
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
                    OnPropertyChanged("ShowEntriesValue");
                    _currentPage = 1;
                    ResetBindings(true);
                }
            }
        }

        public Point FormLocation { get; set; }

        public Size FormSize { get; set; }

        public string SortColumnName { get; set; }

        public ListSortDirection SortOrder { get; set; }

        public ICollection<string> FormColumns { get; set; }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
