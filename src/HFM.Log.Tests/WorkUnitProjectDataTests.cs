
using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class WorkUnitProjectDataTests
   {
      [Test]
      public void WorkUnitProjectData_CopyConstructor_Test()
      {
         // Arrange
         var data = new WorkUnitProjectData(1, 2, 3, 4);
         // Act
         var copy = new WorkUnitProjectData(data);
         // Assert
         Assert.AreEqual(data.ProjectID, copy.ProjectID);
         Assert.AreEqual(data.ProjectRun, copy.ProjectRun);
         Assert.AreEqual(data.ProjectClone, copy.ProjectClone);
         Assert.AreEqual(data.ProjectGen, copy.ProjectGen);
      }

      [Test]
      public void WorkUnitProjectData_CopyConstructor_OtherIsNull_Test()
      {
         // Act
         var copy = new WorkUnitProjectData(null);
         // Assert
         Assert.AreEqual(0, copy.ProjectID);
         Assert.AreEqual(0, copy.ProjectRun);
         Assert.AreEqual(0, copy.ProjectClone);
         Assert.AreEqual(0, copy.ProjectGen);
      }
   }
}
