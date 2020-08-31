
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

using NUnit.Framework;

using HFM.Preferences.Data;

namespace HFM.Preferences
{
    [TestFixture]
    public partial class PreferencesProviderTests
    {
        [Test]
        public void XmlPreferencesProvider_ConstructorArgumentValues_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                const string applicationVersion = "1.0.0.0";
                string applicationPath = Environment.CurrentDirectory;
                // Act
                var prefs = new XmlPreferencesProvider(applicationPath, artifacts.Path, applicationVersion);
                // Assert
                Assert.AreEqual(applicationPath, prefs.ApplicationPath);
                Assert.AreEqual(applicationPath, prefs.Get<string>(Preference.ApplicationPath));
                Assert.AreEqual(artifacts.Path, prefs.ApplicationDataFolderPath);
                Assert.AreEqual(artifacts.Path, prefs.Get<string>(Preference.ApplicationDataFolderPath));
                Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
                string cacheDirectory = Path.Combine(artifacts.Path, "logcache");
                Assert.AreEqual(cacheDirectory, prefs.Get<string>(Preference.CacheDirectory));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Reset_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                const string applicationVersion = "1.0.0.0";
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                var prefs = Create(artifacts.Path, applicationVersion);
                Assert.IsFalse(File.Exists(configPath));
                // Act
                prefs.Reset();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Load_LoadsDefaultsWhenNoConfigXmlOrUserSettingsExists_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                const string applicationVersion = "1.0.0.0";
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                var prefs = Create(artifacts.Path, applicationVersion);
                Assert.IsFalse(File.Exists(configPath));
                // Act
                prefs.Load();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Load_LoadsDefaultsWhenConfigXmlReadFails_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                const string applicationVersion = "1.0.0.0";
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                // write a config.xml file that will fail to read
                File.WriteAllText(configPath, String.Empty);
                var prefs = Create(artifacts.Path, applicationVersion);
                // Act
                prefs.Load();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Load_ExecutesPreferenceUpgrades_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                const string applicationVersion = "1.0.0.0";
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                var prefs = new TestPreferenceUpgradeXmlPreferencesProvider(artifacts.Path, applicationVersion);
                Assert.IsFalse(File.Exists(configPath));
                // Act
                prefs.Load();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
                Assert.AreEqual("https://apps.foldingathome.org/psummary.json", prefs.Get<string>(Preference.ProjectDownloadUrl));
            }
        }

        private class TestPreferenceUpgradeXmlPreferencesProvider : XmlPreferencesProvider
        {
            public TestPreferenceUpgradeXmlPreferencesProvider(string applicationDataFolderPath, string applicationVersion)
               : base(Environment.CurrentDirectory, applicationDataFolderPath, applicationVersion)
            {

            }

            protected override PreferenceData OnRead()
            {
                var data = new PreferenceData();
                data.ApplicationVersion = "0.0.0.0";
                data.ApplicationSettings.ProjectDownloadUrl = String.Empty;
                return data;
            }
        }

        [Test]
        public void XmlPreferencesProvider_Load_FromConfigFile_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                const string applicationVersion = "1.0.0.0";
                string configPath = Path.Combine(artifacts.Path, "config.xml");

                // write a config.xml file
                var data = new PreferenceData { ApplicationVersion = applicationVersion };
                data.ApplicationSettings.CacheFolder = "foo";
                using (var fileStream = new FileStream(Path.Combine(artifacts.Path, "config.xml"), FileMode.Create, FileAccess.Write))
                using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
                {
                    var serializer = new DataContractSerializer(typeof(PreferenceData));
                    serializer.WriteObject(xmlWriter, data);
                }

                var prefs = Create(artifacts.Path, applicationVersion);
                // Act
                prefs.Load();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
                Assert.AreEqual("foo", prefs.Get<string>(Preference.CacheFolder));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Save_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                const string applicationVersion = "1.0.0.0";
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                var prefs = Create(artifacts.Path, applicationVersion);
                Assert.IsFalse(File.Exists(configPath));
                // Act
                prefs.Save();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void PreferencesProvider_RoundTripEncryptedPreference_Test()
        {
            // Arrange
            const string value = "fizzbizz";
            var data = new PreferenceData();
            var prefs = new MockPreferencesProvider(data);
            // Act
            prefs.Set(Preference.WebGenPassword, value);
            // Assert
            Assert.AreNotEqual(value, data.WebDeployment.FtpServer.Password);
            Assert.AreEqual(value, prefs.Get<string>(Preference.WebGenPassword));
        }

        [Test]
        public void PreferencesProvider_PreferenceChanged_Test()
        {
            // Arrange
            var prefs = new MockPreferencesProvider();
            object sender = null;
            PreferenceChangedEventArgs args = null;
            prefs.PreferenceChanged += (s, e) =>
            {
                sender = s;
                args = e;
            };
            // Act
            prefs.Set(Preference.ColorLogFile, false);
            // Assert
            Assert.AreSame(prefs, sender);
            Assert.AreEqual(Preference.ColorLogFile, args.Preference);
        }

        private enum FtpMode
        {
            Default,
            Passive
        }

        private enum BonusCalculation
        {
            Default,
            DownloadTime
        }

        private class MockPreferencesProvider : InMemoryPreferencesProvider
        {
            public MockPreferencesProvider()
               : base(Environment.CurrentDirectory, Environment.CurrentDirectory, "1.0.0.0")
            {

            }

            public MockPreferencesProvider(PreferenceData data) : this()
            {
                Load(data);
            }
        }

        private static XmlPreferencesProvider Create(string applicationDataFolderPath, string applicationVersion)
        {
            return new XmlPreferencesProvider(Environment.CurrentDirectory, applicationDataFolderPath, applicationVersion);
        }
    }
}
