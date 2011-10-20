/*
 * HFM.NET - Queue Data Class
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

namespace HFM.Queue
{
   /// <summary>
   /// Represents the System (CPU) Type
   /// </summary>
   internal enum SystemType
   {
      // ReSharper disable InconsistentNaming
      x86 = 0,
      PPC
      // ReSharper restore InconsistentNaming
   }

   /// <summary>
   /// Queue Data Class
   /// </summary>
   [CLSCompliant(false)]
   public class QueueData
   {
      #region Fields
   
      /// <summary>
      /// Queue Structure
      /// </summary>
      private readonly Data _q;
      
      private readonly SystemType _system = SystemType.x86;
      /// <summary>
      /// The System (CPU) Type
      /// </summary>
      internal SystemType System
      {
         get { return _system; }
      }

      /// <summary>
      /// Epoch 2000 Date Time structure (UTC)
      /// </summary>
      public static readonly DateTime Epoch2000 = new DateTime(2000, 1, 1, 0, 0 ,0, DateTimeKind.Utc);
      
      #endregion

      #region Constructor

      /// <summary>
      /// Primary Constructor (Set Queue)
      /// </summary>
      /// <param name="q">Queue Structure</param>
      internal QueueData(Data q)
      {
         _q = q;

         // determine system type based on the version field
         if (IsBigEndian(_q.Version))
         {
            _system = SystemType.PPC;
         }
      }
      
      #endregion

      #region queue.dat Properties

      /// <summary>
      /// Queue (client) version
      /// </summary>
      public UInt32 Version
      {
         get
         {
            byte[] b = GetSystemBytes(_q.Version);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Current index number
      /// </summary>
      public UInt32 CurrentIndex
      {
         get
         {
            byte[] b = GetSystemBytes(_q.CurrentIndex);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Performance fraction
      /// </summary>
      public float PerformanceFraction
      {
         get
         {
            byte[] b = GetSystemBytes(_q.PerformanceFraction);
            return BitConverter.ToSingle(b, 0);
         }
      }

      /// <summary>
      /// Performance fraction unit weight
      /// </summary>
      public UInt32 PerformanceFractionUnitWeight
      {
         get
         {
            byte[] b = GetSystemBytes(_q.PerformanceFractionUnitWeight);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Download rate sliding average
      /// </summary>
      public float DownloadRateAverage
      {
         get
         {
            byte[] b = GetSystemBytes(_q.DownloadRateAverage);
            return (BitConverter.ToUInt32(b, 0) / 1000f);
         }
      }

      /// <summary>
      /// Download rate unit weight
      /// </summary>
      public UInt32 DownloadRateUnitWeight
      {
         get
         {
            byte[] b = GetSystemBytes(_q.DownloadRateUnitWeight);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Upload rate sliding average
      /// </summary>
      public float UploadRateAverage
      {
         get
         {
            byte[] b = GetSystemBytes(_q.UploadRateAverage);
            return (BitConverter.ToUInt32(b, 0) / 1000f);
         }
      }

      /// <summary>
      /// Upload rate unit weight
      /// </summary>
      public UInt32 UploadRateUnitWeight
      {
         get
         {
            byte[] b = GetSystemBytes(_q.UploadRateUnitWeight);
            return BitConverter.ToUInt32(b, 0);
         }
      }

      /// <summary>
      /// Results successfully sent (after upload failures) (UTC)
      /// </summary>
      public DateTime ResultsSentUtc
      {
         get { return Epoch2000.AddSeconds(BitConverter.ToUInt32(_q.ResultsSent, 0)); }
      }

      /// <summary>
      /// Results successfully sent (after upload failures) (Local)
      /// </summary>
      public DateTime ResultsSentLocal
      {
         get { return ResultsSentUtc.ToLocalTime(); }
      }
      
      #endregion
      
      #region Entry Accessors

      /// <summary>
      /// Get the QueueEntry at the specified Index.
      /// </summary>
      /// <param name="index">Queue Entry Index</param>
      public QueueEntry GetQueueEntry(uint index)
      {
         return new QueueEntry(_q.Entries[index], index, this);
      }
      
      #endregion

      internal byte[] GetSystemBytes(byte[] b)
      {
         byte[] bytes = new byte[b.Length];
         Array.Copy(b, bytes, b.Length);

         if (System.Equals(SystemType.PPC))
         {
            Array.Reverse(bytes);
         }
         return bytes;
      }

      internal static bool IsBigEndian(byte[] b)
      {
         UInt32 value = BitConverter.ToUInt32(b, 0);

         if (value > UInt16.MaxValue)
         {
            return true;
         }

         return false;
      }
   }
}
