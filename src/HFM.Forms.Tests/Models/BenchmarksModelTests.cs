using System.Drawing;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Proteins;

using Moq;

using NUnit.Framework;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class BenchmarksModelTests
    {
        [Test]
        public async Task BenchmarksModel_LoadAsync_FromPreferences()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            preferences.Set(Preference.BenchmarksFormLocation, new Point(10, 20));
            preferences.Set(Preference.BenchmarksFormSize, new Size(30, 40));
            var color = Color.AliceBlue;
            preferences.Set(Preference.GraphColors, new List<Color> { color });
            // Act
            await model.LoadAsync();
            // Assert
            Assert.AreEqual(new Point(10, 20), model.FormLocation);
            Assert.AreEqual(new Size(30, 40), model.FormSize);
            CollectionAssert.AreEqual(new List<ListItem> { new ListItem(color.Name, new ValueItem<Color>(color)) }, model.GraphColors);
        }

        [Test]
        public async Task BenchmarksModel_LoadAsync_FirstSelectedGraphColor()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color = Color.AliceBlue;
            preferences.Set(Preference.GraphColors, new List<Color> { color });
            // Act
            await model.LoadAsync();
            // Assert
            Assert.AreEqual(color, model.SelectedGraphColorItem.Value);
            Assert.AreEqual(color, model.SelectedGraphColor);
        }

        [Test]
        public async Task BenchmarksModel_LoadAsync_SlotsAndProjectsFromBenchmarks()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(benchmarks);
            // Act
            await model.LoadAsync();
            // Assert (slots)
            Assert.AreEqual(2, model.SlotIdentifiers.Count);
            Assert.AreEqual(SlotIdentifier.AllSlots, model.SelectedSlotIdentifier.Value);
            // Assert (projects)
            Assert.AreEqual(1, model.SlotProjects.Count);
            Assert.AreEqual(12345, model.SelectedSlotProject.Value);
        }

        [Test]
        public async Task BenchmarksModel_LoadAsync_SlotIdentifiersRaisesOneListChangedEvent()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(benchmarks);
            int listChanged = 0;
            model.SlotIdentifiers.ListChanged += (s, e) => listChanged++;
            // Act
            await model.LoadAsync();
            // Assert
            Assert.AreEqual(1, listChanged);
        }

        [Test]
        public async Task BenchmarksModel_LoadAsync_SlotProjectsRaisesOneListChangedEvent()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(benchmarks);
            int listChanged = 0;
            model.SlotProjects.ListChanged += (s, e) => listChanged++;
            // Act
            await model.LoadAsync();
            // Assert
            Assert.AreEqual(1, listChanged);
        }

        [Test]
        public async Task BenchmarksModel_SaveAsync_ToPreferences()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            model.FormLocation = new Point(50, 60);
            model.FormSize = new Size(70, 80);
            var color = Color.SaddleBrown;
            model.GraphColors.Add(new ListItem(color.Name, new ValueItem<Color>(color)));
            // Act
            await model.SaveAsync();
            // Assert
            Assert.AreEqual(new Point(50, 60), preferences.Get<Point>(Preference.BenchmarksFormLocation));
            Assert.AreEqual(new Size(70, 80), preferences.Get<Size>(Preference.BenchmarksFormSize));
            CollectionAssert.AreEqual(new List<Color> { Color.SaddleBrown }, preferences.Get<List<Color>>(Preference.GraphColors));
        }

        [Test]
        public void BenchmarksModel_SelectedSlotIdentifier_RaisesPropertyChangedEvents()
        {
            // Arrange
            var slotIdentifier = CreateSlotIdentifier("Test", SlotIdentifier.NoSlotID);
            var model = CreateModel();
            var propertyNames = new List<string>();
            model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
            // Act
            model.SelectedSlotIdentifier = new ValueItem<SlotIdentifier>(slotIdentifier);
            // Assert
            Assert.AreEqual(1, propertyNames.Count);
            var expected = new[] { nameof(BenchmarksModel.SelectedSlotIdentifier) };
            CollectionAssert.AreEqual(expected, propertyNames);
        }

        [Test]
        public async Task BenchmarksModel_SelectedSlotIdentifier_RefreshesSlotProjects()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarks);
            await model.LoadAsync();
            // Act
            model.SelectedSlotIdentifier = model.SlotIdentifierValueItems.Last();
            // Assert
            Assert.AreEqual(2, model.SlotProjects.Count);
            Assert.AreEqual(23456, model.SelectedSlotProject.Value);
        }

        [Test]
        public async Task BenchmarksModel_SelectedSlotProject_IsSetFromDefaultProjectID()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarks);
            model.DefaultProjectID = 65432;
            await model.LoadAsync();
            // Act
            model.SetDefaultSlotProject();
            // Assert
            Assert.IsNotNull(model.SelectedSlotProject);
            Assert.AreEqual(1, model.SelectedSlotProjectListItems.Count);
            Assert.AreEqual(65432, model.SelectedSlotProjectListItems.First().GetValue<ValueItem<int>>().Value);
        }

        [Test]
        public async Task BenchmarksModel_SelectedSlotProject_IsSetWhenProjectListItemIsSelected()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(benchmarks);
            await model.LoadAsync();
            model.SelectedSlotProjectListItems.Clear();
            Assert.IsNull(model.SelectedSlotProject);
            // Act
            model.SelectedSlotProjectListItems.Add(model.SlotProjectListItems.First());
            // Assert
            Assert.IsNotNull(model.SelectedSlotProject);
            Assert.AreEqual(1, model.SelectedSlotProjectListItems.Count);
            Assert.AreEqual(12345, model.SelectedSlotProjectListItems.First().GetValue<ValueItem<int>>().Value);
        }

        [Test]
        public async Task BenchmarksModel_Protein_IsSetFromProteinServiceWhenProjectListItemIsSelected()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarks);
            // Act
            await model.LoadAsync();
            // Assert
            Assert.AreSame(protein, model.Protein);
        }

        [Test]
        public async Task BenchmarksModel_Protein_IsSetNullWhenProjectListItemIsNotSelected()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarks);
            await model.LoadAsync();
            Assert.IsNotNull(model.Protein);
            // Act
            model.SelectedSlotProjectListItems.Clear();
            // Assert
            Assert.IsNull(model.Protein);
        }

        [Test]
        public async Task BenchmarksModel_Protein_IsSetNullWhenProjectDoesNotExistInProteinService()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarks = CreateBenchmarkRepositoryWithTwoSlotsAndProjects();
            var model = CreateModel(proteinService, benchmarks);
            await model.LoadAsync();
            // Act
            model.SelectedSlotIdentifier = model.SlotIdentifierValueItems.Last();
            // Assert
            Assert.IsNull(model.Protein);
        }

        [Test]
        public async Task BenchmarksModel_AsIBenchmarkReportSource()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarks);
            var preferences = model.Preferences;
            preferences.Set(Preference.GraphColors, new List<Color> { Color.AliceBlue });
            // Act
            await model.LoadAsync();
            // Assert
            IBenchmarksReportSource source = model;
            Assert.AreEqual(model.SelectedSlotIdentifier.Value, source.SlotIdentifier);
            CollectionAssert.AreEqual(model.SelectedSlotProjectListItems.Select(x => x.GetValue<ValueItem<int>>().Value), source.Projects);
            CollectionAssert.AreEqual(model.GraphColors.Select(x => x.GetValue<ValueItem<Color>>().Value), source.Colors);
        }

        [Test]
        public async Task BenchmarksModel_AsIBenchmarkReportSource_WhenSelectedSlotIdentifierIsNull()
        {
            // Arrange
            var benchmarks = CreateBenchmarkRepositoryWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarks);
            var preferences = model.Preferences;
            preferences.Set(Preference.GraphColors, new List<Color> { Color.AliceBlue });
            await model.LoadAsync();
            // Act
            model.SelectedSlotIdentifier = null;
            // Assert
            IBenchmarksReportSource source = model;
            Assert.IsNull(source.SlotIdentifier);
            Assert.AreEqual(0, source.Projects.Count);
            CollectionAssert.AreEqual(model.GraphColors.Select(x => x.GetValue<ValueItem<Color>>().Value), source.Colors);
        }

        [Test]
        public async Task BenchmarksModel_BenchmarkText_IsPopulatedFromTextBenchmarksReportWhenProjectIsSelected()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarks = CreateBenchmarkRepositoryWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarks, new[] { new MockTextBenchmarksReport() });
            // Act
            await model.LoadAsync();
            // Assert
            Assert.AreEqual(1, model.BenchmarkText.Count);
            Assert.AreEqual(MockTextBenchmarksReport.Text, model.BenchmarkText.First());
        }

        [Test]
        public async Task BenchmarkModel_MoveSelectedGraphColorUp_DoesNotMoveTheColorAtIndexZero()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            // Act
            model.MoveSelectedGraphColorUp();
            // Assert
            Assert.AreEqual(color0, model.GraphColors[0].GetValue<ValueItem<Color>>().Value);
            Assert.AreEqual(color1, model.GraphColors[1].GetValue<ValueItem<Color>>().Value);
        }

        [Test]
        public async Task BenchmarkModel_MoveSelectedGraphColorUp_MovesTheColorUpOneIndex()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            model.SelectedGraphColorItem = model.GraphColors.Last().GetValue<ValueItem<Color>>();
            // Act
            model.MoveSelectedGraphColorUp();
            // Assert
            Assert.AreEqual(color1, model.GraphColors[0].GetValue<ValueItem<Color>>().Value);
            Assert.AreEqual(color0, model.GraphColors[1].GetValue<ValueItem<Color>>().Value);
        }

        [Test]
        public async Task BenchmarkModel_MoveSelectedGraphColorDown_DoesNotMoveTheColorAtIndexN()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            model.SelectedGraphColorItem = model.GraphColors.Last().GetValue<ValueItem<Color>>();
            // Act
            model.MoveSelectedGraphColorDown();
            // Assert
            Assert.AreEqual(color0, model.GraphColors[0].GetValue<ValueItem<Color>>().Value);
            Assert.AreEqual(color1, model.GraphColors[1].GetValue<ValueItem<Color>>().Value);
        }

        [Test]
        public async Task BenchmarkModel_MoveSelectedGraphColorDown_MovesTheColorDownOneIndex()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            // Act
            model.MoveSelectedGraphColorDown();
            // Assert
            Assert.AreEqual(color1, model.GraphColors[0].GetValue<ValueItem<Color>>().Value);
            Assert.AreEqual(color0, model.GraphColors[1].GetValue<ValueItem<Color>>().Value);
        }

        [Test]
        public async Task BenchmarkModel_AddGraphColor_ReturnsFalseWhenColorAlreadyExists()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            // Act
            bool result = model.AddGraphColor(color0);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task BenchmarkModel_AddGraphColor_ReturnsTrueAndAddsNewColorToTheEndOfTheList()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            var color2 = Color.ForestGreen;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            // Act
            bool result = model.AddGraphColor(color2);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(color0, model.GraphColors[0].GetValue<ValueItem<Color>>().Value);
            Assert.AreEqual(color1, model.GraphColors[1].GetValue<ValueItem<Color>>().Value);
            Assert.AreEqual(color2, model.GraphColors[2].GetValue<ValueItem<Color>>().Value);
            Assert.AreEqual(color2, model.SelectedGraphColorItem.Value);
        }

        [Test]
        public async Task BenchmarkModel_DeleteSelectedGraphColor_DoesNotRemoveColorWhenSelectedColorIsNull()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            model.SelectedGraphColorItem = null;
            // Act
            model.DeleteSelectedGraphColor();
            // Assert
            Assert.AreEqual(2, model.GraphColors.Count);
            Assert.IsNull(model.SelectedGraphColorItem);
            Assert.AreEqual(Color.Empty, model.SelectedGraphColor);
        }

        [Test]
        public async Task BenchmarkModel_DeleteSelectedGraphColor_RemovesTheColorAndSetsNewSelectedColor()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            var color0 = Color.AliceBlue;
            var color1 = Color.SaddleBrown;
            preferences.Set(Preference.GraphColors, new List<Color> { color0, color1 });
            await model.LoadAsync();
            // Act
            model.DeleteSelectedGraphColor();
            // Assert
            Assert.AreEqual(1, model.GraphColors.Count);
            Assert.AreEqual(color1, model.SelectedGraphColorItem.Value);
            Assert.AreEqual(color1, model.SelectedGraphColor);
        }

        private static SlotIdentifier CreateSlotIdentifier(string name, int slotID)
        {
            return new SlotIdentifier(new ClientIdentifier(name, "Server", ClientSettings.DefaultPort, Guid.NewGuid()), slotID);
        }

        private static IProteinService CreateProteinService(Protein protein)
        {
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(protein);
            return new ProteinService(dataContainer, null, NullLogger.Instance);
        }

        private static IProteinBenchmarkRepository CreateBenchmarkRepositoryWithOneSlotAndProject()
        {
            var benchmarks = new Mock<IProteinBenchmarkRepository>();
            var slotIdentifier = CreateSlotIdentifier("Test", SlotIdentifier.NoSlotID);
            benchmarks.Setup(x => x.GetSlotIdentifiersAsync()).Returns(Task.FromResult((ICollection<SlotIdentifier>)new[] { slotIdentifier }));
            benchmarks.Setup(x => x.GetBenchmarkProjectsAsync(It.IsAny<SlotIdentifier>())).Returns(Task.FromResult((ICollection<int>)new[] { 12345 }));
            return benchmarks.Object;
        }

        private static IProteinBenchmarkRepository CreateBenchmarkRepositoryWithTwoSlotsAndProjects()
        {
            var benchmarks = new Mock<IProteinBenchmarkRepository>();
            var slot0 = CreateSlotIdentifier("Test", 0);
            var slot1 = CreateSlotIdentifier("Test", 1);
            benchmarks.Setup(x => x.GetSlotIdentifiersAsync()).Returns(Task.FromResult((ICollection<SlotIdentifier>)new[] { slot0, slot1 }));
            benchmarks.Setup(x => x.GetBenchmarkProjectsAsync(It.IsAny<SlotIdentifier>())).Returns(Task.FromResult((ICollection<int>)new[] { 23456, 65432 }));
            return benchmarks.Object;
        }

        private class MockTextBenchmarksReport : TextBenchmarksReport
        {
            public const string Text = "Mock Report";

            public MockTextBenchmarksReport() : base(null, null, null, null)
            {

            }

            public override void Generate(IBenchmarksReportSource source)
            {
                Result = new List<string> { Text };
            }
        }

        private static BenchmarksModel CreateModel(IProteinBenchmarkRepository benchmarks = null)
        {
            return CreateModel(null, benchmarks);
        }

        private static BenchmarksModel CreateModel(IProteinService proteinService, IProteinBenchmarkRepository benchmarks, IEnumerable<BenchmarksReport> reports = null)
        {
            return new BenchmarksModel(null, proteinService, benchmarks, reports);
        }
    }
}
