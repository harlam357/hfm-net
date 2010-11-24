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
      x86 = 0,
      PPC
   }

   [CLSCompliant(false)]
   public class QueueData
   {
      #region Fields
   
      /// <summary>
      /// Queue Structure
      /// </summary>
      private Data _q;
      
      private readonly bool _dataPopulated;
      /// <summary>
      /// Flag Denoting if Class holds a Populated or Empty Queue Structure
      /// </summary>
      public bool DataPopulated
      {
         get { return _dataPopulated; }
      }

      private readonly SystemType _system = SystemType.x86;
      /// <summary>
      /// The System (CPU) Type
      /// </summary>
      internal SystemType System
      {
         get { return _system; }
      }
      
      #endregion

      #region Constructor

      /// <summary>
      /// Constructor (Clear Queue)
      /// </summary>
      internal QueueData()
      {
         _q = new Data();
      }

      /// <summary>
      /// Constructor (Set Queue)
      /// </summary>
      /// <param name="q">Queue Structure</param>
      internal QueueData(Data q)
      {
         _q = q;
         _dataPopulated = true;

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
      /// Results successfully sent (after upload failures)
      /// </summary>
      public UInt32 ResultsSent
      {
         get
         {
            //byte[] b = new byte[_q.ResultsSent.Length];
            //Array.Copy(_q.ResultsSent, b, _q.ResultsSent.Length);
            return BitConverter.ToUInt32(_q.ResultsSent, 0);
         }
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
