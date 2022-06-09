using NUnit.Framework;

using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Client;

[TestFixture]
public class ClientProjectIsDuplicateValidationRuleTests
{
    [Test]
    public void ClientProjectIsDuplicateValidationRule_FindDuplicateProjects_WhenProjectsAreDuplicates()
    {
        // Arrange
        var clientData1 = CreateFahClientData(new NullClient());
        clientData1.WorkUnitModel = CreateWorkUnitModel(clientData1, new WorkUnit { ProjectID = 1 });
        var clientData2 = CreateFahClientData(new NullClient());
        clientData2.WorkUnitModel = CreateWorkUnitModel(clientData2, new WorkUnit { ProjectID = 1 });
        var collection = new List<IClientData> { clientData1, clientData2 };
        var rule = new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(collection));
        // Act
        collection.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsTrue(clientData1.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsTrue(clientData2.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
    }

    [Test]
    public void ClientProjectIsDuplicateValidationRule_FindDuplicateProjects_WhenProjectsAreNotDuplicates()
    {
        // Arrange
        var clientData1 = CreateFahClientData(new NullClient());
        clientData1.WorkUnitModel = CreateWorkUnitModel(clientData1, new WorkUnit { ProjectID = 1 });
        var clientData2 = CreateFahClientData(new NullClient());
        clientData2.WorkUnitModel = CreateWorkUnitModel(clientData2, new WorkUnit { ProjectID = 2 });
        var collection = new List<IClientData> { clientData1, clientData2 };
        var rule = new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(collection));
        // Act
        collection.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsFalse(clientData1.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsFalse(clientData2.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
    }

    [Test]
    public void ClientProjectIsDuplicateValidationRule_FindDuplicateProjects_WhenSomeProjectsAreDuplicates()
    {
        // Arrange
        var clientData1 = CreateFahClientData(new NullClient());
        clientData1.WorkUnitModel = CreateWorkUnitModel(clientData1, new WorkUnit { ProjectID = 1 });
        var clientData2 = CreateFahClientData(new NullClient());
        clientData2.WorkUnitModel = CreateWorkUnitModel(clientData2, new WorkUnit { ProjectID = 2 });
        var clientData3 = CreateFahClientData(new NullClient());
        clientData3.WorkUnitModel = CreateWorkUnitModel(clientData2, new WorkUnit { ProjectID = 1 });
        var collection = new List<IClientData> { clientData1, clientData2, clientData3 };
        var rule = new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(collection));
        // Act
        collection.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsTrue(clientData1.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsFalse(clientData2.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsTrue(clientData3.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
    }

    private static FahClientData CreateFahClientData(IClient client) =>
        new(new InMemoryPreferencesProvider(), client, default, SlotIdentifier.NoSlotID);

    private static WorkUnitModel CreateWorkUnitModel(IClientData clientData, WorkUnit workUnit) =>
        new(clientData, workUnit, null);
}
