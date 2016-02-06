
using System;

namespace HFM.Client
{
   public interface IConnection : IDisposable
   {
      #region Properties

      bool Connected { get; }

      int ConnectTimeout { get; set; }

      int SendBufferSize { get; set; }

      int ReceiveBufferSize { get; set; }

      #endregion

      #region Events

      event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;
      event EventHandler<DataEventArgs> DataReceived;
      event EventHandler<DataEventArgs> DataSent;
      event EventHandler<StatusMessageEventArgs> StatusMessage;

      #endregion

      #region Methods

      void Connect(string host, int port, string password);

      void Close();

      void SendCommand(string command);

      #endregion
   }
}
