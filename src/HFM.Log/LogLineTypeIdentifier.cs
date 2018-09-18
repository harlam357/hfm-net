
namespace HFM.Log
{
   /// <summary>
   /// Identifies the <see cref="LogLineType"/> based on the contents of a string line.
   /// </summary>
   public interface ILogLineTypeIdentifier
   {
      /// <summary>
      /// Returns a <see cref="LogLineType"/> value if the line represents a known log line type; otherwise, returns <see cref="LogLineType.None"/>.
      /// </summary>
      LogLineType GetLogLineType(string line);
   }

   /// <summary>
   /// Identifies the <see cref="LogLineType"/> based on the contents of a string line.
   /// </summary>
   public abstract class LogLineTypeIdentifier : ILogLineTypeIdentifier
   {
      /// <summary>
      /// Returns a <see cref="LogLineType"/> value if the line represents a known log line type; otherwise, returns <see cref="LogLineType.None"/>.
      /// </summary>
      public LogLineType GetLogLineType(string line)
      {
         return OnDetermineLineType(line);
      }

      /// <summary>
      /// Implement this method in a derived type and return a <see cref="LogLineType"/> value based on the contents of the string line.
      /// </summary>
      protected abstract LogLineType OnDetermineLineType(string line);
   }

   namespace Internal
   {
      internal static class CommonLogLineTypeIdentifier
      {
         internal static LogLineType DetermineLineType(string line)
         {
            return IsLineTypeWorkUnitRunning(line) ? LogLineType.WorkUnitRunning : LogLineType.None;
         }

         private static bool IsLineTypeWorkUnitRunning(string line)
         {
            // Change for v7: Removed the leading "] " portion of the
            // string for all but the ProtoMol specific conditions.

            return line.Contains("Preparing to commence simulation") ||
                   line.Contains("Called DecompressByteArray") ||
                   line.Contains("- Digital signature verified") ||
                   line.Contains("] Digital signatures verified") ||
                   line.Contains("Entering M.D.");
         }
      }
   }

   namespace Legacy
   {
      /// <summary>
      /// Identifies the log line type of Legacy client log lines.
      /// </summary>
      public class LegacyLogLineTypeIdentifier : LogLineTypeIdentifier
      {
         internal static LegacyLogLineTypeIdentifier Instance { get; } = new LegacyLogLineTypeIdentifier();

         /// <summary>
         /// Contains logic to identify the log line type of Legacy client log lines.
         /// </summary>
         protected override LogLineType OnDetermineLineType(string line)
         {
            var logLineType = Internal.CommonLogLineTypeIdentifier.DetermineLineType(line);
            if (logLineType != LogLineType.None)
            {
               return logLineType;
            }

            if (line.Contains("--- Opening Log file"))
            {
               return LogLineType.LogOpen;
            }
            if (line.Contains("###############################################################################"))
            {
               return LogLineType.LogHeader;
            }
            if (line.Contains("Folding@Home Client Version"))
            {
               return LogLineType.ClientVersion;
            }
            if (line.Contains("] Sending work to server"))
            {
               return LogLineType.ClientSendWorkToServer;
            }
            if (line.Contains("Arguments:"))
            {
               return LogLineType.ClientArguments;
            }
            if (line.Contains("] - User name:"))
            {
               return LogLineType.ClientUserNameAndTeam;
            }
            if (line.Contains("- Received User ID ="))
            {
               return LogLineType.ClientReceivedUserID;
            }
            if (line.Contains("] - User ID:"))
            {
               return LogLineType.ClientUserID;
            }
            if (line.Contains("] - Machine ID:"))
            {
               return LogLineType.ClientMachineID;
            }
            if (line.Contains("] + Attempting to get work packet"))
            {
               return LogLineType.ClientAttemptGetWorkPacket;
            }
            if (line.Contains("] + Processing work unit"))
            {
               return LogLineType.WorkUnitProcessing;
            }
            if (line.Contains("] + Downloading new core"))
            {
               return LogLineType.WorkUnitCoreDownload;
            }
            if (line.Contains("] Working on Unit 0"))
            {
               return LogLineType.WorkUnitIndex;
            }
            if (line.Contains("] Working on queue slot 0"))
            {
               return LogLineType.WorkUnitQueueIndex;
            }
            if (line.Contains("] + Working ..."))
            {
               return LogLineType.WorkUnitWorking;
            }
            if (line.Contains("] - Calling"))
            {
               return LogLineType.WorkUnitCallingCore;
            }
            if (line.Contains("] *------------------------------*"))
            {
               return LogLineType.WorkUnitCoreStart;
            }
            /*** ProtoMol Only */
            if (line.Contains("] ************************** ProtoMol Folding@Home Core **************************"))
            {
               return LogLineType.WorkUnitCoreStart;
            }
            /*******************/
            if (line.Contains("] Version"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            /*** ProtoMol Only */
            if (line.Contains("]   Version:"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            /*******************/
            if (line.Contains("] Project:"))
            {
               return LogLineType.WorkUnitProject;
            }
            if (line.Contains("] Completed "))
            {
               return LogLineType.WorkUnitFrame;
            }
            if (line.Contains("] + Paused"))
            {
               return LogLineType.WorkUnitPaused;
            }
            if (line.Contains("] + Running on battery power"))
            {
               return LogLineType.WorkUnitPausedForBattery;
            }
            if (line.Contains("] + Off battery, restarting core"))
            {
               return LogLineType.WorkUnitResumeFromBattery;
            }
            if (line.Contains("] Folding@home Core Shutdown:"))
            {
               return LogLineType.WorkUnitCoreShutdown;
            }
            if (line.Contains("] + Number of Units Completed:"))
            {
               return LogLineType.ClientNumberOfUnitsCompleted;
            }
            if (line.Contains("] Client-core communications error:"))
            {
               return LogLineType.ClientCoreCommunicationsError;
            }
            if (line.Contains("] This is a sign of more serious problems, shutting down."))
            {
               //TODO: No unit test coverage - need test log that contains this string
               return LogLineType.ClientCoreCommunicationsErrorShutdown;
            }
            if (line.Contains("] EUE limit exceeded. Pausing 24 hours."))
            {
               return LogLineType.ClientEuePauseState;
            }
            if (line.Contains("Folding@Home will go to sleep for 1 day"))
            {
               return LogLineType.ClientEuePauseState;
            }
            if (line.Contains("Folding@Home Client Shutdown"))
            {
               return LogLineType.ClientShutdown;
            }

            return LogLineType.None;
         }
      }
   }

   namespace FahClient
   {
      /// <summary>
      /// Identifies the log line type of FahClient client log lines.
      /// </summary>
      public class FahClientLogLineTypeIdentifier : LogLineTypeIdentifier
      {
         internal static FahClientLogLineTypeIdentifier Instance { get; } = new FahClientLogLineTypeIdentifier();

         /// <summary>
         /// Contains logic to identify the log line type of FahClient client log lines.
         /// </summary>
         protected override LogLineType OnDetermineLineType(string line)
         {
            var logLineType = Internal.CommonLogLineTypeIdentifier.DetermineLineType(line);
            if (logLineType != LogLineType.None)
            {
               return logLineType;
            }

            if (line.Contains("*********************** Log Started"))
            {
               return LogLineType.LogOpen;
            }
            if (line.Contains(":Sending unit results:"))
            {
               return LogLineType.ClientSendWorkToServer;
            }
            if (line.Contains(":Requesting new work unit for slot"))
            {
               return LogLineType.ClientAttemptGetWorkPacket;
            }
            if (line.Trim().EndsWith(":Starting"))
            {
               return LogLineType.WorkUnitWorking;
            }
            /*** Appears to be for v7.1.38 and previous only ***/
            if (line.Contains(":Starting Unit"))
            {
               return LogLineType.WorkUnitWorking;
            }
            /***************************************************/
            if (line.Contains(":*------------------------------*"))
            {
               return LogLineType.WorkUnitCoreStart;
            }
            if (line.Contains(":Version"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            if (line.Contains(":    Version"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            if (line.Contains(":Project:"))
            {
               return LogLineType.WorkUnitProject;
            }
            if (line.Contains(":Completed "))
            {
               return LogLineType.WorkUnitFrame;
            }
            if (line.Contains(":Folding@home Core Shutdown:"))
            {
               return LogLineType.WorkUnitCoreShutdown;
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(line, "FahCore returned: "))
            {
               return LogLineType.WorkUnitCoreReturn;
            }
            /*** Appears to be for v7.1.38 and previous only ***/
            if (System.Text.RegularExpressions.Regex.IsMatch(line, "FahCore, running Unit \\d{2}, returned: "))
            {
               return LogLineType.WorkUnitCoreReturn;
            }
            /***************************************************/
            if (line.Contains(":Cleaning up"))
            {
               return LogLineType.WorkUnitCleaningUp;
            }
            /*** Appears to be for v7.1.38 and previous only ***/
            if (line.Contains(":Cleaning up Unit"))
            {
               return LogLineType.WorkUnitCleaningUp;
            }
            /***************************************************/

            return LogLineType.None;
         }
      }
   }
}
