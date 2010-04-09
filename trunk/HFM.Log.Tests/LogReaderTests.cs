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
   // ReSharper disable InconsistentNaming
   [TestFixture]
   public class LogReaderTests
   {
      private LogReader _reader;
   
      [SetUp]
      public void Init()
      {
         _reader = new LogReader();
      }
      
      [Test, Category("SMP")]

      public void SMP_1_FAHlog() // verbosity 9
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt");
         
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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[1]);

         // Verify LogLine Properties
         Assert.IsNotNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);
         
         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(5, _reader.ClientLogLines[33].LineData);
         Assert.AreEqual("2.08", _reader.ClientLogLines[40].LineData);
         Assert.That(_reader.ClientLogLines[51].ToString().Contains("Project: 2677 (Run 10, Clone 29, Gen 28)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, _reader.ClientLogLines[109].LineData);
      }

      [Test, Category("SMP")]
      public void SMP_2_FAHlog() // verbosity 9
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\SMP_2\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(1, _reader.ClientLogLines[33].LineData);
         Assert.AreEqual("2.08", _reader.ClientLogLines[40].LineData);
         Assert.That(_reader.ClientLogLines[47].ToString().Contains("Project: 2677 (Run 10, Clone 49, Gen 38)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, _reader.ClientLogLines[180].LineData);

         // Special Check to be sure the reader is catching the Attempting To Send line
         Assert.AreEqual(LogLineType.ClientSendStart, _reader.ClientLogLines[379].LineType);
      }

      [Test, Category("SMP")]
      public void SMP_3_FAHlog() // verbosity (normal) / Handles Core Download on Startup / notfred's instance
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\SMP_3\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(1, _reader.ClientLogLines[234].LineData);
         Assert.AreEqual("2.08", _reader.ClientLogLines[239].LineData);
         Assert.That(_reader.ClientLogLines[246].ToString().Contains("Project: 2677 (Run 4, Clone 60, Gen 40)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, _reader.ClientLogLines[368].LineData);
      }

      [Test, Category("SMP")]
      public void SMP_10_FAHlog() // -smp 8 -bigadv verbosity 9 / Corrupted Log Section in Client Run Index 5
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\SMP_10\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[5]);

         // Verify LogLine Properties
         Assert.IsNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 8 - Unit Index 0)
         Assert.AreEqual(6, _reader.ClientLogLines[610].LineData);
         Assert.AreEqual("2.10", _reader.ClientLogLines[617].LineData);
         Assert.That(_reader.ClientLogLines[628].ToString().Contains("Project: 2683 (Run 4, Clone 11, Gen 18)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, _reader.ClientLogLines[660].LineData);
      }

      [Test, Category("GPU")]
      public void GPU2_1_FAHlog() // verbosity 9
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\GPU2_1\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[1]);

         // Verify LogLine Properties
         Assert.IsNotNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(1, _reader.ClientLogLines[133].LineData);
         Assert.AreEqual("1.19", _reader.ClientLogLines[140].LineData);
         Assert.That(_reader.ClientLogLines[154].ToString().Contains("Project: 5771 (Run 12, Clone 109, Gen 805)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, _reader.ClientLogLines[286].LineData);
      }

      [Test, Category("GPU")]
      public void GPU2_2_FAHlog() // verbosity (normal)
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\GPU2_2\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(8, _reader.ClientLogLines[37].LineData);
         Assert.AreEqual("1.19", _reader.ClientLogLines[42].LineData);
         Assert.That(_reader.ClientLogLines[56].ToString().Contains("Project: 5751 (Run 8, Clone 205, Gen 527)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, _reader.ClientLogLines[188].LineData);
      }

      [Test, Category("GPU")]
      public void GPU2_3_FAHlog() // verbosity (normal) / EUE Pause Test
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\GPU2_3\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[1]);

         // Verify LogLine Properties
         Assert.IsNotNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 1 - Unit Index 3)
         Assert.AreEqual(9, _reader.ClientLogLines[323].LineData);
         Assert.AreEqual("1.19", _reader.ClientLogLines[328].LineData);
         Assert.That(_reader.ClientLogLines[342].ToString().Contains("Project: 5756 (Run 6, Clone 32, Gen 480)"));
         Assert.AreEqual(WorkUnitResult.UnstableMachine, _reader.ClientLogLines[359].LineData);

         // Special Check to be sure the reader is catching the EUE Pause line
         Assert.AreEqual(LogLineType.ClientEuePauseState, _reader.ClientLogLines[463].LineType);
      }

      [Test, Category("GPU")]
      public void GPU2_7_FAHlog() // verbosity (normal) / Project String After "+ Processing work unit"
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\GPU2_7\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

         // Verify LogLine Properties
         Assert.IsNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(0, _reader.ClientLogLines[28].LineData);
         Assert.AreEqual("1.31", _reader.ClientLogLines[37].LineData);
         Assert.That(_reader.ClientLogLines[50].ToString().Contains("Project: 5781 (Run 2, Clone 700, Gen 2)"));

         IFahLogUnitData unitData = _reader.GetFahLogDataFromLogLines(_reader.CurrentWorkUnitLogLines);
         Assert.AreEqual(new TimeSpan(1, 57, 21), unitData.UnitStartTimeStamp);
         Assert.AreEqual(5, unitData.FrameDataList.Count);
         Assert.AreEqual(5, unitData.FramesObserved);
         Assert.AreEqual("1.31", unitData.CoreVersion);
         Assert.AreEqual(-1, unitData.ProjectInfoIndex);
         Assert.AreEqual(2, unitData.ProjectInfoList.Count);
         Assert.AreEqual(5781, unitData.ProjectID);
         Assert.AreEqual(2, unitData.ProjectRun);
         Assert.AreEqual(700, unitData.ProjectClone);
         Assert.AreEqual(2, unitData.ProjectGen);
         Assert.AreEqual(unitData.ProjectID, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectID);
         Assert.AreEqual(unitData.ProjectRun, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectRun);
         Assert.AreEqual(unitData.ProjectClone, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectClone);
         Assert.AreEqual(unitData.ProjectGen, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitData.UnitResult);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, unitData.Status);
      }

      [Test, Category("Standard")]
      public void Standard_1_FAHlog() // verbosity 9
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\Standard_1\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[1]);

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[2]);

         // Verify LogLine Properties
         Assert.IsNull(_reader.PreviousWorkUnitLogLines); // No Previous Log Lines for this Run
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 1 - Unit Index 0)
         Assert.AreEqual(1, _reader.ClientLogLines[182].LineData);
         Assert.AreEqual("1.90", _reader.ClientLogLines[189].LineData);
         Assert.That(_reader.ClientLogLines[197].ToString().Contains("Project: 4456 (Run 173, Clone 0, Gen 31)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, _reader.ClientLogLines[433].LineData);
      }

      [Test, Category("Standard")]
      public void Standard_5_FAHlog() // verbosity 9
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\Standard_5\\FAHlog.txt");

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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[3]);
         
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

         DoClientRunCheck(expectedRun, _reader.ClientRunList[4]);

         // Verify LogLine Properties
         Assert.IsNull(_reader.PreviousWorkUnitLogLines); // No Previous Log Lines for this Run
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 4 - Unit Index 0)
         Assert.AreEqual(4, _reader.ClientLogLines[967].LineData);
         Assert.AreEqual("23", _reader.ClientLogLines[978].LineData);
         Assert.That(_reader.ClientLogLines[963].ToString().Contains("Project: 6501 (Run 13, Clone 0, Gen 0)"));
         Assert.That(_reader.ClientLogLines[971].ToString().Contains("Project: 6501 (Run 15, Clone 0, Gen 0)"));
         Assert.That(_reader.ClientLogLines[1006].ToString().Contains("Project: 10002 (Run 19, Clone 0, Gen 51)"));

         IFahLogUnitData unitData = _reader.GetFahLogDataFromLogLines(_reader.CurrentWorkUnitLogLines);
         Assert.AreEqual(new TimeSpan(0, 41, 7), unitData.UnitStartTimeStamp);
         Assert.AreEqual(5, unitData.FrameDataList.Count);
         Assert.AreEqual(5, unitData.FramesObserved);
         Assert.AreEqual("23", unitData.CoreVersion);
         Assert.AreEqual(-1, unitData.ProjectInfoIndex);
         Assert.AreEqual(3, unitData.ProjectInfoList.Count);
         Assert.AreEqual(10002, unitData.ProjectID);
         Assert.AreEqual(19, unitData.ProjectRun);
         Assert.AreEqual(0, unitData.ProjectClone);
         Assert.AreEqual(51, unitData.ProjectGen);
         Assert.AreEqual(unitData.ProjectID, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectID);
         Assert.AreEqual(unitData.ProjectRun, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectRun);
         Assert.AreEqual(unitData.ProjectClone, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectClone);
         Assert.AreEqual(unitData.ProjectGen, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitData.UnitResult);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, unitData.Status);
      }

      [Test, Category("Standard")]
      public void Standard_6_FAHlog() // verbosity normal / Gromacs 3.3
      {
         // Scan
         _reader.ScanFahLog("..\\..\\..\\TestFiles\\Standard_6\\FAHlog.txt");

         // Check Run 0 Positions
         ClientRun expectedRun = new ClientRun(2);
         expectedRun.UnitQueueIndex.Add(9);
         expectedRun.UnitStartIndex.Add(27);
         expectedRun.UnitQueueIndex.Add(0);
         expectedRun.UnitStartIndex.Add(294);
         expectedRun.UnitQueueIndex.Add(1);
         expectedRun.UnitStartIndex.Add(554);
         expectedRun.UnitQueueIndex.Add(2);
         expectedRun.UnitStartIndex.Add(814);
         expectedRun.UnitQueueIndex.Add(3);
         expectedRun.UnitStartIndex.Add(1074);
         expectedRun.UnitQueueIndex.Add(4);
         expectedRun.UnitStartIndex.Add(1338);
         expectedRun.UnitQueueIndex.Add(5);
         expectedRun.UnitStartIndex.Add(1602);
         expectedRun.UnitQueueIndex.Add(6);
         expectedRun.UnitStartIndex.Add(1870);
         expectedRun.UnitQueueIndex.Add(7);
         expectedRun.UnitStartIndex.Add(2130);
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "DrSpalding";
         expectedRun.Team = 48083;
         expectedRun.UserID = "1E19BD450434A6ED";
         expectedRun.MachineID = 1;
         expectedRun.NumberOfCompletedUnits = 8;
         expectedRun.NumberOfFailedUnits = 0;
         expectedRun.NumberOfTotalUnitsCompleted = 229;

         DoClientRunCheck(expectedRun, _reader.ClientRunList[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(_reader.PreviousWorkUnitLogLines);
         Assert.IsNotNull(_reader.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 6)
         Assert.AreEqual(7, _reader.ClientLogLines[2133].LineData);
         Assert.AreEqual("1.90", _reader.ClientLogLines[2138].LineData);
         Assert.That(_reader.ClientLogLines[2147].ToString().Contains("Project: 4461 (Run 886, Clone 3, Gen 56)"));
         
         // Special Check to be sure the reader is catching the Pause For Battery line
         Assert.AreEqual(LogLineType.WorkUnitPausedForBattery, _reader.ClientLogLines[2323].LineType);
      }
      
      private static void DoClientRunCheck(ClientRun expectedRun, ClientRun run)
      {
         Assert.AreEqual(expectedRun.ClientStartIndex, run.ClientStartIndex);
         // The Unit Start and Unit Queue Index Lists should have the same
         // number of elements.  This should probably be one generic list.
         Assert.AreEqual(run.UnitQueueIndex.Count, run.UnitStartIndex.Count);

         Assert.AreEqual(expectedRun.UnitQueueIndex.Count, run.UnitQueueIndex.Count);
         Assert.AreEqual(expectedRun.UnitStartIndex.Count, run.UnitStartIndex.Count);
         for (int i = 0; i < expectedRun.UnitQueueIndex.Count; i++)
         {
            Assert.AreEqual(expectedRun.UnitQueueIndex[i], run.UnitQueueIndex[i]);
            Assert.AreEqual(expectedRun.UnitStartIndex[i], run.UnitStartIndex[i]);
         }

         Assert.AreEqual(expectedRun.Arguments, run.Arguments);
         Assert.AreEqual(expectedRun.FoldingID, run.FoldingID);
         Assert.AreEqual(expectedRun.Team, run.Team);
         Assert.AreEqual(expectedRun.UserID, run.UserID);
         Assert.AreEqual(expectedRun.MachineID, run.MachineID);
         Assert.AreEqual(expectedRun.NumberOfCompletedUnits, run.NumberOfCompletedUnits);
         Assert.AreEqual(expectedRun.NumberOfFailedUnits, run.NumberOfFailedUnits);
         Assert.AreEqual(expectedRun.NumberOfTotalUnitsCompleted, run.NumberOfTotalUnitsCompleted);
      }
   }
   // ReSharper restore InconsistentNaming
}
