/*
 * HFM.NET - Info Data Class Tests
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
using System.IO;

using NUnit.Framework;

using HFM.Client.DataTypes;

namespace HFM.Client.Tests.DataTypes
{
   [TestFixture]
   public class ClientInfoTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt");
         var info = Info.Parse(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("http://folding.stanford.edu/", info.Client.Website);
         Assert.AreEqual("(c) 2009,2010 Stanford University", info.Client.Copyright);
         Assert.AreEqual("Joseph Coffland <joseph@cauldrondevelopment.com>", info.Client.Author);
         Assert.AreEqual(" --lifeline 1232 --command-port=36330", info.Client.Args);
         Assert.AreEqual("C:/Documents and Settings/user/Application Data/FAHClient/config.xml", info.Client.Config);
         Assert.AreEqual("7.1.24", info.Build.Version);
         Assert.AreEqual("Apr  6 2011", info.Build.Date);
         Assert.AreEqual("21:37:58", info.Build.Time);
         //Assert.AreEqual(new DateTime(2011, 4, 6, 21, 37, 58), info.Build.BuildDateTime);
         Assert.AreEqual(2908, info.Build.SvnRev);
         Assert.AreEqual("fah/trunk/client", info.Build.Branch);
         Assert.AreEqual("Intel(R) C++ MSVC 1500 mode 1110", info.Build.Compiler);
         Assert.AreEqual("/TP /nologo /EHa /wd4297 /wd4103 /wd1786 /Ox -arch:SSE2 /QaxSSE3,SSSE3,SSE4.1,SSE4.2 /Qrestrict /MT", info.Build.Options);
         Assert.AreEqual("win32 Vista", info.Build.Platform);
         Assert.AreEqual(32, info.Build.Bits);
         Assert.AreEqual("Release", info.Build.Mode);
         Assert.AreEqual("Microsoft(R) Windows(R) XP Professional x64 Edition", info.System.OperatingSystem);
         //Assert.AreEqual(OperatingSystemType.WindowsXPx64, info.System.OperatingSystemEnum);
         Assert.AreEqual("Intel(R) Core(TM)2 Quad CPU    Q6600  @ 2.40GHz", info.System.Cpu);
         Assert.AreEqual("GenuineIntel Family 6 Model 15 Stepping 11", info.System.CpuId);
         Assert.AreEqual(4, info.System.CpuCount);
         Assert.AreEqual("4.00GiB", info.System.Memory);
         //Assert.AreEqual(4.0, info.System.MemoryValue);
         Assert.AreEqual("3.10GiB", info.System.FreeMemory);
         //Assert.AreEqual(3.1, info.System.FreeMemoryValue);
         Assert.AreEqual("WINDOWS_THREADS", info.System.ThreadType);
         Assert.AreEqual(1, info.System.GpuCount);
         Assert.AreEqual("ATI:2 Mobility Radeon HD 3600 Series", info.System.GpuId0);
         Assert.AreEqual(null, info.System.GpuId1);
         Assert.AreEqual(null, info.System.GpuId2);
         Assert.AreEqual(null, info.System.GpuId3);
         Assert.AreEqual(null, info.System.GpuId4);
         Assert.AreEqual(null, info.System.GpuId5);
         Assert.AreEqual(null, info.System.GpuId6);
         Assert.AreEqual(null, info.System.GpuId7);
         Assert.AreEqual("Not detected", info.System.Cuda);
         Assert.AreEqual(false, info.System.OnBattery);
         Assert.AreEqual(-5, info.System.UtcOffset);
         Assert.AreEqual(3080, info.System.ProcessId);
         Assert.AreEqual("C:/Documents and Settings/user/Application Data/FAHClient", info.System.WorkingDirectory);
         Assert.AreEqual(true, info.System.Win32Service);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ParseNullArgumentTest()
      {
         Info.Parse(null);
      }
   }
}
