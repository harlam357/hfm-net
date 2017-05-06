
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using NUnit.Framework;

namespace HFM.Preferences.Tests
{
   [TestFixture]
   public class PreferenceSetTests
   {
      [Test]
      public void PreferenceSet_Get_Test()
      {
         var prefs = new PreferenceSet();

         Assert.AreEqual(360, prefs.Get<int>(Preference.FormSplitterLocation));
         Assert.AreEqual(String.Empty, prefs.Get<string>(Preference.EmailReportingFromAddress));
         Assert.AreEqual("logcache", prefs.Get<string>(Preference.CacheFolder));
         var task1 = prefs.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
         var task2 = prefs.Get<ClientRetrievalTask>(Preference.ClientRetrievalTask);
         Assert.AreNotSame(task1, task2);
         Assert.AreEqual(new ClientRetrievalTask(), task1);
         Assert.IsNull(prefs.Get<ICollection<string>>(Preference.FormColumns));
         var graphColors1 = prefs.Get<IEnumerable<Color>>(Preference.GraphColors);
         var graphColors2 = prefs.Get<IEnumerable<Color>>(Preference.GraphColors);
         Assert.AreNotSame(graphColors1, graphColors2);
      }

      [Test]
      public void PreferenceSet_Get_Benchmark_ValueType_Test()
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
      public void PreferenceSet_Get_Benchmark_Null_String_Test()
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
      public void PreferenceSet_Get_Benchmark_String_Test()
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
      public void PreferenceSet_Get_Benchmark_Class_Test()
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
      public void PreferenceSet_Get_Benchmark_Null_Collection_Test()
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
      public void PreferenceSet_Get_Benchmark_Collection_Test()
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
         var prefs = new PreferenceSet();

         prefs.Set(Preference.FormSplitterLocation, (object)null);
         prefs.Set(Preference.FormSplitterLocation, "60");
         prefs.Set(Preference.FormSplitterLocation, 120);
         prefs.Set(Preference.FormSplitterLocation, 360);
      }

      [Test]
      public void PreferenceSet_Set_String_Test()
      {
         var prefs = new PreferenceSet();

         prefs.Set(Preference.EmailReportingFromAddress, (string)null);
         prefs.Set(Preference.EmailReportingFromAddress, "someone@home.com");
         prefs.Set(Preference.EmailReportingFromAddress, "me@gmail.com");
      }

      [Test]
      public void PreferenceSet_Set_Class_Test()
      {
         var prefs = new PreferenceSet();

         prefs.Set(Preference.ClientRetrievalTask, (ClientRetrievalTask)null);
         prefs.Set(Preference.ClientRetrievalTask, new ClientRetrievalTask());
         prefs.Set(Preference.ClientRetrievalTask, new ClientRetrievalTask { Enabled = false });
      }

      [Test]
      public void PreferenceSet_Set_Collection_Test()
      {
         var prefs = new PreferenceSet();

         prefs.Set(Preference.FormColumns, (List<string>)null);
         prefs.Set(Preference.FormColumns, (IEnumerable<string>)new[] { "a", "b", "c" });
         prefs.Set(Preference.FormColumns, (ICollection<string>)new[] { "a", "b", "c" });
      }
   }
}
