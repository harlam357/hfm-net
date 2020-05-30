/* 
 * Implementation by Joe Stegman - Microsoft Corporation
 * http://social.msdn.microsoft.com/forums/en-US/winformsdatacontrols/thread/12eb59d3-e687-4e36-93ab-bf6741954d39/
 * 
 * IBindingListView, Sorted Event, and plugable SortComparer<T> implementation by harlam357
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using HFM.Core;

namespace HFM.Forms
{
    /// <summary>
    /// Provides a generic collection that supports data binding, sorting, and filtering.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class SortableBindingList<T> : BindingList<T>, IBindingListView, ITypedList
    {
        #region Events

        /// <summary>
        /// Occurs when the SortableBindingList completes a sorting operation.
        /// </summary>
        public event EventHandler<SortedEventArgs> Sorted;

        /// <summary>
        /// Raises the Sorted event.
        /// </summary>
        /// <param name="e">A SortedEventArgs that contains the event data.</param>
        protected virtual void OnSorted(SortedEventArgs e)
        {
            Sorted?.Invoke(this, e);
        }

        #endregion

        #region Fields

        private PropertyDescriptorCollection _shape;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SortableBindingList class.
        /// </summary>
        public SortableBindingList()
        {
            /* Default to non-sorted columns */
            _sortColumns = false;

            /* Get shape (only get public properties marked browsable true) */
            _shape = GetShape();
        }

        /// <summary>
        /// Initializes a new instance of the SortableBindingList class with a list of items.
        /// </summary>
        public SortableBindingList(IList<T> list)
           : base(list)
        {
            /* Default to non-sorted columns */
            _sortColumns = false;

            /* Get shape (only get public properties marked browsable true) */
            _shape = GetShape();
        }

        #endregion

        #region SortableBindingList<T> Column Sorting API

        private bool _sortColumns;
        /// <summary>
        /// Gets or sets a value indicating if the columns are to be sorted.
        /// </summary>
        public bool SortColumns
        {
            get => _sortColumns;
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

        #region BindingList<T> Sorting Overrides

        /// <summary>
        /// Gets or sets a value indicating whether the list is sorted. 
        /// </summary>
        protected bool IsSorted { get; set; }
        /// <summary>
        /// Gets a value indicating whether the list is sorted. 
        /// </summary>
        /// <returns>true if the list is sorted; otherwise, false. The default is false.</returns>
        protected override bool IsSortedCore => IsSorted;

        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null. 
        /// </summary>
        /// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptor"/> used for sorting the list.</returns>
        protected override PropertyDescriptor SortPropertyCore => SortComparer.Property;

        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        /// <returns>One of the <see cref="T:System.ComponentModel.ListSortDirection"/> values. The default is <see cref="F:System.ComponentModel.ListSortDirection.Ascending"/>.</returns>
        protected override ListSortDirection SortDirectionCore => SortComparer.Direction;

        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        /// <returns>true if the list supports sorting; otherwise, false. The default is false.</returns>
        protected override bool SupportsSortingCore => SortComparer.SupportsSorting;

        /// <summary>
        /// Sorts the items if overridden in a derived class; otherwise, throws a <see cref="T:System.NotSupportedException"/>.
        /// </summary>
        /// <param name="prop">A <see cref="T:System.ComponentModel.PropertyDescriptor"/> that specifies the property to sort on.</param>
        /// <param name="direction">One of the <see cref="T:System.ComponentModel.ListSortDirection"/> values.</param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (prop != null)
            {
                /* Set the sort property and direction (in the comparer) */
                SortComparer.SetSortProperties(prop, direction);

                ApplySortCoreInternal();
            }
        }

        /// <summary>
        /// Sorts the items.
        /// </summary>
        protected virtual void ApplySortCoreInternal()
        {
            if (Items is List<T> items)
            {
                /* Execute the sort */
                items.Sort(SortComparer);

                /* Set sorted */
                IsSorted = true;

                if (SortComparer.SortMode.Equals(SortMode.Simple))
                {
                    OnSorted(new SortedEventArgs(SortComparer.Property.Name, SortComparer.Direction));
                }
                else if (SortComparer.SortMode.Equals(SortMode.Advanced))
                {
                    if (SortComparer.SortDescriptions.Count != 0)
                    {
                        ListSortDescription sort = SortComparer.SortDescriptions[0];
                        OnSorted(new SortedEventArgs(sort.PropertyDescriptor.Name, sort.SortDirection));
                    }
                }
            }
            else
            {
                /* Set sorted */
                IsSorted = false;
            }
        }

        /// <summary>
        /// Removes any sort applied with <see cref="M:System.ComponentModel.BindingList`1.ApplySortCore(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)"/> if sorting is implemented in a derived class; otherwise, raises <see cref="T:System.NotSupportedException"/>.
        /// </summary>
        protected override void RemoveSortCore()
        {
            IsSorted = false;
        }

        #endregion

        #region SortComparer<T> Public API

        private SortComparer<T> _sortComparer;
        /// <summary>
        /// User plugable sorting comparer.
        /// </summary>
        public SortComparer<T> SortComparer
        {
            get => _sortComparer ?? (_sortComparer = new SortComparer<T>());
            set => _sortComparer = value;
        }

        #endregion

        #region IBindingListView Implementation

        /// <summary>
        /// Gets the collection of sort descriptions currently applied to the data source.
        /// </summary>
        /// <returns>The <see cref="T:System.ComponentModel.ListSortDescriptionCollection"/> currently applied to the data source.</returns>
        public ListSortDescriptionCollection SortDescriptions => SortComparer.SortDescriptions;

        /// <summary>
        /// Gets a value indicating whether the data source supports advanced sorting. 
        /// </summary>
        /// <returns>true if the data source supports advanced sorting; otherwise, false.</returns>
        public bool SupportsAdvancedSorting => SortComparer.SupportsAdvancedSorting;

        /// <summary>
        /// Sorts the data source based on the given <see cref="T:System.ComponentModel.ListSortDescriptionCollection"/>.
        /// </summary>
        /// <param name="sorts">The <see cref="T:System.ComponentModel.ListSortDescriptionCollection"/> containing the sorts to apply to the data source.</param>
        public virtual void ApplySort(ListSortDescriptionCollection sorts)
        {
            if (sorts != null)
            {
                /* Set the sort descriptions (in the comparer) */
                SortComparer.SetSortProperties(sorts);

                ApplySortCoreInternal();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data source supports filtering. 
        /// </summary>
        /// <returns>true if the data source supports filtering; otherwise, false.</returns>
        public virtual bool SupportsFiltering => false;

        private string _filter;
        /// <summary>
        /// Gets or sets the filter to be used to exclude items from the collection of items returned by the data source
        /// </summary>
        /// <returns>The string used to filter items out in the item collection returned by the data source.</returns>
        public virtual string Filter
        {
            get => _filter;
            set => _filter = null;
        }

        /// <summary>
        /// Removes the current filter applied to the data source.
        /// </summary>
        public virtual void RemoveFilter()
        {
            Filter = null;
        }

        #endregion

        #region ITypedList Implementation
#pragma warning disable 1591

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptorCollection pdc;

            if (listAccessors != null && listAccessors.Length > 0)
            {
                // Return child list shape.
                pdc = ListBindingHelper.GetListItemProperties(listAccessors[0].PropertyType);
            }
            else
            {
                // Return properties in sort order.
                pdc = _shape;
            }

            return pdc;
        }

        // This method is only used in the design-time framework 
        // and by the obsolete DataGrid control.
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(T).Name;
        }

#pragma warning restore 1591
        #endregion
    }

    /// <summary>
    /// Provides data for the SortableBindingList.Sorted event.
    /// </summary>
    public class SortedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the property used to perform the sort (or the most significant in the case of an Advanced sort).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the current sort direction.
        /// </summary>
        public ListSortDirection Direction { get; }

        /// <summary>
        /// Initializes a new instance of the SortedEventArgs class.
        /// </summary>
        /// <param name="name">The sorted property name.</param>
        /// <param name="direction">The sort direction.</param>
        public SortedEventArgs(string name, ListSortDirection direction)
        {
            Name = name;
            Direction = direction;
        }
    }
}
