using System.Runtime.Serialization;

namespace HFM.Core.Client
{
    public interface ICompletedFailedUnitsSource
    {
        /// <summary>
        /// Gets the number of completed units since the last client start.
        /// </summary>
        int TotalRunCompletedUnits { get; }

        /// <summary>
        /// Gets the total number of completed units.
        /// </summary>
        int TotalCompletedUnits { get; }

        /// <summary>
        /// Gets the number of failed units since the last client start.
        /// </summary>
        int TotalRunFailedUnits { get; }

        /// <summary>
        /// Gets the total number of failed units.
        /// </summary>
        int TotalFailedUnits { get; }
    }

    [DataContract(Namespace = "")]
    public class SlotTotals
    {
        [DataMember]
        public double PPD { get; set; }

        [DataMember]
        public double UPD { get; set; }

        [DataMember]
        public int TotalSlots { get; set; }

        [DataMember]
        public int WorkingSlots { get; set; }

        public int NonWorkingSlots => TotalSlots - WorkingSlots;

        [DataMember]
        public int TotalRunCompletedUnits { get; set; }

        [DataMember]
        public int TotalRunFailedUnits { get; set; }

        [DataMember]
        public int TotalCompletedUnits { get; set; }

        [DataMember]
        public int TotalFailedUnits { get; set; }

        /// <summary>
        /// Get the totals for all slots.
        /// </summary>
        /// <returns>The totals for all slots.</returns>
        public static SlotTotals Create(ICollection<IClientData> collection)
        {
            var totals = new SlotTotals();

            if (collection == null)
            {
                return totals;
            }

            totals.TotalSlots = collection.Count;
            foreach (var slot in collection)
            {
                totals.PPD += slot.PPD;
                totals.UPD += slot.UPD;
                if (slot is ICompletedFailedUnitsSource unitsSource)
                {
                    totals.TotalRunCompletedUnits += unitsSource.TotalRunCompletedUnits;
                    totals.TotalRunFailedUnits += unitsSource.TotalRunFailedUnits;
                    totals.TotalCompletedUnits += unitsSource.TotalCompletedUnits;
                    totals.TotalFailedUnits += unitsSource.TotalFailedUnits;
                }

                if (slot.Status.IsRunning())
                {
                    totals.WorkingSlots++;
                }
            }

            return totals;
        }
    }
}
