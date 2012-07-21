/*
 * HFM.NET - Log Fragment Data Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

using HFM.Core.DataTypes;

namespace HFM.Client.DataTypes
{
   /// <summary>
   /// Folding@Home log restart message.
   /// </summary>
   public class LogRestart : LogFragment
   {
      // type is simply for mapping to a JsonMessageKey
   }

   /// <summary>
   /// Folding@Home log update message.
   /// </summary>
   public class LogUpdate : LogFragment
   {
      // type is simply for mapping to a JsonMessageKey
   }

   /// <summary>
   /// Folding@Home client log fragment.
   /// </summary>
   public abstract class LogFragment : TypedMessage
   {
      protected LogFragment()
      {
         Value = new StringBuilder();
      }

      /// <summary>
      /// Log fragment value.
      /// </summary>
      public StringBuilder Value { get; private set; }

      /// <summary>
      /// Fill the LogFragment object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill(JsonMessage message)
      {
         Debug.Assert(message != null);
         message.Value.CopyTo(Value);
         int startIndex = GetStartIndex(Value);
         int length = (Value.EndsWith('\"') ? Value.Length - 1 : Value.Length) - startIndex;
         SetEnvironmentNewLineCharacters(Value.SubstringBuilder(startIndex, length, true));
         SetMessageValues(message);
      }

      private static int GetStartIndex(StringBuilder value)
      {
         int startIndex = value.IndexOf("\r\n\"", false);
         if (startIndex >= 0)
         {
            startIndex += 3;
         }
         else
         {
            startIndex = value.IndexOf("\n\"", false);
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

      private static void SetEnvironmentNewLineCharacters(StringBuilder value)
      {
         value.Replace("\n", Environment.NewLine);
         value.Replace("\\n", Environment.NewLine);
      }
   }
}
