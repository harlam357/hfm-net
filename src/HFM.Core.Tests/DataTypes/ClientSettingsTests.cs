
using System;

using NUnit.Framework;

namespace HFM.Core.DataTypes
{
    [TestFixture]
    public class ClientSettingsTests
    {
        [Test]
        public void ClientSettings_ValidateName_Tests()
        {
            Assert.IsTrue(ClientSettings.ValidateName("+a+"));
            Assert.IsTrue(ClientSettings.ValidateName("=a="));
            Assert.IsTrue(ClientSettings.ValidateName("-a-"));
            Assert.IsTrue(ClientSettings.ValidateName("_a_"));
            Assert.IsTrue(ClientSettings.ValidateName("$a$"));
            Assert.IsTrue(ClientSettings.ValidateName("&a&"));
            Assert.IsTrue(ClientSettings.ValidateName("^a^"));
            Assert.IsTrue(ClientSettings.ValidateName("[a["));
            Assert.IsTrue(ClientSettings.ValidateName("]a]"));

            Assert.IsFalse(ClientSettings.ValidateName("}a}"));
            Assert.IsFalse(ClientSettings.ValidateName("\\a\\"));
            Assert.IsFalse(ClientSettings.ValidateName("|a|"));
            Assert.IsFalse(ClientSettings.ValidateName(";a;"));
            Assert.IsFalse(ClientSettings.ValidateName(":a:"));
            Assert.IsFalse(ClientSettings.ValidateName("\'a\'"));
            Assert.IsFalse(ClientSettings.ValidateName("\"a\""));
            Assert.IsFalse(ClientSettings.ValidateName(",a,"));
            Assert.IsFalse(ClientSettings.ValidateName("<a<"));
            Assert.IsFalse(ClientSettings.ValidateName(">a>"));
            Assert.IsFalse(ClientSettings.ValidateName("/a/"));
            Assert.IsFalse(ClientSettings.ValidateName("?a?"));
            Assert.IsFalse(ClientSettings.ValidateName("`a`"));
            Assert.IsFalse(ClientSettings.ValidateName("~a~"));
            Assert.IsFalse(ClientSettings.ValidateName("!a!"));
            Assert.IsFalse(ClientSettings.ValidateName("@a@"));
            Assert.IsFalse(ClientSettings.ValidateName("#a#"));
            Assert.IsFalse(ClientSettings.ValidateName("%a%"));
            Assert.IsFalse(ClientSettings.ValidateName("*a*"));
            Assert.IsFalse(ClientSettings.ValidateName("(a("));
            Assert.IsFalse(ClientSettings.ValidateName(")a)"));

            Assert.IsFalse(ClientSettings.ValidateName(String.Empty));
            Assert.IsFalse(ClientSettings.ValidateName(null));
        }

        [Test]
        public void ClientSettings_CleanName_Tests()
        {
            string name = ClientSettings.CleanName("+a}");
            Assert.AreEqual("+a", name);
            name = ClientSettings.CleanName("}a+");
            Assert.AreEqual("a+", name);
            name = ClientSettings.CleanName("=a\\");
            Assert.AreEqual("=a", name);
            name = ClientSettings.CleanName("\\a=");
            Assert.AreEqual("a=", name);
            name = ClientSettings.CleanName(String.Empty);
            Assert.AreEqual(String.Empty, name);
            name = ClientSettings.CleanName(null);
            Assert.AreEqual(null, name);
        }
    }
}
