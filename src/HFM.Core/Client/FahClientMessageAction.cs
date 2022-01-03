using System;

using HFM.Client;

namespace HFM.Core.Client
{
    public abstract class FahClientMessageAction
    {
        public void Execute(string messageType)
        {
            if (ShouldExecute(messageType))
            {
                OnExecute(messageType);
            }
        }

        protected abstract bool ShouldExecute(string messageType);

        protected abstract void OnExecute(string messageType);
    }

    public class DelegateFahClientMessageAction : FahClientMessageAction
    {
        protected string MessageType { get; }
        protected Action Action { get; }

        public DelegateFahClientMessageAction(string messageType, Action action)
        {
            MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        protected override bool ShouldExecute(string messageType) => messageType == MessageType;

        protected override void OnExecute(string messageType) => Action();
    }

    public class ExecuteRetrieveMessageAction : FahClientMessageAction
    {
        protected FahClientMessages Messages { get; }
        protected Action Action { get; }

        public ExecuteRetrieveMessageAction(FahClientMessages messages, Action action)
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        protected override bool ShouldExecute(string messageType)
        {
            switch (messageType)
            {
                case FahClientMessageType.SlotInfo:
                    return Messages.LogIsRetrieved;
                case FahClientMessageType.QueueInfo:
                    return Messages.UnitCollection?.Count > 0 && Messages.LogIsRetrieved;
                case FahClientMessageType.LogRestart:
                case FahClientMessageType.LogUpdate:
                    return Messages.SlotCollection != null;
                default:
                    return false;
            }
        }

        protected override void OnExecute(string messageType) => Action();
    }
}
