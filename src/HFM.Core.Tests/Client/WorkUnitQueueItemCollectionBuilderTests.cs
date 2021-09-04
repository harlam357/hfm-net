using System;

using HFM.Client.ObjectModel;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class WorkUnitQueueItemCollectionBuilderTests
    {
        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnNullWhenUnitCollectionIsNull()
        {
            // Arrange
            var builder = new WorkUnitQueueItemCollectionBuilder(null, null);
            // Act
            var collection = builder.BuildForSlot(0);
            // Assert
            Assert.IsNull(collection);
        }

        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnNullWhenUnitCollectionContainsNoWorkUnitsForTheSlot()
        {
            // Arrange
            var unitCollection = new UnitCollection
            {
                new Unit { Slot = 0, ID = 1 }
            };
            var builder = new WorkUnitQueueItemCollectionBuilder(unitCollection, null);
            // Act
            var collection = builder.BuildForSlot(1);
            // Assert
            Assert.IsNull(collection);
        }

        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnsCollectionWithTwoItemsForSlotZero()
        {
            // Arrange
            var builder = new WorkUnitQueueItemCollectionBuilder(CreateUnitCollection(), null);
            // Act
            var collection = builder.BuildForSlot(0);
            // Assert
            Assert.AreEqual(2, collection.Count);
        }

        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnsCollectionWithOneItemForSlotOne()
        {
            // Arrange
            var builder = new WorkUnitQueueItemCollectionBuilder(CreateUnitCollection(), null);
            // Act
            var collection = builder.BuildForSlot(1);
            // Assert
            Assert.AreEqual(1, collection.Count);
        }

        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnsCollectionWithCurrentIDSetToTheRunningWorkUnit()
        {
            // Arrange
            var builder = new WorkUnitQueueItemCollectionBuilder(CreateUnitCollection(), null);
            // Act
            var collection = builder.BuildForSlot(0);
            // Assert
            Assert.AreEqual(0, collection.CurrentID);
        }

        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnsCollectionWithCurrentIDSetToDefaultIDWhenThereIsNoRunningWorkUnit()
        {
            // Arrange
            var unitCollection = new UnitCollection
            {
                new Unit { Slot = 0, ID = 1 }
            };
            var builder = new WorkUnitQueueItemCollectionBuilder(unitCollection, null);
            // Act
            var collection = builder.BuildForSlot(0);
            // Assert
            Assert.AreEqual(collection.DefaultID, collection.CurrentID);
        }

        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnsCollectionWithPopulatedItemsForSlotZero()
        {
            // Arrange
            var builder = new WorkUnitQueueItemCollectionBuilder(CreateUnitCollection(), CreateSystemInfo());
            // Act
            var collection = builder.BuildForSlot(0);
            // Assert
            var item = collection[0];
            Assert.AreEqual(0, item.ID);
            Assert.AreEqual(1, item.ProjectID);
            Assert.AreEqual(2, item.ProjectRun);
            Assert.AreEqual(3, item.ProjectClone);
            Assert.AreEqual(4, item.ProjectGen);
            Assert.AreEqual("RUNNING", item.State);
            Assert.AreEqual("Foo", item.WaitingOn);
            Assert.AreEqual(1, item.Attempts);
            Assert.AreEqual(TimeSpan.FromMinutes(1), item.NextAttempt);
            Assert.AreEqual(new DateTime(2020, 1, 1), item.Assigned);
            Assert.AreEqual(new DateTime(2020, 1, 2), item.Timeout);
            Assert.AreEqual(new DateTime(2020, 1, 3), item.Deadline);
            Assert.AreEqual("1.2.3.4", item.WorkServer);
            Assert.AreEqual("10.20.30.40", item.CollectionServer);
            Assert.AreEqual("Windows", item.OperatingSystem);
            Assert.AreEqual(16384, item.Memory);
            Assert.AreEqual(4, item.CPUThreads);
            Assert.AreEqual(0, item.SlotID);

            item = collection[1];
            Assert.AreEqual(1, item.ID);
            Assert.AreEqual(2, item.ProjectID);
            Assert.AreEqual(3, item.ProjectRun);
            Assert.AreEqual(4, item.ProjectClone);
            Assert.AreEqual(5, item.ProjectGen);
            Assert.AreEqual("READY", item.State);
            Assert.AreEqual("Bar", item.WaitingOn);
            Assert.AreEqual(2, item.Attempts);
            Assert.AreEqual(TimeSpan.FromMinutes(2), item.NextAttempt);
            Assert.AreEqual(new DateTime(2020, 2, 2), item.Assigned);
            Assert.AreEqual(new DateTime(2020, 2, 3), item.Timeout);
            Assert.AreEqual(new DateTime(2020, 2, 4), item.Deadline);
            Assert.AreEqual("2.3.4.5", item.WorkServer);
            Assert.AreEqual("20.30.40.50", item.CollectionServer);
            Assert.AreEqual("Windows", item.OperatingSystem);
            Assert.AreEqual(16384, item.Memory);
            Assert.AreEqual(4, item.CPUThreads);
            Assert.AreEqual(0, item.SlotID);
        }

        [Test]
        public void WorkUnitQueueItemCollectionBuilder_BuildForSlot_ReturnsCollectionWithPopulatedItemsForSlotOne()
        {
            // Arrange
            var builder = new WorkUnitQueueItemCollectionBuilder(CreateUnitCollection(), CreateSystemInfo());
            // Act
            var collection = builder.BuildForSlot(1);
            // Assert
            var item = collection[2];
            Assert.AreEqual(2, item.ID);
            Assert.AreEqual(3, item.ProjectID);
            Assert.AreEqual(4, item.ProjectRun);
            Assert.AreEqual(5, item.ProjectClone);
            Assert.AreEqual(6, item.ProjectGen);
            Assert.AreEqual("RUNNING", item.State);
            Assert.AreEqual("Fizz", item.WaitingOn);
            Assert.AreEqual(3, item.Attempts);
            Assert.AreEqual(TimeSpan.FromMinutes(3), item.NextAttempt);
            Assert.AreEqual(new DateTime(2020, 3, 3), item.Assigned);
            Assert.AreEqual(new DateTime(2020, 3, 4), item.Timeout);
            Assert.AreEqual(new DateTime(2020, 3, 5), item.Deadline);
            Assert.AreEqual("3.4.5.6", item.WorkServer);
            Assert.AreEqual("30.40.50.60", item.CollectionServer);
            Assert.AreEqual("Windows", item.OperatingSystem);
            Assert.AreEqual(16384, item.Memory);
            Assert.AreEqual(4, item.CPUThreads);
            Assert.AreEqual(1, item.SlotID);
        }

        private static UnitCollection CreateUnitCollection()
        {
            var collection = new UnitCollection();

            var unit = new Unit();
            unit.Slot = 0;
            unit.ID = 0;
            unit.Project = 1;
            unit.Run = 2;
            unit.Clone = 3;
            unit.Gen = 4;
            unit.State = "RUNNING";
            unit.WaitingOn = "Foo";
            unit.Attempts = 1;
            unit.NextAttemptTimeSpan = TimeSpan.FromMinutes(1);
            unit.AssignedDateTime = new DateTime(2020, 1, 1);
            unit.TimeoutDateTime = new DateTime(2020, 1, 2);
            unit.DeadlineDateTime = new DateTime(2020, 1, 3);
            unit.WorkServer = "1.2.3.4";
            unit.CollectionServer = "10.20.30.40";
            collection.Add(unit);

            unit = new Unit();
            unit.Slot = 0;
            unit.ID = 1;
            unit.Project = 2;
            unit.Run = 3;
            unit.Clone = 4;
            unit.Gen = 5;
            unit.State = "READY";
            unit.WaitingOn = "Bar";
            unit.Attempts = 2;
            unit.NextAttemptTimeSpan = TimeSpan.FromMinutes(2);
            unit.AssignedDateTime = new DateTime(2020, 2, 2);
            unit.TimeoutDateTime = new DateTime(2020, 2, 3);
            unit.DeadlineDateTime = new DateTime(2020, 2, 4);
            unit.WorkServer = "2.3.4.5";
            unit.CollectionServer = "20.30.40.50";
            collection.Add(unit);

            unit = new Unit();
            unit.Slot = 1;
            unit.ID = 2;
            unit.Project = 3;
            unit.Run = 4;
            unit.Clone = 5;
            unit.Gen = 6;
            unit.State = "RUNNING";
            unit.WaitingOn = "Fizz";
            unit.Attempts = 3;
            unit.NextAttemptTimeSpan = TimeSpan.FromMinutes(3);
            unit.AssignedDateTime = new DateTime(2020, 3, 3);
            unit.TimeoutDateTime = new DateTime(2020, 3, 4);
            unit.DeadlineDateTime = new DateTime(2020, 3, 5);
            unit.WorkServer = "3.4.5.6";
            unit.CollectionServer = "30.40.50.60";
            collection.Add(unit);

            return collection;
        }

        private static SystemInfo CreateSystemInfo()
        {
            var info = new SystemInfo();
            info.OS = "Windows";
            info.MemoryValue = 16;
            info.CPUs = 4;
            return info;
        }
    }
}
