/*
 * HFM.NET - FahClient Class Tests
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
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Client.DataTypes;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class FahClientTests
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
      public void GetHeartbeatTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithHeartbeatData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.Heartbeat, e.Key);
            Assert.AreEqual(typeof(Heartbeat), e.DataType);
            var heartbeat = fahClient.GetMessage<Heartbeat>();
            Assert.IsNotNull(heartbeat);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithHeartbeatData(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\heartbeat.txt", buffer, buffer.Length * 0);
      }

      [Test]
      public void GetInfoTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithClientInfoData1));
            _stream.Expect(x => x.DataAvailable).Return(true);
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithClientInfoData2));
            _stream.Expect(x => x.DataAvailable).Return(true);
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithClientInfoData3));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.Info, e.Key);
            Assert.AreEqual(typeof(Info), e.DataType);
            var info = fahClient.GetMessage<Info>();
            Assert.IsNotNull(info);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithClientInfoData1(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt", buffer, buffer.Length * 0);
      }

      private static int FillBufferWithClientInfoData2(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt", buffer, buffer.Length * 1);
      }

      private static int FillBufferWithClientInfoData3(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt", buffer, buffer.Length * 2);
      }

      [Test]
      public void GetOptionsTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithOptionsData1));
            _stream.Expect(x => x.DataAvailable).Return(true);
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithOptionsData2));
            _stream.Expect(x => x.DataAvailable).Return(true);
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithOptionsData3));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.Options, e.Key);
            Assert.AreEqual(typeof(Options), e.DataType);
            var options = fahClient.GetMessage<Options>();
            Assert.IsNotNull(options);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithOptionsData1(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\options.txt", buffer, buffer.Length * 0);
      }

      private static int FillBufferWithOptionsData2(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\options.txt", buffer, buffer.Length * 1);
      }

      private static int FillBufferWithOptionsData3(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\options.txt", buffer, buffer.Length * 2);
      }

      private static int ReadBytes(string filePath, byte[] buffer, int startingByte)
      {
         string message = File.ReadAllText(filePath);
         var messageBytes = Encoding.ASCII.GetBytes(message);

         int bytesRead = 0;
         for (int i = 0; i < buffer.Length; i++)
         {
            int messageIndex = i + startingByte;
            if (messageIndex >= messageBytes.Length)
            {
               break;
            }
            buffer[i] = messageBytes[messageIndex];
            bytesRead++;
         }
         return bytesRead;
      }

      [Test]
      public void GetSlotCollectionTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithSlotsData1));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.SlotInfo, e.Key);
            Assert.AreEqual(typeof(SlotCollection), e.DataType);
            var slotCollection = fahClient.GetMessage<SlotCollection>();
            Assert.IsNotNull(slotCollection);
            Assert.AreEqual(1, slotCollection.Count);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithSlotsData1(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\slots.txt", buffer, buffer.Length * 0);
      }

      [Test]
      public void GetUnitCollectionTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithUnitsData1));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.QueueInfo, e.Key);
            Assert.AreEqual(typeof(UnitCollection), e.DataType);
            var unitCollection = fahClient.GetMessage<UnitCollection>();
            Assert.IsNotNull(unitCollection);
            Assert.AreEqual(1, unitCollection.Count);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithUnitsData1(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt", buffer, buffer.Length * 0);
      }
   }
}
