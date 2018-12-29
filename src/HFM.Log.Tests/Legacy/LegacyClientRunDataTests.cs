
using System;

using NUnit.Framework;

namespace HFM.Log.Legacy
{
   [TestFixture]
   public class LegacyClientRunDataTests
   {
      [Test]
      public void LegacyClientRunData_CopyConstructor_Test()
      {
         // Arrange
         var data = new LegacyClientRunData
         {
            StartTime = DateTime.UtcNow,
            ClientVersion = "1.29",
            Arguments = "Foo",
            FoldingID = "Bar",
            Team = 32,
            UserID = "12345",
            MachineID = 1
         };
         // Act
         var copy = new LegacyClientRunData(data);
         // Assert
         Assert.AreEqual(data.StartTime, copy.StartTime);
         Assert.AreEqual(data.ClientVersion, copy.ClientVersion);
         Assert.AreEqual(data.Arguments, copy.Arguments);
         Assert.AreEqual(data.FoldingID, copy.FoldingID);
         Assert.AreEqual(data.Team, copy.Team);
         Assert.AreEqual(data.UserID, copy.UserID);
         Assert.AreEqual(data.MachineID, copy.MachineID);
      }
   }
}
