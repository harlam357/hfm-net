using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Core.WorkUnits;
using HFM.Log;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class WorkUnitCollectionBuilderTests
    {
        private static readonly DateTime _UnitRetrievalTime = new DateTime(2020, 1, 1);

        [Test]
        public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_0()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_10", @"..\..\..\..\TestFiles\Client_v7_10");
            var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages.UnitCollection, fahClient.Messages.Options, fahClient.Messages.GetClientRun(), _UnitRetrievalTime);

            // Act
            var workUnits = builder.BuildForSlot(0, new WorkUnit());

            // Assert - AggregatorResult
            Assert.IsNotNull(workUnits);
            Assert.AreEqual(1, workUnits.CurrentID);
            Assert.AreEqual(1, workUnits.Count);
            Assert.IsFalse(workUnits.Any(x => x == null));
            Assert.IsFalse(workUnits.Any(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = workUnits.Current;

            Assert.AreEqual(_UnitRetrievalTime, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), workUnit.Assigned);
            Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), workUnit.Timeout);
            Assert.AreEqual(new TimeSpan(3, 25, 32), workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(new Version(2, 27), workUnit.CoreVersion);
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
        public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_0_ClientDataOnly()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_10", @"..\..\..\..\TestFiles\Client_v7_10");
            // clear the log data so this test operates only on data provided by FahClient
            fahClient.Messages.Log.Clear();

            var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages.UnitCollection, fahClient.Messages.Options, fahClient.Messages.GetClientRun(), _UnitRetrievalTime);

            // Act
            var workUnits = builder.BuildForSlot(0, new WorkUnit());

            // Assert - AggregatorResult
            Assert.IsNotNull(workUnits);
            Assert.AreEqual(1, workUnits.CurrentID);
            Assert.AreEqual(1, workUnits.Count);
            Assert.IsFalse(workUnits.Any(x => x == null));
            Assert.IsTrue(workUnits.All(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = workUnits.Current;

            Assert.AreEqual(_UnitRetrievalTime, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 1, 10, 23, 20, 27), workUnit.Assigned);
            Assert.AreEqual(new DateTime(2012, 1, 22, 16, 22, 51), workUnit.Timeout);
            Assert.AreEqual(TimeSpan.Zero, workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(null, workUnit.CoreVersion);
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
        public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_1()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_10", @"..\..\..\..\TestFiles\Client_v7_10");
            var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages.UnitCollection, fahClient.Messages.Options, fahClient.Messages.GetClientRun(), _UnitRetrievalTime);

            // Act
            var workUnits = builder.BuildForSlot(1, new WorkUnit());

            // Assert - AggregatorResult
            Assert.IsNotNull(workUnits);
            Assert.AreEqual(2, workUnits.CurrentID);
            Assert.AreEqual(1, workUnits.Count);
            Assert.IsFalse(workUnits.Any(x => x == null));
            Assert.IsFalse(workUnits.Any(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = workUnits.Current;

            Assert.AreEqual(_UnitRetrievalTime, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 1, 11, 4, 21, 14), workUnit.Assigned);
            Assert.AreEqual(DateTime.MinValue, workUnit.Timeout);
            Assert.AreEqual(new TimeSpan(4, 21, 52), workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(new Version(1, 31), workUnit.CoreVersion);
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
        public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_1_CompletesPreviousUnit()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_10", @"..\..\..\..\TestFiles\Client_v7_10");
            var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages.UnitCollection, fahClient.Messages.Options, fahClient.Messages.GetClientRun(), _UnitRetrievalTime);

            // Act
            var workUnits = builder.BuildForSlot(1, new WorkUnit { ID = 0, ProjectID = 5767, ProjectRun = 3, ProjectClone = 138, ProjectGen = 144 });

            // Assert - AggregatorResult
            Assert.IsNotNull(workUnits);
            Assert.AreEqual(2, workUnits.CurrentID);
            Assert.AreEqual(2, workUnits.Count);
            Assert.IsFalse(workUnits.Any(x => x == null));
            Assert.IsFalse(workUnits.Any(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = workUnits[0];

            Assert.AreEqual(_UnitRetrievalTime, workUnit.UnitRetrievalTime);
            Assert.AreEqual(null, workUnit.FoldingID);
            Assert.AreEqual(0, workUnit.Team);
            Assert.AreEqual(DateTime.MinValue, workUnit.Assigned);
            Assert.AreEqual(DateTime.MinValue, workUnit.Timeout);
            Assert.AreEqual(new TimeSpan(3, 25, 36), workUnit.UnitStartTimeStamp);
            Assert.AreNotEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(new Version(1, 31), workUnit.CoreVersion);
            Assert.AreEqual(5767, workUnit.ProjectID);
            Assert.AreEqual(3, workUnit.ProjectRun);
            Assert.AreEqual(138, workUnit.ProjectClone);
            Assert.AreEqual(144, workUnit.ProjectGen);
            Assert.AreEqual(WorkUnitResult.FinishedUnit, workUnit.UnitResult);
            Assert.AreEqual(100, workUnit.FramesObserved);
            Assert.AreEqual(100, workUnit.CurrentFrame.ID);
            Assert.AreEqual(100, workUnit.CurrentFrame.RawFramesComplete);
            Assert.AreEqual(100, workUnit.CurrentFrame.RawFramesTotal);
            Assert.AreEqual(new TimeSpan(4, 21, 39), workUnit.CurrentFrame.TimeStamp);
            Assert.AreEqual(new TimeSpan(0, 0, 33), workUnit.CurrentFrame.Duration);
            Assert.AreEqual(186, workUnit.LogLines.Count);
            Assert.AreEqual(null, workUnit.CoreID);
        }

        [Test]
        public async Task WorkUnitCollectionBuilder_Client_v7_11_SlotID_0()
        {
            // Arrange
            var fahClient = await CreateClientWithMessagesLoadedFrom("Client_v7_11", @"..\..\..\..\TestFiles\Client_v7_11");
            var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages.UnitCollection, fahClient.Messages.Options, fahClient.Messages.GetClientRun(), _UnitRetrievalTime);

            // Act
            var workUnits = builder.BuildForSlot(0, new WorkUnit());

            // Assert
            Assert.IsNotNull(workUnits);
            Assert.AreEqual(1, workUnits.CurrentID);
            Assert.AreEqual(1, workUnits.Count);
            Assert.IsFalse(workUnits.Any(x => x == null));
            Assert.IsFalse(workUnits.Any(x => x.LogLines == null));

            // Assert - Work Unit
            var workUnit = workUnits.Current;

            Assert.AreEqual(_UnitRetrievalTime, workUnit.UnitRetrievalTime);
            Assert.AreEqual("harlam357", workUnit.FoldingID);
            Assert.AreEqual(32, workUnit.Team);
            Assert.AreEqual(new DateTime(2012, 2, 17, 21, 48, 22), workUnit.Assigned);
            Assert.AreEqual(new DateTime(2012, 2, 29, 14, 50, 46), workUnit.Timeout);
            Assert.AreEqual(new TimeSpan(6, 34, 38), workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(new Version(2, 27), workUnit.CoreVersion);
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

        [Test]
        public void WorkUnitCollectionBuilder_BuildForSlot_SetsCurrentIDForRunningWorkUnit()
        {
            // Arrange
            var units = new UnitCollection
            {
                new Unit { Slot = 0, ID = 0, State = "READY" },
                new Unit { Slot = 0, ID = 1, State = "RUNNING" }
            };
            var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, units, new Options(), null, DateTime.MinValue);
            // Act
            var workUnits = builder.BuildForSlot(0, new WorkUnit());
            // Assert
            Assert.AreEqual(1, workUnits.CurrentID);
        }

        [Test]
        public void WorkUnitCollectionBuilder_BuildForSlot_SetsCurrentIDForFirstReadyWorkUnit()
        {
            // Arrange
            var units = new UnitCollection
            {
                new Unit { Slot = 0, ID = 2, State = "READY" },
                new Unit { Slot = 0, ID = 1, State = "READY" }
            };
            var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, units, new Options(), null, DateTime.MinValue);
            // Act
            var workUnits = builder.BuildForSlot(0, new WorkUnit());
            // Assert
            Assert.AreEqual(2, workUnits.CurrentID);
        }

        [Test]
        public void WorkUnitCollectionBuilder_BuildForSlot_BuildsWorkUnitFromUnitCollection_PreviousUnitExistsInUnitCollection()
        {
            // Arrange
            var units = new UnitCollection
            {
                new Unit { Slot = 0, ID = 0, State = "READY", Project = 1 },
                new Unit { Slot = 0, ID = 1, State = "RUNNING", Project = 2 }
            };
            var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, units, new Options(), null, DateTime.MinValue);
            // Act
            var workUnits = builder.BuildForSlot(0, new WorkUnit { ID = 0, ProjectID = 2 });
            // Assert
            Assert.AreEqual(2, workUnits.Count);
        }

        [Test]
        public void WorkUnitCollectionBuilder_BuildForSlot_BuildsWorkUnitFromUnitCollection_PreviousUnitIDExistsInUnitCollection()
        {
            // Arrange
            var units = new UnitCollection
            {
                new Unit { Slot = 0, ID = 0, State = "READY", Project = 1 },
                new Unit { Slot = 0, ID = 1, State = "RUNNING", Project = 2 }
            };
            var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, units, new Options(), null, DateTime.MinValue);
            // Act
            var workUnits = builder.BuildForSlot(0, new WorkUnit { ID = 0, ProjectID = 3 });
            // Assert
            Assert.AreEqual(2, workUnits.Count);
        }

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
