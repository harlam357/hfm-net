
using System;
using System.Net;
using System.Net.Sockets;

using HFM.Core;

namespace HFM.Client
{
   public interface ITcpClient : IDisposable
   {
      #region Properties

      int Available { get; }

      Socket Client { get; set; }

      /// <summary>
      /// Gets a value indicating whether the underlying System.Net.Sockets.Socket for a System.Net.Sockets.TcpClient is connected to a remote host.
      /// </summary>
      bool Connected { get; }

      bool ExclusiveAddressUse { get; set; }

      LingerOption LingerState { get; set; }

      bool NoDelay { get; set; }

      int ReceiveBufferSize { get; set; }

      int ReceiveTimeout { get; set; }

      int SendBufferSize { get; set; }

      int SendTimeout { get; set; }

      #endregion

      #region Methods

      IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state);

      IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state);

      void EndConnect(IAsyncResult asyncResult);

      void Connect(string hostname, int port);

      void Connect(IPAddress address, int port);

      void Close();

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      INetworkStream GetStream();

      #endregion
   }

   [CoverageExclude]
   public sealed class TcpClientAdapter : ITcpClient
   {
      private readonly TcpClient _tcpClient;

      public TcpClientAdapter()
      {
         _tcpClient = new TcpClient();
      }

      #region Properties

      public int Available
      {
         get { return _tcpClient.Available; }
      }

      public Socket Client
      {
         get { return _tcpClient.Client; }
         set { _tcpClient.Client = value; }
      }

      /// <summary>
      /// Gets a value indicating whether the underlying System.Net.Sockets.Socket for a System.Net.Sockets.TcpClient is connected to a remote host.
      /// </summary>
      public bool Connected
      {
         get { return _tcpClient.Connected; }
      }

      public bool ExclusiveAddressUse
      {
         get { return _tcpClient.ExclusiveAddressUse; }
         set { _tcpClient.ExclusiveAddressUse = value; }
      }

      public LingerOption LingerState
      {
         get { return _tcpClient.LingerState; }
         set { _tcpClient.LingerState = value; }
      }

      public bool NoDelay
      {
         get { return _tcpClient.NoDelay; }
         set { _tcpClient.NoDelay = value; }
      }

      public int ReceiveBufferSize
      {
         get { return _tcpClient.ReceiveBufferSize; }
         set { _tcpClient.ReceiveBufferSize = value; }
      }

      public int ReceiveTimeout
      {
         get { return _tcpClient.ReceiveTimeout; }
         set { _tcpClient.ReceiveTimeout = value; }
      }

      public int SendBufferSize
      {
         get { return _tcpClient.SendBufferSize; }
         set { _tcpClient.SendBufferSize = value; }
      }

      public int SendTimeout
      {
         get { return _tcpClient.SendTimeout; }
         set { _tcpClient.SendTimeout = value; }
      }

      #endregion

      #region Methods

      public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
      {
         return _tcpClient.BeginConnect(host, port, requestCallback, state);
      }

      public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
      {
         return _tcpClient.BeginConnect(address, port, requestCallback, state);
      }

      public void EndConnect(IAsyncResult asyncResult)
      {
         _tcpClient.EndConnect(asyncResult);
      }

      public void Connect(string hostname, int port)
      {
         _tcpClient.Connect(hostname, port);
      }

      public void Connect(IPAddress address, int port)
      {
         _tcpClient.Connect(address, port);
      }

      public void Close()
      {
         _tcpClient.Close();
      }

      public INetworkStream GetStream()
      {
         return new NetworkStreamAdapter(_tcpClient.GetStream());
      }

      void IDisposable.Dispose()
      {
         ((IDisposable)_tcpClient).Dispose();
      }

      #endregion
   }
}
