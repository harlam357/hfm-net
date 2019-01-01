
using NUnit.Framework;

namespace HFM.Core
{
   [TestFixture]
   public class FileSystemPathTests
   {
      [Test]
      public void FileSystemPath_Equals_Test()
      {
         Assert.IsTrue(FileSystemPath.Equals(@"c:\folder\path", @"C:\FOLDER\PATH\"));
         Assert.IsFalse(FileSystemPath.Equals(@"c:\folder\path", @"D:\FOLDER\PATH\"));
         Assert.IsFalse(FileSystemPath.Equals(@"c:\folder\path/", @"C:\FOLDER\PATH\"));
         Assert.IsTrue(FileSystemPath.Equals(@"folder\path", @"FOLDER\PATH\"));
         Assert.IsTrue(FileSystemPath.Equals(@"folder/path", @"FOLDER/PATH/"));
         Assert.IsFalse(FileSystemPath.Equals(@"/folder/path", @"FOLDER/PATH/"));
         Assert.IsTrue(FileSystemPath.Equals(@"/folder/path", @"/FOLDER/PATH/"));
      }
   }
}
