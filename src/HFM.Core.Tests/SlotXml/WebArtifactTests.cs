﻿using NUnit.Framework;

using HFM.Core.Client;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    [TestFixture]
    public class WebArtifactTests
    {
        [Test]
        public void WebArtifactBuilder_Build_CreatesDirectoryWithXmlFiles()
        {
            var preferences = CreatePreferences();
            preferences.Set(Preference.WebGenCopyHtml, false);

            var collection = CreateClientDataCollection();
            using (var artifacts = new ArtifactFolder())
            {
                var artifactBuilder = new WebArtifactBuilder(null, preferences, artifacts.Path);
                var path = artifactBuilder.Build(collection);

                Assert.IsTrue(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".xml"));
                Assert.IsFalse(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".html"));
                Assert.IsFalse(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".txt"));
            }
        }

        [Test]
        public void WebArtifactBuilder_Build_CreatesDirectoryWithXmlAndHtmlFiles()
        {
            var preferences = CreatePreferences();
            preferences.Set(Preference.WebGenCopyHtml, true);

            var collection = CreateClientDataCollection();
            using (var artifacts = new ArtifactFolder())
            {
                var artifactBuilder = new WebArtifactBuilder(null, preferences, artifacts.Path);
                var path = artifactBuilder.Build(collection);

                Assert.IsTrue(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".xml"));
                Assert.IsTrue(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".html"));
                Assert.IsFalse(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".txt"));
            }
        }

        [Test]
        public void WebArtifactBuilder_Build_CreatesDirectoryWithXmlHtmlAndLogFiles()
        {
            using (var appDataFolder = new ArtifactFolder())
            {
                var preferences = CreatePreferences(appDataFolder.Path);
                preferences.Set(Preference.WebGenCopyHtml, true);
                preferences.Set(Preference.WebGenCopyFAHlog, true);

                string cacheDirectory = preferences.Get<string>(Preference.CacheDirectory);
                Directory.CreateDirectory(cacheDirectory);

                var collection = CreateClientDataCollection();
                foreach (var slot in collection)
                {
                    using (var stream = File.Create(Path.Combine(cacheDirectory, $"{slot.Name}-log.txt")))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(slot.Name);
                    }
                }

                using (var artifacts = new ArtifactFolder())
                {
                    var artifactBuilder = new WebArtifactBuilder(null, preferences, artifacts.Path);
                    var path = artifactBuilder.Build(collection);

                    Assert.IsTrue(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".xml"));
                    Assert.IsTrue(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".html"));
                    Assert.IsTrue(Directory.EnumerateFiles(path).Any(x => Path.GetExtension(x) == ".txt"));
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DeployTestCaseSource))]
        public void WebArtifactDeployment_Deploy_DeploysArtifactsToTargetPath(bool copyXml, bool copyHtml, bool copyLog, int expected)
        {
            using (var appDataFolder = new ArtifactFolder())
            {
                var preferences = CreatePreferences(appDataFolder.Path);
                preferences.Set(Preference.WebGenCopyXml, copyXml);
                preferences.Set(Preference.WebGenCopyHtml, copyHtml);
                preferences.Set(Preference.WebGenCopyFAHlog, copyLog);

                string cacheDirectory = preferences.Get<string>(Preference.CacheDirectory);
                Directory.CreateDirectory(cacheDirectory);

                var collection = CreateClientDataCollection();
                foreach (var slot in collection)
                {
                    using (var stream = File.Create(Path.Combine(cacheDirectory, $"{slot.Name}-log.txt")))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(slot.Name);
                    }
                }

                using (var artifacts = new ArtifactFolder())
                {
                    var artifactBuilder = new WebArtifactBuilder(null, preferences, artifacts.Path);
                    var path = artifactBuilder.Build(collection);

                    using (var deployFolder = new ArtifactFolder())
                    {
                        preferences.Set(Preference.WebDeploymentRoot, deployFolder.Path);
                        var deployment = WebArtifactDeployment.Create(WebDeploymentType.Path, null, preferences);
                        deployment.Deploy(path);

                        Assert.AreEqual(expected, Directory.GetFiles(deployFolder.Path).Length);
                    }
                }
            }
        }

        private static object[] DeployTestCaseSource =
        {
            new object[] { false, false, false, 0 },
            new object[] { true, false, false, 3 },
            new object[] { false, true, false, 6},
            new object[] { false, false, true, 2 },
            new object[] { true, true, false, 9 },
            new object[] { false, true, true, 8 },
            new object[] { true, false, true, 5 },
            new object[] { true, true, true, 11 }
        };

        private static IPreferences CreatePreferences()
        {
            return CreatePreferences(String.Empty);
        }

        private static IPreferences CreatePreferences(string applicationDataFolderPath)
        {
            string applicationPath = Path.GetFullPath(@"..\..\..\..\HFM");
            var preferences = new InMemoryPreferencesProvider(applicationPath, applicationDataFolderPath, null);
            preferences.Set(Preference.DecimalPlaces, 0);
            preferences.Set(Preference.WebOverview, "WebOverview.xslt");
            preferences.Set(Preference.WebSummary, "WebSummary.xslt");
            preferences.Set(Preference.WebSlot, "WebSlot.xslt");
            return preferences;
        }

        private static ICollection<IClientData> CreateClientDataCollection()
        {
            var collection = new List<IClientData>();

            // setup slot
            var client = new NullClient { Settings = new ClientSettings { Name = "Test2" } };
            var slot = new FahClientData(new InMemoryPreferencesProvider(), client, default, SlotIdentifier.NoSlotID);
            var logLines = new List<Log.LogLine>
            {
                new() { LineType = LogLineType.LogOpen, Index = 1, Raw = "Open" }
            };
            slot.CurrentLogLines = logLines;
            slot.WorkUnitModel.WorkUnit.LogLines = logLines;
            collection.Add(slot);

            // setup slot
            client = new NullClient { Settings = new ClientSettings { Name = "Test1" } };
            slot = new FahClientData(new InMemoryPreferencesProvider(), client, default, SlotIdentifier.NoSlotID);
            collection.Add(slot);

            return collection;
        }
    }
}
