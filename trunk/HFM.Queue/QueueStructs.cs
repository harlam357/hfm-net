/*
 * HFM.NET - Queue Structures
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HFM.Queue
{
   // ReSharper disable FieldCanBeMadeReadOnly.Local
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
   internal struct Data
   {
      /* 0000 Queue (client) version (v2.17 and above) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _version;
      /// <summary>
      /// Queue (client) version
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Version
      {
         get { return _version; }
      }

      /* 0004 Current index number */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _currentIndex;
      /// <summary>
      /// Current index number
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CurrentIndex
      {
         get { return _currentIndex; }
      }

      /* 0008 Array of ten queue entries */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
      private Entry[] _entries;
      /// <summary>
      /// Array of ten queue entries
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public Entry[] Entries
      {
         get { return _entries; }
      }

      /* 7128 Performance fraction (as of v3.24) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _performanceFraction;
      /// <summary>
      /// Performance fraction
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] PerformanceFraction
      {
         get { return _performanceFraction; }
      }

      /* 7132 Performance fraction unit weight (as of v3.24) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _performanceFractionUnitWeight;
      /// <summary>
      /// Performance fraction unit weight
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] PerformanceFractionUnitWeight
      {
         get { return _performanceFractionUnitWeight; }
      }

      /* 7136 Download rate sliding average (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _downloadRateAverage;
      /// <summary>
      /// Download rate sliding average
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] DownloadRateAverage
      {
         get { return _downloadRateAverage; }
      }

      /* 7140 Download rate unit weight (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _downloadRateUnitWeight;
      /// <summary>
      /// Download rate unit weight
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] DownloadRateUnitWeight
      {
         get { return _downloadRateUnitWeight; }
      }

      /* 7144 Upload rate sliding average (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _uploadRateAverage;
      /// <summary>
      /// Upload rate sliding average
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UploadRateAverage
      {
         get { return _uploadRateAverage; }
      }

      /* 7148 Upload rate unit weight (as of v4.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _uploadRateUnitWeight;
      /// <summary>
      /// Upload rate unit weight
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UploadRateUnitWeight
      {
         get { return _uploadRateUnitWeight; }
      }

      /* 7152 Results successfully sent (after upload failures) (as of v5.00) (LE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _resultsSent;
      /// <summary>
      /// Results successfully sent (after upload failures)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ResultsSent
      {
         get { return _resultsSent; }
      }

      /* 7156 (as of v5.00) ...all zeros after queue conversion... */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
      private string _z7156;
      ///// <summary>
      ///// (as of v5.00) ...all zeros after queue conversion...
      ///// </summary>
      //public string z7156
      //{
      //   get { return _z7156; }
      //}
   }
   // ReSharper restore FieldCanBeMadeReadOnly.Local

   // ReSharper disable FieldCanBeMadeReadOnly.Local
   [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
   internal struct Entry
   {
      /*** 0 = Empty, Deleted, Finished, or Garbage 
       *   1 = Folding Now or Queued 
       *   2 = Ready for Upload 
       *   3 = Abandonded (Ignore is found)
       *   4 = Fetching from Server
       ***/
      /* 000 Status */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _status;
      /// <summary>
      /// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Status
      {
         get { return _status; }
      }

      /* 004 Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use (LE) */
      private UInt32 _useCores;
      /// <summary>
      /// Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use
      /// </summary>
      public UInt32 UseCores
      {
         get { return _useCores; }
      }

      /*** 0 = Begin Time
       *   4 = End Time 
       *   Others = Unknown 
       ***/
      /* 008 Time data (epoch 0000 1jan00 UTC) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      private UInt32[] _timeData;
      /// <summary>
      /// Time data (epoch 0000 1jan00 UTC)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public UInt32[] TimeData
      {
         get { return _timeData; }
      }

      /* 040 Server IP address (until v3.0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _oldServerIP; /*** Ignore this value ***/
      ///// <summary>
      ///// Server IP address (until v3.0 - Ignore this value)
      ///// </summary>
      //[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      //public byte[] OldServerIP
      //{
      //   get { return _oldServerIP; }
      //}

      /***
       * 0 = Not Uploaded
       * 1 = Uploaded
       ***/
      /* 044 Upload status */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _uploadStatus;
      /// <summary>
      /// Upload status (0 = Not Uploaded / 1 = Uploaded)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UploadStatus
      {
         get { return _uploadStatus; }
      }

      /* 048 Web address for core downloads */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      private string _coreDownloadUrl;
      /// <summary>
      /// Web address for core downloads
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
      public string CoreDownloadUrl
      {
         get { return _coreDownloadUrl; }
      }

      /* 176 Misc1a */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _misc1a;
      /// <summary>
      /// Misc1a
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Misc1a
      {
         get { return _misc1a; }
      }

      /* 180 Core_xx number (hex) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _coreNumber; /*** Convert to Hex ***/
      /// <summary>
      /// Core_xx number
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CoreNumber
      {
         get { return _coreNumber; }
      }

      /* 184 Misc1b */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _misc1b;
      /// <summary>
      /// Misc1b
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Misc1b
      {
         get { return _misc1b; }
      }

      /* 188 wudata_xx.dat file size */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _wuDataFileSize;
      /// <summary>
      /// wudata_xx.dat file size
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] WuDataFileSize
      {
         get { return _wuDataFileSize; }
      }

      /* 192 */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      private byte[] _z192;
      ///// <summary>
      ///// z192
      ///// </summary>
      //public byte[] z192
      //{
      //   get { return _z192; }
      //}

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
      private byte[] _project;
      /// <summary>
      /// Project R/C/G and Issued Time
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Project
      {
         get { return _project; }
      }

      /* 224 */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
      private byte[] _z224;
      ///// <summary>
      ///// z224
      ///// </summary>
      //public byte[] z224
      //{
      //   get { return _z224; }
      //}

      /* 260 Machine ID (LE or BE, was only LE before v5 work servers) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _machineID;
      /// <summary>
      /// Machine ID
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] MachineID
      {
         get { return _machineID; }
      }

      /* 264 Server IP address */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _serverIP;
      /// <summary>
      /// Server IP address
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ServerIP
      {
         get { return _serverIP; }
      }

      /* 268 Server port number */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _serverPort;
      /// <summary>
      /// Server port number
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ServerPort
      {
         get { return _serverPort; }
      }

      /* 272 Work unit type */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      private string _workUnitType;
      /// <summary>
      /// Work unit type
      /// </summary>
      public string WorkUnitType
      {
         get { return _workUnitType; }
      }

      /* 336 User Name */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      private string _foldingID;
      /// <summary>
      /// The Folding ID (Username) attached to this queue entry
      /// </summary>
      public string FoldingID
      {
         get { return _foldingID; }
      }

      /* 400 Team Number */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      private string _team;
      /// <summary>
      /// The Team number attached to this queue entry
      /// </summary>
      public string Team
      {
         get { return _team; }
      }

      /* Stored ID for unit (UserID + MachineID) (LE or BE, usually BE, always BE for v5 work servers) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      private byte[] _userAndMachineID;
      /// <summary>
      /// Stored ID for unit (UserID + MachineID)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] UserAndMachineID
      {
         get { return _userAndMachineID; }
      }

      /* 472 Benchmark (until v3.24) (LE) */
      private UInt32 _oldBenchmark; /*** Ignore this value ***/
      ///// <summary>
      ///// Benchmark (until v3.24 - Ignore this value)
      ///// </summary>
      //public UInt32 OldBenchmark
      //{
      //   get { return _oldBenchmark; }
      //}

      /* 476 Misc3b (unused as of v3.24) (LE); Benchmark (as of v5.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _benchmark;
      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Benchmark
      {
         get { return _benchmark; }
      }

      /* 480 CPU type (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _cpuType;
      /// <summary>
      /// CPU type
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CpuType
      {
         get { return _cpuType; }
      }

      /* 484 OS type (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _osType;
      /// <summary>
      /// OS type
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] OsType
      {
         get { return _osType; }
      }

      /* 488 CPU species (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _cpuSpecies;
      /// <summary>
      /// CPU species
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CpuSpecies
      {
         get { return _cpuSpecies; }
      }

      /* 492 OS species (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _osSpecies;
      /// <summary>
      /// OS species
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] OsSpecies
      {
         get { return _osSpecies; }
      }

      /* 496 Allowed time to return (seconds) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _expirationInSeconds; /*** Final Deadline ***/
      /// <summary>
      /// Allowed time to return (seconds) - Final Deadline
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] ExpirationInSeconds
      {
         get { return _expirationInSeconds; }
      }

      /* 500 */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _z500;
      ///// <summary>
      ///// z500
      ///// </summary>
      //public byte[] z500
      //{
      //   get { return _z500; }
      //}

      /* 504 Client type required (usually 0) (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _requiredClientType;
      /// <summary>
      /// Required client type
      /// </summary>
      public byte[] RequiredClientType
      {
         get { return _requiredClientType; }
      }

      /* 508 Assignment info present flag (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _assignmentInfoPresent;
      /// <summary>
      /// Assignment info present flag
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] AssignmentInfoPresent
      {
         get { return _assignmentInfoPresent; }
      }

      /* 512 Assignment timestamp (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _assignmentTimeStamp;
      /// <summary>
      /// Assignment timestamp
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] AssignmentTimeStamp
      {
         get { return _assignmentTimeStamp; }
      }

      /* 516 Assignment info checksum (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _assignmentInfoChecksum;
      /// <summary>
      /// Assignment info checksum
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] AssignmentInfoChecksum
      {
         get { return _assignmentInfoChecksum; }
      }

      /* 520 Collection server IP address (as of v5.00) (LE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _collectionServerIP;
      /// <summary>
      /// Collection server IP address
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] CollectionServerIP
      {
         get { return _collectionServerIP; }
      }

      /* 524 Download started time (as of v5.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _downloadStartedTime;
      ///// <summary>
      ///// Download started time
      ///// </summary>
      //[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      //public byte[] DownloadStartedTime
      //{
      //   get { return _downloadStartedTime; }
      //}

      /* 528 */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _z528;
      ///// <summary>
      ///// z528
      ///// </summary>
      //public byte[] z528
      //{
      //   get { return _z528; }
      //}

      /* 532 Misc4a (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _Misc4a;
      /// <summary>
      /// Misc4a
      /// </summary>
      public byte[] Misc4a
      {
         get { return _Misc4a; }
      }

      /* 536 */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      private byte[] _z536;
      ///// <summary>
      ///// z536
      ///// </summary>
      //public byte[] z536
      //{
      //   get { return _z536; }
      //}

      /* 544 Number of SMP cores (as of v5.91) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _numberOfSmpCores;
      /// <summary>
      /// Number of SMP cores
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] NumberOfSmpCores
      {
         get { return _numberOfSmpCores; }
      }

      /* 548 Tag of Work Unit (as of v5.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      private byte[] _workUnitTag;
      /// <summary>
      /// Tag of Work Unit
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] WorkUnitTag
      {
         get { return _workUnitTag; }
      }

      /* 564 */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      private byte[] _z564;
      ///// <summary>
      ///// z564
      ///// </summary>
      //public byte[] z564
      //{
      //   get { return _z564; }
      //}

      /* 580 Passkey (as of v6.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
      private byte[] _passkey;
      /// <summary>
      /// Passkey
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Passkey
      {
         get { return _passkey; }
      }

      /* 612 Flops per CPU (core) (as of v6.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _flops;
      /// <summary>
      /// Flops per CPU (core)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Flops
      {
         get { return _flops; }
      }

      /* 616 Available memory (as of v6.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _memory;
      /// <summary>
      /// Available memory (as of v6.00)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] Memory
      {
         get { return _memory; }
      }

      /* 620 Available GPU memory (as of v6.20) (LE) */
      private UInt32 _gpuMemory;
      /// <summary>
      /// Available GPU memory (as of v6.20)
      /// </summary>
      public UInt32 GpuMemory
      {
         get { return _gpuMemory; }
      }

      /* 624 */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
      private byte[] _z624;
      ///// <summary>
      ///// z624
      ///// </summary>
      //public byte[] z624
      //{
      //   get { return _z624; }
      //}

      /***
       * 0 = Due Date - This time is calculated by the client when it downloads a unit.
       *                It is determined by adding the "begin" time to the expiration period.
       * 1-3 = Unknown
       ***/
      /* 688 WU expiration time */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private UInt32[] _expirationTime;
      /// <summary>
      /// WU expiration time (0 = Due Date / 1-3 = Unknown)
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public UInt32[] ExpirationTime
      {
         get { return _expirationTime; }
      }

      /* 704 Packet size limit (as of v5.00) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      private byte[] _packetSizeLimit;
      /// <summary>
      /// Packet size limit
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      public byte[] PacketSizeLimit
      {
         get { return _packetSizeLimit; }
      }

      /* 708 Number of upload failures (as of v5.00) */
      private UInt32 _numberOfUploadFailures;
      /// <summary>
      /// Number of upload failures
      /// </summary>
      public UInt32 NumberOfUploadFailures
      {
         get { return _numberOfUploadFailures; }
      }
   }
   // ReSharper restore FieldCanBeMadeReadOnly.Local

   #region struct layout from qd.c (11/11/2010) (http://linuxminded.nl/?target=software-qd-tools.plc)
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
   //   char	mid[4];		 /* 260 Machine ID (LE or BE, was only LE before v5 work servers) */
   //   u32	svr2;		 /* 264 Server IP address */
   //   u32	port;		 /* 268 Server port number */
   //   char	type[64];	 /* 272 Work unit type */
   //   char	uname[64];	 /* 336 User Name */
   //   char	teamn[64];	 /* 400 Team Number */
   //   char	uid[8];		 /* 464 Stored ID for unit (UserID + MachineID) (LE or BE, usually BE, always BE for v5 work servers) */
   //   char	bench[4];	 /* 472 Benchmark (as of v3.24) (LE) */
   //   char	m476[4];	 /* 476 Misc3b (unused as of v3.24) (LE); Benchmark (as of v5.00) (BE) */
   //   u32	cpu_type;	 /* 480 CPU type (LE or BE, sometimes 0) */
   //   u32	os_type;	 /* 484 OS type (LE or BE, sometimes 0) */
   //   u32	cpu_spec;	 /* 488 CPU species (LE or BE, sometimes 0) */
   //   u32	os_spec;	 /* 492 OS species (LE or BE, sometimes 0) */
   //   u32	expire;		 /* 496 Allowed time to return (seconds) */
   //   char	z500[4];
   //   char    cltype[4];	 /* 504 Client type required (usually 0) (LE or BE) */
   //   char	aiflag[4];	 /* 508 Assignment info present flag (LE or BE) */
   //   char	aitime[4];	 /* 512 Assignment timestamp (LE or BE) */
   //   char	aidata[4];	 /* 516 Assignment info (LE or BE) */
   //   char	csip[4];	 /* 520 Collection server IP address (as of v5.00) (LE) */
   //   char	dstart[4];	 /* 524 Download started time (as of v5.00) (BE) */
   //   char	z528[4];
   //   char	m532[4];	 /* 532 Misc4a (LE or BE) */
   //   char	z536[8];
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
