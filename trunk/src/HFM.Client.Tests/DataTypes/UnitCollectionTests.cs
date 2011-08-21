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
         Assert.AreEqual("129.74.85.16", unitCollection[0].CollectionServer);
         Assert.AreEqual(String.Empty, unitCollection[0].WaitingOn);
         Assert.AreEqual(0, unitCollection[0].Attempts);
         Assert.AreEqual("0.00 secs", unitCollection[0].NextAttempt);
         Assert.AreEqual(0, unitCollection[0].Slot);
         Assert.AreEqual("0.00 secs", unitCollection[0].Eta);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].EtaTimeSpan);
         Assert.AreEqual(0.0, unitCollection[0].Ppd);
         Assert.AreEqual("0.00 secs", unitCollection[0].Tpf);
         Assert.AreEqual(TimeSpan.Zero, unitCollection[0].TpfTimeSpan);
         Assert.AreEqual(600, unitCollection[0].BaseCredit);
         Assert.AreEqual(600, unitCollection[0].CreditEstimate);
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
