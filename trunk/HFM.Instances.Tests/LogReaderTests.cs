/*
 * HFM.NET - Log Reader Class Tests
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
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using HFM.Proteins;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class LogReaderTests
   {
      private LogReader reader;
   
      [SetUp]
      public void Init()
      {
         reader = new LogReader();
      }
      
      [Test, Category("SMP")]
      public void SMPTestLog1()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "SMP Test FAHlog 1";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 1.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 30);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 150);
         Assert.AreEqual(reader.ClientRunList[1].ClientStartPosition, 274);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[0], 302);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[1], 402);
         
         // Check Parsed Data for First Client Start and Work Unit
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), "-smp -verbosity 9");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), "5131EA752EB60547");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 1);
         Assert.AreEqual(reader.ClientLogLines[40].LineData, "2.08");
         Assert.AreEqual(reader.ClientLogLines[51].LineData, "2677 (Run 10, Clone 29, Gen 28)");
         Assert.AreEqual(reader.ClientLogLines[109].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("GPU")]
      public void GPUTestLog1()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "GPU Test FAHlog 1";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 1.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 130);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 326);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[2], 387);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[3], 449);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[4], 510);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[5], 571);

         Assert.AreEqual(reader.ClientRunList[1].ClientStartPosition, 618);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[0], 663);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[1], 737);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[2], 935);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[3], 1132);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[4], 1329);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[5], 1526);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[6], 1724);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[7], 1926);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[8], 2123);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[9], 2321);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[10], 2518);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[11], 2715);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[12], 2917);

         // Check Parsed Data for First Client Start and Work Unit
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), "-verbosity 9 -local");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), "CF185086C102A47");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 2);
         Assert.AreEqual(reader.ClientLogLines[140].LineData, "1.19");
         Assert.AreEqual(reader.ClientLogLines[154].LineData, "5771 (Run 12, Clone 109, Gen 805)");
         Assert.AreEqual(reader.ClientLogLines[286].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("GPU")]
      public void GPUTestLog2()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "GPU Test FAHlog 2";

         // Read, Scan, Check Number of Unit Start Positions *ONLY*
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 2.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 3);

         // Check Parsed Data for First Client Start and Work Unit
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), String.Empty);
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), "51108B97183EA3DF");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 2);
         Assert.AreEqual(reader.ClientLogLines[42].LineData, "1.19");
         Assert.AreEqual(reader.ClientLogLines[56].LineData, "5751 (Run 8, Clone 205, Gen 527)");
         Assert.AreEqual(reader.ClientLogLines[188].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }
   }
}
