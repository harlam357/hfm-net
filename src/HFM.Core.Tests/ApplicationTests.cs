using System;

using NUnit.Framework;

namespace HFM.Core
{
    [TestFixture]
    public class ApplicationTests
    {
        [Test]
        public void Application_ParseVersionNumber_FromFourSegments()
        {
            Assert.AreEqual(1020030004, Application.ParseVersionNumber("1.2.3.4"));
        }

        [Test]
        public void Application_ParseVersionNumber_ThrowsWhenVersionStringIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Application.ParseVersionNumber(null));
        }

        [Test]
        public void Application_ParseVersionNumber_FromThreeSegments()
        {
            Assert.AreEqual(1020030000, Application.ParseVersionNumber("1.2.3"));
        }

        [Test]
        public void Application_ParseVersionNumber_ThrowsWhenSegmentIsNotInteger()
        {
            Assert.Throws<FormatException>(() => Application.ParseVersionNumber("1.2.3.b"));
        }
    }
}
