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
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Preferences;

namespace HFM.Forms.Models
{
   public sealed class MainGridModel
   {
      #region Events

      public event EventHandler AfterResetBindings;
      public event EventHandler<IndexChangedEventArgs> SelectedSlotChanged;

      #endregion

      #region Properties

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

      private string _sortColumnName;
      /// <summary>
      /// Holds current Sort Column Name
      /// </summary>
      public string SortColumnName
      {
         get { return _sortColumnName; }
         set { _sortColumnName = String.IsNullOrEmpty(value) ? "Name" : value; }
      }

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      public ListSortDirection SortColumnOrder { get; set; }

      #endregion

      #region Fields

      private readonly ISynchronizeInvoke _syncObject;
      private readonly IClientConfiguration _clientConfiguration;
      private readonly SlotModelSortableBindingList _slotList;
      private readonly BindingSource _bindingSource;

      private readonly object _slotsListLock = new object();

      #endregion

      public ICollection<SlotModel> SlotCollection
      {
         // ToList() to make a "copy" of the current list.
         // The value returned here is used by web generation
         // and if the collection changes the web generation
         // will not be able to enumerate the collection.
         get
         {
            lock (_slotsListLock)
            {
               return _slotList.ToList().AsReadOnly();
            }
         }
      }

      public SlotTotals SlotTotals
      {
         // use SlotCollection, it's provides synchronized access to the slot list
         get { return SlotCollection.GetSlotTotals(); }
      }

      public object BindingSource
      {
         get { return _bindingSource; }
      }

      public MainGridModel(IPreferenceSet prefs, ISynchronizeInvoke syncObject, IClientConfiguration clientConfiguration)
      {
         _syncObject = syncObject;
         _clientConfiguration = clientConfiguration;
         _slotList = new SlotModelSortableBindingList();
         _slotList.RaiseListChangedEvents = false;
         _slotList.OfflineClientsLast = prefs.Get<bool>(Preference.OfflineLast);
         _slotList.Sorted += (sender, e) =>
                             {
                                SortColumnName = e.Name;
                                prefs.Set(Preference.FormSortColumn, SortColumnName);
                                SortColumnOrder = e.Direction;
                                prefs.Set(Preference.FormSortOrder, SortColumnOrder);
                             };
         _bindingSource = new BindingSource();
         _bindingSource.DataSource = _slotList;
         _bindingSource.CurrentItemChanged += (sender, args) => SelectedSlot = (SlotModel)_bindingSource.Current;
#if DEBUG
         _slotList.ListChanged += (s, e) => Debug.WriteLine($"{s.GetType()} {e.GetType()}: {e.ListChangedType}");
         _bindingSource.ListChanged += (s, e) => Debug.WriteLine($"{s.GetType()} {e.GetType()}: {e.ListChangedType}");
#endif
         // Subscribe to PreferenceSet events
         prefs.PreferenceChanged += (s, e) =>
                                     {
                                        switch (e.Preference)
                                        {
                                           case Preference.OfflineLast:
                                              _slotList.OfflineClientsLast = prefs.Get<bool>(Preference.OfflineLast);
                                              Sort();
                                              break;
                                           case Preference.PpdCalculation:
                                           case Preference.DecimalPlaces:
                                           case Preference.BonusCalculation:
                                              ResetBindings();
                                              break;
                                        }

                                     };

         // Subscribe to ClientDictionary events
         _clientConfiguration.ConfigurationChanged += (sender, args) => ResetBindings();
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

      private void ResetBindingsInternal()
      {
         var control = _syncObject as Control;
         if (control != null && control.IsDisposed)
         {
            return;
         }
         if (_syncObject.InvokeRequired)
         {
            _syncObject.Invoke(new MethodInvoker(ResetBindingsInternal), null);
            return;
         }

         lock (_slotsListLock)
         {
            // get slots from the dictionary
            var slots = _clientConfiguration.Slots as IList<SlotModel> ?? _clientConfiguration.Slots.ToList();

            // refresh the underlying binding list
            _bindingSource.Clear();
            foreach (var slot in slots)
            {
               _bindingSource.Add(slot);
            }
            Debug.WriteLine("Number of slots: {0}", _bindingSource.Count);

            // sort the list
            SortInternal();
            // reset selected slot
            ResetSelectedSlot();
            // find duplicates
            slots.FindDuplicates();

            _bindingSource.ResetBindings(false);
         }
         OnAfterResetBindings(EventArgs.Empty);
      }

      /// <summary>
      /// Sort the grid model
      /// </summary>
      public void Sort()
      {
         lock (_slotsListLock)
         {
            // sort the list
            SortInternal();
         }
      }

      private void SortInternal()
      {
         _bindingSource.Sort = $"{SortColumnName} {SortColumnOrder.ToDirectionString()}";
         if (_slotList is IBindingList bindingList)
         {
            bindingList.ApplySort(bindingList.SortProperty, bindingList.SortDirection);
         }
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

      private void OnAfterResetBindings(EventArgs e)
      {
         AfterResetBindings?.Invoke(this, e);
      }

      private void OnSelectedSlotChanged(IndexChangedEventArgs e)
      {
         SelectedSlotChanged?.Invoke(this, e);
      }
   }

   public sealed class IndexChangedEventArgs : EventArgs
   {
      public int Index { get; }

      public IndexChangedEventArgs(int index)
      {
         Index = index;
      }
   }
}
