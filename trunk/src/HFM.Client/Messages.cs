
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HFM.Client
{
   public class Messages : Connection
   {
      public event EventHandler MessagesUpdated;

      private readonly StringBuilder _readBuffer;
      private readonly Dictionary<string, IMessage> _messages;

      public Messages()
      {
         _readBuffer = new StringBuilder();
         _messages = new Dictionary<string, IMessage>();
      }

      protected override void Update()
      {
         base.Update();

         _readBuffer.Append(GetBuffer(true));
         string bufferValue = _readBuffer.ToString();
         //Console.WriteLine("Buffer Value: " + bufferValue);

         bool messagesUpdated = false;
         Message json;
         while ((json = GetNextMessageValue(ref bufferValue)) != null)
         {
            ProcessMessage(json);
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

      private void ProcessMessage(Message message)
      {
         switch (message.Name)
         {
            // log text will need the platform specific new line character(s) set
            // i.e. message.Value.Replace("\\" + "n", Environment.NewLine);

            case MessageKey.LogRestart:
               _messages[MessageKey.Log] = new Message { Name = MessageKey.Log, Value = message.Value, Received = DateTime.UtcNow };
               Debug.WriteLine("received " + MessageKey.LogRestart);
               break;
            case MessageKey.LogUpdate:
               if (_messages.ContainsKey(MessageKey.Log))
               {
                  _messages[MessageKey.Log] = new Message { Name = MessageKey.Log, Value = _messages[MessageKey.Log] + message.Value, Received = DateTime.UtcNow };
               }
               else
               {
                  _messages[MessageKey.Log] = new Message { Name = MessageKey.Log, Value = message.Value, Received = DateTime.UtcNow };
               }
               Debug.WriteLine("received " + MessageKey.LogUpdate);
               break;
            default:
               _messages[message.Name] = message;
               Debug.WriteLine("received " + message.Name);
               break;
         }
      }

      private static Message GetNextMessageValue(ref string buffer)
      {
         Debug.Assert(buffer != null);

         // find the header
         int messageIndex = buffer.IndexOf("PyON ");
         if (messageIndex < 0)
         {
            return null;
         }
         // "PyON " plus version number and another space - i.e. "PyON 1 "
         messageIndex += 7;

         int startIndex = buffer.IndexOf('\n', messageIndex);
         if (startIndex < 0) return null;

         // find the footer
         int endIndex = buffer.IndexOf("---\n", startIndex);
         if (endIndex < 0) return null;

         var message = new Message { Received = DateTime.UtcNow };
         // get the PyON message name
         message.Name = buffer.Substring(messageIndex, startIndex - messageIndex);

         // get the PyON message
         string pyon = buffer.Substring(startIndex, endIndex - startIndex);
         // replace PyON values with JSON values
         message.Value = pyon.Replace("[\n", String.Empty).Replace("]\n", String.Empty).Replace(": None", ": null");

         // set the index so we know where to trim the string
         int nextStartIndex = endIndex + 4;
         buffer = nextStartIndex < buffer.Length ? buffer.Substring(nextStartIndex) : String.Empty;
         return message;
      }
   }

   public interface IMessage
   {
      string Name { get; }

      string Value { get; }

      DateTime Received { get; }
   }

   public class Message : IMessage
   {
      public string Name { get; set; }

      public string Value { get; set; }

      public DateTime Received { get; set; }
   }
}
