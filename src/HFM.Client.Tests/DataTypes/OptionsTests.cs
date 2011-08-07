/*
 * HFM.NET - Options Data Class Tests
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
   public class OptionsTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\options.txt");
         var options = Options.Parse(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("assign3.stanford.edu:8080 assign4.stanford.edu:80", options.AssignmentServers);
         Assert.AreEqual("capture", options.CaptureDirectory);
         Assert.AreEqual(false, options.CaptureSockets);
         Assert.AreEqual(15, options.Checkpoint);
         Assert.AreEqual(false, options.Child);
         Assert.AreEqual("STDCLI", options.ClientSubType);
         Assert.AreEqual("normal", options.ClientType);
         Assert.AreEqual(ClientTypeEnum.Normal, options.ClientTypeEnum);
         Assert.AreEqual("0.0.0.0", options.CommandAddress);
         Assert.AreEqual("127.0.0.1", options.CommandAllow);
         Assert.AreEqual("127.0.0.1", options.CommandAllowNoPass);
         Assert.AreEqual("0.0.0.0/0", options.CommandDeny);
         Assert.AreEqual("0.0.0.0/0", options.CommandDenyNoPass);
         Assert.AreEqual(36330, options.CommandPort);
         Assert.AreEqual(true, options.ConfigRotate);
         Assert.AreEqual("configs", options.ConfigRotateDir);
         Assert.AreEqual(16, options.ConfigRotateMax);
         Assert.AreEqual("cores", options.CoreDir);
         Assert.AreEqual(null, options.CoreKey);
         Assert.AreEqual(null, options.CorePrep);
         Assert.AreEqual("idle", options.CorePriority);
         Assert.AreEqual(CorePriorityEnum.Idle, options.CorePriorityEnum);
         Assert.AreEqual(null, options.CoreServer);
         Assert.AreEqual(false, options.CpuAffinity);
         Assert.AreEqual("X86_PENTIUM_II", options.CpuSpecies);
         Assert.AreEqual("X86", options.CpuType);
         Assert.AreEqual(100, options.CpuUsage);
         Assert.AreEqual(4, options.Cpus);
         Assert.AreEqual(4, options.CycleRate);
         Assert.AreEqual(-1, options.Cycles);
         Assert.AreEqual(false, options.Daemon);
         Assert.AreEqual(".", options.DataDirectory);
         Assert.AreEqual(false, options.DebugSockets);
         Assert.AreEqual(true, options.DumpAfterDeadline);
         Assert.AreEqual(null, options.Eval);
         Assert.AreEqual(true, options.ExceptionLocations);
         Assert.AreEqual("C:\\Program Files (x86)\\FAHClient", options.ExecDirectory);
         Assert.AreEqual(false, options.ExitWhenDone);
         Assert.AreEqual(null, options.ExtraCoreArgs);
         Assert.AreEqual(null, options.ForceWs);
         Assert.AreEqual(false, options.Gpu);
         Assert.AreEqual("assign-GPU.stanford.edu:80 assign-GPU.stanford.edu:8080", options.GpuAssignmentServers);
         Assert.AreEqual(null, options.GpuDeviceId);
         Assert.AreEqual(0, options.GpuId);
         Assert.AreEqual(null, options.GpuIndex);
         Assert.AreEqual(null, options.GpuVendorId);
         Assert.AreEqual("log.txt", options.Log);
         Assert.AreEqual(false, options.LogColor);
         Assert.AreEqual(true, options.LogCrlf);
         Assert.AreEqual(false, options.LogDate);
         Assert.AreEqual(true, options.LogDebug);
         Assert.AreEqual(false, options.LogDomain);
         Assert.AreEqual(null, options.LogDomainLevels);
         Assert.AreEqual(true, options.LogHeader);
         Assert.AreEqual(true, options.LogLevel);
         Assert.AreEqual(true, options.LogNoInfoHeader);
         Assert.AreEqual(false, options.LogRedirect);
         Assert.AreEqual(true, options.LogRotate);
         Assert.AreEqual("logs", options.LogRotateDir);
         Assert.AreEqual(16, options.LogRotateMax);
         Assert.AreEqual(false, options.LogShortLevel);
         Assert.AreEqual(true, options.LogSimpleDomains);
         Assert.AreEqual(false, options.LogThreadId);
         Assert.AreEqual(true, options.LogTime);
         Assert.AreEqual(true, options.LogToScreen);
         Assert.AreEqual(false, options.LogTruncate);
         Assert.AreEqual(0, options.MachineId);
         Assert.AreEqual(21600, options.MaxDelay);
         Assert.AreEqual("normal", options.MaxPacketSize);
         Assert.AreEqual(MaxPacketSizeEnum.Normal, options.MaxPacketSizeEnum);
         Assert.AreEqual(16, options.MaxQueue);
         Assert.AreEqual(60, options.MaxShutdownWait);
         Assert.AreEqual(5, options.MaxSlotErrors);
         Assert.AreEqual(5, options.MaxUnitErrors);
         Assert.AreEqual(0, options.MaxUnits);
         Assert.AreEqual(null, options.Memory);
         Assert.AreEqual(60, options.MinDelay);
         Assert.AreEqual(99, options.NextUnitPercentage);
         Assert.AreEqual(null, options.Priority);
         Assert.AreEqual(false, options.NoAssembly);
         Assert.AreEqual("WIN_2003_SERVER", options.OsSpecies);
         Assert.AreEqual("WIN32", options.OsType);
         Assert.AreEqual("xxxxx", options.Passkey);
         Assert.AreEqual("yyyyy", options.Password);
         Assert.AreEqual(false, options.PauseOnBattery);
         Assert.AreEqual(false, options.PauseOnStart);
         Assert.AreEqual(false, options.Pid);
         Assert.AreEqual("Folding@home Client.pid", options.PidFile);
         Assert.AreEqual(0, options.ProjectKey);
         Assert.AreEqual(":8080", options.Proxy);
         Assert.AreEqual(false, options.ProxyEnable);
         Assert.AreEqual(String.Empty, options.ProxyPass);
         Assert.AreEqual(String.Empty, options.ProxyUser);
         Assert.AreEqual(false, options.Respawn);
         Assert.AreEqual(null, options.Script);
         Assert.AreEqual(false, options.Service);
         Assert.AreEqual("Folding@home Client", options.ServiceDescription);
         Assert.AreEqual(true, options.ServiceRestart);
         Assert.AreEqual(5000, options.ServiceRestartDelay);
         Assert.AreEqual(true, options.Smp);
         Assert.AreEqual(false, options.StackTraces);
         Assert.AreEqual(32, options.Team);
         Assert.AreEqual(4, options.Threads);
         Assert.AreEqual("harlam357", options.User);
         Assert.AreEqual(5, options.Verbosity);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ParseNullArgumentTest()
      {
         Options.Parse(null);
      }
   }
}
