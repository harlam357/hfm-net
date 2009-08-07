/*
 * HFM.NET - Sortable Binding List Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
 * 
 * Implementation by Joe Stegman - Microsoft Corporation
 * http://social.msdn.microsoft.com/forums/en-US/winformsdatacontrols/thread/12eb59d3-e687-4e36-93ab-bf6741954d39/
 * 
 * With modifications by harlam357 to support sorting DisplayInstance data objects.
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
using System.Windows.Forms;

namespace HFM.Instances
{
   [Serializable]
   public class SortableBindingList<T> : BindingList<T>, ITypedList
   {
      #region Members
      private const string statusColumnName = "Status";
      private const string nameColumnName = "Name";

      private bool _isSorted;
      private ListSortDirection _dir = ListSortDirection.Ascending;
      private bool _sortColumns = false;

      [NonSerialized]
      private PropertyDescriptorCollection _shape = null;

      [NonSerialized]
      private PropertyDescriptor _sort = null;

      [NonSerialized]
      private bool _offlineClientsLast = true; 
      #endregion

      #region Properties
      public bool OfflineClientsLast
      {
         get { return _offlineClientsLast; }
         set { _offlineClientsLast = value; }
      } 
      #endregion
      
      #region Constructor
      public SortableBindingList()
      {
         /* Default to non-sorted columns */
         _sortColumns = false;

         /* Get shape (only get public properties marked browsable true) */
         _shape = GetShape();
      }
      #endregion

      #region SortedBindingList<T> Column Sorting API
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
         ApplySortCore(_sort, _dir);
      }

      public void Sort(string property)
      {
         /* Get the PD */
         _sort = FindPropertyDescriptor(property);

         /* Sort */
         ApplySortCore(_sort, _dir);
      }

      public void Sort(string property, ListSortDirection direction)
      {
         /* Get the sort property */
         _sort = FindPropertyDescriptor(property);
         _dir = direction;

         /* Sort */
         ApplySortCore(_sort, _dir);
      }
      #endregion

      #region BindingList<T> Sorting Overrides
      protected override bool SupportsSortingCore
      {
         get { return true; }
      }

      protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
      {
         List<T> items = Items as List<T>;

         if ((null != items) && (null != property))
         {
            PropertyComparer<T> pc = new PropertyComparer<T>(property, FindPropertyDescriptor(statusColumnName), 
                                                             FindPropertyDescriptor(nameColumnName), direction, _offlineClientsLast);
            items.Sort(pc);

            /* Set sorted */
            _isSorted = true;
         }
         else
         {
            /* Set sorted */
            _isSorted = false;
         }
      }

      protected override bool IsSortedCore
      {
         get { return _isSorted; }
      }

      protected override void RemoveSortCore()
      {
         _isSorted = false;
      }
      #endregion

      #region SortedBindingList<T> Private Sorting API
      private PropertyDescriptor FindPropertyDescriptor(string property)
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
      internal class PropertyComparer<TKey> : IComparer<TKey>
      {
         /*
         * The following code contains code implemented by Rockford Lhotka:
         * msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp" 
         */

         private readonly PropertyDescriptor _property;
         private readonly PropertyDescriptor _statusProperty;
         private readonly PropertyDescriptor _nameProperty;
         private readonly ListSortDirection _direction;
         private readonly bool _offlineClientsLast;

         public PropertyComparer(PropertyDescriptor property, PropertyDescriptor statusProperty, PropertyDescriptor nameProperty, 
                                 ListSortDirection direction, bool offlineClientsLast)
         {
            _property = property;
            _statusProperty = statusProperty;
            _nameProperty = nameProperty;
            _direction = direction;
            _offlineClientsLast = offlineClientsLast;
         }

         public int Compare(TKey xVal, TKey yVal)
         {
            /* Get property values */
            object xValue = GetPropertyValue(xVal, _property);
            object yValue = GetPropertyValue(yVal, _property);
            object xStatusValue = GetPropertyValue(xVal, _statusProperty);
            object yStatusValue = GetPropertyValue(yVal, _statusProperty);
            object xNameValue = GetPropertyValue(xVal, _nameProperty);
            object yNameValue = GetPropertyValue(yVal, _nameProperty);

            //if (xValue.Equals(yValue))
            //{
            //   int returnValue1 = CompareAscending(xNameValue, yNameValue);
            //   if (returnValue1 != 0)
            //   {
            //      return returnValue1;
            //   }

            //   return 0;
            //}
            if (_offlineClientsLast)
            {
               if (((ClientStatus)xStatusValue).Equals(ClientStatus.Offline) && 
                   ((ClientStatus)yStatusValue).Equals(ClientStatus.Offline) == false)
               {
                  return 1;
               }
               if (((ClientStatus)yStatusValue).Equals(ClientStatus.Offline) &&
                   ((ClientStatus)xStatusValue).Equals(ClientStatus.Offline) == false)
               {
                  return -1;
               }
            }
            if (xValue.Equals(yValue))
            {
               int returnValue1 = CompareAscending(xNameValue, yNameValue);
               if (returnValue1 != 0)
               {
                  return returnValue1;
               }

               return 0;
            }

            /* Determine sort order */
            if (_direction == ListSortDirection.Ascending)
            {
               int returnValue = CompareAscending(xValue, yValue);
               return returnValue;
            }
            else
            {
               int returnValue = CompareDescending(xValue, yValue);
               return returnValue;
            }
         }

         public bool Equals(TKey xVal, TKey yVal)
         {
            return xVal.Equals(yVal);
         }

         public int GetHashCode(TKey obj)
         {
            return obj.GetHashCode();
         }

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
}
