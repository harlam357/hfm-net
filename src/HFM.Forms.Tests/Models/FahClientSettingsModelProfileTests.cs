
using AutoMapper;

using NUnit.Framework;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class FahClientSettingsModelProfileTests
    {
        [Test]
        public void FahClientSettingsModelProfile_ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<FahClientSettingsModelProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
