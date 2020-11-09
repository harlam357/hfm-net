namespace HFM.Core.Client
{
    public interface IFahClientCommand
    {
        /// <summary>
        /// Sends the Fold command to the FAH client.
        /// </summary>
        /// <param name="slotId">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
        void Fold(int? slotId);

        /// <summary>
        /// Sends the Pause command to the FAH client.
        /// </summary>
        /// <param name="slotId">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
        void Pause(int? slotId);

        /// <summary>
        /// Sends the Finish command to the FAH client.
        /// </summary>
        /// <param name="slotId">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
        void Finish(int? slotId);
    }
}
