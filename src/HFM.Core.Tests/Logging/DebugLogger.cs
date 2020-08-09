using System;

namespace HFM.Core.Logging
{
    public class DebugLogger : LoggerBase
    {
        public static DebugLogger Instance { get; } = new DebugLogger();

        public DebugLogger() : base(LoggerLevel.Debug)
        {

        }

        protected override void Log(LoggerLevel loggerLevel, string message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine("[{0}] {1}", loggerLevel, message);
            if (exception is null)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine("[{0}] {1}: {2} {3}", loggerLevel, exception.GetType().FullName, exception.Message, exception.StackTrace);
        }
    }
}
