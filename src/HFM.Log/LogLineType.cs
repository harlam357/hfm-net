/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

namespace HFM.Log
{
   /// <summary>
   /// Represents the types of log lines that can be detected by the HFM.Log API.
   /// Once the type has been identified a parser may be assigned to find data in the line text.
   /// </summary>
   public enum LogLineType
   {
      Error = -1,
      /// <summary>
      /// Line does not contain any information or indicate any condition of interest.
      /// </summary>
      None = 0,
      /// <summary>
      /// Line is a log opening line containing client start date and time (FahClient and Legacy clients)
      /// </summary>
      LogOpen = 1,
      /// <summary>
      /// Line is a log header line containing a long string of '#' characters (Legacy clients only)
      /// </summary>
      LogHeader,
      /// <summary>
      /// Line contains client version information (Legacy clients only)
      /// </summary>
      ClientVersion,
      /// <summary>
      /// Line indicates the client is sending work to server (FahClient and Legacy clients)
      /// </summary>
      ClientSendWorkToServer,
      /// <summary>
      /// Line contains client argument information (Legacy clients only)
      /// </summary>
      ClientArguments,
      /// <summary>
      /// Line contains user name and team information (Legacy clients only)
      /// </summary>
      ClientUserNameAndTeam,
      /// <summary>
      /// Line contains user ID information received from server (Legacy clients only)
      /// </summary>
      ClientReceivedUserID,
      /// <summary>
      /// Line contains user ID information stored locally by the client (Legacy clients only)
      /// </summary>
      ClientUserID,
      /// <summary>
      /// Line contains machine ID information (Legacy clients only)
      /// </summary>
      ClientMachineID,
      /// <summary>
      /// Line indicates the client is attempting to get a work packet (FahClient and Legacy clients)
      /// </summary>
      ClientAttemptGetWorkPacket,
      /// <summary>
      /// Line indicates the client has begun processing a work unit (Legacy clients only)
      /// </summary>
      WorkUnitProcessing,
      /// <summary>
      /// Line indicates the client is downloading a new core executable (Legacy clients only)
      /// </summary>
      WorkUnitCoreDownload,
      /// <summary>
      /// Line contains work unit index information (Legacy clients only)
      /// </summary>
      WorkUnitIndex,
      /// <summary>
      /// Line contains work unit queue index information (Legacy clients only)
      /// </summary>
      WorkUnitQueueIndex,
      /// <summary>
      /// Line indicates the client has begun working on a work unit (FahClient and Legacy clients)
      /// </summary>
      WorkUnitWorking,
      /// <summary>
      /// Line contains an echo of the call to the core worker process, including the number of cpu threads (Legacy clients only)
      /// </summary>
      WorkUnitCallingCore,
      /// <summary>
      /// Line "*------------------------------*" indicates the client core process has begun working on a work unit (FahClient and Legacy clients)
      /// </summary>
      WorkUnitCoreStart,
      /// <summary>
      /// Line contains core executable version information (FahClient and Legacy clients)
      /// </summary>
      WorkUnitCoreVersion,
      /// <summary>
      /// Line indicates the client core process did not fail to start and is running (FahClient and Legacy clients)
      /// </summary>
      WorkUnitRunning,
      /// <summary>
      /// Line contains work unit project information (FahClient and Legacy clients)
      /// </summary>
      WorkUnitProject,
      /// <summary>
      /// Line contains work unit frame (progress) information (FahClient and Legacy clients)
      /// </summary>
      WorkUnitFrame,
      /// <summary>
      /// Line indicates the work unit was paused by the client (Legacy clients only)
      /// </summary>
      WorkUnitPaused,
      /// <summary>
      /// Line indicates the work unit was paused by the client due to the host machine transitioning to battery power (Legacy clients only)
      /// </summary>
      WorkUnitPausedForBattery,
      /// <summary>
      /// Line indicates the work unit was resumed by the client due to the host machine transitioning from battery power (Legacy clients only)
      /// </summary>
      WorkUnitResumeFromBattery,
      /// <summary>
      /// Line contains client core process result string (FahClient and Legacy clients)
      /// </summary>
      WorkUnitCoreShutdown,
      /// <summary>
      /// Line contains the client echo of the core process result string (FahClient clients only)
      /// </summary>
      WorkUnitCoreReturn,
      /// <summary>
      /// Line indicates work unit processing is complete (FahClient clients only)
      /// </summary>
      WorkUnitCleaningUp,
      /// <summary>
      /// Line contains the total number of work units completed by the client (Legacy clients only)
      /// </summary>
      ClientNumberOfUnitsCompleted,
      /// <summary>
      /// Line indicates a client-core communications error (Legacy clients only)
      /// </summary>
      ClientCoreCommunicationsError,
      /// <summary>
      /// Line indicates a client-core communications error which caused the client to shutdown (Legacy clients only)
      /// </summary>
      ClientCoreCommunicationsErrorShutdown,
      /// <summary>
      /// Line indicates the client has encountered too many EARLY_UNIT_END results from client core processes and will pause activity for 24 hours (Legacy clients only)
      /// </summary>
      ClientEuePauseState,
      /// <summary>
      /// Line indicates the client has been shutdown (Legacy clients only)
      /// </summary>
      ClientShutdown
   }
}
