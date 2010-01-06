/*
 * HFM.NET - Queue Structures
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HFM.Queue
{
   [CLSCompliant(false)]
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   public struct Queue
   {
      /* 0000 Queue (client) version (v2.17 and above) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Version;
      /// <summary>
      /// Queue (client) version
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Version
      {
         get { return _Version; }
      }

      /* 0004 Current index number */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _CurrentIndex;
      /// <summary>
      /// Current index number
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CurrentIndex
      {
         get { return _CurrentIndex; }
      }

      /* 0008 Array of ten queue entries */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
      private Entry[] _Entries;
      /// <summary>
      /// Array of ten queue entries
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public Entry[] Entries
      {
         get { return _Entries; }
      }

      /* 7128 Performance fraction (as of v3.24) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _PerformanceFraction;
      /// <summary>
      /// Performance fraction
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] PerformanceFraction
      {
         get { return _PerformanceFraction; }
      }

      /* 7132 Performance fraction unit weight (as of v3.24) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _PerformanceFractionUnitWeight;
      /// <summary>
      /// Performance fraction unit weight
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] PerformanceFractionUnitWeight
      {
         get { return _PerformanceFractionUnitWeight; }
      }

      /* 7136 Download rate sliding average (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _DownloadRateAverage;
      /// <summary>
      /// Download rate sliding average
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] DownloadRateAverage
      {
         get { return _DownloadRateAverage; }
      }

      /* 7140 Download rate unit weight (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _DownloadRateUnitWeight;
      /// <summary>
      /// Download rate unit weight
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] DownloadRateUnitWeight
      {
         get { return _DownloadRateUnitWeight; }
      }

      /* 7144 Upload rate sliding average (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _UploadRateAverage;
      /// <summary>
      /// Upload rate sliding average
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UploadRateAverage
      {
         get { return _UploadRateAverage; }
      }

      /* 7148 Upload rate unit weight (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _UploadRateUnitWeight;
      /// <summary>
      /// Upload rate unit weight
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UploadRateUnitWeight
      {
         get { return _UploadRateUnitWeight; }
      }

      /* 7152 Results successfully sent (after upload failures) (as of v5.00) (LE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _ResultsSent;
      /// <summary>
      /// Results successfully sent (after upload failures)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ResultsSent
      {
         get { return _ResultsSent; }
      }

      /* 7156 (as of v5.00) ...all zeros after queue conversion... */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
      private string _z7156;
      /// <summary>
      /// (as of v5.00) ...all zeros after queue conversion...
      /// </summary>
      public string z7156
      {
         get { return _z7156; }
      }
   }

   [CLSCompliant(false)]
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   public struct Entry
   {
      /*** 0 = Empty, Deleted, Finished, or Garbage 
       *   1 = Folding Now or Queued 
       *   2 = Ready for Upload 
       *   3 = Abandonded (Ignore is found)
       *   4 = Fetching from Server
       ***/
      /* 000 Status */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Status;
      /// <summary>
      /// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Status
      {
         get { return _Status; }
      }

      /* 004 Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use (LE) */
      private UInt32 _UseCores;
      /// <summary>
      /// Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use
      /// </summary>
      public UInt32 UseCores
      {
         get { return _UseCores; }
      }

      /*** 0 = Begin Time
       *   4 = End Time 
       *   Others = Unknown 
       ***/
      /* 008 Time data (epoch 0000 1jan00 UTC) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      private UInt32[] _TimeData;
      /// <summary>
      /// Time data (epoch 0000 1jan00 UTC)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public UInt32[] TimeData
      {
         get { return _TimeData; }
      }

      /* 040 Server IP address (until v3.0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _OldServerIP; /*** Ignore this value ***/
      /// <summary>
      /// Server IP address (until v3.0 - Ignore this value)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] OldServerIP
      {
         get { return _OldServerIP; }
      }

      /***
       * 0 = Not Uploaded
       * 1 = Uploaded
       ***/
      /* 044 Upload status */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _UploadStatus;
      /// <summary>
      /// Upload status (0 = Not Uploaded / 1 = Uploaded)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UploadStatus
      {
         get { return _UploadStatus; }
      }

      /* 048 Web address for core downloads */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      private string _CoreDownloadUrl;
      /// <summary>
      /// Web address for core downloads
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
      public string CoreDownloadUrl
      {
         get { return _CoreDownloadUrl; }
      }

      /* 176 Misc1a */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Misc1a;
      /// <summary>
      /// Misc1a
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Misc1a
      {
         get { return _Misc1a; }
      }

      /* 180 Core_xx number (hex) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _CoreNumber; /*** Convert to Hex ***/
      /// <summary>
      /// Core_xx number
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CoreNumber
      {
         get { return _CoreNumber; }
      }

      /* 184 Misc1b */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Misc1b;
      /// <summary>
      /// Misc1b
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Misc1b
      {
         get { return _Misc1b; }
      }

      /* 188 wudata_xx.dat file size */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _WuDataFileSize;
      /// <summary>
      /// wudata_xx.dat file size
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] WuDataFileSize
      {
         get { return _WuDataFileSize; }
      }

      /* 192 */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      private string _z192;
      /// <summary>
      /// z192
      /// </summary>
      public string z192
      {
         get { return _z192; }
      }

      /* 208 Project number (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectID;

      /* 210 Run (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectRun;

      /* 212 Clone (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectClone;

      /* 214 Generation (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectGen;

      /* 216 WU issue time (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      //public string ProjectIssued;

      /* 208-223 Project R/C/G (see above) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      private byte[] _Project;
      /// <summary>
      /// Project R/C/G and Issued Time
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Project
      {
         get { return _Project; }
      }

      /* 224 */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
      private string _z224;
      /// <summary>
      /// z224
      /// </summary>
      public string z224
      {
         get { return _z224; }
      }

      /* 260 Machine ID (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _MachineID;
      /// <summary>
      /// Machine ID
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] MachineID
      {
         get { return _MachineID; }
      }

      /* 264 Server IP address */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _ServerIP;
      /// <summary>
      /// Server IP address
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ServerIP
      {
         get { return _ServerIP; }
      }

      /* 268 Server port number */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _ServerPort;
      /// <summary>
      /// Server port number
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ServerPort
      {
         get { return _ServerPort; }
      }

      /* 272 Work unit type */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      private string _WorkUnitType;
      /// <summary>
      /// Work unit type
      /// </summary>
      public string WorkUnitType
      {
         get { return _WorkUnitType; }
      }

      /* 336 User Name */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      private string _FoldingID;
      /// <summary>
      /// The Folding ID (Username) attached to this queue entry
      /// </summary>
      public string FoldingID
      {
         get { return _FoldingID; }
      }

      /* 400 Team Number */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      private string _Team;
      /// <summary>
      /// The Team number attached to this queue entry
      /// </summary>
      public string Team
      {
         get { return _Team; }
      }

      /* 464 Stored ID for unit (UserID + MachineID) (LE or BE, usually BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      private byte[] _UserAndMachineID;
      /// <summary>
      /// Stored ID for unit (UserID + MachineID)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UserAndMachineID
      {
         get { return _UserAndMachineID; }
      }

      /* 472 Benchmark (until v3.24) (LE) */
      private UInt32 _OldBenchmark; /*** Ignore this value ***/
      /// <summary>
      /// Benchmark (until v3.24 - Ignore this value)
      /// </summary>
      public UInt32 OldBenchmark
      {
         get { return _OldBenchmark; }
      }

      /* 476 Misc3b (unused as of v3.24) (LE); Benchmark (as of v5.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Benchmark;
      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Benchmark
      {
         get { return _Benchmark; }
      }

      /* 480 CPU type (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _CpuType;
      /// <summary>
      /// CPU type
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CpuType
      {
         get { return _CpuType; }
      }

      /* 484 OS type (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _OsType;
      /// <summary>
      /// OS type
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] OsType
      {
         get { return _OsType; }
      }

      /* 488 CPU species (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _CpuSpecies;
      /// <summary>
      /// CPU species
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CpuSpecies
      {
         get { return _CpuSpecies; }
      }

      /* 492 OS species (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _OsSpecies;
      /// <summary>
      /// OS species
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] OsSpecies
      {
         get { return _OsSpecies; }
      }

      /* 496 Allowed time to return (seconds) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _ExpirationInSeconds; /*** Final Deadline ***/
      /// <summary>
      /// Allowed time to return (seconds) - Final Deadline
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ExpirationInSeconds
      {
         get { return _ExpirationInSeconds; }
      }

      /* 500 */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      private string _z500;
      /// <summary>
      /// z500
      /// </summary>
      public string z500
      {
         get { return _z500; }
      }

      /* 508 Assignment info present flag (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _AssignmentInfoPresent;
      /// <summary>
      /// Assignment info present flag
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] AssignmentInfoPresent
      {
         get { return _AssignmentInfoPresent; }
      }

      /* 512 Assignment timestamp (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _AssignmentTimeStamp;
      /// <summary>
      /// Assignment timestamp
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] AssignmentTimeStamp
      {
         get { return _AssignmentTimeStamp; }
      }

      /* 516 Assignment info checksum (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _AssignmentInfoChecksum;
      /// <summary>
      /// Assignment info checksum
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] AssignmentInfoChecksum
      {
         get { return _AssignmentInfoChecksum; }
      }

      /* 520 Collection server IP address (as of v5.00) (LE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _CollectionServerIP;
      /// <summary>
      /// Collection server IP address
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CollectionServerIP
      {
         get { return _CollectionServerIP; }
      }

      /* 524 Download started time (as of v5.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _DownloadStartedTime;
      /// <summary>
      /// Download started time
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] DownloadStartedTime
      {
         get { return _DownloadStartedTime; }
      }

      /* 528 */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      private string _z528;
      /// <summary>
      /// z528
      /// </summary>
      public string z528
      {
         get { return _z528; }
      }

      /* 544 Number of SMP cores (as of v5.91) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _NumberOfSmpCores;
      /// <summary>
      /// Number of SMP cores
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] NumberOfSmpCores
      {
         get { return _NumberOfSmpCores; }
      }

      /* 548 Tag of Work Unit (as of v5.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      private byte[] _WorkUnitTag;
      /// <summary>
      /// Tag of Work Unit
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] WorkUnitTag
      {
         get { return _WorkUnitTag; }
      }

      /* 564 */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      private string _z564;
      /// <summary>
      /// z564
      /// </summary>
      public string z564
      {
         get { return _z564; }
      }

      /* 580 Passkey (as of v6.00) */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      private string _Passkey;
      /// <summary>
      /// Passkey
      /// </summary>
      public string Passkey
      {
         get { return _Passkey; }
      }

      /* 612 Flops per CPU (core) (as of v6.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Flops;
      /// <summary>
      /// Flops per CPU (core)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Flops
      {
         get { return _Flops; }
      }

      /* 616 Available memory (as of v6.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Memory;
      /// <summary>
      /// Available memory (as of v6.00)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Memory
      {
         get { return _Memory; }
      }

      /* 620 Available GPU memory (as of v6.20) (LE) */
      private UInt32 _GpuMemory;
      /// <summary>
      /// Available GPU memory (as of v6.20)
      /// </summary>
      public UInt32 GpuMemory
      {
         get { return _GpuMemory; }
      }

      /* 624 */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      private string _z624;
      /// <summary>
      /// z624
      /// </summary>
      public string z624
      {
         get { return _z624; }
      }

      /***
       * 0 = Due Date - This time is calculated by the client when it downloads a unit.
       *                It is determined by adding the "begin" time to the expiration period.
       * 1-3 = Unknown
       ***/
      /* 688 WU expiration time */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private UInt32[] _ExpirationTime;
      /// <summary>
      /// WU expiration time (0 = Due Date / 1-3 = Unknown)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public UInt32[] ExpirationTime
      {
         get { return _ExpirationTime; }
      }

      /* 704 Packet size limit (as of v5.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _PacketSizeLimit;
      /// <summary>
      /// Packet size limit
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] PacketSizeLimit
      {
         get { return _PacketSizeLimit; }
      }

      /* 708 Number of upload failures (as of v5.00) */
      private UInt32 _NumberOfUploadFailures;
      /// <summary>
      /// Number of upload failures
      /// </summary>
      public UInt32 NumberOfUploadFailures
      {
         get { return _NumberOfUploadFailures; }
      }
   }

   #region struct layout from qd.c (9/10/2009) (http://linuxminded.nl/?target=software-qd-tools.plc)
   //u32		version;	 /* 0000 Queue (client) version (v2.17 and above) */
   //u32		current;	 /* 0004 Current index number */
   //struct qs
   //{	u32	stat;		 /* 000 Status */
   //   char	use_cores[4];	 /* 004 Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use (LE) */
   //   u32	tdata[8];	 /* 008 Time data (epoch 0000 1jan00 UTC) */
   //   u32	svr1;		 /* 040 Server IP address (until v3.0) */
   //   u32	ustat;		 /* 044 Upload status */
   //   char	url[128];	 /* 048 Web address for core downloads */
   //   u32	m176;		 /* 176 Misc1a */
   //   u32	core;		 /* 180 Core_xx number (hex) */
   //   u32	m184;		 /* 184 Misc1b */
   //   u32	dsiz;		 /* 188 wudata_xx.dat file size */
   //   char	z192[16];
   //   union
   //   {	struct
   //      {	char	proj[2];	/* 208 Project number (LE) */
   //         char	run[2];		/* 210 Run (LE) */
   //         char	clone[2];	/* 212 Clone (LE) */
   //         char	gen[2];		/* 214 Generation (LE) */
   //         char	issue[2][4];	/* 216 WU issue time (LE) */
   //      }	f;			/* Folding@home data */
   //      struct
   //      {	char	proj[2];	/* 208 Project number (LE) */
   //         u16	miscg1;		/* 210 Miscg1 */
   //         char	issue[2][4];	/* 212 WU issue time (LE) */
   //         u16	miscg2;		/* 220 Miscg2 */
   //         u16	miscg3;		/* 222 Miscg3 */
   //      }	g;			/* Genome@home data */
   //   }	wuid;		 /* 208 Work unit ID information */
   //   char	z224[36];
   //   char	mid[4];		 /* 260 Machine ID (LE) */
   //   u32	svr2;		 /* 264 Server IP address */
   //   u32	port;		 /* 268 Server port number */
   //   char	type[64];	 /* 272 Work unit type */
   //   char	uname[64];	 /* 336 User Name */
   //   char	teamn[64];	 /* 400 Team Number */
   //   char	uid[8];		 /* 464 Stored ID for unit (UserID + MachineID) (LE or BE, usually BE) */
   //   char	bench[4];	 /* 472 Benchmark (as of v3.24) (LE) */
   //   char	m476[4];	 /* 476 Misc3b (unused as of v3.24) (LE); Benchmark (as of v5.00) (BE) */
   //   u32	cpu_type;	 /* 480 CPU type (LE or BE, sometimes 0) */
   //   u32	os_type;	 /* 484 OS type (LE or BE, sometimes 0) */
   //   u32	cpu_spec;	 /* 488 CPU species (LE or BE, sometimes 0) */
   //   u32	os_spec;	 /* 492 OS species (LE or BE, sometimes 0) */
   //   u32	expire;		 /* 496 Allowed time to return (seconds) */
   //   char	z500[8];
   //   char	aiflag[4];	 /* 508 Assignment info present flag (LE or BE) */
   //   char	aitime[4];	 /* 512 Assignment timestamp (LE or BE) */
   //   char	aidata[4];	 /* 516 Assignment info (LE or BE) */
   //   char	csip[4];	 /* 520 Collection server IP address (as of v5.00) (LE) */
   //   char	dstart[4];	 /* 524 Download started time (as of v5.00) (BE) */
   //   char	z528[16];
   //   char    cores[4];	 /* 544 Number of SMP cores (as of v5.91) (BE) */
   //   char    tag[16];         /* 548 Tag of Work Unit (as of v5.00) */
   //   char    z564[16];
   //   char    passkey[32];     /* 580 Passkey (as of v6.00) */
   //   char    flops[4];        /* 612 Flops per CPU (core) (as of v6.00) (BE) */
   //   char    memory[4];       /* 616 Available memory (as of v6.00) (BE) */
   //   char    gpu_memory[4];	 /* 620 Available GPU memory (as of v6.20) (LE) */
   //   char    z624[64];
   //   u32	due[4];		 /* 688 WU expiration time */
   //   u32	plimit;		 /* 704 Packet size limit (as of v5.00) */
   //   u32	uploads;	 /* 708 Number of upload failures (as of v5.00) */
   //}		entry[10];	 /* 0008 Array of ten queue entries */
   //u32		pfract;		 /* 7128 Performance fraction (as of v3.24) */
   //u32		punits;		 /* 7132 Performance fraction unit weight (as of v3.24) */
   //u32		drate;		 /* 7136 Download rate sliding average (as of v4.00) */
   //u32		dunits;		 /* 7140 Download rate unit weight (as of v4.00) */
   //u32		urate;		 /* 7144 Upload rate sliding average (as of v4.00) */
   //u32		uunits;		 /* 7148 Upload rate unit weight (as of v4.00) */
   //char		results_sent[4]; /* 7152 Results successfully sent (after upload failures) (as of v5.00) (LE) */
   //char		z7156[12];       /* 7156 (as of v5.00) ...all zeros after queue conversion... */
   #endregion
}
