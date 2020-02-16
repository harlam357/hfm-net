
using System;

namespace HFM.Log
{
   /// <summary>
   /// Resolves the <see cref="LogLineType"/> based on the contents of a string line.
   /// </summary>
   public abstract class LogLineTypeResolver
   {
      /// <summary>
      /// Returns a <see cref="LogLineType"/> value if the line represents a known log line type; otherwise, returns <see cref="LogLineType.None"/>.
      /// </summary>
      public LogLineType Resolve(string line)
      {
         return OnResolveLogLineType(line);
      }

      /// <summary>
      /// Implement this method in a derived type and return a <see cref="LogLineType"/> value based on the contents of the string line.
      /// </summary>
      protected abstract LogLineType OnResolveLogLineType(string line);
   }

   namespace Internal
   {
      internal static class CommonLogLineTypeResolver
      {
         internal static LogLineType ResolveLogLineType(string line)
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
                   // ProtoMol Only
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
      public class LegacyLogLineTypeResolver : LogLineTypeResolver
      {
         /// <summary>
         /// Gets a singleton instance of the <see cref="LegacyLogLineTypeResolver"/> class.
         /// </summary>
         public static LegacyLogLineTypeResolver Instance { get; } = new LegacyLogLineTypeResolver();

         /// <summary>
         /// Contains logic to identify the log line type of Legacy client log lines.
         /// </summary>
         protected override LogLineType OnResolveLogLineType(string line)
         {
            var logLineType = Internal.CommonLogLineTypeResolver.ResolveLogLineType(line);
            if (logLineType != LogLineType.None)
            {
               return logLineType;
            }

            if (line.Contains("--- Opening Log file")) return LogLineType.LogOpen;
            if (line.Contains("###############################################################################")) return LogLineType.LogHeader;
            if (line.Contains("Folding@Home Client Version")) return LogLineType.ClientVersion;
            if (line.Contains("] Sending work to server")) return LogLineType.ClientSendWorkToServer;
            if (line.Contains("Arguments:")) return LogLineType.ClientArguments;
            if (line.Contains("] - User name:")) return LogLineType.ClientUserNameAndTeam;
            if (line.Contains("- Received User ID =")) return LogLineType.ClientReceivedUserID;
            if (line.Contains("] - User ID:")) return LogLineType.ClientUserID;
            if (line.Contains("] - Machine ID:")) return LogLineType.ClientMachineID;
            if (line.Contains("] + Attempting to get work packet")) return LogLineType.ClientAttemptGetWorkPacket;
            if (line.Contains("] + Processing work unit")) return LogLineType.WorkUnitProcessing;
            if (line.Contains("] + Downloading new core")) return LogLineType.WorkUnitCoreDownload;
            if (line.Contains("] Working on Unit 0")) return LogLineType.WorkUnitIndex;
            if (line.Contains("] Working on queue slot 0")) return LogLineType.WorkUnitQueueIndex;
            if (line.Contains("] + Working ...")) return LogLineType.WorkUnitWorking;
            if (line.Contains("] - Calling")) return LogLineType.WorkUnitCallingCore;
            if (line.Contains("] *------------------------------*")) return LogLineType.WorkUnitCoreStart;
            // ProtoMol Only
            if (line.Contains("] ************************** ProtoMol Folding@Home Core **************************")) return LogLineType.WorkUnitCoreStart;
            if (line.Contains("] Version")) return LogLineType.WorkUnitCoreVersion;
            // ProtoMol Only
            if (line.Contains("]   Version:")) return LogLineType.WorkUnitCoreVersion;
            if (line.Contains("] Project:")) return LogLineType.WorkUnitProject;
            if (line.Contains("] Completed ")) return LogLineType.WorkUnitFrame;
            if (line.Contains("] + Paused")) return LogLineType.WorkUnitPaused;
            if (line.Contains("] + Running on battery power")) return LogLineType.WorkUnitPausedForBattery;
            if (line.Contains("] + Off battery, restarting core")) return LogLineType.WorkUnitResumeFromBattery;
            if (line.Contains("] Folding@home Core Shutdown:")) return LogLineType.WorkUnitCoreShutdown;
            if (line.Contains("] + Number of Units Completed:")) return LogLineType.ClientNumberOfUnitsCompleted;
            if (line.Contains("] Client-core communications error:")) return LogLineType.ClientCoreCommunicationsError;
            if (line.Contains("] This is a sign of more serious problems, shutting down.")) return LogLineType.ClientCoreCommunicationsErrorShutdown;
            if (line.Contains("] EUE limit exceeded. Pausing 24 hours.")) return LogLineType.ClientEuePauseState;
            if (line.Contains("Folding@Home will go to sleep for 1 day")) return LogLineType.ClientEuePauseState;
            if (line.Contains("Folding@Home Client Shutdown")) return LogLineType.ClientShutdown;

            return LogLineType.None;
         }
      }
   }

   namespace FahClient
   {
      /// <summary>
      /// Identifies the log line type of FahClient client log lines.
      /// </summary>
      public class FahClientLogLineTypeResolver : LogLineTypeResolver
      {
         /// <summary>
         /// Gets a singleton instance of the <see cref="FahClientLogLineTypeResolver"/> class.
         /// </summary>
         public static FahClientLogLineTypeResolver Instance { get; } = new FahClientLogLineTypeResolver();

         /// <summary>
         /// Contains logic to identify the log line type of FahClient client log lines.
         /// </summary>
         protected override LogLineType OnResolveLogLineType(string line)
         {
            var logLineType = Internal.CommonLogLineTypeResolver.ResolveLogLineType(line);
            if (logLineType != LogLineType.None)
            {
               return logLineType;
            }

            if (line.Contains("*********************** Log Started")) return LogLineType.LogOpen;
            if (line.Contains(":Sending unit results:")) return LogLineType.ClientSendWorkToServer;
            if (line.Contains(":Requesting new work unit for slot")) return LogLineType.ClientAttemptGetWorkPacket;
            if (line.Trim().EndsWith(":Starting")) return LogLineType.WorkUnitWorking;
            // Appears to be for v7.1.38 and previous only
            if (line.Contains(":Starting Unit")) return LogLineType.WorkUnitWorking;
            if (line.Contains(":*------------------------------*")) return LogLineType.WorkUnitCoreStart;
            if (line.Contains(":Version")) return LogLineType.WorkUnitCoreVersion;
            // Ignore v7 client version information by looking for this pattern beyond index 8 - see TestFiles\Client_v7_14\log.txt for an example
            if (line.IndexOf(":    Version", StringComparison.InvariantCulture) > 8) return LogLineType.WorkUnitCoreVersion;
            if (line.Contains(":Project:")) return LogLineType.WorkUnitProject;
            if (line.Contains(":Completed ")) return LogLineType.WorkUnitFrame;
            if (line.Contains(":Folding@home Core Shutdown:")) return LogLineType.WorkUnitCoreShutdown;
            if (System.Text.RegularExpressions.Regex.IsMatch(line, "FahCore returned: ")) return LogLineType.WorkUnitCoreReturn;
            // Appears to be for v7.1.38 and previous only
            if (System.Text.RegularExpressions.Regex.IsMatch(line, "FahCore, running Unit \\d{2}, returned: ")) return LogLineType.WorkUnitCoreReturn;
            if (line.Contains(":Cleaning up")) return LogLineType.WorkUnitCleaningUp;
            // Appears to be for v7.1.38 and previous only
            if (line.Contains(":Cleaning up Unit")) return LogLineType.WorkUnitCleaningUp;
            if (line.Contains(":Too many errors, failing")) return LogLineType.WorkUnitTooManyErrors;

            return LogLineType.None;
         }
      }
   }
}
