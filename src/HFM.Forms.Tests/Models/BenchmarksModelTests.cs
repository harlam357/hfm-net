using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Proteins;

using NUnit.Framework;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class BenchmarksModelTests
    {
        [Test]
        public void BenchmarksModel_Load_FromPreferences()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            preferences.Set(Preference.BenchmarksFormLocation, new Point(10, 20));
            preferences.Set(Preference.BenchmarksFormSize, new Size(30, 40));
            preferences.Set(Preference.GraphColors, new List<Color> { Color.AliceBlue });
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(new Point(10, 20), model.FormLocation);
            Assert.AreEqual(new Size(30, 40), model.FormSize);
            CollectionAssert.AreEqual(new List<Color> { Color.AliceBlue }, model.GraphColors);
        }

        [Test]
        public void BenchmarksModel_Load_SlotsAndProjectsFromBenchmarks()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(benchmarkService);
            // Act
            model.Load();
            // Assert (slots)
            Assert.AreEqual(2, model.SlotIdentifiers.Count);
            Assert.AreEqual(SlotIdentifier.AllSlots, model.SelectedSlotIdentifier.Value);
            // Assert (projects)
            Assert.AreEqual(1, model.SlotProjects.Count);
            Assert.AreEqual(12345, model.SelectedSlotProject.Value);
        }

        [Test]
        public void BenchmarksModel_Load_SlotIdentifiersRaisesOneListChangedEvent()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(benchmarkService);
            int listChanged = 0;
            model.SlotIdentifiers.ListChanged += (s, e) => listChanged++;
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(1, listChanged);
        }

        [Test]
        public void BenchmarksModel_Load_SlotProjectsRaisesOneListChangedEvent()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(benchmarkService);
            int listChanged = 0;
            model.SlotProjects.ListChanged += (s, e) => listChanged++;
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(1, listChanged);
        }

        [Test]
        public void BenchmarksModel_Save_ToPreferences()
        {
            // Arrange
            var model = CreateModel();
            var preferences = model.Preferences;
            model.FormLocation = new Point(50, 60);
            model.FormSize = new Size(70, 80);
            model.GraphColors.AddRange(new[] { Color.SaddleBrown });
            // Act
            model.Save();
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
            Assert.AreEqual(2, propertyNames.Count);
            var expected = new[] { nameof(BenchmarksModel.SelectedSlotIdentifier), nameof(BenchmarksModel.SelectedSlotDeleteEnabled) };
            CollectionAssert.AreEqual(expected, propertyNames);
        }

        [Test]
        public void BenchmarksModel_SelectedSlotIdentifier_RefreshesSlotProjects()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            model.Load();
            // Act
            model.SelectedSlotIdentifier = model.SlotIdentifierValueItems.Last();
            // Assert
            Assert.AreEqual(2, model.SlotProjects.Count);
            Assert.AreEqual(23456, model.SelectedSlotProject.Value);
        }

        [Test]
        public void BenchmarksModel_SelectedSlotProject_IsSetFromDefaultProjectID()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            model.DefaultProjectID = 65432;
            model.Load();
            // Act
            model.SetDefaultSlotProject();
            // Assert
            Assert.IsNotNull(model.SelectedSlotProject);
            Assert.AreEqual(1, model.SelectedSlotProjectListItems.Count);
            Assert.AreEqual(65432, model.SelectedSlotProjectListItems.First().GetValue<ValueItem<int>>().Value);
        }

        [Test]
        public void BenchmarksModel_SelectedSlotProject_IsSetWhenProjectListItemIsSelected()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(benchmarkService);
            model.Load();
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
        public void BenchmarksModel_Protein_IsSetFromProteinServiceWhenProjectListItemIsSelected()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarkService);
            // Act
            model.Load();
            // Assert
            Assert.AreSame(protein, model.Protein);
        }

        [Test]
        public void BenchmarksModel_Protein_IsSetNullWhenProjectListItemIsNotSelected()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarkService);
            model.Load();
            Assert.IsNotNull(model.Protein);
            // Act
            model.SelectedSlotProjectListItems.Clear();
            // Assert
            Assert.IsNull(model.Protein);
        }

        [Test]
        public void BenchmarksModel_Protein_IsSetNullWhenProjectDoesNotExistInProteinService()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(proteinService, benchmarkService);
            model.Load();
            // Act
            model.SelectedSlotIdentifier = model.SlotIdentifierValueItems.Last();
            // Assert
            Assert.IsNull(model.Protein);
        }

        [Test]
        public void BenchmarksModel_AsIBenchmarkReportSource()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            var preferences = model.Preferences;
            preferences.Set(Preference.GraphColors, new List<Color> { Color.AliceBlue });
            // Act
            model.Load();
            // Assert
            IBenchmarksReportSource source = model;
            Assert.AreEqual(model.SelectedSlotIdentifier.Value, source.SlotIdentifier);
            CollectionAssert.AreEqual(model.SelectedSlotProjectListItems.Select(x => x.GetValue<ValueItem<int>>().Value), source.Projects);
            CollectionAssert.AreEqual(model.GraphColors, source.Colors);
        }

        [Test]
        public void BenchmarksModel_AsIBenchmarkReportSource_WhenSelectedSlotIdentifierIsNull()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            var preferences = model.Preferences;
            preferences.Set(Preference.GraphColors, new List<Color> { Color.AliceBlue });
            model.Load();
            // Act
            model.SelectedSlotIdentifier = null;
            // Assert
            IBenchmarksReportSource source = model;
            Assert.IsNull(source.SlotIdentifier);
            Assert.AreEqual(0, source.Projects.Count);
            CollectionAssert.AreEqual(model.GraphColors, source.Colors);
        }

        [Test]
        public void BenchmarksModel_BenchmarkText_IsPopulatedFromTextBenchmarksReportWhenProjectIsSelected()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var proteinService = CreateProteinService(protein);
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarkService, new[] { new MockTextBenchmarksReport() });
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(1, model.BenchmarkText.Count);
            Assert.AreEqual(MockTextBenchmarksReport.Text, model.BenchmarkText.First());
        }

        [Test]
        public void BenchmarksModel_RemoveSlot_RemovesSlotFromSlotIdentifiers()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            model.Load();
            Assert.AreEqual(3, model.SlotIdentifiers.Count);
            // Act
            model.RemoveSlot(model.SlotIdentifierValueItems.ElementAt(1).Value);
            // Assert
            Assert.AreEqual(2, model.SlotIdentifiers.Count);
        }

        [Test]
        public void BenchmarksModel_RemoveProject_RemovesProjectFromSlotProjects()
        {
            // Arrange
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(benchmarkService);
            model.Load();
            Assert.AreEqual(3, model.SlotIdentifiers.Count);
            Assert.AreEqual(3, model.SlotProjects.Count);
            var slotIdentifier = model.SlotIdentifierValueItems.ElementAt(1).Value;
            int projectID = model.SlotProjectListItems.First().GetValue<ValueItem<int>>().Value;
            // Act
            model.RemoveProject(slotIdentifier, projectID);
            // Assert
            Assert.AreEqual(2, model.SlotIdentifiers.Count);
            Assert.AreEqual(2, model.SlotProjects.Count);
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

        private static BenchmarksModel CreateModel(IProteinBenchmarkService benchmarkService = null)
        {
            return CreateModel(null, benchmarkService);
        }

        private static BenchmarksModel CreateModel(IProteinService proteinService, IProteinBenchmarkService benchmarkService, IEnumerable<BenchmarksReport> reports = null)
        {
            return new BenchmarksModel(null, proteinService, benchmarkService, reports);
        }
    }
}
