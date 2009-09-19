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
using System.Runtime.InteropServices;
using System.Text;

namespace HFM.Instances
{
   [CLSCompliant(false)]
   [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 7168)]
   public struct Queue
   {
      [FieldOffset(0)] /* 0000 Queue (client) version (v2.17 and above) */
      public UInt32 Version;

      [FieldOffset(4)] /* 0004 Current index number */
      public UInt32 CurrentIndex;

      [FieldOffset(8)] /* 0008 Array of ten queue entries */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
      public Entry[] Entries;

      [FieldOffset(7128)] /* 7128 Performance fraction (as of v3.24) */
      public float PerformanceFraction;
      
      [FieldOffset(7132)] /* 7132 Performance fraction unit weight (as of v3.24) */
      public UInt32 PerformanceFractionUnitWeight;
      
      [FieldOffset(7136)] /* 7136 Download rate sliding average (as of v4.00) */
      public UInt32 DownloadRateAverage;
      
      [FieldOffset(7140)] /* 7140 Download rate unit weight (as of v4.00) */
      public UInt32 DownloadRateUnitWeight;
      
      [FieldOffset(7144)] /* 7144 Upload rate sliding average (as of v4.00) */
      public UInt32 UploadRateAverage;
      
      [FieldOffset(7148)] /* 7148 Upload rate unit weight (as of v4.00) */
      public UInt32 UploadRateUnitWeight;
      
      [FieldOffset(7152)] /* 7152 Results successfully sent (after upload failures) (as of v5.00) (LE) */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
      public string ResultsSent;
      
      [FieldOffset(7156)] /* 7156 (as of v5.00) ...all zeros after queue conversion... */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
      public string z7156;
   }

   [CLSCompliant(false)]
   [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 712)]
   public struct Entry
   {
      /*** 0 = Empty, Deleted, Finished, or Garbage 
       *   1 = Folding Now or Queued 
       *   2 = Ready for Upload 
       *   3 = Abandonded (Ignore is found)
       *   4 = Fetching from Server
       ***/ 
      [FieldOffset(0)] /* 000 Status */
      public UInt32 Status; 

      [FieldOffset(4)] /* 004 Pad for Windows, others as of v4.01, as of v6.01 number of SMP Cores to use (LE) */
      public UInt32 UseCores;

      /*** 0 = Begin Time
       *   4 = End Time 
       *   Others = Unknown 
       ***/
      [FieldOffset(8)] /* 008 Time data (epoch 0000 1jan00 UTC) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public UInt32[] TimeData; 

      [FieldOffset(40)] /* 040 Server IP address (until v3.0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] OldServerIP; /*** Ignore this value ***/

      /***
       * 0 = Not Uploaded
       * 1 = Uploaded
       ***/
      [FieldOffset(44)] /* 044 Upload status */ 
      public UInt32 UploadStatus; 

      [FieldOffset(48)] /* 048 Web address for core downloads */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      public string CoreDownloadUrl;

      [FieldOffset(176)] /* 176 Misc1a */
      public UInt32 Misc1a;
      
      [FieldOffset(180)] /* 180 Core_xx number (hex) */
      public UInt32 CoreNumber; /*** Convert to Hex ***/

      [FieldOffset(184)] /* 184 Misc1b */
      public UInt32 Misc1b;
      
      [FieldOffset(188)] /* 188 wudata_xx.dat file size */
      public UInt32 WuDataFileSize;

      [FieldOffset(192)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string z192;

      //[FieldOffset(208)] /* 208 Project number (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectNumber;

      //[FieldOffset(210)] /* 210 Run (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectRun;

      //[FieldOffset(212)] /* 212 Clone (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectClone;

      //[FieldOffset(214)] /* 214 Generation (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
      //public string ProjectGen;

      //[FieldOffset(216)] /* 216 WU issue time (LE) */
      //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      //public string ProjectIssued;

      [FieldOffset(208)] /* 208-223 Project R/C/G (see above) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      public byte[] Project; /*** Needs post read processing ***/
      
      [FieldOffset(224)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
      public string z224;

      [FieldOffset(260)] /* 260 Machine ID (LE) */
      public UInt32 MachineID;

      [FieldOffset(264)] /* 264 Server IP address */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] ServerIP; /*** Needs post read processing ***/

      [FieldOffset(268)] /* 268 Server port number */
      public UInt32 ServerPort;

      [FieldOffset(272)] /* 272 Work unit type */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string WorkUnitType;

      [FieldOffset(336)] /* 336 User Name */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string UserName;

      [FieldOffset(400)] /* 400 Team Number */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string TeamNumber;

      [FieldOffset(464)] /* 464 Stored ID for unit (UserID + MachineID) (LE or BE, usually BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public byte[] UserAndMachineID; /*** Needs post read processing ***/

      [FieldOffset(472)] /* 472 Benchmark (as of v3.24) (LE) */
      public UInt32 OldBenchmark; /*** Ignore this value ***/

      [FieldOffset(476)] /* 476 Misc3b (unused as of v3.24) (LE); Benchmark (as of v5.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] Benchmark; /*** Needs post read processing ***/

      [FieldOffset(480)] /* 480 CPU type (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] CpuType;

      [FieldOffset(484)] /* 484 OS type (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] OsType;

      [FieldOffset(488)] /* 488 CPU species (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] CpuSpecies;

      [FieldOffset(492)] /* 492 OS species (LE or BE, sometimes 0) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] OsSpecies;

      [FieldOffset(496)] /* 496 Allowed time to return (seconds) */
      public UInt32 ExpirationInSeconds; /*** Final Deadline Length ***/

      [FieldOffset(500)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
      public string z500;

      [FieldOffset(508)] /* 508 Assignment info present flag (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] AssignmentInfoPresent;

      [FieldOffset(512)] /* 512 Assignment timestamp (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] AssignmentTimeStamp;

      [FieldOffset(516)] /* 516 Assignment info (LE or BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] AssignmentInfoChecksum;

      [FieldOffset(520)] /* 520 Collection server IP address (as of v5.00) (LE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] CollectionServerIP; /*** Needs post read processing ***/

      [FieldOffset(524)] /* 524 Download started time (as of v5.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] DownloadStartedTime;

      [FieldOffset(528)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string z528;

      [FieldOffset(544)] /* 544 Number of SMP cores (as of v5.91) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] NumberOfSmpCores; /*** Needs post read processing ***/

      [FieldOffset(548)] /* 548 Tag of Work Unit (as of v5.00) */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string WorkUnitTag;

      [FieldOffset(564)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string z564;

      [FieldOffset(580)] /* 580 Passkey (as of v6.00) */
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string Passkey;

      [FieldOffset(612)] /* 612 Flops per CPU (core) (as of v6.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] Flops; /*** Needs post read processing ***/

      [FieldOffset(616)] /* 616 Available memory (as of v6.00) (BE) */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public byte[] Memory; /*** Needs post read processing ***/

      [FieldOffset(620)] /* 620 Available GPU memory (as of v6.20) (LE) */
      public UInt32 GpuMemory;

      [FieldOffset(624)]
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string z624;

      /***
       * 0 = Due Date - This time is calculated by the client when it downloads a unit.
       *                It is determined by adding the "begin" time to the expiration period.
       * 1-3 = Unknown
       ***/                
      [FieldOffset(688)] /* 688 WU expiration time */
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public UInt32[] ExpirationTime;

      [FieldOffset(704)] /* 704 Packet size limit (as of v5.00) */
      public UInt32 PacketSizeLimit;
      
      [FieldOffset(708)] /* 708 Number of upload failures (as of v5.00) */
      public UInt32 NumberOfUploadFailures;
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
      private Entry _qEntry;
   
      public QueueEntry(Entry qEntry)
      {
         _qEntry = qEntry;
      }
      
      public UInt32 Status
      {
         get { return _qEntry.Status; }
      } 

      public UInt32 UseCores
      {
         get { return _qEntry.UseCores; }
      } 

      public DateTime BeginTimeUtc
      {
         get 
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(_qEntry.TimeData[0]);
         }
      }

      public DateTime BeginTimeLocal
      {
         get
         {
            return BeginTimeUtc.ToLocalTime();
         }
      }


      public DateTime EndTimeUtc
      {
         get
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(_qEntry.TimeData[4]);
         }
      }

      public DateTime EndTimeLocal
      {
         get
         {
            return EndTimeUtc.ToLocalTime();
         }
      }

      public UInt32 UploadStatus
      {
         get { return _qEntry.UploadStatus; }
      }

      public string CoreDownloadUrl
      {
         get { return String.Format("http://{0}/Core_{1}.fah", _qEntry.CoreDownloadUrl, CoreNumber); }
      }

      public UInt32 Misc1a
      {
         get { return _qEntry.Misc1a; }
      }
      
      public string CoreNumber
      {
         get { return _qEntry.CoreNumber.ToString("x"); }
      }

      public UInt32 Misc1b
      {
         get { return _qEntry.Misc1b; }
      }
      
      public UInt32 WuDataFileSize
      {
         get { return _qEntry.WuDataFileSize; }
      }

      public UInt16 ProjectNumber
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 0, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

      public UInt16 ProjectRun
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 2, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

      public UInt16 ProjectClone
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 4, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

      public UInt16 ProjectGen
      {
         get 
         { 
            byte[] b = new byte[2];
            Array.Copy(_qEntry.Project, 6, b, 0, 2);
            return BitConverter.ToUInt16(b, 0);
         }
      }

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

      public DateTime ProjectIssuedLocal
      {
         get
         {
            return ProjectIssuedUtc.ToLocalTime();
         }
      }
      
      public UInt32 MachineID
      {
         get { return _qEntry.MachineID; }
      }
      
      public string ServerIP
      {
         get
         {
            return String.Format("{0}.{1}.{2}.{3}", _qEntry.ServerIP[3], _qEntry.ServerIP[2], 
                                                    _qEntry.ServerIP[1], _qEntry.ServerIP[0]);
         }
      }

      public UInt32 ServerPort
      {
         get { return _qEntry.ServerPort; }
      }

      public string WorkUnitType
      {
         get { return _qEntry.WorkUnitType; }
      }

      public string UserName
      {
         get { return _qEntry.UserName; }
      }

      public string TeamNumber
      {
         get { return _qEntry.TeamNumber; }
      }

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

      public UInt32 CpuType
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.CpuType);
         }
      }

      public UInt32 OsType
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.OsType);
         }
      }

      public UInt32 CpuSpecies
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.CpuSpecies);
         }
      }

      public UInt32 OsSpecies
      {
         get
         {
            return GetCpuOrOsNumber(_qEntry.OsSpecies);
         }
      }
      
      public string CpuString
      {
         get { return GetCpuString(_qEntry.CpuType, _qEntry.CpuSpecies); }
      }
      
      public string OsString
      {
         get { return GetOsString(_qEntry.OsType, _qEntry.OsSpecies); }
      }

      public UInt32 ExpirationInSeconds
      {
         get { return _qEntry.ExpirationInSeconds; }
      }

      public UInt32 ExpirationInMinutes
      {
         get { return ExpirationInSeconds / 60; }
      }

      public UInt32 ExpirationInHours
      {
         get { return ExpirationInMinutes / 60; }
      }

      public UInt32 ExpirationInDays
      {
         get { return ExpirationInHours / 24; }
      }

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

      public DateTime AssignmentTimeStampLocal
      {
         get
         {
            return AssignmentTimeStampUtc.ToLocalTime();
         }
      }

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

      public string CollectionServerIP
      {
         get
         {
            return String.Format("{0}.{1}.{2}.{3}", _qEntry.CollectionServerIP[3], _qEntry.CollectionServerIP[2], 
                                                    _qEntry.CollectionServerIP[1], _qEntry.CollectionServerIP[0]);
         }
      }

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
     
      public string WorkUnitTag
      {
         get { return _qEntry.WorkUnitTag; }
      }

      public string Passkey
      {
         get { return _qEntry.Passkey; }
      }

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
      
      public double MegaFlops
      {
         get 
         { 
            return Flops / 1000000.000000;
         }
      }

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

      public UInt32 GpuMemory
      {
         get { return _qEntry.GpuMemory; }
      }

      public DateTime DueDateUtc
      {
         get 
         {
            DateTime d = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return d.AddSeconds(_qEntry.ExpirationTime[0]); 
         }
      }
      
      public DateTime DueDateLocal
      {
         get { return DueDateUtc.ToLocalTime(); }
      }

      public UInt32 PacketSizeLimit
      {
         get { return _qEntry.PacketSizeLimit; }
      }
      
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
      private Queue _q;
      private string _queueFilePath;
      
      public string QueueFilePath
      {
         get { return _queueFilePath; }
      }
      
      public UInt32 Version
      {
         get { return _q.Version; }
      }
      
      public float PerformanceFraction
      {
         get { return _q.PerformanceFraction; }
      }

      public UInt32 PerformanceFractionUnitWeight
      {
         get { return _q.PerformanceFractionUnitWeight; }
      }
   
      //public QueueReader()
      //{
         
      //}
      
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
      
      public QueueEntry GetQueueEntry(int Index)
      {
         return new QueueEntry(_q.Entries[Index]);
      }
   }
}
