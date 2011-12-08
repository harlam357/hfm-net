/*
 * HFM.NET - Unit Collection Data Class Tests
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
using System.Net;

using NUnit.Framework;

using HFM.Client.DataTypes;

namespace HFM.Client.Tests.DataTypes
{
   [TestFixture]
   public class UnitCollectionTests
   {
      [Test]
      public void FillTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(0, unitCollection[0].Id);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(11020, unitCollection[0].Project);
         Assert.AreEqual(0, unitCollection[0].Run);
         Assert.AreEqual(1921, unitCollection[0].Clone);
         Assert.AreEqual(24, unitCollection[0].Gen);
         Assert.AreEqual("0xa3", unitCollection[0].Core);
         Assert.AreEqual("0x000000210a3b1e5b4d824701aee79f1e", unitCollection[0].UnitId);
         Assert.AreEqual("59.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(1000, unitCollection[0].TotalFrames);
         Assert.AreEqual(590, unitCollection[0].FramesDone);
         Assert.AreEqual("27/May/2011-19:34:24", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 5, 27, 19, 34, 24), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("04/Jun/2011-19:34:24", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 6, 4, 19, 34, 24), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("08/Jun/2011-19:34:24", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 6, 8, 19, 34, 24), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("171.64.65.55", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 55 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.26", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 26 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("2 hours 28 mins", unitCollection[0].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(2, 28, 0), unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(1749.96, unitCollection[0].Ppd);
         Assert.AreEqual("3 mins 38 secs", unitCollection[0].Tpf);
         Assert.AreEqual(new TimeSpan(0, 3, 38), unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(443, unitCollection[0].BaseCredit);
         Assert.AreEqual(443, unitCollection[0].CreditEstimate);
      }

      [Test]
      public void FillDerivedTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill<UnitDerived>(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(0, unitCollection[0].Id);
         Assert.AreEqual("00", ((UnitDerived)unitCollection[0]).IdString);
         Assert.AreEqual(null, ((UnitDerived)unitCollection[0]).IdBool);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(11020, unitCollection[0].Project);
         Assert.AreEqual(0, unitCollection[0].Run);
         Assert.AreEqual(1921, unitCollection[0].Clone);
         Assert.AreEqual(24, unitCollection[0].Gen);
         Assert.AreEqual("0xa3", unitCollection[0].Core);
         Assert.AreEqual("0x000000210a3b1e5b4d824701aee79f1e", unitCollection[0].UnitId);
         Assert.AreEqual("59.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(1000, unitCollection[0].TotalFrames);
         Assert.AreEqual(590, unitCollection[0].FramesDone);
         Assert.AreEqual("27/May/2011-19:34:24", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 5, 27, 19, 34, 24), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("04/Jun/2011-19:34:24", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 6, 4, 19, 34, 24), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("08/Jun/2011-19:34:24", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 6, 8, 19, 34, 24), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("171.64.65.55", unitCollection[0].WorkServer);
         Assert.AreEqual("171.67.108.26", unitCollection[0].CollectionServer);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("2 hours 28 mins", unitCollection[0].Eta);
         Assert.AreEqual(new TimeSpan(2, 28, 0), unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(1749.96, unitCollection[0].Ppd);
         Assert.AreEqual("3 mins 38 secs", unitCollection[0].Tpf);
         Assert.AreEqual(new TimeSpan(0, 3, 38), unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(443, unitCollection[0].BaseCredit);
         Assert.AreEqual(443, unitCollection[0].CreditEstimate);
      }

      [Test]
      [ExpectedException(typeof(InvalidCastException))]
      public void FillNotDerivedTest()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill<UnitNotDerived>(MessageCache.GetNextJsonMessage(ref message));
      }

      [Test]
      public void FillTest2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_2\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(0, unitCollection[0].Id);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(10083, unitCollection[0].Project);
         Assert.AreEqual(0, unitCollection[0].Run);
         Assert.AreEqual(17, unitCollection[0].Clone);
         Assert.AreEqual(24, unitCollection[0].Gen);
         Assert.AreEqual("0xa4", unitCollection[0].Core);
         Assert.AreEqual("0x000000480001329c4ddbf194abdd0077", unitCollection[0].UnitId);
         Assert.AreEqual("0.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(10000, unitCollection[0].TotalFrames);
         Assert.AreEqual(0, unitCollection[0].FramesDone);
         Assert.AreEqual("09/Aug/2011-02:54:54", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 9, 2, 54, 54), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("17/Aug/2011-02:54:54", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 17, 2, 54, 54), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("20/Aug/2011-02:54:54", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 20, 2, 54, 54), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("129.74.85.15", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 129, 74, 85, 15 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("129.74.85.16", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 129, 74, 85, 16 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("0.00 secs", unitCollection[0].Eta);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(0.0, unitCollection[0].Ppd);
         Assert.AreEqual("0.00 secs", unitCollection[0].Tpf);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(600, unitCollection[0].BaseCredit);
         Assert.AreEqual(600, unitCollection[0].CreditEstimate);
      }

      [Test]
      public void FillTest3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_3\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(0, unitCollection[0].Id);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(7018, unitCollection[0].Project);
         Assert.AreEqual(2, unitCollection[0].Run);
         Assert.AreEqual(76, unitCollection[0].Clone);
         Assert.AreEqual(18, unitCollection[0].Gen);
         Assert.AreEqual("0xa4", unitCollection[0].Core);
         Assert.AreEqual("0x0000002c0001329c4dfba21231353df5", unitCollection[0].UnitId);
         Assert.AreEqual("3.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(10000, unitCollection[0].TotalFrames);
         Assert.AreEqual(300, unitCollection[0].FramesDone);
         Assert.AreEqual("09/Aug/2011-05:40:17", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 9, 5, 40, 17), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("17/Aug/2011-05:40:17", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 17, 5, 40, 17), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("20/Aug/2011-05:40:17", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 20, 5, 40, 17), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("129.74.85.15", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 129, 74, 85, 15 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("129.74.85.16", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 129, 74, 85, 16 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("3 hours 32 mins", unitCollection[0].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(3, 32, 0), unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(3937.41, unitCollection[0].Ppd);
         Assert.AreEqual("2 mins 11 secs", unitCollection[0].Tpf);
         Assert.AreEqual(new TimeSpan(0, 2, 11), unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(600, unitCollection[0].BaseCredit);
         Assert.AreEqual(600, unitCollection[0].CreditEstimate);
      }

      [Test]
      public void FillTest4()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(1, unitCollection[0].Id);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(7507, unitCollection[0].Project);
         Assert.AreEqual(0, unitCollection[0].Run);
         Assert.AreEqual(34, unitCollection[0].Clone);
         Assert.AreEqual(1, unitCollection[0].Gen);
         Assert.AreEqual("0xa3", unitCollection[0].Core);
         Assert.AreEqual("0x00000001fbcb017d4e495fa0e0a7ef94", unitCollection[0].UnitId);
         Assert.AreEqual("36.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(500, unitCollection[0].TotalFrames);
         Assert.AreEqual(180, unitCollection[0].FramesDone);
         Assert.AreEqual("17/Aug/2011-15:14:58", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 17, 15, 14, 58), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("21/Aug/2011-12:50:58", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 21, 12, 50, 58), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("24/Aug/2011-03:14:58", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 24, 3, 14, 58), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("128.143.199.97", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 128, 143, 199, 97 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("130.237.165.141", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 130, 237, 165, 141 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("9 hours 14 mins", unitCollection[0].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(9, 14, 0), unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(1332.08, unitCollection[0].Ppd);
         Assert.AreEqual("8 mins 42 secs", unitCollection[0].Tpf);
         Assert.AreEqual(new TimeSpan(0, 8, 42), unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(805, unitCollection[0].BaseCredit);
         Assert.AreEqual(805, unitCollection[0].CreditEstimate);

         Assert.AreEqual(0, unitCollection[1].Id);
         Assert.AreEqual("RUNNING", unitCollection[1].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[1].StateEnum);
         Assert.AreEqual(5788, unitCollection[1].Project);
         Assert.AreEqual(9, unitCollection[1].Run);
         Assert.AreEqual(838, unitCollection[1].Clone);
         Assert.AreEqual(9, unitCollection[1].Gen);
         Assert.AreEqual("0x11", unitCollection[1].Core);
         Assert.AreEqual("0x26fdfab84e4c0607000903460009169c", unitCollection[1].UnitId);
         Assert.AreEqual("96.00%", unitCollection[1].PercentDone);
         Assert.AreEqual(20000, unitCollection[1].TotalFrames);
         Assert.AreEqual(19200, unitCollection[1].FramesDone);
         Assert.AreEqual("17/Aug/2011-18:18:47", unitCollection[1].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 17, 18, 18, 47), unitCollection[1].AssignedDateTime);
         Assert.AreEqual("<invalid>", unitCollection[1].Timeout);
         Assert.AreEqual(null, unitCollection[1].TimeoutDateTime);
         Assert.AreEqual("01/Sep/2011-18:18:47", unitCollection[1].Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 1, 18, 18, 47), unitCollection[1].DeadlineDateTime);
         Assert.AreEqual("171.64.65.106", unitCollection[1].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 106 }), unitCollection[1].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.25", unitCollection[1].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 25 }), unitCollection[1].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[1].WaitingOn);
         Assert.AreEqual(0, unitCollection[1].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[1].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[1].NextAttemptTimeSpan);
         Assert.AreEqual(1, unitCollection[1].Slot);
         Assert.AreEqual("4 mins 22 secs", unitCollection[1].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 4, 22), unitCollection[1].EtaTimeSpan);
         Assert.AreEqual(0, unitCollection[1].Ppd);
         Assert.AreEqual("1 mins 21 secs", unitCollection[1].Tpf);
         Assert.AreEqual(new TimeSpan(0, 1, 21), unitCollection[1].TpfTimeSpan);
         Assert.AreEqual(0, unitCollection[1].BaseCredit);
         Assert.AreEqual(0, unitCollection[1].CreditEstimate);

         Assert.AreEqual(2, unitCollection[2].Id);
         Assert.AreEqual("RUNNING", unitCollection[2].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[2].StateEnum);
         Assert.AreEqual(5796, unitCollection[2].Project);
         Assert.AreEqual(19, unitCollection[2].Run);
         Assert.AreEqual(79, unitCollection[2].Clone);
         Assert.AreEqual(5, unitCollection[2].Gen);
         Assert.AreEqual("0x11", unitCollection[2].Core);
         Assert.AreEqual("0x5df0a3a64e4c24b70005004f001316a4", unitCollection[2].UnitId);
         Assert.AreEqual("4.00%", unitCollection[2].PercentDone);
         Assert.AreEqual(20000, unitCollection[2].TotalFrames);
         Assert.AreEqual(800, unitCollection[2].FramesDone);
         Assert.AreEqual("17/Aug/2011-20:29:43", unitCollection[2].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 17, 20, 29, 43), unitCollection[2].AssignedDateTime);
         Assert.AreEqual("<invalid>", unitCollection[2].Timeout);
         Assert.AreEqual(null, unitCollection[2].TimeoutDateTime);
         Assert.AreEqual("01/Sep/2011-20:29:43", unitCollection[2].Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 1, 20, 29, 43), unitCollection[2].DeadlineDateTime);
         Assert.AreEqual("171.64.65.106", unitCollection[2].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 106 }), unitCollection[2].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.25", unitCollection[2].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 25 }), unitCollection[2].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[2].WaitingOn);
         Assert.AreEqual(0, unitCollection[2].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[2].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[2].NextAttemptTimeSpan);
         Assert.AreEqual(2, unitCollection[2].Slot);
         Assert.AreEqual("3 hours 04 mins", unitCollection[2].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(3, 4, 0), unitCollection[2].EtaTimeSpan);
         Assert.AreEqual(0, unitCollection[2].Ppd);
         Assert.AreEqual("1 mins 55 secs", unitCollection[2].Tpf);
         Assert.AreEqual(new TimeSpan(0, 1, 55), unitCollection[2].TpfTimeSpan);
         Assert.AreEqual(0, unitCollection[2].BaseCredit);
         Assert.AreEqual(0, unitCollection[2].CreditEstimate);
      }

      [Test]
      public void FillTest5()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(2, unitCollection[0].Id);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(11294, unitCollection[0].Project);
         Assert.AreEqual(7, unitCollection[0].Run);
         Assert.AreEqual(243, unitCollection[0].Clone);
         Assert.AreEqual(66, unitCollection[0].Gen);
         Assert.AreEqual("0x16", unitCollection[0].Core);
         Assert.AreEqual("0x000000440a3b1e5c4d9a1d0b2f409204", unitCollection[0].UnitId);
         Assert.AreEqual("98.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(50000, unitCollection[0].TotalFrames);
         Assert.AreEqual(49000, unitCollection[0].FramesDone);
         Assert.AreEqual("08/Aug/2011-16:21:25", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 8, 16, 21, 25), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("14/Aug/2011-16:21:25", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 14, 16, 21, 25), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("18/Aug/2011-16:21:25", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 18, 16, 21, 25), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("171.64.65.56", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 56 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.26", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 26 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(1, unitCollection[0].Slot);
         Assert.AreEqual("6 mins 19 secs", unitCollection[0].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 6, 19), unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(6874.98, unitCollection[0].Ppd);
         Assert.AreEqual("3 mins 50 secs", unitCollection[0].Tpf);
         Assert.AreEqual(new TimeSpan(0, 3, 50), unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(1835, unitCollection[0].BaseCredit);
         Assert.AreEqual(1835, unitCollection[0].CreditEstimate);

         Assert.AreEqual(1, unitCollection[1].Id);
         Assert.AreEqual("RUNNING", unitCollection[1].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[1].StateEnum);
         Assert.AreEqual(7611, unitCollection[1].Project);
         Assert.AreEqual(0, unitCollection[1].Run);
         Assert.AreEqual(34, unitCollection[1].Clone);
         Assert.AreEqual(21, unitCollection[1].Gen);
         Assert.AreEqual("0xa4", unitCollection[1].Core);
         Assert.AreEqual("0x00000015664f2dd04df0f4a23f4a5e77", unitCollection[1].UnitId);
         Assert.AreEqual("26.75%", unitCollection[1].PercentDone);
         Assert.AreEqual(2000, unitCollection[1].TotalFrames);
         Assert.AreEqual(535, unitCollection[1].FramesDone);
         Assert.AreEqual("08/Aug/2011-12:32:11", unitCollection[1].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 8, 12, 32, 11), unitCollection[1].AssignedDateTime);
         Assert.AreEqual("20/Aug/2011-05:34:35", unitCollection[1].Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 20, 5, 34, 35), unitCollection[1].TimeoutDateTime);
         Assert.AreEqual("28/Aug/2011-01:00:59", unitCollection[1].Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 28, 1, 0, 59), unitCollection[1].DeadlineDateTime);
         Assert.AreEqual("171.64.65.104", unitCollection[1].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 104 }), unitCollection[1].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.49", unitCollection[1].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 49 }), unitCollection[1].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[1].WaitingOn);
         Assert.AreEqual(0, unitCollection[1].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[1].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[1].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[1].Slot);
         Assert.AreEqual("1.09 days", unitCollection[1].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(1, 2, 9, 36), unitCollection[1].EtaTimeSpan);
         Assert.AreEqual(527.69, unitCollection[1].Ppd);
         Assert.AreEqual("21 mins 30 secs", unitCollection[1].Tpf);
         Assert.AreEqual(new TimeSpan(0, 21, 30), unitCollection[1].TpfTimeSpan);
         Assert.AreEqual(788, unitCollection[1].BaseCredit);
         Assert.AreEqual(788, unitCollection[1].CreditEstimate);
      }

      [Test]
      public void FillTest6()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_6\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(3, unitCollection[0].Id);
         Assert.AreEqual("SEND", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Send, unitCollection[0].StateEnum);
         Assert.AreEqual(6507, unitCollection[0].Project);
         Assert.AreEqual(19, unitCollection[0].Run);
         Assert.AreEqual(288, unitCollection[0].Clone);
         Assert.AreEqual(32, unitCollection[0].Gen);
         Assert.AreEqual("0x78", unitCollection[0].Core);
         Assert.AreEqual("0x6b4ddec54d9a40f3002001200013196b", unitCollection[0].UnitId);
         Assert.AreEqual("16.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(250, unitCollection[0].TotalFrames);
         Assert.AreEqual(40, unitCollection[0].FramesDone);
         Assert.AreEqual("04/Apr/2011-22:06:43", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 4, 4, 22, 6, 43), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("<invalid>", unitCollection[0].Timeout);
         Assert.AreEqual(null, unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("01/May/2011-22:06:43", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 5, 1, 22, 6, 43), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("171.64.65.62", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 62 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.25", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 25 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual("Send Results", unitCollection[0].WaitingOn);
         Assert.AreEqual(5, unitCollection[0].Attempts);
         Assert.AreEqual("4 mins 14 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(new TimeSpan(0, 4, 14), unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("15 hours 57 mins", unitCollection[0].Eta);
         Assert.AreEqual(new TimeSpan(15, 57, 0), unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Ppd);
         Assert.AreEqual("11 mins 23 secs", unitCollection[0].Tpf);
         Assert.AreEqual(new TimeSpan(0, 11, 23), unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(0, unitCollection[0].BaseCredit);
         Assert.AreEqual(0, unitCollection[0].CreditEstimate);

         Assert.AreEqual(2, unitCollection[1].Id);
         Assert.AreEqual("SEND", unitCollection[1].State);
         Assert.AreEqual(FahSlotStatus.Send, unitCollection[1].StateEnum);
         Assert.AreEqual(6513, unitCollection[1].Project);
         Assert.AreEqual(19, unitCollection[1].Run);
         Assert.AreEqual(316, unitCollection[1].Clone);
         Assert.AreEqual(26, unitCollection[1].Gen);
         Assert.AreEqual("0x78", unitCollection[1].Core);
         Assert.AreEqual("0x5d7cdbac4d9494e3001a013c00131971", unitCollection[1].UnitId);
         Assert.AreEqual("58.00%", unitCollection[1].PercentDone);
         Assert.AreEqual(250, unitCollection[1].TotalFrames);
         Assert.AreEqual(145, unitCollection[1].FramesDone);
         Assert.AreEqual("31/Mar/2011-14:51:15", unitCollection[1].Assigned);
         Assert.AreEqual(new DateTime(2011, 3, 31, 14, 51, 15), unitCollection[1].AssignedDateTime);
         Assert.AreEqual("<invalid>", unitCollection[1].Timeout);
         Assert.AreEqual(null, unitCollection[1].TimeoutDateTime);
         Assert.AreEqual("21/Apr/2011-14:51:15", unitCollection[1].Deadline);
         Assert.AreEqual(new DateTime(2011, 4, 21, 14, 51, 15), unitCollection[1].DeadlineDateTime);
         Assert.AreEqual("171.64.65.62", unitCollection[1].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 62 }), unitCollection[1].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.25", unitCollection[1].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 25 }), unitCollection[1].CollectionServerIPAddress);
         Assert.AreEqual("Send Results", unitCollection[1].WaitingOn);
         Assert.AreEqual(5, unitCollection[1].Attempts);
         Assert.AreEqual("4 mins 07 secs", unitCollection[1].NextAttempt);
         Assert.AreEqual(new TimeSpan(0, 4, 7), unitCollection[1].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[1].Slot);
         Assert.AreEqual("5 hours 23 mins", unitCollection[1].Eta);
         Assert.AreEqual(new TimeSpan(5, 23, 0), unitCollection[1].EtaTimeSpan);
         Assert.AreEqual(0, unitCollection[1].Ppd);
         Assert.AreEqual("7 mins 41 secs", unitCollection[1].Tpf);
         Assert.AreEqual(new TimeSpan(0, 7, 41), unitCollection[1].TpfTimeSpan);
         Assert.AreEqual(0, unitCollection[1].BaseCredit);
         Assert.AreEqual(0, unitCollection[1].CreditEstimate);

         Assert.AreEqual(1, unitCollection[2].Id);
         Assert.AreEqual("SEND", unitCollection[2].State);
         Assert.AreEqual(FahSlotStatus.Send, unitCollection[2].StateEnum);
         Assert.AreEqual(6522, unitCollection[2].Project);
         Assert.AreEqual(7, unitCollection[2].Run);
         Assert.AreEqual(90, unitCollection[2].Clone);
         Assert.AreEqual(43, unitCollection[2].Gen);
         Assert.AreEqual("0x78", unitCollection[2].Core);
         Assert.AreEqual("0x59f9f94c4d93264f002b005a0007197a", unitCollection[2].UnitId);
         Assert.AreEqual("82.80%", unitCollection[2].PercentDone);
         Assert.AreEqual(250, unitCollection[2].TotalFrames);
         Assert.AreEqual(207, unitCollection[2].FramesDone);
         Assert.AreEqual("30/Mar/2011-12:47:11", unitCollection[2].Assigned);
         Assert.AreEqual(new DateTime(2011, 3, 30, 12, 47, 11), unitCollection[2].AssignedDateTime);
         Assert.AreEqual("<invalid>", unitCollection[2].Timeout);
         Assert.AreEqual(null, unitCollection[2].TimeoutDateTime);
         Assert.AreEqual("18/Apr/2011-12:47:11", unitCollection[2].Deadline);
         Assert.AreEqual(new DateTime(2011, 4, 18, 12, 47, 11), unitCollection[2].DeadlineDateTime);
         Assert.AreEqual("171.64.65.62", unitCollection[2].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 62 }), unitCollection[2].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.25", unitCollection[2].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 25 }), unitCollection[2].CollectionServerIPAddress);
         Assert.AreEqual("Send Results", unitCollection[2].WaitingOn);
         Assert.AreEqual(5, unitCollection[2].Attempts);
         Assert.AreEqual("4 mins 10 secs", unitCollection[2].NextAttempt);
         Assert.AreEqual(new TimeSpan(0, 4, 10), unitCollection[2].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[2].Slot);
         Assert.AreEqual("1 hours 52 mins", unitCollection[2].Eta);
         Assert.AreEqual(new TimeSpan(1, 52, 0), unitCollection[2].EtaTimeSpan);
         Assert.AreEqual(0, unitCollection[2].Ppd);
         Assert.AreEqual("6 mins 31 secs", unitCollection[2].Tpf);
         Assert.AreEqual(new TimeSpan(0, 6, 31), unitCollection[2].TpfTimeSpan);
         Assert.AreEqual(0, unitCollection[2].BaseCredit);
         Assert.AreEqual(0, unitCollection[2].CreditEstimate);

         Assert.AreEqual(0, unitCollection[3].Id);
         Assert.AreEqual("SEND", unitCollection[3].State);
         Assert.AreEqual(FahSlotStatus.Send, unitCollection[3].StateEnum);
         Assert.AreEqual(6524, unitCollection[3].Project);
         Assert.AreEqual(14, unitCollection[3].Run);
         Assert.AreEqual(78, unitCollection[3].Clone);
         Assert.AreEqual(83, unitCollection[3].Gen);
         Assert.AreEqual("0x78", unitCollection[3].Core);
         Assert.AreEqual("0x213f1ddd4da208ef0053004e000e197c", unitCollection[3].UnitId);
         Assert.AreEqual("78.80%", unitCollection[3].PercentDone);
         Assert.AreEqual(250, unitCollection[3].TotalFrames);
         Assert.AreEqual(197, unitCollection[3].FramesDone);
         Assert.AreEqual("10/Apr/2011-19:45:51", unitCollection[3].Assigned);
         Assert.AreEqual(new DateTime(2011, 4, 10, 19, 45, 51), unitCollection[3].AssignedDateTime);
         Assert.AreEqual("<invalid>", unitCollection[3].Timeout);
         Assert.AreEqual(null, unitCollection[3].TimeoutDateTime);
         Assert.AreEqual("05/May/2011-19:45:51", unitCollection[3].Deadline);
         Assert.AreEqual(new DateTime(2011, 5, 5, 19, 45, 51), unitCollection[3].DeadlineDateTime);
         Assert.AreEqual("171.64.65.62", unitCollection[3].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 62 }), unitCollection[3].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.25", unitCollection[3].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 25 }), unitCollection[3].CollectionServerIPAddress);
         Assert.AreEqual("Send Results", unitCollection[3].WaitingOn);
         Assert.AreEqual(5, unitCollection[3].Attempts);
         Assert.AreEqual("3 mins 55 secs", unitCollection[3].NextAttempt);
         Assert.AreEqual(new TimeSpan(0, 3, 55), unitCollection[3].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[3].Slot);
         Assert.AreEqual("3 hours 01 mins", unitCollection[3].Eta);
         Assert.AreEqual(new TimeSpan(3, 1, 0), unitCollection[3].EtaTimeSpan);
         Assert.AreEqual(0, unitCollection[3].Ppd);
         Assert.AreEqual("8 mins 33 secs", unitCollection[3].Tpf);
         Assert.AreEqual(new TimeSpan(0, 8, 33), unitCollection[3].TpfTimeSpan);
         Assert.AreEqual(0, unitCollection[3].BaseCredit);
         Assert.AreEqual(0, unitCollection[3].CreditEstimate);

         Assert.AreEqual(4, unitCollection[4].Id);
         Assert.AreEqual("RUNNING", unitCollection[4].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[4].StateEnum);
         Assert.AreEqual(7600, unitCollection[4].Project);
         Assert.AreEqual(41, unitCollection[4].Run);
         Assert.AreEqual(65, unitCollection[4].Clone);
         Assert.AreEqual(3, unitCollection[4].Gen);
         Assert.AreEqual("0xa4", unitCollection[4].Core);
         Assert.AreEqual("0x00000008664f2dcd4dee8ab75177cc82", unitCollection[4].UnitId);
         Assert.AreEqual("78.60%", unitCollection[4].PercentDone);
         Assert.AreEqual(2000, unitCollection[4].TotalFrames);
         Assert.AreEqual(1572, unitCollection[4].FramesDone);
         Assert.AreEqual("09/Aug/2011-12:59:36", unitCollection[4].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 9, 12, 59, 36), unitCollection[4].AssignedDateTime);
         Assert.AreEqual("23/Aug/2011-11:04:24", unitCollection[4].Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 23, 11, 4, 24), unitCollection[4].TimeoutDateTime);
         Assert.AreEqual("01/Sep/2011-17:47:36", unitCollection[4].Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 1, 17, 47, 36), unitCollection[4].DeadlineDateTime);
         Assert.AreEqual("171.64.65.101", unitCollection[4].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 101 }), unitCollection[4].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.49", unitCollection[4].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 49 }), unitCollection[4].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[4].WaitingOn);
         Assert.AreEqual(0, unitCollection[4].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[4].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[4].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[4].Slot);
         Assert.AreEqual("1.12 days", unitCollection[4].Eta);
         Assert.AreEqual(new TimeSpan(1, 2, 52, 48), unitCollection[4].EtaTimeSpan);
         Assert.AreEqual(177.05, unitCollection[4].Ppd);
         Assert.AreEqual("1 hours 16 mins", unitCollection[4].Tpf);
         Assert.AreEqual(new TimeSpan(1, 16, 0), unitCollection[4].TpfTimeSpan);
         Assert.AreEqual(937, unitCollection[4].BaseCredit);
         Assert.AreEqual(937, unitCollection[4].CreditEstimate);
      }

      [Test]
      public void FillTest7()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_7\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(2, unitCollection[0].Id);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(11051, unitCollection[0].Project);
         Assert.AreEqual(0, unitCollection[0].Run);
         Assert.AreEqual(2, unitCollection[0].Clone);
         Assert.AreEqual(39, unitCollection[0].Gen);
         Assert.AreEqual("0xa3", unitCollection[0].Core);
         Assert.AreEqual("0x000000280a3b1e5b4db73f5943ba9ef4", unitCollection[0].UnitId);
         Assert.AreEqual("91.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(1000, unitCollection[0].TotalFrames);
         Assert.AreEqual(910, unitCollection[0].FramesDone);
         Assert.AreEqual("22/Aug/2011-18:29:03", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 22, 18, 29, 3), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("03/Sep/2011-18:29:03", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 9, 3, 18, 29, 3), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("15/Sep/2011-18:29:03", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 15, 18, 29, 3), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("171.64.65.55", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 55 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.26", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 26 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("42 mins 47 secs", unitCollection[0].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 42, 47), unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(1859.89, unitCollection[0].Ppd);
         Assert.AreEqual("4 mins 59 secs", unitCollection[0].Tpf);
         Assert.AreEqual(new TimeSpan(0, 4, 59), unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(645, unitCollection[0].BaseCredit);
         Assert.AreEqual(645, unitCollection[0].CreditEstimate);

         Assert.AreEqual(0, unitCollection[1].Id);
         Assert.AreEqual("RUNNING", unitCollection[1].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[1].StateEnum);
         Assert.AreEqual(6801, unitCollection[1].Project);
         Assert.AreEqual(6348, unitCollection[1].Run);
         Assert.AreEqual(0, unitCollection[1].Clone);
         Assert.AreEqual(305, unitCollection[1].Gen);
         Assert.AreEqual("0x15", unitCollection[1].Core);
         Assert.AreEqual("0x0000014d0a3b1e644d94b9700899a51b", unitCollection[1].UnitId);
         Assert.AreEqual("40.00%", unitCollection[1].PercentDone);
         Assert.AreEqual(50000, unitCollection[1].TotalFrames);
         Assert.AreEqual(20000, unitCollection[1].FramesDone);
         Assert.AreEqual("23/Aug/2011-00:37:16", unitCollection[1].Assigned);
         Assert.AreEqual(new DateTime(2011, 8, 23, 0, 37, 16), unitCollection[1].AssignedDateTime);
         Assert.AreEqual("28/Aug/2011-00:37:16", unitCollection[1].Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 28, 0, 37, 16), unitCollection[1].TimeoutDateTime);
         Assert.AreEqual("02/Sep/2011-00:37:16", unitCollection[1].Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 2, 0, 37, 16), unitCollection[1].DeadlineDateTime);
         Assert.AreEqual("171.64.65.64", unitCollection[1].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 64, 65, 64 }), unitCollection[1].WorkServerIPAddress);
         Assert.AreEqual("171.67.108.26", unitCollection[1].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 171, 67, 108, 26 }), unitCollection[1].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[1].WaitingOn);
         Assert.AreEqual(0, unitCollection[1].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[1].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[1].NextAttemptTimeSpan);
         Assert.AreEqual(1, unitCollection[1].Slot);
         Assert.AreEqual("2 hours 10 mins", unitCollection[1].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(2, 10, 0), unitCollection[1].EtaTimeSpan);
         Assert.AreEqual(8868.96, unitCollection[1].Ppd);
         Assert.AreEqual("2 mins 11 secs", unitCollection[1].Tpf);
         Assert.AreEqual(new TimeSpan(0, 2, 11), unitCollection[1].TpfTimeSpan);
         Assert.AreEqual(1348, unitCollection[1].BaseCredit);
         Assert.AreEqual(1348, unitCollection[1].CreditEstimate);
      }

      [Test]
      public void FillTest9()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_9\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(0, unitCollection[0].Id);
         Assert.AreEqual("RUNNING", unitCollection[0].State);
         Assert.AreEqual(FahSlotStatus.Running, unitCollection[0].StateEnum);
         Assert.AreEqual(7905, unitCollection[0].Project);
         Assert.AreEqual(47, unitCollection[0].Run);
         Assert.AreEqual(37, unitCollection[0].Clone);
         Assert.AreEqual(0, unitCollection[0].Gen);
         Assert.AreEqual("0xa4", unitCollection[0].Core);
         Assert.AreEqual("0x0000000000ac9c234ecff97c3e910a84", unitCollection[0].UnitId);
         Assert.AreEqual("0.00%", unitCollection[0].PercentDone);
         Assert.AreEqual(500, unitCollection[0].TotalFrames);
         Assert.AreEqual(0, unitCollection[0].FramesDone);
         Assert.AreEqual("2011-12-08T02:59:57", unitCollection[0].Assigned);
         Assert.AreEqual(new DateTime(2011, 12, 8, 2, 59, 57), unitCollection[0].AssignedDateTime);
         Assert.AreEqual("2011-12-17T02:59:57", unitCollection[0].Timeout);
         Assert.AreEqual(new DateTime(2011, 12, 17, 2, 59, 57), unitCollection[0].TimeoutDateTime);
         Assert.AreEqual("2011-12-27T02:59:57", unitCollection[0].Deadline);
         Assert.AreEqual(new DateTime(2011, 12, 27, 2, 59, 57), unitCollection[0].DeadlineDateTime);
         Assert.AreEqual("128.113.12.163", unitCollection[0].WorkServer);
         Assert.AreEqual(new IPAddress(new byte[] { 128, 113, 12, 163 }), unitCollection[0].WorkServerIPAddress);
         Assert.AreEqual("129.74.85.16", unitCollection[0].CollectionServer);
         Assert.AreEqual(new IPAddress(new byte[] { 129, 74, 85, 16 }), unitCollection[0].CollectionServerIPAddress);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].NextAttemptTimeSpan);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("0.00 secs", unitCollection[0].Eta);
         // not exactly the same value seen in SimulationInfo.EtaTimeSpan
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(0.0, unitCollection[0].Ppd);
         Assert.AreEqual("0.00 secs", unitCollection[0].Tpf);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(487, unitCollection[0].BaseCredit);
         Assert.AreEqual(487, unitCollection[0].CreditEstimate);
      }
   }

   public class UnitDerived : Unit
   {
      [MessageProperty("id")]
      public string IdString { get; set; }

      [MessageProperty("id")]
      public bool? IdBool { get; set; }
   }

   public class UnitNotDerived : ITypedMessageObject
   {
      #region ITypedMessageObject Members

      public System.Collections.Generic.IEnumerable<MessagePropertyConversionError> Errors
      {
         get { throw new NotImplementedException(); }
      }

      void ITypedMessageObject.AddError(MessagePropertyConversionError error)
      {
         throw new NotImplementedException();
      }

      #endregion
   }
}
