using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

using HFM.Preferences.Data;

using NUnit.Framework;

namespace HFM.Preferences
{
    public partial class PreferencesProviderTests
    {
        [Test]
        public void PreferencesProvider_Get_Test()
        {
            var prefs = new MockPreferencesProvider();

            // value type
            Assert.AreEqual(360, prefs.Get<int>(Preference.FormSplitterLocation));
            // underlying string is null
            Assert.AreEqual(String.Empty, prefs.Get<string>(Preference.EmailReportingFromAddress));
            // string
            Assert.AreEqual("logcache", prefs.Get<string>(Preference.CacheFolder));
            // class
            var task1 = prefs.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
            var task2 = prefs.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
            Assert.AreNotSame(task1, task2);
            Assert.AreEqual(new ClientRetrievalTask(), task1);
            // underlying collection is null
            Assert.IsNull(prefs.Get<ICollection<string>>(Preference.FormColumns));
            // collection
            var graphColors1 = prefs.Get<IEnumerable<Color>>(Preference.GraphColors);
            var graphColors2 = prefs.Get<IEnumerable<Color>>(Preference.GraphColors);
            Assert.AreNotSame(graphColors1, graphColors2);
        }

        [Test]
        public void PreferencesProvider_Get_ThrowsOnDataTypeMismatch_Test()
        {
            // Arrange
            var prefs = new MockPreferencesProvider();
            // Act & Assert
            Assert.Throws<ArgumentException>(() => prefs.Get<int>(Preference.CacheFolder));
        }

        [Test]
        public void PreferencesProvider_Get_NullStringAsEnum_ReturnsEnumDefaultWhenValueIsNull_Test()
        {
            // Arrange
            var data = new PreferenceData();
            data.WebDeployment.FtpMode = null;
            var prefs = new MockPreferencesProvider(data);
            // Act
            var value = prefs.Get<FtpMode>(Preference.WebGenFtpMode);
            // Assert
            Assert.AreEqual(FtpMode.Default, value);
        }

        [Test]
        public void PreferencesProvider_Get_StringAsEnum_ReturnsEnumDefaultWhenParsingFails_Test()
        {
            // Arrange
            var data = new PreferenceData();
            data.ApplicationSettings.BonusCalculation = "Foo";
            var prefs = new MockPreferencesProvider(data);
            // Act
            var value = prefs.Get<BonusCalculation>(Preference.BonusCalculation);
            // Assert
            Assert.AreEqual(BonusCalculation.Default, value);
        }

        [Test]
        public void PreferencesProvider_Get_StringAsEnum_Test()
        {
            // Arrange
            var data = new PreferenceData();
            data.ApplicationSettings.BonusCalculation = "DownloadTime";
            var prefs = new MockPreferencesProvider(data);
            // Act
            var value = prefs.Get<BonusCalculation>(Preference.BonusCalculation);
            // Assert
            Assert.AreEqual(BonusCalculation.DownloadTime, value);
        }

        [Test]
        public void PreferencesProvider_Get_Int32AsEnum_Test()
        {
            // Arrange
            var data = new PreferenceData();
            data.ApplicationSettings.MessageLevel = (int)LoggerLevel.Info;
            var prefs = new MockPreferencesProvider(data);
            // Act
            var value = prefs.Get<LoggerLevel>(Preference.MessageLevel);
            // Assert
            Assert.AreEqual(LoggerLevel.Info, value);
        }

        private enum LoggerLevel
        {
            Off = 0,
            Error = 2,
            Warn = 3,
            Info = 4,
            Debug = 5
        }

        [Test]
        public void PreferencesProvider_Get_ValueType_Benchmark()
        {
            var prefs = new MockPreferencesProvider();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                // ReSharper disable once UnusedVariable
                var obj = prefs.Get<int>(Preference.FormSplitterLocation);
            }
            sw.Stop();
            Debug.WriteLine("Get ValueType: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PreferencesProvider_Get_NullString_Benchmark()
        {
            var prefs = new MockPreferencesProvider();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                // ReSharper disable once UnusedVariable
                var obj = prefs.Get<string>(Preference.EmailReportingFromAddress);
            }
            sw.Stop();
            Debug.WriteLine("Get Null String: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PreferencesProvider_Get_String_Benchmark()
        {
            var prefs = new MockPreferencesProvider();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                // ReSharper disable once UnusedVariable
                var obj = prefs.Get<string>(Preference.CacheFolder);
            }
            sw.Stop();
            Debug.WriteLine("Get String: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PreferencesProvider_Get_Class_Benchmark()
        {
            var prefs = new MockPreferencesProvider();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                // ReSharper disable once UnusedVariable
                var obj = prefs.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
            }
            sw.Stop();
            Debug.WriteLine("Get Class: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PreferencesProvider_Get_NullCollection_Benchmark()
        {
            var prefs = new MockPreferencesProvider();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                // ReSharper disable once UnusedVariable
                var obj = prefs.Get<ICollection<string>>(Preference.FormColumns);
            }
            sw.Stop();
            Debug.WriteLine("Get Null Collection: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PreferencesProvider_Get_Collection_Benchmark()
        {
            var prefs = new MockPreferencesProvider();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++)
            {
                // ReSharper disable once UnusedVariable
                var obj = prefs.Get<IEnumerable<Color>>(Preference.GraphColors);
            }
            sw.Stop();
            Debug.WriteLine("Get Collection: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PreferencesProvider_Get_SetsDefaultPreferencesDataPropertyValueWhenItIsNull()
        {
            // Arrange
            var data = new PreferenceData();
            data.MainWindowGrid = null;
            var preferences = new MockPreferencesProvider(data);
            // Act
            var columns = preferences.Get<ICollection<string>>(Preference.FormColumns);
            var sortOrder = preferences.Get<ListSortDirection>(Preference.FormSortOrder);
            var sortColumn = preferences.Get<string>(Preference.FormSortColumn);
            // Assert
            Assert.IsNull(columns);
            Assert.AreEqual(ListSortDirection.Ascending, sortOrder);
            Assert.AreEqual(String.Empty, sortColumn);
        }
    }
}
