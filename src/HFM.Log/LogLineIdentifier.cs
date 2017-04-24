
using HFM.Core.DataTypes;

namespace HFM.Log
{
   internal static class LogLineIdentifier
   {
      internal static LogLineType GetLogLineType(string line, FahLogType fahLogType)
      {
         var logLineType = Common.DetermineLineType(line);
         if (logLineType == LogLineType.Unknown)
         {
            switch (fahLogType)
            {
               case FahLogType.Legacy:
                  logLineType = Legacy.DetermineLineType(line);
                  break;
               case FahLogType.FahClient:
                  logLineType = FahClient.DetermineLineType(line);
                  break;
            }
         }
         return logLineType;
      }

      private static class Common
      {
         internal static LogLineType DetermineLineType(string logLine)
         {
            return IsLineTypeWorkUnitRunning(logLine) ? LogLineType.WorkUnitRunning : LogLineType.Unknown;
         }

         private static bool IsLineTypeWorkUnitRunning(string logLine)
         {
            // Change for v7: Removed the leading "] " portion of the
            // string for all but the ProtoMol specific conditions.

            return logLine.Contains("Preparing to commence simulation") ||
                   logLine.Contains("Called DecompressByteArray") ||
                   logLine.Contains("- Digital signature verified") ||
                   logLine.Contains("] Digital signatures verified") ||
                   logLine.Contains("Entering M.D.");

            //if (logLine.Contains("Preparing to commence simulation"))
            //{
            //   return true;
            //}
            //if (logLine.Contains("Called DecompressByteArray"))
            //{
            //   return true;
            //}
            //if (logLine.Contains("- Digital signature verified"))
            //{
            //   return true;
            //}
            ////*** ProtoMol Only */
            //if (logLine.Contains("] Digital signatures verified"))
            //{
            //   return true;
            //}
            ////*******************/
            //if (logLine.Contains("Entering M.D."))
            //{
            //   return true;
            //}
            //
            //return false;
         }
      }

      private static class Legacy
      {
         internal static LogLineType DetermineLineType(string logLine)
         {
            if (logLine.Contains("--- Opening Log file"))
            {
               return LogLineType.LogOpen;
            }
            if (logLine.Contains("###############################################################################"))
            {
               return LogLineType.LogHeader;
            }
            if (logLine.Contains("Folding@Home Client Version"))
            {
               return LogLineType.ClientVersion;
            }
            if (logLine.Contains("] Sending work to server"))
            {
               return LogLineType.ClientSendWorkToServer;
            }
            if (logLine.Contains("] - Autosending finished units..."))
            {
               return LogLineType.ClientAutosendStart;
            }
            if (logLine.Contains("] - Autosend completed"))
            {
               return LogLineType.ClientAutosendComplete;
            }
            if (logLine.Contains("] + Attempting to send results"))
            {
               return LogLineType.ClientSendStart;
            }
            if (logLine.Contains("] + Could not connect to Work Server"))
            {
               return LogLineType.ClientSendConnectFailed;
            }
            if (logLine.Contains("] - Error: Could not transmit unit"))
            {
               return LogLineType.ClientSendFailed;
            }
            if (logLine.Contains("] + Results successfully sent"))
            {
               return LogLineType.ClientSendComplete;
            }
            if (logLine.Contains("Arguments:"))
            {
               return LogLineType.ClientArguments;
            }
            if (logLine.Contains("] - User name:"))
            {
               return LogLineType.ClientUserNameTeam;
            }
            if (logLine.Contains("] + Requesting User ID from server"))
            {
               return LogLineType.ClientRequestingUserID;
            }
            if (logLine.Contains("- Received User ID ="))
            {
               return LogLineType.ClientReceivedUserID;
            }
            if (logLine.Contains("] - User ID:"))
            {
               return LogLineType.ClientUserID;
            }
            if (logLine.Contains("] - Machine ID:"))
            {
               return LogLineType.ClientMachineID;
            }
            if (logLine.Contains("] + Attempting to get work packet"))
            {
               return LogLineType.ClientAttemptGetWorkPacket;
            }
            if (logLine.Contains("] - Will indicate memory of"))
            {
               return LogLineType.ClientIndicateMemory;
            }
            if (logLine.Contains("] - Detect CPU. Vendor:"))
            {
               return LogLineType.ClientDetectCpu;
            }
            if (logLine.Contains("] + Processing work unit"))
            {
               return LogLineType.WorkUnitProcessing;
            }
            if (logLine.Contains("] + Downloading new core"))
            {
               return LogLineType.WorkUnitCoreDownload;
            }
            if (logLine.Contains("] Working on Unit 0"))
            {
               return LogLineType.WorkUnitIndex;
            }
            if (logLine.Contains("] Working on queue slot 0"))
            {
               return LogLineType.WorkUnitQueueIndex;
            }
            if (logLine.Contains("] + Working ..."))
            {
               return LogLineType.WorkUnitWorking;
            }
            if (logLine.Contains("] - Calling"))
            {
               return LogLineType.WorkUnitCallingCore;
            }
            if (logLine.Contains("] *------------------------------*"))
            {
               return LogLineType.WorkUnitStart;
            }
            /*** ProtoMol Only */
            if (logLine.Contains("] ************************** ProtoMol Folding@Home Core **************************"))
            {
               return LogLineType.WorkUnitStart;
            }
            /*******************/
            if (logLine.Contains("] Version"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            /*** ProtoMol Only */
            if (logLine.Contains("]   Version:"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            /*******************/
            if (logLine.Contains("] Project:"))
            {
               return LogLineType.WorkUnitProject;
            }
            if (logLine.Contains("] Completed "))
            {
               return LogLineType.WorkUnitFrame;
            }
            if (logLine.Contains("] + Paused"))
            {
               return LogLineType.WorkUnitPaused;
            }
            if (logLine.Contains("] + Running on battery power"))
            {
               return LogLineType.WorkUnitPausedForBattery;
            }
            if (logLine.Contains("] + Off battery, restarting core"))
            {
               return LogLineType.WorkUnitResumeFromBattery;
            }
            if (logLine.Contains("] - Shutting down core"))
            {
               return LogLineType.WorkUnitShuttingDownCore;
            }
            if (logLine.Contains("] Folding@home Core Shutdown:"))
            {
               return LogLineType.WorkUnitCoreShutdown;
            }
            if (logLine.Contains("] + Number of Units Completed:"))
            {
               return LogLineType.ClientNumberOfUnitsCompleted;
            }
            if (logLine.Contains("] Client-core communications error:"))
            {
               return LogLineType.ClientCoreCommunicationsError;
            }
            if (logLine.Contains("] This is a sign of more serious problems, shutting down."))
            {
               //TODO: No unit test coverage - need test log that contains this string
               return LogLineType.ClientCoreCommunicationsErrorShutdown;
            }
            if (logLine.Contains("] EUE limit exceeded. Pausing 24 hours."))
            {
               return LogLineType.ClientEuePauseState;
            }
            if (logLine.Contains("Folding@Home will go to sleep for 1 day"))
            {
               return LogLineType.ClientEuePauseState;
            }
            if (logLine.Contains("Folding@Home Client Shutdown"))
            {
               return LogLineType.ClientShutdown;
            }

            return LogLineType.Unknown;
         }
      }

      private static class FahClient
      {
         internal static LogLineType DetermineLineType(string logLine)
         {
            if (logLine.Contains("*********************** Log Started"))
            {
               return LogLineType.LogOpen;
            }
            if (logLine.Contains(":Sending unit results:"))
            {
               return LogLineType.ClientSendWorkToServer;
            }
            if (logLine.Contains(": Uploading"))
            {
               return LogLineType.ClientSendStart;
            }
            if (logLine.Contains(": Upload complete"))
            {
               return LogLineType.ClientSendComplete;
            }
            if (logLine.Contains(":Requesting new work unit for slot"))
            {
               return LogLineType.ClientAttemptGetWorkPacket;
            }
            if (logLine.Trim().EndsWith(":Starting"))
            {
               return LogLineType.WorkUnitWorking;
            }
            /*** Appears to be for v7.1.38 and previous only ***/
            if (logLine.Contains(":Starting Unit"))
            {
               return LogLineType.WorkUnitWorking;
            }
            /***************************************************/
            if (logLine.Contains(":*------------------------------*"))
            {
               return LogLineType.WorkUnitStart;
            }
            if (logLine.Contains(":Version"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            if (logLine.Contains(":    Version"))
            {
               return LogLineType.WorkUnitCoreVersion;
            }
            if (logLine.Contains(":Project:"))
            {
               return LogLineType.WorkUnitProject;
            }
            if (logLine.Contains(":Completed "))
            {
               return LogLineType.WorkUnitFrame;
            }
            if (logLine.Contains(":- Shutting down core"))
            {
               return LogLineType.WorkUnitShuttingDownCore;
            }
            if (logLine.Contains(":Folding@home Core Shutdown:"))
            {
               return LogLineType.WorkUnitCoreShutdown;
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(logLine, "FahCore returned: "))
            {
               return LogLineType.WorkUnitCoreReturn;
            }
            /*** Appears to be for v7.1.38 and previous only ***/
            if (System.Text.RegularExpressions.Regex.IsMatch(logLine, "FahCore, running Unit \\d{2}, returned: "))
            {
               return LogLineType.WorkUnitCoreReturn;
            }
            /***************************************************/
            if (logLine.Contains(":Cleaning up"))
            {
               return LogLineType.WorkUnitCleaningUp;
            }
            /*** Appears to be for v7.1.38 and previous only ***/
            if (logLine.Contains(":Cleaning up Unit"))
            {
               return LogLineType.WorkUnitCleaningUp;
            }
            /***************************************************/

            return LogLineType.Unknown;
         }
      }
   }
}
