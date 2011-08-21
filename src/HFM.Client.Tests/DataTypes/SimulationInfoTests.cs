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
      public void FillTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\simulation-info.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("harlam357", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(11020, simulationInfo.Project);
         Assert.AreEqual(0, simulationInfo.Run);
         Assert.AreEqual(1921, simulationInfo.Clone);
         Assert.AreEqual(24, simulationInfo.Gen);
         Assert.AreEqual(163, simulationInfo.CoreType);
         Assert.AreEqual("GROGBSMP", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
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
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_2\\simulation-info.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("Shelnutt2", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(10083, simulationInfo.Project);
         Assert.AreEqual(0, simulationInfo.Run);
         Assert.AreEqual(17, simulationInfo.Clone);
         Assert.AreEqual(24, simulationInfo.Gen);
         Assert.AreEqual(164, simulationInfo.CoreType);
         Assert.AreEqual("GROGBA4", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(10000, simulationInfo.TotalIterations);
         Assert.AreEqual(0, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("09/Aug/2011-02:54:54", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 9, 2, 54, 54), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1313549694, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 17, 2, 54, 54), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1313808894, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 20, 2, 54, 54), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(210, simulationInfo.RunTime);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(0, simulationInfo.Eta);
         Assert.AreEqual(TimeSpan.Zero, simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_3\\simulation-info.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("Adak", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(7018, simulationInfo.Project);
         Assert.AreEqual(2, simulationInfo.Run);
         Assert.AreEqual(76, simulationInfo.Clone);
         Assert.AreEqual(18, simulationInfo.Gen);
         Assert.AreEqual(164, simulationInfo.CoreType);
         Assert.AreEqual("GROGBA4", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(10000, simulationInfo.TotalIterations);
         Assert.AreEqual(300, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("09/Aug/2011-05:40:17", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 9, 5, 40, 17), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1313559617, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 17, 5, 40, 17), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1313818817, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 20, 5, 40, 17), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(436, simulationInfo.RunTime);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(12730, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(3, 32, 10), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }
   }
}
