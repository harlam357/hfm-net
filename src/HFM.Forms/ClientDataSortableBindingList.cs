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

        #region BindingList<T> Find Overrides

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            // This key seems to always be null under Mono
            if (key == null) return -1;

            if (key is string)
            {
                var list = Items as List<SlotModel>;
                if ((null != list))
                {
                    return list.FindIndex(item =>
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
    }
}
