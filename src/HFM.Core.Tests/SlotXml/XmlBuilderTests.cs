using NUnit.Framework;

using HFM.Core.Client;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    [TestFixture]
    public class XmlBuilderTests
    {
        [Test]
        public void XmlBuilder_CreateSlotSummary_PopulatesUpdateDateTimeFromArgument()
        {
            // Arrange
            var slots = new[] { new ClientData() };
            var updateDateTime = DateTime.Now;
            var xmlBuilder = new XmlBuilder(new InMemoryPreferencesProvider());
            // Act
            var slotSummary = xmlBuilder.CreateSlotSummary(slots, updateDateTime);
            // Assert
            Assert.AreEqual(updateDateTime, slotSummary.UpdateDateTime);
        }

        [Test]
        public void XmlBuilder_CreateSlotSummary_PopulatesAnXsltStyleNumberFormat()
        {
            // Arrange
            var slots = new[] { new ClientData() };
            var xmlBuilder = new XmlBuilder(new InMemoryPreferencesProvider());
            // Act
            var slotSummary = xmlBuilder.CreateSlotSummary(slots, DateTime.Now);
            // Assert
            Assert.AreEqual("###,###,##0.0", slotSummary.NumberFormat);
        }

        [Test]
        public void XmlBuilder_CreateSlotSummary_PopulatesStringPropertiesThatAreNotNull()
        {
            // Arrange
            var collection = new[]
            {
                new ClientData
                {
                    Settings = new ClientSettings(),
                    CurrentProtein = new Proteins.Protein()
                }
            };
            var xmlBuilder = new XmlBuilder(new InMemoryPreferencesProvider());
            // Act
            var slotSummary = xmlBuilder.CreateSlotSummary(collection, DateTime.Now);
            // Assert
            Assert.IsNotNull(slotSummary.HfmVersion);
            Assert.IsNotNull(slotSummary.NumberFormat);
            Assert.AreEqual(1, slotSummary.Slots.Count);
            var slotData = slotSummary.Slots[0];
            Assert.IsNotNull(slotData.Core);
            Assert.IsNotNull(slotData.CoreId);
            var protein = slotData.Protein;
            Assert.IsNotNull(protein.ServerIP);
            Assert.IsNotNull(protein.WorkUnitName);
            Assert.IsNotNull(protein.Core);
            Assert.IsNotNull(protein.Description);
            Assert.IsNotNull(protein.Contact);
        }

        [Test]
        public void XmlBuilder_CreateSlotDetail_PopulatesUpdateDateTimeFromArgument()
        {
            // Arrange
            var clientData = new ClientData
            {
                Settings = new ClientSettings()
            };
            var updateDateTime = DateTime.Now;
            var xmlBuilder = new XmlBuilder(new InMemoryPreferencesProvider());
            // Act
            var slotDetail = xmlBuilder.CreateSlotDetail(clientData, updateDateTime);
            // Assert
            Assert.AreEqual(updateDateTime, slotDetail.UpdateDateTime);
        }

        [Test]
        public void XmlBuilder_CreateSlotDetail_PopulatesAnXsltStyleNumberFormat()
        {
            // Arrange
            var clientData = new ClientData
            {
                Settings = new ClientSettings()
            };
            var xmlBuilder = new XmlBuilder(new InMemoryPreferencesProvider());
            // Act
            var slotDetail = xmlBuilder.CreateSlotDetail(clientData, DateTime.Now);
            // Assert
            Assert.AreEqual("###,###,##0.0", slotDetail.NumberFormat);
        }

        [Test]
        public void XmlBuilder_CreateSlotDetail_PopulatesStringPropertiesThatAreNotNull()
        {
            // Arrange
            var clientData = new ClientData
            {
                Settings = new ClientSettings(),
                CurrentProtein = new Proteins.Protein()
            };
            var xmlBuilder = new XmlBuilder(new InMemoryPreferencesProvider());
            // Act
            var slotDetail = xmlBuilder.CreateSlotDetail(clientData, DateTime.Now);
            // Assert
            Assert.IsNotNull(slotDetail.HfmVersion);
            Assert.IsNotNull(slotDetail.NumberFormat);
            Assert.IsNotNull(slotDetail.LogFileName);
            var slotData = slotDetail.SlotData;
            Assert.IsNotNull(slotData.Core);
            Assert.IsNotNull(slotData.CoreId);
            var protein = slotData.Protein;
            Assert.IsNotNull(protein.ServerIP);
            Assert.IsNotNull(protein.WorkUnitName);
            Assert.IsNotNull(protein.Core);
            Assert.IsNotNull(protein.Description);
            Assert.IsNotNull(protein.Contact);
        }
    }
}
