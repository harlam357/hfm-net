
namespace HFM.Log
{
   // See HFM.Core.SlotStatus for the global enumeration covering Legacy and v7 clients

   /// <summary>
   /// Slot status types.
   /// </summary>
   public enum LogSlotStatus
   {
      Unknown = 0,
      Paused = 1,
      Stopped = 7,
      EuePause = 8,
      RunningNoFrameTimes = 10,
      SendingWorkPacket = 12,
      GettingWorkPacket = 13
   }
}
