/*
 * HFM.NET - Simulation Info Data Class Tests
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

using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Client.DataTypes;

namespace HFM.Client.Tests.DataTypes
{
   [TestFixture]
   public class SimulationInfoTests
   {
      // ReSharper disable InconsistentNaming

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
         Assert.AreEqual(new TimeSpan(3, 37, 8), simulationInfo.RunTimeTimeSpan);
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
         Assert.AreEqual(new TimeSpan(0, 3, 30), simulationInfo.RunTimeTimeSpan);
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
         Assert.AreEqual(new TimeSpan(0, 7, 16), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(12730, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(3, 32, 10), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest4_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\simulation-info1.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("ChasR", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(7507, simulationInfo.Project);
         Assert.AreEqual(0, simulationInfo.Run);
         Assert.AreEqual(34, simulationInfo.Clone);
         Assert.AreEqual(1, simulationInfo.Gen);
         Assert.AreEqual(163, simulationInfo.CoreType);
         Assert.AreEqual("GROGBSMP", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(500, simulationInfo.TotalIterations);
         Assert.AreEqual(180, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("17/Aug/2011-15:14:58", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 17, 15, 14, 58), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1313931058, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 21, 12, 50, 58), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1314155698, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 24, 3, 14, 58), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(18964, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(5, 16, 4), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(33249, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(9, 14, 9), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest4_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\simulation-info2.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("ChasR", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(5788, simulationInfo.Project);
         Assert.AreEqual(9, simulationInfo.Run);
         Assert.AreEqual(838, simulationInfo.Clone);
         Assert.AreEqual(9, simulationInfo.Gen);
         Assert.AreEqual(17, simulationInfo.CoreType);
         Assert.AreEqual("GROGPU2", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(20000, simulationInfo.TotalIterations);
         Assert.AreEqual(19200, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("17/Aug/2011-18:18:47", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 17, 18, 18, 47), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(0, simulationInfo.Timeout);
         Assert.AreEqual(null, simulationInfo.TimeoutDateTime);
         Assert.AreEqual(0, simulationInfo.Deadline);
         Assert.AreEqual(null, simulationInfo.DeadlineDateTime);
         Assert.AreEqual(7856, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(2, 10, 56), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(262, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 4, 22), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
         // Errors
         Assert.AreEqual(2, simulationInfo.Errors.Count());
         Assert.AreEqual("Deadline", simulationInfo.Errors.ElementAt(0).PropertyName);
         Assert.AreEqual("DeadlineDateTime", simulationInfo.Errors.ElementAt(1).PropertyName);
      }

      [Test]
      public void FillTest4_3()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_4\\simulation-info3.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("ChasR", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(5796, simulationInfo.Project);
         Assert.AreEqual(19, simulationInfo.Run);
         Assert.AreEqual(79, simulationInfo.Clone);
         Assert.AreEqual(5, simulationInfo.Gen);
         Assert.AreEqual(17, simulationInfo.CoreType);
         Assert.AreEqual("GROGPU2", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(20000, simulationInfo.TotalIterations);
         Assert.AreEqual(800, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("17/Aug/2011-20:29:43", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 17, 20, 29, 43), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(0, simulationInfo.Timeout);
         Assert.AreEqual(null, simulationInfo.TimeoutDateTime);
         Assert.AreEqual(0, simulationInfo.Deadline);
         Assert.AreEqual(null, simulationInfo.DeadlineDateTime);
         Assert.AreEqual(496, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(0, 8, 16), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(11079, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(3, 4, 39), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
         // Errors
         Assert.AreEqual(2, simulationInfo.Errors.Count());
         Assert.AreEqual("Deadline", simulationInfo.Errors.ElementAt(0).PropertyName);
         Assert.AreEqual("DeadlineDateTime", simulationInfo.Errors.ElementAt(1).PropertyName);
      }

      [Test]
      public void FillTest5_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\simulation-info1.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("The_Penguin", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(7611, simulationInfo.Project);
         Assert.AreEqual(0, simulationInfo.Run);
         Assert.AreEqual(34, simulationInfo.Clone);
         Assert.AreEqual(21, simulationInfo.Gen);
         Assert.AreEqual(164, simulationInfo.CoreType);
         Assert.AreEqual("GROGBA4", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(2000, simulationInfo.TotalIterations);
         Assert.AreEqual(535, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("08/Aug/2011-12:32:11", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 8, 12, 32, 11), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1313818475, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 20, 5, 34, 35), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1314493259, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 28, 1, 0, 59), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(34641, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(9, 37, 21), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(94379, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(1, 2, 12, 59), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest5_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_5\\simulation-info2.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("The_Penguin", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(11294, simulationInfo.Project);
         Assert.AreEqual(7, simulationInfo.Run);
         Assert.AreEqual(243, simulationInfo.Clone);
         Assert.AreEqual(66, simulationInfo.Gen);
         Assert.AreEqual(22, simulationInfo.CoreType);
         Assert.AreEqual("OPENMM_OPENCL", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(50000, simulationInfo.TotalIterations);
         Assert.AreEqual(49000, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("08/Aug/2011-16:21:25", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 8, 16, 21, 25), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1313338885, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 14, 16, 21, 25), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1313684485, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 8, 18, 16, 21, 25), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(22732, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(6, 18, 52), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(329, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 5, 29), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest6()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_6\\simulation-info1.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("Bayna", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(7600, simulationInfo.Project);
         Assert.AreEqual(41, simulationInfo.Run);
         Assert.AreEqual(65, simulationInfo.Clone);
         Assert.AreEqual(3, simulationInfo.Gen);
         Assert.AreEqual(164, simulationInfo.CoreType);
         Assert.AreEqual("GROGBA4", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(2000, simulationInfo.TotalIterations);
         Assert.AreEqual(1572, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("09/Aug/2011-12:59:36", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 9, 12, 59, 36), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1314097464, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 23, 11, 4, 24), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1314899256, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 1, 17, 47, 36), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(360111, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(4, 4, 1, 51), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(97134, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(1, 2, 58, 54), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest7_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_7\\simulation-info1.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("HayesK", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(11051, simulationInfo.Project);
         Assert.AreEqual(0, simulationInfo.Run);
         Assert.AreEqual(2, simulationInfo.Clone);
         Assert.AreEqual(39, simulationInfo.Gen);
         Assert.AreEqual(163, simulationInfo.CoreType);
         Assert.AreEqual("GROGBSMP", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(1000, simulationInfo.TotalIterations);
         Assert.AreEqual(910, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("22/Aug/2011-18:29:03", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 22, 18, 29, 3), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1315074543, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 9, 3, 18, 29, 3), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1316111343, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 15, 18, 29, 3), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(27396, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(7, 36, 36), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(2567, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 42, 47), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest7_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_7\\simulation-info2.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("HayesK", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(6801, simulationInfo.Project);
         Assert.AreEqual(6348, simulationInfo.Run);
         Assert.AreEqual(0, simulationInfo.Clone);
         Assert.AreEqual(305, simulationInfo.Gen);
         Assert.AreEqual(21, simulationInfo.CoreType);
         Assert.AreEqual("OPENMMGPU", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(50000, simulationInfo.TotalIterations);
         Assert.AreEqual(20000, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("23/Aug/2011-00:37:16", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 8, 23, 0, 37, 16), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1314491836, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 8, 28, 0, 37, 16), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1314923836, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 9, 2, 0, 37, 16), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(5325, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(1, 28, 45), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(7807, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(2, 10, 7), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest9()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_9\\simulation-info1.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("harlam357", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(7905, simulationInfo.Project);
         Assert.AreEqual(47, simulationInfo.Run);
         Assert.AreEqual(37, simulationInfo.Clone);
         Assert.AreEqual(0, simulationInfo.Gen);
         Assert.AreEqual(164, simulationInfo.CoreType);
         Assert.AreEqual("GROGBA4", simulationInfo.Core);
         Assert.AreEqual(String.Empty, simulationInfo.Description);
         Assert.AreEqual(500, simulationInfo.TotalIterations);
         Assert.AreEqual(0, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("2011-12-08T02:59:57", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2011, 12, 8, 2, 59, 57), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1324090797, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2011, 12, 17, 2, 59, 57), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1324954797, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2011, 12, 27, 2, 59, 57), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(15, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(0, 0, 15), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(0, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(TimeSpan.Zero, simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest10_1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\simulation-info1.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("harlam357", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(7610, simulationInfo.Project);
         Assert.AreEqual(630, simulationInfo.Run);
         Assert.AreEqual(0, simulationInfo.Clone);
         Assert.AreEqual(59, simulationInfo.Gen);
         Assert.AreEqual(164, simulationInfo.CoreType);
         Assert.AreEqual("GROGBA4", simulationInfo.Core);
         Assert.AreEqual("This project involves additional sampling of the FiP35 WW domain shot from the ultra-long trajectories run by DE Shaw on their new supercomputer ANTON. We are testing the differences between these new ultra-long trajectories and shorter ones from FAH, to test how simulations run on FAH stack up to more traditional methods.\r", simulationInfo.Description);
         Assert.AreEqual(2000, simulationInfo.TotalIterations);
         Assert.AreEqual(660, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("2012-01-10T23:20:27", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1327249371, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1327924155, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2012, 1, 30, 11, 49, 15), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(4836, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(1, 20, 36), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(8682, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(2, 24, 42), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest10_2()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\simulation-info2.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("harlam357", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(5772, simulationInfo.Project);
         Assert.AreEqual(7, simulationInfo.Run);
         Assert.AreEqual(364, simulationInfo.Clone);
         Assert.AreEqual(252, simulationInfo.Gen);
         Assert.AreEqual(17, simulationInfo.CoreType);
         Assert.AreEqual("GROGPU2", simulationInfo.Core);
         Assert.AreEqual(1196, simulationInfo.Description.Length);
         Assert.AreEqual(15000, simulationInfo.TotalIterations);
         Assert.AreEqual(7650, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("2012-01-11T04:21:14", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2012, 1, 11, 4, 21, 14), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(0, simulationInfo.Timeout);
         Assert.AreEqual(null, simulationInfo.TimeoutDateTime);
         Assert.AreEqual(0, simulationInfo.Deadline);
         Assert.AreEqual(null, simulationInfo.DeadlineDateTime);
         Assert.AreEqual(1725, simulationInfo.RunTime);
         Assert.AreEqual(new TimeSpan(0, 28, 45), simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(1655, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 27, 35), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      [Test]
      public void FillTest11()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\simulation-info1.txt");
         var simulationInfo = new SimulationInfo();
         simulationInfo.Fill(MessageCache.GetNextJsonMessage(ref message));
         Assert.AreEqual("harlam357", simulationInfo.User);
         Assert.AreEqual(32, simulationInfo.Team);
         Assert.AreEqual(7610, simulationInfo.Project);
         Assert.AreEqual(192, simulationInfo.Run);
         Assert.AreEqual(0, simulationInfo.Clone);
         Assert.AreEqual(58, simulationInfo.Gen);
         Assert.AreEqual(164, simulationInfo.CoreType);
         Assert.AreEqual("GROGBA4", simulationInfo.Core);
         Assert.AreEqual("This project involves additional sampling of the FiP35 WW domain shot from the ultra-long trajectories run by DE Shaw on their new supercomputer ANTON. We are testing the differences between these new ultra-long trajectories and shorter ones from FAH, to test how simulations run on FAH stack up to more traditional methods.\r", simulationInfo.Description);
         Assert.AreEqual(2000, simulationInfo.TotalIterations);
         Assert.AreEqual(1900, simulationInfo.IterationsDone);
         Assert.AreEqual(0, simulationInfo.Energy);
         Assert.AreEqual(0, simulationInfo.Temperature);
         Assert.AreEqual("2012-02-17T21:48:22Z", simulationInfo.StartTime);
         Assert.AreEqual(new DateTime(2012, 2, 17, 21, 48, 22), simulationInfo.StartTimeDateTime);
         Assert.AreEqual(1330527046, simulationInfo.Timeout);
         Assert.AreEqual(new DateTime(2012, 2, 29, 14, 50, 46), simulationInfo.TimeoutDateTime);
         Assert.AreEqual(1331201830, simulationInfo.Deadline);
         Assert.AreEqual(new DateTime(2012, 3, 8, 10, 17, 10), simulationInfo.DeadlineDateTime);
         Assert.AreEqual(0, simulationInfo.RunTime);
         Assert.AreEqual(null, simulationInfo.RunTimeTimeSpan);
         Assert.AreEqual(0, simulationInfo.SimulationTime);
         Assert.AreEqual(1379, simulationInfo.Eta);
         // not exactly the same value seen in Unit.EtaTimeSpan
         Assert.AreEqual(new TimeSpan(0, 22, 59), simulationInfo.EtaTimeSpan);
         Assert.AreEqual(String.Empty, simulationInfo.News);
      }

      // ReSharper restore InconsistentNaming
   }
}
