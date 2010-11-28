/*
 * HFM.NET - Work Unit History - Binding Model
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms.Models
{
   public interface IHistoryPresenterModel : INotifyPropertyChanged
   {
      void LoadPreferences();
   
      void SavePreferences();
      
      HistoryProductionView ProductionView { get; set; }
      
      bool ShowFirstChecked { get; set; }

      bool ShowLastChecked { get; set; }

      int ShowEntriesValue { get; set; }
      
      Point FormLocation { get; set; }
      
      Size FormSize { get; set; }

      string SortColumnName { get; set; }
      
      SortOrder SortOrder { get; set; }

      StringCollection FormColumns { get; set; }
   }

   public sealed class HistoryPresenterModel : IHistoryPresenterModel
   {
      private readonly IPreferenceSet _prefs;

      public HistoryPresenterModel(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      public void LoadPreferences()
      {
         _productionView = _prefs.GetPreference<HistoryProductionView>(Preference.HistoryProductionType);
         _showFirstChecked = _prefs.GetPreference<bool>(Preference.ShowFirstChecked);
         _showLastChecked = _prefs.GetPreference<bool>(Preference.ShowLastChecked);
         _showEntriesValue = _prefs.GetPreference<int>(Preference.ShowEntriesValue);
         FormLocation = _prefs.GetPreference<Point>(Preference.HistoryFormLocation);
         FormSize = _prefs.GetPreference<Size>(Preference.HistoryFormSize);
         SortColumnName = _prefs.GetPreference<string>(Preference.HistorySortColumnName);
         SortOrder = _prefs.GetPreference<SortOrder>(Preference.HistorySortOrder);
         FormColumns = _prefs.GetPreference<StringCollection>(Preference.HistoryFormColumns);
      }

      public void SavePreferences()
      {
         _prefs.SetPreference(Preference.HistoryProductionType, _productionView);
         _prefs.SetPreference(Preference.ShowFirstChecked, _showFirstChecked);
         _prefs.SetPreference(Preference.ShowLastChecked, _showLastChecked);
         _prefs.SetPreference(Preference.ShowEntriesValue, _showEntriesValue);
         _prefs.SetPreference(Preference.HistoryFormLocation, FormLocation);
         _prefs.SetPreference(Preference.HistoryFormSize, FormSize);
         _prefs.SetPreference(Preference.HistorySortColumnName, SortColumnName);
         _prefs.SetPreference(Preference.HistorySortOrder, SortOrder);
         _prefs.SetPreference(Preference.HistoryFormColumns, FormColumns);
         _prefs.Save();
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
            }
         }
      }

      public Point FormLocation { get; set; }

      public Size FormSize { get; set; }

      public string SortColumnName { get; set; }

      public SortOrder SortOrder { get; set; }
      
      public StringCollection FormColumns { get; set; }
      
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
