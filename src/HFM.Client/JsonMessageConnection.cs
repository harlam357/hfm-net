/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.Text;

using HFM.Client.DataTypes;

namespace HFM.Client
{
   /// <summary>
   /// Folding@Home client JSON message connection class.  Provides functionality for parsing raw network data into JSON messages.
   /// </summary>
   public class JsonMessageConnection : Connection, IMessageConnection
   {
      #region Events

      /// <summary>
      /// Occurs when a message is received.
      /// </summary>
      public event EventHandler<MessageReceivedEventArgs> MessageReceived;
      /// <summary>
      /// Occurs when the local data buffer update is finished.
      /// </summary>
      public event EventHandler UpdateFinished;
      
      #endregion

      #region Constants

      private const string LineFeed = "\n";
      private const string CarriageReturnLineFeed = "\r\n";
      private const string PyonHeader = "PyON 1 ";
      private const string PyonFooter = "---";

      #endregion

      #region Fields

      private readonly StringBuilder _readBuffer = new StringBuilder();

      #endregion

      #region Constructor

      /// <summary>
      /// Initializes a new instance of the MessageCache class.
      /// </summary>
      [ExcludeFromCodeCoverage]
      public JsonMessageConnection()
      {

      }

      #endregion

      #region Methods

      /// <summary>
      /// Update the local data buffer with data from the remote network stream.
      /// </summary>
      protected internal override void ProcessData(string buffer, int totalBytesRead)
      {
         base.ProcessData(buffer, totalBytesRead);

         _readBuffer.Append(buffer);

         JsonMessage json;
         while ((json = GetNextJsonMessage(_readBuffer)) != null)
         {
            OnStatusMessage(new StatusMessageEventArgs(String.Format(CultureInfo.CurrentCulture,
               "Received message: {0} ({1} bytes)", json.Key, json.Value.Length), TraceLevel.Info));
            OnMessageReceived(new MessageReceivedEventArgs(json));
         }
         // send update finished event
         OnUpdateFinished(EventArgs.Empty);
      }

      /// <summary>
      /// Parse first message from the data buffer and remove it from the buffer value.  The remaining buffer value is returned to the caller.
      /// </summary>
      /// <param name="buffer">Data buffer value.</param>
      /// <returns>JsonMessage or null if no message is available in the buffer.</returns>
      internal static JsonMessage GetNextJsonMessage(ref string buffer)
      {
         var sb = new StringBuilder(buffer);
         var message = GetNextJsonMessage(sb);
         buffer = sb.ToString();
         return message;
      }

      /// <summary>
      /// Parse first message from the data buffer and remove it from the buffer value.  The remaining buffer value is returned to the caller.
      /// </summary>
      /// <param name="buffer">Data buffer value.</param>
      /// <returns>JsonMessage or null if no message is available in the buffer.</returns>
      internal static JsonMessage GetNextJsonMessage(StringBuilder buffer)
      {
         if (buffer == null) return null;

         // find the header
         int messageIndex = buffer.IndexOf(PyonHeader, false);
         if (messageIndex < 0)
         {
            return null;
         }
         // set starting message index
         messageIndex += PyonHeader.Length;

         // find the first CrLf or Lf character after the header
         int startIndex = FindStartIndex(buffer, messageIndex);
         if (startIndex < 0) return null;

         // find the footer
         int endIndex = FindEndIndex(buffer, startIndex);
         if (endIndex < 0)
         {
            return null;
         }

         // create the message and set received time stamp
         var message = new JsonMessage { Received = DateTime.UtcNow };
         // get the message name
         message.Key = buffer.Substring(messageIndex, startIndex - messageIndex);

         // get the PyON message
         buffer.SubstringBuilder(startIndex, message.Value, endIndex - startIndex);
         // replace PyON values with JSON values
         message.Value.Replace(": None", ": null");
         message.Value.Replace(": True", ": true");
         message.Value.Replace(": False", ": false");

         // set the index so we know where to trim the string (end plus footer length)
         int nextStartIndex = endIndex + PyonFooter.Length;
         // if more buffer is available set it and return, otherwise set the buffer empty
         if (nextStartIndex < buffer.Length)
         {
            buffer.Remove(0, nextStartIndex);
         }
         else
         {
            buffer.Clear();
         }

         return message;
      }

      private static int FindStartIndex(StringBuilder buffer, int messageIndex)
      {
         int index = buffer.IndexOf(CarriageReturnLineFeed, messageIndex, false);
         return index >= 0 ? index : buffer.IndexOf(LineFeed, messageIndex, false);
      }

      private static int FindEndIndex(StringBuilder buffer, int startIndex)
      {
         int index = buffer.IndexOf(String.Concat(CarriageReturnLineFeed, PyonFooter, CarriageReturnLineFeed), startIndex, false);
         if (index >= 0)
         {
            return index;
         }

         index = buffer.IndexOf(String.Concat(LineFeed, PyonFooter, LineFeed), startIndex, false);
         if (index >= 0)
         {
            return index;
         }

         //index = buffer.IndexOf(PyonFooter, startIndex, StringComparison.Ordinal);
         //if (index >= 0)
         //{
         //   return index;
         //}

         return -1;
      }

      /// <summary>
      /// Raise the MessageUpdated event.
      /// </summary>
      /// <param name="e">Event arguments (if null the event is cancelled).</param>
      protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
      {
         if (e == null) return;

         var handler = MessageReceived;
         if (handler != null)
         {
            handler(this, e);
         }
      }

      private void OnUpdateFinished(EventArgs e)
      {
         var handler = UpdateFinished;
         if (handler != null)
         {
            handler(this, e);
         }
      }

      #endregion
   }

   /// <summary>
   /// Provides data for message received events.
   /// </summary>
   public class MessageReceivedEventArgs : EventArgs
   {
      /// <summary>
      /// Gets the JSON message that was received.
      /// </summary>
      public JsonMessage JsonMessage { get; private set; }

      /// <summary>
      /// Gets the typed message that was received (FahClient connections only).
      /// </summary>
      public TypedMessage TypedMessage { get; internal set; }

      /// <summary>
      /// Gets the data type of the typed message that was received (FahClient connections only).
      /// </summary>
      public Type DataType
      {
         get { return TypedMessage != null ? TypedMessage.GetType() : null; }
      }

      /// <summary>
      /// Initializes a new instance of the MessageUpdatedEventArgs class.
      /// </summary>
      /// <param name="jsonMessage">The JSON message.</param>
      public MessageReceivedEventArgs(JsonMessage jsonMessage)
      {
         JsonMessage = jsonMessage;
      }
   }
}
