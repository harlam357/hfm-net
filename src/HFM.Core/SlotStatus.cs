
namespace HFM.Core
{
   /// <summary>
   /// Slot status types.
   /// </summary>
   public enum SlotStatus
   {
      // Matches HFM.Client.DataTypes.FahSlotStatus
      Unknown = 0,
      Paused = 1,
      Running = 2,
      Finishing = 3,  // v7 specific
      Ready = 4,      // v7 specific
      Stopping = 5,   // v7 specific
      Failed = 6,     // v7 specific
      // Extended entries for Legacy clients
      Stopped = 7,
      EuePause = 8,
      Hung = 9,
      RunningNoFrameTimes = 10,
      RunningAsync = 11,
      SendingWorkPacket = 12,
      GettingWorkPacket = 13,
      Offline = 14
   }
}