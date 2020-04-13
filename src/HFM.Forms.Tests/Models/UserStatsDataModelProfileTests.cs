
using AutoMapper;

using NUnit.Framework;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class UserStatsDataModelProfileTests
    {
        [Test]
        public void UserStatsDataModelProfile_ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserStatsDataModelProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
