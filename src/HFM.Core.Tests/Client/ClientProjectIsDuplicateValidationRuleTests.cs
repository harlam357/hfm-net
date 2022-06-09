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
        var slotModel1 = CreateFahClientData(new NullClient());
        slotModel1.WorkUnitModel = CreateWorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
        var slotModel2 = CreateFahClientData(new NullClient());
        slotModel2.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 1 });
        var collection = new List<IClientData> { slotModel1, slotModel2 };
        var rule = new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(collection));
        // Act
        collection.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsTrue(slotModel1.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsTrue(slotModel2.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
    }

    [Test]
    public void ClientProjectIsDuplicateValidationRule_FindDuplicateProjects_WhenProjectsAreNotDuplicates()
    {
        // Arrange
        var slotModel1 = CreateFahClientData(new NullClient());
        slotModel1.WorkUnitModel = CreateWorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
        var slotModel2 = CreateFahClientData(new NullClient());
        slotModel2.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 2 });
        var collection = new List<IClientData> { slotModel1, slotModel2 };
        var rule = new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(collection));
        // Act
        collection.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsFalse(slotModel1.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsFalse(slotModel2.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
    }

    [Test]
    public void ClientProjectIsDuplicateValidationRule_FindDuplicateProjects_WhenSomeProjectsAreDuplicates()
    {
        // Arrange
        var slotModel1 = CreateFahClientData(new NullClient());
        slotModel1.WorkUnitModel = CreateWorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
        var slotModel2 = CreateFahClientData(new NullClient());
        slotModel2.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 2 });
        var slotModel3 = CreateFahClientData(new NullClient());
        slotModel3.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 1 });
        var collection = new List<IClientData> { slotModel1, slotModel2, slotModel3 };
        var rule = new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(collection));
        // Act
        collection.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsTrue(slotModel1.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsFalse(slotModel2.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
        Assert.IsTrue(slotModel3.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key));
    }

    private static FahClientData CreateFahClientData(IClient client) =>
        new(new InMemoryPreferencesProvider(), client, default, SlotIdentifier.NoSlotID);

    private static WorkUnitModel CreateWorkUnitModel(IClientData clientData, WorkUnit workUnit) =>
        new(clientData, workUnit, null);
}
