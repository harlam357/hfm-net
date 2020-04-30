
using System;
using System.Collections.Generic;

using NUnit.Framework;

using HFM.Log;

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
            Assert.AreEqual(0.0f, workUnit.CoreVersion);
            Assert.AreEqual(0, workUnit.ProjectID);
            Assert.AreEqual(0, workUnit.ProjectRun);
            Assert.AreEqual(0, workUnit.ProjectClone);
            Assert.AreEqual(0, workUnit.ProjectGen);
            Assert.IsNull(workUnit.ProteinName);
            Assert.IsNull(workUnit.ProteinTag);
            Assert.AreEqual(WorkUnitResult.Unknown, workUnit.UnitResult);
            Assert.AreEqual(0, workUnit.FramesObserved);
            Assert.IsNull(workUnit.CurrentFrame);
            Assert.IsNull(workUnit.LogLines);
            Assert.IsNull(workUnit.FrameData);
            Assert.IsNull(workUnit.CoreID);
            Assert.AreEqual(-1, workUnit.QueueIndex);
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
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>().With(new WorkUnitFrameData { ID = 0 });
            workUnit.FrameData = frameDataDictionary;
            // Act & Assert
            Assert.AreSame(frameDataDictionary[0], workUnit.CurrentFrame);
        }

        [Test]
        public void WorkUnit_CurrentFrame_IsSourcedFromFrameDataWithGreatestID_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 0 }, new WorkUnitFrameData { ID = 1 }, new WorkUnitFrameData { ID = 5 });
            workUnit.FrameData = frameDataDictionary;
            // Act & Assert
            Assert.AreSame(frameDataDictionary[5], workUnit.CurrentFrame);
        }

        [Test]
        public void WorkUnit_CurrentFrame_ReturnsNullIfMaximumFrameDataIdIsNotZeroOrGreater_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>().With(new WorkUnitFrameData { ID = -1 });
            workUnit.FrameData = frameDataDictionary;
            // Act & Assert
            Assert.IsNull(workUnit.CurrentFrame);
        }

        [Test]
        public void WorkUnit_GetFrameData_ReturnsNullIfRequestedIdDoesNotExist_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            // Act & Assert
            Assert.IsNull(workUnit.GetFrameData(0));
        }

        [Test]
        public void WorkUnit_GetFrameData_ReturnsObjectIfRequestedIdExists_Test()
        {
            // Arrange
            var workUnit = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>().With(new WorkUnitFrameData { ID = 0 });
            workUnit.FrameData = frameDataDictionary;
            // Act & Assert
            Assert.IsNotNull(workUnit.GetFrameData(0));
        }
    }
}
