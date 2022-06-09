using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

using NUnit.Framework;

namespace HFM.Core.Client;

[TestFixture]
public class FahClientDataTests
{
    [TestFixture]
    public class GivenFahClientDataThatDisplaysVersions
    {
        private FahClientData _clientData;

        [SetUp]
        public void BeforeEach()
        {
            var client = new NullClient();
            var preferences = new InMemoryPreferencesProvider();
            preferences.Set(Preference.DisplayVersions, true);

            _clientData = new FahClientData(preferences, client, default, SlotIdentifier.NoSlotID);
            _clientData.Description = new GPUSlotDescription { Processor = "GeForce RTX 3070 Ti" };
            _clientData.WorkUnitModel.WorkUnit.Platform = new WorkUnitPlatform(WorkUnitPlatformImplementation.CUDA)
            {
                DriverVersion = "511.79"
            };
        }

        [Test]
        public void ThenSlotModelProcessorIncludesVersionInformation() =>
            Assert.AreEqual("GeForce RTX 3070 Ti (CUDA 511.79)", _clientData.Processor);

        [Test]
        public void ThenProteinBenchmarkDetailSourceProcessorDoesNotIncludeVersionInformation()
        {
            var source = (IProteinBenchmarkDetailSource)_clientData;
            Assert.AreEqual("GeForce RTX 3070 Ti", source.Processor);
        }
    }

    [TestFixture]
    public class FahClientDataCurrentLogLines
    {
        [Test]
        public void IsThreadSafe()
        {
            var clientData = new ClientData();
            var random = new Random();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var token = cts.Token;

            Task.Run(() =>
            {
                while (true)
                {
                    clientData.CurrentLogLines = Enumerable.Repeat(new LogLine(), random.Next(1, 5)).ToList();
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
                    _ = clientData.CurrentLogLines.ToList();
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

    [TestFixture]
    public class FahClientDataStatus
    {
        [Test]
        public void IsGivenStatusWhenWorkUnitDoesNotHaveProject()
        {
            // Arrange
            var client = new NullClient();
            var clientData = CreateFahClientData(client, SlotStatus.Running, 0);
            // Act
            var status = clientData.Status;
            // Assert
            Assert.AreEqual(SlotStatus.Running, status);
        }

        [Test]
        public void IsRunningNoFrameTimesWhenGivenStatusIsRunningAndWorkUnitHasProjectAndNoProgress()
        {
            // Arrange
            var client = new NullClient();
            var clientData = CreateFahClientData(client, SlotStatus.Running, 0);
            clientData.WorkUnitModel.WorkUnit.ProjectID = 1;
            // Act
            var status = clientData.Status;
            // Assert
            Assert.AreEqual(SlotStatus.RunningNoFrameTimes, status);
        }
    }

    private static FahClientData CreateFahClientData(IClient client, SlotStatus status, int slotID) =>
        new(new InMemoryPreferencesProvider(), client, status, slotID);
}
