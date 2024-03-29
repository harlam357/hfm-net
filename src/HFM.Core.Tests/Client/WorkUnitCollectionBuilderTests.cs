﻿using HFM.Client.ObjectModel;
using HFM.Core.Client.Mocks;
using HFM.Core.WorkUnits;
using HFM.Log;

using NUnit.Framework;

namespace HFM.Core.Client;

[TestFixture]
public class WorkUnitCollectionBuilderTests
{
    private static readonly DateTime _UnitRetrievalTime = new(2020, 1, 1);

    [Test]
    public void WorkUnitCollectionBuilder_BuildForSlot_ReturnsEmptyCollectionWhenUnitCollectionIsNull()
    {
        // Arrange
        var builder = new WorkUnitCollectionBuilder(null, new ClientSettings(), new MessageSource(), _UnitRetrievalTime);
        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), null);
        // Assert
        Assert.AreEqual(0, workUnits.Count);
    }

    [Test]
    public async Task WorkUnitCollectionBuilder_BuildForSlot_DoesNotPopulateFoldingIDAndTeamWhenOptionsIsNull()
    {
        // Arrange
        var fahClient = MockFahClient.Create("Client_v7_10");
        await fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_10");
        var source = new MessageSource
        {
            UnitCollection = fahClient.Messages.UnitCollection,
            ClientRun = fahClient.Messages.ClientRun
        };
        var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, source, _UnitRetrievalTime);
        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit());
        // Assert
        var workUnit = workUnits.Current;
        Assert.AreEqual(Unknown.Value, workUnit.FoldingID);
        Assert.AreEqual(0, workUnit.Team);
    }

    [Test]
    public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_0()
    {
        // Arrange
        var fahClient = MockFahClient.Create("Client_v7_10");
        await fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_10");
        var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages, _UnitRetrievalTime);

        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit());

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
        Assert.AreEqual(new WorkUnitPlatform(WorkUnitPlatformImplementation.CPU), workUnit.Platform);
        Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
        Assert.AreEqual(10, workUnit.FramesObserved);
        Assert.AreEqual(33, workUnit.CurrentFrame.ID);
        Assert.AreEqual(660000, workUnit.CurrentFrame.RawFramesComplete);
        Assert.AreEqual(2000000, workUnit.CurrentFrame.RawFramesTotal);
        Assert.AreEqual(new TimeSpan(4, 46, 8), workUnit.CurrentFrame.TimeStamp);
        Assert.AreEqual(new TimeSpan(0, 8, 31), workUnit.CurrentFrame.Duration);
        Assert.AreEqual(39, workUnit.LogLines.Count);
        Assert.AreEqual("0xa4", workUnit.Core);
        Assert.AreEqual("0x00000050664f2dd04de6d4f93deb418d", workUnit.UnitID);
    }

    [Test]
    public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_0_ClientDataOnly()
    {
        // Arrange
        var fahClient = MockFahClient.Create("Client_v7_10");
        await fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_10");
        // clear the log data so this test operates only on data provided by FahClient
        fahClient.Messages.Log.Clear();

        var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages, _UnitRetrievalTime);

        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit());

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
        Assert.AreEqual(null, workUnit.Platform);
        Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
        Assert.AreEqual(0, workUnit.FramesObserved);
        Assert.IsNull(workUnit.CurrentFrame);
        Assert.IsNull(workUnit.LogLines);
        Assert.AreEqual("0xa4", workUnit.Core);
        Assert.AreEqual("0x00000050664f2dd04de6d4f93deb418d", workUnit.UnitID);
    }

    [Test]
    public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_1()
    {
        // Arrange
        var fahClient = MockFahClient.Create("Client_v7_10");
        await fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_10");
        var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages, _UnitRetrievalTime);

        // Act
        var workUnits = builder.BuildForSlot(1, new CPUSlotDescription(), new WorkUnit());

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
        Assert.AreEqual(new WorkUnitPlatform(WorkUnitPlatformImplementation.CPU), workUnit.Platform);
        Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
        Assert.AreEqual(53, workUnit.FramesObserved);
        Assert.AreEqual(53, workUnit.CurrentFrame.ID);
        Assert.AreEqual(53, workUnit.CurrentFrame.RawFramesComplete);
        Assert.AreEqual(100, workUnit.CurrentFrame.RawFramesTotal);
        Assert.AreEqual(new TimeSpan(4, 51, 53), workUnit.CurrentFrame.TimeStamp);
        Assert.AreEqual(new TimeSpan(0, 0, 42), workUnit.CurrentFrame.Duration);
        Assert.AreEqual(98, workUnit.LogLines.Count);
        Assert.AreEqual("0x11", workUnit.Core);
        Assert.AreEqual("0x241a68704f0d0e3a00fc016c0007168c", workUnit.UnitID);
    }

    [Test]
    public async Task WorkUnitCollectionBuilder_Client_v7_10_SlotID_1_CompletesPreviousUnit()
    {
        // Arrange
        var fahClient = MockFahClient.Create("Client_v7_10");
        await fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_10");
        var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages, _UnitRetrievalTime);

        // Act
        var workUnits = builder.BuildForSlot(1, new CPUSlotDescription(), new WorkUnit { ID = 0, ProjectID = 5767, ProjectRun = 3, ProjectClone = 138, ProjectGen = 144 });

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
        Assert.AreEqual(null, workUnit.Platform);
        Assert.AreEqual(WorkUnitResult.FinishedUnit, workUnit.UnitResult);
        Assert.AreEqual(100, workUnit.FramesObserved);
        Assert.AreEqual(100, workUnit.CurrentFrame.ID);
        Assert.AreEqual(100, workUnit.CurrentFrame.RawFramesComplete);
        Assert.AreEqual(100, workUnit.CurrentFrame.RawFramesTotal);
        Assert.AreEqual(new TimeSpan(4, 21, 39), workUnit.CurrentFrame.TimeStamp);
        Assert.AreEqual(new TimeSpan(0, 0, 33), workUnit.CurrentFrame.Duration);
        Assert.AreEqual(186, workUnit.LogLines.Count);
        Assert.AreEqual(null, workUnit.Core);
        Assert.AreEqual(null, workUnit.UnitID);
    }

    [Test]
    public async Task WorkUnitCollectionBuilder_Client_v7_11_SlotID_0()
    {
        // Arrange
        var fahClient = MockFahClient.Create("Client_v7_11");
        await fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_11");
        var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages, _UnitRetrievalTime);

        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit());

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
        Assert.AreEqual(new WorkUnitPlatform(WorkUnitPlatformImplementation.CPU), workUnit.Platform);
        Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
        Assert.AreEqual(3, workUnit.FramesObserved);
        Assert.AreEqual(95, workUnit.CurrentFrame.ID);
        Assert.AreEqual(1900000, workUnit.CurrentFrame.RawFramesComplete);
        Assert.AreEqual(2000000, workUnit.CurrentFrame.RawFramesTotal);
        Assert.AreEqual(new TimeSpan(6, 46, 16), workUnit.CurrentFrame.TimeStamp);
        Assert.AreEqual(new TimeSpan(0, 4, 50), workUnit.CurrentFrame.Duration);
        Assert.AreEqual(32, workUnit.LogLines.Count);
        Assert.AreEqual("0xa4", workUnit.Core);
        Assert.AreEqual("0x0000004e664f2dd04de6d35869ac2ae3", workUnit.UnitID);
    }

    [Test]
    public async Task WorkUnitCollectionBuilder_Client_v7_19_SlotID_1()
    {
        // Arrange
        var fahClient = MockFahClient.Create("Client_v7_19");
        await fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_19");
        var builder = new WorkUnitCollectionBuilder(null, fahClient.Settings, fahClient.Messages, _UnitRetrievalTime);

        // Act
        var workUnits = builder.BuildForSlot(1, new GPUSlotDescription { GPUBus = 13, GPUSlot = 0 }, new WorkUnit());

        // Assert
        Assert.IsNotNull(workUnits);
        Assert.AreEqual(0, workUnits.CurrentID);
        Assert.AreEqual(1, workUnits.Count);
        Assert.IsFalse(workUnits.Any(x => x == null));
        Assert.IsFalse(workUnits.Any(x => x.LogLines == null));

        // Assert - Work Unit
        var workUnit = workUnits.Current;

        Assert.AreEqual(_UnitRetrievalTime, workUnit.UnitRetrievalTime);
        Assert.AreEqual("harlam357", workUnit.FoldingID);
        Assert.AreEqual(32, workUnit.Team);
        Assert.AreEqual(new DateTime(2021, 9, 5, 17, 57, 5), workUnit.Assigned);
        Assert.AreEqual(new DateTime(2021, 9, 7, 17, 57, 5), workUnit.Timeout);
        Assert.AreEqual(new TimeSpan(9, 23, 36), workUnit.UnitStartTimeStamp);
        Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
        Assert.AreEqual(new Version(0, 0, 13), workUnit.CoreVersion);
        Assert.AreEqual(18201, workUnit.ProjectID);
        Assert.AreEqual(44695, workUnit.ProjectRun);
        Assert.AreEqual(3, workUnit.ProjectClone);
        Assert.AreEqual(2, workUnit.ProjectGen);
        Assert.AreEqual(new WorkUnitPlatform(WorkUnitPlatformImplementation.CUDA)
        {
            DriverVersion = "456.71",
            ComputeVersion = "7.5",
            CUDAVersion = "11.1"
        }, workUnit.Platform);
        Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
        Assert.AreEqual(75, workUnit.FramesObserved);
        Assert.AreEqual(74, workUnit.CurrentFrame.ID);
        Assert.AreEqual(925000, workUnit.CurrentFrame.RawFramesComplete);
        Assert.AreEqual(1250000, workUnit.CurrentFrame.RawFramesTotal);
        Assert.AreEqual(new TimeSpan(20, 53, 54), workUnit.CurrentFrame.TimeStamp);
        Assert.AreEqual(new TimeSpan(0, 2, 24), workUnit.CurrentFrame.Duration);
        Assert.AreEqual(181, workUnit.LogLines.Count);
        Assert.AreEqual("0x22", workUnit.Core);
        Assert.AreEqual("0x0000000300000002000047190000ae97", workUnit.UnitID);
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
        var source = new MessageSource
        {
            UnitCollection = units,
            Options = new Options()
        };
        var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, source, DateTime.MinValue);
        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit());
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
        var source = new MessageSource
        {
            UnitCollection = units,
            Options = new Options()
        };
        var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, source, DateTime.MinValue);
        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit());
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
        var source = new MessageSource
        {
            UnitCollection = units,
            Options = new Options()
        };
        var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, source, DateTime.MinValue);
        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit { ID = 0, ProjectID = 2 });
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
        var source = new MessageSource
        {
            UnitCollection = units,
            Options = new Options()
        };
        var builder = new WorkUnitCollectionBuilder(null, new ClientSettings { Name = "Foo" }, source, DateTime.MinValue);
        // Act
        var workUnits = builder.BuildForSlot(0, new CPUSlotDescription(), new WorkUnit { ID = 0, ProjectID = 3 });
        // Assert
        Assert.AreEqual(2, workUnits.Count);
    }

    private class MessageSource : IWorkUnitMessageSource
    {
        public Info Info { get; init; }
        public Options Options { get; init; }
        public UnitCollection UnitCollection { get; init; }
        public ClientRun ClientRun { get; init; }
    }
}
