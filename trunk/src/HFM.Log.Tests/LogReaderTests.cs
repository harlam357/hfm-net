/*
 * HFM.NET - Log Reader Class Tests
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.IO;
using System.Linq;
using NUnit.Framework;

using HFM.Framework.DataTypes;

namespace HFM.Log.Tests
{
   // ReSharper disable InconsistentNaming

   [TestFixture]
   public class LogReaderTests
   {
      [Test, Category("SMP")]
      public void SMP_1_FAHlog() // verbosity 9
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);
         
         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(5, 30));
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 150));
         expectedRun.Arguments = "-smp -verbosity 9";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "5131EA752EB60547";
         expectedRun.MachineID = 1;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 261;
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Check Run 1 Positions
         expectedRun = new ClientRun(274);
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 302));
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 402));
         expectedRun.Arguments = "-smp -verbosity 9";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "5131EA752EB60547";
         expectedRun.MachineID = 1;
         expectedRun.CompletedUnits = 2;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 263;
         expectedRun.Status = ClientStatus.GettingWorkPacket;

         DoClientRunCheck(expectedRun, clientRuns[1]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);
         
         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(5, logLines[33].LineData);
         Assert.AreEqual("2.08", logLines[40].LineData);
         Assert.That(logLines[51].ToString().Contains("Project: 2677 (Run 10, Clone 29, Gen 28)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, logLines[109].LineData);
      }

      [Test, Category("SMP")]
      public void SMP_2_FAHlog() // verbosity 9
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\SMP_2\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 30));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 221));
         expectedRun.Arguments = "-smp -verbosity 9";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "3A49EBB303C19834";
         expectedRun.MachineID = 1;
         expectedRun.CompletedUnits = 2;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 292;
         expectedRun.Status = ClientStatus.SendingWorkPacket;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(1, logLines[33].LineData);
         Assert.AreEqual("2.08", logLines[40].LineData);
         Assert.That(logLines[47].ToString().Contains("Project: 2677 (Run 10, Clone 49, Gen 38)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, logLines[180].LineData);

         // Special Check to be sure the reader is catching the Attempting To Send line
         Assert.AreEqual(LogLineType.ClientSendStart, logLines[379].LineType);
      }

      [Test, Category("SMP")]
      public void SMP_3_FAHlog() // verbosity (normal) / Handles Core Download on Startup / notfred's instance
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\SMP_3\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 231));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 385));
         expectedRun.Arguments = "-local -forceasm -smp 4";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         // verbosity (normal) does not output User ID after requested from server
         // see ClientLogLines indexes 29 & 30
         expectedRun.UserID = String.Empty;
         expectedRun.MachineID = 1;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0; //TODO: not capturing line "+ Starting local stats count at 1"
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(1, logLines[234].LineData);
         Assert.AreEqual("2.08", logLines[239].LineData);
         Assert.That(logLines[246].ToString().Contains("Project: 2677 (Run 4, Clone 60, Gen 40)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, logLines[368].LineData);
      }

      [Test, Category("SMP")]
      public void SMP_10_FAHlog() // -smp 8 -bigadv verbosity 9 / Corrupted Log Section in Client Run Index 5
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\SMP_10\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 5 Positions
         var expectedRun = new ClientRun(401);
         expectedRun.UnitIndexes.Add(new UnitIndex(-1, 426));
         expectedRun.Arguments = "-configonly";
         expectedRun.FoldingID = "sneakysnowman";
         expectedRun.Team = 32;
         expectedRun.UserID = "5D2DCEF06CE524B3";
         expectedRun.MachineID = 1;
         expectedRun.CompletedUnits = 0;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0;
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[5]);

         // Verify LogLine Properties
         Assert.IsNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 8 - Unit Index 0)
         Assert.AreEqual(6, logLines[610].LineData);
         Assert.AreEqual("2.10", logLines[617].LineData);
         Assert.That(logLines[628].ToString().Contains("Project: 2683 (Run 4, Clone 11, Gen 18)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, logLines[660].LineData);
      }

      [Test, Category("SMP")]
      public void SMP_15_FAHlog() // lots of Client-core communications error
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\SMP_15\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 36));
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 234));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 284));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 334));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 658));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 708));
         expectedRun.UnitIndexes.Add(new UnitIndex(3, 758));
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 1082));
         expectedRun.UnitIndexes.Add(new UnitIndex(5, 1147));
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 1219));
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 1269));
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 1341));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 1436));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 1538));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 1588));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 1638));
         expectedRun.UnitIndexes.Add(new UnitIndex(3, 1710));
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 1760));
         expectedRun.UnitIndexes.Add(new UnitIndex(5, 1825));
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 2149));
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 2199));
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 2418));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 2490));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 2540));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 2590));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 2914));
         expectedRun.UnitIndexes.Add(new UnitIndex(3, 2964));
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 3014));
         expectedRun.UnitIndexes.Add(new UnitIndex(5, 3353));
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 3448));
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 3498));
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 3645));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 3710));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 3760));
         expectedRun.Arguments = "-smp -verbosity 9";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "DC1DAF57D91DF79";
         expectedRun.MachineID = 1;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 33;
         expectedRun.TotalCompletedUnits = 617;
         expectedRun.Status = ClientStatus.EuePause;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 33)
         Assert.AreEqual(0, logLines[3763].LineData);
         Assert.AreEqual("2.22", logLines[3770].LineData);
         Assert.That(logLines[3780].ToString().Contains("Project: 6071 (Run 0, Clone 39, Gen 70)"));
         Assert.AreEqual(WorkUnitResult.ClientCoreError, logLines[3786].LineData);
      }

      [Test, Category("GPU")]
      public void GPU2_1_FAHlog() // verbosity 9
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\GPU2_1\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 130));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 326));
         expectedRun.UnitIndexes.Add(new UnitIndex(3, 387));
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 449));
         expectedRun.UnitIndexes.Add(new UnitIndex(5, 510));
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 571));
         expectedRun.Arguments = "-verbosity 9 -local";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "CF185086C102A47";
         expectedRun.MachineID = 2;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 5;
         expectedRun.TotalCompletedUnits = 0; //TODO: not capturing line "+ Starting local stats count at 1"
         expectedRun.Status = ClientStatus.Stopped;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Check Run 1 Positions
         expectedRun = new ClientRun(618);
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 663));
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 737));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 935));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 1132));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 1329));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 1526));
         expectedRun.UnitIndexes.Add(new UnitIndex(3, 1724));
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 1926));
         expectedRun.UnitIndexes.Add(new UnitIndex(5, 2123));
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 2321));
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 2518));
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 2715));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 2917));
         expectedRun.Arguments = "-verbosity 9 -local";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "CF185086C102A47";
         expectedRun.MachineID = 2;
         expectedRun.CompletedUnits = 11;
         expectedRun.FailedUnits = 1;
         expectedRun.TotalCompletedUnits = 12;
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[1]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(1, logLines[133].LineData);
         Assert.AreEqual("1.19", logLines[140].LineData);
         Assert.That(logLines[154].ToString().Contains("Project: 5771 (Run 12, Clone 109, Gen 805)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, logLines[286].LineData);
      }

      [Test, Category("GPU")]
      public void GPU2_2_FAHlog() // verbosity (normal)
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\GPU2_2\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 34));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 208));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 382));
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "51108B97183EA3DF";
         expectedRun.MachineID = 2;
         expectedRun.CompletedUnits = 2;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 4221;
         expectedRun.Status = ClientStatus.Stopped;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(8, logLines[37].LineData);
         Assert.AreEqual("1.19", logLines[42].LineData);
         Assert.That(logLines[56].ToString().Contains("Project: 5751 (Run 8, Clone 205, Gen 527)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, logLines[188].LineData);
      }

      [Test, Category("GPU")]
      public void GPU2_3_FAHlog() // verbosity (normal) / EUE Pause Test
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\GPU2_3\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(0);
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 24));
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "JollySwagman";
         expectedRun.Team = 32;
         expectedRun.UserID = "1D1493BB0A79C9AE";
         expectedRun.MachineID = 2;
         expectedRun.CompletedUnits = 0;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0;
         expectedRun.Status = ClientStatus.Stopped;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Check Run 1 Positions
         expectedRun = new ClientRun(56);
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 80));
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 221));
         expectedRun.UnitIndexes.Add(new UnitIndex(8, 271));
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 320));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 373));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 421));
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "JollySwagman";
         expectedRun.Team = 32;
         expectedRun.UserID = "1D1493BB0A79C9AE";
         expectedRun.MachineID = 2;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 5;
         expectedRun.TotalCompletedUnits = 224;
         expectedRun.Status = ClientStatus.EuePause;

         DoClientRunCheck(expectedRun, clientRuns[1]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 1 - Unit Index 3)
         Assert.AreEqual(9, logLines[323].LineData);
         Assert.AreEqual("1.19", logLines[328].LineData);
         Assert.That(logLines[342].ToString().Contains("Project: 5756 (Run 6, Clone 32, Gen 480)"));
         Assert.AreEqual(WorkUnitResult.UnstableMachine, logLines[359].LineData);

         // Special Check to be sure the reader is catching the EUE Pause line
         Assert.AreEqual(LogLineType.ClientEuePauseState, logLines[463].LineType);
      }

      [Test, Category("GPU")]
      public void GPU2_7_FAHlog() // verbosity (normal) / Project String After "+ Processing work unit"
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\GPU2_7\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(0);
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 24));
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "Zagen30";
         expectedRun.Team = 46301;
         expectedRun.UserID = "xxxxxxxxxxxxxxxxxxx";
         expectedRun.MachineID = 2;
         expectedRun.CompletedUnits = 0;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 1994;
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Verify LogLine Properties
         Assert.IsNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(0, logLines[28].LineData);
         Assert.AreEqual("1.31", logLines[37].LineData);
         Assert.That(logLines[50].ToString().Contains("Project: 5781 (Run 2, Clone 700, Gen 2)"));

         var unitData = LogReader.GetFahLogDataFromLogLines(logInterpreter.CurrentWorkUnitLogLines);
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
      }

      [Test, Category("GPU")]
      public void GPU3_2_FAHlog() // verbosity 9 / OPENMMGPU v2.19
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\GPU3_2\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(3, 27));
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 170));
         expectedRun.Arguments = "-gpu 0 -verbosity 9 -local -verbosity 9";
         expectedRun.FoldingID = "HayesK";
         expectedRun.Team = 32;
         expectedRun.UserID = "37114EB5198643C1";
         expectedRun.MachineID = 2;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 847;
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 0)
         Assert.AreEqual(3, logLines[34].LineData);
         Assert.AreEqual("2.19", logLines[41].LineData);
         Assert.That(logLines[56].ToString().Contains("Project: 10634 (Run 11, Clone 24, Gen 14)"));

         var unitData = LogReader.GetFahLogDataFromLogLines(logInterpreter.CurrentWorkUnitLogLines);
         Assert.AreEqual(new TimeSpan(17, 31, 22), unitData.UnitStartTimeStamp);
         Assert.AreEqual(12, unitData.FrameDataList.Count);
         Assert.AreEqual(12, unitData.FramesObserved);
         Assert.AreEqual("2.19", unitData.CoreVersion);
         Assert.AreEqual(-1, unitData.ProjectInfoIndex);
         Assert.AreEqual(1, unitData.ProjectInfoList.Count);
         Assert.AreEqual(10634, unitData.ProjectID);
         Assert.AreEqual(8, unitData.ProjectRun);
         Assert.AreEqual(24, unitData.ProjectClone);
         Assert.AreEqual(24, unitData.ProjectGen);
         Assert.AreEqual(unitData.ProjectID, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectID);
         Assert.AreEqual(unitData.ProjectRun, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectRun);
         Assert.AreEqual(unitData.ProjectClone, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectClone);
         Assert.AreEqual(unitData.ProjectGen, unitData.ProjectInfoList[unitData.ProjectInfoList.Count - 1].ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitData.UnitResult);
      }

      [Test, Category("Standard")]
      public void Standard_1_FAHlog() // verbosity 9
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\Standard_1\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.Arguments = "-configonly";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "4E34332601E26450";
         expectedRun.MachineID = 5;
         expectedRun.CompletedUnits = 0;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Check Run 1 Positions
         expectedRun = new ClientRun(30);
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 179));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 593));
         expectedRun.Arguments = "-verbosity 9 -forceasm";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "4E34332601E26450";
         expectedRun.MachineID = 5;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0; //TODO: not capturing line "+ Starting local stats count at 1"
         expectedRun.Status = ClientStatus.Stopped;

         DoClientRunCheck(expectedRun, clientRuns[1]);

         // Check Run 2 Positions
         expectedRun = new ClientRun(839);
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 874));
         expectedRun.Arguments = "-verbosity 9 -forceasm -oneunit";
         expectedRun.FoldingID = "harlam357";
         expectedRun.Team = 32;
         expectedRun.UserID = "4E34332601E26450";
         expectedRun.MachineID = 5;
         expectedRun.CompletedUnits = 1;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 2;
         expectedRun.Status = ClientStatus.Stopped;

         DoClientRunCheck(expectedRun, clientRuns[2]);

         // Verify LogLine Properties
         Assert.IsNull(logInterpreter.PreviousWorkUnitLogLines); // No Previous Log Lines for this Run
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 1 - Unit Index 0)
         Assert.AreEqual(1, logLines[182].LineData);
         Assert.AreEqual("1.90", logLines[189].LineData);
         Assert.That(logLines[197].ToString().Contains("Project: 4456 (Run 173, Clone 0, Gen 31)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, logLines[433].LineData);
      }

      [Test, Category("Standard")]
      public void Standard_5_FAHlog() // verbosity 9
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\Standard_5\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 3 Positions
         var expectedRun = new ClientRun(788);
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 820));
         expectedRun.Arguments = "-oneunit -forceasm -verbosity 9";
         expectedRun.FoldingID = "borden.b";
         expectedRun.Team = 131;
         expectedRun.UserID = "722723950C6887C2";
         expectedRun.MachineID = 3;
         expectedRun.CompletedUnits = 0;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0;
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[3]);
         
         // Check Run 4 Positions
         expectedRun = new ClientRun(927);
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 961));
         expectedRun.Arguments = "-forceasm -verbosity 9 -oneunit";
         expectedRun.FoldingID = "borden.b";
         expectedRun.Team = 131;
         expectedRun.UserID = "722723950C6887C2";
         expectedRun.MachineID = 3;
         expectedRun.CompletedUnits = 0;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0;
         expectedRun.Status = ClientStatus.RunningNoFrameTimes;

         DoClientRunCheck(expectedRun, clientRuns[4]);

         // Verify LogLine Properties
         Assert.IsNull(logInterpreter.PreviousWorkUnitLogLines); // No Previous Log Lines for this Run
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 4 - Unit Index 0)
         Assert.AreEqual(4, logLines[967].LineData);
         Assert.AreEqual("23", logLines[978].LineData);
         Assert.That(logLines[963].ToString().Contains("Project: 6501 (Run 13, Clone 0, Gen 0)"));
         Assert.That(logLines[971].ToString().Contains("Project: 6501 (Run 15, Clone 0, Gen 0)"));
         Assert.That(logLines[1006].ToString().Contains("Project: 10002 (Run 19, Clone 0, Gen 51)"));

         var unitData = LogReader.GetFahLogDataFromLogLines(logInterpreter.CurrentWorkUnitLogLines);
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
      }

      [Test, Category("Standard")]
      public void Standard_6_FAHlog() // verbosity normal / Gromacs 3.3
      {
         // Scan
         var logLines = LogReader.GetLogLines("..\\..\\..\\TestFiles\\Standard_6\\FAHlog.txt");
         var clientRuns = LogReader.GetClientRuns(logLines);
         var logInterpreter = new LogInterpreterLegacy(logLines, clientRuns);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(2);
         expectedRun.UnitIndexes.Add(new UnitIndex(9, 27));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 294));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 554));
         expectedRun.UnitIndexes.Add(new UnitIndex(2, 814));
         expectedRun.UnitIndexes.Add(new UnitIndex(3, 1074));
         expectedRun.UnitIndexes.Add(new UnitIndex(4, 1338));
         expectedRun.UnitIndexes.Add(new UnitIndex(5, 1602));
         expectedRun.UnitIndexes.Add(new UnitIndex(6, 1870));
         expectedRun.UnitIndexes.Add(new UnitIndex(7, 2130));
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "DrSpalding";
         expectedRun.Team = 48083;
         expectedRun.UserID = "1E19BD450434A6ED";
         expectedRun.MachineID = 1;
         expectedRun.CompletedUnits = 8;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 229;
         expectedRun.Status = ClientStatus.Paused;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         // Verify LogLine Properties
         Assert.IsNotNull(logInterpreter.PreviousWorkUnitLogLines);
         Assert.IsNotNull(logInterpreter.CurrentWorkUnitLogLines);

         // Spot Check Work Unit Data (Run Index 0 - Unit Index 6)
         Assert.AreEqual(7, logLines[2133].LineData);
         Assert.AreEqual("1.90", logLines[2138].LineData);
         Assert.That(logLines[2147].ToString().Contains("Project: 4461 (Run 886, Clone 3, Gen 56)"));
         
         // Special Check to be sure the reader is catching the Pause For Battery line
         Assert.AreEqual(LogLineType.WorkUnitPausedForBattery, logLines[2323].LineType);
      }

      #region Version 7 Logs

      [Test]
      public void Client_v7_1()
      {
         // Scan
         IList<LogLine> logLines = LogReader.GetLogLines(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_1\\log.txt"), LogFileType.Version7);
         IList<ClientRun> clientRuns = LogReader.GetClientRuns(logLines, LogFileType.Version7);
         
         // Check Run 0 Positions
         var expectedRun = new ClientRun(1);
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 174, 272));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 236, 433));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 399, 609));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 561, 771));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 740, 1085));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 899, 1240));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 1208, 1400));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 1369, 1564));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 1526, 1726));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 1692, 1886));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 1853, 2063));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 2015, 2225));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 2194, 2385));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 2352, 2546));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 2511, 2707));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 2673, 2868));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 2833, 3028));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 2997, 3198));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 3154, 3360));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 3325, 3523));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 3487, 3699));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 3664, 3876));
         expectedRun.UnitIndexes.Add(new UnitIndex(1, 3827, 4053));
         expectedRun.UnitIndexes.Add(new UnitIndex(0, 4005, -1));
         expectedRun.Arguments = String.Empty;
         expectedRun.FoldingID = "Unknown";
         expectedRun.Team = 0;
         expectedRun.UserID = String.Empty;
         expectedRun.MachineID = 0;
         expectedRun.CompletedUnits = 21;
         expectedRun.FailedUnits = 0;
         expectedRun.TotalCompletedUnits = 0;
         expectedRun.Status = ClientStatus.Unknown;

         DoClientRunCheck(expectedRun, clientRuns[0]);

         var logInterpreter = new LogInterpreter(logLines, clientRuns);
         Assert.AreEqual(1, logInterpreter.LogLineParsingErrors.Count());

         logLines = logInterpreter.GetLogLinesForQueueIndex(1, new ProjectInfo { ProjectID = 7136, ProjectRun = 0, ProjectClone = 39, ProjectGen = 103 });
         Assert.AreEqual(99, logLines.Count);
         FahLogUnitData fahLogData = LogReader.GetFahLogDataFromLogLines(logLines.Filter(LogFilterType.IndexAndNonIndexed));
         Assert.AreEqual(new TimeSpan(18, 6, 54), fahLogData.UnitStartTimeStamp);
         Assert.AreEqual(6, fahLogData.FrameDataList.Count);
         Assert.AreEqual(6, fahLogData.FramesObserved);
         Assert.AreEqual(7136, fahLogData.ProjectID);
         Assert.AreEqual(0, fahLogData.ProjectRun);
         Assert.AreEqual(39, fahLogData.ProjectClone);
         Assert.AreEqual(103, fahLogData.ProjectGen);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, fahLogData.UnitResult);

         logLines = logInterpreter.GetLogLinesForQueueIndex(0, new ProjectInfo { ProjectID = 6984, ProjectRun = 0, ProjectClone = 1, ProjectGen = 18 });
         Assert.AreEqual(198, logLines.Count);
         fahLogData = LogReader.GetFahLogDataFromLogLines(logLines.Filter(LogFilterType.IndexAndNonIndexed));
         Assert.AreEqual(new TimeSpan(18, 39, 17), fahLogData.UnitStartTimeStamp);
         Assert.AreEqual(101, fahLogData.FrameDataList.Count);
         Assert.AreEqual(101, fahLogData.FramesObserved);
         Assert.AreEqual(6984, fahLogData.ProjectID);
         Assert.AreEqual(0, fahLogData.ProjectRun);
         Assert.AreEqual(1, fahLogData.ProjectClone);
         Assert.AreEqual(18, fahLogData.ProjectGen);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, fahLogData.UnitResult);
      }

      #endregion
      
      private static void DoClientRunCheck(ClientRun expectedRun, ClientRun run)
      {
         Assert.AreEqual(expectedRun.ClientStartIndex, run.ClientStartIndex);
         Assert.AreEqual(expectedRun.UnitIndexes.Count, run.UnitIndexes.Count);
         for (int i = 0; i < expectedRun.UnitIndexes.Count; i++)
         {
            Assert.AreEqual(expectedRun.UnitIndexes[i].QueueIndex, run.UnitIndexes[i].QueueIndex);
            Assert.AreEqual(expectedRun.UnitIndexes[i].StartIndex, run.UnitIndexes[i].StartIndex);
            Assert.AreEqual(expectedRun.UnitIndexes[i].EndIndex, run.UnitIndexes[i].EndIndex);
         }

         Assert.AreEqual(expectedRun.Arguments, run.Arguments);
         Assert.AreEqual(expectedRun.FoldingID, run.FoldingID);
         Assert.AreEqual(expectedRun.Team, run.Team);
         Assert.AreEqual(expectedRun.UserID, run.UserID);
         Assert.AreEqual(expectedRun.MachineID, run.MachineID);
         Assert.AreEqual(expectedRun.CompletedUnits, run.CompletedUnits);
         Assert.AreEqual(expectedRun.FailedUnits, run.FailedUnits);
         Assert.AreEqual(expectedRun.TotalCompletedUnits, run.TotalCompletedUnits);
         Assert.AreEqual(expectedRun.Status, run.Status);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void GetLogLines_ArgumentNull1()
      {
         string logFilePath = null;
         LogReader.GetLogLines(logFilePath);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void GetLogLines_ArgumentEmpty()
      {
         string logFilePath = String.Empty;
         LogReader.GetLogLines(logFilePath);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void GetLogLines_ArgumentNull2()
      {
         IList<string> logLines = null;
         LogReader.GetLogLines(logLines);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void GetClientRuns_ArgumentNull()
      {
         IList<LogLine> logLines = null;
         LogReader.GetClientRuns(logLines);
      }
      
      [Test, Category("GPU")]
      public void GPU2_5_UnitInfo()
      {
         var data = LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\GPU2_5\\unitinfo.txt");
         Assert.AreEqual("p4744_lam5w_300K", data.ProteinName);
         Assert.AreEqual("-", data.ProteinTag);
         Assert.AreEqual(0, data.ProjectID);
         Assert.AreEqual(0, data.ProjectRun);
         Assert.AreEqual(0, data.ProjectClone);
         Assert.AreEqual(0, data.ProjectGen);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 1, 2, 20, 35, 41), data.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 1, 5, 20, 35, 41), data.DueTime);
         Assert.AreEqual(73, data.Progress);
      }

      [Test, Category("SMP")]
      public void SMP_10_UnitInfo()
      {
         var data = LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\SMP_10\\unitinfo.txt");
         Assert.AreEqual("Gromacs", data.ProteinName);
         Assert.AreEqual("P2683R6C12G21", data.ProteinTag);
         Assert.AreEqual(2683, data.ProjectID);
         Assert.AreEqual(6, data.ProjectRun);
         Assert.AreEqual(12, data.ProjectClone);
         Assert.AreEqual(21, data.ProjectGen);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 12, 12, 0, 9, 22), data.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 12, 18, 0, 9, 22), data.DueTime);
         Assert.AreEqual(1724900, data.Progress);
      }

      [Test]
      public void GetFahLogDataFromLogLines_ArgumentNull()
      {
         Assert.IsNotNull(LogReader.GetFahLogDataFromLogLines(null));
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void GetUnitInfoLogData_ArgumentNull()
      {
         LogReader.GetUnitInfoLogData(null);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void GetUnitInfoLogData_ArgumentEmpty()
      {
         LogReader.GetUnitInfoLogData(String.Empty);
      }
      
      [Test]
      [ExpectedException(typeof(IOException))]
      public void GetUnitInfoLogData_FileDoesNotExist()
      {
         LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\DoesNotExist\\unitinfo.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo1()
      {
         LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo1.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo2()
      {
         LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo2.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo3()
      {
         LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo3.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo4()
      {
         LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo4.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo5()
      {
         LogReader.GetUnitInfoLogData("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo5.txt");
      }

      [Test]
      public void WorkUnitResultFromString()
      {
         Assert.AreEqual(WorkUnitResult.FinishedUnit, "FINISHED_UNIT".ToWorkUnitResult());
         Assert.AreEqual(WorkUnitResult.EarlyUnitEnd, "EARLY_UNIT_END".ToWorkUnitResult());
         Assert.AreEqual(WorkUnitResult.UnstableMachine, "UNSTABLE_MACHINE".ToWorkUnitResult());
         Assert.AreEqual(WorkUnitResult.Interrupted, "INTERRUPTED".ToWorkUnitResult());
         Assert.AreEqual(WorkUnitResult.BadWorkUnit, "BAD_WORK_UNIT".ToWorkUnitResult());
         Assert.AreEqual(WorkUnitResult.CoreOutdated, "CORE_OUTDATED".ToWorkUnitResult());
         Assert.AreEqual(WorkUnitResult.Unknown, "afasfdsafasdfas".ToWorkUnitResult());
      }
   }

   // ReSharper restore InconsistentNaming
}
