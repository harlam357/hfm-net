﻿using System.ComponentModel;

using HFM.Core.Collections;

namespace HFM.Core.Client
{
    public class ClientDataSortComparer : SortComparer<IClientData>
    {
        public bool OfflineClientsLast { get; set; }

        public override bool SupportsAdvancedSorting => false;

        protected override int CompareInternal(IClientData x, IClientData y)
        {
            /* Get property values */
            object xValue = GetPropertyValue(x, Property);
            object yValue = GetPropertyValue(y, Property);
            SlotStatus xStatusValue = x.Status;
            SlotStatus yStatusValue = y.Status;
            object xNameValue = x.Name;
            object yNameValue = y.Name;

            // check for offline clients first
            if (OfflineClientsLast)
            {
                if (xStatusValue == SlotStatus.Offline &&
                    yStatusValue != SlotStatus.Offline)
                {
                    return 1;
                }
                if (yStatusValue == SlotStatus.Offline &&
                    xStatusValue != SlotStatus.Offline)
                {
                    return -1;
                }
            }

            int returnValue;

            /* Determine sort order */
            if (Direction == ListSortDirection.Ascending)
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
    }
}
