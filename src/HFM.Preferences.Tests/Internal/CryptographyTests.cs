
using System;

using NUnit.Framework;

namespace HFM.Preferences.Internal
{
    [TestFixture]
    public class CryptographyTests
    {
        [Test]
        public void Cryptography_EncryptValue_NullString_Test()
        {
            // Arrange
            string plainText = null;
            // Act
            string encryptedText = Cryptography.EncryptValue(plainText);
            // Assert
            Assert.AreEqual(String.Empty, encryptedText);
        }

        [Test]
        public void Cryptography_EncryptValue_EmptyString_Test()
        {
            // Arrange
            string plainText = String.Empty;
            // Act
            string encryptedText = Cryptography.EncryptValue(plainText);
            // Assert
            Assert.AreEqual(String.Empty, encryptedText);
        }

        [Test]
        public void Cryptography_EncryptValue_WhiteSpaceString_Test()
        {
            // Arrange
            string plainText = "   ";
            // Act
            string encryptedText = Cryptography.EncryptValue(plainText);
            // Assert
            Assert.AreEqual(String.Empty, encryptedText);
        }

        [Test]
        public void Cryptography_EncryptValue_Test()
        {
            // Arrange
            string plainText = "fizzbizz";
            // Act
            string encryptedText = Cryptography.EncryptValue(plainText);
            // Assert
            Assert.AreEqual("8YsqRpczouPuCPApFum1YQ==", encryptedText);
        }

        [Test]
        public void Cryptography_DecryptValue_NullString_Test()
        {
            // Arrange
            string encryptedText = null;
            // Act
            string plainText = Cryptography.DecryptValue(encryptedText);
            // Assert
            Assert.AreEqual(String.Empty, plainText);
        }

        [Test]
        public void Cryptography_DecryptValue_EmptyString_Test()
        {
            // Arrange
            string encryptedText = String.Empty;
            // Act
            string plainText = Cryptography.DecryptValue(encryptedText);
            // Assert
            Assert.AreEqual(String.Empty, plainText);
        }

        [Test]
        public void Cryptography_DecryptValue_WhiteSpaceString_Test()
        {
            // Arrange
            string encryptedText = "   ";
            // Act
            string plainText = Cryptography.DecryptValue(encryptedText);
            // Assert
            Assert.AreEqual(String.Empty, plainText);
        }

        [Test]
        public void Cryptography_DecryptValue_Test()
        {
            // Arrange
            string encryptedText = "8YsqRpczouPuCPApFum1YQ==";
            // Act
            string plainText = Cryptography.DecryptValue(encryptedText);
            // Assert
            Assert.AreEqual("fizzbizz", plainText);
        }

        [Test]
        public void Cryptography_DecryptValue_ThatIsNotBase64Encoded_Test()
        {
            // Arrange
            string encryptedText = "*notbase64encoded*";
            // Act
            string plainText = Cryptography.DecryptValue(encryptedText);
            // Assert
            Assert.AreEqual("*notbase64encoded*", plainText);
        }

        [Test]
        public void Cryptography_DecryptValue_ThatIsNotEncrypted_Test()
        {
            // Arrange
            string encryptedText = "notencrypted";
            // Act
            string plainText = Cryptography.DecryptValue(encryptedText);
            // Assert
            Assert.AreEqual("notencrypted", plainText);
        }
    }
}
