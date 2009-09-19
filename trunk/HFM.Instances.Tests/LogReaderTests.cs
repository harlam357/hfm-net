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
      public void SMPTestLog1() // verbosity 9
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "SMP Test FAHlog 1";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 1.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 2);
         
         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 30);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 150);
         
         Assert.AreEqual(reader.ClientRunList[1].ClientStartPosition, 274);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions.Count, 2);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[0], 302);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[1], 402);
         
         // Check Parsed Data (Run Index 1)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[1]), "-smp -verbosity 9");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[1]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[1]), "5131EA752EB60547");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[1]), 1);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfCompletedUnits, 2);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfFailedUnits, 0);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfTotalUnitsCompleted, 263);
         
         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[40].LineData, "2.08");
         Assert.AreEqual(reader.ClientLogLines[51].LineData, "2677 (Run 10, Clone 29, Gen 28)");
         Assert.AreEqual(reader.ClientLogLines[109].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("SMP")]
      public void SMPTestLog2() // verbosity 9
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "SMP Test FAHlog 2";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 2.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 30);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 221);

         // Check Parsed Data (Run Index 0)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), "-smp -verbosity 9");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), "3A49EBB303C19834");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 1);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfCompletedUnits, 2);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfFailedUnits, 0);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfTotalUnitsCompleted, 292);

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[40].LineData, "2.08");
         Assert.AreEqual(reader.ClientLogLines[47].LineData, "2677 (Run 10, Clone 49, Gen 38)");
         Assert.AreEqual(reader.ClientLogLines[180].LineData, WorkUnitResult.FinishedUnit);

         // Special Check to be sure the reader is catching the Attempting To Send line
         Assert.AreEqual(reader.ClientLogLines[379].LineType, LogLineType.ClientSendStart);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("SMP")]
      public void SMPTestLog3() // verbosity (normal) / Handles Core Download on Startup / notfred's instance
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "SMP Test FAHlog 3";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 3.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 231);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 385);

         // Check Parsed Data (Run Index 0)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), "-local -forceasm -smp 4");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), String.Empty); // does not log UserID after requesting
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 1);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfCompletedUnits, 1);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfFailedUnits, 0);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfTotalUnitsCompleted, 0); //TODO: not capturing line "+ Starting local stats count at 1"

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[239].LineData, "2.08");
         Assert.AreEqual(reader.ClientLogLines[246].LineData, "2677 (Run 4, Clone 60, Gen 40)");
         Assert.AreEqual(reader.ClientLogLines[368].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("GPU")]
      public void GPUTestLog1() // verbosity 9
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "GPU Test FAHlog 1";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 1.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 2);
         
         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 6);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 130);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 326);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[2], 387);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[3], 449);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[4], 510);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[5], 571);

         Assert.AreEqual(reader.ClientRunList[1].ClientStartPosition, 618);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions.Count, 13);
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

         // Check Parsed Data (Run Index 1)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[1]), "-verbosity 9 -local");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[1]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[1]), "CF185086C102A47");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[1]), 2);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfCompletedUnits, 11);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfFailedUnits, 1);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfTotalUnitsCompleted, 12);

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
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
      public void GPUTestLog2() // verbosity (normal)
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "GPU Test FAHlog 2";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 2.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 3);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 34);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 208);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[2], 382);

         // Check Parsed Data (Run Index 0)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), String.Empty);
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), "51108B97183EA3DF");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 2);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfCompletedUnits, 2);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfFailedUnits, 0);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfTotalUnitsCompleted, 4221);

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[42].LineData, "1.19");
         Assert.AreEqual(reader.ClientLogLines[56].LineData, "5751 (Run 8, Clone 205, Gen 527)");
         Assert.AreEqual(reader.ClientLogLines[188].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("GPU")]
      public void GPUTestLog3EUEPause() // verbosity (normal)
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "GPU Test FAHlog 3 EUE Pause";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 3 EUE Pause.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 2);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 0);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 1);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 24);

         Assert.AreEqual(reader.ClientRunList[1].ClientStartPosition, 56);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions.Count, 6);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[0], 80);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[1], 221);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[2], 271);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[3], 320);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[4], 373);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[5], 421);

         // Check Parsed Data (Run Index 1)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[1]), String.Empty);
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[1]);
         Assert.AreEqual(UserTeam[0], "JollySwagman");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[1]), "1D1493BB0A79C9AE");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[1]), 2);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfCompletedUnits, 1);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfFailedUnits, 5);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfTotalUnitsCompleted, 224);

         // Check Work Unit Data (Run Index 1 - Unit Index 3)
         Assert.AreEqual(reader.ClientLogLines[328].LineData, "1.19");
         Assert.AreEqual(reader.ClientLogLines[342].LineData, "5756 (Run 6, Clone 32, Gen 480)");
         Assert.AreEqual(reader.ClientLogLines[359].LineData, WorkUnitResult.UnstableMachine);
         
         // Special Check to be sure the reader is catching the EUE Pause line
         Assert.AreEqual(reader.ClientLogLines[463].LineType, LogLineType.ClientEuePauseState);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }
      
      [Test, Category("SMP")]
      public void SMPTestLogINTERRUPTEDBadWUs() // verbosity 9
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "SMP Test FAHlog INTERRUPTED Bad WUs";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\SMP Test INTERRUPTED Bad WUs FAHlog.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 8);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 30);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 171);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[2], 221);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[3], 274);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[4], 338);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[5], 388);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[6], 434);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[7], 498);

         // Check Parsed Data (Run Index 0)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), "-smp -verbosity 9");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), "75B5B5EE3198996B");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 1);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfCompletedUnits, 1);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfFailedUnits, 2);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfTotalUnitsCompleted, 255);

         // Check Work Unit Data (Run Index 0 - Unit Index 6)
         Assert.AreEqual(reader.ClientLogLines[444].LineData, "2.10");
         Assert.AreEqual(reader.ClientLogLines[451].LineData, "2677 (Run 34, Clone 40, Gen 30)");

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("SMP")]
      public void SMPTestLogINTERRUPTEDBadWUs2() // verbosity 9
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "SMP Test FAHlog INTERRUPTED Bad WUs 2";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\SMP Test INTERRUPTED Bad WUs FAHlog 2.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions.Count, 10);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[0], 52);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[1], 243);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[2], 434);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[3], 484);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[4], 537);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[5], 600);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[6], 651);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[7], 702);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[8], 766);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartPositions[9], 1267);

         // Check Parsed Data (Run Index 0)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[0]), "-smp -verbosity 9");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[0]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[0]), "B26C4CA3732C261");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[0]), 1);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfCompletedUnits, 3);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfFailedUnits, 1);
         Assert.AreEqual(reader.ClientRunList[0].NumberOfTotalUnitsCompleted, 319);

         // Check Work Unit Data (Run Index 0 - Unit Index 3)
         Assert.AreEqual(reader.ClientLogLines[494].LineData, "2.10");
         Assert.AreEqual(reader.ClientLogLines[505].LineData, "2677 (Run 0, Clone 74, Gen 38)");
         Assert.AreEqual(reader.ClientLogLines[510].LineData, WorkUnitResult.Interrupted);

         // Check Work Unit Data (Run Index 0 - Unit Index 8)
         Assert.AreEqual(reader.ClientLogLines[776].LineData, "2.10");
         Assert.AreEqual(reader.ClientLogLines[787].LineData, "2677 (Run 15, Clone 90, Gen 48)");
         Assert.AreEqual(reader.ClientLogLines[917].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNotNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }

      [Test, Category("Standard")]
      public void StandardTestLog1() // verbosity 9
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "Standard Test FAHlog 1";

         // Read, Scan, Check Positions
         reader.ReadLogText(Instance, "..\\..\\TestFiles\\Standard Test FAHlog 1.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.ClientRunList.Count, 3);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.ClientRunList[1].ClientStartPosition, 30);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions.Count, 2);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[0], 179);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartPositions[1], 593);
         Assert.AreEqual(reader.ClientRunList[2].ClientStartPosition, 839);
         Assert.AreEqual(reader.ClientRunList[2].UnitStartPositions.Count, 1);
         Assert.AreEqual(reader.ClientRunList[2].UnitStartPositions[0], 874);

         // Check Parsed Data (Run Index 1)
         Assert.AreEqual(reader.ClientLogLines.GetRunArguments(reader.ClientRunList[1]), "-verbosity 9 -forceasm");
         ArrayList UserTeam = (ArrayList)reader.ClientLogLines.GetRunUserNameAndTeam(reader.ClientRunList[1]);
         Assert.AreEqual(UserTeam[0], "harlam357");
         Assert.AreEqual(UserTeam[1], 32);
         Assert.AreEqual(reader.ClientLogLines.GetRunUserID(reader.ClientRunList[1]), "4E34332601E26450");
         Assert.AreEqual(reader.ClientLogLines.GetRunMachineID(reader.ClientRunList[1]), 5);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfCompletedUnits, 1);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfFailedUnits, 0);
         Assert.AreEqual(reader.ClientRunList[1].NumberOfTotalUnitsCompleted, 0); // not yet detecting "+ Starting local stats count at 1"

         // Check Work Unit Data (Run Index 1 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[189].LineData, "1.90");
         Assert.AreEqual(reader.ClientLogLines[197].LineData, "4456 (Run 173, Clone 0, Gen 31)");
         Assert.AreEqual(reader.ClientLogLines[433].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }
   }
}
