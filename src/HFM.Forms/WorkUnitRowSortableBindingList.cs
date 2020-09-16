using System.ComponentModel;

using HFM.Core;
using HFM.Core.Data;

namespace HFM.Forms
{
    internal class WorkUnitRowSortableBindingList : SortableBindingList<WorkUnitRow>
    {
        public WorkUnitRowSortableBindingList()
        {
            SortComparer = new WorkUnitHistoryRowSortComparer();
        }

        #region WorkUnitHistoryRowSortComparer

        private class WorkUnitHistoryRowSortComparer : SortComparer<WorkUnitRow>
        {
            protected override int CompareInternal(WorkUnitRow xVal, WorkUnitRow yVal)
            {
                /* Get property values */
                object xValue = GetPropertyValue(xVal, Property);
                object yValue = GetPropertyValue(yVal, Property);
                long xIdValue = xVal.ID;
                long yIdValue = yVal.ID;

                int result;

                /* Determine sort order */
                if (Direction == ListSortDirection.Ascending)
                {
                    result = CompareAscending(xValue, yValue);
                }
                else
                {
                    result = CompareDescending(xValue, yValue);
                }

                // if values are equal, sort via the entry id (asc)
                if (result == 0)
                {
                    result = CompareAscending(xIdValue, yIdValue);
                }

                return result;
            }

            protected override int RecursiveCompareInternal(WorkUnitRow xVal, WorkUnitRow yVal, int index)
            {
                if (index >= SortDescriptions.Count)
                {
                    return 0; // termination condition
                }

                /* Get property values */
                ListSortDescription listSortDesc = SortDescriptions[index];
                object xValue = listSortDesc.PropertyDescriptor.GetValue(xVal);
                object yValue = listSortDesc.PropertyDescriptor.GetValue(yVal);
                long xIdValue = xVal.ID;
                long yIdValue = yVal.ID;

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
                    result = RecursiveCompareInternal(xVal, yVal, ++index);
                }

                // if values are equal, sort via the entry id (asc)
                if (result == 0)
                {
                    result = CompareAscending(xIdValue, yIdValue);
                }

                return result;
            }
        }

        #endregion
    }
}
