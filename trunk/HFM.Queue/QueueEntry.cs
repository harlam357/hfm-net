/*
 * HFM.NET - Queue Entry Class
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
using System.Diagnostics;
using System.Text;

namespace HFM.Queue
{
   [CLSCompliant(false)]
   public class QueueEntry
   {
      #region Fields
   
      /// <summary>
      /// Wrapped Entry Structure
      /// </summary>
      private Entry _entry;
      /// <summary>
      /// This Entry Index
      /// </summary>
      private readonly UInt32 _thisIndex;
      /// <summary>
      /// The QueueReader that Created this QueueEntry
      /// </summary>
      private readonly QueueData _qData;
      
      /// <summary>
      /// Entry Status String Array (indexes correspond to the EntryStatus property)
      /// </summary>
      public static string[] EntryStatusStrings = new[]
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
      
      #endregion

      #region Constructor

      /// <summary>
      /// Primary Constructor
      /// </summary>
      /// <param name="entry">Entry Structure</param>
      /// <param name="thisIndex">This Entry Index</param>
      /// <param name="qData">The QueueData object that is creating this QueueEntry</param>
      internal QueueEntry(Entry entry, UInt32 thisIndex, QueueData qData)
      {
         _entry = entry;
         _thisIndex = thisIndex;
         _qData = qData;
      }
      
      #endregion
      
      #region queue.dat Properties

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
                  else if (UploadStatus == 0)
                  {
                     /* The unit was explicitly deleted. */
                     return 2; // Deleted
                  }
                  else if (UploadStatus == 1)
                  {
                     /* The unit has been uploaded.  The queue entry is just history. */
                     return 3; // Finished
                  }
                  else
                  {
                     /* The queue entry is available, but its history is unintelligible. */
                     return 4; // Garbage
                  }
               case 1:
                  if (_thisIndex == _qData.CurrentIndex)
                  {
                     /* The unit is in progress.  Presumably the core is running. */
                     return 5; // Folding Now
                  }
                  else
                  {
                     /* The unit has been downloaded but processing hasn't begun yet. */
                     return 6; // Queued
                  }
               case 2:
                  /* The core has finished the unit, but it is still in the queue. */
                  return 7; // Ready For Upload
               case 3: /* Bug before V3b5, neglected to post status (1). */
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
      /// Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use
      /// </summary>
      public UInt32 UseCores
      {
         get { return _entry.UseCores; }
      }

      /// <summary>
      /// Begin Time (UTC)
      /// </summary>
      public DateTime BeginTimeUtc
      {
         get
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (_qData.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_entry.TimeData[1]);
               byte[] bytes = _qData.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return d.AddSeconds(seconds);
            }
            return d.AddSeconds(_entry.TimeData[0]);
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
            if (_qData.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_entry.TimeData[5]);
               byte[] bytes = _qData.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return d.AddSeconds(seconds);
            }
            return d.AddSeconds(_entry.TimeData[4]);
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
         get { return new Uri(String.Format("http://{0}/Core_{1}.fah", _entry.CoreDownloadUrl, CoreNumber)); }
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
      public string CoreNumber
      {
         get
         {
            byte[] b = _qData.GetSystemBytes(_entry.CoreNumber);
            return BitConverter.ToUInt32(b, 0).ToString("x");
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
            byte[] b = new byte[2];
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
            byte[] b = new byte[2];
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
            byte[] b = new byte[2];
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
            byte[] b = new byte[2];
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
            byte[] b = new byte[4];
            Array.Copy(_entry.Project, 8, b, 0, 4);
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
         get
         {
            if (IsMachineIDBigEndian)
            {
               byte[] b = new byte[4];
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
               return String.Format("{0}.{1}.{2}.{3}", _entry.ServerIP[0], _entry.ServerIP[1],
                                                       _entry.ServerIP[2], _entry.ServerIP[3]);
            }

            return String.Format("{0}.{1}.{2}.{3}", _entry.ServerIP[3], _entry.ServerIP[2],
                                                    _entry.ServerIP[1], _entry.ServerIP[0]);
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
            byte[] bytes = new byte[_entry.UserAndMachineID.Length];
            Array.Copy(_entry.UserAndMachineID, bytes, _entry.UserAndMachineID.Length);
            if (IsMachineIDBigEndian == false)
            {
               Array.Reverse(bytes);
            }

            StringBuilder sb = new StringBuilder(_entry.UserAndMachineID.Length * 2);
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
            return GetUserIDFromUserAndMachineID(_entry.UserAndMachineID, MachineID, IsMachineIDBigEndian);
         }
      }

      /// <summary>
      /// Benchmark (as of v5.00)
      /// </summary>
      public UInt32 Benchmark
      {
         get
         {
            byte[] b = new byte[_entry.Benchmark.Length];
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
         get
         {
            return GetCpuOrOsNumber(_entry.CpuType);
         }
      }

      /// <summary>
      /// OS type
      /// </summary>
      public UInt32 OsType
      {
         get
         {
            return GetCpuOrOsNumber(_entry.OsType);
         }
      }

      /// <summary>
      /// CPU species
      /// </summary>
      public UInt32 CpuSpecies
      {
         get
         {
            return GetCpuOrOsNumber(_entry.CpuSpecies);
         }
      }

      /// <summary>
      /// OS species
      /// </summary>
      public UInt32 OsSpecies
      {
         get
         {
            return GetCpuOrOsNumber(_entry.OsSpecies);
         }
      }

      /// <summary>
      /// CPU type (string)
      /// </summary>
      public string CpuString
      {
         get { return GetCpuString(_entry.CpuType, _entry.CpuSpecies); }
      }

      /// <summary>
      /// OS type (string)
      /// </summary>
      public string OsString
      {
         get { return GetOsString(_entry.OsType, _entry.OsSpecies); }
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
      /// Assignment info present flag
      /// </summary>
      public bool AssignmentInfoPresent
      {
         get
         {
            byte[] b = new byte[_entry.AssignmentInfoPresent.Length];
            Array.Copy(_entry.AssignmentInfoPresent, b, 4);

            if (QueueData.IsBigEndian(_entry.AssignmentInfoPresent))
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
               byte[] b = new byte[_entry.AssignmentTimeStamp.Length];
               Array.Copy(_entry.AssignmentTimeStamp, b, 4);

               if (QueueData.IsBigEndian(_entry.AssignmentInfoPresent))
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
               byte[] bytes = new byte[_entry.AssignmentInfoChecksum.Length];
               Array.Copy(_entry.AssignmentInfoChecksum, bytes, 4);

               // Reverse this value if 'AssignmentInfoPresent' IS NOT Big Endian
               // qd.c prints the bytes in reverse order so it stands to reason 
               // that if 'AssignmentInfoPresent' IS Big Endian then the bytes
               // would not need reversed.  This theory is UNTESTED.
               if (QueueData.IsBigEndian(_entry.AssignmentInfoPresent) == false)
               {
                  Array.Reverse(bytes);
               }

               StringBuilder sb = new StringBuilder(_entry.AssignmentInfoChecksum.Length * 2);
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
               return String.Format("{0}.{1}.{2}.{3}", _entry.CollectionServerIP[0], _entry.CollectionServerIP[1],
                                                       _entry.CollectionServerIP[2], _entry.CollectionServerIP[3]);
            }
            return String.Format("{0}.{1}.{2}.{3}", _entry.CollectionServerIP[3], _entry.CollectionServerIP[2],
                                                    _entry.CollectionServerIP[1], _entry.CollectionServerIP[0]);
         }
      }

      /// <summary>
      /// Number of SMP cores
      /// </summary>
      public UInt32 NumberOfSmpCores
      {
         get
         {
            byte[] b = new byte[_entry.NumberOfSmpCores.Length];
            Array.Copy(_entry.NumberOfSmpCores, b, _entry.NumberOfSmpCores.Length);
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
         get { return _entry.Passkey; }
      }

      /// <summary>
      /// Flops per CPU (core)
      /// </summary>
      public UInt32 Flops
      {
         get
         {
            byte[] b = new byte[_entry.Flops.Length];
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
            byte[] b = new byte[_entry.Memory.Length];
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
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (_qData.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_entry.ExpirationTime[1]);
               byte[] bytes = _qData.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return d.AddSeconds(seconds);
            }
            return d.AddSeconds(_entry.ExpirationTime[0]);
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
      
      #endregion

      internal static byte[] HexToData(string hexString)
      {
         if (hexString == null) throw new ArgumentNullException("hexString", "Argument 'hexString' cannot be null.");

         if (hexString.Length % 2 == 1)
         {
            hexString = '0' + hexString; // Pad the first byte
         }

         byte[] data = new byte[hexString.Length / 2];

         for (int i = 0; i < data.Length; i++)
         {
            data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
         }

         return data;
      }

      internal static string GetUserIDFromUserAndMachineID(byte[] userAndMachineID, UInt32 machineID, bool isMachineIDBigEndian)
      {
         Debug.Assert(userAndMachineID.Length == 8);

         /*** Remove the MachineID from UserAndMachineID ***/

         byte[] bytes = new byte[userAndMachineID.Length];
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

         StringBuilder sb = new StringBuilder(userAndMachineID.Length * 2);
         foreach (byte b in bytes)
         {
            sb.AppendFormat("{0:X2}", b);
         }
         return sb.ToString().TrimStart('0');
      }

      private static string GetCpuString(byte[] cpuType, byte[] cpuSpecies)
      {
         UInt32 cpuTypeAsUInt32 = GetCpuOrOsNumber(cpuType);
         UInt32 cpuSpeciesAsUInt32 = GetCpuOrOsNumber(cpuSpecies);

         return GetCpuString((cpuTypeAsUInt32 * 100000) + cpuSpeciesAsUInt32);
      }

      private static string GetCpuString(UInt32 cpuId)
      {
         switch (cpuId)
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

      private static string GetOsString(byte[] osType, byte[] osSpecies)
      {
         UInt32 osTypeAsUInt32 = GetCpuOrOsNumber(osType);
         UInt32 osSpeciesAsUInt32 = GetCpuOrOsNumber(osSpecies);

         return GetOsString((osTypeAsUInt32 * 100000) + osSpeciesAsUInt32);
      }

      private static string GetOsString(UInt32 osId)
      {
         switch (osId)
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
   }
}
