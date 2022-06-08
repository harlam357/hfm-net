using HFM.Core.Client.Mocks;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

using NUnit.Framework;

namespace HFM.Core.Client;

[TestFixture]
public class SlotModelTests
{
    [TestFixture]
    public class GivenFahClientSlotModelThatDisplaysVersions
    {
        private FahClientSlotModel _slotModel;

        [SetUp]
        public void BeforeEach()
        {
            var client = new NullClient();
            var preferences = new InMemoryPreferencesProvider();
            preferences.Set(Preference.DisplayVersions, true);

            _slotModel = new FahClientSlotModel(preferences, client, default, SlotIdentifier.NoSlotID);
            _slotModel.Description = new GPUSlotDescription { Processor = "GeForce RTX 3070 Ti" };
            _slotModel.WorkUnitModel.WorkUnit.Platform = new WorkUnitPlatform(WorkUnitPlatformImplementation.CUDA)
            {
                DriverVersion = "511.79"
            };
        }

        [Test]
        public void ThenSlotModelProcessorIncludesVersionInformation() =>
            Assert.AreEqual("GeForce RTX 3070 Ti (CUDA 511.79)", _slotModel.Processor);

        [Test]
        public void ThenProteinBenchmarkDetailSourceProcessorDoesNotIncludeVersionInformation()
        {
            var source = (IProteinBenchmarkDetailSource)_slotModel;
            Assert.AreEqual("GeForce RTX 3070 Ti", source.Processor);
        }
    }

    [TestFixture]
    public class SlotModelCurrentLogLines
    {
        [Test]
        public void IsThreadSafe()
        {
            var slot = new SlotModel(new MockClient());
            var random = new Random();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var token = cts.Token;

            Task.Run(() =>
            {
                while (true)
                {
                    slot.CurrentLogLines = Enumerable.Repeat(new LogLine(), random.Next(1, 5)).ToList();
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, token);

            const int count = 100;

            var tasks = Enumerable.Range(0, count)
                .Select(i => Task.Run(() =>
                {
                    Thread.Sleep(10);
                    _ = slot.CurrentLogLines.ToList();
                }))
                .ToArray();

            try
            {
                Task.WaitAll(tasks);
                cts.Cancel();
            }
            catch (Exception)
            {
                Assert.Fail("Enumeration failed");
            }
        }
    }
}
