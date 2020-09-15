using System;
using System.Collections.Generic;
using System.ComponentModel;

using HFM.Core.Client;

namespace HFM.Forms
{
    internal class SlotModelSortableBindingList : SortableBindingList<SlotModel>
    {
        public bool OfflineClientsLast
        {
            get { return ((SlotModelSortComparer)SortComparer).OfflineClientsLast; }
            set { ((SlotModelSortComparer)SortComparer).OfflineClientsLast = value; }
        }

        public SlotModelSortableBindingList()
        {
            SortComparer = new SlotModelSortComparer();
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
