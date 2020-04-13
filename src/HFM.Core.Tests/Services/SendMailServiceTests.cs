
using System;

using NUnit.Framework;

namespace HFM.Core.Services
{
    [TestFixture]
    public class SendMailServiceTests
    {
        [Test]
        public void SendMailService_ValidateEmail_Tests()
        {
            Assert.IsTrue(SendMailService.ValidateEmail("someone@home.co"));
            Assert.IsTrue(SendMailService.ValidateEmail("someone@home.com"));
            Assert.IsTrue(SendMailService.ValidateEmail("someone@home.comm"));
            Assert.IsTrue(SendMailService.ValidateEmail("a@home.com"));

            Assert.IsFalse(SendMailService.ValidateEmail("@home.com"));
            Assert.IsFalse(SendMailService.ValidateEmail("someone@home"));
            Assert.IsFalse(SendMailService.ValidateEmail("someone@home.c"));
            Assert.IsFalse(SendMailService.ValidateEmail("someelse@not.at.home..com"));

            Assert.IsFalse(SendMailService.ValidateEmail(String.Empty));
            Assert.IsFalse(SendMailService.ValidateEmail("  "));
            Assert.IsFalse(SendMailService.ValidateEmail(null));
        }
    }
}
