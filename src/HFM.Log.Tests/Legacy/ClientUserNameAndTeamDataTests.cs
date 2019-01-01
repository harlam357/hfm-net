
using NUnit.Framework;

namespace HFM.Log.Legacy
{
   [TestFixture]
   public class ClientUserNameAndTeamDataTests
   {
      [Test]
      public void ClientUserNameAndTeamData_CopyConstructor_Test()
      {
         // Arrange
         var data = new ClientUserNameAndTeamData("harlam357", 32);
         // Act
         var copy = new ClientUserNameAndTeamData(data);
         // Assert
         Assert.AreEqual(data.FoldingID, copy.FoldingID);
         Assert.AreEqual(data.Team, copy.Team);
      }

      [Test]
      public void ClientUserNameAndTeamData_CopyConstructor_OtherIsNull_Test()
      {
         // Act
         var copy = new ClientUserNameAndTeamData(null);
         // Assert
         Assert.AreEqual(null, copy.FoldingID);
         Assert.AreEqual(0, copy.Team);
      }
   }
}
