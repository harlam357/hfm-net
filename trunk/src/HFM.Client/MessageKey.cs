
namespace HFM.Client
{
   public static class MessageKey
   {
      /// <summary>
      /// Heartbeat Message Key
      /// </summary>
      public const string Heartbeat = "heartbeat";
      /// <summary>
      /// Log Restart Message Key
      /// </summary>
      internal const string LogRestart = "log-restart";
      /// <summary>
      /// Log Update Message Key
      /// </summary>
      internal const string LogUpdate = "log-update";
      /// <summary>
      /// Log Message Key (aggregated log text - this key is specific to the HFM.Client API)
      /// </summary>
      public const string Log = "log";
      /// <summary>
      /// Info Message Key
      /// </summary>
      public const string Info = "info";
      /// <summary>
      /// Options Message Key
      /// </summary>
      public const string Options = "options";
      /// <summary>
      /// Queue Info Message Key
      /// </summary>
      public const string QueueInfo = "units";
      /// <summary>
      /// Simulation Info Message Key
      /// </summary>
      /// <remarks>This message is in response to a command that takes a slot id argument.
      /// Will probably need to save messages for each slot requested in a dictionary.</remarks>
      public const string SimulationInfo = "simulation-info";
      /// <summary>
      /// Slot Info Message Key
      /// </summary>
      public const string SlotInfo = "slots";
      /// <summary>
      /// Slot Options Message Key
      /// </summary>
      /// <remarks>This message is in response to a command that takes a slot id argument.
      /// Will probably need to save messages for each slot requested in a dictionary.</remarks>
      public const string SlotOptions = "slot-options";
   }
}
