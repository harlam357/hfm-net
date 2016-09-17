
using NUnit.Framework;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class FileSystemPathTests
   {
      [Test]
      public void FileSystemPath_Equal_Test()
      {
         Assert.IsTrue(FileSystemPath.Equal(@"c:\folder\path", @"C:\FOLDER\PATH\"));
         Assert.IsFalse(FileSystemPath.Equal(@"c:\folder\path", @"D:\FOLDER\PATH\"));
         Assert.IsFalse(FileSystemPath.Equal(@"c:\folder\path/", @"C:\FOLDER\PATH\"));
         Assert.IsTrue(FileSystemPath.Equal(@"folder\path", @"FOLDER\PATH\"));
         Assert.IsTrue(FileSystemPath.Equal(@"folder/path", @"FOLDER/PATH/"));
         Assert.IsFalse(FileSystemPath.Equal(@"/folder/path", @"FOLDER/PATH/"));
         Assert.IsTrue(FileSystemPath.Equal(@"/folder/path", @"/FOLDER/PATH/"));
      }
   }
}
