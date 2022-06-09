namespace HFM.Core.Client
{
    public interface IFahClientCommand
    {
        /// <summary>
        /// Sends the Fold command to the FAH client.
        /// </summary>
        /// <param name="slotID">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
        void Fold(int? slotID);

        /// <summary>
        /// Sends the Pause command to the FAH client.
        /// </summary>
        /// <param name="slotID">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
        void Pause(int? slotID);

        /// <summary>
        /// Sends the Finish command to the FAH client.
        /// </summary>
        /// <param name="slotID">If not null, sends the command to the specified slot; otherwise, the command will be sent to all client slots.</param>
        void Finish(int? slotID);
    }
}
