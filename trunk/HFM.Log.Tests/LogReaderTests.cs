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

using NUnit.Framework;

using HFM.Framework;

namespace HFM.Log.Tests
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
      public void SMP_1_FAHlog() // verbosity 9
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt");
         
         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(2);
         expectedRun.UnitQueueIndex.Add(5);
         expectedRun.UnitStartIndex.Add(30);
         expectedRun.UnitQueueIndex.Add(6);
         expectedRun.UnitStartIndex.Add(150);
         expectedRun.Arguments = "-smp -verbosity 9";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "5131EA752EB60547";
         expectedRun.MachineID = 1;
         expectedRun.NumberOfCompletedUnits = 1;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 261;

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(274);
         expectedRun.UnitQueueIndex.Add(6);
         expectedRun.UnitStartIndex.Add(302);
         expectedRun.UnitQueueIndex.Add(7);
         expectedRun.UnitStartIndex.Add(402);
         expectedRun.Arguments = "-smp -verbosity 9";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "5131EA752EB60547";
         expectedRun.MachineID = 1;
         expectedRun.NumberOfCompletedUnits = 2;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 263;

         DoClientRunCheck(reader.ClientRunList[1], expectedRun);

         // Verify LogLine Properties
         Assert.IsNotNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);
         
         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[33].LineData, 5);
         Assert.AreEqual(reader.ClientLogLines[40].LineData, "2.08");
         Assert.That(reader.ClientLogLines[51].ToString().Contains("Project: 2677 (Run 10, Clone 29, Gen 28)"));
         Assert.AreEqual(reader.ClientLogLines[109].LineData, WorkUnitResult.FinishedUnit);
      }

      [Test, Category("SMP")]
      public void SMP_2_FAHlog() // verbosity 9
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\SMP_2\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(2);
         expectedRun.UnitQueueIndex.Add(1);
         expectedRun.UnitStartIndex.Add(30);
         expectedRun.UnitQueueIndex.Add(2);
         expectedRun.UnitStartIndex.Add(221);
         expectedRun.Arguments = "-smp -verbosity 9";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "3A49EBB303C19834";
         expectedRun.MachineID = 1;
         expectedRun.NumberOfCompletedUnits = 2;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 292;

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Verify LogLine Properties
         Assert.IsNotNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[33].LineData, 1);
         Assert.AreEqual(reader.ClientLogLines[40].LineData, "2.08");
         Assert.That(reader.ClientLogLines[47].ToString().Contains("Project: 2677 (Run 10, Clone 49, Gen 38)"));
         Assert.AreEqual(reader.ClientLogLines[180].LineData, WorkUnitResult.FinishedUnit);

         // Special Check to be sure the reader is catching the Attempting To Send line
         Assert.AreEqual(reader.ClientLogLines[379].LineType, LogLineType.ClientSendStart);
      }

      [Test, Category("SMP")]
      public void SMP_3_FAHlog() // verbosity (normal) / Handles Core Download on Startup / notfred's instance
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\SMP_3\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(2);
         expectedRun.UnitQueueIndex.Add(1);
         expectedRun.UnitStartIndex.Add(231);
         expectedRun.UnitQueueIndex.Add(2);
         expectedRun.UnitStartIndex.Add(385);
         expectedRun.Arguments = "-local -forceasm -smp 4";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         // verbosity (normal) does not output User ID after requested from server
         // see ClientLogLines indexes 29 & 30
         expectedRun.UserID = String.Empty;
         expectedRun.MachineID = 1;
         expectedRun.NumberOfCompletedUnits = 1;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 0; //TODO: not capturing line "+ Starting local stats count at 1"

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Verify LogLine Properties
         Assert.IsNotNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[234].LineData, 1);
         Assert.AreEqual(reader.ClientLogLines[239].LineData, "2.08");
         Assert.That(reader.ClientLogLines[246].ToString().Contains("Project: 2677 (Run 4, Clone 60, Gen 40)"));
         Assert.AreEqual(reader.ClientLogLines[368].LineData, WorkUnitResult.FinishedUnit);
      }

      [Test, Category("SMP")]
      public void SMP_10_FAHlog() // -smp 8 -bigadv verbosity 9 / Corrupted Log Section in Client Run Index 5
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\SMP_10\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(401);
         expectedRun.Arguments = "-configonly";
         expectedRun.FoldingID = "sneakysnowman";
         expectedRun.Team = 32;
         expectedRun.UserID = "5D2DCEF06CE524B3";
         expectedRun.MachineID = 1;
         expectedRun.NumberOfCompletedUnits = 0;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 0;

         DoClientRunCheck(reader.ClientRunList[5], expectedRun);

         // Verify LogLine Properties
         Assert.IsNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 8 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[610].LineData, 6);
         Assert.AreEqual(reader.ClientLogLines[617].LineData, "2.10");
         Assert.That(reader.ClientLogLines[628].ToString().Contains("Project: 2683 (Run 4, Clone 11, Gen 18)"));
         Assert.AreEqual(reader.ClientLogLines[660].LineData, WorkUnitResult.FinishedUnit);
      }

      [Test, Category("GPU")]
      public void GPU2_1_FAHlog() // verbosity 9
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\GPU2_1\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(2);
         expectedRun.UnitQueueIndex.Add(1);
         expectedRun.UnitStartIndex.Add(130);
         expectedRun.UnitQueueIndex.Add(2);
         expectedRun.UnitStartIndex.Add(326);
         expectedRun.UnitQueueIndex.Add(3);
         expectedRun.UnitStartIndex.Add(387);
         expectedRun.UnitQueueIndex.Add(4);
         expectedRun.UnitStartIndex.Add(449);
         expectedRun.UnitQueueIndex.Add(5);
         expectedRun.UnitStartIndex.Add(510);
         expectedRun.UnitQueueIndex.Add(6);
         expectedRun.UnitStartIndex.Add(571);
         expectedRun.Arguments = "-verbosity 9 -local";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "CF185086C102A47";
         expectedRun.MachineID = 2;
         expectedRun.NumberOfCompletedUnits = 1;
         expectedRun.NumberOfFailedUnits = 5;
         expectedRun.NumberOfTotalUnitsCompleted = 0; //TODO: not capturing line "+ Starting local stats count at 1"

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(618);
         expectedRun.UnitQueueIndex.Add(7);
         expectedRun.UnitStartIndex.Add(663);
         expectedRun.UnitQueueIndex.Add(8);
         expectedRun.UnitStartIndex.Add(737);
         expectedRun.UnitQueueIndex.Add(9);
         expectedRun.UnitStartIndex.Add(935);
         expectedRun.UnitQueueIndex.Add(0);
         expectedRun.UnitStartIndex.Add(1132);
         expectedRun.UnitQueueIndex.Add(1);
         expectedRun.UnitStartIndex.Add(1329);
         expectedRun.UnitQueueIndex.Add(2);
         expectedRun.UnitStartIndex.Add(1526);
         expectedRun.UnitQueueIndex.Add(3);
         expectedRun.UnitStartIndex.Add(1724);
         expectedRun.UnitQueueIndex.Add(4);
         expectedRun.UnitStartIndex.Add(1926);
         expectedRun.UnitQueueIndex.Add(5);
         expectedRun.UnitStartIndex.Add(2123);
         expectedRun.UnitQueueIndex.Add(6);
         expectedRun.UnitStartIndex.Add(2321);
         expectedRun.UnitQueueIndex.Add(7);
         expectedRun.UnitStartIndex.Add(2518);
         expectedRun.UnitQueueIndex.Add(8);
         expectedRun.UnitStartIndex.Add(2715);
         expectedRun.UnitQueueIndex.Add(9);
         expectedRun.UnitStartIndex.Add(2917);
         expectedRun.Arguments = "-verbosity 9 -local";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "CF185086C102A47";
         expectedRun.MachineID = 2;
         expectedRun.NumberOfCompletedUnits = 11;
         expectedRun.NumberOfFailedUnits = 1;
         expectedRun.NumberOfTotalUnitsCompleted = 12;

         DoClientRunCheck(reader.ClientRunList[1], expectedRun);

         // Verify LogLine Properties
         Assert.IsNotNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[133].LineData, 1);
         Assert.AreEqual(reader.ClientLogLines[140].LineData, "1.19");
         Assert.That(reader.ClientLogLines[154].ToString().Contains("Project: 5771 (Run 12, Clone 109, Gen 805)"));
         Assert.AreEqual(reader.ClientLogLines[286].LineData, WorkUnitResult.FinishedUnit);
      }

      [Test, Category("GPU")]
      public void GPU2_2_FAHlog() // verbosity (normal)
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\GPU2_2\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(2);
         expectedRun.UnitQueueIndex.Add(8);
         expectedRun.UnitStartIndex.Add(34);
         expectedRun.UnitQueueIndex.Add(9);
         expectedRun.UnitStartIndex.Add(208);
         expectedRun.UnitQueueIndex.Add(0);
         expectedRun.UnitStartIndex.Add(382);
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "51108B97183EA3DF";
         expectedRun.MachineID = 2;
         expectedRun.NumberOfCompletedUnits = 2;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 4221;

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Verify LogLine Properties
         Assert.IsNotNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[37].LineData, 8);
         Assert.AreEqual(reader.ClientLogLines[42].LineData, "1.19");
         Assert.That(reader.ClientLogLines[56].ToString().Contains("Project: 5751 (Run 8, Clone 205, Gen 527)"));
         Assert.AreEqual(reader.ClientLogLines[188].LineData, WorkUnitResult.FinishedUnit);
      }

      [Test, Category("GPU")]
      public void GPU2_3_FAHlog() // verbosity (normal) / EUE Pause Test
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\GPU2_3\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(0);
         expectedRun.UnitQueueIndex.Add(6);
         expectedRun.UnitStartIndex.Add(24);
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "JollySwagman";
         expectedRun.Team = 32;
         expectedRun.UserID = "1D1493BB0A79C9AE";
         expectedRun.MachineID = 2;
         expectedRun.NumberOfCompletedUnits = 0;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 0;

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(56);
         expectedRun.UnitQueueIndex.Add(6);
         expectedRun.UnitStartIndex.Add(80);
         expectedRun.UnitQueueIndex.Add(7);
         expectedRun.UnitStartIndex.Add(221);
         expectedRun.UnitQueueIndex.Add(8);
         expectedRun.UnitStartIndex.Add(271);
         expectedRun.UnitQueueIndex.Add(9);
         expectedRun.UnitStartIndex.Add(320);
         expectedRun.UnitQueueIndex.Add(0);
         expectedRun.UnitStartIndex.Add(373);
         expectedRun.UnitQueueIndex.Add(1);
         expectedRun.UnitStartIndex.Add(421);
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "JollySwagman";
         expectedRun.Team = 32;
         expectedRun.UserID = "1D1493BB0A79C9AE";
         expectedRun.MachineID = 2;
         expectedRun.NumberOfCompletedUnits = 1;
         expectedRun.NumberOfFailedUnits = 5;
         expectedRun.NumberOfTotalUnitsCompleted = 224;

         DoClientRunCheck(reader.ClientRunList[1], expectedRun);

         // Verify LogLine Properties
         Assert.IsNotNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 1 - Unit Index 3)
         Assert.AreEqual(reader.ClientLogLines[323].LineData, 9);
         Assert.AreEqual(reader.ClientLogLines[328].LineData, "1.19");
         Assert.That(reader.ClientLogLines[342].ToString().Contains("Project: 5756 (Run 6, Clone 32, Gen 480)"));
         Assert.AreEqual(reader.ClientLogLines[359].LineData, WorkUnitResult.UnstableMachine);

         // Special Check to be sure the reader is catching the EUE Pause line
         Assert.AreEqual(reader.ClientLogLines[463].LineType, LogLineType.ClientEuePauseState);
      }

      [Test, Category("GPU")]
      public void GPU2_7_FAHlog() // verbosity (normal) / Project String After "+ Processing work unit"
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\GPU2_7\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(0);
         expectedRun.UnitQueueIndex.Add(0);
         expectedRun.UnitStartIndex.Add(24);
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "Zagen30";
         expectedRun.Team = 46301;
         expectedRun.UserID = "xxxxxxxxxxxxxxxxxxx";
         expectedRun.MachineID = 2;
         expectedRun.NumberOfCompletedUnits = 0;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 1994;

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Verify LogLine Properties
         Assert.IsNull(reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[28].LineData, 0);
         Assert.AreEqual(reader.ClientLogLines[37].LineData, "1.31");
         Assert.That(reader.ClientLogLines[50].ToString().Contains("Project: 5781 (Run 2, Clone 700, Gen 2)"));

         IFahLogUnitData unitData = reader.GetFahLogDataFromLogLines(reader.CurrentWorkUnitLogLines);
         Assert.AreEqual(new TimeSpan(1, 57, 21), unitData.UnitStartTimeStamp);
         Assert.AreEqual(5, unitData.FrameDataList.Count);
         Assert.AreEqual(5, unitData.FramesObserved);
         Assert.AreEqual("1.31", unitData.CoreVersion);
         Assert.AreEqual(2, unitData.ProjectInfoList.Count);
         Assert.AreEqual(5781, unitData.ProjectInfoList[1].ProjectID);
         Assert.AreEqual(2, unitData.ProjectInfoList[1].ProjectRun);
         Assert.AreEqual(700, unitData.ProjectInfoList[1].ProjectClone);
         Assert.AreEqual(2, unitData.ProjectInfoList[1].ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitData.UnitResult);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, unitData.Status);
      }

      [Test, Category("Standard")]
      public void Standard_1_FAHlog() // verbosity 9
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\Standard_1\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(2);
         expectedRun.Arguments = "-configonly";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "4E34332601E26450";
         expectedRun.MachineID = 5;
         expectedRun.NumberOfCompletedUnits = 0;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 0;

         DoClientRunCheck(reader.ClientRunList[0], expectedRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(30);
         expectedRun.UnitQueueIndex.Add(1);
         expectedRun.UnitStartIndex.Add(179);
         expectedRun.UnitQueueIndex.Add(2);
         expectedRun.UnitStartIndex.Add(593);
         expectedRun.Arguments = "-verbosity 9 -forceasm";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "4E34332601E26450";
         expectedRun.MachineID = 5;
         expectedRun.NumberOfCompletedUnits = 1;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 0; //TODO: not capturing line "+ Starting local stats count at 1"

         DoClientRunCheck(reader.ClientRunList[1], expectedRun);

         // Check Run 2 Positions
         expectedRun = new ClientRun(839);
         expectedRun.UnitQueueIndex.Add(2);
         expectedRun.UnitStartIndex.Add(874);
         expectedRun.Arguments = "-verbosity 9 -forceasm -oneunit";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "4E34332601E26450";
         expectedRun.MachineID = 5;
         expectedRun.NumberOfCompletedUnits = 1;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 2;

         DoClientRunCheck(reader.ClientRunList[2], expectedRun);

         // Verify LogLine Properties
         Assert.IsNull(reader.PreviousWorkUnitLogLines); // No Previous Log Lines for this Run
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 1 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[182].LineData, 1);
         Assert.AreEqual(reader.ClientLogLines[189].LineData, "1.90");
         Assert.That(reader.ClientLogLines[197].ToString().Contains("Project: 4456 (Run 173, Clone 0, Gen 31)"));
         Assert.AreEqual(reader.ClientLogLines[433].LineData, WorkUnitResult.FinishedUnit);
      }

      [Test, Category("Standard")]
      public void Standard_5_FAHlog() // verbosity 9
      {
         // Scan
         reader.ScanFAHLog("..\\..\\..\\TestFiles\\Standard_5\\FAHlog.txt");

         // Check Run 3 Positions
         ClientRun expectedRun = new ClientRun(788);
         expectedRun.UnitQueueIndex.Add(4);
         expectedRun.UnitStartIndex.Add(820);
         expectedRun.Arguments = "-oneunit -forceasm -verbosity 9";
         expectedRun.FoldingID = "borden.b";
         expectedRun.Team = 131;
         expectedRun.UserID = "722723950C6887C2";
         expectedRun.MachineID = 3;
         expectedRun.NumberOfCompletedUnits = 0;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 0;

         DoClientRunCheck(reader.ClientRunList[3], expectedRun);

         // Check Run 4 Positions
         expectedRun = new ClientRun(927);
         expectedRun.UnitQueueIndex.Add(4);
         expectedRun.UnitStartIndex.Add(961);
         expectedRun.Arguments = "-forceasm -verbosity 9 -oneunit";
         expectedRun.FoldingID = "borden.b";
         expectedRun.Team = 131;
         expectedRun.UserID = "722723950C6887C2";
         expectedRun.MachineID = 3;
         expectedRun.NumberOfCompletedUnits = 0;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 0;

         DoClientRunCheck(reader.ClientRunList[4], expectedRun);

         // Verify LogLine Properties
         Assert.IsNull(reader.PreviousWorkUnitLogLines); // No Previous Log Lines for this Run
         Assert.IsNotNull(reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 4 - Unit Index 0)
         Assert.AreEqual(reader.ClientLogLines[967].LineData, 4);
         Assert.AreEqual(reader.ClientLogLines[978].LineData, "23");
         Assert.That(reader.ClientLogLines[963].ToString().Contains("Project: 6501 (Run 13, Clone 0, Gen 0)"));
         Assert.That(reader.ClientLogLines[971].ToString().Contains("Project: 6501 (Run 15, Clone 0, Gen 0)"));
         Assert.That(reader.ClientLogLines[1006].ToString().Contains("Project: 10002 (Run 19, Clone 0, Gen 51)"));
      }
      
      private static void DoClientRunCheck(ClientRun run, ClientRun expectedRun)
      {
         Assert.AreEqual(run.ClientStartIndex, expectedRun.ClientStartIndex);
         // The Unit Start and Unit Queue Index Lists should have the same
         // number of elements.  This should probably be one generic list.
         Assert.AreEqual(run.UnitQueueIndex.Count, run.UnitStartIndex.Count);

         Assert.AreEqual(run.UnitQueueIndex.Count, expectedRun.UnitQueueIndex.Count);
         Assert.AreEqual(run.UnitStartIndex.Count, expectedRun.UnitStartIndex.Count);
         for (int i = 0; i < expectedRun.UnitQueueIndex.Count; i++)
         {
            Assert.AreEqual(run.UnitQueueIndex[i], expectedRun.UnitQueueIndex[i]);
            Assert.AreEqual(run.UnitStartIndex[i], expectedRun.UnitStartIndex[i]);
         }

         Assert.AreEqual(run.Arguments, expectedRun.Arguments);
         Assert.AreEqual(run.FoldingID, expectedRun.FoldingID);
         Assert.AreEqual(run.Team, expectedRun.Team);
         Assert.AreEqual(run.UserID, expectedRun.UserID);
         Assert.AreEqual(run.MachineID, expectedRun.MachineID);
         Assert.AreEqual(run.NumberOfCompletedUnits, expectedRun.NumberOfCompletedUnits);
         Assert.AreEqual(run.NumberOfFailedUnits, expectedRun.NumberOfFailedUnits);
         Assert.AreEqual(run.NumberOfTotalUnitsCompleted, expectedRun.NumberOfTotalUnitsCompleted);
      }
   }
}
