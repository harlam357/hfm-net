
using System.Linq;

using NUnit.Framework;

using HFM.Log.FahClient;

namespace HFM.Log
{
   [TestFixture]
   public class LogLineEnumerableTests
   {
      [Test]
      public void LogLineEnumerable_FromFahLog_Test()
      {
         // Arrange
         var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_16\\log.txt");
         // Act
         var enumerable = LogLineEnumerable.Create(fahLog);
         // Assert
         Assert.AreEqual(34346, enumerable.Count());
      }

      [Test]
      public void LogLineEnumerable_FromClientRun_Test()
      {
         // Arrange
         var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_16\\log.txt");
         var clientRun = fahLog.ClientRuns.Last();
         // Act
         var enumerable = LogLineEnumerable.Create(clientRun);
         // Assert
         Assert.AreEqual(34346, enumerable.Count());
      }

      [Test]
      public void LogLineEnumerable_FromSlotRun_Test()
      {
         // Arrange
         var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_16\\log.txt");
         var slotRun = fahLog.ClientRuns.Last().SlotRuns[0];
         // Act
         var enumerable = LogLineEnumerable.Create(slotRun);
         // Assert
         Assert.AreEqual(34147, enumerable.Count());
      }

      [Test]
      public void LogLineEnumerable_FromUnitRun_Test()
      {
         // Arrange
         var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_16\\log.txt");
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         // Act
         var enumerable = LogLineEnumerable.Create(unitRun);
         // Assert
         Assert.AreEqual(150, enumerable.Count());
      }
   }
}
