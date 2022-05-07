using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Proteins;

using Microsoft.Data.Sqlite;

using NUnit.Framework;

namespace HFM.Core.Data;

[TestFixture]
public class WorkUnitContextRepositoryTests
{
    private readonly DateTime _assigned = DateTime.UtcNow.Normalize();

    [TestFixture]
    public class WhenUpdatingNewWorkUnit : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private readonly Guid _clientGuid = Guid.NewGuid();
        private IWorkUnitRepository _repository;
        private long _updateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort,
                Guid = _clientGuid
            };
            var workUnit = new WorkUnit
            {
                FoldingID = "harlam357",
                Team = 32,
                CoreVersion = new Version("0.0.18"),
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Platform = new WorkUnitPlatform("CUDA")
                {
                    DriverVersion = "123",
                    ComputeVersion = "456",
                    CUDAVersion = "789"
                },
                UnitResult = WorkUnitResult.FinishedUnit,
                Assigned = _assigned,
                Finished = _assigned.AddHours(6),
                FramesObserved = 1,
                Frames = new Dictionary<int, LogLineFrameData>
                {
                    {
                        99, new LogLineFrameData
                        {
                            ID = 99,
                            RawFramesComplete = 990_000,
                            RawFramesTotal = 1_000_000,
                            TimeStamp = TimeSpan.FromSeconds(10),
                            Duration = TimeSpan.Zero
                        }
                    },
                    {
                        100, new LogLineFrameData
                        {
                            ID = 100,
                            RawFramesComplete = 1_000_000,
                            RawFramesTotal = 1_000_000,
                            TimeStamp = TimeSpan.FromSeconds(38),
                            Duration = TimeSpan.FromSeconds(28)
                        }
                    }
                },
                UnitID = "0x0000005a000000150000467700000003"
            };
            var protein = new Protein
            {
                ProjectNumber = 1,
                NumberOfAtoms = 350000,
                PreferredDays = 1.0,
                MaximumDays = 3.0,
                Credit = 25000.0,
                Frames = 100,
                Core = "GRO_A8",
                KFactor = 0.3
            };

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein, 1, "AMD Ryzen 7 5800X", 14);
            _updateResult = _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenNewClientIsInserted()
        {
            Assert.AreEqual(1, _updateResult);

            using var context = new WorkUnitContext(_connection);
            var client = context.Clients.First();
            Assert.AreNotEqual(0, client.ID);
            Assert.AreEqual("GTX3090", client.Name);
            Assert.AreEqual("gtx3090.awesome.com:36330", client.ConnectionString);
            Assert.AreEqual(_clientGuid.ToString(), client.Guid);
        }

        [Test]
        public void ThenNewProteinIsInserted()
        {
            Assert.AreEqual(1, _updateResult);

            using var context = new WorkUnitContext(_connection);
            var protein = context.Proteins.First();
            Assert.AreNotEqual(0, protein.ID);
            Assert.AreEqual(1, protein.ProjectID);
            Assert.AreEqual(25000.0, protein.Credit);
            Assert.AreEqual(0.3, protein.KFactor);
            Assert.AreEqual(100, protein.Frames);
            Assert.AreEqual("GRO_A8", protein.Core);
            Assert.AreEqual(350000, protein.Atoms);
            Assert.AreEqual(1.0, protein.TimeoutDays);
            Assert.AreEqual(3.0, protein.ExpirationDays);
        }

        [Test]
        public void ThenNewPlatformIsInserted()
        {
            Assert.AreEqual(1, _updateResult);

            using var context = new WorkUnitContext(_connection);
            var platform = context.Platforms.First();
            Assert.AreEqual("7", platform.ClientVersion);
            Assert.AreEqual("Windows", platform.OperatingSystem);
            Assert.AreEqual("CUDA", platform.Implementation);
            Assert.AreEqual("AMD Ryzen 7 5800X", platform.Processor);
            Assert.AreEqual(14, platform.Threads);
            Assert.AreEqual("123", platform.DriverVersion);
            Assert.AreEqual("456", platform.ComputeVersion);
            Assert.AreEqual("789", platform.CUDAVersion);
        }

        [Test]
        public void ThenPlatformIsMappedToWorkUnitRow()
        {
            var row = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None).First();
            Assert.AreEqual("7", row.ClientVersion);
            Assert.AreEqual("Windows", row.OperatingSystem);
            Assert.AreEqual("CUDA", row.PlatformImplementation);
            Assert.AreEqual("AMD Ryzen 7 5800X", row.PlatformProcessor);
            Assert.AreEqual(14, row.PlatformThreads);
            Assert.AreEqual("123", row.DriverVersion);
            Assert.AreEqual("456", row.ComputeVersion);
            Assert.AreEqual("789", row.CUDAVersion);
        }

        [Test]
        public void ThenPlatformFieldsCanBeQueried()
        {
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.ClientVersion, WorkUnitQueryOperator.Equal, "7"), BonusCalculation.None).Count);
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.OperatingSystem, WorkUnitQueryOperator.GreaterThan, "Linux"), BonusCalculation.None).Count);
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.PlatformImplementation, WorkUnitQueryOperator.GreaterThanOrEqual, "CUDA"), BonusCalculation.None).Count);
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.PlatformProcessor, WorkUnitQueryOperator.LessThan, "Intel"), BonusCalculation.None).Count);
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.PlatformThreads, WorkUnitQueryOperator.LessThanOrEqual, 14), BonusCalculation.None).Count);
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.DriverVersion, WorkUnitQueryOperator.Like, "1%"), BonusCalculation.None).Count);
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.ComputeVersion, WorkUnitQueryOperator.NotLike, "1%"), BonusCalculation.None).Count);
            Assert.AreEqual(1, _repository.Fetch(new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.CUDAVersion, WorkUnitQueryOperator.NotEqual, "123"), BonusCalculation.None).Count);
        }

        [Test]
        public void ThenNewWorkUnitIsInserted()
        {
            Assert.AreEqual(1, _updateResult);

            using var context = new WorkUnitContext(_connection);
            var workUnit = context.WorkUnits.First();
            Assert.AreNotEqual(0, workUnit.ID);
            Assert.AreEqual("harlam357", workUnit.DonorName);
            Assert.AreEqual(32, workUnit.DonorTeam);
            Assert.AreEqual("0.0.18", workUnit.CoreVersion);
            Assert.AreEqual(WorkUnitResultString.FinishedUnit, workUnit.Result);
            Assert.AreEqual(_assigned, workUnit.Assigned);
            Assert.AreEqual(_assigned.AddHours(6), workUnit.Finished);
            Assert.AreEqual(2, workUnit.ProjectRun);
            Assert.AreEqual(3, workUnit.ProjectClone);
            Assert.AreEqual(4, workUnit.ProjectGen);
            Assert.AreEqual("0x0000005a000000150000467700000003", workUnit.HexID);
            Assert.AreEqual(100, workUnit.FramesCompleted);
            Assert.AreEqual(28, workUnit.FrameTimeInSeconds);
            Assert.AreEqual(context.Proteins.First().ID, workUnit.ProteinID);
            Assert.AreEqual(context.Clients.First().ID, workUnit.ClientID);
            Assert.AreEqual(1, workUnit.ClientSlot);
        }

        [Test]
        public void ThenNewWorkUnitFramesAreInserted()
        {
            Assert.AreEqual(1, _updateResult);

            using var context = new WorkUnitContext(_connection);
            long workUnitID = context.WorkUnits.First().ID;
            var frames = context.WorkUnitFrames.Where(x => x.WorkUnitID == workUnitID).ToDictionary(x => x.FrameID);

            var f = frames[99];
            Assert.AreEqual(99, f.FrameID);
            Assert.AreEqual(990_000, f.RawFramesComplete);
            Assert.AreEqual(1_000_000, f.RawFramesTotal);
            Assert.AreEqual(TimeSpan.FromSeconds(10), f.TimeStamp);
            Assert.AreEqual(TimeSpan.Zero, f.Duration);

            f = frames[100];
            Assert.AreEqual(100, f.FrameID);
            Assert.AreEqual(1_000_000, f.RawFramesComplete);
            Assert.AreEqual(1_000_000, f.RawFramesTotal);
            Assert.AreEqual(TimeSpan.FromSeconds(38), f.TimeStamp);
            Assert.AreEqual(TimeSpan.FromSeconds(28), f.Duration);
        }
    }

    [TestFixture]
    public class WhenUpdatingExistingIncompleteWorkUnit : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private IWorkUnitRepository _repository;
        private long _firstUpdateResult;
        private long _secondUpdateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort
            };
            var workUnit = new WorkUnit
            {
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Assigned = _assigned,
                Frames = new Dictionary<int, LogLineFrameData>
                {
                    { 1, CreateLogLineFrameData(1) }
                }
            };
            var protein = new Protein
            {
                ProjectNumber = 1
            };

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _firstUpdateResult = _repository.Update(workUnitModel);

            workUnit.Frames[2] = CreateLogLineFrameData(2);
            workUnit.Frames[3] = CreateLogLineFrameData(3);
            workUnit.Frames[4] = CreateLogLineFrameData(4);

            workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _secondUpdateResult = _repository.Update(workUnitModel);
        }

        private static LogLineFrameData CreateLogLineFrameData(int id) =>
            new()
            {
                ID = id,
                RawFramesComplete = 10_000 * id,
                RawFramesTotal = 1_000_000,
                TimeStamp = TimeSpan.FromMinutes(1 * id),
                Duration = TimeSpan.FromMinutes(1)
            };

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenUpdateReturnsTheSameWorkUnitID() => Assert.AreEqual(_firstUpdateResult, _secondUpdateResult);

        [Test]
        public void ThenNewWorkUnitFramesAreInserted()
        {
            using var context = new WorkUnitContext(_connection);
            long workUnitID = context.WorkUnits.First().ID;
            int count = context.WorkUnitFrames.Count(x => x.WorkUnitID == workUnitID);
            Assert.AreEqual(4, count);
        }

        [Test]
        public void ThenFinishedValueIsNull()
        {
            using var context = new WorkUnitContext(_connection);
            var workUnit = context.WorkUnits.First();
            Assert.IsNull(workUnit.Finished);
        }

        [Test]
        public void ThenResultIsNull()
        {
            using var context = new WorkUnitContext(_connection);
            var workUnit = context.WorkUnits.First();
            Assert.IsNull(workUnit.Result);
        }
    }

    [TestFixture]
    public class WhenUpdatingInvalidWorkUnit : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private IWorkUnitRepository _repository;
        private long _updateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings();
            var workUnit = new WorkUnit();
            var protein = new Protein();

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _updateResult = _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenUpdateReturnsNegativeOne() => Assert.AreEqual(-1, _updateResult);
    }

    [TestFixture]
    public class WhenUpdatingWithExistingClientGuid : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private readonly Guid _clientGuid = Guid.NewGuid();
        private IWorkUnitRepository _repository;
        private long _updateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort,
                Guid = _clientGuid
            };
            var workUnit = new WorkUnit
            {
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Assigned = _assigned,
                Finished = _assigned.AddHours(6)
            };
            var protein = new Protein
            {
                ProjectNumber = 1
            };

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _repository.Update(workUnitModel);

            workUnit.Assigned = _assigned.AddHours(24);
            workUnit.Finished = _assigned.AddHours(30);

            workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _updateResult = _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenExistingClientIsReferenced()
        {
            Assert.AreEqual(2, _updateResult);

            using var context = new WorkUnitContext(_connection);
            Assert.AreEqual(1, context.Clients.Count());
            Assert.AreEqual(2, context.WorkUnits.Count());

            var workUnit = context.WorkUnits.OrderByDescending(x => x.ID).First();
            Assert.AreEqual(context.Clients.First().ID, workUnit.ClientID);
        }
    }

    [TestFixture]
    public class WhenUpdatingWithExistingClientGuidAndNameOrConnectionChanged : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private readonly Guid _clientGuid = Guid.NewGuid();
        private IWorkUnitRepository _repository;
        private long _updateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort,
                Guid = _clientGuid
            };
            var workUnit = new WorkUnit
            {
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Assigned = _assigned,
                Finished = _assigned.AddHours(6)
            };
            var protein = new Protein
            {
                ProjectNumber = 1
            };

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _repository.Update(workUnitModel);

            settings.Name = "GTX4080";
            settings.Server = "gtx4080.awesome.com";

            workUnit.Assigned = _assigned.AddHours(24);
            workUnit.Finished = _assigned.AddHours(30);

            workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _updateResult = _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenNewClientIsInserted()
        {
            Assert.AreEqual(2, _updateResult);

            using var context = new WorkUnitContext(_connection);
            Assert.AreEqual(2, context.Clients.Count());
            Assert.AreEqual(2, context.WorkUnits.Count());

            var client = context.Clients.OrderByDescending(x => x.ID).First();
            Assert.AreNotEqual(0, client.ID);
            Assert.AreEqual("GTX4080", client.Name);
            Assert.AreEqual("gtx4080.awesome.com:36330", client.ConnectionString);
            Assert.AreEqual(_clientGuid.ToString(), client.Guid);
        }
    }

    [TestFixture]
    public class WhenUpdatingWithExistingClientNameAndConnection : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private readonly Guid _clientGuid = Guid.NewGuid();
        private IWorkUnitRepository _repository;
        private long _updateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort
            };
            var workUnit = new WorkUnit
            {
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Assigned = _assigned,
                Finished = _assigned.AddHours(6)
            };
            var protein = new Protein
            {
                ProjectNumber = 1
            };

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _repository.Update(workUnitModel);

            settings.Guid = _clientGuid;

            workUnit.Assigned = _assigned.AddHours(24);
            workUnit.Finished = _assigned.AddHours(30);

            workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _updateResult = _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenExistingClientIsReferencedAndUpdatedWithGuidValue()
        {
            Assert.AreEqual(2, _updateResult);

            using var context = new WorkUnitContext(_connection);
            Assert.AreEqual(1, context.Clients.Count());
            Assert.AreEqual(2, context.WorkUnits.Count());

            var workUnit = context.WorkUnits.OrderByDescending(x => x.ID).First();
            var client = context.Clients.First();
            Assert.AreEqual(client.ID, workUnit.ClientID);
            Assert.AreEqual(client.Guid, _clientGuid.ToString());
        }
    }

    [TestFixture]
    public class WhenUpdatingExistingProtein : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private IWorkUnitRepository _repository;
        private long _updateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort
            };
            var workUnit = new WorkUnit
            {
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Assigned = _assigned,
                Finished = _assigned.AddHours(6)
            };
            var protein = new Protein
            {
                ProjectNumber = 1,
                NumberOfAtoms = 350000,
                PreferredDays = 1.0,
                MaximumDays = 3.0,
                Credit = 25000.0,
                Frames = 100,
                Core = "GRO_A8",
                KFactor = 0.3
            };

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _repository.Update(workUnitModel);

            workUnit.Assigned = _assigned.AddHours(24);
            workUnit.Finished = _assigned.AddHours(30);

            workUnitModel = CreateWorkUnitModel(settings, workUnit, protein);
            _updateResult = _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenExistingProteinIsReferenced()
        {
            Assert.AreEqual(2, _updateResult);

            using var context = new WorkUnitContext(_connection);
            Assert.AreEqual(1, context.Proteins.Count());
            Assert.AreEqual(2, context.WorkUnits.Count());

            var workUnit = context.WorkUnits.OrderByDescending(x => x.ID).First();
            Assert.AreEqual(context.Proteins.First().ID, workUnit.ProteinID);
        }
    }

    [TestFixture]
    public class WhenUpdatingExistingPlatform : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private IWorkUnitRepository _repository;
        private long _updateResult;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort
            };
            var workUnit = new WorkUnit
            {
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Platform = new WorkUnitPlatform("CUDA"),
                Assigned = _assigned,
                Finished = _assigned.AddHours(6)
            };

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, new Protein(), processor: "AMD Ryzen 7 5800X", threads: 14);
            _repository.Update(workUnitModel);

            workUnit.Assigned = _assigned.AddHours(24);
            workUnit.Finished = _assigned.AddHours(30);

            workUnitModel = CreateWorkUnitModel(settings, workUnit, new Protein(), processor: "AMD Ryzen 7 5800X", threads: 14);
            _updateResult = _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenExistingPlatformIsReferenced()
        {
            Assert.AreEqual(2, _updateResult);

            using var context = new WorkUnitContext(_connection);
            Assert.AreEqual(1, context.Platforms.Count());
            Assert.AreEqual(2, context.WorkUnits.Count());

            var workUnit = context.WorkUnits.OrderByDescending(x => x.ID).First();
            Assert.AreEqual(context.Platforms.First().ID, workUnit.PlatformID);
        }
    }

    [TestFixture]
    public class GivenFinishedAndFailedWorkUnits : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private readonly Guid _clientGuid = Guid.NewGuid();
        private IWorkUnitRepository _repository;
        private readonly DateTime _utcNow = DateTime.UtcNow;

        [SetUp]
        public void BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort,
                Guid = _clientGuid
            };
            InsertFinishedAndFailedAssignedAt(_utcNow, settings);
            InsertFinishedAndFailedAssignedAt(new DateTime(2020, 1, 1), settings);
        }

        private void InsertFinishedAndFailedAssignedAt(DateTime assigned, ClientSettings settings)
        {
            var protein = new Protein();
            var finishedWorkUnit = new WorkUnit
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

            var workUnitModel = CreateWorkUnitModel(settings, finishedWorkUnit, protein, 1);
            _repository.Update(workUnitModel);
            workUnitModel = CreateWorkUnitModel(settings, failedWorkUnit, protein, 1);
            _repository.Update(workUnitModel);
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void WhenClientStartTimeIsNullThenRepositoryReturnsAllFinishedAndFailedCounts()
        {
            long finished = _repository.CountCompleted("GTX3090 Slot 01", null);
            long failed = _repository.CountFailed("GTX3090 Slot 01", null);
            Assert.AreEqual(2, finished);
            Assert.AreEqual(2, failed);
        }

        [Test]
        public void WhenClientStartTimeIsNotNullThenRepositoryReturnsCountsFinishedAfterDateTime()
        {
            long finished = _repository.CountCompleted("GTX3090 Slot 01", _utcNow);
            long failed = _repository.CountFailed("GTX3090 Slot 01", _utcNow);
            Assert.AreEqual(1, finished);
            Assert.AreEqual(1, failed);
        }
    }

    [TestFixture]
    public class WhenDeletingExistingWorkUnit : WorkUnitContextRepositoryTests
    {
        private SqliteConnection _connection;
        private IWorkUnitRepository _repository;
        private int _deleteResult;

        [SetUp]
        public async Task BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableWorkUnitContextRepository(_connection);

            var settings = new ClientSettings
            {
                Name = "GTX3090",
                Server = "gtx3090.awesome.com",
                Port = ClientSettings.DefaultPort
            };
            var workUnit = new WorkUnit
            {
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                Assigned = _assigned,
                Finished = _assigned.AddHours(6),
                Platform = new WorkUnitPlatform("CPU"),
                Frames = new Dictionary<int, LogLineFrameData>
                {
                    {
                        99, new LogLineFrameData
                        {
                            ID = 99,
                            RawFramesComplete = 990_000,
                            RawFramesTotal = 1_000_000,
                            TimeStamp = TimeSpan.FromSeconds(10),
                            Duration = TimeSpan.Zero
                        }
                    },
                    {
                        100, new LogLineFrameData
                        {
                            ID = 100,
                            RawFramesComplete = 1_000_000,
                            RawFramesTotal = 1_000_000,
                            TimeStamp = TimeSpan.FromSeconds(38),
                            Duration = TimeSpan.FromSeconds(28)
                        }
                    }
                }
            };
            var protein = new Protein();

            var workUnitModel = CreateWorkUnitModel(settings, workUnit, protein, processor: "Ryzen", threads: 16);
            long workUnitID = _repository.Update(workUnitModel);

            _deleteResult = await _repository.DeleteAsync(new WorkUnitRow { ID = workUnitID });
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenWorkUnitAndWorkUnitFramesAreDeleted()
        {
            Assert.AreEqual(1, _deleteResult);

            using var context = new WorkUnitContext(_connection);
            Assert.AreEqual(1, context.Clients.Count());
            Assert.AreEqual(1, context.Proteins.Count());
            Assert.AreEqual(1, context.Platforms.Count());
            Assert.AreEqual(0, context.WorkUnits.Count());
            Assert.AreEqual(0, context.WorkUnitFrames.Count());
        }
    }

    [TestFixture]
    public class GivenWorkUnitsInTheDatabase : WorkUnitContextRepositoryTests
    {
        private string _connectionString;
        private IWorkUnitRepository _repository;

        [SetUp]
        public virtual void BeforeEach()
        {
            string path = Path.GetFullPath(@"TestFiles\WorkUnits.db");
            _connectionString = $"Data Source={path}";
            _repository = new TestableWorkUnitContextRepository(_connectionString);
        }

        [TestFixture]
        public class WhenFetchingAllWorkUnits : GivenWorkUnitsInTheDatabase
        {
            private IList<WorkUnitRow> _result;

            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();
                _result = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);
            }

            [Test]
            public void ThenAllWorkUnitsAreReturned()
            {
                Assert.AreEqual(99070, _result.Count);
            }
        }

        [TestFixture]
        public class WhenFetchingSomeWorkUnits : GivenWorkUnitsInTheDatabase
        {
            private IList<WorkUnitRow> _result;

            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();
                var query = new WorkUnitQuery()
                    .AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.Equal, 6600)
                    .AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.Equal, 0);
                _result = _repository.Fetch(query, BonusCalculation.None);
            }

            [Test]
            public void ThenSomeWorkUnitsAreReturned()
            {
                Assert.AreEqual(195, _result.Count);
            }
        }

        [TestFixture]
        public class WhenFetchingFirstPageOfWorkUnits : GivenWorkUnitsInTheDatabase
        {
            private Page<WorkUnitRow> _result;

            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();
                _result = _repository.Page(1, 1000, WorkUnitQuery.SelectAll, BonusCalculation.None);
            }

            [Test]
            public void ThenThePageOfWorkUnitsIsReturned()
            {
                Assert.AreEqual(1, _result.CurrentPage);
                Assert.AreEqual(100, _result.TotalPages);
                Assert.AreEqual(99070, _result.TotalItems);
                Assert.AreEqual(1000, _result.ItemsPerPage);
                Assert.AreEqual(1000, _result.Items.Count);
            }
        }

        [TestFixture]
        public class WhenFetchingLastPageOfWorkUnits : GivenWorkUnitsInTheDatabase
        {
            private Page<WorkUnitRow> _result;

            [SetUp]
            public override void BeforeEach()
            {
                base.BeforeEach();
                _result = _repository.Page(100, 1000, WorkUnitQuery.SelectAll, BonusCalculation.None);
            }

            [Test]
            public void ThenThePageOfWorkUnitsIsReturned()
            {
                Assert.AreEqual(100, _result.CurrentPage);
                Assert.AreEqual(100, _result.TotalPages);
                Assert.AreEqual(99070, _result.TotalItems);
                Assert.AreEqual(1000, _result.ItemsPerPage);
                Assert.AreEqual(70, _result.Items.Count);
            }
        }
    }

    private static WorkUnitModel CreateWorkUnitModel(ClientSettings settings, WorkUnit workUnit, Protein protein,
        int slotID = SlotIdentifier.NoSlotID, string processor = null, int? threads = null)
    {
        var client = new NullClient
        {
            Settings = settings,
            Platform = new ClientPlatform("7", "Windows")
        };
        var slotModel = new SlotModel(client)
        {
            SlotID = slotID,
            Description = new CPUSlotDescription
            {
                Processor = processor,
                CPUThreads = threads
            }
        };
        var workUnitModel = new WorkUnitModel(slotModel, workUnit)
        {
            CurrentProtein = protein
        };
        return workUnitModel;
    }
}
