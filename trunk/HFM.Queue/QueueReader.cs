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
using System.IO;
using System.Runtime.InteropServices;

namespace HFM.Queue
{
   [CLSCompliant(false)]
   public class QueueReader
   {
      public const int QueueLength = 7168;
      public const int QueueEntryLength = 712;

      /// <summary>
      /// Queue Base Class
      /// </summary>
      private QueueBase _qBase;
      /// <summary>
      /// Queue Base Class
      /// </summary>
      public QueueBase Queue
      {
         get { return _qBase; }
      }

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
      /// Queue Read Ok Flag
      /// </summary>
      private bool _queueReadOk;
      /// <summary>
      /// Queue Read Ok Flag
      /// </summary>
      public bool QueueReadOk
      {
         get { return _queueReadOk; }
      }
      
      /// <summary>
      /// Read queue.dat file
      /// </summary>
      /// <param name="filePath">Path to queue.dat file</param>
      /// <exception cref="ArgumentException">Throws if FileName is Null or Empty.</exception>
      public void ReadQueue(string filePath)
      {
         if (String.IsNullOrEmpty(filePath)) throw new ArgumentException("Argument 'FilePath' cannot be a null or empty string.");
      
         _queueFilePath = filePath;
      
         try
         {
            using (var reader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
               QueueData q = FromBinaryReaderBlock(reader);

               // at this point we know we've read a file of expected length
               // and no exceptions were thrown in the process
               if (QueueReadOk)
               {
                  _qBase = new QueueBase(q);

                  // If version is less than 5.xx, don't trust this data
                  // this class is not setup to handle legacy clients
                  // If version is greater than 6.xx, don't trust this data
                  // this class has not been tested with clients beyond 6.xx
                  if (_qBase.Version < 500 || _qBase.Version > 699)
                  {
                     ClearQueue();
                  }
               }
            }
         }
         catch (Exception)
         {
            ClearQueue();
            
            throw;
         }
      }

      private QueueData FromBinaryReaderBlock(BinaryReader br)
      {
         Debug.Assert(br != null);
      
         // Read byte array
         byte[] buff = br.ReadBytes(Marshal.SizeOf(typeof(QueueData)));

         if (buff.Length == QueueLength)
         {
            _queueReadOk = true;

            // Make sure that the Garbage Collector doesn't move our buffer 
            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);

            // Marshal the bytes
            var q = (QueueData)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(QueueData));
            handle.Free(); //Give control of the buffer back to the GC 

            return q;
         }

         _queueReadOk = false;
         return new QueueData();
      }
      
      /// <summary>
      /// Clear the Queue Structure and Set Read Flag False
      /// </summary>
      public void ClearQueue()
      {
         _queueReadOk = false;
         _qBase = new QueueBase();
      }
   }
}
