/*
 * HFM.NET - Queue Entry Class
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace HFM.Queue
{
   /// <summary>
   /// Queue Entry Class
   /// </summary>
   [CLSCompliant(false)]
   public class QueueEntry
   {
      #region Constants

      private const string UnknownValue = "Unknown";

      #endregion

      #region Fields

      /// <summary>
      /// Wrapped Entry Structure
      /// </summary>
      private Entry _entry;

      /// <summary>
      /// Entry Index
      /// </summary>
      private readonly UInt32 _index;

      /// <summary>
      /// Entry Index
      /// </summary>
      public UInt32 Index
      {
         get { return _index; }
      }

      /// <summary>
      /// The QueueReader that Created this QueueEntry
      /// </summary>
      private readonly QueueData _qData;

      /// <summary>
      /// CPU Type Dictionary
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public static Dictionary<UInt32, string> GetCpuTypes()
      {
         return new Dictionary<UInt32, string>
                {
                   { 100000, "x86" },
                   { 100085, "x86" },
                   { 100086, "i86" },
                   { 100087, "Pentium IV" },
                   { 100186, "i186" },
                   { 100286, "i286" },
                   { 100386, "i386" },
                   { 100486, "i486" },
                   { 100586, "Pentium" },
                   { 100587, "Pentium MMX" },
                   { 100686, "Pentium Pro" },
                   { 100687, "Pentium II/III" },
                   { 101000, "Cyrix x86" },
                   { 102000, "AMD x86" },
                   { 200000, "PowerPC" },
                   { 1100000, "IA64" },
                   { 1600000, "AMD64" }
                };
      }

      /// <summary>
      /// OS Type Dictionary
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      [SuppressMessage("Microsoft.Globalization", "CA1302:DoNotHardcodeLocaleSpecificStrings", MessageId = "WinNT")]
      public static Dictionary<UInt32, string> GetOsTypes()
      {
         return new Dictionary<UInt32, string>
                {
                   { 100000, "Windows" },
                   { 100001, "Win95" },
                   { 100002, "Win95_OSR2" },
                   { 100003, "Win98" },
                   { 100004, "Win98SE" },
                   { 100005, "WinME" },
                   { 100006, "WinNT" },
                   { 100007, "Win2K" },
                   { 100008, "WinXP" },
                   { 100009, "Win2K3" },
                   { 200000, "MacOS" },
                   { 300000, "OSX" },
                   { 400000, "Linux" },
                   { 700000, "FreeBSD" },
                   { 800000, "OpenBSD" },
                   { 1800000, "Win64" },
                   { 1900000, "OS2" }
                };
      }
      
      /// <summary>
      /// Entry Status Literals (index corresponds to the EntryStatus property)
      /// </summary>
      public static string[] GetEntryStatusLiterals()
      {
         return new[]
                {
                   "Unknown",
                   "Empty",
                   "Deleted",
                   "Finished",
                   "Garbage",
                   "Folding Now",
                   "Queued",
                   "Ready For Upload",
                   "Abandonded",
                   "Fetching From Server"
                };
      }                                              

      /// <summary>
      /// Required Client Type Literals (index corresponds to the RequiredClientType property)
      /// </summary>
      public static string[] GetRequiredClientTypeLiterals()
      {
         return new[]
                {
                   String.Empty,
                   "Regular",
                   "No Deadline",
                   "Advmethods",
                   "Beta",
                   "Internal",
                   "Big Beta",
                   "BigAdv",
                   "Alpha",
                   "Big Alpha"
                };
      } 
      
      /// <summary>
      /// Core Name Dictionary (keys correspond to the CoreNumber property)
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public static Dictionary<UInt32, string> GetCoreNames()
      {
         return new Dictionary<UInt32, string>
                {
                   { 0, String.Empty },
                   { 0x10, "GROGPU" },       /* GPU */
                   { 0x11, "GROGPU2" },		/* GPU2 (ATI CAL / NVIDIA CUDA) */
                   { 0x12, "ATI-DEV" },		/* GPU2 (ATI Development) */
                   { 0x13, "NVIDIA-DEV" },	/* GPU2 (NVIDIA Development) */
                   { 0x14, "GROGPU2-MT" },	/* GPU2 (NVIDIA Development) */
                   { 0x15, "OPENMMGPU" },		/* GPU3 (OpenMM) */
                   { 0x20, "SHARPEN" },		/* SHARPEN */
                   { 0x65, "TINKER" },		   /* Tinker */
                   { 0x78, "GROMACS" },		/* Gromacs */
                   { 0x79, "DGROMACS" },		/* Double-precision Gromacs */
                   { 0x7a, "GBGROMACS" },		/* GB Gromacs (Generalized Born implicit solvent) */
                   { 0x7b, "DGROMACSB" },		/* Double-precision Gromacs B */
                   { 0x7c, "DGROMACSC" },		/* Double-precision Gromacs C */
                   { 0x80, "GROST" },		   /* Gromacs SREM */
                   { 0x81, "GROSIMT" },		/* Gromacs Simulated Tempering */
                   { 0x82, "AMBER" },		   /* Amber */
                   { 0x96, "QMD" },		      /* QMD */
                   { 0xa0, "GROMACS33" },		/* Gromacs 3.3 */
                   { 0xa1, "GRO-SMP" },		/* Gromacs SMP (V1.71) */
                   { 0xa2, "GROCVS" },		   /* Gromacs CVS / Gromacs SMP (V1.90) */
                   { 0xa3, "GRO-A3" },		   /* Gromacs SMP2 / Gromacs SMP (V2.13) */
                   { 0xa4, "GRO-A4" },		   /* Gromacs GB (V2.06) */
                   { 0xa5, "GRO-A5" },       /* Gromacs SMP2 (v2.27) - added to HFM API on 2-20-11 */
                   { 0xb4, "ProtoMol" },		/* ProtoMol */
                };
      } 
      
      #endregion

      #region Constructor

      /// <summary>
      /// Primary Constructor
      /// </summary>
      /// <param name="entry">Entry Structure</param>
      /// <param name="index">Entry Index</param>
      /// <param name="qData">The QueueData object that is creating this QueueEntry</param>
      internal QueueEntry(Entry entry, UInt32 index, QueueData qData)
      {
         _entry = entry;
         _index = index;
         _qData = qData;
      }
      
      #endregion
      
      #region queue.dat Properties

      // ReSharper disable InconsistentNaming

      /// <summary>
      /// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      /// </summary>
      public UInt32 Status
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.Status);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Entry Status (status value based on Status property and other properties of this queue entry)
      /// (0) Unknown / (1) Empty / (2) Deleted / (3) Finished / (4) Garbage / (5) Folding Now
      /// (6) Queued / (7) Ready For Upload / (8) Abandonded / (9) Fetching From Server
      /// </summary>
      public UInt32 EntryStatus
      {
         get
         {
            switch (Status)
            {
               case 0:
                  if (ProjectID == 0)
                  {
                     /* The queue entry has never been used, or has been completely cleared. */
                     return 1; // Empty
                  }
                  if (UploadStatus == 0)
                  {
                     /* The unit was explicitly deleted. */
                     return 2; // Deleted
                  }
                  if (UploadStatus == 1)
                  {
                     /* The unit has been uploaded.  The queue entry is just history. */
                     return 3; // Finished
                  }

                  /* The queue entry is available, but its history is unintelligible. */
                  return 4; // Garbage
               case 1:
                  if (_index == _qData.CurrentIndex)
                  {
                     /* The unit is in progress.  Presumably the core is running. */
                     return 5; // Folding Now
                  }

                  /* The unit has been downloaded but processing hasn't begun yet. */
                  return 6; // Queued
               case 2:
                  /* The core has finished the unit, but it is still in the queue. */
                  return 7; // Ready For Upload
               case 3: /* Issue before V3b5, neglected to post status (1). */
                  return 8; // Abandonded
               case 4:
                  /* Client presently contacting the server, or something failed in download.
			          * If this state persists past the current unit, the queue entry will be
			          * unusable, but otherwise things will go on as usual.
			          */
                  return 9; // Fetching From Server
               default:
                  /* Something other than 0 to 4. */
                  return 0; // Unknown
            }
         }
      }

      /// <summary>
      /// Entry Status (status value based on Status property and other properties of this queue entry)
      /// (0) Unknown / (1) Empty / (2) Deleted / (3) Finished / (4) Garbage / (5) Folding Now
      /// (6) Queued / (7) Ready For Upload / (8) Abandonded / (9) Fetching From Server
      /// </summary>
      public string EntryStatusLiteral
      {
         get
         {
            var literals = GetEntryStatusLiterals();
            return EntryStatus >= 0 && EntryStatus <= literals.Length ? literals[EntryStatus] : UnknownValue;
         }
      }

      /// <summary>
      /// Specifies a Factor Value denoting the Speed of Completion in relationship to the Maximum Expiration Time.
      /// </summary>
      public double SpeedFactor
      {
         get
         {
            if (EntryStatus.Equals(3) || // Finished
                EntryStatus.Equals(7))   // Ready For Upload
            {
               return Math.Round(ExpirationInSeconds / EndTimeUtc.Subtract(BeginTimeUtc).TotalSeconds, 2, MidpointRounding.AwayFromZero);
            }

            return 0;
         }
      }

      /// <summary>
      /// Number of SMP cores
      /// </summary>
      public UInt32 NumberOfSmpCores
      {
         get { return _entry.NumberOfSmpCores; }
      }

      /// <summary>
      /// Begin Time (UTC)
      /// </summary>
      public DateTime BeginTimeUtc
      {
         get
         {
            if (_qData.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_entry.TimeData[1]);
               byte[] bytes = _qData.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return QueueData.Epoch2000.AddSeconds(seconds);
            }
            return QueueData.Epoch2000.AddSeconds(_entry.TimeData[0]);
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
            if (_qData.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_entry.TimeData[5]);
               byte[] bytes = _qData.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return QueueData.Epoch2000.AddSeconds(seconds);
            }
            return QueueData.Epoch2000.AddSeconds(_entry.TimeData[4]);
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
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.UploadStatus);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Web address for core downloads
      /// </summary>
      public Uri CoreDownloadUrl
      {
         get { return new Uri(String.Format(CultureInfo.InvariantCulture, "http://{0}/Core_{1}.fah", _entry.CoreDownloadUrl, CoreNumberHex)); }
      }

      /// <summary>
      /// Misc1a
      /// </summary>
      public UInt32 Misc1a
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.Misc1a);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Core_xx number
      /// </summary>
      public UInt32 CoreNumber
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.CoreNumber);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Core_xx number
      /// </summary>
      public string CoreNumberHex
      {
         get { return CoreNumber.ToString("x", CultureInfo.InvariantCulture); }
      }

      /// <summary>
      /// Core_xx name
      /// </summary>
      public string CoreName
      {
         get
         {
            var names = GetCoreNames();
            return names.ContainsKey(CoreNumber) ? names[CoreNumber] : UnknownValue;
         }
      }

      /// <summary>
      /// Misc1b
      /// </summary>
      public UInt32 Misc1b
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.Misc1b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// wudata_xx.dat file size
      /// </summary>
      public UInt32 WuDataFileSize
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.WuDataFileSize);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Project ID
      /// </summary>
      public UInt16 ProjectID
      {
         get
         {
            var b = new byte[2];
            Array.Copy(_entry.Project, 0, b, 0, 2);
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
            var b = new byte[2];
            Array.Copy(_entry.Project, 2, b, 0, 2);
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
            var b = new byte[2];
            Array.Copy(_entry.Project, 4, b, 0, 2);
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
            var b = new byte[2];
            Array.Copy(_entry.Project, 6, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

      /// <summary>
      /// Project Issued Time (UTC)
      /// </summary>
      public DateTime ProjectIssuedUtc
      {
         get
         {
            var b = new byte[4];
            Array.Copy(_entry.Project, 8, b, 0, 4);
            UInt32 seconds = BitConverter.ToUInt32(b, 0);
            var d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
         get
         {
            if (IsMachineIDBigEndian)
            {
               var b = new byte[4];
               Array.Copy(_entry.MachineID, b, _entry.MachineID.Length);
               Array.Reverse(b);
               return BitConverter.ToUInt32(b, 0);
            }

            return BitConverter.ToUInt32(_entry.MachineID, 0);
         }
      }

      /// <summary>
      /// Denotes Byte Order of Machine ID Field
      /// </summary>
      public bool IsMachineIDBigEndian
      {
         get
         {
            return QueueData.IsBigEndian(_entry.MachineID);
         }
      }

      /// <summary>
      /// Server IP address
      /// </summary>
      public string ServerIP
      {
         get
         {
            if (_qData.System.Equals(SystemType.PPC))
            {
               return String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", 
                  _entry.ServerIP[0], _entry.ServerIP[1], _entry.ServerIP[2], _entry.ServerIP[3]);
            }

            return String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", 
               _entry.ServerIP[3], _entry.ServerIP[2], _entry.ServerIP[1], _entry.ServerIP[0]);
         }
      }

      /// <summary>
      /// Server port number
      /// </summary>
      public UInt32 ServerPort
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.ServerPort);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Work unit type
      /// </summary>
      public string WorkUnitType
      {
         get { return _entry.WorkUnitType; }
      }

      /// <summary>
      /// Folding ID (User name)
      /// </summary>
      public string FoldingID
      {
         get { return _entry.FoldingID; }
      }

      /// <summary>
      /// Team Number
      /// </summary>
      public string Team
      {
         get { return _entry.Team; }
      }

      /// <summary>
      /// Team Number
      /// </summary>
      public UInt32 TeamNumber
      {
         get
         {
            UInt32 team;
            if (UInt32.TryParse(_entry.Team, out team))
            {
               return team;
            }

            return 0;
         }
      }

      /// <summary>
      /// ID associated with this queue entry
      /// </summary>
      public string ID
      {
         get
         {
            var bytes = new byte[_entry.UserAndMachineID.Length];
            Array.Copy(_entry.UserAndMachineID, bytes, _entry.UserAndMachineID.Length);
            if (IsMachineIDBigEndian == false)
            {
               Array.Reverse(bytes);
            }

            var sb = new StringBuilder(_entry.UserAndMachineID.Length * 2);
            foreach (byte b in bytes)
            {
               sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
         }
      }

      /// <summary>
      /// User ID (unique hexadecimal value)
      /// </summary>
      public string UserID
      {
         get
         {
            return GetUserID(_entry.UserAndMachineID, MachineID, IsMachineIDBigEndian);
         }
      }

      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      public UInt32 Benchmark
      {
         get
         {
            var b = new byte[_entry.Benchmark.Length];
            Array.Copy(_entry.Benchmark, b, _entry.Benchmark.Length);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// CPU type
      /// </summary>
      public UInt32 CpuType
      {
         get { return GetCpuOrOsNumber(_entry.CpuType); }
      }

      /// <summary>
      /// OS type
      /// </summary>
      public UInt32 OsType
      {
         get { return GetCpuOrOsNumber(_entry.OsType); }
      }

      /// <summary>
      /// CPU species
      /// </summary>
      public UInt32 CpuSpecies
      {
         get { return GetCpuOrOsNumber(_entry.CpuSpecies); }
      }

      /// <summary>
      /// OS species
      /// </summary>
      public UInt32 OsSpecies
      {
         get { return GetCpuOrOsNumber(_entry.OsSpecies); }
      }

      /// <summary>
      /// CPU type
      /// </summary>
      public string CpuString
      {
         get { return GetCpuString((CpuType * 100000) + CpuSpecies); }
      }

      /// <summary>
      /// OS type
      /// </summary>
      public string OsString
      {
         get { return GetOsString((OsType * 100000) + OsSpecies); }
      }

      /// <summary>
      /// Allowed time to return (seconds) - Final Deadline
      /// </summary>
      public UInt32 ExpirationInSeconds
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.ExpirationInSeconds);
            return BitConverter.ToUInt32(b, 0);
         }
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
      /// Required client type endian flag
      /// </summary>
      private bool RequiredClientTypeBigEndian
      {
         get { return QueueData.IsBigEndian(_entry.RequiredClientType); }
      }

      /// <summary>
      /// Required client type
      /// </summary>
      public UInt32 RequiredClientType
      {
         get
         {
            var b = new byte[_entry.RequiredClientType.Length];
            Array.Copy(_entry.RequiredClientType, b, b.Length);

            if (RequiredClientTypeBigEndian)
            {
               Array.Reverse(b);
            }

            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Required client type
      /// </summary>
      public string RequiredClientTypeLiteral
      {
         get
         {
            var literals = GetRequiredClientTypeLiterals();
            return RequiredClientType >= 0 && RequiredClientType <= literals.Length
                      ? literals[RequiredClientType]
                      : UnknownValue;
         }
      }
      
      /// <summary>
      /// Assignment info endian flag
      /// </summary>
      public bool AssignmentInfoBigEndian
      {
         get { return QueueData.IsBigEndian(_entry.AssignmentInfoPresent); }
      }

      /// <summary>
      /// Assignment info present flag
      /// </summary>
      public bool AssignmentInfoPresent
      {
         get
         {
            var b = new byte[_entry.AssignmentInfoPresent.Length];
            Array.Copy(_entry.AssignmentInfoPresent, b, 4);

            if (AssignmentInfoBigEndian)
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
               var b = new byte[_entry.AssignmentTimeStamp.Length];
               Array.Copy(_entry.AssignmentTimeStamp, b, 4);

               if (QueueData.IsBigEndian(_entry.AssignmentInfoPresent))
               {
                  Array.Reverse(b);
               }

               seconds = BitConverter.ToUInt32(b, 0);
            }

            return QueueData.Epoch2000.AddSeconds(seconds);
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
               var bytes = new byte[_entry.AssignmentInfoChecksum.Length];
               Array.Copy(_entry.AssignmentInfoChecksum, bytes, 4);

               // Reverse this value if 'AssignmentInfoPresent' IS NOT Big Endian
               // qd.c prints the bytes in reverse order so it stands to reason 
               // that if 'AssignmentInfoPresent' IS Big Endian then the bytes
               // would not need reversed.  This theory is UNTESTED.
               if (QueueData.IsBigEndian(_entry.AssignmentInfoPresent) == false)
               {
                  Array.Reverse(bytes);
               }

               var sb = new StringBuilder(_entry.AssignmentInfoChecksum.Length * 2);
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
            if (_qData.System.Equals(SystemType.PPC))
            {
               return String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", 
                  _entry.CollectionServerIP[0], _entry.CollectionServerIP[1],
                  _entry.CollectionServerIP[2], _entry.CollectionServerIP[3]);
            }
            return String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", 
               _entry.CollectionServerIP[3], _entry.CollectionServerIP[2],
               _entry.CollectionServerIP[1], _entry.CollectionServerIP[0]);
         }
      }

      /// <summary>
      /// Misc4a endian flag
      /// </summary>
      public bool Misc4aBigEndian
      {
         get { return QueueData.IsBigEndian(_entry.Misc4a); }
      }
      
      /// <summary>
      /// Misc4a
      /// </summary>
      public UInt32 Misc4a
      {
         get
         {
            var b = new byte[_entry.Misc4a.Length];
            Array.Copy(_entry.Misc4a, b, _entry.Misc4a.Length);
            
            if (Misc4aBigEndian)
            {
               Array.Reverse(b);
            }

            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Number of SMP Cores to use
      /// </summary>
      public UInt32 UseCores
      {
         get
         {
            var b = new byte[_entry.UseCores.Length];
            Array.Copy(_entry.UseCores, b, _entry.UseCores.Length);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Tag of Work Unit
      /// </summary>
      public string WorkUnitTag
      {
         get
         {
            int i = Array.IndexOf(_entry.WorkUnitTag, (byte)0);
            if (i >= 0)
            {
               return Encoding.ASCII.GetString(_entry.WorkUnitTag, 0, i);
            }
            
            return Encoding.ASCII.GetString(_entry.WorkUnitTag, 0, _entry.WorkUnitTag.Length);
         }
      }

      /// <summary>
      /// Passkey
      /// </summary>
      public string Passkey
      {
         get
         {
            if (_entry.Passkey[0] == 0)
            {
               return String.Empty;
            }
            
            var sb = new StringBuilder();
            foreach (byte b in _entry.Passkey)
            {
               sb.Append(Convert.ToChar(b));
            }
            return sb.ToString();
         }
      }

      /// <summary>
      /// Flops per CPU (core)
      /// </summary>
      public UInt32 Flops
      {
         get
         {
            var b = new byte[_entry.Flops.Length];
            Array.Copy(_entry.Flops, b, _entry.Flops.Length);
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
            var b = new byte[_entry.Memory.Length];
            Array.Copy(_entry.Memory, b, _entry.Memory.Length);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Available GPU memory (as of v6.20)
      /// </summary>
      public UInt32 GpuMemory
      {
         get { return _entry.GpuMemory; }
      }

      /// <summary>
      /// WU expiration time (UTC)
      /// </summary>
      public DateTime DueTimeUtc
      {
         get
         {
            if (_qData.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_entry.ExpirationTime[1]);
               byte[] bytes = _qData.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return QueueData.Epoch2000.AddSeconds(seconds);
            }
            return QueueData.Epoch2000.AddSeconds(_entry.ExpirationTime[0]);
         }
      }

      /// <summary>
      /// WU expiration time (Local)
      /// </summary>
      public DateTime DueTimeLocal
      {
         get { return DueTimeUtc.ToLocalTime(); }
      }

      /// <summary>
      /// Packet size limit
      /// </summary>
      public UInt32 PacketSizeLimit
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.PacketSizeLimit);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Number of upload failures
      /// </summary>
      public UInt32 NumberOfUploadFailures
      {
         get { return _entry.NumberOfUploadFailures; }
      }

      // ReSharper restore InconsistentNaming
      
      #endregion

      internal static byte[] HexToData(string hexString)
      {
         Debug.Assert(hexString != null);

         if (hexString.Length % 2 == 1)
         {
            hexString = '0' + hexString; // Pad the first byte
         }

         var data = new byte[hexString.Length / 2];
         for (int i = 0; i < data.Length; i++)
         {
            data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
         }

         return data;
      }

      // ReSharper disable InconsistentNaming
      internal static string GetUserID(byte[] userAndMachineID, UInt32 machineID, bool isMachineIDBigEndian)
      // ReSharper restore InconsistentNaming
      {
         Debug.Assert(userAndMachineID.Length == 8);

         /*** Remove the MachineID from UserAndMachineID ***/

         var bytes = new byte[userAndMachineID.Length];
         Array.Copy(userAndMachineID, bytes, userAndMachineID.Length);
         if (isMachineIDBigEndian)
         {
            // Reverse the bytes so we get the least significant byte first
            Array.Reverse(bytes);
         }

         // Convert to 64bit integer
         UInt64 value = BitConverter.ToUInt64(bytes, 0);
         value = value - machineID;
         // Convert back to bytes after MachineID has been subtracted
         bytes = BitConverter.GetBytes(value);
         // Reverse the bytes so we show the most significant byte first
         Array.Reverse(bytes);

         var sb = new StringBuilder(userAndMachineID.Length * 2);
         foreach (byte b in bytes)
         {
            sb.AppendFormat("{0:X2}", b);
         }
         return sb.ToString().TrimStart('0');
      }

      private static string GetCpuString(UInt32 cpuId)
      {
         var literals = GetCpuTypes();
         return literals.ContainsKey(cpuId) ? literals[cpuId] : UnknownValue;
      }

      private static string GetOsString(UInt32 osId)
      {
         var literals = GetOsTypes();
         return literals.ContainsKey(osId) ? literals[osId] : UnknownValue;
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
   }
}
