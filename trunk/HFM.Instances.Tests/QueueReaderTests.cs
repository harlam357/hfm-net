/*
 * HFM.NET - Queue Reader Class Tests
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using NUnit.Framework;

using HFM.Instances;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class QueueReaderTests
   {
      [Test, Category("SMP")]
      public void WinSmpQueueTest_624R3()
      {
         QueueReader queue = new QueueReader();
         queue.ReadQueue("..\\..\\TestFiles\\WinSMP 6.24R3 queue.dat");
         Assert.AreEqual(600, queue.Version);
         
         QueueEntry entry8 = queue.GetQueueEntry(8);
         //finished
         Assert.AreEqual("171.64.65.64", entry8.ServerIP);
         Assert.AreEqual(8080, entry8.ServerPort);
         Assert.AreEqual(2653, entry8.ProjectNumber);
         Assert.AreEqual(3, entry8.ProjectRun);
         Assert.AreEqual(71, entry8.ProjectClone);
         Assert.AreEqual(119, entry8.ProjectGen);
         Assert.AreEqual(0, entry8.Benchmark);
         Assert.AreEqual(500, entry8.Misc1a);
         Assert.AreEqual(200, entry8.Misc1b);
         Assert.AreEqual(new DateTime(2009, 9, 11, 13, 34, 24, DateTimeKind.Local), entry8.ProjectIssuedLocal);
         Assert.AreEqual(new DateTime(2009, 9, 11, 13, 34, 56, DateTimeKind.Local), entry8.BeginTimeLocal);
         Assert.AreEqual(new DateTime(2009, 9, 12, 11, 7, 37, DateTimeKind.Local), entry8.EndTimeLocal);
         Assert.AreEqual(new DateTime(2009, 9, 15, 13, 34, 56, DateTimeKind.Local), entry8.DueDateLocal);
         //preferred
         Assert.AreEqual("http://www.stanford.edu/~pande/Win32/x86/Core_a1.fah", entry8.CoreDownloadUrl.AbsoluteUri);
         Assert.AreEqual("a1", entry8.CoreNumber);
         //core name
         Assert.AreEqual(1, entry8.CpuType);
         Assert.AreEqual(687, entry8.CpuSpecies);
         Assert.AreEqual("Pentium II/III", entry8.CpuString);
         Assert.AreEqual(1, entry8.OsType);
         Assert.AreEqual(8, entry8.OsSpecies);
         Assert.AreEqual("WinXP", entry8.OsString);
         Assert.AreEqual(2, entry8.NumberOfSmpCores);
         Assert.AreEqual(2, entry8.UseCores);
         Assert.AreEqual("P2653R3C71G119", entry8.WorkUnitTag);
         Assert.AreEqual(1061857101, entry8.Flops);
         Assert.AreEqual(1061.857101, entry8.MegaFlops);
         Assert.AreEqual("1061.857101", entry8.MegaFlops.ToString());
         Assert.AreEqual(1534, entry8.Memory);
         Assert.AreEqual(true, entry8.AssignmentInfoPresent);
         Assert.AreEqual(new DateTime(2009, 9, 11, 13, 34, 20, DateTimeKind.Local), entry8.AssignmentTimeStampLocal);
         //Assert.AreEqual("B9645213", entry8.AssignmentInfoChecksum); // why is my value reversed?
         Assert.AreEqual("171.67.108.25", entry8.CollectionServerIP);
         Assert.AreEqual(524286976, entry8.PacketSizeLimit);
         Assert.AreEqual("harlam357", entry8.UserName);
         Assert.AreEqual("32", entry8.TeamNumber);
         Assert.AreEqual("2A73A923B9D2FA7A", entry8.UserID);
         Assert.AreEqual(1, entry8.MachineID);
         Assert.AreEqual(2432236, entry8.WuDataFileSize);
         Assert.AreEqual("Folding@Home", entry8.WorkUnitType);
      }

      [Test, Category("SMP")]
      public void LinuxSmpQueueTest_624()
      {
         QueueReader queue = new QueueReader();
         queue.ReadQueue("..\\..\\TestFiles\\Linux SMP 6.24 queue.dat");
         Assert.AreEqual(600, queue.Version);

         QueueEntry entry4 = queue.GetQueueEntry(4);
         //finished
         Assert.AreEqual("171.64.65.56", entry4.ServerIP);
         Assert.AreEqual(8080, entry4.ServerPort);
         Assert.AreEqual(2677, entry4.ProjectNumber);
         Assert.AreEqual(33, entry4.ProjectRun);
         Assert.AreEqual(19, entry4.ProjectClone);
         Assert.AreEqual(44, entry4.ProjectGen);
         Assert.AreEqual(0, entry4.Benchmark);
         Assert.AreEqual(500, entry4.Misc1a);
         Assert.AreEqual(200, entry4.Misc1b);
         Assert.AreEqual(new DateTime(2009, 9, 13, 21, 36, 34, DateTimeKind.Local), entry4.ProjectIssuedLocal);
         Assert.AreEqual(new DateTime(2009, 9, 13, 21, 38, 21, DateTimeKind.Local), entry4.BeginTimeLocal);
         /*** Entry Not Completed ***/
         Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), entry4.EndTimeUtc); //Utc
         Assert.AreEqual(new DateTime(2009, 9, 16, 21, 38, 21, DateTimeKind.Local), entry4.DueDateLocal);
         //preferred
         Assert.AreEqual("http://www.stanford.edu/~pande/Linux/AMD64/Core_a2.fah", entry4.CoreDownloadUrl.AbsoluteUri);
         Assert.AreEqual("a2", entry4.CoreNumber);
         //core name
         Assert.AreEqual(16, entry4.CpuType);
         Assert.AreEqual(0, entry4.CpuSpecies);
         Assert.AreEqual("AMD64", entry4.CpuString);
         Assert.AreEqual(4, entry4.OsType);
         Assert.AreEqual(0, entry4.OsSpecies);
         Assert.AreEqual("Linux", entry4.OsString);
         Assert.AreEqual(2, entry4.NumberOfSmpCores);
         Assert.AreEqual(2, entry4.UseCores);
         Assert.AreEqual("P2677R33C19G44", entry4.WorkUnitTag);
         Assert.AreEqual(1060722910, entry4.Flops);
         Assert.AreEqual(1060.722910, entry4.MegaFlops);
         Assert.AreEqual("1060.72291", entry4.MegaFlops.ToString());
         Assert.AreEqual(685, entry4.Memory);
         Assert.AreEqual(true, entry4.AssignmentInfoPresent);
         Assert.AreEqual(new DateTime(2009, 9, 13, 21, 36, 31, DateTimeKind.Local), entry4.AssignmentTimeStampLocal);
         //Assert.AreEqual("B9645213", entry4.AssignmentInfoChecksum); // why is my value reversed?
         Assert.AreEqual("171.67.108.25", entry4.CollectionServerIP);
         Assert.AreEqual(524286976, entry4.PacketSizeLimit);
         Assert.AreEqual("harlam357", entry4.UserName);
         Assert.AreEqual("32", entry4.TeamNumber);
         Assert.AreEqual("9B1ED93B634D9D3D", entry4.UserID);
         Assert.AreEqual(1, entry4.MachineID);
         Assert.AreEqual(4838584, entry4.WuDataFileSize);
         Assert.AreEqual("Folding@Home", entry4.WorkUnitType);
      }

      [Test, Category("GPU")]
      public void WinGpuQueueTest_623()
      {
         QueueReader queue = new QueueReader();
         queue.ReadQueue("..\\..\\TestFiles\\GPU2 6.23 queue.dat");
         Assert.AreEqual(600, queue.Version);

         QueueEntry entry7 = queue.GetQueueEntry(7);
         //finished
         Assert.AreEqual("171.64.65.106", entry7.ServerIP);
         Assert.AreEqual(8080, entry7.ServerPort);
         Assert.AreEqual(5790, entry7.ProjectNumber);
         Assert.AreEqual(5, entry7.ProjectRun);
         Assert.AreEqual(360, entry7.ProjectClone);
         Assert.AreEqual(1, entry7.ProjectGen);
         Assert.AreEqual(0, entry7.Benchmark);
         Assert.AreEqual(500, entry7.Misc1a);
         Assert.AreEqual(200, entry7.Misc1b);
         Assert.AreEqual(new DateTime(2009, 9, 13, 19, 45, 30, DateTimeKind.Local), entry7.ProjectIssuedLocal);
         Assert.AreEqual(new DateTime(2009, 9, 13, 19, 45, 30, DateTimeKind.Local), entry7.BeginTimeLocal);
         Assert.AreEqual(new DateTime(2009, 9, 13, 21, 46, 30, DateTimeKind.Local), entry7.EndTimeLocal);
         Assert.AreEqual(new DateTime(2009, 9, 28, 19, 45, 30, DateTimeKind.Local), entry7.DueDateLocal);
         //preferred
         Assert.AreEqual("http://www.stanford.edu/~pande/Win32/x86/NVIDIA/G80/Core_11.fah", entry7.CoreDownloadUrl.AbsoluteUri);
         Assert.AreEqual("11", entry7.CoreNumber);
         //core name
         Assert.AreEqual(1, entry7.CpuType);
         Assert.AreEqual(687, entry7.CpuSpecies);
         Assert.AreEqual("Pentium II/III", entry7.CpuString);
         Assert.AreEqual(1, entry7.OsType);
         Assert.AreEqual(0, entry7.OsSpecies);
         Assert.AreEqual("Windows", entry7.OsString);
         Assert.AreEqual(0, entry7.NumberOfSmpCores);
         Assert.AreEqual(0, entry7.UseCores);
         Assert.AreEqual("P5790R5C360G1", entry7.WorkUnitTag);
         Assert.AreEqual(1065171903, entry7.Flops);
         Assert.AreEqual(1065.171903, entry7.MegaFlops);
         Assert.AreEqual("1065.171903", entry7.MegaFlops.ToString());
         Assert.AreEqual(4094, entry7.Memory);
         Assert.AreEqual(true, entry7.AssignmentInfoPresent);
         Assert.AreEqual(new DateTime(2009, 9, 13, 19, 45, 26, DateTimeKind.Local), entry7.AssignmentTimeStampLocal);
         //Assert.AreEqual("B9645213", entry7.AssignmentInfoChecksum); // why is my value reversed?
         Assert.AreEqual("171.67.108.25", entry7.CollectionServerIP);
         Assert.AreEqual(524286976, entry7.PacketSizeLimit);
         Assert.AreEqual("harlam357", entry7.UserName);
         Assert.AreEqual("32", entry7.TeamNumber);
         Assert.AreEqual("492A106C0885F10C", entry7.UserID);
         Assert.AreEqual(2, entry7.MachineID);
         Assert.AreEqual(67869, entry7.WuDataFileSize);
         Assert.AreEqual("Folding@Home", entry7.WorkUnitType);
      }
   }
}
