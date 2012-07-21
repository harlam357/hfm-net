/*
 * HFM.NET - Queue Reader Class
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace HFM.Queue
{
   /// <summary>
   /// Queue Reader Class
   /// </summary>
   [CLSCompliant(false)]
   public static class QueueReader
   {
      private const int QueueLength = 7168;
      //private const int QueueEntryLength = 712;

      /// <summary>
      /// Read queue.dat file
      /// </summary>
      /// <param name="filePath">Path to queue.dat file</param>
      /// <exception cref="System.ArgumentException">Throws if fileName is null or empty.</exception>
      /// <exception cref="System.IO.IOException">Throw on queue.dat read failure.</exception>
      /// <exception cref="System.NotSupportedException">Throws if queue.dat version is not supported. Supported versions are between 500 and 699.</exception>
      public static QueueData ReadQueue(string filePath)
      {
         if (String.IsNullOrEmpty(filePath)) throw new ArgumentException("Argument 'filePath' cannot be a null or empty string.");
      
         Data q = FromBinaryReaderBlock(filePath);

         // at this point we know we've read a file of expected length
         // and no exceptions were thrown in the process
         var qData = new QueueData(q);

         // If version is less than 5.xx, don't trust this data
         // this class is not setup to handle legacy clients
         // If version is greater than 6.xx, don't trust this data
         // this class has not been tested with clients beyond 6.xx
         if (qData.Version < 500 || qData.Version > 699)
         {
            throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture,
               "The version ({0}) of this queue.dat file is not supported.", qData.Version));
         }

         return qData;
      }

      private static Data FromBinaryReaderBlock(string filePath)
      {
         Debug.Assert(String.IsNullOrEmpty(filePath) == false);

         byte[] buff = GetRawData(filePath);
         if (buff.Length == QueueLength)
         {
            // Make sure that the Garbage Collector doesn't move our buffer 
            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);

            // Marshal the bytes
            var q = (Data)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Data));
            handle.Free(); //Give control of the buffer back to the GC 

            return q;
         }
          
         throw new IOException(String.Format(CultureInfo.CurrentCulture, 
            "Length of data read was {0} bytes and not the expected {1} bytes.", buff.Length, QueueLength));
      }

      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      private static byte[] GetRawData(string filePath)
      {
         Debug.Assert(String.IsNullOrEmpty(filePath) == false);

         try
         {
            // .NET types - presents little to no danger if Dispose() is called multiple times
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
               // Read byte array
               return reader.ReadBytes(Marshal.SizeOf(typeof(Data)));
            }
         }
         catch (Exception ex)
         {
            throw new IOException(String.Format(CultureInfo.CurrentCulture, "Failed to read {0}.", filePath), ex);
         }
      }
   }
}
