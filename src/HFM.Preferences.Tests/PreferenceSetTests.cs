
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using NUnit.Framework;

using HFM.Preferences.Data;

namespace HFM.Preferences.Tests
{
   [TestFixture]
   public class PreferenceSetTests
   {
      [Test]
      public void PreferenceSet_Get_Test()
      {
         var prefs = new PreferenceSet();

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
      public void PreferenceSet_Get_ConstructorValues_Test()
      {
         // Arrange
         var prefs = new PreferenceSet("AppPath", "DataPath", "1.0.0");
         // Assert
         Assert.AreEqual(prefs.ApplicationPath, prefs.Get<string>(Preference.ApplicationPath));
         Assert.AreEqual(prefs.ApplicationDataFolderPath, prefs.Get<string>(Preference.ApplicationDataFolderPath));
         string cacheFolderPath = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, "logcache");
         Assert.AreEqual(cacheFolderPath, prefs.Get<string>(Preference.CacheDirectory));
      }

      [Test]
      public void PreferenceSet_Get_StringAsEnum_ReturnsEnumDefaultWhenParsingFails_Test()
      {
         // Arrange
         var data = new PreferenceData();
         data.ApplicationSettings.BonusCalculation = "Foo";
         var prefs = new PreferenceSet(data);
         // Act
         var value = prefs.Get<BonusCalculation>(Preference.BonusCalculation);
         // Assert
         Assert.AreEqual(BonusCalculation.Default, value);
      }

      [Test]
      public void PreferenceSet_Get_StringAsEnum_Test()
      {
         // Arrange
         var prefs = new PreferenceSet();
         // Act
         var value = prefs.Get<BonusCalculation>(Preference.BonusCalculation);
         // Assert
         Assert.AreEqual(BonusCalculation.DownloadTime, value);
      }

      [Test]
      public void PreferenceSet_Get_ValueType_Benchmark()
      {
         var prefs = new PreferenceSet();

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
      public void PreferenceSet_Get_NullString_Benchmark()
      {
         var prefs = new PreferenceSet();

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
      public void PreferenceSet_Get_String_Benchmark()
      {
         var prefs = new PreferenceSet();

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
      public void PreferenceSet_Get_Class_Benchmark()
      {
         var prefs = new PreferenceSet();

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
      public void PreferenceSet_Get_NullCollection_Benchmark()
      {
         var prefs = new PreferenceSet();

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
      public void PreferenceSet_Get_Collection_Benchmark()
      {
         var prefs = new PreferenceSet();

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
      public void PreferenceSet_Set_ValueType_Test()
      {
         var data = new PreferenceData();
         var prefs = new PreferenceSet(data);

         prefs.Set(Preference.FormSplitterLocation, (object)null);
         Assert.AreEqual(0, data.MainWindowState.SplitterLocation);
         prefs.Set(Preference.FormSplitterLocation, "60");
         Assert.AreEqual(60, data.MainWindowState.SplitterLocation);
         prefs.Set(Preference.FormSplitterLocation, 120);
         Assert.AreEqual(120, data.MainWindowState.SplitterLocation);
         prefs.Set(Preference.FormSplitterLocation, 360);
         Assert.AreEqual(360, data.MainWindowState.SplitterLocation);
      }

      [Test]
      public void PreferenceSet_Set_String_Test()
      {
         var data = new PreferenceData();
         var prefs = new PreferenceSet(data);

         prefs.Set(Preference.EmailReportingFromAddress, (string)null);
         Assert.AreEqual(null, data.Email.FromAddress);
         prefs.Set(Preference.EmailReportingFromAddress, "someone@home.com");
         Assert.AreEqual("someone@home.com", data.Email.FromAddress);
         prefs.Set(Preference.EmailReportingFromAddress, "me@gmail.com");
         Assert.AreEqual("me@gmail.com", data.Email.FromAddress);
      }

      [Test]
      public void PreferenceSet_Set_StringAsEnum_Test()
      {
         // Arrange
         var data = new PreferenceData();
         var prefs = new PreferenceSet(data);
         // Act
         prefs.Set(Preference.BonusCalculation, BonusCalculation.Default);
         // Assert
         Assert.AreEqual("Default", data.ApplicationSettings.BonusCalculation);
      }

      [Test]
      public void PreferenceSet_Set_Class_Test()
      {
         var data = new PreferenceData();
         var prefs = new PreferenceSet(data);

         ClientRetrievalTask task = null;
         prefs.Set(Preference.ClientRetrievalTask, task);
         Assert.AreEqual(null, data.ClientRetrievalTask);
         task = new ClientRetrievalTask();
         prefs.Set(Preference.ClientRetrievalTask, task);
         Assert.AreNotSame(task, data.ClientRetrievalTask);
         task = new ClientRetrievalTask { Enabled = false };
         prefs.Set(Preference.ClientRetrievalTask, task);
         Assert.AreNotSame(task, data.ClientRetrievalTask);
      }

      [Test]
      public void PreferenceSet_Set_Collection_Test()
      {
         var data = new PreferenceData();
         var prefs = new PreferenceSet(data);

         prefs.Set(Preference.FormColumns, (List<string>)null);
         Assert.AreEqual(null, data.MainWindowGrid.Columns);
         var enumerable = (IEnumerable<string>)new[] { "a", "b", "c" };
         prefs.Set(Preference.FormColumns, enumerable);
         Assert.AreEqual(3, data.MainWindowGrid.Columns.Count);
         Assert.AreNotSame(enumerable, data.MainWindowGrid.Columns);
         var collection = (ICollection<string>)new[] { "a", "b", "c" };
         prefs.Set(Preference.FormColumns, collection);
         Assert.AreEqual(3, data.MainWindowGrid.Columns.Count);
         Assert.AreNotSame(collection, data.MainWindowGrid.Columns);
      }

      private enum BonusCalculation
      {
         Default,
         DownloadTime
      }
   }
}
