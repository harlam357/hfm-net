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
using System.Collections.Generic;

using NUnit.Framework;

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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 1.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 2);
         
         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 30);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[1], 150);
         
         Assert.AreEqual(reader.ClientRunList[1].ClientStartIndex, 274);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex.Count, 2);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[0], 302);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[1], 402);
         
         // Check Parsed Data (Run Index 1)
         reader.PopulateClientStartupArgumentData(Instance, 1);
         reader.PopulateUserAndMachineData(Instance, 1);
         reader.PopulateWorkUnitCountData(Instance, 1);
         Assert.AreEqual(Instance.Arguments, "-smp -verbosity 9");
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "5131EA752EB60547");
         Assert.AreEqual(Instance.MachineID, 1);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 2);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 0);
         Assert.AreEqual(Instance.TotalUnits, 263);
         
         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[40].LineData, "2.08");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[51]), "P2677 (R10, C29, G28)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[51]), "2677 (Run 10, Clone 29, Gen 28)");
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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 2.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 30);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[1], 221);

         // Check Parsed Data (Run Index 0)
         reader.PopulateClientStartupArgumentData(Instance, 0);
         reader.PopulateUserAndMachineData(Instance, 0);
         reader.PopulateWorkUnitCountData(Instance, 0);
         Assert.AreEqual(Instance.Arguments, "-smp -verbosity 9");
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "3A49EBB303C19834");
         Assert.AreEqual(Instance.MachineID, 1);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 2);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 0);
         Assert.AreEqual(Instance.TotalUnits, 292);

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[40].LineData, "2.08");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[47]), "P2677 (R10, C49, G38)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[47]), "2677 (Run 10, Clone 49, Gen 38)");
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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 3.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 231);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[1], 385);

         // Check Parsed Data (Run Index 0)
         reader.PopulateClientStartupArgumentData(Instance, 0);
         reader.PopulateUserAndMachineData(Instance, 0);
         reader.PopulateWorkUnitCountData(Instance, 0);
         Assert.AreEqual(Instance.Arguments, "-local -forceasm -smp 4");
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, String.Empty);
         Assert.AreEqual(Instance.MachineID, 1);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 1);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 0);
         Assert.AreEqual(Instance.TotalUnits, 0); //TODO: not capturing line "+ Starting local stats count at 1"

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[239].LineData, "2.08");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[246]), "P2677 (R4, C60, G40)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[246]), "2677 (Run 4, Clone 60, Gen 40)");
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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 1.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 2);
         
         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 6);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 130);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[1], 326);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[2], 387);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[3], 449);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[4], 510);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[5], 571);

         Assert.AreEqual(reader.ClientRunList[1].ClientStartIndex, 618);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex.Count, 13);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[0], 663);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[1], 737);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[2], 935);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[3], 1132);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[4], 1329);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[5], 1526);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[6], 1724);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[7], 1926);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[8], 2123);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[9], 2321);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[10], 2518);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[11], 2715);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[12], 2917);

         // Check Parsed Data (Run Index 1)
         reader.PopulateClientStartupArgumentData(Instance, 1);
         reader.PopulateUserAndMachineData(Instance, 1);
         reader.PopulateWorkUnitCountData(Instance, 1);
         Assert.AreEqual(Instance.Arguments, "-verbosity 9 -local");
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "CF185086C102A47");
         Assert.AreEqual(Instance.MachineID, 2);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 11);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 1);
         Assert.AreEqual(Instance.TotalUnits, 12);

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[140].LineData, "1.19");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[154]), "P5771 (R12, C109, G805)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[154]), "5771 (Run 12, Clone 109, Gen 805)");
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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 2.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 3);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 34);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[1], 208);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[2], 382);

         // Check Parsed Data (Run Index 0)
         reader.PopulateClientStartupArgumentData(Instance, 0);
         reader.PopulateUserAndMachineData(Instance, 0);
         reader.PopulateWorkUnitCountData(Instance, 0);
         Assert.AreEqual(Instance.Arguments, String.Empty);
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "51108B97183EA3DF");
         Assert.AreEqual(Instance.MachineID, 2);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 2);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 0);
         Assert.AreEqual(Instance.TotalUnits, 4221);

         // Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[42].LineData, "1.19");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[56]), "P5751 (R8, C205, G527)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[56]), "5751 (Run 8, Clone 205, Gen 527)");
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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 3 EUE Pause.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 2);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 0);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 1);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 24);

         Assert.AreEqual(reader.ClientRunList[1].ClientStartIndex, 56);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex.Count, 6);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[0], 80);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[1], 221);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[2], 271);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[3], 320);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[4], 373);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[5], 421);

         // Check Parsed Data (Run Index 1)
         reader.PopulateClientStartupArgumentData(Instance, 1);
         reader.PopulateUserAndMachineData(Instance, 1);
         reader.PopulateWorkUnitCountData(Instance, 1);
         Assert.AreEqual(Instance.Arguments, String.Empty);
         Assert.AreEqual(Instance.FoldingID, "JollySwagman");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "1D1493BB0A79C9AE");
         Assert.AreEqual(Instance.MachineID, 2);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 1);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 5);
         Assert.AreEqual(Instance.TotalUnits, 224);

         // Check Work Unit Data (Run Index 1 - Unit Index 3)
         Assert.AreEqual(reader.ClientLogLines[328].LineData, "1.19");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[342]), "P5756 (R6, C32, G480)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[342]), "5756 (Run 6, Clone 32, Gen 480)");
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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\SMP Test INTERRUPTED Bad WUs FAHlog.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 8);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 30);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[1], 171);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[2], 221);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[3], 274);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[4], 338);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[5], 388);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[6], 434);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[7], 498);

         // Check Parsed Data (Run Index 0)
         reader.PopulateClientStartupArgumentData(Instance, 0);
         reader.PopulateUserAndMachineData(Instance, 0);
         reader.PopulateWorkUnitCountData(Instance, 0);
         Assert.AreEqual(Instance.Arguments, "-smp -verbosity 9");
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "75B5B5EE3198996B");
         Assert.AreEqual(Instance.MachineID, 1);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 1);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 2);
         Assert.AreEqual(Instance.TotalUnits, 255);

         // Check Work Unit Data (Run Index 0 - Unit Index 6)
         Assert.AreEqual(reader.ClientLogLines[444].LineData, "2.10");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[451]), "P2677 (R34, C40, G30)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[451]), "2677 (Run 34, Clone 40, Gen 30)");
         //Assert.AreEqual(reader.ClientLogLines[458].LineData, WorkUnitResult.Interrupted);

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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\SMP Test INTERRUPTED Bad WUs FAHlog 2.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 1);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex.Count, 10);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[0], 52);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[1], 243);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[2], 434);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[3], 484);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[4], 537);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[5], 600);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[6], 651);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[7], 702);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[8], 766);
         Assert.AreEqual(reader.ClientRunList[0].UnitStartIndex[9], 1267);

         // Check Parsed Data (Run Index 0)
         reader.PopulateClientStartupArgumentData(Instance, 0);
         reader.PopulateUserAndMachineData(Instance, 0);
         reader.PopulateWorkUnitCountData(Instance, 0);
         Assert.AreEqual(Instance.Arguments, "-smp -verbosity 9");
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "B26C4CA3732C261");
         Assert.AreEqual(Instance.MachineID, 1);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 3);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 1);
         Assert.AreEqual(Instance.TotalUnits, 319);

         // Check Work Unit Data (Run Index 0 - Unit Index 3)
         Assert.AreEqual(reader.ClientLogLines[494].LineData, "2.10");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[505]), "P2677 (R0, C74, G38)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[505]), "2677 (Run 0, Clone 74, Gen 38)");
         Assert.AreEqual(reader.ClientLogLines[510].LineData, WorkUnitResult.Interrupted);

         // Check Work Unit Data (Run Index 0 - Unit Index 8)
         Assert.AreEqual(reader.ClientLogLines[776].LineData, "2.10");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[787]), "P2677 (R15, C90, G48)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[787]), "2677 (Run 15, Clone 90, Gen 48)");
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
         reader.ScanFAHLog(Instance, "..\\..\\TestFiles\\Standard Test FAHlog 1.txt");
         Assert.AreEqual(reader.ClientRunList.Count, 3);

         Assert.AreEqual(reader.ClientRunList[0].ClientStartIndex, 2);
         Assert.AreEqual(reader.ClientRunList[1].ClientStartIndex, 30);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex.Count, 2);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[0], 179);
         Assert.AreEqual(reader.ClientRunList[1].UnitStartIndex[1], 593);
         Assert.AreEqual(reader.ClientRunList[2].ClientStartIndex, 839);
         Assert.AreEqual(reader.ClientRunList[2].UnitStartIndex.Count, 1);
         Assert.AreEqual(reader.ClientRunList[2].UnitStartIndex[0], 874);

         // Check Parsed Data (Run Index 1)
         reader.PopulateClientStartupArgumentData(Instance, 1);
         reader.PopulateUserAndMachineData(Instance, 1);
         reader.PopulateWorkUnitCountData(Instance, 1);
         Assert.AreEqual(Instance.Arguments, "-verbosity 9 -forceasm");
         Assert.AreEqual(Instance.FoldingID, "harlam357");
         Assert.AreEqual(Instance.Team, 32);
         Assert.AreEqual(Instance.UserID, "4E34332601E26450");
         Assert.AreEqual(Instance.MachineID, 5);
         Assert.AreEqual(Instance.NumberOfCompletedUnitsSinceLastStart, 1);
         Assert.AreEqual(Instance.NumberOfFailedUnitsSinceLastStart, 0);
         Assert.AreEqual(Instance.TotalUnits, 0); //TODO: not capturing line "+ Starting local stats count at 1"

         // Check Work Unit Data (Run Index 1 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[189].LineData, "1.90");
         Assert.AreEqual(LogLine.GetProjectString(reader.ClientLogLines[197]), "P4456 (R173, C0, G31)");
         Assert.AreEqual(LogLine.GetLongProjectString(reader.ClientLogLines[197]), "4456 (Run 173, Clone 0, Gen 31)");
         Assert.AreEqual(reader.ClientLogLines[433].LineData, WorkUnitResult.FinishedUnit);

         // Check that the Previous and Current LogLines return a value
         ICollection<LogLine> logLines = reader.PreviousWorkUnitLogLines;
         Assert.IsNull(logLines);

         logLines = reader.CurrentWorkUnitLogLines;
         Assert.IsNotNull(logLines);
      }
   }
}
