/*
 * HFM.NET - Simulation Info Data Class Tests
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
   public class SimulationInfoTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\simulation-info.txt");
         var simulationInfo = SimulationInfo.Parse(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("harlam357", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(11020, simulationInfo.Project);
         Assert.AreEqual(0, simulationInfo.Run);
         Assert.AreEqual(1921, simulationInfo.Clone);
         Assert.AreEqual(24, simulationInfo.Gen);
         Assert.AreEqual(163, simulationInfo.CoreType);
         Assert.AreEqual("GROGBSMP", simulationInfo.Core);
         Assert.AreEqual("", simulationInfo.Description);
         Assert.AreEqual(1000, simulationInfo.TotalIterations);
         Assert.AreEqual(590, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("27/May/2011-19:34:24", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 5, 27, 19, 34, 24), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1307216064, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 6, 4, 19, 34, 24), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1307561664, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 6, 8, 19, 34, 24), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(13028, simulationInfo.RunTime);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(8844, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(2, 27, 24), simulationInfo.EtaTimeSpan);
         Assert.AreEqual("", simulationInfo.News);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void ParseNullArgumentTest()
      {
         SimulationInfo.Parse(null);
      }
   }
}
