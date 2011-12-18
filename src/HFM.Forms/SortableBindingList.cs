/*
 * HFM.NET - Sortable Binding List Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

/* 
 * Implementation by Joe Stegman - Microsoft Corporation
 * http://social.msdn.microsoft.com/forums/en-US/winformsdatacontrols/thread/12eb59d3-e687-4e36-93ab-bf6741954d39/
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using HFM.Core;

namespace HFM.Forms
{
   [CoverageExclude]
   public class SortableBindingList<T> : BindingList<T>, ITypedList
   {
      #region Events

      public event EventHandler<SortedEventArgs> Sorted;

      protected virtual void OnSorted(SortedEventArgs e)
      {
         if (Sorted != null) Sorted(this, e);
      }

      #endregion

      #region Fields

      private PropertyDescriptorCollection _shape;
      private readonly ISynchronizeInvoke _syncObject;
      private readonly Action<ListChangedEventArgs> _fireEventAction;

      #endregion
      
      #region Constructor

      public SortableBindingList()
      {
         /* Default to non-sorted columns */
         _sortColumns = false;

         /* Get shape (only get public properties marked browsable true) */
         _shape = GetShape();
      }

      public SortableBindingList(IList<T> list)
         : base(list)
      {
         /* Default to non-sorted columns */
         _sortColumns = false;

         /* Get shape (only get public properties marked browsable true) */
         _shape = GetShape();
      }

      public SortableBindingList(ISynchronizeInvoke syncObject)
      {
         /* Default to non-sorted columns */
         _sortColumns = false;

         /* Get shape (only get public properties marked browsable true) */
         _shape = GetShape();

         _syncObject = syncObject;
         _fireEventAction = FireEvent;
      }

      protected override void OnListChanged(ListChangedEventArgs args)
      {
         if (_syncObject == null)
         {
            FireEvent(args);
         }
         else
         {
            _syncObject.Invoke(_fireEventAction, new object[] { args });
         }
      }

      private void FireEvent(ListChangedEventArgs args)
      {
         base.OnListChanged(args);
      }

      #endregion

      #region SortableBindingList<T> Column Sorting API

      private bool _sortColumns;

      public bool SortColumns
      {
         get { return _sortColumns; }
         set
         {
            if (value != _sortColumns)
            {
               /* Set Column Sorting */
               _sortColumns = value;

               /* Set shape */
               _shape = GetShape();

               /* Fire MetaDataChanged */
               OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
            }
         }
      }

      #endregion

      #region BindingList<T> Public Sorting API

      public void Sort()
      {
         ApplySortCore(SortProperty, SortDirection);
      }

      public void Sort(string property)
      {
         /* Set the sort property */
         SortProperty = FindPropertyDescriptor(property);

         /* Sort */
         ApplySortCore(SortProperty, SortDirection);
      }

      public void Sort(string property, ListSortDirection direction)
      {
         /* Set the sort property and direction */
         SortProperty = FindPropertyDescriptor(property);
         SortDirection = direction;

         /* Sort */
         ApplySortCore(SortProperty, SortDirection);
      }

      #endregion

      #region BindingList<T> Sorting Overrides

      protected bool IsSorted { get; set; }
      /// <summary>
      /// Gets a value indicating whether the list is sorted. 
      /// </summary>
      /// <returns>
      /// true if the list is sorted; otherwise, false. The default is false.
      /// </returns>
      protected override bool IsSortedCore
      {
         get { return IsSorted; }
      }

      protected ListSortDirection SortDirection { get; set; }
      /// <summary>
      /// Gets the direction the list is sorted.
      /// </summary>
      /// <returns>
      /// One of the <see cref="T:System.ComponentModel.ListSortDirection"/> values. The default is <see cref="F:System.ComponentModel.ListSortDirection.Ascending"/>. 
      /// </returns>
      protected override ListSortDirection SortDirectionCore
      {
         get { return SortDirection; }
      }

      protected PropertyDescriptor SortProperty { get; set; }
      /// <summary>
      /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null. 
      /// </summary>
      /// <returns>
      /// The <see cref="T:System.ComponentModel.PropertyDescriptor"/> used for sorting the list.
      /// </returns>
      protected override PropertyDescriptor SortPropertyCore
      {
         get { return SortProperty; }
      }

      /// <summary>
      /// Gets a value indicating whether the list supports sorting.
      /// </summary>
      /// <returns>
      /// true if the list supports sorting; otherwise, false. The default is false.
      /// </returns>
      protected override bool SupportsSortingCore
      {
         get { return true; }
      }

      protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
      {
         var items = Items as List<T>;

         if ((null != items) && (null != property))
         {
            /* Set the sort property and direction */
            SortProperty = property;
            SortDirection = direction;

            var pc = new PropertyComparer<T>(property, direction);
            items.Sort(pc);

            /* Set sorted */
            IsSorted = true;

            OnSorted(new SortedEventArgs(property.Name, direction));
         }
         else
         {
            /* Set sorted */
            IsSorted = false;
         }
      }

      protected override void RemoveSortCore()
      {
         IsSorted = false;
      }

      #endregion

      #region SortableBindingList<T> Private Sorting API

      protected PropertyDescriptor FindPropertyDescriptor(string property)
      {
         PropertyDescriptor prop = null;

         if (null != _shape)
         {
            prop = _shape.Find(property, true);
         }

         return prop;
      }

      private PropertyDescriptorCollection GetShape()
      {
         /* Get shape (only get public properties marked browsable true) */
         PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(T), new Attribute[] { new BrowsableAttribute(true) });

         /* Sort if required */
         if (_sortColumns)
         {
            pdc = pdc.Sort();
         }

         return pdc;
      }

      #endregion

      #region PropertyComparer<TKey>

      [CoverageExclude]
      private class PropertyComparer<TKey> : IComparer<TKey>
      {
         /*
         * The following code contains code implemented by Rockford Lhotka:
         * msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp" 
         */

         private readonly PropertyDescriptor _property;
         private readonly ListSortDirection _direction;

         public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
         {
            _property = property;
            _direction = direction;
         }

         public int Compare(TKey xVal, TKey yVal)
         {
            /* Get property values */
            object xValue = GetPropertyValue(xVal, _property);
            object yValue = GetPropertyValue(yVal, _property);

            /* Determine sort order */
            if (_direction == ListSortDirection.Ascending)
            {
               return CompareAscending(xValue, yValue);
            }
            
            return CompareDescending(xValue, yValue);
         }

         //public bool Equals(TKey xVal, TKey yVal)
         //{
         //   return xVal.Equals(yVal);
         //}

         //public int GetHashCode(TKey obj)
         //{
         //   return obj.GetHashCode();
         //}

         /* Compare two property values of any type */
         private static int CompareAscending(object xValue, object yValue)
         {
            int result;

            if (xValue is IComparable)
            {
               /* If values implement IComparer */
               result = ((IComparable)xValue).CompareTo(yValue);
            }
            else if (xValue.Equals(yValue))
            {
               /* If values don't implement IComparer but are equivalent */
               result = 0;
            }
            else
            {
               /* Values don't implement IComparer and are not equivalent, so compare as string values */
               result = xValue.ToString().CompareTo(yValue.ToString());
            }

            /* Return result */
            return result;
         }

         private static int CompareDescending(object xValue, object yValue)
         {
            /* Return result adjusted for ascending or descending sort order ie
               multiplied by 1 for ascending or -1 for descending */
            return CompareAscending(xValue, yValue) * -1;
         }

         private static object GetPropertyValue(TKey value, PropertyDescriptor property)
         {
            /* Get property */
            return property.GetValue(value);
         }
      }

      #endregion

      #region ITypedList Implementation

      public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
      {
         PropertyDescriptorCollection pdc;

         if (null == listAccessors)
         {
            /* Return properties in sort order */
            pdc = _shape;
         }
         else
         {
            /* Return child list shape */
            pdc = ListBindingHelper.GetListItemProperties(listAccessors[0].PropertyType);
         }

         return pdc;
      }

      public string GetListName(PropertyDescriptor[] listAccessors)
      {
         /* Not really used anywhere other than DT and the old DataGrid */
         return typeof(T).Name;
      }

      #endregion
   }

   public class SortedEventArgs : EventArgs
   {
      private readonly string _name;

      public string Name
      {
         get { return _name; }
      }

      private readonly ListSortDirection _direction;

      public ListSortDirection Direction
      {
         get { return _direction; }
      }

      public SortedEventArgs(string name, ListSortDirection direction)
      {
         _name = name;
         _direction = direction;
      }
   }
}
