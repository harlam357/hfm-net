
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HFM.Core
{
    /// <summary>
    /// Specifies the sorting mode.
    /// </summary>
    public enum SortMode
    {
        /// <summary>
        /// No sorting.
        /// </summary>
        None,
        /// <summary>
        /// Simple, single property sorting.
        /// </summary>
        Simple,
        /// <summary>
        /// Advanced, multi property sorting.
        /// </summary>
        Advanced
    }

    /// <summary>
    /// Represents a comparer for the SortableBindingList that supports comparing one or more properties of two objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public class SortComparer<T> : IComparer<T>
    {
        // The following code contains code implemented by Rockford Lhotka:
        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp

        /// <summary>
        /// Gets a value indicating the active sorting mode.
        /// </summary>
        public SortMode SortMode
        {
            get
            {
                if (Property != null)
                {
                    return SortMode.Simple;
                }
                if (SortDescriptions != null)
                {
                    return SortMode.Advanced;
                }
                return SortMode.None;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the comparer supports simple sorting.
        /// </summary>
        public virtual bool SupportsSorting
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the comparer supports advanced sorting.
        /// </summary>
        public virtual bool SupportsAdvancedSorting
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the property descriptor used in a simple sort.
        /// </summary>
        public PropertyDescriptor Property { get; private set; }

        /// <summary>
        /// Gets the sort direction used in a simple sort.
        /// </summary>
        public ListSortDirection Direction { get; private set; }

        /// <summary>
        /// Gets the sort description collection used in an advanced sort.
        /// </summary>
        public ListSortDescriptionCollection SortDescriptions { get; private set; }

        /// <summary>
        /// Sets the sorting properties.
        /// </summary>
        /// <param name="property">Property descriptor for a simple sort.</param>
        /// <param name="direction">Sort direction for a simple sort.</param>
        public void SetSortProperties(PropertyDescriptor property, ListSortDirection direction)
        {
            Property = property;
            Direction = direction;
            SortDescriptions = null;
        }

        /// <summary>
        /// Sets the sorting properties.
        /// </summary>
        /// <param name="sortDescriptions">Sort description collection for an advanced sort.</param>
        public void SetSortProperties(ListSortDescriptionCollection sortDescriptions)
        {
            Property = null;
            SortDescriptions = sortDescriptions;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public int Compare(T x, T y)
        {
            /* Knowing how to sort is dependent on what sorting properties are set */
            if (SupportsSorting && Property != null)
            {
                return CompareInternal(x, y);
            }
            if (SupportsAdvancedSorting && SortDescriptions != null)
            {
                return RecursiveCompareInternal(x, y, 0);
            }

            return 0;
        }

        #region protected virtual

        /// <summary>
        /// Single property compare method.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        protected virtual int CompareInternal(T x, T y)
        {
            /* Get property values */
            object xValue = GetPropertyValue(x, Property);
            object yValue = GetPropertyValue(y, Property);

            /* Determine sort order */
            if (Direction == ListSortDirection.Ascending)
            {
                return CompareAscending(xValue, yValue);
            }

            return CompareDescending(xValue, yValue);
        }

        /// <summary>
        /// Multiple property compare method.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="index">Zero based index of SortDescriptions collection.</param>
        protected virtual int RecursiveCompareInternal(T x, T y, int index)
        {
            if (index >= SortDescriptions.Count)
            {
                return 0; // termination condition
            }

            /* Get property values */
            ListSortDescription listSortDesc = SortDescriptions[index];
            object xValue = listSortDesc.PropertyDescriptor.GetValue(x);
            object yValue = listSortDesc.PropertyDescriptor.GetValue(y);

            int result;
            /* Determine sort order */
            if (listSortDesc.SortDirection == ListSortDirection.Ascending)
            {
                result = CompareAscending(xValue, yValue);
            }
            else
            {
                result = CompareDescending(xValue, yValue);
            }

            /* If the properties are equal, compare the next property */
            if (result == 0)
            {
                return RecursiveCompareInternal(x, y, ++index);
            }

            return result;
        }

        /// <summary>
        /// Compare two property values in ascending order.
        /// </summary>
        /// <param name="xValue">The first property value to compare.</param>
        /// <param name="yValue">The second property value to compare.</param>
        protected virtual int CompareAscending(object xValue, object yValue)
        {
            int result;

            if (xValue == null && yValue == null)
            {
                result = 0;
            }
            else if (xValue == null)
            {
                result = -1;
            }
            else if (yValue == null)
            {
                result = 1;
            }
            else if (xValue is IComparable)
            {
                /* If values implement IComparable */
                result = ((IComparable)xValue).CompareTo(yValue);
            }
            else if (xValue.Equals(yValue))
            {
                /* If values don't implement IComparable but are equivalent */
                result = 0;
            }
            else
            {
                /* Values don't implement IComparable and are not equivalent, so compare as string values */
                // old comparison
                //result = xValue.ToString().CompareTo(yValue.ToString());
                // use Ordinal rules instead
                result = String.CompareOrdinal(xValue.ToString(), yValue.ToString());
            }

            return result;
        }

        /// <summary>
        /// Compare two property values in descending order.
        /// </summary>
        /// <param name="xValue">The first property value to compare.</param>
        /// <param name="yValue">The second property value to compare.</param>
        protected virtual int CompareDescending(object xValue, object yValue)
        {
            /* Return result adjusted for ascending or descending sort order ie
               multiplied by 1 for ascending or -1 for descending */
            return CompareAscending(xValue, yValue) * -1;
        }

        /// <summary>
        /// Get the property value from the object.
        /// </summary>
        /// <param name="value">Object instance.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>The property value.</returns>
        protected virtual object GetPropertyValue(T value, PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor == null ? null : propertyDescriptor.GetValue(value);
        }

        #endregion
    }
}
