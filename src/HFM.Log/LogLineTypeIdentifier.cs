
namespace HFM.Log
{
   public interface ILogLineTypeIdentifier
   {
      /// <summary>
      /// Determines the log line type based on the contents of the string line.
      /// </summary>
      LogLineType DetermineLineType(string line);
   }

   public abstract class LogLineTypeIdentifier : ILogLineTypeIdentifier
   {
      /// <summary>
      /// Determines the log line type based on the contents of the string line.
      /// </summary>
      public LogLineType DetermineLineType(string line)
      {
         return OnDetermineLineType(line);
      }

      /// <summary>
      /// Implement this method in derived types to determine the log line type based on the contents of the string line.
      /// </summary>
      protected abstract LogLineType OnDetermineLineType(string line);
   }

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

   namespace Legacy
   {
      public class LegacyLogLineTypeIdentifier : LogLineTypeIdentifier
      {
         /// <summary>
         /// Contains logic to determine the log line type for Legacy logs.
         /// </summary>
         protected override LogLineType OnDetermineLineType(string line)
         {
            var logLineType = CommonLogLineTypeIdentifier.DetermineLineType(line);
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
            if (line.Contains("] - Autosending finished units..."))
            {
               return LogLineType.ClientAutosendStart;
            }
            if (line.Contains("] - Autosend completed"))
            {
               return LogLineType.ClientAutosendComplete;
            }
            if (line.Contains("] + Attempting to send results"))
            {
               return LogLineType.ClientSendStart;
            }
            if (line.Contains("] + Could not connect to Work Server"))
            {
               return LogLineType.ClientSendConnectFailed;
            }
            if (line.Contains("] - Error: Could not transmit unit"))
            {
               return LogLineType.ClientSendFailed;
            }
            if (line.Contains("] + Results successfully sent"))
            {
               return LogLineType.ClientSendComplete;
            }
            if (line.Contains("Arguments:"))
            {
               return LogLineType.ClientArguments;
            }
            if (line.Contains("] - User name:"))
            {
               return LogLineType.ClientUserNameTeam;
            }
            if (line.Contains("] + Requesting User ID from server"))
            {
               return LogLineType.ClientRequestingUserID;
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
            if (line.Contains("] - Will indicate memory of"))
            {
               return LogLineType.ClientIndicateMemory;
            }
            if (line.Contains("] - Detect CPU. Vendor:"))
            {
               return LogLineType.ClientDetectCpu;
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
               return LogLineType.WorkUnitStart;
            }
            /*** ProtoMol Only */
            if (line.Contains("] ************************** ProtoMol Folding@Home Core **************************"))
            {
               return LogLineType.WorkUnitStart;
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
            if (line.Contains("] - Shutting down core"))
            {
               return LogLineType.WorkUnitShuttingDownCore;
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
      public class FahClientLogLineTypeIdentifier : LogLineTypeIdentifier
      {
         /// <summary>
         /// Contains logic to determine the log line type for FahClient logs.
         /// </summary>
         protected override LogLineType OnDetermineLineType(string line)
         {
            var logLineType = CommonLogLineTypeIdentifier.DetermineLineType(line);
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
            if (line.Contains(": Uploading"))
            {
               return LogLineType.ClientSendStart;
            }
            if (line.Contains(": Upload complete"))
            {
               return LogLineType.ClientSendComplete;
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
               return LogLineType.WorkUnitStart;
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
            if (line.Contains(":- Shutting down core"))
            {
               return LogLineType.WorkUnitShuttingDownCore;
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
