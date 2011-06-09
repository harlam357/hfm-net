/*
 * HFM.NET - Queue Data Class Tests
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

namespace HFM.Client.Tests
{
   [TestFixture]
   public class QueueTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt");
         var queue = Queue.Parse(Messages.GetNextJsonMessage(ref message));
         Assert.AreEqual(0, queue[0].ID);
         Assert.AreEqual("RUNNING", queue[0].State);
         Assert.AreEqual(11020, queue[0].Project);
         Assert.AreEqual(0, queue[0].Run);
         Assert.AreEqual(1921, queue[0].Clone);
         Assert.AreEqual(24, queue[0].Gen);
         Assert.AreEqual("0xa3", queue[0].Core);
         Assert.AreEqual("0x000000210a3b1e5b4d824701aee79f1e", queue[0].Unit);
         Assert.AreEqual("59.00%", queue[0].PercentDone);
         Assert.AreEqual(1000, queue[0].TotalFrames);
         Assert.AreEqual(590, queue[0].FramesDone);
         Assert.AreEqual("27/May/2011-19:34:24", queue[0].Assigned);
         Assert.AreEqual("04/Jun/2011-19:34:24", queue[0].Timeout);
         Assert.AreEqual("08/Jun/2011-19:34:24", queue[0].Deadline);
         Assert.AreEqual("171.64.65.55", queue[0].WorkServer);
         Assert.AreEqual("171.67.108.26", queue[0].CollectionServer);
         Assert.AreEqual(String.Empty, queue[0].WaitingOn);
         Assert.AreEqual(0, queue[0].Attempts);
         Assert.AreEqual("0.00 secs", queue[0].NextAttempt);
         Assert.AreEqual(0, queue[0].Slot);
         Assert.AreEqual("2 hours 28 mins", queue[0].ETA);
         Assert.AreEqual(1749.96, queue[0].PPD);
         Assert.AreEqual("3 mins 38 secs", queue[0].TPF);
         Assert.AreEqual(443, queue[0].BaseCredit);
         Assert.AreEqual(443, queue[0].CreditEstimate);
      }
   }
}
