
using System;

using NUnit.Framework;

namespace HFM.Log.FahClient
{
   [TestFixture]
   public class FahClientClientRunDataTests
   {
      [Test]
      public void FahClientClientRunData_CopyConstructor_Test()
      {
         // Arrange
         var data = new FahClientClientRunData { StartTime = DateTime.UtcNow };
         // Act
         var copy = new FahClientClientRunData(data);
         // Assert
         Assert.AreEqual(data.StartTime, copy.StartTime);
      }

      [Test]
      public void FahClientClientRunData_CopyConstructor_OtherIsNull_Test()
      {
         // Act
         var copy = new FahClientClientRunData(null);
         // Assert
         Assert.AreEqual(default(DateTime), copy.StartTime);
      }
   }
}
