
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using HFM.Client;
using HFM.Core.WorkUnits;
using HFM.Log;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientMessageAggregatorTests
    {
        // ReSharper disable InconsistentNaming

        private static FahClient CreateClient(ClientSettings settings)
        {
            var client = new FahClient(null, null, null, null, null);
            client.Settings = settings;
            return client;
        }

        [Test]
        public async Task FahClientMessageAggregator_Client_v7_10_SlotID_0()
        {
            // Arrange
            var settings = new ClientSettings { Name = "Client_v7_10" };
            var fahClient = CreateClient(settings);

            using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                await fahClient.Messages.Log.ReadAsync(reader);
            }
            
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\units.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\info.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\options.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slots.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options1.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options2.txt"))));

            var slotModel = new SlotModel(fahClient) { SlotID = 0 };
            var aggregator = new FahClientMessageAggregator(fahClient, slotModel);

            // Act
            var result = aggregator.AggregateData();

            // Assert
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x.Value == null));

            #region Check Data Aggregator

            Assert.IsNotNull(result.WorkUnitInfos);
            Assert.AreEqual(1, result.CurrentUnitIndex);
            Assert.AreEqual(new DateTime(2012, 1, 11, 3, 24, 22), result.StartTime);
            Assert.AreEqual(null, result.Arguments);
            Assert.AreEqual(null, result.ClientVersion);
            Assert.AreEqual(null, result.UserID);
            Assert.AreEqual(0, result.MachineID);
            Assert.AreEqual(SlotStatus.Unknown, result.Status);
            Assert.IsNotNull(result.CurrentLogLines);
            Assert.IsFalse(result.WorkUnits.Any(x => x.Value.LogLines == null));
            if (result.WorkUnits.ContainsKey(result.CurrentUnitIndex))
            {
                Assert.AreEqual(result.CurrentLogLines, result.WorkUnits[result.CurrentUnitIndex].LogLines);
            }

            #endregion

            var unitInfoData = result.WorkUnits[result.CurrentUnitIndex];

            #region Check Unit Info Data Values
            Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
            Assert.AreEqual("harlam357", unitInfoData.FoldingID);
            Assert.AreEqual(32, unitInfoData.Team);
            Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), unitInfoData.Assigned);
            Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), unitInfoData.Timeout);
            Assert.AreEqual(new TimeSpan(3, 25, 32), unitInfoData.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, unitInfoData.Finished);
            Assert.AreEqual(2.27f, unitInfoData.CoreVersion);
            Assert.AreEqual(7610, unitInfoData.ProjectID);
            Assert.AreEqual(630, unitInfoData.ProjectRun);
            Assert.AreEqual(0, unitInfoData.ProjectClone);
            Assert.AreEqual(59, unitInfoData.ProjectGen);
            Assert.AreEqual(null, unitInfoData.ProteinName);
            Assert.AreEqual(null, unitInfoData.ProteinTag);
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
        public async Task FahClientMessageAggregator_Client_v7_10_SlotID_0_UnitDataOnly()
        {
            // Arrange
            var settings = new ClientSettings { Name = "Client_v7_10" };
            var fahClient = CreateClient(settings);

            string filteredLogText = String.Join(Environment.NewLine, File.ReadLines("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt").Where(x => x.Length != 0).Take(82));
            using (var textReader = new StringReader(filteredLogText))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                await fahClient.Messages.Log.ReadAsync(reader);
            }
            
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\units.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\info.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\options.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slots.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options1.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options2.txt"))));

            var slotModel = new SlotModel(fahClient) { SlotID = 0 };
            var aggregator = new FahClientMessageAggregator(fahClient, slotModel);

            // Act
            var result = aggregator.AggregateData();
            
            // Assert
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x.Value == null));

            #region Check Data Aggregator

            Assert.IsNotNull(result.WorkUnitInfos);
            Assert.AreEqual(1, result.CurrentUnitIndex);
            Assert.AreEqual(new DateTime(2012, 1, 11, 3, 24, 22), result.StartTime);
            Assert.AreEqual(null, result.Arguments);
            Assert.AreEqual(null, result.ClientVersion);
            Assert.AreEqual(null, result.UserID);
            Assert.AreEqual(0, result.MachineID);
            Assert.AreEqual(SlotStatus.Unknown, result.Status);
            Assert.IsNotNull(result.CurrentLogLines);
            Assert.IsTrue(result.WorkUnits.All(x => x.Value.LogLines == null));
            if (result.WorkUnits.ContainsKey(result.CurrentUnitIndex))
            {
                Assert.AreEqual(result.CurrentLogLines, LogLineEnumerable.Create(fahClient.Messages.Log.ClientRuns.Last()));
            }

            #endregion

            var unitInfoData = result.WorkUnits[result.CurrentUnitIndex];

            #region Check Unit Info Data Values
            Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
            Assert.AreEqual("harlam357", unitInfoData.FoldingID);
            Assert.AreEqual(32, unitInfoData.Team);
            Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), unitInfoData.Assigned);
            Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), unitInfoData.Timeout);
            Assert.AreEqual(TimeSpan.Zero, unitInfoData.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, unitInfoData.Finished);
            Assert.AreEqual(0, unitInfoData.CoreVersion);
            Assert.AreEqual(7610, unitInfoData.ProjectID);
            Assert.AreEqual(630, unitInfoData.ProjectRun);
            Assert.AreEqual(0, unitInfoData.ProjectClone);
            Assert.AreEqual(59, unitInfoData.ProjectGen);
            Assert.AreEqual(null, unitInfoData.ProteinName);
            Assert.AreEqual(null, unitInfoData.ProteinTag);
            Assert.AreEqual(WorkUnitResult.Unknown, unitInfoData.UnitResult);
            Assert.AreEqual(0, unitInfoData.FramesObserved);
            Assert.IsNull(unitInfoData.CurrentFrame);
            Assert.AreEqual("A4", unitInfoData.CoreID);
            #endregion
        }

        [Test]
        public async Task FahClientMessageAggregator_Client_v7_10_SlotID_1()
        {
            // Arrange
            var settings = new ClientSettings { Name = "Client_v7_10" };
            var fahClient = CreateClient(settings);

            using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                await fahClient.Messages.Log.ReadAsync(reader);
            }
            
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\units.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\info.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\options.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slots.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options1.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slot-options2.txt"))));

            var slotModel = new SlotModel(fahClient) { SlotID = 1 };
            var aggregator = new FahClientMessageAggregator(fahClient, slotModel);

            // Act
            var result = aggregator.AggregateData();

            // Assert
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x.Value == null));

            #region Check Data Aggregator

            Assert.IsNotNull(result.WorkUnitInfos);
            Assert.AreEqual(2, result.CurrentUnitIndex);
            Assert.AreEqual(new DateTime(2012, 1, 11, 3, 24, 22), result.StartTime);
            Assert.AreEqual(null, result.Arguments);
            Assert.AreEqual(null, result.ClientVersion);
            Assert.AreEqual(null, result.UserID);
            Assert.AreEqual(0, result.MachineID);
            Assert.AreEqual(SlotStatus.Unknown, result.Status);
            Assert.IsNotNull(result.CurrentLogLines);
            Assert.IsFalse(result.WorkUnits.Any(x => x.Value.LogLines == null));
            if (result.WorkUnits.ContainsKey(result.CurrentUnitIndex))
            {
                Assert.AreEqual(result.CurrentLogLines, result.WorkUnits[result.CurrentUnitIndex].LogLines);
            }

            #endregion

            var unitInfoData = result.WorkUnits[result.CurrentUnitIndex];

            #region Check Unit Info Data Values
            Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
            Assert.AreEqual("harlam357", unitInfoData.FoldingID);
            Assert.AreEqual(32, unitInfoData.Team);
            Assert.AreEqual(new DateTime(2012, 1, 11, 4, 21, 14), unitInfoData.Assigned);
            Assert.AreEqual(DateTime.MinValue, unitInfoData.Timeout);
            Assert.AreEqual(new TimeSpan(4, 21, 52), unitInfoData.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, unitInfoData.Finished);
            Assert.AreEqual(1.31f, unitInfoData.CoreVersion);
            Assert.AreEqual(5772, unitInfoData.ProjectID);
            Assert.AreEqual(7, unitInfoData.ProjectRun);
            Assert.AreEqual(364, unitInfoData.ProjectClone);
            Assert.AreEqual(252, unitInfoData.ProjectGen);
            Assert.AreEqual(null, unitInfoData.ProteinName);
            Assert.AreEqual(null, unitInfoData.ProteinTag);
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
        public async Task FahClientMessageAggregator_Client_v7_11_SlotID_0()
        {
            // Arrange
            var settings = new ClientSettings { Name = "Client_v7_11" };
            var fahClient = CreateClient(settings);

            using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_11\\log.txt"))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                await fahClient.Messages.Log.ReadAsync(reader);
            }
            
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\units.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\info.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\options.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\slots.txt"))));

            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_11\\slot-options1.txt"))));

            var slotModel = new SlotModel(fahClient) { SlotID = 0 };
            var aggregator = new FahClientMessageAggregator(fahClient, slotModel);

            // Act
            var result = aggregator.AggregateData();

            // Assert
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x.Value == null));

            #region Check Data Aggregator

            Assert.IsNotNull(result.WorkUnitInfos);
            Assert.AreEqual(1, result.CurrentUnitIndex);
            Assert.AreEqual(new DateTime(2012, 2, 18, 6, 33, 41), result.StartTime);
            Assert.AreEqual(null, result.Arguments);
            Assert.AreEqual(null, result.ClientVersion);
            Assert.AreEqual(null, result.UserID);
            Assert.AreEqual(0, result.MachineID);
            Assert.AreEqual(SlotStatus.Unknown, result.Status);
            Assert.IsNotNull(result.CurrentLogLines);
            Assert.IsFalse(result.WorkUnits.Any(x => x.Value.LogLines == null));
            if (result.WorkUnits.ContainsKey(result.CurrentUnitIndex))
            {
                Assert.AreEqual(result.CurrentLogLines, result.WorkUnits[result.CurrentUnitIndex].LogLines);
            }

            #endregion

            var unitInfoData = result.WorkUnits[result.CurrentUnitIndex];

            #region Check Unit Info Data Values
            Assert.AreEqual(DateTime.MinValue, unitInfoData.UnitRetrievalTime);
            Assert.AreEqual("harlam357", unitInfoData.FoldingID);
            Assert.AreEqual(32, unitInfoData.Team);
            Assert.AreEqual(new DateTime(2012, 2, 17, 21, 48, 22), unitInfoData.Assigned);
            Assert.AreEqual(new DateTime(2012, 2, 29, 14, 50, 46), unitInfoData.Timeout);
            Assert.AreEqual(new TimeSpan(6, 34, 38), unitInfoData.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, unitInfoData.Finished);
            Assert.AreEqual(2.27f, unitInfoData.CoreVersion);
            Assert.AreEqual(7610, unitInfoData.ProjectID);
            Assert.AreEqual(192, unitInfoData.ProjectRun);
            Assert.AreEqual(0, unitInfoData.ProjectClone);
            Assert.AreEqual(58, unitInfoData.ProjectGen);
            Assert.AreEqual(null, unitInfoData.ProteinName);
            Assert.AreEqual(null, unitInfoData.ProteinTag);
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
