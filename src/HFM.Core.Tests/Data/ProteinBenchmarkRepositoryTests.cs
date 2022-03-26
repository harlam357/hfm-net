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
                Assert.AreEqual(14, actual.Count);
            }

            [Test]
            public void ThenBenchmarkProjectsAreReturnedSpecificSlot()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);
                var actual = _repository.GetBenchmarkProjects(s);
                Assert.AreEqual(6, actual.Count);
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
                Assert.AreEqual(18213, actual.ProjectID);
                Assert.AreEqual("GeForce RTX 3070 Ti", actual.Processor);
                Assert.AreEqual(ProteinBenchmarkIdentifier.NoThreads, actual.Threads);
                Assert.AreEqual(b, actual.BenchmarkIdentifier);
                Assert.AreEqual(118, actual.FrameTimes.Count);
                Assert.AreEqual(TimeSpan.FromSeconds(70), actual.AverageFrameTime);
                Assert.AreEqual(TimeSpan.FromSeconds(69), actual.MinimumFrameTime);
            }
        }
    }
}
