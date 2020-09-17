using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core;
using HFM.Core.Client;

namespace HFM.Forms
{
    [TestFixture]
    public class ClientSettingsManagerTests
    {
        [Test]
        public void ClientSettingsManager_VerifyDefaultState()
        {
            // Act
            var manager = new ClientSettingsManager(null);
            // Assert
            Assert.AreEqual(String.Empty, manager.FileName);
            Assert.AreEqual(1, manager.FilterIndex);
            Assert.AreEqual("hfmx", manager.FileExtension);
            Assert.AreEqual("HFM Configuration Files|*.hfmx", manager.FileTypeFilters);
        }

        [Test]
        public void ClientSettingsManager_Read_ReturnsClientSettingsCollectionAndSetsManagerState()
        {
            // Arrange
            var manager = new ClientSettingsManager(null);
            // Act
            var settings = manager.Read("..\\..\\..\\TestFiles\\ClientSettings_0_9_11.hfmx", 1);
            // Assert
            Assert.IsNotNull(settings);
            Assert.AreEqual(1, settings.Count());
            Assert.AreEqual("..\\..\\..\\TestFiles\\ClientSettings_0_9_11.hfmx", manager.FileName);
            Assert.AreEqual(1, manager.FilterIndex);
            Assert.AreEqual(".hfmx", manager.FileExtension);
        }

        [Test]
        public void ClientSettingsManager_Write_WritesTheClientSettingsToDisk()
        {
            // Arrange
            var client = new NullClient();
            client.Settings = new ClientSettings { Name = "test" };
            using (var artifacts = new ArtifactFolder())
            {
                var manager = new ClientSettingsManager(null);
                string path = Path.ChangeExtension(artifacts.GetRandomFilePath(), ".hfmx");
                // Act
                manager.Write(new[] { client.Settings }, path, 1);
                // Assert
                Assert.AreEqual(path, manager.FileName);
                Assert.AreEqual(1, manager.FilterIndex);
                Assert.AreEqual(".hfmx", manager.FileExtension);
            }
        }
    }
}
