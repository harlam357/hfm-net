/*
 * HFM.NET - Queue Entry Class
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
using System.Diagnostics;
using System.Text;

using HFM.Framework;

namespace HFM.Queue
{
   [CLSCompliant(false)]
   public class QueueEntry : IQueueEntry
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
      /// The QueueReader that Created this QueueEntry
      /// </summary>
      private readonly QueueBase _qBase;

      /// <summary>
      /// Primary Constructor
      /// </summary>
      /// <param name="qEntry">Entry Structure</param>
      /// <param name="thisIndex">This Entry Index</param>
      /// <param name="currentIndex">Current Entry Index</param>
      /// <param name="qBase">The QueueReader that Created this QueueEntry</param>
      public QueueEntry(Entry qEntry, UInt32 thisIndex, UInt32 currentIndex, QueueBase qBase)
      {
         _qEntry = qEntry;
         _thisIndex = thisIndex;
         _currentIndex = currentIndex;
         _qBase = qBase;
      }

      /// <summary>
      /// Status (0) Empty / (1) Active or (1) Ready / (2) Ready for Upload / (3) = Abandonded (Ignore if found) / (4) Fetching from Server
      /// </summary>
      public UInt32 Status
      {
         get
         {
            byte[] b = _qBase.GetSystemBytes(_qEntry.Status);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Status Enumeration
      /// </summary>
      public QueueEntryStatus EntryStatus
      {
         get
         {
            switch (Status)
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
      /// Specifies a Factor Value denoting the Speed of Completion in relationship to the Maximum Expiration Time.
      /// </summary>
      public double SpeedFactor
      {
         get
         {
            if (EntryStatus.Equals(QueueEntryStatus.Finished) ||
                EntryStatus.Equals(QueueEntryStatus.ReadyForUpload))
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
            if (_qBase.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_qEntry.TimeData[1]);
               byte[] bytes = _qBase.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return d.AddSeconds(seconds);
            }
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
            if (_qBase.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_qEntry.TimeData[5]);
               byte[] bytes = _qBase.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return d.AddSeconds(seconds);
            }
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
         get
         {
            byte[] b = _qBase.GetSystemBytes(_qEntry.UploadStatus);
            return BitConverter.ToUInt32(b, 0);
         }
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
         get
         {
            byte[] b = _qBase.GetSystemBytes(_qEntry.Misc1a);
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
            byte[] b = _qBase.GetSystemBytes(_qEntry.CoreNumber);
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
            byte[] b = _qBase.GetSystemBytes(_qEntry.Misc1b);
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
            byte[] b = _qBase.GetSystemBytes(_qEntry.WuDataFileSize);
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
         get
         {
            if (IsMachineIDBigEndian)
            {
               byte[] b = new byte[4];
               Array.Copy(_qEntry.MachineID, b, _qEntry.MachineID.Length);
               Array.Reverse(b);
               return BitConverter.ToUInt32(b, 0);
            }

            return BitConverter.ToUInt32(_qEntry.MachineID, 0);
         }
      }

      /// <summary>
      /// Denotes Byte Order of Machine ID Field
      /// </summary>
      public bool IsMachineIDBigEndian
      {
         get
         {
            return QueueBase.IsBigEndian(_qEntry.MachineID);
         }
      }

      /// <summary>
      /// Server IP address
      /// </summary>
      public string ServerIP
      {
         get
         {
            if (_qBase.System.Equals(SystemType.PPC))
            {
               return String.Format("{0}.{1}.{2}.{3}", _qEntry.ServerIP[0], _qEntry.ServerIP[1],
                                                       _qEntry.ServerIP[2], _qEntry.ServerIP[3]);
            }

            return String.Format("{0}.{1}.{2}.{3}", _qEntry.ServerIP[3], _qEntry.ServerIP[2],
                                                    _qEntry.ServerIP[1], _qEntry.ServerIP[0]);
         }
      }

      /// <summary>
      /// Server port number
      /// </summary>
      public UInt32 ServerPort
      {
         get
         {
            byte[] b = _qBase.GetSystemBytes(_qEntry.ServerPort);
            return BitConverter.ToUInt32(b, 0);
         }
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
      /// The Team number attached to this queue entry
      /// </summary>
      public UInt32 TeamNumber
      {
         get
         {
            UInt32 team;
            if (UInt32.TryParse(_qEntry.Team, out team))
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
            byte[] bytes = new byte[_qEntry.UserAndMachineID.Length];
            Array.Copy(_qEntry.UserAndMachineID, bytes, _qEntry.UserAndMachineID.Length);
            if (IsMachineIDBigEndian == false)
            {
               Array.Reverse(bytes);
            }

            StringBuilder sb = new StringBuilder(_qEntry.UserAndMachineID.Length * 2);
            foreach (byte b in bytes)
            {
               sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
         }
      }

      /// <summary>
      /// UserID associated with this queue entry
      /// </summary>
      public string UserID
      {
         get
         {
            return GetUserIDFromUserAndMachineID(_qEntry.UserAndMachineID, MachineID, IsMachineIDBigEndian);
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
         get
         {
            byte[] b = _qBase.GetSystemBytes(_qEntry.ExpirationInSeconds);
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
            byte[] b = new byte[_qEntry.AssignmentInfoPresent.Length];
            Array.Copy(_qEntry.AssignmentInfoPresent, b, 4);

            if (QueueBase.IsBigEndian(_qEntry.AssignmentInfoPresent))
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

               if (QueueBase.IsBigEndian(_qEntry.AssignmentInfoPresent))
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

               // Reverse this value if 'AssignmentInfoPresent' IS NOT Big Endian
               // qd.c prints the bytes in reverse order so it stands to reason 
               // that if 'AssignmentInfoPresent' IS Big Endian then the bytes
               // would not need reversed.  This theory is UNTESTED.
               if (QueueBase.IsBigEndian(_qEntry.AssignmentInfoPresent) == false)
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
            if (_qBase.System.Equals(SystemType.PPC))
            {
               return String.Format("{0}.{1}.{2}.{3}", _qEntry.CollectionServerIP[0], _qEntry.CollectionServerIP[1],
                                                       _qEntry.CollectionServerIP[2], _qEntry.CollectionServerIP[3]);
            }
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
         get
         {
            int i = Array.IndexOf(_qEntry.WorkUnitTag, (byte)0);
            if (i >= 0)
            {
               return ASCIIEncoding.ASCII.GetString(_qEntry.WorkUnitTag, 0, i);
            }
            else
            {
               return ASCIIEncoding.ASCII.GetString(_qEntry.WorkUnitTag, 0, _qEntry.WorkUnitTag.Length);
            }
         }
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
      public DateTime DueTimeUtc
      {
         get
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (_qBase.System.Equals(SystemType.PPC))
            {
               byte[] b = BitConverter.GetBytes(_qEntry.ExpirationTime[1]);
               byte[] bytes = _qBase.GetSystemBytes(b);
               UInt32 seconds = BitConverter.ToUInt32(bytes, 0);
               return d.AddSeconds(seconds);
            }
            return d.AddSeconds(_qEntry.ExpirationTime[0]);
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
            byte[] b = _qBase.GetSystemBytes(_qEntry.PacketSizeLimit);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Number of upload failures
      /// </summary>
      public UInt32 NumberOfUploadFailures
      {
         get { return _qEntry.NumberOfUploadFailures; }
      }

      public static byte[] HexToData(string hexString)
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

      public static string GetUserIDFromUserAndMachineID(byte[] UserAndMachineID, UInt32 MachineID, bool IsMachineIDBigEndian)
      {
         Debug.Assert(UserAndMachineID.Length == 8);

         /*** Remove the MachineID from UserAndMachineID ***/

         byte[] bytes = new byte[UserAndMachineID.Length];
         Array.Copy(UserAndMachineID, bytes, UserAndMachineID.Length);
         if (IsMachineIDBigEndian)
         {
            // Reverse the bytes so we get the least significant byte first
            Array.Reverse(bytes);
         }

         // Convert to 64bit integer
         UInt64 value = BitConverter.ToUInt64(bytes, 0);
         value = value - MachineID;
         // Convert back to bytes after MachineID has been subtracted
         bytes = BitConverter.GetBytes(value);
         // Reverse the bytes so we show the most significant byte first
         Array.Reverse(bytes);

         StringBuilder sb = new StringBuilder(UserAndMachineID.Length * 2);
         foreach (byte b in bytes)
         {
            sb.AppendFormat("{0:X2}", b);
         }
         return sb.ToString().TrimStart('0');
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
   }
}
