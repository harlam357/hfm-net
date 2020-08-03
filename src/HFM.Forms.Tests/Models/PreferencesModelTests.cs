
using System;

using NUnit.Framework;

using HFM.Preferences;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class PreferencesModelTests
    {
        [Test]
        public void PreferencesModel_DefaultPropertyValues()
        {
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            Assert.IsNotNull(model.Preferences);
            Assert.IsNotNull(model.ClientsModel);
            Assert.IsNotNull(model.OptionsModel);
            Assert.IsNotNull(model.WebGenerationModel);
            Assert.IsNotNull(model.WebVisualStylesModel);
            Assert.IsNotNull(model.ReportingModel);
            Assert.IsNotNull(model.WebProxyModel);
        }

        [Test]
        public void PreferencesModel_HasNoError()
        {
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            Assert.AreEqual(String.Empty, model.Error);
            Assert.IsFalse(model.HasError);
        }

        [Test]
        public void PreferencesModel_HasError()
        {
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            model.WebProxyModel.Enabled = true;
            Assert.AreNotEqual(String.Empty, model.Error);
            Assert.IsTrue(model.HasError);
        }

        [Test]
        public void PreferencesModel_ValidateAcceptance_RaisesPropertyChanged()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            string propertyName = null;
            model.PropertyChanged += (s, e) => propertyName = e.PropertyName;
            // Act
            model.ValidateAcceptance();
            // Assert
            Assert.AreEqual(String.Empty, propertyName);
        }

        [Test]
        public void PreferencesModel_ValidateAcceptance_ReturnsTrueWhenModelHasNoError()
        {
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            Assert.IsTrue(model.ValidateAcceptance());
        }

        [Test]
        public void PreferencesModel_ValidateAcceptance_ReturnsFalseWhenModelHasError()
        {
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            model.WebProxyModel.Enabled = true;
            Assert.IsFalse(model.ValidateAcceptance());
        }

        [Test]
        public void PreferencesModel_Load_FromPreferences()
        {
            // Arrange
            var preferences = new InMemoryPreferenceSet();
            preferences.Set(Preference.UseProxy, true);
            var model = new PreferencesModel(preferences, new InMemoryAutoRunConfiguration());
            // Act
            model.Load();
            // Assert
            Assert.IsTrue(model.WebProxyModel.Enabled);
        }

        [Test]
        public void PreferencesModel_Save_ToPreferences()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            model.WebProxyModel.Enabled = true;
            // Act
            model.Save();
            // Assert
            Assert.IsTrue(model.Preferences.Get<bool>(Preference.UseProxy));
        }

        [Test]
        public void PreferencesModel_Load_FromAutoRunConfiguration()
        {
            // Arrange
            var autoRunConfiguration = new InMemoryAutoRunConfiguration();
            autoRunConfiguration.SetFilePath("foo");
            var model = new PreferencesModel(new InMemoryPreferenceSet(), autoRunConfiguration);
            // Act
            model.Load();
            // Assert
            Assert.IsTrue(model.OptionsModel.AutoRun);
        }

        [Test]
        public void PreferencesModel_Save_ToAutoRunConfiguration()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            model.OptionsModel.AutoRun = true;
            // Act
            model.Save();
            // Assert
            Assert.IsTrue(model.OptionsModel.AutoRunConfiguration.IsEnabled());
        }
    }
}
