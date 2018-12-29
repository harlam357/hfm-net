
using NUnit.Framework;

using HFM.Log.Internal;

namespace HFM.Log.FahClient
{
   [TestFixture]
   public class FahClientUnitRunDataTests
   {
      [Test]
      public void FahClientUnitRunData_CopyConstructor_Test()
      {
         // Arrange
         var data = new FahClientUnitRunData
         {
            FramesObserved = 50,
            CoreVersion = "1.23",
            ProjectID = 1,
            ProjectRun = 2,
            ProjectClone = 3,
            ProjectGen = 4,
            WorkUnitResult = WorkUnitResult.FINISHED_UNIT
         };
         // Act
         var copy = new FahClientUnitRunData(data);
         // Assert
         Assert.AreEqual(data.FramesObserved, copy.FramesObserved);
         Assert.AreEqual(data.CoreVersion, copy.CoreVersion);
         Assert.AreEqual(data.ProjectID, copy.ProjectID);
         Assert.AreEqual(data.ProjectRun, copy.ProjectRun);
         Assert.AreEqual(data.ProjectClone, copy.ProjectClone);
         Assert.AreEqual(data.ProjectGen, copy.ProjectGen);
         Assert.AreEqual(data.WorkUnitResult, copy.WorkUnitResult);
      }
   }
}
