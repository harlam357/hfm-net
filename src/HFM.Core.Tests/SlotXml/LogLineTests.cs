using NUnit.Framework;

namespace HFM.Core.SlotXml
{
    [TestFixture]
    public class LogLineTests
    {
        [Test]
        public void LogLine_RemoveInvalidXmlChars_RemovesInvalidCharacters()
        {
            const string value = "[91m13:21:22:ERROR:WU01:FS01:Exception: Server did not assign work unit[0m";
            const string expected = "[91m13:21:22:ERROR:WU01:FS01:Exception: Server did not assign work unit[0m";
            string actual = LogLine.RemoveInvalidXmlChars(value);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LogLine_RemoveInvalidXmlChars_ReturnsOriginalStringWhenNoInvalidCharacters()
        {
            const string expected = "13:21:21:WU01:FS01:Requesting new work unit for slot 01: RUNNING gpu:0:TU104GL [Tesla T4] 8141 from 207.53.233.146";
            string actual = LogLine.RemoveInvalidXmlChars(expected);
            Assert.AreSame(expected, actual);
        }
    }
}
