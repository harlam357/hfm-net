/*
 * HFM.NET - Client Connection Class
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
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace HFM.Client
{
   public class Connection : IDisposable
   {
      #region Constants

      /// <summary>
      /// Internal Network Stream Buffer Size
      /// </summary>
      private const int InternalBufferSize = 1024;
      /// <summary>
      /// Default TcpClient Send and Receive Buffer Size
      /// </summary>
      private const int DefaultSocketBufferSize = 8196;
      /// <summary>
      /// Default Connection, Send, and Receive Timeout Length
      /// </summary>
      private const int DefaultTimeoutLength = 2000;
      /// <summary>
      /// Default Socket Receive Timer Length
      /// </summary>
      private const int DefaultSocketTimerLength = 100;

      #endregion

      #region Fields

      private ITcpClient _tcpClient;
      private INetworkStream _stream;
      private readonly ITcpClientFactory _tcpClientFactory;
      private readonly byte[] _internalBuffer;
      private readonly StringBuilder _readBuffer;
      private readonly Timer _timer;

      private static readonly object BufferLock = new object();

      #endregion

      #region Properties

      internal byte[] InternalBuffer
      {
         get { return _internalBuffer; }
      }

      /// <summary>
      /// Gets a value indicating whether the Connection is connected to a remote host.
      /// </summary>
      public bool Connected
      {
         get { return _tcpClient.Client == null ? false : _tcpClient.Connected; }
      }

      /// <summary>
      /// Gets a value that indicates whether data is available to be read.
      /// </summary>
      public bool DataAvailable
      {
         get { return _readBuffer.Length != 0; }
      }

      /// <summary>
      /// Gets or sets a value indicating whether the Connection should process updates.
      /// </summary>
      public bool UpdateEnabled
      {
         get { return _timer.Enabled; }
         set { _timer.Enabled = value; }
      }

      /// <summary>
      /// Length of time to wait for Connection response (default - 2 seconds).
      /// </summary>
      public int ConnectTimeout { get; set; }

      /// <summary>
      /// Length of time between each network stream read (default - 100ms).
      /// </summary>
      public int ReceiveLoopTime { get; set; }

      private int _sendTimeout = DefaultTimeoutLength;

      /// <summary>
      /// Length of time to wait for a command to be sent (default - 2 seconds).
      /// </summary>
      public int SendTimeout
      {
         get { return _sendTimeout; }
         set
         {
            _tcpClient.SendTimeout = value;
            _sendTimeout = value;
         }
      }

      private int _sendBufferSize = DefaultSocketBufferSize;

      /// <summary>
      /// Size of outgoing data buffer (default - 8k).
      /// </summary>
      public int SendBufferSize
      {
         get { return _sendBufferSize; }
         set
         {
            _tcpClient.SendBufferSize = value;
            _sendBufferSize = value;
         }
      }

      private int _receiveTimeout = DefaultTimeoutLength;

      /// <summary>
      /// Length of time to wait for a response to be received (default - 2 seconds).
      /// </summary>
      public int ReceiveTimeout
      {
         get { return _receiveTimeout; }
         set
         {
            _tcpClient.ReceiveTimeout = value;
            _receiveTimeout = value;
         }
      }

      private int _receiveBufferSize = DefaultSocketBufferSize;

      /// <summary>
      /// Size of incoming data buffer (default - 8k).
      /// </summary>
      public int ReceiveBufferSize
      {
         get { return _receiveBufferSize; }
         set
         {
            _tcpClient.ReceiveBufferSize = value;
            _receiveBufferSize = value;
         }
      }

      #endregion

      #region Constructor

      /// <summary>
      /// Create a Server Connection.
      /// </summary>
      public Connection()
         : this(new TcpClientFactory())
      {
         
      }

      /// <summary>
      /// Create a Server Connection.
      /// </summary>
      internal Connection(ITcpClientFactory tcpClientFactory)
      {
         ConnectTimeout = DefaultTimeoutLength;
         ReceiveLoopTime = DefaultSocketTimerLength;

         _tcpClientFactory = tcpClientFactory;
         _tcpClient = CreateClient();
         _internalBuffer = new byte[InternalBufferSize];
         _readBuffer = new StringBuilder();
         _timer = new Timer(ReceiveLoopTime);
         _timer.Elapsed += SocketTimerElapsed;
      }

      #endregion

      #region Methods
      
      /// <summary>
      /// Connect to a Server.
      /// </summary>
      /// <param name="hostname">Hostname or IP Address</param>
      /// <param name="port">TCP Port</param>
      public void Connect(string hostname, int port)
      {
         if (_tcpClient.Client == null)
         {
            _tcpClient = CreateClient();
         }

         IAsyncResult ar = _tcpClient.BeginConnect(hostname, port, null, null);
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
            ar.AsyncWaitHandle.Close();
         } 
      }

      private ITcpClient CreateClient()
      {
         var tcpClient = _tcpClientFactory.Create();
         tcpClient.SendTimeout = SendTimeout;
         tcpClient.SendBufferSize = SendBufferSize;
         tcpClient.ReceiveTimeout = ReceiveTimeout;
         tcpClient.ReceiveBufferSize = ReceiveBufferSize;
         return tcpClient;
      }

      /// <summary>
      /// Close the Connection to the Server.
      /// </summary>
      public void Close()
      {
         // stop the timer
         _timer.Stop();
         // close the actual connection
         _tcpClient.Close();
         // remove reference to the network stream
         _stream = null;
      }

      /// <summary>
      /// Send a Command to the Server.
      /// </summary>
      /// <remarks>Callers should make sure they're connected by checking the Connected property.</remarks>
      public void SendCommand(string command)
      {
         // check connection status, callers should make sure they're connected first
         if (!Connected) throw new InvalidOperationException("Client is not connected.");

         if (!command.EndsWith("\n", StringComparison.Ordinal))
         {
            command += "\n";
         }
         // get the network stream
         if (_stream == null)
         {
            _stream = _tcpClient.GetStream();
         }
         var buffer = Encoding.ASCII.GetBytes(command);
         //_stream.Write(buffer, 0, buffer.Length);
         _stream.BeginWrite(buffer, 0, buffer.Length, null, null);
      }

      internal void SocketTimerElapsed(object sender, ElapsedEventArgs e)
      {
         if (Connected)
         {
            try
            {
               Update();
            }
            catch (Exception ex)
            {
               //TODO: log it!!!
               Close();
            }
         }
         else
         {
            Close();
         }
      }

      /// <summary>
      /// Update the Data Buffer.
      /// </summary>
      protected virtual void Update()
      {
         // this method should only be called from
         // SocketTimerElapsed() and that method
         // makes sure the connection is open first
         Debug.Assert(Connected);
         // get the network stream
         if (_stream == null)
         {
            _stream = _tcpClient.GetStream();
         }

         // lock so we're not append to and reading 
         // from the buffer at the same time
         lock (BufferLock)
         {
            //int bytesRead = _stream.Read(buffer, 0, buffer.Length);
            //while (bytesRead != 0)
            //{
            //   _readBuffer.Append(Encoding.ASCII.GetString(buffer));
            //   bytesRead = _stream.Read(buffer, 0, buffer.Length);
            //}

            // this seems to work better than the method above
            while (_stream.DataAvailable)
            {
               _stream.Read(_internalBuffer, 0, _internalBuffer.Length);
               _readBuffer.Append(Encoding.ASCII.GetString(_internalBuffer));
            }
         }
      }

      /// <summary>
      /// Get the buffer value.
      /// </summary>
      /// <remarks>Automatically clears the Connection's buffer.</remarks>
      public string GetBuffer()
      {
         return GetBuffer(true);
      }

      /// <summary>
      /// Get the buffer value.
      /// </summary>
      /// <param name="clear">True to clear the Connection's buffer.</param>
      public string GetBuffer(bool clear)
      {
         // lock so we're not append to and reading 
         // from the buffer at the same time
         lock (BufferLock)
         {
            string value = _readBuffer.ToString();
            if (clear) _readBuffer.Clear();
            return value;
         }
      }

      /// <summary>
      /// Clear the buffer value.
      /// </summary>
      public void ClearBuffer()
      {
         // lock so we're not append to and reading 
         // from the buffer at the same time
         lock (BufferLock)
         {
            _readBuffer.Clear();
         }
      }

      #endregion

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
