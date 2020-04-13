
using System;

using NUnit.Framework;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class ProjectInfoExtensionsTests
    {
        [Test]
        public void IProjectInfo_ProjectIsUnknown_Test1()
        {
            var projectInfo = new ProjectInfo { ProjectID = 1 };
            Assert.IsFalse(projectInfo.ProjectIsUnknown());
        }

        [Test]
        public void IProjectInfo_ProjectIsUnknown_Test2()
        {
            var projectInfo = new ProjectInfo();
            Assert.IsTrue(projectInfo.ProjectIsUnknown());
        }

        [Test]
        public void IProjectInfo_ProjectIsUnknown_Test3()
        {
            IProjectInfo projectInfo = null;
            Assert.IsTrue(projectInfo.ProjectIsUnknown());
        }

        [Test]
        public void IProjectInfo_ToShortProjectString_Test1()
        {
            var projectInfo = new ProjectInfo();
            Assert.AreEqual("P0 (R0, C0, G0)", projectInfo.ToShortProjectString());
        }

        [Test]
        public void IProjectInfo_ToShortProjectString_Test2()
        {
            var projectInfo = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
            Assert.AreEqual("P1 (R2, C3, G4)", projectInfo.ToShortProjectString());
        }

        [Test]
        public void IProjectInfo_ToShortProjectString_Test3()
        {
            IProjectInfo projectInfo = null;
            Assert.AreEqual(String.Empty, projectInfo.ToShortProjectString());
        }

        [Test]
        public void IProjectInfo_EqualsProject_Test1()
        {
            var projectInfo1 = new ProjectInfo();
            var projectInfo2 = new ProjectInfo();
            Assert.IsTrue(projectInfo1.EqualsProject(projectInfo2));
        }

        [Test]
        public void IProjectInfo_EqualsProject_Test2()
        {
            var projectInfo1 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
            var projectInfo2 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
            Assert.IsTrue(projectInfo1.EqualsProject(projectInfo2));
        }

        [Test]
        public void IProjectInfo_EqualsProject_Test3()
        {
            var projectInfo1 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
            var projectInfo2 = new ProjectInfo { ProjectID = 5, ProjectRun = 6, ProjectClone = 7, ProjectGen = 8 };
            Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
        }

        [Test]
        public void IProjectInfo_EqualsProject_Test4()
        {
            IProjectInfo projectInfo1 = null;
            var projectInfo2 = new ProjectInfo { ProjectID = 5, ProjectRun = 6, ProjectClone = 7, ProjectGen = 8 };
            Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
        }

        [Test]
        public void IProjectInfo_EqualsProject_Test5()
        {
            var projectInfo1 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
            IProjectInfo projectInfo2 = null;
            Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
        }

        [Test]
        public void IProjectInfo_EqualsProject_Test6()
        {
            IProjectInfo projectInfo1 = null;
            IProjectInfo projectInfo2 = null;
            Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
        }
    }
}
