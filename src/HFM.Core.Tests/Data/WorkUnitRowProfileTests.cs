
using AutoMapper;
using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRowProfileTests
    {
        [Test]
        public void WorkUnitRowProfile_ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<WorkUnitRowProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
