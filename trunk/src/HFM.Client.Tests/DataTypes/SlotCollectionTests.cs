/*
 * HFM.NET - Slot Collection Data Class Tests
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
   public class SlotCollectionTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\slots.txt");
         var slotCollection = SlotCollection.Parse(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual(1, slotCollection.Count);
         Assert.AreEqual(0, slotCollection[0].Id);
         Assert.AreEqual("RUNNING", slotCollection[0].Status);
         Assert.AreEqual("smp:4", slotCollection[0].Description);
         Assert.AreEqual(true, slotCollection[0].SlotOptions.PauseOnStart);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ParseNullArgumentTest()
      {
         SlotCollection.Parse(null);
      }
   }
}
