/*
 * HFM.NET - Display Instance Sortable Binding List Class
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

using System;
using System.Collections.Generic;
using System.ComponentModel;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   [Serializable]
   [CoverageExclude]
   [CLSCompliant(false)]
   public class DisplayInstanceSortableBindingList : SortableBindingList<IDisplayInstance>
   {
      #region Fields

      private const string StatusColumnName = "Status";
      private const string NameColumnName = "Name";

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

      #region BindingList<T> Sorting Overrides

      protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
      {
         var items = Items as List<IDisplayInstance>;

         if ((null != items) && (null != property))
         {
            var pc = new DisplayInstancePropertyComparer<IDisplayInstance>(property,
                                                                           FindPropertyDescriptor(StatusColumnName),
                                                                           FindPropertyDescriptor(NameColumnName),
                                                                           direction,
                                                                           OfflineClientsLast);
            items.Sort(pc);

            /* Set sorted */
            IsSorted = true;
         }
         else
         {
            /* Set sorted */
            IsSorted = false;
         }
      }
      
      #endregion

      #region BindingList<T> Find Overrides

      protected override bool SupportsSearchingCore
      {
         get { return true; }
      }

      protected override int FindCore(PropertyDescriptor prop, object key)
      {
         // This key seems to always be null under Mono
         if (key == null) return -1;

         if (key is String)
         {
            var list = Items as List<IDisplayInstance>;

            if ((null != list))
            {
               return list.FindIndex(delegate(IDisplayInstance item)
               {
                  if (prop.GetValue(item).Equals(key))
                  {
                     return true;
                  }
                  return false;
               });
            }

            return -1;
         }

         throw new NotSupportedException("Key must be of Type System.String.");
      }

      #endregion

      #region DisplayInstancePropertyComparer<T>

      [CoverageExclude]
      internal class DisplayInstancePropertyComparer<T> : IComparer<T>
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

         public DisplayInstancePropertyComparer(PropertyDescriptor property, PropertyDescriptor statusProperty, PropertyDescriptor nameProperty,
                                                ListSortDirection direction, bool offlineClientsLast)
         {
            _property = property;
            _statusProperty = statusProperty;
            _nameProperty = nameProperty;
            _direction = direction;
            _offlineClientsLast = offlineClientsLast;
         }

         public int Compare(T xVal, T yVal)
         {
            /* Get property values */
            object xValue = GetPropertyValue(xVal, _property);
            object yValue = GetPropertyValue(yVal, _property);
            object xStatusValue = GetPropertyValue(xVal, _statusProperty);
            object yStatusValue = GetPropertyValue(yVal, _statusProperty);
            object xNameValue = GetPropertyValue(xVal, _nameProperty);
            object yNameValue = GetPropertyValue(yVal, _nameProperty);

            // check for offline clients first
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

            int returnValue;

            /* Determine sort order */
            if (_direction == ListSortDirection.Ascending)
            {
               returnValue = CompareAscending(xValue, yValue);
            }
            else
            {
               returnValue = CompareDescending(xValue, yValue);
            }

            // if values are equal, sort via the client name (asc)
            if (returnValue == 0)
            {
               returnValue = CompareAscending(xNameValue, yNameValue);
            }

            return returnValue;
         }

         //public bool Equals(T xVal, T yVal)
         //{
         //   return xVal.Equals(yVal);
         //}

         //public int GetHashCode(T obj)
         //{
         //   return obj.GetHashCode();
         //}

         /* Compare two property values of any type */
         private static int CompareAscending(object xValue, object yValue)
         {
            int result;

            try
            {
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
            }
            // we encounterd a null value, just return 0
            catch (NullReferenceException)
            {
               result = 0;
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

         private static object GetPropertyValue(T value, PropertyDescriptor property)
         {
            /* Get property */
            return property.GetValue(value);
         }
      }

      #endregion
   }
}
