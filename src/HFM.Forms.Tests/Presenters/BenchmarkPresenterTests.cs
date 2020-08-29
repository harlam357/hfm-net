using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.Mocks;
using HFM.Core.Services;
using HFM.Core.WorkUnits;
using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Presenters.Mocks;
using HFM.Forms.Views;
using HFM.Preferences;
using HFM.Proteins;

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
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                presenter.Show();
                Assert.AreEqual(2, presenter.Model.SlotIdentifiers.Count);
                presenter.Model.SelectedSlotIdentifier = presenter.Model.SlotIdentifierValueItems.Last();
                // Act
                presenter.DeleteSlotClicked();
                // Assert
                Assert.AreEqual(2, presenter.Model.SlotIdentifiers.Count);
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
        }

        [Test]
        public void BenchmarkPresenter_DeleteSlotClicked_AsksYesNoQuestionAndDeletesSlot()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(benchmarkService);
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                presenter.Show();
                Assert.AreEqual(2, presenter.Model.SlotIdentifiers.Count);
                presenter.Model.SelectedSlotIdentifier = presenter.Model.SlotIdentifierValueItems.Last();
                // Act
                presenter.DeleteSlotClicked();
                // Assert
                Assert.AreEqual(1, presenter.Model.SlotIdentifiers.Count);
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
        }

        [Test]
        public void BenchmarkPresenter_DeleteProjectClicked_AsksYesNoQuestionAndExitsAfterNoAnswer()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.No);
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
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
        }

        [Test]
        public void BenchmarkPresenter_DeleteProjectClicked_AsksYesNoQuestionAndDeletesSlot()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
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
        }

        [Test]
        public void BenchmarkPresenter_DescriptionLinkClicked_StartsLocalProcess()
        {
            // Arrange
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(new Protein { ProjectNumber = 12345, Description = "http://someurl"});
            var proteinService = new ProteinService(dataContainer, null, null);
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarkService);
            using (var presenter = new MockFormBenchmarksPresenter(model))
            {
                presenter.Show();
                var localProcess = new MockLocalProcessService();
                // Act
                presenter.DescriptionLinkClicked(localProcess);
                // Assert
                Assert.AreEqual(1, presenter.Model.SlotProjects.Count);
                Assert.IsTrue(localProcess.Invocations.First().FileName == "http://someurl");
            }
        }

        [Test]
        public void BenchmarkPresenter_DescriptionLinkClicked_ShowsMessageBoxWhenLocalProcessFailsToStart()
        {
            // Arrange
            var model = CreateModel();
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                presenter.Show();
                var localProcess = new LocalProcessServiceThrows();
                // Act
                presenter.DescriptionLinkClicked(localProcess);
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void BenchmarkPresenter_AddGraphColorClicked_AddsNewColorToGraphColors()
        {
            // Arrange
            var model = CreateModel();
            model.Preferences.Set(Preference.GraphColors, new List<Color>());
            using (var presenter = new MockFormBenchmarksPresenter(model))
            {
                presenter.Show();
                var dialog = new MockColorDialogPresenter(window => DialogResult.OK);
                dialog.Color = Color.FromArgb(72, 134, 186);
                // Act
                presenter.AddGraphColorClicked(dialog);
                // Assert
                Assert.AreEqual(1, model.GraphColors.Count);
                Assert.AreEqual(Color.SteelBlue, model.SelectedGraphColorItem.Value);
            }
        }

        [Test]
        public void BenchmarkPresenter_AddGraphColorClicked_ShowsMessageBoxAndDoesNotAddExistingColor()
        {
            // Arrange
            var model = CreateModel();
            var color = Color.AliceBlue;
            model.Preferences.Set(Preference.GraphColors, new List<Color> { color });
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                presenter.Show();
                var dialog = new MockColorDialogPresenter(window => DialogResult.OK);
                dialog.Color = color;
                // Act
                presenter.AddGraphColorClicked(dialog);
                // Assert
                Assert.AreEqual(1, model.GraphColors.Count);
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
        }

        [Test]
        public void BenchmarkPresenter_DeleteGraphColorClicked_ShowsMessageBoxWhenSelectedGraphColorItemIsNull()
        {
            // Arrange
            var model = CreateModel();
            model.Preferences.Set(Preference.GraphColors, new List<Color>());
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                presenter.Show();
                Assert.IsNull(presenter.Model.SelectedGraphColorItem);
                // Act
                presenter.DeleteGraphColorClicked();
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
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
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                presenter.Show();
                Assert.AreEqual(3, presenter.Model.GraphColors.Count);
                // Act
                presenter.DeleteGraphColorClicked();
                // Assert
                Assert.AreEqual(3, presenter.Model.GraphColors.Count);
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
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
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                presenter.Show();
                Assert.AreEqual(4, presenter.Model.GraphColors.Count);
                // Act
                presenter.DeleteGraphColorClicked();
                // Assert
                Assert.AreEqual(3, presenter.Model.GraphColors.Count);
                Assert.AreEqual(0, messageBox.Invocations.Count);
            }
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

        private static BenchmarksModel CreateModel(IProteinService proteinService, IProteinBenchmarkService benchmarkService = null)
        {
            return new BenchmarksModel(null, proteinService, benchmarkService, null);
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

        private class LocalProcessServiceThrows : LocalProcessService
        {
            public override LocalProcess Start(string fileName)
            {
                throw new Exception("Process start failed.");
            }

            public override LocalProcess Start(string fileName, string arguments)
            {
                throw new Exception("Process start failed.");
            }
        }
    }
}
