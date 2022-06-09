using HFM.Preferences;

using NUnit.Framework;

namespace HFM.Core.Client;

[TestFixture]
public class FahClientDataTests
{
    [Test]
    public void FahClientData_Status_IsGivenStatusWhenWorkUnitDoesNotHaveProject()
    {
        // Arrange
        var client = new NullClient();
        var clientData = CreateFahClientData(client, SlotStatus.Running, 0);
        // Act
        var status = clientData.Status;
        // Assert
        Assert.AreEqual(SlotStatus.Running, status);
    }

    [Test]
    public void FahClientData_Status_IsRunningNoFrameTimesWhenGivenStatusIsRunningAndWorkUnitHasProjectAndNoProgress()
    {
        // Arrange
        var client = new NullClient();
        var clientData = CreateFahClientData(client, SlotStatus.Running, 0);
        clientData.WorkUnitModel.WorkUnit.ProjectID = 1;
        // Act
        var status = clientData.Status;
        // Assert
        Assert.AreEqual(SlotStatus.RunningNoFrameTimes, status);
    }

    private static FahClientData CreateFahClientData(IClient client, SlotStatus status, int slotID) =>
        new(new InMemoryPreferencesProvider(), client, status, slotID);
}
