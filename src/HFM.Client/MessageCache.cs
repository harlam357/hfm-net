/*
 * HFM.NET - MessageCache Class
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using HFM.Client.DataTypes;

namespace HFM.Client
{
   public class MessageCache : Connection
   {
      #region Events

      /// <summary>
      /// Fired when a new or updated message is received.
      /// </summary>
      public event EventHandler MessagesUpdated;

      #endregion

      #region Fields

      private readonly StringBuilder _readBuffer;
      private readonly Dictionary<string, JsonMessage> _messages;

      #endregion

      #region Constructor

      /// <summary>
      /// Create a Server Message Handler.
      /// </summary>
      public MessageCache()
      {
         _readBuffer = new StringBuilder();
         _messages = new Dictionary<string, JsonMessage>();
      }

      #endregion

      #region Methods

      /// <summary>
      /// Get a Server Message.  Returns null if the message is not in the cache.
      /// </summary>
      /// <param name="key">Server Message Key</param>
      /// <returns>The server message or null if the message is not in the cache.</returns>
      public JsonMessage GetJsonMessage(string key)
      {
         return _messages.ContainsKey(key) ? _messages[key] : null;
      }

      /// <summary>
      /// Update the Data Buffer.
      /// </summary>
      protected override void Update()
      {
         // first, update the connection's buffer
         base.Update();

         // get the connection buffer and clear the connection buffer
         _readBuffer.Append(GetBuffer());
         string bufferValue = _readBuffer.ToString();

         bool messagesUpdated = false;
         JsonMessage json;
         while ((json = GetNextJsonMessage(ref bufferValue)) != null)
         {
            UpdateMessageCache(json);
            messagesUpdated = true;
         }
         _readBuffer.Clear();
         _readBuffer.Append(bufferValue);

         if (messagesUpdated) OnMessagesUpdated(EventArgs.Empty);
      }

      protected virtual void OnMessagesUpdated(EventArgs e)
      {
         if (MessagesUpdated != null)
         {
            MessagesUpdated(this, e);
         }
      }

      /// <summary>
      /// Parse first Message from the incoming data buffer.  Remaining buffer value is returned to the caller.
      /// </summary>
      /// <param name="buffer">Data Buffer Value</param>
      /// <returns>Message or null if no message is available in the buffer.</returns>
      public static JsonMessage GetNextJsonMessage(ref string buffer)
      {
         //TODO: Should be internal exposure

         Debug.Assert(buffer != null);

         const string pyonHeader = "PyON 1 ";
         const char lineFeed = '\n';
         const string pyonFooter1 = "---\n";
         const string pyonFooter2 = "---";

         // find the header
         int messageIndex = buffer.IndexOf(pyonHeader);
         if (messageIndex < 0)
         {
            return null;
         }
         // set starting message index
         messageIndex += pyonHeader.Length;

         // find the first line feed character after the header
         int startIndex = buffer.IndexOf(lineFeed, messageIndex);
         if (startIndex < 0) return null;

         // find the footer
         int endIndex = buffer.IndexOf(pyonFooter1, startIndex);
         if (endIndex < 0)
         {
            endIndex = buffer.IndexOf(pyonFooter2, startIndex);
            if (endIndex < 0)
            {
               return null;
            }
         }

         // create the message and set received time stamp
         var message = new JsonMessage { Received = DateTime.UtcNow };
         // get the message name
         message.Key = buffer.Substring(messageIndex, startIndex - messageIndex);

         // get the PyON message
         string pyon = buffer.Substring(startIndex, endIndex - startIndex);
         // replace PyON values with JSON values
         //message.Value = pyon.Replace("[\n", String.Empty).Replace("]\n", String.Empty).Replace(": None", ": null");
         message.Value = pyon.Replace(": None", ": null");

         // set the index so we know where to trim the string (end plus footer length)
         int nextStartIndex = endIndex + pyonFooter1.Length;
         // if more buffer is available set it and return, otherwise set the buffer empty
         buffer = nextStartIndex < buffer.Length ? buffer.Substring(nextStartIndex) : String.Empty;

         return message;
      }

      private void UpdateMessageCache(JsonMessage message)
      {
         switch (message.Key)
         {
            // log text will need the platform specific new line character(s) set
            // i.e. message.Value.Replace("\\" + "n", Environment.NewLine);

            case JsonMessageKey.LogRestart:
               _messages[JsonMessageKey.Log] = new JsonMessage { Key = JsonMessageKey.Log, Value = message.Value, Received = DateTime.UtcNow };
               Debug.WriteLine("received " + JsonMessageKey.LogRestart);
               break;
            case JsonMessageKey.LogUpdate:
               if (_messages.ContainsKey(JsonMessageKey.Log))
               {
                  _messages[JsonMessageKey.Log] = new JsonMessage { Key = JsonMessageKey.Log, Value = _messages[JsonMessageKey.Log] + message.Value, Received = DateTime.UtcNow };
               }
               else
               {
                  _messages[JsonMessageKey.Log] = new JsonMessage { Key = JsonMessageKey.Log, Value = message.Value, Received = DateTime.UtcNow };
               }
               Debug.WriteLine("received " + JsonMessageKey.LogUpdate);
               break;
            default:
               _messages[message.Key] = message;
               Debug.WriteLine("received " + message.Key);
               break;
         }
      }

      #endregion
   }

   public static class JsonMessageKey
   {
      /// <summary>
      /// Heartbeat Message Key
      /// </summary>
      public const string Heartbeat = "heartbeat";
      /// <summary>
      /// Log Restart Message Key
      /// </summary>
      internal const string LogRestart = "log-restart";
      /// <summary>
      /// Log Update Message Key
      /// </summary>
      internal const string LogUpdate = "log-update";
      /// <summary>
      /// Log Message Key (aggregated log text - this key is specific to the HFM.Client API)
      /// </summary>
      public const string Log = "log";
      /// <summary>
      /// Info Message Key
      /// </summary>
      public const string Info = "info";
      /// <summary>
      /// Options Message Key
      /// </summary>
      public const string Options = "options";
      /// <summary>
      /// Queue Info Message Key
      /// </summary>
      public const string QueueInfo = "units";
      /// <summary>
      /// Simulation Info Message Key
      /// </summary>
      /// <remarks>This message is in response to a command that takes a slot id argument.
      /// Will probably need to save messages for each slot requested in a dictionary.</remarks>
      public const string SimulationInfo = "simulation-info";
      /// <summary>
      /// Slot Info Message Key
      /// </summary>
      public const string SlotInfo = "slots";
      /// <summary>
      /// Slot Options Message Key
      /// </summary>
      /// <remarks>This message is in response to a command that takes a slot id argument.
      /// Will probably need to save messages for each slot requested in a dictionary.</remarks>
      public const string SlotOptions = "slot-options";
   }
}
