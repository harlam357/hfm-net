/*
 * HFM.NET - String Operations Helper Class Tests
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using HFM.Framework;
using HFM.Helpers;

namespace HFM.Helpers.Tests
{
   [TestFixture]
   public class StringOpsTests
   {
      [Test]
      public void ValidateInstanceName()
      {
         Assert.IsTrue(StringOps.ValidateInstanceName("+a+"));
         Assert.IsTrue(StringOps.ValidateInstanceName("=a="));
         Assert.IsTrue(StringOps.ValidateInstanceName("-a-"));
         Assert.IsTrue(StringOps.ValidateInstanceName("_a_"));
         Assert.IsTrue(StringOps.ValidateInstanceName("$a$"));
         Assert.IsTrue(StringOps.ValidateInstanceName("&a&"));
         Assert.IsTrue(StringOps.ValidateInstanceName("^a^"));
         Assert.IsTrue(StringOps.ValidateInstanceName("[a["));
         Assert.IsTrue(StringOps.ValidateInstanceName("]a]"));

         Assert.IsFalse(StringOps.ValidateInstanceName("}a}"));
         Assert.IsFalse(StringOps.ValidateInstanceName("\\a\\"));
         Assert.IsFalse(StringOps.ValidateInstanceName("|a|"));
         Assert.IsFalse(StringOps.ValidateInstanceName(";a;"));
         Assert.IsFalse(StringOps.ValidateInstanceName(":a:"));
         Assert.IsFalse(StringOps.ValidateInstanceName("\'a\'"));
         Assert.IsFalse(StringOps.ValidateInstanceName("\"a\""));
         Assert.IsFalse(StringOps.ValidateInstanceName(",a,"));
         Assert.IsFalse(StringOps.ValidateInstanceName("<a<"));
         Assert.IsFalse(StringOps.ValidateInstanceName(">a>"));
         Assert.IsFalse(StringOps.ValidateInstanceName("/a/"));
         Assert.IsFalse(StringOps.ValidateInstanceName("?a?"));
         Assert.IsFalse(StringOps.ValidateInstanceName("`a`"));
         Assert.IsFalse(StringOps.ValidateInstanceName("~a~"));
         Assert.IsFalse(StringOps.ValidateInstanceName("!a!"));
         Assert.IsFalse(StringOps.ValidateInstanceName("@a@"));
         Assert.IsFalse(StringOps.ValidateInstanceName("#a#"));
         Assert.IsFalse(StringOps.ValidateInstanceName("%a%"));
         Assert.IsFalse(StringOps.ValidateInstanceName("*a*"));
         Assert.IsFalse(StringOps.ValidateInstanceName("(a("));
         Assert.IsFalse(StringOps.ValidateInstanceName(")a)"));
      }
      
      [Test]
      public void CleanInstanceName()
      {
         string str;
         str = StringOps.CleanInstanceName("+a}");
         Assert.AreEqual("+a", str);
         str = StringOps.CleanInstanceName("}a+");
         Assert.AreEqual("a+", str);
         str = StringOps.CleanInstanceName("=a\\");
         Assert.AreEqual("=a", str);
         str = StringOps.CleanInstanceName("\\a=");
         Assert.AreEqual("a=", str);
      }

      [Test]
      public void ValidateFileName()
      {
         Assert.IsTrue(StringOps.ValidateFileName("FAHlog.txt"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t\\t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t/t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t:t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t*t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t?t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t\"t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t<t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t>t"));
         Assert.IsFalse(StringOps.ValidateFileName("FAHlog.t|t"));
      }
      
      [Test]
      public void ValidatePathInstancePath()
      {
         // Windows
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"C:\"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"C:\Data\Subfolder"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"C:\Data\Subfolder\"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"C:\Data\Subfolder\MyFile.txt"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"C:\My Documents\My Letters"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"C:\My Documents\My Letters\"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"C:\My Documents\My Letters\My Letter.txt"));
         
         // UNC
         Assert.IsFalse(StringOps.ValidatePathInstancePath(@"\\server\"));
         Assert.IsFalse(StringOps.ValidatePathInstancePath(@"\\server\c$"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\c$\"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\c$\autoexec.bat"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\data\Subfolder"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\data\Subfo$#!@$^%$#(lder\"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\data\Subfolder\MyFile.txt"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\docs\My Letters"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\docs\My Letters\"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server\docs\My Letters\My Letter.txt"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"\\server4\Folding@h`~!@#$%^&()_-ome-gpu\"));
         
         // Unix-Like
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"/somewhere/somewhereelse"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"/somewhere/somewhereelse/"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"/somewhere/somewhereelse/fasfsdf"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"/somewhere/somewhereelse/fasfsdf/"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"~/somesubhomefolder"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"~/somesubhomefolder/"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"~/somesubhomefolder/subagain"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"~/somesubhomefolder/subagain/"));
         Assert.IsTrue(StringOps.ValidatePathInstancePath(@"/b/"));
      }
      
      [Test]
      public void ValidateServerName()
      {
         // This Validation is pretty wide open.  Should probably not validate
         // (..) two dots in a row and server names should probably be forced
         // to begin and end an alpha-numeric character.
         Assert.IsTrue(StringOps.ValidateServerName(@"ftp.someserver.com"));
         Assert.IsTrue(StringOps.ValidateServerName(@"ftp..some.server..com"));
         Assert.IsTrue(StringOps.ValidateServerName(@"MediaServer2"));
         Assert.IsTrue(StringOps.ValidateServerName("-a-"));
         Assert.IsTrue(StringOps.ValidateServerName("_a_"));
         Assert.IsTrue(StringOps.ValidateServerName(".a."));
         Assert.IsTrue(StringOps.ValidateServerName("%a%"));

         Assert.IsFalse(StringOps.ValidateServerName("+a+"));
         Assert.IsFalse(StringOps.ValidateServerName("=a="));
         Assert.IsFalse(StringOps.ValidateServerName("$a$"));
         Assert.IsFalse(StringOps.ValidateServerName("&a&"));
         Assert.IsFalse(StringOps.ValidateServerName("^a^"));
         Assert.IsFalse(StringOps.ValidateServerName("[a["));
         Assert.IsFalse(StringOps.ValidateServerName("]a]"));
      }
      
      [Test]
      public void ValidateFtpPath()
      {
         // Unix-Like (same RegEx used for Path Instance Path)
         Assert.IsTrue(StringOps.ValidateFtpPath(@"/somewhere/somewhereelse"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"/somewhere/somewhereelse/"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"/somewhere/somewhereelse/fasfsdf"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"/somewhere/somewhereelse/fasfsdf/"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"~/somesubhomefolder"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"~/somesubhomefolder/"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"~/somesubhomefolder/subagain"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"~/somesubhomefolder/subagain/"));
         Assert.IsTrue(StringOps.ValidateFtpPath(@"/b/"));
      }
      
      [Test]
      public void ValidateHttpURL()
      {
         Assert.IsTrue(StringOps.ValidateHttpURL(@"http://www.domain.com/somesite/index.html"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"https://some-server/serverfolder/dsfasfsdf"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"https://some-server/serverfolder/dsfasfsdf/"));
         Assert.IsFalse(StringOps.ValidateHttpURL(@"ftp://ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(StringOps.ValidateHttpURL(@"ftp://user:pass@ftp.ftp.com/ftpfolder/"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"file://c:/folder/subfolder"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"file://c:/folder/subfolder/"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"file://c:/folder/subfolder/myfile.txt"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"smb://smb.smb.com"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"smb://smb.smb.com/"));
         Assert.IsTrue(StringOps.ValidateHttpURL(@"http://fah-web.stanford.edu/psummary.html"));
      }

      [Test]
      public void ValidateHttpOrFtpUrl()
      {
         Assert.IsTrue(StringOps.ValidateHttpOrFtpUrl(@"http://www.domain.com/somesite/index.html"));
         Assert.IsTrue(StringOps.ValidateHttpOrFtpUrl(@"https://some-server/serverfolder/dsfasfsdf"));
         Assert.IsTrue(StringOps.ValidateHttpOrFtpUrl(@"https://some-server/serverfolder/dsfasfsdf/"));
         Assert.IsTrue(StringOps.ValidateHttpOrFtpUrl(@"ftp://ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(StringOps.ValidateHttpOrFtpUrl(@"ftp://user:pass@ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(StringOps.ValidateHttpOrFtpUrl(@"file://c:/folder/subfolder"));
         Assert.IsFalse(StringOps.ValidateHttpOrFtpUrl(@"file://c:/folder/subfolder/"));
         Assert.IsFalse(StringOps.ValidateHttpOrFtpUrl(@"file://c:/folder/subfolder/myfile.txt"));
         Assert.IsFalse(StringOps.ValidateHttpOrFtpUrl(@"smb://smb.smb.com"));
         Assert.IsFalse(StringOps.ValidateHttpOrFtpUrl(@"smb://smb.smb.com/"));
         Assert.IsTrue(StringOps.ValidateHttpOrFtpUrl(@"http://fah-web.stanford.edu/psummary.html"));
      }
      
      [Test]
      public void ValidateFtpWithUserPassUrl()
      {
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"http://www.domain.com/somesite/index.html"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"https://some-server/serverfolder/dsfasfsdf"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"https://some-server/serverfolder/dsfasfsdf/"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"ftp://ftp.ftp.com/ftpfolder/"));
         Assert.IsTrue(StringOps.ValidateFtpWithUserPassUrl(@"ftp://user:pass@ftp.ftp.com/ftpfolder/"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"file://c:/folder/subfolder"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"file://c:/folder/subfolder/"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"file://c:/folder/subfolder/myfile.txt"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"smb://smb.smb.com"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"smb://smb.smb.com/"));
         Assert.IsFalse(StringOps.ValidateFtpWithUserPassUrl(@"http://fah-web.stanford.edu/psummary.html"));
      }
      
      [Test]
      public void ValidateEmailAddress()
      {
         Assert.IsTrue(StringOps.ValidateEmailAddress("someone@home.co"));
         Assert.IsTrue(StringOps.ValidateEmailAddress("someone@home.com"));
         Assert.IsTrue(StringOps.ValidateEmailAddress("someone@home.comm"));
         Assert.IsFalse(StringOps.ValidateEmailAddress("@home.com"));
         Assert.IsTrue(StringOps.ValidateEmailAddress("a@home.com"));
         Assert.IsFalse(StringOps.ValidateEmailAddress("someone@home"));
         Assert.IsFalse(StringOps.ValidateEmailAddress("someone@home.c"));
         // RegEx here that does not validate (..) two dots in a row - see ValidateServerName()
         Assert.IsFalse(StringOps.ValidateEmailAddress("someelse@not.at.home..com"));
      }
      
      [Test]
      public void ValidateUsernamePasswordPair()
      {
         Assert.IsTrue(StringOps.ValidateUsernamePasswordPair("Username", "Password"));

         try
         {
            StringOps.ValidateUsernamePasswordPair("Username", String.Empty);
         }
         catch (ArgumentException)
         { }

         try
         {
            StringOps.ValidateUsernamePasswordPair(String.Empty, "Password");
         }
         catch (ArgumentException)
         { }
         
         Assert.IsFalse(StringOps.ValidateUsernamePasswordPair(String.Empty, String.Empty));
         
         try
         {
            StringOps.ValidateUsernamePasswordPair(String.Empty, String.Empty, true);
         }
         catch (ArgumentException)
         { }
      }

      [Test]
      public void ValidateServerPortPair()
      {
         Assert.IsTrue(StringOps.ValidateServerPortPair("Server", "Port"));

         try
         {
            StringOps.ValidateServerPortPair("Server", String.Empty);
         }
         catch (ArgumentException)
         { }

         try
         {
            StringOps.ValidateServerPortPair(String.Empty, "Port");
         }
         catch (ArgumentException)
         { }

         try
         {
            StringOps.ValidateServerPortPair(String.Empty, String.Empty);
         }
         catch (ArgumentException)
         { }
      }
      
      [Test]
      public void WorkUnitResultFromString()
      {
         Assert.AreEqual(WorkUnitResult.FinishedUnit, StringOps.WorkUnitResultFromString("FINISHED_UNIT"));
         Assert.AreEqual(WorkUnitResult.EarlyUnitEnd, StringOps.WorkUnitResultFromString("EARLY_UNIT_END"));
         Assert.AreEqual(WorkUnitResult.UnstableMachine, StringOps.WorkUnitResultFromString("UNSTABLE_MACHINE"));
         Assert.AreEqual(WorkUnitResult.Interrupted, StringOps.WorkUnitResultFromString("INTERRUPTED"));
         Assert.AreEqual(WorkUnitResult.BadWorkUnit, StringOps.WorkUnitResultFromString("BAD_WORK_UNIT"));
         Assert.AreEqual(WorkUnitResult.CoreOutdated, StringOps.WorkUnitResultFromString("CORE_OUTDATED"));
         Assert.AreEqual(WorkUnitResult.Unknown, StringOps.WorkUnitResultFromString("afasfdsafasdfas"));
      }
   }
}
