
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace HFM.Client
{
   public class Connection : IDisposable
   {
      private const int InternalBufferSize = 1024;
      private const int SocketBufferSize = 8196;
      private const int DefaultTimeoutLength = 2000;
      private const int DefaultSocketTimerLength = 100;

      private TcpClient _tcpClient;
      private NetworkStream _stream;
      private readonly StringBuilder _readBuffer;
      private readonly Timer _timer;

      private static readonly object BufferLock = new object();

      #region TcpClient Properties

      public bool Connected
      {
         get { return _tcpClient.Client == null ? false : _tcpClient.Connected; }
      }

      public bool DataAvailable
      {
         get { return _readBuffer.Length != 0; }
      }

      public int ConnectTimeout { get; set; }

      public int SendTimeout { get; set; }

      public int SendBufferSize { get; set; }

      public int ReceiveTimeout { get; set; }

      public int ReceiveBufferSize { get; set; }

      #endregion

      public Connection()
      {
         ConnectTimeout = DefaultTimeoutLength;
         SendTimeout = DefaultTimeoutLength;
         SendBufferSize = SocketBufferSize;
         ReceiveTimeout = DefaultTimeoutLength;
         ReceiveBufferSize = SocketBufferSize;
         _tcpClient = CreateClient();
         _readBuffer = new StringBuilder();
         _timer = new Timer(DefaultSocketTimerLength);
         _timer.Elapsed += SocketTimerElapsed;
      }

      private void SocketTimerElapsed(object sender, ElapsedEventArgs e)
      {
 	      if (Connected)
 	      {
 	         Update();
 	      }
         else
 	      {
 	         _timer.Stop();
 	      }
      }

      private TcpClient CreateClient()
      {
         return new TcpClient
                {
                   SendTimeout = SendTimeout,
                   SendBufferSize = SendBufferSize,
                   ReceiveTimeout = ReceiveTimeout,
                   ReceiveBufferSize = ReceiveBufferSize
                };
      }

      public void Connect(string hostname, int port)
      {
         if (_tcpClient.Client == null)
         {
            _tcpClient = CreateClient();
         }

         IAsyncResult ar = _tcpClient.BeginConnect(hostname, port, null, null);
         System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
         try
         {
            if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(ConnectTimeout), false))
            {
               _tcpClient.Close();
               throw new TimeoutException("Client connection has timed out.");
            }

            _tcpClient.EndConnect(ar);
            // start listening for messages
            // from the network stream
            _timer.Start();
         }
         finally
         {
            wh.Close();
         } 
      }

      public void Close()
      {
         _tcpClient.Close();
      }

      public void SendCommand(string command)
      {
         if (!Connected) throw new InvalidOperationException("Client is not connected.");

         if (!command.EndsWith("\n"))
         {
            command += "\n";
         }
         if (_stream == null)
         {
            _stream = _tcpClient.GetStream();
         }
         var buffer = Encoding.ASCII.GetBytes(command);
         //_stream.Write(buffer, 0, buffer.Length);
         _stream.BeginWrite(buffer, 0, buffer.Length, null, null);
      }

      protected virtual void Update()
      {
         //if (!Connected) throw new InvalidOperationException("Client is not connected.");
         Debug.Assert(Connected);

         if (_stream == null)
         {
            _stream = _tcpClient.GetStream();
         }
         var buffer = new byte[InternalBufferSize];

         //int bytesRead = _stream.Read(buffer, 0, buffer.Length);
         //while (bytesRead != 0)
         //{
         //   _readBuffer.Append(Encoding.ASCII.GetString(buffer));
         //   bytesRead = _stream.Read(buffer, 0, buffer.Length);
         //}

         lock (BufferLock)
         {
            while (_stream.DataAvailable)
            {
               _stream.Read(buffer, 0, buffer.Length);
               _readBuffer.Append(Encoding.ASCII.GetString(buffer));
            }
         }
      }

      public string GetBuffer(bool clear)
      {
         lock (BufferLock)
         {
            string value = _readBuffer.ToString();
            if (clear) _readBuffer.Clear();
            return value;
         }
      }

      public void ClearBuffer()
      {
         lock (BufferLock)
         {
            _readBuffer.Clear();
         }
      }

      #region IDisposable Members

      private bool _disposed;

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
         if (!_disposed)
         {
            if (disposing)
            {
               Close();
            }
         }

         _disposed = true;
      }

      ~Connection()
      {
         Dispose(false);
      }

      #endregion
   }
}
