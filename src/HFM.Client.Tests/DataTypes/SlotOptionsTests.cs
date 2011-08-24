/*
 * HFM.NET - Slot Options Data Class Tests
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
   public class SlotOptionsTests
   {
      // ReSharper disable InconsistentNaming

      [Test]
      public void FillTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\slot-options.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("normal", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Normal, slotOptions.ClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.SMP, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(true, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_2\\slot-options.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("normal", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Normal, slotOptions.ClientTypeEnum);
         Assert.AreEqual("LINUX", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.Linux, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_3\\slot-options.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("normal", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Normal, slotOptions.ClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.SMP, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(30, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest4_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("beta", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Beta, slotOptions.ClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.SMP, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("big", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(100, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest4_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\slot-options2.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("beta", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Beta, slotOptions.ClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.GPU, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(1, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("low", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Low, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest4_3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\slot-options3.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("beta", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Beta, slotOptions.ClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.GPU, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(2, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("low", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Low, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest5_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("advanced", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Advanced, slotOptions.ClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.SMP, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("big", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(30, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest5_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\slot-options2.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("advanced", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Advanced, slotOptions.ClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.GPU, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(1, slotOptions.MachineId);
         Assert.AreEqual("big", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(30, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      [Test]
      public void FillTest6()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_6\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("advanced", slotOptions.ClientType);
         Assert.AreEqual(ClientType.Advanced, slotOptions.ClientTypeEnum);
         Assert.AreEqual("STDCLI", slotOptions.ClientSubType);
         Assert.AreEqual(ClientSubType.Normal, slotOptions.ClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuVendorId);
         Assert.AreEqual(null, slotOptions.GpuDeviceId);
      }

      // ReSharper restore InconsistentNaming
   }
}
