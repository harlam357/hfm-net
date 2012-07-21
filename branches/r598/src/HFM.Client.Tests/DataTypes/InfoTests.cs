/*
 * HFM.NET - Info Data Class Tests
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

using System.IO;

using NUnit.Framework;

using HFM.Client.DataTypes;
using HFM.Core.DataTypes;

namespace HFM.Client.Tests.DataTypes
{
   [TestFixture]
   public class ClientInfoTests
   {
      [Test]
      public void FillTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 1232 --command-port=36330", info.Client.Args);
         Assert.AreEqual("C:/Documents and Settings/user/Application Data/FAHClient/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.24", info.Build.Version);
         Assert.AreEqual("Apr  6 2011", info.Build.Date);
         Assert.AreEqual("21:37:58", info.Build.Time);
         Assert.AreEqual(2908, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1110", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /wd4297 /wd4103 /wd1786 /Ox -arch:SSE2 /QaxSSE3,SSSE3,SSE4.1,SSE4.2 /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 Vista", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
                         // this string is no longer returned in newer clients
         Assert.AreEqual("Microsoft(R) Windows(R) XP Professional x64 Edition", info.System.OperatingSystem);
                         // simply detect as Windows
         Assert.AreEqual(OperatingSystemType.Windows, info.System.OperatingSystemEnum);
         Assert.AreEqual(null, info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.Unknown, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("Intel(R) Core(TM)2 Quad CPU    Q6600  @ 2.40GHz", info.System.Cpu);
         Assert.AreEqual(CpuType.Core2, info.System.CpuType);
         Assert.AreEqual("GenuineIntel Family 6 Model 15 Stepping 11", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.Intel, info.System.CpuManufacturer);
         Assert.AreEqual(4, info.System.CpuCount);
         Assert.AreEqual("4.00GiB", info.System.Memory);
         Assert.AreEqual(4.0, info.System.MemoryValue);
         Assert.AreEqual("3.10GiB", info.System.FreeMemory);
         Assert.AreEqual(3.1, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(1, info.System.GpuCount);
         Assert.AreEqual("ATI:2 Mobility Radeon HD 3600 Series", info.System.GpuId0);
         Assert.AreEqual("Radeon HD 3600 Series", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.ATI, info.System.GpuId0Manufacturer);
         Assert.AreEqual(null, info.System.GpuId1);
         Assert.AreEqual(null, info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("Not detected", info.System.Cuda);
         Assert.AreEqual(null, info.System.CudaVersion);
         Assert.AreEqual(null, info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-5, info.System.UtcOffset);
         Assert.AreEqual(3080, info.System.ProcessId);
         Assert.AreEqual("C:/Documents and Settings/user/Application Data/FAHClient", info.System.WorkingDirectory);
         Assert.AreEqual(true, info.System.Win32Service);
      }

      [Test]
      public void FillTest2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_2\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --password yyyyy", info.Client.Args);
         Assert.AreEqual("/home/s.shelnutt/folding/FAHClient-7.1.24-Ubuntu-10.10-64bit-release/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.24", info.Build.Version);
         Assert.AreEqual("Apr  6 2011", info.Build.Date);
         Assert.AreEqual("21:24:39", info.Build.Time);
         Assert.AreEqual(2908, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ g++ 4.3 mode 1110", info.Build.Compiler);
         Assert.AreEqual("-std=gnu++98 -O3 -funroll-loops -msse2 -axSSE3,SSSE3,SSE4.1,SSE4.2 -restrict", info.Build.Options);
         Assert.AreEqual("linux2 2.6.35-22-server", info.Build.Platform);
         Assert.AreEqual(64, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
         Assert.AreEqual("Linux 2.6.36-gentoo-r1 x86_64", info.System.OperatingSystem);
         Assert.AreEqual(OperatingSystemType.Linux, info.System.OperatingSystemEnum);
         Assert.AreEqual(null, info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.Unknown, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("Intel(R) Core(TM)2 CPU          6300  @ 1.86GHz", info.System.Cpu);
         Assert.AreEqual(CpuType.Core2, info.System.CpuType);
         Assert.AreEqual("GenuineIntel Family 6 Model 15 Stepping 2", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.Intel, info.System.CpuManufacturer);
         Assert.AreEqual(2, info.System.CpuCount);
         Assert.AreEqual("1.96GiB", info.System.Memory);
         Assert.AreEqual(1.96, info.System.MemoryValue);
         Assert.AreEqual("237.61MiB", info.System.FreeMemory);
         Assert.AreEqual(0.232041015625, info.System.FreeMemoryValue);
         Assert.AreEqual("POSIX_THREADS", info.System.ThreadType);
         Assert.AreEqual(1, info.System.GpuCount);
         Assert.AreEqual("FERMI:1 GF104 [GeForce GTX 460]", info.System.GpuId0);
         Assert.AreEqual("GeForce GTX 460", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.Nvidia, info.System.GpuId0Manufacturer);
         Assert.AreEqual(null, info.System.GpuId1);
         Assert.AreEqual(null, info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("2.1", info.System.Cuda);
         Assert.AreEqual(2.1, info.System.CudaVersion);
         Assert.AreEqual("4000", info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-4, info.System.UtcOffset);
         Assert.AreEqual(1408, info.System.ProcessId);
         Assert.AreEqual("/home/s.shelnutt/folding/FAHClient-7.1.24-Ubuntu-10.10-64bit-release", info.System.WorkingDirectory);
         Assert.AreEqual(null, info.System.Win32Service);
      }

      [Test]
      public void FillTest3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_3\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 1344 --command-port=36330", info.Client.Args);
         Assert.AreEqual("C:/Users/adak/AppData/Roaming/FAHClient/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.24", info.Build.Version);
         Assert.AreEqual("Apr  6 2011", info.Build.Date);
         Assert.AreEqual("21:37:58", info.Build.Time);
         Assert.AreEqual(2908, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1110", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /wd4297 /wd4103 /wd1786 /Ox -arch:SSE2 /QaxSSE3,SSSE3,SSE4.1,SSE4.2 /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 Vista", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
         Assert.AreEqual("Microsoft Windows 7 Professional", info.System.OperatingSystem);
         Assert.AreEqual(OperatingSystemType.Windows7, info.System.OperatingSystemEnum);
         Assert.AreEqual(null, info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.Unknown, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("Intel(R) Core(TM) i7 CPU         940  @ 2.93GHz", info.System.Cpu);
         Assert.AreEqual(CpuType.Corei7, info.System.CpuType);
         Assert.AreEqual("GenuineIntel Family 6 Model 26 Stepping 4", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.Intel, info.System.CpuManufacturer);
         Assert.AreEqual(8, info.System.CpuCount);
         Assert.AreEqual("11.99GiB", info.System.Memory);
         Assert.AreEqual(11.99, info.System.MemoryValue);
         Assert.AreEqual("10.50GiB", info.System.FreeMemory);
         Assert.AreEqual(10.50, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(2, info.System.GpuCount);
         Assert.AreEqual("ATI:1 RV530 [Radeon X1600]", info.System.GpuId0);
         Assert.AreEqual("Radeon X1600", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.ATI, info.System.GpuId0Manufacturer);
         Assert.AreEqual("ATI:1 RV530 [Radeon X1600] (Secondary)", info.System.GpuId1);
         Assert.AreEqual("Radeon X1600", info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.ATI, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("Not detected", info.System.Cuda);
         Assert.AreEqual(null, info.System.CudaVersion);
         Assert.AreEqual(null, info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-7, info.System.UtcOffset);
         Assert.AreEqual(2932, info.System.ProcessId);
         Assert.AreEqual("C:/Users/adak/AppData/Roaming/FAHClient", info.System.WorkingDirectory);
         Assert.AreEqual(false, info.System.Win32Service);
      }

      [Test]
      public void FillTest4()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 3388 --command-port=36330", info.Client.Args);
         Assert.AreEqual("G:/FAH/Data/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.24", info.Build.Version);
         Assert.AreEqual("Apr  6 2011", info.Build.Date);
         Assert.AreEqual("21:37:58", info.Build.Time);
         Assert.AreEqual(2908, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1110", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /wd4297 /wd4103 /wd1786 /Ox -arch:SSE2 /QaxSSE3,SSSE3,SSE4.1,SSE4.2 /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 Vista", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
                         // this string is no longer returned in newer clients
         Assert.AreEqual("Microsoft(R) Windows(R) XP Professional x64 Edition", info.System.OperatingSystem);
                         // simply detect as Windows
         Assert.AreEqual(OperatingSystemType.Windows, info.System.OperatingSystemEnum);
         Assert.AreEqual(null, info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.Unknown, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("Intel(R) Core(TM)2 Quad  CPU   Q9450  @ 2.66GHz", info.System.Cpu);
         Assert.AreEqual(CpuType.Core2, info.System.CpuType);
         Assert.AreEqual("GenuineIntel Family 6 Model 23 Stepping 7", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.Intel, info.System.CpuManufacturer);
         Assert.AreEqual(4, info.System.CpuCount);
         Assert.AreEqual("8.00GiB", info.System.Memory);
         Assert.AreEqual(8.0, info.System.MemoryValue);
         Assert.AreEqual("6.68GiB", info.System.FreeMemory);
         Assert.AreEqual(6.68, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(2, info.System.GpuCount);
         Assert.AreEqual("NVIDIA:1 GT200 [GeForce GTX 260]", info.System.GpuId0);
         Assert.AreEqual("GeForce GTX 260", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.Nvidia, info.System.GpuId0Manufacturer);
         Assert.AreEqual("NVIDIA:1 G92 [GeForce 9800 GTX]", info.System.GpuId1);
         Assert.AreEqual("GeForce 9800 GTX", info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.Nvidia, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("1.3", info.System.Cuda);
         Assert.AreEqual(1.3, info.System.CudaVersion);
         Assert.AreEqual("3020", info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-4, info.System.UtcOffset);
         Assert.AreEqual(2884, info.System.ProcessId);
         Assert.AreEqual("G:/FAH/Data", info.System.WorkingDirectory);
         Assert.AreEqual(false, info.System.Win32Service);
      }

      [Test]
      public void FillTest5()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 3540 --command-port=36330", info.Client.Args);
         Assert.AreEqual("C:/FAH/SMPBeta/FAHClient/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.24", info.Build.Version);
         Assert.AreEqual("Apr  6 2011", info.Build.Date);
         Assert.AreEqual("21:37:58", info.Build.Time);
         Assert.AreEqual(2908, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1110", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /wd4297 /wd4103 /wd1786 /Ox -arch:SSE2 /QaxSSE3,SSSE3,SSE4.1,SSE4.2 /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 Vista", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
         Assert.AreEqual("Microsoft Windows 7 Professional", info.System.OperatingSystem);
         Assert.AreEqual(OperatingSystemType.Windows7, info.System.OperatingSystemEnum);
         Assert.AreEqual(null, info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.Unknown, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("AMD Phenom(tm) II X4 945 Processor", info.System.Cpu);
         Assert.AreEqual(CpuType.PhenomII, info.System.CpuType);
         Assert.AreEqual("AuthenticAMD Family 16 Model 4 Stepping 3", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.AMD, info.System.CpuManufacturer);
         Assert.AreEqual(4, info.System.CpuCount);
         Assert.AreEqual("4.00GiB", info.System.Memory);
         Assert.AreEqual(4.0, info.System.MemoryValue);
         Assert.AreEqual("2.64GiB", info.System.FreeMemory);
         Assert.AreEqual(2.64, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(1, info.System.GpuCount);
         Assert.AreEqual("ATI:4 Radeon HD 5870 (Cypress)", info.System.GpuId0);
         Assert.AreEqual("Radeon HD 5870 (Cypress)", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.ATI, info.System.GpuId0Manufacturer);
         Assert.AreEqual(null, info.System.GpuId1);
         Assert.AreEqual(null, info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("Not detected", info.System.Cuda);
         Assert.AreEqual(null, info.System.CudaVersion);
         Assert.AreEqual(null, info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-4, info.System.UtcOffset);
         Assert.AreEqual(760, info.System.ProcessId);
         Assert.AreEqual("C:/FAH/SMPBeta/FAHClient", info.System.WorkingDirectory);
         Assert.AreEqual(false, info.System.Win32Service);
      }

      [Test]
      public void FillTest6()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_6\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 43012", info.Client.Args);
         Assert.AreEqual("C:/Documents and Settings/All Users/Application Data/FAHClient/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.21", info.Build.Version);
         Assert.AreEqual("Mar 23 2011", info.Build.Date);
         Assert.AreEqual("15:13:48", info.Build.Time);
         Assert.AreEqual(2883, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1110", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /wd4297 /wd4103 /wd1786 /Ox -arch:SSE /QaxSSE2,SSE3,SSSE3,SSE4.1,SSE4.2 /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 XP", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
         Assert.AreEqual("Microsoft Windows XP Professional", info.System.OperatingSystem);
         Assert.AreEqual(OperatingSystemType.WindowsXP, info.System.OperatingSystemEnum);
         Assert.AreEqual(null, info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.Unknown, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("AMD Athlon(tm) Processor 1640B", info.System.Cpu);
         Assert.AreEqual(CpuType.Athlon, info.System.CpuType);
         Assert.AreEqual("AuthenticAMD Family 15 Model 127 Stepping 2", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.AMD, info.System.CpuManufacturer);
         Assert.AreEqual(1, info.System.CpuCount);
         Assert.AreEqual("1.75GiB", info.System.Memory);
         Assert.AreEqual(1.75, info.System.MemoryValue);
         Assert.AreEqual("764.64MiB", info.System.FreeMemory);
         Assert.AreEqual(0.74671875, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(1, info.System.GpuCount);
         Assert.AreEqual("ATI:2 Radeon 3100 Graphics", info.System.GpuId0);
         Assert.AreEqual("Radeon 3100 Graphics", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.ATI, info.System.GpuId0Manufacturer);
         Assert.AreEqual(null, info.System.GpuId1);
         Assert.AreEqual(null, info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("Not detected", info.System.Cuda);
         Assert.AreEqual(null, info.System.CudaVersion);
         Assert.AreEqual(null, info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-4, info.System.UtcOffset);
         Assert.AreEqual(45024, info.System.ProcessId);
         Assert.AreEqual("C:/Documents and Settings/All Users/Application Data/FAHClient", info.System.WorkingDirectory);
         Assert.AreEqual(false, info.System.Win32Service);
      }

      [Test]
      public void FillTest7()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_7\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 4040 --command-port=36330", info.Client.Args);
         Assert.AreEqual("C:/folding/FAH-V7/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.24", info.Build.Version);
         Assert.AreEqual("Apr  6 2011", info.Build.Date);
         Assert.AreEqual("21:37:50", info.Build.Time);
         Assert.AreEqual(2908, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1110", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /wd4297 /wd4103 /wd1786 /Ox -arch:SSE /QaxSSE2,SSE3,SSSE3,SSE4.1,SSE4.2 /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 XP", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
         Assert.AreEqual("Microsoft Windows XP Professional", info.System.OperatingSystem);
         Assert.AreEqual(OperatingSystemType.WindowsXP, info.System.OperatingSystemEnum);
         Assert.AreEqual(null, info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.Unknown, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("Intel(R) Core(TM)2 Quad  CPU   Q9450  @ 2.66GHz", info.System.Cpu);
         Assert.AreEqual(CpuType.Core2, info.System.CpuType);
         Assert.AreEqual("GenuineIntel Family 6 Model 23 Stepping 7", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.Intel, info.System.CpuManufacturer);
         Assert.AreEqual(4, info.System.CpuCount);
         Assert.AreEqual("3.25GiB", info.System.Memory);
         Assert.AreEqual(3.25, info.System.MemoryValue);
         Assert.AreEqual("2.54GiB", info.System.FreeMemory);
         Assert.AreEqual(2.54, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(1, info.System.GpuCount);
         Assert.AreEqual("FERMI:1 GF106 [GeForce 450 GTS]", info.System.GpuId0);
         Assert.AreEqual("GeForce 450 GTS", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.Nvidia, info.System.GpuId0Manufacturer);
         Assert.AreEqual(null, info.System.GpuId1);
         Assert.AreEqual(null, info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("2.1", info.System.Cuda);
         Assert.AreEqual(2.1, info.System.CudaVersion);
         Assert.AreEqual("3020", info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-5, info.System.UtcOffset);
         Assert.AreEqual(3204, info.System.ProcessId);
         Assert.AreEqual("C:/folding/FAH-V7", info.System.WorkingDirectory);
         Assert.AreEqual(false, info.System.Win32Service);
      }

      [Test]
      public void FillTest10()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\info.txt");
         var info = new Info();
         info.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009-2012 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 2600 --command-port=36330", info.Client.Args);
         Assert.AreEqual("", info.Client.Config);
         Assert.AreEqual("7.1.43", info.Build.Version);
         Assert.AreEqual("Jan  2 2012", info.Build.Date);
         Assert.AreEqual("12:33:05", info.Build.Time);
         Assert.AreEqual(3223, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1200", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /Qdiag-disable:4297,4103,1786,279 /Ox -arch:SSE /QaxSSE2,SSE3,SSSE3,SSE4.1,SSE4.2 /Qopenmp /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 XP", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
         Assert.AreEqual("Microsoft Windows XP Service Pack 2", info.System.OperatingSystem);
         Assert.AreEqual(OperatingSystemType.WindowsXP, info.System.OperatingSystemEnum);
         Assert.AreEqual("AMD64", info.System.OperatingSystemArchitecture);
         Assert.AreEqual(OperatingSystemArchitectureType.x64, info.System.OperatingSystemArchitectureEnum);
         Assert.AreEqual("Intel(R) Core(TM)2 Quad CPU    Q6600  @ 2.40GHz", info.System.Cpu);
         Assert.AreEqual(CpuType.Core2, info.System.CpuType);
         Assert.AreEqual("GenuineIntel Family 6 Model 15 Stepping 11", info.System.CpuId);
         Assert.AreEqual(CpuManufacturer.Intel, info.System.CpuManufacturer);
         Assert.AreEqual(4, info.System.CpuCount);
         Assert.AreEqual("4.00GiB", info.System.Memory);
         Assert.AreEqual(4.0, info.System.MemoryValue);
         Assert.AreEqual("3.22GiB", info.System.FreeMemory);
         Assert.AreEqual(3.22, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(1, info.System.GpuCount);
         Assert.AreEqual("NVIDIA:1 GT200b [GeForce GTX 285]", info.System.GpuId0);
         Assert.AreEqual("GeForce GTX 285", info.System.GpuId0Type);
         Assert.AreEqual(GpuManufacturer.Nvidia, info.System.GpuId0Manufacturer);
         Assert.AreEqual(null, info.System.GpuId1);
         Assert.AreEqual(null, info.System.GpuId1Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId1Manufacturer);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId2Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId2Manufacturer);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId3Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId3Manufacturer);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId4Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId4Manufacturer);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId5Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId5Manufacturer);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId6Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId6Manufacturer);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual(null, info.System.GpuId7Type);
         Assert.AreEqual(GpuManufacturer.Unknown, info.System.GpuId7Manufacturer);
         Assert.AreEqual("1.3", info.System.Cuda);
         Assert.AreEqual(1.3, info.System.CudaVersion);
         Assert.AreEqual("3010", info.System.CudaDriver);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-6, info.System.UtcOffset);
         Assert.AreEqual(1520, info.System.ProcessId);
         Assert.AreEqual("", info.System.WorkingDirectory);
         Assert.AreEqual(false, info.System.Win32Service);
      }
   }
}
