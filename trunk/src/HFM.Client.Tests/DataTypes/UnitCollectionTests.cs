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
