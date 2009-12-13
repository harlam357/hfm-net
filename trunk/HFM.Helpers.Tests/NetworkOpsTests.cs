/*
 * HFM.NET - Network Operations Helper Class Tests
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Globalization;
using System.IO;
using System.Net;

using NUnit.Framework;
using Rhino.Mocks;

namespace HFM.Helpers.Tests
{
   [TestFixture]
   public class NetworkOpsTests
   {
      private readonly string TestFilesFolder = String.Format(CultureInfo.InvariantCulture, "..{0}..{0}TestFiles", Path.DirectorySeparatorChar);
      private readonly string TestFilesWorkFolder = String.Format(CultureInfo.InvariantCulture, "..{0}..{0}TestFiles{0}Work", Path.DirectorySeparatorChar);
   
      [SetUp]
      public void Init()
      {
         DirectoryInfo di = new DirectoryInfo(TestFilesWorkFolder);
         if (di.Exists)
         {
            di.Delete(true);
         }
         
         di.Create();
      }
      
      [TearDown]
      public void CleanUp()
      {
         DirectoryInfo di = new DirectoryInfo(TestFilesWorkFolder);
         if (di.Exists)
         {
            di.Delete(true);
         }
      }

      [Test]
      public void FtpUploadHelper()
      {
         MockRepository mocks = new MockRepository();
         FtpWebRequest Request = mocks.DynamicMock<FtpWebRequest>();
         
         using (FileStream stream = new FileStream(Path.Combine(TestFilesWorkFolder, "upload.html"), FileMode.Create))
         {
            Expect.Call(Request.GetRequestStream()).Return(stream);
            mocks.ReplayAll();

            NetworkOps.FtpUploadHelper(Request, Path.Combine(TestFilesFolder, "test.html"), 
               String.Empty, String.Empty, true);

            mocks.VerifyAll();
         }

         Assert.IsTrue(File.Exists(Path.Combine(TestFilesWorkFolder, "upload.html")));
      }

      [Test]
      public void FtpDownloadHelper_BinaryDownload()
      {
         MockRepository mocks = new MockRepository();
         FtpWebRequest Request = mocks.DynamicMock<FtpWebRequest>();
         FtpWebResponse Response = mocks.DynamicMock<FtpWebResponse>();
         Expect.Call(Request.GetResponse()).Return(Response);
         
         using (FileStream stream = new FileStream(Path.Combine(TestFilesFolder, "test.html"), FileMode.Open))
         {
            Expect.Call(Response.GetResponseStream()).Return(stream);
            mocks.ReplayAll();

            NetworkOps.FtpDownloadHelper(Request, Path.Combine(TestFilesWorkFolder, "ftp_download_binary.html"), 
               String.Empty, String.Empty, DownloadType.Binary);

            mocks.VerifyAll();
         }

         Assert.IsTrue(File.Exists(Path.Combine(TestFilesWorkFolder, "ftp_download_binary.html")));
      }

      [Test]
      public void FtpDownloadHelper_TextDownload()
      {
         MockRepository mocks = new MockRepository();
         FtpWebRequest Request = mocks.DynamicMock<FtpWebRequest>();
         FtpWebResponse Response = mocks.DynamicMock<FtpWebResponse>();
         Expect.Call(Request.GetResponse()).Return(Response);

         using (FileStream stream = new FileStream(Path.Combine(TestFilesFolder, "test.html"), FileMode.Open))
         {
            Expect.Call(Response.GetResponseStream()).Return(stream);
            mocks.ReplayAll();

            NetworkOps.FtpDownloadHelper(Request, Path.Combine(TestFilesWorkFolder, "ftp_download_text.html"), 
               String.Empty, String.Empty, DownloadType.ASCII);

            mocks.VerifyAll();
         }

         Assert.IsTrue(File.Exists(Path.Combine(TestFilesWorkFolder, "ftp_download_text.html")));
      }

      [Test]
      public void HttpDownloadHelper_BinaryDownload()
      {
         MockRepository mocks = new MockRepository();
         WebRequest Request = mocks.DynamicMock<WebRequest>();
         WebResponse Response = mocks.DynamicMock<WebResponse>();
         Expect.Call(Request.GetResponse()).Return(Response);

         using (FileStream stream = new FileStream(Path.Combine(TestFilesFolder, "test.html"), FileMode.Open))
         {
            Expect.Call(Response.GetResponseStream()).Return(stream);
            mocks.ReplayAll();

            NetworkOps.HttpDownloadHelper(Request, Path.Combine(TestFilesWorkFolder, "http_download_binary.html"), 
               "InstanceName", String.Empty, String.Empty, DownloadType.Binary);

            mocks.VerifyAll();
         }

         Assert.IsTrue(File.Exists(Path.Combine(TestFilesWorkFolder, "http_download_binary.html")));
      }

      [Test]
      public void HttpDownloadHelper_TextDownload()
      {
         MockRepository mocks = new MockRepository();
         WebRequest Request = mocks.DynamicMock<WebRequest>();
         WebResponse Response = mocks.DynamicMock<WebResponse>();
         Expect.Call(Request.GetResponse()).Return(Response);

         using (FileStream stream = new FileStream(Path.Combine(TestFilesFolder, "test.html"), FileMode.Open))
         {
            Expect.Call(Response.GetResponseStream()).Return(stream);
            mocks.ReplayAll();

            NetworkOps.HttpDownloadHelper(Request, Path.Combine(TestFilesWorkFolder, "http_download_text.html"),
               "InstanceName", String.Empty, String.Empty, DownloadType.ASCII);

            mocks.VerifyAll();
         }

         Assert.IsTrue(File.Exists(Path.Combine(TestFilesWorkFolder, "http_download_text.html")));
      }

      [Test]
      public void HttpDownloadHelper_UnitInfoTooBig()
      {
         MockRepository mocks = new MockRepository();
         WebRequest Request = mocks.DynamicMock<WebRequest>();
         WebResponse Response = mocks.DynamicMock<WebResponse>();
         Expect.Call(Request.GetResponse()).Return(Response);
         Expect.Call(Response.ContentLength).Return(1050000);
         
         File.CreateText(Path.Combine(TestFilesWorkFolder, "unitinfo.txt")).Close();

         mocks.ReplayAll();

         NetworkOps.HttpDownloadHelper(Request, Path.Combine(TestFilesWorkFolder, "unitinfo.txt"),
            "InstanceName", String.Empty, String.Empty, DownloadType.UnitInfo);

         mocks.VerifyAll();

         Assert.IsFalse(File.Exists(Path.Combine(TestFilesWorkFolder, "unitinfo.txt")));
      }

      [Test]
      public void GetProteinDescription()
      {
         MockRepository mocks = new MockRepository();
         WebRequest Request = mocks.DynamicMock<WebRequest>();
         WebResponse Response = mocks.DynamicMock<WebResponse>();
         Expect.Call(Request.GetResponse()).Return(Response);

         string Description;
         using (FileStream stream = new FileStream(Path.Combine(TestFilesFolder, "fahproject2669.html"), FileMode.Open))
         {
            Expect.Call(Response.GetResponseStream()).Return(stream);
            mocks.ReplayAll();

            Description = NetworkOps.GetProteinDescription(Request, String.Empty, String.Empty);

            mocks.VerifyAll();
         }

         Assert.AreEqual("<TABLE align=center width=650 border=0 cellpadding=2 >\n<TR><TD><font size=\"3\"><b><A name = 2669>Project 2669</A></b></font></TD></TR><TR><TD><center>\n<br>\nThese projects study how influenza virus recognizes and infects cells.  We are developing new simulation methods to better understand these processes.\n<br><br>\n</center>\n<BR><BR><BR></TD></TR><TR><TD></TD></TR></TABLE>", 
                         Description);
      }

      [Test]
      public void SetNetworkCredentials()
      {
         WebRequest request = WebRequest.Create("http://www.google.com");
         Assert.IsNull(request.Credentials);
         NetworkOps.SetNetworkCredentials(request, String.Empty, String.Empty);
         Assert.IsNull(request.Credentials);
         NetworkOps.SetNetworkCredentials(request, "username", "password");
         Assert.IsNotNull(request.Credentials);

         WebRequest request2 = WebRequest.Create("http://www.google.com");
         Assert.IsNull(request2.Credentials);
         NetworkOps.SetNetworkCredentials(request2, String.Empty, String.Empty);
         Assert.IsNull(request2.Credentials);
         NetworkOps.SetNetworkCredentials(request2, "domain\\username", "password");
         Assert.IsNotNull(request2.Credentials);
      }
   }
}
