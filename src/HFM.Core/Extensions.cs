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
using System.Globalization;

using HFM.Client.DataTypes;
using HFM.Core.Client;

namespace HFM.Core
{
    public static class Extensions
    {
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

        internal static string AppendSlotId(this string name, int slotId)
        {
            return slotId >= 0 ? String.Format(CultureInfo.InvariantCulture, "{0} Slot {1:00}", name, slotId) : name;
        }

        
    }
}
