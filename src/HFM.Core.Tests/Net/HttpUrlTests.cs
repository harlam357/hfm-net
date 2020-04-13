
using NUnit.Framework;

namespace HFM.Core.Net
{
    [TestFixture]
    public class HttpUrlTests
    {
        [Test]
        public void HttpUrl_Validate_Tests()
        {
            Assert.IsTrue(HttpUrl.Validate(@"http://www.domain.com/somesite/index.html"));
            Assert.IsTrue(HttpUrl.Validate(@"https://some-server/serverfolder/dsfasfsdf"));
            Assert.IsTrue(HttpUrl.Validate(@"https://some-server/serverfolder/dsfasfsdf/"));
            Assert.IsTrue(HttpUrl.Validate(@"http://fah-web.stanford.edu/psummary.html"));

            Assert.IsFalse(HttpUrl.Validate(@"ftp://ftp.ftp.com/ftpfolder/"));
            Assert.IsFalse(HttpUrl.Validate(@"ftp://user:pass@ftp.ftp.com/ftpfolder/"));
            Assert.IsFalse(HttpUrl.Validate(@"file://c:/folder/subfolder"));
            Assert.IsFalse(HttpUrl.Validate(@"file://c:/folder/subfolder/"));
            Assert.IsFalse(HttpUrl.Validate(@"file://c:/folder/subfolder/myfile.txt"));
        }
    }
}
