/*
 * HFM.NET - MessageCache Class Tests
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
using System.IO;

using NUnit.Framework;
using Rhino.Mocks;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class MessageCacheTests
   {
      private ITcpClient _tcpClient;
      private INetworkStream _stream;

      private ITcpClientFactory CreateClientFactory()
      {
         var tcpClientFactory = MockRepository.GenerateStub<ITcpClientFactory>();
         _tcpClient = MockRepository.GenerateMock<ITcpClient>();
         _stream = MockRepository.GenerateMock<INetworkStream>();
         tcpClientFactory.Stub(x => x.Create()).Return(_tcpClient);
         return tcpClientFactory;
      }

      private void Connect(Connection connection)
      {
         ConnectionTests.Connect(connection, _tcpClient, _stream);
      }

      [Test]
      public void UpdateTest1()
      {
         using (var messageCache = new MessageCache(CreateClientFactory()))
         {
            Connect(messageCache);

            var buffer = messageCache.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(ConnectionTests.FillBufferWithTestData));

            MessageUpdatedEventArgs e = null;
            messageCache.MessageUpdated += (sender, args) => e = args;
            messageCache.SocketTimerElapsed(null, null);

            Assert.AreEqual("units", e.Key);
            Assert.IsNull(e.Type);
            Assert.IsNotNull(messageCache.GetJsonMessage(JsonMessageKey.QueueInfo));
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void GetNextJsonMessageTest1()
      {
         string message = null;
         Assert.IsNull(MessageCache.GetNextJsonMessage(ref message));
      }

      [Test]
      public void GetNextJsonMessageTest2()
      {
         // no PyON header
         string message = String.Empty;
         Assert.IsNull(MessageCache.GetNextJsonMessage(ref message));
      }

      [Test]
      public void GetNextJsonMessageTest3()
      {
         // nothing but PyON header
         string message = "PyON 1 ";
         Assert.IsNull(MessageCache.GetNextJsonMessage(ref message));
      }

      [Test]
      public void GetNextJsonMessageTest4()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\partial-info.txt");
         Assert.IsNull(MessageCache.GetNextJsonMessage(ref message));
      }

      [Test]
      public void GetNextJsonMessageTest5()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt");
         var jsonMessage = MessageCache.GetNextJsonMessage(ref message);
         Assert.AreEqual("info", jsonMessage.Key);
         Assert.GreaterOrEqual(DateTime.UtcNow, jsonMessage.Received);
         Assert.IsNotNull(jsonMessage.Value);
      }
   }
}
