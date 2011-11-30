/*
 * HFM.NET - Core.DataTypes Enumerations
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

namespace HFM.Core.DataTypes
{
   /// <summary>
   /// Client Types
   /// </summary>
   public enum ClientType
   {
      FahClient,
      Legacy,
      External //?
   }

   /// <summary>
   /// Legacy Client Sub Types
   /// </summary>
   public enum LegacyClientSubType
   {
      None,
      Path,
      Ftp,
      Http
   }

   /// <summary>
   /// Folding Slot Type
   /// </summary>
   public enum SlotType
   {
      Unknown,
      Uniprocessor,
      SMP,
      GPU
   }

   /// <summary>
   /// Slot Status Types
   /// </summary>
   public enum SlotStatus
   {
      // Matches HFM.Client.DataTypes.FahSlotStatus
      Unknown,
      Paused,
      Running,
      Finishing,
      SendingWorkPacket,
      // Extended entries for Legacy clients
      Stopped,
      EuePause,
      Hung,
      RunningNoFrameTimes,
      RunningAsync,
      GettingWorkPacket,
      Offline
   }

   /// <summary>
   /// Log Line Types
   /// </summary>
   public enum LogLineType
   {
      Error = -2,
      Unknown = -1,
      LogOpen = 0,
      LogHeader,
      ClientVersion,
      ClientSendWorkToServer,
      ClientAutosendStart,
      ClientAutosendComplete,
      ClientSendStart,
      ClientSendConnectFailed,
      ClientSendFailed,
      ClientSendComplete,
      ClientArguments,
      ClientUserNameTeam,
      ClientRequestingUserID,
      ClientReceivedUserID,
      ClientUserID,
      ClientMachineID,
      ClientAttemptGetWorkPacket,
      ClientIndicateMemory,
      ClientDetectCpu,
      WorkUnitProcessing,
      WorkUnitCoreDownload,
      WorkUnitIndex,
      WorkUnitQueueIndex,
      WorkUnitWorking,
      WorkUnitStart,
      WorkUnitCoreVersion,
      WorkUnitRunning,
      WorkUnitProject,
      WorkUnitFrame,
      WorkUnitPaused,
      WorkUnitPausedForBattery,
      WorkUnitResumeFromBattery,
      WorkUnitShuttingDownCore,
      WorkUnitCoreShutdown,
      WorkUnitCoreReturn,           // v7 specific
      WorkUnitCleaningUp,           // v7 specific
      ClientNumberOfUnitsCompleted,
      ClientCoreCommunicationsError,
      ClientCoreCommunicationsErrorShutdown,
      ClientEuePauseState,
      ClientShutdown
   }

   /// <summary>
   /// Work Unit Result Types
   /// </summary>
   public enum WorkUnitResult
   {
      Unknown,
      FinishedUnit,
      EarlyUnitEnd,
      UnstableMachine,
      Interrupted,
      BadWorkUnit,
      CoreOutdated,
      ClientCoreError
   }

   /// <summary>
   /// Queue Entry Status Types
   /// </summary>
   public enum QueueEntryStatus
   {
      Unknown,
      Empty,
      Deleted,
      Finished,
      Garbage,
      FoldingNow,
      Queued,
      ReadyForUpload,
      Abandonded,
      FetchingFromServer
   }

   /// <summary>
   /// Work Unit Log Filter Types
   /// </summary>
   public enum LogFilterType
   {
      IndexOnly,
      IndexAndNonIndexed
   }
}
