
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using NUnit.Framework;

using HFM.Preferences.Data;

namespace HFM.Preferences
{
   [TestFixture]
   public partial class PreferenceSetTests
   {
      [Test]
      public void PreferenceSet_ConstructorArgumentValues_Test()
      {
         // Arrange
         using (var artifacts = new ArtifactFolder())
         {
            const string applicationVersion = "1.0.0.0";
            string applicationPath = Environment.CurrentDirectory;
            // Act
            var prefs = new PreferenceSet(applicationPath, artifacts.Path, applicationVersion);
            // Assert
            Assert.AreEqual(applicationPath, prefs.ApplicationPath);
            Assert.AreEqual(applicationPath, prefs.Get<string>(Preference.ApplicationPath));
            Assert.AreEqual(artifacts.Path, prefs.ApplicationDataFolderPath);
            Assert.AreEqual(artifacts.Path, prefs.Get<string>(Preference.ApplicationDataFolderPath));
            Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
            Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            string cacheDirectory = System.IO.Path.Combine(artifacts.Path, "logcache");
            Assert.AreEqual(cacheDirectory, prefs.Get<string>(Preference.CacheDirectory));
         }
      }

      [Test]
      public void PreferenceSet_Reset_Test()
      {
         // Arrange
         using (var artifacts = new ArtifactFolder())
         {
            const string applicationVersion = "1.0.0.0";
            string configPath = System.IO.Path.Combine(artifacts.Path, "config.xml");
            var prefs = Create(artifacts.Path, applicationVersion);
            Assert.IsFalse(System.IO.File.Exists(configPath));
            // Act
            prefs.Reset();
            // Assert
            Assert.IsTrue(System.IO.File.Exists(configPath));
            Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
            Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
         }
      }

      [Test]
      public void PreferenceSet_Load_LoadsDefaultsWhenNoConfigXmlOrUserSettingsExists_Test()
      {
         // Arrange
         using (var artifacts = new ArtifactFolder())
         {
            const string applicationVersion = "1.0.0.0";
            string configPath = System.IO.Path.Combine(artifacts.Path, "config.xml");
            var prefs = Create(artifacts.Path, applicationVersion);
            Assert.IsFalse(System.IO.File.Exists(configPath));
            // Act
            prefs.Load();
            // Assert
            Assert.IsTrue(System.IO.File.Exists(configPath));
            Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
            Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
         }
      }

      [Test]
      public void PreferenceSet_Load_MigratesFromUserSettings_Test()
      {
         // Arrange
         using (var artifacts = new ArtifactFolder())
         {
            const string applicationVersion = "1.0.0.0";
            string configPath = System.IO.Path.Combine(artifacts.Path, "config.xml");
            var prefs = new TestMigrateFromUserSettingsPreferenceSet(artifacts.Path, applicationVersion);
            Assert.IsFalse(System.IO.File.Exists(configPath));
            // Act
            prefs.Load();
            // Assert
            Assert.IsTrue(System.IO.File.Exists(configPath));
            Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
            Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            Assert.AreEqual("Foo", prefs.Get<string>(Preference.StanfordId));
         }
      }

      private class TestMigrateFromUserSettingsPreferenceSet : PreferenceSet
      {
         public TestMigrateFromUserSettingsPreferenceSet(string applicationDataFolderPath, string applicationVersion)
            : base(Environment.CurrentDirectory, applicationDataFolderPath, applicationVersion)
         {

         }

         protected override PreferenceData OnMigrateFromUserSettings()
         {
            var data = new PreferenceData();
            data.UserSettings.StanfordId = "Foo";
            return data;
         }
      }

      [Test]
      public void PreferenceSet_Load_ExecutesPreferenceUpgrades_Test()
      {
         // Arrange
         using (var artifacts = new ArtifactFolder())
         {
            const string applicationVersion = "1.0.0.0";
            string configPath = System.IO.Path.Combine(artifacts.Path, "config.xml");
            var prefs = new TestPreferenceUpgradePreferenceSet(artifacts.Path, applicationVersion);
            Assert.IsFalse(System.IO.File.Exists(configPath));
            // Act
            prefs.Load();
            // Assert
            Assert.IsTrue(System.IO.File.Exists(configPath));
            Assert.AreEqual(applicationVersion, prefs.ApplicationVersion);
            Assert.AreEqual(applicationVersion, prefs.Get<string>(Preference.ApplicationVersion));
            Assert.AreEqual("foo", prefs.Get<string>(Preference.CacheFolder));
         }
      }

      private class TestPreferenceUpgradePreferenceSet : PreferenceSet
      {
         public TestPreferenceUpgradePreferenceSet(string applicationDataFolderPath, string applicationVersion)
            : base(Environment.CurrentDirectory, applicationDataFolderPath, applicationVersion)
         {

         }

         protected override PreferenceData OnRead()
         {
            var data = new PreferenceData();
            data.ApplicationVersion = "0.0.0.0";
            return data;
         }

         protected override IEnumerable<PreferenceUpgrade> OnEnumerateUpgrades()
         {
            yield return new PreferenceUpgrade
            {
               Version = new Version(0, 1, 0),
               Action = data => { data.ApplicationSettings.CacheFolder = "foo"; }
            };
         }
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

      private class InMemoryPreferenceSet : PreferenceSetBase
      {
         public InMemoryPreferenceSet() 
            : base(Environment.CurrentDirectory, Environment.CurrentDirectory, "1.0.0.0")
         {

         }

         public InMemoryPreferenceSet(PreferenceData data)
            : this()
         {
            Load(data);
         }
      }

      private static PreferenceSet Create(string applicationDataFolderPath, string applicationVersion)
      {
         var path = Environment.CurrentDirectory;
         return new PreferenceSet(path, applicationDataFolderPath, applicationVersion);
      }
   }
}
