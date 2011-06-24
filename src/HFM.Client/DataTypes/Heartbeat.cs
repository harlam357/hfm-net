/*
 * HFM.NET - Heartbeat Data Class
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

namespace HFM.Client.DataTypes
{
   public class Heartbeat : TypedMessage
   {
      private Heartbeat()
      {
         
      }

      public int Value { get; set; }

      /// <summary>
      /// Create a Heartbeat object from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      /// <exception cref="ArgumentNullException">Throws if message parameter is null.</exception>
      public static Heartbeat Parse(JsonMessage message)
      {
         if (message == null) throw new ArgumentNullException("message");

         var heartbeat = new Heartbeat();
         heartbeat.Value = Int32.Parse(message.Value);
         return heartbeat;
      }
   }
}
