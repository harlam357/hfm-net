/*
 * HFM.NET - Main Grid Data Model
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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

using Castle.Core.Logging;

using HFM.Core;

namespace HFM.Forms.Models
{
   public sealed class MainGridModel
   {
      public event EventHandler BeforeResetBindings;
      public event EventHandler AfterResetBindings;
      public event EventHandler<IndexChangedEventArgs> SelectedSlotChanged;

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      private SlotModel _selectedSlot;
      public SlotModel SelectedSlot
      {
         get { return _selectedSlot; }
         set
         {
            if (!ReferenceEquals(_selectedSlot, value))
            {
               _selectedSlot = value;
               OnSelectedSlotChanged(new IndexChangedEventArgs(_bindingSource.Position));
            }
         }
      }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Client Files' is Visible
      /// </summary>
      public bool ClientFilesMenuItemVisible
      {
         get { return SelectedSlot != null && SelectedSlot.Settings.LegacyClientSubType.Equals(LegacyClientSubType.Path); }
      }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Cached Log File' is Visible
      /// </summary>
      public bool CachedLogMenuItemVisible
      {
         get { return SelectedSlot != null && SelectedSlot.Settings.ClientType.Equals(ClientType.Legacy); }
      }

      /// <summary>
      /// Holds current Sort Column Name
      /// </summary>
      public string SortColumnName { get; set; }

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      public ListSortDirection SortColumnOrder { get; set; }

      private readonly IPreferenceSet _prefs;
      private readonly ISynchronizeInvoke _syncObject;
      private readonly IClientDictionary _clientDictionary;
      private readonly SlotModelSortableBindingList _slotList;
      public IEnumerable<SlotModel> SlotCollection
      {
         get { return _slotList; }
      }
      private readonly BindingSource _bindingSource;
      public BindingSource BindingSource
      {
         get { return _bindingSource; }
      }

      public MainGridModel(IPreferenceSet prefs, ISynchronizeInvoke syncObject, IClientDictionary clientDictionary)
      {
         _prefs = prefs;
         _syncObject = syncObject;
         _clientDictionary = clientDictionary;
         _slotList = new SlotModelSortableBindingList(_prefs.Get<bool>(Preference.OfflineLast), _syncObject);
         _slotList.Sorted += (sender, e) =>
                             {
                                SortColumnName = e.Name;
                                _prefs.Set(Preference.FormSortColumn, SortColumnName);
                                SortColumnOrder = e.Direction;
                                _prefs.Set(Preference.FormSortOrder, SortColumnOrder);
                             };
         _bindingSource = new BindingSource();
         _bindingSource.DataSource = _slotList;
         _bindingSource.CurrentItemChanged += delegate
                                              {
                                                 SelectedSlot = (SlotModel)_bindingSource.Current;
                                              };

         // Subscribe to PreferenceSet events
         _prefs.OfflineLastChanged += delegate
                                      {
                                         _slotList.OfflineClientsLast = _prefs.Get<bool>(Preference.OfflineLast);
                                         Sort();
                                      };
         _prefs.PpdCalculationChanged += delegate { ResetBindings(); };
         _prefs.DecimalPlacesChanged += delegate { ResetBindings(); };
         _prefs.CalculateBonusChanged += delegate { ResetBindings(); };

         // Subscribe to ClientDictionary events
         _clientDictionary.DictionaryChanged += delegate { ResetBindings(); };
         _clientDictionary.ClientDataInvalidated += delegate { ResetBindings(); };
      }

      private bool _resetInProgress;

      private void ResetBindings()
      {
         if (_syncObject.InvokeRequired)
         {
            _syncObject.BeginInvoke(new MethodInvoker(ResetBindings), null);
            return;
         }

         // this check appears to fix the duplicate slot issue.
         // every time this condition is met and the return
         // statement is removed then subsequently in 
         // RefreshSlotList() the _slotList shows duplicate slots
         if (_resetInProgress)
         {
            Debug.WriteLine("Reset already in progress...");
            return;
         }

         _resetInProgress = true;
         try
         {
            OnBeforeResetBindings(EventArgs.Empty);
            // halt binding source updates
            _bindingSource.RaiseListChangedEvents = false;
            // refresh the underlying binding list
            RefreshSlotList();
            // sort the list
            _bindingSource.Sort = null;
            _bindingSource.Sort = SortColumnName + " " + SortColumnOrder.ToDirectionString();
            // reset selected slot
            ResetSelectedSlot();
            // find duplicates
            FindDuplicates();
            // enable binding source updates
            _bindingSource.RaiseListChangedEvents = true;
            // reset AFTER RaiseListChangedEvents is enabled
            _bindingSource.ResetBindings(false);
            // restore binding source updates
            OnAfterResetBindings(EventArgs.Empty);
         }
         finally
         {
            _resetInProgress = false;
         }
      }

      /// <summary>
      /// Refresh the SlotModel list from the ClientDictionary.
      /// </summary>
      private void RefreshSlotList()
      {
         _slotList.Clear();
         foreach (var slot in _clientDictionary.Slots)
         {
            _slotList.Add(slot);
         }
         string message = String.Format(CultureInfo.InvariantCulture, "Number of slots: {0}", _slotList.Count);
         Debug.WriteLine(message);
      }

      /// <summary>
      /// Sort the grid model
      /// </summary>
      public void Sort()
      {
         _bindingSource.RaiseListChangedEvents = false;
         // sort the list
         _bindingSource.Sort = null;
         _bindingSource.Sort = SortColumnName + " " + SortColumnOrder.ToDirectionString();
         // enable binding source updates
         _bindingSource.RaiseListChangedEvents = true;
      }

      public void ResetSelectedSlot()
      {
         if (SelectedSlot == null) return;

         int row = _bindingSource.Find("Name", SelectedSlot.Name);
         if (row > -1)
         {
            _bindingSource.Position = row;
         }
      }

      public void FindDuplicates()
      {
         _clientDictionary.Slots.FindDuplicates();
      }

      private void OnBeforeResetBindings(EventArgs e)
      {
         if (BeforeResetBindings != null)
         {
            BeforeResetBindings(this, e);
         }
      }

      private void OnAfterResetBindings(EventArgs e)
      {
         if (AfterResetBindings != null)
         {
            AfterResetBindings(this, e);
         }
      }

      private void OnSelectedSlotChanged(IndexChangedEventArgs e)
      {
         if (SelectedSlotChanged != null)
         {
            SelectedSlotChanged(this, e);
         }
      }
   }

   [CoverageExclude]
   public sealed class IndexChangedEventArgs : EventArgs
   {
      private readonly int _index;

      public int Index
      {
         get { return _index; }
      }

      public IndexChangedEventArgs(int index)
      {
         _index = index;
      }
   }
}
