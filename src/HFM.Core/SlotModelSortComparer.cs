
using System.ComponentModel;

using harlam357.Core.ComponentModel;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public class SlotModelSortComparer : SortComparer<SlotModel>
   {
      public bool OfflineClientsLast { get; set; }

      public override bool SupportsAdvancedSorting
      {
         get { return false; }
      }

      protected override int CompareInternal(SlotModel xVal, SlotModel yVal)
      {
         /* Get property values */
         object xValue = GetPropertyValue(xVal, Property);
         object yValue = GetPropertyValue(yVal, Property);
         SlotStatus xStatusValue = xVal.Status;
         SlotStatus yStatusValue = yVal.Status;
         object xNameValue = xVal.Name;
         object yNameValue = yVal.Name;

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