/*
 * HFM.NET - Info Data Converters
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
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Client.Converters
{
   internal sealed class MemoryValueConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;

         double value;
         // always returns value in gigabytes
         int gigabyteIndex = inputString.IndexOf("GiB", StringComparison.Ordinal);
         if (gigabyteIndex > 0)
         {
            if (Double.TryParse(inputString.Substring(0, gigabyteIndex), out value))
            {
               return value;
            }
         }
         int megabyteIndex = inputString.IndexOf("MiB", StringComparison.Ordinal);
         if (megabyteIndex > 0)
         {
            if (Double.TryParse(inputString.Substring(0, megabyteIndex), out value))
            {
               return value / 1024;
            }
         }
         int kilobyteIndex = inputString.IndexOf("KiB", StringComparison.Ordinal);
         if (kilobyteIndex > 0)
         {
            if (Double.TryParse(inputString.Substring(0, kilobyteIndex), out value))
            {
               return value / 1048576;   
            }
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse memory value of '{0}'.", inputString));
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

            if (inputString.Contains("Windows XP"))
            {
               os = OperatingSystemType.WindowsXP;
            }
            else if (inputString.Contains("Vista"))
            {
               os = OperatingSystemType.WindowsVista;
            }
            else if (inputString.Contains("Windows 7"))
            {
               os = OperatingSystemType.Windows7;
            }
            else if (inputString.Contains("Windows 8"))
            {
               os = OperatingSystemType.Windows8;
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
            "Failed to parse OS value from '{0}'.", inputString));
      }
   }

   internal sealed class OperatingSystemArchitectureConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString == "X86")
         {
            return OperatingSystemArchitectureType.x86;
         }
         if (inputString == "AMD64")
         {
            return OperatingSystemArchitectureType.x64;
         }
         
         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse OS value from '{0}'.", inputString));
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

         double value;
         if (Double.TryParse(inputString, out value))
         {
            return value;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse CUDA version value of '{0}'.", inputString));
      }
   }

   internal sealed class CpuManufacturerConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString.Contains("GenuineIntel"))
         {
            return CpuManufacturer.Intel;
         }
         if (inputString.Contains("AuthenticAMD"))
         {
            return CpuManufacturer.AMD;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse CPU manufacturer value from '{0}'.", inputString));
      }
   }

   internal sealed class CpuTypeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString.Contains("Core(TM)2"))
         {
            return CpuType.Core2;
         }
         if (inputString.Contains("Core(TM) i7"))
         {
            return CpuType.Corei7;
         }
         // assumed, no unit tests
         if (inputString.Contains("Core(TM) i5"))
         {
            return CpuType.Corei5;
         }
         // assumed, no unit tests
         if (inputString.Contains("Core(TM) i3"))
         {
            return CpuType.Corei3;
         }
         if (inputString.Contains("Core(TM) i3"))
         {
            return CpuType.Corei3;
         }
         if (inputString.Contains("Phenom(tm) II"))
         {
            return CpuType.PhenomII;
         }
         // assumed, no unit tests
         if (inputString.Contains("Phenom(tm)"))
         {
            return CpuType.PhenomII;
         }
         if (inputString.Contains("Athlon(tm)"))
         {
            return CpuType.Athlon;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse CPU type value from '{0}'.", inputString));
      }
   }

   internal sealed class GpuManufacturerConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString.Contains("ATI"))
         {
            return GpuManufacturer.ATI;
         }
         if (inputString.Contains("NVIDIA") || inputString.Contains("FERMI"))
         {
            return GpuManufacturer.Nvidia;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse GPU manufacturer value from '{0}'.", inputString));
      }
   }

   internal sealed class GpuTypeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         if (input == null)
         {
            // not an error, but no value
            return null;
         }

         var inputString = (string)input;

         var regex1 = new Regex("\\[(?<GpuType>.+)\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
         Match matchRegex1;
         if ((matchRegex1 = regex1.Match(inputString)).Success)
         {
            return matchRegex1.Result("${GpuType}");
         }

         int radeonIndex = inputString.IndexOf("Radeon", StringComparison.Ordinal);
         if (radeonIndex >= 0)
         {
            return inputString.Substring(radeonIndex, inputString.Length - radeonIndex);
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse GPU type value from '{0}'.", inputString));
      }
   }
}
