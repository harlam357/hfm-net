using System.Drawing;

using HFM.Preferences;

using NUnit.Framework;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class MainModelTests
    {
        [Test]
        public void MainModel_Load_FromPreferences()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            preferences.Set(Preference.FormLocation, new Point(10, 20));
            preferences.Set(Preference.FormSize, new Size(30, 40));
            preferences.Set(Preference.FormLogWindowVisible, true);
            preferences.Set(Preference.FormLogWindowHeight, 11);
            preferences.Set(Preference.FormSplitterLocation, 31);
            preferences.Set(Preference.QueueWindowVisible, true);
            preferences.Set(Preference.FollowLog, true);
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(new Point(10, 20), model.FormLocation);
            Assert.AreEqual(new Size(30, 40), model.FormSize);
            Assert.AreEqual(true, model.FormLogWindowVisible);
            Assert.AreEqual(11, model.FormLogWindowHeight);
            Assert.AreEqual(31, model.FormSplitterLocation);
            Assert.AreEqual(true, model.QueueWindowVisible);
            Assert.AreEqual(true, model.FollowLog);
        }

        [Test]
        public void MainModel_Save_ToPreferences()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            model.FormLocation = new Point(50, 60);
            model.FormSize = new Size(70, 80);
            model.FormLogWindowVisible = true;
            model.FormLogWindowHeight = 11;
            model.FormSplitterLocation = 31;
            model.QueueWindowVisible = true;
            model.FollowLog = true;
            // Act
            model.Save();
            // Assert
            Assert.AreEqual(new Point(50, 60), preferences.Get<Point>(Preference.FormLocation));
            Assert.AreEqual(new Size(70, 80), preferences.Get<Size>(Preference.FormSize));
            Assert.AreEqual(true, preferences.Get<bool>(Preference.FormLogWindowVisible));
            Assert.AreEqual(11, preferences.Get<int>(Preference.FormLogWindowHeight));
            Assert.AreEqual(31, preferences.Get<int>(Preference.FormSplitterLocation));
            Assert.AreEqual(true, preferences.Get<bool>(Preference.QueueWindowVisible));
            Assert.AreEqual(true, preferences.Get<bool>(Preference.FollowLog));
        }

        private static MainModel CreateModel()
        {
            return new MainModel(null);
        }
    }
}
