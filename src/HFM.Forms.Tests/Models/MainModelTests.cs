using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Client;
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
            preferences.Set(Preference.QueueSplitterLocation, 200);
            preferences.Set(Preference.FollowLog, true);
            preferences.Set(Preference.FormColumns, new List<string> { "A", "B", "C" });
            preferences.Set(Preference.MinimizeTo, MinimizeToOption.Both);
            preferences.Set(Preference.ColorLogFile, true);
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(new Point(10, 20), model.FormLocation);
            Assert.AreEqual(new Size(30, 40), model.FormSize);
            Assert.AreEqual(true, model.FormLogWindowVisible);
            Assert.AreEqual(11, model.FormLogWindowHeight);
            Assert.AreEqual(31, model.FormSplitterLocation);
            Assert.AreEqual(true, model.QueueWindowVisible);
            Assert.AreEqual(200, model.QueueSplitterLocation);
            Assert.AreEqual(true, model.FollowLog);
            CollectionAssert.AreEqual(new List<string> { "A", "B", "C" }, model.FormColumns);
            Assert.AreEqual(MinimizeToOption.Both, model.MinimizeTo);
            Assert.AreEqual(true, model.ColorLogFile);
        }

        [Test]
        public void MainModel_PreferenceChanged_UpdatesMinimizeTo()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            model.Load();
            // Act
            preferences.Set(Preference.MinimizeTo, MinimizeToOption.TaskBar);
            // Assert
            Assert.AreEqual(MinimizeToOption.TaskBar, model.MinimizeTo);
        }

        [Test]
        public void MainModel_PreferenceChanged_UpdatesColorLogFile()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            model.Load();
            // Act
            preferences.Set(Preference.ColorLogFile, false);
            // Assert
            Assert.AreEqual(false, model.ColorLogFile);
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
            model.QueueSplitterLocation = 200;
            model.FollowLog = true;
            model.FormColumns.Reset(new List<string> { "A", "B", "C" });
            // Act
            model.Save();
            // Assert
            Assert.AreEqual(new Point(50, 60), preferences.Get<Point>(Preference.FormLocation));
            Assert.AreEqual(new Size(70, 80), preferences.Get<Size>(Preference.FormSize));
            Assert.AreEqual(true, preferences.Get<bool>(Preference.FormLogWindowVisible));
            Assert.AreEqual(11, preferences.Get<int>(Preference.FormLogWindowHeight));
            Assert.AreEqual(31, preferences.Get<int>(Preference.FormSplitterLocation));
            Assert.AreEqual(true, preferences.Get<bool>(Preference.QueueWindowVisible));
            Assert.AreEqual(200, preferences.Get<int>(Preference.QueueSplitterLocation));
            Assert.AreEqual(true, preferences.Get<bool>(Preference.FollowLog));
            CollectionAssert.AreEqual(new List<string> { "A", "B", "C" }, preferences.Get<ICollection<string>>(Preference.FormColumns));
        }

        [Test]
        public void MainModel_GridModelSelectedSlotChanged_SetsClientDetailsNullWhenSenderIsMainGridModelWithNullSelectedSlot()
        {
            // Arrange
            var model = CreateModel();
            model.ClientDetails = "foo";
            SlotModel selectedSlot = null;
            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            model.GridModelSelectedSlotChanged(selectedSlot);
            // Assert
            Assert.IsNull(model.ClientDetails);
        }

        [Test]
        public void MainModel_GridModelSelectedSlotChanged_SetsClientDetailsWhenSenderIsMainGridModelWithSelectedSlot()
        {
            // Arrange
            var model = CreateModel();
            var selectedSlot = new SlotModel(new NullClient
            {
                Settings = new ClientSettings { Server = "test", Port = ClientSettings.DefaultPort }
            });
            // Act
            model.GridModelSelectedSlotChanged(selectedSlot);
            // Assert
            Assert.AreEqual("test:36330", model.ClientDetails);
        }

        [Test]
        public void MainModel_GridModelSelectedSlotChanged_SetsClientDetailsWhenSenderIsMainGridModelWithSelectedSlotClientDisabled()
        {
            // Arrange
            var model = CreateModel();
            var selectedSlot = new SlotModel(new NullClient
            {
                Settings = new ClientSettings { Server = "test", Port = ClientSettings.DefaultPort, Disabled = true }
            });
            // Act
            model.GridModelSelectedSlotChanged(selectedSlot);
            // Assert
            Assert.AreEqual("test:36330 (Disabled)", model.ClientDetails);
        }

        [Test]
        public void MainModel_WindowState_PreviousValueSetToOriginalWindowState()
        {
            // Arrange
            var model = CreateModel();
            model.WindowState = FormWindowState.Maximized;
            // Act
            model.WindowState = FormWindowState.Minimized;
            // Assert
            Assert.AreEqual(FormWindowState.Maximized, model.OriginalWindowState);
        }

        [Test]
        public void MainModel_WindowState_RaisesPropertyChangedEvents()
        {
            // Arrange
            var model = CreateModel();
            var propertyNames = new List<string>();
            model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
            // Act
            model.WindowState = FormWindowState.Maximized;
            // Assert
            Assert.AreEqual(2, propertyNames.Count);
            var expected = new[] { nameof(MainModel.WindowState), nameof(MainModel.ShowInTaskbar) };
            CollectionAssert.AreEqual(expected, propertyNames);
        }

        [Test]
        public void MainModel_MinimizeTo_IsUpdatedWhenPreferenceChanges()
        {
            // Arrange
            var model = CreateModel();
            model.Load();
            Assert.AreEqual(default(MinimizeToOption), model.MinimizeTo);
            // Act
            model.Preferences.Set(Preference.MinimizeTo, MinimizeToOption.Both);
            // Assert
            Assert.AreEqual(MinimizeToOption.Both, model.MinimizeTo);
        }

        [Test]
        public void MainModel_MinimizeTo_RaisesPropertyChangedEvents()
        {
            // Arrange
            var model = CreateModel();
            var propertyNames = new List<string>();
            model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
            // Act
            model.MinimizeTo = MinimizeToOption.Both;
            // Assert
            Assert.AreEqual(3, propertyNames.Count);
            var expected = new[] { nameof(MainModel.MinimizeTo), nameof(MainModel.NotifyIconVisible), nameof(MainModel.ShowInTaskbar) };
            CollectionAssert.AreEqual(expected, propertyNames);
        }

        [Test]
        public void MainModel_NotifyIconVisible_ReturnsFalseWhenMinimizeToIsTaskBar()
        {
            // Arrange
            var model = CreateModel();
            model.MinimizeTo = MinimizeToOption.TaskBar;
            // Act & Assert
            Assert.IsFalse(model.NotifyIconVisible);
        }

        [Test]
        public void MainModel_NotifyIconVisible_ReturnsTrueWhenMinimizeToIsSystemTray()
        {
            // Arrange
            var model = CreateModel();
            model.MinimizeTo = MinimizeToOption.SystemTray;
            // Act & Assert
            Assert.IsTrue(model.NotifyIconVisible);
        }

        [Test]
        public void MainModel_NotifyIconVisible_ReturnsTrueWhenMinimizeToIsBoth()
        {
            // Arrange
            var model = CreateModel();
            model.MinimizeTo = MinimizeToOption.Both;
            // Act & Assert
            Assert.IsTrue(model.NotifyIconVisible);
        }

        [Test]
        public void MainModel_ShowInTaskbar_ReturnsTrueWhenWindowStateIsNotMinimized()
        {
            // Arrange
            var model = CreateModel();
            model.WindowState = FormWindowState.Normal;
            // Act & Assert
            Assert.IsTrue(model.ShowInTaskbar);
        }

        [Test]
        public void MainModel_ShowInTaskbar_ReturnsFalseWhenWindowStateIsMinimizedAndMinimizeToIsSystemTray()
        {
            // Arrange
            var model = CreateModel();
            model.WindowState = FormWindowState.Minimized;
            model.MinimizeTo = MinimizeToOption.SystemTray;
            // Act & Assert
            Assert.IsFalse(model.ShowInTaskbar);
        }

        [Test]
        public void MainModel_ShowInTaskbar_ReturnsTrueWhenWindowStateIsMinimizedAndMinimizeToIsTaskBar()
        {
            // Arrange
            var model = CreateModel();
            model.WindowState = FormWindowState.Minimized;
            model.MinimizeTo = MinimizeToOption.TaskBar;
            // Act & Assert
            Assert.IsTrue(model.ShowInTaskbar);
        }

        [Test]
        public void MainModel_ShowInTaskbar_ReturnsTrueWhenWindowStateIsMinimizedAndMinimizeToIsBoth()
        {
            // Arrange
            var model = CreateModel();
            model.WindowState = FormWindowState.Minimized;
            model.MinimizeTo = MinimizeToOption.Both;
            // Act & Assert
            Assert.IsTrue(model.ShowInTaskbar);
        }

        private static MainModel CreateModel()
        {
            return new MainModel(null);
        }
    }
}
