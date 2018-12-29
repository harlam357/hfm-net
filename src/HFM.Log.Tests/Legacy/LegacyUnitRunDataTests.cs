
using NUnit.Framework;

using HFM.Log.Internal;

namespace HFM.Log.Legacy
{
   [TestFixture]
   public class LegacyUnitRunDataTests
   {
      [Test]
      public void LegacyUnitRunData_CopyConstructor_Test()
      {
         // Arrange
         var data = new LegacyUnitRunData
         {
            FramesObserved = 50,
            CoreVersion = "1.23",
            ProjectID = 1,
            ProjectRun = 2,
            ProjectClone = 3,
            ProjectGen = 4,
            WorkUnitResult = WorkUnitResult.FINISHED_UNIT,
            Threads = 4,
            ClientCoreCommunicationsError = true,
            TotalCompletedUnits = 1025
         };
         // Act
         var copy = new LegacyUnitRunData(data);
         // Assert
         Assert.AreEqual(data.FramesObserved, copy.FramesObserved);
         Assert.AreEqual(data.CoreVersion, copy.CoreVersion);
         Assert.AreEqual(data.ProjectID, copy.ProjectID);
         Assert.AreEqual(data.ProjectRun, copy.ProjectRun);
         Assert.AreEqual(data.ProjectClone, copy.ProjectClone);
         Assert.AreEqual(data.ProjectGen, copy.ProjectGen);
         Assert.AreEqual(data.WorkUnitResult, copy.WorkUnitResult);
         Assert.AreEqual(data.Threads, copy.Threads);
         Assert.AreEqual(data.ClientCoreCommunicationsError, copy.ClientCoreCommunicationsError);
         Assert.AreEqual(data.TotalCompletedUnits, copy.TotalCompletedUnits);
      }

      [Test]
      public void LegacyUnitRunData_CopyConstructor_OtherIsNull_Test()
      {
         // Act
         var copy = new LegacyUnitRunData(null);
         // Assert
         Assert.AreEqual(0, copy.FramesObserved);
         Assert.AreEqual(null, copy.CoreVersion);
         Assert.AreEqual(0, copy.ProjectID);
         Assert.AreEqual(0, copy.ProjectRun);
         Assert.AreEqual(0, copy.ProjectClone);
         Assert.AreEqual(0, copy.ProjectGen);
         Assert.AreEqual(null, copy.WorkUnitResult);
         Assert.AreEqual(0, copy.Threads);
         Assert.AreEqual(false, copy.ClientCoreCommunicationsError);
         Assert.AreEqual(null, copy.TotalCompletedUnits);
      }
   }
}
