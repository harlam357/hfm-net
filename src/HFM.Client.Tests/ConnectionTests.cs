/*
 * HFM.NET - Client Connection Class Tests
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Net.Sockets;
using System.Text;
using System.Threading;

using NUnit.Framework;
using Rhino.Mocks;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class ConnectionTests
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

      [Test]
      public void Connection_TimeAndBufferValue_Test1()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Assert.AreEqual(5000, connection.ConnectTimeout);
            Assert.AreEqual(1024 * 8, connection.SendBufferSize);
            Assert.AreEqual(1024 * 8, connection.ReceiveBufferSize);
         }
      }

      [Test]
      public void Connection_TimeAndBufferValue_Test2()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            connection.ConnectTimeout = 3000;
            Assert.AreEqual(3000, connection.ConnectTimeout);

            _tcpClient.Expect(x => x.SendBufferSize = 2048);
            _tcpClient.Expect(x => x.ReceiveBufferSize = 2048);

            connection.SendBufferSize = 2048;
            Assert.AreEqual(2048, connection.SendBufferSize);
            connection.ReceiveBufferSize = 2048;
            Assert.AreEqual(2048, connection.ReceiveBufferSize);
         }

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Connected_Test1()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Assert.IsFalse(connection.Connected);
         }
      }

      [Test]
      public void Connection_Connected_Test2()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            _tcpClient.Expect(x => x.Client).Return(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            _tcpClient.Expect(x => x.Connected).Return(false);
            Assert.IsFalse(connection.Connected);
         }

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Connected_Test3()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            _tcpClient.Expect(x => x.Client).Return(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            _tcpClient.Expect(x => x.Connected).Return(true);
            Assert.IsTrue(connection.Connected);
         }

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Connect_Test1()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            SetupSuccessfulConnectionExpectations(_tcpClient, _stream);

            bool connectedChangedFired = false;
            bool statusMessageFired = false;
            connection.ConnectedChanged += (sender, args) => connectedChangedFired = true;
            connection.StatusMessage += (sender, args) => statusMessageFired = true;
            connection.Connect("server", 10000, "password");

            Assert.IsTrue(connectedChangedFired);
            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Connect_NoPassword_Test1()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            SetupSuccessfulConnectionExpectations(_tcpClient, _stream, false);

            bool connectedChangedFired = false;
            bool statusMessageFired = false;
            connection.ConnectedChanged += (sender, args) => connectedChangedFired = true;
            connection.StatusMessage += (sender, args) => statusMessageFired = true;
            connection.Connect("server", 10000);

            Assert.IsTrue(connectedChangedFired);
            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      private static void SetupSuccessfulConnectionExpectations(ITcpClient tcpClient, INetworkStream stream, bool withPassword = true)
      {
         // client not connected
         tcpClient.Expect(x => x.Client).PropertyBehavior();

         // setup connect expectations
         var asyncResult = MockRepository.GenerateStub<IAsyncResult>();
         tcpClient.Expect(x => x.BeginConnect("server", 10000, null, null)).Return(asyncResult);
         asyncResult.Stub(x => x.AsyncWaitHandle).Return(new EventWaitHandle(true, EventResetMode.ManualReset));
         tcpClient.Expect(x => x.EndConnect(asyncResult)).Do(new Action<IAsyncResult>(ar =>
         {
            // setup Connected property expectations
            tcpClient.Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpClient.Expect(x => x.Connected).Return(true).Repeat.AtLeastOnce();
         }));
         tcpClient.Expect(x => x.GetStream()).Return(stream);

         if (withPassword)
         {
            // setup SendCommand() expectation
            stream.Expect(x => x.BeginWrite(null, 0, 0, null, null)).IgnoreArguments().Do(
               new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(TestUtilities.DoBeginWrite)).Repeat.AtLeastOnce();
         }
      }

      private void Connect(Connection connection)
      {
         Connect(connection, _tcpClient, _stream);
      }

      internal static void Connect(Connection connection, ITcpClient tcpClient, INetworkStream stream)
      {
         SetupSuccessfulConnectionExpectations(tcpClient, stream);
         connection.Connect("server", 10000, "password");
      }

      [Test]
      public void Connection_Connect_Test2()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            // client not connected
            _tcpClient.Expect(x => x.Client).Return(null);

            // setup connect expectations
            var asyncResult = MockRepository.GenerateStub<IAsyncResult>();
            _tcpClient.Expect(x => x.BeginConnect("server", 10000, null, null)).Return(asyncResult);
            asyncResult.Stub(x => x.AsyncWaitHandle).Return(new EventWaitHandle(false, EventResetMode.ManualReset));
            _tcpClient.Expect(x => x.Close()).Do(new Action(() => _tcpClient.Client = null));

            // set to 1/100 of a second so the test doesn't take long
            connection.ConnectTimeout = 10;
            try
            {
               connection.Connect("server", 10000, String.Empty);
               Assert.Fail("Connection attempt did not timeout as expected");
            }
            catch (TimeoutException)
            { }
         }

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Connect_Test3()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Connect(connection);

            Assert.IsTrue(connection.Connected);
            // cannot connect again if a connection is already established
            Assert.Throws<InvalidOperationException>(() => connection.Connect("server", 10000, "password"));
         }
      }

      [Test]
      public void Connection_Connect_Test4()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Assert.Throws<ArgumentNullException>(() => connection.Connect(null, 10000, String.Empty));
         }
      }

      [Test]
      public void Connection_Connect_Test5()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Assert.Throws<ArgumentNullException>(() => connection.Connect("server", 10000, null));
         }
      }

      [Test]
      public void Connection_Close_Test()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Connect(connection);

            _stream.Expect(x => x.Dispose());
            _tcpClient.Expect(x => x.Close()).Do(new Action(() => _tcpClient.Client = null));

            bool statusMessageFired = false;
            connection.StatusMessage += (sender, args) => statusMessageFired = true;
            connection.Close();

            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_SendCommand_Test1()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Assert.Throws<InvalidOperationException>(() => connection.SendCommand("command"));
         }
      }

      [Test]
      public void Connection_SendCommand_Test2()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Connect(connection);

            var mre = new ManualResetEvent(false);
            bool dataSentFired = false;
            bool statusMessageFired = false;

            connection.DataSent += (sender, args) =>
            {
               dataSentFired = true;
               mre.Set();
            };
            connection.StatusMessage += (sender, args) => statusMessageFired = true;

            connection.SendCommand("command");
            mre.WaitOne();

            Assert.IsTrue(dataSentFired);
            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_SendCommand_Test3()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Connect(connection);

            var mre = new ManualResetEvent(false);
            bool dataSentFired = false;
            bool statusMessageFired = false;
            bool connectedChangedFired = false;

            connection.DataSent += (sender, args) => dataSentFired = true;
            connection.StatusMessage += (sender, args) =>
            {
               statusMessageFired = true;
               mre.Set();
            };
            connection.ConnectedChanged += (sender, args) => connectedChangedFired = true;

            _stream.Expect(x => x.EndWrite(null)).IgnoreArguments().Throw(new IOException("Write failed."));
            _tcpClient.Expect(x => x.Close()).Do(new Action(() => _tcpClient.Client = null));

            connection.SendCommand("command");
            mre.WaitOne();

            Assert.IsFalse(dataSentFired);
            Assert.IsTrue(statusMessageFired);
            Assert.IsTrue(connectedChangedFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      #region SendCommand - Null, Empty, & Whitespace Tests

      [Test]
      public void Connection_SendCommandNull_Test()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Connect(connection);

            bool statusMessageFired = false;
            connection.StatusMessage += (sender, args) => statusMessageFired = true;
            connection.SendCommand(null);

            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_SendCommandEmpty_Test()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Connect(connection);

            bool statusMessageFired = false;
            connection.StatusMessage += (sender, args) => statusMessageFired = true;
            connection.SendCommand(String.Empty);

            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_SendCommandWhitespace_Test()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            Connect(connection);

            bool statusMessageFired = false;
            connection.StatusMessage += (sender, args) => statusMessageFired = true;
            connection.SendCommand("   ");

            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      #endregion

      [Test]
      public void Connection_Update_Test1()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            string dataReceived = null;
            var mre = new ManualResetEvent(false);
            connection.DataReceived += (sender, args) =>
            {
               dataReceived = args.Value;
               mre.Set();
            };

            _stream.Expect(x => x.BeginRead(null, 0, 0, null, null)).IgnoreArguments().Do(
               new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(
                  (buffer, offset, size, callback, state) =>
                     TestUtilities.DoBeginRead(buffer, offset, size, callback, state, TestData.QueueInfo))).Repeat.Once();

            _stream.Expect(x => x.EndRead(null)).IgnoreArguments().Do(
               new Func<IAsyncResult, int>(result => Encoding.ASCII.GetBytes(TestData.QueueInfo).Length));

            Connect(connection);
            mre.WaitOne();

            Assert.AreEqual(TestData.QueueInfo, dataReceived);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Update_Test2()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            string dataReceived = null;
            var mre = new ManualResetEvent(false);
            connection.DataReceived += (sender, args) =>
            {
               dataReceived = args.Value;
               mre.Set();
            };

            _stream.Expect(x => x.BeginRead(null, 0, 0, null, null)).IgnoreArguments().Do(
               new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(
                  (buffer, offset, size, callback, state) =>
                     TestUtilities.DoBeginRead(buffer, offset, size, callback, state, TestData.QueueInfo))).Repeat.Twice();

            _stream.Expect(x => x.EndRead(null)).IgnoreArguments().Do(
               new Func<IAsyncResult, int>(result => Encoding.ASCII.GetBytes(TestData.QueueInfo).Length)).Repeat.Twice();

            _stream.Expect(x => x.DataAvailable).Return(true).Repeat.Once();

            Connect(connection);
            mre.WaitOne();

            Assert.AreEqual(TestData.QueueInfo + TestData.QueueInfo, dataReceived);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Update_Test3()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            bool statusMessageFired = false;
            var mre = new ManualResetEvent(false);
            connection.StatusMessage += (sender, args) => statusMessageFired = true;
            connection.ConnectedChanged += (sender, args) =>
            {
               if (!args.Connected) mre.Set();
            };

            // when an EndRead returns 0 the connection has been lost
            _stream.Expect(x => x.BeginRead(null, 0, 0, null, null)).IgnoreArguments().Do(
               new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(
                  (buffer, offset, size, callback, state) =>
                     TestUtilities.DoBeginRead(buffer, offset, size, callback, state, String.Empty))).Repeat.Once();

            _stream.Expect(x => x.EndRead(null)).IgnoreArguments().Do(
               new Func<IAsyncResult, int>(result => 0));

            // as a result the connection is closed, set expectations
            _stream.Expect(x => x.Dispose());
            _tcpClient.Expect(x => x.Close()).Do(new Action(() => _tcpClient.Client = null));

            Connect(connection);
            mre.WaitOne();

            Assert.IsTrue(statusMessageFired);
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Update_Test4()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            var mre = new ManualResetEvent(false);
            connection.ConnectedChanged += (sender, args) =>
            {
               if (!args.Connected) mre.Set();
            };

            // close the connection manually, the callback will see
            // that _stream is null and exit the update loop
            _stream.Expect(x => x.BeginRead(null, 0, 0, null, null)).IgnoreArguments().Do(
               new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(
                  (buffer, offset, size, callback, state) =>
                  {
                     // ReSharper disable once AccessToDisposedClosure
                     connection.Close();
                     return TestUtilities.DoBeginRead(buffer, offset, size, callback, state, String.Empty);
                  })).Repeat.Once();

            // the connection is closed manually, set expectations
            _stream.Expect(x => x.Dispose());
            _tcpClient.Expect(x => x.Close()).Do(new Action(() => _tcpClient.Client = null));

            Connect(connection);
            mre.WaitOne();
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      [Test]
      public void Connection_Update_Test5()
      {
         using (var connection = new Connection(CreateClientFactory()))
         {
            var mre = new ManualResetEvent(false);
            connection.ConnectedChanged += (sender, args) =>
            {
               if (!args.Connected) mre.Set();
            };

            // when EndRead throws an exception the connection has been lost
            _stream.Expect(x => x.BeginRead(null, 0, 0, null, null)).IgnoreArguments().Do(
               new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(
                  (buffer, offset, size, callback, state) =>
                     TestUtilities.DoBeginRead(buffer, offset, size, callback, state, String.Empty)));

            _stream.Expect(x => x.EndRead(null)).IgnoreArguments().Throw(new IOException());

            // as a result the connection is closed, set expectations
            _stream.Expect(x => x.Dispose());
            _tcpClient.Expect(x => x.Close()).Do(new Action(() => _tcpClient.Client = null));

            Connect(connection);
            mre.WaitOne();
         }

         _tcpClient.VerifyAllExpectations();
         _stream.VerifyAllExpectations();
      }

      //[Test]
      //public void Connection_Update_Test6()
      //{
      //   using (var connection = new Connection(CreateClientFactory()))
      //   {
      //      Connect(connection);
      //
      //      // when we call Close() ourselves then the Read() method called by
      //      // Update() will throw a WSACancelBlockingCall error code as the
      //      // inner exception of an IOException.  in this case Close() does
      //      // not need called again since it was called through the API.
      //      _stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Throw(new IOException(String.Empty, new SocketException(10004)));
      //
      //      connection.SocketTimerElapsed(null, null);
      //   }
      //
      //   _tcpClient.VerifyAllExpectations();
      //   _stream.VerifyAllExpectations();
      //}

   }
}
