
using System;

using NUnit.Framework;

namespace HFM.Core.Net
{
    [TestFixture]
    public class HostNameTests
    {
        [Test]
        public void HostName_Validate_Tests()
        {
            // This Validation is pretty wide open.  Should probably not validate
            // (..) two dots in a row and server names should probably be forced
            // to begin and end an alpha-numeric character.
            Assert.IsTrue(HostName.Validate(@"ftp.someserver.com"));
            Assert.IsTrue(HostName.Validate(@"ftp..some.server..com"));
            Assert.IsTrue(HostName.Validate(@"MediaServer2"));
            Assert.IsTrue(HostName.Validate("-a-"));
            Assert.IsTrue(HostName.Validate("_a_"));
            Assert.IsTrue(HostName.Validate(".a."));
            Assert.IsTrue(HostName.Validate("%a%"));

            Assert.IsFalse(HostName.Validate("+a+"));
            Assert.IsFalse(HostName.Validate("=a="));
            Assert.IsFalse(HostName.Validate("$a$"));
            Assert.IsFalse(HostName.Validate("&a&"));
            Assert.IsFalse(HostName.Validate("^a^"));
            Assert.IsFalse(HostName.Validate("[a["));
            Assert.IsFalse(HostName.Validate("]a]"));

            Assert.IsFalse(HostName.Validate(String.Empty));
            Assert.IsFalse(HostName.Validate("  "));
            Assert.IsFalse(HostName.Validate(null));
        }
    }
}
