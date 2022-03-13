using HFM.Core.Client;

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
            string path = Path.GetFullPath(@"TestFiles\WorkUnits.db");
            _connectionString = $"Data Source={path}";
            _repository = new TestableProteinBenchmarkRepository(_connectionString);
        }

        [TestFixture]
        public class WhenFetchingAllSlotIdentifiers : GivenPopulatedDatabase
        {
            [Test]
            public void ThenAllSlotIdentifiersAreReturned()
            {
                var actual = _repository.GetAllSlotIdentifiers();
                Assert.AreEqual(148, actual.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarkProjects : GivenPopulatedDatabase
        {
            [Test]
            public void ThenAllBenchmarkProjectsAreReturnedForAllSlots()
            {
                var actual = _repository.GetAllBenchmarkProjects(SlotIdentifier.AllSlots);
                Assert.AreEqual(1440, actual.Count);
            }
        }
    }
}
