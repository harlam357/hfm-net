/*
 * HFM.NET - General Data Converters
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
   public interface IConversionProvider
   {
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
         try
         {
            return DateTime.ParseExact(inputString, "s", CultureInfo.InvariantCulture);
         }
         catch (FormatException)
         { }

         // custom format for older v7 clients
         try
         {
            return DateTime.ParseExact(inputString, "dd/MMM/yyyy-HH:mm:ss", CultureInfo.InvariantCulture);
         }
         catch (FormatException)
         { }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse date time value of '{0}'.", inputString));
      }
   }

   internal sealed class IPAddressConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         try
         {
            return IPAddress.Parse((string)input);
         }
         catch (FormatException ex)
         {
            throw new FormatException(String.Format(CultureInfo.InvariantCulture,
               "Failed to parse IP address value of '{0}'.", input), ex);
         }
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
            case "SEND":
               return FahSlotStatus.Send;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse status value of '{0}'.", inputString));
      }
   }
}
