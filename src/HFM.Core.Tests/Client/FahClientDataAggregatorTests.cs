/*
 * HFM.NET - Fah Client Data Aggregator Class Tests
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Client;
using HFM.Client.DataTypes;
using HFM.Core.DataTypes;
using HFM.Log;
using HFM.Log.FahClient;

namespace HFM.Core
{
   [TestFixture]
   public class FahClientDataAggregatorTests
   {
      private FahClientDataAggregator _dataAggregator;

      [SetUp]
      public void Init()
      {
         _dataAggregator = new FahClientDataAggregator();
      }

      // ReSharper disable InconsistentNaming

      [Test]
      public void Client_v7_10_0()
      {
         const int slotId = 0;
         _dataAggregator.ClientName = "Client_v7_10";

         var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt");

         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\info.txt");
         var info = new Info();
         info.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\options.txt");
         var options = new Options();
         options.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         var result = _dataAggregator.AggregateData(fahLog.ClientRuns.Last(), unitCollection, info, options, slotOptions, new UnitInfo(), slotId);
         Assert.AreEqual(1, result.UnitInfos.Count);
         Assert.IsFalse(result.UnitInfos.Any(x => x.Value == null));

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(2012, 1, 11, 3, 24, 22), result.StartTime);
         Assert.AreEqual(null, result.Arguments);
         Assert.AreEqual(null, result.ClientVersion);
         Assert.AreEqual(null, result.UserID);
         Assert.AreEqual(0, result.MachineID);
         Assert.AreEqual(SlotStatus.Unknown, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsFalse(result.UnitInfos.Any(x => x.Value.LogLines == null));
         if (result.UnitInfos.ContainsKey(result.CurrentUnitIndex))
         {
            Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);
         }

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(3, 25, 32), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.27f, unitInfoData.CoreVersion);
         Assert.AreEqual(7610, unitInfoData.ProjectID);
         Assert.AreEqual(630, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(59, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(10, unitInfoData.FramesObserved);
         Assert.AreEqual(33, unitInfoData.CurrentFrame.ID);
         Assert.AreEqual(660000, unitInfoData.CurrentFrame.RawFramesComplete);
         Assert.AreEqual(2000000, unitInfoData.CurrentFrame.RawFramesTotal);
         Assert.AreEqual(new TimeSpan(4, 46, 8), unitInfoData.CurrentFrame.TimeStamp);
         Assert.AreEqual(new TimeSpan(0, 8, 31), unitInfoData.CurrentFrame.Duration);
         Assert.AreEqual("A4", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void Client_v7_10_0_UnitDataOnly()
      {
         const int slotId = 0;
         _dataAggregator.ClientName = "Client_v7_10";

         var fahLog = new FahClientLog();
         string filteredLogText = String.Join(Environment.NewLine, File.ReadLines("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt").Where(x => x.Length != 0).Take(82));
         using (var textReader = new StringReader(filteredLogText))
         using (var reader = new FahClientLogTextReader(textReader))
         {
            fahLog.Read(reader);
         }

         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\info.txt");
         var info = new Info();
         info.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\options.txt");
         var options = new Options();
         options.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         var result = _dataAggregator.AggregateData(fahLog.ClientRuns.Last(), unitCollection, info, options, slotOptions, new UnitInfo(), slotId);
         Assert.AreEqual(1, result.UnitInfos.Count);
         Assert.IsFalse(result.UnitInfos.Any(x => x.Value == null));

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(2012, 1, 11, 3, 24, 22), result.StartTime);
         Assert.AreEqual(null, result.Arguments);
         Assert.AreEqual(null, result.ClientVersion);
         Assert.AreEqual(null, result.UserID);
         Assert.AreEqual(0, result.MachineID);
         Assert.AreEqual(SlotStatus.Unknown, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsTrue(result.UnitInfos.All(x => x.Value.LogLines == null));
         if (result.UnitInfos.ContainsKey(result.CurrentUnitIndex))
         {
            Assert.AreEqual(result.CurrentLogLines, LogLineEnumerable.Create(fahLog.ClientRuns.Last()));
         }

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), unitInfoData.DueTime);
         Assert.AreEqual(TimeSpan.Zero, unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(0, unitInfoData.CoreVersion);
         Assert.AreEqual(7610, unitInfoData.ProjectID);
         Assert.AreEqual(630, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(59, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(0, unitInfoData.FramesObserved);
         Assert.IsNull(unitInfoData.CurrentFrame);
         Assert.AreEqual("A4", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void Client_v7_10_1()
      {
         const int slotId = 1;
         _dataAggregator.ClientName = "Client_v7_10";

         var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt");

         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\info.txt");
         var info = new Info();
         info.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\options.txt");
         var options = new Options();
         options.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options2.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         var result = _dataAggregator.AggregateData(fahLog.ClientRuns.Last(), unitCollection, info, options, slotOptions, new UnitInfo(), slotId);
         Assert.AreEqual(1, result.UnitInfos.Count);
         Assert.IsFalse(result.UnitInfos.Any(x => x.Value == null));

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(2, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(2012, 1, 11, 3, 24, 22), result.StartTime);
         Assert.AreEqual(null, result.Arguments);
         Assert.AreEqual(null, result.ClientVersion);
         Assert.AreEqual(null, result.UserID);
         Assert.AreEqual(0, result.MachineID);
         Assert.AreEqual(SlotStatus.Unknown, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsFalse(result.UnitInfos.Any(x => x.Value.LogLines == null));
         if (result.UnitInfos.ContainsKey(result.CurrentUnitIndex))
         {
            Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);
         }

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2012, 1, 11, 4, 21, 14), unitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(4, 21, 52), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(1.31f, unitInfoData.CoreVersion);
         Assert.AreEqual(5772, unitInfoData.ProjectID);
         Assert.AreEqual(7, unitInfoData.ProjectRun);
         Assert.AreEqual(364, unitInfoData.ProjectClone);
         Assert.AreEqual(252, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(53, unitInfoData.FramesObserved);
         Assert.AreEqual(53, unitInfoData.CurrentFrame.ID);
         Assert.AreEqual(53, unitInfoData.CurrentFrame.RawFramesComplete);
         Assert.AreEqual(100, unitInfoData.CurrentFrame.RawFramesTotal);
         Assert.AreEqual(new TimeSpan(4, 51, 53), unitInfoData.CurrentFrame.TimeStamp);
         Assert.AreEqual(new TimeSpan(0, 0, 42), unitInfoData.CurrentFrame.Duration);
         Assert.AreEqual("11", unitInfoData.CoreID);
         #endregion
      }

      [Test]
      public void Client_v7_11_0()
      {
         const int slotId = 0;
         _dataAggregator.ClientName = "Client_v7_11";

         var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_11\\log.txt");

         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\units.txt");
         var unitCollection = new UnitCollection();
         unitCollection.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\info.txt");
         var info = new Info();
         info.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\options.txt");
         var options = new Options();
         options.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\slot-options1.txt");
         var slotOptions = new SlotOptions();
         slotOptions.Fill(JsonMessageConnection.GetNextJsonMessage(ref message));

         var result = _dataAggregator.AggregateData(fahLog.ClientRuns.Last(), unitCollection, info, options, slotOptions, new UnitInfo(), slotId);
         Assert.AreEqual(1, result.UnitInfos.Count);
         Assert.IsFalse(result.UnitInfos.Any(x => x.Value == null));

         #region Check Data Aggregator

         Assert.IsNotNull(result.Queue);
         Assert.AreEqual(1, result.CurrentUnitIndex);
         Assert.AreEqual(new DateTime(2012, 2, 18, 6, 33, 41), result.StartTime);
         Assert.AreEqual(null, result.Arguments);
         Assert.AreEqual(null, result.ClientVersion);
         Assert.AreEqual(null, result.UserID);
         Assert.AreEqual(0, result.MachineID);
         Assert.AreEqual(SlotStatus.Unknown, result.Status);
         Assert.IsNotNull(result.CurrentLogLines);
         Assert.IsFalse(result.UnitInfos.Any(x => x.Value.LogLines == null));
         if (result.UnitInfos.ContainsKey(result.CurrentUnitIndex))
         {
            Assert.AreEqual(result.CurrentLogLines, result.UnitInfos[result.CurrentUnitIndex].LogLines);
         }

         #endregion

         var unitInfoData = result.UnitInfos[result.CurrentUnitIndex];

         #region Check Unit Info Data Values
         Assert.AreEqual(null, unitInfoData.OwningSlotName);
         Assert.AreEqual(null, unitInfoData.OwningClientName);
         Assert.AreEqual(null, unitInfoData.OwningClientPath);
         Assert.AreEqual(-1, unitInfoData.OwningSlotId);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
         Assert.AreEqual("harlam357", unitInfoData.FoldingID);
         Assert.AreEqual(32, unitInfoData.Team);
         Assert.AreEqual(SlotType.CPU, unitInfoData.SlotType);
         Assert.AreEqual(new DateTime(2012, 2, 17, 21, 48, 22), unitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2012, 2, 29, 14, 50, 46), unitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(6, 34, 38), unitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, unitInfoData.FinishedTime);
         Assert.AreEqual(2.27f, unitInfoData.CoreVersion);
         Assert.AreEqual(7610, unitInfoData.ProjectID);
         Assert.AreEqual(192, unitInfoData.ProjectRun);
         Assert.AreEqual(0, unitInfoData.ProjectClone);
         Assert.AreEqual(58, unitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, unitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
         Assert.AreEqual(3, unitInfoData.FramesObserved);
         Assert.AreEqual(95, unitInfoData.CurrentFrame.ID);
         Assert.AreEqual(1900000, unitInfoData.CurrentFrame.RawFramesComplete);
         Assert.AreEqual(2000000, unitInfoData.CurrentFrame.RawFramesTotal);
         Assert.AreEqual(new TimeSpan(6, 46, 16), unitInfoData.CurrentFrame.TimeStamp);
         Assert.AreEqual(new TimeSpan(0, 4, 50), unitInfoData.CurrentFrame.Duration);
         Assert.AreEqual("A4", unitInfoData.CoreID);
         #endregion
      }

      // ReSharper restore InconsistentNaming
   }
}
