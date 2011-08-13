/*
 * HFM.NET - Log Fragment Data Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

namespace HFM.Client.DataTypes
{
   public class LogRestart : LogFragment
   {
      // type is simply for mapping to a JsonMessageKey
   }

   public class LogUpdate : LogFragment
   {
      // type is simply for mapping to a JsonMessageKey
   }

   public abstract class LogFragment : TypedMessage
   {
      /// <summary>
      /// Log Fragment Value
      /// </summary>
      public string Value { get; set; }

      /// <summary>
      /// Fill the LogFragment object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill(JsonMessage message)
      {
         Debug.Assert(message != null);
         int startIndex = GetStartIndex(message.Value);
         int length = (message.Value.EndsWith("\"") ? message.Value.Length - 1 : message.Value.Length) - startIndex;
         Value = SetEnvironmentNewLineCharacters(message.Value.Substring(startIndex, length));
      }

      private static int GetStartIndex(string value)
      {
         int startIndex = value.IndexOf("\r\n\"");
         if (startIndex >= 0)
         {
            startIndex += 3;
         }
         else
         {
            startIndex = value.IndexOf("\n\"");
            if (startIndex >= 0)
            {
               startIndex += 2;
            }
            else
            {
               startIndex = 0;
            }
         }
         return startIndex;
      }

      private static string SetEnvironmentNewLineCharacters(string value)
      {
         value = value.Replace("\n", Environment.NewLine);
         return value.Replace("\\n", Environment.NewLine);
      }
   }
}
