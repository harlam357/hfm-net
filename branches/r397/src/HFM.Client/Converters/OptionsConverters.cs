/*
 * HFM.NET - Options Data Converters
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

using HFM.Client.DataTypes;

namespace HFM.Client.Converters
{
   internal sealed class ClientTypeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         switch (inputString)
         {
            case "normal":
               return ClientType.Normal;
            case "advanced":
               return ClientType.Advanced;
            case "bigadv":
               return ClientType.BigAdv;
            case "beta":
               return ClientType.Beta;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse client-type value of '{0}'.", inputString));
      }
   }

   internal sealed class ClientSubTypeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         switch (inputString)
         {
            case "normal":
               return ClientSubType.Normal;
            case "SMP":
               return ClientSubType.SMP;
            case "GPU":
               return ClientSubType.GPU;
            case "STDCLI":
               return ClientSubType.StdCli;
            case "LINUX":
               return ClientSubType.Linux;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse client-subtype value of '{0}'.", inputString));
      }
   }

   internal sealed class MaxPacketSizeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         switch (inputString)
         {
            case "small":
               return MaxPacketSize.Small;
            case "normal":
               return MaxPacketSize.Normal;
            case "big":
               return MaxPacketSize.Big;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse max-packet-size value of '{0}'.", inputString));
      }
   }

   internal sealed class CorePriorityConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         switch (inputString)
         {
            case "idle":
               return CorePriority.Idle;
            case "low":
               return CorePriority.Low;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse core-priority value of '{0}'.", inputString));
      }
   }
}
