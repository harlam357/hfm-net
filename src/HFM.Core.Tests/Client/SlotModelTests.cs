
using NUnit.Framework;

using HFM.Core.WorkUnits;

namespace HFM.Core.Client
{
    [TestFixture]
    public class SlotModelTests
    {
        [Test]
        public void SlotModel_FindDuplicateProjects_WhenProjectsAreDuplicates()
        {
            // Arrange
            var slotModel1 = new SlotModel(new NullClient());
            slotModel1.WorkUnitModel = new WorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
            var slotModel2 = new SlotModel(new NullClient());
            slotModel2.WorkUnitModel = new WorkUnitModel(slotModel2, new WorkUnit { ProjectID = 1 });
            // Act
            SlotModel.FindDuplicateProjects(new[] { slotModel1, slotModel2 });
            // Assert
            Assert.IsTrue(slotModel1.ProjectIsDuplicate);
            Assert.IsTrue(slotModel2.ProjectIsDuplicate);
        }

        [Test]
        public void SlotModel_FindDuplicateProjects_WhenProjectsAreNotDuplicates()
        {
            // Arrange
            var slotModel1 = new SlotModel(new NullClient());
            slotModel1.WorkUnitModel = new WorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
            var slotModel2 = new SlotModel(new NullClient());
            slotModel2.WorkUnitModel = new WorkUnitModel(slotModel2, new WorkUnit { ProjectID = 2 });
            // Act
            SlotModel.FindDuplicateProjects(new[] { slotModel1, slotModel2 });
            // Assert
            Assert.IsFalse(slotModel1.ProjectIsDuplicate);
            Assert.IsFalse(slotModel2.ProjectIsDuplicate);
        }

        [Test]
        public void SlotModel_FindDuplicateProjects_WhenSomeProjectsAreDuplicates()
        {
            // Arrange
            var slotModel1 = new SlotModel(new NullClient());
            slotModel1.WorkUnitModel = new WorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
            var slotModel2 = new SlotModel(new NullClient());
            slotModel2.WorkUnitModel = new WorkUnitModel(slotModel2, new WorkUnit { ProjectID = 2 });
            var slotModel3 = new SlotModel(new NullClient());
            slotModel3.WorkUnitModel = new WorkUnitModel(slotModel2, new WorkUnit { ProjectID = 1 });
            // Act
            SlotModel.FindDuplicateProjects(new[] { slotModel1, slotModel2, slotModel3 });
            // Assert
            Assert.IsTrue(slotModel1.ProjectIsDuplicate);
            Assert.IsFalse(slotModel2.ProjectIsDuplicate);
            Assert.IsTrue(slotModel3.ProjectIsDuplicate);
        }
    }
}
