
using NUnit.Framework;

namespace HFM.Log.FahClient
{
   [TestFixture]
   public class FahClientSlotRunDataTests
   {
      [Test]
      public void FahClientSlotRunData_CopyConstructor_Test()
      {
         // Arrange
         var data = new FahClientSlotRunData
         {
            CompletedUnits = 50,
            FailedUnits = 2
         };
         // Act
         var copy = new FahClientSlotRunData(data);
         // Assert
         Assert.AreEqual(data.CompletedUnits, copy.CompletedUnits);
         Assert.AreEqual(data.FailedUnits, copy.FailedUnits);
      }
   }
}
