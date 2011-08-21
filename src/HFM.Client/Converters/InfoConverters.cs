/*
 * HFM.NET - Info Data Converters
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
   internal sealed class MemoryValueConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         Exception innerException = null;
         try
         {
            // always returns value in gigabytes
            int gigabyteIndex = inputString.IndexOf("GiB");
            if (gigabyteIndex > 0)
            {
               double gigabytes = Double.Parse(inputString.Substring(0, gigabyteIndex));
               return gigabytes;
            }
            int megabyteIndex = inputString.IndexOf("MiB");
            if (megabyteIndex > 0)
            {
               double megabytes = Double.Parse(inputString.Substring(0, megabyteIndex));
               return megabytes / 1024;
            }
            int kilobyteIndex = inputString.IndexOf("KiB");
            if (kilobyteIndex > 0)
            {
               double kilobytes = Double.Parse(inputString.Substring(0, kilobyteIndex));
               return kilobytes / 1048576;
            }
         }
         catch (FormatException ex)
         {
            innerException = ex;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse memory value of '{0}'.", inputString), innerException);
      }
   }

   internal sealed class OperatingSystemConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString.Contains("Windows"))
         {
            OperatingSystemType os = OperatingSystemType.Windows;

            #region Detect Specific Windows Version

            switch (inputString)
            {
               case "Microsoft(R) Windows(R) XP Professional x64 Edition":
                  os = OperatingSystemType.WindowsXPx64;
                  break;
            }

            #endregion

            return os;
         }
         if (inputString.Contains("Linux"))
         {
            return OperatingSystemType.Linux;
         }
         if (inputString.Contains("OSX"))
         {
            return OperatingSystemType.OSX;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse OS value of '{0}'.", inputString));
      }
   }

   internal sealed class CudaVersionConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString == "Not Detected")
         {
            // not an error, but no value
            return null;
         }

         Exception innerException;
         try
         {
            return Double.Parse(inputString);
         }
         catch (FormatException ex)
         {
            innerException = ex;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse CUDA version value of '{0}'.", inputString), innerException);
      }
   }
}
