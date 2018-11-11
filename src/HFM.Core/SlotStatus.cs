
namespace HFM.Core
{
   /// <summary>
   /// Represents the status of a Folding@Home client slot.
   /// </summary>
   public enum SlotStatus
   {
      // Matches HFM.Client.DataTypes.FahSlotStatus
      // Matches HFM.Log.LogSlotStatus

      /// <summary>
      /// The status of the slot is unknown.
      /// </summary>
      Unknown = 0,
      /// <summary>
      /// The slot is paused.
      /// </summary>
      Paused = 1,
      /// <summary>
      /// The slot is running.
      /// </summary>
      Running = 2,
      /// <summary>
      /// The slot is finishing (v7 or newer only).
      /// </summary>
      Finishing = 3,
      /// <summary>
      /// The slot is ready for work (v7 or newer only).
      /// </summary>
      Ready = 4,
      /// <summary>
      /// The slot is stopping (v7 or newer only).
      /// </summary>
      Stopping = 5,
      /// <summary>
      /// The slot work has failed (v7 or newer only).
      /// </summary>
      Failed = 6,
      /// <summary>
      /// The slot is stopped (v6 or prior only).
      /// </summary>
      Stopped = 7,
      /// <summary>
      /// The slot has paused due to too many early 'unit end work' work unit results (v6 or prior only).
      /// </summary>
      EuePause = 8,
      /// <summary>
      /// The slot is hung.  A hung slot is defined as a one that does not report frame progress after an expected period of time (v6 or prior only).
      /// </summary>
      Hung = 9,
      /// <summary>
      /// The slot is running but does not have enough frame data to calculate a frame time.
      /// </summary>
      RunningNoFrameTimes = 10,
      /// <summary>
      /// The slot is running but the clock of the client machine and the clock of the Folding@Home servers appear to out of sync (v6 or prior only).
      /// </summary>
      RunningAsync = 11,
      /// <summary>
      /// The slot is sending a work packet back to the Folding@Home servers (v6 or prior only).
      /// </summary>
      SendingWorkPacket = 12,
      /// <summary>
      /// The slot is getting a work packet from the Folding@Home servers (v6 or prior only).
      /// </summary>
      GettingWorkPacket = 13,
      /// <summary>
      /// The slot is offline.
      /// </summary>
      Offline = 14
   }
}