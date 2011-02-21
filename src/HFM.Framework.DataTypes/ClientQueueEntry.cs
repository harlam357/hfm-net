/*
 * HFM.NET - Client Queue Entry Class
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

      /// <summary>
      /// Status Enumeration
      /// </summary>
      public QueueEntryStatus EntryStatus { get; set; }

      /// <summary>
      /// Specifies a Factor Value denoting the Speed of Completion in relationship to the Maximum Expiration Time.
      /// </summary>
      public double SpeedFactor { get; set; }

      /// <summary>
      /// Pad for Windows, others as of v4.01, as of v6.01 Number of SMP cores
      /// </summary>
      public int NumberOfSmpCores { get; set; }

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

      /// <summary>
      /// Machine ID
      /// </summary>
      public int MachineID { get; set; }

      /// <summary>
      /// Server IP address
      /// </summary>
      public string ServerIP { get; set; }

      /// <summary>
      /// User ID (unique hexadecimal value)
      /// </summary>
      public string UserID { get; set; }

      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      public int Benchmark { get; set; }

      /// <summary>
      /// CPU type (string)
      /// </summary>
      public string CpuString { get; set; }

      /// <summary>
      /// OS type (string)
      /// </summary>
      public string OsString { get; set; }

      /// <summary>
      /// Number of SMP cores to use
      /// </summary>
      public int UseCores { get; set; }

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

      #endregion
   }
}
