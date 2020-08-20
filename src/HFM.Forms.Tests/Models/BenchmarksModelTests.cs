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
            // get the second slot identifier
            var slotIdentifier = benchmarkService.GetSlotIdentifiers().OrderBy(x => x.Name).Last();
            // Act
            model.SelectedSlotIdentifier = new ValueItem<SlotIdentifier>(slotIdentifier);
            // Assert
            Assert.AreEqual(2, model.SlotProjects.Count);
            Assert.AreEqual(23456, model.SelectedSlotProject.Value);
        }

        [Test]
        public void BenchmarksModel_Protein_IsSetFromProteinService()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(protein);
            var proteinService = new ProteinService(dataContainer, null, NullLogger.Instance);
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarkService);
            // Act
            model.Load();
            // Assert
            Assert.AreSame(protein, model.Protein);
        }

        [Test]
        public void BenchmarksModel_Protein_IsSetNullWhenSelectedSlotProjectIsNull()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(protein);
            var proteinService = new ProteinService(dataContainer, null, NullLogger.Instance);
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarkService);
            model.Load();
            // Act
            model.SelectedSlotProject = null;
            // Assert
            Assert.IsNull(model.Protein);
        }

        [Test]
        public void BenchmarksModel_Protein_IsSetNullWhenProjectDoesNotExistInProteinService()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(protein);
            var proteinService = new ProteinService(dataContainer, null, NullLogger.Instance);
            var benchmarkService = CreateBenchmarkServiceWithTwoSlotsAndProjects();
            var model = CreateModel(proteinService, benchmarkService);
            model.Load();
            // get the second slot identifier
            var slotIdentifier = benchmarkService.GetSlotIdentifiers().OrderBy(x => x.Name).Last();
            // Act
            model.SelectedSlotIdentifier = new ValueItem<SlotIdentifier>(slotIdentifier);
            // Assert
            Assert.IsNull(model.Protein);
        }

        [Test]
        public void BenchmarksModel_BenchmarkText_IsPopulatedFromTextBenchmarksReport()
        {
            // Arrange
            var protein = new Protein { ProjectNumber = 12345 };
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(protein);
            var proteinService = new ProteinService(dataContainer, null, NullLogger.Instance);
            var benchmarkService = CreateBenchmarkServiceWithOneSlotAndProject();
            var model = CreateModel(proteinService, benchmarkService, new[] { new MockTextBenchmarksReport() });
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(1, model.BenchmarkText.Count);
            Assert.AreEqual(MockTextBenchmarksReport.Text, model.BenchmarkText.First());
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
