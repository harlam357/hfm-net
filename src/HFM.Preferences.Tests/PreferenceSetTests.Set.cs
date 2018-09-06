
using System.Collections.Generic;

using HFM.Preferences.Data;

using NUnit.Framework;

namespace HFM.Preferences
{
   public partial class PreferenceSetTests
   {
      [Test]
      public void PreferenceSet_Set_ValueType_Test()
      {
         var data = new PreferenceData();
         var prefs = new InMemoryPreferenceSet(data);

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
         var prefs = new InMemoryPreferenceSet(data);

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
         var prefs = new InMemoryPreferenceSet(data);
         // Act
         prefs.Set(Preference.BonusCalculation, BonusCalculation.Default);
         // Assert
         Assert.AreEqual("Default", data.ApplicationSettings.BonusCalculation);
      }

      [Test]
      public void PreferenceSet_Set_Class_Test()
      {
         var data = new PreferenceData();
         var prefs = new InMemoryPreferenceSet(data);

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
         var prefs = new InMemoryPreferenceSet(data);

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
   }
}
