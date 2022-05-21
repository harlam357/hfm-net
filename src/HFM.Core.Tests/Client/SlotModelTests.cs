using HFM.Core.WorkUnits;
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
            client.Preferences.Set(Preference.DisplayVersions, true);

            _slotModel = new FahClientSlotModel(client, default, SlotIdentifier.NoSlotID);
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
}
