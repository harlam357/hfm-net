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
   internal sealed class DateTimeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString == "<invalid>")
         {
            // no value
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

         throw new ConversionProviderException(String.Format(CultureInfo.InvariantCulture,
            "Failed to convert date time value '{0}'.", inputString));
      }
   }

   internal sealed class IPAddressConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         return IPAddress.TryParse((string)input, out IPAddress value) ? value : null;
      }
   }

   internal sealed class FahClientSlotStatusConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         switch (inputString)
         {
            case "PAUSED":
               return FahClientSlotStatus.Paused;
            case "RUNNING":
               return FahClientSlotStatus.Running;
            case "FINISHING":
               return FahClientSlotStatus.Finishing;
            case "READY":
               return FahClientSlotStatus.Ready;
            case "STOPPING":
               return FahClientSlotStatus.Stopping;
            case "FAILED":
               return FahClientSlotStatus.Failed;
         }

         throw new ConversionProviderException(String.Format(CultureInfo.InvariantCulture,
            "Failed to convert status value '{0}'.", inputString));
      }
   }
}
