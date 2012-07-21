/*
 * HFM.NET - Network Operations Helper Class Tests
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Net;
using System.Net.Cache;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Net;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class NetworkOpsTests
   {
      private MockRepository _mocks;
      private NetworkOps _net;

      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();

         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(prefs.Get<bool>(Preference.UseProxy)).Return(false).Repeat.Any();
         _net = new NetworkOps(prefs);
      }

      [Test]
      public void FtpUploadHelper()
      {
         var operation = _mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(() => operation.Upload("testpath", -1));

         var operationRequest = _mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(operation.FtpOperationRequest).Return(operationRequest);
         SetupResult.For(operation.OperationRequest).Return(operationRequest);

         var ftpRequest = _mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(operationRequest.FtpRequest).Return(ftpRequest);
         SetupResult.For(operationRequest.Request).Return(ftpRequest);

         _mocks.ReplayAll();

         _net.FtpUploadHelper(operation, "testpath", String.Empty, String.Empty, FtpType.Passive);

         Assert.AreSame(operation, _net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, operationRequest.CachePolicy.Level);
         Assert.AreEqual(true, operationRequest.UsePassive);

         _mocks.VerifyAll();
      }

      [Test]
      public void FtpDownloadHelper()
      {
         var operation = _mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(() => operation.Download("testpath"));

         var operationRequest = _mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(operation.FtpOperationRequest).Return(operationRequest);
         SetupResult.For(operation.OperationRequest).Return(operationRequest);

         var ftpRequest = _mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(operationRequest.FtpRequest).Return(ftpRequest);
         SetupResult.For(operationRequest.Request).Return(ftpRequest);

         _mocks.ReplayAll();

         _net.FtpDownloadHelper(operation, "testpath", String.Empty, String.Empty, FtpType.Active);

         Assert.AreSame(operation, _net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, operationRequest.CachePolicy.Level);
         Assert.AreEqual(false, operationRequest.UsePassive);

         _mocks.VerifyAll();
      }

      [Test]
      public void GetFtpDownloadLength()
      {
         var operation = _mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(operation.GetDownloadLength()).Return(100);

         var operationRequest = _mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(operation.FtpOperationRequest).Return(operationRequest);
         SetupResult.For(operation.OperationRequest).Return(operationRequest);

         var ftpRequest = _mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(operationRequest.FtpRequest).Return(ftpRequest);
         SetupResult.For(operationRequest.Request).Return(ftpRequest);

         _mocks.ReplayAll();

         long length = _net.GetFtpDownloadLength(operation, String.Empty, String.Empty, FtpType.Active);
         Assert.AreEqual(100, length);

         Assert.AreSame(operation, _net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, operationRequest.CachePolicy.Level);
         Assert.AreEqual(false, operationRequest.UsePassive);

         _mocks.VerifyAll();
      }

      [Test]
      public void HttpDownloadHelper()
      {
         var operation = _mocks.DynamicMock<IWebOperation>();
         Expect.Call(() => operation.Download("testpath"));

         var operationRequest = _mocks.Stub<IWebOperationRequest>();
         SetupResult.For(operation.OperationRequest).Return(operationRequest);

         var request = _mocks.DynamicMock<WebRequest>();
         SetupResult.For(operationRequest.Request).Return(request);

         _mocks.ReplayAll();

         _net.HttpDownloadHelper(operation, "testpath", String.Empty, String.Empty);

         Assert.AreSame(operation, _net.HttpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, operationRequest.CachePolicy.Level);

         _mocks.VerifyAll();
      }

      [Test]
      public void GetHttpDownloadLength()
      {
         var operation = _mocks.DynamicMock<IWebOperation>();
         Expect.Call(operation.GetDownloadLength()).Return(100);

         var operationRequest = _mocks.Stub<IWebOperationRequest>();
         SetupResult.For(operation.OperationRequest).Return(operationRequest);

         var request = _mocks.DynamicMock<WebRequest>();
         SetupResult.For(operationRequest.Request).Return(request);

         _mocks.ReplayAll();

         long length = _net.GetHttpDownloadLength(operation, String.Empty, String.Empty);
         Assert.AreEqual(100, length);

         Assert.AreSame(operation, _net.HttpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, operationRequest.CachePolicy.Level);

         _mocks.VerifyAll();
      }

      [Test]
      public void FtpCheckConnection()
      {
         var operation = _mocks.DynamicMock<IFtpWebOperation>();
         Expect.Call(operation.CheckConnection);

         var operationRequest = _mocks.Stub<IFtpWebOperationRequest>();
         SetupResult.For(operation.FtpOperationRequest).Return(operationRequest);
         SetupResult.For(operation.OperationRequest).Return(operationRequest);

         var ftpRequest = _mocks.DynamicMock<FtpWebRequest>();
         SetupResult.For(operationRequest.FtpRequest).Return(ftpRequest);
         SetupResult.For(operationRequest.Request).Return(ftpRequest);

         _mocks.ReplayAll();

         _net.FtpCheckConnection(operation, String.Empty, String.Empty, FtpType.Passive);

         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, operationRequest.CachePolicy.Level);
         Assert.AreEqual(false, operationRequest.KeepAlive);
         Assert.AreEqual(5000, operationRequest.Timeout);
         Assert.AreEqual(true, operationRequest.UsePassive);

         _mocks.VerifyAll();
      }

      [Test]
      public void HttpCheckConnection()
      {
         var operation = _mocks.DynamicMock<IWebOperation>();
         Expect.Call(operation.CheckConnection);

         var operationRequest = _mocks.Stub<IWebOperationRequest>();
         SetupResult.For(operation.OperationRequest).Return(operationRequest);

         var request = _mocks.DynamicMock<WebRequest>();
         SetupResult.For(operationRequest.Request).Return(request);

         _mocks.ReplayAll();

         _net.HttpCheckConnection(operation, String.Empty, String.Empty);

         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, operationRequest.CachePolicy.Level);
         Assert.AreEqual(5000, operationRequest.Timeout);

         _mocks.VerifyAll();
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