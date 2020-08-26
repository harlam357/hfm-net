using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace HFM.Forms
{
    /// <summary>
    /// Represents a wrapper around a non-generic list of <see cref="ListItem"/>.
    /// </summary>
    public class ListItemCollection : IList<ListItem>, INotifyCollectionChanged
    {
        public ListItemCollection()
        {
            Items = new List<ListItem>();
        }

        public ListItemCollection(IList list)
        {
            Items = list ?? throw new ArgumentNullException(nameof(list));
        }

        public int Count => Items.Count;

        protected IList Items { get; }

        public ListItem this[int index]
        {
            get => (ListItem)Items[index];
            set
            {
                if (index < 0 || index >= Items.Count) throw new ArgumentOutOfRangeException(nameof(index));

                SetItem(index, value);
            }
        }

        public void Add(ListItem item) => AddItem(item);

        public void Clear() => ClearItems();

        public void CopyTo(ListItem[] array, int index) => Items.CopyTo(array, index);

        public bool Contains(ListItem item) => Items.Contains(item);

        public IEnumerator<ListItem> GetEnumerator() => Items.Cast<ListItem>().GetEnumerator();

        public int IndexOf(ListItem item) => Items.IndexOf(item);

        void IList<ListItem>.Insert(int index, ListItem item) => throw new NotSupportedException("Insert at index not supported.");

        public bool Remove(ListItem item)
        {
            bool result = Items.Contains(item);
            RemoveItem(item);
            return result;
        }

        void IList<ListItem>.RemoveAt(int index) => throw new NotSupportedException("Remove at index not supported.");

        protected virtual void ClearItems()
        {
            Items.Clear();
            OnCollectionChanged(this);
        }

        protected virtual void AddItem(ListItem item)
        {
            Items.Add(item);
            OnCollectionChanged(this);
        }

        protected virtual void RemoveItem(ListItem item)
        {
            Items.Remove(item);
            OnCollectionChanged(this);
        }

        protected virtual void SetItem(int index, ListItem item)
        {
            Items[index] = item;
            OnCollectionChanged(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Can use Items.ReadOnly")]
        bool ICollection<ListItem>.IsReadOnly => Items.IsReadOnly;

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(object sender) => CollectionChanged?.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public class BindingSourceListItemCollection : ListItemCollection
    {
        private readonly BindingSource _bindingSource;

        public BindingSourceListItemCollection(BindingSource bindingSource)
        {
            _bindingSource = bindingSource;
            _bindingSource.ListChanged += OnBindingSourceListChanged;
        }

        protected virtual void OnBindingSourceListChanged(object sender, ListChangedEventArgs e)
        {
            // simulates how a ListBox reacts to BindingSource.ListChanged events
            int position = _bindingSource.Position;
            if (0 <= position && position < _bindingSource.Count)
            {
                Items.Clear();
                Items.Add(_bindingSource.Cast<ListItem>().ElementAt(position));
            }
            OnCollectionChanged(this);
        }
    }

    /// <summary>
    /// Represents a wrapper around a <see cref="ListBox.SelectedObjectCollection"/> containing <see cref="ListItem"/> values.
    /// </summary>
    public class ListBoxSelectedListItemCollection : ListItemCollection
    {
        private readonly ListBox.SelectedObjectCollection _selectedItems;

        public ListBoxSelectedListItemCollection(ListBox listBox) : base(listBox.SelectedItems)
        {
            _selectedItems = listBox.SelectedItems;
            listBox.SelectedIndexChanged += (s, e) => OnCollectionChanged(this);
        }

        protected override void ClearItems() => _selectedItems.Clear();

        protected override void AddItem(ListItem item) => _selectedItems.Add(item);

        protected override void RemoveItem(ListItem item) => _selectedItems.Remove(item);

        protected override void SetItem(int index, ListItem item) => _selectedItems[index] = item;
    }
}
