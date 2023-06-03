using HFM.Core.Client;
using HFM.Core.WorkUnits;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Memory;

using NUnit.Framework;

namespace HFM.Core.Data;

[TestFixture]
public class ProteinBenchmarkRepositoryCachingDecoratorTests
{
    private MemoryCache _memoryCache;
    private IProteinBenchmarkRepository _repository;

    [TestFixture]
    public class GivenEmptyDatabase : ProteinBenchmarkRepositoryCachingDecoratorTests, IDisposable
    {
        private SqliteConnection _connection;

        [SetUp]
        public virtual void BeforeEach()
        {
            _connection = new("Data Source=:memory:");
            _connection.Open();

            _memoryCache = new(new MemoryCacheOptions());
            _repository = new ProteinBenchmarkRepositoryCachingDecorator(new TestableProteinBenchmarkRepository(_connection), _memoryCache);
        }

        [TestFixture]
        public class WhenFetchingSlotIdentifiers : GivenEmptyDatabase
        {
            [Test]
            public async Task ThenTheResultsAreNotCached()
            {
                var first = await _repository.GetSlotIdentifiersAsync();
                var second = await _repository.GetSlotIdentifiersAsync();
                Assert.AreNotSame(first, second);
                Assert.AreEqual(0, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarkProjects : GivenEmptyDatabase
        {
            [Test]
            public async Task ThenTheResultsAreNotCached()
            {
                var first = await _repository.GetBenchmarkProjectsAsync(SlotIdentifier.AllSlots);
                var second = await _repository.GetBenchmarkProjectsAsync(SlotIdentifier.AllSlots);
                Assert.AreNotSame(first, second);
                Assert.AreEqual(0, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmark : GivenEmptyDatabase
        {
            [Test]
            public void ThenTheResultsAreNotCached()
            {
                var s = new SlotIdentifier(ClientIdentifier.FromGuid(Guid.NewGuid()), 1);
                var b = new ProteinBenchmarkIdentifier(1, "", ProteinBenchmarkIdentifier.NoThreads);

                var actual = _repository.GetBenchmark(s, b);
                Assert.IsNull(actual);
                Assert.AreEqual(0, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarksForOneProject : GivenEmptyDatabase
        {
            [Test]
            public async Task ThenTheResultsAreNotCached()
            {
                var first = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, 1, 1);
                var second = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, 1, 1);
                Assert.AreNotSame(first, second);
                Assert.AreEqual(0, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarksForMultipleProjects : GivenEmptyDatabase
        {
            [Test]
            public async Task ThenTheResultsAreNotCached()
            {
                var first = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, new[] { 1, 2 });
                var second = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, new[] { 1, 2 });
                Assert.AreNotSame(first, second);
                Assert.AreEqual(0, _memoryCache.Count);
            }
        }

        public void Dispose() => _connection?.Dispose();
    }

    [TestFixture]
    public class GivenPopulatedDatabase : ProteinBenchmarkRepositoryCachingDecoratorTests
    {
        private string _connectionString;

        [SetUp]
        public virtual void BeforeEach()
        {
            string path = Path.GetFullPath(@"TestFiles\WorkUnitBenchmarks.db");
            _connectionString = $"Data Source={path}";

            _memoryCache = new(new MemoryCacheOptions());
            _repository = new ProteinBenchmarkRepositoryCachingDecorator(new TestableProteinBenchmarkRepository(_connectionString), _memoryCache);
        }

        [TestFixture]
        public class WhenFetchingSlotIdentifiers : GivenPopulatedDatabase
        {
            [Test]
            public async Task ThenTheResultsAreNotCached()
            {
                var first = await _repository.GetSlotIdentifiersAsync();
                var second = await _repository.GetSlotIdentifiersAsync();
                Assert.AreNotSame(first, second);
                Assert.AreEqual(0, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarkProjects : GivenPopulatedDatabase
        {
            [Test]
            public async Task ThenTheResultsAreNotCached()
            {
                var first = await _repository.GetBenchmarkProjectsAsync(SlotIdentifier.AllSlots);
                var second = await _repository.GetBenchmarkProjectsAsync(SlotIdentifier.AllSlots);
                Assert.AreNotSame(first, second);
                Assert.AreEqual(0, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmark : GivenPopulatedDatabase
        {
            [Test]
            public void ThenTheResultsAreCached()
            {
                var c = ClientIdentifier.FromGuid(Guid.Parse("4e39610d-f40b-409a-baea-9e78a8c78e7c"));
                var s = new SlotIdentifier(c, 2);
                var b = new ProteinBenchmarkIdentifier(18213, "GeForce RTX 3070 Ti", ProteinBenchmarkIdentifier.NoThreads);

                var first = _repository.GetBenchmark(s, b);
                var second = _repository.GetBenchmark(s, b);
                Assert.AreSame(first, second);
                Assert.AreEqual(1, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarksForOneProject : GivenPopulatedDatabase
        {
            [Test]
            public async Task ThenTheResultsAreCached()
            {
                var first = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, 18213, 2);
                var second = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, 18213, 2);
                Assert.AreSame(first, second);
                Assert.AreEqual(1, _memoryCache.Count);
            }
        }

        [TestFixture]
        public class WhenGettingBenchmarksForMultipleProjects : GivenPopulatedDatabase
        {
            [Test]
            public async Task ThenTheResultsAreCached()
            {
                var first = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, new[] { 18213 });
                var second = await _repository.GetBenchmarksAsync(SlotIdentifier.AllSlots, new[] { 18213 });
                Assert.AreSame(first, second);
                Assert.AreEqual(1, _memoryCache.Count);
            }
        }
    }
}
