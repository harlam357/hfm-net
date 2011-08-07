/*
 * HFM.NET - Common Options Data Types and Converters
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

namespace HFM.Client.DataTypes
{
   public enum ClientTypeEnum
   {
      Unknown,
      Normal,
      Advanced,
      BigAdv
   }

   // ReSharper disable InconsistentNaming

   public enum ClientSubTypeEnum
   {
      Unknown,
      Normal,  // ???
      SMP,
      GPU      // ???
   }

   // ReSharper restore InconsistentNaming

   public enum MaxPacketSizeEnum
   {
      Unknown,
      Small,
      Normal,
      Big
   }

   public enum CorePriorityEnum
   {
      Unknown,
      Idle,
      Low
   }

   internal sealed class ClientTypeConverter : IConversionProvider
   {
      public object Convert(string input)
      {
         switch (input)
         {
            case "normal":
               return ClientTypeEnum.Normal;
            case "advanced":
               return ClientTypeEnum.Advanced;
            case "bigadv":
               return ClientTypeEnum.BigAdv;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse client-type value of '{0}'.", input));
      }
   }

   internal sealed class ClientSubTypeConverter : IConversionProvider
   {
      public object Convert(string input)
      {
         switch (input)
         {
            case "normal":
               return ClientSubTypeEnum.Normal;
            case "SMP":
               return ClientSubTypeEnum.SMP;
            case "GPU":
               return ClientSubTypeEnum.GPU;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse client-subtype value of '{0}'.", input));
      }
   }

   internal sealed class MaxPacketSizeConverter : IConversionProvider
   {
      public object Convert(string input)
      {
         switch (input)
         {
            case "small":
               return MaxPacketSizeEnum.Small;
            case "normal":
               return MaxPacketSizeEnum.Normal;
            case "big":
               return MaxPacketSizeEnum.Big;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse max-packet-size value of '{0}'.", input));
      }
   }

   internal sealed class CorePriorityConverter : IConversionProvider
   {
      public object Convert(string input)
      {
         switch (input)
         {
            case "idle":
               return CorePriorityEnum.Idle;
            case "low":
               return CorePriorityEnum.Low;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse core-priority value of '{0}'.", input));
      }
   }
}
