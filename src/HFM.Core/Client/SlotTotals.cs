/*
 * HFM.NET - Slot Totals Structure
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */


using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HFM.Core.Client
{
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

        public int NonWorkingSlots
        {
            get { return TotalSlots - WorkingSlots; }
        }

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
        public static SlotTotals Create(ICollection<SlotModel> slots)
        {
            var totals = new SlotTotals();

            // If no slots return initialized totals.
            if (slots == null)
            {
                return totals;
            }

            totals.TotalSlots = slots.Count;
            foreach (SlotModel slot in slots)
            {
                totals.PPD += slot.PPD;
                totals.UPD += slot.UPD;
                totals.TotalRunCompletedUnits += slot.TotalRunCompletedUnits;
                totals.TotalRunFailedUnits += slot.TotalRunFailedUnits;
                totals.TotalCompletedUnits += slot.TotalCompletedUnits;
                totals.TotalFailedUnits += slot.TotalFailedUnits;

                if (slot.ProductionValuesOk)
                {
                    totals.WorkingSlots++;
                }
            }

            return totals;
        }
    }
}
