/*
 * HFM.NET - Info Data Class
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Newtonsoft.Json.Linq;

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

   // ReSharper disable InconsistentNaming

   public enum OperatingSystem
   {
      Unknown,
      Windows,
      WindowsXP,
      WindowsXPx64,
      Vista32,
      Vista64,
      Windows7x32,
      Windows7x64,
      Linux,
      OSX

      // Expand Linux and OSX members if necessary
   }

   // ReSharper restore InconsistentNaming

   public class SystemInfo
   {
      #region Properties

      [MessageProperty("OS")]
      public string OperatingSystem { get; set; }

      [MessageProperty("OS", typeof(OperatingSystemConverter))]
      public OperatingSystem OperatingSystemEnum { get; set; }

      [MessageProperty("CPU")]
      public string Cpu { get; set; }

      [MessageProperty("CPU ID")]
      public string CpuId { get; set; }

      //TODO: parse CpuEnum value from Cpu property
      //public CpuEnum CpuEnum
      //{
      //   get { return ???; }
      //}

      [MessageProperty("CPUs")]
      public int CpuCount { get; set; }

      [MessageProperty("Memory")]
      public string Memory { get; set; }

      //[MessageProperty("Memory", typeof(MemoryValueConverter))]
      //public double MemoryValue { get; set; }

      [MessageProperty("Free Memory")]
      public string FreeMemory { get; set; }

      //[MessageProperty("Free Memory", typeof(MemoryValueConverter))]
      //public double FreeMemoryValue { get; set; }

      [MessageProperty("Threads")]
      public string ThreadType { get; set; }

      [MessageProperty("GPUs")]
      public int GpuCount { get; set; }

      //TODO: parse GpuEnum value from GpuId properties

      [MessageProperty("GPU 0")]
      public string GpuId0 { get; set; }

      [MessageProperty("GPU 1")]
      public string GpuId1 { get; set; }

      [MessageProperty("GPU 2")]
      public string GpuId2 { get; set; }

      [MessageProperty("GPU 3")]
      public string GpuId3 { get; set; }

      [MessageProperty("GPU 4")]
      public string GpuId4 { get; set; }

      [MessageProperty("GPU 5")]
      public string GpuId5 { get; set; }

      [MessageProperty("GPU 6")]
      public string GpuId6 { get; set; }

      [MessageProperty("GPU 7")]
      public string GpuId7 { get; set; }

      [MessageProperty("CUDA")]
      public string Cuda { get; set; }

      //TODO: parse CudaDetected value from Cuda property (suspect the values are "Detected" and "Not detected")
      //public bool CudaDetected
      //{
      //   get { return ???; }
      //}

      [MessageProperty("On Battery")]
      public bool OnBattery { get; set; }

      [MessageProperty("UTC offset")]
      public int UtcOffset { get; set; }

      [MessageProperty("PID")]
      public int ProcessId { get; set; }

      [MessageProperty("CWD")]
      public string WorkingDirectory { get; set; }

      [MessageProperty("Win32 Service")]
      public bool Win32Service { get; set; }

      #endregion
   }

   #region IConversionProvider Classes

   //internal sealed class MemoryValueConverter : IConversionProvider
   //{
   //   public object Convert(string input)
   //   {
   //      int gigabyteIndex = input.IndexOf("GiB");
   //      if (gigabyteIndex > 0)
   //      {
   //         double gigabytes = Double.Parse(input.Substring(0, gigabyteIndex));
   //         return gigabytes;
   //      }

   //      return 0;
   //   }
   //}

   internal sealed class OperatingSystemConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputString = (string)input;
         if (inputString.Contains("Windows"))
         {
            OperatingSystem os = OperatingSystem.Windows;

            #region Detect Specific Windows Version

            switch (inputString)
            {
               case "Microsoft(R) Windows(R) XP Professional x64 Edition":
                  os = OperatingSystem.WindowsXPx64;
                  break;
            }

            #endregion

            return os;
         }
         if (inputString.Contains("Linux"))
         {
            return OperatingSystem.Linux;
         }
         if (inputString.Contains("OSX"))
         {
            return OperatingSystem.OSX;
         }

         throw new FormatException(String.Format(CultureInfo.InvariantCulture,
            "Failed to parse OS value of '{0}'.", inputString));
      }
   }

   #endregion
}
