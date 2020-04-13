
using System;

using NUnit.Framework;

namespace HFM.Core
{
    [TestFixture]
    public class FileSystemPathTests
    {
        [Test]
        public void FileSystemPath_Validate_Tests()
        {
            // Windows
            Assert.IsTrue(FileSystemPath.Validate(@"C:\"));
            Assert.IsTrue(FileSystemPath.Validate(@"C:\Data\Subfolder"));
            Assert.IsTrue(FileSystemPath.Validate(@"C:\Data\Subfolder\"));
            Assert.IsTrue(FileSystemPath.Validate(@"C:\Data\Subfolder\MyFile.txt"));
            Assert.IsTrue(FileSystemPath.Validate(@"C:\My Documents\My Letters"));
            Assert.IsTrue(FileSystemPath.Validate(@"C:\My Documents\My Letters\"));
            Assert.IsTrue(FileSystemPath.Validate(@"C:\My Documents\My Letters\My Letter.txt"));

            // UNC
            Assert.IsFalse(FileSystemPath.Validate(@"\\server\"));
            Assert.IsFalse(FileSystemPath.Validate(@"\\server\c$"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\c$\"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\c$\autoexec.bat"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\data\Subfolder"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\data\Subfo$#!@$^%$#(lder\"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\data\Subfolder\MyFile.txt"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\docs\My Letters"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\docs\My Letters\"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server\docs\My Letters\My Letter.txt"));
            Assert.IsTrue(FileSystemPath.Validate(@"\\server4\Folding@h`~!@#$%^&()_-ome-gpu\"));

            // Unix
            Assert.IsTrue(FileSystemPath.Validate(@"/somewhere/somewhereelse"));
            Assert.IsTrue(FileSystemPath.Validate(@"/somewhere/somewhereelse/"));
            Assert.IsTrue(FileSystemPath.Validate(@"/somewhere/somewhereelse/fasfsdf"));
            Assert.IsTrue(FileSystemPath.Validate(@"/somewhere/somewhereelse/fasfsdf/"));
            Assert.IsTrue(FileSystemPath.Validate(@"~/somesubhomefolder"));
            Assert.IsTrue(FileSystemPath.Validate(@"~/somesubhomefolder/"));
            Assert.IsTrue(FileSystemPath.Validate(@"~/somesubhomefolder/subagain"));
            Assert.IsTrue(FileSystemPath.Validate(@"~/somesubhomefolder/subagain/"));
            Assert.IsTrue(FileSystemPath.Validate(@"/b/"));

            Assert.IsFalse(FileSystemPath.Validate(String.Empty));
            Assert.IsFalse(FileSystemPath.Validate("  "));
            Assert.IsFalse(FileSystemPath.Validate(null));
        }

        [Test]
        public void FileSystemPath_ValidateUnix_Tests()
        {
            // Unix
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"/somewhere/somewhereelse"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"/somewhere/somewhereelse/"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"/somewhere/somewhereelse/fasfsdf"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"/somewhere/somewhereelse/fasfsdf/"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"~/somesubhomefolder"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"~/somesubhomefolder/"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"~/somesubhomefolder/subagain"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"~/somesubhomefolder/subagain/"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"/b/"));
            Assert.IsTrue(FileSystemPath.ValidateUnix(@"/"));

            Assert.IsFalse(FileSystemPath.ValidateUnix(String.Empty));
            Assert.IsFalse(FileSystemPath.ValidateUnix("  "));
            Assert.IsFalse(FileSystemPath.ValidateUnix(null));
        }
    }
}
