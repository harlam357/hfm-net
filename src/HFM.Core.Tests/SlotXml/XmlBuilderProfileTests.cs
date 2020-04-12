
using AutoMapper;
using NUnit.Framework;

namespace HFM.Core.SlotXml
{
    [TestFixture]
    public class XmlBuilderProfileTests
    {
        [Test]
        public void XmlBuilderProfile_ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<XmlBuilderProfile>());
            config.AssertConfigurationIsValid();
        }
    }
}
