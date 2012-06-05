/*
 * HFM.NET - FahClient Class Tests
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
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Client.DataTypes;
using HFM.Client.Tests.DataTypes;
using HFM.Core.DataTypes;

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
            Assert.AreEqual(0, heartbeat.Errors.Count());
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
            Assert.AreEqual(0, info.Errors.Count());
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
            Assert.AreEqual(0, options.Errors.Count());
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

      [Test]
      public void GetSimulationInfoTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithSimulationInfoData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.SimulationInfo, e.Key);
            Assert.AreEqual(typeof(SimulationInfo), e.DataType);
            var simulationInfo = fahClient.GetMessage<SimulationInfo>();
            Assert.IsNotNull(simulationInfo);
            Assert.AreEqual(0, simulationInfo.Errors.Count());
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithSimulationInfoData(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\simulation-info.txt", buffer, buffer.Length * 0);
      }

      [Test]
      public void GetSlotCollectionTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithSlotsData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.SlotInfo, e.Key);
            Assert.AreEqual(typeof(SlotCollection), e.DataType);
            var slotCollection = fahClient.GetMessage<SlotCollection>();
            Assert.IsNotNull(slotCollection);
            Assert.AreEqual(0, slotCollection.Errors.Count());
            Assert.AreEqual(1, slotCollection.Count);
            Assert.AreEqual(0, slotCollection[0].Errors.Count());
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void GetSlotDerivedCollectionTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithSlotsData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.SlotInfo, e.Key);
            Assert.AreEqual(typeof(SlotCollection), e.DataType);
            var slotCollection = fahClient.GetMessage<SlotCollection, SlotDerived>();
            Assert.IsNotNull(slotCollection);
            Assert.AreEqual(0, slotCollection.Errors.Count());
            Assert.AreEqual(1, slotCollection.Count);
            Assert.AreEqual(1, slotCollection[0].Errors.Count());
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithSlotsData(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\slots.txt", buffer, buffer.Length * 0);
      }

      [Test]
      public void GetSlotOptionsTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithSlotOptionsData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.SlotOptions, e.Key);
            Assert.AreEqual(typeof(SlotOptions), e.DataType);
            var slotOptions = fahClient.GetMessage<SlotOptions>();
            Assert.IsNotNull(slotOptions);
            Assert.AreEqual(0, slotOptions.Errors.Count());
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithSlotOptionsData(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\slot-options.txt", buffer, buffer.Length * 0);
      }

      [Test]
      public void GetUnitCollectionTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithUnitsData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.QueueInfo, e.Key);
            Assert.AreEqual(typeof(UnitCollection), e.DataType);
            var unitCollection = fahClient.GetMessage<UnitCollection>();
            Assert.IsNotNull(unitCollection);
            Assert.AreEqual(0, unitCollection.Errors.Count());
            Assert.AreEqual(1, unitCollection.Count);
            Assert.AreEqual(0, unitCollection[0].Errors.Count());
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void GetUnitDerivedCollectionTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = fahClient.InternalBuffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithUnitsData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.QueueInfo, e.Key);
            Assert.AreEqual(typeof(UnitCollection), e.DataType);
            var unitCollection = fahClient.GetMessage<UnitCollection, UnitDerived>();
            Assert.IsNotNull(unitCollection);
            Assert.AreEqual(0, unitCollection.Errors.Count());
            Assert.AreEqual(1, unitCollection.Count);
            Assert.AreEqual(1, unitCollection[0].Errors.Count());
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithUnitsData(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt", buffer, buffer.Length * 0);
      }

      [Test]
      public void GetLogRestartTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = new byte[70 * 1024];
            fahClient.InternalBuffer = buffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithLogRestartData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.LogRestart, e.Key);
            Assert.AreEqual(typeof(LogRestart), e.DataType);
            var logRestart = fahClient.GetMessage<LogRestart>();
            Assert.IsNotNull(logRestart);
            Assert.AreEqual('*', logRestart.Value[0]);
            Assert.IsFalse(logRestart.Value.EndsWith('\"'));
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithLogRestartData(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\log-restart.txt", buffer, buffer.Length * 0);
      }

      [Test]
      public void GetLogUpdateTest()
      {
         using (var fahClient = new FahClient(CreateClientFactory()))
         {
            Connect(fahClient);

            var buffer = new byte[70 * 1024];
            fahClient.InternalBuffer = buffer;
            _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(
               new Func<byte[], int, int, int>(FillBufferWithLogUpdateData));

            MessageUpdatedEventArgs e = null;
            fahClient.MessageUpdated += (sender, args) => e = args;
            fahClient.SocketTimerElapsed(null, null);

            Assert.AreEqual(JsonMessageKey.LogUpdate, e.Key);
            Assert.AreEqual(typeof(LogUpdate), e.DataType);
            var logUpdate = fahClient.GetMessage<LogUpdate>();
            Assert.IsNotNull(logUpdate);
            Assert.AreEqual('s', logUpdate.Value[0]);
            Assert.IsFalse(logUpdate.Value.EndsWith('\"'));
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static int FillBufferWithLogUpdateData(byte[] buffer, int offset, int size)
      {
         return ReadBytes("..\\..\\..\\TestFiles\\Client_v7_1\\log-update_1.txt", buffer, buffer.Length * 0);
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
   }
}
