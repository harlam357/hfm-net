
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
      private Connection _connection;
      private ITcpClient _tcpClient;

      [SetUp]
      public void Init()
      {
         var tcpClientFactory = MockRepository.GenerateStub<ITcpClientFactory>();
         _tcpClient = MockRepository.GenerateMock<ITcpClient>();
         tcpClientFactory.Stub(x => x.Create()).Return(_tcpClient);

         _connection = new Connection(tcpClientFactory);
      }

      [Test]
      public void TimeAndBufferValueTest1()
      {
         Assert.AreEqual(2000, _connection.ConnectTimeout);
         Assert.AreEqual(100, _connection.ReceiveLoopTime);
         Assert.AreEqual(2000, _connection.SendTimeout);
         Assert.AreEqual(8196, _connection.SendBufferSize);
         Assert.AreEqual(2000, _connection.ReceiveTimeout);
         Assert.AreEqual(8196, _connection.ReceiveBufferSize);
      }

      [Test]
      public void TimeAndBufferValueTest2()
      {
         _connection.ConnectTimeout = 3000;
         Assert.AreEqual(3000, _connection.ConnectTimeout);
         _connection.ReceiveLoopTime = 1500;
         Assert.AreEqual(1500, _connection.ReceiveLoopTime);

         _tcpClient.Expect(x => x.SendTimeout = 3000);
         _tcpClient.Expect(x => x.SendBufferSize = 2048);
         _tcpClient.Expect(x => x.ReceiveTimeout = 3000);
         _tcpClient.Expect(x => x.ReceiveBufferSize = 2048);

         _connection.SendTimeout = 3000;
         Assert.AreEqual(3000, _connection.SendTimeout);
         _connection.SendBufferSize = 2048;
         Assert.AreEqual(2048, _connection.SendBufferSize);
         _connection.ReceiveTimeout = 3000;
         Assert.AreEqual(3000, _connection.ReceiveTimeout);
         _connection.ReceiveBufferSize = 2048;
         Assert.AreEqual(2048, _connection.ReceiveBufferSize);

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void ConnectedTest1()
      {
         Assert.IsFalse(_connection.Connected);
      }

      [Test]
      public void ConnectedTest2()
      {
         _tcpClient.Expect(x => x.Client).Return(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
         _tcpClient.Expect(x => x.Connected).Return(false);
         Assert.IsFalse(_connection.Connected);

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void ConnectedTest3()
      {
         _tcpClient.Expect(x => x.Client).Return(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
         _tcpClient.Expect(x => x.Connected).Return(true);
         Assert.IsTrue(_connection.Connected);

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void ConnectTest1()
      {
         var asyncResult = MockRepository.GenerateStub<IAsyncResult>();
         _tcpClient.Expect(x => x.BeginConnect("server", 10000, null, null)).Return(asyncResult);
         asyncResult.Stub(x => x.AsyncWaitHandle).Return(new EventWaitHandle(true, EventResetMode.ManualReset));
         _tcpClient.Expect(x => x.EndConnect(asyncResult));

         // set to 10 seconds so the update loop never gets a chance to fire
         _connection.ReceiveLoopTime = 10000;
         _connection.Connect("server", 10000);

         Assert.IsTrue(_connection.UpdateEnabled);

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void ConnectTest2()
      {
         var asyncResult = MockRepository.GenerateStub<IAsyncResult>();
         _tcpClient.Expect(x => x.BeginConnect("server", 10000, null, null)).Return(asyncResult);
         asyncResult.Stub(x => x.AsyncWaitHandle).Return(new EventWaitHandle(false, EventResetMode.ManualReset));
         _tcpClient.Expect(x => x.Close());

         // set to 1/100 of a second so the test doesn't take long
         _connection.ConnectTimeout = 10;
         try
         {
            _connection.Connect("server", 10000);
            Assert.Fail("Connection attempt did not timeout as expected");
         }
         catch (TimeoutException)
         { }
         Assert.IsFalse(_connection.UpdateEnabled);

         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      public void CloseTest()
      {
         _tcpClient.Expect(x => x.Close());
         _connection.Close();
         _tcpClient.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException(typeof(InvalidOperationException))]
      public void SendCommandTest1()
      {
         _connection.SendCommand("command");
      }

      [Test]
      public void SendCommandTest2()
      {
         _tcpClient.Stub(x => x.Client).Return(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
         _tcpClient.Stub(x => x.Connected).Return(true);
         var stream = MockRepository.GenerateMock<INetworkStream>();
         _tcpClient.Expect(x => x.GetStream()).Return(stream);
         var buffer = Encoding.ASCII.GetBytes("command\n");
         stream.Expect(x => x.BeginWrite(buffer, 0, buffer.Length, null, null)).Return(null);
         _connection.SendCommand("command");

         _tcpClient.VerifyAllExpectations();
         stream.VerifyAllExpectations();
      }

      [Test]
      public void SocketTimerElapsedTest()
      {
         _tcpClient.Stub(x => x.Client).Return(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
         _tcpClient.Stub(x => x.Connected).Return(true);
         var stream = MockRepository.GenerateMock<INetworkStream>();
         _tcpClient.Expect(x => x.GetStream()).Return(stream);
         stream.Expect(x => x.DataAvailable).Return(true);
         stream.Expect(x => x.DataAvailable).Return(false);
         var buffer = _connection.InternalBuffer;
         stream.Expect(x => x.Read(buffer, 0, buffer.Length)).Do(new Func<byte[], int, int, int>(FillBuffer));

         _connection.SocketTimerElapsed(null, null);

         Assert.IsTrue(_connection.DataAvailable);
         string connectionBuffer = _connection.GetBuffer(false);
         Assert.IsFalse(String.IsNullOrEmpty(connectionBuffer));
         Assert.IsTrue(_connection.DataAvailable);
         connectionBuffer = _connection.GetBuffer();
         Assert.IsFalse(String.IsNullOrEmpty(connectionBuffer));
         Assert.IsFalse(_connection.DataAvailable);

         _tcpClient.VerifyAllExpectations();
         stream.VerifyAllExpectations();
      }

      private static int FillBuffer(byte[] buffer, int offset, int size)
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt");
         var messageBytes = Encoding.ASCII.GetBytes(message);

         for (int i = 0; i < messageBytes.Length; i++)
         {
            buffer[i] = messageBytes[i];
         }
         return messageBytes.Length;
      }
   }
}
