
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
   }
}
