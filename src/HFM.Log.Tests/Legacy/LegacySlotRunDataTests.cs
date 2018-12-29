
using NUnit.Framework;

namespace HFM.Log.Legacy
{
   [TestFixture]
   public class LegacySlotRunDataTests
   {
      [Test]
      public void LegacySlotRunData_CopyConstructor_Test()
      {
         // Arrange
         var data = new LegacySlotRunData
         {
            CompletedUnits = 50,
            FailedUnits = 2,
            TotalCompletedUnits = 1025
         };
         // Act
         var copy = new LegacySlotRunData(data);
         // Assert
         Assert.AreEqual(data.CompletedUnits, copy.CompletedUnits);
         Assert.AreEqual(data.FailedUnits, copy.FailedUnits);
         Assert.AreEqual(data.TotalCompletedUnits, copy.TotalCompletedUnits);
      }

      [Test]
      public void LegacySlotRunData_CopyConstructor_OtherIsNull_Test()
      {
         // Act
         var copy = new LegacySlotRunData(null);
         // Assert
         Assert.AreEqual(0, copy.CompletedUnits);
         Assert.AreEqual(0, copy.FailedUnits);
         Assert.AreEqual(null, copy.TotalCompletedUnits);
      }
   }
}
