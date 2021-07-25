using System;
using System.Collections.Generic;
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
        private const string ApplicationVersion = "1.0.0.0";
        private const string NoApplicationVersion = "0.0.0.0";

        [Test]
        public void XmlPreferencesProvider_ConstructorArgumentValues()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string applicationPath = Environment.CurrentDirectory;
                // Act
                var prefs = new XmlPreferencesProvider(applicationPath, artifacts.Path, ApplicationVersion);
                // Assert
                Assert.AreEqual(applicationPath, prefs.ApplicationPath);
                Assert.AreEqual(applicationPath, prefs.Get<string>(Preference.ApplicationPath));
                Assert.AreEqual(artifacts.Path, prefs.ApplicationDataFolderPath);
                Assert.AreEqual(artifacts.Path, prefs.Get<string>(Preference.ApplicationDataFolderPath));
                Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
                string cacheDirectory = Path.Combine(artifacts.Path, "logcache");
                Assert.AreEqual(cacheDirectory, prefs.Get<string>(Preference.CacheDirectory));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Reset()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                var prefs = Create(artifacts.Path, ApplicationVersion);
                Assert.IsFalse(File.Exists(configPath));
                // Act
                prefs.Reset();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Load_LoadsDefaultsWhenNoConfigXmlOrUserSettingsExists()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                var prefs = Create(artifacts.Path, ApplicationVersion);
                Assert.IsFalse(File.Exists(configPath));
                // Act
                prefs.Load();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Load_LoadsDefaultsWhenConfigXmlReadFails()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                // write a config.xml file that will fail to read
                File.WriteAllText(configPath, String.Empty);
                var prefs = Create(artifacts.Path, ApplicationVersion);
                // Act
                prefs.Load();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Load_ExecutesProjectDownloadUrlUpgrade()
        {
            // Arrange
            var prefs = new TestPreferenceUpgradeXmlPreferencesProvider(ApplicationVersion, null);
            // Act
            prefs.Load();
            // Assert
            Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
            Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            Assert.AreEqual("https://apps.foldingathome.org/psummary.json", prefs.Get<string>(Preference.ProjectDownloadUrl));
        }

        [Test]
        public void XmlPreferencesProvider_Load_ExecutesMainWindowGridColumnsUpgrade()
        {
            // Arrange
            var data = new PreferenceData { ApplicationVersion = NoApplicationVersion };
            data.MainWindowGrid.Columns = new List<string>
            {
                "00,50,True,0",
                "01,60,True,1",
                "02,110,True,2",
                "03,93,True,3",
                "04,44,True,4"
            };
            var prefs = new TestPreferenceUpgradeXmlPreferencesProvider(ApplicationVersion, data);

            // Act
            prefs.Load();
            // Assert
            Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
            Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));

            var expected = new List<string>
            {
                "00,50,True,0",
                "01,60,True,1",
                "02,110,True,2",
                "03,93,True,3",
                "05,44,True,5"
            };
            CollectionAssert.AreEqual(expected, prefs.Get<ICollection<string>>(Preference.FormColumns));
        }

        private class TestPreferenceUpgradeXmlPreferencesProvider : XmlPreferencesProvider
        {
            public PreferenceData Data { get; private set; }

            public TestPreferenceUpgradeXmlPreferencesProvider(string applicationVersion, PreferenceData data)
               : base(Environment.CurrentDirectory, null, applicationVersion)
            {
                Data = data;
            }

            protected override PreferenceData OnRead() => Data;

            protected override void OnWrite(PreferenceData data) => Data = data;
        }

        [Test]
        public void XmlPreferencesProvider_Load_FromConfigFile()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string configPath = Path.Combine(artifacts.Path, "config.xml");

                // write a config.xml file
                var data = new PreferenceData { ApplicationVersion = ApplicationVersion };
                data.ApplicationSettings.CacheFolder = "foo";
                WriteConfigXml(artifacts.Path, data);

                var prefs = Create(artifacts.Path, ApplicationVersion);
                // Act
                prefs.Load();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
                Assert.AreEqual("foo", prefs.Get<string>(Preference.CacheFolder));
            }
        }

        [Test]
        public void XmlPreferencesProvider_Save()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string configPath = Path.Combine(artifacts.Path, "config.xml");
                var prefs = Create(artifacts.Path, ApplicationVersion);
                Assert.IsFalse(File.Exists(configPath));
                // Act
                prefs.Save();
                // Assert
                Assert.IsTrue(File.Exists(configPath));
                Assert.AreEqual(ApplicationVersion, prefs.ApplicationVersion);
                Assert.AreEqual(ApplicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            }
        }

        [Test]
        public void PreferencesProvider_RoundTripEncryptedPreference()
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
        public void PreferencesProvider_PreferenceChanged()
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

        private static void WriteConfigXml(string path, PreferenceData data)
        {
            using (var fileStream = new FileStream(Path.Combine(path, "config.xml"), FileMode.Create, FileAccess.Write))
            using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
            {
                var serializer = new DataContractSerializer(typeof(PreferenceData));
                serializer.WriteObject(xmlWriter, data);
            }
        }
    }
}
