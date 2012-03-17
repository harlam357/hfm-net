/*
 * HFM.NET - General Data Converters
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

using System;
using System.Globalization;
using System.Net;

using HFM.Client.DataTypes;

namespace HFM.Client.Converters
{
   /// <summary>
   /// Provides functionality to convert a Folding@Home message property value to another type.  Types specified as the ConverterType of a MessagePropertyAttribute must implement this interface.
   /// </summary>
   public interface IConversionProvider
   {
      /// <summary>
      /// Returns an System.Object whose value has been converted from the specified input object.
      /// </summary>
      object Convert(object input);
   }

   internal sealed class DateTimeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString == "<invalid>")
         {
            // not an error, but no value
            return null;
         }

         // ISO 8601
         DateTime value;
         if (DateTime.TryParse(inputString, CultureInfo.InvariantCulture, 
             DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out value))
         {
            return value;
         }

         // custom format for older v7 clients
         if (DateTime.TryParseExact(inputString, "dd/MMM/yyyy-HH:mm:ss", CultureInfo.InvariantCulture,
             DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out value))
         {
            return value;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse date time value of '{0}'.", inputString));
      }
   }

   internal sealed class IPAddressConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         IPAddress value;
         if (IPAddress.TryParse((string)input, out value))
         {
            return value;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse IP address value of '{0}'.", input));
      }
   }

   internal sealed class SlotStatusConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         switch (inputString)
         {
            case "PAUSED":
               return FahSlotStatus.Paused;
            case "RUNNING":
               return FahSlotStatus.Running;
            case "FINISHING":
               return FahSlotStatus.Finishing;
            case "READY":
               return FahSlotStatus.Ready;
            case "STOPPING":
               return FahSlotStatus.Stopping;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse status value of '{0}'.", inputString));
      }
   }
}
