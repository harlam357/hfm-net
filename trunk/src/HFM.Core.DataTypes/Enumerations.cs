/*
 * HFM.NET - Core.DataTypes Enumerations
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
   /// Slot status types.
   /// </summary>
   public enum SlotStatus
   {
      // Matches HFM.Client.DataTypes.FahSlotStatus
      Unknown,
      Paused,
      Running,
      Finishing,  // v7 specific
      Ready,      // v7 specific
      Stopping,   // v7 specific
      Failed,     // v7 specific
      // Extended entries for Legacy clients
      Stopped,
      EuePause,
      Hung,
      RunningNoFrameTimes,
      RunningAsync,
      SendingWorkPacket,
      GettingWorkPacket,
      Offline
   }

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

   /// <summary>
   /// Work unit result types.
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
   /// Work unit log filter types.
   /// </summary>
   public enum LogFilterType
   {
      UnitIndex,
      UnitAndNonIndexed,
      SlotIndex,
      SlotAndNonIndexed
   }

   // ReSharper disable InconsistentNaming

   /// <summary>
   /// Operating system types.
   /// </summary>
   public enum OperatingSystemType
   {
      Unknown,
      Windows,
      WindowsXP,
      WindowsVista,
      Windows7,
      Linux,
      OSX
   }

   /// <summary>
   /// Operating system architecture types.
   /// </summary>
   public enum OperatingSystemArchitectureType
   {
      Unknown,
      x86,
      x64
   }

   /// <summary>
   /// CPU manufacturers.
   /// </summary>
   public enum CpuManufacturer
   {
      Unknown,
      Intel,
      AMD
   }

   /// <summary>
   /// CPU types.
   /// </summary>
   public enum CpuType
   {
      Unknown,
      Core2,
      Corei7,
      Corei5,
      Corei3,
      PhenomII,
      Phenom,
      Athlon
   }

   /// <summary>
   /// GPU manufacturers.
   /// </summary>
   public enum GpuManufacturer
   {
      Unknown,
      ATI,
      Nvidia
   }

   // ReSharper restore InconsistentNaming

   #pragma warning restore 1591
}
