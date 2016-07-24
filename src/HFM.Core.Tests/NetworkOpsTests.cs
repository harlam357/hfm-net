/*
 * HFM.NET - Network Operations Helper Class Tests
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
using System.Net.Cache;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Core.Net;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class NetworkOpsTests
   {
      private NetworkOps _net;

      [SetUp]
      public void Init()
      {
         var prefs = MockRepository.GenerateStub<IPreferenceSet>();
         prefs.Stub(x => x.Get<bool>(Preference.UseProxy)).Return(false).Repeat.Any();
         _net = new NetworkOps(prefs);
      }

      [Test]
      public void FtpUploadHelper_Test()
      {
         var webOperation = MockRepository.GenerateMock<IWebOperation>();
         webOperation.Expect(x => x.Upload("testpath", -1));
         var webRequest = MockRepository.GenerateStub<IFtpWebRequest>();
         webOperation.Stub(x => x.WebRequest).Return(webRequest);

         _net.FtpUploadHelper(webOperation, "testpath", String.Empty, String.Empty, FtpMode.Passive);

         Assert.AreSame(webOperation, _net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, webRequest.CachePolicy.Level);
         Assert.AreEqual(true, webRequest.UsePassive);

         webOperation.VerifyAllExpectations();
      }

      [Test]
      public void FtpDownloadHelper_Test()
      {
         var webOperation = MockRepository.GenerateMock<IWebOperation>();
         webOperation.Expect(x => x.Download("testpath"));
         var webRequest = MockRepository.GenerateStub<IFtpWebRequest>();
         webOperation.Stub(x => x.WebRequest).Return(webRequest);

         _net.FtpDownloadHelper(webOperation, "testpath", String.Empty, String.Empty, FtpMode.Active);

         Assert.AreSame(webOperation, _net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, webRequest.CachePolicy.Level);
         Assert.AreEqual(false, webRequest.UsePassive);

         webOperation.VerifyAllExpectations();
      }

      [Test]
      public void GetFtpDownloadLength_Test()
      {
         var webOperation = MockRepository.GenerateMock<IWebOperation>();
         webOperation.Expect(x => x.GetDownloadLength()).Return(100);
         var webRequest = MockRepository.GenerateStub<IFtpWebRequest>();
         webOperation.Stub(x => x.WebRequest).Return(webRequest);

         long length = _net.GetFtpDownloadLength(webOperation, String.Empty, String.Empty, FtpMode.Active);
         Assert.AreEqual(100, length);

         Assert.AreSame(webOperation, _net.FtpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, webRequest.CachePolicy.Level);
         Assert.AreEqual(false, webRequest.UsePassive);

         webOperation.VerifyAllExpectations();
      }

      [Test]
      public void HttpDownloadHelper_Test()
      {
         var webOperation = MockRepository.GenerateMock<IWebOperation>();
         webOperation.Expect(x => x.Download("testpath"));
         var webRequest = MockRepository.GenerateStub<IWebRequest>();
         webOperation.Stub(x => x.WebRequest).Return(webRequest);

         _net.HttpDownloadHelper(webOperation, "testpath", String.Empty, String.Empty);

         Assert.AreSame(webOperation, _net.HttpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, webRequest.CachePolicy.Level);

         webOperation.VerifyAllExpectations();
      }

      [Test]
      public void GetHttpDownloadLength_Test()
      {
         var webOperation = MockRepository.GenerateMock<IWebOperation>();
         webOperation.Expect(x => x.GetDownloadLength()).Return(100);
         var webRequest = MockRepository.GenerateStub<IWebRequest>();
         webOperation.Stub(x => x.WebRequest).Return(webRequest);

         long length = _net.GetHttpDownloadLength(webOperation, String.Empty, String.Empty);
         Assert.AreEqual(100, length);

         Assert.AreSame(webOperation, _net.HttpOperation);
         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, webRequest.CachePolicy.Level);

         webOperation.VerifyAllExpectations();
      }

      [Test]
      public void FtpCheckConnection_Test()
      {
         var webOperation = MockRepository.GenerateMock<IWebOperation>();
         webOperation.Expect(x => x.CheckConnection());
         var webRequest = MockRepository.GenerateStub<IFtpWebRequest>();
         webOperation.Stub(x => x.WebRequest).Return(webRequest);

         _net.FtpCheckConnection(webOperation, String.Empty, String.Empty, FtpMode.Passive);

         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, webRequest.CachePolicy.Level);
         Assert.AreEqual(false, webRequest.KeepAlive);
         Assert.AreEqual(5000, webRequest.Timeout);
         Assert.AreEqual(true, webRequest.UsePassive);

         webOperation.VerifyAllExpectations();
      }

      [Test]
      public void HttpCheckConnection_Test()
      {
         var webOperation = MockRepository.GenerateMock<IWebOperation>();
         webOperation.Expect(x => x.CheckConnection());
         var webRequest = MockRepository.GenerateStub<IWebRequest>();
         webOperation.Stub(x => x.WebRequest).Return(webRequest);

         _net.HttpCheckConnection(webOperation, String.Empty, String.Empty);

         Assert.AreEqual(RequestCacheLevel.NoCacheNoStore, webRequest.CachePolicy.Level);
         Assert.AreEqual(5000, webRequest.Timeout);

         webOperation.VerifyAllExpectations();
      }
   }
}
