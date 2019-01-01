
namespace HFM.Log.Legacy
{
   // See HFM.Core.SlotStatus for the global enumeration covering Legacy and v7 clients

   /// <summary>
   /// Represents the status of a Folding@Home client slot.
   /// </summary>
   public enum LegacySlotStatus
   {
      /// <summary>
      /// The status of the slot is unknown.
      /// </summary>
      Unknown = 0,
      /// <summary>
      /// The slot is paused (v6 or prior only).
      /// </summary>
      Paused = 1,
      /// <summary>
      /// The slot is stopped (v6 or prior only).
      /// </summary>
      Stopped = 7,
      /// <summary>
      /// The slot has paused due to too many early 'unit end work' work unit results (v6 or prior only).
      /// </summary>
      EuePause = 8,
      /// <summary>
      /// The slot is running but does not have enough frame data to calculate a frame time (v6 or prior only).
      /// </summary>
      RunningNoFrameTimes = 10,
      /// <summary>
      /// The slot is sending a work packet back to the Folding@Home servers (v6 or prior only).
      /// </summary>
      SendingWorkPacket = 12,
      /// <summary>
      /// The slot is getting a work packet from the Folding@Home servers (v6 or prior only).
      /// </summary>
      GettingWorkPacket = 13
   }
}
