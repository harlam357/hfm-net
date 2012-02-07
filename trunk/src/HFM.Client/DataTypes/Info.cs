/*
 * HFM.NET - Info Data Class
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

using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json.Linq;

using HFM.Client.Converters;

namespace HFM.Client.DataTypes
{
   public class Info : TypedMessage
   {
      public Info()
      {
         Client = new ClientInfo();
         Build = new BuildInfo();
         System = new SystemInfo();
      }

      #region Properties

      [MessageProperty("Folding@home Client")]
      public ClientInfo Client { get; private set; }

      [MessageProperty("Build")]
      public BuildInfo Build { get; private set; }

      [MessageProperty("System")]
      public SystemInfo System { get; private set; }

      #endregion

      /// <summary>
      /// Fill the Info object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill(JsonMessage message)
      {
         Debug.Assert(message != null);

         var propertySetter = new MessagePropertySetter(this);
         foreach (var token in JArray.Parse(message.Value))
         {
            if (!token.HasValues)
            {
               continue;
            }

            object clientInfoProperty = propertySetter.GetPropertyValue((string)token[0]);
            if (clientInfoProperty != null)
            {
               var innerPropertySetter = new MessagePropertySetter(clientInfoProperty);
               foreach (var innerToken in token)
               {
                  if (!innerToken.HasValues)
                  {
                     continue;
                  }

                  if (innerToken.Values().Count() >= 2)
                  {
                     innerPropertySetter.SetProperty((string)innerToken[0], (string)innerToken[1]);
                  }
               }
            }
         }
         SetMessageValues(message);
      }      
   }

   public class ClientInfo
   {
      #region Properties

      [MessageProperty("Website")]
      public string Website { get; set; }

      [MessageProperty("Copyright")]
      public string Copyright { get; set; }

      [MessageProperty("Author")]
      public string Author { get; set; }

      [MessageProperty("Args")]
      public string Args { get; set; }

      [MessageProperty("Config")]
      public string Config { get; set; }

      #endregion
   }

   public class BuildInfo
   {
      #region Properties

      [MessageProperty("Version")]
      public string Version { get; set; }

      [MessageProperty("Date")]
      public string Date { get; set; }

      [MessageProperty("Time")]
      public string Time { get; set; }

      #region BuildDateTime Property (commented)

      //public DateTime? BuildDateTime
      //{
      //   get { return GetDateTime(); }
      //}

      //private DateTime? GetDateTime()
      //{
      //   if (Date != null && Time != null)
      //   {
      //      try
      //      {
      //         return DateTime.ParseExact(String.Concat(Date, " ", Time), "MMM  d yyyy HH:mm:ss", CultureInfo.InvariantCulture);
      //      }
      //      catch (FormatException)
      //      {
      //         return null;
      //      }
      //   }

      //   return null;
      //}

      #endregion

      [MessageProperty("SVN Rev")]
      public int SvnRev { get; set; }

      [MessageProperty("Branch")]
      public string Branch { get; set; }

      [MessageProperty("Compiler")]
      public string Compiler { get; set; }

      [MessageProperty("Options")]
      public string Options { get; set; }

      [MessageProperty("Platform")]
      public string Platform { get; set; }

      [MessageProperty("Bits")]
      public int Bits { get; set; }

      [MessageProperty("Mode")]
      public string Mode { get; set; }

      #endregion
   }

   public class SystemInfo
   {
      #region Properties

      [MessageProperty("OS")]
      public string OperatingSystem { get; set; }

      [MessageProperty("OS", typeof(OperatingSystemConverter))]
      public OperatingSystemType OperatingSystemEnum { get; set; }

      [MessageProperty("OS Arch")]
      public string OperatingSystemArchitecture { get; set; }

      [MessageProperty("OS Arch", typeof(OperatingSystemArchitectureConverter))]
      public OperatingSystemArchitectureType OperatingSystemArchitectureEnum { get; set; }

      [MessageProperty("CPU")]
      public string Cpu { get; set; }

      [MessageProperty("CPU", typeof(CpuTypeConverter))]
      public CpuType CpuType { get; set; }

      [MessageProperty("CPU ID")]
      public string CpuId { get; set; }

      [MessageProperty("CPU ID", typeof(CpuManufacturerConverter))]
      public CpuManufacturer CpuManufacturer { get; set; }

      [MessageProperty("CPUs")]
      public int CpuCount { get; set; }

      [MessageProperty("Memory")]
      public string Memory { get; set; }

      [MessageProperty("Memory", typeof(MemoryValueConverter))]
      public double? MemoryValue { get; set; }

      [MessageProperty("Free Memory")]
      public string FreeMemory { get; set; }

      [MessageProperty("Free Memory", typeof(MemoryValueConverter))]
      public double? FreeMemoryValue { get; set; }

      [MessageProperty("Threads")]
      public string ThreadType { get; set; }

      [MessageProperty("GPUs")]
      public int GpuCount { get; set; }

      [MessageProperty("GPU 0")]
      public string GpuId0 { get; set; }

      [MessageProperty("GPU 0", typeof(GpuTypeConverter))]
      public string GpuId0Type { get; set; }

      [MessageProperty("GPU 0", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId0Manufacturer { get; set; }

      [MessageProperty("GPU 1")]
      public string GpuId1 { get; set; }

      [MessageProperty("GPU 1", typeof(GpuTypeConverter))]
      public string GpuId1Type { get; set; }

      [MessageProperty("GPU 1", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId1Manufacturer { get; set; }

      [MessageProperty("GPU 2")]
      public string GpuId2 { get; set; }

      [MessageProperty("GPU 2", typeof(GpuTypeConverter))]
      public string GpuId2Type { get; set; }

      [MessageProperty("GPU 2", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId2Manufacturer { get; set; }

      [MessageProperty("GPU 3")]
      public string GpuId3 { get; set; }

      [MessageProperty("GPU 3", typeof(GpuTypeConverter))]
      public string GpuId3Type { get; set; }

      [MessageProperty("GPU 3", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId3Manufacturer { get; set; }

      [MessageProperty("GPU 4")]
      public string GpuId4 { get; set; }

      [MessageProperty("GPU 4", typeof(GpuTypeConverter))]
      public string GpuId4Type { get; set; }

      [MessageProperty("GPU 4", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId4Manufacturer { get; set; }

      [MessageProperty("GPU 5")]
      public string GpuId5 { get; set; }

      [MessageProperty("GPU 5", typeof(GpuTypeConverter))]
      public string GpuId5Type { get; set; }

      [MessageProperty("GPU 5", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId5Manufacturer { get; set; }

      [MessageProperty("GPU 6")]
      public string GpuId6 { get; set; }

      [MessageProperty("GPU 6", typeof(GpuTypeConverter))]
      public string GpuId6Type { get; set; }

      [MessageProperty("GPU 6", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId6Manufacturer { get; set; }

      [MessageProperty("GPU 7")]
      public string GpuId7 { get; set; }

      [MessageProperty("GPU 7", typeof(GpuTypeConverter))]
      public string GpuId7Type { get; set; }

      [MessageProperty("GPU 7", typeof(GpuManufacturerConverter))]
      public GpuManufacturer GpuId7Manufacturer { get; set; }

      [MessageProperty("CUDA")]
      public string Cuda { get; set; }

      [MessageProperty("CUDA", typeof(CudaVersionConverter))]
      public double? CudaVersion { get; set; }

      [MessageProperty("CUDA Driver")]
      public string CudaDriver { get; set; }

      [MessageProperty("On Battery")]
      public bool OnBattery { get; set; }

      [MessageProperty("UTC offset")]
      public int UtcOffset { get; set; }

      [MessageProperty("PID")]
      public int ProcessId { get; set; }

      [MessageProperty("CWD")]
      public string WorkingDirectory { get; set; }

      [MessageProperty("Win32 Service")]
      public bool? Win32Service { get; set; }

      #endregion
   }
}
