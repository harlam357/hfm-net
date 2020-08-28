using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Presenters.Mocks;
using HFM.Forms.Views;
using HFM.Preferences;
using NUnit.Framework;

namespace HFM.Forms.Presenters
{
    [TestFixture]
    public class BenchmarkPresenterTests
    {
        [Test]
        public void BenchmarkPresenter_Show_ShowsView()
        {
            // Arrange
            var model = CreateModel();
            var presenter = new MockFormBenchmarksPresenter(model);
            // Act
            presenter.Show();
            // Assert
            Assert.IsTrue(presenter.MockForm.Shown);
        }

        [Test]
        public void BenchmarkPresenter_DeleteSlotClicked_AsksYesNoQuestionAndExitsAfterNoAnswer()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(benchmarkService);
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.No);
            var presenter = new MockFormBenchmarksPresenter(model, messageBox);
            presenter.Show();
            Assert.AreEqual(2, presenter.Model.SlotIdentifiers.Count);
            presenter.Model.SelectedSlotIdentifier = presenter.Model.SlotIdentifierValueItems.Last();
            // Act
            presenter.DeleteSlotClicked();
            // Assert
            Assert.AreEqual(2, presenter.Model.SlotIdentifiers.Count);
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void BenchmarkPresenter_DeleteSlotClicked_AsksYesNoQuestionAndDeletesSlot()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(benchmarkService);
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = new MockFormBenchmarksPresenter(model, messageBox);
            presenter.Show();
            Assert.AreEqual(2, presenter.Model.SlotIdentifiers.Count);
            presenter.Model.SelectedSlotIdentifier = presenter.Model.SlotIdentifierValueItems.Last();
            // Act
            presenter.DeleteSlotClicked();
            // Assert
            Assert.AreEqual(1, presenter.Model.SlotIdentifiers.Count);
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void BenchmarkPresenter_DeleteProjectClicked_AsksYesNoQuestionAndExitsAfterNoAnswer()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.No);
            var presenter = new MockFormBenchmarksPresenter(model, messageBox);
            presenter.Show();
            presenter.Model.SelectedSlotIdentifier = presenter.Model.SlotIdentifierValueItems.Last();
            Assert.AreEqual(2, presenter.Model.SlotProjects.Count);
            presenter.Model.SelectedSlotProjectListItems.Clear();
            presenter.Model.SelectedSlotProjectListItems.Add(presenter.Model.SlotProjectListItems.Last());
            // Act
            presenter.DeleteProjectClicked();
            // Assert
            Assert.AreEqual(2, presenter.Model.SlotProjects.Count);
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void BenchmarkPresenter_DeleteProjectClicked_AsksYesNoQuestionAndDeletesSlot()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = new MockFormBenchmarksPresenter(model, messageBox);
            presenter.Show();
            presenter.Model.SelectedSlotIdentifier = presenter.Model.SlotIdentifierValueItems.Last();
            Assert.AreEqual(2, presenter.Model.SlotProjects.Count);
            presenter.Model.SelectedSlotProjectListItems.Clear();
            presenter.Model.SelectedSlotProjectListItems.Add(presenter.Model.SlotProjectListItems.Last());
            // Act
            presenter.DeleteProjectClicked();
            // Assert
            Assert.AreEqual(1, presenter.Model.SlotProjects.Count);
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void BenchmarkPresenter_DeleteGraphColorClicked_ShowsMessageBoxWhenSelectedGraphColorItemIsNull()
        {
            // Arrange
            var model = CreateModel();
            model.Preferences.Set(Preference.GraphColors, new List<Color>());
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = new MockFormBenchmarksPresenter(model, messageBox);
            presenter.Show();
            Assert.IsNull(presenter.Model.SelectedGraphColorItem);
            // Act
            presenter.DeleteGraphColorClicked();
            // Assert
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void BenchmarkPresenter_DeleteGraphColorClicked_ShowsMessageBoxWhenThreeOrLessGraphColors()
        {
            // Arrange
            var model = CreateModel();
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            var color2 = Color.Yellow;
            model.Preferences.Set(Preference.GraphColors, new List<Color> { color0, color1, color2 });
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = new MockFormBenchmarksPresenter(model, messageBox);
            presenter.Show();
            Assert.AreEqual(3, presenter.Model.GraphColors.Count);
            // Act
            presenter.DeleteGraphColorClicked();
            // Assert
            Assert.AreEqual(3, presenter.Model.GraphColors.Count);
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void BenchmarkPresenter_DeleteGraphColorClicked_DeletesColor()
        {
            // Arrange
            var model = CreateModel();
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            var color2 = Color.Yellow;
            var color3 = Color.Red;
            model.Preferences.Set(Preference.GraphColors, new List<Color> { color0, color1, color2, color3 });
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = new MockFormBenchmarksPresenter(model, messageBox);
            presenter.Show();
            Assert.AreEqual(4, presenter.Model.GraphColors.Count);
            // Act
            presenter.DeleteGraphColorClicked();
            // Assert
            Assert.AreEqual(3, presenter.Model.GraphColors.Count);
            Assert.AreEqual(0, messageBox.Invocations.Count);
        }

        private static SlotIdentifier CreateSlotIdentifier(string name, int slotID)
        {
            return new SlotIdentifier(new ClientIdentifier(name, "Server", ClientSettings.DefaultPort, Guid.NewGuid()), slotID);
        }

        private static IProteinBenchmarkService CreateBenchmarkServiceWithOneSlotAndProject()
        {
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var slotIdentifier = CreateSlotIdentifier("Test", SlotIdentifier.NoSlotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(12345);
            benchmarkService.Update(slotIdentifier, benchmarkIdentifier, Array.Empty<TimeSpan>());
            return benchmarkService;
        }

        private static IProteinBenchmarkService CreateBenchmarkServiceWithTwoSlotsAndProjects()
        {
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());

            var slotIdentifier = CreateSlotIdentifier("Test", 0);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(12345);
            benchmarkService.Update(slotIdentifier, benchmarkIdentifier, Array.Empty<TimeSpan>());

            slotIdentifier = CreateSlotIdentifier("Test", 1);
            benchmarkIdentifier = new ProteinBenchmarkIdentifier(23456);
            benchmarkService.Update(slotIdentifier, benchmarkIdentifier, Array.Empty<TimeSpan>());
            benchmarkIdentifier = new ProteinBenchmarkIdentifier(65432);
            benchmarkService.Update(slotIdentifier, benchmarkIdentifier, Array.Empty<TimeSpan>());

            return benchmarkService;
        }

        private static BenchmarksModel CreateModel(IProteinBenchmarkService benchmarkService = null)
        {
            return new BenchmarksModel(null, null, benchmarkService, null);
        }

        private class MockFormBenchmarksPresenter : BenchmarksPresenter
        {
            public MockFormBenchmarksPresenter(BenchmarksModel model) : base(model, null, null)
            {

            }

            public MockFormBenchmarksPresenter(BenchmarksModel model, MessageBoxPresenter messageBox) : base(model, null, messageBox)
            {

            }

            public MockWin32Form MockForm => Form as MockWin32Form;

            protected override IWin32Form OnCreateForm()
            {
                return new MockWin32Form();
            }
        }
    }
}
