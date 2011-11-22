
using System;

using Castle.Core.Logging;

namespace HFM.Core.Logging
{
   [CoverageExclude]
   public class Logger : TraceLogger
   {
      public Logger(string name)
         : base(name)
      {
         
      }

      public Logger(string name, LoggerLevel level)
         : base(name, level)
      {
         
      }

      protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
      {
         if (loggerLevel >= Level)
         {
            OnTextMessage(new TextMessageEventArgs(message));
         }

         base.Log(loggerLevel, loggerName, message, exception);
      }

      #region Text Message Event

      public event EventHandler<TextMessageEventArgs> TextMessage;

      private void OnTextMessage(TextMessageEventArgs e)
      {
         if (TextMessage != null)
         {
            TextMessage(this, e);
         }
      }

      #endregion
   }

   [CoverageExclude]
   public class TextMessageEventArgs : EventArgs
   {
      private readonly string _message;

      public string Message
      {
         get { return _message; }
      }

      public TextMessageEventArgs(string message)
      {
         _message = message;
      }
   }
}
