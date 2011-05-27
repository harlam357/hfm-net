
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HFM.Client
{
   public class Messages : Connection
   {
      #region Events

      /// <summary>
      /// Fired when a new or updated message is received.
      /// </summary>
      public event EventHandler MessagesUpdated;

      #endregion

      #region Fields

      private readonly StringBuilder _readBuffer;
      private readonly Dictionary<string, IMessage> _messages;

      #endregion

      #region Constructor

      /// <summary>
      /// Create a Server Message Handler.
      /// </summary>
      public Messages()
      {
         _readBuffer = new StringBuilder();
         _messages = new Dictionary<string, IMessage>();
      }

      #endregion

      #region Methods

      /// <summary>
      /// Get a Server Message.  Returns null if the message is not in the cache.
      /// </summary>
      /// <param name="key">Server Message Key</param>
      /// <returns>The server message or null if the message is not in the cache.</returns>
      public IMessage GetMessage(string key)
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
         Message json;
         while ((json = GetNextMessage(ref bufferValue)) != null)
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
      private static Message GetNextMessage(ref string buffer)
      {
         Debug.Assert(buffer != null);

         const string pyonHeader = "PyON 1 ";
         const char lineFeed = '\n';
         const string pyonFooter = "---\n";

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
         int endIndex = buffer.IndexOf(pyonFooter, startIndex);
         if (endIndex < 0) return null;

         // create the message and set received time stamp
         var message = new Message { Received = DateTime.UtcNow };
         // get the message name
         message.Key = buffer.Substring(messageIndex, startIndex - messageIndex);

         // get the PyON message
         string pyon = buffer.Substring(startIndex, endIndex - startIndex);
         // replace PyON values with JSON values
         //message.Value = pyon.Replace("[\n", String.Empty).Replace("]\n", String.Empty).Replace(": None", ": null");
         message.Value = pyon.Replace(": None", ": null");

         // set the index so we know where to trim the string (end plus footer length)
         int nextStartIndex = endIndex + pyonFooter.Length;
         // if more buffer is available set it and return, otherwise set the buffer empty
         buffer = nextStartIndex < buffer.Length ? buffer.Substring(nextStartIndex) : String.Empty;

         return message;
      }

      private void UpdateMessageCache(IMessage message)
      {
         switch (message.Key)
         {
            // log text will need the platform specific new line character(s) set
            // i.e. message.Value.Replace("\\" + "n", Environment.NewLine);

            case MessageKey.LogRestart:
               _messages[MessageKey.Log] = new Message { Key = MessageKey.Log, Value = message.Value, Received = DateTime.UtcNow };
               Debug.WriteLine("received " + MessageKey.LogRestart);
               break;
            case MessageKey.LogUpdate:
               if (_messages.ContainsKey(MessageKey.Log))
               {
                  _messages[MessageKey.Log] = new Message { Key = MessageKey.Log, Value = _messages[MessageKey.Log] + message.Value, Received = DateTime.UtcNow };
               }
               else
               {
                  _messages[MessageKey.Log] = new Message { Key = MessageKey.Log, Value = message.Value, Received = DateTime.UtcNow };
               }
               Debug.WriteLine("received " + MessageKey.LogUpdate);
               break;
            default:
               _messages[message.Key] = message;
               Debug.WriteLine("received " + message.Key);
               break;
         }
      }

      #endregion
   }

   public interface IMessage
   {
      /// <summary>
      /// Message Key
      /// </summary>
      string Key { get; }

      /// <summary>
      /// Message Value
      /// </summary>
      string Value { get; }

      /// <summary>
      /// Received Time Stamp
      /// </summary>
      DateTime Received { get; }
   }

   public class Message : IMessage
   {
      /// <summary>
      /// Message Key
      /// </summary>
      public string Key { get; set; }

      /// <summary>
      /// Message Value
      /// </summary>
      public string Value { get; set; }

      /// <summary>
      /// Received Time Stamp
      /// </summary>
      public DateTime Received { get; set; }
   }
}
