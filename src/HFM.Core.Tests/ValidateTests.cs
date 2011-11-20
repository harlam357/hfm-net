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

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ValidateTests
   {
      [Test]
      public void InstanceNameTest()
      {
         Assert.IsTrue(Validate.ClientName("+a+"));
         Assert.IsTrue(Validate.ClientName("=a="));
         Assert.IsTrue(Validate.ClientName("-a-"));
         Assert.IsTrue(Validate.ClientName("_a_"));
         Assert.IsTrue(Validate.ClientName("$a$"));
         Assert.IsTrue(Validate.ClientName("&a&"));
         Assert.IsTrue(Validate.ClientName("^a^"));
         Assert.IsTrue(Validate.ClientName("[a["));
         Assert.IsTrue(Validate.ClientName("]a]"));

         Assert.IsFalse(Validate.ClientName("}a}"));
         Assert.IsFalse(Validate.ClientName("\\a\\"));
         Assert.IsFalse(Validate.ClientName("|a|"));
         Assert.IsFalse(Validate.ClientName(";a;"));
         Assert.IsFalse(Validate.ClientName(":a:"));
         Assert.IsFalse(Validate.ClientName("\'a\'"));
         Assert.IsFalse(Validate.ClientName("\"a\""));
         Assert.IsFalse(Validate.ClientName(",a,"));
         Assert.IsFalse(Validate.ClientName("<a<"));
         Assert.IsFalse(Validate.ClientName(">a>"));
         Assert.IsFalse(Validate.ClientName("/a/"));
         Assert.IsFalse(Validate.ClientName("?a?"));
         Assert.IsFalse(Validate.ClientName("`a`"));
         Assert.IsFalse(Validate.ClientName("~a~"));
         Assert.IsFalse(Validate.ClientName("!a!"));
         Assert.IsFalse(Validate.ClientName("@a@"));
         Assert.IsFalse(Validate.ClientName("#a#"));
         Assert.IsFalse(Validate.ClientName("%a%"));
         Assert.IsFalse(Validate.ClientName("*a*"));
         Assert.IsFalse(Validate.ClientName("(a("));
         Assert.IsFalse(Validate.ClientName(")a)"));

         Assert.IsFalse(Validate.ClientName(String.Empty));
         Assert.IsFalse(Validate.ClientName(null));
      }
      
      [Test]
      public void CleanInstanceNameTest()
      {
         string str = Validate.CleanInstanceName("+a}");
         Assert.AreEqual("+a", str);
         str = Validate.CleanInstanceName("}a+");
         Assert.AreEqual("a+", str);
         str = Validate.CleanInstanceName("=a\\");
         Assert.AreEqual("=a", str);
         str = Validate.CleanInstanceName("\\a=");
         Assert.AreEqual("a=", str);
         str = Validate.CleanInstanceName(String.Empty);
         Assert.AreEqual(String.Empty, str);
         str = Validate.CleanInstanceName(null);
         Assert.AreEqual(null, str);
      }

      [Test]
      public void FileName()
      {
         Assert.IsFalse(Validate.FileName(null));
         Assert.IsFalse(Validate.FileName(String.Empty));
         Assert.IsFalse(Validate.FileName("          "));
         Assert.IsTrue(Validate.FileName("FAHlog.txt"));
         Assert.IsFalse(Validate.FileName("FAHlog.t\\t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t/t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t:t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t*t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t?t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t\"t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t<t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t>t"));
         Assert.IsFalse(Validate.FileName("FAHlog.t|t"));
      }
      
      [Test]
      public void PathInstancePath()
      {
         // Windows
         Assert.IsTrue(Validate.PathInstancePath(@"C:\"));
         Assert.IsTrue(Validate.PathInstancePath(@"C:\Data\Subfolder"));
         Assert.IsTrue(Validate.PathInstancePath(@"C:\Data\Subfolder\"));
         Assert.IsTrue(Validate.PathInstancePath(@"C:\Data\Subfolder\MyFile.txt"));
         Assert.IsTrue(Validate.PathInstancePath(@"C:\My Documents\My Letters"));
         Assert.IsTrue(Validate.PathInstancePath(@"C:\My Documents\My Letters\"));
         Assert.IsTrue(Validate.PathInstancePath(@"C:\My Documents\My Letters\My Letter.txt"));
         
         // UNC
         Assert.IsFalse(Validate.PathInstancePath(@"\\server\"));
         Assert.IsFalse(Validate.PathInstancePath(@"\\server\c$"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\c$\"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\c$\autoexec.bat"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\data\Subfolder"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\data\Subfo$#!@$^%$#(lder\"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\data\Subfolder\MyFile.txt"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\docs\My Letters"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\docs\My Letters\"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server\docs\My Letters\My Letter.txt"));
         Assert.IsTrue(Validate.PathInstancePath(@"\\server4\Folding@h`~!@#$%^&()_-ome-gpu\"));
         
         // Unix-Like
         Assert.IsTrue(Validate.PathInstancePath(@"/somewhere/somewhereelse"));
         Assert.IsTrue(Validate.PathInstancePath(@"/somewhere/somewhereelse/"));
         Assert.IsTrue(Validate.PathInstancePath(@"/somewhere/somewhereelse/fasfsdf"));
         Assert.IsTrue(Validate.PathInstancePath(@"/somewhere/somewhereelse/fasfsdf/"));
         Assert.IsTrue(Validate.PathInstancePath(@"~/somesubhomefolder"));
         Assert.IsTrue(Validate.PathInstancePath(@"~/somesubhomefolder/"));
         Assert.IsTrue(Validate.PathInstancePath(@"~/somesubhomefolder/subagain"));
         Assert.IsTrue(Validate.PathInstancePath(@"~/somesubhomefolder/subagain/"));
         Assert.IsTrue(Validate.PathInstancePath(@"/b/"));
      }
      
      [Test]
      public void ServerName()
      {
         // This Validation is pretty wide open.  Should probably not validate
         // (..) two dots in a row and server names should probably be forced
         // to begin and end an alpha-numeric character.
         Assert.IsTrue(Validate.ServerName(@"ftp.someserver.com"));
         Assert.IsTrue(Validate.ServerName(@"ftp..some.server..com"));
         Assert.IsTrue(Validate.ServerName(@"MediaServer2"));
         Assert.IsTrue(Validate.ServerName("-a-"));
         Assert.IsTrue(Validate.ServerName("_a_"));
         Assert.IsTrue(Validate.ServerName(".a."));
         Assert.IsTrue(Validate.ServerName("%a%"));

         Assert.IsFalse(Validate.ServerName("+a+"));
         Assert.IsFalse(Validate.ServerName("=a="));
         Assert.IsFalse(Validate.ServerName("$a$"));
         Assert.IsFalse(Validate.ServerName("&a&"));
         Assert.IsFalse(Validate.ServerName("^a^"));
         Assert.IsFalse(Validate.ServerName("[a["));
         Assert.IsFalse(Validate.ServerName("]a]"));
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
      public void HttpOrFtpUrl()
      {
         Assert.IsTrue(Validate.HttpOrFtpUrl(@"http://www.domain.com/somesite/index.html"));
         Assert.IsTrue(Validate.HttpOrFtpUrl(@"https://some-server/serverfolder/dsfasfsdf"));
         Assert.IsTrue(Validate.HttpOrFtpUrl(@"https://some-server/serverfolder/dsfasfsdf/"));
         Assert.IsTrue(Validate.HttpOrFtpUrl(@"ftp://ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(Validate.HttpOrFtpUrl(@"ftp://user:pass@ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(Validate.HttpOrFtpUrl(@"file://c:/folder/subfolder"));
         Assert.IsFalse(Validate.HttpOrFtpUrl(@"file://c:/folder/subfolder/"));
         Assert.IsFalse(Validate.HttpOrFtpUrl(@"file://c:/folder/subfolder/myfile.txt"));
         Assert.IsFalse(Validate.HttpOrFtpUrl(@"smb://smb.smb.com"));
         Assert.IsFalse(Validate.HttpOrFtpUrl(@"smb://smb.smb.com/"));
         Assert.IsTrue(Validate.HttpOrFtpUrl(@"http://fah-web.stanford.edu/psummary.html"));
      }
      
      [Test]
      public void FtpWithUserPassUrl()
      {
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"http://www.domain.com/somesite/index.html"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"https://some-server/serverfolder/dsfasfsdf"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"https://some-server/serverfolder/dsfasfsdf/"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"ftp://ftp.ftp.com/ftpfolder/"));
         Assert.IsTrue(Validate.FtpWithUserPassUrl(@"ftp://user:pass@ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"file://c:/folder/subfolder"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"file://c:/folder/subfolder/"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"file://c:/folder/subfolder/myfile.txt"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"smb://smb.smb.com"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"smb://smb.smb.com/"));
         Assert.IsFalse(Validate.FtpWithUserPassUrl(@"http://fah-web.stanford.edu/psummary.html"));
      }
      
      [Test]
      public void EmailAddress()
      {
         Assert.IsTrue(Validate.EmailAddress("someone@home.co"));
         Assert.IsTrue(Validate.EmailAddress("someone@home.com"));
         Assert.IsTrue(Validate.EmailAddress("someone@home.comm"));
         Assert.IsFalse(Validate.EmailAddress("@home.com"));
         Assert.IsTrue(Validate.EmailAddress("a@home.com"));
         Assert.IsFalse(Validate.EmailAddress("someone@home"));
         Assert.IsFalse(Validate.EmailAddress("someone@home.c"));
         // RegEx here that does not validate (..) two dots in a row - see ServerName()
         Assert.IsFalse(Validate.EmailAddress("someelse@not.at.home..com"));
      }
      
      [Test]
      public void UsernamePasswordPair()
      {
         Assert.IsTrue(Validate.UsernamePasswordPair("Username", "Password"));

         try
         {
            Validate.UsernamePasswordPair("Username", String.Empty);
         }
         catch (ArgumentException)
         { }

         try
         {
            Validate.UsernamePasswordPair(String.Empty, "Password");
         }
         catch (ArgumentException)
         { }
         
         Assert.IsFalse(Validate.UsernamePasswordPair(String.Empty, String.Empty));
         
         try
         {
            Validate.UsernamePasswordPair(String.Empty, String.Empty, true);
         }
         catch (ArgumentException)
         { }
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
