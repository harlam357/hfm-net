using System.ComponentModel;

using HFM.Core.Client;

namespace HFM.Forms
{
    internal class ClientDataSortableBindingList : SortableBindingList<IClientData>
    {
        public bool OfflineClientsLast
        {
            get { return ((ClientDataSortComparer)SortComparer).OfflineClientsLast; }
            set { ((ClientDataSortComparer)SortComparer).OfflineClientsLast = value; }
        }

        public ClientDataSortableBindingList()
        {
            SortComparer = new ClientDataSortComparer();
        }

        protected override bool SupportsSearchingCore => true;

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            // This key seems to always be null under Mono
            if (key == null) return -1;

            if (key is string)
            {
                if (Items is List<IClientData> list)
                {
                    return list.FindIndex(item => prop.GetValue(item).Equals(key));
                }

                return -1;
            }

            throw new NotSupportedException("Key must be of Type System.String.");
        }
    }
}
