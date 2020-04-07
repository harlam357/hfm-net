/*
 * HFM.NET - String Operations Helper Class Tests
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;

using NUnit.Framework;

namespace HFM.Core
{
   [TestFixture]
   public class ValidateTests
   {
      [Test]
      public void PathInstancePath()
      {
         // Windows
         Assert.IsTrue(Validate.Path(@"C:\"));
         Assert.IsTrue(Validate.Path(@"C:\Data\Subfolder"));
         Assert.IsTrue(Validate.Path(@"C:\Data\Subfolder\"));
         Assert.IsTrue(Validate.Path(@"C:\Data\Subfolder\MyFile.txt"));
         Assert.IsTrue(Validate.Path(@"C:\My Documents\My Letters"));
         Assert.IsTrue(Validate.Path(@"C:\My Documents\My Letters\"));
         Assert.IsTrue(Validate.Path(@"C:\My Documents\My Letters\My Letter.txt"));
         
         // UNC
         Assert.IsFalse(Validate.Path(@"\\server\"));
         Assert.IsFalse(Validate.Path(@"\\server\c$"));
         Assert.IsTrue(Validate.Path(@"\\server\c$\"));
         Assert.IsTrue(Validate.Path(@"\\server\c$\autoexec.bat"));
         Assert.IsTrue(Validate.Path(@"\\server\data\Subfolder"));
         Assert.IsTrue(Validate.Path(@"\\server\data\Subfo$#!@$^%$#(lder\"));
         Assert.IsTrue(Validate.Path(@"\\server\data\Subfolder\MyFile.txt"));
         Assert.IsTrue(Validate.Path(@"\\server\docs\My Letters"));
         Assert.IsTrue(Validate.Path(@"\\server\docs\My Letters\"));
         Assert.IsTrue(Validate.Path(@"\\server\docs\My Letters\My Letter.txt"));
         Assert.IsTrue(Validate.Path(@"\\server4\Folding@h`~!@#$%^&()_-ome-gpu\"));
         
         // Unix-Like
         Assert.IsTrue(Validate.Path(@"/somewhere/somewhereelse"));
         Assert.IsTrue(Validate.Path(@"/somewhere/somewhereelse/"));
         Assert.IsTrue(Validate.Path(@"/somewhere/somewhereelse/fasfsdf"));
         Assert.IsTrue(Validate.Path(@"/somewhere/somewhereelse/fasfsdf/"));
         Assert.IsTrue(Validate.Path(@"~/somesubhomefolder"));
         Assert.IsTrue(Validate.Path(@"~/somesubhomefolder/"));
         Assert.IsTrue(Validate.Path(@"~/somesubhomefolder/subagain"));
         Assert.IsTrue(Validate.Path(@"~/somesubhomefolder/subagain/"));
         Assert.IsTrue(Validate.Path(@"/b/"));
      }
      
      [Test]
      public void FtpPath()
      {
         // Unix-Like (same RegEx used for Path Instance Path)
         Assert.IsTrue(Validate.FtpPath(@"/somewhere/somewhereelse"));
         Assert.IsTrue(Validate.FtpPath(@"/somewhere/somewhereelse/"));
         Assert.IsTrue(Validate.FtpPath(@"/somewhere/somewhereelse/fasfsdf"));
         Assert.IsTrue(Validate.FtpPath(@"/somewhere/somewhereelse/fasfsdf/"));
         Assert.IsTrue(Validate.FtpPath(@"~/somesubhomefolder"));
         Assert.IsTrue(Validate.FtpPath(@"~/somesubhomefolder/"));
         Assert.IsTrue(Validate.FtpPath(@"~/somesubhomefolder/subagain"));
         Assert.IsTrue(Validate.FtpPath(@"~/somesubhomefolder/subagain/"));
         Assert.IsTrue(Validate.FtpPath(@"/b/"));
         Assert.IsTrue(Validate.FtpPath(@"/"));

         Assert.IsFalse(Validate.FtpPath(null));
         Assert.IsFalse(Validate.FtpPath(String.Empty));
      }
      
      [Test]
      public void HttpUrl()
      {
         Assert.IsTrue(Validate.HttpUrl(@"http://www.domain.com/somesite/index.html"));
         Assert.IsTrue(Validate.HttpUrl(@"https://some-server/serverfolder/dsfasfsdf"));
         Assert.IsTrue(Validate.HttpUrl(@"https://some-server/serverfolder/dsfasfsdf/"));
         Assert.IsFalse(Validate.HttpUrl(@"ftp://ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(Validate.HttpUrl(@"ftp://user:pass@ftp.ftp.com/ftpfolder/"));
         Assert.IsTrue(Validate.HttpUrl(@"file://c:/folder/subfolder"));
         Assert.IsTrue(Validate.HttpUrl(@"file://c:/folder/subfolder/"));
         Assert.IsTrue(Validate.HttpUrl(@"file://c:/folder/subfolder/myfile.txt"));
         Assert.IsTrue(Validate.HttpUrl(@"http://fah-web.stanford.edu/psummary.html"));
      }

      [Test]
      public void ServerPortPair()
      {
         Assert.IsTrue(Validate.ServerPortPair("Server", "Port"));

         try
         {
            Validate.ServerPortPair("Server", String.Empty);
         }
         catch (ArgumentException)
         { }

         try
         {
            Validate.ServerPortPair(String.Empty, "Port");
         }
         catch (ArgumentException)
         { }

         try
         {
            Validate.ServerPortPair(String.Empty, String.Empty);
         }
         catch (ArgumentException)
         { }
      }
   }
}
