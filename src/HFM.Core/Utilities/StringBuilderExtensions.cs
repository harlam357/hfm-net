/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace HFM.Core
{
   internal static class StringBuilderExtensions
   {
      private const int MaxChunkSize = 8000;

      internal static IEnumerable<char[]> GetChunks(this StringBuilder sb)
      {
         if (sb == null) throw new ArgumentNullException("sb");

         return GetChunks(sb, MaxChunkSize);
      }

      private static IEnumerable<char[]> GetChunks(this StringBuilder sb, int chunkSize)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (chunkSize < 1000) throw new ArgumentOutOfRangeException("chunkSize");

         var list = new List<char[]>();
         for (int i = 0; i < sb.Length; i += chunkSize)
         {
            int length = i + chunkSize < sb.Length ? chunkSize : sb.Length - i;
            var temp = new char[length];
            sb.CopyTo(i, temp, 0, temp.Length);
            list.Add(temp);
         }
         return list.AsReadOnly();
      }

      //internal static StringBuilder MergeChunks(this IEnumerable<char[]> chunks)
      //{
      //   if (chunks == null) throw new ArgumentNullException("chunks");
      //
      //   var sb = new StringBuilder(chunks.Count() * MaxChunkSize);
      //   foreach (var chunk in chunks)
      //   {
      //      sb.Append(chunk);
      //   }
      //   return sb;
      //}

      internal static IEnumerable<string> Split(this StringBuilder sb, char splitChar)
      {
         if (sb == null) throw new ArgumentNullException("sb");

         var list = new LinkedList<string>();

         int lastIndex = 0;
         for (int i = 0; i < sb.Length; i++)
         {
            if (sb[i] == splitChar)
            {
               var buffer = new char[i - lastIndex];
               sb.CopyTo(lastIndex, buffer, 0, buffer.Length);
               lastIndex = i + 1;

               list.AddLast(new string(buffer));
            }
         }

         return list;
      }
   }
}
