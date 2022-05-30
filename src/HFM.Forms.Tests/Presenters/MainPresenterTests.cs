using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.ScheduledTasks;
using HFM.Core.UserStats;
using HFM.Forms.Models;
using HFM.Forms.Services.Mocks;
using HFM.Preferences;

using Moq;

using NUnit.Framework;

namespace HFM.Forms.Presenters;

[TestFixture]
public class MainPresenterTests
{
    [TestFixture]
    public class WhenClosing : MainPresenterTests
    {
        [Test]
        public void FiresApplicationUpdateUsingLocalProcessService()
        {
            var presenter = CreatePresenter();
            presenter.ApplicationUpdateModel = new ApplicationUpdateModel(null)
            {
                SelectedUpdateFileLocalFilePath = @"C:\foo\bar.msi",
                SelectedUpdateFileIsReadyToBeExecuted = true
            };
            var localProcessService = new MockLocalProcessService();
            presenter.FormClosing(null, localProcessService);
            Assert.AreEqual(1, localProcessService.Invocations.Count);
            Assert.AreEqual(@"C:\foo\bar.msi", localProcessService.Invocations.First().FileName);
        }
    }

    private static MainPresenter CreatePresenter()
    {
        var preferences = new InMemoryPreferencesProvider();
        var clientConfiguration = new ClientConfiguration(null, preferences, new ClientFactory());
        var userStatsScheduledTask = new UserStatsScheduledTask(preferences, null, Mock.Of<IUserStatsService>(), new UserStatsDataContainer());
        return new MainPresenter(new MainModel(preferences), null, null, null, clientConfiguration, null, userStatsScheduledTask);
    }
}
