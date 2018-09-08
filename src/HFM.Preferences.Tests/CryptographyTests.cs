
using NUnit.Framework;

namespace HFM.Preferences
{
   [TestFixture]
   public class CryptographyTests
   {
      [Test]
      public void Cryptography_EncryptValue_Test()
      {
         // Arrange
         string plainText = "fizzbizz";
         // Act
         string encryptedText = Support.Cryptography.EncryptValue(plainText);
         // Assert
         Assert.AreEqual("8YsqRpczouPuCPApFum1YQ==", encryptedText);
      }

      [Test]
      public void Cryptography_DecryptValue_Test()
      {
         // Arrange
         string encryptedText = "8YsqRpczouPuCPApFum1YQ==";
         // Act
         string plainText = Support.Cryptography.DecryptValue(encryptedText);
         // Assert
         Assert.AreEqual("fizzbizz", plainText);
      }

      [Test]
      public void Cryptography_DecryptValue_ThatIsNotEncrypted_Test()
      {
         // Arrange
         string encryptedText = "notencrypted";
         // Act
         string plainText = Support.Cryptography.DecryptValue(encryptedText);
         // Assert
         Assert.AreEqual("notencrypted", plainText);
      }
   }
}
