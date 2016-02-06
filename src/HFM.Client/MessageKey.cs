
namespace HFM.Client
{
   /// <summary>
   /// Folding@Home client message keys.
   /// </summary>
   public static class MessageKey
   {
      /// <summary>
      /// Heartbeat Message Key.
      /// </summary>
      public const string Heartbeat = "heartbeat";
      /// <summary>
      /// Info Message Key.
      /// </summary>
      public const string Info = "info";
      /// <summary>
      /// Options Message Key.
      /// </summary>
      public const string Options = "options";
      /// <summary>
      /// Simulation Info Message Key.
      /// </summary>
      /// <remarks>This message is in response to a command that takes a slot id argument.</remarks>
      public const string SimulationInfo = "simulation-info";
      /// <summary>
      /// Slot Info Message Key.
      /// </summary>
      public const string SlotInfo = "slots";
      /// <summary>
      /// Slot Options Message Key.
      /// </summary>
      /// <remarks>This message is in response to a command that takes a slot id argument.</remarks>
      public const string SlotOptions = "slot-options";
      /// <summary>
      /// Queue Info Message Key.
      /// </summary>
      public const string QueueInfo = "units";

      /// <summary>
      /// Log Restart Message Key.
      /// </summary>
      public const string LogRestart = "log-restart";
      /// <summary>
      /// Log Update Message Key.
      /// </summary>
      public const string LogUpdate = "log-update";
   }
}
