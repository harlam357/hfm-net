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
   /// Provides methods to read a Folding@Home client v5 or v6 queue.dat file.
   /// </summary>
   [CLSCompliant(false)]
   public static class QueueReader
   {
      private const int QueueLength = 7168;
      //private const int QueueEntryLength = 712;

      /// <summary>
      /// Reads a queue.dat file from disk.
      /// </summary>
      /// <param name="path">Path to the queue.dat file.</param>
      /// <exception cref="System.ArgumentException">Throws if fileName is null or empty.</exception>
      /// <exception cref="System.IO.IOException">Throw on queue.dat read failure.</exception>
      /// <exception cref="System.InvalidOperationException">Throws if queue.dat version is not supported. Supported versions are between 500 and 699.</exception>
      public static QueueData ReadQueue(string path)
      {
         if (String.IsNullOrEmpty(path)) throw new ArgumentException("Argument 'path' cannot be a null or empty string.", "path");
      
         Data q = FromBinaryReaderBlock(path);

         // at this point we know we've read a file of expected length
         // and no exceptions were thrown in the process
         var data = new QueueData(q);

         // If version is less than 5.xx, don't trust this data
         // this class is not setup to handle legacy clients
         // If version is greater than 6.xx, don't trust this data
         // this class has not been tested with clients beyond 6.xx
         if (data.Version < 500 || data.Version > 699)
         {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
               "The version ({0}) of this queue.dat file is not supported.", data.Version));
         }

         return data;
      }

      private static Data FromBinaryReaderBlock(string path)
      {
         Debug.Assert(String.IsNullOrEmpty(path) == false);

         byte[] buff = GetRawData(path);
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
      private static byte[] GetRawData(string path)
      {
         Debug.Assert(String.IsNullOrEmpty(path) == false);

         try
         {
            // .NET types - presents little to no danger if Dispose() is called multiple times
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream))
            {
               // Read byte array
               return reader.ReadBytes(Marshal.SizeOf(typeof(Data)));
            }
         }
         catch (Exception ex)
         {
            throw new IOException(String.Format(CultureInfo.CurrentCulture, "Failed to read {0}.", path), ex);
         }
      }
   }
}
