using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Proteins;

using Microsoft.Data.Sqlite;

using NUnit.Framework;

namespace HFM.Core.Data;

[TestFixture]
public class ProteinBenchmarkRepositoryTests
{
    [TestFixture]
    public class GivenPopulatedDatabase : ProteinBenchmarkRepositoryTests
    {
        private string _connectionString;
        private IProteinBenchmarkRepository _repository;

        [SetUp]
        public virtual void BeforeEach()
        {
            string path = Path.GetFullPath(@"TestFiles\WorkUnitBenchmarks.db");
            _connectionString = $"Data Source={path}";
            _repository = new TestableProteinBenchmarkRepository(_connectionString);
        }

        [TestFixture]
        public class WhenFetchingSlotIdentifiers : GivenPopulatedDatabase
        {
            [Test]
            public async Task ThenSlotIdentifiersAreReturned()
            {
                var actual = await _repository.GetSlotIdentifiersAsync();
                Assert.AreEqual(3, actual.Count);
                foreach (var s in actual)
                {
                    TestLogger.Instance.Debug(s.ToString());
                    TestLogger.Instance.Debug(s.ClientIdentifier.Guid.ToString());
                }
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarkProjects : GivenPopulatedDatabase
        {
            [Test]
            public async Task ThenBenchmarkProjectsAreReturnedForAllSlots()
            {
                var actual = await _repository.GetBenchmarkProjectsAsync(SlotIdentifier.AllSlots);
                Assert.AreEqual(6, actual.Count);
            }

            [Test]
            public async Task ThenBenchmarkProjectsAreReturnedSpecificSlot()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);
                var actual = await _repository.GetBenchmarkProjectsAsync(s);
                Assert.AreEqual(2, actual.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmark : GivenPopulatedDatabase
        {
            [Test]
            public void ThenBenchmarkIsReturnedSpecificSlot()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);
                var b = new ProteinBenchmarkIdentifier(18213, "GeForce RTX 3070 Ti", ProteinBenchmarkIdentifier.NoThreads);

                var actual = _repository.GetBenchmark(s, b);
                Assert.AreEqual(s, actual.SlotIdentifier);
                Assert.AreEqual(b, actual.BenchmarkIdentifier);
                Assert.AreEqual(100, actual.FrameTimes.Count);
                Assert.AreEqual(TimeSpan.FromSeconds(72), actual.AverageFrameTime);
                Assert.AreEqual(TimeSpan.FromSeconds(70), actual.MinimumFrameTime);
            }

            [Test]
            public void ThenBenchmarkIsNullWhenNoBenchmarkExists()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);
                var b = new ProteinBenchmarkIdentifier(99999, "GeForce RTX 3070 Ti", ProteinBenchmarkIdentifier.NoThreads);

                var actual = _repository.GetBenchmark(s, b);
                Assert.IsNull(actual);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarks : GivenPopulatedDatabase
        {
            [Test]
            public async Task ThenBenchmarksAreReturnedForAllSlots()
            {
                var actual = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, 18213);
                Assert.AreEqual(2, actual.Count);

                var x = actual.ElementAt(0);
                Assert.AreEqual("Ryzen 5800X", x.SlotIdentifier.ClientIdentifier.Name);
                Assert.AreEqual("192.168.1.194", x.SlotIdentifier.ClientIdentifier.Server);
                Assert.AreEqual(36330, x.SlotIdentifier.ClientIdentifier.Port);
                Assert.AreEqual(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"), x.SlotIdentifier.ClientIdentifier.Guid);
                Assert.AreEqual(3, x.SlotIdentifier.SlotID);
                Assert.AreEqual(18213, x.BenchmarkIdentifier.ProjectID);
                Assert.AreEqual("GeForce RTX 3060 Ti Lite Hash Rate", x.BenchmarkIdentifier.Processor);
                Assert.AreEqual(ProteinBenchmarkIdentifier.NoThreads, x.BenchmarkIdentifier.Threads);
                Assert.AreEqual(100, x.FrameTimes.Count);
                Assert.AreEqual(TimeSpan.FromSeconds(87), x.AverageFrameTime);
                Assert.AreEqual(TimeSpan.FromSeconds(87), x.MinimumFrameTime);

                x = actual.ElementAt(1);
                Assert.AreEqual("Ryzen 5800X", x.SlotIdentifier.ClientIdentifier.Name);
                Assert.AreEqual("192.168.1.194", x.SlotIdentifier.ClientIdentifier.Server);
                Assert.AreEqual(36330, x.SlotIdentifier.ClientIdentifier.Port);
                Assert.AreEqual(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"), x.SlotIdentifier.ClientIdentifier.Guid);
                Assert.AreEqual(2, x.SlotIdentifier.SlotID);
                Assert.AreEqual(18213, x.BenchmarkIdentifier.ProjectID);
                Assert.AreEqual("GeForce RTX 3070 Ti", x.BenchmarkIdentifier.Processor);
                Assert.AreEqual(ProteinBenchmarkIdentifier.NoThreads, x.BenchmarkIdentifier.Threads);
                Assert.AreEqual(100, x.FrameTimes.Count);
                Assert.AreEqual(TimeSpan.FromSeconds(72), x.AverageFrameTime);
                Assert.AreEqual(TimeSpan.FromSeconds(70), x.MinimumFrameTime);
            }

            [Test]
            public async Task ThenMaximumCountOfBenchmarksAreReturnedForAllSlots()
            {
                var actual = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, 18213, 1);
                Assert.AreEqual(1, actual.Count);
            }

            [Test]
            public async Task ThenBenchmarksAreReturnedSpecificSlot()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);

                var actual = await _repository.GetBenchmarksAsync(s, 18213);
                Assert.AreEqual(1, actual.Count);

                var b = actual.ElementAt(0);
                Assert.AreEqual("Ryzen 5800X", b.SlotIdentifier.ClientIdentifier.Name);
                Assert.AreEqual("192.168.1.194", b.SlotIdentifier.ClientIdentifier.Server);
                Assert.AreEqual(36330, b.SlotIdentifier.ClientIdentifier.Port);
                Assert.AreEqual(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"), b.SlotIdentifier.ClientIdentifier.Guid);
                Assert.AreEqual(2, b.SlotIdentifier.SlotID);
                Assert.AreEqual(18213, b.BenchmarkIdentifier.ProjectID);
                Assert.AreEqual("GeForce RTX 3070 Ti", b.BenchmarkIdentifier.Processor);
                Assert.AreEqual(ProteinBenchmarkIdentifier.NoThreads, b.BenchmarkIdentifier.Threads);
                Assert.AreEqual(100, b.FrameTimes.Count);
                Assert.AreEqual(TimeSpan.FromSeconds(72), b.AverageFrameTime);
                Assert.AreEqual(TimeSpan.FromSeconds(70), b.MinimumFrameTime);
            }

            [Test]
            public async Task ThenBenchmarksAreReturnedForAllSlotsAndMultipleProjects()
            {
                var actual = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, new[] { 17910, 18213, 17908 });
                Assert.AreEqual(4, actual.Count);
                Assert.AreEqual(17908, actual.ElementAt(0).BenchmarkIdentifier.ProjectID);
                Assert.AreEqual(17910, actual.ElementAt(1).BenchmarkIdentifier.ProjectID);
                Assert.AreEqual(18213, actual.ElementAt(2).BenchmarkIdentifier.ProjectID);
                Assert.AreEqual(18213, actual.ElementAt(3).BenchmarkIdentifier.ProjectID);
                Assert.AreEqual(61, actual.ElementAt(0).FrameTimes.Count);
                Assert.AreEqual(100, actual.ElementAt(1).FrameTimes.Count);
                Assert.AreEqual(100, actual.ElementAt(2).FrameTimes.Count);
                Assert.AreEqual(100, actual.ElementAt(3).FrameTimes.Count);
            }
        }
    }

    [TestFixture]
    public class GivenCompletedWorkUnitWithNoPlatform : ProteinBenchmarkRepositoryTests
    {
        private SqliteConnection _connection;
        private IProteinBenchmarkRepository _repository;

        private readonly string _clientName = "GTX3090";
        private readonly Guid _clientGuid = Guid.NewGuid();
        private readonly int _projectID = 1;
        private readonly DateTime _utcNow = DateTime.UtcNow;

        [SetUp]
        public async Task BeforeEach()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new TestableProteinBenchmarkRepository(_connection);

            var settings = new ClientSettings
            {
                Name = _clientName,
                Guid = _clientGuid
            };
            var client = new NullClient
            {
                Settings = settings,
                Platform = new ClientPlatform("7", "Windows")
            };
            var slotModel = new SlotModel(client)
            {
                Description = new UnknownSlotDescription()
            };
            var workUnit = new WorkUnit
            {
                ProjectID = _projectID,
                Assigned = _utcNow,
                Finished = _utcNow.AddHours(6),
                UnitResult = WorkUnitResult.FinishedUnit,
                Frames = new Dictionary<int, LogLineFrameData>
                {
                    { 0, new LogLineFrameData { ID = 0, Duration = TimeSpan.FromMinutes(3) } }
                }
            };
            var workUnitModel = new WorkUnitModel(slotModel, workUnit, null)
            {
                CurrentProtein = new Protein
                {
                    ProjectNumber = 1
                }
            };

            var workUnitRepository = new TestableWorkUnitContextRepository(_connection);
            await workUnitRepository.UpdateAsync(workUnitModel);
        }

        private class UnknownSlotDescription : SlotDescription
        {
            public override SlotType SlotType => SlotType.Unknown;
        }

        [TearDown]
        public void AfterEach() => _connection?.Dispose();

        [Test]
        public void ThenBenchmarksWithoutPlatformDoNotThrowOnRetrieval()
        {
            var slotIdentifier = SlotIdentifier.FromName("GTX3090", null, _clientGuid);
            Assert.DoesNotThrowAsync(() => _repository.GetBenchmarksAsync(slotIdentifier, _projectID));
        }
    }
}
