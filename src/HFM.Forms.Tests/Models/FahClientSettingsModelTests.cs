
using System;
using System.Linq;

using NUnit.Framework;

using HFM.Client.ObjectModel;
using HFM.Core.Client;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class FahClientSettingsModelTests
    {
        [Test]
        public void FahClientSettingsModel_DefaultPropertyValues()
        {
            var model = new FahClientSettingsModel();
            Assert.IsFalse(model.ConnectEnabled);
            Assert.AreEqual(String.Empty, model.Name);
            Assert.AreEqual(String.Empty, model.Server);
            Assert.AreEqual(ClientSettings.DefaultPort, model.Port);
            Assert.AreEqual(String.Empty, model.Password);
            Assert.AreEqual(default(Guid), model.Guid);
            Assert.IsNotNull(model.Slots);
        }

        [Test]
        public void FahClientSettingsModel_Load_FromClientSettings()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var settings = new ClientSettings
            {
                Name = "Foo",
                Server = "Bar",
                Port = 12345,
                Password = "fizzbizz",
                Guid = guid
            };
            var model = new FahClientSettingsModel(settings);
            // Act
            model.Load();
            // Assert
            Assert.AreEqual("Foo", model.Name);
            Assert.AreEqual("Bar", model.Server);
            Assert.AreEqual(12345, model.Port);
            Assert.AreEqual("fizzbizz", model.Password);
            Assert.AreEqual(guid, model.Guid);
        }

        [Test]
        public void FahClientSettingsModel_Save_ToClientSettings()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var model = new FahClientSettingsModel
            {
                Name = "Foo",
                Server = "Bar",
                Port = 12345,
                Password = "fizzbizz",
                Guid = guid
            };
            // Act
            model.Save();
            // Assert
            Assert.AreEqual(ClientType.FahClient, model.ClientSettings.ClientType);
            Assert.AreEqual("Foo", model.ClientSettings.Name);
            Assert.AreEqual("Bar", model.ClientSettings.Server);
            Assert.AreEqual(12345, model.ClientSettings.Port);
            Assert.AreEqual("fizzbizz", model.ClientSettings.Password);
            Assert.AreEqual(guid, model.ClientSettings.Guid);
        }

        [Test]
        public void FahClientSettingsModel_Name_HasError()
        {
            var model = new FahClientSettingsModel();
            string error = model[nameof(FahClientSettingsModel.Name)];
            Assert.IsNotNull(error);
        }

        [Test]
        public void FahClientSettingsModel_Server_HasError()
        {
            var model = new FahClientSettingsModel();
            string error = model[nameof(FahClientSettingsModel.Server)];
            Assert.IsNotNull(error);
        }

        [Test]
        public void FahClientSettingsModel_Port_HasError()
        {
            var model = new FahClientSettingsModel();
            model.Port = 0;
            string error = model[nameof(FahClientSettingsModel.Port)];
            Assert.IsNotNull(error);
        }

        [Test]
        public void FahClientSettingsModel_ConnectEnabled_ReturnsTrueWhenModelHasNoError()
        {
            var model = new FahClientSettingsModel();
            model.Name = "foo";
            model.Server = "foo";
            Assert.IsTrue(model.ConnectEnabled);
        }

        [Test]
        public void FahClientSettingsModel_ConnectEnabled_SetFalseReturnsFalse()
        {
            var model = new FahClientSettingsModel();
            model.Name = "foo";
            model.Server = "foo";
            model.ConnectEnabled = false;
            Assert.IsFalse(model.ConnectEnabled);
        }

        [Test]
        public void FahClientSettingsModel_StringPropertiesSetNullAreSetEmptyString()
        {
            var model = new FahClientSettingsModel();
            model.Name = null;
            Assert.AreEqual(String.Empty, model.Name);
            model.Server = null;
            Assert.AreEqual(String.Empty, model.Server);
            model.Password = null;
            Assert.AreEqual(String.Empty, model.Password);
        }

        [Test]
        public void FahClientSettingsModel_StringPropertiesSetValuesAreTrimmed()
        {
            var model = new FahClientSettingsModel();
            model.Name = "  foo  ";
            Assert.AreEqual("foo", model.Name);
            model.Server = "bar   ";
            Assert.AreEqual("bar", model.Server);
            model.Password = "   fizz";
            Assert.AreEqual("fizz", model.Password);
        }

        [Test]
        public void FahClientSettingsModel_ValidateAcceptance_RaisesPropertyChanged()
        {
            // Arrange
            var model = new FahClientSettingsModel();
            string propertyName = null;
            model.PropertyChanged += (s, e) => propertyName = e.PropertyName;
            // Act
            model.ValidateAcceptance();
            // Assert
            Assert.AreEqual(String.Empty, propertyName);
        }

        [Test]
        public void FahClientSettingsModel_ValidateAcceptance_ReturnsTrueWhenModelHasNoError()
        {
            var model = new FahClientSettingsModel();
            model.Name = "foo";
            model.Server = "foo";
            Assert.IsTrue(model.ValidateAcceptance());
        }

        [Test]
        public void FahClientSettingsModel_ValidateAcceptance_ReturnsTrueWhenModelHasError()
        {
            var model = new FahClientSettingsModel();
            Assert.IsFalse(model.ValidateAcceptance());
        }

        [Test]
        public void FahClientSettingsModel_RefreshSlots_FromClientObjectModel()
        {
            // Arrange
            var model = new FahClientSettingsModel();
            var slotCollection = new SlotCollection();
            var slotOptions = new SlotOptions { { Options.ClientType, "foo" }, { Options.MaxPacketSize, "bar" } };
            slotCollection.Add(new Slot { ID = 0, SlotOptions = slotOptions });
            // Act
            model.RefreshSlots(slotCollection);
            // Assert
            Assert.AreEqual(1, model.Slots.Count);
            var slotModel = model.Slots.First();
            Assert.AreEqual("00", slotModel.ID);
            Assert.AreEqual("CPU", slotModel.SlotType);
            Assert.AreEqual("foo", slotModel.ClientType);
            Assert.AreEqual("bar", slotModel.MaxPacketSize);
        }
    }
}
