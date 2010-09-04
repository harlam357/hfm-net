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

using System.ComponentModel;
using System.Windows.Forms;

using HFM.Framework;

namespace HFM.Models
{
   public interface IHistoryPresenterModel : INotifyPropertyChanged
   {
      void LoadPreferences();
   
      void SavePreferences();
      
      HistoryProductionView ProductionView { get; set; }
      
      bool ShowTopChecked { get; set; }
      
      int ShowTopValue { get; set; }
      
      string SortColumnName { get; set; }
      
      SortOrder SortOrder { get; set; }
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
         _showTopChecked = _prefs.GetPreference<bool>(Preference.ShowTopChecked);
         _showTopValue = _prefs.GetPreference<int>(Preference.ShowTopValue);
         SortColumnName = _prefs.GetPreference<string>(Preference.HistorySortColumnName);
         SortOrder = _prefs.GetPreference<SortOrder>(Preference.HistorySortOrder);
      }

      public void SavePreferences()
      {
         _prefs.SetPreference(Preference.HistoryProductionType, _productionView);
         _prefs.SetPreference(Preference.ShowTopChecked, _showTopChecked);
         _prefs.SetPreference(Preference.ShowTopValue, _showTopValue);
         _prefs.SetPreference(Preference.HistorySortColumnName, SortColumnName);
         _prefs.SetPreference(Preference.HistorySortOrder, SortOrder);
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

      private bool _showTopChecked;

      public bool ShowTopChecked
      {
         get { return _showTopChecked; }
         set
         {
            if (_showTopChecked != value)
            {
               _showTopChecked = value;
               OnPropertyChanged("ShowTopChecked");
            }
         }
      }

      private int _showTopValue;

      public int ShowTopValue
      {
         get { return _showTopValue; }
         set
         {
            if (_showTopValue != value)
            {
               _showTopValue = value;
               OnPropertyChanged("ShowTopValue");
            }
         }
      }

      public string SortColumnName { get; set; }

      public SortOrder SortOrder { get; set; }

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
