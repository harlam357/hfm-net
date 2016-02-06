
using System;

namespace HFM.Client
{
   public interface IMessageConnection : IConnection
   {
      #region Events

      event EventHandler<MessageReceivedEventArgs> MessageReceived;
      event EventHandler UpdateFinished;

      #endregion
   }
}
