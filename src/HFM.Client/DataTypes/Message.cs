/*
 * HFM.NET - Message Data Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Globalization;

namespace HFM.Client.DataTypes
{
   /// <summary>
   /// Provides the base functionality for creating Folding@Home client messages.
   /// </summary>
   public abstract class Message
   {
      /// <summary>
      /// Message key.
      /// </summary>
      public string Key { get; internal set; }

      /// <summary>
      /// Received time stamp.
      /// </summary>
      public DateTime Received { get; internal set; }

      internal void SetMessageValues(Message message)
      {
         Key = message.Key;
         Received = message.Received;
      }

      /// <summary>
      /// Gets a formatted string that represents the metadata of the message.
      /// </summary>
      /// <returns>A formatted string that represents the metadata of the message.</returns>
      public virtual string GetHeader()
      {
         return String.Format(CultureInfo.CurrentCulture, "Message Key: {0} - Received at: {1}", Key, Received);
      }

      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         return GetHeader();
      }
   }
}
