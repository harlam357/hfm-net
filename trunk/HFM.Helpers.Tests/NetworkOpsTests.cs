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
using System.Net.Cache;
using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Net;

using HFM.Framework;

namespace HFM.Helpers.Tests
{
   [TestFixture]
   public class NetworkOpsTests
   {
      private readonly string TestFilesFolder = String.Format(CultureInfo.InvariantCulture, "..{0}..{0}TestFiles", Path.DirectorySeparatorChar);
      private readonly string TestFilesWorkFolder = String.Format(CultureInfo.InvariantCulture, "..{0}..{0}TestFiles{0}Work", Path.DirectorySeparatorChar);

      private readonly NetworkOps net = new NetworkOps();

      private IWindsorContainer container;
      private MockRepository mocks;
   
      [SetUp]
      public void Init()
      {
         DirectoryInfo di = new DirectoryInfo(TestFilesWorkFolder);
         if (di.Exists)
         {
            di.Delete(true);
         }
         
         di.Create();

         container = new WindsorContainer();
         mocks = new MockRepository();
         
         IPreferenceSet Prefs = mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(Prefs.GetPreference<bool>(Preference.UseProxy)).Return(false).Repeat.Any();
         container.Kernel.AddComponentInstance<IPreferenceSet>(typeof(IPreferenceSet), Prefs);
         InstanceProvider.SetContainer(container);
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
         IFtpWebOperation Operation = mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(delegate { Operation.Upload("testpath"); });
         
         IFtpWebOperationRequest OperationRequest = mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(Operation.FtpOperationRequest).Return(OperationRequest);
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);
         
         FtpWebRequest FtpRequest = mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(OperationRequest.FtpRequest).Return(FtpRequest);
         SetupResult.For(OperationRequest.Request).Return(FtpRequest);
         
         mocks.ReplayAll();
         
         net.FtpUploadHelper(Operation, "testpath", String.Empty, String.Empty, FtpType.Passive);

         Assert.AreSame(Operation, net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);
         Assert.AreEqual(true, OperationRequest.UsePassive);
         
         mocks.VerifyAll();
      }

      [Test]
      public void FtpDownloadHelper()
      {
         IFtpWebOperation Operation = mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(delegate { Operation.Download("testpath"); });

         IFtpWebOperationRequest OperationRequest = mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(Operation.FtpOperationRequest).Return(OperationRequest);
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);

         FtpWebRequest FtpRequest = mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(OperationRequest.FtpRequest).Return(FtpRequest);
         SetupResult.For(OperationRequest.Request).Return(FtpRequest);

         mocks.ReplayAll();

         net.FtpDownloadHelper(Operation, "testpath", String.Empty, String.Empty, FtpType.Active);

         Assert.AreSame(Operation, net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);
         Assert.AreEqual(false, OperationRequest.UsePassive);

         mocks.VerifyAll();
      }

      [Test]
      public void GetFtpDownloadLength()
      {
         IFtpWebOperation Operation = mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(Operation.GetDownloadLength()).Return(100);

         IFtpWebOperationRequest OperationRequest = mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(Operation.FtpOperationRequest).Return(OperationRequest);
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);

         FtpWebRequest FtpRequest = mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(OperationRequest.FtpRequest).Return(FtpRequest);
         SetupResult.For(OperationRequest.Request).Return(FtpRequest);

         mocks.ReplayAll();

         long length = net.GetFtpDownloadLength(Operation, String.Empty, String.Empty, FtpType.Active);
         Assert.AreEqual(100, length);

         Assert.AreSame(Operation, net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);
         Assert.AreEqual(false, OperationRequest.UsePassive);

         mocks.VerifyAll();
      }

      [Test]
      public void HttpDownloadHelper()
      {
         IWebOperation Operation = mocks.DynamicMock<IWebOperation>();
         Expect.Call(delegate { Operation.Download("testpath"); });

         IWebOperationRequest OperationRequest = mocks.Stub<IWebOperationRequest>();
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);

         WebRequest Request = mocks.DynamicMock<WebRequest>();
         SetupResult.For(OperationRequest.Request).Return(Request);

         mocks.ReplayAll();

         net.HttpDownloadHelper(Operation, "testpath", String.Empty, String.Empty);

         Assert.AreSame(Operation, net.HttpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);

         mocks.VerifyAll();
      }

      [Test]
      public void GetHttpDownloadLength()
      {
         IWebOperation Operation = mocks.DynamicMock<IWebOperation>();
         Expect.Call(Operation.GetDownloadLength()).Return(100);

         IWebOperationRequest OperationRequest = mocks.Stub<IWebOperationRequest>();
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);

         WebRequest Request = mocks.DynamicMock<WebRequest>();
         SetupResult.For(OperationRequest.Request).Return(Request);

         mocks.ReplayAll();

         long length = net.GetHttpDownloadLength(Operation, String.Empty, String.Empty);
         Assert.AreEqual(100, length);

         Assert.AreSame(Operation, net.HttpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);

         mocks.VerifyAll();
      }

      [Test]
      public void FtpCheckConnection()
      {
         IFtpWebOperation Operation = mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(delegate { Operation.CheckConnection(); });

         IFtpWebOperationRequest OperationRequest = mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(Operation.FtpOperationRequest).Return(OperationRequest);
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);

         FtpWebRequest FtpRequest = mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(OperationRequest.FtpRequest).Return(FtpRequest);
         SetupResult.For(OperationRequest.Request).Return(FtpRequest);

         mocks.ReplayAll();

         net.FtpCheckConnection(Operation, String.Empty, String.Empty, FtpType.Passive);

         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);
         Assert.AreEqual(false, OperationRequest.KeepAlive);
         Assert.AreEqual(5000, OperationRequest.Timeout);
         Assert.AreEqual(true, OperationRequest.UsePassive);

         mocks.VerifyAll();
      }

      [Test]
      public void HttpCheckConnection()
      {
         IWebOperation Operation = mocks.DynamicMock<IWebOperation>();
         Expect.Call(delegate { Operation.CheckConnection(); });

         IWebOperationRequest OperationRequest = mocks.Stub<IWebOperationRequest>();
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);

         WebRequest Request = mocks.DynamicMock<WebRequest>();
         SetupResult.For(OperationRequest.Request).Return(Request);

         mocks.ReplayAll();

         net.HttpCheckConnection(Operation, String.Empty, String.Empty);

         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);
         Assert.AreEqual(5000, OperationRequest.Timeout);

         mocks.VerifyAll();
      }

      [Test]
      public void GetProteinDescription()
      {
         IWebOperation Operation = mocks.DynamicMock<IWebOperation>();
         Expect.Call(delegate { Operation.Download("testpath"); })
            .IgnoreArguments()
            .Do(new DoTestCopyDelegate(DoTestCopy));

         IWebOperationRequest OperationRequest = mocks.Stub<IWebOperationRequest>();
         SetupResult.For(Operation.OperationRequest).Return(OperationRequest);

         WebRequest Request = mocks.DynamicMock<WebRequest>();
         SetupResult.For(OperationRequest.Request).Return(Request);

         mocks.ReplayAll();

         string Description = net.GetProteinDescription(Operation);

         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, OperationRequest.CachePolicy.Level);
         Assert.AreEqual(5000, OperationRequest.Timeout);

         mocks.VerifyAll();

         Assert.AreEqual("<TABLE align=center width=650 border=0 cellpadding=2 >\n<TR><TD><font size=\"3\"><b><A name = 2669>Project 2669</A></b></font></TD></TR><TR><TD><center>\n<br>\nThese projects study how influenza virus recognizes and infects cells.  We are developing new simulation methods to better understand these processes.\n<br><br>\n</center>\n<BR><BR><BR></TD></TR><TR><TD></TD></TR></TABLE>",
                         Description);
      }
      
      private delegate void DoTestCopyDelegate(string FilePath);
      
      private void DoTestCopy(string FilePath)
      {
         File.Copy(Path.Combine(TestFilesFolder, "fahproject2669.html"), FilePath, true);
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
