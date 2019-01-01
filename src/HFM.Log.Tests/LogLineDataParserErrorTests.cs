
using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class LogLineDataParserErrorTests
   {
      [Test]
      public void LogLineDataParserError_CopyConstructor_Test()
      {
         // Arrange
         var data = new LogLineDataParserError { Message = "Foo" };
         // Act
         var copy = new LogLineDataParserError(data);
         // Assert
         Assert.AreEqual(data.Message, copy.Message);
      }

      [Test]
      public void LogLineDataParserError_CopyConstructor_OtherIsNull_Test()
      {
         // Act
         var copy = new LogLineDataParserError(null);
         // Assert
         Assert.AreEqual(null, copy.Message);
      }
   }
}
