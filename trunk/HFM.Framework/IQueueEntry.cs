/*
 * HFM.NET - Queue Entry Interface
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using System;

namespace HFM.Framework
{
   [CLSCompliant(false)]
   public interface IQueueEntry
   {
      /// <summary>
      /// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      /// </summary>
      UInt32 Status { get; }

      /// <summary>
      /// Status Enumeration
      /// </summary>
      QueueEntryStatus EntryStatus { get; }

      /// <summary>
      /// Specifies a Factor Value denoting the Speed of Completion in relationship to the Maximum Expiration Time.
      /// </summary>
      double SpeedFactor { get; }

      /// <summary>
      /// Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use
      /// </summary>
      UInt32 UseCores { get; }

      /// <summary>
      /// Begin Time (UTC)
      /// </summary>
      DateTime BeginTimeUtc { get; }

      /// <summary>
      /// Begin Time (Local)
      /// </summary>
      DateTime BeginTimeLocal { get; }

      /// <summary>
      /// End Time (UTC)
      /// </summary>
      DateTime EndTimeUtc { get; }

      /// <summary>
      /// End Time (Local)
      /// </summary>
      DateTime EndTimeLocal { get; }

      /// <summary>
      /// Upload status (0 = Not Uploaded / 1 = Uploaded)
      /// </summary>
      UInt32 UploadStatus { get; }

      /// <summary>
      /// Web address for core downloads
      /// </summary>
      Uri CoreDownloadUrl { get; }

      /// <summary>
      /// Misc1a
      /// </summary>
      UInt32 Misc1a { get; }

      /// <summary>
      /// Core_xx number
      /// </summary>
      string CoreNumber { get; }

      /// <summary>
      /// Misc1b
      /// </summary>
      UInt32 Misc1b { get; }

      /// <summary>
      /// wudata_xx.dat file size
      /// </summary>
      UInt32 WuDataFileSize { get; }

      /// <summary>
      /// Project ID
      /// </summary>
      UInt16 ProjectID { get; }

      /// <summary>
      /// Project Run
      /// </summary>
      UInt16 ProjectRun { get; }

      /// <summary>
      /// Project Clone
      /// </summary>
      UInt16 ProjectClone { get; }

      /// <summary>
      /// Project Gen
      /// </summary>
      UInt16 ProjectGen { get; }

      /// <summary>
      /// Formatted Project (Run, Clone, Gen) Information
      /// </summary>
      string ProjectRunCloneGen { get; }

      /// <summary>
      /// Project Issued Time (UTC)
      /// </summary>
      DateTime ProjectIssuedUtc { get; }

      /// <summary>
      /// Project Issued Time (Local)
      /// </summary>
      DateTime ProjectIssuedLocal { get; }

      /// <summary>
      /// Machine ID
      /// </summary>
      UInt32 MachineID { get; }

      /// <summary>
      /// Denotes Byte Order of Machine ID Field
      /// </summary>
      bool IsMachineIDBigEndian { get; }

      /// <summary>
      /// Server IP address
      /// </summary>
      string ServerIP { get; }

      /// <summary>
      /// Server port number
      /// </summary>
      UInt32 ServerPort { get; }

      /// <summary>
      /// Work unit type
      /// </summary>
      string WorkUnitType { get; }

      /// <summary>
      /// Folding ID (User name)
      /// </summary>
      string FoldingID { get; }

      /// <summary>
      /// Team Number
      /// </summary>
      string Team { get; }

      /// <summary>
      /// Team Number
      /// </summary>
      UInt32 TeamNumber { get; }

      /// <summary>
      /// ID associated with this queue entry
      /// </summary>
      string ID { get; }

      /// <summary>
      /// User ID (unique hexadecimal value)
      /// </summary>
      string UserID { get; }

      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      UInt32 Benchmark { get; }

      /// <summary>
      /// CPU type
      /// </summary>
      UInt32 CpuType { get; }

      /// <summary>
      /// OS type
      /// </summary>
      UInt32 OsType { get; }

      /// <summary>
      /// CPU species
      /// </summary>
      UInt32 CpuSpecies { get; }

      /// <summary>
      /// OS species
      /// </summary>
      UInt32 OsSpecies { get; }

      /// <summary>
      /// CPU type (string)
      /// </summary>
      string CpuString { get; }

      /// <summary>
      /// OS type (string)
      /// </summary>
      string OsString { get; }

      /// <summary>
      /// Allowed time to return (seconds) - Final Deadline
      /// </summary>
      UInt32 ExpirationInSeconds { get; }

      /// <summary>
      /// Allowed time to return (minutes) - Final Deadline
      /// </summary>
      UInt32 ExpirationInMinutes { get; }

      /// <summary>
      /// Allowed time to return (hours) - Final Deadline
      /// </summary>
      UInt32 ExpirationInHours { get; }

      /// <summary>
      /// Allowed time to return (days) - Final Deadline
      /// </summary>
      UInt32 ExpirationInDays { get; }

      /// <summary>
      /// Assignment info present flag
      /// </summary>
      bool AssignmentInfoPresent { get; }

      /// <summary>
      /// Assignment timestamp (UTC)
      /// </summary>
      DateTime AssignmentTimeStampUtc { get; }

      /// <summary>
      /// Assignment timestamp (Local)
      /// </summary>
      DateTime AssignmentTimeStampLocal { get; }

      /// <summary>
      /// Assignment info checksum
      /// </summary>
      string AssignmentInfoChecksum { get; }

      /// <summary>
      /// Collection server IP address
      /// </summary>
      string CollectionServerIP { get; }

      /// <summary>
      /// Number of SMP cores
      /// </summary>
      UInt32 NumberOfSmpCores { get; }

      /// <summary>
      /// Tag of Work Unit
      /// </summary>
      string WorkUnitTag { get; }

      /// <summary>
      /// Passkey
      /// </summary>
      string Passkey { get; }

      /// <summary>
      /// Flops per CPU (core)
      /// </summary>
      UInt32 Flops { get; }

      /// <summary>
      /// MegaFlops per CPU (core)
      /// </summary>
      double MegaFlops { get; }

      /// <summary>
      /// Available memory (as of v6.00)
      /// </summary>
      UInt32 Memory { get; }

      /// <summary>
      /// Available GPU memory (as of v6.20)
      /// </summary>
      UInt32 GpuMemory { get; }

      /// <summary>
      /// WU expiration time (UTC)
      /// </summary>
      DateTime DueTimeUtc { get; }

      /// <summary>
      /// WU expiration time (Local)
      /// </summary>
      DateTime DueTimeLocal { get; }

      /// <summary>
      /// Packet size limit
      /// </summary>
      UInt32 PacketSizeLimit { get; }

      /// <summary>
      /// Number of upload failures
      /// </summary>
      UInt32 NumberOfUploadFailures { get; }
   }
}