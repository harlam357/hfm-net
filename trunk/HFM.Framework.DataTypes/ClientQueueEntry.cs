/*
 * HFM.NET - Client Queue Entry Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

/*
 * This class is currently populated by the queue.dat data only.
 * 
 * The reason for its existance is to provide a buffer for the
 * queue.dat data coming from v6 and below clients as well as a
 * buffer for the data (presumably different) coming from v7 clients.
 * 
 * This class also provides a concrete type that can be serailized
 * into a client data file.  Previously the queue information that
 * was available to a DisplayInstance was of type IQueueBase which
 * is not serializable by protobuf-net.
 */

using System;

namespace HFM.Framework.DataTypes
{
   /// <summary>
   /// Data class used to hold queue information for display to the user
   /// </summary>
   /// <remarks></remarks>
   public class ClientQueueEntry : IProjectInfo
   {
      #region queue.dat Properties

      ///// <summary>
      ///// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      ///// </summary>
      //public int Status { get; set; }

      /// <summary>
      /// Status Enumeration
      /// </summary>
      public QueueEntryStatus EntryStatus { get; set; }

      /// <summary>
      /// Specifies a Factor Value denoting the Speed of Completion in relationship to the Maximum Expiration Time.
      /// </summary>
      public double SpeedFactor { get; set; }

      /// <summary>
      /// Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use
      /// </summary>
      public int UseCores { get; set; }

      /// <summary>
      /// Begin Time (UTC)
      /// </summary>
      public DateTime BeginTimeUtc { get; set; }

      /// <summary>
      /// Begin Time (Local)
      /// </summary>
      public DateTime BeginTimeLocal { get; set; }

      /// <summary>
      /// End Time (UTC)
      /// </summary>
      public DateTime EndTimeUtc { get; set; }

      /// <summary>
      /// End Time (Local)
      /// </summary>
      public DateTime EndTimeLocal { get; set; }

      ///// <summary>
      ///// Upload status (0 = Not Uploaded / 1 = Uploaded)
      ///// </summary>
      //public int UploadStatus { get; set; }

      ///// <summary>
      ///// Web address for core downloads
      ///// </summary>
      //public Uri CoreDownloadUrl { get; set; }

      ///// <summary>
      ///// Misc1a
      ///// </summary>
      //public UInt32 Misc1a { get; set; }

      ///// <summary>
      ///// Core_xx number
      ///// </summary>
      //public string CoreNumber { get; set; }

      ///// <summary>
      ///// Misc1b
      ///// </summary>
      //public UInt32 Misc1b { get; set; }

      ///// <summary>
      ///// wudata_xx.dat file size
      ///// </summary>
      //public int WuDataFileSize { get; set; }

      /// <summary>
      /// Project ID
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Project Run
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Project Clone
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Project Gen
      /// </summary>
      public int ProjectGen { get; set; }

      ///// <summary>
      ///// Project Issued Time (UTC)
      ///// </summary>
      //public DateTime ProjectIssuedUtc { get; set; }

      ///// <summary>
      ///// Project Issued Time (Local)
      ///// </summary>
      //public DateTime ProjectIssuedLocal { get; set; }

      /// <summary>
      /// Machine ID
      /// </summary>
      public int MachineID { get; set; }

      ///// <summary>
      ///// Denotes Byte Order of Machine ID Field
      ///// </summary>
      //public bool IsMachineIDBigEndian { get; set; }

      /// <summary>
      /// Server IP address
      /// </summary>
      public string ServerIP { get; set; }

      ///// <summary>
      ///// Server port number
      ///// </summary>
      //public int ServerPort { get; set; }

      ///// <summary>
      ///// Work unit type
      ///// </summary>
      //public string WorkUnitType { get; set; }

      ///// <summary>
      ///// Folding ID (User name)
      ///// </summary>
      //public string FoldingID { get; set; }

      ///// <summary>
      ///// Team Number
      ///// </summary>
      //public string Team { get; set; }

      ///// <summary>
      ///// Team Number
      ///// </summary>
      //public int TeamNumber { get; set; }

      ///// <summary>
      ///// ID associated with this queue entry
      ///// </summary>
      //public string ID { get; set; }

      /// <summary>
      /// User ID (unique hexadecimal value)
      /// </summary>
      public string UserID { get; set; }

      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      public int Benchmark { get; set; }

      ///// <summary>
      ///// CPU type
      ///// </summary>
      //public int CpuType { get; set; }

      ///// <summary>
      ///// OS type
      ///// </summary>
      //public int OsType { get; set; }

      ///// <summary>
      ///// CPU species
      ///// </summary>
      //public int CpuSpecies { get; set; }

      ///// <summary>
      ///// OS species
      ///// </summary>
      //public int OsSpecies { get; set; }

      /// <summary>
      /// CPU type (string)
      /// </summary>
      public string CpuString { get; set; }

      /// <summary>
      /// OS type (string)
      /// </summary>
      public string OsString { get; set; }

      ///// <summary>
      ///// Allowed time to return (seconds) - Final Deadline
      ///// </summary>
      //public int ExpirationInSeconds { get; set; }

      ///// <summary>
      ///// Allowed time to return (minutes) - Final Deadline
      ///// </summary>
      //public int ExpirationInMinutes { get; set; }

      ///// <summary>
      ///// Allowed time to return (hours) - Final Deadline
      ///// </summary>
      //public int ExpirationInHours { get; set; }

      ///// <summary>
      ///// Allowed time to return (days) - Final Deadline
      ///// </summary>
      //public int ExpirationInDays { get; set; }

      ///// <summary>
      ///// Assignment info present flag
      ///// </summary>
      //public bool AssignmentInfoPresent { get; set; }

      ///// <summary>
      ///// Assignment timestamp (UTC)
      ///// </summary>
      //public DateTime AssignmentTimeStampUtc { get; set; }

      ///// <summary>
      ///// Assignment timestamp (Local)
      ///// </summary>
      //public DateTime AssignmentTimeStampLocal { get; set; }

      ///// <summary>
      ///// Assignment info checksum
      ///// </summary>
      //public string AssignmentInfoChecksum { get; set; }

      ///// <summary>
      ///// Collection server IP address
      ///// </summary>
      //public string CollectionServerIP { get; set; }

      /// <summary>
      /// Number of SMP cores
      /// </summary>
      public int NumberOfSmpCores { get; set; }

      ///// <summary>
      ///// Tag of Work Unit
      ///// </summary>
      //public string WorkUnitTag { get; set; }

      ///// <summary>
      ///// Passkey
      ///// </summary>
      //public string Passkey { get; set; }

      ///// <summary>
      ///// Flops per CPU (core)
      ///// </summary>
      //public int Flops { get; set; }

      /// <summary>
      /// MegaFlops per CPU (core)
      /// </summary>
      public double MegaFlops { get; set; }

      /// <summary>
      /// Available memory (as of v6.00)
      /// </summary>
      public int Memory { get; set; }

      /// <summary>
      /// Available GPU memory (as of v6.20)
      /// </summary>
      public int GpuMemory { get; set; }

      ///// <summary>
      ///// WU expiration time (UTC)
      ///// </summary>
      //public DateTime DueTimeUtc { get; set; }

      ///// <summary>
      ///// WU expiration time (Local)
      ///// </summary>
      //public DateTime DueTimeLocal { get; set; }

      ///// <summary>
      ///// Packet size limit
      ///// </summary>
      //public int PacketSizeLimit { get; set; }

      ///// <summary>
      ///// Number of upload failures
      ///// </summary>
      //public int NumberOfUploadFailures { get; set; }
      
      #endregion

      /// <summary>
      /// Formatted Project (Run, Clone, Gen) Information
      /// </summary>
      public string ProjectRunCloneGen
      {
         get
         {
            return String.Format("P{0} (R{1}, C{2}, G{3})", ProjectID,
                                                            ProjectRun,
                                                            ProjectClone,
                                                            ProjectGen);
         }
      }
   }
}
