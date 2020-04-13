
using System;

using NUnit.Framework;

namespace HFM.Core.Net
{
    [TestFixture]
    public class NetworkCredentialFactoryTests
    {
        [Test]
        public void NetworkCredentialFactory_ValidateRequired_Tests()
        {
            Assert.IsTrue(NetworkCredentialFactory.ValidateRequired("Username", "Password", out _));
            Assert.IsFalse(NetworkCredentialFactory.ValidateRequired("Username", String.Empty, out _));
            Assert.IsFalse(NetworkCredentialFactory.ValidateRequired(String.Empty, "Password", out _));
            Assert.IsFalse(NetworkCredentialFactory.ValidateRequired(String.Empty, String.Empty, out _));
        }

        [Test]
        public void NetworkCredentialFactory_ValidateOrEmpty_Tests()
        {
            Assert.IsTrue(NetworkCredentialFactory.ValidateOrEmpty("Username", "Password", out _));
            Assert.IsFalse(NetworkCredentialFactory.ValidateOrEmpty("Username", String.Empty, out _));
            Assert.IsFalse(NetworkCredentialFactory.ValidateOrEmpty(String.Empty, "Password", out _));
            Assert.IsTrue(NetworkCredentialFactory.ValidateOrEmpty(String.Empty, String.Empty, out _));
        }
    }
}
