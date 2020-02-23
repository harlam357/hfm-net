
using NUnit.Framework;

namespace HFM.Core.Internal
{
    [TestFixture]
    public class FileSystemTests
    {
        [Test]
        public void FileSystem_PathsEqual_Test()
        {
            Assert.IsTrue(FileSystem.PathsEqual(@"c:\folder\path", @"C:\FOLDER\PATH\"));
            Assert.IsFalse(FileSystem.PathsEqual(@"c:\folder\path", @"D:\FOLDER\PATH\"));
            Assert.IsFalse(FileSystem.PathsEqual(@"c:\folder\path/", @"C:\FOLDER\PATH\"));
            Assert.IsTrue(FileSystem.PathsEqual(@"folder\path", @"FOLDER\PATH\"));
            Assert.IsTrue(FileSystem.PathsEqual(@"folder/path", @"FOLDER/PATH/"));
            Assert.IsFalse(FileSystem.PathsEqual(@"/folder/path", @"FOLDER/PATH/"));
            Assert.IsTrue(FileSystem.PathsEqual(@"/folder/path", @"/FOLDER/PATH/"));
        }
    }
}
