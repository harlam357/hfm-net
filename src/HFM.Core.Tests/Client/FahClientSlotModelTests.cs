using HFM.Preferences;

using NUnit.Framework;

namespace HFM.Core.Client;

[TestFixture]
public class FahClientSlotModelTests
{
    [Test]
    public void FahClientSlotModel_Status_IsGivenStatusWhenWorkUnitDoesNotHaveProject()
    {
        // Arrange
        var client = new NullClient();
        var slotModel = CreateFahClientSlotModel(client, SlotStatus.Running, 0);
        // Act
        var status = slotModel.Status;
        // Assert
        Assert.AreEqual(SlotStatus.Running, status);
    }

    [Test]
    public void FahClientSlotModel_Status_IsRunningNoFrameTimesWhenGivenStatusIsRunningAndWorkUnitHasProjectAndNoProgress()
    {
        // Arrange
        var client = new NullClient();
        var slotModel = CreateFahClientSlotModel(client, SlotStatus.Running, 0);
        slotModel.WorkUnitModel.WorkUnit.ProjectID = 1;
        // Act
        var status = slotModel.Status;
        // Assert
        Assert.AreEqual(SlotStatus.RunningNoFrameTimes, status);
    }

    private static FahClientSlotModel CreateFahClientSlotModel(IClient client, SlotStatus status, int slotID) =>
        new(new InMemoryPreferencesProvider(), client, status, slotID);
}
