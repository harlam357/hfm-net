using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Proteins;

using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitContextRepositoryTests
    {
        [TestFixture]
        public class WhenInsertingNewWorkUnit : WorkUnitContextRepositoryTests
        {
            private ArtifactFolder _artifacts;
            private string _connectionString;
            private readonly Guid _clientGuid = Guid.NewGuid();
            private IWorkUnitRepository _repository;
            private bool _insertResult;

            [SetUp]
            public void BeforeEach()
            {
                _artifacts = new ArtifactFolder();
                _connectionString = $"Data Source={_artifacts.GetRandomFilePath()}";
                _repository = new TestableWorkUnitContextRepository(_connectionString);

                var settings = new ClientSettings
                {
                    Name = "GTX3090",
                    Server = "gtx3090.awesome.com",
                    Port = ClientSettings.DefaultPort,
                    Guid = _clientGuid
                };
                var assigned = DateTime.UtcNow;
                var workUnit = new WorkUnit
                {
                    ProjectID = 1,
                    ProjectRun = 1,
                    ProjectClone = 1,
                    ProjectGen = 1,
                    Assigned = assigned,
                    Finished = assigned.AddHours(6)
                };
                var protein = new Protein();

                var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
                _insertResult = _repository.Insert(workUnitModel);
            }

            [TearDown]
            public void AfterEach() => _artifacts?.Dispose();

            [Test]
            public void ThenNewClientIsCreated()
            {
                Assert.IsTrue(_insertResult);

                using var context = new WorkUnitContext(_connectionString);
                var client = context.Clients.First();
                Assert.AreNotEqual(0, client.ID);
                Assert.AreEqual("GTX3090", client.Name);
                Assert.AreEqual("gtx3090.awesome.com:36330", client.ConnectionString);
                Assert.AreEqual(_clientGuid.ToString(), client.Guid);
            }
        }

        [TestFixture]
        public class GivenFinishedAndFailedWorkUnits : WorkUnitContextRepositoryTests
        {
            private ArtifactFolder _artifacts;
            private string _connectionString;
            private readonly Guid _clientGuid = Guid.NewGuid();
            private IWorkUnitRepository _repository;

            [SetUp]
            public void BeforeEach()
            {
                _artifacts = new ArtifactFolder();
                _connectionString = $"Data Source={_artifacts.GetRandomFilePath()}";
                _repository = new TestableWorkUnitContextRepository(_connectionString);

                var settings = new ClientSettings
                {
                    Name = "GTX3090",
                    Server = "gtx3090.awesome.com",
                    Port = ClientSettings.DefaultPort,
                    Guid = _clientGuid
                };
                var assigned = DateTime.UtcNow;
                var finishedworkUnit = new WorkUnit
                {
                    ProjectID = 1,
                    ProjectRun = 1,
                    ProjectClone = 1,
                    ProjectGen = 1,
                    Assigned = assigned,
                    Finished = assigned.AddHours(6),
                    UnitResult = WorkUnitResult.FinishedUnit
                };
                assigned = assigned.AddHours(6);
                var failedWorkUnit = new WorkUnit
                {
                    ProjectID = 1,
                    ProjectRun = 1,
                    ProjectClone = 1,
                    ProjectGen = 1,
                    Assigned = assigned,
                    Finished = assigned.AddHours(6),
                    UnitResult = WorkUnitResult.BadWorkUnit
                };
                var protein = new Protein();

                var workUnitModel = CreateWorkUnitModel(settings, finishedworkUnit, protein);
                _repository.Insert(workUnitModel);
                workUnitModel = CreateWorkUnitModel(settings, failedWorkUnit, protein);
                _repository.Insert(workUnitModel);
            }

            [TearDown]
            public void AfterEach() => _artifacts?.Dispose();

            [Test]
            public void ThenRepositoryReturnsCounts()
            {
                long finished = _repository.CountCompleted("GTX3090", null);
                long failed = _repository.CountFailed("GTX3090", null);
                Assert.AreEqual(1, finished);
                Assert.AreEqual(1, failed);
            }
        }

        private static WorkUnitModel CreateWorkUnitModel(ClientSettings settings, WorkUnit workUnit, Protein protein)
        {
            var slotModel = new SlotModel(new NullClient { Settings = settings });
            var workUnitModel = new WorkUnitModel(slotModel, workUnit);
            workUnitModel.CurrentProtein = protein;
            return workUnitModel;
        }
    }

    public class TestableWorkUnitContextRepository : WorkUnitContextRepository
    {
        private readonly string _connectionString;

        public TestableWorkUnitContextRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override WorkUnitContext CreateWorkUnitContext()
        {
            var context = new WorkUnitContext(_connectionString);
            context.Database.EnsureCreated();
            return context;
        }
    }
}
