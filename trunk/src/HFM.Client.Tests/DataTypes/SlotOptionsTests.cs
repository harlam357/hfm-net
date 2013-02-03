/*
 * HFM.NET - Slot Options Data Class Tests
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
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("normal", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Normal, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(true, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_2\\slot-options.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("normal", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Normal, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("LINUX", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_3\\slot-options.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("normal", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Normal, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(30, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest4_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("beta", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("big", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(100, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest4_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\slot-options2.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("beta", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.GPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(1, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("low", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Low, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest4_3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\slot-options3.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("beta", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.GPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(2, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("low", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Low, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest5_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("advanced", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Advanced, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("big", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(30, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest5_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\slot-options2.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("advanced", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Advanced, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.GPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(1, slotOptions.MachineId);
         Assert.AreEqual("big", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(30, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest6()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_6\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("advanced", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Advanced, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("STDCLI", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest7_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_7\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("beta", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Beta, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("big", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Big, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(100, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest7_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_7\\slot-options2.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("normal", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Normal, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.GPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(1, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(false, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest10_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("normal", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Normal, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(true, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest10_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options2.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("normal", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Normal, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("GPU", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.GPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(1, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(true, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(null, slotOptions.GpuUsage);
      }

      [Test]
      public void FillTest11()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(null, slotOptions.Cpus);
         Assert.AreEqual("normal", slotOptions.FahClientType);
         Assert.AreEqual(FahClientType.Normal, slotOptions.FahClientTypeEnum);
         Assert.AreEqual("SMP", slotOptions.FahClientSubType);
         Assert.AreEqual(FahClientSubType.CPU, slotOptions.FahClientSubTypeEnum);
         Assert.AreEqual(0, slotOptions.MachineId);
         Assert.AreEqual("normal", slotOptions.MaxPacketSize);
         Assert.AreEqual(MaxPacketSize.Normal, slotOptions.MaxPacketSizeEnum);
         Assert.AreEqual("idle", slotOptions.CorePriority);
         Assert.AreEqual(CorePriority.Idle, slotOptions.CorePriorityEnum);
         Assert.AreEqual(99, slotOptions.NextUnitPercentage);
         Assert.AreEqual(0, slotOptions.MaxUnits);
         Assert.AreEqual(15, slotOptions.Checkpoint);
         Assert.AreEqual(true, slotOptions.PauseOnStart);
         Assert.AreEqual(null, slotOptions.GpuIndex);
         Assert.AreEqual(100, slotOptions.GpuUsage);
      }

      // ReSharper restore InconsistentNaming
   }
}
