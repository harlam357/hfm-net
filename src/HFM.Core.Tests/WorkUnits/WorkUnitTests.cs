using System;
using System.Collections.Generic;

using HFM.Log;

using NUnit.Framework;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class WorkUnitTests
    {
        [Test]
        public void WorkUnit_DefaultPropertyValues()
        {
            var workUnit = new WorkUnit();
            Assert.AreEqual(DateTime.MinValue, workUnit.UnitRetrievalTime);
            Assert.IsNull(workUnit.FoldingID);
            Assert.AreEqual(0, workUnit.Team);
            Assert.AreEqual(DateTime.MinValue, workUnit.Assigned);
            Assert.AreEqual(DateTime.MinValue, workUnit.Timeout);
            Assert.AreEqual(TimeSpan.Zero, workUnit.UnitStartTimeStamp);
            Assert.AreEqual(DateTime.MinValue, workUnit.Finished);
            Assert.AreEqual(null, workUnit.CoreVersion);
            Assert.AreEqual(0, workUnit.ProjectID);
            Assert.AreEqual(0, workUnit.ProjectRun);
            Assert.AreEqual(0, workUnit.ProjectClone);
            Assert.AreEqual(0, workUnit.ProjectGen);
            Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
            Assert.AreEqual(0, workUnit.FramesObserved);
            Assert.IsNull(workUnit.CurrentFrame);
            Assert.IsNull(workUnit.LogLines);
            Assert.IsNull(workUnit.Frames);
            Assert.IsNull(workUnit.Core);
            Assert.AreEqual(WorkUnitCollection.NoID, workUnit.ID);
        }

        [Test]
        public void WorkUnit_CurrentFrame_DefaultValueIsNull_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            // Act & Assert
            Assert.IsNull(workUnit.CurrentFrame);
        }

        [Test]
        public void WorkUnit_CurrentFrame_IsSourcedFromFrameDataDictionary_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>().With(new LogLineFrameData { ID = 0 });
            workUnit.Frames = frames;
            // Act & Assert
            Assert.AreSame(frames[0], workUnit.CurrentFrame);
        }

        [Test]
        public void WorkUnit_CurrentFrame_IsSourcedFromFrameDataWithGreatestID_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 0 }, new LogLineFrameData { ID = 1 }, new LogLineFrameData { ID = 5 });
            workUnit.Frames = frames;
            // Act & Assert
            Assert.AreSame(frames[5], workUnit.CurrentFrame);
        }

        [Test]
        public void WorkUnit_CurrentFrame_ReturnsNullIfMaximumFrameDataIdIsNotZeroOrGreater_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>().With(new LogLineFrameData { ID = -1 });
            workUnit.Frames = frames;
            // Act & Assert
            Assert.IsNull(workUnit.CurrentFrame);
        }

        [Test]
        public void WorkUnit_GetFrame_ReturnsNullIfRequestedIdDoesNotExist_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            // Act & Assert
            Assert.IsNull(workUnit.GetFrame(0));
        }

        [Test]
        public void WorkUnit_GetFrame_ReturnsObjectIfRequestedIdExists_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>().With(new LogLineFrameData { ID = 0 });
            workUnit.Frames = frames;
            // Act & Assert
            Assert.IsNotNull(workUnit.GetFrame(0));
        }
    }
}
