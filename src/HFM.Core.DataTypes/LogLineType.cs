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

namespace HFM.Core.DataTypes
{
#pragma warning disable 1591

   /// <summary>
   /// Log line types.
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
      WorkUnitCallingCore,
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

#pragma warning restore 1591
}
