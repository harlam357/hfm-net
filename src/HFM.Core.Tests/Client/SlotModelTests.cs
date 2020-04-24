﻿
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
            var instance1 = new SlotModel(new NullClient());
            var unitInfo1 = new WorkUnit { ProjectID = 1 };
            var logic1 = CreateWorkUnitModel(unitInfo1);
            instance1.WorkUnitModel = logic1;

            var instance2 = new SlotModel(new NullClient());
            var unitInfo2 = new WorkUnit { ProjectID = 1 };
            var logic2 = CreateWorkUnitModel(unitInfo2);
            instance2.WorkUnitModel = logic2;

            SlotModel.FindDuplicateProjects(new[] { instance1, instance2 });

            Assert.IsTrue(instance1.ProjectIsDuplicate);
            Assert.IsTrue(instance2.ProjectIsDuplicate);
        }

        [Test]
        public void SlotModel_FindDuplicateProjects_WhenProjectsAreNotDuplicates()
        {
            var instance1 = new SlotModel(new NullClient());
            var unitInfo1 = new WorkUnit { ProjectID = 1 };
            var logic1 = CreateWorkUnitModel(unitInfo1);
            instance1.WorkUnitModel = logic1;

            var instance2 = new SlotModel(new NullClient());
            var unitInfo2 = new WorkUnit { ProjectID = 2 };
            var logic2 = CreateWorkUnitModel(unitInfo2);
            instance2.WorkUnitModel = logic2;

            SlotModel.FindDuplicateProjects(new[] { instance1, instance2 });

            Assert.IsFalse(instance1.ProjectIsDuplicate);
            Assert.IsFalse(instance2.ProjectIsDuplicate);
        }

        [Test]
        public void SlotModel_FindDuplicateProjects_WhenSomeProjectsAreDuplicates()
        {
            var instance1 = new SlotModel(new NullClient());
            var unitInfo1 = new WorkUnit { ProjectID = 1 };
            var logic1 = CreateWorkUnitModel(unitInfo1);
            instance1.WorkUnitModel = logic1;

            var instance2 = new SlotModel(new NullClient());
            var unitInfo2 = new WorkUnit { ProjectID = 2 };
            var logic2 = CreateWorkUnitModel(unitInfo2);
            instance2.WorkUnitModel = logic2;

            var instance3 = new SlotModel(new NullClient());
            var unitInfo3 = new WorkUnit { ProjectID = 1 };
            var logic3 = CreateWorkUnitModel(unitInfo3);
            instance3.WorkUnitModel = logic3;

            SlotModel.FindDuplicateProjects(new[] { instance1, instance2, instance3 });

            Assert.IsTrue(instance1.ProjectIsDuplicate);
            Assert.IsFalse(instance2.ProjectIsDuplicate);
            Assert.IsTrue(instance3.ProjectIsDuplicate);
        }

        private static WorkUnitModel CreateWorkUnitModel(WorkUnit workUnit)
        {
            return new WorkUnitModel { Data = workUnit };
        }
    }
}
