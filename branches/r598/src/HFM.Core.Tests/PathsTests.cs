
using NUnit.Framework;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class PathsTests
   {
      [Test]
      public void PathsEqualTest()
      {
         Assert.IsTrue(Paths.Equal(@"c:\folder\path", @"C:\FOLDER\PATH\"));
         Assert.IsFalse(Paths.Equal(@"c:\folder\path", @"D:\FOLDER\PATH\"));
         Assert.IsFalse(Paths.Equal(@"c:\folder\path/", @"C:\FOLDER\PATH\"));
         Assert.IsTrue(Paths.Equal(@"folder\path", @"FOLDER\PATH\"));
         Assert.IsTrue(Paths.Equal(@"folder/path", @"FOLDER/PATH/"));
         Assert.IsFalse(Paths.Equal(@"/folder/path", @"FOLDER/PATH/"));
         Assert.IsTrue(Paths.Equal(@"/folder/path", @"/FOLDER/PATH/"));
      }
   }
}
