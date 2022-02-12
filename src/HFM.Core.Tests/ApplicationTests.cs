using NUnit.Framework;

namespace HFM.Core
{
    [TestFixture]
    public class ApplicationTests
    {
        [Test]
        public void Application_Version_IsNotNull()
        {
            Assert.IsNotNull(Application.Version);
        }

        [Test]
        public void Application_VersionNumber_HasThreeParts()
        {
            var version = Application.VersionNumber;
            Assert.AreNotEqual(0, version.Major);
            Assert.AreNotEqual(-1, version.Minor);
            Assert.AreNotEqual(-1, version.Build);
            Assert.AreEqual(-1, version.Revision);
        }
    }
}
