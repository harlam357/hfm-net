using System;
using System.Collections.Generic;
using System.Drawing;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Forms.Views;
using HFM.Preferences;

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
            var preferences = new InMemoryPreferenceSet();
            preferences.Set(Preference.BenchmarksFormLocation, new Point(10, 20));
            preferences.Set(Preference.BenchmarksFormSize, new Size(30, 40));
            preferences.Set(Preference.BenchmarksGraphLayoutType, GraphLayoutType.ClientsPerGraph);
            preferences.Set(Preference.BenchmarksClientsPerGraph, 3);
            preferences.Set(Preference.GraphColors, new List<Color> { Color.AliceBlue });
            var model = new BenchmarksModel(preferences, null);
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(new Point(10, 20), model.FormLocation);
            Assert.AreEqual(new Size(30, 40), model.FormSize);
            Assert.AreEqual(GraphLayoutType.ClientsPerGraph, model.GraphLayoutType);
            Assert.AreEqual(3, model.ClientsPerGraph);
            CollectionAssert.AreEqual(new List<Color> { Color.AliceBlue }, model.GraphColors);
        }

        [Test]
        public void BenchmarksModel_Load_SlotIdentifiersFromBenchmarks()
        {
            // Arrange
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Test", "Server", ClientSettings.DefaultPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(12345);
            benchmarkService.Update(slotIdentifier, benchmarkIdentifier, Array.Empty<TimeSpan>());
            var model = new BenchmarksModel(null, benchmarkService);
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(2, model.SlotIdentifiers.Count);
            var listItems = new List<ListItem>
            {
                new ListItem { DisplayMember = SlotIdentifier.AllSlots.ToString(), ValueMember = SlotIdentifier.AllSlots },
                new ListItem { DisplayMember = slotIdentifier.ToString(), ValueMember = slotIdentifier }
            };
            CollectionAssert.AreEqual(listItems, model.SlotIdentifiers);
        }

        [Test]
        public void BenchmarksModel_Load_SlotIdentifiersRaisesOneListChangedEvent()
        {
            // Arrange
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Test", "Server", ClientSettings.DefaultPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(12345);
            benchmarkService.Update(slotIdentifier, benchmarkIdentifier, Array.Empty<TimeSpan>());
            var model = new BenchmarksModel(null, benchmarkService);
            int listChanged = 0;
            model.SlotIdentifiers.ListChanged += (s, e) => listChanged++;
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(1, listChanged);
        }

        [Test]
        public void BenchmarksModel_Save_ToPreferences()
        {
            // Arrange
            var preferences = new InMemoryPreferenceSet();
            var model = new BenchmarksModel(preferences, null);
            model.FormLocation = new Point(50, 60);
            model.FormSize = new Size(70, 80);
            model.GraphLayoutType = GraphLayoutType.ClientsPerGraph;
            model.ClientsPerGraph = 4;
            model.GraphColors.AddRange(new[] { Color.SaddleBrown });
            // Act
            model.Save();
            // Assert
            Assert.AreEqual(new Point(50, 60), preferences.Get<Point>(Preference.BenchmarksFormLocation));
            Assert.AreEqual(new Size(70, 80), preferences.Get<Size>(Preference.BenchmarksFormSize));
            Assert.AreEqual(GraphLayoutType.ClientsPerGraph, preferences.Get<GraphLayoutType>(Preference.BenchmarksGraphLayoutType));
            Assert.AreEqual(4, preferences.Get<int>(Preference.BenchmarksClientsPerGraph));
            CollectionAssert.AreEqual(new List<Color> { Color.SaddleBrown }, preferences.Get<List<Color>>(Preference.GraphColors));
        }

        [Test]
        public void BenchmarksModel_DecimalPlaces_IsLiveValueFromPreferences()
        {
            // Arrange
            var preferences = new InMemoryPreferenceSet();
            preferences.Set(Preference.DecimalPlaces, 2);
            var model = new BenchmarksModel(preferences, null);
            // Act
            int decimalPlaces = model.DecimalPlaces;
            // Assert
            Assert.AreEqual(2, decimalPlaces);
            // Act (Change)
            preferences.Set(Preference.DecimalPlaces, 3);
            decimalPlaces = model.DecimalPlaces;
            // Assert
            Assert.AreEqual(3, decimalPlaces);
        }

        [Test]
        public void BenchmarksModel_BonusCalculation_IsLiveValueFromPreferences()
        {
            // Arrange
            var preferences = new InMemoryPreferenceSet();
            preferences.Set(Preference.BonusCalculation, BonusCalculation.DownloadTime);
            var model = new BenchmarksModel(preferences, null);
            // Act
            var bonusCalculation = model.BonusCalculation;
            // Assert
            Assert.AreEqual(BonusCalculation.DownloadTime, bonusCalculation);
            // Act (Change)
            preferences.Set(Preference.BonusCalculation, BonusCalculation.FrameTime);
            bonusCalculation = model.BonusCalculation;
            // Assert
            Assert.AreEqual(BonusCalculation.FrameTime, bonusCalculation);
        }

        [Test]
        public void BenchmarksModel_SelectedSlotIdentifier_RaisesPropertyChangedEvents()
        {
            // Arrange
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Test", "Server", ClientSettings.DefaultPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var model = new BenchmarksModel(null, null);
            var propertyNames = new List<string>();
            model.PropertyChanged += (s, e) => propertyNames.Add(e.PropertyName);
            // Act
            model.SelectedSlotIdentifier = slotIdentifier;
            // Assert
            Assert.AreEqual(2, propertyNames.Count);
            CollectionAssert.AreEqual(new[] { nameof(BenchmarksModel.SelectedSlotIdentifier), nameof(BenchmarksModel.SelectedSlotDeleteEnabled) }, propertyNames);
        }
    }
}
