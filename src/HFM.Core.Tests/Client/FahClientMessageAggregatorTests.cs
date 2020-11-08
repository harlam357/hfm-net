using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Core.WorkUnits;
using HFM.Log;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientMessageAggregatorTests
    {
        // ReSharper disable InconsistentNaming

        [Test]
        public async Task FahClientMessageAggregator_Client_v7_10_SlotID_0()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_10", @"..\..\..\..\TestFiles\Client_v7_10");
            var aggregator = new FahClientMessageAggregator(fahClient);

            // Act
            var result = aggregator.AggregateData(0, new WorkUnit(), "foo");

            // Assert - AggregatorResult
            Assert.IsNotNull(result.WorkUnitQueue);
            Assert.AreEqual(1, result.WorkUnitQueue.CurrentID);

            Assert.IsNotNull(result.WorkUnits);
            Assert.AreEqual(1, result.WorkUnits.CurrentID);
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x == null));
            Assert.IsFalse(result.WorkUnits.Any(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = result.WorkUnits.Current;

            Assert.AreEqual(DateTime.MinValue, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), workUnit.Assigned);
            Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), workUnit.Timeout);
            Assert.AreEqual(new TimeSpan(3, 25, 32), workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(2.27f, workUnit.CoreVersion);
            Assert.AreEqual(7610, workUnit.ProjectID);
            Assert.AreEqual(630, workUnit.ProjectRun);
            Assert.AreEqual(0, workUnit.ProjectClone);
            Assert.AreEqual(59, workUnit.ProjectGen);
            Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
            Assert.AreEqual(10, workUnit.FramesObserved);
            Assert.AreEqual(33, workUnit.CurrentFrame.ID);
            Assert.AreEqual(660000, workUnit.CurrentFrame.RawFramesComplete);
            Assert.AreEqual(2000000, workUnit.CurrentFrame.RawFramesTotal);
            Assert.AreEqual(new TimeSpan(4, 46, 8), workUnit.CurrentFrame.TimeStamp);
            Assert.AreEqual(new TimeSpan(0, 8, 31), workUnit.CurrentFrame.Duration);
            Assert.AreEqual(39, workUnit.LogLines.Count);
            Assert.AreEqual("A4", workUnit.CoreID);
        }

        [Test]
        public async Task FahClientMessageAggregator_Client_v7_10_SlotID_0_UnitDataOnly()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_10", @"..\..\..\..\TestFiles\Client_v7_10");

            fahClient.Messages.Log.Clear();
            string filteredLogText = String.Join(Environment.NewLine, File.ReadLines(@"..\..\..\..\TestFiles\Client_v7_10\log.txt").Where(x => x.Length != 0).Take(82));
            using (var textReader = new StringReader(filteredLogText))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                await fahClient.Messages.Log.ReadAsync(reader);
            }

            var aggregator = new FahClientMessageAggregator(fahClient);

            // Act
            var result = aggregator.AggregateData(0, new WorkUnit(), "foo");

            // Assert - AggregatorResult
            Assert.IsNotNull(result.WorkUnitQueue);
            Assert.AreEqual(1, result.WorkUnitQueue.CurrentID);

            Assert.IsNotNull(result.WorkUnits);
            Assert.AreEqual(1, result.WorkUnits.CurrentID);
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x == null));
            Assert.IsTrue(result.WorkUnits.All(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = result.WorkUnits.Current;

            Assert.AreEqual(DateTime.MinValue, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), workUnit.Assigned);
            Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), workUnit.Timeout);
            Assert.AreEqual(TimeSpan.Zero, workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(0, workUnit.CoreVersion);
            Assert.AreEqual(7610, workUnit.ProjectID);
            Assert.AreEqual(630, workUnit.ProjectRun);
            Assert.AreEqual(0, workUnit.ProjectClone);
            Assert.AreEqual(59, workUnit.ProjectGen);
            Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
            Assert.AreEqual(0, workUnit.FramesObserved);
            Assert.IsNull(workUnit.CurrentFrame);
            Assert.IsNull(workUnit.LogLines);
            Assert.AreEqual("A4", workUnit.CoreID);
        }

        [Test]
        public async Task FahClientMessageAggregator_Client_v7_10_SlotID_1()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_10", @"..\..\..\..\TestFiles\Client_v7_10");
            var aggregator = new FahClientMessageAggregator(fahClient);

            // Act
            var result = aggregator.AggregateData(1, new WorkUnit(), "foo");

            // Assert - AggregatorResult
            Assert.IsNotNull(result.WorkUnitQueue);
            Assert.AreEqual(2, result.WorkUnitQueue.CurrentID);

            Assert.IsNotNull(result.WorkUnits);
            Assert.AreEqual(2, result.WorkUnits.CurrentID);
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x == null));
            Assert.IsFalse(result.WorkUnits.Any(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = result.WorkUnits.Current;

            Assert.AreEqual(DateTime.MinValue, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 1, 11, 4, 21, 14), workUnit.Assigned);
            Assert.AreEqual(DateTime.MinValue, workUnit.Timeout);
            Assert.AreEqual(new TimeSpan(4, 21, 52), workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(1.31f, workUnit.CoreVersion);
            Assert.AreEqual(5772, workUnit.ProjectID);
            Assert.AreEqual(7, workUnit.ProjectRun);
            Assert.AreEqual(364, workUnit.ProjectClone);
            Assert.AreEqual(252, workUnit.ProjectGen);
            Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
            Assert.AreEqual(53, workUnit.FramesObserved);
            Assert.AreEqual(53, workUnit.CurrentFrame.ID);
            Assert.AreEqual(53, workUnit.CurrentFrame.RawFramesComplete);
            Assert.AreEqual(100, workUnit.CurrentFrame.RawFramesTotal);
            Assert.AreEqual(new TimeSpan(4, 51, 53), workUnit.CurrentFrame.TimeStamp);
            Assert.AreEqual(new TimeSpan(0, 0, 42), workUnit.CurrentFrame.Duration);
            Assert.AreEqual(98, workUnit.LogLines.Count);
            Assert.AreEqual("11", workUnit.CoreID);
        }

        [Test]
        public async Task FahClientMessageAggregator_Client_v7_11_SlotID_0()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_11", @"..\..\..\..\TestFiles\Client_v7_11");
            var aggregator = new FahClientMessageAggregator(fahClient);

            // Act
            var result = aggregator.AggregateData(0, new WorkUnit(), "foo");

            // Assert
            Assert.IsNotNull(result.WorkUnitQueue);
            Assert.AreEqual(1, result.WorkUnitQueue.CurrentID);

            Assert.IsNotNull(result.WorkUnits);
            Assert.AreEqual(1, result.WorkUnits.CurrentID);
            Assert.AreEqual(1, result.WorkUnits.Count);
            Assert.IsFalse(result.WorkUnits.Any(x => x == null));
            Assert.IsFalse(result.WorkUnits.Any(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = result.WorkUnits.Current;

            Assert.AreEqual(DateTime.MinValue, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 2, 17, 21, 48, 22), workUnit.Assigned);
            Assert.AreEqual(new DateTime(2012, 2, 29, 14, 50, 46), workUnit.Timeout);
            Assert.AreEqual(new TimeSpan(6, 34, 38), workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(2.27f, workUnit.CoreVersion);
            Assert.AreEqual(7610, workUnit.ProjectID);
            Assert.AreEqual(192, workUnit.ProjectRun);
            Assert.AreEqual(0, workUnit.ProjectClone);
            Assert.AreEqual(58, workUnit.ProjectGen);
            Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
            Assert.AreEqual(3, workUnit.FramesObserved);
            Assert.AreEqual(95, workUnit.CurrentFrame.ID);
            Assert.AreEqual(1900000, workUnit.CurrentFrame.RawFramesComplete);
            Assert.AreEqual(2000000, workUnit.CurrentFrame.RawFramesTotal);
            Assert.AreEqual(new TimeSpan(6, 46, 16), workUnit.CurrentFrame.TimeStamp);
            Assert.AreEqual(new TimeSpan(0, 4, 50), workUnit.CurrentFrame.Duration);
            Assert.AreEqual(32, workUnit.LogLines.Count);
            Assert.AreEqual("A4", workUnit.CoreID);
        }

        // ReSharper restore InconsistentNaming

        private static async Task<FahClient> CreateClientWithMessagesLoadedFrom(string clientName, string path)
        {
            var fahClient = CreateClient(clientName);
            await LoadMessagesFrom(fahClient, path);
            return fahClient;
        }

        private static FahClient CreateClient(string clientName)
        {
            var client = new FahClient(null, null, null, null, null);
            client.Settings = new ClientSettings { Name = clientName };
            return client;
        }

        private static async Task LoadMessagesFrom(FahClient fahClient, string path)
        {
            var extractor = new FahClientJsonMessageExtractor();

            foreach (var file in Directory.EnumerateFiles(path))
            {
                if (Path.GetFileName(file) == "log.txt")
                {
                    using (var textReader = new StreamReader(file))
                    using (var reader = new FahClientLogTextReader(textReader))
                    {
                        await fahClient.Messages.Log.ReadAsync(reader);
                    }
                }
                else
                {
                    await fahClient.Messages.UpdateMessageAsync(extractor.Extract(new StringBuilder(File.ReadAllText(file))));
                }
            }
        }
    }
}
