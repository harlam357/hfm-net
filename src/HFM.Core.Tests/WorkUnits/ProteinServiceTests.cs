
using System;
using System.IO;
using System.Linq;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.Data;
using HFM.Core.Services;
using HFM.Proteins;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class ProteinServiceTests
    {
        [Test]
        public void ProteinService_Get_AvailableProtein()
        {
            // Arrange
            var protein = CreateValidProtein(2483);
            var dataContainer = new ProteinDataContainer();
            dataContainer.Data.Add(protein);
            var service = new ProteinService(dataContainer, null, null);
            // Act
            var p = service.Get(2483);
            // Assert
            Assert.AreSame(protein, p);
        }

        [Test]
        public void ProteinService_Get_UnavailableProtein()
        {
            // Arrange
            var service = new ProteinService();
            // Act
            var p = service.Get(2482);
            // Assert
            Assert.IsNull(p);
        }

        [Test]
        public void ProteinService_Get_UnavailableProtein_AddsTheProjectIDToLastProjectRefresh()
        {
            // Arrange
            var summaryService = MockRepository.GenerateStub<IProjectSummaryService>();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            // Act
            service.GetOrRefresh(2482);
            // Assert
            Assert.IsTrue(service.LastProjectRefresh.ContainsKey(2482));
        }

        [Test]
        public void ProteinService_GetOrRefresh_CalledMultipleTimesOnlyTriggersOneRefresh_ByLastRefresh()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceMockRepeatOnce();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            // Act
            var p = service.GetOrRefresh(2482);
            Assert.IsNull(p);
            // Call twice to internally exercise the LastRefresh value
            p = service.GetOrRefresh(2482);
            Assert.IsNull(p);
            // Assert
            summaryService.VerifyAllExpectations();
        }

        [Test]
        public void ProteinService_GetOrRefresh_CalledMultipleTimesOnlyTriggersOneRefresh_ByLastProjectRefresh()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceMockRepeatOnce();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            // Act
            var p = service.GetOrRefresh(2482);
            Assert.IsNull(p);
            // Call twice to internally exercise the projects not found list
            service.LastRefresh = null;
            p = service.GetOrRefresh(2482);
            Assert.IsNull(p);
            // Assert
            summaryService.VerifyAllExpectations();
        }

        [Test]
        public void ProteinService_GetOrRefresh_AllowsRefreshWhenLastRefreshElapsed()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceMockRepeatOnce();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            // set to over the elapsed time (1 hour)
            service.LastRefresh = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(61));
            // Act
            var p = service.GetOrRefresh(6940);
            Assert.IsNotNull(p);
            // Assert
            summaryService.VerifyAllExpectations();
        }

        [Test]
        public void ProteinService_GetOrRefresh_RemovesFromLastProjectRefresh()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceStub();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            // Set project not found to exercise removal code
            service.LastProjectRefresh.Add(6940, DateTime.MinValue);
            // Act
            Protein p = service.GetOrRefresh(6940);
            // Assert
            Assert.IsNotNull(p);
            Assert.IsFalse(service.LastProjectRefresh.ContainsKey(6940));
        }

        [Test]
        public void ProteinService_GetProjects_Test()
        {
            // Arrange
            var dataContainer = new ProteinDataContainer();
            var projects = Enumerable.Range(1, 5).ToList();
            foreach (int projectNumber in projects)
            {
                dataContainer.Data.Add(CreateValidProtein(projectNumber));
            }
            var service = new ProteinService(dataContainer, null, null);
            // Act
            var serviceProjects = service.GetProjects();
            // Assert
            Assert.IsTrue(projects.SequenceEqual(serviceProjects));
        }

        [Test]
        public void ProteinService_RefreshRemovesFromProjectsNotFound_Test()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceStub();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            service.LastProjectRefresh.Add(6940, DateTime.MinValue);
            // Act
            service.Refresh(null);
            // Assert
            Assert.IsFalse(service.LastProjectRefresh.ContainsKey(6940));
        }

        [Test]
        public void ProteinService_Refresh_UpdatesRefreshProperties()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceStub();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            service.LastProjectRefresh.Add(2968, DateTime.MinValue);
            service.LastRefresh = DateTime.MinValue;
            // Act
            service.Refresh(null);
            // Assert
            Assert.AreNotEqual(DateTime.MinValue, service.LastProjectRefresh[2968]);
            Assert.AreNotEqual(DateTime.MinValue, service.LastRefresh);
        }

        [Test]
        public void ProteinService_Refresh_RefreshesProjects()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceStub();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            Assert.AreEqual(0, service.GetProjects().Count());
            // Act
            service.Refresh(null);
            // Assert
            Assert.AreNotEqual(624, service.GetProjects().Count());
        }

        [Test]
        public void ProteinService_Refresh_ReturnsProteinChanges()
        {
            // Arrange
            var summaryService = CreateProjectSummaryServiceStub();
            var service = new ProteinService(new ProteinDataContainer(), summaryService, new Logging.DebugLogger());
            // Act
            var changes = service.Refresh(null);
            // Assert
            Assert.AreEqual(624, changes.Count);
        }

        private static IProjectSummaryService CreateProjectSummaryServiceStub()
        {
            var summaryService = MockRepository.GenerateStub<IProjectSummaryService>();
            summaryService.Stub(x => x.CopyToStream(null, null)).IgnoreArguments()
                .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
                {
                    File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
                    return true;
                }));
            return summaryService;
        }

        private static IProjectSummaryService CreateProjectSummaryServiceMockRepeatOnce()
        {
            var summaryService = MockRepository.GenerateMock<IProjectSummaryService>();
            summaryService.Expect(x => x.CopyToStream(null, null)).IgnoreArguments()
                .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
                {
                    File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
                    return true;
                })).Repeat.Once();
            return summaryService;
        }

        private static Protein CreateValidProtein(int projectNumber)
        {
            return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
        }
    }
}
