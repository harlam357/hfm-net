using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HFM.Core
{
    public interface IQueueItem
    {
        int ID { get; }
    }

    public abstract class QueueItemCollection<T> : IEnumerable<T> where T : IQueueItem
    {
        private readonly QueueItemKeyedCollection _inner = new QueueItemKeyedCollection();

        public int DefaultID => _inner.Count > 0 ? _inner.First().ID : NoID;

        public const int NoID = -1;

        public int CurrentID { get; set; } = NoID;

        public T Current => (T)(_inner.Contains(CurrentID) ? _inner[CurrentID] : null);

        public void Add(T workUnit) => _inner.Add(workUnit);

        public int Count => _inner.Count;

        public T this[int id] => (T)(_inner.Contains(id) ? _inner[id] : null);

        public IEnumerator<T> GetEnumerator() => _inner.Cast<T>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class QueueItemKeyedCollection : KeyedCollection<int, IQueueItem>
        {
            public QueueItemKeyedCollection() : base(EqualityComparer<int>.Default, 1)
            {

            }

            protected override int GetKeyForItem(IQueueItem item) => item.ID;
        }
    }
}
