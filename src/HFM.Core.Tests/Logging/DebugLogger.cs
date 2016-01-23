
using System;

using Castle.Core.Logging;

namespace HFM.Core.Tests.Logging
{
   public class DebugLogger : LevelFilteredLogger
   {
      public DebugLogger()
      {
         Level = LoggerLevel.Debug;
      }

      public override ILogger CreateChildLogger(string loggerName)
      {
         throw new NotImplementedException();
      }

      protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
      {
         System.Diagnostics.Debug.WriteLine("[{0}] {1}", loggerLevel, message);
         if (exception == null)
            return;
         System.Diagnostics.Debug.WriteLine("[{0}] {1}: {2} {3}", loggerLevel, exception.GetType().FullName, exception.Message, exception.StackTrace);
      }
   }
}
