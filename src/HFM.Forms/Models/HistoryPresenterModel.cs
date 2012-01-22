/*
 * HFM.NET - Work Unit History - Binding Model
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms.Models
{
   public sealed class HistoryPresenterModel : INotifyPropertyChanged
   {
      private readonly IUnitInfoDatabase _database;

      private readonly List<QueryParameters> _queryList;
      private readonly BindingSource _queryBindingSource;
      public BindingSource QueryBindingSource
      {
         get { return _queryBindingSource; }
      }

      private readonly HistoryEntrySortableBindingList _historyList;
      private readonly BindingSource _historyBindingSource;
      public BindingSource HistoryBindingSource
      {
         get { return _historyBindingSource; }
      }

      private IList<HistoryEntry> _allEntries;
      private IList<HistoryEntry> _shownEntries;
      
      public HistoryPresenterModel(IUnitInfoDatabase database)
      {
         _database = database;

         _queryList = new List<QueryParameters>();
         _queryList.Add(new QueryParameters());
         _queryList.Sort();
         _queryBindingSource = new BindingSource();
         _queryBindingSource.DataSource = _queryList;
         _queryBindingSource.CurrentItemChanged += delegate
                                                   {
                                                      OnPropertyChanged("EditAndDeleteButtonsEnabled");
                                                      ResetBindings(true);
                                                   };

         _historyList = new HistoryEntrySortableBindingList();
         _historyList.Sorted += (sender, e) =>
         {
            SortColumnName = e.Name;
            SortOrder = e.Direction;
         };
         _historyBindingSource = new BindingSource();
         _historyBindingSource.DataSource = _historyList;
         
         _allEntries = new List<HistoryEntry>();
         _shownEntries = new List<HistoryEntry>();
      }

      public void Load(IPreferenceSet prefs, IQueryParametersCollection queryCollection)
      {
         _productionView = prefs.Get<HistoryProductionView>(Preference.HistoryProductionType);
         _showFirstChecked = prefs.Get<bool>(Preference.ShowFirstChecked);
         _showLastChecked = prefs.Get<bool>(Preference.ShowLastChecked);
         _showEntriesValue = prefs.Get<int>(Preference.ShowEntriesValue);
         FormLocation = prefs.Get<Point>(Preference.HistoryFormLocation);
         FormSize = prefs.Get<Size>(Preference.HistoryFormSize);
         SortColumnName = prefs.Get<string>(Preference.HistorySortColumnName);
         SortOrder = prefs.Get<ListSortDirection>(Preference.HistorySortOrder);
         FormColumns = prefs.Get<StringCollection>(Preference.HistoryFormColumns);

         _queryList.Clear();
         _queryList.Add(new QueryParameters());
         foreach (var query in queryCollection)
         {
            // don't load Select All twice
            if (query.Name != QueryParameters.SelectAll)
            {
               _queryList.Add(query);
            }
         }
         _queryList.Sort();
         ResetBindings(true);
      }

      public void Update(IPreferenceSet prefs, IQueryParametersCollection queryCollection)
      {
         prefs.Set(Preference.HistoryProductionType, _productionView);
         prefs.Set(Preference.ShowFirstChecked, _showFirstChecked);
         prefs.Set(Preference.ShowLastChecked, _showLastChecked);
         prefs.Set(Preference.ShowEntriesValue, _showEntriesValue);
         prefs.Set(Preference.HistoryFormLocation, FormLocation);
         prefs.Set(Preference.HistoryFormSize, FormSize);
         prefs.Set(Preference.HistorySortColumnName, SortColumnName);
         prefs.Set(Preference.HistorySortOrder, SortOrder);
         prefs.Set(Preference.HistoryFormColumns, FormColumns);
         prefs.Save();

         queryCollection.Clear();
         foreach (var query in _queryList)
         {
            // don't save Select All to disk
            if (query.Name != QueryParameters.SelectAll)
            {
               queryCollection.Add(query);
            }
         }
         queryCollection.Write();
      }

      public void AddQuery(QueryParameters parameters)
      {
         CheckQueryParametersForAddOrReplace(parameters);

         if (_queryList.FirstOrDefault(x => x.Name == parameters.Name) != null)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "A query with name '{0}' already exists.", parameters.Name));
         }

         _queryList.Add(parameters);
         _queryList.Sort();
         _queryBindingSource.ResetBindings(false);
         _queryBindingSource.Position = _queryBindingSource.IndexOf(parameters);
      }

      public void ReplaceQuery(QueryParameters parameters)
      {
         if (SelectedQuery.Name == QueryParameters.SelectAll)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Cannot replace the '{0}' query.", QueryParameters.SelectAll));
         }

         CheckQueryParametersForAddOrReplace(parameters);

         var existing = _queryList.FirstOrDefault(x => x.Name == parameters.Name);
         if (existing != null && existing.Name != SelectedQuery.Name)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "A query with name '{0}' already exists.", parameters.Name));
         }

         _queryList.Remove(SelectedQuery);
         _queryList.Add(parameters);
         _queryList.Sort();
         _queryBindingSource.ResetBindings(false);
         _queryBindingSource.Position = _queryBindingSource.IndexOf(parameters);
      }

      private static void CheckQueryParametersForAddOrReplace(QueryParameters parameters)
      {
         if (parameters.Name == QueryParameters.SelectAll)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Query name cannot be '{0}'.", QueryParameters.SelectAll));
         }

         if (parameters.Fields.Count == 0)
         {
            throw new ArgumentException("No query fields defined.");
         }

         for (int i = 0; i < parameters.Fields.Count; i++)
         {
            if (parameters.Fields[i].Value == null)
            {
               throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Field index {0} must have a query value.", (i + 1)));
            }
         }
      }

      public void RemoveQuery(QueryParameters parameters)
      {
         if (parameters.Name == QueryParameters.SelectAll)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Cannot remove '{0}' query.", QueryParameters.SelectAll));
         }

         _queryList.Remove(parameters);
         _queryBindingSource.ResetBindings(false);
      }

      public void DeleteUnitInfo(long id)
      {
         if (_database.DeleteUnitInfo(id) != 0)
         {
            ResetBindings(true);
         }
      }

      public void ResetBindings(bool executeQuery)
      {
         Debug.Assert(SelectedQuery != null);

         if (executeQuery)
         {
            _allEntries = _database.QueryUnitData(SelectedQuery, ProductionView) ?? new List<HistoryEntry>();
         }
         _shownEntries = _allEntries;
         if (ShowFirstChecked)
         {
            _shownEntries = _allEntries.Take(ShowEntriesValue).ToList();
         }
         else if (ShowLastChecked)
         {
            _shownEntries = _allEntries.Reverse().Take(ShowEntriesValue).ToList();
         }

         // halt binding source updates
         _historyBindingSource.RaiseListChangedEvents = false;
         // refresh the underlying binding list
         RefreshHistoryList(_shownEntries);
         // sort the list
         _historyBindingSource.Sort = null;
         if (!String.IsNullOrEmpty(SortColumnName))
         {
            _historyBindingSource.Sort = SortColumnName + " " + SortOrder.ToDirectionString();
         }
         // enable binding source updates
         _historyBindingSource.RaiseListChangedEvents = true;
         // reset AFTER RaiseListChangedEvents is enabled
         _historyBindingSource.ResetBindings(false);

         OnPropertyChanged("TotalEntries");
         OnPropertyChanged("ShownEntries");
      }

      private void RefreshHistoryList(IEnumerable<HistoryEntry> historyEntries)
      {
         _historyList.Clear();
         foreach (var entry in historyEntries)
         {
            _historyList.Add(entry);
         }
      }

      #region Properties

      public QueryParameters SelectedQuery
      {
         get
         {
            if (_queryBindingSource.Current != null)
            {
               return (QueryParameters)_queryBindingSource.Current;
            }
            return null;
         }
      }

      public HistoryEntry SelectedHistoryEntry
      {
         get
         {
            if (_historyBindingSource.Current != null)
            {
               return (HistoryEntry)_historyBindingSource.Current;
            }
            return null;
         }
      }

      public bool EditAndDeleteButtonsEnabled
      {
         get { return SelectedQuery.Name != QueryParameters.SelectAll; }
      }

      private HistoryProductionView _productionView;

      public HistoryProductionView ProductionView
      {
         get { return _productionView; }
         set
         {
            if (_productionView != value)
            {
               _productionView = value;
               OnPropertyChanged("ProductionView");
            }
         }
      }

      public int TotalEntries
      {
         get { return _allEntries.Count; }
      }

      public int ShownEntries
      {
         get { return _shownEntries.Count; }
      }

      private bool _showFirstChecked;

      public bool ShowFirstChecked
      {
         get { return _showFirstChecked; }
         set
         {
            if (_showFirstChecked != value)
            {
               _showFirstChecked = value;
               if (_showFirstChecked) _showLastChecked = false;
               OnPropertyChanged("ShowFirstChecked");
               ResetBindings(false);
            }
         }
      }

      private bool _showLastChecked;

      public bool ShowLastChecked
      {
         get { return _showLastChecked; }
         set
         {
            if (_showLastChecked != value)
            {
               _showLastChecked = value;
               if (_showLastChecked) _showFirstChecked = false;
               OnPropertyChanged("ShowLastChecked");
               ResetBindings(false);
            }
         }
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
               ResetBindings(false);
            }
         }
      }

      public Point FormLocation { get; set; }

      public Size FormSize { get; set; }

      public string SortColumnName { get; set; }

      public ListSortDirection SortOrder { get; set; }
      
      public StringCollection FormColumns { get; set; }

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
