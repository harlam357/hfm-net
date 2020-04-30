
using System;

using NUnit.Framework;

using HFM.Core.Client;

namespace HFM.Core.SlotXml
{
    [TestFixture]
    public class XmlBuilderTests
    {
        [Test]
        public void XmlBuilder_CreateSlotSummary_PopulatesUpdateDateTimeFromArgument()
        {
            // Arrange
            var client = new NullClient { Settings = new ClientSettings() };
            var slots = new[] { new SlotModel(client) };
            var updateDateTime = DateTime.Now;
            var xmlBuilder = new XmlBuilder(client.Preferences);
            // Act
            var slotSummary = xmlBuilder.CreateSlotSummary(slots, updateDateTime);
            // Assert
            Assert.AreEqual(updateDateTime, slotSummary.UpdateDateTime);
        }

        [Test]
        public void XmlBuilder_CreateSlotSummary_PopulatesAnXsltStyleNumberFormat()
        {
            // Arrange
            var client = new NullClient { Settings = new ClientSettings() };
            var slots = new[] { new SlotModel(client) };
            var xmlBuilder = new XmlBuilder(client.Preferences);
            // Act
            var slotSummary = xmlBuilder.CreateSlotSummary(slots, DateTime.Now);
            // Assert
            Assert.AreEqual("###,###,##0.0", slotSummary.NumberFormat);
        }

        [Test]
        public void XmlBuilder_CreateSlotSummary_PopulatesStringPropertiesThatAreNotNull()
        {
            // Arrange
            var client = new NullClient { Settings = new ClientSettings() };
            var slots = new[] { new SlotModel(client) };
            var xmlBuilder = new XmlBuilder(client.Preferences);
            // Act
            var slotSummary = xmlBuilder.CreateSlotSummary(slots, DateTime.Now);
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
            var client = new NullClient { Settings = new ClientSettings() };
            var slot = new SlotModel(client);
            var updateDateTime = DateTime.Now;
            var xmlBuilder = new XmlBuilder(client.Preferences);
            // Act
            var slotDetail = xmlBuilder.CreateSlotDetail(slot, updateDateTime);
            // Assert
            Assert.AreEqual(updateDateTime, slotDetail.UpdateDateTime);
        }

        [Test]
        public void XmlBuilder_CreateSlotDetail_PopulatesAnXsltStyleNumberFormat()
        {
            // Arrange
            var client = new NullClient { Settings = new ClientSettings() };
            var slot = new SlotModel(client);
            var xmlBuilder = new XmlBuilder(client.Preferences);
            // Act
            var slotDetail = xmlBuilder.CreateSlotDetail(slot, DateTime.Now);
            // Assert
            Assert.AreEqual("###,###,##0.0", slotDetail.NumberFormat);
        }

        [Test]
        public void XmlBuilder_CreateSlotDetail_PopulatesStringPropertiesThatAreNotNull()
        {
            // Arrange
            var client = new NullClient { Settings = new ClientSettings() };
            var slot = new SlotModel(client);
            var xmlBuilder = new XmlBuilder(client.Preferences);
            // Act
            var slotDetail = xmlBuilder.CreateSlotDetail(slot, DateTime.Now);
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
