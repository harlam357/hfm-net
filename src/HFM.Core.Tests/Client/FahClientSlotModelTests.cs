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
        var slotModel = new FahClientSlotModel(client, SlotStatus.Running, 0);
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
        var slotModel = new FahClientSlotModel(client, SlotStatus.Running, 0);
        slotModel.WorkUnitModel.WorkUnit.ProjectID = 1;
        // Act
        var status = slotModel.Status;
        // Assert
        Assert.AreEqual(SlotStatus.RunningNoFrameTimes, status);
    }
}
