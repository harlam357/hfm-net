using System.Drawing;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.Services;
using HFM.Core.Services.Mocks;
using HFM.Core.WorkUnits;
using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Presenters.Mocks;
using HFM.Forms.Views;
using HFM.Preferences;
using HFM.Proteins;

using Moq;

using NUnit.Framework;

namespace HFM.Forms.Presenters
{
    [TestFixture]
    public class BenchmarkPresenterTests
    {
        [Test]
        public async Task BenchmarkPresenter_Show_ShowsView()
        {
            // Arrange
            var model = CreateModel();
            var presenter = new MockFormBenchmarksPresenter(model);
            // Act
            await presenter.ShowAsync();
            // Assert
            Assert.IsTrue(presenter.MockForm.Shown);
        }

        [Test]
        public async Task BenchmarkPresenter_DescriptionLinkClicked_StartsLocalProcess()
        {
            // Arrange
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(new Protein { ProjectNumber = 12345, Description = "http://someurl"});
            var proteinService = new ProteinService(dataContainer, null, null);
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarks);
            using (var presenter = new MockFormBenchmarksPresenter(model))
            {
                await presenter.ShowAsync();
                var localProcess = new MockLocalProcessService();
                // Act
                presenter.DescriptionLinkClicked(localProcess);
                // Assert
                Assert.AreEqual(1, presenter.Model.SlotProjects.Count);
                Assert.IsTrue(localProcess.Invocations.First().FileName == "http://someurl");
            }
        }

        [Test]
        public async Task BenchmarkPresenter_DescriptionLinkClicked_ShowsMessageBoxWhenLocalProcessFailsToStart()
        {
            // Arrange
            var model = CreateModel();
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                await presenter.ShowAsync();
                var localProcess = new LocalProcessServiceThrows();
                // Act
                presenter.DescriptionLinkClicked(localProcess);
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public async Task BenchmarkPresenter_AddGraphColorClicked_AddsNewColorToGraphColors()
        {
            // Arrange
            var model = CreateModel();
            model.Preferences.Set(Preference.GraphColors, new List<Color>());
            using (var presenter = new MockFormBenchmarksPresenter(model))
            {
                await presenter.ShowAsync();
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
        public async Task BenchmarkPresenter_AddGraphColorClicked_ShowsMessageBoxAndDoesNotAddExistingColor()
        {
            // Arrange
            var model = CreateModel();
            var color = Color.AliceBlue;
            model.Preferences.Set(Preference.GraphColors, new List<Color> { color });
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                await presenter.ShowAsync();
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
        public async Task BenchmarkPresenter_DeleteGraphColorClicked_ShowsMessageBoxWhenSelectedGraphColorItemIsNull()
        {
            // Arrange
            var model = CreateModel();
            model.Preferences.Set(Preference.GraphColors, new List<Color>());
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            using (var presenter = new MockFormBenchmarksPresenter(model, messageBox))
            {
                await presenter.ShowAsync();
                Assert.IsNull(presenter.Model.SelectedGraphColorItem);
                // Act
                presenter.DeleteGraphColorClicked();
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
        }

        [Test]
        public async Task BenchmarkPresenter_DeleteGraphColorClicked_ShowsMessageBoxWhenThreeOrLessGraphColors()
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
                await presenter.ShowAsync();
                Assert.AreEqual(3, presenter.Model.GraphColors.Count);
                // Act
                presenter.DeleteGraphColorClicked();
                // Assert
                Assert.AreEqual(3, presenter.Model.GraphColors.Count);
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
        }

        [Test]
        public async Task BenchmarkPresenter_DeleteGraphColorClicked_DeletesColor()
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
                await presenter.ShowAsync();
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

        private static IProteinBenchmarkRepository CreateBenchmarkRepositoryWithOneSlotAndProject()
        {
            var benchmarks = new Mock<IProteinBenchmarkRepository>();
            var slotIdentifier = CreateSlotIdentifier("Test", SlotIdentifier.NoSlotID);
            benchmarks.Setup(x => x.GetSlotIdentifiersAsync()).Returns(Task.FromResult((ICollection<SlotIdentifier>)new[] { slotIdentifier }));
            benchmarks.Setup(x => x.GetBenchmarkProjects(It.IsAny<SlotIdentifier>())).Returns(new[] { 12345 });
            return benchmarks.Object;
        }

        private static BenchmarksModel CreateModel(IProteinBenchmarkRepository benchmarks = null)
        {
            return new BenchmarksModel(null, null, benchmarks, null);
        }

        private static BenchmarksModel CreateModel(IProteinService proteinService, IProteinBenchmarkRepository benchmarks = null)
        {
            return new BenchmarksModel(null, proteinService, benchmarks, null);
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
