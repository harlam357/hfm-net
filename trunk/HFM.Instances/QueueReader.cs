/*
 * HFM.NET - Queue Reader Class
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
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace HFM.Instances
{
   /// <summary>
   /// The Queue Entry Status
   /// </summary>
   public enum QueueEntryStatus
   {
      Unknown = -1,
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
   
   [CLSCompliant(false)]
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 7168)]
   public struct Queue
   {
      /* 0000 Queue (client) version (v2.17 and above) */
      private UInt32 _Version;
      /// <summary>
      /// Queue (client) version
      /// </summary>
      public UInt32 Version
      {
         get { return _Version; }
      }

      /* 0004 Current index number */
      private UInt32 _CurrentIndex;
      /// <summary>
      /// Current index number
      /// </summary>
      public UInt32 CurrentIndex
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
      private float _PerformanceFraction;
      /// <summary>
      /// Performance fraction
      /// </summary>
      public float PerformanceFraction
      {
         get { return _PerformanceFraction; }
      }
      
      /* 7132 Performance fraction unit weight (as of v3.24) */
      private UInt32 _PerformanceFractionUnitWeight;
      /// <summary>
      /// Performance fraction unit weight
      /// </summary>
      public UInt32 PerformanceFractionUnitWeight
      {
         get { return _PerformanceFractionUnitWeight; }
      }
      
      /* 7136 Download rate sliding average (as of v4.00) */
      private UInt32 _DownloadRateAverage;
      /// <summary>
      /// Download rate sliding average
      /// </summary>
      public UInt32 DownloadRateAverage
      {
         get { return _DownloadRateAverage; }
      }
     
      /* 7140 Download rate unit weight (as of v4.00) */
      private UInt32 _DownloadRateUnitWeight;
      /// <summary>
      /// Download rate unit weight
      /// </summary>
      public UInt32 DownloadRateUnitWeight
      {
         get { return _DownloadRateUnitWeight; }
      }
      
      /* 7144 Upload rate sliding average (as of v4.00) */
      private UInt32 _UploadRateAverage;
      /// <summary>
      /// Upload rate sliding average
      /// </summary>
      public UInt32 UploadRateAverage
      {
         get { return _UploadRateAverage; }
      }
      
      /* 7148 Upload rate unit weight (as of v4.00) */
      private UInt32 _UploadRateUnitWeight;
      /// <summary>
      /// Upload rate unit weight
      /// </summary>
      public UInt32 UploadRateUnitWeight
      {
         get { return _UploadRateUnitWeight; }
      }
      
      /* 7152 Results successfully sent (after upload failures) (as of v5.00) (LE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _ResultsSent;
      /// <summary>
      /// Results successfully sent (after upload failures)
      /// </summary>
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
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 712)]
   public struct Entry
   {
      /*** 0 = Empty, Deleted, Finished, or Garbage 
       *   1 = Folding Now or Queued 
       *   2 = Ready for Upload 
       *   3 = Abandonded (Ignore is found)
       *   4 = Fetching from Server
       ***/ 
      /* 000 Status */
      private UInt32 _Status;
      /// <summary>
      /// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      /// </summary>
      public UInt32 Status
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
      private UInt32 _UploadStatus;
      /// <summary>
      /// Upload status (0 = Not Uploaded / 1 = Uploaded)
      /// </summary>
      public UInt32 UploadStatus
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
      private UInt32 _Misc1a;
      /// <summary>
      /// Misc1a
      /// </summary>
      public UInt32 Misc1a
      {
         get { return _Misc1a; }
      }
      
      /* 180 Core_xx number (hex) */
      private UInt32 _CoreNumber; /*** Convert to Hex ***/
      /// <summary>
      /// Core_xx number
      /// </summary>
      public UInt32 CoreNumber
      {
         get { return _CoreNumber; }
      }

      /* 184 Misc1b */
      private UInt32 _Misc1b;
      /// <summary>
      /// Misc1b
      /// </summary>
      public UInt32 Misc1b
      {
         get { return _Misc1b; }
      }
      
      /* 188 wudata_xx.dat file size */
      private UInt32 _WuDataFileSize;
      /// <summary>
      /// wudata_xx.dat file size
      /// </summary>
      public UInt32 WuDataFileSize
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
      private byte[] _Project; /*** Needs post read processing ***/
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

      /* 260 Machine ID (LE) */
      private UInt32 _MachineID;
      /// <summary>
      /// Machine ID
      /// </summary>
      public UInt32 MachineID
      {
         get { return _MachineID; }
      }

      /* 264 Server IP address */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _ServerIP; /*** Needs post read processing ***/
      /// <summary>
      /// Server IP address
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ServerIP
      {
         get { return _ServerIP; }
      }

      /* 268 Server port number */
      private UInt32 _ServerPort;
      /// <summary>
      /// Server port number
      /// </summary>
      public UInt32 ServerPort
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
      private byte[] _UserAndMachineID; /*** Needs post read processing ***/
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
      private byte[] _Benchmark; /*** Needs post read processing ***/
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
      private UInt32 _ExpirationInSeconds; /*** Final Deadline ***/
      /// <summary>
      /// Allowed time to return (seconds) - Final Deadline
      /// </summary>
      public UInt32 ExpirationInSeconds
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
      private byte[] _CollectionServerIP; /*** Needs post read processing ***/
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
      private byte[] _NumberOfSmpCores; /*** Needs post read processing ***/
      /// <summary>
      /// Number of SMP cores
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] NumberOfSmpCores
      {
         get { return _NumberOfSmpCores; }
      }

      /* 548 Tag of Work Unit (as of v5.00) */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      private string _WorkUnitTag;
      /// <summary>
      /// Tag of Work Unit
      /// </summary>
      public string WorkUnitTag
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
      private byte[] _Flops; /*** Needs post read processing ***/
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
      private byte[] _Memory; /*** Needs post read processing ***/
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
      private UInt32 _PacketSizeLimit;
      /// <summary>
      /// Packet size limit
      /// </summary>
      public UInt32 PacketSizeLimit
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

   [CLSCompliant(false)]
   public class QueueEntry
   {
      /// <summary>
      /// Wrapped Entry Structure
      /// </summary>
      private Entry _qEntry;
      /// <summary>
      /// This Entry Index
      /// </summary>
      private readonly UInt32 _thisIndex;
      /// <summary>
      /// Current Entry Index
      /// </summary>
      private readonly UInt32 _currentIndex;
   
      /// <summary>
      /// Primary Constructor
      /// </summary>
      /// <param name="qEntry">Entry Structure</param>
      /// <param name="thisIndex">This Entry Index</param>
      /// <param name="currentIndex">Current Entry Index</param>
      public QueueEntry(Entry qEntry, UInt32 thisIndex, UInt32 currentIndex)
      {
         _qEntry = qEntry;
         _thisIndex = thisIndex;
         _currentIndex = currentIndex;
      }

      /// <summary>
      /// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      /// </summary>
      public UInt32 Status
      {
         get { return _qEntry.Status; }
      } 
      
      /// <summary>
      /// Status Enumeration
      /// </summary>
      public QueueEntryStatus EntryStatus
      {
         get
         {
            switch (_qEntry.Status)
            {
               case 0:
                  if (ProjectID == 0)
                  {
                     /* The queue entry has never been used, or has been completely cleared. */
                     return QueueEntryStatus.Empty;
                  }
                  else if (UploadStatus == 0)
                  {
                     /* The unit was explicitly deleted. */
                     return QueueEntryStatus.Deleted;
                  }
                  else if (UploadStatus == 1)
                  {
                     /* The unit has been uploaded.  The queue entry is just history. */
                     return QueueEntryStatus.Finished;
                  }
                  else
                  {
                     /* The queue entry is available, but its history is unintelligible. */
                     return QueueEntryStatus.Garbage;
                  }
               case 1:
                  if (_thisIndex == _currentIndex)
                  {
                     /* The unit is in progress.  Presumably the core is running. */
                     return QueueEntryStatus.FoldingNow;
                  }
                  else
                  {
                     /* The unit has been downloaded but processing hasn't begun yet. */
                     return QueueEntryStatus.Queued;
                  }
               case 2:
                  /* The core has finished the unit, but it is still in the queue. */
                  return QueueEntryStatus.ReadyForUpload;
               case 3: /* Bug before V3b5, neglected to post status (1). */
                  return QueueEntryStatus.Abandonded;
               case 4:
                  /* Client presently contacting the server, or something failed in download.
			          * If this state persists past the current unit, the queue entry will be
			          * unusable, but otherwise things will go on as usual.
			          */
                  return QueueEntryStatus.FetchingFromServer;
               default:
                  /* Something other than 0 to 4. */
                  return QueueEntryStatus.Unknown;
            }
         }
      }

      /// <summary>
      /// Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use
      /// </summary>
      public UInt32 UseCores
      {
         get { return _qEntry.UseCores; }
      } 

      /// <summary>
      /// Begin Time (UTC)
      /// </summary>
      public DateTime BeginTimeUtc
      {
         get 
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(_qEntry.TimeData[0]);
         }
      }

      /// <summary>
      /// Begin Time (Local)
      /// </summary>
      public DateTime BeginTimeLocal
      {
         get
         {
            return BeginTimeUtc.ToLocalTime();
         }
      }

      /// <summary>
      /// End Time (UTC)
      /// </summary>
      public DateTime EndTimeUtc
      {
         get
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(_qEntry.TimeData[4]);
         }
      }

      /// <summary>
      /// End Time (Local)
      /// </summary>
      public DateTime EndTimeLocal
      {
         get
         {
            return EndTimeUtc.ToLocalTime();
         }
      }

      /// <summary>
      /// Upload status (0 = Not Uploaded / 1 = Uploaded)
      /// </summary>
      public UInt32 UploadStatus
      {
         get { return _qEntry.UploadStatus; }
      }

      /// <summary>
      /// Web address for core downloads
      /// </summary>
      public Uri CoreDownloadUrl
      {
         get { return new Uri(String.Format("http://{0}/Core_{1}.fah", _qEntry.CoreDownloadUrl, CoreNumber)); }
      }

      /// <summary>
      /// Misc1a
      /// </summary>
      public UInt32 Misc1a
      {
         get { return _qEntry.Misc1a; }
      }

      /// <summary>
      /// Core_xx number
      /// </summary>
      public string CoreNumber
      {
         get { return _qEntry.CoreNumber.ToString("x"); }
      }

      /// <summary>
      /// Misc1b
      /// </summary>
      public UInt32 Misc1b
      {
         get { return _qEntry.Misc1b; }
      }

      /// <summary>
      /// wudata_xx.dat file size
      /// </summary>
      public UInt32 WuDataFileSize
      {
         get { return _qEntry.WuDataFileSize; }
      }

      /// <summary>
      /// Project ID
      /// </summary>
      public UInt16 ProjectID
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 0, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

      /// <summary>
      /// Project Run
      /// </summary>
      public UInt16 ProjectRun
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 2, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

      /// <summary>
      /// Project Clone
      /// </summary>
      public UInt16 ProjectClone
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 4, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

      /// <summary>
      /// Project Gen
      /// </summary>
      public UInt16 ProjectGen
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 6, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

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

      /// <summary>
      /// Project Issued Time (UTC)
      /// </summary>
      public DateTime ProjectIssuedUtc
      {
         get
         {
            byte[] b = new byte[4];
            Array.Copy(_qEntry.Project, 8, b, 0, 4);
            UInt32 seconds = BitConverter.ToUInt32(b, 0);
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(seconds);
         }
      }

      /// <summary>
      /// Project Issued Time (Local)
      /// </summary>
      public DateTime ProjectIssuedLocal
      {
         get
         {
            return ProjectIssuedUtc.ToLocalTime();
         }
      }

      /// <summary>
      /// Machine ID
      /// </summary>
      public UInt32 MachineID
      {
         get { return _qEntry.MachineID; }
      }

      /// <summary>
      /// Server IP address
      /// </summary>
      public string ServerIP
      {
         get
         {
            return String.Format("{0}.{1}.{2}.{3}", _qEntry.ServerIP[3], _qEntry.ServerIP[2], 
                                                    _qEntry.ServerIP[1], _qEntry.ServerIP[0]);
         }
      }

      /// <summary>
      /// Server port number
      /// </summary>
      public UInt32 ServerPort
      {
         get { return _qEntry.ServerPort; }
      }

      /// <summary>
      /// Work unit type
      /// </summary>
      public string WorkUnitType
      {
         get { return _qEntry.WorkUnitType; }
      }

      /// <summary>
      /// The Folding ID (Username) attached to this queue entry
      /// </summary>
      public string FoldingID
      {
         get { return _qEntry.FoldingID; }
      }

      /// <summary>
      /// The Team number attached to this queue entry
      /// </summary>
      public string Team
      {
         get { return _qEntry.Team; }
      }

      /// <summary>
      /// User ID associated with this queue entry
      /// </summary>
      public string UserID
      {
         get
         {
            StringBuilder sb = new StringBuilder(_qEntry.UserAndMachineID.Length * 2);
            foreach (byte b in _qEntry.UserAndMachineID)
            {
               sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
         }
      }

      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      public UInt32 Benchmark
      {
         get 
         {
            byte[] b = new byte[_qEntry.Benchmark.Length];
            Array.Copy(_qEntry.Benchmark, b, _qEntry.Benchmark.Length);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// CPU type
      /// </summary>
      public UInt32 CpuType
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.CpuType);
         }
      }

      /// <summary>
      /// OS type
      /// </summary>
      public UInt32 OsType
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.OsType);
         }
      }

      /// <summary>
      /// CPU species
      /// </summary>
      public UInt32 CpuSpecies
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.CpuSpecies);
         }
      }

      /// <summary>
      /// OS species
      /// </summary>
      public UInt32 OsSpecies
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.OsSpecies);
         }
      }

      /// <summary>
      /// CPU type (string)
      /// </summary>
      public string CpuString
      {
         get { return GetCpuString(_qEntry.CpuType, _qEntry.CpuSpecies); }
      }

      /// <summary>
      /// OS type (string)
      /// </summary>
      public string OsString
      {
         get { return GetOsString(_qEntry.OsType, _qEntry.OsSpecies); }
      }

      /// <summary>
      /// Allowed time to return (seconds) - Final Deadline
      /// </summary>
      public UInt32 ExpirationInSeconds
      {
         get { return _qEntry.ExpirationInSeconds; }
      }

      /// <summary>
      /// Allowed time to return (minutes) - Final Deadline
      /// </summary>
      public UInt32 ExpirationInMinutes
      {
         get { return ExpirationInSeconds / 60; }
      }

      /// <summary>
      /// Allowed time to return (hours) - Final Deadline
      /// </summary>
      public UInt32 ExpirationInHours
      {
         get { return ExpirationInMinutes / 60; }
      }

      /// <summary>
      /// Allowed time to return (days) - Final Deadline
      /// </summary>
      public UInt32 ExpirationInDays
      {
         get { return ExpirationInHours / 24; }
      }

      /// <summary>
      /// Assignment info present flag
      /// </summary>
      public bool AssignmentInfoPresent
      {
         get 
         {
            byte[] b = new byte[_qEntry.AssignmentInfoPresent.Length];
            Array.Copy(_qEntry.AssignmentInfoPresent, b, 4);
         
            if (IsBigEndian(_qEntry.AssignmentInfoPresent))
            {
               Array.Reverse(b);
            }

            return BitConverter.ToUInt32(b, 0) == 1;
         }
      }

      /// <summary>
      /// Assignment timestamp (UTC)
      /// </summary>
      public DateTime AssignmentTimeStampUtc
      {
         get
         {
            UInt32 seconds = 0;
         
            if (AssignmentInfoPresent)
            {
               byte[] b = new byte[_qEntry.AssignmentTimeStamp.Length];
               Array.Copy(_qEntry.AssignmentTimeStamp, b, 4);

               if (IsBigEndian(_qEntry.AssignmentInfoPresent))
               {
                  Array.Reverse(b);
               }

               seconds = BitConverter.ToUInt32(b, 0);
            }

            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(seconds);
         }
      }

      /// <summary>
      /// Assignment timestamp (Local)
      /// </summary>
      public DateTime AssignmentTimeStampLocal
      {
         get
         {
            return AssignmentTimeStampUtc.ToLocalTime();
         }
      }

      /// <summary>
      /// Assignment info checksum
      /// </summary>
      public string AssignmentInfoChecksum
      {
         get
         {
            if (AssignmentInfoPresent)
            {
               byte[] bytes = new byte[_qEntry.AssignmentInfoChecksum.Length];
               Array.Copy(_qEntry.AssignmentInfoChecksum, bytes, 4);

               if (IsBigEndian(_qEntry.AssignmentInfoPresent))
               {
                  Array.Reverse(bytes);
               }

               StringBuilder sb = new StringBuilder(_qEntry.AssignmentInfoChecksum.Length * 2);
               foreach (byte b in bytes)
               {
                  sb.AppendFormat("{0:X2}", b);
               }
               return sb.ToString();
            }
            
            return String.Empty;
         }
      }

      /// <summary>
      /// Collection server IP address
      /// </summary>
      public string CollectionServerIP
      {
         get
         {
            return String.Format("{0}.{1}.{2}.{3}", _qEntry.CollectionServerIP[3], _qEntry.CollectionServerIP[2], 
                                                    _qEntry.CollectionServerIP[1], _qEntry.CollectionServerIP[0]);
         }
      }

      /// <summary>
      /// Number of SMP cores
      /// </summary>
      public UInt32 NumberOfSmpCores
      {
         get 
         {
            byte[] b = new byte[_qEntry.NumberOfSmpCores.Length];
            Array.Copy(_qEntry.NumberOfSmpCores, b, _qEntry.NumberOfSmpCores.Length);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Tag of Work Unit
      /// </summary>
      public string WorkUnitTag
      {
         get { return _qEntry.WorkUnitTag; }
      }

      /// <summary>
      /// Passkey
      /// </summary>
      public string Passkey
      {
         get { return _qEntry.Passkey; }
      }

      /// <summary>
      /// Flops per CPU (core)
      /// </summary>
      public UInt32 Flops
      {
         get 
         {
            byte[] b = new byte[_qEntry.Flops.Length];
            Array.Copy(_qEntry.Flops, b, _qEntry.Flops.Length);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// MegaFlops per CPU (core)
      /// </summary>
      public double MegaFlops
      {
         get 
         { 
            return Flops / 1000000.000000;
         }
      }

      /// <summary>
      /// Available memory (as of v6.00)
      /// </summary>
      public UInt32 Memory
      {
         get 
         {
            byte[] b = new byte[_qEntry.Memory.Length];
            Array.Copy(_qEntry.Memory, b, _qEntry.Memory.Length);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Available GPU memory (as of v6.20)
      /// </summary>
      public UInt32 GpuMemory
      {
         get { return _qEntry.GpuMemory; }
      }

      /// <summary>
      /// WU expiration time (UTC)
      /// </summary>
      public DateTime DueDateUtc
      {
         get 
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(_qEntry.ExpirationTime[0]); 
         }
      }

      /// <summary>
      /// WU expiration time (Local)
      /// </summary>
      public DateTime DueDateLocal
      {
         get { return DueDateUtc.ToLocalTime(); }
      }

      /// <summary>
      /// Packet size limit
      /// </summary>
      public UInt32 PacketSizeLimit
      {
         get { return _qEntry.PacketSizeLimit; }
      }

      /// <summary>
      /// Number of upload failures
      /// </summary>
      public UInt32 NumberOfUploadFailures
      {
         get { return _qEntry.NumberOfUploadFailures; }
      }

      private static string GetCpuString(byte[] CpuType, byte[] CpuSpecies)
      {
         UInt32 CpuTypeAsUInt32 = GetCpuOrOsNumber(CpuType);
         UInt32 CpuSpeciesAsUInt32 = GetCpuOrOsNumber(CpuSpecies);
         
         return GetCpuString((CpuTypeAsUInt32 * 100000) + CpuSpeciesAsUInt32);
      }

      private static string GetCpuString(UInt32 CpuId)
      {
         switch (CpuId)
         {
            case 100000:
               return "x86";
	         case 100085:
	            return "x86";
	         case 100086:
	            return "i86";
	         case 100087:
	            return "Pentium IV";
	         case 100186:
	            return "i186";
	         case 100286:
	            return "i286";
	         case 100386:
	            return "i386";
	         case 100486:
	            return "i486";
	         case 100586:
	            return "Pentium";
	         case 100587:
	            return "Pentium MMX";
	         case 100686:
	            return "Pentium Pro";
	         case 100687:
	            return "Pentium II/III";
	         case 101000:
	            return "Cyrix x86";
	         case 102000:
	            return "AMD x86";
	         case 200000:
	            return "PowerPC";
	         case 1100000:
	            return "IA64";
	         case 1600000:
	            return "AMD64";
	     }
	     
	     return "Unknown";
      }

      private static string GetOsString(byte[] OsType, byte[] OsSpecies)
      {
         UInt32 OsTypeAsUInt32 = GetCpuOrOsNumber(OsType);
         UInt32 OsSpeciesAsUInt32 = GetCpuOrOsNumber(OsSpecies);

         return GetOsString((OsTypeAsUInt32 * 100000) + OsSpeciesAsUInt32);
      }
      
      private static string GetOsString(UInt32 OsId)
      {
         switch (OsId)
         {
            case 100000:
               return "Windows";
            case 100001:
               return "Win95";
            case 100002:
               return "Win95_OSR2";
            case 100003:
               return "Win98";
            case 100004:
               return "Win98SE";
            case 100005:
               return "WinME";
            case 100006:
               return "WinNT";
            case 100007:
               return "Win2K";
            case 100008:
               return "WinXP";
            case 100009:
               return "Win2K3";
            case 200000:
               return "MacOS";
            case 300000:
               return "OSX";
            case 400000:
               return "Linux";
            case 700000:
               return "FreeBSD";
            case 800000:
               return "OpenBSD";
            case 1800000:
               return "Win64";
            case 1900000:
               return "OS2";
         }
         
         return "Unknown";
      }
      
      private static UInt32 GetCpuOrOsNumber(byte[] b)
      {
         UInt32 value = BitConverter.ToUInt32(b, 0);

         if (value > UInt16.MaxValue)
         {
            Array.Reverse(b);
            value = BitConverter.ToUInt32(b, 0);
         }
         
         return value;
      }
      
      private static bool IsBigEndian(byte[] b)
      {
         UInt32 value = BitConverter.ToUInt32(b, 0);

         if (value > UInt16.MaxValue)
         {
            return true;
         }

         return false;
      }
   }

   [CLSCompliant(false)]
   public class QueueReader
   {
      /// <summary>
      /// Queue Structure
      /// </summary>
      private Queue _q;
      /// <summary>
      /// queue.dat File Path
      /// </summary>
      private string _queueFilePath;
      
      /// <summary>
      /// queue.dat File Path
      /// </summary>
      public string QueueFilePath
      {
         get { return _queueFilePath; }
      }

      /// <summary>
      /// Queue (client) version
      /// </summary>
      public UInt32 Version
      {
         get { return _q.Version; }
      }

      /// <summary>
      /// Performance fraction
      /// </summary>
      public float PerformanceFraction
      {
         get { return _q.PerformanceFraction; }
      }

      /// <summary>
      /// Performance fraction unit weight
      /// </summary>
      public UInt32 PerformanceFractionUnitWeight
      {
         get { return _q.PerformanceFractionUnitWeight; }
      }

      /// <summary>
      /// Download rate sliding average
      /// </summary>
      public float DownloadRateAverage
      {
         get { return (float)_q.DownloadRateAverage / 1000; }
      }

      /// <summary>
      /// Download rate unit weight
      /// </summary>
      public UInt32 DownloadRateUnitWeight
      {
         get { return _q.DownloadRateUnitWeight; }
      }

      /// <summary>
      /// Upload rate sliding average
      /// </summary>
      public float UploadRateAverage
      {
         get { return (float)_q.UploadRateAverage / 1000; }
      }

      /// <summary>
      /// Upload rate unit weight
      /// </summary>
      public UInt32 UploadRateUnitWeight
      {
         get { return _q.UploadRateUnitWeight; }
      }

      /// <summary>
      /// Results successfully sent (after upload failures)
      /// </summary>
      public UInt32 ResultsSent
      {
         get 
         {
            byte[] b = new byte[_q.ResultsSent.Length];
            Array.Copy(_q.ResultsSent, b, _q.ResultsSent.Length);
            return BitConverter.ToUInt32(b, 0);
         }
      }
      
      /// <summary>
      /// Read queue.dat file
      /// </summary>
      /// <param name="FilePath">Path to queue.dat file</param>
      public void ReadQueue(string FilePath)
      {
         BinaryReader reader = new BinaryReader(new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read));
         _q = FromBinaryReaderBlock(reader);
         _queueFilePath = FilePath;
      }

      private static Queue FromBinaryReaderBlock(BinaryReader br)
      {
         // Read byte array
         byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(Queue)));
         
         // Make sure that the Garbage Collector doesn't move our buffer 
         GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
         
         // Marshal the bytes
         Queue q = (Queue)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Queue));
         handle.Free(); //Give control of the buffer back to the GC 

         return q;
      }
      
      /// <summary>
      /// Get the QueueEntry at the specified Index.
      /// </summary>
      /// <param name="Index">Queue Entry Index</param>
      /// <exception cref="IndexOutOfRangeException">Throws when Index is less than 0 or greater than 9.</exception>
      public QueueEntry GetQueueEntry(uint Index)
      {
         return new QueueEntry(_q.Entries[Index], Index, _q.CurrentIndex);
      }
   }
}
