using System;
using System.IO;
using NUnit.Framework;

namespace HFM.Core
{
    [TestFixture]
    public class ApplicationUpdateTests
    {
        [Test]
        public void ApplicationUpdate_VersionIsGreaterThan_ReturnsTrue()
        {
            // Arrange
            var update = new ApplicationUpdate { Version = "1.0.0.0" };
            // Act
            bool result = update.VersionIsGreaterThan(Application.ParseVersionNumber("0.9.9.9"));
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ApplicationUpdate_VersionIsGreaterThan_ReturnsFalse()
        {
            // Arrange
            var update = new ApplicationUpdate { Version = "1.0.0.0" };
            // Act
            bool result = update.VersionIsGreaterThan(Application.ParseVersionNumber("1.0.0.1"));
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ApplicationUpdate_VersionIsGreaterThan_ReturnsFalseWhenVersionFailsToParse()
        {
            // Arrange
            var update = new ApplicationUpdate { Version = "FailsToParse" };
            // Act
            bool result = update.VersionIsGreaterThan(0);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ApplicationUpdateFile_Verify_DoesNotThrowWhenFileOnDiskMatches()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string path = artifacts.GetRandomFilePath();
                File.WriteAllText(path, "FoobarFizzbizz");

                var fileInfo = new FileInfo(path);

                var hash = new Hash(HashProvider.SHA1);
                var sha1 = hash.Calculate(File.OpenRead(path));

                hash = new Hash(HashProvider.MD5);
                var md5 = hash.Calculate(File.OpenRead(path));
                
                var updateFile = new ApplicationUpdateFile { Size = (int)fileInfo.Length, SHA1 = sha1.ToHex(), MD5 = md5.ToHex() };
                // Act
                Assert.DoesNotThrow(() => updateFile.Verify(path));
            }
        }

        [Test]
        public void ApplicationUpdateFile_Verify_ThrowsWhenFileSizeDoesNotMatch()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string path = artifacts.GetRandomFilePath();
                File.WriteAllText(path, "FoobarFizzbizz");

                var updateFile = new ApplicationUpdateFile { Size = 0 };
                // Act
                Assert.Throws<IOException>(() => updateFile.Verify(path));
            }
        }

        [Test]
        public void ApplicationUpdateFile_Verify_ThrowsWhenSHA1DoesNotMatch()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string path = artifacts.GetRandomFilePath();
                File.WriteAllText(path, "FoobarFizzbizz");

                var fileInfo = new FileInfo(path);

                var updateFile = new ApplicationUpdateFile { Size = (int)fileInfo.Length, SHA1 = "DoesNotMatch" };
                // Act
                Assert.Throws<IOException>(() => updateFile.Verify(path));
            }
        }

        [Test]
        public void ApplicationUpdateFile_Verify_ThrowsWhenMD5DoesNotMatch()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string path = artifacts.GetRandomFilePath();
                File.WriteAllText(path, "FoobarFizzbizz");

                var fileInfo = new FileInfo(path);

                var hash = new Hash(HashProvider.SHA1);
                var sha1 = hash.Calculate(File.OpenRead(path));

                var updateFile = new ApplicationUpdateFile { Size = (int)fileInfo.Length, SHA1 = sha1.ToHex(), MD5 = "DoesNotMatch" };
                // Act
                Assert.Throws<IOException>(() => updateFile.Verify(path));
            }
        }
    }
}
