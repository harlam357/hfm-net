using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using NUnit.Framework;

namespace HFM.Core.Data;

[TestFixture]
public class ProteinBenchmarkRepositoryTests
{
    [TestFixture]
    public class GivenPopulatedDatabase : ProteinBenchmarkRepositoryTests
    {
        private string _connectionString;
        private ProteinBenchmarkRepository _repository;

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
            public void ThenSlotIdentifiersAreReturned()
            {
                var actual = _repository.GetSlotIdentifiers();
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
            public void ThenBenchmarkProjectsAreReturnedForAllSlots()
            {
                var actual = _repository.GetBenchmarkProjects(SlotIdentifier.AllSlots);
                Assert.AreEqual(6, actual.Count);
            }

            [Test]
            public void ThenBenchmarkProjectsAreReturnedSpecificSlot()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);
                var actual = _repository.GetBenchmarkProjects(s);
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
            public void ThenBenchmarksAreReturnedForAllSlots()
            {
                var actual = _repository.GetBenchmarks(SlotIdentifier.AllSlots, 18213);
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
            public void ThenBenchmarksAreReturnedSpecificSlot()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);

                var actual = _repository.GetBenchmarks(s, 18213);
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
            public void ThenBenchmarksAreReturnedForAllSlotsAndMultipleProjects()
            {
                var actual = _repository.GetBenchmarks(SlotIdentifier.AllSlots, new[] { 17910, 18213, 17908 });
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
}
