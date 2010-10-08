
using System;
using System.Collections.Generic;
using System.ComponentModel;

using HFM.Framework;

namespace HFM.Instances
{
   [Serializable]
   [CoverageExclude]
   public class HistoryEntrySortableBindingList : SortableBindingList<HistoryEntry>
   {
      #region Fields

      private const string IdColumnName = "ID";

      #endregion

      public HistoryEntrySortableBindingList(IList<HistoryEntry> list)
         : base(list)
      {
         
      }

      #region BindingList<T> Sorting Overrides

      protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
      {
         var items = Items as List<HistoryEntry>;

         if ((null != items) && (null != property))
         {
            var pc = new HistoryEntryPropertyComparer<HistoryEntry>(property,
                                                                    FindPropertyDescriptor(IdColumnName),
                                                                    direction);
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

      #region HistoryEntryPropertyComparer<T>

      [CoverageExclude]
      internal class HistoryEntryPropertyComparer<T> : IComparer<T>
      {
         /*
         * The following code contains code implemented by Rockford Lhotka:
         * msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp" 
         */

         private readonly PropertyDescriptor _property;
         private readonly PropertyDescriptor _idProperty;
         private readonly ListSortDirection _direction;

         public HistoryEntryPropertyComparer(PropertyDescriptor property, PropertyDescriptor idProperty, ListSortDirection direction)
         {
            _property = property;
            _idProperty = idProperty;
            _direction = direction;
         }

         public int Compare(T xVal, T yVal)
         {
            /* Get property values */
            object xValue = GetPropertyValue(xVal, _property);
            object yValue = GetPropertyValue(yVal, _property);
            object xIdValue = GetPropertyValue(xVal, _idProperty);
            object yIdValue = GetPropertyValue(yVal, _idProperty);

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

            // if values are equal, sort via the entry id (asc)
            if (returnValue == 0)
            {
               returnValue = CompareAscending(xIdValue, yIdValue);
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
