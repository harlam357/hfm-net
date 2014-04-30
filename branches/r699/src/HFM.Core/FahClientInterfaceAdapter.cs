
using System;

using HFM.Client;
using HFM.Client.DataTypes;

namespace HFM.Core
{
   public interface IFahClientInterface : IDisposable
   {
      #region Properties

      bool Connected { get; }

      int ConnectTimeout { get; set; }

      int SendBufferSize { get; set; }

      int ReceiveBufferSize { get; set; }

      double ReceiveLoopTime { get; set; }

      bool UpdateEnabled { get; set; }

      bool DataAvailable { get; }

      #endregion

      #region Events

      event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;
      event EventHandler<DataLengthEventArgs> DataLengthReceived;
      event EventHandler<DataLengthEventArgs> DataLengthSent;
      event EventHandler<MessageUpdatedEventArgs> MessageUpdated;
      event EventHandler<StatusMessageEventArgs> StatusMessage;
      event EventHandler UpdateFinished;

      #endregion

      #region Methods

      void Connect(string host, int port, string password);

      void Close();

      void ClearBuffer();

      void SendCommand(string command);

      JsonMessage GetJsonMessage(string key);

      T GetMessage<T>() where T : TypedMessage, new();

      T GetMessage<T, TCollectionType>() where T : TypedMessageCollection, new() where TCollectionType : ITypedMessageObject, new();

      #endregion
   }

   public sealed class FahClientInterfaceAdapter : IFahClientInterface
   {
      #region Properties

      public bool Connected
      {
         get { return _fahClient.Connected; }
      }

      public int ConnectTimeout
      {
         get { return _fahClient.ConnectTimeout; }
         set { _fahClient.ConnectTimeout = value; }
      }

      public int SendBufferSize
      {
         get { return _fahClient.SendBufferSize; }
         set { _fahClient.SendBufferSize = value; }
      }

      public int ReceiveBufferSize
      {
         get { return _fahClient.ReceiveBufferSize; }
         set { _fahClient.ReceiveBufferSize = value; }
      }

      public double ReceiveLoopTime
      {
         get { return _fahClient.ReceiveLoopTime; }
         set { _fahClient.ReceiveLoopTime = value; }
      }

      public bool UpdateEnabled
      {
         get { return _fahClient.UpdateEnabled; }
         set { _fahClient.UpdateEnabled = value; }
      }

      public bool DataAvailable
      {
         get { return _fahClient.DataAvailable; }
      }

      #endregion

      #region Events

      public event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;
      public event EventHandler<DataLengthEventArgs> DataLengthReceived;
      public event EventHandler<DataLengthEventArgs> DataLengthSent;
      public event EventHandler<MessageUpdatedEventArgs> MessageUpdated;
      public event EventHandler<StatusMessageEventArgs> StatusMessage;
      public event EventHandler UpdateFinished;

      private void OnConnectedChanged(object sender, ConnectedChangedEventArgs e)
      {
         if ((ConnectedChanged != null))
         {
            ConnectedChanged(sender, e);
         }
      }

      private void OnDataLengthReceived(object sender, DataLengthEventArgs e)
      {
         if ((DataLengthReceived != null))
         {
            DataLengthReceived(sender, e);
         }
      }

      private void OnDataLengthSent(object sender, DataLengthEventArgs e)
      {
         if ((DataLengthSent != null))
         {
            DataLengthSent(sender, e);
         }
      }

      private void OnMessageUpdated(object sender, MessageUpdatedEventArgs e)
      {
         if ((MessageUpdated != null))
         {
            MessageUpdated(sender, e);
         }
      }

      private void OnStatusMessage(object sender, StatusMessageEventArgs e)
      {
         if ((StatusMessage != null))
         {
            StatusMessage(sender, e);
         }
      }

      private void OnUpdateFinished(object sender, EventArgs e)
      {
         if ((UpdateFinished != null))
         {
            UpdateFinished(sender, e);
         }
      }

      #endregion

      #region Constructor

      private readonly HFM.Client.FahClient _fahClient;

      public FahClientInterfaceAdapter()
      {
         _fahClient = new HFM.Client.FahClient();
         _fahClient.ConnectedChanged += OnConnectedChanged;
         _fahClient.DataLengthReceived += OnDataLengthReceived;
         _fahClient.DataLengthSent += OnDataLengthSent;
         _fahClient.MessageUpdated += OnMessageUpdated;
         _fahClient.StatusMessage += OnStatusMessage;
         _fahClient.UpdateFinished += OnUpdateFinished;
      }

      #endregion

      #region Methods

      public void Connect(string host, int port, string password)
      {
         _fahClient.Connect(host, port, password);
      }

      public void Close()
      {
         _fahClient.Close();
      }

      public void ClearBuffer()
      {
         _fahClient.ClearBuffer();
      }

      public void SendCommand(string command)
      {
         _fahClient.SendCommand(command);
      }
      
      public JsonMessage GetJsonMessage(string key)
      {
         return _fahClient.GetJsonMessage(key);
      }

      public T GetMessage<T>() where T : TypedMessage, new()
      {
         return _fahClient.GetMessage<T>();
      }

      public T GetMessage<T, TCollectionType>() where T : TypedMessageCollection, new() where TCollectionType : ITypedMessageObject, new()
      {
         return _fahClient.GetMessage<T, TCollectionType>();
      }

      public void Dispose()
      {
         _fahClient.Dispose();
      }

      #endregion
   }
}
