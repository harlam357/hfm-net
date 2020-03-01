/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AutoMapper;
using NUnit.Framework;

using HFM.Core.Client;
using HFM.Core.Configuration;
using HFM.Core.DataTypes;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    [TestFixture]
    public class WebArtifactTests
    {
        [SetUp]
        public void Init()
        {
            Mapper.Initialize(c => c.AddProfile<AutoMapperProfile>());
        }

        [Test]
        public void WebArtifactBuilder_Build_CreatesDirectoryWithXmlFiles()
        {
            var preferences = CreatePreferences();
            preferences.Set(Preference.WebGenCopyHtml, false);

            var slots = CreateSlotModelCollection(preferences);
            using (var artifacts = new ArtifactFolder())
            {
                var artifactBuilder = new WebArtifactBuilder(null, preferences, artifacts.Path);
                var path = artifactBuilder.Build(slots);

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

            var slots = CreateSlotModelCollection(preferences);
            using (var artifacts = new ArtifactFolder())
            {
                var artifactBuilder = new WebArtifactBuilder(null, preferences, artifacts.Path);
                var path = artifactBuilder.Build(slots);

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

                var slots = CreateSlotModelCollection(preferences);
                foreach (var slot in slots)
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
                    var path = artifactBuilder.Build(slots);

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

                var slots = CreateSlotModelCollection(preferences);
                foreach (var slot in slots)
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
                    var path = artifactBuilder.Build(slots);

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
            new object[] { false, true, false, 7 },
            new object[] { false, false, true, 2 },
            new object[] { true, true, false, 10 },
            new object[] { false, true, true, 9 },
            new object[] { true, false, true, 5 },
            new object[] { true, true, true, 12 }
        };

        private static IPreferenceSet CreatePreferences()
        {
            return CreatePreferences(String.Empty);
        }

        private static IPreferenceSet CreatePreferences(string applicationDataFolderPath)
        {
            var preferences = new InMemoryPreferenceSet(@"..\..\..\HFM", applicationDataFolderPath, null);
            preferences.Set(Preference.DecimalPlaces, 0);
            preferences.Set(Preference.WebOverview, "WebOverview.xslt");
            preferences.Set(Preference.WebMobileOverview, "WebMobileOverview.xslt");
            preferences.Set(Preference.WebSummary, "WebSummary.xslt");
            preferences.Set(Preference.WebMobileSummary, "WebMobileSummary.xslt");
            preferences.Set(Preference.WebSlot, "WebSlot.xslt");
            return preferences;
        }

        private static ICollection<SlotModel> CreateSlotModelCollection(IPreferenceSet preferences)
        {
            var slots = new List<SlotModel>();

            // setup slot
            var slot = new SlotModel();
            slot.Prefs = preferences;
            // set concrete values
            slot.Settings = new ClientSettings {Name = "Test2"};
            var logLines = new List<Log.LogLine>
            {
                new Log.LogLine {LineType = LogLineType.LogHeader, Index = 1, Raw = "Header"}
            };
            slot.CurrentLogLines = logLines;
            slot.WorkUnit.LogLines = logLines;
            slots.Add(slot);

            // setup slot
            slot = new SlotModel();
            slot.Prefs = preferences;
            // Test For - Issue 201 - Web Generation Fails when a Client with no CurrentLogLines is encountered.
            // Make sure we return null for CurrentLogLines in the second SlotModel.
            slot.Settings = new ClientSettings {Name = "Test1"};
            slot.CurrentLogLines = null;
            slots.Add(slot);

            return slots;
        }
    }
}
