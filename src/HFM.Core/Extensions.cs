/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

using HFM.Client.DataTypes;
using HFM.Core.Client;
using HFM.Core.DataTypes;
using HFM.Core.WorkUnits;

namespace HFM.Core
{
    public static partial class Extensions
    {
        #region DateTime/TimeSpan

        public static bool IsKnown(this DateTime dateTime)
        {
            return !IsUnknown(dateTime);
        }

        public static bool IsUnknown(this DateTime dateTime)
        {
            return dateTime.Equals(DateTime.MinValue);
        }

        public static string ToDateString(this DateTime date)
        {
            return ToDateString(date, String.Format(CultureInfo.CurrentCulture,
                     "{0} {1}", date.ToShortDateString(), date.ToShortTimeString()));
        }

        public static string ToDateString(this IEquatable<DateTime> date, string formattedValue)
        {
            return date.Equals(DateTime.MinValue) ? "Unknown" : formattedValue;
        }

        #endregion

        #region SlotStatus

        /// <summary>
        /// Gets Status Html Color String
        /// </summary>
        public static string GetHtmlColor(this SlotStatus status)
        {
            return ColorTranslator.ToHtml(status.GetStatusColor());
        }

        /// <summary>
        /// Gets Status Html Font Color String
        /// </summary>
        public static string GetHtmlFontColor(this SlotStatus status)
        {
            switch (status)
            {
                case SlotStatus.RunningNoFrameTimes:
                case SlotStatus.Paused:
                case SlotStatus.Finishing:
                case SlotStatus.Offline:
                    return ColorTranslator.ToHtml(Color.Black);
                default:
                    return ColorTranslator.ToHtml(Color.White);
            }
        }

        /// <summary>
        /// Gets Status Color Object
        /// </summary>
        public static Color GetStatusColor(this SlotStatus status)
        {
            switch (status)
            {
                case SlotStatus.Running:
                    return Color.Green;
                case SlotStatus.RunningNoFrameTimes:
                    return Color.Yellow;
                case SlotStatus.Finishing:
                    return Color.Khaki;
                case SlotStatus.Ready:
                    return Color.DarkCyan;
                case SlotStatus.Stopping:
                case SlotStatus.Failed:
                    return Color.DarkRed;
                case SlotStatus.Paused:
                    return Color.Orange;
                case SlotStatus.Offline:
                    return Color.Gray;
                default:
                    return Color.Gray;
            }
        }

        #endregion

        #region SlotName

        internal static string AppendSlotId(this string name, int slotId)
        {
            return slotId >= 0 ? String.Format(CultureInfo.InvariantCulture, "{0} Slot {1:00}", name, slotId) : name;
        }

        #endregion

        #region ClientSettings

        public static bool IsFahClient(this ClientSettings settings)
        {
            return settings != null && settings.ClientType == ClientType.FahClient;
        }

        /// <summary>
        /// Used to supply a "path" value to the benchmarks and unit info database.
        /// </summary>
        public static string DataPath(this ClientSettings settings)
        {
            if (settings == null) return String.Empty;

            return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Server, settings.Port);
        }

        public static string CachedFahLogFileName(this ClientSettings settings)
        {
            return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, Constants.FahClientLogFileName);
        }

        #endregion

        #region SlotTotals

        /// <summary>
        /// Get the totals for all slots.
        /// </summary>
        /// <returns>The totals for all slots.</returns>
        public static SlotTotals GetSlotTotals(this IEnumerable<SlotModel> slots)
        {
            var totals = new SlotTotals();

            // If no slots return initialized totals.
            if (slots == null)
            {
                return totals;
            }

            totals.TotalSlots = slots.Count();

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

        #endregion

        #region FindDuplicates

        /// <summary>
        /// Find Clients with Duplicate User IDs or Project (R/C/G).
        /// </summary>
        public static void FindDuplicates(this IEnumerable<SlotModel> slots)
        {
            // Issue 19
            FindDuplicateUserId(slots);
            FindDuplicateProjects(slots);
        }

        private static void FindDuplicateUserId(IEnumerable<SlotModel> slots)
        {
            var duplicates = (from x in slots
                              group x by x.UserAndMachineId into g
                              let count = g.Count()
                              where count > 1 && g.First().UserIdUnknown == false
                              select g.Key);

            foreach (SlotModel slot in slots)
            {
                slot.UserIdIsDuplicate = duplicates.Contains(slot.UserAndMachineId);
            }
        }

        private static void FindDuplicateProjects(IEnumerable<SlotModel> slots)
        {
            var duplicates = (from x in slots
                              group x by x.UnitInfoModel.UnitInfoData.ToShortProjectString() into g
                              let count = g.Count()
                              where count > 1 && g.First().UnitInfoModel.UnitInfoData.ProjectIsUnknown() == false
                              select g.Key);

            foreach (SlotModel slot in slots)
            {
                slot.ProjectIsDuplicate = duplicates.Contains(slot.UnitInfoModel.UnitInfoData.ToShortProjectString());
            }
        }

        #endregion

        #region SlotType

        public static SlotType ToSlotType(this SlotOptions value)
        {
            return (SlotType)Enum.Parse(typeof(SlotType), value.GpuIndex.HasValue ? "GPU" : "CPU");
        }

        public static SlotType ToSlotType(this string value)
        {
            SlotType type = ToSlotTypeFromCoreName(value);
            if (type == SlotType.Unknown)
            {
                type = ToSlotTypeFromCoreId(value);
            }
            return type;
        }

        /// <summary>
        /// Determine the Client Type based on the FAH Core Name
        /// </summary>
        /// <param name="coreName">FAH Core Name (from psummary)</param>
        private static SlotType ToSlotTypeFromCoreName(string coreName)
        {
            // make this method more forgiving - rwh 9/6/10
            if (String.IsNullOrEmpty(coreName))
            {
                return SlotType.Unknown;
            }

            switch (coreName.ToUpperInvariant())
            {
                case "GROMACS":
                case "DGROMACS":
                case "GBGROMACS":
                case "AMBER":
                case "GROMACS33":
                case "GROST":
                case "GROSIMT":
                case "DGROMACSB":
                case "DGROMACSC":
                case "GRO-A4":
                case "GRO_A4":
                case "PROTOMOL":
                case "GRO-SMP":
                case "GROCVS":
                case "GRO-A3":
                case "GRO_A3":
                case "GRO-A5":
                case "GRO_A5":
                case "GRO-A6":
                case "GRO_A7":
                    return SlotType.CPU;
                case "GROGPU2":
                case "GROGPU2-MT":
                case "OPENMM_21":
                case "OPENMMGPU":
                case "OPENMM_OPENCL":
                case "ATI-DEV":
                case "NVIDIA-DEV":
                case "ZETA":
                case "ZETA_DEV":
                    return SlotType.GPU;
                default:
                    return SlotType.Unknown;
            }
        }

        /// <summary>
        /// Determine the Client Type based on the FAH Core ID
        /// </summary>
        /// <param name="coreId">FAH Core ID</param>
        private static SlotType ToSlotTypeFromCoreId(string coreId)
        {
            // make this method more forgiving - rwh 9/6/10
            if (String.IsNullOrEmpty(coreId))
            {
                return SlotType.Unknown;
            }

            switch (coreId.ToUpperInvariant())
            {
                case "78": // Gromacs
                case "79": // Double Gromacs
                case "7A": // GB Gromacs
                case "7B": // Double Gromacs B
                case "7C": // Double Gromacs C
                case "80": // Gromacs SREM
                case "81": // Gromacs SIMT
                case "82": // Amber
                case "A0": // Gromacs 33
                case "B4": // ProtoMol
                case "A1": // Gromacs SMP
                case "A2": // Gromacs SMP
                case "A3": // Gromacs SMP2
                case "A5": // Gromacs SMP2
                case "A6": // Gromacs SMP2
                case "A7":
                    return SlotType.CPU;
                case "11": // GPU2 - GROGPU2
                case "12": // GPU2 - ATI-DEV
                case "13": // GPU2 - NVIDIA-DEV
                case "14": // GPU2 - GROGPU2-MT
                case "15": // GPU3 - OPENMMGPU - NVIDIA
                case "16": // GPU3 - OPENMMGPU - ATI
                case "17": // GPU3 - ZETA
                case "18": // GPU3 - ZETA_DEV
                    return SlotType.GPU;
                default:
                    return SlotType.Unknown;
            }
        }

        #endregion

        #region BonusCalculationType

        public static bool IsEnabled(this BonusCalculationType type)
        {
            return type.Equals(BonusCalculationType.DownloadTime) ||
                   type.Equals(BonusCalculationType.FrameTime);
        }

        #endregion

        #region UnitInfo

        internal static bool IsSameUnitAs(this UnitInfo unit1, UnitInfo unit2)
        {
            if (unit1 == null) throw new ArgumentNullException("unit1");

            if (unit2 == null)
            {
                return false;
            }

            // if the Projects are known
            if (!unit1.ProjectIsUnknown() && !unit2.ProjectIsUnknown())
            {
                // equals the Project and Download Time
                if (unit1.EqualsProject(unit2) &&
                    unit1.DownloadTime.Equals(unit2.DownloadTime))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region DeepClone

        public static QueryParameters DeepClone(this QueryParameters value)
        {
            return ProtoBuf.Serializer.DeepClone(value);
        }

        #endregion

        internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector, bool ignoreDuplicateKeys)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var value in source)
            {
                var key = keySelector(value);
                if (ignoreDuplicateKeys && dictionary.ContainsKey(key))
                {
                    continue;
                }
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        #region ProteinBenchmark

        internal static ProteinBenchmarkSlotIdentifier ToSlotIdentifier(this ProteinBenchmark proteinBenchmark)
        {
            return new ProteinBenchmarkSlotIdentifier(proteinBenchmark.OwningSlotName, proteinBenchmark.OwningClientPath);
        }

        #endregion

        public static string GetExecTime(this Stopwatch sw)
        {
            return String.Format("{0:#,##0} ms", sw.ElapsedMilliseconds);
        }
    }
}
