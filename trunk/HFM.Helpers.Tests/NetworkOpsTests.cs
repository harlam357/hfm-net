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
using System.IO;
using System.Net;

using NUnit.Framework;

namespace HFM.Helpers.Tests
{
   [TestFixture]
   public class NetworkOpsTests
   {
      [Test]
      [ExpectedException(typeof(FileNotFoundException))]
      public void FtpUploadHelper_LocalFileDoesNotExistTest()
      {
         NetworkOps.FtpUploadHelper("notexistservername", "/rootsub/subfolder", "page.html", String.Empty, String.Empty);
      }
      
      [Test]
      [ExpectedException(typeof(WebException))]
      public void FtpUploadHelper_HostNameDoesNotExistTest()
      {
         NetworkOps.FtpUploadHelper("notexistservername", "/rootsub/subfolder", Path.Combine("TestFiles", "test.html"), String.Empty, String.Empty);
      }

      [Test]
      [ExpectedException(typeof(WebException))]
      public void FtpDownloadHelper_HostNameDoesNotExistTest()
      {
         NetworkOps.FtpDownloadHelper("notexistservername", "/rootsub/subfolder", "FAHlog.txt", "FAHlog.txt", String.Empty, String.Empty);
      }

      [Test]
      [ExpectedException(typeof(WebException))]
      public void HttpDownloadHelper_HostNameDoesNotExistTest()
      {
         NetworkOps.HttpDownloadHelper("http://notexistservername/unitinfo.txt", "unitinfo.txt", "InstanceName", String.Empty, String.Empty, DownloadType.UnitInfo);
      }

      [Test]
      public void GetProteinDescription_HostNameDoesNotExistTest()
      {
         string Url = "http://notexistservername/page.html";
         Assert.AreEqual(Url, NetworkOps.GetProteinDescription(Url));
      }

      [Test]
      public void SetNetworkCredentials_CredentialsCreationTest()
      {
         WebRequest request = WebRequest.Create("http://www.google.com");
         Assert.IsNull(request.Credentials);
         NetworkOps.SetNetworkCredentials(request, String.Empty, String.Empty);
         Assert.IsNull(request.Credentials);
         NetworkOps.SetNetworkCredentials(request, "username", "password");
         Assert.IsNotNull(request.Credentials);
      }
   }
}
