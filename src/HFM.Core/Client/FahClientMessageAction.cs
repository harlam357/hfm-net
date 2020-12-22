using System;

using HFM.Client;

namespace HFM.Core.Client
{
    public abstract class FahClientMessageAction
    {
        protected Action Action { get; }

        protected FahClientMessageAction(Action action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Execute(string messageType)
        {
            if (ShouldExecute(messageType))
            {
                OnExecute(messageType);
            }
        }

        protected abstract bool ShouldExecute(string messageType);

        protected virtual void OnExecute(string messageType) => Action();
    }

    public class SlotInfoMessageAction : FahClientMessageAction
    {
        public SlotInfoMessageAction(Action action) : base(action)
        {
        }


        protected override bool ShouldExecute(string messageType) => messageType == FahClientMessageType.SlotInfo;
    }

    public class ExecuteRetrieveMessageAction : FahClientMessageAction
    {
        private readonly FahClientMessages _messages;

        public ExecuteRetrieveMessageAction(FahClientMessages messages, Action action) : base(action)
        {
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }

        protected override bool ShouldExecute(string messageType)
        {
            switch (messageType)
            {
                case FahClientMessageType.SlotInfo:
                    return _messages.LogIsRetrieved;
                case FahClientMessageType.QueueInfo:
                    return _messages.UnitCollection?.Count > 0 && _messages.LogIsRetrieved;
                case FahClientMessageType.LogRestart:
                case FahClientMessageType.LogUpdate:
                    return _messages.SlotCollection != null;
                default:
                    return false;
            }
        }
    }
}
