using NUnit.Framework;

using HFM.Core.WorkUnits;

namespace HFM.Core.Client;

[TestFixture]
public class SlotModelProjectIsDuplicateRuleTests
{
    [Test]
    public void SlotModelProjectIsDuplicateRule_FindDuplicateProjects_WhenProjectsAreDuplicates()
    {
        // Arrange
        var slotModel1 = new SlotModel(new NullClient());
        slotModel1.WorkUnitModel = CreateWorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
        var slotModel2 = new SlotModel(new NullClient());
        slotModel2.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 1 });
        var slots = new List<SlotModel> { slotModel1, slotModel2 };
        var rule = new SlotModelProjectIsDuplicateRule(SlotModelProjectIsDuplicateRule.FindDuplicateProjects(slots));
        // Act
        slots.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsTrue(slotModel1.ProjectIsDuplicate);
        Assert.IsTrue(slotModel2.ProjectIsDuplicate);
    }

    [Test]
    public void SlotModelProjectIsDuplicateRule_FindDuplicateProjects_WhenProjectsAreNotDuplicates()
    {
        // Arrange
        var slotModel1 = new SlotModel(new NullClient());
        slotModel1.WorkUnitModel = CreateWorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
        var slotModel2 = new SlotModel(new NullClient());
        slotModel2.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 2 });
        var slots = new List<SlotModel> { slotModel1, slotModel2 };
        var rule = new SlotModelProjectIsDuplicateRule(SlotModelProjectIsDuplicateRule.FindDuplicateProjects(slots));
        // Act
        slots.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsFalse(slotModel1.ProjectIsDuplicate);
        Assert.IsFalse(slotModel2.ProjectIsDuplicate);
    }

    [Test]
    public void SlotModelProjectIsDuplicateRule_FindDuplicateProjects_WhenSomeProjectsAreDuplicates()
    {
        // Arrange
        var slotModel1 = new SlotModel(new NullClient());
        slotModel1.WorkUnitModel = CreateWorkUnitModel(slotModel1, new WorkUnit { ProjectID = 1 });
        var slotModel2 = new SlotModel(new NullClient());
        slotModel2.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 2 });
        var slotModel3 = new SlotModel(new NullClient());
        slotModel3.WorkUnitModel = CreateWorkUnitModel(slotModel2, new WorkUnit { ProjectID = 1 });
        var slots = new List<SlotModel> { slotModel1, slotModel2, slotModel3 };
        var rule = new SlotModelProjectIsDuplicateRule(SlotModelProjectIsDuplicateRule.FindDuplicateProjects(slots));
        // Act
        slots.ForEach(x => rule.Validate(x));
        // Assert
        Assert.IsTrue(slotModel1.ProjectIsDuplicate);
        Assert.IsFalse(slotModel2.ProjectIsDuplicate);
        Assert.IsTrue(slotModel3.ProjectIsDuplicate);
    }

    private static WorkUnitModel CreateWorkUnitModel(SlotModel slotModel, WorkUnit workUnit) =>
        new(slotModel, workUnit, null);
}
