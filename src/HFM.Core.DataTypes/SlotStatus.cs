
namespace HFM.Core.DataTypes
{
   /// <summary>
   /// Slot status types.
   /// </summary>
   public enum SlotStatus
   {
      // Matches HFM.Client.DataTypes.FahSlotStatus
      Unknown,
      Paused,
      Running,
      Finishing,  // v7 specific
      Ready,      // v7 specific
      Stopping,   // v7 specific
      Failed,     // v7 specific
      // Extended entries for Legacy clients
      Stopped,
      EuePause,
      Hung,
      RunningNoFrameTimes,
      RunningAsync,
      SendingWorkPacket,
      GettingWorkPacket,
      Offline
   }
}