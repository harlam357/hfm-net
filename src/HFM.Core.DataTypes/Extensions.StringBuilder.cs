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

namespace HFM.Core.DataTypes
{
   public static partial class Extensions
   {
      ///// <summary>
      ///// Reports the index of the first occurrence of the specified string in this instance.
      ///// </summary>
      ///// <param name="sb">The System.Text.StringBuilder to search.</param>
      ///// <param name="value">The string to seek.</param>
      ///// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
      ///// <exception cref="System.ArgumentNullException">value is null.</exception>
      //public static int IndexOf(this StringBuilder sb, string value)
      //{
      //   return IndexOf(sb, value, 0);
      //}

      ///// <summary>
      ///// Reports the index of the first occurrence of the specified string in this instance. The search starts at a specified character position.
      ///// </summary>
      ///// <param name="sb">The System.Text.StringBuilder to search.</param>
      ///// <param name="value">The string to seek.</param>
      ///// <param name="startIndex">The search starting position.</param>
      ///// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is startIndex.</returns>
      ///// <exception cref="System.ArgumentNullException">value is null.</exception>
      ///// <exception cref="System.ArgumentOutOfRangeException">startIndex is negative. -or- startIndex specifies a position not within this instance.</exception>
      //public static int IndexOf(this StringBuilder sb, string value, int startIndex)
      //{
      //   return IndexOf(sb, value, startIndex, StringComparison.CurrentCulture);
      //}

      /// <summary>
      /// Reports the index of the first occurrence of the specified string in the current System.String object. A parameter specifies the type of search to use for the specified string.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder to search.</param>
      /// <param name="value">The string to seek.</param>
      /// <param name="ignoreCase">Ignore character case.</param>
      /// <returns>The index position of the value parameter if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
      /// <exception cref="System.ArgumentNullException">value is null.</exception>
      public static int IndexOf(this StringBuilder sb, string value, bool ignoreCase)
      {
         return IndexOf(sb, value, 0, ignoreCase);
      }

      /// <summary>
      /// Reports the index of the first occurrence of the specified string in the current System.String object. Parameters specify the starting search position in the current string and the type of search to use for the specified string.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder to search.</param>
      /// <param name="value">The string to seek.</param>
      /// <param name="startIndex">The search starting position.</param>
      /// <param name="ignoreCase">Ignore character case.</param>
      /// <returns>The zero-based index position of the value parameter if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is startIndex.</returns>
      /// <exception cref="System.ArgumentNullException">value is null.</exception>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex is negative. -or- startIndex specifies a position not within this instance.</exception>
      public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (value == null) throw new ArgumentNullException("value");
         if (startIndex < 0 || startIndex > sb.Length) throw new ArgumentOutOfRangeException("startIndex");

         if (value.Length == 0)
         {
            return startIndex;
         }

         int index;
         int length = value.Length;
         int maxSearchLength = (sb.Length - length) + 1;

         if (ignoreCase)
         {
            for (int i = startIndex; i < maxSearchLength; ++i)
            {
               if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
               {
                  index = 1;
                  while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                  {
                     ++index;
                  }

                  if (index == length)
                  {
                     return i;
                  }
               }
            }

            return -1;
         }

         for (int i = startIndex; i < maxSearchLength; ++i)
         {
            if (sb[i] == value[0])
            {
               index = 1;
               while ((index < length) && (sb[i + index] == value[index]))
               {
                  ++index;
               }

               if (index == length)
               {
                  return i;
               }
            }
         }

         return -1;
      }

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <returns>A string that is equivalent to the substring of length length that begins at startIndex in this instance, or System.String.Empty if startIndex is equal to the length of this instance and length is zero.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      public static string Substring(this StringBuilder sb, int startIndex, int length)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
         if (length < 0) throw new ArgumentOutOfRangeException("length");
         if (startIndex + length > sb.Length) throw new ArgumentOutOfRangeException("length");

         var temp = new char[length];
         sb.CopyTo(startIndex, temp, 0, length);
         return new string(temp);
      }

      ///// <summary>
      ///// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      ///// </summary>
      ///// <param name="sb">The System.Text.StringBuilder source instance.</param>
      ///// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      ///// <param name="length">The number of characters in the substring.</param>
      ///// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      ///// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      //public static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, int length)
      //{
      //   return SubstringBuilder(sb, startIndex, length, null);
      //}

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="dest">The System.Text.StringBuilder destination instance. If null a new StringBuilder will be returned.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      public static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, StringBuilder dest, int length)
      {
         return SubstringBuilder(sb, startIndex, dest, length, false);
      }

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <param name="trimOnly">if true trim the existing StringBuilder instance. -or- if false create a new StringBuilder instance and leave the existing instance in tact.</param>
      /// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      public static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, int length, bool trimOnly)
      {
         return SubstringBuilder(sb, startIndex, null, length, trimOnly);
      }

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="dest">The System.Text.StringBuilder destination instance. If null a new StringBuilder will be returned.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <param name="trimOnly">if true trim the existing StringBuilder instance. -or- if false create a new StringBuilder instance and leave the existing instance in tact.</param>
      /// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      private static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, StringBuilder dest, int length, bool trimOnly)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
         if (length < 0) throw new ArgumentOutOfRangeException("length");
         if (startIndex + length > sb.Length) throw new ArgumentOutOfRangeException("length");

         if (trimOnly)
         {
            sb.Remove(0, startIndex);
            sb.Remove(length, sb.Length - length);
            return sb;
         }

         var result = dest ?? new StringBuilder();
         sb.CopyTo(startIndex, result, length);
         return result;
      }

      public static void CopyTo(this StringBuilder sb, StringBuilder dest)
      {
         CopyTo(sb, 0, dest, sb.Length);
      }

      private static void CopyTo(this StringBuilder sb, int startIndex, StringBuilder dest, int length)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
         if (dest == null) throw new ArgumentNullException("dest");
         if (length < 0) throw new ArgumentOutOfRangeException("length");
         if (startIndex + length > sb.Length) throw new ArgumentOutOfRangeException("length");

         dest.Clear();
         dest.EnsureCapacity(length);
         for (int i = startIndex; i < (startIndex + length); i++)
         {
            dest.Append(sb[i]);
         }
      }

      private const int MaxChunkSize = 8000;

      public static IEnumerable<char[]> GetChunks(this StringBuilder sb)
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

      //public static StringBuilder MergeChunks(this IEnumerable<char[]> chunks)
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

      public static IEnumerable<string> Split(this StringBuilder sb, char splitChar)
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

      public static bool EndsWith(this StringBuilder sb, char value)
      {
         if (sb == null) throw new ArgumentNullException("sb");

         if (sb.Length > 0)
         {
            return sb[sb.Length - 1].Equals(value);
         }

         return false;
      }

      //public static bool EndsWith(this StringBuilder sb, string value)
      //{
      //   if (sb == null) throw new ArgumentNullException("sb");
      //   if (value == null) throw new ArgumentNullException("value");
      //
      //   if (value.Length <= sb.Length)
      //   {
      //      var temp = new char[value.Length];
      //      sb.CopyTo(sb.Length - value.Length, temp, 0, value.Length);
      //      return new string(temp).Equals(value);
      //   }
      //
      //   return false;
      //}
   }
}
